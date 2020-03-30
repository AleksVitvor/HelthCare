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
                          while (true)
                          {
                              MessageBoxResult result = MessageBox.Show("Первый приём?",
                                  "Сообщение",
                                  MessageBoxButton.YesNo,
                                  MessageBoxImage.Question,
                                  MessageBoxResult.Yes,
                                  MessageBoxOptions.DefaultDesktopOnly);
                              if (result == MessageBoxResult.Yes)
                              {
                                  _doctorWindow.OnlyID.Visibility = Visibility.Visible;
                                  _doctorWindow.SearchPatient.Visibility = Visibility.Visible;
                                  break;
                              }
                              else if (result == MessageBoxResult.No)
                              {
                                  _doctorWindow.AllInfo.Visibility = Visibility.Visible;
                                  break;
                              }
                          }
                          
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
                          SqlCommand command = new SqlCommand();
                          command.Connection = SingletonForSqlConnection.SqlConnection;
                          command.CommandText = $"select PATIENTS.Surname, PATIENTS.Name, PATIENTS.Patronymic from PATIENTS where PATIENTS.id={_doctorWindow.PatientID.Text}";
                          using (SqlDataReader reader= command.ExecuteReader())
                          {
                              reader.Read();
                              SelectedPatient = new Patient();
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
