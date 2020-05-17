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
    /// Логика взаимодействия для AdminWindow.xaml
    /// </summary>
    public partial class AdminWindow : Window
    {
        private MainWindow _mainWindow;
        public AdminWindow(MainWindow mainWindow, int adminid)
        {
            _mainWindow = mainWindow;
            _mainWindow.Hide();
            InitializeComponent();
            DataContext = new DoctorViewModel(this, adminid);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _mainWindow.Show();
        }
    }
}
