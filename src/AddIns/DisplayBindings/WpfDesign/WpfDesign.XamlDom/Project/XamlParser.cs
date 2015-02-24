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
using System.Globalization;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Xml;
using System.Windows.Media;

namespace ICSharpCode.WpfDesign.XamlDom
{
	/// <summary>
	/// Class with static methods to parse XAML files and output a <see cref="XamlDocument"/>.
	/// </summary>
	public sealed class XamlParser
	{
		#region Static methods
		/// <summary>
		/// Parses a XAML document using a stream.
		/// </summary>
		public static XamlDocument Parse(Stream stream)
		{
			return Parse(stream, new XamlParserSettings());
		}
		
		/// <summary>
		/// Parses a XAML document using a TextReader.
		/// </summary>
		public static XamlDocument Parse(TextReader reader)
		{
			return Parse(reader, new XamlParserSettings());
		}
		
		/// <summary>
		/// Parses a XAML document using an XmlReader.
		/// </summary>
		public static XamlDocument Parse(XmlReader reader)
		{
			return Parse(reader, new XamlParserSettings());
		}
		
		/// <summary>
		/// Parses a XAML document using a stream.
		/// </summary>
		public static XamlDocument Parse(Stream stream, XamlParserSettings settings)
		{
			if (stream == null)
				throw new ArgumentNullException("stream");
			return Parse(XmlReader.Create(stream), settings);
		}
		
		/// <summary>
		/// Parses a XAML document using a TextReader.
		/// </summary>
		public static XamlDocument Parse(TextReader reader, XamlParserSettings settings)
		{
			if (reader == null)
				throw new ArgumentNullException("reader");
			return Parse(XmlReader.Create(reader), settings);
		}

		private XmlNode currentParsedNode;
		
		/// <summary>
		/// Parses a XAML document using an XmlReader.
		/// </summary>
		public static XamlDocument Parse(XmlReader reader, XamlParserSettings settings)
		{
			if (reader == null)
				throw new ArgumentNullException("reader");
			if (settings == null)
				throw new ArgumentNullException("settings");
			
			XmlDocument doc = new PositionXmlDocument();
			var errorSink = (IXamlErrorSink)settings.ServiceProvider.GetService(typeof(IXamlErrorSink));

			try {
				doc.Load(reader);
				return Parse(doc, settings);
			} catch (XmlException x) {
				if (errorSink != null) {
					errorSink.ReportError(x.Message, x.LineNumber, x.LinePosition);
				} else {
					throw;
				}
			}

			return null;
		}
		
