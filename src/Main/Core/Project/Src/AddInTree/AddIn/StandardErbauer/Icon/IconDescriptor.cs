// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Collections;
using System.Reflection;
using System.Windows.Forms;

namespace ICSharpCode.Core
{
	public class IconDescriptor 
	{
		Codon codon;
		
		public string ID {
			get {
				return codon.ID;
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
