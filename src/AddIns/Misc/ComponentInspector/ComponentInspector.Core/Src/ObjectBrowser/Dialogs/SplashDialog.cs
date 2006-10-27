// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Oakland Software Incorporated" email="general@oaklandsoftware.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Drawing;
using System.Windows.Forms;

namespace NoGoop.ObjBrowser.Dialogs
{
	internal class SplashDialog : Form
	{
		protected Label _productNameLabel;

		internal String SplashProductName {
			set {
				_productNameLabel.Text = value;
				_productNameLabel.Invalidate();
			}
		}

		internal SplashDialog()
		{
			StartPosition = FormStartPosition.CenterScreen;

			DockPadding.All = 10;

			// Can't close me
			ControlBox = false;
			MinimizeBox = false;
			MaximizeBox = false;

			FormBorderStyle = FormBorderStyle.FixedDialog;
			InitializeComponent();
		}

		public void InitializeComponent()
		{
			BackColor = Color.White;

			Label l;
			int startY = 30;
			Height = 200;
			Width = 220;

			l = new Label();
			l.Font = new Font("Arial", 24F, 
							  FontStyle.Regular, 
							  GraphicsUnit.Point,
							  ((Byte)(0)));
			l.ForeColor = Color.FromArgb(0, 0, 99);
			l.Location = new Point(24, startY);
			l.Size = new Size(128, 40);
			l.Text = "oakland";
			Controls.Add(l);

			l = new Label();
			l.Font = new Font("Arial", 10F, 
							  FontStyle.Regular, 
							  GraphicsUnit.Point, 
							  ((System.Byte)(0)));
			l.Location = new Point(24, startY + 40);
			l.ForeColor = Color.FromArgb(0, 0, 99);
			l.Size = new Size(112, 24); 
			l.Text = "software";
			l.TextAlign = ContentAlignment.MiddleRight;
			Controls.Add(l);

			_productNameLabel = new Label();
			_productNameLabel.Font = new Font("Arial", 10F, 
							  FontStyle.Regular, 
							  GraphicsUnit.Point,
							  ((System.Byte)(0)));
			_productNameLabel.Location = new Point(24, startY + 80);
			_productNameLabel.Size = new Size(296, 23);
			_productNameLabel.AutoSize = true;
			Controls.Add(_productNameLabel);
		}
	}
}
