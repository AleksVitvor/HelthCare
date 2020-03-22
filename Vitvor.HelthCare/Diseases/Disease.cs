using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Vitvor.HelthCare
{
    class Disease : INotifyPropertyChanged
    {
        private string _diseaseName;
        public string DiseaseName
        {
            get
            {
                return _diseaseName;
            }
            set
            {
                _diseaseName = value;
                OnPropertyChanged("DiseaseName");
            }
        }
        private string _symptoms;
        public string Symptoms
        {
            get
            {
                return _symptoms;
            }
            set
            {
                _symptoms = value;
                OnPropertyChanged("Symptoms");
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
