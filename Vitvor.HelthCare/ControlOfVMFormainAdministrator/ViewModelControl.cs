﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Vitvor.HelthCare
{
    class ViewModelControl : INotifyPropertyChanged
    {
        private MainAdminWindow MainAdminWindow;
        private RelayCommand addMI;
        public RelayCommand AddMI
        {
            get
            {
                return addMI ??
                    (addMI = new RelayCommand(obj =>
                    {
                        MainAdminWindow.DataContext = new MedicalInstitutionViewModel(MainAdminWindow);
                        MainAdminWindow.MedicalInstitutionDescription.Visibility = Visibility.Visible;
                        MainAdminWindow.ConfirmAddMI.Visibility = Visibility.Visible;
                    }));
            }
        }
        private RelayCommand deleteMI;
        public RelayCommand DeleteMI
        {
            get
            {
                return deleteMI ??
                    (deleteMI = new RelayCommand(obj =>
                    {
                        MainAdminWindow.DataContext = new MedicalInstitutionViewModel(MainAdminWindow);
                        MainAdminWindow.MedicalInstitutionDescription.Visibility = Visibility.Visible;
                        MainAdminWindow.ConfirmDeleteMI.Visibility = Visibility.Visible;
                    }));
            }
        }
        public ViewModelControl(MainAdminWindow adminWindow)
        {
            MainAdminWindow = adminWindow;
        }
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
