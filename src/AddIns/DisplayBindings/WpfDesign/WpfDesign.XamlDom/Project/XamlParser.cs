// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Markup;
using System.Xml;

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
				p.ReportException(x, document.DocumentElement);
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
				throw new XamlLoadException("Cannot find type " + localName + " in " + namespaceUri);
			return elementType;
		}
		
		static string GetAttributeNamespace(XmlAttribute attribute)
		{
			if (attribute.NamespaceURI.Length > 0)
				return attribute.NamespaceURI;
			else
				return attribute.OwnerElement.GetNamespaceOfPrefix("");
		}
		
		readonly static object[] emptyObjectArray = new object[0];
		XmlSpace currentXmlSpace = XmlSpace.None;
		XamlObject currentXamlObject;

		void ReportException(Exception x, XmlNode node)
		{
			if (errorSink != null) {
				var lineInfo = node as IXmlLineInfo;
				if (lineInfo != null) {
					errorSink.ReportError(x.Message, lineInfo.LineNumber, lineInfo.LinePosition);
				} else {
					errorSink.ReportError(x.Message, 0, 0);
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
							initializeFromTextValueInsteadOfConstructor = (XamlTextValue)ParseValue(childNode);
						}
					}
				}
			}
			
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
				if (GetAttributeNamespace(attribute) == XamlConstants.XamlNamespace)
					continue;
				ParseObjectAttribute(obj, attribute);
			}
			
			if (!(obj.Instance is Style)) {
				ParseObjectContent(obj, element, defaultProperty, initializeFromTextValueInsteadOfConstructor);
			}
			
			if (iSupportInitializeInstance != null) {
				iSupportInitializeInstance.EndInit();
			}
			
			currentXmlSpace = oldXmlSpace;
			currentXamlObject = parentXamlObject;
			
			return obj;
		}
		
		void ParseObjectContent(XamlObject obj, XmlElement element, XamlPropertyInfo defaultProperty, XamlTextValue initializeFromTextValueInsteadOfConstructor)
		{
			XamlPropertyValue setDefaultValueTo = null;
			object defaultPropertyValue = null;
			XamlProperty defaultCollectionProperty = null;
			
			if (defaultProperty != null && defaultProperty.IsCollection && !element.IsEmpty) {
				defaultPropertyValue = defaultProperty.GetValue(obj.Instance);
				obj.AddProperty(defaultCollectionProperty = new XamlProperty(obj, defaultProperty));
			}
			
			foreach (XmlNode childNode in GetNormalizedChildNodes(element)) {
				
				// I don't know why the official XamlReader runs the property getter
				// here, but let's try to imitate it as good as possible
				if (defaultProperty != null && !defaultProperty.IsCollection) {
					for (; combinedNormalizedChildNodes > 0; combinedNormalizedChildNodes--) {
						defaultProperty.GetValue(obj.Instance);
					}
				}
				
				XmlElement childElement = childNode as XmlElement;
				if (childElement != null) {
					if (childElement.NamespaceURI == XamlConstants.XamlNamespace)
						continue;
					
					if (ObjectChildElementIsPropertyElement(childElement)) {
						// I don't know why the official XamlReader runs the property getter
						// here, but let's try to imitate it as good as possible
						if (defaultProperty != null && !defaultProperty.IsCollection) {
							defaultProperty.GetValue(obj.Instance);
						}
						ParseObjectChildElementAsPropertyElement(obj, childElement, defaultProperty, defaultPropertyValue);
						continue;
					}
				}
				if (initializeFromTextValueInsteadOfConstructor != null)
					continue;
				XamlPropertyValue childValue = ParseValue(childNode);
				if (childValue != null) {
					if (defaultProperty != null && defaultProperty.IsCollection) {
						defaultCollectionProperty.ParserAddCollectionElement(null, childValue);
						CollectionSupport.AddToCollection(defaultProperty.ReturnType, defaultPropertyValue, childValue);
					} else {
						if (setDefaultValueTo != null)
							throw new XamlLoadException("default property may have only one value assigned");
						setDefaultValueTo = childValue;
					}
				}
			}
			
			if (defaultProperty != null && !defaultProperty.IsCollection && !element.IsEmpty) {
				// Runs even when defaultValueSet==false!
				// Again, no idea why the official XamlReader does this.
				defaultProperty.GetValue(obj.Instance);
			}
			if (setDefaultValueTo != null) {
				if (defaultProperty == null) {
					throw new XamlLoadException("This element does not have a default value, cannot assign to it");
				}
				obj.AddProperty(new XamlProperty(obj, defaultProperty, setDefaultValueTo));
			}
		}
		
		int combinedNormalizedChildNodes;
		
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
						combinedNormalizedChildNodes++;
						
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
			try {
				return ParseValueCore(childNode);
			} catch (Exception x) {
				ReportException(x, childNode);
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
		
		static XamlPropertyInfo GetDefaultProperty(Type elementType)
		{
			foreach (ContentPropertyAttribute cpa in elementType.GetCustomAttributes(typeof(ContentPropertyAttribute), true)) {
				return FindProperty(null, elementType, cpa.Name);
			}
			return null;
		}
		
		internal static XamlPropertyInfo FindProperty(object elementInstance, Type propertyType, string propertyName)
		{
			PropertyDescriptorCollection properties;
			if (elementInstance != null) {
				properties = TypeDescriptor.GetProperties(elementInstance);
			} else {
				properties = TypeDescriptor.GetProperties(propertyType);
			}
			PropertyDescriptor propertyInfo = properties[propertyName];
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
			if (getMethod != null && setMethod != null) {
				FieldInfo field = elementType.GetField(propertyName + "Property", BindingFlags.Public | BindingFlags.Static);
				if (field != null && field.FieldType == typeof(DependencyProperty)) {
					return new XamlDependencyPropertyInfo((DependencyProperty)field.GetValue(null), true);
				}
			}
			return null;
		}
		
		static XamlPropertyInfo FindAttachedProperty(Type elementType, string propertyName)
		{
			XamlPropertyInfo pi = TryFindAttachedProperty(elementType, propertyName);
			if (pi != null) {
				return pi;
			} else {
				throw new XamlLoadException("attached property " + elementType.Name + "." + propertyName + " not found");
			}
		}
		
		static XamlPropertyInfo GetPropertyInfo(object elementInstance, Type elementType, XmlAttribute attribute, XamlTypeFinder typeFinder)
		{
			if (attribute.LocalName.Contains(".")) {
				return GetPropertyInfo(typeFinder, elementInstance, elementType, GetAttributeNamespace(attribute), attribute.LocalName);
			} else {
				return FindProperty(elementInstance, elementType, attribute.LocalName);
			}
		}
		
		internal static XamlPropertyInfo GetPropertyInfo(XamlTypeFinder typeFinder, object elementInstance, Type elementType, string xmlNamespace, string localName)
		{
			string typeName, propertyName;
			SplitQualifiedIdentifier(localName, out typeName, out propertyName);
			Type propertyType = FindType(typeFinder, xmlNamespace, typeName);
			if (elementType == propertyType || propertyType.IsAssignableFrom(elementType)) {
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
		
		void ParseObjectChildElementAsPropertyElement(XamlObject obj, XmlElement element, XamlPropertyInfo defaultProperty, object defaultPropertyValue)
		{
			Debug.Assert(element.LocalName.Contains("."));
			// this is a element property syntax
			
			XamlPropertyInfo propertyInfo = GetPropertyInfo(settings.TypeFinder, obj.Instance, obj.ElementType, element.NamespaceURI, element.LocalName);
			bool valueWasSet = false;
			
			object collectionInstance = null;
			XamlProperty collectionProperty = null;
			if (propertyInfo.IsCollection) {
				if (defaultProperty != null && defaultProperty.FullyQualifiedName == propertyInfo.FullyQualifiedName) {
					collectionInstance = defaultPropertyValue;
					foreach (XamlProperty existing in obj.Properties) {
						if (existing.propertyInfo == defaultProperty) {
							collectionProperty = existing;
							break;
						}
					}
				} else {
					collectionInstance = propertyInfo.GetValue(obj.Instance);
				}
				if (collectionProperty == null) {
					obj.AddProperty(collectionProperty = new XamlProperty(obj, propertyInfo));
				}
				collectionProperty.ParserSetPropertyElement(element);
			}
			
			XmlSpace oldXmlSpace = currentXmlSpace;
			if (element.HasAttribute("xml:space")) {
				currentXmlSpace = (XmlSpace)Enum.Parse(typeof(XmlSpace), element.GetAttribute("xml:space"), true);
			}
			
			foreach (XmlNode childNode in element.ChildNodes) {
				XamlPropertyValue childValue = ParseValue(childNode);
				if (childValue != null) {
					if (propertyInfo.IsCollection) {
						CollectionSupport.AddToCollection(propertyInfo.ReturnType, collectionInstance, childValue);
						collectionProperty.ParserAddCollectionElement(element, childValue);
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
			
			currentXmlSpace = oldXmlSpace;
		}

		internal static object CreateObjectFromAttributeText(string valueText, XamlPropertyInfo targetProperty, XamlObject scope)
		{
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
		/// Method use to parse a piece of Xaml.
		/// </summary>
		/// <param name="root">The Root XamlObject of the current document.</param>
		/// <param name="xaml">The Xaml being parsed.</param>
		/// <param name="settings">Parser settings used by <see cref="XamlParser"/>.</param>
		/// <returns>Returns the XamlObject of the parsed <paramref name="xaml"/>.</returns>
		public static XamlObject ParseSnippet(XamlObject root, string xaml, XamlParserSettings settings)
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
				parser.document = root.OwnerDocument;
				var xamlObject = parser.ParseObject(element as XmlElement);
				if (xamlObject != null)
					return xamlObject;
			}
			return null;
		}
	}
}
