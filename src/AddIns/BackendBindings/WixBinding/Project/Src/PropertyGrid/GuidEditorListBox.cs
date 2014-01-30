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
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace ICSharpCode.WixBinding
{
	/// <summary>
	/// A list box which has one entry called "New Guid".
	/// </summary>
	public class GuidEditorListBox : ListBox
	{
		string guid = String.Empty;
		IWindowsFormsEditorService editorService;
		
		public GuidEditorListBox(IWindowsFormsEditorService editorService)
		{
			this.editorService = editorService;
			Items.Add("New Guid");
			Size = new Size(Width, ItemHeight);
			BorderStyle = BorderStyle.None;
		}
		
		public string Guid {
			get {
				return guid;
			}
			set {
				guid = value;
			}
		}
		
		protected override bool ShowFocusCues {
			get {
				return false;
			}
		}
		
		protected override void OnMouseClick(MouseEventArgs e)
		{
			base.OnMouseClick(e);
			int index = IndexFromPoint(e.Location);
			if (index != -1) {
				CreateNewGuid();
				editorService.CloseDropDown();
			}
		}
		
		protected override void OnPreviewKeyDown(PreviewKeyDownEventArgs e)
		{
			base.OnPreviewKeyDown(e);
			if (e.KeyData == Keys.Return) {
				if (SelectedIndex != -1) {
					CreateNewGuid();
				}
				editorService.CloseDropDown();
			}
		}
		
		void CreateNewGuid()
		{
			guid = System.Guid.NewGuid().ToString().ToUpperInvariant();
		}
	}
}
