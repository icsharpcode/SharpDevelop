/*
 * Created by SharpDevelop.
 * User: Omnibrain
 * Date: 24.12.2004
 * Time: 10:42
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.Core
{
	/// <summary>
	/// Description of PadDescriptor.
	/// </summary>
	public class PadDescriptor : IDisposable
	{
		Codon       codon;
		IPadContent padContent = null;
		
		/// <summary>
		/// Returns the title of the pad.
		/// </summary>
		public string Title {
			get {
				return codon.Properties["title"];
			}
		}
		
		/// <summary>
		/// Returns the icon bitmap resource name of the pad. May be null, if the pad has no
		/// icon defined.
		/// </summary>
		public string Icon {
			get {
				return codon.Properties["icon"];
			}
		}
		
		/// <summary>
		/// Returns the category (this is used for defining where the menu item to
		/// this pad goes)
		/// </summary>
		public string Category {
			get {
				return codon.Properties["category"];
			}
		}
		
		/// <summary>
		/// Returns the menu shortcut for the view menu item.
		/// </summary>
		public string[] Shortcut {
			get {
				return codon.Properties["category"].Split('|');
			}
		}
		
		public string Class {
			get {
				return codon.Properties["class"];
			}
		}
		
		public bool HasFocus {
			get {
				if (padContent == null) {
					return false;
				}
				return padContent.Control.ContainsFocus;
			}
		}
			
		
		public IPadContent PadContent {
			get {
				if (padContent == null) {
					padContent = (IPadContent)codon.AddIn.CreateObject(Class);
				}
				return padContent;
			}
		}
		
		public void Dispose()
		{
			if (padContent != null) {
				padContent.Dispose();
				padContent = null;
			}
		}
		
		public void RedrawContent()
		{
			if (padContent != null) {
				padContent.RedrawContent();
			}
		}
		
		public void CreatePad()
		{
			if (padContent == null) {
				padContent = (IPadContent)codon.AddIn.CreateObject(Class);
			}
		}
		
		public void BringPadToFront()
		{
			CreatePad();
			if (!WorkbenchSingleton.Workbench.WorkbenchLayout.IsVisible(this)) {
				WorkbenchSingleton.Workbench.WorkbenchLayout.ShowPad(this);
			}
			WorkbenchSingleton.Workbench.WorkbenchLayout.ActivatePad(this);
		}
		
		public PadDescriptor(Codon codon)
		{
			this.codon = codon;
		}
	}
}
