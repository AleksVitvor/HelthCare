using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace Vitvor.HelthCare.UserWork
{
    class DiseasForUserViewModel:INotifyPropertyChanged
    {
        private PatientWindow _patientWindow;
        private int _userid;
        private List<DiseaseForUser> diseaseForUsers = new List<DiseaseForUser>();
        private ObservableCollection<TimeSpan> times = new ObservableCollection<TimeSpan>();
        private Dictionary<string, int> map = new Dictionary<string, int>();
        private DiseaseForUser _disease;
        public DiseaseForUser Disease
        {
            get
            {
                return _disease;
            }
            set
            {
                _disease = value;
                OnPropertyChanged("Disease");
            }
        }
        private RelayCommand _lookDiagnosises;
        public RelayCommand LookDiagnosises
        {
            get
            {
                return _lookDiagnosises ??
                    (_lookDiagnosises = new RelayCommand(obj =>
                      {
                          SqlCommand search = SingletonForSqlConnection.SqlConnection.CreateCommand();
                          search.CommandText = $"select DISEASES.Name, diag.dateOfDiagnosis, diag.dateofclosing FROM DISEASES join " +
                        $"(select DIAGNOSES.id, DIAGNOSES.diseaseid, DIAGNOSES.dateOfDiagnosis, DIAGNOSES.dateofclosing from DIAGNOSES join " +
                        $"(select APPOINTMENT.id from APPOINTMENT join " +
                        $"(select TIMETABLE.id from TIMETABLE join PATIENTS " +
                        $"on PATIENTS.id = TIMETABLE.patientid " +
                        $"where PATIENTS.id = {_userid}) timetab " +
                        $"on timetab.id = APPOINTMENT.timetableid) appoint " +
                        $"on DIAGNOSES.appointmentid = appoint.id) diag " +
                        $"on diag.diseaseid = DISEASES.id";
                          using(SqlDataReader reader=search.ExecuteReader())
                          {
                              if (reader.HasRows)
                              {
                                  diseaseForUsers.Clear();
                                  while (reader.Read())
                                  {
                                      DiseaseForUser disease = new DiseaseForUser();
                                      disease.DiseaseName = reader.GetString(0);
                                      disease.DateOfDiagnosis = Convert.ToString(reader.GetDateTime(1).Date);
                                      disease.DateOfClosing = Convert.ToString(reader.GetDateTime(2).Date);
                                      disease.DateOfDiagnosis = disease.DateOfDiagnosis.Replace(" 0:00:00", "");
                                      disease.DateOfClosing = disease.DateOfClosing.Replace(" 0:00:00", "");
                                      diseaseForUsers.Add(disease);
                                  }
                                  ShowDiagnosises();
                                  Disease = diseaseForUsers[0];
                              }
                              else
                                  MessageBox.Show("У вас на данный момент нет диагнозов");
                          }
                      }));
            }
        }
        private RelayCommand _nextDiagnosis;
        public RelayCommand NextDiagnosis
        {
            get
            {
                return _nextDiagnosis ??
                    (_nextDiagnosis = new RelayCommand(obj =>
                      {
                          DiseaseForUser diseaseForUser = obj as DiseaseForUser;
                          int i = diseaseForUsers.IndexOf(diseaseForUser);
                          if (i != diseaseForUsers.Count - 1)
                          {
                              Disease = diseaseForUsers[i + 1];
                          }
                          else
                          {
                              Disease = diseaseForUsers[0];
                          }
                      }));
            }
        }
        private RelayCommand _previousDiagnosis;
        public RelayCommand PreviousDiagnosis
        {
            get
            {
                return _previousDiagnosis ??
                    (_previousDiagnosis = new RelayCommand(obj =>
                    {
                        DiseaseForUser diseaseForUser = obj as DiseaseForUser;
                        int i = diseaseForUsers.IndexOf(diseaseForUser);
                        if (i != 0)
                        {
                            Disease = diseaseForUsers[i - 1];
                        }
                        else
                        {
                            Disease = diseaseForUsers[diseaseForUsers.Count-1];
                        }
                    }));
            }
        }
        private RelayCommand _showAndFindDoctors;
        public RelayCommand ShowAndFindDoctors
        {
            get
            {
                return _showAndFindDoctors ??
                    (_showAndFindDoctors = new RelayCommand(obj =>
                      {
                          SqlCommand searchDoctors = new SqlCommand();
                          searchDoctors.Connection = SingletonForSqlConnection.SqlConnection;
                          searchDoctors.CommandText = $"select DOCTORS.Surname, " +
                          $"DOCTORS.Name, " +
                          $"DOCTORS.Patronymic, " +
                          $"DOCTORS.Specialty,  " +
                          $"DOCTORS.Direction, " +
                          $"DOCTORS.id " +
                          $"from DOCTORS " +
                          $"where DOCTORS.Direction like '%бщее направление%'";
                          using (SqlDataReader reader = searchDoctors.ExecuteReader())
                          {
                              if (reader.HasRows)
                              {
                                  _patientWindow.Doctors.Items.Clear();
                                  map.Clear();
                                  while (reader.Read())
                                  {
                                      _patientWindow.Doctors.Items.Add($"{reader.GetString(0)} {reader.GetString(1)} {reader.GetString(2)}, врач-{reader.GetString(3).ToLower()}, {reader.GetString(4)}");
                                      map.Add($"{reader.GetString(0)} {reader.GetString(1)} {reader.GetString(2)}, врач-{reader.GetString(3).ToLower()}, {reader.GetString(4)}", reader.GetInt32(5));
                                  }
                                  ShowAppointment();
                              }
                              else
                              {
                                  MessageBox.Show("Ни в одном медучреждении нет зарегистрированных врачей");
                              }
                          }
                      }));
            }
        }
        private RelayCommand _doctorIsSelected;
        public RelayCommand DoctorIsSelected
        {
            get
            {
                return _doctorIsSelected ??
                    (_doctorIsSelected = new RelayCommand(obj =>
                     {
                         ShowCalendar();
                     }));
            }
        }
        private RelayCommand _dateIsSelected;
        public RelayCommand DateIsSelected
        {
            get
            {
                return _dateIsSelected ??
                    (_dateIsSelected = new RelayCommand(obj =>
                      {
                          int docid;
                          map.TryGetValue($"{_patientWindow.Doctors.SelectedItem}", out docid);
                          string selectTime = $"select TIMETABLE.time " +
                          $"from TIMETABLE join DOCTORS " +
                          $"on TIMETABLE.doctorid=DOCTORS.id " +
                          $"where DOCTORS.id={docid} " +
                          $"and TIMETABLE.date='{_patientWindow.Appointmentdate.SelectedDate}' " +
                          $"and  TIMETABLE.patientid is null";
                          SqlCommand command = new SqlCommand(selectTime, SingletonForSqlConnection.SqlConnection);
                          using (SqlDataReader reader = command.ExecuteReader())
                          {
                              _patientWindow.Time.Visibility = Visibility.Collapsed;
                              times.Clear();
                              if (reader.HasRows)
                              {
                                  times.Clear();
                                  while (reader.Read())
                                  {
                                      times.Add(reader.GetTimeSpan(0));
                                  }
                                  _patientWindow.Time.ItemsSource = times;
                                  _patientWindow.Time.Visibility = Visibility.Visible;
                              }
                              else
                              {
                                  MessageBox.Show("Расписание для врача на данный день не сформировано");
                              }
                          }
                      }));
            }
        }
        private RelayCommand _create;
        public RelayCommand Create
        {
            get
            {
                return _create ??
                    (_create = new RelayCommand(obj =>
                      {
                          try
                          {
                              int doctorid;
                              map.TryGetValue($"{_patientWindow.Doctors.SelectedItem}", out doctorid);
                              string update = $"update TIMETABLE set " +
                              $"TIMETABLE.patientid={_userid} where " +
                              $"time='{_patientWindow.Time.SelectedItem}' " +
                              $"and date='{_patientWindow.Appointmentdate.SelectedDate}' " +
                              $"and doctorid={doctorid}";
                              SqlCommand command = new SqlCommand(update, SingletonForSqlConnection.SqlConnection);
                              command.ExecuteNonQuery();
                              string sub = "Запись на приём";
                              string part = $"{_patientWindow.Doctors.SelectedItem}, дата {_patientWindow.Appointmentdate.SelectedDate.ToString().Replace(" 0:00:00","")}, время {_patientWindow.Time.SelectedItem.ToString().Remove(5,3)}.";
                              SendEmailAsync(_userid, sub, part).GetAwaiter();
                              Hide();
                          }
                          catch(Exception ex)
                          {
                              MessageBox.Show("Попытка записи не удалась");
                          }
                      }));
            }
        }
        private static async Task SendEmailAsync(int patid, string sub, string partAboutAppointment)
        {
            string find = $"select * from PATIENTS where id={patid}";
            SqlCommand command = new SqlCommand(find, SingletonForSqlConnection.SqlConnection);
            using (SqlDataReader reader = command.ExecuteReader())
            {
                reader.Read();
                string message = $"Уважаемый(-ая) {reader.GetString(2)} {reader.GetString(3)} вы записаны к врачу " + partAboutAppointment + " Благодарим вас за запись.";
                MailAddress from = new MailAddress("healthcaresupbelstu@gmail.com", "Ministry of Health by BelSTU");
                if (!System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
                {
                    MessageBox.Show("Отсутствует или ограниченно физическое подключение к сети\nПроверьте настройки вашего сетевого подключения");
                }
                else if (isValid(reader.GetString(6)))
                {
                    MailAddress to = new MailAddress(reader.GetString(6));
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
                    MessageBox.Show("Ваша запись была сформирована, отправка уведомления не удалась");
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
        private void Hide()
        {
            _patientWindow.Diagnosises.Visibility = Visibility.Collapsed;
            _patientWindow.Appointment.Visibility = Visibility.Collapsed;
        }
        private void HideParts()
        {
            _patientWindow.Appointmentdate.Visibility = Visibility.Collapsed;
            _patientWindow.Time.Visibility = Visibility.Collapsed;
        }
        private void ShowDiagnosises()
        {
            Hide();
            _patientWindow.Diagnosises.Visibility = Visibility.Visible;
        }
        private void ShowAppointment()
        {
            Hide();
            HideParts();
            _patientWindow.Appointment.Visibility = Visibility.Visible;
        }
        private void ShowCalendar()
        {
            _patientWindow.Appointmentdate.Visibility = Visibility.Visible;
        }
        public DiseasForUserViewModel(PatientWindow patientWindow, int userid)
        {
            _userid = userid;
            _patientWindow = patientWindow;
        }
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
