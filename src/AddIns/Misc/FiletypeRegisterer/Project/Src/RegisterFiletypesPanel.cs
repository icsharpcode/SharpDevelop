// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.FiletypeRegisterer
{
	public partial class RegisterFiletypesPanel : AbstractOptionPanel
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
