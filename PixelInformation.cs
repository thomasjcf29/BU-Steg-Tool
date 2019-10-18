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
}