// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop
{
	public sealed class ParserDescriptor 
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
