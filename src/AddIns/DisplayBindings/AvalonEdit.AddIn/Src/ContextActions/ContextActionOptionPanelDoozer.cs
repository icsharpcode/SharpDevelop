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
using ICSharpCode.NRefactory;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Refactoring;

namespace ICSharpCode.AvalonEdit.AddIn.ContextActions
{
	/// <summary>
	/// Creates IOptionPanelDescriptor objects that are used in option dialogs.
	/// </summary>
	/// <attribute name="path" use="required">
	/// AddInTree-path that contains the IContextActionProviders.
	/// </attribute>
	/// <attribute name="label" use="optional">
	/// Caption of the dialog panel.
	/// </attribute>
	/// <children childTypes="OptionPanel">
	/// In the SharpDevelop options, option pages can have subpages by specifying them
	/// as children in the AddInTree.
	/// </children>
	/// <usage>In /SharpDevelop/BackendBindings/ProjectOptions/ and /SharpDevelop/Dialogs/OptionsDialog</usage>
	/// <returns>
	/// IOptionPanelDescriptor object.
	/// </returns>
	public class ContextActionOptionPanelDoozer : IDoozer
	{
		public bool HandleConditions {
			get { return false; }
		}
		
		public object BuildItem(BuildItemArgs args)
		{
			return new ContextActionOptionPanelDescriptor(args.Codon);
		}
		
		sealed class ContextActionOptionPanelDescriptor : IOptionPanelDescriptor
		{
			readonly string id;
			readonly string label;
			readonly string path;
			
			public ContextActionOptionPanelDescriptor(Codon codon)
			{
				this.id = codon.Id;
				this.path = codon.Properties["path"];
				this.label = codon.Properties["label"];
				if (string.IsNullOrEmpty(label))
					label = "Context Actions"; // TODO: Translate
			}
			
			public string ID {
				get { return this.id; }
			}
			
			public string Label {
				get { return this.label; }
			}
			
			public IEnumerable<IOptionPanelDescriptor> ChildOptionPanelDescriptors {
				get { return EmptyList<IOptionPanelDescriptor>.Instance; }
			}
			
			IOptionPanel optionPanel;
			
			public IOptionPanel OptionPanel {
				get { 
					if (optionPanel == null) {
						var providers = AddInTree.BuildItems<IContextActionProvider>(path, null, false);
						optionPanel = new ContextActionOptions(providers.Where(p => p.AllowHiding));
					}
					return optionPanel;
				}
			}
			
			public bool HasOptionPanel {
				get { return true; }
			}
		}
	}
}
