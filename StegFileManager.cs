using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;

public class StegFileManager
{
    private Boolean valid = false;
    private FILE_TYPE action;
    private SteganographyManager.ACTION parent_action;

    //If Buffered Stream
    private int totalRead = 0;
    private Boolean fileRead = false;

    private FileStream file;
    
    //The encode/decode files are read/written in binary.
    private BinaryWriter bWriter;
    private BinaryReader bReader;
    
    //The file to encode / the output is read/written as a byte stream.
    private BufferedStream bStream;

    private SteganographyManager parent;

    public enum FILE_TYPE {READ_FILE, WRITE_FILE};

    public StegFileManager(SteganographyManager parent, FILE_TYPE type, String location)
    {
        this.parent = parent;
        parent_action = parent.getAction();
        action = type;

        if(parent_action == SteganographyManager.ACTION.ENCODING && action == FILE_TYPE.READ_FILE)
        {
            Console.WriteLine("Opening file to encode.");
            checkValid(location, false);
            if(isValid())
            {
                openFile(location);
                openBufferedStream();
            }
        }
        else if(parent_action == SteganographyManager.ACTION.DECODING && action == FILE_TYPE.READ_FILE)
        {
            Console.WriteLine("Opening file to decode.");
            checkValid(location, false);
            if(isValid())
            {
                openFile(location);
                openBinaryReader();
            }   
        }
        else if(parent_action == SteganographyManager.ACTION.ENCODING && action == FILE_TYPE.WRITE_FILE)
        {
            Console.WriteLine("Creating file to write to.");
            checkValid(location, true);
            if(isValid())
            {
                createFile(location);
                openBinaryWriter();
            }  
        }
        else if(parent_action == SteganographyManager.ACTION.DECODING && action == FILE_TYPE.WRITE_FILE)
        {
            Console.WriteLine("Creating file to write to.");
            checkValid(location, true);
            if(isValid())
            {
                createFile(location);
                openBufferedStream();
            }  
        }
    }

    public Boolean isValid()
    {
        return valid;
    }

    public void close()
    {
        if(parent_action == SteganographyManager.ACTION.ENCODING && action == FILE_TYPE.READ_FILE)
        {
            bStream.Close();
            bStream = null;
        }
        else if(parent_action == SteganographyManager.ACTION.DECODING && action == FILE_TYPE.READ_FILE)
        {
            bReader.Close();
            bReader = null;
        }
        else if(parent_action == SteganographyManager.ACTION.ENCODING && action == FILE_TYPE.WRITE_FILE)
        {
            bWriter.Close();
            bWriter = null;
        }
        else if(parent_action == SteganographyManager.ACTION.DECODING && action == FILE_TYPE.WRITE_FILE)
        {
            bStream.Close();
            bStream = null;
        }

        file.Close();
        file = null;
    }

    public byte[] getBytes()
    {
        if(action == FILE_TYPE.WRITE_FILE)
        {
            throw new ArgumentException("You cannot read from a file opened in write mode. (or rather, you shouldn't be.)");
        }

        if(parent_action != SteganographyManager.ACTION.ENCODING)
        {
            throw new ArgumentException("You cannot retrieve bytes on a binary reader!");
        }

        byte[] receivedData = new byte[4096];
        byte[] actualData;

        int numBytesToRead = receivedData.Length;

        int count = 0;

        while (numBytesToRead > 0)
        {
            // Read may return anything from 0 to numBytesToRead.
            int n = bStream.Read(receivedData, 0, receivedData.Length);
            // The end of the file is reached.
            if (n == 0)
            {
                fileRead = true;
                break;
            }

            totalRead += n;
            numBytesToRead -= n;
            count += n;
        }

        actualData = new byte[count];
        Array.Copy(receivedData, actualData, actualData.Length);

        return actualData;
    }

    public Boolean isFileRead()
    {
        return fileRead;
    }

    public void writeToFile(List<Location> locations)
    {
        if(action == FILE_TYPE.READ_FILE)
        {
            throw new ArgumentException("You cannot write to a file opened in read mode.");
        }

        if(parent_action == SteganographyManager.ACTION.DECODING)
        {
            throw new ArgumentException("You cannot write to a file opened in binary stream mode.");
        }

        foreach(Location l in locations)
        {
            writeToFile(l.getX());
            writeToFile(l.getY());
            writeToFile(l.getHashLocation());
        }

        bWriter.Flush();
    }

