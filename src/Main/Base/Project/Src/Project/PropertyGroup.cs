// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.ComponentModel;
using System.Text;
using System.Collections.Generic;
using System.Xml;

namespace ICSharpCode.SharpDevelop.Project
{
	/// <summary>
	/// Description of PropertyGroup.
	/// </summary>
	public class PropertyGroup
	{
		// TODO: Isn't MSBuild case-insensitive ???
		Dictionary<string, bool>   isGuardedProperty   = new Dictionary<string, bool>();
		Dictionary<string, string> properties          = new Dictionary<string, string>();
		
		/// <summary>
		/// Gets the number of properties in this group.
		/// </summary>
		public int PropertyCount {
			get {
				return properties.Count;
			}
		}
		
		public string this[string property] {
			get {
				return Get(property, String.Empty);
			}
			set {
				Set(property, String.Empty, value);
			}
		}
		
		public T Get<T>(string property, T defaultValue)
		{
			if (!properties.ContainsKey(property)) {
				return defaultValue;
			}
			TypeConverter c = TypeDescriptor.GetConverter(typeof(T));
			try {
				return (T)c.ConvertFromInvariantString(properties[property]);
			} catch (FormatException ex) {
				ICSharpCode.Core.LoggingService.Warn("Cannot get property " + property, ex);
				return defaultValue;
			}
		}
		
		public void Set<T>(string property, T defaultValue, T value)
		{
			if (value == null || value.Equals(defaultValue)) {
				properties.Remove(property);
				return;
			}
			
			if (!properties.ContainsKey(property)) {
				properties.Add(property, value.ToString());
			} else {
				properties[property] = value.ToString();
			}
		}
		
		public void SetIsGuarded(string property, bool isGuarded)
		{
			isGuardedProperty[property] = isGuarded;
		}
		
		public bool IsSet(string property)
		{
			return properties.ContainsKey(property);
		}
		
		public bool Remove(string property)
		{
			return properties.Remove(property);
		}
		
		
		public void Set(string property, object value)
		{
			properties[property] = value.ToString();
		}
		
		public PropertyGroup()
		{
		}
		
		public void Merge(PropertyGroup group)
		{
			foreach (KeyValuePair<string, string> entry in group.properties) {
				properties[entry.Key] = entry.Value;
			}
			foreach (KeyValuePair<string, bool> entry in group.isGuardedProperty) {
				isGuardedProperty[entry.Key] = entry.Value;
			}
		}
		
		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append("[PropertyGroup:Properties={");
			foreach (KeyValuePair<string, string> entry in properties) {
				sb.Append(entry.Key);
				sb.Append("=");
				sb.Append(entry.Value);
				sb.Append(",");
			}
			sb.Append("}]");
			return sb.ToString();
		}
		
		internal static void ReadProperties(XmlReader reader, PropertyGroup properties, string endElement)
		{
			if (reader.IsEmptyElement) {
				return;
			}
			while (reader.Read()) {
			reLoop:
				switch (reader.NodeType) {
					case XmlNodeType.EndElement:
						if (reader.LocalName == endElement) {
							return;
						}
						break;
					case XmlNodeType.Element:
						string propertyName = reader.LocalName;
						
						properties.isGuardedProperty[propertyName] = reader.HasAttributes;
						
						if (reader.IsEmptyElement) {
							properties[propertyName] = null;
						} else {
							reader.Read();
							if (reader.NodeType != XmlNodeType.Text) {
								properties[propertyName] = null;
								goto reLoop;
							}
							properties[propertyName] = reader.Value.Trim();
						}
						break;
				}
			}
		}
		
		internal void WriteProperties(XmlWriter writer)
		{
			foreach (KeyValuePair<string, string> entry in properties) {
				writer.WriteStartElement(entry.Key);
				
				if (isGuardedProperty.ContainsKey(entry.Key) && isGuardedProperty[entry.Key]) {
					writer.WriteAttributeString("Condition", " '$(" + entry.Key + ")' == '' ");
				}
				
				if (entry.Value != null) {
					writer.WriteValue(entry.Value);
				}
				writer.WriteEndElement();
			}
		}
	}
}
