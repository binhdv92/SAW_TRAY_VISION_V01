using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace SAW_TRAY_VISION_V01.sources
{
    class ProductsList
    {
        public string _ProductFileName= AppDomain.CurrentDomain.BaseDirectory + @"sources\ProductLists.csv";

        public string[] _ProductLists_Str;
   

        public void LoadProductLists_Str()
        {
            this._ProductLists_Str = File.ReadAllLines(Path.ChangeExtension(_ProductFileName, "csv"));
        }
    }
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
    class Parameters
    {
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
                            this.Modbus_Server_Name.Name  = node.ChildNodes[0].InnerText;
                            this.Modbus_Server_Name.Value = node.ChildNodes[1].InnerText;
                            break;
                        case "Modbus_Server_IP":
                            this.Modbus_Server_IP.Name  = node.ChildNodes[0].InnerText;
                            this.Modbus_Server_IP.Value = node.ChildNodes[1].InnerText;
                            break;
                        case "Modbus_Server_Port":
                            this.Modbus_Server_Port.Name  = node.ChildNodes[0].InnerText;
                            this.Modbus_Server_Port.Value = node.ChildNodes[1].InnerText;
                            break;
                        case "Modbus_Server_LogFileFilename":
                            this.Modbus_Server_LogFileFilename.Name  = node.ChildNodes[0].InnerText;
                            this.Modbus_Server_LogFileFilename.Value = node.ChildNodes[1].InnerText;
                            break;
                        case "Yolov3_Cfg":
                            this.Yolov3_Cfg.Name  = node.ChildNodes[0].InnerText;
                            this.Yolov3_Cfg.Value = node.ChildNodes[1].InnerText;
                            break;
                        case "Yolov3_Weights":
                            this.Yolov3_Weights.Name  = node.ChildNodes[0].InnerText;
                            this.Yolov3_Weights.Value = node.ChildNodes[1].InnerText;
                            break;
                        case "Yolov3_Names":
                            this.Yolov3_Names.Name  = node.ChildNodes[0].InnerText;
                            this.Yolov3_Names.Value = node.ChildNodes[1].InnerText;
                            break;
                        case "DI_Tray_Present_Sensor":
                            this.DI_Tray_Present_Sensor.Name  = node.ChildNodes[0].InnerText;
                            this.DI_Tray_Present_Sensor.Value = node.ChildNodes[1].InnerText;
                            break;
                        case "DO_Red_Light":
                            this.DO_Red_Light.Name  = node.ChildNodes[0].InnerText;
                            this.DO_Red_Light.Value = node.ChildNodes[1].InnerText;
                            break;
                        case "DO_Amber_Light":
                            this.DO_Amber_Light.Name  = node.ChildNodes[0].InnerText;
                            this.DO_Amber_Light.Value = node.ChildNodes[1].InnerText;
                            break;
                        case "DO_Green_Light":
                            this.DO_Green_Light.Name  = node.ChildNodes[0].InnerText;
                            this.DO_Green_Light.Value = node.ChildNodes[1].InnerText;
                            break;
                        case "DO_Buzzer":
                            this.DO_Buzzer.Name  = node.ChildNodes[0].InnerText;
                            this.DO_Buzzer.Value = node.ChildNodes[1].InnerText;
                            break;
                        case "DO_Disable_Tray_Loading":
                            this.DO_Disable_Tray_Loading.Name  = node.ChildNodes[0].InnerText;
                            this.DO_Disable_Tray_Loading.Value = node.ChildNodes[1].InnerText;
                            break;
                        case "Threshold_Trigger":
                            this.Threshold_Trigger.Name  = node.ChildNodes[0].InnerText;
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
        public Parameter ReadSignalParameter(string Key)
        {
            Parameter Temp = new Parameter();
            XmlDocument Doc = new XmlDocument();
            try
            {
                foreach (XmlNode node in Doc.DocumentElement)
                {
                    string NamePara = node.ChildNodes[0].InnerText;
                    if (NamePara == Key)
                    {
                        Temp.Name = node.ChildNodes[0].InnerText;
                        Temp.Value = node.ChildNodes[1].InnerText;
                    }
                }
            }
            catch
            {
                Temp.Name = null;
                Temp.Value = null;
            }
            return Temp;
        }
    }
}
