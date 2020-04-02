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
    class PatientViewModel : INotifyPropertyChanged
    {
        private GeneralDoctorWindow _doctorWindow;
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
                                  MessageBox.Show($"Номер карточки для входа: {patientid}");
                                  break;
                              }
                          }
                          
                      }));
            }
        }
        private void Hide()
        {
            _doctorWindow.OnlyID.Visibility = Visibility.Collapsed;
            _doctorWindow.SearchPatient.Visibility = Visibility.Collapsed;
            _doctorWindow.AllInfo.Visibility = Visibility.Collapsed;
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
        public PatientViewModel(GeneralDoctorWindow doctorWindow)
        {
            SelectedPatient = new Patient();
            _doctorWindow = doctorWindow;
        }
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
