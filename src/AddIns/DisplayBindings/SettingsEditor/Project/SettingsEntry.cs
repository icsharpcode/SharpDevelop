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
	
	/// <summary>
	/// Describes one settings entry. Used for binding to the DataGridView.
	/// </summary>
	public sealed class SettingsEntry : INotifyPropertyChanged
	{
		public const bool GenerateDefaultValueInCodeDefault = true;
		
		string description;
		bool generateDefaultValueInCode = GenerateDefaultValueInCodeDefault;
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
				generateDefaultValueInCode = GenerateDefaultValueInCodeDefault;
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
				SettingsPropertyValue v = GetSettingConverter(type, name);
				v.SerializedValue = element["Value"].InnerText;
				this.value = v.PropertyValue;
			} else {
				this.value = element["Value"].InnerText;
			}
		}
		
		static SettingsPropertyValue GetSettingConverter(Type type, string name)
		{
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
			return new SettingsPropertyValue(p);
		}
		
		public void WriteTo(XmlWriter writer)
		{
			writer.WriteStartElement("Setting");
			writer.WriteAttributeString("Name", this.Name);
			writer.WriteAttributeString("Type", this.SerializedSettingType);
			writer.WriteAttributeString("Scope", this.Scope.ToString());
			
			if (!string.IsNullOrEmpty(description)) {
				writer.WriteAttributeString("Description", description);
			}
			if (generateDefaultValueInCode != GenerateDefaultValueInCodeDefault) {
				writer.WriteAttributeString("GenerateDefaultValueInCode", XmlConvert.ToString(generateDefaultValueInCode));
			}
			if (!string.IsNullOrEmpty(provider)) {
				writer.WriteAttributeString("Provider", provider);
			}
			if (scope == SettingScope.User) {
				writer.WriteAttributeString("Roaming", XmlConvert.ToString(roaming));
			}
			
			writer.WriteStartElement("Value");
			writer.WriteAttributeString("Profile", "(Default)");
			if (type != null && type != typeof(string)) {
				SettingsPropertyValue v = GetSettingConverter(type, name);
				v.PropertyValue = value;
				writer.WriteValue(v.SerializedValue);
			} else {
				writer.WriteValue((value ?? "").ToString());
			}
			writer.WriteEndElement();
			
			writer.WriteEndElement(); // Setting
		}
		
		Type GetType(string typeName)
		{
			return typeof(object).Assembly.GetType(typeName, false)
				?? typeof(Uri).Assembly.GetType(typeName, false)
				?? typeof(System.Data.DataRow).Assembly.GetType(typeName, false);
		}
		
		[Browsable(false)]
		public string Description {
			get { return description; }
			set {
				description = value;
				OnPropertyChanged(new PropertyChangedEventArgs("Description"));
			}
		}
		
		[Browsable(false)]
		public bool GenerateDefaultValueInCode {
			get { return generateDefaultValueInCode; }
			set {
				generateDefaultValueInCode = value;
				OnPropertyChanged(new PropertyChangedEventArgs("GenerateDefaultValueInCode"));
			}
		}
		
		[LocalizedProperty("Name",
		                   Description="Name of the setting.")]
		public string Name {
			get { return name; }
			set {
				if (string.IsNullOrEmpty(value))
					throw new ArgumentException("The name is invalid.");
				name = value;
				OnPropertyChanged(new PropertyChangedEventArgs("Name"));
			}
		}
		
		[Browsable(false)]
		public string Provider {
			get { return provider; }
			set {
				provider = value;
				OnPropertyChanged(new PropertyChangedEventArgs("Provider"));
			}
		}
		
		[Browsable(false)]
		public bool Roaming {
			get { return roaming; }
			set {
				roaming = value;
				OnPropertyChanged(new PropertyChangedEventArgs("Roaming"));
			}
		}
		
		public SettingScope Scope {
			get { return scope; }
			set {
				scope = value;
				OnPropertyChanged(new PropertyChangedEventArgs("Scope"));
			}
		}
		
		[Browsable(false)]
		public string SerializedSettingType {
			get {
				if (type != null)
					return type.FullName;
				else
					return "(null)";
			}
		}
		
		public object Value {
			get { return value; }
			set {
				if (type == null || type.IsInstanceOfType(value)) {
					this.value = value;
					OnPropertyChanged(new PropertyChangedEventArgs("Value"));
				} else {
					throw new ArgumentException("Invalid type for property value.");
				}
			}
		}
		
		[Browsable(false)]
		public Type Type {
			get { return type; }
			set {
				type = value;
				OnPropertyChanged(new PropertyChangedEventArgs("Type"));
			}
		}
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		void OnPropertyChanged(PropertyChangedEventArgs e)
		{
			if (PropertyChanged != null) {
				PropertyChanged(this, e);
			}
		}
	}
}
