using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Web;
namespace GeneratorBase.MVC.Models
{
///// <summary>Old encrypt decrypt.</summary>
//public class EncryptDecrypt
//{
//    /// <summary>Encrypts a string.</summary>
//    ///
//    /// <param name="param">The parameter.</param>
//    ///
//    /// <returns>A string.</returns>

//    public string EncryptString(object param)
//    {
//        string Message = Convert.ToString(param);
//        if(string.IsNullOrEmpty(Message))
//            return null;
//        string Passphrase = "Turanto";
//        byte[] Results;
//        System.Text.UTF8Encoding UTF8 = new System.Text.UTF8Encoding();
//        // Step 1. We hash the passphrase using MD5
//        // We use the MD5 hash generator as the result is a 128 bit byte array
//        // which is a valid length for the TripleDES encoder we use below
//        MD5CryptoServiceProvider HashProvider = new MD5CryptoServiceProvider();
//        byte[] TDESKey = HashProvider.ComputeHash(UTF8.GetBytes(Passphrase));
//        // Step 2. Create a new TripleDESCryptoServiceProvider object
//        TripleDESCryptoServiceProvider TDESAlgorithm = new TripleDESCryptoServiceProvider();
//        // Step 3. Setup the encoder
//        TDESAlgorithm.Key = TDESKey;
//        TDESAlgorithm.Mode = CipherMode.ECB;
//        TDESAlgorithm.Padding = PaddingMode.PKCS7;
//        // Step 4. Convert the input string to a byte[]
//        byte[] DataToEncrypt = UTF8.GetBytes(Message);
//        // Step 5. Attempt to encrypt the string
//        try
//        {
//            ICryptoTransform Encryptor = TDESAlgorithm.CreateEncryptor();
//            Results = Encryptor.TransformFinalBlock(DataToEncrypt, 0, DataToEncrypt.Length);
//        }
//        finally
//        {
//            // Clear the TripleDes and Hashprovider services of any sensitive information
//            TDESAlgorithm.Clear();
//            HashProvider.Clear();
//        }
//        // Step 6. Return the encrypted string as a base64 encoded string
//        return Convert.ToBase64String(Results);
//    }

//    /// <summary>Decrypt string.</summary>
//    ///
//    /// <param name="param">The parameter.</param>
//    ///
//    /// <returns>A string.</returns>

//    public string DecryptString(object param)
//    {
//        string Message = Convert.ToString(param);
//        if(string.IsNullOrEmpty(Message))
//            return null;
//        string Passphrase = "Turanto";
//        byte[] Results;
//        System.Text.UTF8Encoding UTF8 = new System.Text.UTF8Encoding();
//        // Step 1. We hash the passphrase using MD5
//        // We use the MD5 hash generator as the result is a 128 bit byte array
//        // which is a valid length for the TripleDES encoder we use below
//        MD5CryptoServiceProvider HashProvider = new MD5CryptoServiceProvider();
//        byte[] TDESKey = HashProvider.ComputeHash(UTF8.GetBytes(Passphrase));
//        // Step 2. Create a new TripleDESCryptoServiceProvider object
//        TripleDESCryptoServiceProvider TDESAlgorithm = new TripleDESCryptoServiceProvider();
//        // Step 3. Setup the decoder
//        TDESAlgorithm.Key = TDESKey;
//        TDESAlgorithm.Mode = CipherMode.ECB;
//        TDESAlgorithm.Padding = PaddingMode.PKCS7;
//        // Step 4. Convert the input string to a byte[]
//        byte[] DataToDecrypt = Convert.FromBase64String(Message);
//        // Step 5. Attempt to decrypt the string
//        try
//        {
//            ICryptoTransform Decryptor = TDESAlgorithm.CreateDecryptor();
//            Results = Decryptor.TransformFinalBlock(DataToDecrypt, 0, DataToDecrypt.Length);
//        }
//        finally
//        {
//            // Clear the TripleDes and Hashprovider services of any sensitive information
//            TDESAlgorithm.Clear();
//            HashProvider.Clear();
//        }
//        // Step 6. Return the decrypted string in UTF8 format
//        return UTF8.GetString(Results);
//    }
//}
/// <summary>An encrypt decrypt.</summary>
public class EncryptDecrypt
{
    /// <summary>Encrypts a string.</summary>
    ///
    /// <param name="param">The parameter.</param>
    ///
    /// <returns>A string.</returns>
    
    public string EncryptString(object param)
    {
        string Message = Convert.ToString(param);
        if(string.IsNullOrEmpty(Message))
            return null;
        string EncryptionKey = System.Configuration.ConfigurationManager.AppSettings["DBEncryptionKey"];
        byte[] Results;
        System.Text.UTF8Encoding UTF8 = new System.Text.UTF8Encoding();
        byte[] TDESKey = UTF8.GetBytes(EncryptionKey);
        using(var aes = new AesCryptoServiceProvider())
        {
            Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, TDESKey);
            aes.Key = pdb.GetBytes(32);
            aes.IV = pdb.GetBytes(16);
            aes.Mode = CipherMode.CBC;
            byte[] DataToEncrypt = UTF8.GetBytes(Message);
            try
            {
                ICryptoTransform Encryptor = aes.CreateEncryptor(aes.Key, aes.IV); //use machine key to make it more secure
                using(System.IO.MemoryStream ms = new System.IO.MemoryStream())
                {
                    using(CryptoStream cs = new CryptoStream(ms, Encryptor, CryptoStreamMode.Write))
                    {
                        cs.Write(DataToEncrypt, 0, DataToEncrypt.Length);
                        cs.Close();
                    }
                    Results = ms.ToArray();
                }
            }
            finally
            {
                aes.Clear();
            }
        }
        return Convert.ToBase64String(Results);
    }
    
    /// <summary>Decrypt string.</summary>
    ///
    /// <param name="param">The parameter.</param>
    ///
    /// <returns>A string.</returns>
    public string DecryptString(object param)
    {
        string Message = Convert.ToString(param);
        if(string.IsNullOrEmpty(Message))
            return null;
        string EncryptionKey = System.Configuration.ConfigurationManager.AppSettings["DBEncryptionKey"];
        byte[] Results;
        System.Text.UTF8Encoding UTF8 = new System.Text.UTF8Encoding();
        byte[] TDESKey = UTF8.GetBytes(EncryptionKey);
        using(var aes = new AesCryptoServiceProvider())
        {
            Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, TDESKey);
            aes.Key = pdb.GetBytes(32);
            aes.IV = pdb.GetBytes(16);
            aes.Mode = CipherMode.CBC;
            try
            {
                byte[] DataToDecrypt = Convert.FromBase64String(Message);
                ICryptoTransform Decryptor = aes.CreateDecryptor(aes.Key, aes.IV);//use machine key to make it more secure
                using(System.IO.MemoryStream ms = new System.IO.MemoryStream())
                {
                    using(CryptoStream cs = new CryptoStream(ms, Decryptor, CryptoStreamMode.Write))
                    {
                        cs.Write(DataToDecrypt, 0, DataToDecrypt.Length);
                    }
                    Results = ms.ToArray();
                }
            }
            catch
            {
                return Message;
            }
            finally
            {
                aes.Clear();
            }
        }
        return UTF8.GetString(Results);
    }
}
}