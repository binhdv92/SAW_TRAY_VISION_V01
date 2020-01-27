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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Configuration;

namespace SAW_TRAY_VISION_V01
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private HomePage    FWP  = new HomePage();
        private Recipe_Page RWP  = new Recipe_Page();
        


        public MainWindow()
        {
            InitializeComponent();
            Frame_WorkPage.Content = this.FWP;
        }

        //private void BtnIO_Click(object sender, RoutedEventArgs e)
        //{
        //    WindowIO WIO = new WindowIO();
        //    WIO.Show();
        //}

        private void BtHome_Click(object sender, RoutedEventArgs e)
        {

            Frame_WorkPage.Content = this.FWP;
        }

        private void BtnRecipe_Click(object sender, RoutedEventArgs e)
        {
            Frame_WorkPage.Content = RWP;
        }

        private void Btn_About_Click(object sender, RoutedEventArgs e)
        {
            Window_About WA = new Window_About();
            WA.Show();
        }
    }
}
