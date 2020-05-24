using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Vitvor.HelthCare
{
    class UserRegistration : INotifyPropertyChanged
    {
        private int _id = -1;
        public int id
        {
            get
            {
                return _id;
            }
            set
            {
                _id = value;
                OnPropertyChanged("id");
            }
        }
        public bool isUpdating { get; private set; } = false;
        private string _gender;
        public string Gender 
        { 
            get
            {
                return _gender;
            }
            set 
            {
                _gender = value;
                if(isUpdating)
                {
                    SqlCommand command = new SqlCommand();
                    command.Connection = SingletonForSqlConnection.SqlConnection;
                    command.CommandText = $"update PATIENTS set PATIENTS.Gender='{value}' where PATIENTS.id={id}";
                    command.ExecuteNonQuery();
                }
            }
        }
        private DateTime _dateOfBirth = DateTime.Now;
        public DateTime DateOfBirth
        {
            get
            {
                return _dateOfBirth;
            }
            set
            {
                if(isUpdating)
                {
                    SqlCommand command = new SqlCommand();
                    command.Connection = SingletonForSqlConnection.SqlConnection;
                    command.CommandText = $"update PATIENTS set PATIENTS.DateOfBirth='{value}' where PATIENTS.id={id}";
                    command.ExecuteNonQuery();
                }
                _dateOfBirth = value;
                OnPropertyChanged("DateOfBirth");
            }
        }
        private string _name;
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged("UserName");
            }
        }
        private string _surname;
        public string Surname
        {
            get
            {
                return _surname;
            }
            set
            {
                _surname = value;
                OnPropertyChanged("Surname");
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
                OnPropertyChanged("Patronymic");
            }
        }
        private string _email;
        public string Email
        {
            get
            {
                return _email;
            }
            set
            {
                _email = value;
                OnPropertyChanged("Email");
            }
        }
        private string _phoneNumber;
        public string PhoneNumber
        {
            get { return _phoneNumber; }
            set
            {
                if(isUpdating)
                {
                    SqlCommand command = new SqlCommand();
                    command.Connection = SingletonForSqlConnection.SqlConnection;
                    command.CommandText = $"update PATIENTS set PATIENTS.PhoneNumber='{value}' where PATIENTS.id={id}";
                    command.ExecuteNonQuery();
                }
                _phoneNumber = value;
                OnPropertyChanged("PhoneNumber");
            }
        }
        private string _password;
        public string Password
        {
            get { return _password; }
            set
            {
                MD5 md5 = MD5.Create();
                _password = GetMd5Hash(md5, value);
                if(isUpdating)
                {
                    SqlCommand command = new SqlCommand();
                    command.Connection = SingletonForSqlConnection.SqlConnection;
                    command.CommandText = $"update PATIENTS set PATIENTS.Password='{_password}' where PATIENTS.id={id}";
                    command.ExecuteNonQuery();
                }
                OnPropertyChanged("Password");
            }
        }
        private RelayCommand _checked;
        public RelayCommand Checked
        {
            get
            {
                return _checked ??
                    (_checked = new RelayCommand(obj =>
                    {
                        Gender = obj as string;
                      }));
            }
        }
        private RelayCommand _passwordChanged;
        public RelayCommand PasswordChanged
        {
            get
            {
                return _passwordChanged ??
                    (_passwordChanged = new RelayCommand(obj =>
                    {
                        PasswordBox password = obj as PasswordBox;
                        Password = password.Password;
                    }));
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
        public UserRegistration()
        { }
        public UserRegistration(int id, string Email, string Name, string Patronymic, bool isUpdating = true)
        {
            this.id = id;
            this.isUpdating = isUpdating;
            this.Email = Email;
            this.Name = Name;
            this.Patronymic = Patronymic;
        }
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
