// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Xml;
using System.IO;
using System.Collections;
using System.Reflection;
using System.CodeDom.Compiler;

using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.Core
{
	public class LanguageBindingDescriptor 
	{
		ILanguageBinding binding = null;
		Codon codon;
		
		public ILanguageBinding Binding {
			get {
				if (binding == null) {
					binding = (ILanguageBinding)codon.AddIn.CreateObject(codon.Properties["class"]);
				}
				return binding;
			}
		}
		public Codon Codon {
			get {
				return codon;
			}
		}
		
		public string ProjectFileExtension {
			get {
				return codon.Properties["projectfileextension"];
			}
		}
		
		public string Guid {
			get {
				return codon.Properties["guid"];
			}
		}
		
		public string Language {
			get {
				return codon.Id;
			}
		}
		
		public string[] Supportedextensions {
			get {
				return codon.Properties["supportedextensions"].Split(';');
			}
		}
		
		public LanguageBindingDescriptor(Codon codon)
		{
			this.codon = codon;
		}
	}
}
