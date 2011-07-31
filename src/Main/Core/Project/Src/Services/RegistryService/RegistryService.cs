// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Globalization;
using Microsoft.Win32;

namespace ICSharpCode.Core
{
	/// <summary>
	/// RegistryService.
	/// </summary>
	public static class RegistryService
	{
		/// <summary>
		/// Gets the registry value.
		/// </summary>
		/// <param name="hive">Registry hive.</param>
		/// <param name="key">Registry key.</param>
		/// <param name="value">Registry value.</param>
		/// <param name="kind">Registry kind.</param>
		/// <param name="data">Data.</param>
		/// <returns>True, if the data was found, False otherwise.</returns>
		public static bool GetRegistryValue<T>(RegistryHive hive, string key, string value, RegistryValueKind kind, out T data)
		{
			data = default(T);
			
			try {
				using (RegistryKey baseKey = RegistryKey.OpenRemoteBaseKey(hive, String.Empty))
				{
					if (baseKey != null)
					{
						using (RegistryKey registryKey = baseKey.OpenSubKey(key, RegistryKeyPermissionCheck.ReadSubTree))
						{
							if (registryKey != null)
							{
								RegistryValueKind kindFound = registryKey.GetValueKind(value);
								if (kindFound == kind)
								{
									object regValue = registryKey.GetValue(value, null);
									if (regValue != null)
									{
										data = (T)Convert.ChangeType(regValue, typeof(T), CultureInfo.InvariantCulture);
										return true;
									}
								}								
							}
						}
					}
				}
				
				return false;
			} catch {
				return false;
			}
		}
	}
}
