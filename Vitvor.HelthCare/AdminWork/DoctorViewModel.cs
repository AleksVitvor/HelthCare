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
    class DoctorViewModel : INotifyPropertyChanged
    {
        private int MedicalInstitutionId;
        private AdminWindow _adminWindow;
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
        private RelayCommand _addDoctor;
        public RelayCommand AddDoctor
        {
            get
            {
                return _addDoctor ??
                    (_addDoctor = new RelayCommand(obj =>
                      {
                          SelectedDoctor = new Doctor(MedicalInstitutionId);
                          Show();
                          _adminWindow.ConfirmAdd.Visibility = Visibility.Visible;
                      }));
            }
        }
        private RelayCommand _changeDoctor;
        public RelayCommand ChangeDoctor
        {
            get
            {
                return _changeDoctor ??
                    (_changeDoctor = new RelayCommand(obj =>
                      {
                          SelectedDoctor = new Doctor(MedicalInstitutionId);
                          _adminWindow.BackToMenu.Visibility = Visibility.Visible;
                          _adminWindow.DoctorUsername.Visibility = Visibility.Visible;
                          _adminWindow.ConfirmChange.Visibility = Visibility.Visible;
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
        private RelayCommand _deleteDoctor;
        public RelayCommand DeleteDoctor
        {
            get
            {
                return _deleteDoctor ??
                    (_deleteDoctor = new RelayCommand(obj =>
                      {
                          SelectedDoctor = new Doctor(MedicalInstitutionId);
                          _adminWindow.BackToMenu.Visibility = Visibility.Visible;
                          _adminWindow.DoctorUsername.Visibility = Visibility.Visible;
                          _adminWindow.ConfirmDelete.Visibility = Visibility.Visible;
                      }));
            }
        }
        private RelayCommand _lookAllWorkDoctors;
        public RelayCommand LookAllWorkDoctors
        {
            get
            {
                return _lookAllWorkDoctors ??
                    (_lookAllWorkDoctors = new RelayCommand(obj =>
                      {

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
                          SelestedDoctorFinish();
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
                                  $"'{SelectedDoctor._dateOfBirth}','{SelectedDoctor.Specialty}', '{SelectedDoctor._direction}'," +
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
        private void SelestedDoctorFinish()
        {
            SelectedDoctor.Password = _adminWindow.PasswordBox.Password;
            SelectedDoctor._dateOfBirth = (DateTime)_adminWindow.DateOfBirth.SelectedDate;
            if ((bool)_adminWindow.GeneralDirection.IsChecked)
            {
                SelectedDoctor._direction = _adminWindow.GeneralDirection.Content.ToString();
            }
            else if ((bool)_adminWindow.NarrowDirection.IsChecked)
            {
                SelectedDoctor._direction = _adminWindow.NarrowDirection.Content.ToString();
            }
        }
        private void Hide()
        {
            _adminWindow.BaseDescription.Visibility = Visibility.Collapsed;
            _adminWindow.PasswordDescription.Visibility = Visibility.Collapsed;
            _adminWindow.DoctorUsername.Visibility = Visibility.Collapsed;
            _adminWindow.BackToMenu.Visibility = Visibility.Collapsed;
            _adminWindow.ConfirmAdd.Visibility = Visibility.Collapsed;
            _adminWindow.ConfirmChange.Visibility = Visibility.Collapsed;
            _adminWindow.ConfirmDelete.Visibility = Visibility.Collapsed;
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
