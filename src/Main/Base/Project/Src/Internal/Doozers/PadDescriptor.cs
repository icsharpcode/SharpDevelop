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
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Workbench;

namespace ICSharpCode.SharpDevelop
{
	/// <summary>
	/// Indicates the default position for a pad.
	/// This is a bit-flag enum, Hidden can be combined with the directions.
	/// </summary>
	[Flags]
	public enum DefaultPadPositions
	{
		None = 0,
		Right = 1,
		Left = 2,
		Bottom = 4,
		Top = 8,
		Hidden = 16
	}
	
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
		
		string serviceInterfaceName;
		Type serviceInterface;
		
		IPadContent padContent;
		bool        padContentCreated;
		
		/// <summary>
		/// Creates a new pad descriptor from the AddIn tree.
		/// </summary>
		public PadDescriptor(Codon codon)
		{
			if (codon == null)
				throw new ArgumentNullException("codon");
			addIn = codon.AddIn;
			shortcut = codon.Properties["shortcut"];
			category = codon.Properties["category"];
			icon = codon.Properties["icon"];
			title = codon.Properties["title"];
			@class = codon.Properties["class"];
			serviceInterfaceName = codon.Properties["serviceInterface"];
			if (!string.IsNullOrEmpty(codon.Properties["defaultPosition"])) {
				DefaultPosition = (DefaultPadPositions)Enum.Parse(typeof(DefaultPadPositions), codon.Properties["defaultPosition"]);
			}
		}
		
		/// <summary>
		/// Creates a pad descriptor for the specified pad type.
		/// </summary>
		public PadDescriptor(Type padType, string title, string icon)
		{
			if (padType == null)
				throw new ArgumentNullException("padType");
			if (title == null)
				throw new ArgumentNullException("title");
			if (icon == null)
				throw new ArgumentNullException("icon");
			this.padType = padType;
			this.@class = padType.FullName;
			this.title = title;
			this.icon = icon;
			this.category = "none";
			this.shortcut = "";
			this.serviceInterface = null;
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
		
		/// <summary>
		/// Gets the type of the service interface.
		/// </summary>
		public Type ServiceInterface {
			get {
				if (serviceInterface == null && addIn != null && !string.IsNullOrEmpty(serviceInterfaceName)) {
					serviceInterface = addIn.FindType(serviceInterfaceName);
				}
				return serviceInterface;
			}
		}
		
		/// <summary>
		/// Gets/sets the default position of the pad.
		/// </summary>
		public DefaultPadPositions DefaultPosition { get; set; }
		
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
		
		public void CreatePad()
		{
			if (SD.MainThread.InvokeRequired) {
				throw new InvalidOperationException("This action could trigger pad creation and is only valid on the main thread!");
			}
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
					MessageService.ShowException(ex, "Error creating pad instance");
				}
			}
		}
		
		public void BringPadToFront()
		{
			CreatePad();
			if (padContent == null) return;
			SD.Workbench.ActivatePad(this);
		}
		
		public override string ToString()
		{
			return "[PadDescriptor " + this.Class + "]";
		}
	}
}
