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

namespace ICSharpCode.SharpDevelop.Gui
{
	partial class AbstractAttachToProcessForm
	{
		/// <summary>
		/// Designer variable used to keep track of non-visual components.
		/// </summary>
		private System.ComponentModel.IContainer components = null;
		
		/// <summary>
		/// Disposes resources used by the form.
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
		
		/// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor. The Forms designer might
		/// not be able to load this method if it was changed manually.
		/// </summary>
		private void InitializeComponent()
		{
			this.attachButton = new System.Windows.Forms.Button();
			this.cancelButton = new System.Windows.Forms.Button();
			this.refreshButton = new System.Windows.Forms.Button();
			this.listView = new System.Windows.Forms.ListView();
			this.processColumnHeader = new System.Windows.Forms.ColumnHeader();
			this.processIdColumnHeader = new System.Windows.Forms.ColumnHeader();
			this.titleColumnHeader = new System.Windows.Forms.ColumnHeader();
			this.typeColumnHeader = new System.Windows.Forms.ColumnHeader();
			this.showNonManagedCheckBox = new System.Windows.Forms.CheckBox();
			this.SuspendLayout();
			// 
			// attachButton
			// 
			this.attachButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.attachButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.attachButton.Location = new System.Drawing.Point(389, 339);
			this.attachButton.Name = "attachButton";
			this.attachButton.Size = new System.Drawing.Size(75, 23);
			this.attachButton.TabIndex = 1;
			this.attachButton.Text = "Attach";
			this.attachButton.UseVisualStyleBackColor = true;
			// 
			// cancelButton
			// 
			this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancelButton.Location = new System.Drawing.Point(550, 339);
			this.cancelButton.Name = "cancelButton";
			this.cancelButton.Size = new System.Drawing.Size(75, 23);
			this.cancelButton.TabIndex = 3;
			this.cancelButton.Text = "Cancel";
			this.cancelButton.UseVisualStyleBackColor = true;
			// 
			// refreshButton
			// 
			this.refreshButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.refreshButton.Location = new System.Drawing.Point(469, 339);
			this.refreshButton.Name = "refreshButton";
			this.refreshButton.Size = new System.Drawing.Size(75, 23);
			this.refreshButton.TabIndex = 2;
			this.refreshButton.Text = "Refresh";
			this.refreshButton.UseVisualStyleBackColor = true;
			this.refreshButton.Click += new System.EventHandler(this.RefreshButtonClick);
			// 
			// listView
			// 
			this.listView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
									| System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.listView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
									this.processColumnHeader,
									this.processIdColumnHeader,
									this.titleColumnHeader,
									this.typeColumnHeader});
			this.listView.FullRowSelect = true;
			this.listView.HideSelection = false;
			this.listView.Location = new System.Drawing.Point(0, 0);
			this.listView.MultiSelect = false;
			this.listView.Name = "listView";
			this.listView.Size = new System.Drawing.Size(636, 334);
			this.listView.Sorting = System.Windows.Forms.SortOrder.Ascending;
			this.listView.TabIndex = 4;
			this.listView.UseCompatibleStateImageBehavior = false;
			this.listView.View = System.Windows.Forms.View.Details;
			this.listView.ItemActivate += new System.EventHandler(this.ListViewItemActivate);
			this.listView.SelectedIndexChanged += new System.EventHandler(this.ListViewSelectedIndexChanged);
			// 
			// processColumnHeader
			// 
			this.processColumnHeader.Text = "Process";
			this.processColumnHeader.Width = 189;
			// 
			// processIdColumnHeader
			// 
			this.processIdColumnHeader.Text = "ID";
			// 
			// titleColumnHeader
			// 
			this.titleColumnHeader.Text = "Title";
			this.titleColumnHeader.Width = 294;
			// 
			// typeColumnHeader
			// 
			this.typeColumnHeader.Text = "Type";
			this.typeColumnHeader.Width = 74;
			// 
			// showNonManagedCheckBox
			// 
			this.showNonManagedCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.showNonManagedCheckBox.Location = new System.Drawing.Point(13, 339);
			this.showNonManagedCheckBox.Name = "showNonManagedCheckBox";
			this.showNonManagedCheckBox.Size = new System.Drawing.Size(370, 24);
			this.showNonManagedCheckBox.TabIndex = 5;
			this.showNonManagedCheckBox.Text = "Show Non-Managed";
			this.showNonManagedCheckBox.UseVisualStyleBackColor = true;
			this.showNonManagedCheckBox.CheckedChanged += new System.EventHandler(this.ShowNonManagedCheckBoxCheckedChanged);
			// 
			// AttachToProcessForm
			// 
			this.AcceptButton = this.attachButton;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.cancelButton;
			this.ClientSize = new System.Drawing.Size(637, 374);
			this.Controls.Add(this.showNonManagedCheckBox);
			this.Controls.Add(this.listView);
			this.Controls.Add(this.refreshButton);
			this.Controls.Add(this.cancelButton);
			this.Controls.Add(this.attachButton);
			this.MinimumSize = new System.Drawing.Size(273, 240);
			this.Name = "bstractAAttachToProcessForm";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.Text = "Attach to Process";
			this.ResumeLayout(false);
		}
		private System.Windows.Forms.CheckBox showNonManagedCheckBox;
		private System.Windows.Forms.ColumnHeader typeColumnHeader;
		private System.Windows.Forms.ColumnHeader titleColumnHeader;
		private System.Windows.Forms.ColumnHeader processIdColumnHeader;
		private System.Windows.Forms.ColumnHeader processColumnHeader;
		private System.Windows.Forms.ListView listView;
		private System.Windows.Forms.Button refreshButton;
		private System.Windows.Forms.Button cancelButton;
		private System.Windows.Forms.Button attachButton;
	}
}
