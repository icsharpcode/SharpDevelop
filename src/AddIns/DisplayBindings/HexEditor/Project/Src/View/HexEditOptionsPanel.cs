// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="sie_pam@gmx.at"/>
//     <version>$Revision$</version>
// </file>

using HexEditor.Util;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;

namespace HexEditor.View
{
	public class HexEditOptionsPanel : AbstractOptionPanel
	{
		ComboBox cmbForeColor;
		ListBox lstElements;
		CheckBox cbBold;
		CheckBox cbItalic;
		CheckBox cbUnderline;
		CheckBox cbFitToWidth;
		Label lblOffsetPreview;
		Label lblDataPreview;
		Button btnSelectFont;
		FontDialog fdSelectFont;
		TextBox txtExtensions;
		
		NumericUpDown nUDBytesPerLine;
		DomainUpDown dUDViewModes;
		
		List<Color> Colors;
		Color customFore = Color.Transparent;
		Color customBack = Color.Transparent;
				
		bool fcmanualchange = false;
		
		public HexEditOptionsPanel()
		{
			Colors = new List<Color>();
			
			ListColors();
		}
		
		public override void LoadPanelContents()
		{
			SetupFromXmlStream(this.GetType().Assembly.GetManifestResourceStream("HexEditor.Resources.HexEditOptions.xfrm"));
			this.InitializeComponents();
			
			lblOffsetPreview.Font = Settings.OffsetFont;
			lblDataPreview.Font = Settings.DataFont;
			
			fdSelectFont.Font = new Font(Settings.OffsetFont, FontStyle.Regular);
			
			if (IsNamedColor(Settings.OffsetForeColor)) {
				Settings.OffsetForeColor = Color.FromName(GetColorName(Settings.OffsetForeColor));
			} else {
				customFore = Settings.OffsetForeColor;
			}
			
			if (IsNamedColor(Settings.DataForeColor)) {
				Settings.DataForeColor = Color.FromName(GetColorName(Settings.DataForeColor));
			} else {
				customFore = Settings.DataForeColor;
			}
			
			if (!Settings.OffsetForeColor.IsNamedColor) {
				cmbForeColor.SelectedIndex = 0;
			} else {
				cmbForeColor.SelectedIndex = cmbForeColor.Items.IndexOf(Settings.OffsetForeColor.Name);
			}
			
			this.cbBold.Checked = Settings.OffsetFont.Bold;
			this.cbItalic.Checked = Settings.OffsetFont.Italic;
			this.cbUnderline.Checked = Settings.OffsetFont.Underline;
			
			this.cbFitToWidth.Checked = Settings.FitToWidth;
			
			this.nUDBytesPerLine.Value = Settings.BytesPerLine;
			this.dUDViewModes.SelectedIndex = this.dUDViewModes.Items.IndexOf(Settings.ViewMode.ToString());
		}

		public override bool StorePanelContents()
		{
			Settings.BytesPerLine = (int)this.nUDBytesPerLine.Value;
			
			Settings.DataFont = this.lblDataPreview.Font;
			Settings.OffsetFont = this.lblOffsetPreview.Font;
			
			Settings.FileTypes = this.txtExtensions.Text.Split(new char[] {';', ' '}, StringSplitOptions.RemoveEmptyEntries);
			
			Settings.FitToWidth = this.cbFitToWidth.Checked;
			
			Settings.OffsetForeColor = this.lblOffsetPreview.ForeColor;
			Settings.DataForeColor = this.lblDataPreview.ForeColor;
			Settings.ViewMode = (ViewMode)Enum.Parse(typeof(ViewMode), ((this.dUDViewModes.SelectedItem == null) ? ViewMode.Hexadecimal.ToString() : this.dUDViewModes.SelectedItem.ToString()));
			
			return true;
		}

		void InitializeComponents()
		{
			cmbForeColor = (ComboBox)ControlDictionary["cmbForeColor"];
			lstElements = (ListBox)ControlDictionary["lstElements"];
			cbBold = (CheckBox)ControlDictionary["cbBold"];
			cbItalic = (CheckBox)ControlDictionary["cbItalic"];
			cbUnderline = (CheckBox)ControlDictionary["cbUnderline"];
			lblOffsetPreview = (Label)ControlDictionary["lblOffsetPreview"];
			lblDataPreview = (Label)ControlDictionary["lblDataPreview"];
			btnSelectFont = (Button)ControlDictionary["btnSelectFont"];
			
			nUDBytesPerLine = (NumericUpDown)ControlDictionary["nUDBytesPerLine"];
			dUDViewModes = (DomainUpDown)ControlDictionary["dUDViewModes"];
			cbFitToWidth = (CheckBox)ControlDictionary["cbFitToWidth"];
			
			txtExtensions = (TextBox)ControlDictionary["txtExtensions"];
			
			fdSelectFont = new FontDialog();
			
			// Initialize FontDialog
			fdSelectFont.FontMustExist = true;
			fdSelectFont.FixedPitchOnly = true;
			fdSelectFont.ShowEffects = false;
			fdSelectFont.ShowColor = false;
			
			cmbForeColor.Items.Add(StringParser.Parse("${res:Global.FontStyle.CustomColor}"));
			
			foreach (Color c in Colors) {
				cmbForeColor.Items.Add(c.Name);
			}
			
			lstElements.Items.Add(StringParser.Parse("${res:AddIns.HexEditor.Display.Elements.Offset}"));
			lstElements.Items.Add(StringParser.Parse("${res:AddIns.HexEditor.Display.Elements.Data}"));

			lstElements.SetSelected(0, true);
			
			foreach (string s in HexEditor.Util.ViewMode.GetNames(typeof(HexEditor.Util.ViewMode)))
			{
				dUDViewModes.Items.Add(s);
			}
			
			btnSelectFont.Click += new EventHandler(this.btnSelectFontClick);
			cmbForeColor.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.cmbForeColorDrawItem);
			cmbForeColor.SelectedValueChanged += new EventHandler(this.cmbForeColorSelectedValueChanged);
			
