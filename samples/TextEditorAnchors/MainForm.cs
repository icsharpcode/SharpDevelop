// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;

namespace TextEditorAnchors
{
	/// <summary>
	/// This program demonstrates how TextAnchor objects can be used to refer to locations
	/// in the document and how they are updated when text is added/removed.
	/// </summary>
	public partial class MainForm : Form
	{
		public MainForm()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
		}
		
		
		void AddAnchorButtonClick(object sender, EventArgs e)
		{
			Caret caret = textEditorControl.ActiveTextAreaControl.Caret;
			LineSegment line = textEditorControl.Document.GetLineSegment(caret.Line);
			anchorListBox.Items.Add(new AnchorWrapper(line.CreateAnchor(caret.Column)));
		}
		
		void AnchorListBoxKeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Delete) {
				int index = anchorListBox.SelectedIndex;
				if (index >= 0) {
					anchorListBox.Items.RemoveAt(index);
					if (index < anchorListBox.Items.Count)
						anchorListBox.SelectedIndex = index;
					else
						anchorListBox.SelectedIndex = anchorListBox.Items.Count - 1;
				}
			}
		}
		
		void AnchorListBoxDoubleClick(object sender, EventArgs e)
		{
			if (anchorListBox.SelectedIndex >= 0) {
				AnchorWrapper w = (AnchorWrapper)anchorListBox.Items[anchorListBox.SelectedIndex];
				if (!w.Anchor.IsDeleted) {
					textEditorControl.ActiveTextAreaControl.Caret.Position = w.Anchor.Location;
					textEditorControl.Focus();
				}
			}
		}
		
		void TextEditorControlTextChanged(object sender, EventArgs e)
		{
			for (int i = 0; i < anchorListBox.Items.Count; i++) {
				anchorListBox.Items[i] = anchorListBox.Items[i];
			}
		}
		
		/// <summary>
		/// Provide a ToString() implementation for TextAnchor
		/// (the implementation in TextAnchor uses 0-based coordinates)
		/// </summary>
		sealed class AnchorWrapper
		{
			public readonly TextAnchor Anchor;
			
			public AnchorWrapper(TextAnchor anchor)
			{
				this.Anchor = anchor;
			}
			
			public override string ToString()
			{
				if (Anchor.IsDeleted)
					return "Anchor deleted";
				else
					return "Line " + (Anchor.LineNumber+1) + ", Column " + (Anchor.ColumnNumber+1)
						+ " (Offset " + Anchor.Offset + ")";
			}
		}
	}
}
