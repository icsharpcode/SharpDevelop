// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections;
using System.ComponentModel;
using System.Globalization;

using ICSharpCode.SharpDevelop.Internal.Templates;

namespace ICSharpCode.SharpDevelop.Gui
{
	public class CustomTypeConverter : TypeConverter
	{
		TemplateType templateType;
		
		public CustomTypeConverter(TemplateType templateType)
		{
			this.templateType = templateType;
		}
		
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			return sourceType == typeof(string);
		}
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			return destinationType == typeof(string);
		}
		
		public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
		{
			return true;
		}
		public override  bool GetStandardValuesExclusive(ITypeDescriptorContext context)
		{
			return true;
		}
		
		public override System.ComponentModel.TypeConverter.StandardValuesCollection GetStandardValues(System.ComponentModel.ITypeDescriptorContext context)
		{
			// Passes the local integer array.
			ArrayList values = new ArrayList();
			foreach (DictionaryEntry entry in templateType.Pairs) {
				values.Add(entry.Key);
			}
			StandardValuesCollection svc = new StandardValuesCollection(values);
			return svc;
		}
		
		// Overrides the ConvertFrom method of TypeConverter.
		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) 
		{
			
			if (templateType.Pairs[value] != null) {
				return templateType.Pairs[value];
			}
			return value.ToString();
		}
		
		// Overrides the ConvertTo method of TypeConverter.
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			foreach (DictionaryEntry entry in templateType.Pairs) {
				if (entry.Value.ToString() == value.ToString()) {
					return entry.Key;
				}
			}
			return value.ToString();
		}
	}
}
