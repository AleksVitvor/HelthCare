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
    /// Логика взаимодействия для RegistrationWindow.xaml
    /// </summary>
    public partial class RegistrationWindow : Window
    {
        private MainWindow _mainWindow;
        public RegistrationWindow(MainWindow mainWindow)
        {
            InitializeComponent();
            _mainWindow = mainWindow;
            _mainWindow.Hide();
            DataContext = new UserRegistrationViewModel(this);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            UserRegistrationViewModel registrationViewModel = DataContext as UserRegistrationViewModel;
            if(registrationViewModel.UserRegistration.isUpdating)
            {

            }
            _mainWindow.Show();
        }
    }
}
