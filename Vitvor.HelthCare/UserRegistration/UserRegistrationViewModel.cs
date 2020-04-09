using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Vitvor.HelthCare
{
    class UserRegistrationViewModel : INotifyPropertyChanged
    {
        private RegistrationWindow _registrationWindow;
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
                          using (SqlDataReader reader = sqlCommand.ExecuteReader())
                          {
                              if(reader.HasRows)
                              {
                                  reader.Read();
                                  string message = $"Здравствуйте, {UserRegistration.Name} {UserRegistration.Patronymic}. Номер вашей карточки для входа: {reader.GetInt32(0)}. Для продолжения работы " +
                                  $"вернитесь на начальный экран и произведите вход в систему";
                                  string sub = "Регистрация";
                                  SendEmailAsync(UserRegistration, message, sub).GetAwaiter();
                              }
                          }
                          _registrationWindow.Close();
                      }));
            }
        }
        private RelayCommand _search;
        public RelayCommand Search
        {
            get
            {
                return _search ??
                    (_search = new RelayCommand(obj =>
                      {
                          SqlCommand searchPatient = new SqlCommand();
                          searchPatient.Connection = SingletonForSqlConnection.SqlConnection;
                          searchPatient.CommandText = $"select PATIENTS.id, PATIENTS.Email, PATIENTS.Name, PATIENTS.Patronymic from PATIENTS where PATIENTS.id={_registrationWindow.CardNumberInfo.Text}";
                          using(SqlDataReader reader=searchPatient.ExecuteReader())
                          {
                              if(reader.HasRows)
                              {
                                  reader.Read();
                                  UserRegistration = new UserRegistration(reader.GetInt32(0), reader.GetString(1), reader.GetString(2), reader.GetString(3));
                                  string alphabet = "ABSDEFGHIJKLMNOPQRSTVUWXYZabsdefghijklmnopqrstvuwxyz0123456789";
                                  char[] letters = alphabet.ToCharArray();
                                  string resultstr = "";
                                  Random random = new Random();
                                  for (int i = 0; i < 10; i++) 
                                  {
                                      resultstr += letters[random.Next(0, letters.Length - 1)];
                                      Thread.Sleep(100);
                                  }
                                  reader.Close();
                                  SqlCommand command = new SqlCommand();
                                  command.Connection = SingletonForSqlConnection.SqlConnection;
                                  UserRegistration.Password = resultstr;
                                  command.CommandText = $"update PATIENTS set PATIENTS.Password='{UserRegistration.Password}' where PATIENTS.id={UserRegistration.id}";
                                  command.ExecuteNonQuery();
                                  string message = $"Здравствуйте, {UserRegistration.Name} {UserRegistration.Patronymic}, для подтверждения уточнения данных введите данную строку\n{UserRegistration.Password}\n в поле, которое у вас отобразилось сейчас.";
                                  string sub = $"Уточнение данных";
                                  SendEmailAsync(UserRegistration, message, sub).GetAwaiter();
                                  ShowCheckingPass();
                              }
                              else
                              {
                                  MessageBox.Show("Проверьте введённые параметры");
                              }    
                          }
                      }));
            }
        }
        private RelayCommand _confirm;
        public RelayCommand Confirm
        {
            get
            {
                return _confirm ??
                    (_confirm = new RelayCommand(obj =>
                      {
                          SqlCommand check = new SqlCommand();
                          check.Connection = SingletonForSqlConnection.SqlConnection;
                          check.CommandText = $"select PATIENTS.id from PATIENTS where PATIENTS.id={UserRegistration.id} and PATIENTS.Password='{_registrationWindow.CheckPass.Text.Trim(' ')}'";
                          using(SqlDataReader reader=check.ExecuteReader())
                          {
                              if(reader.HasRows)
                              {
                                  ShowAfterRegistration();
                              }
                              else
                              {
                                  MessageBox.Show("Проверьте введённые данные");
                              }
                          }

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
            _registrationWindow.Pass.Visibility = Visibility.Collapsed;
            _registrationWindow.SpecificInformation.Visibility = Visibility.Visible;
            _registrationWindow.AfterNameAndSurnameInfo.Visibility = Visibility.Collapsed;
            _registrationWindow.AfterSpecificInformaton.Visibility = Visibility.Visible;
        }
        private void ShowNameAndSurname()
        {
            _registrationWindow.NameAndSurname.Visibility = Visibility.Visible;
            _registrationWindow.Pass.Visibility = Visibility.Visible;
            _registrationWindow.SpecificInformation.Visibility = Visibility.Collapsed;
            _registrationWindow.AfterNameAndSurnameInfo.Visibility = Visibility.Visible;
            _registrationWindow.AfterSpecificInformaton.Visibility = Visibility.Collapsed;
        }
        private void ShowAfterRegistration()
        {
            _registrationWindow.Confirm.Visibility = Visibility.Collapsed;
            _registrationWindow.Check.Visibility = Visibility.Collapsed;
            _registrationWindow.Search.Visibility = Visibility.Collapsed;
            _registrationWindow.CardNumber.Visibility = Visibility.Collapsed;
            _registrationWindow.NameAndSurname.Visibility = Visibility.Collapsed;
            _registrationWindow.Pass.Visibility = Visibility.Visible;
            _registrationWindow.SpecificInformation.Visibility = Visibility.Visible;
            _registrationWindow.ContactInformation.Visibility = Visibility.Visible;
            _registrationWindow.EmailLabel.Visibility = Visibility.Collapsed;
            _registrationWindow.EmailTextBox.Visibility = Visibility.Collapsed;
        }
        private void ShowCardNumberAndSearch()
        {
            _registrationWindow.NameAndSurname.Visibility = Visibility.Collapsed;
            _registrationWindow.Pass.Visibility = Visibility.Collapsed;
            _registrationWindow.CardNumber.Visibility = Visibility.Visible;
            _registrationWindow.Commands.Visibility = Visibility.Collapsed;
            _registrationWindow.Search.Visibility = Visibility.Visible;
        }
        private void ShowCheckingPass()
        {
            _registrationWindow.Search.Visibility = Visibility.Collapsed;
            _registrationWindow.CardNumber.Visibility = Visibility.Collapsed;
            _registrationWindow.Check.Visibility = Visibility.Visible;
            _registrationWindow.Confirm.Visibility = Visibility.Visible;
        }
        public void SendEmailAboutUpdate()
        {
            string message = $"Здравствуйте, {UserRegistration.Name} {UserRegistration.Patronymic}, наш центр благодарит вас за уточнение информации о Вас. Полную функциональность вы сможете изучить, войдя " +
                $"в систему на начальном окне.\nТакже с помощью этого окна вы сможете изменить свой пароль.";
            string sub = "Завершение регистрации";
            SendEmailAsync(UserRegistration, message, sub).GetAwaiter();
        }
        private static async Task SendEmailAsync(UserRegistration user, string message, string sub)
        {
            MailAddress from = new MailAddress("healthcaresupbelstu@gmail.com", "Ministry of Health by BelSTU");
            if (!System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
            {
                MessageBox.Show("Отсутствует или ограниченно физическое подключение к сети\nПроверьте настройки вашего сетевого подключения");
            }
            else if (isValid(user.Email))
            {
                MailAddress to = new MailAddress(user.Email);
                MailMessage m = new MailMessage(from, to);
                m.Subject = sub;
                m.Body = message;
                SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
                smtp.Credentials = new NetworkCredential("healthcaresupbelstu@gmail.com", $"{Password.getInstance().myCredential.Password}");
                smtp.EnableSsl = true;
                await smtp.SendMailAsync(m);
            }
            else
            {
                MessageBox.Show("Проверьте введённую электронную почту");
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
        public UserRegistrationViewModel(RegistrationWindow registrationWindow)
        {
            UserRegistration = new UserRegistration();
            _registrationWindow = registrationWindow;
            MessageBoxResult result = MessageBox.Show("Вы уже имеете зарегистрированную карту?",
                                  "Сообщение",
                                  MessageBoxButton.YesNo,
                                  MessageBoxImage.Question,
                                  MessageBoxResult.Yes,
                                  MessageBoxOptions.DefaultDesktopOnly);
            if (result == MessageBoxResult.Yes)
            {
                ShowCardNumberAndSearch();
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
