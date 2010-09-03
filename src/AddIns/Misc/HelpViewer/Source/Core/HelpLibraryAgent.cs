// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using Microsoft.Win32;
using ICSharpCode.Core;

namespace MSHelpSystem.Core
{
	public sealed class HelpLibraryAgent
	{
		HelpLibraryAgent()
		{
		}

		public static bool IsRunning
		{
			get
			{
				Process[] agents = Process.GetProcessesByName("HelpLibAgent");
				LoggingService.Debug(string.Format("Help 3.0: {0} {1} of HelpLibraryAgent.exe found", agents.Length, (agents.Length == 1)?"process":"processes"));
				return agents.Length > 0;
			}
		}

		public static string Agent
		{
			get
			{
				if (string.IsNullOrEmpty(Help3Environment.AppRoot)) return string.Empty;
				string agent = Path.Combine(Help3Environment.AppRoot, "HelpLibAgent.exe");
				LoggingService.Debug(string.Format("Help 3.0: Help library agent is \"{0}\"", agent));
				return (File.Exists(agent)) ? agent : string.Empty;
			}
		}

		public static int PortNumber
		{
			get
			{
				try {
					RegistryKey hklm = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64).OpenSubKey(@"SOFTWARE\Microsoft\Help\v1.0", false);
					string port = (string)hklm.GetValue("AgentPort", "47873");
					hklm.Close();
					return Convert.ToInt32(port);
				}
				catch (Exception ex) {
					LoggingService.Error(string.Format("Help 3.0: {0}", ex.ToString()));
				}
				return 47873; // This is the DEFAULT port number!
			}
		}

		public static bool PortIsReady
		{
			get
			{
				try {
					Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
					socket.Connect(IPAddress.Parse("127.0.0.1"), PortNumber);
					bool isReady = socket.Connected;
					socket.Close();
					return isReady;
				}
				catch (SocketException ex) {
					if (ex.ErrorCode == 10061) {
						LoggingService.Debug("Help 3.0: Port is available but not ready");
						return true;
					}
					LoggingService.Error(string.Format("Help 3.0: {0}", ex.ToString()));
				}
				return false;
			}
		}

		public static int ProcessId
		{
			get
			{
				Process[] agents = Process.GetProcessesByName("HelpLibAgent");
				int processId = (agents.Length > 0) ? agents[0].Id:0;
				LoggingService.Debug(string.Format("Help 3.0: Help library agent has the process ID \"{0}\"", processId));
				return processId;
			}
		}

		public static string CurrentViewer
		{
			get
			{
				string viewer = string.Empty;
				try {
					RegistryKey hklm = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64).OpenSubKey(@"SOFTWARE\Microsoft\Help\v1.0", false);
					viewer = (string)hklm.GetValue("HelpViewerProgID", string.Empty);
					hklm.Close();

					if (string.IsNullOrEmpty(viewer)) {
						RegistryKey hkcr = RegistryKey.OpenBaseKey(RegistryHive.ClassesRoot, RegistryView.Registry64).OpenSubKey(@"HTTP\shell\open\command", false);
						viewer = (string)hkcr.GetValue("", string.Empty);
						hkcr.Close();
						if (viewer.LastIndexOf(' ') > 0) viewer = viewer.Substring(0, viewer.LastIndexOf(' '));
						viewer = viewer.Trim("\"".ToCharArray());
					}
				}
				catch (Exception ex) {
					LoggingService.Error(string.Format("Help 3.0: {0}", ex.ToString()));
				}
				LoggingService.Debug(string.Format("Help 3.0: Default viewer is \"{0}\"", viewer));
				return viewer;
			}
		}

		public static bool Start()
		{
			if (IsRunning) return true;
			if (!Help3Environment.IsLocalHelp) return false;
			try {
				Process p = Process.Start(Agent);
				p.WaitForInputIdle();
				LoggingService.Info("Help 3.0: Help library agent started");
				return IsRunning;
			}
			catch (Exception ex) {
				LoggingService.Error(string.Format("Help 3.0: {0}", ex.ToString()));
			}
			return false;
		}

		public static bool Stop()
		{
			return Stop(true);
		}

		public static bool Stop(bool waitForExit)
		{
			try {
				Process[] agents = Process.GetProcessesByName("HelpLibAgent");

				foreach (Process agent in agents) {
					agent.Kill();
					if (waitForExit) agent.WaitForExit();
				}
				LoggingService.Debug(string.Format("Help 3.0: {0} {1} of HelpLibraryAgent.exe stopped", agents.Length, (agents.Length == 1)?"process":"processes"));
			}
			catch (Exception ex) {
				LoggingService.Error(string.Format("Help 3.0: {0}", ex.ToString()));
			}			
			return true;
		}
	}
}
