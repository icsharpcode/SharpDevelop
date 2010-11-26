// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Editor;

namespace ICSharpCode.SharpDevelop
{
	public class LanguageBindingDescriptor
	{
		Codon codon;
		string[] extensions;
		
		public LanguageBindingDescriptor(Codon codon)
		{
			this.codon = codon;
		}
		
		internal ILanguageBinding CreateBinding(ITextEditor editor) {
			return (ILanguageBinding)codon.AddIn.CreateObject(codon.Properties["class"]);
		}
		
		internal bool CanAttach(ITextEditor editor)
		{
			if (Extensions == null || Extensions.Length == 0)
				return true;
			
			if (!string.IsNullOrEmpty(editor.FileName)) {
				string extension = Path.GetExtension(editor.FileName).ToLowerInvariant();
				foreach (var ext in Extensions) {
					if (extension == ext)
						return true;
				}
			}
			
			return false;
		}
		
		public string Language {
			get {
				return codon.Id;
			}
		}
		
		public Codon Codon {
			get {
				return codon;
			}
		}
		
		public string[] Extensions {
			get {
				if (extensions == null) {
					if (codon.Properties["extensions"].Length == 0)
						extensions = new string[0];
					else
						extensions = codon.Properties["extensions"].ToLowerInvariant().Split(';');
				}
				return extensions;
			}
		}
	}
}
