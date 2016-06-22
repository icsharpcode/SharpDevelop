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
using System.Windows.Forms;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Workbench;

namespace AddInScout
{
	public class AddInScoutViewContent : AbstractViewContent
	{
		Control control = null;
		
		public override object Control {
			get {
				return control;
			}
		}
		
		public override bool IsViewOnly {
			get {
				return true;
			}
		}
		
		public override void Dispose()
		{
			base.Dispose();
			control.Dispose();
		}
		
		AddInDetailsPanel addInDetailsPanel = new AddInDetailsPanel();
		CodonListPanel    codonListPanel    = new CodonListPanel();
		
		public AddInScoutViewContent() : base()
		{
			this.TitleName = "AddIn Scout";
			
			Panel p = new Panel();
			p.Dock = DockStyle.Fill;
			p.BorderStyle = BorderStyle.FixedSingle;
			
			Panel RightPanel = new Panel();
			RightPanel.Dock = DockStyle.Fill;
			p.Controls.Add(RightPanel);
			
			codonListPanel.Dock = DockStyle.Fill;
			codonListPanel.CurrentAddinChanged += new EventHandler(CodonListPanelCurrentAddinChanged);
			RightPanel.Controls.Add(codonListPanel);
			
			Splitter hs = new Splitter();
			hs.Dock = DockStyle.Top;
			RightPanel.Controls.Add(hs);
			
			addInDetailsPanel.Dock = DockStyle.Top;
			addInDetailsPanel.Height = 175;
			RightPanel.Controls.Add(addInDetailsPanel);
			
			Splitter s1 = new Splitter();
			s1.Dock = DockStyle.Left;
			p.Controls.Add(s1);
			
			AddinTreeView addinTreeView = new AddinTreeView();
			addinTreeView.Dock = DockStyle.Fill;
			addinTreeView.treeView.AfterSelect += new TreeViewEventHandler(this.tvSelectHandler);
			
			TreeTreeView treeTreeView = new TreeTreeView();
			treeTreeView.Dock = DockStyle.Fill;
			treeTreeView.treeView.AfterSelect += new TreeViewEventHandler(this.tvSelectHandler);
			
			TabControl tab = new TabControl();
			tab.Width = 300;
			tab.Dock = DockStyle.Left;
			
			TabPage tabPage2 = new TabPage("Tree");
			tabPage2.Dock = DockStyle.Left;
			tabPage2.Controls.Add(treeTreeView);
			tab.TabPages.Add(tabPage2);
			
			TabPage tabPage = new TabPage("AddIns");
			tabPage.Dock = DockStyle.Left;
			tabPage.Controls.Add(addinTreeView);
			tab.TabPages.Add(tabPage);
			
			p.Controls.Add(tab);
			
			this.control = p;
			this.TitleName = "AddIn Scout";
		}
		
		void CodonListPanelCurrentAddinChanged(object sender, EventArgs e)
		{
			addInDetailsPanel.ShowAddInDetails(codonListPanel.CurrentAddIn);
		}
		
		public void tvSelectHandler(object sender, TreeViewEventArgs e)
		{
			if (e.Node.Tag == null) {
				codonListPanel.ClearList();
				return;
			}
			
			TreeNode tn = e.Node;
			
			object o = e.Node.Tag;
			
			if (o is AddIn) {
				AddIn addIn = (AddIn)o;
				addInDetailsPanel.ShowAddInDetails(addIn);
				if (tn.FirstNode != null) {
					codonListPanel.ListCodons((ExtensionPath)tn.FirstNode.Tag);
				} else {
					codonListPanel.ClearList();
				}
			} else {
				ExtensionPath ext = (ExtensionPath)o;
				AddIn addIn = tn.Parent.Tag as AddIn;
				if (addIn == null) {
					codonListPanel.ListCodons(ext.Name);
				} else {
					addInDetailsPanel.ShowAddInDetails(addIn);
					codonListPanel.ListCodons(ext);
				}
			}
		}
	}
}
