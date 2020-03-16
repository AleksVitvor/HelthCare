using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Vitvor.HelthCare
{
    class User : INotifyPropertyChanged
    {
        private string _userName;
        public string UserName
        {
            get { return _userName; }
            set
            {
                _userName = value;
                OnPropertyChanged("UserName");
            }
        }
        private string _password;
        public string Password
        {
            get { return _password; }
            set
            {
                MD5 md5=MD5.Create();
                _password = GetMd5Hash(md5, value);
                OnPropertyChanged("Password");
            }
        }

        private string GetMd5Hash(MD5 md5, string value)
        {
            byte[] data = md5.ComputeHash(Encoding.UTF8.GetBytes(value));
            StringBuilder sbuilder = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sbuilder.Append(data[i].ToString("x2"));
            }
            return sbuilder.ToString();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
