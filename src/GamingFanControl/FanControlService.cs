using GamingFanControl.Enums;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.ServiceProcess;
using System.Threading;

namespace GamingFanControl
{
    public partial class FanControlService : ServiceBase
    {
        private Timer timerFanCheck;
        private bool gameOpened = false;
        private bool runnning = true;

        private const int INTERVAL_FAN_CHECK = 20 * 1000; // 20 secs (it may be reduced, wouldnt affect performance)

        private const string ASUS_FANCONTROL_DIR = @"C:\Program Files (x86)\ASUS\AsusFanControlService\2.01.11"; // asus fancontrol directory (your directory and version may be different)

        private Dictionary<FanProfile, string> fanstoreProfileFilenames = new Dictionary<FanProfile, string>() // fanstore file mapping  (you can reach your fanstore.xml file at asus fancontrol directory, when you apply a profile that xml will be updated)
        {
            [FanProfile.SILENT] = @"D:\backup\AsusFanProfile\FanStore_UltraSilent.xml", // your silent fanstore file path
            [FanProfile.GAMING] = @"D:\backup\AsusFanProfile\FanStore_Gaming.xml"  // your gaming fanstore file path
        };

        public FanControlService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            OnFanCheck();
        }

        void OnFanCheck()
        {
            var gameProcessFound = false;

            try
            {
                using (var key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Valve\Steam"))
                {
                    if (key != null)
                    {
                        var runningAppIdObj = key.GetValue("RunningAppID"); // steam updates this key instantly when you enter or exit a game, 0 means no game is running
                        if (runningAppIdObj is int runningAppId)
                        {
                            gameProcessFound = runningAppId != 0;
                        }
                    }
                }
            }
            catch (Exception)
            {
                // TODO: exception can be logged 
            }

            if (gameProcessFound && !gameOpened) // new game process found and not any game opened already
            {
                ApplyFanStoreProfile(FanProfile.GAMING);
                gameOpened = true;
            }
            else if (!gameProcessFound && gameOpened) // no more game process and already a game opened
            {
                ApplyFanStoreProfile(FanProfile.SILENT);
                gameOpened = false;
            }

            SetTimer(ref timerFanCheck, INTERVAL_FAN_CHECK, OnFanCheck); // method will be called periodically
        }

        protected override void OnStop()
        {
            runnning = false;
        }


        private void ApplyFanStoreProfile(FanProfile fanProfile)
        {
            try
            {
                var fanstoreFilename = fanstoreProfileFilenames[fanProfile];

                var serviceController = new ServiceController();
                serviceController.ServiceName = "AsusFanControlService"; // constant, the name of Asus Fan Control Service 

                if (serviceController.Status == ServiceControllerStatus.Running) // we should check the status, if its not running, trying to stop it will cause throwing exception. Same rule applies to Start() below
                {
                    serviceController.Stop();
                    serviceController.WaitForStatus(ServiceControllerStatus.Stopped);
                }

                File.Copy(fanstoreFilename, $@"{ASUS_FANCONTROL_DIR}\FanStore.xml", true);

                if (serviceController.Status != ServiceControllerStatus.Running)
                {
                    serviceController.Start();
                    serviceController.WaitForStatus(ServiceControllerStatus.Running);
                }
            }
            catch (Exception)
            {
                // TODO: exception can be logged 
            }
        }

        protected void SetTimer(ref Timer timer, int interval, Action action)
        {
            if (timer == null)
                timer = new Timer(_ => action(), null, interval, Timeout.Infinite);
            else if (runnning)
                timer.Change(interval, Timeout.Infinite);
        }
    }

}
