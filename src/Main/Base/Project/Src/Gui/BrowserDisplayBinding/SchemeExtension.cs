// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections;
using System.Windows.Forms;

using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.BrowserDisplayBinding
{
	public interface ISchemeExtension
	{
		void InterceptNavigate(HtmlViewPane pane, WebBrowserNavigatingEventArgs e);
		void DocumentCompleted(HtmlViewPane pane, WebBrowserDocumentCompletedEventArgs e);
		void GoHome(HtmlViewPane pane);
		void GoSearch(HtmlViewPane pane);
	}
	
	public class DefaultSchemeExtension : ISchemeExtension
	{
		public virtual void InterceptNavigate(HtmlViewPane pane, WebBrowserNavigatingEventArgs e) {}
		
		public virtual void DocumentCompleted(HtmlViewPane pane, WebBrowserDocumentCompletedEventArgs e) {}
		
		public virtual void GoHome(HtmlViewPane pane)
		{
			pane.Navigate(HtmlViewPane.DefaultHomepage);
		}
		
		public virtual void GoSearch(HtmlViewPane pane)
		{
			pane.Navigate(HtmlViewPane.DefaultSearchUrl);
		}
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
	
	/// <summary>
	/// Creates browser scheme extensions that can intercept calls on one protocol.
	/// </summary>
	/// <attribute name="scheme" use="required">
	/// Specifies the name of the protocol the extension handles. (e.g. 'ms-help' or 'startpage')
	/// </attribute>
	/// <attribute name="class" use="required">
	/// Name of the ISchemeExtension class (normally deriving from DefaultSchemeExtension).
	/// </attribute>
	/// <usage>Only in /SharpDevelop/Views/Browser/SchemeExtensions</usage>
	/// <returns>
	/// An SchemeExtensionDescriptor object that exposes the protocol name and ISchemeExtension object (lazy-loading).
	/// </returns>
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
		
		public object BuildItem(BuildItemArgs args)
		{
			return new SchemeExtensionDescriptor(args.Codon);
		}
	}
}
