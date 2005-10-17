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
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.Core
{
	public class ParserDescriptor 
	{
		IParser parser = null;
		string[] supportedExtensions = null;
		Codon codon;
		
		public IParser Parser {
			get {
				if (parser == null) {
					parser = (IParser)codon.AddIn.CreateObject(codon.Properties["class"]);
				}
				return parser;
			}
		}
		
		public Codon Codon {
			get {
				return codon;
			}
		}
		
		public string Language {
			get {
				return codon.Id;
			}
		}
		
		public string ProjectFileExtension {
			get {
				return codon.Properties["projectfileextension"];
			}
		}
		
		public string[] Supportedextensions {
			get {
				if (supportedExtensions == null) {
					supportedExtensions = codon.Properties["supportedextensions"].ToUpperInvariant().Split(';');
				}
				return supportedExtensions;
			}
		}
		
		public bool CanParse(string fileName)
		{
			string fileExtension = Path.GetExtension(fileName).ToUpperInvariant();
			foreach (string ext in Supportedextensions) {
				if (fileExtension == ext) {
					return true;
				}
			}
			return false;
		}
		
		public ParserDescriptor (Codon codon)
		{
			this.codon = codon;
		}
	}
}
