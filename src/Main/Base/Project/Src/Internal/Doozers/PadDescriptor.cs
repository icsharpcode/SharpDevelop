// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

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
		IPadContent padContent;
		bool        padContentCreated;
		
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
		public string Shortcut {
			get {
				return codon.Properties["shortcut"];
			}
		}
		
		public string Class {
			get {
				return codon.Properties["class"];
			}
		}
		
		public bool HasFocus {
			get {
				return (padContent != null) ? padContent.Control.ContainsFocus : false;
			}
		}
		
		public IPadContent PadContent {
			get {
				CreatePad();
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
			#if DEBUG
			if (WorkbenchSingleton.InvokeRequired)
				throw new InvalidOperationException("This action could trigger pad creation and is only valid on the main thread!");
			#endif
			if (!padContentCreated) {
				padContentCreated = true;
				padContent = (IPadContent)codon.AddIn.CreateObject(Class);
			}
		}
		
		public void BringPadToFront()
		{
			CreatePad();
			if (padContent == null) return;
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
