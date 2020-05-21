
using System;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Navigation;
using System.Windows.Shapes;
using AForge.Video;
using AForge.Video.DirectShow;

using System.Data;
using System.Globalization;
using System.IO;

using Alturos.Yolo;
using Alturos.Yolo.Model;
using EasyModbus;
using System.Windows.Threading;
//using SAW_TRAY_VISION_V01.sources;
using AForge.Wpf;

namespace SAW_TRAY_VISION_V01
{
    public partial class HomePage : Page, INotifyPropertyChanged
    {

        #region Public properties

        public ObservableCollection<FilterInfo> VideoDevices { get; set; }

        public FilterInfo CurrentDevice
        {
            get { return _currentDevice; }
            set { _currentDevice = value; this.OnPropertyChanged("CurrentDevice"); }
        }
        private FilterInfo _currentDevice;

        public string RandomString(int size, bool lowerCase)
        {
            StringBuilder builder = new StringBuilder();
            Random random = new Random();
            char ch;
            for (int i = 0; i < size; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }
            if (lowerCase)
                return builder.ToString().ToLower();
            return builder.ToString();
        }
        #endregion


        #region Private fields

        private IVideoSource _videoSource;

        #endregion


        #region Khai Bao Bien Toan Cuc
        public BitmapImage BitmapImage_public;
        public System.Collections.Generic.IEnumerable<YoloItem> Items_Temp;
        string UriSource_TestImage = AppDomain.CurrentDomain.BaseDirectory + @"sources\media\CameraSnapshot.jpg";
        public byte[] DataByte_Public;
        public BitmapImage Bi_Public;
        public RenderTargetBitmap Bs_Detected_Public;

        public System.Windows.Controls.Image tempImage = new System.Windows.Controls.Image();
        //Parametersv1 Paras = new Parametersv1();
        //Parametersv3 Parasv3 = new Parametersv3();

        //ProductsList Products = new ProductsList();
        YoloWrapper yoloWrapper;
        // Modbus Server Setup
        ModbusClient modbusClient;
        DispatcherTimer Dt_Modbus = new DispatcherTimer();
        DispatcherTimer Dt_StateMachine = new DispatcherTimer();
        float Threshold_Counter = 0;
        public string StateMachine_Flag;
        public bool flag_init;
        public bool flag_init_Camera;
        public bool flag_init_Modbus;
        public bool Capture_Flag=false;
        //public string[] TrayIDList { get; set; }
        //public int tempi = 0;
        //public List<string> TrayIDList;
        public DetectionResult result;
        public int Pass_FLAG = 0;
        public int Fail_FLAG = 0;
        public string Final_Result_Flag;
        public System.Drawing.Image Img_Drawing;
        public RandomGenerator _randomGenerator = new RandomGenerator();

        #endregion

        #region Ham con
        public List<string> ToYolov3OriginFormat(System.Collections.Generic.IEnumerable<YoloItem> Items_Temp, BitmapImage BitmapImage_public,string filename)
        {
            List<string> tempstrlist=new List<string>();
            string tempstr;

            float X, Y, Width, Height;
            int Type;
            var templen = BitmapImage_public.Width;

            using (StreamWriter writer = new StreamWriter(filename))
            {
                foreach (YoloItem Item in Items_Temp)
                {
                    X = (float)(Item.X+ Item.Width / 2) / (float)BitmapImage_public.Width;
                    Y = (float)(Item.Y+ Item.Height/ 2) / (float)BitmapImage_public.Height;
                    Width = (float)Item.Width / (float)BitmapImage_public.Width;
                    Height = (float)Item.Height / (float)BitmapImage_public.Height;
                    if (Item.Type == "box")
                    {
                        Type = 10;
                    }
                    else
                    {
                        Type = int.Parse(Item.Type);
                    }

                    tempstr = $"{Type} {X} {Y} {Width} {Height}";
                    tempstrlist.Add(tempstr);
                    writer.WriteLine(tempstr);
                }
            }

            return (tempstrlist);
        }

        private System.Windows.Media.SolidColorBrush ReturnColor_byType(string type)
        {
            switch (type)
            {
                case "box":
                    return System.Windows.Media.Brushes.Green;
                case "0":
                    return System.Windows.Media.Brushes.Blue;
                    
                case "1":
                    return System.Windows.Media.Brushes.Chocolate;
                    
                case "2":
                    return System.Windows.Media.Brushes.Red;
                    
                case "3":
                    return System.Windows.Media.Brushes.Orange;
                    
                case "4":
                    return System.Windows.Media.Brushes.Violet;
                    
                case "5":
                    return System.Windows.Media.Brushes.Tomato;
                    
                case "6":
                    return System.Windows.Media.Brushes.MidnightBlue;
                    
                case "7":
                    return System.Windows.Media.Brushes.SeaGreen;
                    
                case "8":
                    return System.Windows.Media.Brushes.MediumVioletRed;
                    
                case "9":
                    return System.Windows.Media.Brushes.Magenta;
                    
            }
            return System.Windows.Media.Brushes.Black;
        }
        public class DetectionResult
        {
            public string Number { get; set; }
            public string TrayID { get; set; }
            public string Target { get; set; }

            public string Result { get; set; }

            public DetectionResult(string No, string trayID, string Target)
            {
                this.Target = Target;
                this.Number = No;
                this.TrayID = trayID;
                if (trayID == Target)
                {
                    this.Result = "PASS";
                }
                else
                {
                    this.Result = "FAIL";
                }
            }

        }
        #endregion

