using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

public class SteganographyManager
{
    private Image coverImage;
    private Boolean valid = false;
    private ACTION performAction;

    public enum ACTION {ENCODING, DECODING};

    private StegFileManager input;
    private StegFileManager output;

    protected SteganographyManager(String file, ACTION performAction, String inputFile, String outputFile)
    {
        this.performAction = performAction;
        createImage(file);
        if(isValid())
        {
            openFiles(inputFile, outputFile);
        }
    }

    public Boolean isValid()
    {
        return valid;
    }

    public Image getImage()
    {
        return coverImage;
    }

    public ACTION getAction()
    {
        return performAction;
    }

    public void close()
    {
        input.close();
        output.close();
    }

    protected void setValid(Boolean state)
    {
        valid = state;
    }

    private void createImage(String imageLocation)
    {
        coverImage = new Image(imageLocation);

        if (!coverImage.isValid())
        {
            setValid(false);
            System.Environment.Exit(99);
        }

        setValid(true);
    }

    private void openFiles(String inputFile, String outputFile)
    {
        input = new StegFileManager(this, StegFileManager.FILE_TYPE.READ_FILE, inputFile);
        if(input.isValid())
        {
            output = new StegFileManager(this, StegFileManager.FILE_TYPE.WRITE_FILE, outputFile);
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