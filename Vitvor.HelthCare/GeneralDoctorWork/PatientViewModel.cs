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
using System.Threading.Tasks;
using System.Windows;

namespace Vitvor.HelthCare
{
    class PatientViewModel : INotifyPropertyChanged
    {
        private GeneralDoctorWindow _doctorWindow;
        private int idofMI;
        private Patient _selectedPatient;
        public Patient SelectedPatient
        {
            get
            {
                return _selectedPatient;
            }
            set
            {
                _selectedPatient = value;
                OnPropertyChanged("SelectedPatient");
            }
        }
        private RelayCommand _inspection;
        public RelayCommand Inspection
        {
            get
            {
                return _inspection ??
                    (_inspection = new RelayCommand(obj =>
                      {
                          Hide();
                          SqlCommand command = new SqlCommand();
                          command.Connection = SingletonForSqlConnection.SqlConnection;
                          command.CommandText= "delete from PATIENTS where PATIENTS.Surname='' and " +
                                  $"PATIENTS.Name=''";
                          command.ExecuteNonQuery();
                          while (true)
                          {
                              MessageBoxResult result = MessageBox.Show("Первый приём?",
                                  "Сообщение",
                                  MessageBoxButton.YesNo,
                                  MessageBoxImage.Question,
                                  MessageBoxResult.Yes,
                                  MessageBoxOptions.DefaultDesktopOnly);
                              if (result == MessageBoxResult.No)
                              {
                                  _doctorWindow.OnlyID.Visibility = Visibility.Visible;
                                  _doctorWindow.SearchPatient.Visibility = Visibility.Visible;
                                  _doctorWindow.GeneralInfo.IsEnabled = false;
                                  break;
                              }
                              else if (result == MessageBoxResult.Yes)
                              {
                                  _doctorWindow.AllInfo.Visibility = Visibility.Visible;
                                  _doctorWindow.SendInfoPatient.Visibility = Visibility.Visible;
                                  SqlCommand add = new SqlCommand();
                                  add.Connection = SingletonForSqlConnection.SqlConnection;
                                  add.CommandText = $"insert into PATIENTS(Surname, Name, Patronymic) values ('','','')";
                                  add.ExecuteNonQuery();
                                  add.CommandText = $"select PATIENTS.id from PATIENTS where PATIENTS.Surname='' and " +
                                  $"PATIENTS.Name=''";
                                  int patientid = 0;
                                  using(SqlDataReader reader=add.ExecuteReader())
                                  {
                                      reader.Read();
                                      patientid = reader.GetInt32(0);
                                      reader.Close();
                                  }
                                  SelectedPatient = new Patient(patientid);
                                  SelectedPatient.Name = "Имя";
                                  break;
                              }
                          }
                          
                      }));
            }
        }
        private RelayCommand _sendEmail;
        public RelayCommand SendEmail
        {
            get
            {
                return _sendEmail ??
                    (_sendEmail = new RelayCommand(obj =>
                      {
                          Patient patient = obj as Patient;
                          if (patient != null)
                          {
                              SendEmailAsync(patient).GetAwaiter();
                          }
                      }));
            }
        }
        private static async Task SendEmailAsync(Patient patient)
        {
            MailAddress from = new MailAddress("healthcaresupbelstu@gmail.com", "Ministry of Health by BelSTU");
            if (!System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
            {
                MessageBox.Show("Отсутствует или ограниченно физическое подключение к сети\nПроверьте настройки вашего сетевого подключения");
            }
            else  if (isValid(patient.Email))
            {
                MailAddress to = new MailAddress(patient.Email);
                MailMessage m = new MailMessage(from, to);
                m.Subject = "Первое посещение";
                m.Body = $"Здравствуйте, {patient.Name} {patient.Patronymic}. Очень рады, что вы выбрали нашу сеть медицинских центров. Для дальнейшего полноценного использования всех " +
                    $"возможностей приложения нашего центра введите свои данные при первом входе в приложение, перейдя по кнопке регистрация и выберите уточнение данных.\nНомер карточки для вас: {patient.patientid}";
                SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
                smtp.Credentials = new NetworkCredential("healthcaresupbelstu@gmail.com", $"{Password.getInstance().myCredential.Password}");
                smtp.EnableSsl = true;
                
                await smtp.SendMailAsync(m);
                MessageBox.Show("Письмо отправлено");
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
        private void Hide()
        {
            _doctorWindow.OnlyID.Visibility = Visibility.Collapsed;
            _doctorWindow.SearchPatient.Visibility = Visibility.Collapsed;
            _doctorWindow.AllInfo.Visibility = Visibility.Collapsed;
            _doctorWindow.Appointment.Visibility = Visibility.Collapsed;
        }
        private RelayCommand _search;
        public RelayCommand Search
        {
            get
            {
                return _search ??
                    (_search = new RelayCommand(obj =>
                      {
                          SqlCommand command = new SqlCommand();
                          command.Connection = SingletonForSqlConnection.SqlConnection;
                          command.CommandText = $"select PATIENTS.Surname, PATIENTS.Name, PATIENTS.Patronymic, PATIENTS.id from PATIENTS where PATIENTS.id={_doctorWindow.PatientID.Text}";
                          using (SqlDataReader reader= command.ExecuteReader())
                          {
                              reader.Read();
                              SelectedPatient = new Patient(reader.GetInt32(3));
                              SelectedPatient.Surname = reader.GetString(0);
                              SelectedPatient.Name = reader.GetString(1);
                              SelectedPatient.Patronymic = reader.GetString(2);
                              _doctorWindow.AllInfo.Visibility = Visibility.Visible;
                              _doctorWindow.OnlyID.Visibility = Visibility.Collapsed;
                              _doctorWindow.SearchPatient.Visibility = Visibility.Collapsed;
                          }
                      }));
            }
        }
        private RelayCommand _appointment;
        public RelayCommand Appointment
        {
            get
            {
                return _appointment ??
                    (_appointment = new RelayCommand(obj =>
                      {
                          Hide();
                          SqlCommand searchDoctors = new SqlCommand();
                          searchDoctors.Connection = SingletonForSqlConnection.SqlConnection;
                          searchDoctors.CommandText = $"select DOCTORS.Surname, " +
                          $"DOCTORS.Name, " +
                          $"DOCTORS.Patronymic, " +
                          $"DOCTORS.Specialty,  " +
                          $"DOCTORS.Direction " +
                          $"from DOCTORS " +
                          $"where DOCTORS.Direction like '%направление%' and " +
                          $"DOCTORS.MedicalInstitutionid = {idofMI}";
                          using(SqlDataReader reader=searchDoctors.ExecuteReader())
                          {
                              if (reader.HasRows)
                              {
                                  while (reader.Read())
                                  {
                                      _doctorWindow.Doctors.Items.Add($"{reader.GetString(0)} {reader.GetString(1)} {reader.GetString(2)}, врач-{reader.GetString(3)}, {reader.GetString(4)}");
                                  }
                              }
                              else
                              {
                                  MessageBox.Show("В данном медучреждении нет зарегистрированных врачей");
                              }
                          }
                          _doctorWindow.Appointment.Visibility = Visibility.Visible;
                      }));
            }
        }
        private RelayCommand _selectDoctor;
        public RelayCommand SelectDoctor
        {
            get
            {
                return _selectDoctor ??
                    (_selectDoctor = new RelayCommand(obj =>
                      {
                          SqlCommand searchDates = new SqlCommand();
                          searchDates.Connection = SingletonForSqlConnection.SqlConnection;
                          string doctor = _doctorWindow.Doctors.Text;
                          string[] doctorinfo = doctor.Split(' ', ',');
                          List<string> docinfo=doctorinfo.ToList();
                          docinfo.RemoveAll(u => u == "");
                          searchDates.CommandText = $"select TIMETABLE.date from TIMETABLE inner join DOCTORS on TIMETABLE.doctorid in " +
                                $"(select DOCTORS.id from DOCTORS " +
                                $"where DOCTORS.Surname = '{docinfo[0]}' " +
                                $"and DOCTORS.Name = '{docinfo[1]}' " +
                                $"and DOCTORS.Patronymic = '{docinfo[2]}' " +
                                $"and DOCTORS.Specialty = '{docinfo[3].Remove(0,5)}' " +
                                $"and DOCTORS.Direction like '%{docinfo[4]}%' " +
                                $"and DOCTORS.MedicalInstitutionid = {idofMI}) " +
                                $"where TIMETABLE.date > SYSDATETIME() " +
                                $"group by TIMETABLE.date";
                          using(SqlDataReader reader=searchDates.ExecuteReader())
                          {
                              if(reader.HasRows)
                              {
                                  while(reader.Read())
                                  {
                                      _doctorWindow.Dates.Items.Add($"{reader.GetDateTime(0).Date}");
                                  }
                              }
                              else
                              {
                                  MessageBox.Show("Расписание для этого врача ещё не сформировано");
                              }
                          }
                          _doctorWindow.SelectDate.Visibility = Visibility.Visible;
                      }));
            }
        }
        public PatientViewModel(GeneralDoctorWindow doctorWindow, int idofMI)
        {
            SelectedPatient = new Patient();
            _doctorWindow = doctorWindow;
            this.idofMI = idofMI;
        }
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