        #region StateMachine Functions
        public void StateMachine_NotInit()
        {
            Lb_Status.Content = "NOT INIT";
            Lb_Reslut.Content = "---";
            Lb_Reslut.Background = System.Windows.Media.Brushes.Gray;
            //State
            Btn_Init.IsEnabled = true;
            Btn_Start.IsEnabled = false;
            Btn_Stop.IsEnabled = false;
            Btn_Capture.IsEnabled = false;
            Btn_Detect.IsEnabled = false;
            Btn_Result.IsEnabled = false;

            //Action
            Btn_Restart.IsEnabled = false;
            Btn_BuzzerOff.IsEnabled = false;

            Btn_Save_Result.IsEnabled = false;


            // ComboBox
            Cb_Camera.IsEnabled = true;
            Cb_Recipe.IsEnabled = true;

            //Digital Input
            Cb_DI_Tray_Present_Sensor.IsChecked     = false; Cb_DI_Tray_Present_Sensor.IsEnabled    = false;
            Cb_Trigger.IsChecked                    = false; Cb_Trigger.IsEnabled                   = false;

            //Digital Output
            Cb_DO_Red_Light.IsChecked               = false; Cb_DO_Red_Light.IsEnabled              = false;
            //Cb_DO_Amber_Light.IsChecked             = false; 
            //Cb_DO_Amber_Light.IsEnabled            = false;
            Cb_DO_Green_Light.IsChecked             = false; Cb_DO_Green_Light.IsChecked            = false;
            Cb_DO_Buzzer.IsChecked                  = false; Cb_DO_Buzzer.IsEnabled                 = false;
            Cb_DO_Disable_Tray_Loading.IsChecked    = false; Cb_DO_Disable_Tray_Loading.IsEnabled   = false;
        }
        public void StateMachine_Running()
        {
            Lb_Status.Content = "RUNNING";
            // --------------------Button
            Btn_Mode.IsEnabled = false;
            //Btn_Auto.IsEnabled = false;
            //Btn_Dry.IsEnabled = false;
            //Btn_Manual.IsEnabled = false;

            Btn_Init.IsEnabled = false;
            Btn_Start.IsEnabled = false;
            Btn_Stop.IsEnabled = true;
            Btn_Restart.IsEnabled = false;

            Btn_BuzzerOff.IsEnabled = false;
            Btn_Capture.IsEnabled = false;
            Btn_Detect.IsEnabled = false;
            Btn_Result.IsEnabled = false;

            Btn_Save_Result.IsEnabled = false;

            // --------------------------------------Label
            //Lb_Status.Content = "RUNNING";
            //Lb_Reslut.Content = "---";
            //Lb_Reslut.Background = System.Windows.Media.Brushes.Gray;

            Cb_Camera.IsEnabled = false;
            Cb_Recipe.IsEnabled = false;
            //-----------------------------------Digital Input
            Cb_DI_Tray_Present_Sensor.IsEnabled = false;
            Cb_Trigger.IsEnabled = false;

            //Cb_DI_Tray_Present_Sensor.IsChecked = false; 
            //Cb_Trigger.IsChecked = false; 

            //Digital Output
            Cb_DO_Red_Light.IsChecked = false;
            //Cb_DO_Amber_Light.IsChecked = false;
            Cb_DO_Green_Light.IsChecked = true;
            Cb_DO_Buzzer.IsChecked = false;
            Cb_DO_Disable_Tray_Loading.IsChecked = false;


            Cb_DO_Red_Light.IsEnabled = false;
            //Cb_DO_Amber_Light.IsEnabled = false;
            Cb_DO_Green_Light.IsEnabled = false;
            Cb_DO_Buzzer.IsEnabled = false;
        }
        public void StateMachine_Idle()
        {
            Lb_Status.Content = "IDLE";
            // --------------------Button
            Btn_Mode.IsEnabled = true;
            //Btn_Auto.IsEnabled = true;
            //Btn_Dry.IsEnabled = true;
            //Btn_Manual.IsEnabled = true;

            Btn_Init.IsEnabled = true;
            Btn_Start.IsEnabled = true;
            Btn_Stop.IsEnabled = false;
            Btn_Restart.IsEnabled = false;

            Btn_BuzzerOff.IsEnabled = false;
            Btn_Capture.IsEnabled = false;
            Btn_Detect.IsEnabled = false;
            Btn_Result.IsEnabled = false;

            Btn_Save_Result.IsEnabled = false;

            // --------------------------------------Label
            //Lb_Status.Content = "RUNNING";
            //Lb_Reslut.Content = "---";
            //Lb_Reslut.Background = System.Windows.Media.Brushes.Gray;

            Cb_Camera.IsEnabled = true;
            Cb_Recipe.IsEnabled = true;
            //-----------------------------------Digital Input
            Cb_DI_Tray_Present_Sensor.IsEnabled = false;
            Cb_Trigger.IsEnabled = false;

            //Cb_DI_Tray_Present_Sensor.IsChecked = false; 
            //Cb_Trigger.IsChecked = false; 

            //Digital Output
            Cb_DO_Red_Light.IsChecked = false;
            //Cb_DO_Amber_Light.IsChecked = false;
            Cb_DO_Green_Light.IsChecked = true;
            Cb_DO_Buzzer.IsChecked = false;
            Cb_DO_Disable_Tray_Loading.IsChecked = false;


            Cb_DO_Red_Light.IsEnabled = false;
            //Cb_DO_Amber_Light.IsEnabled = false;
            Cb_DO_Green_Light.IsEnabled = false;
            Cb_DO_Buzzer.IsEnabled = false;
            Cb_DO_Disable_Tray_Loading.IsEnabled = false;
        }
        public void StateMachine_NoTray()
        {
            //State
            Btn_Init.IsEnabled = false;
            Btn_Start.IsEnabled = false;
            Btn_Stop.IsEnabled = true;
            Btn_Capture.IsEnabled = false;
            Btn_Detect.IsEnabled = false;
            Btn_Result.IsEnabled = false;

            //Action
            Btn_Restart.IsEnabled = false;
            Btn_BuzzerOff.IsEnabled = false;

            Btn_Save_Result.IsEnabled = false;

            //Status
            //Lb_Status.Content = "RUNNING";
            Lb_Reslut.Content = "NO TRAY DETECTED";
            Lb_Reslut.Background = System.Windows.Media.Brushes.Red;

            // ComboBox
            Cb_Camera.IsEnabled = false;
            Cb_Recipe.IsEnabled = false;
            //Digital Input
            Cb_DI_Tray_Present_Sensor.IsEnabled = false;
            Cb_Trigger.IsEnabled = false;

            //Cb_DI_Tray_Present_Sensor.IsChecked = false; 
            //Cb_Trigger.IsChecked = false; 

            //Digital Output
            Cb_DO_Red_Light.IsChecked = false;
            //Cb_DO_Amber_Light.IsChecked = false;
            Cb_DO_Green_Light.IsChecked = true;
            Cb_DO_Buzzer.IsChecked = false;
            Cb_DO_Disable_Tray_Loading.IsChecked = false;


            Cb_DO_Red_Light.IsEnabled = false;
            //Cb_DO_Amber_Light.IsEnabled = false;
            Cb_DO_Green_Light.IsEnabled = false;
            Cb_DO_Buzzer.IsEnabled = false;
            Cb_DO_Disable_Tray_Loading.IsEnabled = false;
        }
        public void StateMachine_WrongTray()
        {
            //State
            Btn_Init.IsEnabled = false;
            Btn_Start.IsEnabled = false;
            Btn_Stop.IsEnabled = true;
            Btn_Capture.IsEnabled = false;
            Btn_Detect.IsEnabled = false;
            Btn_Result.IsEnabled = false;
            
            //Action
            Btn_Restart.IsEnabled = true;
            Btn_BuzzerOff.IsEnabled = true;

            Btn_Save_Result.IsEnabled = true;
            
            // ComboBox
            Cb_Camera.IsEnabled = true;
            Cb_Recipe.IsEnabled = true;

            //Status
            Lb_Status.Content = "WRONG TRAY";
            //Lb_Reslut.Content = "WRONG TRAY";
            //Lb_Reslut.Background = System.Windows.Media.Brushes.Red;

            //Digital Input
            Cb_DI_Tray_Present_Sensor.IsEnabled = false;
            Cb_Trigger.IsEnabled = false;

            //Cb_DI_Tray_Present_Sensor.IsChecked = false; 
            //Cb_Trigger.IsChecked = false; 

            //Digital Output
            Cb_DO_Red_Light.IsChecked = true;
            ////Cb_DO_Amber_Light.IsChecked = false;
            Cb_DO_Green_Light.IsChecked = false;
            Cb_DO_Buzzer.IsChecked = true;
            //Cb_DO_Disable_Tray_Loading.IsChecked = true;


            Cb_DO_Red_Light.IsEnabled = false;
            ////Cb_DO_Amber_Light.IsEnabled = false;
            Cb_DO_Green_Light.IsEnabled = false;
            Cb_DO_Buzzer.IsEnabled = false;

            if(Lb_Mode.Content.ToString() == "AUTO")
            {
                Cb_DO_Disable_Tray_Loading.IsEnabled = false;
                Cb_DO_Disable_Tray_Loading.IsChecked = true;
            }
            else if(Lb_Mode.Content.ToString() == "DRY")
            {
                Cb_DO_Disable_Tray_Loading.IsEnabled = false;
                Cb_DO_Disable_Tray_Loading.IsChecked = false;
            }
            else if (Lb_Mode.Content.ToString() == "MANUAL")
            {
                Cb_DO_Disable_Tray_Loading.IsEnabled = true;
                Cb_DO_Disable_Tray_Loading.IsChecked = false;
            }
            else
            {

            }
        }
        #endregion

