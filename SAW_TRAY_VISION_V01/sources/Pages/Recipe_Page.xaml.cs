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
using System.IO;
using System.Configuration;
using SAW_TRAY_VISION_V01.sources;

namespace SAW_TRAY_VISION_V01
{
    /// <summary>
    /// Interaction logic for Recipe_Page.xaml
    /// </summary>
    public partial class Recipe_Page : Page
    {
        Parameters Paras = new Parameters();

        public Recipe_Page()
        {
            InitializeComponent();
            Tb_Modbus_Server_IP.Text = Paras.Modbus_Server_IP.Value;
            Tb_Modbus_Server_Port.Text = Paras.Modbus_Server_Port.Value;
            Tb_Yolov3_Cfg.Text = Paras.Yolov3_Cfg.Value;
            Tb_Yolov3_Weights.Text = Paras.Yolov3_Weights.Value;
            Tb_Yolov3_Names.Text = Paras.Yolov3_Names.Value;
        }
    }
}
