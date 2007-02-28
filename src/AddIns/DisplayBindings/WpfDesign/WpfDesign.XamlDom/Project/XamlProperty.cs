// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Xml;

namespace ICSharpCode.WpfDesign.XamlDom
{
	/// <summary>
	/// Describes a property on a <see cref="XamlObject"/>.
	/// </summary>
	public sealed class XamlProperty
	{
		XamlObject parentObject;
		XamlPropertyInfo propertyInfo;
		XamlPropertyValue propertyValue;
		
		List<XamlPropertyValue> collectionElements;
		bool isCollection;
		
		static readonly IList<XamlPropertyValue> emptyCollectionElementsArray = new XamlPropertyValue[0];
		
		// for use by parser only
		internal XamlProperty(XamlObject parentObject, XamlPropertyInfo propertyInfo, XamlPropertyValue propertyValue)
		{
			this.parentObject = parentObject;
			this.propertyInfo = propertyInfo;
			
			this.propertyValue = propertyValue;
			if (propertyValue != null) {
				propertyValue.ParentProperty = this;
			} else {
				if (propertyInfo.IsCollection) {
					isCollection = true;
					collectionElements = new List<XamlPropertyValue>();
				}
			}
		}
		
		internal XamlProperty(XamlObject parentObject, XamlPropertyInfo propertyInfo)
		{
			this.parentObject = parentObject;
			this.propertyInfo = propertyInfo;
			isCollection = propertyInfo.IsCollection;
		}
		
		/// <summary>
		/// Gets the parent object for which this property was declared.
		/// </summary>
		public XamlObject ParentObject {
			get { return parentObject; }
		}
		
		/// <summary>
		/// Gets the property name.
		/// </summary>
		public string PropertyName {
			get { return propertyInfo.Name; }
		}
		
		/// <summary>
		/// Gets the type the property is declared on.
		/// </summary>
		public Type PropertyTargetType {
			get { return propertyInfo.TargetType; }
		}
		
		/// <summary>
		/// Gets if this property is an attached property.
		/// </summary>
		public bool IsAttached {
			get { return propertyInfo.IsAttached; }
		}
		
		/// <summary>
		/// Gets the return type of the property.
		/// </summary>
		public Type ReturnType {
			get { return propertyInfo.ReturnType; }
		}
		
		/// <summary>
		/// Gets the type converter used to convert property values to/from string.
		/// </summary>
		public TypeConverter TypeConverter {
			get { return propertyInfo.TypeConverter; }
		}
		
		/// <summary>
		/// Gets the category of the property.
		/// </summary>
		public string Category {
			get { return propertyInfo.Category; }
		}
		
		/// <summary>
		/// Gets the value of the property. Can be null if the property is a collection property.
		/// </summary>
		public XamlPropertyValue PropertyValue {
			get { return propertyValue; }
			set {
				if (IsCollection) {
					throw new InvalidOperationException("Cannot set the value of collection properties.");
				}
				
				bool wasSet = this.IsSet;
				
				ResetInternal();
				propertyValue = value;
				propertyValue.AddNodeTo(this);
				propertyValue.ParentProperty = this;
				
				if (!wasSet) {
					if (IsSetChanged != null) {
						IsSetChanged(this, EventArgs.Empty);
					}
				}
				if (ValueChanged != null) {
					ValueChanged(this, EventArgs.Empty);
				}
			}
		}
		
		XmlElement _propertyElement;
		
		internal void ParserSetPropertyElement(XmlElement propertyElement)
		{
			_propertyElement = propertyElement;
		}
		
		internal void AddChildNodeToProperty(XmlNode newChildNode)
		{
			if (_propertyElement == null) {
				_propertyElement = parentObject.OwnerDocument.XmlDocument.CreateElement(
					this.PropertyTargetType.Name + "." + this.PropertyName,
					parentObject.OwnerDocument.GetNamespaceFor(this.PropertyTargetType)
				);
				parentObject.XmlElement.InsertBefore(_propertyElement, parentObject.XmlElement.FirstChild);
			}
			_propertyElement.AppendChild(newChildNode);
		}
		
		/// <summary>
		/// Gets if the property is a collection property.
		/// </summary>
		public bool IsCollection {
			get { return isCollection; }
		}
		
		/// <summary>
		/// Gets the collection elements of the property. Is empty if the property is not a collection.
		/// </summary>
		public IList<XamlPropertyValue> CollectionElements {
			get { return collectionElements != null ? collectionElements.AsReadOnly() : emptyCollectionElementsArray; }
		}
		
		/// <summary>
		/// Gets if the property is set.
		/// </summary>
		public bool IsSet {
			get { return propertyValue != null || collectionElements != null; }
		}
		
		/// <summary>
		/// Occurs when the value of the IsSet property has changed.
		/// </summary>
		public event EventHandler IsSetChanged;
		
		/// <summary>
		/// Occurs when the value of the property has changed.
		/// </summary>
		public event EventHandler ValueChanged;
		
		/// <summary>
		/// Resets the properties value.
		/// </summary>
		public void Reset()
		{
			if (IsSet) {
				propertyInfo.ResetValue(parentObject.Instance);
				
				ResetInternal();
				
				if (IsSetChanged != null) {
					IsSetChanged(this, EventArgs.Empty);
				}
				if (ValueChanged != null) {
					ValueChanged(this, EventArgs.Empty);
				}
			}
		}
		