        #region GUI
        public HomePage()
        {
            InitializeComponent();
            StateMachine_NotInit();
            GetVideoDevices();
            CurrentDevice = VideoDevices[0];
            Cb_Camera.SelectedIndex = 0;
            StartCamera();


            //Initial Random string
            // ---Create a Output_File_Name Randomly.
            Cb_Recipe.ItemsSource = MyGlobals.Prods.TrayIDList;
            Cb_Recipe.SelectedIndex = MyGlobals.Parasv3.RecipeSelectedIndex;
            Console.WriteLine($"Seleted Tray Target: {MyGlobals.Parasv3.Detection_Target_ID}");

            Lb_Threshold.Content = MyGlobals.Parasv3.Threshold_Trigger;

            //
            yoloWrapper = new YoloWrapper(MyGlobals.Parasv3.Algorithm.Cfg, MyGlobals.Parasv3.Algorithm.Weights, MyGlobals.Parasv3.Algorithm.Names);

            //
            Btn_Auto_Click(null, null);

            //
            Dt_Modbus.Interval = TimeSpan.FromMilliseconds(MyGlobals.Parasv3.Modbus.IntervalTime);
            Dt_Modbus.Tick += Dt_ModbusTicker;
            Dt_Modbus.Stop();

            //
            Dt_StateMachine.Interval = TimeSpan.FromMilliseconds(MyGlobals.Parasv3.Timer_Interval_StateMachine);
            Dt_StateMachine.Tick += Dt_StateMachineTicker;
            Dt_StateMachine.Stop();
            //
            this.DataContext = this;

        }
        #endregion

