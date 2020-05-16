using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Vitvor.HelthCare
{
    class DiseaseForUser:INotifyPropertyChanged
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
        private string _dateOfDiagnosis;
        public string DateOfDiagnosis
        {
            get
            {
                return _dateOfDiagnosis;
            }
            set
            {
                _dateOfDiagnosis = value;
                OnPropertyChanged("DateOfDiagnosis");
            }
        }
        private string _dateOfClosing;
        public string DateOfClosing
        {
            get
            {
                return _dateOfClosing;
            }
            set
            {
                _dateOfClosing = value;
                OnPropertyChanged("DateOfClosing");
            }
        }
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            DiseaseForUser m = obj as DiseaseForUser;
            if (m as DiseaseForUser == null)
                return false;
            return m.DiseaseName == this.DiseaseName && m.DateOfClosing == this.DateOfClosing && m.DateOfDiagnosis==this.DateOfDiagnosis;
        }
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
