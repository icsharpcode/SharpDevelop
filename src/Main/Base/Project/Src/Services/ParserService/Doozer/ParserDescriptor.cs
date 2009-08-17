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
		Codon codon;
		
		public IParser CreateParser()
		{
			return (IParser)codon.AddIn.CreateObject(codon.Properties["class"]);
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
				return codon.Properties["supportedextensions"].Split(';');
			}
		}

		public bool CanParse(string fileName)
		{
			string fileExtension = Path.GetExtension(fileName);
			foreach (string ext in Supportedextensions) {
				if (string.Equals(fileExtension, ext, StringComparison.OrdinalIgnoreCase)) {
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
