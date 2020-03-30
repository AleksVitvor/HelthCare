using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Vitvor.HelthCare
{
    class Patient : INotifyPropertyChanged
    {
        private string _surname;
        public string Surname
        {
            get
            {
                return _surname;
            }
            set
            {
                _surname = value;
                OnPropertyChanged("Surname");
            }
        }
        private string _name;
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
                OnPropertyChanged("Name");
            }
        }
        private string _patronymic;
        public string Patronymic
        {
            get
            {
                return _patronymic;
            }
            set
            {
                _patronymic = value;
                OnPropertyChanged("Patronymic");
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
                if(_symptoms[_symptoms.Length-1]==',')
                {
                    //получение в бд информации о симптоме, если нет, то MessageBox, иначе добавление в таблицу
                }
            }
        }
        public Patient(string Surname, string Name, string Patronymic)
        {

        }
        public Patient()
        { }
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
