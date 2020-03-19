﻿using System;
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
                                sqlCommand.CommandText = $"select * from PATIENTS where id={user.UserName} and password={user.Password}";
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
