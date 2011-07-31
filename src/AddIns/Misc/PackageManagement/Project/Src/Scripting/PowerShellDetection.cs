// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using Microsoft.Win32;

namespace ICSharpCode.PackageManagement.Scripting
{
	public class PowerShellDetection : IPowerShellDetection
	{
		public static readonly string PowerShellRegistryKeyName = 
			@"SOFTWARE\Microsoft\PowerShell\1\PowerShellEngine";
		
		public static readonly string PowerShellVersionRegistryValueName =
			"PowerShellVersion";
		
		public bool? installed;
		
		public bool IsPowerShell2Installed()
		{
			if (!installed.HasValue) {
				installed = CheckIfPowerShell2IsInstalled();
			}
			return installed.Value;
		}
		
		bool CheckIfPowerShell2IsInstalled()
		{
			RegistryKey key = OpenPowerShellRegistryKey();
			if (key != null) {
				using (key) {
					return IsPowerShell2Installed(key);
				}
			}
			return false;
		}
		
		RegistryKey OpenPowerShellRegistryKey()
		{
			return Registry.LocalMachine.OpenSubKey(PowerShellRegistryKeyName);
		}
		
		bool IsPowerShell2Installed(RegistryKey key)
		{
			string readValue = GetPowerShellVersion(key);
			return readValue == "2.0";
		}		
		
		string GetPowerShellVersion(RegistryKey key)
		{
			return (string)key.GetValue(PowerShellVersionRegistryValueName);
		}
	}
}
