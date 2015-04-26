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
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Windows.Markup;
using System.Xaml;
using XamlReader = System.Windows.Markup.XamlReader;

namespace ICSharpCode.WpfDesign.XamlDom
{
	/// <summary>
	/// Allows finding types in a set of assemblies.
	/// </summary>
	public class XamlTypeFinder : ICloneable
	{
		sealed class AssemblyNamespaceMapping : IEquatable<AssemblyNamespaceMapping>
		{
			internal readonly Assembly Assembly;
			internal readonly string Namespace;
			
			internal AssemblyNamespaceMapping(Assembly assembly, string @namespace)
			{
				this.Assembly = assembly;
				this.Namespace = @namespace;
			}
			
			public override int GetHashCode()
			{
				return Assembly.GetHashCode() ^ Namespace.GetHashCode();
			}
			
			public override bool Equals(object obj)
			{
				return Equals(obj as AssemblyNamespaceMapping);
			}
			
			public bool Equals(AssemblyNamespaceMapping other)
			{
				return other != null && other.Assembly == this.Assembly && other.Namespace == this.Namespace;
			}
		}
		
		sealed class XamlNamespace
		{
			internal readonly string XmlNamespacePrefix;
			internal readonly string XmlNamespace;
			
			internal XamlNamespace(string xmlNamespacePrefix, string xmlNamespace)
			{
				this.XmlNamespacePrefix = xmlNamespacePrefix;
				this.XmlNamespace = xmlNamespace;
			}
			
			internal List<AssemblyNamespaceMapping> ClrNamespaces = new List<AssemblyNamespaceMapping>();
			
			internal XamlNamespace Clone()
			{
				XamlNamespace copy = new XamlNamespace(this.XmlNamespacePrefix, this.XmlNamespace);
				// AssemblyNamespaceMapping is immutable
				copy.ClrNamespaces.AddRange(this.ClrNamespaces);
				return copy;
			}
		}
		
		Dictionary<string, XamlNamespace> namespaces = new Dictionary<string, XamlNamespace>();
		Dictionary<AssemblyNamespaceMapping, string> reverseDict = new Dictionary<AssemblyNamespaceMapping, string>();
		Dictionary<AssemblyNamespaceMapping, List<string>> reverseDictList = new Dictionary<AssemblyNamespaceMapping, List<string>>();
		
		/// <summary>
		/// Gets a type referenced in XAML.
		/// </summary>
		/// <param name="xmlNamespace">The XML namespace to use to look up the type.
		/// This can be a registered namespace or a 'clr-namespace' value.</param>
		/// <param name="localName">The local name of the type to find.</param>
		/// <returns>
		/// The requested type, or null if it could not be found.
		/// </returns>
		public Type GetType(string xmlNamespace, string localName)
		{
			if (xmlNamespace == null)
				throw new ArgumentNullException("xmlNamespace");
			if (localName == null)
				throw new ArgumentNullException("localName");
			XamlNamespace ns;
			if (!namespaces.TryGetValue(xmlNamespace, out ns)) {
				if (xmlNamespace.StartsWith("clr-namespace:", StringComparison.Ordinal)) {
					ns = namespaces[xmlNamespace] = ParseNamespace(xmlNamespace);
				} else {
					return null;
				}
			}
			foreach (AssemblyNamespaceMapping mapping in ns.ClrNamespaces) {
				Type type = mapping.Assembly.GetType(mapping.Namespace + "." + localName);
				if (type != null)
					return type;
			}
			return null;
		}
		
		/// <summary>
		/// Gets the XML namespace that can be used for the specified assembly/namespace combination.
		/// </summary>
		public string GetXmlNamespaceFor(Assembly assembly, string @namespace, bool getClrNamespace = false)
		{
			AssemblyNamespaceMapping mapping = new AssemblyNamespaceMapping(assembly, @namespace);
			string xmlNamespace;
			if (!getClrNamespace && reverseDict.TryGetValue(mapping, out xmlNamespace)) {
				return xmlNamespace;
			} else {
				return "clr-namespace:" + mapping.Namespace + ";assembly=" + mapping.Assembly.GetName().Name;
			}
		}

