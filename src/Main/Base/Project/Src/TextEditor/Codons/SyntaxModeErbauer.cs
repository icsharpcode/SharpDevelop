// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;
using System.Collections;
using System.Reflection;
using System.Windows.Forms;
using System.Xml;

using ICSharpCode.Core;

using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.DefaultEditor.Actions;
using ICSharpCode.TextEditor.Actions;
using ICSharpCode.TextEditor.Document;

namespace ICSharpCode.SharpDevelop.DefaultEditor.Codons
{
	public class AddInTreeSyntaxMode : SyntaxMode
	{
		Assembly[] assemblies;
		
		public AddInTreeSyntaxMode(Assembly[] assemblies, string fileName, string name, string[] extensions) : base(fileName, name, extensions)
		{
			this.assemblies = assemblies;
		}
		
		public XmlTextReader CreateTextReader()
		{
			foreach (Assembly assembly in assemblies) {
				Stream stream = assembly.GetManifestResourceStream(FileName);
				if (stream != null) {
					return new XmlTextReader(stream);
				}
			}
			return null;
		}
	}
	
//	[CodonNameAttribute("SyntaxMode")]
	public class SyntaxModeErbauer : IErbauer
	{
		public object BuildItem(object caller, Codon codon, ArrayList subItems)
		{
			string   highlightingName = codon.Properties["name"];
			string[] extensions       = codon.Properties["extensions"].Split(';');
			string   resource         = codon.Properties["resource"];
			
			Assembly[] assemblies = new Assembly[codon.AddIn.Runtimes.Count];
			int i = 0;
			foreach (Runtime library in codon.AddIn.Runtimes) {
				assemblies[i++] = library.LoadedAssembly;
			}
			return new AddInTreeSyntaxMode(assemblies, resource, highlightingName, extensions);
		}
		
	}
}