		void ResetInternal()
		{
			if (propertyValue != null) {
				propertyValue.RemoveNodeFromParent();
				propertyValue.ParentProperty = null;
				propertyValue = null;
			}
			if (_propertyElement != null) {
				_propertyElement.ParentNode.RemoveChild(_propertyElement);
				_propertyElement = null;
			}
		}
		
		/// <summary>
		/// used internally by the XamlParser.
		/// Add a collection element that already is part of the XML DOM.
		/// </summary>
		internal void ParserAddCollectionElement(XamlPropertyValue val)
		{
			collectionElements.Add(val);
			val.ParentProperty = this;
		}
		
		/// <summary>
		/// Gets/Sets the value of the property on the instance without updating the XAML document.
		/// </summary>
		public object ValueOnInstance {
			get{
				return propertyInfo.GetValue(parentObject.Instance);
			}
			set {
				propertyInfo.SetValue(parentObject.Instance, value);
			}
		}
		
		/*public bool IsAttributeSyntax {
			get {
				return attribute != null;
			}
		}
		
		public bool IsElementSyntax {
			get {
				return element != null;
			}
		}
		
		public bool IsImplicitDefaultProperty {
			get {
				return attribute == null && element == null;
			}
		}*/
	}
	
	/// <summary>
	/// Used for the value of a <see cref="XamlProperty"/>.
	/// Can be a <see cref="XamlTextValue"/> or a <see cref="XamlObject"/>.
	/// </summary>
	public abstract class XamlPropertyValue
	{
		/// <summary>
		/// used internally by the XamlParser.
		/// </summary>
		internal abstract object GetValueFor(XamlPropertyInfo targetProperty);
		
		XamlProperty _parentProperty;
		
		/// <summary>
		/// Gets the parent property that this value is assigned to.
		/// </summary>
		public XamlProperty ParentProperty {
			get { return _parentProperty; }
			internal set { _parentProperty = value; }
		}
		
		internal abstract void RemoveNodeFromParent();
		
		internal abstract void AddNodeTo(XamlProperty property);
	}
	
	/// <summary>
	/// A textual value in a .xaml file.
	/// </summary>
	public sealed class XamlTextValue : XamlPropertyValue
	{
		XmlAttribute attribute;
		XmlText textNode;
		XmlSpace xmlSpace;
		string textValue;
		
		internal XamlTextValue(XmlAttribute attribute)
		{
			this.attribute = attribute;
		}
		
		internal XamlTextValue(string textValue)
		{
			this.textValue = textValue;
		}
		
		internal XamlTextValue(XmlText textNode, XmlSpace xmlSpace)
		{
			this.xmlSpace = xmlSpace;
			this.textNode = textNode;
		}
		
		/// <summary>
		/// The text represented by the value.
		/// </summary>
		public string Text {
			get {
				if (attribute != null)
					return attribute.Value;
				else if (textValue != null)
					return textValue;
				else
					return NormalizeWhitespace(textNode.Value);
			}
			set {
				if (value == null)
					throw new ArgumentNullException("value");
				
				if (attribute != null)
					attribute.Value = value;
				else if (textValue != null)
					textValue = value;
				else
					textNode.Value = value;
			}
		}
		
		string NormalizeWhitespace(string text)
		{
			if (xmlSpace == XmlSpace.Preserve) {
				return text.Replace("\r", "");
			}
			StringBuilder b = new StringBuilder();
			bool wasWhitespace = true;
			foreach (char c in text) {
				if (char.IsWhiteSpace(c)) {
					if (!wasWhitespace) {
						b.Append(' ');
					}
					wasWhitespace = true;
				} else {
					wasWhitespace = false;
					b.Append(c);
				}
			}
			if (b.Length > 0 && wasWhitespace)
				b.Length -= 1;
			return b.ToString();
		}
		
		internal override object GetValueFor(XamlPropertyInfo targetProperty)
		{
			if (targetProperty == null)
				return this.Text;
			TypeConverter converter = targetProperty.TypeConverter;
			if (converter != null) {
				return converter.ConvertFromInvariantString(this.Text);
			} else {
				return this.Text;
			}
		}
		
		internal override void RemoveNodeFromParent()
		{
			if (attribute != null)
				attribute.OwnerElement.RemoveAttribute(attribute.Name);
			else if (textNode != null)
				textNode.ParentNode.RemoveChild(textNode);
		}
		
		internal override void AddNodeTo(XamlProperty property)
		{
			if (attribute != null) {
				property.ParentObject.XmlElement.Attributes.Append(attribute);
			} else if (textValue != null) {
				string ns = property.ParentObject.OwnerDocument.GetNamespaceFor(property.PropertyTargetType);
				string name;
				if (property.IsAttached)
					name = property.PropertyTargetType.Name + "." + property.PropertyName;
				else
					name = property.PropertyName;
				if (property.ParentObject.XmlElement.GetPrefixOfNamespace(ns) == "") {
					property.ParentObject.XmlElement.SetAttribute(name, textValue);
					attribute = property.ParentObject.XmlElement.GetAttributeNode(name);
				} else {
					property.ParentObject.XmlElement.SetAttribute(name, ns, textValue);
					attribute = property.ParentObject.XmlElement.GetAttributeNode(name, ns);
				}
				textValue = null;
			} else {
				property.AddChildNodeToProperty(textNode);
			}
		}
	}
}