			cmbForeColor.DropDown += cmbForeColorDropDown;
			
			cbBold.CheckedChanged += new EventHandler(this.cbBoldCheckedChanged);
			cbItalic.CheckedChanged += new EventHandler(this.cbItalicCheckedChanged);
			cbUnderline.CheckedChanged += new EventHandler(this.cbUnderlineCheckedChanged);
			
			lstElements.SelectedValueChanged += new EventHandler(this.lstElementsSelectedValueChanged);
		}
		
		void ListColors()
		{
			foreach (Color c in System.ComponentModel.TypeDescriptor.GetConverter(typeof (Color)).GetStandardValues()) {
				Colors.Add(c);
			}
		}
		
		void btnSelectFontClick(object sender, EventArgs e)
		{
			if (fdSelectFont.ShowDialog() == DialogResult.OK) {
				lblOffsetPreview.Font = new Font(fdSelectFont.Font, lblOffsetPreview.Font.Style);
				lblDataPreview.Font = new Font(fdSelectFont.Font, lblDataPreview.Font.Style);
			}
		}
		
		void cmbForeColorSelectedValueChanged(object sender, EventArgs e)
		{
			if (lstElements.SelectedIndex != -1) {
				if (cmbForeColor.SelectedIndex == 0) {
					if (fcmanualchange) {
						ColorDialog cdColor = new ColorDialog();
						if (cdColor.ShowDialog() == DialogResult.OK) {
							customFore = cdColor.Color;
							switch (lstElements.SelectedIndex) {
								case 0 :
									Settings.OffsetForeColor = customFore;
									break;
								case 1 :
									Settings.DataForeColor = customFore;
									break;
							}
						}
					}
				} else {
					if (cmbForeColor.SelectedIndex == -1) cmbForeColor.SelectedIndex = 0;
					switch (lstElements.SelectedIndex) {
						case 0 :
							Settings.OffsetForeColor = Color.FromName(cmbForeColor.Items[cmbForeColor.SelectedIndex].ToString());
							break;
						case 1 :
							Settings.DataForeColor = Color.FromName(cmbForeColor.Items[cmbForeColor.SelectedIndex].ToString());
							break;
					}
				}
				
				lblOffsetPreview.ForeColor = Settings.OffsetForeColor;
				
				lblDataPreview.ForeColor = Settings.DataForeColor;
			} else {
				MessageService.ShowError("Please select an element first!");
			}
			fcmanualchange = false;
		}
		
		void cmbForeColorDrawItem(object sender, System.Windows.Forms.DrawItemEventArgs e)
		{
			Rectangle rc = new Rectangle(e.Bounds.X, e.Bounds.Y,
			                             e.Bounds.Width, e.Bounds.Height);
			Rectangle rc2 = new Rectangle(e.Bounds.X + 20, e.Bounds.Y,
			                              e.Bounds.Width, e.Bounds.Height);
			Rectangle rc3 = new Rectangle(e.Bounds.X + 5, e.Bounds.Y + 2, 10, 10);
			
			string str;
			Color color;
			if (e.Index != -1) {
				str = (string)cmbForeColor.Items[e.Index];
			} else {
				str = (string)cmbForeColor.Items[0];
			}
			
			if (str == "Custom") {
				color = customFore;
			} else {
				color = Color.FromName(str);
			}

			if ( e.State == (DrawItemState.Selected | DrawItemState.Focus | DrawItemState.NoAccelerator | DrawItemState.NoFocusRect)) {
				e.Graphics.FillRectangle(new SolidBrush(SystemColors.Highlight) , rc);
				e.Graphics.DrawString( str , cmbForeColor.Font, new SolidBrush(Color.White), rc2);
				e.Graphics.FillRectangle(new SolidBrush(color), rc3);
				e.Graphics.DrawRectangle(Pens.White, rc3);
			} else {
				e.Graphics.FillRectangle(new SolidBrush(Color.White) , rc);
				e.Graphics.DrawString( str , cmbForeColor.Font, new SolidBrush(Color.Black), rc2);
				e.Graphics.FillRectangle(new SolidBrush(color), rc3);
				e.Graphics.DrawRectangle(Pens.Black, rc3);
			}
		}
		
