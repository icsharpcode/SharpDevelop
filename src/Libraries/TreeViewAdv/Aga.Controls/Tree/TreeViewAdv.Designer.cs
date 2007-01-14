using System.Windows.Forms;

namespace Aga.Controls.Tree
{
	partial class TreeViewAdv
	{
		private System.ComponentModel.IContainer components = null;

		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Component Designer generated code
		private void InitializeComponent()
		{
			this._vScrollBar = new System.Windows.Forms.VScrollBar();
			this._hScrollBar = new System.Windows.Forms.HScrollBar();
			this.SuspendLayout();
			// 
			// _vScrollBar
			// 
			this._vScrollBar.LargeChange = 1;
			this._vScrollBar.Location = new System.Drawing.Point(0, 0);
			this._vScrollBar.Maximum = 0;
			this._vScrollBar.Name = "_vScrollBar";
			this._vScrollBar.Size = new System.Drawing.Size(13, 80);
			this._vScrollBar.TabIndex = 1;
			this._vScrollBar.ValueChanged += new System.EventHandler(this._vScrollBar_ValueChanged);
			// 
			// _hScrollBar
			// 
			this._hScrollBar.LargeChange = 1;
			this._hScrollBar.Location = new System.Drawing.Point(0, 0);
			this._hScrollBar.Maximum = 0;
			this._hScrollBar.Name = "_hScrollBar";
			this._hScrollBar.Size = new System.Drawing.Size(80, 13);
			this._hScrollBar.TabIndex = 2;
			this._hScrollBar.ValueChanged += new System.EventHandler(this._hScrollBar_ValueChanged);
			// 
			// TreeViewAdv
			// 
			this.BackColor = System.Drawing.SystemColors.Window;
			this.Controls.Add(this._vScrollBar);
			this.Controls.Add(this._hScrollBar);
			this.ResumeLayout(false);

		}
		#endregion

		private VScrollBar _vScrollBar;
		private HScrollBar _hScrollBar;
	}
}
