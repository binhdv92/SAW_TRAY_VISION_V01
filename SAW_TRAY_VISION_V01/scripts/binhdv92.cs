using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

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
            Output_File_Name[0] = @"outputs\CameraSnapshot_" + NowStr + "_" + _builder.ToString() + "_Origin.jpg";
            Output_File_Name[1] = @"outputs\CameraSnapshot_" + NowStr + "_" + _builder.ToString() + "_Detected.jpg";
            return Output_File_Name;
        }
    }
    class Parametersv1
    {
        public struct Parameter
        {
            public string Name;
            public string Value;
            public Parameter(string name, string value)
            {
                Name = name;
                Value = value;
            }
        }

        public string FileName = AppDomain.CurrentDomain.BaseDirectory + @"sources\MachineConfig.xml";

        //Modbus Server
        public Parameter Modbus_Server_Name;
        public Parameter Modbus_Server_IP;
        public Parameter Modbus_Server_Port;
        public Parameter Modbus_Server_LogFileFilename;

        //Algorithm
        public Parameter Yolov3_Cfg;
        public Parameter Yolov3_Weights;
        public Parameter Yolov3_Names;

        //Digital Input
        public Parameter DI_Tray_Present_Sensor;

        //Digital Output
        public Parameter DO_Red_Light;
        public Parameter DO_Amber_Light;
        public Parameter DO_Green_Light;
        public Parameter DO_Buzzer;
        public Parameter DO_Disable_Tray_Loading;

        //
        public Parameter Threshold_Trigger;
        public Parameter Timer_Interval_Modbus;
        public Parameter Timer_Interval_StateMachine;

        //
        public Parameter Box_Height_Min;

        public string LoadAllParameters()
        {
            XmlDocument Doc = new XmlDocument();
            string Flag_result;
            try
            {
                Doc.Load(this.FileName);
                foreach (XmlNode node in Doc.DocumentElement)
                {
                    string NamePara = node.ChildNodes[0].InnerText;
                    switch (NamePara)
                    {
                        case "Modbus_Server_Name":
                            this.Modbus_Server_Name.Name = node.ChildNodes[0].InnerText;
                            this.Modbus_Server_Name.Value = node.ChildNodes[1].InnerText;
                            break;
                        case "Modbus_Server_IP":
                            this.Modbus_Server_IP.Name = node.ChildNodes[0].InnerText;
                            this.Modbus_Server_IP.Value = node.ChildNodes[1].InnerText;
                            break;
                        case "Modbus_Server_Port":
                            this.Modbus_Server_Port.Name = node.ChildNodes[0].InnerText;
                            this.Modbus_Server_Port.Value = node.ChildNodes[1].InnerText;
                            break;
                        case "Modbus_Server_LogFileFilename":
                            this.Modbus_Server_LogFileFilename.Name = node.ChildNodes[0].InnerText;
                            this.Modbus_Server_LogFileFilename.Value = node.ChildNodes[1].InnerText;
                            break;
                        case "Yolov3_Cfg":
                            this.Yolov3_Cfg.Name = node.ChildNodes[0].InnerText;
                            this.Yolov3_Cfg.Value = node.ChildNodes[1].InnerText;
                            break;
                        case "Yolov3_Weights":
                            this.Yolov3_Weights.Name = node.ChildNodes[0].InnerText;
                            this.Yolov3_Weights.Value = node.ChildNodes[1].InnerText;
                            break;
                        case "Yolov3_Names":
                            this.Yolov3_Names.Name = node.ChildNodes[0].InnerText;
                            this.Yolov3_Names.Value = node.ChildNodes[1].InnerText;
                            break;
                        case "DI_Tray_Present_Sensor":
                            this.DI_Tray_Present_Sensor.Name = node.ChildNodes[0].InnerText;
                            this.DI_Tray_Present_Sensor.Value = node.ChildNodes[1].InnerText;
                            break;
                        case "DO_Red_Light":
                            this.DO_Red_Light.Name = node.ChildNodes[0].InnerText;
                            this.DO_Red_Light.Value = node.ChildNodes[1].InnerText;
                            break;
                        case "DO_Amber_Light":
                            this.DO_Amber_Light.Name = node.ChildNodes[0].InnerText;
                            this.DO_Amber_Light.Value = node.ChildNodes[1].InnerText;
                            break;
                        case "DO_Green_Light":
                            this.DO_Green_Light.Name = node.ChildNodes[0].InnerText;
                            this.DO_Green_Light.Value = node.ChildNodes[1].InnerText;
                            break;
                        case "DO_Buzzer":
                            this.DO_Buzzer.Name = node.ChildNodes[0].InnerText;
                            this.DO_Buzzer.Value = node.ChildNodes[1].InnerText;
                            break;
                        case "DO_Disable_Tray_Loading":
                            this.DO_Disable_Tray_Loading.Name = node.ChildNodes[0].InnerText;
                            this.DO_Disable_Tray_Loading.Value = node.ChildNodes[1].InnerText;
                            break;
                        case "Threshold_Trigger":
                            this.Threshold_Trigger.Name = node.ChildNodes[0].InnerText;
                            this.Threshold_Trigger.Value = node.ChildNodes[1].InnerText;
                            break;
                        case "Timer_Interval_Modbus":
                            this.Timer_Interval_Modbus.Name = node.ChildNodes[0].InnerText;
                            this.Timer_Interval_Modbus.Value = node.ChildNodes[1].InnerText;
                            break;
                        case "Timer_Interval_StateMachine":
                            this.Timer_Interval_StateMachine.Name = node.ChildNodes[0].InnerText;
                            this.Timer_Interval_StateMachine.Value = node.ChildNodes[1].InnerText;
                            break;
                        case "Box_Height_Min":
                            this.Box_Height_Min.Name = node.ChildNodes[0].InnerText;
                            this.Box_Height_Min.Value = node.ChildNodes[1].InnerText;
                            break;
                    }
                }
                Flag_result = "GOOD";

            }
            catch
            {
                Flag_result = "ERROR";

            }
            return Flag_result;
        }
    }

    public class Parametersv2
    {
        #region Định Nghĩa biến Structure
        public struct __Modbus
        {
            public string Name;
            public string IP;
            public int Port;
            public string Logfile;
            public int IntervalTime;
            public __Modbus(string name, string ip, int port, string logfile, int intervalltime)
            {
                Name = name;
                IP = ip;
                Port = port;
                Logfile = logfile;
                IntervalTime = intervalltime;
            }
        }

        public struct __Algorithm
        {
            public string Name;
            public string Cfg;
            public string Weights;
            public string Names;
            public int BoxHeightMin;

            public __Algorithm(string name, string cfg, string weights, string names, int boxheightmin)
            {
                Name = name;
                Cfg = cfg;
                Weights = weights;
                Names = names;
                BoxHeightMin = boxheightmin;
            }
        }

        public struct __Tower
        {
            public string Name;
            public int Green_Port;
            public int Amber_Port;
            public int Yellow_Port;
            public int Buzzer_Port;

            public __Tower(string name, int green_port, int amber_port, int yellow_port, int buzzer_port)
            {
                Name = name;
                Green_Port = green_port;
                Amber_Port = amber_port;
                Yellow_Port = yellow_port;
                Buzzer_Port = buzzer_port;
            }
        }

        public struct __Parameter
        {
            public string Name;
            public string Value;
            public __Parameter(string name, string value)
            {
                Name = name;
                Value = value;
            }
        }
        #endregion

        #region Khai báo biến và khởi tạo value default
        public string Xmlfile = "sources/parameters.xml";
        public __Modbus Modbus = new __Modbus("Wago 750-324", "172.19.20.201", 502, "Modbus_IO_Log.txt", 50);
        public __Algorithm Algorithm = new __Algorithm(
            "yolov3",
            "sources/algorithm/cfg/yolov3-itn-fullbox.cfg",
            "sources/algorithm/cfg/yolov3-itn-fullbox_last.weights",
            "sources/algorithm/cfg/itn_fullbox.names",
            40
         );
        public int DI_Tray_Present_Sensor = 3;
        public int Threshold_Trigger = 10;
        public int Timer_Interval_StateMachine = 50;
        public DataTable Paras = new DataTable();
        #endregion

        #region Init, startup cho Class 
        public Parametersv2()
        {

        }
        #endregion

        #region Định nghĩa các Hàm, Method của Class
        public DataTable FromDefault()
        {
            DataTable Paras = new DataTable("Parameters");
            Paras.Columns.Add("id");
            Paras.Columns.Add("key");
            Paras.Columns.Add("value");
            Paras.Columns.Add("description");

            DataColumn[] PrimaryKeyColumns = new DataColumn[1];
            PrimaryKeyColumns[0] = Paras.Columns["key"];
            Paras.PrimaryKey = PrimaryKeyColumns;

            Paras.Rows.Add(new Object[] { 1, "Modbus_Server_Name", "Wago 750-324", "na" });
            Paras.Rows.Add(new Object[] { 2, "Modbus_Server_IP", "172.19.20.201", "na" });
            Paras.Rows.Add(new Object[] { 3, "Modbus_Server_Port", "502", "Always 502" });
            Paras.Rows.Add(new Object[] { 4, "Modbus_Server_LogFileFilename", "Modbus_IO_Log.txt", "na" });
            Paras.Rows.Add(new Object[] { 5, "Yolov3_Cfg", "sources/algorithm/cfg/yolov3-itn-fullbox.cfg", "na" });
            Paras.Rows.Add(new Object[] { 6, "Yolov3_Weights", "sources/algorithm/cfg/yolov3-itn-fullbox_last.weights", "na" });
            Paras.Rows.Add(new Object[] { 7, "Yolov3_Names", "sources/algorithm/cfg/itn_fullbox.names", "na" });
            Paras.Rows.Add(new Object[] { 8, "DI_Tray_Present_Sensor", "0", "pin number (-1 is off)" });
            Paras.Rows.Add(new Object[] { 9, "DO_Red_Light", "0", "pin number (-1 is off)" });
            Paras.Rows.Add(new Object[] { 10, "DO_Amber_Light", "-1", "pin number (-1 is off)" });
            Paras.Rows.Add(new Object[] { 11, "DO_Green_Light", "1", "pin number (-1 is off)" });
            Paras.Rows.Add(new Object[] { 12, "DO_Buzzer", "2", "pin number (-1 is off)" });
            Paras.Rows.Add(new Object[] { 13, "DO_Disable_Tray_Loading", "3", "pin number (-1 is off)" });
            Paras.Rows.Add(new Object[] { 14, "Threshold_Trigger", "10", "ms" });
            Paras.Rows.Add(new Object[] { 15, "Timer_Interval_Modbus", "50", "ms" });
            Paras.Rows.Add(new Object[] { 16, "Timer_Interval_StateMachine", "50", "ms" });
            Paras.Rows.Add(new Object[] { 17, "Box_Height_Min", "40", "pixels" });

            this.Paras = Paras;
            return (Paras);
        }

        public DataTable FromXML()
        {
            DataSet TempDataset = new DataSet();
            TempDataset.ReadXml(Xmlfile);
            Paras = TempDataset.Tables["Parameters"];
            return (Paras);
        }

        public string GetParasValue(string Searchkey)
        {
            string expression = $"key=\'{Searchkey}\'";
            DataRow[] Temprow = Paras.Select(expression);
            string result = Temprow[0]["value"].ToString();
            return (result);
        }

        public string SearchValue(DataTable Parameters, string Searchkey)
        {
            string expression = $"key=\'{Searchkey}\'";
            DataRow[] Temprow = Parameters.Select(expression);
            string result = Temprow[0]["value"].ToString();
            return (result);
        }

        public void ToXML()
        {
            Paras.WriteXml(Xmlfile);
        }
        #endregion
    }

    public class Parametersv3
    {
        #region Định Nghĩa biến Structure
        public struct __Modbus
        {
            public string Name;
            public string IP;
            public int Port;
            public string LogFile;
            public int IntervalTime;
            public __Modbus(string name, string ip, int port, string logfile, int intervalltime)
            {
                Name = name;
                IP = ip;
                Port = port;
                LogFile = logfile;
                IntervalTime = intervalltime;
            }
        }

        public struct __Algorithm
        {
            public string Name;
            public string Cfg;
            public string Weights;
            public string Names;
            public int BoxHeightMin;

            public __Algorithm(string name, string cfg, string weights, string names, int boxheightmin)
            {
                Name = name;
                Cfg = cfg;
                Weights = weights;
                Names = names;
                BoxHeightMin = boxheightmin;
            }
        }

        public struct __Tower
        {
            public string Name;
            public int Red_Port;
            public int Amber_Port;
            public int Green_Port;
            public int Buzzer_Port;

            public __Tower(string name, int red_port, int amber_port, int green_port, int buzzer_port)
            {
                Name = name;
                Red_Port = red_port;
                Amber_Port = amber_port;
                Green_Port = green_port;
                Buzzer_Port = buzzer_port;
            }
        }

        public struct __Parameter
        {
            public string Name;
            public string Value;
            public __Parameter(string name, string value)
            {
                Name = name;
                Value = value;
            }
        }


        #endregion

        #region Khai báo biến và khởi tạo value default
        public string Xmlfile = "sources/parameters.xml";
        public __Modbus Modbus;
        public __Algorithm Algorithm;
        public __Tower Tower;
        public int DO_Disable_Tray_Loading;
        public int DI_Tray_Present_Sensor;
        public int Threshold_Trigger;
        public int Timer_Interval_StateMachine;
        public Boolean Flag_Auto_Save_Image;
        //
        public DataSet ParametersDataset = new DataSet();


        #endregion Khai báo biến và khởi tạo value default

        #region Init, startup cho Class 
        public Parametersv3()
        {
            try
            {
                FromXml();
            }
            catch
            {
                FromDefault();
            }
            //
            ToDataSet();
        }
        #endregion

        #region Định nghĩa các Hàm, Method của Class
        public void FromDefault()
        {
            Modbus = new __Modbus("Wago 750-324", "172.19.20.201", 502, "modbus.log", 50);
            Algorithm = new __Algorithm(
                "yolov3",
                "sources/algorithm/cfg/yolov3-itn-fullbox.cfg",
                "sources/algorithm/cfg/yolov3-itn-fullbox_last.weights",
                "sources/algorithm/cfg/itn_fullbox.names",
                40
             );
            Tower = new __Tower("Tower", 0, -1, 1, 2);
            this.DO_Disable_Tray_Loading = 3;
            this.DI_Tray_Present_Sensor = 0;
            this.Threshold_Trigger = 10;
            this.Timer_Interval_StateMachine = 50;
            this.Flag_Auto_Save_Image = false;
        }

        public void ToXml()
        {
            XmlNode TempNode;
            XmlAttribute TempAttribute;
            XmlDocument xmlDoc = new XmlDocument();
            //root node
            XmlNode rootNode = xmlDoc.CreateElement("Parameters");
            xmlDoc.AppendChild(rootNode);

            #region Modbus
            XmlNode ModbusesNode = xmlDoc.CreateElement("Modbuses");
            rootNode.AppendChild(ModbusesNode);

            XmlNode modbusNode = xmlDoc.CreateElement("Modbus");
            TempAttribute = xmlDoc.CreateAttribute("Name");
            TempAttribute.Value = Modbus.Name;
            modbusNode.Attributes.Append(TempAttribute);

            TempAttribute = xmlDoc.CreateAttribute("IP");
            TempAttribute.Value = Modbus.IP;
            modbusNode.Attributes.Append(TempAttribute);

            TempAttribute = xmlDoc.CreateAttribute("Port");
            TempAttribute.Value = Modbus.Port.ToString();
            modbusNode.Attributes.Append(TempAttribute);

            TempAttribute = xmlDoc.CreateAttribute("LogFile");
            TempAttribute.Value = Modbus.LogFile;
            modbusNode.Attributes.Append(TempAttribute);

            TempAttribute = xmlDoc.CreateAttribute("IntervalTime");
            TempAttribute.Value = Modbus.IntervalTime.ToString();
            modbusNode.Attributes.Append(TempAttribute);

            ModbusesNode.AppendChild(modbusNode);
            #endregion Modbus

            #region Algorithms
            XmlNode AlgorithmsNode = xmlDoc.CreateElement("Algorithms");
            rootNode.AppendChild(AlgorithmsNode);

            XmlNode AlgorithmNode = xmlDoc.CreateElement("Algorithm");
            TempAttribute = xmlDoc.CreateAttribute("Name");
            TempAttribute.Value = Algorithm.Name;
            AlgorithmNode.Attributes.Append(TempAttribute);

            TempAttribute = xmlDoc.CreateAttribute("Cfg");
            TempAttribute.Value = Algorithm.Cfg;
            AlgorithmNode.Attributes.Append(TempAttribute);

            TempAttribute = xmlDoc.CreateAttribute("Weights");
            TempAttribute.Value = Algorithm.Weights;
            AlgorithmNode.Attributes.Append(TempAttribute);

            TempAttribute = xmlDoc.CreateAttribute("Names");
            TempAttribute.Value = Algorithm.Names;
            AlgorithmNode.Attributes.Append(TempAttribute);

            TempAttribute = xmlDoc.CreateAttribute("BoxHeightMin");
            TempAttribute.Value = Algorithm.BoxHeightMin.ToString();
            AlgorithmNode.Attributes.Append(TempAttribute);

            AlgorithmsNode.AppendChild(AlgorithmNode);
            #endregion Algorithms

            #region Tower
            XmlNode TowersNode = xmlDoc.CreateElement("Towers");
            rootNode.AppendChild(TowersNode);

            XmlNode TowerNode = xmlDoc.CreateElement("Tower");
            TempAttribute = xmlDoc.CreateAttribute("Name");
            TempAttribute.Value = Tower.Name;
            TowerNode.Attributes.Append(TempAttribute);

            TempAttribute = xmlDoc.CreateAttribute("Red_Port");
            TempAttribute.Value = Tower.Red_Port.ToString();
            TowerNode.Attributes.Append(TempAttribute);

            TempAttribute = xmlDoc.CreateAttribute("Amber_Port");
            TempAttribute.Value = Tower.Amber_Port.ToString();
            TowerNode.Attributes.Append(TempAttribute);

            TempAttribute = xmlDoc.CreateAttribute("Green_Port");
            TempAttribute.Value = Tower.Green_Port.ToString();
            TowerNode.Attributes.Append(TempAttribute);

            TempAttribute = xmlDoc.CreateAttribute("Buzzer_Port");
            TempAttribute.Value = Tower.Buzzer_Port.ToString();
            TowerNode.Attributes.Append(TempAttribute);

            TowersNode.AppendChild(TowerNode);
            #endregion Tower

            #region other
            XmlNode OthersNode = xmlDoc.CreateElement("Others");
            rootNode.AppendChild(OthersNode);

            TempNode = xmlDoc.CreateElement("DO_Disable_Tray_Loading");
            TempNode.InnerText = DO_Disable_Tray_Loading.ToString();
            OthersNode.AppendChild(TempNode);

            TempNode = xmlDoc.CreateElement("DI_Tray_Present_Sensor");
            TempNode.InnerText = DI_Tray_Present_Sensor.ToString();
            OthersNode.AppendChild(TempNode);

            TempNode = xmlDoc.CreateElement("Threshold_Trigger");
            TempNode.InnerText = Threshold_Trigger.ToString();
            OthersNode.AppendChild(TempNode);

            TempNode = xmlDoc.CreateElement("Timer_Interval_StateMachine");
            TempNode.InnerText = Timer_Interval_StateMachine.ToString();
            OthersNode.AppendChild(TempNode);

            TempNode = xmlDoc.CreateElement("Flag_Auto_Save_Image");
            TempNode.InnerText = Flag_Auto_Save_Image.ToString();
            OthersNode.AppendChild(TempNode);

            #endregion other

            // Save xml
            xmlDoc.Save(Xmlfile);
        }

        public void FromXml()
        {
            XmlNodeList TempNode;
            XmlDocument TempDoc = new XmlDocument();
            TempDoc.Load(Xmlfile);

            TempNode = TempDoc.SelectNodes("//Parameters/Modbuses/Modbus");
            Modbus.Name = TempNode[0].Attributes["Name"].Value;
            Modbus.IP = TempNode[0].Attributes["IP"].Value;
            Modbus.Port = int.Parse(TempNode[0].Attributes["Port"].Value);
            Modbus.LogFile = TempNode[0].Attributes["LogFile"].Value;
            Modbus.IntervalTime = int.Parse(TempNode[0].Attributes["IntervalTime"].Value);


            TempNode = TempDoc.SelectNodes("//Parameters/Algorithms/Algorithm");
            Algorithm.Name = TempNode[0].Attributes["Name"].Value;
            Algorithm.Cfg = TempNode[0].Attributes["Cfg"].Value;
            Algorithm.Weights = TempNode[0].Attributes["Weights"].Value;
            Algorithm.Names = TempNode[0].Attributes["Names"].Value;
            Algorithm.BoxHeightMin = int.Parse(TempNode[0].Attributes["BoxHeightMin"].Value);


            TempNode = TempDoc.SelectNodes("//Parameters/Towers/Tower");
            Tower.Name = TempNode[0].Attributes["Name"].Value;
            Tower.Red_Port = int.Parse(TempNode[0].Attributes["Red_Port"].Value);
            Tower.Amber_Port = int.Parse(TempNode[0].Attributes["Amber_Port"].Value);
            Tower.Green_Port = int.Parse(TempNode[0].Attributes["Green_Port"].Value);
            Tower.Buzzer_Port = int.Parse(TempNode[0].Attributes["Buzzer_Port"].Value);

            TempNode = TempDoc.SelectNodes("//Parameters/Others/DO_Disable_Tray_Loading");
            DO_Disable_Tray_Loading = int.Parse(TempNode[0].InnerText);

            TempNode = TempDoc.SelectNodes("//Parameters/Others/DI_Tray_Present_Sensor");
            DI_Tray_Present_Sensor = int.Parse(TempNode[0].InnerText);

            TempNode = TempDoc.SelectNodes("//Parameters/Others/Threshold_Trigger");
            Threshold_Trigger = int.Parse(TempNode[0].InnerText);

            TempNode = TempDoc.SelectNodes("//Parameters/Others/Timer_Interval_StateMachine");
            Timer_Interval_StateMachine = int.Parse(TempNode[0].InnerText);

            TempNode = TempDoc.SelectNodes("//Parameters/Others/Flag_Auto_Save_Image");
            Flag_Auto_Save_Image = Boolean.Parse(TempNode[0].InnerText);
        }

        public void ToDataSet()
        {
            DataSet TempDataSet = new DataSet();

            DataTable ModbusTable = new DataTable("Modbus");
            DataTable AlgorithmTable = new DataTable("Algorithm");
            DataTable TowerTable = new DataTable("Tower");
            DataTable OtherTable = new DataTable("Other");

            ModbusTable.Columns.Add("Name", typeof(string));
            ModbusTable.Columns.Add("IP", typeof(string));
            ModbusTable.Columns.Add("Port", typeof(int));
            ModbusTable.Columns.Add("LogFile", typeof(string));
            ModbusTable.Columns.Add("IntervalTime", typeof(int));
            ModbusTable.Rows.Add(new Object[] { Modbus.Name, Modbus.IP, Modbus.Port, Modbus.LogFile, Modbus.IntervalTime });

            AlgorithmTable.Columns.Add("Name", typeof(string));
            AlgorithmTable.Columns.Add("Cfg", typeof(string));
            AlgorithmTable.Columns.Add("Weights", typeof(string));
            AlgorithmTable.Columns.Add("Names", typeof(string));
            AlgorithmTable.Columns.Add("BoxHeightMin", typeof(int));
            AlgorithmTable.Rows.Add(new Object[] { Algorithm.Name, Algorithm.Cfg, Algorithm.Weights, Algorithm.Names, Algorithm.BoxHeightMin });

            TowerTable.Columns.Add("Name", typeof(string));
            TowerTable.Columns.Add("Red_Port", typeof(int));
            TowerTable.Columns.Add("Amber_Port", typeof(int));
            TowerTable.Columns.Add("Green_Port", typeof(int));
            TowerTable.Columns.Add("Buzzer_Port", typeof(int));
            TowerTable.Rows.Add(new object[] { Tower.Name, Tower.Red_Port, Tower.Amber_Port, Tower.Green_Port, Tower.Buzzer_Port });

            OtherTable.Columns.Add("Name", typeof(string));
            OtherTable.Columns.Add("Value", typeof(string));
            OtherTable.Rows.Add(new object[] { "DO_Disable_Tray_Loading", DO_Disable_Tray_Loading });
            OtherTable.Rows.Add(new object[] { "DI_Tray_Present_Sensor", DI_Tray_Present_Sensor });
            OtherTable.Rows.Add(new object[] { "Threshold_Trigger", Threshold_Trigger });
            OtherTable.Rows.Add(new object[] { "Timer_Interval_StateMachine", Timer_Interval_StateMachine });
            OtherTable.Rows.Add(new object[] { "Flag_Auto_Save_Image", Flag_Auto_Save_Image });

            //
            TempDataSet.Tables.Add(ModbusTable);
            TempDataSet.Tables.Add(AlgorithmTable);
            TempDataSet.Tables.Add(TowerTable);
            TempDataSet.Tables.Add(OtherTable);

            ParametersDataset = TempDataSet;
        }

        public void FromDataSet()
        {
            DataRow TempRow;
            DataRow[] TempRow2;
            string Searchkey;
            string expression;

            TempRow = ParametersDataset.Tables["Modbus"].Rows[0];

            Modbus.Name = TempRow["Name"].ToString();
            Modbus.IP = TempRow["IP"].ToString();
            Modbus.Port = (int)TempRow["Port"];
            Modbus.LogFile = TempRow["LogFile"].ToString();
            Modbus.IntervalTime = (int)TempRow["IntervalTime"];

            TempRow = ParametersDataset.Tables["Algorithm"].Rows[0];
            Algorithm.Name = TempRow["Name"].ToString();
            Algorithm.Cfg = TempRow["Cfg"].ToString();
            Algorithm.Weights = TempRow["Weights"].ToString();
            Algorithm.Names = TempRow["Names"].ToString();
            Algorithm.BoxHeightMin = (int)TempRow["BoxHeightMin"];


            TempRow = ParametersDataset.Tables["Tower"].Rows[0];
            Tower.Name = TempRow["Name"].ToString();
            Tower.Red_Port = (int)TempRow["Red_Port"];
            Tower.Amber_Port = (int)TempRow["Amber_Port"];
            Tower.Green_Port = (int)TempRow["Green_Port"];
            Tower.Buzzer_Port = (int)TempRow["Buzzer_Port"];

            Searchkey = "DO_Disable_Tray_Loading";
            expression = $"Name=\'{Searchkey}\'";
            //ParametersDataset.Tables["Other"].Columns["Name"].
            TempRow2 = ParametersDataset.Tables["Other"].Select(expression);
            DO_Disable_Tray_Loading = int.Parse(TempRow2[0][1].ToString());

            Searchkey = "DI_Tray_Present_Sensor";
            expression = $"Name=\'{Searchkey}\'";
            TempRow2 = ParametersDataset.Tables["Other"].Select(expression);
            DI_Tray_Present_Sensor = int.Parse(TempRow2[0][1].ToString());

            Searchkey = "Threshold_Trigger";
            expression = $"Name=\'{Searchkey}\'";
            TempRow2 = ParametersDataset.Tables["Other"].Select(expression);
            Threshold_Trigger = int.Parse(TempRow2[0][1].ToString());

            Searchkey = "Timer_Interval_StateMachine";
            expression = $"Name=\'{Searchkey}\'";
            TempRow2 = ParametersDataset.Tables["Other"].Select(expression);
            Timer_Interval_StateMachine = int.Parse(TempRow2[0][1].ToString());

            Searchkey = "Flag_Auto_Save_Image";
            expression = $"Name=\'{Searchkey}\'";
            TempRow2 = ParametersDataset.Tables["Other"].Select(expression);
            Flag_Auto_Save_Image = Boolean.Parse(TempRow2[0][1].ToString());
        }

        #endregion Định nghĩa các Hàm, Method của Class
    }

    public class Products
    {
        #region Định Nghĩa Struct
        public struct __Product
        {
            public int ID;
            public string TrayID;
            public string Product;

            public __Product(int id, string trayid, string product)
            {
                ID = id;
                TrayID = trayid;
                Product = product;
            }
        }
        #endregion Định Nghĩa Struct

        #region Khai báo biến và khởi tạo value default
        public string Xmlfile = "sources/products.xml";
        public string XmlfileSchema = "sources/products_schema.xml";
        public DataTable ProductsTable;
        public List<string> TrayIDList;

        //public DataSet ProductsDataSet;

        #endregion Khai báo biến và khởi tạo value default
        public Products(){
            FromDefault();
            //FromXmlDataTable();
        }

        public void FromDefault()
        {
            //ProductsList
            //DataSet TempDataSet = new DataSet("Products");
            DataTable TempDataTable = new DataTable("Product");

            TempDataTable.Columns.Add("ID",typeof(int));
            TempDataTable.Columns.Add("TrayID", typeof(string));
            TempDataTable.Columns.Add("Product", typeof(string));

            TempDataTable.Rows.Add(new Object[] { 0, "500253654", "Molded XG736" });
            TempDataTable.Rows.Add(new Object[] { 1, "500289821", "Jefferson Peak" });
            TempDataTable.Rows.Add(new Object[] { 2, "500237051", "Molded Titan Ridge" });
            TempDataTable.Rows.Add(new Object[] { 3, "500349947", "XG-756 B0 ICE" });
            TempDataTable.Rows.Add(new Object[] { 4, "500289821", "Jefferson Peak A1" });
            TempDataTable.Rows.Add(new Object[] { 5, "500289821", "Harrison Peak 2" });
            TempDataTable.Rows.Add(new Object[] { 6, "500377825", "XG-766 250UM STREET" });
            TempDataTable.Rows.Add(new Object[] { 7, "500237051", "Alpine Ridge" });
            TempDataTable.Rows.Add(new Object[] { 8, "500209901", "Cooper Bridge" });
            TempDataTable.Rows.Add(new Object[] { 9, "500125525", "XG727" });
            TempDataTable.Rows.Add(new Object[] { 10, "500319672", "Sue Creek 150 Mold Cap" });
            TempDataTable.Rows.Add(new Object[] { 11, "500316867", "Monette Hill 550" });
            TempDataTable.Rows.Add(new Object[] { 12, "500288750", "XG748" });
            TempDataTable.Rows.Add(new Object[] { 13, "500322975", "Bandos A0" });
            TempDataTable.Rows.Add(new Object[] { 14, "500209901", "Delta Bridge" });
            TempDataTable.Rows.Add(new Object[] { 15, "500289820", "Thunder peak 2" });
            TempDataTable.Rows.Add(new Object[] { 16, "500337384", "XG-742 IBIS DC" });
            TempDataTable.Rows.Add(new Object[] { 17, "500209901", "Burnside Bridge" });
            TempDataTable.Rows.Add(new Object[] { 18, "500337384", "XG-743" });
            TempDataTable.Rows.Add(new Object[] { 19, "500209901", "Mystery Ridge 3 TV" });
            TempDataTable.Rows.Add(new Object[] { 20, "500300257", "Cannon Lake PCH Mold" });
            TempDataTable.Rows.Add(new Object[] { 21, "500237051", "Goshen Ridge" });
            TempDataTable.Rows.Add(new Object[] { 22, "500237051", "Maple Ridge" });
            TempDataTable.Rows.Add(new Object[] { 23, "500355085", "Elixir Springs" });
            TempDataTable.Rows.Add(new Object[] { 24, "500390261", "Harrison Peak 1 iSFE" });
            TempDataTable.Rows.Add(new Object[] { 25, "500125525", "Lakefield" });

            ProductsTable = TempDataTable;
            GetTrayIDList();
            //TempDataSet.Tables.Add(TempDataTable);
            //ProductsDataSet = TempDataSet;
        }

        public void ToXml()
        {
            ProductsTable.WriteXmlSchema(XmlfileSchema);
            ProductsTable.WriteXml(Xmlfile);
        }
        public void ToXmlDataTable()
        {

            ProductsTable.WriteXmlSchema(XmlfileSchema);
            ProductsTable.WriteXml(Xmlfile);
        }

        public void FromXml()
        {
            DataTable TempDataTable = new DataTable("Product");
            TempDataTable.ReadXmlSchema(XmlfileSchema);
            TempDataTable.ReadXml(Xmlfile);
            ProductsTable = TempDataTable;
            GetTrayIDList();
        }

        public void FromXmlDataTable()
        {
            DataTable TempDataTable = new DataTable("Product");
            TempDataTable.ReadXmlSchema(XmlfileSchema);
            TempDataTable.ReadXml(Xmlfile);
            ProductsTable = TempDataTable;
            GetTrayIDList();
        }
        public void GetTrayIDList()
        {
            TrayIDList = new List<string>();
            for (int tempi = 0; tempi < ProductsTable.Rows.Count; tempi++)
            {
                TrayIDList.Add($"{ProductsTable.Rows[tempi]["ID"].ToString()}|{ProductsTable.Rows[tempi]["TrayID"]}|{ProductsTable.Rows[tempi]["Product"]}");
            }
        }
    }
}
