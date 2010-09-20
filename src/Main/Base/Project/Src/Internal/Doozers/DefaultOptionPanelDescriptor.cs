// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop
{
	public class DefaultOptionPanelDescriptor : IOptionPanelDescriptor
	{
		string       id    = String.Empty;
		List<IOptionPanelDescriptor> optionPanelDescriptors = null;
		IOptionPanel optionPanel = null;
		
		public string ID {
			get {
				return id;
			}
		}
		
		public string Label { get; set; }
		
		public IEnumerable<IOptionPanelDescriptor> ChildOptionPanelDescriptors {
			get {
				return optionPanelDescriptors;
			}
		}
		
		AddIn addin;
		object owner;
		string optionPanelPath;
		
		public IOptionPanel OptionPanel {
			get {
				if (optionPanelPath != null) {
					if (optionPanel == null) {
						optionPanel = (IOptionPanel)addin.CreateObject(optionPanelPath);
						if (optionPanel != null) {
							optionPanel.Owner = owner;
						}
					}
					optionPanelPath = null;
					addin = null;
				}
				return optionPanel;
			}
		}
		
		public bool HasOptionPanel {
			get {
				return optionPanelPath != null;
			}
		}
		
		public DefaultOptionPanelDescriptor(string id, string label)
		{
			this.id    = id;
			this.Label = label;
		}
		
		public DefaultOptionPanelDescriptor(string id, string label, List<IOptionPanelDescriptor> dialogPanelDescriptors) : this(id, label)
		{
			this.optionPanelDescriptors = dialogPanelDescriptors;
		}
		
		public DefaultOptionPanelDescriptor(string id, string label, AddIn addin, object owner, string optionPanelPath) : this(id, label)
		{
			this.addin = addin;
			this.owner = owner;
			this.optionPanelPath = optionPanelPath;
		}
	}
}
