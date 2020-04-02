using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Vitvor.HelthCare
{
    class Password
    {
        public SecureString PasswordString { get; private set; }
        public byte[] Key { get; private set; }
        public byte[] IV { get; private set; }
        public byte[] PasswordBytes { get; private set; }
        public Password()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(byte[]));
            using(FileStream fs=new FileStream("Key.xml", FileMode.Open))
            {
                Key = (byte[])serializer.Deserialize(fs);
            }
            using(FileStream fs=new FileStream("IV.xml", FileMode.Open))
            {
                IV = (byte[])serializer.Deserialize(fs);
            }
            using(FileStream fs=new FileStream("Pass.xml", FileMode.Open))
            {
                PasswordBytes = (byte[])serializer.Deserialize(fs);
            }
        }
    }
}
