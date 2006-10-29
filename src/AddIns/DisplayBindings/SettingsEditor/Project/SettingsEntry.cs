/*
 * Created by SharpDevelop.
 * User: Daniel Grunwald
 * Date: 10/28/2006
 * Time: 11:16 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.ComponentModel;
using System.Xml;
using System.Configuration;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.SettingsEditor
{
	public enum SettingScope
	{
		Application, User
	}
	
	public class SettingsEntry : LocalizedObject
	{
		string description;
		bool   generateDefaultValueInCode = true;
		string name;
		string provider = "";
		bool roaming;
		SettingScope scope = SettingScope.User;
		Type type;
		object value;
		
		public SettingsEntry()
		{
		}
		
		public SettingsEntry(XmlElement element)
		{
			description = element.GetAttribute("Description");
			if (!bool.TryParse(element.GetAttribute("GenerateDefaultValueInCode"), out generateDefaultValueInCode))
				generateDefaultValueInCode = true;
			name = element.GetAttribute("Name");
			provider = element.GetAttribute("Provider");
			bool.TryParse(element.GetAttribute("Roaming"), out roaming);
			if ("Application".Equals(element.GetAttribute("Scope"), StringComparison.OrdinalIgnoreCase)) {
				scope = SettingScope.Application;
			} else {
				scope = SettingScope.User;
			}
			type = GetType(element.GetAttribute("Type"));
			if (type != null && type != typeof(string)) {
				TypeConverter c = TypeDescriptor.GetConverter(type);
				SettingsSerializeAs ssa;
				if (c.CanConvertFrom(typeof(string)) && c.CanConvertTo(typeof(string))) {
					ssa = SettingsSerializeAs.String;
				} else {
					ssa = SettingsSerializeAs.Xml;
				}
				SettingsProperty p = new SettingsProperty(name);
				p.PropertyType = type;
				p.SerializeAs = ssa;
				SettingsPropertyValue v = new SettingsPropertyValue(p);
				v.SerializedValue = element["Value"].InnerText;
				this.value = v.PropertyValue;
			} else {
				this.value = element["Value"].InnerText;
			}
		}
		
		Type GetType(string typeName)
		{
			return typeof(object).Assembly.GetType(typeName, false)
				?? typeof(Uri).Assembly.GetType(typeName, false)
				?? typeof(System.Data.DataRow).Assembly.GetType(typeName, false);
		}
		
		[LocalizedProperty("Description",
		                   Description="Description of the setting.")]
		public string Description {
			get { return description; }
			set { description = value; }
		}
		
		[LocalizedProperty("Generate default value in code",
		                   Description="Specifies whether the value should be saved as attribute in the generated code.")]
		[DefaultValue(true)]
		public bool GenerateDefaultValueInCode {
			get { return generateDefaultValueInCode; }
			set { generateDefaultValueInCode = value; }
		}
		
		[LocalizedProperty("Name",
		                   Description="Name of the setting.")]
		public string Name {
			get { return name; }
			set { name = value; }
		}
		
		[LocalizedProperty("Provider",
		                   Description="The provider used to manage the setting.")]
		public string Provider {
			get { return provider; }
			set { provider = value; }
		}
		
		[LocalizedProperty("Roaming",
		                   Description="Specifies whether changes to the setting are stored in 'Application Data' (Roaming=true) or 'Local Application Data' (Roaming=false)")]
		public bool Roaming {
			get { return roaming; }
			set { roaming = value; }
		}
		
		[LocalizedProperty("Scope",
		                   Description="Specifies whether the setting is per-application (read-only) or per-user (read/write).")]
		public SettingScope Scope {
			get { return scope; }
			set { scope = value; }
		}
		
		[LocalizedProperty("SerializedSettingType",
		                   Description="The type used for the setting in the strongly-typed settings class.")]
		public string SerializedSettingType {
			get {
				if (type != null)
					return type.FullName;
				else
					return "(null)";
			}
		}
		
		[LocalizedProperty("Value",
		                   Description="The default value of the setting.")]
		public object Value {
			get { return value; }
			set {
				if (type == null || type.IsInstanceOfType(value)) {
					this.value = value;
				} else {
					throw new ArgumentException("Invalid type for property value.");
				}
			}
		}
		
		#region Custom property descriptors
		protected override void FilterProperties(PropertyDescriptorCollection col)
		{
			base.FilterProperties(col);
			PropertyDescriptor oldValue = col["Value"];
			col.Remove(oldValue);
			col.Add(new CustomValuePropertyDescriptor(oldValue, type));
			PropertyDescriptor oldRoaming = col["Roaming"];
			col.Remove(oldRoaming);
			col.Add(new RoamingPropertyDescriptor(oldRoaming, this));
		}
		
		class RoamingPropertyDescriptor : ProxyPropertyDescriptor
		{
			SettingsEntry entry;
			
			public RoamingPropertyDescriptor(PropertyDescriptor baseDescriptor, SettingsEntry entry)
				: base(baseDescriptor)
			{
				this.entry = entry;
			}
			
			public override bool IsReadOnly {
				get {
					return entry.Scope == SettingScope.Application; 
				}
			}
		}
		
		class CustomValuePropertyDescriptor : ProxyPropertyDescriptor
		{
			Type newPropertyType;
			
			public CustomValuePropertyDescriptor(PropertyDescriptor baseDescriptor, Type newPropertyType)
				: base(baseDescriptor)
			{
				this.newPropertyType = newPropertyType;
			}
			
			public override Type PropertyType {
				get {
					return newPropertyType;
				}
			}
			
			public override object GetEditor(Type editorBaseType)
			{
				return TypeDescriptor.GetEditor(newPropertyType, editorBaseType);
			}
		}
		#endregion
	}
}
