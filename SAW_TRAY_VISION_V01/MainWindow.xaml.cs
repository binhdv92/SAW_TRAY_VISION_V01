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

namespace SAW_TRAY_VISION_V01
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    public class MyGlobals
    {
        public static Parametersv3 Parasv3 = new Parametersv3();
        public static HomePage _Home_Page = new HomePage();
        //public static Recipe_Page RPW = new Recipe_Page();
        public static AboutPage _About_Page = new AboutPage();
        public static ParametersPage _Parameters_Page = new ParametersPage();
    }

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            
            Frame_WorkPage.Content = MyGlobals._Home_Page;

        }

        private void BtHome_Click(object sender, RoutedEventArgs e)
        {

            Frame_WorkPage.Content = MyGlobals._Home_Page;
        }

        private void BtnRecipe_Click(object sender, RoutedEventArgs e)
        {

            //Frame_WorkPage.Content = MyGlobals.RPW;

        }

        private void Btn_About_Click(object sender, RoutedEventArgs e)
        {
            Frame_WorkPage.Content = MyGlobals._About_Page;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            //MyGlobals._Home_Page.StopCamera();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MyGlobals._Home_Page.StopCamera();
        }

        private void BtnParameter_Click(object sender, RoutedEventArgs e)
        {
            Frame_WorkPage.Content = MyGlobals._Parameters_Page;
        }

        private void BtnProduct_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
