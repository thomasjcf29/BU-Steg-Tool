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

    public AnswerFile(String location, Boolean operationMode)
    {
        checkValid(location, operationMode);

        if (isValid())
        {
            if (operationMode)
            {
                createFile(location);
            }
            else
            {
                openFile(location);
            }
        }
    }

    public Boolean isValid()
    {
        return valid;
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
        catch(Exception ex) when (
            ex is ArgumentException
            || ex is ArgumentNullException
            || ex is PathTooLongException
        )
        {
            Console.Error.WriteLine("[ERROR]: Decryption filename is invalid, please change it.");
            valid = false;
        }
        catch(Exception ex) when (
            ex is IOException
            || ex is UnauthorizedAccessException
        )
        {
            Console.Error.WriteLine("[ERROR]: There is a problem reading the file, please check your permissions.");
            valid = false;
        }
        catch (Exception ex) when (
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

    private void createFile(String location)
    {
        try
        {
            file = File.Create(location);
        }
        catch(Exception ex) when (
            ex is UnauthorizedAccessException
            || ex is IOException
        )
        {
            Console.Error.WriteLine("[ERROR]: You do not have permission to create a file here.");
            valid = false;
        }
        catch(Exception ex) when (
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

    public void close()
    {
        file.Close();
        file = null;
    }

    public Boolean isReadOnly()
    {
        return readOnly;
    }
}