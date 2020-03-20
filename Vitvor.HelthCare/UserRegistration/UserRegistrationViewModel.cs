using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Vitvor.HelthCare
{
    class UserRegistrationViewModel : INotifyPropertyChanged
    {
        private UserRegistration _userRegistration;
        public UserRegistration UserRegistration
        {
            get
            {
                return _userRegistration;
            }
            set
            {
                _userRegistration = value;
                OnPropertyChanged("UserRegistration");
            }
        }
        private RegistrationWindow _registrationWindow;
        public UserRegistrationViewModel(RegistrationWindow registrationWindow)
        {
            UserRegistration = new UserRegistration();
            _registrationWindow = registrationWindow;
        }
        private RelayCommand _afterNameAndSurnameInfo;
        public RelayCommand AfterNameAndSurnameInfo
        {
            get
            {
                return _afterNameAndSurnameInfo ??
                    (
                    _afterNameAndSurnameInfo = new RelayCommand(obj =>
                    {
                        ShowSpecificInformation();
                    }));
            }
        }
        private RelayCommand _cancel;
        public RelayCommand Cancel
        {
            get
            {
                return _cancel ??
                    (
                    _cancel = new RelayCommand(obj =>
                    {
                        _registrationWindow.Close();
                    }));
            }
        }
        private RelayCommand _toContactInformation;
        public RelayCommand ToContactInformation
        {
            get
            {
                return _toContactInformation ??
                    (
                    _toContactInformation = new RelayCommand(obj =>
                      {
                          ShowContackInformation();
                      }));
            }
        }
        private RelayCommand _backToNameAndSurname;
        public RelayCommand BackToNameAndSurname
        {
            get
            {
                return _backToNameAndSurname ??
                    (
                    _backToNameAndSurname = new RelayCommand(obj =>
                      {
                          ShowNameAndSurname();
                      }));
            }
        }
        private RelayCommand _backToSpecificInformation;
        public RelayCommand BackToSpecificInformation
        {
            
            get
            {
                return _backToSpecificInformation ??
                    (
                    _backToSpecificInformation = new RelayCommand(obj =>
                      {
                          ShowSpecificInformation();
                      }));
            }
        }
        private RelayCommand _ready;
        public RelayCommand Ready
        {
            get
            {
                return _ready ??
                    (
                    _ready = new RelayCommand(obj =>
                      {
                          UserRegistration userRegistration = obj as UserRegistration;
                          userRegistration.DateOfBirth = (DateTime)_registrationWindow.DateOfBirth.SelectedDate;
                          if((bool)_registrationWindow.Male.IsChecked)
                          {
                              userRegistration.Gender = Convert.ToString(_registrationWindow.Male.Content);
                          }
                          else if((bool)_registrationWindow.Female.IsChecked)
                          {
                              userRegistration.Gender = Convert.ToString(_registrationWindow.Female.Content);
                          }
                          userRegistration.Password = _registrationWindow.RegistrationPassword.Password;
                          SqlCommand sqlCommand = new SqlCommand();
                          sqlCommand.CommandText = $"insert into PATIENTS values ('{userRegistration.Surname}'," +
                          $"'{userRegistration.Name}','{userRegistration.Patronymic}','{userRegistration.DateOfBirth}'," +
                          $"'{userRegistration.Gender}', '{userRegistration.Email}', '{userRegistration.PhoneNumber}', " +
                          $"'{userRegistration.Password}')";
                          sqlCommand.Connection = SingletonForSqlConnection.SqlConnection;
                          sqlCommand.ExecuteNonQuery();
                          sqlCommand.CommandText = $"select PATIENTS.id, PATIENTS.Name, PATIENTS.Patronymic from PATIENTS where PATIENTS.Name='{userRegistration.Name}' " +
                          $"and PATIENTS.Surname='{userRegistration.Surname}' and " +
                          $"PATIENTS.Patronymic='{userRegistration.Patronymic}' and PATIENTS.Password='{userRegistration.Password}'";
                          SqlDataReader reader = sqlCommand.ExecuteReader();
                          foreach(var i in reader)
                          {
                              MessageBox.Show($"Id для входа: {reader.GetInt32(0)}", $"Здравствуйте, {reader.GetString(1)} {reader.GetString(2)}");
                          }
                          _registrationWindow.Close();
                      }));
            }
        }
        private void ShowContackInformation()
        {
            _registrationWindow.AfterSpecificInformaton.Visibility = Visibility.Collapsed;
            _registrationWindow.SpecificInformation.Visibility = Visibility.Collapsed;
            _registrationWindow.ContactInformation.Visibility = Visibility.Visible;
            _registrationWindow.AfterContackInformation.Visibility = Visibility.Visible;
        }
        private void ShowSpecificInformation()
        {
            _registrationWindow.ContactInformation.Visibility = Visibility.Collapsed;
            _registrationWindow.AfterContackInformation.Visibility = Visibility.Collapsed;
            _registrationWindow.NameAndSurname.Visibility = Visibility.Collapsed;
            _registrationWindow.SpecificInformation.Visibility = Visibility.Visible;
            _registrationWindow.AfterNameAndSurnameInfo.Visibility = Visibility.Collapsed;
            _registrationWindow.AfterSpecificInformaton.Visibility = Visibility.Visible;
        }
        private void ShowNameAndSurname()
        {
            _registrationWindow.NameAndSurname.Visibility = Visibility.Visible;
            _registrationWindow.SpecificInformation.Visibility = Visibility.Collapsed;
            _registrationWindow.AfterNameAndSurnameInfo.Visibility = Visibility.Visible;
            _registrationWindow.AfterSpecificInformaton.Visibility = Visibility.Collapsed;
        }
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
