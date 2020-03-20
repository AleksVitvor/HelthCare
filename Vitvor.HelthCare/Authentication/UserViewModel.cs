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
        public MainAdminWindow _adminWindow { get; set; }
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
                            if (Regex.IsMatch(user.UserName, @"\d"))
                            {
                                sqlCommand.CommandText = $"select * from PATIENTS where PATIENTS.id='{user.UserName}' and PATIENTS.Password='{user.Password}'";
                                sqlCommand.Connection = SingletonForSqlConnection.SqlConnection;
                                SqlDataReader reader = sqlCommand.ExecuteReader();
                                if(reader!=null && reader.NextResult()==false)
                                {
                                    PatientWindow patientWindow = new PatientWindow(MainWindow);
                                    MainWindow.Hide();
                                    patientWindow.Show();
                                }
                            }
                            else if(Regex.IsMatch(user.UserName,@"mainAdministrator(\w*)"))
                            {
                                sqlCommand.CommandText = $"select * from ADMINS where id={user.UserName} and password={user.Password}";
                                MainAdminWindow mainAdminWindow = new MainAdminWindow(MainWindow);
                                _adminWindow = mainAdminWindow;
                                mainAdminWindow.Show();                                                               
                            }
                            else if(Regex.IsMatch(user.UserName, @"Administrator(\w*)"))
                            {
                                sqlCommand.CommandText= $"select* from ADMINS where id ={ user.UserName} and password = { user.Password }";
                            }
                            else
                            {
                                sqlCommand.CommandText = $"select * from DOCTORS where id={user.UserName} and password={user.Password}";
                            }
                            MainWindow.Hide();
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
