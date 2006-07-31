/*
 * Created by SharpDevelop.
 * User: Daniel
 * Date: 7/30/2006
 * Time: 5:49 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
namespace IconEditor
{
	partial class IconPanel : System.Windows.Forms.UserControl
	{
		/// <summary>
		/// Designer variable used to keep track of non-visual components.
		/// </summary>
		private System.ComponentModel.IContainer components = null;
		
		/// <summary>
		/// Disposes resources used by the control.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing) {
				if (components != null) {
					components.Dispose();
					this.Entry = null; // dispose temp. bitmaps
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
			this.SuspendLayout();
			// 
			// IconPanel
			// 
			this.AllowDrop = true;
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.Name = "IconPanel";
			this.Size = new System.Drawing.Size(173, 171);
			this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.IconPanelMouseDown);
			this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.IconPanelMouseMove);
			this.DragDrop += new System.Windows.Forms.DragEventHandler(this.IconPanelDragDrop);
			this.DragEnter += new System.Windows.Forms.DragEventHandler(this.IconPanelDragEnter);
			this.Paint += new System.Windows.Forms.PaintEventHandler(this.IconPanelPaint);
			this.ResumeLayout(false);
		}
	}
}
