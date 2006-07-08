// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mathias Simmack" email="mathias@simmack.de"/>
//     <version>$Revision$</version>
// </file>

namespace HtmlHelp2.Environment
{
	using System;
	using System.IO;
	using Microsoft.Win32;

	public sealed class Help2ControlsValidation
	{
		Help2ControlsValidation()
		{
		}

		public static bool IsTocControlRegistered
		{
			get
			{
				return IsClassRegistered("{314111b8-a502-11d2-bbca-00c04f8ec294}");
			}
		}

		public static bool IsIndexControlRegistered
		{
			get
			{
				return IsClassRegistered("{314111c6-a502-11d2-bbca-00c04f8ec294}");
			}
		}

		private static bool IsClassRegistered(string classId)
		{
			try
			{
				RegistryKey tmp = Registry.ClassesRoot.OpenSubKey
					(string.Format(null, @"CLSID\{0}\InprocServer32", classId), false);
				string help2Library = (string)tmp.GetValue("", string.Empty);
				tmp.Close();
				return (!string.IsNullOrEmpty(help2Library) && File.Exists(help2Library));
			}
			catch (System.NullReferenceException)
			{
			}
			return false;
		}
	}
}
