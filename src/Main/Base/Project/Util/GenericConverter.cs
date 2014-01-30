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
using System.ComponentModel;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop
{
	/// <summary>
	/// Provides static conversion functions to quickly convert types
	/// having a TypeConverter to and from string (culture-invariant).
	/// </summary>
	public static class GenericConverter
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
