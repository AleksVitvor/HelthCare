using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Vitvor.HelthCare
{
    class MedicalInstitutionViewModel : INotifyPropertyChanged
    {
        public MainWindow MainWindow { get; set; }
        private MedicalInstitution _medicalInstitution;
        public MedicalInstitution MedicalInstitution
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
        public MedicalInstitutionViewModel(MainWindow mainWindow)
        {
            MedicalInstitution = new MedicalInstitution();
            MainWindow = mainWindow;
        }
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