        #region Btn function
        private void Dt_ModbusTicker(object sender, EventArgs e)
        {
            try// ---read Input
            {
                bool[] DI_Tray_Present_Sensor = modbusClient.ReadDiscreteInputs(MyGlobals.Parasv3.DI_Tray_Present_Sensor, 1);
                Cb_DI_Tray_Present_Sensor.IsChecked = DI_Tray_Present_Sensor[0];
                
                if (DI_Tray_Present_Sensor[0] )
                {
                    if(Threshold_Counter <= MyGlobals.Parasv3.Threshold_Trigger)
                    {
                        Threshold_Counter++;
                    }
                }
                else
                {
                    Threshold_Counter = 0;
                    Cb_Trigger.IsChecked = false;
                }                
                Lb_Trigger.Content = Threshold_Counter.ToString();
                
            }
            catch
            {
                MessageBox.Show("Error104: Fail to read Digital Input");
                StateMachine_NotInit();
                Dt_Modbus.Stop();
            }
            
            // ---Write Digital Output
            try
            {
                modbusClient.WriteSingleCoil(MyGlobals.Parasv3.Tower.Red_Port, Convert.ToBoolean(Cb_DO_Red_Light.IsChecked));
                //modbusClient.WriteSingleCoil(MyGlobals.Parasv3.Tower.Amber_Port, Convert.ToBoolean(//Cb_DO_Amber_Light.IsChecked));
                modbusClient.WriteSingleCoil(MyGlobals.Parasv3.Tower.Green_Port, Convert.ToBoolean(Cb_DO_Green_Light.IsChecked));
                modbusClient.WriteSingleCoil(MyGlobals.Parasv3.Tower.Buzzer_Port, Convert.ToBoolean(Cb_DO_Buzzer.IsChecked));
                modbusClient.WriteSingleCoil(MyGlobals.Parasv3.DO_Disable_Tray_Loading, Convert.ToBoolean(Cb_DO_Disable_Tray_Loading.IsChecked));
            }
            catch
            {
                MessageBox.Show("Error105: Fail to write Digital Output");
                StateMachine_NotInit();
                Dt_Modbus.Stop();
            }
        }

        private void Dt_StateMachineTicker(object sender, EventArgs e)
        {
            if (Lb_Mode.Content.ToString() == "AUTO" || Lb_Mode.Content.ToString() == "DRY")
            {
                switch (StateMachine_Flag)
                {
                    case "RUNNING":
                        Lb_Status.Content = "RUNNING";
                        StateMachine_Running();
                        if (Threshold_Counter == MyGlobals.Parasv3.Threshold_Trigger)
                        {
                            MyGlobals.Parasv3.WriteToLog("HomePage", "Trigger new Tray and capture new image");
                            StateMachine_Flag = "CAPTURE";
                        }
                        break;

                    case "CAPTURE":
                        Lb_Status.Content = "CAPTURE";
                        Btn_Capture_Click(null, null);
                        StateMachine_Flag = "DETECT";
                        break;

                    case "DETECT":
                        Lb_Status.Content = "DETECT";
                        Btn_Detect_Click(null, null);
                        if (Lb_Mode.Content.ToString() == "AUTO")
                        {
                            StateMachine_Flag = "MAKE DECISION";
                            if (MyGlobals.Parasv3.Flag_Auto_Save_All_Image == true)
                            {
                                MyGlobals.Parasv3.Update_All_FileName();
                                Save_Inspection_Result(MyGlobals.Parasv3.FileName_FolderAutoSaveAllImage);

                            }
                        }
                        else if(Lb_Mode.Content.ToString() == "DRY")
                        {
                            StateMachine_Flag = "RUNNING";
                            if (MyGlobals.Parasv3.Flag_Auto_Save_All_Image == true)
                            {
                                MyGlobals.Parasv3.Update_All_FileName();
                                Save_Inspection_Result(MyGlobals.Parasv3.FileName_FolderAutoSaveAllImage);

                            }
                        }
                        break;

                    case "MAKE DECISION":
                        Lb_Status.Content = "MAKE DECISION";
                        if (Lb_Reslut.Content.ToString() == "PASS")
                        {
                            MyGlobals.Parasv3.WriteToLog("Result", $"PASS on {MyGlobals.Parasv3.Detection_Target_ID} {MyGlobals.Parasv3.Detection_Target_ID_Str}");
                            StateMachine_Running();
                            StateMachine_Flag = "RUNNING";
                        }
                        else if (Lb_Reslut.Content.ToString() == "NO TRAY")
                        {
                            MyGlobals.Parasv3.WriteToLog("Result", $"NO TRAY on {MyGlobals.Parasv3.Detection_Target_ID} {MyGlobals.Parasv3.Detection_Target_ID_Str}");
                            StateMachine_Running();
                            StateMachine_Flag = "RUNNING";
                        }
                        else if (Lb_Reslut.Content.ToString() == "WRONG TRAY")
                        {
                            MyGlobals.Parasv3.WriteToLog("Result", $"WRONG TRAY on {MyGlobals.Parasv3.Detection_Target_ID} {MyGlobals.Parasv3.Detection_Target_ID_Str}");
                            StateMachine_WrongTray();
                            Dt_StateMachine.Stop();
                            if (MyGlobals.Parasv3.Flag_Auto_Save_Defect_Image == true)
                            {
                                MyGlobals.Parasv3.Update_All_FileName();
                                Save_Inspection_Result(MyGlobals.Parasv3.FileName_AutoSaveDefectImage);
                            }
                        }
                        break;
                }
            }
        }
        
