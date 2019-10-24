using System;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

public class PixelInformation
{
	private PixelManager parent;

    private int x;
    private int y;

    private int[] count = new int[16];
    
    private Dictionary<String, List<int>> letterLocations = new Dictionary<String, List<int>>();

    private string hexColor;
    private string hash;
    private char[] breakDown;

    public PixelInformation(PixelManager par, int x, int y)
    {
        parent = par;

        this.x = x;
        this.y = y;

        getImageInformation();
        setupLetterMap();
        breakDown = hash.ToCharArray();
    }

    public int[] getLetterCount()
    {
        return count;
    }

    public Location getLetterLocation(String letter)
    {
        int hexNumber = Converter.hexToInt(letter);

        int hashLocation = letterLocations[letter][0];

        if((count[hexNumber] <= 0) || (hashLocation == -1))
        {
            Console.Error.WriteLine("[ERROR]: System miscalculated, do not trust this encoding.");
            System.Environment.Exit(94);
        }

        letterLocations[letter].RemoveAt(0);
        count[hexNumber]--;

        return new Location(x, y, hashLocation);
    }

    public string getLetter(int number)
    {
        return breakDown[number].ToString();
    }

    private void setupLetterMap()
    {
        for(int i = 0; i < 16; i++)
        {
            string hex = Converter.intToHex(i);

            //For The Actual Class Management (Speed of Service)
            letterLocations.Add(hex, new List<int>());

            int location = 0;
            int letterCount = 0;

            foreach(Char c in hash)
            {
                if(c.ToString().Equals(hex))
                {
                    letterLocations[hex].Add(location);
                    letterCount++;
                }
                location++;
            }

            //For The Parent System
            count[i] = letterCount;
        }
    }

    private void getImageInformation()
    {
        Image image = parent.getParent().getImage();

        Color color = image.getPixel(x, y);
        hexColor = image.getColorHex(color);

        StringBuilder source = new StringBuilder();
        source.Append(x.ToString());
        source.Append(y.ToString());
        source.Append(hexColor);

        using (SHA512 sha512Hash = SHA512.Create())
        {
            byte[] sourceBytes = Encoding.UTF8.GetBytes(source.ToString());
            byte[] hashBytes = sha512Hash.ComputeHash(sourceBytes);
            hash = BitConverter.ToString(hashBytes).Replace("-", String.Empty);
        }
    }
}