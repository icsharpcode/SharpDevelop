// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
			get {
				Process[] agents = Process.GetProcessesByName("HelpLibAgent");
				LoggingService.Debug(string.Format("HelpViewer: {0} HelpLibraryAgent {1} found", agents.Length, (agents.Length == 1)?"process":"processes"));
				return agents.Length > 0;
			}
		}

		public static string Agent
		{
			get {
				if (string.IsNullOrEmpty(Help3Environment.AppRoot)) return string.Empty;
				string agent = Path.Combine(Help3Environment.AppRoot, "HelpLibAgent.exe");
				LoggingService.Debug(string.Format("HelpViewer: HelpLibraryAgent is \"{0}\"", agent));
				return (File.Exists(agent)) ? agent : string.Empty;
			}
		}

		public static int PortNumber
		{
			get {
				try {
					RegistryKey hklm = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64).OpenSubKey(@"SOFTWARE\Microsoft\Help\v1.0", false);
					string port = (string)hklm.GetValue("AgentPort", "47873");
					hklm.Close();
					return Convert.ToInt32(port);
				}
				catch (Exception ex) {
					LoggingService.Error(string.Format("HelpViewer: {0}", ex.ToString()));
				}
				return 47873; // This is the DEFAULT port number!
			}
		}

		public static bool PortIsReady
		{
			get {
				try {
					Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
					socket.Connect(IPAddress.Parse("127.0.0.1"), PortNumber);
					bool isReady = socket.Connected;
					socket.Close();
					return isReady;
				}
				catch (SocketException ex) {
					if (ex.ErrorCode == 10061) {
						LoggingService.Debug("HelpViewer: Port is available but not ready");
						return true;
					}
					LoggingService.Error(string.Format("HelpViewer: {0}", ex.ToString()));
				}
				return false;
			}
		}

		public static int ProcessId
		{
			get {
				Process[] agents = Process.GetProcessesByName("HelpLibAgent");
				int processId = (agents.Length > 0) ? agents[0].Id:0;
				LoggingService.Debug(string.Format("HelpViewer: HelpLibraryAgent has process ID \"{0}\"", processId));
				return processId;
			}
		}

		public static string CurrentViewer
		{
			get {
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
					LoggingService.Error(string.Format("HelpViewer: {0}", ex.ToString()));
				}
				LoggingService.Debug(string.Format("HelpViewer: Default viewer is \"{0}\"", viewer));
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
				LoggingService.Info("HelpViewer: HelpLibraryAgent started");
				return IsRunning;
			}
			catch (Exception ex) {
				LoggingService.Error(string.Format("HelpViewer: {0}", ex.ToString()));
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
				LoggingService.Debug(string.Format("HelpViewer: {0} HelpLibraryAgent {1} stopped", agents.Length, (agents.Length == 1)?"process":"processes"));
			}
			catch (Exception ex) {
				LoggingService.Error(string.Format("HelpViewer: {0}", ex.ToString()));
			}			
			return true;
		}
	}
}
