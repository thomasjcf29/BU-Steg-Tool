using System;
using System.Collections.Generic;
using System.IO;

namespace FrankStore
{
    /// <summary>
    /// <c>StegFileManager</c> is incharge of reading from and writting to files.
    /// What it does is dependent on the variables which are passed in.
    /// </summary>
    public class StegFileManager
    {
        /// <summary>
        /// Boolean to if this class was successfully setup.
        /// If false, do not use this class!
        /// </summary>
        private bool valid;

        /// <summary>
        /// Enum of the whether this class is for reading or writing to the specified file.
        /// </summary>
        private readonly FileType action;

        /// <summary>
        /// Enum of the SteganographyManagers (parents) choice of if it is for encoding or decoding.
        /// </summary>
        private readonly SteganographyManager.Action parentAction;

        /// <summary>
        /// Determines that all of the file has been read.
        /// </summary>
        private bool fileRead;

        /// <summary>
        ///  The lower layer file stream for both reading and writing to files.
        /// </summary>
        private FileStream file;

        //The encode/decode files are read/written in binary.
        private BinaryWriter bWriter;
        private BinaryReader bReader;

        //The file to encode / the output is read/written as a byte stream.
        private BufferedStream bStream;

        /// <summary>
        /// FileType of this specific file, either ReadFile or WriteFile.
        /// </summary>
        public enum FileType
        {
            ReadFile,
            WriteFile
        };

        /// <summary>
        /// Constructor for the <c>StegFileManager</c> class, which will open/create the file and the relevant streamers.
        /// There are four possible different actions which can be taked.
        ///     1. Decoding Mode - Read File
        ///     2. Decoding Mode - Write File
        ///     3. Encoding Mode - Read File
        ///     4. Encoding Mode - Write File
        /// </summary>
        /// <param name="parent">The parent <c>SteganographyManager</c> class.</param>
        /// <param name="type">Type of file (read or write).</param>
        /// <param name="location">Location of the file to be read / written to.</param>
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

        /// <summary>
        /// Returns the state of the class and if it was successfully setup.
        /// Do not use the class if returned as false.
        /// </summary>
        /// <returns>True if successfully setup and false if not.</returns>
        public bool isValid()
        {
            return valid;
        }

        /// <summary>
        /// Closes the file and its relevant file streamer in use.
        /// </summary>
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

        /// <summary>
        /// Gets the next 4096 bytes of the file being read.
        /// Will strip out any trailing zeros from the end if it runs out of file to read.
        /// </summary>
        /// <returns>Byte array of all text read so far.</returns>
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

        /// <summary>
        /// Tells you if you have read to the end of the file or not.
        /// </summary>
        /// <returns>True if file is completely read or false if not.</returns>
        public bool isFileRead()
        {
            return fileRead;
        }

        /// <summary>
        /// Writes to file the binary version of the list of locations provided.
        /// For each location it writes the x, y, and hash location in a binary UINT16 format.
        /// </summary>
        /// <param name="locations">List of locations to be written.</param>
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

        /// <summary>
        /// Writes to file the bytes of converted hex from decoding.
        /// </summary>
        /// <param name="bytes">When decoding the hex converted to bytes.</param>
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

        /// <summary>
        /// Returns 4096 locations at a time, which can then be proceesed and actioned accordingly.
        /// For use during decoding.
        /// </summary>
        /// <returns>List of locations.</returns>
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

        /// <summary>
        /// Checks to make sure the file exists or doesn't exist depending on the operating mode.
        /// </summary>
        /// <param name="location">Location of the file to check.</param>
        /// <param name="operationMode">True = encoding, false = decoding.</param>
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

        /// <summary>
        /// Opens the file to be read from, no matter if it's for binary or byte stream.
        /// </summary>
        /// <param name="location">Location of the file to be opened.</param>
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

        /// <summary>
        /// Opens the binary reader to read from the file which has just been opened.
        /// </summary>
        private void openBinaryReader()
        {
            bReader = new BinaryReader(file);
        }

        /// <summary>
        /// Creates the file to be written to.
        /// </summary>
        /// <param name="location">Location of file to write to.</param>
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

        /// <summary>
        /// Opens the binary writer on top of the file to be written to.
        /// </summary>
        private void openBinaryWriter()
        {
            bWriter = new BinaryWriter(file);
        }

        /// <summary>
        /// Opens the buffered steam on top of the file which needs to be written or read to/from.
        /// </summary>
        private void openBufferedStream()
        {
            bStream = new BufferedStream(file);
        }

        /// <summary>
        /// Writes to file the ushort number as binary.
        /// </summary>
        /// <param name="number">The number which should be written to the file.</param>
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