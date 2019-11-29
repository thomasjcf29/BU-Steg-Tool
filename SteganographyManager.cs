using System;

namespace FrankStore
{
    /// <summary>
    /// <c>SteganographyManager</c> is the parent (inheritied) from the FrankEncoding/Decoding classes.
    /// It's a place to store generic information across the two classes.
    /// </summary>
    public class SteganographyManager
    {
        /// <summary>
        /// Image file which will be used as the coverImage.
        /// </summary>
        private Image coverImage;

        /// <summary>
        /// Boolean to determine if this class was successfully setup.
        /// If false, do not use!
        /// </summary>
        private bool valid;

        /// <summary>
        /// Variable to determine if this initilised class is for encoding or decoding.
        /// It can only be one.
        /// </summary>
        private readonly Action performAction;

        /// <summary>
        /// Enum for Encoding or Decoding.
        /// </summary>
        public enum Action {Encoding, Decoding};

        /// <summary>
        /// Input file to be read from.
        /// </summary>
        private StegFileManager input;

        /// <summary>
        /// Output file to be written to.
        /// </summary>
        private StegFileManager output;

        /// <summary>
        /// <c>SteganographyManager</c> constructor, which sets up all the files to be read / written from.
        /// It also sets up the class with information.
        /// </summary>
        /// <param name="file">The image file to be used as a cover image.</param>
        /// <param name="performAction">Are you encoding or decoding?</param>
        /// <param name="inputFile">Input file to be read from.</param>
        /// <param name="outputFile">Output file to be written to.</param>
        protected SteganographyManager(string file, Action performAction, string inputFile, string outputFile)
        {
            this.performAction = performAction;
            createImage(file);
            if(isValid())
            {
                openFiles(inputFile, outputFile);
            }
        }

        /// <summary>
        /// Returns if valid or not, if false do not use.
        /// </summary>
        /// <returns>True for successfully setup, or false for not.</returns>
        public bool isValid()
        {
            return valid;
        }

        /// <summary>
        /// Returns the coverimage to anyone asking for it.
        /// </summary>
        /// <returns>Cover image class.</returns>
        public Image getImage()
        {
            return coverImage;
        }

        /// <summary>
        /// Returns whether this class was setup for encoding or decoding.
        /// </summary>
        /// <returns>Action.Encoding or Action.Decoding.</returns>
        public Action getAction()
        {
            return performAction;
        }

        /// <summary>
        /// Closes the handler to the input / output file.
        /// </summary>
        public void close()
        {
            input.close();
            output.close();
        }

        /// <summary>
        /// Sets the classes valid state.
        /// </summary>
        /// <param name="state">True - class can be used, False - cannot be used.</param>
        protected void setValid(bool state)
        {
            valid = state;
        }

        /// <summary>
        /// Returns the input file to be read from.
        /// </summary>
        /// <returns><c>StegFileManager</c> of the input file.</returns>
        protected StegFileManager getInputFile()
        {
            return input;
        }

        /// <summary>
        /// Returns the output file to be written to.
        /// </summary>
        /// <returns><c>StegFileManager</c> of the output file.</returns>
        protected StegFileManager getOutputFile()
        {
            return output;
        }

        /// <summary>
        /// Creates the <c>Image</c> class based on the specified image location.
        /// Will exit if it trying to open the image fails.
        /// </summary>
        /// <param name="imageLocation">Location of the image to be used.</param>
        private void createImage(string imageLocation)
        {
            coverImage = new Image(imageLocation);

            if (!coverImage.isValid())
            {
                setValid(false);
                Environment.Exit(99);
            }

            setValid(true);
        }

        /// <summary>
        /// Creates the handlers for the input and output files.
        /// If it cannot open/create one it will set the class validity to false.
        /// </summary>
        /// <param name="inputFile">Location of the input file.</param>
        /// <param name="outputFile">Location of the output file.</param>
        private void openFiles(string inputFile, string outputFile)
        {
            input = new StegFileManager(this, StegFileManager.FileType.ReadFile, inputFile);
            output = new StegFileManager(this, StegFileManager.FileType.WriteFile, outputFile);
            
            if(!input.isValid() || !output.isValid()) setValid(false);
        }
    }
}