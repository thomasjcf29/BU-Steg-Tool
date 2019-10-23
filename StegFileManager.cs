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

    private FileStream file;
    private BinaryWriter bWriter;
    private BinaryReader bReader;
    private SteganographyManager parent;

    public enum FILE_TYPE {READ_FILE, WRITE_FILE};

    public StegFileManager(SteganographyManager parent, FILE_TYPE type, String location)
    {
        this.parent = parent;
        action = type;

        if(parent.getAction() == SteganographyManager.ACTION.ENCODING && action == FILE_TYPE.READ_FILE)
        {
            Console.WriteLine("Opening file to encode.");
            checkValid(location, false);
            if(isValid())
            {
                openFile(location);
                openBinaryReader();
            }
        }
        else if(parent.getAction() == SteganographyManager.ACTION.DECODING && action == FILE_TYPE.READ_FILE)
        {
            Console.WriteLine("Opening file to decode.");
            checkValid(location, false);
            if(isValid())
            {
                openFile(location);
                openBinaryReader();
            }   
        }
        else
        {
            Console.WriteLine("Creating file to write to.");
            checkValid(location, true);
            if(isValid())
            {
                createFile(location);
                openBinaryWriter();
            }  
        }
    }

    public Boolean isValid()
    {
        return valid;
    }

    public void close()
    {
        if(action == FILE_TYPE.READ_FILE)
        {
            bReader.Close();
            bReader = null;
        }
        else
        {
            bWriter.Close();
            bWriter = null;
        }

        file.Close();
        file = null;
    }

    public void writeToFile(List<Location> locations)
    {
        Console.WriteLine("Writing to file");

        if(action == FILE_TYPE.READ_FILE)
        {
            throw new ArgumentException("You cannot write to a file opened in read mode.");
        }

        foreach(Location l in locations)
        {
            writeToFile(l.getX());
            writeToFile(l.getY());
            writeToFile(l.getHashLocation());
        }
        Console.WriteLine("");
    }

    public void writeToFile(String hex)
    {
        Console.WriteLine("Writing to file");

        if(action == FILE_TYPE.READ_FILE)
        {
            throw new ArgumentException("You cannot write to a file opened in read mode.");
        }
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

    private void writeToFile(UInt16 number)
    {
        try {
            bWriter.Write(number);
            bWriter.Flush();
            Console.Write("+");
        }
        catch(Exception ex) when 
        (
            ex is ArgumentException
            || ex is ArgumentNullException
            || ex is ArgumentOutOfRangeException
        )
        {
            Console.Write("-");
            Console.WriteLine("");
            Console.Error.WriteLine("[ERROR]: There is a problem with the argument entered.");
            System.Environment.Exit(93);
        }
        catch(Exception ex) when
        (
            ex is IOException
            || ex is ObjectDisposedException
        )
        {
            Console.Write("-");
            Console.WriteLine("");
            Console.Error.WriteLine("[ERROR]: There is a problem with the binary writer.");
            System.Environment.Exit(92);
        }
    }
}