        private void Btn_Auto_Click(object sender, RoutedEventArgs e)
        {
            Lb_Mode.Content = "AUTO";
            //State
            Btn_Init.IsEnabled = true;
            Btn_Start.IsEnabled = true;
            Btn_Stop.IsEnabled = false;
            Btn_Capture.IsEnabled = false;
            Btn_Detect.IsEnabled = false;
            Btn_Result.IsEnabled = false;

            //Action
            Btn_Restart.IsEnabled = false;
            Btn_BuzzerOff.IsEnabled = false;

            Btn_Save_Result.IsEnabled = false;

            //Status
            Lb_Reslut.Content = "---";
            Lb_Reslut.Background = System.Windows.Media.Brushes.Gray;
            // ComboBox
            Cb_Camera.IsEnabled = true;
            Cb_Recipe.IsEnabled = true;

            //Digital Input
            Cb_DI_Tray_Present_Sensor.IsEnabled = false;
            Cb_Trigger.IsEnabled = false;

            //Cb_DI_Tray_Present_Sensor.IsChecked = false; 
            //Cb_Trigger.IsChecked = false; 

            //Digital Output
            Cb_DO_Red_Light.IsChecked = false;
            //Cb_DO_Amber_Light.IsChecked = true; 
            Cb_DO_Green_Light.IsChecked = false;
            Cb_DO_Buzzer.IsChecked = false;
            Cb_DO_Disable_Tray_Loading.IsChecked = false;


            Cb_DO_Red_Light.IsEnabled = false;
            //Cb_DO_Amber_Light.IsEnabled = false;
            Cb_DO_Green_Light.IsEnabled = false;
            Cb_DO_Buzzer.IsEnabled = false;
            Cb_DO_Disable_Tray_Loading.IsEnabled = false;
        }
        private void Btn_Dry_Click(object sender, RoutedEventArgs e)
        {
            Lb_Mode.Content = "DRY";
            //State
            Btn_Init.IsEnabled = true;
            Btn_Start.IsEnabled = true;
            Btn_Stop.IsEnabled = false;
            Btn_Capture.IsEnabled = false;
            Btn_Detect.IsEnabled = false;
            Btn_Result.IsEnabled = false;

            //Action
            Btn_Restart.IsEnabled = false;
            Btn_BuzzerOff.IsEnabled = false;

            Btn_Save_Result.IsEnabled = false;

            //Status
            //Lb_Status.Content = "---";
            Lb_Reslut.Content = "---";
            Lb_Reslut.Background = System.Windows.Media.Brushes.Gray;
            // ComboBox
            Cb_Camera.IsEnabled = true;
            Cb_Recipe.IsEnabled = true;

            //Digital Input
            Cb_DI_Tray_Present_Sensor.IsEnabled = false;
            Cb_Trigger.IsEnabled = false;

            //Digital Output
            Cb_DO_Red_Light.IsChecked = false;
            //Cb_DO_Amber_Light.IsChecked = true; 
            Cb_DO_Green_Light.IsChecked = false;
            Cb_DO_Buzzer.IsChecked = false;
            Cb_DO_Disable_Tray_Loading.IsChecked = false;


            Cb_DO_Red_Light.IsEnabled = false;
            //Cb_DO_Amber_Light.IsEnabled = false;
            Cb_DO_Green_Light.IsEnabled = false;
            Cb_DO_Buzzer.IsEnabled = false;
            Cb_DO_Disable_Tray_Loading.IsEnabled = false;
        }
         
        private void Btn_Manual_Click(object sender, RoutedEventArgs e)
        {
            Lb_Mode.Content = "MANUAL";
            //State
            Btn_Init.IsEnabled = true;
            Btn_Start.IsEnabled = false;
            Btn_Stop.IsEnabled = false;
            Btn_Capture.IsEnabled = true;
            Btn_Detect.IsEnabled = true;
            Btn_Result.IsEnabled = true;

            //Action
            Btn_Restart.IsEnabled = false;
            Btn_BuzzerOff.IsEnabled = true;

            Btn_Save_Result.IsEnabled = true;

            //Status
            Lb_Reslut.Content = "---";
            Lb_Reslut.Background = System.Windows.Media.Brushes.Gray;

            // ComboBox
            Cb_Camera.IsEnabled = true;
            Cb_Recipe.IsEnabled = true;

            //Digital Input
            Cb_DI_Tray_Present_Sensor.IsEnabled = false;
            Cb_Trigger.IsEnabled = false;

            //Digital Output
            Cb_DO_Red_Light.IsChecked = false;
            //Cb_DO_Amber_Light.IsChecked = true; 
            Cb_DO_Green_Light.IsChecked = false;
            Cb_DO_Buzzer.IsChecked = false;
            Cb_DO_Disable_Tray_Loading.IsChecked = false;


            Cb_DO_Red_Light.IsEnabled = true;
            //Cb_DO_Amber_Light.IsEnabled             = true;
            Cb_DO_Green_Light.IsEnabled = true;
            Cb_DO_Buzzer.IsEnabled = true;
            Cb_DO_Disable_Tray_Loading.IsEnabled = true;
        }

        private void Btn_Init_Click(object sender, RoutedEventArgs e)
        {
            MyGlobals.Parasv3.WriteToLog("HomePage", "Click Init");
            StopCamera();
            try // Init Camera
            {
                StartCamera_02();
                Lb_Status_Camera.Content = "Good";
                Lb_Status_Camera.Background = System.Windows.Media.Brushes.Green;
            
            }
            catch
            {
                MessageBox.Show("Error103: Failt to start Camera");
                StopCamera();
                Lb_Status_Camera.Content = "Fail";
                Lb_Status_Camera.Background = System.Windows.Media.Brushes.Red;
            }
            
            try // ---Modbus Server Setup
            {
                modbusClient = new ModbusClient(MyGlobals.Parasv3.Modbus.IP, MyGlobals.Parasv3.Modbus.Port);
                modbusClient.LogFileFilename = MyGlobals.Parasv3.Modbus.LogFile;
                modbusClient.Connect();
                Dt_Modbus.Start();
                Lb_Status_Modbus.Content = "Good";
                Lb_Status_Modbus.Background = System.Windows.Media.Brushes.Green;
            }
            catch
            {
                MessageBox.Show("Error102: Fail to connect to the Modbus Server at the address " + MyGlobals.Parasv3.Modbus.IP + ":" + MyGlobals.Parasv3.Modbus.Port);
                Lb_Status_Modbus.Content = "Fail";
                Lb_Status_Modbus.Background = System.Windows.Media.Brushes.Red;
            }


            if(Lb_Status_Camera.Content.ToString() == "Good" & Lb_Status_Modbus.Content.ToString() == "Good")
            {
                flag_init = true;
                Lb_Status_Global.Content = "Good";
                Lb_Status_Global.Background= System.Windows.Media.Brushes.Green;
                Lb_Status.Content = "Init success";
                MyGlobals.Parasv3.WriteToLog("HomePage", "Init success");
            }
            else
            {
                flag_init = false;
                Lb_Status_Global.Content= "Fail";
                Lb_Status_Global.Background = System.Windows.Media.Brushes.Red;
                MyGlobals.Parasv3.WriteToLog("HomePage", "Init fail");
            }
        }

