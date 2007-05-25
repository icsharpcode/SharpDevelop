// *****************************************************************************
// 
//  Copyright 2003, Weifen Luo
//  All rights reserved. The software and associated documentation 
//  supplied hereunder are the proprietary information of Weifen Luo
//  and are supplied subject to licence terms.
// 
//  DockSample Application 1.0
// *****************************************************************************

using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI;

namespace DockSample
{
	/// <summary>
	/// Summary description for DummySolutionExplorer.
	/// </summary>
	public class DummySolutionExplorer : DockContent
	{
		private System.Windows.Forms.TreeView treeView1;
		private System.Windows.Forms.ImageList imageList1;
		private System.ComponentModel.IContainer components;

		public DummySolutionExplorer()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
			treeView1.Nodes[1].Expand();
			treeView1.Nodes[2].Expand();
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(DummySolutionExplorer));
			this.treeView1 = new System.Windows.Forms.TreeView();
			this.imageList1 = new System.Windows.Forms.ImageList(this.components);
			this.SuspendLayout();
			// 
			// treeView1
			// 
			this.treeView1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.treeView1.ImageList = this.imageList1;
			this.treeView1.Indent = 19;
			this.treeView1.Location = new System.Drawing.Point(0, 24);
			this.treeView1.Name = "treeView1";
			this.treeView1.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
																				  new System.Windows.Forms.TreeNode("Solution \'WinFormsUI\' (2 projects)"),
																				  new System.Windows.Forms.TreeNode("DockSample", 3, 3, new System.Windows.Forms.TreeNode[] {
																																												new System.Windows.Forms.TreeNode("References", 4, 4, new System.Windows.Forms.TreeNode[] {
																																																																			  new System.Windows.Forms.TreeNode("System", 6, 6),
																																																																			  new System.Windows.Forms.TreeNode("System.Data", 6, 6),
																																																																			  new System.Windows.Forms.TreeNode("System.Drawing", 6, 6),
																																																																			  new System.Windows.Forms.TreeNode("System.Windows.Forms", 6, 6),
																																																																			  new System.Windows.Forms.TreeNode("System.XML", 6, 6),
																																																																			  new System.Windows.Forms.TreeNode("WeifenLuo.WinFormsUI", 6, 6)}),
																																												new System.Windows.Forms.TreeNode("Images", 2, 1, new System.Windows.Forms.TreeNode[] {
																																																																		  new System.Windows.Forms.TreeNode("BlankIcon.ico", 5, 5),
																																																																		  new System.Windows.Forms.TreeNode("CSProject.ico", 5, 5),
																																																																		  new System.Windows.Forms.TreeNode("OutputWindow.ico", 5, 5),
																																																																		  new System.Windows.Forms.TreeNode("References.ico", 5, 5),
																																																																		  new System.Windows.Forms.TreeNode("SolutionExplorer.ico", 5, 5),
																																																																		  new System.Windows.Forms.TreeNode("TaskListWindow.ico", 5, 5),
																																																																		  new System.Windows.Forms.TreeNode("ToolboxWindow.ico", 5, 5)}),
																																												new System.Windows.Forms.TreeNode("AboutDialog.cs", 8, 8),
																																												new System.Windows.Forms.TreeNode("App.ico", 5, 5),
																																												new System.Windows.Forms.TreeNode("AssemblyInfo.cs", 7, 7),
																																												new System.Windows.Forms.TreeNode("DummyOutputWindow.cs", 8, 8),
																																												new System.Windows.Forms.TreeNode("DummyPropertyWindow.cs", 8, 8),
																																												new System.Windows.Forms.TreeNode("DummySolutionExplorer.cs", 8, 8),
																																												new System.Windows.Forms.TreeNode("DummyTaskList.cs", 8, 8),
																																												new System.Windows.Forms.TreeNode("DummyToolbox.cs", 8, 8),
																																												new System.Windows.Forms.TreeNode("MianForm.cs", 8, 8),
																																												new System.Windows.Forms.TreeNode("Options.cs", 7, 7),
																																												new System.Windows.Forms.TreeNode("OptionsDialog.cs", 8, 8)}),
																				  new System.Windows.Forms.TreeNode("WeifenLuo.WinFormsUI", 3, 3, new System.Windows.Forms.TreeNode[] {
																																														  new System.Windows.Forms.TreeNode("References", 4, 4, new System.Windows.Forms.TreeNode[] {
																																																																						new System.Windows.Forms.TreeNode("System", 6, 6),
																																																																						new System.Windows.Forms.TreeNode("System.Data", 6, 6),
																																																																						new System.Windows.Forms.TreeNode("System.Design", 6, 6),
																																																																						new System.Windows.Forms.TreeNode("System.Drawing", 6, 6),
																																																																						new System.Windows.Forms.TreeNode("System.Windows.Forms", 6, 6),
																																																																						new System.Windows.Forms.TreeNode("System.XML", 6, 6)}),
																																														  new System.Windows.Forms.TreeNode("Resources", 2, 1, new System.Windows.Forms.TreeNode[] {
																																																																					   new System.Windows.Forms.TreeNode("DockWindow.AutoHideNo.bmp", 9, 9),
																																																																					   new System.Windows.Forms.TreeNode("DockWindow.AutoHideYes.bmp", 9, 9),
																																																																					   new System.Windows.Forms.TreeNode("DockWindow.Close.bmp", 9, 9),
																																																																					   new System.Windows.Forms.TreeNode("DocumentWindow.Close.bmp", 9, 9),
																																																																					   new System.Windows.Forms.TreeNode("DocumentWindow.ScrollLeftDisabled.bmp", 9, 9),
																																																																					   new System.Windows.Forms.TreeNode("DocumentWindow.ScrollLeftEnabled.bmp", 9, 9),
																																																																					   new System.Windows.Forms.TreeNode("DocumentWindow.ScrollRightDisabled.bmp", 9, 9),
																																																																					   new System.Windows.Forms.TreeNode("DocumentWindow.ScrollRightEnabled.bmp", 9, 9)}),
																																														  new System.Windows.Forms.TreeNode("Win32", 2, 1, new System.Windows.Forms.TreeNode[] {
																																																																				   new System.Windows.Forms.TreeNode("Enums.cs", 7, 7),
																																																																				   new System.Windows.Forms.TreeNode("Gdi32.cs", 7, 3),
																																																																				   new System.Windows.Forms.TreeNode("Structs.cs", 7, 7),
																																																																				   new System.Windows.Forms.TreeNode("User32.cs", 7, 7)}),
																																														  new System.Windows.Forms.TreeNode("AssemblyInfo.cs", 7, 7),
																																														  new System.Windows.Forms.TreeNode("Content.cs", 8, 8),
																																														  new System.Windows.Forms.TreeNode("CotentCollection.cs", 7, 7),
																																														  new System.Windows.Forms.TreeNode("CotentWindowCollection.cs", 7, 7),
																																														  new System.Windows.Forms.TreeNode("DockHelper.cs", 7, 7),
																																														  new System.Windows.Forms.TreeNode("DragHandler.cs", 7, 7),
																																														  new System.Windows.Forms.TreeNode("DragHandlerBase.cs", 7, 7),
																																														  new System.Windows.Forms.TreeNode("FloatWindow.cs", 8, 8),
																																														  new System.Windows.Forms.TreeNode("HiddenMdiChild.cs", 8, 8),
																																														  new System.Windows.Forms.TreeNode("InertButton.cs", 7, 7),
																																														  new System.Windows.Forms.TreeNode("Measures.cs", 7, 7),
																																														  new System.Windows.Forms.TreeNode("NormalTabStripWindow.cs", 8, 8),
																																														  new System.Windows.Forms.TreeNode("ResourceHelper.cs", 7, 7)})});
			this.treeView1.Size = new System.Drawing.Size(245, 297);
			this.treeView1.TabIndex = 0;
			// 
			// imageList1
			// 
			this.imageList1.ImageSize = new System.Drawing.Size(16, 16);
			this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
			this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
			// 
			// DummySolutionExplorer
			// 
			this.ClientSize = new System.Drawing.Size(245, 322);
			this.Controls.Add(this.treeView1);
			this.DockableAreas = ((WeifenLuo.WinFormsUI.DockAreas)((((WeifenLuo.WinFormsUI.DockAreas.DockLeft | WeifenLuo.WinFormsUI.DockAreas.DockRight) 
				| WeifenLuo.WinFormsUI.DockAreas.DockTop) 
				| WeifenLuo.WinFormsUI.DockAreas.DockBottom)));
			this.DockPadding.Bottom = 1;
			this.DockPadding.Top = 24;
			this.HideOnClose = true;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "DummySolutionExplorer";
			this.ShowHint = WeifenLuo.WinFormsUI.DockState.DockRight;
			this.TabText = "Solution Explorer";
			this.Text = "Solution Explorer - WinFormsUI";
			this.ResumeLayout(false);

		}
		#endregion
	}
}
