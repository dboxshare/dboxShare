using dboxSyncer;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;
using System.Windows.Forms;


namespace dboxSyncer
{


    sealed class SyncTask
    {


        public static void Main()
        {
            if (Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName).Length - 1 > 0)
            {
                Application.Exit();
            }

            TimeSpan TS = new TimeSpan();

            try
            {
                do
                {
                    TS = DateTime.Now.AddMinutes(AppConfig.IntervalTime.TypeInt()) - DateTime.Now;

                    Thread.Sleep((int)TS.TotalMilliseconds);

                    if (AppCommon.ProcessExist("dboxSyncer") == false)
                    {
                        Application.Exit();
                    }

                    if (AppCommon.ProcessExist("dboxSyncer.Process") == false)
                    {
                        AppCommon.ProcessExecute(AppCommon.PathCombine(AppConfig.AppPath, "dboxSyncer.Process.exe"), "", 0);
                    }
                } while (true);
            }
            catch (Exception ex)
            {
                File.AppendAllText(AppCommon.PathCombine(AppConfig.AppPath, "error.log"), "" + DateTime.Now.ToString() + "\n" + ex.ToString() + "\n\n");
            }
            finally
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }


    }


}