        private void Btn_Start_Click(object sender, RoutedEventArgs e)
        {
            MyGlobals.Parasv3.WriteToLog("HomePage", "Click START");
            if(Lb_Status_Global.Content.ToString() != "Good")
            {
                MessageBox.Show("Error1010: Tool is not yet INIT, please do INIT to continue");
            }
            else
            {
                StateMachine_Flag = "RUNNING";
                StateMachine_Running();
                Dt_StateMachine.Start();

                Threshold_Counter = 0;
                Cb_Camera.IsEnabled = false;
            }
        }

        private void Btn_Restart_Click(object sender, RoutedEventArgs e)
        {
            MyGlobals.Parasv3.WriteToLog("HomePage", "Click RE-START");
            Btn_Start_Click(null, null);
        }

        private void Btn_Stop_Click(object sender, RoutedEventArgs e)
        {
            MyGlobals.Parasv3.WriteToLog("HomePage", "Click STOP");
            StateMachine_Idle();
            Dt_StateMachine.Stop();

            Cb_Camera.IsEnabled = true;
        }

        private void Btn_Capture_Click(object sender, RoutedEventArgs e)
        {
            Preprocess_Image_Function();
            Dg_TrayID.ItemsSource = null;
            Lb_Reslut.Content = "---";
            Lb_Reslut.Background = System.Windows.Media.Brushes.Gray;
            MyGlobals.Parasv3.RandomStrDateKey = MyGlobals.Parasv3.Random_Date_String_Key();
            MyGlobals.Parasv3.Update_All_FileName();
            Tb_Save_Result.Text = MyGlobals.Parasv3.FileName_To_ManualSaveImage;
        }

        public void Preprocess_Image_Function()
        {
            try
            {
                BitmapImage_public = videoPlayer.Source as BitmapImage;
                ImgSnapShoot.Source = BitmapImage_public;
                this.Bi_Public = BitmapImage_public;

                //Convert Bitmap to byte[]
                PngBitmapEncoder Encoder_Temp = new PngBitmapEncoder();
                Encoder_Temp.Frames.Add(BitmapFrame.Create((BitmapSource)ImgSnapShoot.Source));
                using (MemoryStream ms = new MemoryStream())
                {
                    Encoder_Temp.Save(ms);
                    this.DataByte_Public = ms.ToArray();
                }
            }
            catch
            {
                MessageBox.Show("Error101: Cannot Capture Image");
            }
        }

