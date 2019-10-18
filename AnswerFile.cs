using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;

public class AnswerFile
{
    private Boolean valid = false;
    private Boolean readOnly = false;

    private FileStream file;
    private BinaryWriter bWriter;
    private BinaryReader bReader;

    //operationMode = true/false (encoding/decoding)
    public AnswerFile(String location, Boolean operationMode)
    {
        checkValid(location, operationMode);

        if (isValid())
        {
            if (operationMode)
            {
                createFile(location);
                openBinaryWriter();
            }
            else
            {
                openFile(location);
                openBinaryReader();
            }
        }
    }

    public Boolean isValid()
    {
        return valid;
    }

    public Boolean writeToFile(UInt16 number)
    {
        try {
            bWriter.Write(number);
            bWriter.Flush();
            return true;
        }
        catch(Exception ex) when 
        (
            ex is ArgumentException
            || ex is ArgumentNullException
            || ex is ArgumentOutOfRangeException
        )
        {
            Console.Error.WriteLine("[ERROR]: There is a problem with the argument entered.");
            return false;
        }
        catch(Exception ex) when
        (
            ex is IOException
            || ex is ObjectDisposedException
        )
        {
            Console.Error.WriteLine("[ERROR]: There is a problem with the binary writer.");
            return false;
        }
    }

    public List<Location> readFromFile()
    {
        List<Location> list = new List<Location>();

        try
        {
            while (bReader.PeekChar() != -1)
            {
                //x,y,hashlocation
                UInt16[] test = new UInt16[3];

                for(int i = 0; i < 3; i++)
                {
                    test[i] = bReader.ReadUInt16();
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

    public void close()
    {
        if (readOnly)
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

    public Boolean isReadOnly()
    {
        return readOnly;
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
            Console.Error.WriteLine("[ERROR]: Decryption file does not exist, please check.");
            return;
        }

        valid = true;
    }

    private void openFile(String location)
    {
        try
        {
            file = File.Open(location, FileMode.Open, FileAccess.Read);
            readOnly = true;
        }
        catch(Exception ex) when
        (
            ex is ArgumentException
            || ex is ArgumentNullException
            || ex is PathTooLongException
        )
        {
            Console.Error.WriteLine("[ERROR]: Decryption filename is invalid, please change it.");
            valid = false;
        }
        catch(Exception ex) when
        (
            ex is IOException
            || ex is UnauthorizedAccessException
        )
        {
            Console.Error.WriteLine("[ERROR]: There is a problem reading the file, please check your permissions.");
            valid = false;
        }
        catch (Exception ex) when
        (
            ex is ArgumentOutOfRangeException
            || ex is FileNotFoundException
            || ex is NotSupportedException
        )
        {
            Console.Error.WriteLine("[ERROR]: The decryption file likely does not exist, please recheck it.");
            valid = false;
        }
        catch (DirectoryNotFoundException)
        {
            Console.Error.WriteLine("[ERROR]: The directory specified was not found.");
            valid = false;
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
        }
    }

    private void openBinaryWriter()
    {
        bWriter = new BinaryWriter(file);
    }
}