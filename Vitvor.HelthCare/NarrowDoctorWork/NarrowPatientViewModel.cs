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
    class NarrowPatientViewModel : INotifyPropertyChanged
    {
        private int doctorid;
        private NarrowDoctorWindow _narrowDoctorWindow;
        private NarrowPatient _narrowPatient;
        public NarrowPatient NarrowPatient
        {
            get
            {
                return _narrowPatient;
            }
            set
            {
                _narrowPatient = value;
                OnPropertyChanged("NarrowPatient");
                _narrowDoctorWindow.Diagnoses.Items.Insert(0,$"{/*s*/_narrowPatient.ID}");
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
                          SqlCommand search = new SqlCommand();
                          search.Connection = SingletonForSqlConnection.SqlConnection;
                          search.CommandText = $"select PATIENTS.Name, " +
                          $"PATIENTS.Patronymic from " +
                          $"PATIENTS inner join	" +
                          $"(select TIMETABLE.patientid		" +
                          $"from TIMETABLE		" +
                          $"where TIMETABLE.date='{DateTime.Today}' 		" +
                          $"and TIMETABLE.patientid={NarrowPatient.ID} 		" +
                          $"and TIMETABLE.doctorid={doctorid}) maypatients	" +
                          $"on PATIENTS.id=maypatients.patientid";
                          using(SqlDataReader reader=search.ExecuteReader())
                          {
                              if(reader.HasRows)
                              {
                                  reader.Read();
                                  NarrowPatient.Name = reader.GetString(0);
                                  NarrowPatient.Patronymic = reader.GetString(1);
                                  _narrowDoctorWindow.ID.Visibility = Visibility.Collapsed;
                                  _narrowDoctorWindow.NameAndPatronymicAndDiagnosis.Visibility = Visibility.Visible;
                                  search.CommandText = $"select checked.Name from " +
                                  $"(select COUNT(*) as COUNTER, DISEASES.Name from " +
                                  $"DISEASES inner join " +
                                  $"(select DISEASESSYMPTOMS.diseaseid " +
                                  $"from DISEASESSYMPTOMS inner join " +
                                  $"(select symptomid from PATIENTSANDSYMPTOMS " +
                                  $"where PATIENTSANDSYMPTOMS.patientid = {NarrowPatient.ID}) sympt " +
                                  $"on DISEASESSYMPTOMS.symptomid = sympt.symptomid) des " +
                                  $"on DISEASES.id = des.diseaseid " +
                                  $"group by DISEASES.Name) checked " +
                                  $"where checked.COUNTER >= 2";
                                  reader.Close();
                                  using (SqlDataReader searchDiseasis = search.ExecuteReader())
                                  {
                                      if (searchDiseasis.HasRows)
                                      {
                                          for (int i = 0; i < _narrowDoctorWindow.Diagnoses.Items.Count - 1; i++)
                                          {
                                              _narrowDoctorWindow.Diagnoses.Items.RemoveAt(i);
                                          }
                                          while (searchDiseasis.Read())
                                          {
                                              _narrowDoctorWindow.Diagnoses.Items.Insert(0, searchDiseasis.GetString(0));
                                          }
                                      }
                                      else
                                      {
                                          MessageBox.Show("Данных в базе недостаточно для выставления возможных диагнозов");
                                      }                                   
                                  }
                                  //дописать запрос на симптомы и имя-фамилия пациента
                                  HideSearch();
                              }
                              else
                              {
                                  MessageBox.Show("У данного пациента нет записи на сегодня");
                              }
                          }
                          
                      }));
            }
        }
        private void HideSearch()
        {
            _narrowDoctorWindow.SearchPatient.Visibility = Visibility.Collapsed;
            _narrowDoctorWindow.AddDiagnosis.Visibility = Visibility.Visible;
        }

        public NarrowPatientViewModel(NarrowDoctorWindow narrowDoctorWindow, int doctorid)
        {
            _narrowDoctorWindow = narrowDoctorWindow;
            NarrowPatient = new NarrowPatient();
            this.doctorid = doctorid;
        }
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
