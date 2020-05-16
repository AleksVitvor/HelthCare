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
        private int _appontmentid;
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
                          using (SqlTransaction transaction = SingletonForSqlConnection.SqlConnection.BeginTransaction())
                          {
                              try
                              {
                                  SqlCommand search = SingletonForSqlConnection.SqlConnection.CreateCommand();
                                  search.Transaction = transaction;
                                  search.CommandText = $"select PATIENTS.Name, " +
                                  $"PATIENTS.Patronymic," +
                                  $"maypatients.id from " +
                                  $"PATIENTS inner join	" +
                                  $"(select TIMETABLE.patientid, TIMETABLE.id		" +
                                  $"from TIMETABLE		" +
                                  $"where TIMETABLE.date='{DateTime.Today}' 		" +
                                  $"and TIMETABLE.patientid={NarrowPatient.ID} 		" +
                                  $"and TIMETABLE.doctorid={doctorid}) maypatients	" +
                                  $"on PATIENTS.id=maypatients.patientid";
                                  using (SqlDataReader reader = search.ExecuteReader())
                                  {
                                      if (reader.HasRows)
                                      {
                                          reader.Read();
                                          _appontmentid = reader.GetInt32(2);
                                          NarrowPatient.Name = reader.GetString(0);
                                          NarrowPatient.Patronymic = reader.GetString(1);
                                          search.CommandText = $"select checked.Name from " +
                                          $"(select COUNT(*) as COUNTER, DISEASES.Name from " +
                                          $"DISEASES inner join " +
                                          $"(select DISEASESSYMPTOMS.diseaseid " +
                                          $"from DISEASESSYMPTOMS inner join " +
                                          $"(select distinct symptomid from PATIENTSANDSYMPTOMS " +
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
                                                  int k = _narrowDoctorWindow.Diagnoses.Items.Count;
                                                  for (int i = 0; i < k - 1; i++)
                                                  {
                                                      _narrowDoctorWindow.Diagnoses.Items.RemoveAt(0);
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
                                          HideSearch();
                                          search.CommandText = $"select distinct SYMPTOMS.Name from SYMPTOMS join " +
                                          $"PATIENTSANDSYMPTOMS on PATIENTSANDSYMPTOMS.symptomid = SYMPTOMS.id " +
                                          $"where PATIENTSANDSYMPTOMS.patientid = {NarrowPatient.ID}";
                                          using(SqlDataReader symptomsreader=search.ExecuteReader())
                                          {
                                              string str = "";
                                              while(symptomsreader.Read())
                                              {
                                                  str += symptomsreader.GetString(0);
                                                  str += ", ";
                                              }
                                              str = str.Remove(str.Length - 2, 2);
                                              _narrowDoctorWindow.Symptoms.Content = str;
                                              
                                          }
                                      }
                                      else
                                      {
                                          MessageBox.Show("У данного пациента нет записи на сегодня");
                                      }
                                  }
                                  transaction.Commit();
                              }
                              catch(Exception ex)
                              {
                                  transaction.Rollback();
                              }
                          }
                          
                      }));
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
                          NarrowPatient = new NarrowPatient();
                          ShowSearch();
                      }));
            }
        }
        private RelayCommand _addDiagnosis;
        public RelayCommand AddDiagnosis
        {
            get
            {
                return _addDiagnosis ??
                    (_addDiagnosis = new RelayCommand(obj =>
                     {
                         using(SqlTransaction transaction=SingletonForSqlConnection.SqlConnection.BeginTransaction())
                         {
                             try
                             {
                                 string updateTimeTable = $"update TIMETABLE set patientid={NarrowPatient.ID} where date='{_narrowDoctorWindow.DateOfAppointment.SelectedDate}' " +
                                  $"and time='{_narrowDoctorWindow.Times.SelectedItem}' and doctorid={doctorid}";
                                 SqlCommand insert = SingletonForSqlConnection.SqlConnection.CreateCommand();
                                 insert.Transaction = transaction;
                                 insert.CommandText = updateTimeTable;
                                 insert.ExecuteNonQuery();
                                 string insertAppointment = $"insert into APPOINTMENT values({_appontmentid})";
                                 insert.CommandText = insertAppointment;
                                 insert.ExecuteNonQuery();
                                 string selectAppointmentid = $"select id from APPOINTMENT where timetableid={_appontmentid}";
                                 insert.CommandText = selectAppointmentid;
                                 int appoinid;
                                 using(SqlDataReader reader=insert.ExecuteReader())
                                 {
                                     reader.Read();
                                     appoinid = reader.GetInt32(0);
                                 }
                                 string insertDiagnosis = $"insert into DIAGNOSES values(@ID, " +
                                 $"{appoinid}, '{DateTime.Now.Date}', 0)";
                                 string selectDiseaseId = "select id from DISEASES where Name=@Disease";
                                 foreach(var i in _narrowDoctorWindow.Diagnoses.SelectedItems)
                                 {
                                     SqlParameter parameter = new SqlParameter("@Disease", i);
                                     insert.Parameters.Add(parameter);
                                     insert.CommandText = selectDiseaseId;
                                     using(SqlDataReader reader=insert.ExecuteReader())
                                     {
                                         reader.Read();
                                         SqlParameter sqlParameter = new SqlParameter("@ID", reader.GetInt32(0));
                                         insert.Parameters.Add(sqlParameter);
                                     }
                                     
                                     insert.CommandText = insertDiagnosis;
                                     insert.ExecuteNonQuery();
                                     insert.Parameters.Clear();
                                 }
                                 transaction.Commit();
                                 MessageBox.Show("Диагнозы успешно добавлены");
                                 ShowSearch();
                             }
                             catch(Exception ex)
                             {
                                 transaction.Rollback();
                             }

                         }
                     }));
            }
        }
        private RelayCommand _selectDate;
        public RelayCommand SelectDate
        {
            get
            {
                return _selectDate ??
                    (_selectDate = new RelayCommand(obj =>
                      {
                          if (_narrowDoctorWindow.DateOfAppointment.SelectedDate > DateTime.Now.Date)
                          {
                              _narrowDoctorWindow.Times.Items.Clear();
                              _narrowDoctorWindow.Times.Visibility = Visibility.Collapsed;
                              string selectTime = $"select TIMETABLE.time from TIMETABLE where TIMETABLE.date='{_narrowDoctorWindow.DateOfAppointment.SelectedDate}' and TIMETABLE.doctorid={doctorid}";
                              SqlCommand searchTime = new SqlCommand(selectTime, SingletonForSqlConnection.SqlConnection);
                              using (SqlDataReader reader = searchTime.ExecuteReader())
                              {
                                  if (reader.HasRows)
                                  {
                                      while (reader.Read())
                                      {
                                          _narrowDoctorWindow.Times.Items.Add(reader.GetValue(0));
                                      }
                                      _narrowDoctorWindow.Times.Visibility = Visibility.Visible;
                                  }
                                  else
                                  {
                                      MessageBox.Show("На выбранную дату нет сформированного расписания");
                                  }
                              }      
                          }
                          else
                          {
                              MessageBox.Show("Проверьте введённые данные");
                          }
                      }));
            }
        }
        private RelayCommand _chooseAppointment;
        public RelayCommand ChooseAppointment
        {
            get
            {
                return _chooseAppointment ??
                    (_chooseAppointment = new RelayCommand(obj =>
                      {
                          NarrowPatient = new NarrowPatient();
                          ShowSelectionForAppointment();
                      }));
            }
        }
        private RelayCommand _closeDiagnosis;
        public RelayCommand CloseDiagnosis
        {
            get
            {
                return _closeDiagnosis ??
                    (_closeDiagnosis = new RelayCommand(obj =>
                      {
                          NarrowPatient = new NarrowPatient();
                          ShowSearchForCloseDiagnosis();
                      }));
            }
        }
        private RelayCommand _chooseDiagnoses;
        public RelayCommand ChooseDiagnoses
        {
            get
            {
                return _chooseDiagnoses ??
                    (_chooseDiagnoses = new RelayCommand(obj =>
                      {
                          string selectDiagnoses = $"select diag.id, DISEASES.Name, diag.Name, diag.Patronymic FROM DISEASES join " +
                        $"(select DIAGNOSES.id, DIAGNOSES.diseaseid, appoint.Name, appoint.Patronymic from DIAGNOSES join " +
                        $"(select APPOINTMENT.id, timetab.Name, timetab.Patronymic from APPOINTMENT join " +
                        $"(select TIMETABLE.id, PATIENTS.Name, PATIENTS.Patronymic from TIMETABLE join PATIENTS " +
                        $"on PATIENTS.id = TIMETABLE.patientid " +
                        $"where PATIENTS.id = {NarrowPatient.ID}) timetab " +
                        $"on timetab.id = APPOINTMENT.timetableid) appoint " +
                        $"on DIAGNOSES.appointmentid = appoint.id " +
                        $"where DIAGNOSES.curedornot=0) diag " +
                        $"on diag.diseaseid = DISEASES.id";
                          SqlCommand search = new SqlCommand(selectDiagnoses, SingletonForSqlConnection.SqlConnection);
                          using(SqlDataReader reader=search.ExecuteReader())
                          {
                              if(reader.HasRows)
                              {
                                  _narrowDoctorWindow.DiagnosedNow.Items.Clear();
                                  while(reader.Read())
                                  {
                                      NarrowPatient.diagnosis.Add(reader.GetInt32(0));
                                      NarrowPatient.Name = reader.GetString(2);
                                      NarrowPatient.Patronymic = reader.GetString(3);
                                      _narrowDoctorWindow.DiagnosedNow.Items.Add(reader.GetValue(1));
                                  }
                                  ShowAlreadyDiagnosed();
                              }
                              else
                              {
                                  MessageBox.Show("У данного пациента нет выставленных незакрытых диагнозов");
                              }
                          }

                      }));
            }
        }
        private RelayCommand _closeOpenDiagnosis;
        public RelayCommand CloseOpenDiagnosis
        {
            get
            {
                return _closeOpenDiagnosis ??
                    (_closeOpenDiagnosis = new RelayCommand(obj =>
                      {
                          string delete = $"delete from PATIENTSANDSYMPTOMS " +
                          $"where PATIENTSANDSYMPTOMS.patientid = {NarrowPatient.ID} and PATIENTSANDSYMPTOMS.symptomid in " +
                          $"(select DISEASESSYMPTOMS.symptomid from DISEASESSYMPTOMS join " +
                          $"DISEASES on DISEASESSYMPTOMS.diseaseid = DISEASES.id " +
                          $"where DISEASES.Name = @Disease)";
                          string finishDiagnosis = $"update DIAGNOSES set DIAGNOSES.curedornot=1, dateofclosing=CONVERT(date, GETDATE()) where DIAGNOSES.id=@Diagnosis";
                          using (SqlTransaction transaction = SingletonForSqlConnection.SqlConnection.BeginTransaction())
                          {
                              try
                              {
                                  SqlCommand deleteSymptoms = SingletonForSqlConnection.SqlConnection.CreateCommand();
                                  deleteSymptoms.CommandText = delete;
                                  deleteSymptoms.Transaction = transaction;
                                  foreach (var i in _narrowDoctorWindow.DiagnosedNow.SelectedItems)
                                  {
                                      SqlParameter parameter = new SqlParameter("@Disease", i);
                                      deleteSymptoms.Parameters.Add(parameter);
                                      deleteSymptoms.ExecuteNonQuery();
                                      deleteSymptoms.Parameters.Clear();
                                  }
                                  deleteSymptoms.CommandText = finishDiagnosis;
                                  foreach(int i in NarrowPatient.diagnosis)
                                  {
                                      SqlParameter parameter = new SqlParameter("Diagnosis", i);
                                      deleteSymptoms.Parameters.Add(parameter);
                                      deleteSymptoms.ExecuteNonQuery();
                                      deleteSymptoms.Parameters.Clear();
                                  }
                                  transaction.Commit();
                                  Hide();
                              }
                              catch (Exception ex)
                              {
                                  transaction.Rollback();
                              }
                          }                          
                      }));
            }
        }
        private void Hide()
        {
            _narrowDoctorWindow.AddDiagnosis.Visibility = Visibility.Collapsed;
            _narrowDoctorWindow.ChooseAppointment.Visibility = Visibility.Collapsed;
            _narrowDoctorWindow.DateOfAppointment.Visibility = Visibility.Collapsed;
            _narrowDoctorWindow.DiagnosisNow.Visibility = Visibility.Collapsed;
            _narrowDoctorWindow.ID.Visibility = Visibility.Collapsed;
            _narrowDoctorWindow.NameAndPatronymicAndDiagnosis.Visibility = Visibility.Collapsed;
            _narrowDoctorWindow.SearchPatient.Visibility = Visibility.Collapsed;
            _narrowDoctorWindow.SearchPatientWithReadyDiagnosis.Visibility = Visibility.Collapsed;
            _narrowDoctorWindow.Times.Visibility = Visibility.Collapsed;
            _narrowDoctorWindow.CloseOpenDiagnosis.Visibility = Visibility.Collapsed;
        }
        private void ShowAlreadyDiagnosed()
        {
            Hide();
            _narrowDoctorWindow.DiagnosisNow.Visibility = Visibility.Visible;
            _narrowDoctorWindow.CloseOpenDiagnosis.Visibility = Visibility.Visible;
        }
        private void ShowSearchForCloseDiagnosis()
        {
            Hide();
            _narrowDoctorWindow.ID.Visibility = Visibility.Visible;
            _narrowDoctorWindow.SearchPatientWithReadyDiagnosis.Visibility = Visibility.Visible;
        }
        private void ShowSelectionForAppointment()
        {
            Hide();
            _narrowDoctorWindow.DateOfAppointment.Visibility = Visibility.Visible;
            _narrowDoctorWindow.AddDiagnosis.Visibility = Visibility.Visible;
        }
        private void ShowSearch()
        {
            Hide();
            _narrowDoctorWindow.ID.Visibility = Visibility.Visible;
            _narrowDoctorWindow.SearchPatient.Visibility = Visibility.Visible;
        }
        private void HideSearch()
        {
            Hide();
            _narrowDoctorWindow.NameAndPatronymicAndDiagnosis.Visibility = Visibility.Visible;
            _narrowDoctorWindow.ChooseAppointment.Visibility = Visibility.Visible;
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
