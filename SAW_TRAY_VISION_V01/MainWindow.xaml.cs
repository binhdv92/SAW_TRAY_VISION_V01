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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Configuration;
//using SAW_TRAY_VISION_V01.sources;

namespace SAW_TRAY_VISION_V01
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public HomePage    FWP  = new HomePage();
        public Recipe_Page RWP  = new Recipe_Page();
        public Window_About WA = new Window_About();

        public MainWindow()
        {
            InitializeComponent();
            Frame_WorkPage.Content = this.FWP;
        }

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
            Frame_WorkPage.Content = WA;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            FWP.StopCamera();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            FWP.StopCamera();
        }
    }
}
