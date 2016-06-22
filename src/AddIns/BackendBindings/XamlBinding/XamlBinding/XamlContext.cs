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
using System.Collections.ObjectModel;
using System.Linq;

using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.Xml;
using ICSharpCode.SharpDevelop.Editor;

namespace ICSharpCode.XamlBinding
{
	public class XamlNamespace
	{
		public string XmlNamespace { get; private set; }
		
		public IList<ITypeDefinition> GetContents(ICompilation compilation)
		{
			List<INamespace> namespaces = ResolveNamespaces(compilation).ToList();
			var contents = new List<ITypeDefinition>();
			foreach (var ns in namespaces) {
				contents.AddRange(ns.Types);
			}
			return contents;
		}
		
		public XamlNamespace(string xmlNamespace)
		{
			this.XmlNamespace = xmlNamespace;
		}
		
		IEnumerable<INamespace> ResolveNamespaces(ICompilation compilation)
		{
			IType xmlnsDefinition = compilation.FindType(typeof(System.Windows.Markup.XmlnsDefinitionAttribute));
			if (XmlNamespace.StartsWith("clr-namespace:", StringComparison.Ordinal)) {
				string name = XmlNamespace.Substring("clr-namespace:".Length);
				IAssembly asm = compilation.MainAssembly;
				int asmIndex = name.IndexOf(";assembly=", StringComparison.Ordinal);
				if (asmIndex >= 0) {
					string asmName = name.Substring(asmIndex + ";assembly=".Length);
					asm = compilation.ReferencedAssemblies.FirstOrDefault(a => a.AssemblyName == asmName) ?? compilation.MainAssembly;
					name = name.Substring(0, asmIndex);
				}
				string[] parts = name.Split('.');
				var @namespace = FindNamespace(asm, parts);
				if (@namespace != null) yield return @namespace;
			} else {
				foreach (IAssembly asm in compilation.Assemblies) {
					foreach (IAttribute attr in asm.AssemblyAttributes) {
						if (xmlnsDefinition.Equals(attr.AttributeType) && attr.PositionalArguments.Count == 2) {
							string xmlns = attr.PositionalArguments[0].ConstantValue as string;
							if (xmlns != XmlNamespace) continue;
							string ns = attr.PositionalArguments[1].ConstantValue as string;
							if (ns == null) continue;
							var @namespace = FindNamespace(asm, ns.Split('.'));
							if (@namespace != null) yield return @namespace;
						}
					}
				}
			}
		}
		
		static INamespace FindNamespace(IAssembly asm, string[] parts)
		{
			INamespace ns = asm.RootNamespace;
			for (int i = 0; i < parts.Length; i++) {
				INamespace tmp = ns.ChildNamespaces.FirstOrDefault(n => n.Name == parts[i]);
				if (tmp == null)
					return null;
				ns = tmp;
			}
			return ns;
		}
	}
	
	public class XamlContext
	{
		public AXmlElement ActiveElement { get; set; }
		public AXmlElement ParentElement { get; set; }
		public AXmlElement RootElement { get; set; }
		public ReadOnlyCollection<AXmlElement> Ancestors { get; set; }
		public AXmlAttribute Attribute { get; set; }
		public AttributeValue AttributeValue { get; set; }
		public string RawAttributeValue { get; set; }
		public int ValueStartOffset { get; set; }
		public XamlContextDescription Description { get; set; }
		public Dictionary<string, XamlNamespace> XmlnsDefinitions { get; set; }
		public XamlFullParseInformation ParseInformation { get; set; }
		public bool InRoot { get; set; }
		public ReadOnlyCollection<string> IgnoredXmlns { get; set; }
		public string XamlNamespacePrefix { get; set; }
		
		public XamlContext() {}
		
		public bool InAttributeValueOrMarkupExtension {
			get { return Description == XamlContextDescription.InMarkupExtension ||
					Description == XamlContextDescription.InAttributeValue; }
		}
		
		public bool InCommentOrCData {
			get { return Description == XamlContextDescription.InComment ||
					Description == XamlContextDescription.InCData; }
		}
	}
	
	public class XamlCompletionContext : XamlContext
	{
		public XamlCompletionContext() { }
		
		public XamlCompletionContext(XamlContext context)
		{
			this.ActiveElement = context.ActiveElement;
			this.Ancestors = context.Ancestors;
			this.Attribute = context.Attribute;
			this.AttributeValue = context.AttributeValue;
			this.Description = context.Description;
			this.ParentElement = context.ParentElement;
			this.RootElement = context.RootElement;
			this.ParseInformation = context.ParseInformation;
			this.RawAttributeValue = context.RawAttributeValue;
			this.ValueStartOffset = context.ValueStartOffset;
			this.XmlnsDefinitions = context.XmlnsDefinitions;
			this.InRoot = context.InRoot;
			this.IgnoredXmlns = context.IgnoredXmlns;
			this.XamlNamespacePrefix = context.XamlNamespacePrefix;
		}
		
		public char PressedKey { get; set; }
		public bool Forced { get; set; }
		public ITextEditor Editor { get; set; }
	}
	
	public enum XamlContextDescription
	{
		/// <summary>
		/// Outside any tag
		/// </summary>
		None,
		/// <summary>
		/// After '&lt;'
		/// </summary>
		AtTag,
		/// <summary>
		/// Inside '&lt;TagName &gt;'
		/// </summary>
		InTag,
		/// <summary>
		/// Inside '="Value"'
		/// </summary>
		InAttributeValue,
		/// <summary>
		/// Inside '="{}"'
		/// </summary>
		InMarkupExtension,
		/// <summary>
		/// Inside '&lt;!-- --&gt;'
		/// </summary>
		InComment,
		/// <summary>
		/// Inside '&lt;![CDATA[]]&gt;'
		/// </summary>
		InCData
	}
}
