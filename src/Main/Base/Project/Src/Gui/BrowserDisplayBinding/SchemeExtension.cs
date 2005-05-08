// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version value="$version"/>
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
		
		ISchemeExtension ext;
		
		public void InterceptNavigate(HtmlViewPane pane, WebBrowserNavigatingEventArgs e)
		{
			if (ext == null) {
				ext = (ISchemeExtension)codon.AddIn.CreateObject(codon.Properties["class"]);
			}
			ext.InterceptNavigate(pane, e);
		}
	}
	
	public class SchemeExtensionErbauer : IErbauer
	{
		public object BuildItem(object caller, Codon codon, ArrayList subItems)
		{
			return new SchemeExtensionDescriptor(codon);
		}
	}
}
