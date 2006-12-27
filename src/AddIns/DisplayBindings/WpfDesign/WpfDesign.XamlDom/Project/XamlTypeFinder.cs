// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Markup;

namespace ICSharpCode.WpfDesign.XamlDom
{
	/// <summary>
	/// Allows finding types in a set of assemblies.
	/// </summary>
	public class XamlTypeFinder : ICloneable
	{
		sealed class AssemblyNamespaceMapping
		{
			internal readonly Assembly Assembly;
			internal readonly string Namespace;
			
			internal AssemblyNamespaceMapping(Assembly assembly, string @namespace)
			{
				this.Assembly = assembly;
				this.Namespace = @namespace;
			}
		}
		
		sealed class XamlNamespace
		{
			internal List<AssemblyNamespaceMapping> ClrNamespaces = new List<AssemblyNamespaceMapping>();
			
			internal XamlNamespace Clone()
			{
				XamlNamespace copy = new XamlNamespace();
				// AssemblyNamespaceMapping is immutable
				copy.ClrNamespaces.AddRange(this.ClrNamespaces);
				return copy;
			}
		}
		
		Dictionary<string, XamlNamespace> namespaces = new Dictionary<string, XamlNamespace>();
		
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
				if (xmlNamespace.StartsWith("clr-namespace:")) {
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
		
		XamlNamespace ParseNamespace(string name)
		{
			Debug.Assert(name.StartsWith("clr-namespace:"));
			name = name.Substring("clr-namespace:".Length);
			string namespaceName, assembly;
			int pos = name.IndexOf(';');
			if (pos < 0) {
				namespaceName = name;
				assembly = "";
			} else {
				namespaceName = name.Substring(0, pos);
				name = name.Substring(pos + 1).Trim();
				if (!name.StartsWith("assembly=")) {
					throw new XamlLoadException("Expected: 'assembly='");
				}
				assembly = name.Substring("assembly=".Length);
			}
			XamlNamespace ns = new XamlNamespace();
			ns.ClrNamespaces.Add(new AssemblyNamespaceMapping(LoadAssembly(assembly), namespaceName));
			return ns;
		}
		
		/// <summary>
		/// Registers XAML namespaces defined in the <paramref name="assembly"/> for lookup.
		/// </summary>
		public void RegisterAssembly(Assembly assembly)
		{
			if (assembly == null)
				throw new ArgumentNullException("assembly");
			foreach (XmlnsDefinitionAttribute xmlnsDef in assembly.GetCustomAttributes(typeof(XmlnsDefinitionAttribute), true)) {
				XamlNamespace ns;
				if (!namespaces.TryGetValue(xmlnsDef.XmlNamespace, out ns)) {
					ns = namespaces[xmlnsDef.XmlNamespace] = new XamlNamespace();
				}
				if (string.IsNullOrEmpty(xmlnsDef.AssemblyName)) {
					ns.ClrNamespaces.Add(new AssemblyNamespaceMapping(assembly, xmlnsDef.ClrNamespace));
				} else {
					Assembly asm = LoadAssembly(xmlnsDef.AssemblyName);
					if (asm != null) {
						ns.ClrNamespaces.Add(new AssemblyNamespaceMapping(asm, xmlnsDef.ClrNamespace));
					}
				}
			}
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
		
		static class WpfTypeFinder
		{
			internal static readonly XamlTypeFinder Instance;
			
			[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline")]
			static WpfTypeFinder()
			{
				Instance = new XamlTypeFinder();
				Instance.RegisterAssembly(typeof(MarkupExtension).Assembly); // WindowsBase
				Instance.RegisterAssembly(typeof(IAddChild).Assembly); // PresentationCore
				Instance.RegisterAssembly(typeof(XamlReader).Assembly); // PresentationFramework
			}
		}
	}
}
