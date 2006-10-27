// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

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
