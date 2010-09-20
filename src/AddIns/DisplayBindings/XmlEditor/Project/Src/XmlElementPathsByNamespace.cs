// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ICSharpCode.XmlEditor
{
	public class XmlElementPathsByNamespace : Collection<XmlElementPath>
	{
		Dictionary<string, XmlElementPath> pathsByNamespace = new Dictionary<string, XmlElementPath>();
		XmlNamespaceCollection namespacesWithoutPaths = new XmlNamespaceCollection();
		
		public XmlElementPathsByNamespace(XmlElementPath path)
		{
			SeparateIntoPathsByNamespace(path);
			AddSeparatedPathsToCollection();
			FindNamespacesWithoutAssociatedPaths(path.NamespacesInScope);
			pathsByNamespace.Clear();
		}
		
		void SeparateIntoPathsByNamespace(XmlElementPath path)
		{
			foreach (QualifiedName elementName in path.Elements) {
				XmlElementPath matchedPath = FindOrCreatePath(elementName.Namespace);
				matchedPath.AddElement(elementName);
			}
		}
		
		XmlElementPath FindOrCreatePath(string elementNamespace)
		{
			XmlElementPath path = FindPath(elementNamespace);
			if (path != null) {
				return path;
			}
			return CreatePath(elementNamespace);
		}
		
		XmlElementPath FindPath(string elementNamespace)
		{
			XmlElementPath path;
			if (pathsByNamespace.TryGetValue(elementNamespace, out path)) {
				return path;
			}
			return null;
		}
		
		XmlElementPath CreatePath(string elementNamespace)
		{
			XmlElementPath path = new XmlElementPath();
			pathsByNamespace.Add(elementNamespace, path);
			return path;
		}
		
		void AddSeparatedPathsToCollection()
		{
			foreach (KeyValuePair<string, XmlElementPath> dictionaryEntry in pathsByNamespace) {
				Add(dictionaryEntry.Value);
			}
		}
		
		void FindNamespacesWithoutAssociatedPaths(XmlNamespaceCollection namespacesInScope)
		{
			foreach (XmlNamespace ns in namespacesInScope) {
				if (!HavePathForNamespace(ns)) {
					namespacesWithoutPaths.Add(ns);
				}
			}
		}
		
		bool HavePathForNamespace(XmlNamespace ns)
		{
			return pathsByNamespace.ContainsKey(ns.Name);
		}
		
		public XmlNamespaceCollection NamespacesWithoutPaths {
			get { return namespacesWithoutPaths; }
		}
	}
}
