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
using System.Collections;
using System.ComponentModel;
using System.Globalization;

namespace ICSharpCode.SharpDevelop.Templates
{
	internal class CustomTypeConverter : TypeConverter
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
