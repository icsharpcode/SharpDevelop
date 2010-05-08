/*
 * Created by SharpDevelop.
 * User: PeterForstmeier
 * Date: 3/31/2007
 * Time: 2:09 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;

namespace ICSharpCode.Reports.Core
{
	/// <summary>
	/// Serializes object trees in a MycroXaml compatible way.
	/// </summary>
	public class MycroWriter
	{
		protected virtual string GetTypeName(Type t)
		{
			return t.Name;
		}
		
		public void Save(object obj, XmlWriter writer)
		{
			
			Type t = obj.GetType();
			writer.WriteStartElement(GetTypeName(t));
			foreach (PropertyInfo info in t.GetProperties(BindingFlags.Public | BindingFlags.Instance)) {
				if (!info.CanRead) continue;
				if (info.GetCustomAttributes(typeof(XmlIgnoreAttribute), true).Length != 0) continue;
				object val = info.GetValue(obj, null);
				if (val == null) continue;
				if (val is ICollection) {
					writer.WriteStartElement(info.Name);
					foreach (object element in (ICollection)val) {
						Save(element, writer);
					}
					writer.WriteEndElement();
				} else {
					if (!info.CanWrite) continue;
					TypeConverter c = TypeDescriptor.GetConverter(info.PropertyType);
					if (c.CanConvertFrom(typeof(string)) && c.CanConvertTo(typeof(string))) {
						try {
							writer.WriteElementString(info.Name, c.ConvertToInvariantString(val));
						} catch (Exception ex) {
							writer.WriteComment(ex.ToString());
						}
					} else if (info.PropertyType == typeof(Type)) {
						writer.WriteElementString(info.Name, ((Type)val).AssemblyQualifiedName);
					} else {
						writer.WriteStartElement(info.Name);
						Save(val, writer);
						writer.WriteEndElement();
					}
				}
			}
			writer.WriteEndElement();
		}
	}
}
