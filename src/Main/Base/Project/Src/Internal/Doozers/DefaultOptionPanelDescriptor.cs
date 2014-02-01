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
using System.Collections.Generic;
using System.Linq;
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
				return optionPanelDescriptors ?? Enumerable.Empty<IOptionPanelDescriptor>();
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