		void cbBoldCheckedChanged(object sender, EventArgs e)
		{
			if (lstElements.SelectedIndex != -1) {
				switch (lstElements.SelectedIndex) {
					case 0 :
						if ((cbBold.Checked & !lblOffsetPreview.Font.Bold) ^ (!cbBold.Checked & lblOffsetPreview.Font.Bold))
							lblOffsetPreview.Font = new Font(lblOffsetPreview.Font, lblOffsetPreview.Font.Style ^ FontStyle.Bold);
						break;
					case 1 :
						if ((cbBold.Checked & !lblDataPreview.Font.Bold) ^ (!cbBold.Checked & lblDataPreview.Font.Bold))
							lblDataPreview.Font = new Font(lblDataPreview.Font, lblDataPreview.Font.Style ^ FontStyle.Bold);
						break;
				}
			} else {
				MessageService.ShowError("Please select an element first!");
			}
		}
		
		void cbItalicCheckedChanged(object sender, EventArgs e)
		{
			if (lstElements.SelectedIndex != -1) {
				switch (lstElements.SelectedIndex) {
					case 0 :
						if ((cbItalic.Checked & !lblOffsetPreview.Font.Italic) || (!cbItalic.Checked & lblOffsetPreview.Font.Italic))
							lblOffsetPreview.Font = new Font(lblOffsetPreview.Font, lblOffsetPreview.Font.Style ^ FontStyle.Italic);
						break;
					case 1 :
						if ((cbItalic.Checked & !lblDataPreview.Font.Italic) || (!cbItalic.Checked & lblDataPreview.Font.Italic))
							lblDataPreview.Font = new Font(lblDataPreview.Font, lblDataPreview.Font.Style ^ FontStyle.Italic);
						break;
				}
			} else {
				MessageService.ShowError("Please select an element first!");
			}
		}
		
		void cbUnderlineCheckedChanged(object sender, EventArgs e)
		{
			if (lstElements.SelectedIndex != -1) {
				switch (lstElements.SelectedIndex) {
					case 0 :
						if ((cbUnderline.Checked & !lblOffsetPreview.Font.Underline) || (!cbUnderline.Checked & lblOffsetPreview.Font.Underline))
							lblOffsetPreview.Font = new Font(lblOffsetPreview.Font, lblOffsetPreview.Font.Style ^ FontStyle.Underline);
						break;
					case 1 :
						if ((cbUnderline.Checked & !lblDataPreview.Font.Underline) || (!cbUnderline.Checked & lblDataPreview.Font.Underline))
							lblDataPreview.Font = new Font(lblDataPreview.Font, lblDataPreview.Font.Style ^ FontStyle.Underline);
						break;
				}
			} else {
				MessageService.ShowError("Please select an element first!");
			}
		}
		
		void lstElementsSelectedValueChanged(object sender, EventArgs e)
		{
			switch (lstElements.SelectedIndex) {
				case 0 :
					this.cbBold.Checked = Settings.OffsetFont.Bold;
					this.cbItalic.Checked = Settings.OffsetFont.Italic;
					this.cbUnderline.Checked = Settings.OffsetFont.Underline;

					if (Settings.OffsetForeColor == customFore) {
						cmbForeColor.SelectedIndex = 0;
					} else {
						cmbForeColor.SelectedIndex = cmbForeColor.Items.IndexOf(Settings.OffsetForeColor.Name);
					}
					break;
				case 1 :
					this.cbBold.Checked = Settings.DataFont.Bold;
					this.cbItalic.Checked = Settings.DataFont.Italic;
					this.cbUnderline.Checked = Settings.DataFont.Underline;

					if (Settings.DataForeColor == customFore) {
						cmbForeColor.SelectedIndex = 0;
					} else {
						cmbForeColor.SelectedIndex = cmbForeColor.Items.IndexOf(Settings.DataForeColor.Name);
					}
					break;
			}
		}
		
		void cmbForeColorDropDown(object sender, EventArgs e)
		{
			fcmanualchange = true;
		}
		
		bool IsNamedColor(Color color)
		{
			foreach (Color c in Colors) {
				if (c.A == color.A) {
					if (c.R == color.R) {
						if (c.G == color.G) {
							if (c.B == color.B) {
								return true;
							}
						}
					}
				}
			}
			
			return false;
		}
		
		string GetColorName(Color color)
		{
			if (IsNamedColor(color)) {
				foreach (Color c in Colors) {
					if (c.A == color.A) {
						if (c.R == color.R) {
							if (c.G == color.G) {
								if (c.B == color.B) {
									return c.Name;
								}
							}
						}
					}
				}
			}
			return String.Empty;
		}
	}
}
