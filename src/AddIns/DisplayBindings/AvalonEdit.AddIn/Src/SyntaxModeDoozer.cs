// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;

using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using ICSharpCode.Core;

namespace ICSharpCode.AvalonEdit.AddIn
{
	public class AddInTreeSyntaxMode
	{
		ICSharpCode.Core.AddIn addin;
		string resourceName;
		
		public string Name { get; private set; }
		public string[] Extensions { get; private set; }
		
		public AddInTreeSyntaxMode(ICSharpCode.Core.AddIn addin, string resourceName, string name, string[] extensions)
		{
			if (addin == null)
				throw new ArgumentNullException("addin");
			if (resourceName == null)
				throw new ArgumentNullException("resourceName");
			if (name == null)
				throw new ArgumentNullException("name");
			if (extensions == null)
				throw new ArgumentNullException("extensions");
			
			this.addin = addin;
			this.resourceName = resourceName;
			this.Name = name;
			this.Extensions = extensions;
		}
		
		public XmlReader CreateXmlReader()
		{
			Stream stream = addin.GetManifestResourceStream(resourceName);
			if (stream != null) {
				return new XmlTextReader(stream);
			} else {
				throw new InvalidOperationException("Could not find resource '" + resourceName + "' in any of the AddIn runtime assemblies.");
			}
		}
		
		public XshdSyntaxDefinition LoadXshd()
		{
			using (XmlReader reader = CreateXmlReader()) {
				XshdSyntaxDefinition xshd = HighlightingLoader.LoadXshd(reader);
				if (xshd.Name != this.Name)
					throw new InvalidOperationException("Loaded XSHD has name '" + xshd.Name + "', but expected was '" + this.Name + "'.");
				if (!Enumerable.SequenceEqual(xshd.Extensions, this.Extensions))
					throw new InvalidOperationException("Loaded XSHD has extensions '" + string.Join(";", xshd.Extensions) + "', but expected was '" + string.Join(";", this.Extensions) + "'.");
				return xshd;
			}
		}
		
		public void Register(HighlightingManager manager)
		{
			manager.RegisterHighlighting(
				this.Name, this.Extensions, delegate {
					return HighlightingLoader.Load(LoadXshd(), manager);
				});
		}
	}
	
	/// <summary>
	/// Creates AddInTreeSyntaxMode objects that wrap a .xshd syntax mode stored as resource in the
	/// addin assembly.
	/// </summary>
	/// <attribute name="name" use="required">
	/// Name of the language for which the syntax mode is used.
	/// </attribute>
	/// <attribute name="extensions" use="required">
	/// Semicolon-separated list of file extensions for which the syntax mode is used.
	/// </attribute>
	/// <attribute name="resource" use="required">
	/// Fully qualified name of the resource file.
	/// </attribute>
	/// <usage>Only in /SharpDevelop/ViewContent/AvalonEdit/SyntaxModes</usage>
	/// <returns>
	/// An AddInTreeSyntaxMode object that loads the resource from the addin assembly when
	/// its CreateTextReader method is called.
	/// </returns>
	public class SyntaxModeDoozer : IDoozer
	{
		public const string Path = "/SharpDevelop/ViewContent/AvalonEdit/SyntaxModes";
		
		/// <summary>
		/// Gets if the doozer handles codon conditions on its own.
		/// If this property return false, the item is excluded when the condition is not met.
		/// </summary>
		public bool HandleConditions {
			get { return false; }
		}
		
		public object BuildItem(BuildItemArgs args)
		{
			Codon codon = args.Codon;
			string   highlightingName = codon.Properties["name"];
			string[] extensions       = codon.Properties["extensions"].Split(';');
			string   resource         = codon.Properties["resource"];
			
			return new AddInTreeSyntaxMode(codon.AddIn, resource, highlightingName, extensions);
		}
	}
}
