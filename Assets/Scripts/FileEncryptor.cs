// Add System.IO to work with files!
using System.IO;
// Add System.Security.Crytography to use Encryption!
using System.Security.Cryptography;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

public static class FileEncryptor
{
    // Key for reading and writing encrypted data.
    // (This is a "hardcoded" secret key. )
    private static byte[] savedKey = { 0x9, 0x1, 0x5, 0x5, 0x13, 0x12, 0x8, 0x4, 0x15, 0x8, 0x16, 0x04, 0x06, 0x7, 0x5, 0x12 };

    public static PlayerData ReadFile(string path)
    {
        // Does the file exist?
        if (File.Exists(path))
        {
            // Create FileStream for opening files.
            FileStream dataStream = new FileStream(path, FileMode.Open);

            // Create new AES instance.
            Aes oAes = Aes.Create();

            // Create an array of correct size based on AES IV.
            byte[] outputIV = new byte[oAes.IV.Length];

            // Read the IV from the file.
            dataStream.Read(outputIV, 0, outputIV.Length);

            // Create CryptoStream, wrapping FileStream
            CryptoStream oStream = new CryptoStream(
                   dataStream,
                   oAes.CreateDecryptor(savedKey, outputIV),
                   CryptoStreamMode.Read);

            // Create a StreamReader, wrapping CryptoStream
            StreamReader reader = new StreamReader(oStream);

            // Read the entire file into a String value.
            string text = reader.ReadToEnd();

            dataStream.Close();

            Debug.Log(text);

            // Deserialize the JSON data 
            //  into a pattern matching the GameData class.
            return JsonConvert.DeserializeObject<PlayerData>(text);
        }

        return new PlayerData();
    }

    public static async Task WriteFile(string path, PlayerData data)
    {
        // Create new AES instance.
        Aes iAes = Aes.Create();

        // Create a FileStream for creating files.
        FileStream dataStream = new FileStream(path + "_temp", FileMode.OpenOrCreate);

        // Save the new generated IV.
        byte[] inputIV = iAes.IV;

        // Write the IV to the FileStream unencrypted.
        dataStream.Write(inputIV, 0, inputIV.Length);

        // Create CryptoStream, wrapping FileStream.
        CryptoStream iStream = new CryptoStream(
                dataStream,
                iAes.CreateEncryptor(savedKey, iAes.IV),
                CryptoStreamMode.Write);

        // Create StreamWriter, wrapping CryptoStream.
        StreamWriter sWriter = new StreamWriter(iStream);

        // Serialize the object into JSON and save string.
        string jsonString = JsonConvert.SerializeObject(data);
        Debug.Log(jsonString);

        // Write to the innermost stream (which will encrypt).
        await sWriter.WriteAsync(jsonString);

        // Close StreamWriter.
        sWriter.Close();

        // Close CryptoStream.
        iStream.Close();

        // Close FileStream.
        dataStream.Close();

        File.Delete(path);
        File.Move(path + "_temp", path);
    }
}