// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop
{
	public class ProjectBindingDescriptor
	{
		IProjectBinding binding = null;
		Codon codon;
		
		public IProjectBinding Binding {
			get {
				if (binding == null) {
					binding = (IProjectBinding)codon.AddIn.CreateObject(codon.Properties["class"]);
					if (binding != null) {
						if (binding.Language != this.Language)
							throw new InvalidOperationException("The Language property of the project binding must be equal to the id of the ProjectBinding codon!");
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
		
		public ProjectBindingDescriptor(Codon codon)
		{
			this.codon = codon;
		}
	}
}
