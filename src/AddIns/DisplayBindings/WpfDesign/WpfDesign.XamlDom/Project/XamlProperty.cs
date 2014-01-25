// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Xml;
using System.Windows;
using System.Windows.Markup;

namespace ICSharpCode.WpfDesign.XamlDom
{
	/// <summary>
	/// Describes a property on a <see cref="XamlObject"/>.
	/// </summary>
	[DebuggerDisplay("XamlProperty: {PropertyName}")]
	public sealed class XamlProperty
	{
		XamlObject parentObject;
		internal readonly XamlPropertyInfo propertyInfo;
		XamlPropertyValue propertyValue;
		
		CollectionElementsCollection collectionElements;
		bool isCollection;
		bool isResources;
		
		static readonly IList<XamlPropertyValue> emptyCollectionElementsArray = new XamlPropertyValue[0];
		
		// for use by parser only
		internal XamlProperty(XamlObject parentObject, XamlPropertyInfo propertyInfo, XamlPropertyValue propertyValue)
			: this(parentObject, propertyInfo)
		{
			PossiblyNameChanged(null, propertyValue);

			this.propertyValue = propertyValue;
			if (propertyValue != null) {
				propertyValue.ParentProperty = this;
			}

			UpdateValueOnInstance();
		}
		
		internal XamlProperty(XamlObject parentObject, XamlPropertyInfo propertyInfo)
		{
			this.parentObject = parentObject;
			this.propertyInfo = propertyInfo;
			
			if (propertyInfo.IsCollection) {
				isCollection = true;
				collectionElements = new CollectionElementsCollection(this);
				
				if (propertyInfo.Name.Equals(XamlConstants.ResourcesPropertyName, StringComparison.Ordinal) &&
				    propertyInfo.ReturnType == typeof(ResourceDictionary)) {
					isResources = true;
				}
			}
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
		/// Gets if this property is an event.
		/// </summary>
		public bool IsEvent {
			get { return propertyInfo.IsEvent; }
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
			set { SetPropertyValue(value); }
		}
		
		/// <summary>
		/// Gets if the property represents the FrameworkElement.Resources property that holds a locally-defined resource dictionary. 
		/// </summary>
		public bool IsResources {
			get { return isResources; }
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
			get { return collectionElements ?? emptyCollectionElementsArray; }
		}
		
		/// <summary>
		/// Gets if the property is set.
		/// </summary>
		public bool IsSet {
			get { return propertyValue != null ||
					_propertyElement != null; // collection
			}
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
		/// Occurs when MarkupExtension evaluated PropertyValue dosn't changed but ValueOnInstance does.
		/// </summary>
		public event EventHandler ValueOnInstanceChanged;

		void SetPropertyValue(XamlPropertyValue value)
		{
			// Binding...
			//if (IsCollection) {
			//    throw new InvalidOperationException("Cannot set the value of collection properties.");
			//}
			
			bool wasSet = this.IsSet;
			
			PossiblyNameChanged(propertyValue, value);

			//reset expression
			var xamlObject = propertyValue as XamlObject;
			if (xamlObject != null && xamlObject.IsMarkupExtension)
				propertyInfo.ResetValue(parentObject.Instance);
			
			ResetInternal();

			propertyValue = value;
			propertyValue.ParentProperty = this;
			propertyValue.AddNodeTo(this);
			UpdateValueOnInstance();
			
			ParentObject.OnPropertyChanged(this);
			
			if (!wasSet) {
				if (IsSetChanged != null) {
					IsSetChanged(this, EventArgs.Empty);
				}
			}

			if (ValueChanged != null) {
				ValueChanged(this, EventArgs.Empty);
			}
		}

		internal void UpdateValueOnInstance()
		{
			if (PropertyValue != null) {
				try {
					ValueOnInstance = PropertyValue.GetValueFor(propertyInfo);
				}
				catch {
					Debug.WriteLine("UpdateValueOnInstance() failed");
				}
			}
		}
		
		/// <summary>
		/// Resets the properties value.
		/// </summary>
		public void Reset()
		{
			if (IsSet) {
				
				propertyInfo.ResetValue(parentObject.Instance);
				ResetInternal();

				ParentObject.OnPropertyChanged(this);
				
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
		
		XmlElement _propertyElement;
		
		internal void ParserSetPropertyElement(XmlElement propertyElement)
		{
			XmlElement oldPropertyElement = _propertyElement;
			if (oldPropertyElement == propertyElement) return;
			
			_propertyElement = propertyElement;
			
			if (oldPropertyElement != null && IsCollection) {
				Debug.WriteLine("Property element for " + this.PropertyName + " already exists, merging..");
				foreach (XamlPropertyValue val in this.collectionElements) {
					val.RemoveNodeFromParent();
					val.AddNodeTo(this);
				}
				oldPropertyElement.ParentNode.RemoveChild(oldPropertyElement);
			}
		}
		
		bool IsFirstChildResources(XamlObject obj)
		{
			return obj.XmlElement.FirstChild != null &&
				obj.XmlElement.FirstChild.Name.EndsWith("." + XamlConstants.ResourcesPropertyName) &&
				obj.Properties.Where((prop) => prop.IsResources).FirstOrDefault() != null;
		}

		XmlElement CreatePropertyElement()
		{
			Type propertyElementType = GetPropertyElementType();
			string ns = parentObject.OwnerDocument.GetNamespaceFor(propertyElementType);
			return parentObject.OwnerDocument.XmlDocument.CreateElement(
				parentObject.OwnerDocument.GetPrefixForNamespace(ns),
				propertyElementType.Name + "." + this.PropertyName,
				ns
			);
		}

		Type GetPropertyElementType()
		{
			return this.IsAttached ? this.PropertyTargetType : parentObject.ElementType;
		}
		
		static XmlNode FindChildNode(XmlNode node, string localName, string namespaceURI)
		{
			foreach (XmlNode childNode in node.ChildNodes) {
				if (childNode.LocalName == localName && childNode.NamespaceURI == namespaceURI)
					return childNode;
			}

			return null;
		}

		bool IsNodeCollectionForThisProperty(XmlNode node)
		{
			return _propertyElement == null && this.PropertyName != this.ParentObject.ContentPropertyName && this.ReturnType.IsAssignableFrom(this.ParentObject.OwnerDocument.TypeFinder.GetType(node.NamespaceURI, node.LocalName));
		}
		
		internal void AddChildNodeToProperty(XmlNode newChildNode)
		{
			if (this.IsCollection) {
				if (IsNodeCollectionForThisProperty(newChildNode)) {
					Type propertyElementType = GetPropertyElementType();
					XmlNode parentNode = FindChildNode(parentObject.XmlElement, propertyElementType.Name + "." + this.PropertyName, parentObject.OwnerDocument.GetNamespaceFor(propertyElementType));

					if (parentNode == null) {
						parentNode = CreatePropertyElement();

						parentObject.XmlElement.AppendChild(parentNode);
					}
					else if (parentNode.ChildNodes.Count > 0)
						throw new XamlLoadException("Collection property node must have no children when adding collection element.");

					parentNode.AppendChild(newChildNode);
					_propertyElement = (XmlElement)newChildNode;
				}
				else {
					// this is the default collection
					InsertNodeInCollection(newChildNode, collectionElements.Count);
				}
				
				return;
			}
			if (_propertyElement == null) {
				if (PropertyName == parentObject.ContentPropertyName) {
					if (IsFirstChildResources(parentObject)) {
						// Resources element should always be first
						parentObject.XmlElement.InsertAfter(newChildNode, parentObject.XmlElement.FirstChild);
					}
					else
						parentObject.XmlElement.InsertBefore(newChildNode, parentObject.XmlElement.FirstChild);
					return;
				}
				_propertyElement = CreatePropertyElement();
				
				if (IsFirstChildResources(parentObject)) {
					// Resources element should always be first
					parentObject.XmlElement.InsertAfter(_propertyElement, parentObject.XmlElement.FirstChild);
				}
				else
					parentObject.XmlElement.InsertBefore(_propertyElement, parentObject.XmlElement.FirstChild);
			}
			_propertyElement.AppendChild(newChildNode);
		}
		
		internal void InsertNodeInCollection(XmlNode newChildNode, int index)
		{
			Debug.Assert(index >= 0 && index <= collectionElements.Count);
			XmlElement collection = _propertyElement;
			if (collection == null) {
				if (collectionElements.Count == 0 && this.PropertyName != this.ParentObject.ContentPropertyName) {
					// we have to create the collection element
                    _propertyElement = CreatePropertyElement();

					if (this.IsResources) {
						parentObject.XmlElement.PrependChild(_propertyElement);
					} else {
						parentObject.XmlElement.AppendChild(_propertyElement);
					}

					collection = _propertyElement;
				} else {
					// this is the default collection
					collection = parentObject.XmlElement;
				}
			}
			if (collectionElements.Count == 0) {
				// collection is empty -> we may insert anywhere
				collection.AppendChild(newChildNode);
			} else if (index == collectionElements.Count) {
				// insert after last element in collection
				collection.InsertAfter(newChildNode, collectionElements[collectionElements.Count - 1].GetNodeForCollection());
			} else {
				// insert before specified index
				collection.InsertBefore(newChildNode, collectionElements[index].GetNodeForCollection());
			}
		}

		internal XmlAttribute SetAttribute(string value)
		{
			string name;
			var element = ParentObject.XmlElement;

			if (IsAttached)
			{
				if (PropertyTargetType == typeof (DesignTimeProperties) || PropertyTargetType == typeof (MarkupCompatibilityProperties))
					name = PropertyName;
				else
					name = PropertyTargetType.Name + "." + PropertyName;

				string ns = ParentObject.OwnerDocument.GetNamespaceFor(PropertyTargetType);
                string prefix = element.GetPrefixOfNamespace(ns);

				if (String.IsNullOrEmpty(prefix)) {
                    prefix = ParentObject.OwnerDocument.GetPrefixForNamespace(ns);
                }

				if (!string.IsNullOrEmpty(prefix)) {
					element.SetAttribute(name, ns, value);
					return element.GetAttributeNode(name, ns);
				}
			} else {
				name = PropertyName;
			}

			element.SetAttribute(name, string.Empty, value);
			return element.GetAttributeNode(name);
		}

		internal string GetNameForMarkupExtension()
		{
			if (IsAttached) {
				string name = PropertyTargetType.Name + "." + PropertyName;
				
				var element = ParentObject.XmlElement;
				string ns = ParentObject.OwnerDocument.GetNamespaceFor(PropertyTargetType);
				var prefix = element.GetPrefixOfNamespace(ns);
				if (string.IsNullOrEmpty(prefix))
					return name;
				else
					return prefix + ":" + name;
			} else
				return PropertyName;
		}
		
		/// <summary>
		/// used internally by the XamlParser.
		/// Add a collection element that already is part of the XML DOM.
		/// </summary>
		internal void ParserAddCollectionElement(XmlElement collectionPropertyElement, XamlPropertyValue val)
		{
			if (collectionPropertyElement != null && _propertyElement == null) {
				ParserSetPropertyElement(collectionPropertyElement);
			}
			collectionElements.AddInternal(val);
			val.ParentProperty = this;
			if (collectionPropertyElement != _propertyElement) {
				val.RemoveNodeFromParent();
				val.AddNodeTo(this);
			}
		}
		
		/// <summary>
		/// Gets/Sets the value of the property on the instance without updating the XAML document.
		/// </summary>
		public object ValueOnInstance {
			get {
				if (IsEvent) {
					if (propertyValue != null)
						return propertyValue.GetValueFor(null);
					else
						return null;
				} else {
					return propertyInfo.GetValue(parentObject.Instance);
				}
			}
			set {
				propertyInfo.SetValue(parentObject.Instance, value);
				if (ValueOnInstanceChanged != null)
					ValueOnInstanceChanged(this, EventArgs.Empty);
			}
		}

		/// <summary>
		/// Gets if this property is considered "advanced" and should be hidden by default in a property grid.
		/// </summary>
		public bool IsAdvanced {
			get { return propertyInfo.IsAdvanced; }
		}

		/// <summary>
		/// Gets the dependency property.
		/// </summary>
		public DependencyProperty DependencyProperty {
			get {
				return propertyInfo.DependencyProperty;
			}
		}

		void PossiblyNameChanged(XamlPropertyValue oldValue, XamlPropertyValue newValue)
		{
			if (ParentObject.RuntimeNameProperty != null && PropertyName == ParentObject.RuntimeNameProperty) {
				
				if (!String.IsNullOrEmpty(ParentObject.GetXamlAttribute("Name"))) {
					throw new XamlLoadException("The property 'Name' is set more than once.");
				}

				string oldName = null;
				string newName = null;

				var oldTextValue = oldValue as XamlTextValue;
				if (oldTextValue != null) oldName = oldTextValue.Text;
				
				var newTextValue = newValue as XamlTextValue;
				if (newTextValue != null) newName = newTextValue.Text;

				NameScopeHelper.NameChanged(ParentObject, oldName, newName);
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
}
