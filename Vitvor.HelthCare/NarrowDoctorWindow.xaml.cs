﻿using System;
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
    /// Логика взаимодействия для NarrowDoctorWindow.xaml
    /// </summary>
    public partial class NarrowDoctorWindow : Window
    {
        private MainWindow _mainWindow;
        public NarrowDoctorWindow(MainWindow mainWindow, int doctorid)
        {
            InitializeComponent();
            _mainWindow = mainWindow;
            _mainWindow.Hide();
            DataContext = new NarrowPatientViewModel(this, doctorid);

        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _mainWindow.Show();
        }
    }
}
