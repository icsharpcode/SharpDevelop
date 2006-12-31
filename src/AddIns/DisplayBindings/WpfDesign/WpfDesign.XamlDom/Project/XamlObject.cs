// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml;

namespace ICSharpCode.WpfDesign.XamlDom
{
	/// <summary>
	/// Represents a xaml object element.
	/// </summary>
	public sealed class XamlObject : XamlPropertyValue
	{
		XamlDocument document;
		XmlElement element;
		Type elementType;
		object instance;
		List<XamlProperty> properties = new List<XamlProperty>();
		
		/// <summary>For use by XamlParser only.</summary>
		internal XamlObject(XamlDocument document, XmlElement element, Type elementType, object instance)
		{
			this.document = document;
			this.element = element;
			this.elementType = elementType;
			this.instance = instance;
		}
		
		/// <summary>For use by XamlParser only.</summary>
		internal void AddProperty(XamlProperty property)
		{
			properties.Add(property);
		}
		
		#region XamlPropertyValue implementation
		internal override object GetValueFor(XamlPropertyInfo targetProperty)
		{
			return instance;
		}
		#endregion
		
		internal XmlElement XmlElement {
			get { return element; }
		}
		
		internal override void AddNodeTo(XamlProperty property)
		{
			property.AddChildNodeToProperty(element);
		}
		
		/// <summary>
		/// Gets the XamlDocument where this XamlObject is declared in.
		/// </summary>
		public XamlDocument OwnerDocument {
			get { return document; }
		}
		
		/// <summary>
		/// Gets the instance created by this object element.
		/// </summary>
		public object Instance {
			get { return instance; }
		}
		
		/// <summary>
		/// Gets the type of this object element.
		/// </summary>
		public Type ElementType {
			get { return elementType; }
		}
		
		/// <summary>
		/// Gets a read-only collection of properties set on this XamlObject.
		/// This includes both attribute and element properties.
		/// </summary>
		public IList<XamlProperty> Properties {
			get {
				return properties.AsReadOnly();
			}
		}
		
		internal override void RemoveNodeFromParent()
		{
			element.ParentNode.RemoveChild(element);
		}
		
		/// <summary>
		/// Finds the specified property, or creates it if it doesn't exist.
		/// </summary>
		public XamlProperty FindOrCreateProperty(string propertyName)
		{
			if (propertyName == null)
				throw new ArgumentNullException("propertyName");
			
			foreach (XamlProperty p in properties) {
				if (p.PropertyName == propertyName)
					return p;
			}
			PropertyDescriptorCollection propertyDescriptors = TypeDescriptor.GetProperties(elementType);
			PropertyDescriptor propertyInfo = propertyDescriptors[propertyName];
			if (propertyInfo == null) {
				throw new ArgumentException("The property '" + propertyName + "' doesn't exist on " + elementType.FullName, "propertyName");
			}
			XamlProperty newProperty = new XamlProperty(this, new XamlNormalPropertyInfo(propertyInfo));
			properties.Add(newProperty);
			return newProperty;
		}
	}
}
