// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.Core;

namespace ICSharpCode.Reports.Addin.ReportWizard
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
