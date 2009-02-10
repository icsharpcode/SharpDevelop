// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop
{
	public class LanguageBindingDescriptor
	{
		ILanguageBinding binding = null;
		Codon codon;
		
		public ILanguageBinding Binding {
			get {
				if (binding == null) {
					binding = (ILanguageBinding)codon.AddIn.CreateObject(codon.Properties["class"]);
					if (binding != null) {
						if (binding.Language != this.Language)
							throw new InvalidOperationException("The Language property of the language binding must be equal to the id of the LanguageBinding codon!");
					}
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
		
		string[] codeFileExtensions;
		
		public string[] CodeFileExtensions {
			get {
				if (codeFileExtensions == null) {
					if (codon.Properties["supportedextensions"].Length == 0)
						codeFileExtensions = new string[0];
					else
						codeFileExtensions = codon.Properties["supportedextensions"].ToLowerInvariant().Split(';');
				}
				return codeFileExtensions;
			}
		}
		
		public LanguageBindingDescriptor(Codon codon)
		{
			this.codon = codon;
		}
	}
}
