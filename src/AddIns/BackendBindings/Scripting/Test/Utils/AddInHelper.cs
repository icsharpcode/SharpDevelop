// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;

namespace ICSharpCode.Scripting.Tests.Utils
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
		public static Codon GetCodon(IEnumerable<Codon> codons, string name)
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
