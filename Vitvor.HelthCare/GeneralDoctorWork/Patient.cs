using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace Vitvor.HelthCare
{
    class Patient : INotifyPropertyChanged
    {
        public int patientid { get; set; }
        private string _surname;
        public string Surname
        {
            get
            {
                return _surname;
            }
            set
            {
                SqlCommand sqlCommand = new SqlCommand();
                sqlCommand.CommandText = $"update PATIENTS set Surname='{value}' where id={this.patientid}";
                sqlCommand.Connection = SingletonForSqlConnection.SqlConnection;
                sqlCommand.ExecuteNonQuery();
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
                SqlCommand sqlCommand = new SqlCommand();
                sqlCommand.CommandText = $"update PATIENTS set Name='{value}' where id={this.patientid}";
                sqlCommand.Connection = SingletonForSqlConnection.SqlConnection;
                sqlCommand.ExecuteNonQuery();
                _name = value;
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
                SqlCommand sqlCommand = new SqlCommand();
                sqlCommand.CommandText = $"update PATIENTS set Patronymic='{value}' where id={this.patientid}";
                sqlCommand.Connection = SingletonForSqlConnection.SqlConnection;
                sqlCommand.ExecuteNonQuery();
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
                if (isValid(value))
                {
                    SqlCommand sqlCommand = new SqlCommand();
                    sqlCommand.CommandText = $"update PATIENTS set Email='{value}' where id={this.patientid}";
                    sqlCommand.Connection = SingletonForSqlConnection.SqlConnection;
                    sqlCommand.ExecuteNonQuery();
                    _email = value;
                    OnPropertyChanged("Email");
                }
                else
                {
                    MessageBox.Show("Почта введена неверно");
                }
            }
        }
        private static bool isValid(string email)
        {
            string pattern = "[.\\-_a-z0-9]+@([a-z0-9][\\-a-z0-9]+\\.)+[a-z]{2,6}";
            Match isMatch = Regex.Match(email, pattern, RegexOptions.IgnoreCase);

            if (isMatch.Success)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private string _symptoms;
        public string Symptoms
        {
            get
            {
                return _symptoms;
            }
            set
            {
                _symptoms = value;
                OnPropertyChanged("Symptoms");
            }
        }

        public Patient(int id)
        {
            patientid = id;
        }
        public Patient()
        { }
        public Patient(string surname, string name, string patronymic, string email)
        {
            _surname = surname;
            _name = name;
            _patronymic = patronymic;
            _email = email;
        }
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
