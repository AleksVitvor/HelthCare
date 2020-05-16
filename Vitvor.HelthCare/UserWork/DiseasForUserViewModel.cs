using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Vitvor.HelthCare.UserWork
{
    class DiseasForUserViewModel:INotifyPropertyChanged
    {
        private PatientWindow _patientWindow;
        private int _userid;
        private List<DiseaseForUser> diseaseForUsers = new List<DiseaseForUser>();
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
        private void Hide()
        {
            _patientWindow.Diagnosises.Visibility = Visibility.Collapsed;
        }
        private void ShowDiagnosises()
        {
            Hide();
            _patientWindow.Diagnosises.Visibility = Visibility.Visible;
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
