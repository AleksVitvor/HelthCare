using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Vitvor.HelthCare.AdminWork;

namespace Vitvor.HelthCare
{
    class DoctorViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<UsersOnline> usersOnlines = new ObservableCollection<UsersOnline>();
        private int MedicalInstitutionId;
        private AdminWindow _adminWindow;
        private UsersOnline _online;
        public UsersOnline Online
        {
            get
            {
                return _online;
            }
            set
            {
                _online = value;
                OnPropertyChanged("Online");
            }
        }
        private Doctor _doctor;
        public Doctor SelectedDoctor
        { 
            get
            {
                return _doctor;
            }
            set
            {
                _doctor = value;
                OnPropertyChanged("SelectedDoctor");
            }
        }
        //просмотр работающих
        private RelayCommand _lookOnline;
        public RelayCommand LookOnline
        {
            get
            {
                return _lookOnline ??
                    (_lookOnline = new RelayCommand(obj =>
                     {
                         string selectAll = $"select * from USERSONLINE where " +
                         $"dateOfInfo='{DateTime.Now.Date}' and timeOfExit is null and (idOfMI is null or idOfMI={MedicalInstitutionId})";
                         string selectNameP = $"select Name, Surname from PATIENTS where id=@id";
                         string selectNameD = $"select Name, Surname from DOCTORS where id=@id";
                         SqlCommand selectName = SingletonForSqlConnection.SqlConnection.CreateCommand();
                         SqlCommand select = new SqlCommand(selectAll, SingletonForSqlConnection.SqlConnection);
                         using(SqlDataReader reader=select.ExecuteReader())
                         {
                             if(reader.HasRows)
                             {
                                 while(reader.Read())
                                 {
                                     UsersOnline online = new UsersOnline();
                                     online.Type = reader.GetString(1);
                                     online.TimeOfEnter = reader.GetTimeSpan(3);
                                     online.id = reader.GetInt32(0);
                                     usersOnlines.Add(online);
                                 }
                                 reader.Close();
                                 foreach(var i in usersOnlines)
                                 {
                                     SqlParameter parameter = new SqlParameter("@id", i.id);
                                     selectName.Parameters.Add(parameter);
                                     if (i.Type.Equals("doctor"))
                                     {
                                         selectName.CommandText = selectNameD;
                                         using(SqlDataReader dataReader=selectName.ExecuteReader())
                                         {
                                             dataReader.Read();
                                             i.Name = dataReader.GetString(0);
                                             i.Surname = dataReader.GetString(1);
                                         }
                                     }
                                     else
                                     {
                                         selectName.CommandText = selectNameP;
                                         using (SqlDataReader dataReader = selectName.ExecuteReader())
                                         {
                                             dataReader.Read();
                                             i.Name = dataReader.GetString(0);
                                             i.Surname = dataReader.GetString(1);
                                         }
                                     }
                                     selectName.Parameters.Clear();
                                 }
                                 ShowOnline();
                                 Online = usersOnlines[0];
                             }
                             else
                             {
                                 MessageBox.Show("В сети никого(");
                             }
                         }
                     }));
            }
        }
        private void ShowOnline()
        {
            Hide();
            _adminWindow.Online.Visibility = Visibility.Visible;
        }
        private RelayCommand _forward;
        public RelayCommand Forward
        {
            get
            {
                return _forward ??
                    (_forward = new RelayCommand(obj =>
                      {
                          UsersOnline User = obj as UsersOnline;
                          int i = usersOnlines.IndexOf(User);
                          if (i != usersOnlines.Count - 1)
                          {
                              Online = usersOnlines[i + 1];
                          }
                          else
                          {
                              
                              Online= usersOnlines[0];
                          }
                      }));
            }
        }
        private RelayCommand _back;
        public RelayCommand Back
        {
            get
            {
                return _back ??
                    (_back = new RelayCommand(obj =>
                      {
                          UsersOnline User = obj as UsersOnline;
                          int i = usersOnlines.IndexOf(User);
                          if (i != 0)
                          {
                              Online = usersOnlines[i - 1];
                          }
                          else
                          {
                              Online = usersOnlines[usersOnlines.Count - 1];
                          }
                      }));
            }
        }
        //остальные команды
        private RelayCommand _addDoctor;
        public RelayCommand AddDoctor
        {
            get
            {
                return _addDoctor ??
                    (_addDoctor = new RelayCommand(obj =>
                      {
                          Hide();
                          SelectedDoctor = new Doctor(MedicalInstitutionId);
                          Show();
                          _adminWindow.ConfirmAdd.Visibility = Visibility.Visible;
                      }));
            }
        }
        private void Show()
        {
            _adminWindow.BaseDescription.Visibility = Visibility.Visible;
            _adminWindow.PasswordDescription.Visibility = Visibility.Visible;
            _adminWindow.DoctorUsername.Visibility = Visibility.Visible;
            _adminWindow.BackToMenu.Visibility = Visibility.Visible;
        }
        private RelayCommand _changeDoctor;
        public RelayCommand ChangeDoctor
        {
            get
            {
                return _changeDoctor ??
                    (_changeDoctor = new RelayCommand(obj =>
                      {
                          Hide();
                          SelectedDoctor = new Doctor(MedicalInstitutionId);
                          _adminWindow.BackToMenu.Visibility = Visibility.Visible;
                          _adminWindow.DoctorUsername.Visibility = Visibility.Visible;
                          _adminWindow.ConfirmChange.Visibility = Visibility.Visible;
                      }));
            }
        }

