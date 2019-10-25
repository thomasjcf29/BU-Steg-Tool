using System;

namespace FrankStore
{
    public class SteganographyManager
    {
        private Image coverImage;
        private bool valid;
        private readonly Action performAction;

        public enum Action {Encoding, Decoding};

        private StegFileManager input;
        private StegFileManager output;

        protected SteganographyManager(string file, Action performAction, string inputFile, string outputFile)
        {
            this.performAction = performAction;
            createImage(file);
            if(isValid())
            {
                openFiles(inputFile, outputFile);
            }
        }

        public bool isValid()
        {
            return valid;
        }

        public Image getImage()
        {
            return coverImage;
        }

        public Action getAction()
        {
            return performAction;
        }

        public void close()
        {
            input.close();
            output.close();
        }

        protected void setValid(bool state)
        {
            valid = state;
        }

        protected StegFileManager getInputFile()
        {
            return input;
        }

        protected StegFileManager getOutputFile()
        {
            return output;
        }

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

        private void openFiles(string inputFile, string outputFile)
        {
            input = new StegFileManager(this, StegFileManager.FileType.ReadFile, inputFile);
            if(input.isValid())
            {
                output = new StegFileManager(this, StegFileManager.FileType.WriteFile, outputFile);
                if(!output.isValid())
                {
                    setValid(false);
                }
            }
            else
            {
                setValid(false);
            }
        }
    }
}