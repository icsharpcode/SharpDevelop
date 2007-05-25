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
	/// Summary description for DummyPropertyGrid.
	/// </summary>
	public class DummyToolbox : DockContent
	{
		private System.Windows.Forms.ImageList imageList;
		private WeifenLuo.WinFormsUI.InertButton inertButtonUp;
		private WeifenLuo.WinFormsUI.InertButton inertButtonDown;
		private InertButton inertButtonGeneral;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.ImageList imageList1;
		private System.ComponentModel.IContainer components;

		public DummyToolbox()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(DummyToolbox));
			this.inertButtonUp = new WeifenLuo.WinFormsUI.InertButton();
			this.imageList = new System.Windows.Forms.ImageList(this.components);
			this.inertButtonGeneral = new WeifenLuo.WinFormsUI.InertButton();
			this.inertButtonDown = new WeifenLuo.WinFormsUI.InertButton();
			this.label2 = new System.Windows.Forms.Label();
			this.imageList1 = new System.Windows.Forms.ImageList(this.components);
			this.SuspendLayout();
			// 
			// inertButtonUp
			// 
			this.inertButtonUp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.inertButtonUp.Enabled = false;
			this.inertButtonUp.ImageDisabled = ((System.Drawing.Image)(resources.GetObject("inertButtonUp.ImageDisabled")));
			this.inertButtonUp.ImageIndexDisabled = 0;
			this.inertButtonUp.ImageList = this.imageList;
			this.inertButtonUp.IsPopup = true;
			this.inertButtonUp.Location = new System.Drawing.Point(199, 3);
			this.inertButtonUp.Name = "inertButtonUp";
			this.inertButtonUp.Size = new System.Drawing.Size(19, 19);
			this.inertButtonUp.TabIndex = 0;
			// 
			// imageList
			// 
			this.imageList.ImageSize = new System.Drawing.Size(21, 21);
			this.imageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList.ImageStream")));
			this.imageList.TransparentColor = System.Drawing.Color.Black;
			// 
			// inertButtonGeneral
			// 
			this.inertButtonGeneral.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.inertButtonGeneral.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.inertButtonGeneral.IsPopup = true;
			this.inertButtonGeneral.Location = new System.Drawing.Point(2, 3);
			this.inertButtonGeneral.Name = "inertButtonGeneral";
			this.inertButtonGeneral.Size = new System.Drawing.Size(195, 19);
			this.inertButtonGeneral.TabIndex = 1;
			this.inertButtonGeneral.Text = "General";
			this.inertButtonGeneral.TextAlign = System.Drawing.ContentAlignment.TopLeft;
			// 
			// inertButtonDown
			// 
			this.inertButtonDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.inertButtonDown.Enabled = false;
			this.inertButtonDown.ImageDisabled = ((System.Drawing.Image)(resources.GetObject("inertButtonDown.ImageDisabled")));
			this.inertButtonDown.ImageIndexDisabled = 1;
			this.inertButtonDown.ImageList = this.imageList;
			this.inertButtonDown.IsPopup = true;
			this.inertButtonDown.Location = new System.Drawing.Point(199, 344);
			this.inertButtonDown.Name = "inertButtonDown";
			this.inertButtonDown.Size = new System.Drawing.Size(19, 19);
			this.inertButtonDown.TabIndex = 3;
			// 
			// label2
			// 
			this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.label2.BackColor = System.Drawing.SystemColors.ControlLightLight;
			this.label2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.label2.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.label2.ImageIndex = 0;
			this.label2.ImageList = this.imageList1;
			this.label2.Location = new System.Drawing.Point(2, 26);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(215, 19);
			this.label2.TabIndex = 5;
			this.label2.Text = "         Pointer";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// imageList1
			// 
			this.imageList1.ImageSize = new System.Drawing.Size(18, 18);
			this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
			this.imageList1.TransparentColor = System.Drawing.Color.White;
			// 
			// DummyToolbox
			// 
			this.ClientSize = new System.Drawing.Size(221, 365);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.inertButtonDown);
			this.Controls.Add(this.inertButtonGeneral);
			this.Controls.Add(this.inertButtonUp);
			this.DockableAreas = ((WeifenLuo.WinFormsUI.DockAreas)(((((WeifenLuo.WinFormsUI.DockAreas.Float | WeifenLuo.WinFormsUI.DockAreas.DockLeft) 
				| WeifenLuo.WinFormsUI.DockAreas.DockRight) 
				| WeifenLuo.WinFormsUI.DockAreas.DockTop) 
				| WeifenLuo.WinFormsUI.DockAreas.DockBottom)));
			this.DockPadding.Bottom = 2;
			this.DockPadding.Top = 2;
			this.HideOnClose = true;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "DummyToolbox";
			this.ShowHint = WeifenLuo.WinFormsUI.DockState.DockLeftAutoHide;
			this.TabText = "Toolbox";
			this.Text = "Toolbox";
			this.ResumeLayout(false);

		}
		#endregion
	}
}
