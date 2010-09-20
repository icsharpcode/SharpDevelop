// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.ComponentModel;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop
{
	/// <summary>
	/// Provides static conversion functions to quickly convert types
	/// having a TypeConverter to and from string (culture-invariant).
	/// </summary>
	internal static class GenericConverter
	{
		/// <summary>
		/// Converts the value from string.
		/// </summary>
		public static T FromString<T>(string v, T defaultValue)
		{
			if (string.IsNullOrEmpty(v))
				return defaultValue;
			if (typeof(T) == typeof(string))
				return (T)(object)v;
			try {
				TypeConverter c = TypeDescriptor.GetConverter(typeof(T));
				return (T)c.ConvertFromInvariantString(v);
			} catch (Exception ex) {
				LoggingService.Info(ex);
				return defaultValue;
			}
		}
		
		/// <summary>
		/// Converts the value to string.
		/// </summary>
		public static string ToString<T>(T val)
		{
			if (typeof(T) == typeof(string)) {
				string s = (string)(object)val;
				return string.IsNullOrEmpty(s) ? null : s;
			}
			try {
				TypeConverter c = TypeDescriptor.GetConverter(typeof(T));
				string s = c.ConvertToInvariantString(val);
				return string.IsNullOrEmpty(s) ? null : s;
			} catch (Exception ex) {
				LoggingService.Info(ex);
				return null;
			}
		}
	}
}
