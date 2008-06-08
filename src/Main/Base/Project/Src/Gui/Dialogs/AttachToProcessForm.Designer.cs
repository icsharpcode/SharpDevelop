// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

namespace ICSharpCode.SharpDevelop.Gui
{
	partial class AttachToProcessForm
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
									this.titleColumnHeader});
			this.listView.FullRowSelect = true;
			this.listView.HideSelection = false;
			this.listView.Location = new System.Drawing.Point(0, 0);
			this.listView.Name = "listView";
			this.listView.Size = new System.Drawing.Size(636, 334);
			this.listView.Sorting = System.Windows.Forms.SortOrder.Ascending;
			this.listView.TabIndex = 4;
			this.listView.UseCompatibleStateImageBehavior = false;
			this.listView.View = System.Windows.Forms.View.Details;
			this.listView.ItemActivate += new System.EventHandler(this.ListViewItemActivate);
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
			this.titleColumnHeader.Width = 337;
			// 
			// AttachToProcessForm
			// 
			this.AcceptButton = this.attachButton;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.cancelButton;
			this.ClientSize = new System.Drawing.Size(637, 374);
			this.Controls.Add(this.listView);
			this.Controls.Add(this.refreshButton);
			this.Controls.Add(this.cancelButton);
			this.Controls.Add(this.attachButton);
			this.MinimumSize = new System.Drawing.Size(273, 140);
			this.Name = "AttachToProcessForm";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.Text = "Attach to Process";
			this.ResumeLayout(false);
		}
		private System.Windows.Forms.ColumnHeader titleColumnHeader;
		private System.Windows.Forms.ColumnHeader processIdColumnHeader;
		private System.Windows.Forms.ColumnHeader processColumnHeader;
		private System.Windows.Forms.ListView listView;
		private System.Windows.Forms.Button refreshButton;
		private System.Windows.Forms.Button cancelButton;
		private System.Windows.Forms.Button attachButton;
	}
}
