// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mathias Simmack" email="mathias@simmack.de"/>
//     <version>$Revision$</version>
// </file>

namespace HtmlHelp2.ControlsValidation
{
	using System;
	using System.IO;
	using Microsoft.Win32;

	public sealed class Help2ControlsValidation
	{
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
				using (RegistryKey tempRegKey = Registry.ClassesRoot.OpenSubKey(String.Format("CLSID\\{0}\\InprocServer32", classId)))
				{
					string help2Dll        = (string)tempRegKey.GetValue("");
					return (help2Dll != null && help2Dll != "" && File.Exists(help2Dll));
				}
			}
			catch
			{
				return false;
			}
		}
	}
}
