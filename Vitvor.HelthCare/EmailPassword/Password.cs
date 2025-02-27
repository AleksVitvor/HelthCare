﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Serialization;

namespace Vitvor.HelthCare
{
    class Password
    {
        private static Password instance;
        public NetworkCredential myCredential { get; private set; } 
        private byte[] Key;
        private byte[] IV;
        private byte[] PasswordBytes;
        private Password()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(byte[]));
            using (FileStream fs = new FileStream("Key.xml", FileMode.Open))
            {
                Key = (byte[])serializer.Deserialize(fs);
            }
            using (FileStream fs = new FileStream("IV.xml", FileMode.Open))
            {
                IV = (byte[])serializer.Deserialize(fs);
            }
            using (FileStream fs = new FileStream("Pass.xml", FileMode.Open))
            {
                PasswordBytes = (byte[])serializer.Deserialize(fs);
            }
            using (Aes myAes = Aes.Create())
            {
                myAes.Key = Key;
                myAes.IV = IV;
                // Decrypt the bytes to a string.
                string roundtrip = DecryptStringFromBytes_Aes(PasswordBytes, myAes.Key, myAes.IV);
                SecureString PasswordString = new SecureString();
                foreach(var i in roundtrip)
                {
                    PasswordString.AppendChar(i);
                }
                myCredential = new NetworkCredential("healthcaresupbelstu@gmail.com", PasswordString);
                PasswordString.Dispose();
            }
        }
        static string DecryptStringFromBytes_Aes(byte[] cipherText, byte[] Key, byte[] IV)
        {
            // Check arguments.
            if (cipherText == null || cipherText.Length <= 0)
                throw new ArgumentNullException("cipherText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");

            // Declare the string used to hold
            // the decrypted text.
            string plaintext = null;

            // Create an Aes object
            // with the specified key and IV.
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                // Create a decryptor to perform the stream transform.
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for decryption.
                using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {

                            // Read the decrypted bytes from the decrypting stream
                            // and place them in a string.
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }

            return plaintext;
        }
        public static Password getInstance()
        {
            if (instance == null)
                instance = new Password();
            return instance;
        }

    }
}