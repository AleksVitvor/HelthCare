using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Vitvor.HelthCare
{
    class DiseaseViewModel : INotifyPropertyChanged
    {
        public MainWindow MainWindow { get; set; }
        private Disease _disease;
        public Disease Disease
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
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
