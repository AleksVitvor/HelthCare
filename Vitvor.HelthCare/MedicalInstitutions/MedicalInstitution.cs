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
    class MedicalInstitution : INotifyPropertyChanged
    {
        private string _medicalInstitutionName;
        public string MedicalInstitutionName
        {
            get
            {
                return _medicalInstitutionName;
            }
            set
            {
                _medicalInstitutionName = value;
                OnPropertyChanged("MedicalInstitutionName");
            }
        }
        private string _adminUsername;
        public string AdminUsername
        {
            get
            {
                return _adminUsername;
            }
            set
            {
                _adminUsername = value;
                OnPropertyChanged("AdminUsername");
            }
        }
        private string _adminPassword;
        public string AdminPassword
        {
            get
            {
                return _adminPassword;
            }
            set
            {
                MD5 md5 = MD5.Create();
                _adminPassword = GetMd5Hash(md5, value);
                OnPropertyChanged("AdminPassword");
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
