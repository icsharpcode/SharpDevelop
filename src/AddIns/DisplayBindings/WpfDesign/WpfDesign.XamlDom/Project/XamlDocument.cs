// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.ComponentModel;
using System.Windows.Markup;
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
		
		/// <summary>
		/// Gets the type descriptor context used for type conversions.
		/// </summary>
		/// <param name="containingObject">The containing object, used when the
		/// type descriptor context needs to resolve an XML namespace.</param>
		internal ITypeDescriptorContext GetTypeDescriptorContext(XamlObject containingObject)
		{
			IServiceProvider serviceProvider;
			if (containingObject != null) {
				if (containingObject.OwnerDocument != this)
					throw new ArgumentException("Containing object must belong to the document!");
				serviceProvider = containingObject.ServiceProvider;
			} else {
				serviceProvider = this.ServiceProvider;
			}
			return new DummyTypeDescriptorContext(serviceProvider);
		}
		
		sealed class DummyTypeDescriptorContext : ITypeDescriptorContext
		{
			readonly IServiceProvider baseServiceProvider;
			
			public DummyTypeDescriptorContext(IServiceProvider serviceProvider)
			{
				this.baseServiceProvider = serviceProvider;
			}
			
			public IContainer Container {
				get { return null; }
			}
			
			public object Instance {
				get; set;
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
				return baseServiceProvider.GetService(serviceType);
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
			return CreateObject(new NullExtension());
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
			var ctx = new DummyTypeDescriptorContext(this.ServiceProvider);
			ctx.Instance = instance;
			bool hasStringConverter = c.CanConvertTo(ctx, typeof(string)) && c.CanConvertFrom(typeof(string));
			if (forProperty != null && hasStringConverter) {
				return new XamlTextValue(this, c.ConvertToInvariantString(ctx, instance));
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
