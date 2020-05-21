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
using System.IO;

namespace SAW_TRAY_VISION_V01
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    public class MyGlobals
    {
        public static Parametersv3 Parasv3 = new Parametersv3();
        public static Products Prods = new Products();


        public static HomePage _Home_Page = new HomePage();
        public static ParametersPage _Parameters_Page = new ParametersPage();
        public static ProductListPage _ProductList_Page = new ProductListPage();
        public static AboutPage _About_Page = new AboutPage();
    }

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            // init Para
            //MyGlobals.Parasv3.Update_FolderAutoSaveDefectImage_FileName();
            //MyGlobals.Parasv3.Update_ManualSaveImage_FileName();
            //MyGlobals.Parasv3.Update_FolderAutoSaveAllImage_FileName();
            MyGlobals.Parasv3.Update_All_FileName();
            //MyGlobals.Parasv3.LogWriter = new StreamWriter(MyGlobals.Parasv3.LogFile);
            MyGlobals.Parasv3.WriteToLog("MainWindow", "Start Machine");

            //
            Frame_WorkPage.Content = MyGlobals._Home_Page;
            MyGlobals._Home_Page.Tb_Save_Result.Text = MyGlobals.Parasv3.FileName_ManualSaveImage[0];



        }

        private void BtHome_Click(object sender, RoutedEventArgs e)
        {

            Frame_WorkPage.Content = MyGlobals._Home_Page;
            MyGlobals.Parasv3.WriteToLog("MainWindow", "Click Home Button");
        }

        private void Btn_About_Click(object sender, RoutedEventArgs e)
        {
            Frame_WorkPage.Content = MyGlobals._About_Page;
            MyGlobals.Parasv3.WriteToLog("MainWindow", "Click About Button");
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            //MyGlobals._Home_Page.StopCamera();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MyGlobals._Home_Page.StopCamera();
            MyGlobals.Parasv3.WriteToLog("MainWindow", "Stop Machine");
        }

        private void BtnParameter_Click(object sender, RoutedEventArgs e)
        {
            Frame_WorkPage.Content = MyGlobals._Parameters_Page;
            MyGlobals.Parasv3.WriteToLog("MainWindow", "Click Parameter Button");
        }

        private void BtnProduct_Click(object sender, RoutedEventArgs e)
        {
            Frame_WorkPage.Content = MyGlobals._ProductList_Page;
            MyGlobals.Parasv3.WriteToLog("MainWindow", "Click Product Button");
        }
    }
}