    public void writeToFile(String hex)
    {
        Console.WriteLine("Writing to file");

        if(action == FILE_TYPE.READ_FILE)
        {
            throw new ArgumentException("You cannot write to a file opened in read mode.");
        }
    }

    public List<Location> getLocations()
    {
        if(action == FILE_TYPE.WRITE_FILE)
        {
            throw new ArgumentException("You cannot read from a file opened in write mode. (or rather, you shouldn't be.)");
        }

        if(parent_action != SteganographyManager.ACTION.DECODING)
        {
            throw new ArgumentException("You cannot retrieve binary from a byte reader!");
        }

        List<Location> list = new List<Location>();

        try
        {
            for(int i = 0; i < 1024; i++)
            {
                if((bReader.BaseStream.Length - bReader.BaseStream.Position) < 6)
                {
                    fileRead = true;
                    break;
                }

                //x,y,hashlocation
                UInt16[] test = new UInt16[3];

                for(int x = 0; x < 3; x++)
                {
                    test[x] = bReader.ReadUInt16();
                }

                Location loc = new Location(test[0], test[1], test[2]);

                list.Add(loc);
            }
        }
        catch(Exception ex) when
        (
            ex is IOException
            || ex is ArgumentException
        )
        {
            Console.Error.WriteLine("[ERROR]: Problem reading from file.");
        }

        return list;
    }

    private void checkValid(String location, Boolean operationMode)
    {
        if (File.Exists(location) && operationMode)
        {
            Console.Error.WriteLine("[ERROR]: Output file already exists, please change the output name.");
            return;
        }

        else if(!File.Exists(location) && !operationMode)
        {
            Console.Error.WriteLine("[ERROR]: Input file does not exist, please check.");
            return;
        }

        valid = true;
    }

    private void openFile(String location)
    {
        try
        {
            file = File.Open(location, FileMode.Open, FileAccess.Read);
        }
        catch(Exception ex) when
        (
            ex is ArgumentException
            || ex is ArgumentNullException
            || ex is PathTooLongException
        )
        {
            Console.Error.WriteLine("[ERROR]: Input filename is invalid, please change it.");
            valid = false;
            System.Environment.Exit(88);
        }
        catch(Exception ex) when
        (
            ex is IOException
            || ex is UnauthorizedAccessException
        )
        {
            Console.Error.WriteLine("[ERROR]: There is a problem reading the file, please check your permissions.");
            valid = false;
            System.Environment.Exit(87);
        }
        catch (Exception ex) when
        (
            ex is ArgumentOutOfRangeException
            || ex is FileNotFoundException
            || ex is NotSupportedException
        )
        {
            Console.Error.WriteLine("[ERROR]: The input file likely does not exist, please recheck it.");
            valid = false;
            System.Environment.Exit(86);
        }
        catch (DirectoryNotFoundException)
        {
            Console.Error.WriteLine("[ERROR]: The directory specified was not found.");
            valid = false;
            System.Environment.Exit(85);
        }
    }

    private void openBinaryReader()
    {
        bReader = new BinaryReader(file);
    }

    private void createFile(String location)
    {
        try
        {
            file = File.Create(location);
        }
        catch(Exception ex) when
        (
            ex is UnauthorizedAccessException
            || ex is IOException
        )
        {
            Console.Error.WriteLine("[ERROR]: You do not have permission to create a file here.");
            valid = false;
            System.Environment.Exit(89);
        }
        catch(Exception ex) when
        (
            ex is ArgumentException
            || ex is ArgumentNullException
            || ex is PathTooLongException
            || ex is DirectoryNotFoundException
            || ex is NotSupportedException
        )
        {
            Console.Error.WriteLine("[ERROR]: Invalid file name/path.");
            valid = false;
            System.Environment.Exit(90);
        }
    }

    private void openBinaryWriter()
    {
        bWriter = new BinaryWriter(file);
    }

    private void openBufferedStream()
    {
        bStream = new BufferedStream(file);
    }

    private void writeToFile(UInt16 number)
    {
        try {
            bWriter.Write(number);
        }
        catch(Exception ex) when 
        (
            ex is ArgumentException
            || ex is ArgumentNullException
            || ex is ArgumentOutOfRangeException
        )
        {
            Console.Error.WriteLine("[ERROR]: There is a problem with the argument entered.");
            System.Environment.Exit(93);
        }
        catch(Exception ex) when
        (
            ex is IOException
            || ex is ObjectDisposedException
        )
        {
            Console.Error.WriteLine("[ERROR]: There is a problem with the binary writer.");
            System.Environment.Exit(92);
        }
    }
}