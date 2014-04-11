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
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

using ICSharpCode.Core;

namespace AddInScout
{
	public class CodonListPanel : Panel
	{
		ListView CodonLV   = new ListView();    // show codin details
		TextBox ExtTextBox     = new TextBox();	    // show extension name
		AddIn currentAddIn = null;
		
		public AddIn CurrentAddIn {
			get {
				return currentAddIn;
			}
			set {
				currentAddIn = value;
				this.OnCurrentAddinChanged(EventArgs.Empty);
			}
		}
		
		public CodonListPanel()
		{
			CodonLV.Dock = DockStyle.Fill;
			CodonLV.GridLines = true;
			CodonLV.View = View.Details;
			CodonLV.FullRowSelect = true;
			CodonLV.MultiSelect = false;
			CodonLV.BorderStyle = BorderStyle.FixedSingle;
			CodonLV.SelectedIndexChanged += new EventHandler(CodonLVSelectedIndexChanged);
			CodonLV.Columns.Add("Codon", 100,HorizontalAlignment.Left);
			CodonLV.Columns.Add("Codon ID", 175,HorizontalAlignment.Left);
			CodonLV.Columns.Add("Codon Class", 400,HorizontalAlignment.Left);
			CodonLV.Columns.Add("Codon Condition -> Action on Fail", 600,HorizontalAlignment.Left);
			CodonLV.Columns.Add("Label", 450, HorizontalAlignment.Left);
			
			ExtTextBox.Text = "Extension : ";
			ExtTextBox.ReadOnly = true;
			ExtTextBox.BackColor = SystemColors.Control;
			ExtTextBox.Dock = DockStyle.Top;
			//ExtLabel.FlatStyle = FlatStyle.Flat;
			//ExtLabel.TextAlign = ContentAlignment.MiddleLeft;
			ExtTextBox.BorderStyle = BorderStyle.FixedSingle;
			
			Controls.Add(CodonLV);
			Controls.Add(ExtTextBox);
		}
		
		
		void CodonLVSelectedIndexChanged(object sender, EventArgs e)
		{
			if (CodonLV.SelectedItems.Count != 1) {
				return;
			}
			Codon c = CodonLV.SelectedItems[0].Tag as Codon;
			if (c == null) {
				return;
			}
			
			CurrentAddIn = c.AddIn;
		}
		
		public void ClearList()
		{
			ExtTextBox.Text = "Extension : ";
			CodonLV.Items.Clear();
		}
		
		public void ListCodons(string path)
		{
			CodonLV.Items.Clear();
			if (path == null) {
				ExtTextBox.Text = "Extension : ";
				return;
			}
			
			ExtTextBox.Text = "Extension : " + path;
			
			AddInTreeNode node = AddInTree.GetTreeNode(path, false);
			if (node == null) return;
			foreach (Codon c in node.Codons) {
				ListViewItem lvi = new ListViewItem(c.Name);
				lvi.Tag = c;
				lvi.SubItems.Add(c.Id);
				
				lvi.SubItems.Add(c.Properties.Contains("class") ? c.Properties["class"] : "");
				
				lvi.SubItems.Add(string.Join(";", c.Conditions.Select(a => a.Name + ": " + a.Action)));
				lvi.SubItems.Add(c.Properties.Contains("label") ? c.Properties["label"] : "");
				CodonLV.Items.Add(lvi);
			}
		}
		
		public void ListCodons(ExtensionPath ext)
		{
			CodonLV.Items.Clear();
			if (ext == null) {
				ExtTextBox.Text = "Extension : ";
				return;
			}
			ListCodons(ext.Name);
		}
		
		protected virtual void OnCurrentAddinChanged(EventArgs e)
		{
			if (CurrentAddinChanged != null) {
				CurrentAddinChanged(this, e);
			}
		}
		
		public event EventHandler CurrentAddinChanged;
	}
}
