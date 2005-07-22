// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Xml;
using System.IO;
using System.Collections;
using System.Reflection;
using System.CodeDom.Compiler;

using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.Core
{
	public class DisplayBindingDescriptor 
	{
		object binding = null;
		Codon codon;
		
		public IDisplayBinding Binding {
			get {
				if (binding == null) {
					binding = codon.AddIn.CreateObject(codon.Properties["class"]);
				}
				return binding as IDisplayBinding;
			}
		}
		
		public ISecondaryDisplayBinding SecondaryBinding {
			get {
				if (binding == null) {
					binding = codon.AddIn.CreateObject(codon.Properties["class"]);
				}
				return binding as ISecondaryDisplayBinding;
			}
		}
		
		public bool IsSecondary {
			get {
				return codon.Properties["type"] == "Secondary";
			}
		}
		
		public Codon Codon {
			get {
				return codon;
			}
		}

		public DisplayBindingDescriptor(Codon codon)
		{
			this.codon = codon;
		}
		
	}
}
