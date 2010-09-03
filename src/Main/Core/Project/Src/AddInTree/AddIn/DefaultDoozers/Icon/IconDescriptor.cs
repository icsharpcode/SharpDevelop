// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.Core
{
	public class IconDescriptor 
	{
		Codon codon;
		
		public string Id {
			get {
				return codon.Id;
			}
		}
		
		public string Language {
			get {
				return codon.Properties["language"];
			}
		}
		
		public string Resource {
			get {
				return codon.Properties["resource"];
			}
		}
		
		public string[] Extensions {
			get {
				return codon.Properties["extensions"].Split(';');
			}
		}
		
		public IconDescriptor(Codon codon)
		{
			this.codon = codon;
		}
	}
}
