using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace FrankStore
{
    public class PixelManager
    {
        private readonly SteganographyManager parent;

        private int imageWidth;
        private int imageHeight;
        private readonly bool valid;

        private readonly Dictionary<string, HexCharacter> characterBreakdown = new Dictionary<string, HexCharacter>();
        private readonly Dictionary<string, PixelInformation> pixelMap = new Dictionary<string, PixelInformation>();

        public PixelManager(SteganographyManager manager)
        {
            Console.WriteLine("Initialising pixel manager.");

            parent = manager;

            //Setup Class Params
            setupClass();

            if (parent.getAction() == SteganographyManager.Action.Encoding)
            {
                //Setup Manager For Each Letter
                setupHexCharacters();

                //Choose Initial Pixels
                Console.WriteLine("Choosing 1000 random pixels (this may increase later on).");
                addPixels(1000);
                Console.WriteLine("");
            }

            valid = true;
        }

        public bool isValid()
        {
            return valid;
        }

        public SteganographyManager getParent()
        {
            return parent;
        }

        public List<PixelInformation> getPixels()
        {
            return pixelMap.Values.ToList();
        }

        public List<Location> encode(string message)
        {
            return message.Select(c => characterBreakdown[c.ToString()].chooseHexCharacter()).ToList();
        }

        public string decode(IEnumerable<Location> locations)
        {
            var sb = new StringBuilder();

            foreach (var loc in locations)
            {

                var x = Convert.ToInt32(loc.getX());
                var y = Convert.ToInt32(loc.getY());
                var hashLocation = Convert.ToInt32(loc.getHashLocation());

                if (x >= imageWidth || y >= imageHeight || hashLocation >= 128)
                {
                    Console.Error.WriteLine("[ERROR]: Decode file has invalid sizes.");
                    Environment.Exit(91);
                }

                var key = x + "-" + y;

                PixelInformation px;

                try
                {
                    px = pixelMap[key];
                }
                catch (KeyNotFoundException)
                {
                    px = new PixelInformation(this, x, y);
                    pixelMap.Add(key, px);
                }

                sb.Append(px.getLetter(hashLocation));
            }

            return sb.ToString();
        }

        public void addPixels(int amount)
        {
            if (pixelMap.Count + 1 + amount > getImageSize())
            {
                Console.Error.WriteLine(
                    "[ERROR]: This image is not large enough to hide the data, please start again with a bigger image.");
                Environment.Exit(97);
            }

            using (var provider = new RNGCryptoServiceProvider())
            {
                for (var i = 0; i < amount; i++)
                {
                    var data = new byte[100];
                    var invalid = true;

                    do
                    {
                        //X
                        provider.GetBytes(data);
                        var value = BitConverter.ToUInt64(data, 0);
                        var x = (int) (value % Convert.ToUInt64(imageWidth));

                        //Y
                        provider.GetBytes(data);
                        value = BitConverter.ToUInt64(data, 0);
                        var y = (int) (value % Convert.ToUInt64(imageHeight));

                        var key = x.ToString() + "-" + y.ToString();

                        try
                        {
                            PixelInformation px = new PixelInformation(this, x, y);
                            pixelMap.Add(key, px);
                        }
                        //If it's duplicated we need a different one
                        catch (ArgumentException)
                        {
                            continue;
                        }

                        invalid = false;

                    } while (invalid);
                }

                updateHexCharacters();
            }
        }

        private int getImageSize()
        {
            return imageWidth * imageHeight;
        }

        private void updateHexCharacters()
        {
            var overall = new List<PixelInformation>[16];

            for (var i = 0; i < 16; i++)
            {
                overall[i] = new List<PixelInformation>();
            }

            foreach (var pi in pixelMap.Values.ToList())
            {
                var count = pi.getLetterCount();

                for (var i = 0; i < 16; i++)
                {
                    for (var y = 0; y < count[i]; y++)
                    {
                        overall[i].Add(pi);
                    }
                }
            }

            for (var i = 0; i < 16; i++)
            {
                characterBreakdown[Converter.intToHex(i)].updatePixels(overall[i]);
            }
        }

        private void setupClass()
        {
            Image image = parent.getImage();

            imageWidth = image.getWidth();
            imageHeight = image.getHeight();

            if (imageWidth * imageHeight >= 10) return;
            
            Console.Error.WriteLine("[ERROR]: The image does not have more than 10 pixels.");
        }

        private void setupHexCharacters()
        {
            for (var i = 0; i < 16; i++)
            {
                var hex = Converter.intToHex(i);
                var hexC = new HexCharacter(this, hex);
                characterBreakdown.Add(hex, hexC);
            }
        }
    }
}