using System;
using System.IO;
using System.ComponentModel;
using System.Text;
using System.Collections.Generic;
using System.Xml;

namespace ICSharpCode.Core
{
	/// <summary>
	/// Description of PropertyGroup.
	/// </summary>
	public class Properties
	{
		Dictionary<string, object> properties = new Dictionary<string, object>();
		
		public string this[string property] {
			get {
				return Convert.ToString(Get(property));
			}
			set {
				Set(property, value);
			}
		}
		
		public T Get<T>(string property, T defaultValue) 
		{
			if (!properties.ContainsKey(property)) {
				properties.Add(property, defaultValue);
				return defaultValue;
			}
			object o = properties[property];
			
			if (o is System.String && typeof(T) != typeof(System.String)) {
				TypeConverter c = TypeDescriptor.GetConverter(typeof(T));
				return (T)c.ConvertFromInvariantString(o.ToString());
			}
			return (T)o;
		}
		
		public object Get(string property)
		{
			if (!properties.ContainsKey(property)) {
				return null;
			}
			return properties[property];
		}
		
		public void Set<T>(string property, T value)
		{
			T oldValue = default(T);
			if (!properties.ContainsKey(property)) {
				properties.Add(property, value);
			} else {
				oldValue = Get<T>(property, value);
				properties[property] = value;
			}
			OnPropertyChanged(new PropertyChangedEventArgs(this, property, oldValue, value));
		}
		
		public bool Contains(string property)
		{
			return properties.ContainsKey(property);
		}	
		
		public bool Remove(string property)
		{
			return properties.Remove(property);
		}
		
		public Properties()
		{
		}
				
		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append("[Properties:{");
			foreach (KeyValuePair<string, object> entry in properties) {
				sb.Append(entry.Key);
				sb.Append("=");
				sb.Append(entry.Value);
				sb.Append(",");
			}
			sb.Append("}]");
			return sb.ToString();
		}
		
		public static Properties ReadFromAttributes(XmlReader reader) 
		{
			Properties properties = new Properties();
			if (reader.HasAttributes) {
				for (int i = 0; i < reader.AttributeCount; i++) {
					reader.MoveToAttribute(i);
					properties[reader.Name] = reader.Value;
				}
				reader.MoveToElement(); //Moves the reader back to the element node.
			}
			return properties;
		}
		
		public void ReadProperties(XmlTextReader reader, string endElement)
		{
			if (reader.IsEmptyElement) {
				return;
			}
			while (reader.Read()) {
				switch (reader.NodeType) {
					case XmlNodeType.EndElement:
						if (reader.LocalName == endElement) {
							return;
						}
						break;
					case XmlNodeType.Element:
						string propertyName = reader.LocalName;
						if (propertyName == "Properties") {
							propertyName = reader.GetAttribute(0);
							Properties p = new Properties();
							p.ReadProperties(reader, "Properties");
							properties[propertyName] = p;
						} else {
							properties[propertyName] = reader.HasAttributes ? reader.GetAttribute(0) : null;
						}
						break;
				}
			}
		}
		
		public void WriteProperties(XmlTextWriter writer)
		{
			foreach (KeyValuePair<string, object> entry in properties) {
				
				if (entry.Value is Properties) {
					writer.WriteStartElement("Properties");
					writer.WriteAttributeString("name", entry.Key);
					((Properties)entry.Value).WriteProperties(writer);
					writer.WriteEndElement();
					continue;
				}
				
				writer.WriteStartElement(entry.Key);
				if (entry.Value != null) {
					writer.WriteAttributeString("value", entry.Value.ToString());
				}
				writer.WriteEndElement();
			}
		}
		
		public void Save(string fileName)
		{
			using (XmlTextWriter writer = new XmlTextWriter(fileName, Encoding.Default)) {
				writer.Formatting = Formatting.Indented;
				writer.WriteStartElement("Properties");
				WriteProperties(writer);
				writer.WriteEndElement();
			}
		}
		
//		public void BinarySerialize(BinaryWriter writer)
//		{
//			writer.Write((byte)properties.Count);
//			foreach (KeyValuePair<string, object> entry in properties) {
//				writer.Write(AddInTree.GetNameOffset(entry.Key));
//				writer.Write(AddInTree.GetNameOffset(entry.Value.ToString()));
//			}
//		}
		
		public static Properties Load(string fileName)
		{
			if (!File.Exists(fileName)) {
				return null;
			}
			using (XmlTextReader reader = new XmlTextReader(fileName)) {
				while (reader.Read()){
					if (reader.IsStartElement()) {
						switch (reader.LocalName) {
							case "Properties":
								Properties properties = new Properties();
								properties.ReadProperties(reader, "Properties");
								return properties;
						}
					}
				}
			}
			return null;
		}
		protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
		{
			if (PropertyChanged != null) {
				PropertyChanged(this, e);
			}
		}
		
		public event PropertyChangedEventHandler PropertyChanged;
	}
}
