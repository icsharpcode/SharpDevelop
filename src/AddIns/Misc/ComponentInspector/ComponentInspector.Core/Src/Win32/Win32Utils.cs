// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Oakland Software Incorporated" email="general@oaklandsoftware.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.Text;

using Microsoft.Win32;

namespace NoGoop.Win32
{
	// Win32 utility functions
	public class Win32Utils
	{ 
		protected static void SingleRegKey(StringBuilder outStr,
									RegistryKey regKey,
									int indent)
		{
			// Just print the key and default value for now
			for (int i = 0; i < indent; i++)
				outStr.Append(" ");
			outStr.Append(regKey.Name);
			outStr.Append("/");
			outStr.Append(regKey.GetValue(null));
			outStr.Append("\n");
		}

		// Prints information about a given registry key
		public static String RegKeyToString(RegistryKey regKey)
		{
			if (regKey == null)
				return "";

			IList subKeys = regKey.GetSubKeyNames();
			StringBuilder outStr = new StringBuilder();

			SingleRegKey(outStr, regKey, 0);

			foreach (String subKeyName in subKeys)
				SingleRegKey(outStr, regKey.OpenSubKey(subKeyName), 2);
			return outStr.ToString();
		}
	}
}