		/// <summary>
		/// Gets the XML namespaces that can be used for the specified assembly/namespace combination.
		/// </summary>
		public List<string> GetXmlNamespacesFor(Assembly assembly, string @namespace, bool getClrNamespace = false)
		{
			AssemblyNamespaceMapping mapping = new AssemblyNamespaceMapping(assembly, @namespace);
			List<string> xmlNamespaces;
			if (!getClrNamespace && reverseDictList.TryGetValue(mapping, out xmlNamespaces)) {
				return xmlNamespaces;
			} else {
				return new List<string>() { "clr-namespace:" + mapping.Namespace + ";assembly=" + mapping.Assembly.GetName().Name };
			}
		}
		
		/// <summary>
		/// Gets the prefix to use for the specified XML namespace,
		/// or null if no suitable prefix could be found.
		/// </summary>
		public string GetPrefixForXmlNamespace(string xmlNamespace)
		{
			XamlNamespace ns;

			if (namespaces.TryGetValue(xmlNamespace, out ns)) {
				return ns.XmlNamespacePrefix;
			} else {
				return null;
			}
		}
		
		XamlNamespace ParseNamespace(string xmlNamespace)
		{
			string name = xmlNamespace;
			Debug.Assert(name.StartsWith("clr-namespace:", StringComparison.Ordinal));
			name = name.Substring("clr-namespace:".Length);
			string namespaceName, assembly;
			int pos = name.IndexOf(';');
			if (pos < 0) {
				namespaceName = name;
				assembly = "";
			} else {
				namespaceName = name.Substring(0, pos);
				name = name.Substring(pos + 1).Trim();
				if (!name.StartsWith("assembly=", StringComparison.Ordinal)) {
					throw new XamlLoadException("Expected: 'assembly='");
				}
				assembly = name.Substring("assembly=".Length);
			}
			XamlNamespace ns = new XamlNamespace(null, xmlNamespace);

			Assembly asm = LoadAssembly(assembly);

			if (asm == null && assembly == "mscorlib")
				asm = typeof (Boolean).Assembly;

			if (asm != null) {
				AddMappingToNamespace(ns, new AssemblyNamespaceMapping(asm, namespaceName));
			}
			return ns;
		}
		
		void AddMappingToNamespace(XamlNamespace ns, AssemblyNamespaceMapping mapping)
		{
			ns.ClrNamespaces.Add(mapping);

			List<string> xmlNamespaceList;
			if (reverseDictList.TryGetValue(mapping, out xmlNamespaceList)) {
				if (!xmlNamespaceList.Contains(ns.XmlNamespace))
					xmlNamespaceList.Add(ns.XmlNamespace);
			}
			else
				reverseDictList.Add(mapping, new List<string>(){ ns.XmlNamespace });
			
			string xmlNamespace;
			if (reverseDict.TryGetValue(mapping, out xmlNamespace)) {
				if (xmlNamespace == XamlConstants.PresentationNamespace) {
					return;
				}
			}
			reverseDict[mapping] = ns.XmlNamespace;
		}
		
		/// <summary>
		/// Registers XAML namespaces defined in the <paramref name="assembly"/> for lookup.
		/// </summary>
		public void RegisterAssembly(Assembly assembly)
		{
			if (assembly == null)
				throw new ArgumentNullException("assembly");

			Dictionary<string, string> namespacePrefixes = new Dictionary<string, string>();
			foreach (XmlnsPrefixAttribute xmlnsPrefix in assembly.GetCustomAttributes(typeof(XmlnsPrefixAttribute), true)) {
				namespacePrefixes.Add(xmlnsPrefix.XmlNamespace, xmlnsPrefix.Prefix);
			}

			foreach (XmlnsDefinitionAttribute xmlnsDef in assembly.GetCustomAttributes(typeof(XmlnsDefinitionAttribute), true)) {
				XamlNamespace ns;
				if (!namespaces.TryGetValue(xmlnsDef.XmlNamespace, out ns)) {
					string prefix;
					namespacePrefixes.TryGetValue(xmlnsDef.XmlNamespace, out prefix);
					ns = namespaces[xmlnsDef.XmlNamespace] = new XamlNamespace(prefix, xmlnsDef.XmlNamespace);
				}
				if (string.IsNullOrEmpty(xmlnsDef.AssemblyName)) {
					AddMappingToNamespace(ns, new AssemblyNamespaceMapping(assembly, xmlnsDef.ClrNamespace));
				} else {
					Assembly asm = LoadAssembly(xmlnsDef.AssemblyName);
					if (asm != null) {
						AddMappingToNamespace(ns, new AssemblyNamespaceMapping(asm, xmlnsDef.ClrNamespace));
					}
				}
			}
		}
		
