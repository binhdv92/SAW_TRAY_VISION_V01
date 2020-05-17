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
using EasyModbus;
using System.Windows.Threading;
using System.Configuration;
//using SAW_TRAY_VISION_V01.sources;

namespace SAW_TRAY_VISION_V01
{
    /// <summary>
    /// Interaction logic for WindowIO.xaml
    /// </summary>
    public partial class WindowIO : System.Windows.Window
    {
        ModbusClient modbusClient;
        bool Connect_flag;
        Parametersv1 Paras = new Parametersv1();

        #region Ham Con
        public void WindowIO_Init()
        {
            tb_Modbus_Server_Name.Text = Paras.Modbus_Server_Name.Value;
            Tb_Modbus_Server_IP.Text = Paras.Modbus_Server_IP.Value;
            Tb_Modbus_Server_Port.Text = Paras.Modbus_Server_Port.Value;

            Lb_DI_00_Name.Content = Paras.DI_Tray_Present_Sensor.Name;

            Lb_DO_00_Name.Content = Paras.DO_Red_Light.Name;
            Lb_DO_01_Name.Content = Paras.DO_Amber_Light.Name;
            Lb_DO_02_Name.Content = Paras.DO_Green_Light.Name;
            Lb_DO_03_Name.Content = Paras.DO_Buzzer.Name;
            Lb_DO_04_Name.Content = Paras.DO_Red_Light.Name;
        }
        #endregion

        public WindowIO()
        {
            InitializeComponent();
            Paras.LoadAllParameters();
            WindowIO_Init();

            DispatcherTimer Dt_Scan_IO = new DispatcherTimer();
            Dt_Scan_IO.Interval = TimeSpan.FromMilliseconds(100);
            Dt_Scan_IO.Tick += Dt_Scan_IO_Ticker;
            Dt_Scan_IO.Start();
        }

        private void BtnConnectIO_Click(object sender, RoutedEventArgs e)
        {
            if (Lb_Connection_Status.Content.ToString() != "Connected!") 
            {
                try
                {
                    modbusClient = new ModbusClient(Paras.Modbus_Server_IP.Value, int.Parse(Paras.Modbus_Server_Port.Value));
                    modbusClient.LogFileFilename = Paras.Modbus_Server_LogFileFilename.Value;
                    modbusClient.Connect();
                    //modbusClient.ConnectionTimeout = 5000;
                    Lb_Connection_Status.Content = "Connected!";
                    Connect_flag = true;
                    // Read all Coils and update to check boxs
                    bool[] _ReadCoils = modbusClient.ReadCoils(0, 8);
                    CheckBox[] _CheckBoxes = { Cb_DO_00, Cb_DO_01, Cb_DO_02, Cb_DO_03, Cb_DO_04, Cb_DO_05, Cb_DO_06, Cb_DO_07 };
                    int _index = 0;
                    foreach (CheckBox _checkBox in _CheckBoxes)
                    {
                        _checkBox.IsChecked = _ReadCoils[_index];
                        _index++;
                    }
                }
                catch (Exception)
                {
                    Lb_Connection_Status.Content = "Fail to Connect!";
                }
            }
            
        }

        private void BtDisonnectIO_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                modbusClient.Disconnect();
                Lb_Connection_Status.Content = "Disconnected!";
                Connect_flag = false;
            }
            catch (Exception)
            {
                Lb_Connection_Status.Content = "Fail to Disconnect!";
            }
        }

        private void Cb_DO_00_Checked(object sender, RoutedEventArgs e)
        {
            modbusClient.WriteSingleCoil(0, true);
        }

        private void Cb_DO_01_Checked(object sender, RoutedEventArgs e)
        {
            modbusClient.WriteSingleCoil(1, true);
        }

        private void Cb_DO_02_Checked(object sender, RoutedEventArgs e)
        {
            modbusClient.WriteSingleCoil(2, true);
        }

        private void Cb_DO_03_Checked(object sender, RoutedEventArgs e)
        {
            modbusClient.WriteSingleCoil(3, true);
        }

        private void Cb_DO_04_Checked(object sender, RoutedEventArgs e)
        {
            modbusClient.WriteSingleCoil(4,true);
        }

        private void Cb_DO_05_Checked(object sender, RoutedEventArgs e)
        {
            modbusClient.WriteSingleCoil(5, true);
        }

        private void Cb_DO_06_Checked(object sender, RoutedEventArgs e)
        {
            modbusClient.WriteSingleCoil(6, true);
        }

        private void Cb_DO_07_Checked(object sender, RoutedEventArgs e)
        {
            modbusClient.WriteSingleCoil(7, true);
        }

        // Uncheck DO
        private void Cb_DO_00_Unchecked(object sender, RoutedEventArgs e)
        {
            modbusClient.WriteSingleCoil(0, false);
        }

        private void Cb_DO_01_Unchecked(object sender, RoutedEventArgs e)
        {
            modbusClient.WriteSingleCoil(1, false);
        }
        private void Cb_DO_02_Unchecked(object sender, RoutedEventArgs e)
        {
            modbusClient.WriteSingleCoil(2, false);
        }
        private void Cb_DO_03_Unchecked(object sender, RoutedEventArgs e)
        {
            modbusClient.WriteSingleCoil(3, false);
        }
        private void Cb_DO_04_Unchecked(object sender, RoutedEventArgs e)
        {
            modbusClient.WriteSingleCoil(4, false);
        }
        private void Cb_DO_05_Unchecked(object sender, RoutedEventArgs e)
        {
            modbusClient.WriteSingleCoil(5, false);
        }
        private void Cb_DO_06_Unchecked(object sender, RoutedEventArgs e)
        {
            modbusClient.WriteSingleCoil(6, false);
        }
        private void Cb_DO_07_Unchecked(object sender, RoutedEventArgs e)
        {
            modbusClient.WriteSingleCoil(7, false);
        }

        private void Dt_Scan_IO_Ticker(object sender, EventArgs e)
        {
            if (Connect_flag == true)
            {
                bool[] _ReadCoils = modbusClient.ReadDiscreteInputs(0, 8);
                int _index = 0;
                CheckBox[] _CheckBoxes = { lb_DI_00, lb_DI_01, lb_DI_02, lb_DI_03, lb_DI_04, lb_DI_05, lb_DI_06, lb_DI_07 };

                foreach (CheckBox cb in _CheckBoxes)
                {
                    cb.IsChecked = _ReadCoils[_index];
                    if (cb.IsChecked == true)
                    {
                        cb.Foreground = new SolidColorBrush(Colors.Green);
                        cb.FontWeight = FontWeights.ExtraBold;
                    }
                    else
                    {
                        cb.Foreground = new SolidColorBrush(Colors.Black);
                        cb.FontWeight = FontWeights.Bold;
                    }
                    _index++;
                }
            }
        }
    }
}
