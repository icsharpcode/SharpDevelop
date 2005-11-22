/* ***********************************************************
 *
 * Help 2.0 Environment for SharpDevelop
 * Base Help 2.0 Services (Help 2.0 Controls Validation)
 * Copyright (c) 2005, Mathias Simmack. All rights reserved.
 *
 * ********************************************************* */
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

		public Help2ControlsValidation()
		{
		}

		static Help2ControlsValidation()
		{
		}
	}
}
