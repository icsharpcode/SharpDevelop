using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Markup;
using System.Xml.Linq;

namespace ICSharpCode.Xaml
{
	public class XamlTypeFinder
	{
		public XamlTypeFinder(XamlProject project)
		{
			Project = project;
		}

		public XamlProject Project { get; private set; }

		Dictionary<XNamespace, XamlNamespace> xamlNamespaces = new Dictionary<XNamespace, XamlNamespace>();
		Dictionary<Mapping, XNamespace> xmlNamespaces = new Dictionary<Mapping, XNamespace>();

		public XamlType FindType(XName name)
		{
			if (name == IntristicType.CodeName) return IntristicType.Code;
			if (name == IntristicType.XDataName) return IntristicType.XData;

			return FindType(name, null);
		}

		public XamlType FindExtensionType(XName name)
		{
			return FindType(name, "Extension");
		}

		XamlType FindType(XName name, string suffix)
		{
			XamlNamespace ns;
			if (!xamlNamespaces.TryGetValue(name.Namespace, out ns)) {
				var mapping = ParseMappingString(name.Namespace);
				if (mapping != null) {
					ns = new XamlNamespace() { XmlNamespace = name.Namespace };
					AddMapping(ns, mapping);
					xamlNamespaces[name.Namespace] = ns;
				}
				else {
					return null;
				}
			}
			foreach (var mapping in ns.Mappings) {
				var type = mapping.Assembly.GetType(mapping.ClrNamespace + "." + name.LocalName + suffix);
				if (type != null) return type;
			}
			return null;
		}

		public XNamespace GetXmlNamespaceForType(XamlType type)
		{
			Mapping mapping = new Mapping() { Assembly = type.Assembly, ClrNamespace = type.Namespace };
			XNamespace ns;
			if (xmlNamespaces.TryGetValue(mapping, out ns)) {
				return ns;
			}
			if (mapping.Assembly == null || mapping.Assembly == Project.ProjectAssembly) {
				return "clr-namespace:" + mapping.ClrNamespace;
			}
			return "clr-namespace:" + mapping.ClrNamespace + ";assembly=" + mapping.Assembly.Name;
		}

		public void RegisterAssembly(XamlAssembly assembly)
		{
			foreach (var def in assembly.XmlnsDefinitions) {
				XamlNamespace ns;
				if (!xamlNamespaces.TryGetValue(def.XmlNamespace, out ns)) {
					ns = new XamlNamespace() { XmlNamespace = def.XmlNamespace };
					xamlNamespaces[ns.XmlNamespace] = ns;
				}
				//TODO def.AssemblyName
				AddMapping(ns, new Mapping() { Assembly = assembly, ClrNamespace = def.ClrNamespace });
			}
		}

		public void UnregisterAssembly(XamlAssembly assembly)
		{
			foreach (var ns in xamlNamespaces.Values) {
				foreach (var mapping in ns.Mappings.ToArray()) {
					if (mapping.Assembly == assembly) {
						RemoveMapping(ns, mapping);
					}
				}
				if (ns.Mappings.Count == 0) {
					xamlNamespaces.Remove(ns.XmlNamespace);
				}
			}
		}

		void AddMapping(XamlNamespace ns, Mapping mapping)
		{
			ns.Mappings.Add(mapping);

			// XamlWriter compares prefixes lengths (av < wpf, av < xps)
			// but we want Presentation2007Namespace as default
			XNamespace xmlns;
			xmlNamespaces.TryGetValue(mapping, out xmlns);
			if (xmlns != XamlConstants.Presentation2007Namespace) {
				xmlNamespaces[mapping] = ns.XmlNamespace;
			}
		}

		void RemoveMapping(XamlNamespace ns, Mapping mapping)
		{
			ns.Mappings.Remove(mapping);
			xmlNamespaces.Remove(mapping);
		}

		Mapping ParseMappingString(XNamespace ns)
		{
			var text = ns.NamespaceName;
			if (text.StartsWith("clr-namespace:")) {
				var mapping = new Mapping();
				text = text.Substring("clr-namespace:".Length);

				int pos = text.IndexOf(';');
				if (pos < 0) {
					mapping.ClrNamespace = text;
					mapping.Assembly = FindAssembly(null);
				}
				else {
					mapping.ClrNamespace = text.Substring(0, pos);
					text = text.Substring(pos + 1).Trim();
					if (!text.StartsWith("assembly=")) {
						throw new XamlException("Expected: 'assembly='");
					}
					var assemblyName = text.Substring("assembly=".Length);
					mapping.Assembly = FindAssembly(assemblyName);
				}
				return mapping;
			}
			return null;
		}

		XamlAssembly FindAssembly(string name)
		{
			XamlAssembly result = null;
			if (string.IsNullOrEmpty(name)) {
				result = Project.ProjectAssembly;
			}
			else {
				foreach (var a in Project.References) {
					if (a.Name == name) {
						result = a;
					}
				}
			}
			if (result != null) {
				return result;
			}
			throw new XamlException(string.Format("Assembly '{0}' not found in project.", name));
		}

		class XamlNamespace
		{
			public XNamespace XmlNamespace;
			public List<Mapping> Mappings = new List<Mapping>();
		}

		class Mapping : IEquatable<Mapping>
		{
			public XamlAssembly Assembly;
			public string ClrNamespace;

			public override int GetHashCode()
			{
				return Assembly.GetHashCode() ^ ClrNamespace.GetHashCode();
			}

			public override bool Equals(object obj)
			{
				return Equals(obj as Mapping);
			}

			public bool Equals(Mapping other)
			{
				return other != null && other.Assembly == this.Assembly && other.ClrNamespace == this.ClrNamespace;
			}
		}
	}
}
