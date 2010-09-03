// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Markup;
using System.Xml;

namespace ICSharpCode.WpfDesign.XamlDom
{
	sealed class XamlTypeResolverProvider : IXamlTypeResolver, IServiceProvider
	{
		XamlDocument document;
		XamlObject containingObject;
		
		public XamlTypeResolverProvider(XamlObject containingObject)
		{
			if (containingObject == null)
				throw new ArgumentNullException("containingObject");
			this.document = containingObject.OwnerDocument;
			this.containingObject = containingObject;
		}

		XmlElement ContainingElement{
			get { return containingObject.XmlElement; }
		}
		
		public Type Resolve(string typeName)
		{
			string typeNamespaceUri;
			string typeLocalName;
			if (typeName.Contains(":")) {
				typeNamespaceUri = ContainingElement.GetNamespaceOfPrefix(typeName.Substring(0, typeName.IndexOf(':')));
				typeLocalName = typeName.Substring(typeName.IndexOf(':') + 1);
			} else {
				typeNamespaceUri = ContainingElement.GetNamespaceOfPrefix("");
				typeLocalName = typeName;
			}
			if (string.IsNullOrEmpty(typeNamespaceUri))
				throw new XamlMarkupExtensionParseException("Unrecognized namespace prefix in type " + typeName);
			return document.TypeFinder.GetType(typeNamespaceUri, typeLocalName);
		}
		
		public object GetService(Type serviceType)
		{
			if (serviceType == typeof(IXamlTypeResolver) || serviceType == typeof(XamlTypeResolverProvider))
				return this;
			else
				return document.ServiceProvider.GetService(serviceType);
		}
		
		public XamlPropertyInfo ResolveProperty(string propertyName)
		{
			string propertyNamespace;
			if (propertyName.Contains(":")) {
				propertyNamespace = ContainingElement.GetNamespaceOfPrefix(propertyName.Substring(0, propertyName.IndexOf(':')));
				propertyName = propertyName.Substring(propertyName.IndexOf(':') + 1);
			} else {
				propertyNamespace = ContainingElement.GetNamespaceOfPrefix("");
			}
			Type elementType = null;
			XamlObject obj = containingObject;
			while (obj != null) {
				Style style = obj.Instance as Style;
				if (style != null && style.TargetType != null) {
					elementType = style.TargetType;
					break;
				}
				obj = obj.ParentObject;
			}
			if (propertyName.Contains(".")) {
				return XamlParser.GetPropertyInfo(document.TypeFinder, null, elementType, propertyNamespace, propertyName);
			} else if (elementType != null) {
				return XamlParser.FindProperty(null, elementType, propertyName);
			} else {
				return null;
			}
		}
		
		public object FindResource(object key)
		{
			XamlObject obj = containingObject;
			while (obj != null) {
				FrameworkElement el = obj.Instance as FrameworkElement;
				if (el != null) {
					object val = el.Resources[key];
					if (val != null)
						return val;
				}
				obj = obj.ParentObject;
			}
			return null;
		}
	}
}
