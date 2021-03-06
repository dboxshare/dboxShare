﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Xml;
using dboxShare.Base;


namespace dboxShare
{


    sealed class Startup
    {


        public static string AppPath
        {
            get
            {
                return AppDomain.CurrentDomain.BaseDirectory;
            }
        }


        public static void Main()
        {
            if (Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName).Length > 1)
            {
                Environment.Exit(0);
            }

            try
            {
                var XDocument = new XmlDocument();

                XDocument.Load(Base.Common.PathCombine(AppPath, "startup.xml"));

                var XNodes = XDocument.SelectSingleNode("/list");

                foreach (XmlNode XNode in XNodes.ChildNodes)
                {
                    var Exec = XNode.Attributes["exec"].Value;

                    if (string.IsNullOrEmpty(Exec) == true)
                    {
                        return;
                    }

                    var Cmd = XNode.Attributes["cmd"].Value;

                    Base.Common.ProcessExecute(Exec, Cmd, -1);
                }
            }
            catch (Exception) { }
        }


    }


}