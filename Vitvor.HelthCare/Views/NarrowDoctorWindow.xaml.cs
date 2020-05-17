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

namespace Vitvor.HelthCare
{
    /// <summary>
    /// Логика взаимодействия для NarrowDoctorWindow.xaml
    /// </summary>
    public partial class NarrowDoctorWindow : Window
    {
        private MainWindow _mainWindow;
        private int _doctorid;
        public delegate void WindowLoaded(string type, int id, int idOfMI);
        public event WindowLoaded Load;
        public delegate void WindowClosing(string type, int id);
        public event WindowClosing CloseWindow;
        public NarrowDoctorWindow(MainWindow mainWindow, int doctorid, int idOfMi)
        {
            InitializeComponent();
            _mainWindow = mainWindow;
            _mainWindow.Hide();
            _doctorid = doctorid;
            Load += LoadAndCloseControl.Load;
            CloseWindow += LoadAndCloseControl.Close;
            Load.Invoke("doctor", doctorid, idOfMi);
            DataContext = new NarrowPatientViewModel(this, doctorid);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            CloseWindow.Invoke("doctor", _doctorid);
            _mainWindow.Show();
        }
    }
}
