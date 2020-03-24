using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Vitvor.HelthCare
{
    class Doctor : INotifyPropertyChanged
    {
        private bool isChanging;
        public int MIid { get; private set; }
        public DateTime _dateOfBirth { get; set; }
        public string _direction { get; set; }
        private string _surname;
        public string Surname
        {
            get
            {
                return _surname;
            }
            set
            {
                if(isChanging)
                {
                    SqlCommand command = new SqlCommand();
                    command.CommandText = $"update DOCTORS set DOCTORS.Surname='{value}' where DOCTORS.Username='{Username}'";
                    command.Connection = SingletonForSqlConnection.SqlConnection;
                    command.ExecuteNonQuery();
                }
                _surname = value;
                OnPropertyChanged("Surname");
            }
        }
        private string _name;
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
                if(isChanging)
                {
                    SqlCommand command = new SqlCommand();
                    command.CommandText = $"update DOCTORS set DOCTORS.Name='{value}' where DOCTORS.Username='{Username}'";
                    command.Connection = SingletonForSqlConnection.SqlConnection;
                    command.ExecuteNonQuery();
                }
                OnPropertyChanged("Name");
            }
        }
        private string _patronymic;
        public string Patronymic
        {
            get
            {
                return _patronymic;
            }
            set
            {
                _patronymic = value;
                if(isChanging)
                {
                    SqlCommand command = new SqlCommand();
                    command.CommandText = $"update DOCTORS set DOCTORS.Patronymic='{value}' where DOCTORS.Username='{Username}'";
                    command.Connection = SingletonForSqlConnection.SqlConnection;
                    command.ExecuteNonQuery();
                }
                OnPropertyChanged("Patronymic");
            }
        }
        private string _specialty;
        public string Specialty
        {
            get
            {
                return _specialty;
            }
            set
            {
                _specialty = value;
                if(isChanging)
                {
                    SqlCommand command = new SqlCommand();
                    command.CommandText = $"update DOCTORS set DOCTORS.Specialty='{value}' where DOCTORS.Username='{Username}'";
                    command.Connection = SingletonForSqlConnection.SqlConnection;
                    command.ExecuteNonQuery();
                }
                OnPropertyChanged("Specialty");
            }
        }
        private string _username;
        public string Username
        {
            get
            {
                return _username;
            }
            set
            {
                _username = value;
                OnPropertyChanged("Username");
            }
        }
        private string _phoneNumber;
        public string PhoneNumber
        {
            get
            {
                return _phoneNumber;
            }
            set
            {
                _phoneNumber = value;
                if(isChanging)
                {
                    SqlCommand command = new SqlCommand();
                    command.CommandText = $"update DOCTORS set DOCTORS.PhoneNumber='{value}' where DOCTORS.Username='{Username}'";
                    command.Connection = SingletonForSqlConnection.SqlConnection;
                    command.ExecuteNonQuery();
                }
                OnPropertyChanged("PhoneNumber");
            }
        }
        private string _password;
        public string Password
        {
            get
            {
                return _password;
            }
            set
            {
                MD5 md5 = MD5.Create();
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

        public Doctor(int MIid)
        {
            this.isChanging = false;
            this.MIid = MIid;
        }
        public Doctor(int MIid, bool isChanging, string Surname, string Name, string Patronymic, string Specialty, string PhoneNumber, string Username)
        {
            this.MIid = MIid;
            this.isChanging = isChanging;
            _surname = Surname;
            _name = Name;
            _patronymic = Patronymic;
            _specialty = Specialty;
            _phoneNumber = PhoneNumber;
            this.Username = Username;
        }
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
