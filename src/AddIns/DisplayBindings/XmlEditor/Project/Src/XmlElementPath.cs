// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Text;

namespace ICSharpCode.XmlEditor
{
	/// <summary>
	/// Represents the path to an xml element starting from the root of the
	/// document.
	/// </summary>
	public class XmlElementPath
	{
		QualifiedNameCollection elements = new QualifiedNameCollection();
		XmlNamespaceCollection namespacesInScope = new XmlNamespaceCollection();
		
		public XmlElementPath()
		{
		}
		
		/// <summary>
		/// Gets the elements specifying the path.
		/// </summary>
		/// <remarks>The order of the elements determines the path.</remarks>
		public QualifiedNameCollection Elements {
			get { return elements; }
		}
		
		public void AddElement(QualifiedName elementName)
		{
			elements.Add(elementName);
		}
		
		public bool IsEmpty {
			get { return elements.IsEmpty; }
		}
		
		/// <summary>
		/// Compacts the path so it only contains the elements that are from 
		/// the namespace of the last element in the path. 
		/// </summary>
		/// <remarks>This method is used when we need to know the path for a
		/// particular namespace and do not care about the complete path.
		/// </remarks>
		public void Compact()
		{
			if (elements.HasItems) {
				QualifiedName lastName = Elements.GetLast();
				int index = LastIndexNotMatchingNamespace(lastName.Namespace);
				if (index != -1) {
					elements.RemoveFirst(index + 1);
				}
			}
		}
		
		public XmlNamespaceCollection NamespacesInScope {
			get { return namespacesInScope; }
		}
		
		public string GetNamespaceForPrefix(string prefix)
		{
			return namespacesInScope.GetNamespaceForPrefix(prefix);
		}
		
		/// <summary>
		/// An xml element path is considered to be equal if 
		/// each path item has the same name and namespace.
		/// </summary>
		public override bool Equals(object obj) 
		{
			XmlElementPath rhsPath = obj as XmlElementPath;			
			if (rhsPath == null) {
				return false;
			}
			
			return elements.Equals(rhsPath.elements);
		}
		
		public override int GetHashCode() 
		{
			return elements.GetHashCode();
		}
		
		public override string ToString()
		{
			return elements.ToString();
		}
		
		public string GetRootNamespace() 
		{
			return elements.GetRootNamespace();
		}
		
		/// <summary>
		/// Only updates those names without a namespace.
		/// </summary>
		public void SetNamespaceForUnqualifiedNames(string namespaceUri)
		{
			foreach (QualifiedName name in elements) {
				if (!name.HasNamespace) {
					name.Namespace = namespaceUri;
				}
			}
		}
				
		int LastIndexNotMatchingNamespace(string namespaceUri)
		{
			if (elements.Count > 1) {
				// Start the check from the last but one item.
				for (int i = elements.Count - 2; i >= 0; --i) {
					QualifiedName name = elements[i];
					if (name.Namespace != namespaceUri) {
						return i;
					}
				}
			}
			return -1;
		}
	}
}
