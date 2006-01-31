// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.Core;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace ICSharpCode.CodeCoverage
{
	public class CodeCoverageControl : UserControl
	{
		CodeCoverageTreeView treeView;
		ListView listView;
		SplitContainer splitContainer;
		ColumnHeader visitCountColumnHeader;
		ColumnHeader startLineColumnHeader;
		ColumnHeader endLineColumnHeader;
		ColumnHeader startColumnColumnHeader;
		ColumnHeader endColumnColumnHeader;
		ToolStrip toolStrip;
		
		public CodeCoverageControl()
		{
			// TreeView
			treeView = new CodeCoverageTreeView();
			treeView.Dock = DockStyle.Fill;
			treeView.ImageList = ClassBrowserIconService.ImageList;
			treeView.AfterSelect += CodeCoverageTreeViewAfterSelect;
			
			// ListView
			listView = new ListView();
			listView.View = View.Details;
			listView.Dock = DockStyle.Fill;
			listView.FullRowSelect = true;
			listView.HideSelection = false;
			listView.ItemActivate += ListViewItemActivate;
			
			visitCountColumnHeader = new ColumnHeader();
			visitCountColumnHeader.Text = "Visit Count";
			visitCountColumnHeader.Width = 80;
			
			startLineColumnHeader = new ColumnHeader();
			startLineColumnHeader.Text = "Line";
			startLineColumnHeader.Width = 80;
				
			startColumnColumnHeader = new ColumnHeader();
			startColumnColumnHeader.Text = "Column";
			startColumnColumnHeader.Width = 80;

			endLineColumnHeader = new ColumnHeader();
			endLineColumnHeader.Text = "End Line";
			endLineColumnHeader.Width = 80;

			endColumnColumnHeader = new ColumnHeader();
			endColumnColumnHeader.Text = "End Column";
			endColumnColumnHeader.Width = 80;

			listView.Columns.AddRange(new ColumnHeader[] {visitCountColumnHeader,
			                          startLineColumnHeader,
			                          startColumnColumnHeader,
			                          endLineColumnHeader,
			                          endColumnColumnHeader});
			
			// SplitContainer.
			splitContainer = new SplitContainer();
			splitContainer.SplitterWidth = 2;
			splitContainer.Dock = DockStyle.Fill;
			splitContainer.Panel1.Controls.Add(treeView);
			splitContainer.Panel2.Controls.Add(listView);
			
			Controls.Add(splitContainer);
			
			// Toolstrip
			toolStrip = ToolbarService.CreateToolStrip(this, "/SharpDevelop/Pads/CodeCoveragePad/Toolbar");
			toolStrip.GripStyle = ToolStripGripStyle.Hidden;
			Controls.Add(toolStrip);
		}
		
		public void UpdateToolbar()
		{
			ToolbarService.UpdateToolbar(toolStrip);
			toolStrip.Refresh();
		}
		
		public void AddModules(List<CodeCoverageModule> modules)
		{
			LoggingService.Debug("AddModules...");
			treeView.AddModules(modules);
		}
		
		public void Clear()
		{
			treeView.Clear();
			listView.Items.Clear();
		}
		
		void CodeCoverageTreeViewAfterSelect(object sender, TreeViewEventArgs e)
		{
			UpdateListView((CodeCoverageTreeNode)e.Node);
		}
		
		void UpdateListView(CodeCoverageTreeNode node)
		{
			listView.BeginUpdate();
			try {
				listView.Items.Clear();
				if (node is CodeCoverageClassTreeNode) {
					AddClass((CodeCoverageClassTreeNode)node);
				} else if (node is CodeCoverageMethodTreeNode) {
					AddMethod((CodeCoverageMethodTreeNode)node);
				}
			} finally {
				listView.EndUpdate();
			}
		}
		
		void AddClass(CodeCoverageClassTreeNode node)
		{
			foreach (CodeCoverageMethodTreeNode method in node.Nodes) {
				AddMethod(method);
			}
		}
		
		void AddMethod(CodeCoverageMethodTreeNode node)
		{
			foreach (CodeCoverageSequencePoint sequencePoint in node.Method.SequencePoints) {
				AddSequencePoint(sequencePoint);
			}
		}
		
		void AddSequencePoint(CodeCoverageSequencePoint sequencePoint)
		{
			ListViewItem item = new ListViewItem(sequencePoint.VisitCount.ToString());
			item.SubItems.Add(sequencePoint.Line.ToString());
			item.SubItems.Add(sequencePoint.Column.ToString());
			item.SubItems.Add(sequencePoint.EndLine.ToString());
			item.SubItems.Add(sequencePoint.EndColumn.ToString());
			item.Tag = sequencePoint;
			
			listView.Items.Add(item);
		}
		
		void ListViewItemActivate(object sender, EventArgs e)
		{
			if (listView.SelectedItems.Count > 0) {
				CodeCoverageSequencePoint sequencePoint = (CodeCoverageSequencePoint)listView.SelectedItems[0].Tag;
				if (sequencePoint.Document.Length > 0) {
					FileService.JumpToFilePosition(sequencePoint.Document, sequencePoint.Line - 1, sequencePoint.Column - 1);
				}
			}
		}
	}
}
