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
                        if (medicalInstitution != null) 
                        {
                            SqlCommand sqlCommand = new SqlCommand();
                            medicalInstitution.AdminPassword = MainAdminWindow.MedicalInstitutionPassBox.Password;
                            sqlCommand.CommandText = $"insert into MEDICALINSTITUTION value ({medicalInstitution.MedicalInstitutionName}, " +
                            $"{medicalInstitution.AdminUsername}, {medicalInstitution.AdminPassword})";
                            Hide();
                        }
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
                              sqlCommand.CommandText = $"delete from MEDICALINSTITUTION where MEDICALINSTITUTION.PASSWORD={medicalInstitution.AdminPassword} and" +
                              $"MEDICALINSTITUTION.NAME={medicalInstitution.MedicalInstitutionName} and MEDICALINSTITUTION.ADMINUSERNAME={medicalInstitution.AdminUsername}";
                              Hide();
                          }
                      }));
            }
        }
        private void Hide()
        {
            MainAdminWindow.DataContext = new ViewModelControl(MainAdminWindow);
            MainAdminWindow.MedicalInstitutionDescription.Visibility = Visibility.Collapsed;
            MainAdminWindow.ConfirmAddDisease.Visibility = Visibility.Collapsed;
            MainAdminWindow.ConfirmAddMI.Visibility = Visibility.Collapsed;
            MainAdminWindow.ConfirmChangeDisease.Visibility = Visibility.Collapsed;
            MainAdminWindow.ConfirmDeleteMI.Visibility = Visibility.Collapsed;
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
