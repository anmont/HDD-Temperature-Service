using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Management;
using System.Timers;
using System.IO;

namespace HDD_Temp_Mon
{
    public partial class Service1 : ServiceBase
    {
        public static bool warn = false;
        public static bool alarm = false;
        public static Int32 warnVal = 46;
        public static Int32 alarmVal = 50;
        public static int frequency = 20;
        private System.Timers.Timer timer;
        private System.Timers.Timer timerWarn;
        private System.Timers.Timer timerAlarm;


        public Service1()
        {
            InitializeComponent();
        }

        public void OnDebug()
        {
            OnStart(null);
        }

        protected override void OnStart(string[] args)
        {
            //running code here
            this.timer = new System.Timers.Timer(20000D);  // 30000 milliseconds = 30 seconds
            this.timer.AutoReset = true;
            this.timer.Elapsed += new System.Timers.ElapsedEventHandler(this.timer_Elapsed);
            this.timer.Start();

            this.timerWarn = new System.Timers.Timer(10000D);  // 30000 milliseconds = 30 seconds
            this.timerWarn.AutoReset = true;
            this.timerWarn.Elapsed += new System.Timers.ElapsedEventHandler(this.timer_Warn);
            

            this.timerAlarm = new System.Timers.Timer(7000D);  // 30000 milliseconds = 30 seconds
            this.timerAlarm.AutoReset = true;
            this.timerAlarm.Elapsed += new System.Timers.ElapsedEventHandler(this.timer_Alarm);

        }

        protected override void OnStop()
        {
            //stopping code here
        }

        public List<byte> GetDriveTemp()
        {
            byte TEMPERATURE_ATTRIBUTE = 194;
            List<byte> retval = new List<byte>();
            try
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher("root\\WMI", "SELECT * FROM MSStorageDriver_ATAPISmartData");
                //loop through all the hard disks
                foreach (ManagementObject queryObj in searcher.Get())
                {
                    byte[] arrVendorSpecific = (byte[])queryObj.GetPropertyValue("VendorSpecific");
                    //Find the temperature attribute
                    int tempIndex = Array.IndexOf(arrVendorSpecific, TEMPERATURE_ATTRIBUTE);
                    retval.Add(arrVendorSpecific[tempIndex + 5]);
                }
            }
            catch (ManagementException err)
            {
                Console.WriteLine("An error occurred while querying for WMI data: " + err.Message);
            }
            return retval;
        }

        private void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            {
                //string text = "";//debugging
                //rtbResults.Text = "";
                List<byte> temps = GetDriveTemp();

                bool localwarn = false;
                bool localalarm = false;

                foreach (byte val in temps)
                {
                    if (Convert.ToInt32(val) >= warnVal)
                    {
                        localwarn = true;
                    }
                    if (Convert.ToInt32(val) >= alarmVal)
                    {
                        localalarm = true;
                    }
                    //text = text + val.ToString() + Environment.NewLine; //debugging
                }

                /* using (StreamWriter sw = File.AppendText(AppDomain.CurrentDomain.BaseDirectory + "debug.txt"))//debugging
                {
                    sw.Write(text);
                    sw.Dispose();
                }*/

                if (localalarm)
                {
                    //warnings off
                    warn = false;
                    timerWarn.Enabled = false;
                    //this.timerWarn..Stop();

                    //alarm on
                    //this.timerAlarm.Start();
                    alarm = true;
                    timerAlarm.Enabled = true;
                }
                else if (localwarn)
                {
                    //alarm off
                    alarm = false;
                    timerAlarm.Enabled = false;

                    //warning on
                    warn = true;
                    timerWarn.Enabled = true;

                }
                else
                {
                    warn = false;
                    alarm = false;
                    timerAlarm.Enabled = false;
                    timerWarn.Enabled = false;
                }


            }
        }

        private void timer_Warn(object sender, System.Timers.ElapsedEventArgs e)
        {
            System.Media.SoundPlayer warnAlarm = new System.Media.SoundPlayer(AppDomain.CurrentDomain.BaseDirectory + "warn.wav");
            warnAlarm.Play();
            //Console.Beep();
            System.Threading.Thread.Sleep(500);
            warnAlarm.Play();
            //Console.Beep();
        }

        private void timer_Alarm(object sender, EventArgs e)
        {
            System.Media.SoundPlayer alarmAlarm = new System.Media.SoundPlayer(AppDomain.CurrentDomain.BaseDirectory + "alarm.wav");

            alarmAlarm.Play();
            //Console.Beep();
            System.Threading.Thread.Sleep(200);
            alarmAlarm.Play();
            //Console.Beep();
            System.Threading.Thread.Sleep(200);
            alarmAlarm.Play();
            //Console.Beep();
        }

    }
}
