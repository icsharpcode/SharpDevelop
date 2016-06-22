// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
