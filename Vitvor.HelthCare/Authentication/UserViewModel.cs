using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
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
                            sqlCommand.CommandText = $"select * from PATIENTS where id={user.UserName}";
                            MainWindow.Authentication.Visibility = Visibility.Hidden;
                            MainWindow.BaseMainAdmin.Visibility = Visibility.Visible;
                        }
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
