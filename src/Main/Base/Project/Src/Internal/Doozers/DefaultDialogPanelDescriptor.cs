// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop
{
	public class DefaultDialogPanelDescriptor : IDialogPanelDescriptor
	{
		string       id    = String.Empty;
		string       label = String.Empty;
		List<IDialogPanelDescriptor> dialogPanelDescriptors = null;
		IDialogPanel dialogPanel = null;
		
		public string ID {
			get {
				return id;
			}
		}
		
		public string Label {
			get {
				return label;
			}
			set {
				label = value;
			}
		}
		
		public IEnumerable<IDialogPanelDescriptor> ChildDialogPanelDescriptors {
			get {
				return dialogPanelDescriptors;
			}
		}
		
		AddIn addin;
		string dialogPanelPath;
		
		public IDialogPanel DialogPanel {
			get {
				if (dialogPanelPath != null) {
					if (dialogPanel == null) {
						dialogPanel = (IDialogPanel)addin.CreateObject(dialogPanelPath);
					}
					dialogPanelPath = null;
					addin = null;
				}
				return dialogPanel;
			}
			set {
				dialogPanel = value;
			}
		}
		
		public DefaultDialogPanelDescriptor(string id, string label)
		{
			this.id    = id;
			this.label = label;
		}
		
		public DefaultDialogPanelDescriptor(string id, string label, List<IDialogPanelDescriptor> dialogPanelDescriptors) : this(id, label)
		{
			this.dialogPanelDescriptors = dialogPanelDescriptors;
		}
		
		public DefaultDialogPanelDescriptor(string id, string label, AddIn addin, string dialogPanelPath) : this(id, label)
		{
			this.addin = addin;
			this.dialogPanelPath = dialogPanelPath;
		}
	}
}
