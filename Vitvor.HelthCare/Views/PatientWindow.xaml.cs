using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Vitvor.HelthCare.Services;
using Vitvor.HelthCare.UserWork;

namespace Vitvor.HelthCare
{
    /// <summary>
    /// Логика взаимодействия для PatientWindow.xaml
    /// </summary>
    public partial class PatientWindow : Window
    {
        MainWindow _mainWindow;
        private int _patientid;
        public delegate void WindowLoaded(string type, int id);
        public event WindowLoaded Load;
        public delegate void WindowClosing(string type, int id);
        public event WindowClosing CloseWindow;
        public PatientWindow(MainWindow mainWindow, int patientid)
        {
            _mainWindow = mainWindow;
            _mainWindow.Hide();
            InitializeComponent();
            _patientid = patientid;
            Appointmentdate.DisplayDateStart = DateTime.Now;
            Load += LoadAndCloseControl.Load;
            CloseWindow += LoadAndCloseControl.Close;
            Load.Invoke("Patient", patientid);
            DataContext = new DiseasForUserViewModel(this, patientid);
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            CloseWindow.Invoke("Patient", _patientid);
            _mainWindow.Show();
        }
    }
}
