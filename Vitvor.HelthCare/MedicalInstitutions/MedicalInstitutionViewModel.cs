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
    class MedicalInstitutionViewModel : INotifyPropertyChanged
    {
        public MainAdminWindow MainAdminWindow { get; set; }
        private MedicalInstitution _medicalInstitution;
        public MedicalInstitution SelectedMedicalInstitution
        {
            get
            {
                return _medicalInstitution;
            }
            set
            {
                _medicalInstitution = value;
                OnPropertyChanged("MedicalInstitution");
            }
        }
        private RelayCommand _addMedicalInstitution;
        public RelayCommand AddMedicalInstitution
        {
            get
            {
                return _addMedicalInstitution ??
                    (_addMedicalInstitution = new RelayCommand(obj =>
                    {
                        MedicalInstitution medicalInstitution = obj as MedicalInstitution;
                        if (medicalInstitution != null && medicalInstitution.AdminUsername.Contains("Admin"))
                        {
                            SqlCommand sqlCommand = new SqlCommand();
                            medicalInstitution.AdminPassword = MainAdminWindow.MedicalInstitutionPassBox.Password;
                            sqlCommand.CommandText = $"insert into ADMINS values ('{medicalInstitution.MedicalInstitutionName}', " +
                            $"'{medicalInstitution.AdminUsername}', '{medicalInstitution.AdminPassword}', 'MI')";
                            sqlCommand.Connection = SingletonForSqlConnection.SqlConnection;
                            sqlCommand.ExecuteNonQuery();
                            Hide();
                        }
                        else
                            MessageBox.Show("Введены неверные данные");
                    }));
            }
        }
        private RelayCommand _deleteMedicalInstitution;
        public RelayCommand DeleteMedicalInstitution
        {
            get
            {
                return _deleteMedicalInstitution ??
                    (_deleteMedicalInstitution = new RelayCommand(obj =>
                      {
                          MedicalInstitution medicalInstitution = obj as MedicalInstitution;
                          if(medicalInstitution!=null)
                          {
                              SqlCommand sqlCommand = new SqlCommand();
                              medicalInstitution.AdminPassword = MainAdminWindow.MedicalInstitutionPassBox.Password;
                              sqlCommand.CommandText = $"delete from ADMINS where ADMINS.AdminPassword='{medicalInstitution.AdminPassword}' and" +
                              $"ADMINS.Name='{medicalInstitution.MedicalInstitutionName}' and ADMINS.AdminUsername='{medicalInstitution.AdminUsername}'";
                              sqlCommand.ExecuteNonQuery();
                              Hide();
                          }
                      }));
            }
        }
        private RelayCommand _backFromMI;
        public RelayCommand BackFromMI
        {
            get
            {
                return _backFromMI ??
                    (_backFromMI = new RelayCommand(obj =>
                    {
                        Hide();
                    }));
            }
        }
        private void Hide()
        {
            MainAdminWindow.DataContext = new ViewModelControl(MainAdminWindow);
            MainAdminWindow.BaseMainAdmin.IsEnabled = true;
            MainAdminWindow.MedicalInstitutionDescription.Visibility = Visibility.Collapsed;
            MainAdminWindow.ConfirmAddDisease.Visibility = Visibility.Collapsed;
            MainAdminWindow.ConfirmAddMI.Visibility = Visibility.Collapsed;
            MainAdminWindow.ConfirmChangeDisease.Visibility = Visibility.Collapsed;
            MainAdminWindow.ConfirmDeleteMI.Visibility = Visibility.Collapsed;
            MainAdminWindow.BackFromMI.Visibility = Visibility.Collapsed;
        }
        public MedicalInstitutionViewModel(MainAdminWindow mainAdminWindow)
        {
            SelectedMedicalInstitution = new MedicalInstitution();
            MainAdminWindow = mainAdminWindow;
        }
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
