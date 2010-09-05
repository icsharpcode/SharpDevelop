// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using System.Reflection;

namespace ICSharpCode.Scripting.Tests.Utils
{
	[TypeConverter(typeof(CustomClassTypeConverter))]
	public class CustomClass
	{
		public string Name { get; set; }
		public string Category { get; set; }

		public CustomClass()
		{
		}

		public CustomClass(string name, string category)
		{
			this.Name = name;
			this.Category = category;
		}
	}

	public class CustomClassTypeConverter : TypeConverter
	{
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			if (destinationType == typeof(InstanceDescriptor)) {
				CustomClass c = value as CustomClass;
				if (c != null) {
					ConstructorInfo info = typeof(CustomClass).GetConstructor(new Type[] {typeof(String), typeof(String)});
					return new InstanceDescriptor(info, new object[] {c.Name, c.Category});
				}
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
		
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			if (destinationType == typeof(InstanceDescriptor)) {
				return true;
			}
			return base.CanConvertTo(context, destinationType);
		}
	}
}
