/*
 * Erstellt mit SharpDevelop.
 * Benutzer: Peter Forstmeier
 * Datum: 30.01.2008
 * Zeit: 11:47
 * 
 * Sie können diese Vorlage unter Extras > Optionen > Codeerstellung > Standardheader ändern.
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;

namespace ICSharpCode.Reports.Addin
{
	/// <summary>
	/// Serializes object trees in a MycroXaml compatible way. 
	/// See http://www.codeproject.com/dotnet/MycroXaml.asp
	/// 
	/// </summary>
	public class MycroWriter
	{
		protected virtual string GetTypeName(Type t)
		{
			return t.Name;
		}
		
		public void Save(object obj, XmlWriter writer)
		{
//			System.Console.WriteLine("Save <{0}>",obj.GetType().Name);
			Type t = obj.GetType();
		
//			writer.WriteStartElement(GetTypeName(t));
//			System.Console.WriteLine("\tSave <{0}>",GetTypeName(t));
			                        
			writer.WriteStartElement(obj.GetType().Name);
			
			PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(obj);
			PropertyInfo [] propertyInfos = new PropertyInfo[properties.Count];
			
			for (int i = 0;i < properties.Count ; i ++){
				propertyInfos[i] = t.GetProperty(properties[i].Name);
			}
			
			foreach (PropertyInfo info in propertyInfos) {
				if (info ==  null){
					continue;
				}
				if (!info.CanRead) continue;
				
				if (info.GetCustomAttributes(typeof(XmlIgnoreAttribute), true).Length != 0) continue;
				object val = info.GetValue(obj, null);
				if (val == null) continue;
				if (val is ICollection) {
//					PropertyInfo pinfo = t.GetProperty(info.Name);
//					Console.WriteLine("pinfo {0}",pinfo.Name);
					if (info.Name.StartsWith("Contr")) {
						writer.WriteStartElement("Items");
					} else {
					writer.WriteStartElement(info.Name);
					}
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
//						System.Diagnostics.Trace.WriteLine("Serialize Pagelayout");
//						writer.WriteStartElement(info.Name);
//						Save(val, writer);
//						writer.WriteEndElement();
					}
				}
			}
			writer.WriteEndElement();
		}
	}
}
