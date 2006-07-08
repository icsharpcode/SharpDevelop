// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mathias Simmack" email="mathias@simmack.de"/>
//     <version>$Revision$</version>
// </file>

namespace HtmlHelp2.Environment
{
	using System;
	using System.Drawing;
	using System.Windows.Forms;

	public class IconPictureBox : PictureBox
	{
		private Icon icon;

		public Icon Icon
		{
			get
			{
				return this.icon;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				else
				{
					this.icon  = value;
					base.Image = this.icon.ToBitmap();
				}
			}
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			e.Graphics.DrawIconUnstretched(this.icon, base.ClientRectangle);
		}
	}

	public class HtmlHelp2Dialog : Form
	{
		private IconPictureBox pictureBox1;
		private System.Windows.Forms.Label actionLabel;

		public string ActionLabel
		{
			get { return actionLabel.Text; }
			set { actionLabel.Text = value; }
		}

		public Icon ActionIcon
		{
			get { return pictureBox1.Icon; }
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				else
				{
					pictureBox1.Icon = value;
				}
			}
		}

		public HtmlHelp2Dialog()
		{
			this.InitializeComponent();
			pictureBox1.Icon = SystemIcons.Question;
		}

		#region Windows Forms Designer generated code
		/// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor. The Forms designer might
		/// not be able to load this method if it was changed manually.
		/// </summary>
		private void InitializeComponent()
		{
			this.actionLabel = new System.Windows.Forms.Label();
			this.pictureBox1 = new IconPictureBox();

			this.SuspendLayout();
			// 
			// actionLabel
			// 
			this.actionLabel.Location = new System.Drawing.Point(66, 16);
			this.actionLabel.Name = "actionLabel";
			this.actionLabel.Size = new System.Drawing.Size(190, 64);
			this.actionLabel.TabIndex = 1;
			// 
			// pictureBox1
			// 
			this.pictureBox1.Location = new System.Drawing.Point(16, 16);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(32, 32);
			this.pictureBox1.TabIndex = 0;
			this.pictureBox1.TabStop = false;
			// 
			// HtmlHelp2Dialog
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.ClientSize = new System.Drawing.Size(268, 96);
			this.Controls.Add(this.actionLabel);
			this.Controls.Add(this.pictureBox1);
			this.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.ControlBox = false;
			this.Name = "HtmlHelp2Dialog";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
//			this.Text = "HtmlHelp2Dialog";
			this.ResumeLayout(false);
		}
		#endregion
	}
}
