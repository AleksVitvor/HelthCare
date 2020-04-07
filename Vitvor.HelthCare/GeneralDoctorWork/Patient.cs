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
                SqlCommand sqlCommand = new SqlCommand();
                sqlCommand.CommandText = $"update PATIENTS set Email='{value}' where id={this.patientid}";
                sqlCommand.Connection = SingletonForSqlConnection.SqlConnection;
                sqlCommand.ExecuteNonQuery();
                _email = value;
                OnPropertyChanged("Email");
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
                if (_symptoms.Length != 0)
                {
                    if (_symptoms[_symptoms.Length - 1] == ',' || _symptoms[_symptoms.Length - 1] == ';')
                    {
                        string[] symptoms = Symptoms.Split(',');
                        symptoms[symptoms.Length - 2] = symptoms[symptoms.Length - 2].Trim(' ').ToLower();
                        SqlCommand command = new SqlCommand();
                        command.Connection = SingletonForSqlConnection.SqlConnection;
                        command.CommandText = $"select * from SYMPTOMS where SYMPTOMS.Name='{symptoms[symptoms.Length - 2]}'";
                        using (SqlDataReader checkSymptoms = command.ExecuteReader())
                        {
                            if (checkSymptoms.HasRows)
                            {
                                checkSymptoms.Read();
                                int id = checkSymptoms.GetInt32(0);
                                checkSymptoms.Close();
                                command.CommandText = $"select * from PATIENTSANDSYMPTOMS where PATIENTSANDSYMPTOMS.patientid='{patientid}' and " +
                                    $"PATIENTSANDSYMPTOMS.symptomid='{id}' and PATIENTSANDSYMPTOMS.dateofexhibiting='{DateTime.Today}'";
                                using (SqlDataReader checkdata = command.ExecuteReader())
                                {
                                    if (!checkdata.HasRows)
                                    {
                                        checkdata.Close();
                                        command.CommandText = $"insert into PATIENTSANDSYMPTOMS values ({patientid},{id},'{DateTime.Today}')";
                                        command.ExecuteNonQuery();
                                    }
                                    else
                                    {
                                        MessageBox.Show("Такие данные уже добавлены");
                                    }
                                }
                            }
                            else
                            {
                                MessageBox.Show("Данный симптом будет добавлен позже, приносим свои извинения");
                            }
                        }
                    }
                }
                OnPropertyChanged("Symptoms");
            }
        }

        public Patient(int id)
        {
            patientid = id;
        }
        public Patient()
        { }
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
