// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Windows.Forms;

using HtmlHelp2.Environment;
using HtmlHelp2.JScriptGlobals;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.BrowserDisplayBinding;

namespace HtmlHelp2
{
	public class BrowserScheme : DefaultSchemeExtension
	{
		JScriptExternal scriptObject;
		
		public override void GoHome(HtmlViewPane pane)
		{
			if (pane == null)
			{
				throw new ArgumentNullException("pane");
			}
			pane.Navigate(new Uri(HtmlHelp2Environment.DefaultPage));
		}
		
		public override void GoSearch(HtmlViewPane pane)
		{
			if (pane == null)
			{
				throw new ArgumentNullException("pane");
			}
			pane.Navigate(new Uri(HtmlHelp2Environment.SearchPage));
		}
		
//		[PermissionSet(SecurityAction.LinkDemand, Name="Execution")]
		public override void InterceptNavigate(HtmlViewPane pane, WebBrowserNavigatingEventArgs e)
		{
			if (pane == null)
			{
				throw new ArgumentNullException("pane");
			}
			if (scriptObject == null) {
				scriptObject = new JScriptExternal();
				LoadHelpState();
			}
			pane.WebBrowser.ObjectForScripting = scriptObject;
			// add event (max. 1 one time)
			pane.WebBrowser.Disposed -= SaveHelpState;
			pane.WebBrowser.Disposed += SaveHelpState;
			base.InterceptNavigate(pane, e);
		}
		
		void LoadHelpState()
		{
			foreach (string line in PropertyService.Get("HtmlHelpPersistedJScriptGlobals", new string[0])) {
				int pos = line.IndexOf('=');
				string name = line.Substring(0, pos);
				scriptObject.Globals.VariablePersistCollection[name] = true;
				scriptObject.Globals.VariableValueCollection[name] = line.Substring(pos + 1);
			}
		}
		
		void SaveHelpState(object sender, EventArgs e)
		{
			((System.ComponentModel.IComponent)sender).Disposed -= SaveHelpState;
			List<string> lines = new List<string>();
			foreach (KeyValuePair<string, bool> pair in scriptObject.Globals.VariablePersistCollection) {
				if (pair.Value) {
					lines.Add(pair.Key + "=" + scriptObject.Globals.VariableValueCollection[pair.Key]);
				}
			}
			PropertyService.Set("HtmlHelpPersistedJScriptGlobals", lines.ToArray());
		}

//		[PermissionSet(SecurityAction.LinkDemand, Name="Execution")]
		public override void DocumentCompleted(HtmlViewPane pane, WebBrowserDocumentCompletedEventArgs e)
		{
			if (pane == null)
			{
				throw new ArgumentNullException("pane");
			}
			ShowHelpBrowser.HighlightDocument(pane);
		}
	}
}
