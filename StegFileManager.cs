using System;
using System.Collections.Generic;
using System.IO;

namespace FrankStore
{
    public class StegFileManager
    {
        private bool valid;
        private readonly FileType action;
        private readonly SteganographyManager.Action parentAction;

        //If Buffered Stream
        private bool fileRead;

        private FileStream file;

        //The encode/decode files are read/written in binary.
        private BinaryWriter bWriter;
        private BinaryReader bReader;

        //The file to encode / the output is read/written as a byte stream.
        private BufferedStream bStream;

        public enum FileType
        {
            ReadFile,
            WriteFile
        };

        public StegFileManager(SteganographyManager parent, FileType type, string location)
        {
            parentAction = parent.getAction();
            action = type;

            switch (parentAction)
            {
                case SteganographyManager.Action.Encoding when action == FileType.ReadFile:
                {
                    Console.WriteLine("Opening file to encode.");
                    checkValid(location, false);
                    if (isValid())
                    {
                        openFile(location);
                        openBufferedStream();
                    }

                    break;
                }
                case SteganographyManager.Action.Decoding when action == FileType.ReadFile:
                {
                    Console.WriteLine("Opening file to decode.");
                    checkValid(location, false);
                    if (isValid())
                    {
                        openFile(location);
                        openBinaryReader();
                    }

                    break;
                }
                case SteganographyManager.Action.Encoding when action == FileType.WriteFile:
                {
                    Console.WriteLine("Creating file to write to.");
                    checkValid(location, true);
                    if (isValid())
                    {
                        createFile(location);
                        openBinaryWriter();
                    }

                    break;
                }
                case SteganographyManager.Action.Decoding when action == FileType.WriteFile:
                {
                    Console.WriteLine("Creating file to write to.");
                    checkValid(location, true);
                    if (isValid())
                    {
                        createFile(location);
                        openBufferedStream();
                    }

                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public bool isValid()
        {
            return valid;
        }

        public void close()
        {
            switch (parentAction)
            {
                case SteganographyManager.Action.Encoding when action == FileType.ReadFile:
                    bStream.Close();
                    bStream = null;
                    break;
                case SteganographyManager.Action.Decoding when action == FileType.ReadFile:
                    bReader.Close();
                    bReader = null;
                    break;
                case SteganographyManager.Action.Encoding when action == FileType.WriteFile:
                    bWriter.Close();
                    bWriter = null;
                    break;
                case SteganographyManager.Action.Decoding when action == FileType.WriteFile:
                    bStream.Close();
                    bStream = null;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            file.Close();
            file = null;
        }

        public byte[] getBytes()
        {
            if (action == FileType.WriteFile)
            {
                throw new ArgumentException(
                    "You cannot read from a file opened in write mode. (or rather, you shouldn't be.)");
            }

            if (parentAction != SteganographyManager.Action.Encoding)
            {
                throw new ArgumentException("You cannot retrieve bytes on a binary reader!");
            }

            var receivedData = new byte[4096];

            var numBytesToRead = receivedData.Length;

            var count = 0;

            while (numBytesToRead > 0)
            {
                // Read may return anything from 0 to numBytesToRead.
                var n = bStream.Read(receivedData, 0, receivedData.Length);
                // The end of the file is reached.
                if (n == 0)
                {
                    fileRead = true;
                    break;
                }

                numBytesToRead -= n;
                count += n;
            }

            var actualData = new byte[count];
            Array.Copy(receivedData, actualData, actualData.Length);

            return actualData;
        }

        public bool isFileRead()
        {
            return fileRead;
        }

        public void writeToFile(IEnumerable<Location> locations)
        {
            if (action == FileType.ReadFile)
            {
                throw new ArgumentException("You cannot write to a file opened in read mode.");
            }

            if (parentAction == SteganographyManager.Action.Decoding)
            {
                throw new ArgumentException("You cannot write to a file opened in binary stream mode.");
            }

            foreach (var l in locations)
            {
                writeToFile(l.getX());
                writeToFile(l.getY());
                writeToFile(l.getHashLocation());
            }

            bWriter.Flush();
        }

        public void writeToFile(byte[] bytes)
        {
            if (action == FileType.ReadFile)
            {
                throw new ArgumentException("You cannot write to a file opened in read mode.");
            }

            if (parentAction == SteganographyManager.Action.Encoding)
            {
                throw new ArgumentException("You cannot write binary to a file opened in byte writing mode.");
            }

            bStream.Write(bytes, 0, bytes.Length);
        }

        public IEnumerable<Location> getLocations()
        {
            if (action == FileType.WriteFile)
            {
                throw new ArgumentException(
                    "You cannot read from a file opened in write mode. (or rather, you shouldn't be.)");
            }

            if (parentAction != SteganographyManager.Action.Decoding)
            {
                throw new ArgumentException("You cannot retrieve binary from a byte reader!");
            }

            var list = new List<Location>();

            try
            {
                for (var i = 0; i < 4096; i++)
                {
                    if ((bReader.BaseStream.Length - bReader.BaseStream.Position) < 6)
                    {
                        fileRead = true;
                        break;
                    }

                    //x,y,hashlocation
                    var test = new ushort[3];

                    for (var x = 0; x < 3; x++)
                    {
                        test[x] = bReader.ReadUInt16();
                    }

                    var loc = new Location(test[0], test[1], test[2]);

                    list.Add(loc);
                }
            }
            catch (Exception ex) when
            (
                ex is IOException
                || ex is ArgumentException
            )
            {
                Console.Error.WriteLine("[ERROR]: Problem reading from file.");
            }

            return list;
        }

        private void checkValid(string location, bool operationMode)
        {
            if (File.Exists(location) && operationMode)
            {
                Console.Error.WriteLine("[ERROR]: Output file already exists, please change the output name.");
                return;
            }

            if (!File.Exists(location) && !operationMode)
            {
                Console.Error.WriteLine("[ERROR]: Input file does not exist, please check.");
                return;
            }

            valid = true;
        }

        private void openFile(string location)
        {
            try
            {
                file = File.Open(location, FileMode.Open, FileAccess.Read);
            }
            catch (Exception ex) when
            (
                ex is ArgumentException
                || ex is PathTooLongException
            )
            {
                Console.Error.WriteLine("[ERROR]: Input filename is invalid, please change it.");
                valid = false;
                Environment.Exit(88);
            }
            catch (Exception ex) when
            (
                ex is IOException
                || ex is UnauthorizedAccessException
            )
            {
                Console.Error.WriteLine("[ERROR]: There is a problem reading the file, please check your permissions.");
                valid = false;
                Environment.Exit(87);
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
                Environment.Exit(86);
            }
            catch (DirectoryNotFoundException)
            {
                Console.Error.WriteLine("[ERROR]: The directory specified was not found.");
                valid = false;
                Environment.Exit(85);
            }
        }

        private void openBinaryReader()
        {
            bReader = new BinaryReader(file);
        }

        private void createFile(string location)
        {
            try
            {
                file = File.Create(location);
            }
            catch (Exception ex) when
            (
                ex is UnauthorizedAccessException
                || ex is IOException
            )
            {
                Console.Error.WriteLine("[ERROR]: You do not have permission to create a file here.");
                valid = false;
                Environment.Exit(89);
            }
            catch (Exception ex) when
            (
                ex is ArgumentException
                || ex is PathTooLongException
                || ex is DirectoryNotFoundException
                || ex is NotSupportedException
            )
            {
                Console.Error.WriteLine("[ERROR]: Invalid file name/path.");
                valid = false;
                Environment.Exit(90);
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

        private void writeToFile(ushort number)
        {
            try
            {
                bWriter.Write(number);
            }
            catch (ArgumentException)
            {
                Console.Error.WriteLine("[ERROR]: There is a problem with the argument entered.");
                Environment.Exit(93);
            }
            catch (Exception ex) when
            (
                ex is IOException
                || ex is ObjectDisposedException
            )
            {
                Console.Error.WriteLine("[ERROR]: There is a problem with the binary writer.");
                Environment.Exit(92);
            }
        }
    }
}