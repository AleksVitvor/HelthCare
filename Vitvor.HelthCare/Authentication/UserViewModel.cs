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
using System.Windows.Controls;

namespace Vitvor.HelthCare
{
    class UserViewModel : INotifyPropertyChanged
    {
        private MainWindow _mainWindow;
        public MainWindow MainWindow
        {
            get { return _mainWindow; }
            set
            {
                _mainWindow = value;
            }
        }
        private User _user;
        public User User
        {
            get { return _user; }
            set
            {
                _user = value;
                OnPropertyChanged("User");
            }
        }
        private RelayCommand enter;
        public RelayCommand Enter
        {
            get
            {
                return enter ??
                    (enter = new RelayCommand(obj =>
                    {
                        User user = obj as User;
                        if (user != null)
                        {
                            SqlCommand sqlCommand = new SqlCommand();
                            user.Password = MainWindow.PassBox.Password;
                            if (Regex.IsMatch(user.UserName, @"^[0-9]"))
                            {
                                sqlCommand.CommandText = $"select * from PATIENTS where PATIENTS.id='{user.UserName}' and PATIENTS.Password='{user.Password}'";
                                sqlCommand.Connection = SingletonForSqlConnection.SqlConnection;
                                using (SqlDataReader reader = sqlCommand.ExecuteReader())
                                {
                                    if (reader.HasRows)
                                    {
                                        PatientWindow patientWindow = new PatientWindow(MainWindow, Convert.ToInt32(user.UserName));
                                        patientWindow.Show();
                                    }
                                    else
                                    {
                                        MessageBox.Show("Проверьте введённые данные", "Предупреждение");
                                    }
                                }
                            }
                            else if(Regex.IsMatch(user.UserName,@"Admin(\w*)"))
                            {
                                sqlCommand.CommandText = $"select ADMINS.AdminStatus, ADMINS.id from ADMINS where ADMINS.AdminUserName='{user.UserName}' and ADMINS.AdminPassword='{user.Password}'";
                                sqlCommand.Connection = SingletonForSqlConnection.SqlConnection;
                                using (SqlDataReader reader = sqlCommand.ExecuteReader())
                                {
                                    if (reader.HasRows)
                                    {
                                        reader.Read();
                                        int id = reader.GetInt32(1);
                                        if (reader != null && Convert.ToString(reader.GetValue(0)).Equals("main") && reader.NextResult() == false)
                                        {
                                            MainAdminWindow mainAdminWindow = new MainAdminWindow(MainWindow);
                                            mainAdminWindow.Show();
                                        }
                                        else if (reader != null && Convert.ToString(reader.GetValue(0)).Equals("MI") && reader.NextResult() == false)
                                        {
                                            AdminWindow adminWindow = new AdminWindow(MainWindow, id);
                                            adminWindow.Show();
                                        }
                                    }
                                    else
                                    {
                                        MessageBox.Show("Проверьте введённые данные", "Предупреждение"); ;
                                    }
                                }
                            }
                            else
                            {
                                sqlCommand.CommandText = $"SELECT [Name]," +
                                $"[Patronymic]," +
                                $"[Direction], [MedicalInstitutionid], [id] from DOCTORS where Username='{user.UserName}' and Password='{user.Password}'";
                                sqlCommand.Connection = SingletonForSqlConnection.SqlConnection;
                                using (SqlDataReader reader=sqlCommand.ExecuteReader())
                                {
                                    if (reader.HasRows)
                                    {
                                        reader.Read();
                                        if (reader.GetString(2).Equals("Медсестра"))
                                        {
                                            MessageBox.Show("Вход как медсестра");
                                        }
                                        else if (reader.GetString(2).Equals("Узкое направление"))
                                        {
                                            NarrowDoctorWindow narrowDoctorWindow = new NarrowDoctorWindow(_mainWindow, reader.GetInt32(4));
                                            narrowDoctorWindow.Show();
                                        }
                                        else if (reader.GetString(2).Equals("Общее направление"))
                                        {
                                            GeneralDoctorWindow generalDoctor = new GeneralDoctorWindow(_mainWindow, reader.GetInt32(3));
                                            generalDoctor.Show();
                                        }
                                    }
                                    else
                                    {
                                        MessageBox.Show("Проверьте введённые данные", "Предупреждение");
                                    }
                                }
                            }
                            MainWindow.PassBox.Clear();
                            MainWindow.UserNameBox.Clear();
                        }
                    }));
            }
        }
        private RelayCommand _registrate;
        public RelayCommand Registrate
        {
            get
            {
                return _registrate ??
                    (
                    _registrate=new RelayCommand(obj=>
                        {
                            RegistrationWindow registrationWindow = new RegistrationWindow(_mainWindow);
                            registrationWindow.Show();
                            _mainWindow.Hide();
                    }));
            }
        }
        public UserViewModel(MainWindow mainWindow)
        {
            User = new User();
            MainWindow = mainWindow;
        }
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
