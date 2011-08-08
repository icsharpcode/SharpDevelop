// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.ComponentModel;
using System.Globalization;
using System.Text;

namespace ICSharpCode.SharpDevelop.Project.SavedData
{
	public sealed class ProjectSavedDataConverter : TypeConverter
	{
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			if (sourceType == typeof(string)) {
				return true;
			} else {
				return base.CanConvertFrom(context, sourceType);
			}
		}
		
		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			// convert from saved data(string) to objects
			if (value is string) {
				string[] v = ((string)value).Split('|');
				ProjectSavedDataType type = (ProjectSavedDataType)Enum.Parse(typeof(ProjectSavedDataType), v[3]);
				var data = new DummyProjectSavedData { 
					SavedString = (string)value,
					SavedDataType = type
				};
				return data;
			} else {
				return base.ConvertFrom(context, culture, value);
			}
		}
		
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			// convert objects to saved data(string)
			var data = value as IProjectSavedData;
			if (destinationType == typeof(string) && data != null) {
				switch (data.SavedDataType) {
					case ProjectSavedDataType.WatchVariables:
						return data.SavedString;
					default:
						throw new Exception("Invalid value for ProjectSavedDataType");
				}
			} else {
				return base.ConvertTo(context, culture, value, destinationType);
			}
		}
	}
}
