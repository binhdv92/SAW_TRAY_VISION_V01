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

namespace SAW_TRAY_VISION_V01
{
    /// <summary>
    /// Interaction logic for Parameters.xaml
    /// </summary>
    /// 
   

    public partial class ParametersPage : Page
    {
        public int SelectedTable = 0;
        public ParametersPage()
        {
            
            InitializeComponent();
            Show_Table();
        }

        private void Bt_Save_Click(object sender, RoutedEventArgs e)
        {
            MyGlobals.Parasv3.WriteToLog("ParametersPage", "Clicked Save Button");
            MyGlobals.Parasv3.FromDataSet();
            MyGlobals.Parasv3.ToXml();
        }

        private void Bt_Default_Click(object sender, RoutedEventArgs e)
        {
            MyGlobals.Parasv3.WriteToLog("ParametersPage", "Clicked Default Button");
            MyGlobals.Parasv3.FromDefault();
            Show_Table();
        }

        private void Bt_Refesh_Click(object sender, RoutedEventArgs e)
        {
            MyGlobals.Parasv3.WriteToLog("ParametersPage", "Clicked Refesh Button");
            MyGlobals.Parasv3.FromXml();
            Show_Table();
        }

        private void Bt_Switch_Table_Click(object sender, RoutedEventArgs e)
        {
            SelectedTable += 1;
            if (SelectedTable >= MyGlobals.Parasv3.ParametersDataset.Tables.Count)
            {
                SelectedTable = 0;
            }
            Show_Table();
            
        }
        public void Show_Table()
        {
            MyGlobals.Parasv3.ToDataSet();
            Lb_value.Content = $"{SelectedTable} - {MyGlobals.Parasv3.ParametersDataset.Tables[SelectedTable].TableName}";
            dg1.ItemsSource = MyGlobals.Parasv3.ParametersDataset.Tables[SelectedTable].DefaultView;
        }

        private void Bt_Apply_Click(object sender, RoutedEventArgs e)
        {
            MyGlobals.Parasv3.WriteToLog("ParametersPage", "Clicked Apply Button");
            MyGlobals.Parasv3.FromDataSet();
        }
    }
}
