// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Windows.Data;
using System.Globalization;
using System.Reflection;
using System.Runtime.Serialization;

namespace ICSharpCode.CodeQualityAnalysis.Utility.Localizeable
{
	/// <summary>
	/// Description of EnumToFriendlyNameConverter.
	/// http://www.codeproject.com/KB/WPF/FriendlyEnums.aspx
	/// </summary>
	
	[ValueConversion(typeof(object), typeof(String))]
	public class EnumToFriendlyNameConverter : IValueConverter
	{
		#region IValueConverter implementation

		/// <summary>
		/// Convert value for binding from source object
		/// </summary>
		public object Convert(object value, System.Type targetType,
		                      object parameter, CultureInfo culture)
		{
			// To get around the stupid WPF designer bug
			if (value != null)
			{
				FieldInfo fi = value.GetType().GetField(value.ToString());

				// To get around the stupid WPF designer bug
				if (fi != null)
				{
					var attributes =
						(LocalizableDescriptionAttribute[])
						fi.GetCustomAttributes(typeof
						                       (LocalizableDescriptionAttribute), false);

					return ((attributes.Length > 0) &&
					        (!String.IsNullOrEmpty(attributes[0].Description)))
						?
						attributes[0].Description
						: value.ToString();
				}
			}

			return string.Empty;
		}

		/// <summary>
		/// ConvertBack value from binding back to source object
		/// </summary>
		public object ConvertBack(object value, System.Type targetType,
		                          object parameter, CultureInfo culture)
		{
			throw new Exception("Cant convert back");
		}
		#endregion
		
	}
}
