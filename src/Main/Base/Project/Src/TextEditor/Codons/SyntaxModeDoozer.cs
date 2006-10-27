// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.IO;
using System.Reflection;
using System.Xml;

using ICSharpCode.Core;
using ICSharpCode.TextEditor.Document;

namespace ICSharpCode.SharpDevelop.DefaultEditor.Codons
{
	public class AddInTreeSyntaxMode : SyntaxMode
	{
		Runtime[] runtimes;
		
		public AddInTreeSyntaxMode(Runtime[] runtimes, string fileName, string name, string[] extensions) : base(fileName, name, extensions)
		{
			this.runtimes = runtimes;
		}
		
		public XmlTextReader CreateTextReader()
		{
			foreach (Runtime runtime in runtimes) {
				Assembly assembly = runtime.LoadedAssembly;
				if (assembly != null) {
					Stream stream = assembly.GetManifestResourceStream(FileName);
					if (stream != null) {
						return new XmlTextReader(stream);
					}
				}
			}
			return null;
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
	/// <usage>Only in /SharpDevelop/ViewContent/DefaultTextEditor/SyntaxModes</usage>
	/// <returns>
	/// An AddInTreeSyntaxMode object that loads the resource from the addin assembly when
	/// its CreateTextReader method is called.
	/// </returns>
	public class SyntaxModeDoozer : IDoozer
	{
		/// <summary>
		/// Gets if the doozer handles codon conditions on its own.
		/// If this property return false, the item is excluded when the condition is not met.
		/// </summary>
		public bool HandleConditions {
			get {
				return false;
			}
		}
		
		public object BuildItem(object caller, Codon codon, ArrayList subItems)
		{
			string   highlightingName = codon.Properties["name"];
			string[] extensions       = codon.Properties["extensions"].Split(';');
			string   resource         = codon.Properties["resource"];
			
			Runtime[] assemblies = new Runtime[codon.AddIn.Runtimes.Count];
			int i = 0;
			foreach (Runtime library in codon.AddIn.Runtimes) {
				assemblies[i++] = library;
			}
			return new AddInTreeSyntaxMode(assemblies, resource, highlightingName, extensions);
		}
		
	}
}