        private RelayCommand _deleteDoctor;
        public RelayCommand DeleteDoctor
        {
            get
            {
                return _deleteDoctor ??
                    (_deleteDoctor = new RelayCommand(obj =>
                      {
                          Hide();
                          SelectedDoctor = new Doctor(MedicalInstitutionId);
                          _adminWindow.BackToMenu.Visibility = Visibility.Visible;
                          _adminWindow.DoctorUsername.Visibility = Visibility.Visible;
                          _adminWindow.ConfirmDelete.Visibility = Visibility.Visible;
                      }));
            }
        }
        private RelayCommand _backToMenu;
        public RelayCommand BackToMenu
        {
            get
            {
                return _backToMenu ??
                    (_backToMenu = new RelayCommand(obj =>
                      {
                          Hide();
                      }));
            }
        }
        private RelayCommand _confirmAddDoctor;
        public RelayCommand ConfirmAddDoctor
        {
            get
            {
                return _confirmAddDoctor ??
                    (_confirmAddDoctor = new RelayCommand(obj =>
                      {
                          SqlCommand command = new SqlCommand();
                          command.Connection = SingletonForSqlConnection.SqlConnection;
                          SelectedDoctorFinish();
                          command.CommandText = $"select DOCTORS.id from DOCTORS where DOCTORS.Username='{SelectedDoctor.Username}'";
                          using (SqlDataReader reader = command.ExecuteReader())
                          {
                              if (reader.HasRows)
                              {
                                  MessageBox.Show("Данное имя пользователя занято");
                              }
                              else
                              {
                                  reader.Close();
                                  command.CommandText = $"insert into DOCTORS values ({SelectedDoctor.MIid}," +
                                  $"'{SelectedDoctor.Surname}','{SelectedDoctor.Name}','{SelectedDoctor.Patronymic}'," +
                                  $"'{SelectedDoctor.DateOfBirth}','{SelectedDoctor.Specialty}', '{SelectedDoctor._direction}'," +
                                  $"'{SelectedDoctor.Username}','{SelectedDoctor.Password}', '{SelectedDoctor.PhoneNumber}')";
                                  command.ExecuteNonQuery();
                              }
                          }
                          Hide();
                      }));
            }
        }
        private RelayCommand _confirmChangeDoctor;
        public RelayCommand ConfirmChangeDoctor
        {
            get
            {
                return _confirmChangeDoctor ??
                    (_confirmChangeDoctor = new RelayCommand(obj =>
                      {
                          SqlCommand command = new SqlCommand();
                          command.Connection = SingletonForSqlConnection.SqlConnection;
                          command.CommandText = $"select * from DOCTORS where DOCTORS.Username='{SelectedDoctor.Username}'";
                          using(SqlDataReader reader=command.ExecuteReader())
                          {
                              if(reader.HasRows)
                              {
                                  reader.Read();
                                  SelectedDoctor = new Doctor(MedicalInstitutionId, true, reader.GetString(2), reader.GetString(3), reader.GetString(4), reader.GetString(6),
                                      reader.GetString(10), reader.GetString(8));
                                  _adminWindow.BaseDescription.Visibility = Visibility.Visible;
                                  _adminWindow.date.Visibility = Visibility.Collapsed;
                                  _adminWindow.direction.Visibility = Visibility.Collapsed;
                                  _adminWindow.ConfirmChange.Visibility = Visibility.Collapsed;
                              }
                              else
                              {
                                  MessageBox.Show("Попытка изменения несуществующей учётной записи");
                              }
                          }
                          Hide();
                      }));
            }
        }
        private RelayCommand _confirmDeleteDoctor;
        public RelayCommand ConfirmDeleteDoctor
        {
            get
            {
                return _confirmDeleteDoctor ??
                    (_confirmDeleteDoctor = new RelayCommand(obj =>
                      {
                          SqlCommand command = new SqlCommand();
                          command.Connection = SingletonForSqlConnection.SqlConnection;
                          command.CommandText= $"select * from DOCTORS where DOCTORS.Username='{SelectedDoctor.Username}'";
                          using (SqlDataReader reader = command.ExecuteReader())
                          {
                              if (reader.HasRows)
                              {
                                  reader.Close();
                                  command.CommandText = $"delete from DOCTORS where DOCTORS.Username='{SelectedDoctor.Username}'";
                                  command.ExecuteNonQuery();
                              }
                              else
                              {
                                  MessageBox.Show("Попытка удаления несуществующей учётной записи");
                              }
                          }
                          Hide();
                      }));
            }
        }
        private void SelectedDoctorFinish()
        {
            SelectedDoctor.Password = _adminWindow.PasswordBox.Password;
            if ((bool)_adminWindow.GeneralDirection.IsChecked)
            {
                SelectedDoctor._direction = _adminWindow.GeneralDirection.Content.ToString();
            }
            else if ((bool)_adminWindow.NarrowDirection.IsChecked)
            {
                SelectedDoctor._direction = _adminWindow.NarrowDirection.Content.ToString();
            }
            else if((bool)_adminWindow.Nurse.IsChecked)
            {
                SelectedDoctor._direction = _adminWindow.Nurse.Content.ToString();
            }
        }
        private RelayCommand _createTimeTable;
        public RelayCommand CreateTimeTable
        {
            get
            {
                return _createTimeTable ??
                    (_createTimeTable = new RelayCommand(obj =>
                      {
                          Hide();
                          SqlCommand searchDoctors = new SqlCommand();
                          searchDoctors.Connection = SingletonForSqlConnection.SqlConnection;
                          searchDoctors.CommandText = $"select DOCTORS.Surname, " +
                          $"DOCTORS.Name, " +
                          $"DOCTORS.Patronymic " +
                          $"from DOCTORS inner join ADMINS " +
                          $"on DOCTORS.MedicalInstitutionid = ADMINS.id " +
                          $"	where ADMINS.id=1 and " +
                          $"(DOCTORS.Direction = 'Узкое направление' " +
                          $"or DOCTORS.Direction = 'Общее направление')";
                          _adminWindow.Doctors.Items.Clear();
                          using(SqlDataReader reader=searchDoctors.ExecuteReader())
                          {
                              while(reader.Read())
                              {
                                  _adminWindow.Doctors.Items.Add($"{reader.GetString(0)} {reader.GetString(1)} {reader.GetString(2)}");
                              }
                          }
                          _adminWindow.Timetable.Visibility = Visibility.Visible;
                          _adminWindow.Create.Visibility = Visibility.Visible;
                          MessageBox.Show(Convert.ToString(_adminWindow.SelectedDate.IsEnabled));
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
                          CreateTimetable();
                          _adminWindow.SelectedTime.SelectedIndex = -1;
                          _adminWindow.EvenOrNot.SelectedIndex = -1;
                          _adminWindow.Doctors.UnSelectAll();
                          Hide();
                      }));
            }
        }
        private void CreateTimetable()
        {
            SqlCommand searchDoctors = new SqlCommand();
            searchDoctors.Connection = SingletonForSqlConnection.SqlConnection;
            string[] FIO;
            foreach (var i in _adminWindow.Doctors.SelectedItems)
            {
                FIO = i.ToString().Split(' ');
                searchDoctors.CommandText = $"select DOCTORS.id " +
                $"from DOCTORS " +
                $"where DOCTORS.Surname='{FIO[0]}' and " +
                $"DOCTORS.Name='{FIO[1]}' and " +
                $"DOCTORS.Patronymic='{FIO[2]}'";
                int id = 0;
                using (SqlDataReader reader = searchDoctors.ExecuteReader())
                {
                    reader.Read();
                    id = reader.GetInt32(0);
                }
                SqlCommand searchLastDate = new SqlCommand();
                searchLastDate.Connection = SingletonForSqlConnection.SqlConnection;
                searchLastDate.CommandText = $"select top(1) TIMETABLE.date from TIMETABLE where TIMETABLE.doctorid={id} order by TIMETABLE.date desc";
                DateTime date = new DateTime();
                using (SqlDataReader reader1 = searchLastDate.ExecuteReader())
                {
                    if (reader1.HasRows)
                    {
                        reader1.Read();
                        date = reader1.GetDateTime(0);
                    }
                    else
                    {
                        date = DateTime.Today;
                    }
                }
                for (DateTime dateTime = date; dateTime < _adminWindow.SelectedDate.SelectedDate; dateTime = dateTime.AddDays(1))
                {
                    if (dateTime.DayOfWeek == DayOfWeek.Saturday || dateTime.DayOfWeek == DayOfWeek.Sunday)
                    {
                        continue;
                    }
                    DateTime endtime = new DateTime();
                    string[] times = _adminWindow.SelectedTime.SelectedItem.ToString().Split('-');
                    times[0] = times[0].Remove(0, 39).Trim('(', ')');
                    times[1] = times[1].Trim('(', ')');
                    SqlCommand addintoTimetable = new SqlCommand();
                    addintoTimetable.Connection = SingletonForSqlConnection.SqlConnection;
                    if ((dateTime.Date.Day % 2 == 0 && _adminWindow.EvenOrNot.SelectedIndex == 1) || (dateTime.Date.Day % 2 == 1 && _adminWindow.EvenOrNot.SelectedIndex == 0))
                    {
                        endtime = Convert.ToDateTime(times[1]);
                        for (DateTime starttime = Convert.ToDateTime(times[0]); starttime < endtime; starttime = starttime.AddMinutes(15))
                        {
                            addintoTimetable.CommandText = $"insert into TIMETABLE values({id}, '{starttime}', '{dateTime}', NULL)";
                            addintoTimetable.ExecuteNonQuery();
                        }
                    }
                    else if ((dateTime.Date.Day % 2 == 0 && _adminWindow.EvenOrNot.SelectedIndex == 0) || (dateTime.Date.Day % 2 == 1 && _adminWindow.EvenOrNot.SelectedIndex == 1))
                    {
                        int save = _adminWindow.SelectedTime.SelectedIndex;
                        if (_adminWindow.SelectedTime.SelectedIndex == 0)
                            _adminWindow.SelectedTime.SelectedIndex = 1;
                        else if (_adminWindow.SelectedTime.SelectedIndex == 1)
                            _adminWindow.SelectedTime.SelectedIndex = 0;
                        string[] times1 = _adminWindow.SelectedTime.SelectedItem.ToString().Split('-');
                        times1[0] = times1[0].Remove(0, 39).Trim('(', ')');
                        times1[1] = times1[1].Trim('(', ')');
                        endtime = Convert.ToDateTime(times1[1]);
                        for (DateTime starttime = Convert.ToDateTime(times1[0]); starttime < endtime; starttime = starttime.AddMinutes(15))
                        {
                            addintoTimetable.CommandText = $"insert into TIMETABLE values({id}, '{starttime}', '{dateTime}', NULL)";
                            addintoTimetable.ExecuteNonQuery();
                        }
                        _adminWindow.SelectedTime.SelectedIndex = save;
                    }

                }
            }
        }
        private void Hide()
        {
            _adminWindow.Online.Visibility = Visibility.Collapsed;
            _adminWindow.BaseDescription.Visibility = Visibility.Collapsed;
            _adminWindow.PasswordDescription.Visibility = Visibility.Collapsed;
            _adminWindow.DoctorUsername.Visibility = Visibility.Collapsed;
            _adminWindow.BackToMenu.Visibility = Visibility.Collapsed;
            _adminWindow.ConfirmAdd.Visibility = Visibility.Collapsed;
            _adminWindow.ConfirmChange.Visibility = Visibility.Collapsed;
            _adminWindow.ConfirmDelete.Visibility = Visibility.Collapsed;
            _adminWindow.Timetable.Visibility = Visibility.Collapsed;
            _adminWindow.Create.Visibility = Visibility.Collapsed;
            _adminWindow.PasswordBox.Clear();
        }
        public DoctorViewModel(AdminWindow adminWindow, int MIid)
        {
            _adminWindow = adminWindow;
            MedicalInstitutionId = MIid;
        }
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
