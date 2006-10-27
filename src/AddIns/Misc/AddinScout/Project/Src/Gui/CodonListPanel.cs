// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="none" email=""/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Drawing;
using System.Windows.Forms;

using ICSharpCode.Core;

namespace AddInScout
{
	public class CodonListPanel : Panel
	{
		ListView CodonLV   = new ListView();    // show codin details
		Label ExtLabel     = new Label();	    // show extension name
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
			
			
			ExtLabel.Text = "Extension : ";
			ExtLabel.Dock = DockStyle.Top;
			ExtLabel.FlatStyle = FlatStyle.Flat;
			ExtLabel.TextAlign = ContentAlignment.MiddleLeft;
			ExtLabel.BorderStyle = BorderStyle.FixedSingle;
			
			Controls.Add(CodonLV);
			Controls.Add(ExtLabel);
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
			ExtLabel.Text = "Extension : ";
			CodonLV.Items.Clear();
		}
		
		public void ListCodons(string path)
		{
			CodonLV.Items.Clear();
			if (path == null) {
				ExtLabel.Text = "Extension : ";
				return;
			}
			
			ExtLabel.Text = "Extension : " + path;
			
			AddInTreeNode node = AddInTree.GetTreeNode(path, false);
			if (node == null) return;
			foreach (Codon c in node.Codons) {
				ListViewItem lvi = new ListViewItem(c.Name);
				lvi.Tag = c;
				lvi.SubItems.Add(c.Id);
				
				lvi.SubItems.Add(c.Properties.Contains("class") ? c.Properties["class"] : "");
				
				foreach (ICondition condition in c.Conditions) {
					lvi.SubItems.Add(condition.Name + ", " + condition.Action);
				}
				CodonLV.Items.Add(lvi);
			}
		}
		
		public void ListCodons(ExtensionPath ext)
		{
			CodonLV.Items.Clear();
			if (ext == null) {
				ExtLabel.Text = "Extension : ";
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
