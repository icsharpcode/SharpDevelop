// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Xml;
using System.ComponentModel;

namespace ICSharpCode.WpfDesign.XamlDom
{
	/// <summary>
	/// Represents a .xaml document.
	/// </summary>
	public sealed class XamlDocument
	{
		XmlDocument _xmlDoc;
		XamlObject _rootElement;
		
		XamlTypeFinder _typeFinder;
		
		internal XmlDocument XmlDocument {
			get { return _xmlDoc; }
		}
		
		/// <summary>
		/// Gets the root xaml object.
		/// </summary>
		public XamlObject RootElement {
			get { return _rootElement; }
		}
		
		/// <summary>
		/// Gets the object instance created by the root xaml object.
		/// </summary>
		public object RootInstance {
			get { return (_rootElement != null) ? _rootElement.Instance : null; }
		}
		
		/// <summary>
		/// Saves the xaml document into the <paramref name="writer"/>.
		/// </summary>
		public void Save(XmlWriter writer)
		{
			if (writer == null)
				throw new ArgumentNullException("writer");
			_xmlDoc.Save(writer);
		}
		
		/// <summary>
		/// Internal constructor, used by XamlParser.
		/// </summary>
		internal XamlDocument(XmlDocument xmlDoc, XamlTypeFinder typeFinder)
		{
			this._xmlDoc = xmlDoc;
			this._typeFinder = typeFinder;
		}
		
		/// <summary>
		/// Called by XamlParser to finish initializing the document.
		/// </summary>
		internal void ParseComplete(XamlObject rootElement)
		{
			this._rootElement = rootElement;
		}
		
		/// <summary>
		/// Create an XamlObject from the instance.
		/// </summary>
		public XamlObject CreateObject(object instance)
		{
			return (XamlObject)CreatePropertyValue(instance, null);
		}
		
		/// <summary>
		/// Create a XamlPropertyValue for the specified value instance.
		/// </summary>
		public XamlPropertyValue CreatePropertyValue(object instance, XamlProperty forProperty)
		{
			if (instance == null)
				throw new ArgumentNullException("instance");
			
			Type elementType = instance.GetType();
			TypeConverter c = TypeDescriptor.GetConverter(instance);
			bool hasStringConverter = c.CanConvertTo(typeof(string)) && c.CanConvertFrom(typeof(string));
			
			if (forProperty != null && hasStringConverter) {
				return new XamlTextValue(c.ConvertToInvariantString(instance));
			}
			
			
			XmlElement xml = _xmlDoc.CreateElement(elementType.Name, GetNamespaceFor(elementType));
			
			if (hasStringConverter) {
				xml.InnerText = c.ConvertToInvariantString(instance);
			}
			return new XamlObject(this, xml, elementType, instance);
		}
		
		internal string GetNamespaceFor(Type type)
		{
			return _typeFinder.GetXmlNamespaceFor(type.Assembly, type.Namespace);
		}
	}
}
