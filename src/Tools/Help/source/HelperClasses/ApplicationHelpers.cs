//
// Help 2.0 Registration Utility
// Copyright (c) 2005 Mathias Simmack. All rights reserved.
//
namespace HtmlHelp2Registration.HelperClasses
{
	using System;
	using System.Diagnostics;
	using System.Globalization;
	using System.IO;
	using System.Security;
	using System.Security.Permissions;
	using System.Security.Principal;
	using System.Threading;
	using Microsoft.Win32;

	public sealed class ApplicationHelpers
	{
		public const string Help2NamespaceUri = "http://www.simmack.de/2006/help2";

		ApplicationHelpers()
		{
		}

		public static bool IsClassRegistered(string classId)
		{
			try
			{
				RegistryKey tmp = Registry.ClassesRoot.OpenSubKey
					(string.Format(CultureInfo.InvariantCulture, @"CLSID\{0}\InprocServer32", classId), false);
				string help2Library = (string)tmp.GetValue("", string.Empty);
				tmp.Close();

				return (!string.IsNullOrEmpty(help2Library) && File.Exists(help2Library));
			}
			catch (ArgumentException)
			{
			}
			catch (SecurityException)
			{
			}
			return false;
		}

		[PermissionSet(SecurityAction.LinkDemand, Name="Execution")]
		public static void KillIllegalProcesses(string[] invalidProcesses)
		{
			if (invalidProcesses == null || invalidProcesses.Length == 0)
			{
				return;
			}

			Process[] runningProcesses = Process.GetProcesses();
			foreach (Process currentProcess in runningProcesses)
			{
				try
				{
					string fileName = Path.GetFileName(currentProcess.MainModule.FileName);
					foreach (string invalidProcess in invalidProcesses)
					{
						if (string.Compare(invalidProcess.ToLower(CultureInfo.InvariantCulture),
						                   fileName.ToLower(CultureInfo.InvariantCulture)) == 0)
						{
							currentProcess.Kill();
							currentProcess.WaitForExit();
						}
					}
				}
				catch (System.ComponentModel.Win32Exception)
				{
				}
			}
		}

		public static bool IsThisUserPrivileged()
		{
			switch(Environment.OSVersion.Platform)
			{
				case PlatformID.Win32NT:
					AppDomain.CurrentDomain.SetPrincipalPolicy(PrincipalPolicy.WindowsPrincipal);
					WindowsPrincipal wp = (Thread.CurrentPrincipal as WindowsPrincipal);
					return (wp.IsInRole(WindowsBuiltInRole.Administrator) || wp.IsInRole(WindowsBuiltInRole.PowerUser));
				case PlatformID.Win32Windows:
					return true;
				default:
					return false;
			}
		}
	}
}
