// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.DefaultEditor.Codons;

namespace RubyBinding.Tests.Utils
{
	public class AddInHelper
	{
		AddInHelper()
		{
		}
		
		/// <summary>
		/// Gets the codon with the specified extension path and name.
		/// </summary>
		public static Codon GetCodon(AddIn addin, string extensionPath, string name)
		{
			if (addin.Paths.ContainsKey(extensionPath)) {
				ExtensionPath path = addin.Paths[extensionPath];
				return GetCodon(path.Codons, name);
			}
			return null;
		}
		
		/// <summary>
		/// Gets the codon with the specified name.
		/// </summary>
		public static Codon GetCodon(List<Codon> codons, string name)
		{
			foreach (Codon codon in codons) {
				if (codon.Id == name) {
					return codon;
				}
			}
			return null;
		}		
	}
}
