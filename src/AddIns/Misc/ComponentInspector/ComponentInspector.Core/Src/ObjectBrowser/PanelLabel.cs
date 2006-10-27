// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Oakland Software Incorporated" email="general@oaklandsoftware.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Drawing;
using System.Windows.Forms;

using NoGoop.Controls;

namespace NoGoop.ObjBrowser
{


	// Used to provide a label at the top of panels
	internal class PanelLabel : Panel
	{

		internal const int LABEL_OFFSET = 2;
		internal const int LINE_HEIGHT = 2;
		internal const int SPACING_HEIGHT = 2;

		protected Label         _spacingLabel2;
		protected Label         _spacingLabel3;
		protected Control       _textLabel;

		internal PanelLabel() : base()
		{
		}

		internal PanelLabel(Control parent, String text) : this()
		{
			_textLabel = new Label();
			_textLabel.Text = text;
			_textLabel.Height = TreeListPanel.BASE_HEADER_HEIGHT;
			AddControls(parent, _textLabel, true);
		}


		internal PanelLabel(Control parent, Panel labelPanel) : this()
		{
			AddControls(parent, labelPanel, true);
		}


		// This becomes the control that holds the specified panel
		internal PanelLabel(Panel labelPanel) : this()
		{
			AddControls(this, labelPanel, false);
			int h = 0;
			foreach (Control c in Controls)
				h += c.Height;
			Height = h;
		}

		protected void AddControls(Control parent, 
								   Control labelPanel, 
								   bool needBottomStuff)
		{
			// In reverse order of course

			if (needBottomStuff)
			{
				// Space
				_spacingLabel3 = new Label();
				_spacingLabel3.Dock = DockStyle.Top;
				_spacingLabel3.Height = SPACING_HEIGHT;
				parent.Controls.Add(_spacingLabel3);

				// Line at bottom
				Label lineLab = new Label();
				lineLab.Dock = DockStyle.Top;
				lineLab.BorderStyle = BorderStyle.Fixed3D;
				lineLab.Height = LINE_HEIGHT;
				parent.Controls.Add(lineLab);
			}

			// Text of label
			_textLabel = labelPanel;
			_textLabel.Dock = DockStyle.Top;
			//_textLabel.Left = LABEL_OFFSET;
			//_textLabel.Height = TreeListPanel.BASE_HEADER_HEIGHT;
			//_textLabel.Height = Utils.BUTTON_HEIGHT;
			parent.Controls.Add(_textLabel);

			// Top space
			_spacingLabel2 = new Label();
			_spacingLabel2.Dock = DockStyle.Top;
			_spacingLabel2.Height = SPACING_HEIGHT;
			parent.Controls.Add(_spacingLabel2);

		}

		internal void SetBackColor(Color backColor)
		{
			_spacingLabel2.BackColor = backColor;
			_spacingLabel3.BackColor = backColor;
			_textLabel.BackColor = backColor;
		}

	}
}