		/// <summary>
		/// Creates a XAML document from an existing XmlDocument.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes",
		                                                 Justification="We need to continue parsing, and the error is reported to the user.")]
		internal static XamlDocument Parse(XmlDocument document, XamlParserSettings settings)
		{
			if (document == null)
				throw new ArgumentNullException("document");
			if (settings == null)
				throw new ArgumentNullException("settings");
			XamlParser p = new XamlParser();
			p.settings = settings;
			p.errorSink = (IXamlErrorSink)settings.ServiceProvider.GetService(typeof(IXamlErrorSink));
			p.document = new XamlDocument(document, settings);
			
			try {
				var root = p.ParseObject(document.DocumentElement);
				p.document.ParseComplete(root);
			} catch (Exception x) {
				p.ReportException(x, p.currentParsedNode);
			}

			return p.document;
		}
		#endregion
		
		private XamlParser() { }
		
		XamlDocument document;
		XamlParserSettings settings;
		IXamlErrorSink errorSink;
		
		static Type FindType(XamlTypeFinder typeFinder, string namespaceUri, string localName)
		{
			Type elementType = typeFinder.GetType(namespaceUri, localName);
			if (elementType == null)
				elementType = typeFinder.GetType(namespaceUri, localName+"Extension");
			if (elementType == null)
				throw new XamlLoadException("Cannot find type " + localName + " in " + namespaceUri);
			return elementType;
		}
		
		static string GetAttributeNamespace(XmlAttribute attribute)
		{
			if (attribute.NamespaceURI.Length > 0)
				return attribute.NamespaceURI;
			else
			{
				var ns = attribute.OwnerElement.GetNamespaceOfPrefix("");
				if (string.IsNullOrEmpty(ns)) {
					ns = XamlConstants.PresentationNamespace;
				}
				return ns;
			}
		}
		
		readonly static object[] emptyObjectArray = new object[0];
		XmlSpace currentXmlSpace = XmlSpace.None;
		XamlObject currentXamlObject;

		void ReportException(Exception x, XmlNode node)
		{
			if (errorSink != null) {
				var lineInfo = node as IXmlLineInfo;
				var msg = x.Message;
				if (x.InnerException != null)
					msg += " (" + x.InnerException.Message + ")";
				if (lineInfo != null) {
					errorSink.ReportError(msg, lineInfo.LineNumber, lineInfo.LinePosition);
				} else {
					errorSink.ReportError(msg, 0, 0);
				}
				if (currentXamlObject != null) {
					currentXamlObject.HasErrors = true;
				}
			} else {
				throw x;
			}
		}
		
		XamlObject ParseObject(XmlElement element)
		{
			Type elementType = settings.TypeFinder.GetType(element.NamespaceURI, element.LocalName);

			if (typeof (FrameworkTemplate).IsAssignableFrom(elementType))
			{
				var xamlObj = new XamlObject(document, element, elementType, TemplateHelper.GetFrameworkTemplate(element, currentXamlObject));
				xamlObj.ParentObject = currentXamlObject;
				return xamlObj;
			}


			if (elementType == null) {
				elementType = settings.TypeFinder.GetType(element.NamespaceURI, element.LocalName + "Extension");
				if (elementType == null) {
					throw new XamlLoadException("Cannot find type " + element.Name);
				}
			}
			
			XmlSpace oldXmlSpace = currentXmlSpace;
			XamlObject parentXamlObject = currentXamlObject;
			if (element.HasAttribute("xml:space")) {
				currentXmlSpace = (XmlSpace)Enum.Parse(typeof(XmlSpace), element.GetAttribute("xml:space"), true);
			}
			
			XamlPropertyInfo defaultProperty = GetDefaultProperty(elementType);
			
			XamlTextValue initializeFromTextValueInsteadOfConstructor = null;
			
			if (defaultProperty == null) {
				int numberOfTextNodes = 0;
				bool onlyTextNodes = true;
				foreach (XmlNode childNode in element.ChildNodes) {
					if (childNode.NodeType == XmlNodeType.Text) {
						numberOfTextNodes++;
					} else if (childNode.NodeType == XmlNodeType.Element) {
						onlyTextNodes = false;
					}
				}
				if (onlyTextNodes && numberOfTextNodes == 1) {
					foreach (XmlNode childNode in element.ChildNodes) {
						if (childNode.NodeType == XmlNodeType.Text) {
							currentParsedNode = childNode;
							initializeFromTextValueInsteadOfConstructor = (XamlTextValue)ParseValue(childNode);
						}
					}
				}
			}

			currentParsedNode = element;

			object instance;
			if (initializeFromTextValueInsteadOfConstructor != null) {
				instance = TypeDescriptor.GetConverter(elementType).ConvertFromString(
					document.GetTypeDescriptorContext(null),
					CultureInfo.InvariantCulture,
					initializeFromTextValueInsteadOfConstructor.Text);
			} else {
				instance = settings.CreateInstanceCallback(elementType, emptyObjectArray);
			}
			
			XamlObject obj = new XamlObject(document, element, elementType, instance);
			currentXamlObject = obj;
			obj.ParentObject = parentXamlObject;

			if (parentXamlObject == null && obj.Instance is DependencyObject) {
				NameScope.SetNameScope((DependencyObject)obj.Instance, new NameScope());
			}
			
			ISupportInitialize iSupportInitializeInstance = instance as ISupportInitialize;
			if (iSupportInitializeInstance != null) {
				iSupportInitializeInstance.BeginInit();
			}
			
			foreach (XmlAttribute attribute in element.Attributes) {
				if (attribute.Value.StartsWith("clr-namespace", StringComparison.OrdinalIgnoreCase)) {
					// the format is "clr-namespace:<Namespace here>;assembly=<Assembly name here>"
					var clrNamespace = attribute.Value.Split(new[] {':', ';', '='});
					if (clrNamespace.Length == 4) {
						// get the assembly name
						var assembly=settings.TypeFinder.LoadAssembly(clrNamespace[3]);
						if(assembly!=null)
							settings.TypeFinder.RegisterAssembly(assembly);
					} else {
						// if no assembly name is there, then load the assembly of the opened file.
						var assembly=settings.TypeFinder.LoadAssembly(null);
						if(assembly!=null)
							settings.TypeFinder.RegisterAssembly(assembly);
					}
				}
				if (attribute.NamespaceURI == XamlConstants.XmlnsNamespace)
					continue;
				if (attribute.Name == "xml:space") {
					continue;
				}
				if (GetAttributeNamespace(attribute) == XamlConstants.XamlNamespace) {
					if (attribute.LocalName == "Name") {
						try {
							NameScopeHelper.NameChanged(obj, null, attribute.Value);
						} catch (Exception x) {
							ReportException(x, attribute);
						}
					}
					continue;
				}
				
				ParseObjectAttribute(obj, attribute);
			}
			
			ParseObjectContent(obj, element, defaultProperty, initializeFromTextValueInsteadOfConstructor);
			
			if (iSupportInitializeInstance != null) {
				iSupportInitializeInstance.EndInit();
			}
			
			currentXmlSpace = oldXmlSpace;
			currentXamlObject = parentXamlObject;
			
			return obj;
		}
		
		void ParseObjectContent(XamlObject obj, XmlElement element, XamlPropertyInfo defaultProperty, XamlTextValue initializeFromTextValueInsteadOfConstructor)
		{
			bool isDefaultValueSet = false;
			
			XamlProperty collectionProperty = null;
			object collectionInstance = null;
			Type collectionType = null;
			XmlElement collectionPropertyElement = null;
			var elementChildNodes = GetNormalizedChildNodes(element);
			
			if (defaultProperty == null && obj.Instance != null && CollectionSupport.IsCollectionType(obj.Instance.GetType())) {
				XamlObject parentObj = obj.ParentObject;
				var parentElement = element.ParentNode;
				XamlPropertyInfo propertyInfo;
				if (parentObj != null) {
					propertyInfo = GetPropertyInfo(settings.TypeFinder, parentObj.Instance, parentObj.ElementType, parentElement.NamespaceURI, parentElement.LocalName);
					collectionProperty = FindExistingXamlProperty(parentObj, propertyInfo);
				}
				collectionInstance = obj.Instance;
				collectionType = obj.ElementType;
				collectionPropertyElement = element;
			} else if (defaultProperty != null && defaultProperty.IsCollection && !element.IsEmpty) {
				foreach (XmlNode childNode in elementChildNodes) {
					currentParsedNode = childNode;
					XmlElement childElement = childNode as XmlElement;
					if (childElement == null || !ObjectChildElementIsPropertyElement(childElement)) {
						obj.AddProperty(collectionProperty = new XamlProperty(obj, defaultProperty));
						collectionType = defaultProperty.ReturnType;
						collectionInstance = defaultProperty.GetValue(obj.Instance);
						break;
					}
				}
			}

			currentParsedNode = element;

			if (collectionType != null && collectionInstance == null && elementChildNodes.Count() == 1)
			{
				var firstChild = elementChildNodes.First() as XmlElement;
				if (ObjectChildElementIsCollectionInstance(firstChild, collectionType))
				{
					collectionInstance = ParseObject(firstChild);
					collectionProperty.PropertyValue = (XamlPropertyValue) collectionInstance;
				}
				else
				{
					throw new XamlLoadException("Collection Instance is null");
				}
			}
			else
			{
				foreach (XmlNode childNode in elementChildNodes) {
					currentParsedNode = childNode;
					XmlElement childElement = childNode as XmlElement;
					if (childElement != null) {
						if (childElement.NamespaceURI == XamlConstants.XamlNamespace)
							continue;
						
						if (ObjectChildElementIsPropertyElement(childElement)) {
							ParseObjectChildElementAsPropertyElement(obj, childElement, defaultProperty);
							continue;
						}
					}
					if (initializeFromTextValueInsteadOfConstructor != null)
						continue;
					XamlPropertyValue childValue = ParseValue(childNode);
					if (childValue != null) {
						if (collectionProperty != null) {
							collectionProperty.ParserAddCollectionElement(collectionPropertyElement, childValue);
							CollectionSupport.AddToCollection(collectionType, collectionInstance, childValue);
						} else {
							if (defaultProperty == null)
								throw new XamlLoadException("This element does not have a default value, cannot assign to it");
							
							if (isDefaultValueSet)
								throw new XamlLoadException("default property may have only one value assigned");
							
							obj.AddProperty(new XamlProperty(obj, defaultProperty, childValue));
							isDefaultValueSet = true;
						}
					}
				}
			}

			currentParsedNode = element;
		}

		IEnumerable<XmlNode> GetNormalizedChildNodes(XmlElement element)
		{
			XmlNode node = element.FirstChild;
			while (node != null) {
				XmlText text = node as XmlText;
				XmlCDataSection cData = node as XmlCDataSection;
				if (node.NodeType == XmlNodeType.SignificantWhitespace) {
					text = element.OwnerDocument.CreateTextNode(node.Value);
					element.ReplaceChild(text, node);
					node = text;
				}
				if (text != null || cData != null) {
					node = node.NextSibling;
					while (node != null
					       && (node.NodeType == XmlNodeType.Text
					           || node.NodeType == XmlNodeType.CDATA
					           || node.NodeType == XmlNodeType.SignificantWhitespace)) {
						if (text != null) text.Value += node.Value;
						else cData.Value += node.Value;
						XmlNode nodeToDelete = node;
						node = node.NextSibling;
						element.RemoveChild(nodeToDelete);
					}
					if (text != null) yield return text;
					else yield return cData;
				} else {
					yield return node;
					node = node.NextSibling;
				}
			}
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes",
		                                                 Justification="We need to continue parsing, and the error is reported to the user.")]
		XamlPropertyValue ParseValue(XmlNode childNode)
		{
			currentParsedNode = childNode;

			try {
				return ParseValueCore(currentParsedNode);
			} catch (Exception x) {
				ReportException(x, currentParsedNode);
			}
			return null;
		}
		
		XamlPropertyValue ParseValueCore(XmlNode childNode)
		{
			XmlText childText = childNode as XmlText;
			if (childText != null) {
				return new XamlTextValue(document, childText, currentXmlSpace);
			}
			XmlCDataSection cData = childNode as XmlCDataSection;
			if (cData != null) {
				return new XamlTextValue(document, cData, currentXmlSpace);
			}
			XmlElement element = childNode as XmlElement;
			if (element != null) {
				return ParseObject(element);
			}
			return null;
		}
		
		static XamlProperty FindExistingXamlProperty(XamlObject obj, XamlPropertyInfo propertyInfo)
		{
			foreach (XamlProperty existing in obj.Properties) {
				if (existing.propertyInfo.FullyQualifiedName == propertyInfo.FullyQualifiedName)
					return existing;
			}

			throw new XamlLoadException("Existing XamlProperty " + propertyInfo.FullyQualifiedName + " not found.");
		}
		
		static XamlPropertyInfo GetDefaultProperty(Type elementType)
		{
			foreach (ContentPropertyAttribute cpa in elementType.GetCustomAttributes(typeof(ContentPropertyAttribute), true)) {
				return FindProperty(null, elementType, cpa.Name);
			}
			return null;
		}
		
		internal static XamlPropertyInfo FindProperty(object elementInstance, Type propertyType, string propertyName)
		{
			PropertyDescriptor propertyInfo = TypeDescriptor.GetProperties(propertyType)[propertyName];

			if (propertyInfo == null && elementInstance != null)
				propertyInfo = TypeDescriptor.GetProperties(elementInstance).OfType<DependencyPropertyDescriptor>().FirstOrDefault(x => x.IsAttached && x.Name == propertyName);
			
			if (propertyInfo != null) {
				return new XamlNormalPropertyInfo(propertyInfo);
			} else {
				XamlPropertyInfo pi = TryFindAttachedProperty(propertyType, propertyName);
				if (pi != null) {
					return pi;
				}
			}
			EventDescriptorCollection events;
			if (elementInstance != null) {
				events = TypeDescriptor.GetEvents(elementInstance);
			} else {
				events = TypeDescriptor.GetEvents(propertyType);
			}
			EventDescriptor eventInfo = events[propertyName];
			if (eventInfo != null) {
				return new XamlEventPropertyInfo(eventInfo);
			}

			throw new XamlLoadException("property " + propertyName + " not found");
		}
		
		internal static XamlPropertyInfo TryFindAttachedProperty(Type elementType, string propertyName)
		{
			MethodInfo getMethod = elementType.GetMethod("Get" + propertyName, BindingFlags.Public | BindingFlags.Static);
			MethodInfo setMethod = elementType.GetMethod("Set" + propertyName, BindingFlags.Public | BindingFlags.Static);
			if (getMethod != null || setMethod != null) {
				FieldInfo field = elementType.GetField(propertyName + "Property", BindingFlags.Public | BindingFlags.Static);
				if (field != null && field.FieldType == typeof(DependencyProperty)) {
					return new XamlDependencyPropertyInfo((DependencyProperty)field.GetValue(null), true);
				}
			}

			if (elementType.BaseType != null) {
				return TryFindAttachedProperty(elementType.BaseType, propertyName);
			}
			
			return null;
		}

		internal static XamlPropertyInfo TryFindAttachedEvent(Type elementType, string propertyName)
		{
			FieldInfo fieldEvent = elementType.GetField(propertyName + "Event", BindingFlags.Public | BindingFlags.Static);
			if (fieldEvent != null && fieldEvent.FieldType == typeof(RoutedEvent))
			{
				return new XamlEventPropertyInfo(TypeDescriptor.GetEvents(elementType)[propertyName]);
			}

			if (elementType.BaseType != null)
			{
				return TryFindAttachedEvent(elementType.BaseType, propertyName);
			}

			return null;
		}
		static XamlPropertyInfo FindAttachedProperty(Type elementType, string propertyName)
		{
			XamlPropertyInfo pi = TryFindAttachedProperty(elementType, propertyName);

			if (pi == null) {
				pi = TryFindAttachedEvent(elementType, propertyName);
			}
			if (pi != null) {
				return pi;
			} else {
				throw new XamlLoadException("attached property " + elementType.Name + "." + propertyName + " not found");
			}
		}
		
		static XamlPropertyInfo GetPropertyInfo(object elementInstance, Type elementType, XmlAttribute attribute, XamlTypeFinder typeFinder)
		{
			var ret = GetXamlSpecialProperty(attribute);
			if (ret != null)
				return ret;
			if (attribute.LocalName.Contains(".")) {
				return GetPropertyInfo(typeFinder, elementInstance, elementType, GetAttributeNamespace(attribute), attribute.LocalName);
			} else {
				return FindProperty(elementInstance, elementType, attribute.LocalName);
			}
		}
		
		internal static XamlPropertyInfo GetXamlSpecialProperty(XmlAttribute attribute)
		{
			if (attribute.LocalName == "Ignorable" && attribute.NamespaceURI == XamlConstants.MarkupCompatibilityNamespace) {
				return FindAttachedProperty(typeof(MarkupCompatibilityProperties), attribute.LocalName);
			} else if (attribute.LocalName == "DesignHeight" && attribute.NamespaceURI == XamlConstants.DesignTimeNamespace) {
				return FindAttachedProperty(typeof(DesignTimeProperties), attribute.LocalName);
			} else if (attribute.LocalName == "DesignWidth" && attribute.NamespaceURI == XamlConstants.DesignTimeNamespace) {
				return FindAttachedProperty(typeof(DesignTimeProperties), attribute.LocalName);
			} else if (attribute.LocalName == "IsHidden" && attribute.NamespaceURI == XamlConstants.DesignTimeNamespace) {
				return FindAttachedProperty(typeof(DesignTimeProperties), attribute.LocalName);
			} else if (attribute.LocalName == "IsLocked" && attribute.NamespaceURI == XamlConstants.DesignTimeNamespace) {
				return FindAttachedProperty(typeof(DesignTimeProperties), attribute.LocalName);
			} else if (attribute.LocalName == "LayoutOverrides" && attribute.NamespaceURI == XamlConstants.DesignTimeNamespace) {
				return FindAttachedProperty(typeof(DesignTimeProperties), attribute.LocalName);
			} else if (attribute.LocalName == "LayoutRounding" && attribute.NamespaceURI == XamlConstants.DesignTimeNamespace) {
				return FindAttachedProperty(typeof(DesignTimeProperties), attribute.LocalName);
			} else if (attribute.LocalName == "DataContext" && attribute.NamespaceURI == XamlConstants.DesignTimeNamespace) {
				return FindAttachedProperty(typeof(DesignTimeProperties), attribute.LocalName);
			}
			

			return null;
		}
		
		internal static XamlPropertyInfo GetPropertyInfo(XamlTypeFinder typeFinder, object elementInstance, Type elementType, string xmlNamespace, string localName, bool tryFindAllProperties = false)
		{
			string typeName, propertyName;
			SplitQualifiedIdentifier(localName, out typeName, out propertyName);
			Type propertyType = FindType(typeFinder, xmlNamespace, typeName);

			//Tries to Find All properties, even if they are not attached (For Setters, Bindings, ...)
			if (tryFindAllProperties)
			{
				XamlPropertyInfo propertyInfo = null;
				try
				{
					propertyInfo = FindProperty(elementInstance, propertyType, propertyName);
				}
				catch (Exception)
				{ }
				if (propertyInfo != null)
					return propertyInfo;
			}

			if (elementType.IsAssignableFrom(propertyType) || propertyType.IsAssignableFrom(elementType)) {
				return FindProperty(elementInstance, propertyType, propertyName);
			} else {
				// This is an attached property
				return FindAttachedProperty(propertyType, propertyName);
			}
		}
		
		static void SplitQualifiedIdentifier(string qualifiedName, out string typeName, out string propertyName)
		{
			int pos = qualifiedName.IndexOf('.');
			Debug.Assert(pos > 0);
			typeName = qualifiedName.Substring(0, pos);
			propertyName = qualifiedName.Substring(pos + 1);
		}
		
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes",
		                                                 Justification="We need to continue parsing, and the error is reported to the user.")]
		void ParseObjectAttribute(XamlObject obj, XmlAttribute attribute)
		{
			try {
				ParseObjectAttribute(obj, attribute, true);
			} catch (Exception x) {
				ReportException(x, attribute);
			}
		}

		internal static void ParseObjectAttribute(XamlObject obj, XmlAttribute attribute, bool real)
		{
			XamlPropertyInfo propertyInfo = GetPropertyInfo(obj.Instance, obj.ElementType, attribute, obj.OwnerDocument.TypeFinder);
			XamlPropertyValue value = null;

			var valueText = attribute.Value;
			if (valueText.StartsWith("{", StringComparison.Ordinal) && !valueText.StartsWith("{}", StringComparison.Ordinal)) {
				var xamlObject = MarkupExtensionParser.Parse(valueText, obj, real ? attribute : null);
				value = xamlObject;
			} else {
				if (real)
					value = new XamlTextValue(obj.OwnerDocument, attribute);
				else
					value = new XamlTextValue(obj.OwnerDocument, valueText);
			}

			var property = new XamlProperty(obj, propertyInfo, value);
			obj.AddProperty(property);
		}
		
		static bool ObjectChildElementIsPropertyElement(XmlElement element)
		{
			return element.LocalName.Contains(".");
		}

		static bool ObjectChildElementIsCollectionInstance(XmlElement element, Type collectionType)
		{
			return element.Name == collectionType.Name;
		}

		static bool IsElementChildACollectionForProperty(XamlTypeFinder typeFinder, XmlElement element, XamlPropertyInfo propertyInfo)
		{
			var nodes = element.ChildNodes.Cast<XmlNode>().Where(x => !(x is XmlWhitespace)).ToList();
			return nodes.Count == 1 && propertyInfo.ReturnType.IsAssignableFrom(FindType(typeFinder, nodes[0].NamespaceURI, nodes[0].LocalName));
		}
		
		void ParseObjectChildElementAsPropertyElement(XamlObject obj, XmlElement element, XamlPropertyInfo defaultProperty)
		{
			Debug.Assert(element.LocalName.Contains("."));
			// this is a element property syntax
			
			XamlPropertyInfo propertyInfo = GetPropertyInfo(settings.TypeFinder, obj.Instance, obj.ElementType, element.NamespaceURI, element.LocalName);
			bool valueWasSet = false;
			
			object collectionInstance = null;
			bool isElementChildACollectionForProperty = false;
			XamlProperty collectionProperty = null;
			if (propertyInfo.IsCollection) {
				if (defaultProperty != null && defaultProperty.FullyQualifiedName == propertyInfo.FullyQualifiedName) {
					foreach (XamlProperty existing in obj.Properties) {
						if (existing.propertyInfo == defaultProperty) {
							collectionProperty = existing;
							break;
						}
					}
				}
				
				if (collectionProperty == null) {
					obj.AddProperty(collectionProperty = new XamlProperty(obj, propertyInfo));
				}
				
				isElementChildACollectionForProperty = IsElementChildACollectionForProperty(settings.TypeFinder, element, propertyInfo);
				if (isElementChildACollectionForProperty)
					collectionProperty.ParserSetPropertyElement((XmlElement)element.ChildNodes.Cast<XmlNode>().Where(x => !(x is XmlWhitespace)).First());
				else {
					collectionInstance = collectionProperty.propertyInfo.GetValue(obj.Instance);
					collectionProperty.ParserSetPropertyElement(element);
					collectionInstance = collectionInstance ?? Activator.CreateInstance(collectionProperty.propertyInfo.ReturnType);
				}
			}
			
			XmlSpace oldXmlSpace = currentXmlSpace;
			if (element.HasAttribute("xml:space")) {
				currentXmlSpace = (XmlSpace)Enum.Parse(typeof(XmlSpace), element.GetAttribute("xml:space"), true);
			}
			
			foreach (XmlNode childNode in element.ChildNodes) {
				currentParsedNode = childNode;
				XamlPropertyValue childValue = ParseValue(childNode);
				if (childValue != null) {
					if (propertyInfo.IsCollection) {
						if (isElementChildACollectionForProperty) {
							collectionProperty.PropertyValue = childValue;
						}
						else {
							CollectionSupport.AddToCollection(propertyInfo.ReturnType, collectionInstance, childValue);
							collectionProperty.ParserAddCollectionElement(element, childValue);
						}
					} else {
						if (valueWasSet)
							throw new XamlLoadException("non-collection property may have only one child element");
						valueWasSet = true;
						XamlProperty xp = new XamlProperty(obj, propertyInfo, childValue);
						xp.ParserSetPropertyElement(element);
						obj.AddProperty(xp);
					}
				}
			}

			currentParsedNode = element;

			currentXmlSpace = oldXmlSpace;
		}

		internal static object CreateObjectFromAttributeText(string valueText, XamlPropertyInfo targetProperty, XamlObject scope)
		{
			if (targetProperty.ReturnType == typeof(Uri)) {
				return scope.OwnerDocument.TypeFinder.ConvertUriToLocalUri(new Uri(valueText, UriKind.RelativeOrAbsolute));
			} else if (targetProperty.ReturnType == typeof(ImageSource)) {
				var uri = scope.OwnerDocument.TypeFinder.ConvertUriToLocalUri(new Uri(valueText, UriKind.RelativeOrAbsolute));
				return targetProperty.TypeConverter.ConvertFromString(scope.OwnerDocument.GetTypeDescriptorContext(scope), CultureInfo.InvariantCulture, uri.ToString());
			}

			return targetProperty.TypeConverter.ConvertFromString(
				scope.OwnerDocument.GetTypeDescriptorContext(scope),
				CultureInfo.InvariantCulture, valueText);
		}

		internal static object CreateObjectFromAttributeText(string valueText, Type targetType, XamlObject scope)
		{
			var converter =
				XamlNormalPropertyInfo.GetCustomTypeConverter(targetType) ??
				TypeDescriptor.GetConverter(targetType);

			return converter.ConvertFromInvariantString(
				scope.OwnerDocument.GetTypeDescriptorContext(scope), valueText);
		}
		
		/// <summary>
		/// Removes namespace attributes defined in the root from the specified node and all child nodes.
		/// </summary>
		static void RemoveRootNamespacesFromNodeAndChildNodes(XamlObject root, XmlNode node)
		{
			foreach (XmlNode childNode in node.ChildNodes) {
				RemoveRootNamespacesFromNodeAndChildNodes(root, childNode);
			}

			if (node.Attributes != null) {
				List<XmlAttribute> removeAttributes = new List<XmlAttribute>();
				foreach (XmlAttribute attrib in node.Attributes) {
					if (attrib.Name.StartsWith("xmlns:")) {
						var rootPrefix = root.OwnerDocument.GetPrefixForNamespace(attrib.Value);
						if (rootPrefix == null) {
							//todo: check if we can add to root, (maybe same ns exists)
							root.OwnerDocument.XmlDocument.Attributes.Append((XmlAttribute)attrib.CloneNode(true));
							removeAttributes.Add(attrib);
						}
						else if (rootPrefix == attrib.Name.Substring("xmlns:".Length)) {
							removeAttributes.Add(attrib);
						}
					}
					else if (attrib.Name == "xmlns" && attrib.Value == XamlConstants.PresentationNamespace) {
						removeAttributes.Add(attrib);
					}
				}
				foreach (var removeAttribute in removeAttributes) {
					node.Attributes.Remove(removeAttribute);
				}
			}
		}

		/// <summary>
		/// Method use to parse a piece of Xaml.
		/// </summary>
		/// <param name="root">The Root XamlObject of the current document.</param>
		/// <param name="xaml">The Xaml being parsed.</param>
		/// <param name="settings">Parser settings used by <see cref="XamlParser"/>.</param>
		/// <returns>Returns the XamlObject of the parsed <paramref name="xaml"/>.</returns>
		public static XamlObject ParseSnippet(XamlObject root, string xaml, XamlParserSettings settings)
		{
			return ParseSnippet(root, xaml, settings, null);
		}

		/// <summary>
		/// Method use to parse a piece of Xaml.
		/// </summary>
		/// <param name="root">The Root XamlObject of the current document.</param>
		/// <param name="xaml">The Xaml being parsed.</param>
		/// <param name="settings">Parser settings used by <see cref="XamlParser"/>.</param>
		/// <param name="parentObject">Parent Object, where the Parsed snippet will be inserted (Needed for Example for Bindings).</param>
		/// <returns>Returns the XamlObject of the parsed <paramref name="xaml"/>.</returns>
		public static XamlObject ParseSnippet(XamlObject root, string xaml, XamlParserSettings settings, XamlObject parentObject)
		{
			XmlTextReader reader = new XmlTextReader(new StringReader(xaml));
			var element = root.OwnerDocument.XmlDocument.ReadNode(reader);
			
			if (element != null) {
				XmlAttribute xmlnsAttribute=null;
				foreach (XmlAttribute attrib in element.Attributes) {
					if (attrib.Name == "xmlns")
						xmlnsAttribute = attrib;
				}
				if(xmlnsAttribute!=null)
					element.Attributes.Remove(xmlnsAttribute);
				
				XamlParser parser = new XamlParser();
				parser.settings = settings;
				parser.errorSink = (IXamlErrorSink)settings.ServiceProvider.GetService(typeof(IXamlErrorSink));
				parser.document = root.OwnerDocument;
				parser.currentXamlObject = parentObject;
				var xamlObject = parser.ParseObject(element as XmlElement);

				RemoveRootNamespacesFromNodeAndChildNodes(root, element);

				if (xamlObject != null)
					return xamlObject;
			}
			return null;
		}
	}
}