        private void Btn_Detect_Click(object sender, RoutedEventArgs e)
        {
            Items_Temp = yoloWrapper.Detect(this.DataByte_Public);            
            Dg_Debug.ItemsSource = Items_Temp;

            #region Draw the result onto Raw image
            List<DetectionResult> tray_ID_list = new List<DetectionResult>();
            DrawingVisual Dv = new DrawingVisual();
            using (DrawingContext dc = Dv.RenderOpen())
            {
                dc.DrawImage(this.Bi_Public, new Rect(0, 0, Bi_Public.PixelWidth, Bi_Public.PixelHeight));
                for (int i = 0; i < Dg_Debug.Items.Count - 1; i++)
                {
                    try
                    {
                        Alturos.Yolo.Model.YoloItem row = (Alturos.Yolo.Model.YoloItem)Dg_Debug.Items[i];
                        dc.DrawRectangle(null, new System.Windows.Media.Pen(ReturnColor_byType(row.Type), 3), new Rect(row.X, row.Y, row.Width, row.Height));

                        Typeface typeface = new Typeface(new System.Windows.Media.FontFamily("Segoe UI"), FontStyles.Normal, FontWeights.UltraBold, FontStretches.Normal);
                        FormattedText formattedText = new FormattedText(row.Type, CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, typeface, 16, System.Windows.Media.Brushes.White);

                        System.Windows.Point textLocation = new System.Windows.Point(row.X + 1, row.Y - row.Height / 2 - 3);
                        dc.DrawRectangle(ReturnColor_byType(row.Type), null, new Rect(textLocation.X - 2.5, textLocation.Y - 2.5, formattedText.Width + 5, formattedText.Height + 5));
                        dc.DrawText(formattedText, textLocation);
                    }
                    catch
                    {

                    }
                }
            }
            RenderTargetBitmap rtb = new RenderTargetBitmap(Bi_Public.PixelWidth, Bi_Public.PixelHeight, 96, 96, PixelFormats.Pbgra32);
            rtb.Render(Dv);
            ImgSnapShoot.Source = rtb;
            this.Bs_Detected_Public = rtb;
            #endregion

            #region Grouping
            string[] header_array = { "Type", "Confidence", "X", "Y", "Width", "Height" };
            DataTable dt = new DataTable();
            DataTable ID_table = new DataTable();
            DataColumn column;
            // Add the column to the table.
            for (int i = 0; i < header_array.Length; i++)
            {
                column = new DataColumn();
                column.DataType = System.Type.GetType("System.String");
                column.ColumnName = header_array[i];
                column.AutoIncrement = false;
                column.Caption = header_array[i];
                column.ReadOnly = false;
                column.Unique = false;
                dt.Columns.Add(column);
            }

            for (int i = 0; i < Dg_Debug.Items.Count - 1; i++)
            {
                try
                {
                    Alturos.Yolo.Model.YoloItem row = (Alturos.Yolo.Model.YoloItem)Dg_Debug.Items[i];
                    DataRow newRow = dt.NewRow();
                    newRow["Type"] = row.Type;
                    newRow["Confidence"] = row.Confidence;
                    // string a = "asd"
                    newRow["X"] = row.X.ToString().PadLeft(5, '0');
                    newRow["Y"] = row.Y.ToString().PadLeft(5, '0');
                    newRow["Width"] = row.Width;
                    newRow["Height"] = row.Height;
                    dt.Rows.Add(newRow);
                }
                catch
                {

                }
            }
            DataView dview_sort = new DataView();
            DataView boxgroup = new DataView(dt);
            boxgroup.RowFilter = "Type = 'box'";
            boxgroup.Sort = "Y asc";
            for (int i_group = 0; i_group < boxgroup.Count; i_group++)
            {
                double _Temp_Box_Height = double.Parse(boxgroup[i_group]["Height"].ToString());
                if (_Temp_Box_Height >= (double)MyGlobals.Parasv3.Algorithm.BoxHeightMin)
                {
                    ID_table = new DataTable();
                    for (int i = 0; i < header_array.Length; i++)
                    {
                        column = new DataColumn();
                        column.DataType = System.Type.GetType("System.String");
                        column.ColumnName = header_array[i];
                        column.AutoIncrement = false;
                        column.Caption = header_array[i];
                        column.ReadOnly = false;
                        column.Unique = false;
                        ID_table.Columns.Add(column);
                    }

                    double x_max, x_min, y_max, y_min;
                    x_min = double.Parse(boxgroup[i_group]["X"].ToString());
                    x_max = x_min + double.Parse(boxgroup[i_group]["Width"].ToString());
                    y_min = double.Parse(boxgroup[i_group]["Y"].ToString());
                    y_max = y_min + _Temp_Box_Height;


                    for (int i = 0; i < Dg_Debug.Items.Count - 1; i++)
                    {
                        Alturos.Yolo.Model.YoloItem row = (Alturos.Yolo.Model.YoloItem)Dg_Debug.Items[i];
                        DataRow newRow = ID_table.NewRow();
                        newRow["Type"] = row.Type;
                        newRow["Confidence"] = row.Confidence;
                        //Dataview sort by string - X-Y data need to be a sting

                        newRow["X"] = row.X.ToString().PadLeft(5, '0');
                        newRow["Y"] = row.Y.ToString().PadLeft(5, '0');
                        newRow["Width"] = row.Width;
                        newRow["Height"] = row.Height;
                        double x = double.Parse(newRow["X"].ToString()) + double.Parse(newRow["Width"].ToString()) / 2;
                        double y = double.Parse(newRow["Y"].ToString()) + _Temp_Box_Height / 2;
                        if ((x_min <= x) && (x_max >= x) && (y_min <= y) && (y_max >= y) && (row.Type.ToString() != "box"))
                        {
                            ID_table.Rows.Add(newRow);
                        }
                    }
                    //
                    dview_sort = ID_table.DefaultView;
                    dview_sort.Sort = "X asc";
                    string trayID = "";
                    for (int i_char = 0; i_char < dview_sort.Count; i_char++)
                    {
                        trayID = trayID + dview_sort[i_char]["Type"].ToString();
                    }
                    result = new DetectionResult(i_group.ToString(), trayID, MyGlobals.Parasv3.Detection_Target_ID);
                    tray_ID_list.Add(result);
                }
            }

            //Validate the result
            Pass_FLAG = 0;
            Fail_FLAG = 0;
            foreach(DetectionResult _iresult in tray_ID_list)
            {
                if (_iresult.Result == "PASS")
                {
                    Pass_FLAG += 1;
                }
                else if (_iresult.Result == "FAIL")
                {
                    Fail_FLAG += 1;
                }
            }
            //
            if (Pass_FLAG == 0 && Fail_FLAG == 0)
            {
                MyGlobals.Parasv3.Detect_Result = "NO TRAY";
                Lb_Reslut.Content = "NO TRAY";
                Lb_Reslut.Background = System.Windows.Media.Brushes.Orange;
            }
            else if(Fail_FLAG > 0)
            {
                MyGlobals.Parasv3.Detect_Result = "WRONG TRAY";
                Lb_Reslut.Content = "WRONG TRAY";
                Lb_Reslut.Background = System.Windows.Media.Brushes.Red;
            }
            else if (Pass_FLAG>0 && Fail_FLAG == 0)
            {
                MyGlobals.Parasv3.Detect_Result = "PASS";
                Lb_Reslut.Content = "PASS";
                Lb_Reslut.Background = System.Windows.Media.Brushes.Green;
            }
            #endregion
            Dg_TrayID.ItemsSource = tray_ID_list;
        }

        private void Btn_Result_Click(object sender, RoutedEventArgs e)
        {
            if (Lb_Reslut.Content.ToString() == "PASS")
            {
                StateMachine_Running();
                StateMachine_Flag = "RUNNING";
            }
            else if (Lb_Reslut.Content.ToString() == "NO TRAY")
            {
                StateMachine_Running();
                StateMachine_Flag = "RUNNING";
            }
            else if (Lb_Reslut.Content.ToString() == "WRONG TRAY")
            {
                StateMachine_WrongTray();
                Dt_StateMachine.Stop();

            }
        }

        private void Btn_BuzzerOff_Click(object sender, RoutedEventArgs e)
        {
            MyGlobals.Parasv3.WriteToLog("HomePage", "Click BUZZER-OFF");
            Cb_DO_Buzzer.IsChecked = false;
        }

        private void Cb_DO_Buzzer_Checked(object sender, RoutedEventArgs e)
        {
            if (Lb_Status_Modbus.Content.ToString() == "Good")
            {
                modbusClient.WriteSingleCoil(MyGlobals.Parasv3.Tower.Buzzer_Port, true);
            }
            else
            {
                MessageBox.Show("Error 0001: Wago Modbus Cotroller is not inited, Please do init to continue");
            }
        }

        private void Cb_DO_Buzzer_Unchecked(object sender, RoutedEventArgs e)
        {
            if (Lb_Status_Modbus.Content.ToString() == "Good")
            {
                modbusClient.WriteSingleCoil(MyGlobals.Parasv3.Tower.Buzzer_Port, false);
            }
            else
            {
                MessageBox.Show("Error 0001: Wago Modbus Cotroller is not inited, Please do init to continue");
            }
        }

        #endregion

