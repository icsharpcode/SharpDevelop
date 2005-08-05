// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.BrowserDisplayBinding
{
	public interface ISchemeExtension
	{
		void InterceptNavigate(HtmlViewPane pane, WebBrowserNavigatingEventArgs e);
		void GoHome(HtmlViewPane pane);
		void GoSearch(HtmlViewPane pane);
	}
	
	public class SchemeExtensionDescriptor
	{
		string schemeName;
		
		public string SchemeName {
			get {
				return schemeName;
			}
		}
		
		Codon codon;
		
		public SchemeExtensionDescriptor(Codon codon)
		{
			this.codon = codon;
			schemeName = codon.Properties["scheme"];
			if (schemeName == null || schemeName.Length == 0)
				schemeName = codon.Id;
		}
		
		ISchemeExtension extension;
		
		public ISchemeExtension Extension {
			get {
				if (extension == null) {
					extension = (ISchemeExtension)codon.AddIn.CreateObject(codon.Properties["class"]);
				}
				return extension;
			}
		}
	}
	
	public class SchemeExtensionDoozer : IDoozer
	{
		/// <summary>
		/// Gets if the doozer handles codon conditions on its own.
		/// If this property return false, the item is excluded when the condition is not met.
		/// </summary>
		public bool HandleConditions {
			get {
				return false;
			}
		}
		
		public object BuildItem(object caller, Codon codon, ArrayList subItems)
		{
			return new SchemeExtensionDescriptor(codon);
		}
	}
}
