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

namespace Vitvor.HelthCare
{
    /// <summary>
    /// Логика взаимодействия для GeneralDoctorWindow.xaml
    /// </summary>
    public partial class GeneralDoctorWindow : Window
    {
        private MainWindow _mainWindow;
        public GeneralDoctorWindow(MainWindow mainWindow)
        {
            _mainWindow = mainWindow;
            _mainWindow.Hide();
            InitializeComponent();
            DataContext = new PatientViewModel(this);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _mainWindow.Show();
        }
    }
}