        #region Helper Videos Function
        private void Cb_Recipe_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MyGlobals.Parasv3.Detection_Target_ID = Cb_Recipe.SelectedItem.ToString().Split('-')[1];
            MyGlobals.Parasv3.Detection_Target_ID_Str = Cb_Recipe.SelectedItem.ToString();
            Console.WriteLine($"Seleted Tray Target: {MyGlobals.Parasv3.Detection_Target_ID}");
            MyGlobals.Parasv3.WriteToLog("HomePage", $"Choosed Recipe {MyGlobals.Parasv3.Detection_Target_ID} {MyGlobals.Parasv3.Detection_Target_ID_Str}");
        
        }

        private void video_NewFrame(object sender, AForge.Video.NewFrameEventArgs eventArgs)
        {
            try
            {
                BitmapImage bi;
                using (var bitmap = (Bitmap)eventArgs.Frame.Clone())
                {
                    bitmap.RotateFlip(RotateFlipType.Rotate180FlipNone);
                    bi = bitmap.ToBitmapImage();

                }
                bi.Freeze(); // avoid cross thread operations and prevents leaks
                Dispatcher.BeginInvoke(new ThreadStart(delegate { videoPlayer.Source = bi; }));
            }
            catch (Exception exc)
            {
                MessageBox.Show("Error on _videoSource_NewFrame:\n" + exc.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                StopCamera();
            }
        } 

        private void GetVideoDevices()
        {
            VideoDevices = new ObservableCollection<FilterInfo>();
            foreach (FilterInfo filterInfo in new FilterInfoCollection(FilterCategory.VideoInputDevice))
            {
                VideoDevices.Add(filterInfo);
            }
            if (VideoDevices.Any())
            {
                //if (_videoSource != null && _videoSource.IsRunning)
                //{
                    //CurrentDevice = VideoDevices[0];
                    //Cb_Camera.SelectedIndex = 0;
                //}

            }
            else
            {
                MessageBox.Show("No video sources found", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void StartCamera()
        {
            if (CurrentDevice != null)
            {
                // _videoSource = new VideoCaptureDevice(CurrentDevice.MonikerString);
                _videoSource = new VideoCaptureDevice(VideoDevices[0].MonikerString);
                _videoSource.NewFrame += video_NewFrame;
                _videoSource.Start();
            }
        }

        private void StartCamera_02()//(string monikerString)
        {
            if (CurrentDevice != null)
            {
                //_videoSource = new VideoCaptureDevice(monikerString);
                _videoSource = new VideoCaptureDevice(VideoDevices[Cb_Camera.SelectedIndex].MonikerString);
                _videoSource.NewFrame += video_NewFrame;
                _videoSource.Start();
            }
        }

        public void StopCamera_Old()
        {
            if (_videoSource != null && _videoSource.IsRunning)
            {
                _videoSource.SignalToStop();
                _videoSource.NewFrame -= new NewFrameEventHandler(video_NewFrame);
            }
        }
        public void StopCamera()
        {
            _videoSource.SignalToStop();
            _videoSource.Stop();
            _videoSource.NewFrame -= new NewFrameEventHandler(video_NewFrame);
            
        }
        #endregion

        #region INotifyPropertyChanged members

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                var e = new PropertyChangedEventArgs(propertyName);
                handler(this, e);
            }
        }

        private void RotateAndSaveImage(String input, String output)
        {
            //create an object that we can use to examine an image file
            using (System.Drawing.Image img = System.Drawing.Image.FromFile(input))
            {
                //rotate the picture by 90 degrees and re-save the picture as a Jpeg
                img.RotateFlip(RotateFlipType.Rotate180FlipNone);
                img.Save(output, System.Drawing.Imaging.ImageFormat.Jpeg);
            }
        }

        #endregion

        private void Btn_Save_Result_Click(object sender, RoutedEventArgs e)
        {
            Save_Inspection_Result(MyGlobals.Parasv3.FileName_ManualSaveImage);

        }

        private void Save_Inspection_Result(string[] Output_File_Name)
        {
            //Preprocess_Image_Function();
            PngBitmapEncoder Encoder_Public = new PngBitmapEncoder();
            Encoder_Public.Frames.Add(BitmapFrame.Create((BitmapSource)this.Bi_Public));
            try
            {
                using (FileStream stream5 = new FileStream(Output_File_Name[0], FileMode.Create))
                {
                    Encoder_Public.Save(stream5);
                    stream5.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error205: Fail to save result file to " + Output_File_Name[0] + "\n\r\n\r" + ex);
            }

            //to Yolov3 Text Format label
            ToYolov3OriginFormat(this.Items_Temp, this.BitmapImage_public, Output_File_Name[2]);

            //------------ Detected
            PngBitmapEncoder Encoder_Detected_Public = new PngBitmapEncoder();
            Encoder_Detected_Public.Frames.Add(BitmapFrame.Create(this.Bs_Detected_Public));
            try
            {
                using (FileStream stream5 = new FileStream(Output_File_Name[1], FileMode.Create))
                {
                    Encoder_Detected_Public.Save(stream5);
                    stream5.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error205: Fail to save result file to " + Output_File_Name[1] + "\n\r\n\r" + ex);
            }
        }

        private void Cb_Camera_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            StopCamera();
            StartCamera_02();
        }

        private void Btn_Mode_Click(object sender, RoutedEventArgs e)
        {
            if(Lb_Mode.Content.ToString() == "AUTO")
            {
                MyGlobals.Parasv3.WriteToLog("HomePage", "Switch to DRY mode");
                Btn_Dry_Click(null, null);

            }
            else if(Lb_Mode.Content.ToString() == "DRY")
            {
                MyGlobals.Parasv3.WriteToLog("HomePage", "Switch to MANUAL mode");
                Btn_Manual_Click(null, null);

            }
            else if (Lb_Mode.Content.ToString() == "MANUAL")
            {
                MyGlobals.Parasv3.WriteToLog("HomePage", "Switch to AUTO mode");
                Btn_Auto_Click(null, null);

            }
        }
    }

}
