using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Vitvor.HelthCare
{
    class NarrowPatientViewModel : INotifyPropertyChanged
    {
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
        public NarrowPatientViewModel(NarrowDoctorWindow narrowDoctorWindow)
        {
            _narrowDoctorWindow = narrowDoctorWindow;
        }
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
