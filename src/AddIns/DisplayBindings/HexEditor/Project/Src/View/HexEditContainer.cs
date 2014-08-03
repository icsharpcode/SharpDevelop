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

using System;
using System.IO;
using System.Windows.Forms;

using HexEditor.Util;
using ICSharpCode.Core;
using ICSharpCode.Core.WinForms;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Workbench;

namespace HexEditor.View
{
	public partial class HexEditContainer : UserControl
	{
		public bool HasSomethingSelected {
			get { return hexEditControl.HasSomethingSelected; }
		}
		
		public bool CanUndo {
			get { return hexEditControl.CanUndo; }
		}
		
		public bool CanRedo {
			get { return hexEditControl.CanRedo; }
		}
		
		public HexEditContainer()
		{
			InitializeComponent();
		}
		
		bool loaded = false;
		
		void Init(object sender, EventArgs e)
		{
			try {
				hexEditControl.Initializing = true;
				if (loaded)
					return;
				loaded = true;
				
				tbSizeToFit.Text = StringParser.Parse(tbSizeToFit.Text);
				
				ToolStripControlHost bytesPerLine = new ToolStripControlHost(tSTBCharsPerLine);
				this.toolStrip1.Items.Insert(1, bytesPerLine);
				
				this.toolStrip1.Items.Insert(3, tCBViewMode);
				
				hexEditControl.BytesPerLine = Settings.BytesPerLine;
				tSTBCharsPerLine.Text = hexEditControl.BytesPerLine.ToString();
				this.hexEditControl.ContextMenuStrip = MenuService.CreateContextMenu(this.hexEditControl, "/AddIns/HexEditor/Editor/ContextMenu");
				tCBViewMode.SelectedIndex = 0;
				
				tCBViewMode.SelectedItem = Settings.ViewMode.ToString();
				hexEditControl.ViewMode = Settings.ViewMode;
				tbSizeToFit.Checked = hexEditControl.FitToWindowWidth = Settings.FitToWidth;
				tSTBCharsPerLine.Enabled = !Settings.FitToWidth;
				
				hexEditControl.Invalidate();
			} finally {
				hexEditControl.Initializing = false;
			}
		}

		void TbSizeToFitClick(object sender, EventArgs e)
		{
			tSTBCharsPerLine.Enabled = !tbSizeToFit.Checked;
			hexEditControl.FitToWindowWidth = tbSizeToFit.Checked;
			tSTBCharsPerLine.Text = hexEditControl.BytesPerLine.ToString();
		}

		void TCBViewModeSelectedIndexChanged(object sender, EventArgs e)
		{
			switch (tCBViewMode.SelectedIndex)
			{
				case 0:
					hexEditControl.ViewMode = ViewMode.Hexadecimal;
					break;
				case 1:
					hexEditControl.ViewMode = ViewMode.Octal;
					break;
				case 2:
					hexEditControl.ViewMode = ViewMode.Decimal;
					break;
			}
		}

		void HexEditContainerGotFocus(object sender, EventArgs e)
		{
			hexEditControl.Focus();
		}
		
		public void LoadFile(OpenedFile file, Stream stream)
		{
			hexEditControl.LoadFile(file, stream);
			hexEditControl.Invalidate();
		}
		
		public void SaveFile(OpenedFile file, Stream stream)
		{
			hexEditControl.SaveFile(file, stream);
			hexEditControl.Invalidate();
		}
		
		public string Cut()
		{
			string text = hexEditControl.Copy();
			hexEditControl.Delete();
			return text;
		}
		
		public string Copy()
		{
			return hexEditControl.Copy();
		}
		
		public void Paste(string text)
		{
			hexEditControl.Paste(text);
		}
		
		public void Delete()
		{
			hexEditControl.Delete();
		}
		
		public void SelectAll()
		{
			hexEditControl.SelectAll();
		}
		
		public void Undo()
		{
			hexEditControl.Undo();
		}
		
		public void Redo()
		{
			hexEditControl.Redo();
		}

		void tSTBCharsPerLine_TextChanged(object sender, EventArgs e)
		{
			int value = 0;
			if (int.TryParse(tSTBCharsPerLine.Text, out value)) {
				hexEditControl.BytesPerLine = value;
				tSTBCharsPerLine.Text = hexEditControl.BytesPerLine.ToString();
			}
		}

		void HexEditContainer_Resize(object sender, EventArgs e)
		{
			Init(sender, e);
			if (this.tbSizeToFit.Checked) tSTBCharsPerLine.Text = hexEditControl.BytesPerLine.ToString();
		}
	}
}
