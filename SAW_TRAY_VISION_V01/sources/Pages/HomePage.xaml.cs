
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
using EasyModbus;
using System.Windows.Threading;
using SAW_TRAY_VISION_V01.sources;
using AForge.Wpf;

namespace SAW_TRAY_VISION_V01
{
    public class RandomGenerator
    {
        // Generate a random number between two numbers    
        public int RandomNumber(int min, int max)
        {
            Random random = new Random();
            return random.Next(min, max);
        }

        // Generate a random string with a given size    
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

        // Generate a random password    
        public string RandomPassword()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(RandomString(4, true));
            builder.Append(RandomNumber(1000, 9999));
            builder.Append(RandomString(2, false));
            return builder.ToString();
        }

        public string[] Random_Output_File_Name()
        {
            DateTime _Now = new DateTime();
            _Now = DateTime.Now;
            //
            string NowStr = _Now.ToString("yyy_M_dd_hh_mm_tt");
            //
            StringBuilder _builder = new StringBuilder();
            _builder.Append(RandomString(5, true));
            //
            string[] Output_File_Name = new string[2];
            Output_File_Name[0]= @"outputs\CameraSnapshot_" + NowStr + "_" + _builder.ToString() + "_Origin.jpg";
            Output_File_Name[1]= @"outputs\CameraSnapshot_" + NowStr + "_" + _builder.ToString() + "_Detected.jpg";
            return Output_File_Name;
        }
    }
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
        string Detection_Target_ID = "N/A";

        string UriSource_TestImage = AppDomain.CurrentDomain.BaseDirectory + @"sources\media\CameraSnapshot.jpg";
        public byte[] DataByte_Public;
        public BitmapImage Bi_Public;
        public RenderTargetBitmap Bs_Detected_Public;

        public System.Windows.Controls.Image tempImage = new System.Windows.Controls.Image();
        Parameters Paras = new Parameters();
        ProductsList Products = new ProductsList();
        YoloWrapper yoloWrapper;
        // Modbus Server Setup
        ModbusClient modbusClient;
        DispatcherTimer Dt_Modbus = new DispatcherTimer();
        DispatcherTimer Dt_StateMachine = new DispatcherTimer();
        float Threshold_Counter = 0;
        public string StateMachine_Flag;
        public bool Capture_Flag=false;
        public string[] TrayIDList { get; set; }
        public DetectionResult result;
        public int Pass_FLAG = 0;
        public int Fail_FLAG = 0;
        public string Final_Result_Flag;
        public System.Drawing.Image Img_Drawing;
        public RandomGenerator _randomGenerator = new RandomGenerator();
        public string[] Output_File_Name;
        #endregion

        #region Ham con
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
            //State
            Btn_Init.IsEnabled = true;
            Btn_Mode_Change.IsEnabled = false;
            Btn_Start.IsEnabled = false;
            Btn_Stop.IsEnabled = false;
            Btn_Capture.IsEnabled = false;
            Btn_Detect.IsEnabled = false;
            Btn_Result.IsEnabled = false;

            //Action
            Btn_Restart.IsEnabled = false;
            Btn_BuzzerOff.IsEnabled = false;

            Btn_Save_Result.IsEnabled = false;

            //Status
            Lb_Status.Content = "Not Initialization";
            Lb_Reslut.Content = "---";
            Lb_Reslut.Background = System.Windows.Media.Brushes.Gray;
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

        public void StateMachine_Auto()
        {
            //State
            Btn_Init.IsEnabled = false;
            Btn_Mode_Change.IsEnabled = true;
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
            Lb_Status.Content = "AUTO";
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

        public void StateMachine_Manual()
        {
            //State
            Btn_Init.IsEnabled = false;
            Btn_Mode_Change.IsEnabled = true;
            Btn_Start.IsEnabled = false;
            Btn_Stop.IsEnabled = false;
            Btn_Capture.IsEnabled = true;
            Btn_Detect.IsEnabled = true;
            Btn_Result.IsEnabled = true;

            //Action
            Btn_Restart.IsEnabled = false;
            Btn_BuzzerOff.IsEnabled = false;

            Btn_Save_Result.IsEnabled = true;

            //Status
            Lb_Status.Content = "MANUAL";
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


            Cb_DO_Red_Light.IsEnabled               = true;
            //Cb_DO_Amber_Light.IsEnabled             = true;
            Cb_DO_Green_Light.IsEnabled             = true;
            Cb_DO_Buzzer.IsEnabled                  = true;
            Cb_DO_Disable_Tray_Loading.IsEnabled    = true;
        }

        public void StateMachine_Running()
        {
            //State
            Btn_Init.IsEnabled = false;
            Btn_Mode_Change.IsEnabled = false;
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
            Lb_Status.Content = "RUNNING";
            //Lb_Reslut.Content = "---";
            //Lb_Reslut.Background = System.Windows.Media.Brushes.Gray;

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

        public void StateMachine_NoTray()
        {
            //State
            Btn_Init.IsEnabled = false;
            Btn_Mode_Change.IsEnabled = false;
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
            Lb_Status.Content = "RUNNING";
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
            Btn_Mode_Change.IsEnabled = false;
            Btn_Start.IsEnabled = false;
            Btn_Stop.IsEnabled = true;
            Btn_Capture.IsEnabled = false;
            Btn_Detect.IsEnabled = false;
            Btn_Result.IsEnabled = false;
            
            //Action
            Btn_Restart.IsEnabled = true;
            Btn_BuzzerOff.IsEnabled = true;

            Btn_Save_Result.IsEnabled = true;

            //Status
            Lb_Status.Content = "WRONG TRAY";
            Lb_Reslut.Content = "WRONG TRAY";
            Lb_Reslut.Background = System.Windows.Media.Brushes.Red;

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
            Cb_DO_Disable_Tray_Loading.IsChecked = true;


            Cb_DO_Red_Light.IsEnabled = false;
            ////Cb_DO_Amber_Light.IsEnabled = false;
            Cb_DO_Green_Light.IsEnabled = false;
            Cb_DO_Buzzer.IsEnabled = false;
            Cb_DO_Disable_Tray_Loading.IsEnabled = false;
        }
        #endregion

        #region GUI
        public HomePage()
        {
            InitializeComponent();

            //Initial Random string
            // ---Create a Output_File_Name Randomly.
            Output_File_Name= _randomGenerator.Random_Output_File_Name();
            Tb_Save_Result.Text = Output_File_Name[0];

            //
            string TempStr=Paras.LoadAllParameters();
            if (TempStr == "ERROR"){
                MessageBox.Show("Error105: MyConfiguration.LoadAllParameters() get error");
            }

            //
            Products.LoadProductLists_Str();
            TrayIDList = Products._ProductLists_Str;
            Cb_Recipe.SelectedIndex = 2;
            
            //
            StateMachine_NotInit();

            //
            Dt_Modbus.Interval = TimeSpan.FromMilliseconds(int.Parse(Paras.Timer_Interval_Modbus.Value));
            Dt_Modbus.Tick += Dt_ModbusTicker;
            Dt_Modbus.Stop();

            //
            Dt_StateMachine.Interval = TimeSpan.FromMilliseconds(int.Parse(Paras.Timer_Interval_StateMachine.Value));
            Dt_StateMachine.Tick += Dt_StateMachineTicker;
            Dt_StateMachine.Stop();
            //
            this.DataContext = this;
            GetVideoDevices();

        }
        #endregion

        #region Btn function
        private void Dt_ModbusTicker(object sender, EventArgs e)
        {
            try// ---read Input
            {
                bool[] DI_Tray_Present_Sensor = modbusClient.ReadDiscreteInputs(int.Parse(Paras.DI_Tray_Present_Sensor.Value), 1);
                Cb_DI_Tray_Present_Sensor.IsChecked = DI_Tray_Present_Sensor[0];
                
                if (DI_Tray_Present_Sensor[0] )
                {
                    if(Threshold_Counter <= float.Parse(Paras.Threshold_Trigger.Value))
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
                modbusClient.WriteSingleCoil(int.Parse(Paras.DO_Red_Light.Value), Convert.ToBoolean(Cb_DO_Red_Light.IsChecked));
                //modbusClient.WriteSingleCoil(int.Parse(Paras.DO_Amber_Light.Value), Convert.ToBoolean(//Cb_DO_Amber_Light.IsChecked));
                modbusClient.WriteSingleCoil(int.Parse(Paras.DO_Green_Light.Value), Convert.ToBoolean(Cb_DO_Green_Light.IsChecked));
                modbusClient.WriteSingleCoil(int.Parse(Paras.DO_Buzzer.Value), Convert.ToBoolean(Cb_DO_Buzzer.IsChecked));
                modbusClient.WriteSingleCoil(int.Parse(Paras.DO_Disable_Tray_Loading.Value), Convert.ToBoolean(Cb_DO_Disable_Tray_Loading.IsChecked));
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
            if (Lb_Status.Content.ToString() == "RUNNING")
            {
                switch (StateMachine_Flag)
                {
                    case "RUNNING":
                        StateMachine_Running();
                        if (Threshold_Counter == float.Parse(Paras.Threshold_Trigger.Value))
                        {
                            StateMachine_Flag = "CAPTURE";
                        }
                        break;
                    case "CAPTURE":
                        Btn_Capture_Click(null, null);
                        StateMachine_Flag = "DETECT";
                        break;
                    case "DETECT":
                        Btn_Detect_Click(null, null);
                        StateMachine_Flag = "RESULT";
                        break;
                    case "RESULT":
                        Btn_Result_Click(null, null);
                        if (Lb_Reslut.Content.ToString() == "PASS")
                        {
                            StateMachine_Flag = "RUNNING";
                        }
                        else if (Lb_Reslut.Content.ToString() == "WRONG TRAY")
                        {
                            StateMachine_WrongTray();
                            Dt_StateMachine.Stop();
                        }
                        //
                       // else if (Lb_Reslut.Content.ToString() == "NO TRAY DETECTED")
                        //{
                        //    StateMachine_NoTray();
                        //}
                        break;
                }
            }
        }

        private void Btn_Init_Click(object sender, RoutedEventArgs e)
        {
            // Yolov3 process
            yoloWrapper = new YoloWrapper(Paras.Yolov3_Cfg.Value, Paras.Yolov3_Weights.Value, Paras.Yolov3_Names.Value);
            //var Items_Temp = yoloWrapper.Detect(this.DataByte_Public);
            //Dg_Debug.ItemsSource = Items_Temp;

            try // ---Modbus Server Setup
            {
                modbusClient = new ModbusClient(Paras.Modbus_Server_IP.Value, int.Parse(Paras.Modbus_Server_Port.Value));
                modbusClient.LogFileFilename = Paras.Modbus_Server_LogFileFilename.Value;
                modbusClient.Connect();
                Dt_Modbus.Start();

                string TempStr = Paras.LoadAllParameters();
                if (TempStr == "ERROR")
                {
                    MessageBox.Show("Error105: MyConfiguration.LoadAllParameters() get error");
                }

                try
                {
                    StartCamera(VideoDevices[Cb_Camera.SelectedIndex].MonikerString);
                    StateMachine_Auto();
                }
                catch
                {
                    MessageBox.Show("Error103: Failt to start Camera");
                    StopCamera();
                    StateMachine_NotInit();
                }
            }
            catch
            {
                MessageBox.Show("Error102: Fail to connect to the Modbus Server at the address " + Paras.Modbus_Server_IP.Value + ":" + Paras.Modbus_Server_Port.Value);
                StateMachine_NotInit();
            }
        }

        private void Btn_Mode_Change_Click(object sender, RoutedEventArgs e)
        {
            if (Lb_Status.Content.ToString() == "IDLE")
            {
                Lb_Status.Content = "AUTO";
                StateMachine_Auto();
            }
            else if (Lb_Status.Content.ToString() == "AUTO")
            {
                Lb_Status.Content = "MANUAL";
                StateMachine_Manual();
            }
            else if (Lb_Status.Content.ToString() == "MANUAL")
            {
                Lb_Status.Content = "AUTO";
                StateMachine_Auto();
            }
        }

        private void Btn_Start_Click(object sender, RoutedEventArgs e)
        {

            StateMachine_Running();
            Dt_StateMachine.Start();
            StateMachine_Flag = "RUNNING";
            Threshold_Counter = 0;
        }

        private void Btn_Stop_Click(object sender, RoutedEventArgs e)
        {
            StateMachine_Auto();
            Dt_StateMachine.Stop();
        }

        private void Btn_Capture_Click(object sender, RoutedEventArgs e)
        {
            Preprocess_Image_Function();
            Dg_TrayID.ItemsSource = null;
            Lb_Reslut.Content = "---";
            Lb_Reslut.Background = System.Windows.Media.Brushes.Gray;
        }

        public void Preprocess_Image_Function()
        {
            try
            {
                BitmapImage img = videoPlayer.Source as BitmapImage;
                ImgSnapShoot.Source = img;
                this.Bi_Public = img;

                //Convert Bitmap to byte[]
                PngBitmapEncoder Encoder_Temp = new PngBitmapEncoder();
                Encoder_Temp.Frames.Add(BitmapFrame.Create((BitmapSource)ImgSnapShoot.Source));
                //Encoder_Public.Frames.Add(BitmapFrame.Create((BitmapSource)ImgSnapShoot.Source));
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
            // Yolov3 process
            //yoloWrapper = new YoloWrapper(Paras.Yolov3_Cfg.Value, Paras.Yolov3_Weights.Value, Paras.Yolov3_Names.Value);
            var Items_Temp = yoloWrapper.Detect(this.DataByte_Public);
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
                if (_Temp_Box_Height >= double.Parse(Paras.Box_Height_Min.Value))
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
                        //double x = double.Parse(newRow["X"].ToString()) + double.Parse(newRow["Width"].ToString()) / 2;
                        //double y = double.Parse(newRow["Y"].ToString()) + double.Parse(newRow["Height"].ToString()) / 2;
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
                    result = new DetectionResult(i_group.ToString(), trayID, Detection_Target_ID);
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
                Final_Result_Flag = "NO_TRAY_DETECTED";
            }
            else if(Fail_FLAG > 0)
            {
                Final_Result_Flag = "WRONG_TRAY_DETECTED";
            }
            else if (Pass_FLAG>0 && Fail_FLAG == 0)
            {
                Final_Result_Flag = "PASS";
            }
            #endregion
            Dg_TrayID.ItemsSource = tray_ID_list;
        }

        private void Btn_Result_Click(object sender, RoutedEventArgs e)
        {
            if (Lb_Status.Content.ToString() == "MANUAL")
            {
                if (Final_Result_Flag == "PASS")
                {
                    //StateMachine_Running();
                    Lb_Reslut.Content = "PASS";
                    Lb_Reslut.Background = System.Windows.Media.Brushes.Green;
                }
                else if (Final_Result_Flag == "NO_TRAY_DETECTED")
                {
                    //StateMachine_NoTray();
                    //StateMachine_Running();
                    Lb_Reslut.Content = "NO TRAY DETECTED";
                    Lb_Reslut.Background = System.Windows.Media.Brushes.Red;
                }
                else if (Final_Result_Flag == "WRONG_TRAY_DETECTED")
                {
                    //StateMachine_WrongTray();
                    Lb_Reslut.Content = "WRONG TRAY DETECTED";
                    Lb_Reslut.Background = System.Windows.Media.Brushes.Red;
                }
            }
            else
            {
                if (Final_Result_Flag == "PASS")
                {
                    StateMachine_Running();
                    Lb_Reslut.Content = "PASS";
                    Lb_Reslut.Background = System.Windows.Media.Brushes.Green;
                }
                else if (Final_Result_Flag == "NO_TRAY_DETECTED")
                {
                    //StateMachine_NoTray();
                    StateMachine_Running();
                    Lb_Reslut.Content = "NO TRAY DETECTED";
                    Lb_Reslut.Background = System.Windows.Media.Brushes.Red;
                }
                else if (Final_Result_Flag == "WRONG_TRAY_DETECTED")
                {
                    StateMachine_WrongTray();
                }
            }
            
        }

        private void Btn_Restart_Click(object sender, RoutedEventArgs e)
        {
            Btn_Start_Click(null,null);
        }

        private void Btn_BuzzerOff_Click(object sender, RoutedEventArgs e)
        {
            Cb_DO_Buzzer.IsChecked = false;
        }


        private void Cb_DO_Buzzer_Unchecked(object sender, RoutedEventArgs e)
        {
            modbusClient.WriteSingleCoil(int.Parse(Paras.DO_Buzzer.Value), false);
        }

        private void Cb_DO_Buzzer_Checked(object sender, RoutedEventArgs e)
        {
            modbusClient.WriteSingleCoil(int.Parse(Paras.DO_Buzzer.Value), true);
        }
        #endregion

        #region Helper Videos Function
        private void Cb_Recipe_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.Detection_Target_ID = Cb_Recipe.SelectedItem.ToString().Split(',')[1];

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
                CurrentDevice = VideoDevices[0];
                Cb_Camera.SelectedIndex = 0;
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
                _videoSource = new VideoCaptureDevice(CurrentDevice.MonikerString);
                _videoSource.NewFrame += video_NewFrame;
                _videoSource.Start();
            }
        }

        private void StartCamera(string monikerString)
        {
            if (CurrentDevice != null)
            {
                _videoSource = new VideoCaptureDevice(monikerString);
                _videoSource.NewFrame += video_NewFrame;
                _videoSource.Start();
            }
        }

        public void StopCamera()
        {
            if (_videoSource != null && _videoSource.IsRunning)
            {
                _videoSource.SignalToStop();
                _videoSource.NewFrame -= new NewFrameEventHandler(video_NewFrame);
            }
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
            catch(Exception ex)
            {
                MessageBox.Show("Error205: Fail to save result file to " + Output_File_Name[0] + "\n\r\n\r" + ex);
            }

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

            //--- update random name
            Output_File_Name = _randomGenerator.Random_Output_File_Name();
            Tb_Save_Result.Text = Output_File_Name[0];
        }
    }

}
