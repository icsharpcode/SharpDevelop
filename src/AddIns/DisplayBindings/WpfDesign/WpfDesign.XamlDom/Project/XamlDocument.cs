// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.ComponentModel;
using System.Xml;

namespace ICSharpCode.WpfDesign.XamlDom
{
	/// <summary>
	/// Represents a .xaml document.
	/// </summary>
	public sealed class XamlDocument
	{
		XmlDocument _xmlDoc;
		XamlObject _rootElement;
		IServiceProvider _serviceProvider;
		
		XamlTypeFinder _typeFinder;
		
		internal XmlDocument XmlDocument {
			get { return _xmlDoc; }
		}
		
		/// <summary>
		/// Gets the type finder used for this XAML document.
		/// </summary>
		public XamlTypeFinder TypeFinder {
			get { return _typeFinder; }
		}
		
		/// <summary>
		/// Gets the service provider used for markup extensions in this document.
		/// </summary>
		public IServiceProvider ServiceProvider {
			get { return _serviceProvider; }
		}
		
		internal ITypeDescriptorContext GetTypeDescriptorContext()
		{
			return new DummyTypeDescriptorContext(this);
		}
		
		sealed class DummyTypeDescriptorContext : ITypeDescriptorContext
		{
			XamlDocument document;
			
			public DummyTypeDescriptorContext(XamlDocument document)
			{
				this.document = document;
			}
			
			public IContainer Container {
				get { return null; }
			}
			
			public object Instance {
				get { return null; }
			}
			
			public PropertyDescriptor PropertyDescriptor {
				get { return null; }
			}
			
			public bool OnComponentChanging()
			{
				return false;
			}
			
			public void OnComponentChanged()
			{
			}
			
			public object GetService(Type serviceType)
			{
				return document.ServiceProvider.GetService(serviceType);
			}
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
		internal XamlDocument(XmlDocument xmlDoc, XamlParserSettings settings)
		{
			this._xmlDoc = xmlDoc;
			this._typeFinder = settings.TypeFinder;
			this._serviceProvider = settings.ServiceProvider;
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
		/// Creates a value that represents {x:Null}
		/// </summary>
		public XamlPropertyValue CreateNullValue()
		{
			return new XamlMarkupValue((XamlObject)CreatePropertyValue(new System.Windows.Markup.NullExtension(), null));
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
				return new XamlTextValue(this, c.ConvertToInvariantString(instance));
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
