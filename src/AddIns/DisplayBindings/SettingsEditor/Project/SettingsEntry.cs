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
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml;
using System.Configuration;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop;

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
		ISettingsEntryHost host;
		
		public SettingsEntry(ISettingsEntryHost host)
		{
			if (host == null)
				throw new ArgumentNullException("host");
			this.host = host;
		}
		
		public SettingsEntry(ISettingsEntryHost host, XmlElement element)
			: this(host)
		{
			if (element == null)
				throw new ArgumentNullException("element");
			if (element["Value"] == null)
				throw new FormatException("Not a settings file.");
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
			this.SerializedValue = element["Value"].InnerText;
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
			writer.WriteValue(SerializedValue);
			writer.WriteEndElement();
			
			writer.WriteEndElement(); // Setting
		}
		
		Type GetType(string typeName)
		{
			foreach (SpecialTypeDescriptor d in SpecialTypeDescriptor.Descriptors) {
				if (string.Equals(typeName, d.name, StringComparison.OrdinalIgnoreCase))
					return d.type;
			}
			
			return typeof(object).Assembly.GetType(typeName, false)
				?? typeof(Uri).Assembly.GetType(typeName, false)
				?? typeof(System.Drawing.Font).Assembly.GetType(typeName, false)
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
		
		public object Value {
			get { return this.value; }
			set {
				if (type == null || value == null || type.IsInstanceOfType(value)) {
					this.value = value;
					cachedSerializedValue = null;
					OnPropertyChanged(new PropertyChangedEventArgs("Value"));
					OnPropertyChanged(new PropertyChangedEventArgs("SerializedValue"));
				} else {
					throw new ArgumentException("Invalid type for property value.");
				}
			}
		}
		
		string cachedSerializedValue;
		
		public string SerializedValue {
			get {
				if (cachedSerializedValue != null)
					return cachedSerializedValue;
				if (type != null && type != typeof(string)) {
					foreach (SpecialTypeDescriptor d in SpecialTypeDescriptor.Descriptors) {
						if (type == d.type) {
							return cachedSerializedValue = d.GetString(value);
						}
					}
					
					SettingsPropertyValue v = GetSettingConverter(type, name);
					v.PropertyValue = value;
					cachedSerializedValue = (v.SerializedValue ?? "").ToString();
				} else if (value != null) {
					cachedSerializedValue = value.ToString();
				} else {
					cachedSerializedValue = "";
				}
				return cachedSerializedValue;
			}
			set {
				if (type != null && type != typeof(string)) {
					foreach (SpecialTypeDescriptor d in SpecialTypeDescriptor.Descriptors) {
						if (type == d.type) {
							this.Value = d.GetValue(value);
							return;
						}
					}
					
					SettingsPropertyValue v = GetSettingConverter(type, name);
					v.SerializedValue = value;
					this.Value = v.PropertyValue;
				} else {
					this.Value = value;
				}
			}
		}
		
		public Type Type {
			get { return type; }
			set {
				if (type != value) {
					string oldValue = this.SerializedValue;
					this.cachedSerializedValue = null;
					this.value = null;
					
					type = value;
					OnPropertyChanged(new PropertyChangedEventArgs("Type"));
					OnPropertyChanged(new PropertyChangedEventArgs("WrappedSettingType"));
					
					this.SerializedValue = oldValue;
				}
			}
		}
		
		[Browsable(false)]
		public string SerializedSettingType {
			get {
				foreach (SpecialTypeDescriptor d in SpecialTypeDescriptor.Descriptors) {
					if (type == d.type)
						return d.name;
				}
				if (type != null)
					return type.FullName;
				else
					return "(null)";
			}
		}
		
		public string WrappedSettingType {
			get {
				return host.GetDisplayNameForType(type);
			}
			set {
				this.Type = host.GetTypeByDisplayName(value);
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
