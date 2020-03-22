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
    class DiseaseViewModel : INotifyPropertyChanged
    {
        public MainAdminWindow MainAdminWindow { get; set; }
        private Disease _disease;
        public Disease SelectedDisease
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
        private RelayCommand _changeDisease;
        public RelayCommand ChangeDisease
        {
            get
            {
                return _changeDisease ??
                    (_changeDisease = new RelayCommand(obj =>
                      {

                      }));
            }
        }
        private RelayCommand _addDisease;
        public RelayCommand AddDisease
        {
            get
            {
                return _addDisease ??
                    (_addDisease = new RelayCommand(obj =>
                      {
                          SqlCommand command = new SqlCommand();
                          command.Connection = SingletonForSqlConnection.SqlConnection;
                          command.CommandText = $"select * from DISEASES where DISEASES.Name='{SelectedDisease.DiseaseName.ToLower()}'";
                          SqlDataReader reader = command.ExecuteReader();
                          if (reader.HasRows)
                          {
                              MessageBox.Show("Попытка добавления уже существующей болезни");
                              Hide();
                          }
                          else
                          {
                              command.CommandText = $"insert into DISEASES values ('{SelectedDisease.DiseaseName.ToLower()}')";
                              reader.Close();
                              command.ExecuteNonQuery();
                              command.CommandText = $"select DISEASES.id from DISEASES where DISEASES.Name='{SelectedDisease.DiseaseName.ToLower()}'";
                              reader.Close();
                              reader = command.ExecuteReader();
                              reader.Read();
                              int diseaseid = reader.GetInt32(0);
                              foreach (string i in SelectedDisease.Symptoms.Split(','))
                              {
                                  i.TrimStart(' ');
                                  command.CommandText = $"select * from SYMPTOMS where SYMPTOMS.Name='{i.ToLower()}'";
                                  reader.Close();
                                  reader = command.ExecuteReader();
                                  if (reader.HasRows)
                                  {
                                      reader.Read();
                                      int id = reader.GetInt32(0);
                                      reader.Close();
                                      command.CommandText = $"insert into DISEASESSYMPTOMS values ({diseaseid},{id})";
                                      command.ExecuteNonQuery();
                                  }
                                  else
                                  {
                                      command.CommandText = $"insert into SYMPTOMS values ('{i}')";
                                      reader.Close();
                                      command.ExecuteNonQuery();
                                      command.CommandText= $"select * from SYMPTOMS where SYMPTOMS.Name='{i.ToLower()}'";
                                      reader = command.ExecuteReader();
                                      reader.Read();
                                      int id = reader.GetInt32(0);
                                      reader.Close();
                                      command.CommandText = $"insert into DISEASESSYMPTOMS values ({diseaseid},{id})";
                                      command.ExecuteNonQuery();
                                  }
                              }
                          }
                          Hide();
                      }));
            }
        }
        private RelayCommand _backFromDisease;
        public RelayCommand BackFromDisease
        {
            get
            {
                return _backFromDisease ??
                    (_backFromDisease = new RelayCommand(obj =>
                      {
                          Hide();
                      }));
            }
        }
        private void Hide()
        {
            MainAdminWindow.DataContext = new ViewModelControl(MainAdminWindow);
            MainAdminWindow.BaseMainAdmin.IsEnabled = true;
            MainAdminWindow.DiseaseDescription.Visibility = Visibility.Collapsed;
            MainAdminWindow.ConfirmChangeDisease.Visibility = Visibility.Collapsed;
            MainAdminWindow.BackFromDisease.Visibility = Visibility.Collapsed;
            MainAdminWindow.ConfirmAddDisease.Visibility = Visibility.Collapsed;
        }
        public DiseaseViewModel(MainAdminWindow mainAdminWindow)
        {
            SelectedDisease = new Disease();
            MainAdminWindow = mainAdminWindow;
        }
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
