using System;
using System.IO;
using System.ComponentModel;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

namespace ICSharpCode.Core
{
	/// <summary>
	/// This interface flags an object beeing "mementocapable". This means that the
	/// state of the object could be saved to an <see cref="Properties"/> object
	/// and set from a object from the same class.
	/// This is used to save and restore the state of GUI objects.
	/// </summary>
	public interface IMementoCapable
	{
		/// <summary>
		/// Creates a new memento from the state.
		/// </summary>
		Properties CreateMemento();
		
		/// <summary>
		/// Sets the state to the given memento.
		/// </summary>
		void SetMemento(Properties memento);
	}
	
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
						} else if (propertyName == "Array") {
							propertyName = reader.GetAttribute(0);
							properties[propertyName] = ReadArray(reader);
						} else {
							properties[propertyName] = reader.HasAttributes ? reader.GetAttribute(0) : null;
						}
						break;
				}
			}
		}
		
		ArrayList ReadArray(XmlTextReader reader)
		{
			ArrayList l = new ArrayList();
			while (reader.Read()) {
				switch (reader.NodeType) {
					case XmlNodeType.EndElement:
						if (reader.LocalName == "Array") {
							return l;
						}
						break;
					case XmlNodeType.Element:
						l.Add(reader.HasAttributes ? reader.GetAttribute(0) : null);
						break;
				}
			}
			return l;
		}
		
		public void WriteProperties(XmlTextWriter writer)
		{
			foreach (KeyValuePair<string, object> entry in properties) {
				object val = entry.Value;
				if (val is Properties) {
					writer.WriteStartElement("Properties");
					writer.WriteAttributeString("name", entry.Key);
					((Properties)val).WriteProperties(writer);
					writer.WriteEndElement();
				} else if (val is Array) {
					writer.WriteStartElement("Array");
					writer.WriteAttributeString("name", entry.Key);
					foreach (object o in (Array)val) {
						writer.WriteStartElement("Element");
						WriteValue(writer, o);
						writer.WriteEndElement();
					}
					writer.WriteEndElement();
				} else {
					writer.WriteStartElement(entry.Key);
					WriteValue(writer, val);
					writer.WriteEndElement();
				}
			}
		}
		
		void WriteValue(XmlTextWriter writer, object val)
		{
			if (val != null) {
				if (val is string) {
					writer.WriteAttributeString("value", val.ToString());
				} else {
					TypeConverter c = TypeDescriptor.GetConverter(val.GetType());
					writer.WriteAttributeString("value", c.ConvertToInvariantString(val));
				}
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
		
		public T Get<T>(string property, T defaultValue)
		{
			if (!properties.ContainsKey(property)) {
				properties.Add(property, defaultValue);
				return defaultValue;
			}
			object o = properties[property];
			
			if (o is string && typeof(T) != typeof(string)) {
				TypeConverter c = TypeDescriptor.GetConverter(typeof(T));
				o = c.ConvertFromInvariantString(o.ToString());
				properties[property] = o; // store for future look up
			} else if (o is ArrayList && typeof(T).IsArray) {
				ArrayList list = (ArrayList)o;
				Type elementType = typeof(T).GetElementType();
				Array arr = System.Array.CreateInstance(elementType, list.Count);
				TypeConverter c = TypeDescriptor.GetConverter(elementType);
				for (int i = 0; i < arr.Length; ++i) {
					if (list[i] != null) {
						arr.SetValue(c.ConvertFromInvariantString(list[i].ToString()), i);
					}
				}
				o = arr;
				properties[property] = o; // store for future look up
			}
			return (T)o;
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
