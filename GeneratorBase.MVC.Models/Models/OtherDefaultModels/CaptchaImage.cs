using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
namespace GeneratorBase.MVC.Models
{
/// <summary>A captcha image.</summary>
public class CaptchaImage
{
    /// <summary>Gets the image.</summary>
    ///
    /// <value>The image.</value>
    
    public Bitmap Image
    {
        get
        {
            return this.image;
        }
    }
    /// <summary>The text.</summary>
    private string text;
    /// <summary>The width.</summary>
    private int width;
    /// <summary>The height.</summary>
    private int height;
    /// <summary>The font family.</summary>
    private System.Drawing.FontFamily fontFamily;
    /// <summary>The image.</summary>
    private Bitmap image;
    
    
    /// <summary>Constructor.</summary>
    ///
    /// <param name="width">     The width.</param>
    /// <param name="height">    The height.</param>
    /// <param name="fontFamily">The font family.</param>
    
    public CaptchaImage(int width, int height, System.Drawing.FontFamily fontFamily)
    {
        this.width = width;
        this.height = height;
        this.fontFamily = fontFamily;
    }
    
    /// <summary>Constructor.</summary>
    ///
    /// <param name="s">         The string.</param>
    /// <param name="width">     The width.</param>
    /// <param name="height">    The height.</param>
    /// <param name="fontFamily">The font family.</param>
    
    public CaptchaImage(string s, int width, int height, System.Drawing.FontFamily fontFamily)
    {
        this.text = s;
        this.width = width;
        this.height = height;
        this.fontFamily = fontFamily;
    }
    /// <summary>Default constructor.</summary>
    public CaptchaImage()
    {
        // TODO: Complete member initialization
    }
    
    /// <summary>Creates random text.</summary>
    ///
    /// <param name="Length">The length.</param>
    ///
    /// <returns>The new random text.</returns>
    
    public string CreateRandomText(int Length)
    {
        string allowedChars = "abcdefghijkmnopqrstuvwxyzABCDEFGHJKLMNOPQRSTUVWXYZ1234567890";
        char[] chars = new char[Length];
        System.Security.Cryptography.RandomNumberGenerator rd = new System.Security.Cryptography.RNGCryptoServiceProvider(); // Compliant for security-sensitive use cases
        for(int i = 0; i < Length; i++)
        {
            chars[i] =  allowedChars[GetNextRandomNumber(rd, allowedChars.Length)];
        }
        return new string(chars);
    }
    private static int GetNextRandomNumber(System.Security.Cryptography.RandomNumberGenerator random, int max)
    {
        if(max <= 0)
        {
            throw new ArgumentOutOfRangeException("max");
        }
        byte[] data = new byte[16];
        random.GetBytes(data);
        int value = BitConverter.ToInt32(data, 0) % max;
        if(value < 0)
        {
            value = -value;
        }
        return value;
    }
    /// <summary>Generates an image.</summary>
    
    
    /// <summary>Sets a text.</summary>
    ///
    /// <param name="text">The text.</param>
    
    public void SetText(string text)
    {
        this.text = text;
    }
}
}