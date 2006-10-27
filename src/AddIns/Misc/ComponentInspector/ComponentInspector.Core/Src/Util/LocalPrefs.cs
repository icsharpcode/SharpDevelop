// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Oakland Software Incorporated" email="general@oaklandsoftware.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.Configuration;
using System.Diagnostics;

namespace NoGoop.Util
{
	public class LocalPrefs
	{
		// Supported settings
		internal const String LICENSE_SERVER        = "licenseServer";
		internal const String VERSION_HOST          = "versionHost";
		
		// Show internal details on detail panel
		internal const String SHOW_INTERNAL_DETAILS = "showInternalDetails";
		internal const String DEV                   = "dev";
		
		// Show development menu for tracing and stuff
		internal const String DEV_MENU              = "devMenu";
		const String APPL_NAME =          "oakland software";
		static AppSettingsReader          _appSettings;
		static Hashtable                  _settingsCache;
		
		static LocalPrefs()
		{
			_appSettings = new AppSettingsReader();
			_settingsCache = new Hashtable();
		}
		
		static String NOT_FOUND = "NOT_FOUND";
		
		public static void Set(String value, String setting)
		{
			// -t sets trace level
			if (value.ToLower().Equals("t")) {
				TraceUtil.Level = (TraceLevel)Convert.ToInt32(setting);
				Console.WriteLine("trace level set to: " + setting);
				return;
			}
			lock (typeof(LocalPrefs)) {
				_settingsCache.Add(value, setting);
			}
		}
		
		public static String Get(String value)
		{
			lock (typeof(LocalPrefs)) {
				// Use the cache to avoid getting an exception
				String setting = (String)_settingsCache[value];
				if (setting != null) {
					// Yes, make sure the *object* is the same
					if (setting == NOT_FOUND)
						return null;
					return setting;
				}
				try {
					setting = (String)_appSettings.
						GetValue(APPL_NAME + "." + value, 
								typeof(String));
					Set(value, setting);
					return setting;
				} catch {
					_settingsCache.Add(value, NOT_FOUND);
					return null;
				}
			}
		}
	}
}
