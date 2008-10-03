// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.SharpDevelop
{
	/// <summary>
	/// Describes a pad.
	/// </summary>
	public class PadDescriptor : IDisposable
	{
		string @class;
		string title;
		string icon;
		string category;
		string shortcut;
		
		AddIn addIn;
		Type padType;
		
		IPadContent padContent;
		bool        padContentCreated;
		
		/// <summary>
		/// Creates a new pad descriptor from the AddIn tree.
		/// </summary>
		public PadDescriptor(Codon codon)
		{
			addIn = codon.AddIn;
			shortcut = codon.Properties["shortcut"];
			category = codon.Properties["category"];
			icon = codon.Properties["icon"];
			title = codon.Properties["title"];
			@class = codon.Properties["class"];
		}
		
		/// <summary>
		/// Creates a pad descriptor for the specified pad type.
		/// </summary>
		public PadDescriptor(Type padType, string title, string icon)
		{
			this.padType = padType;
			this.@class = padType.FullName;
			this.title = title;
			this.icon = icon;
			this.category = "none";
			this.shortcut = "";
		}
		
		/// <summary>
		/// Returns the title of the pad.
		/// </summary>
		public string Title {
			get {
				return title;
			}
		}
		
		/// <summary>
		/// Returns the icon bitmap resource name of the pad. May be an empty string
		/// if the pad has no icon defined.
		/// </summary>
		public string Icon {
			get {
				return icon;
			}
		}
		
		/// <summary>
		/// Returns the category (this is used for defining where the menu item to
		/// this pad goes)
		/// </summary>
		public string Category {
			get {
				return category;
			}
			set {
				if (value == null)
					throw new ArgumentNullException("value");
				category = value;
			}
		}
		
		/// <summary>
		/// Returns the menu shortcut for the view menu item.
		/// </summary>
		public string Shortcut {
			get {
				return shortcut;
			}
			set {
				if (value == null)
					throw new ArgumentNullException("value");
				shortcut = value;
			}
		}
		
		/// <summary>
		/// Gets the name of the pad class.
		/// </summary>
		public string Class {
			get {
				return @class;
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
				try {
					if (addIn != null) {
						LoggingService.Debug("Creating pad " + Class + "...");
						padContent = (IPadContent)addIn.CreateObject(Class);
					} else {
						padContent = (IPadContent)Activator.CreateInstance(padType);
					}
				} catch (Exception ex) {
					MessageService.ShowError(ex, "Error creating pad instance");
				}
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
	}
}
