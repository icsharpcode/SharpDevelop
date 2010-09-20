// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Windows.Forms;
using ICSharpCode.Core;
using ICSharpCode.Core.WinForms;

namespace ICSharpCode.WixBinding
{
	public class WixPackageFilesDiffControl : System.Windows.Forms.UserControl
	{
		public WixPackageFilesDiffControl()
		{
			InitializeComponent();
			InitImageList();
			InitStrings();
		}
		
		/// <summary>
		/// Shows the no difference found message.
		/// </summary>
		public void ShowNoDiffMessage()
		{
			Clear();
			ListViewItem item = new ListViewItem();
			item.SubItems.Add(StringParser.Parse("${res:ICSharpCode.WixBinding.WixPackageFilesDiffControl.NoDiffFound}"));
			diffResultListView.Items.Add(item);
		}
		
		/// <summary>
		/// Shows the list of diff results.
		/// </summary>
		public void ShowDiffResults(WixPackageFilesDiffResult[] diffResults)
		{
			Clear();
			foreach (WixPackageFilesDiffResult result in diffResults) {
				AddListItem(result);
			}
		}
		
		/// <summary>
		/// Clears all the displayed diff results.
		/// </summary>
		public void Clear()
		{
			diffResultListView.Items.Clear();
		}
		
		/// <summary>
		/// Disposes resources used by the control.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing) {
				if (components != null) {
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}
		
		#region Forms Designer generated code

		/// <summary>
		/// Designer variable used to keep track of non-visual components.
		/// </summary>
		System.ComponentModel.IContainer components = null;

		/// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor. The Forms designer might
		/// not be able to load this method if it was changed manually.
		/// </summary>
		void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.diffResultListView = new System.Windows.Forms.ListView();
			this.diffTypeColumnHeader = new System.Windows.Forms.ColumnHeader();
			this.fileNameColumnHeader = new System.Windows.Forms.ColumnHeader();
			this.diffResultsImageList = new System.Windows.Forms.ImageList(this.components);
			this.SuspendLayout();
			// 
			// diffResultListView
			// 
			this.diffResultListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
									this.diffTypeColumnHeader,
									this.fileNameColumnHeader});
			this.diffResultListView.Dock = System.Windows.Forms.DockStyle.Fill;
			this.diffResultListView.FullRowSelect = true;
			this.diffResultListView.Location = new System.Drawing.Point(0, 0);
			this.diffResultListView.Name = "diffResultListView";
			this.diffResultListView.Size = new System.Drawing.Size(474, 258);
			this.diffResultListView.TabIndex = 0;
			this.diffResultListView.UseCompatibleStateImageBehavior = false;
			this.diffResultListView.View = System.Windows.Forms.View.Details;
			// 
			// diffTypeColumnHeader
			// 
			this.diffTypeColumnHeader.Text = "";
			this.diffTypeColumnHeader.Width = 20;
			// 
			// fileNameColumnHeader
			// 
			this.fileNameColumnHeader.Text = "File";
			this.fileNameColumnHeader.Width = 400;
			// 
			// diffResultsImageList
			// 
			this.diffResultsImageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
			this.diffResultsImageList.ImageSize = new System.Drawing.Size(16, 16);
			this.diffResultsImageList.TransparentColor = System.Drawing.Color.Transparent;
			// 
			// WixPackageFilesDiffControl
			// 
			this.Controls.Add(this.diffResultListView);
			this.Name = "WixPackageFilesDiffControl";
			this.Size = new System.Drawing.Size(474, 258);
			this.ResumeLayout(false);
		}
		private System.Windows.Forms.ImageList diffResultsImageList;
		private System.Windows.Forms.ColumnHeader fileNameColumnHeader;
		private System.Windows.Forms.ColumnHeader diffTypeColumnHeader;
		private System.Windows.Forms.ListView diffResultListView;
				
		#endregion
		
		int GetDiffTypeImageIndex(WixPackageFilesDiffResultType diffType)
		{
			switch (diffType) {
				case WixPackageFilesDiffResultType.MissingFile:
					return 0;
				default:
					return 1;
			}
		}
		
		void AddListItem(WixPackageFilesDiffResult diffResult)
		{
			ListViewItem item = new ListViewItem();
			item.ImageIndex = GetDiffTypeImageIndex(diffResult.DiffType);
			item.SubItems.Add(diffResult.FileName);
			diffResultListView.Items.Add(item);
		}
		
		void InitStrings()
		{
			fileNameColumnHeader.Text = StringParser.Parse("${res:CompilerResultView.FileText}");
		}
		
		void InitImageList()
		{
			try {
				diffResultListView.SmallImageList = diffResultsImageList;
				diffResultsImageList.Images.Add(WinFormsResourceService.GetBitmap("Icons.16x16.Error"));
				diffResultsImageList.Images.Add(WinFormsResourceService.GetBitmap("Icons.16x16.Question"));
			} catch (ResourceNotFoundException) { }
		}
	}
}