		/// <summary>
		/// Register the Namspaces not found in any Assembly, but used by VS and Expression Blend
		/// </summary>
		public void RegisterDesignerNamespaces()
		{
			var ns = namespaces[XamlConstants.DesignTimeNamespace] = new XamlNamespace("d", XamlConstants.DesignTimeNamespace);
			AddMappingToNamespace(ns, new AssemblyNamespaceMapping(typeof(DesignTimeProperties).Assembly, typeof(DesignTimeProperties).Namespace));
			ns = namespaces[XamlConstants.MarkupCompatibilityNamespace] = new XamlNamespace("mc", XamlConstants.MarkupCompatibilityNamespace);
			AddMappingToNamespace(ns, new AssemblyNamespaceMapping(typeof(MarkupCompatibilityProperties).Assembly, typeof(MarkupCompatibilityProperties).Namespace));
		}
		
		/// <summary>
		/// Load the assembly with the specified name.
		/// You can override this method to implement custom assembly lookup.
		/// </summary>
		public virtual Assembly LoadAssembly(string name)
		{
			return Assembly.Load(name);
		}
		
		/// <summary>
		/// Clones this XamlTypeFinder.
		/// </summary>
		public virtual XamlTypeFinder Clone()
		{
			XamlTypeFinder copy = new XamlTypeFinder();
			copy.ImportFrom(this);
			return copy;
		}
		
		/// <summary>
		/// Import information from another XamlTypeFinder.
		/// Use this if you override Clone().
		/// </summary>
		protected void ImportFrom(XamlTypeFinder source)
		{
			if (source == null)
				throw new ArgumentNullException("source");
			foreach (KeyValuePair<string, XamlNamespace> pair in source.namespaces) {
				this.namespaces.Add(pair.Key, pair.Value.Clone());
			}
			foreach (KeyValuePair<AssemblyNamespaceMapping, string> pair in source.reverseDict) {
				this.reverseDict.Add(pair.Key, pair.Value);
			}
			foreach (KeyValuePair<AssemblyNamespaceMapping, List<string>> pair in source.reverseDictList) {
				this.reverseDictList.Add(pair.Key, pair.Value.ToList());
			}
		}
		
		object ICloneable.Clone()
		{
			return this.Clone();
		}
		
		/// <summary>
		/// Creates a new XamlTypeFinder where the WPF namespaces are registered.
		/// </summary>
		public static XamlTypeFinder CreateWpfTypeFinder()
		{
			return WpfTypeFinder.Instance.Clone();
		}
		
		/// <summary>
		/// Converts the specified <see cref="Uri"/> to local.
		/// </summary>
		public virtual Uri ConvertUriToLocalUri(Uri uri)
		{
			return uri;
		}
		
		static class WpfTypeFinder
		{
			internal static readonly XamlTypeFinder Instance;
			
			[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline",
			                                                 Justification = "We're using an explicit constructor to get it's lazy-loading semantics.")]
			static WpfTypeFinder()
			{
				Instance = new XamlTypeFinder();
				Instance.RegisterDesignerNamespaces();
				Instance.RegisterAssembly(typeof(MarkupExtension).Assembly); // WindowsBase
				Instance.RegisterAssembly(typeof(IAddChild).Assembly); // PresentationCore
				Instance.RegisterAssembly(typeof(XamlReader).Assembly); // PresentationFramework
				Instance.RegisterAssembly(typeof(XamlType).Assembly); // System.Xaml
				Instance.RegisterAssembly(typeof(Type).Assembly); // mscorelib
			}
		}
	}
}
