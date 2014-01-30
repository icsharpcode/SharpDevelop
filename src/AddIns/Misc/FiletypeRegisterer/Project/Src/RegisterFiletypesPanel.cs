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

using ICSharpCode.SharpDevelop.Gui.OptionPanels;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.FiletypeRegisterer
{
	#pragma warning disable 618
	public partial class RegisterFiletypesPanel : XmlFormsOptionPanel
	{
		sealed class ListEntry
		{
			internal readonly FiletypeAssociation Association;
			internal readonly bool InitiallyChecked;
			
			public ListEntry(FiletypeAssociation association)
			{
				this.Association = association;
				this.InitiallyChecked = RegisterFiletypesCommand.IsRegisteredToSharpDevelop(association.Extension);
			}
			
			public override string ToString()
			{
				return Association.Text + " (." + Association.Extension + ")";
			}
		}
		
		public RegisterFiletypesPanel()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			captionLabel.Text = StringParser.Parse(captionLabel.Text);
		}
		
		public override void LoadPanelContents()
		{
			foreach (FiletypeAssociation assoc in FiletypeAssociationDoozer.GetList()) {
				ListEntry entry = new ListEntry(assoc);
				fileTypesListBox.Items.Add(entry, entry.InitiallyChecked);
			}
		}
		
		public override bool StorePanelContents()
		{
			for (int i = 0; i < fileTypesListBox.Items.Count; i++) {
				bool newChecked = fileTypesListBox.GetItemChecked(i);
				ListEntry entry = (ListEntry)fileTypesListBox.Items[i];
				if (entry.InitiallyChecked != newChecked) {
					if (newChecked) {
						RegisterFiletypesCommand.RegisterToSharpDevelop(entry.Association);
					} else {
						RegisterFiletypesCommand.UnRegisterFiletype(entry.Association.Extension);
					}
				}
			}
			return true;
		}
	}
}
