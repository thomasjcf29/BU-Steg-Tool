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

    private List<String> usedCharacters = new List<String>();
    private List<String> leftCharacters = new List<String>();

    private string hexColor;
    private string hash;

    public PixelInformation(PixelManager par, int x, int y)
    {
        parent = par;

        this.x = x;
        this.y = y;

        getImageInformation();
        addCharacters();
    }

    public int getLetterCount(String letter)
    {
        int count = 0;

        foreach(String s in leftCharacters)
        {
            if (s.Equals(letter)) count++;
        }

        return count;
    }

    public Location getLetterLocation(String letter)
    {
        int amountToSkip = getUsedLetterCount(letter);
        int hashLocation = getHashLocation(letter, amountToSkip);

        if((getLetterCount(letter) <= 0) || (hashLocation == -1))
        {
            Console.Error.WriteLine("[ERROR]: System miscalculated, do not trust this encoding.");
            System.Environment.Exit(94);
        }

        usedCharacters.Add(letter);
        leftCharacters.Remove(letter);

        return new Location(x, y, hashLocation);
    }

    public string getLetter(int number)
    {
        return hash.ToCharArray()[number].ToString();
    }

    private void getImageInformation()
    {
        Image image = parent.getParent().getParent().getImage();

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

    private void addCharacters()
    {
        foreach(Char c in hash)
        {
            leftCharacters.Add(c.ToString());
        }
    }

    private int getUsedLetterCount(String letter)
    {
        int count = 0;

        foreach(String s in usedCharacters)
        {
            if (s.Equals(letter)) count++;
        }

        return count;
    }

    private int getHashLocation(string letter, int skipBy)
    {
        int count = 0;
        int skipCount = 0;

        foreach(Char c in hash)
        {
            if (c.ToString().Equals(letter))
            {
                if(skipCount >= skipBy)
                {
                    return count;
                }

                skipCount++;
            }
            count++;
        }

        return -1;
    }
}