// SharpDevelop samples
// Copyright (c) 2006, AlphaSierraPapa
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, are
// permitted provided that the following conditions are met:
//
// - Redistributions of source code must retain the above copyright notice, this list
//   of conditions and the following disclaimer.
//
// - Redistributions in binary form must reproduce the above copyright notice, this list
//   of conditions and the following disclaimer in the documentation and/or other materials
//   provided with the distribution.
//
// - Neither the name of the SharpDevelop team nor the names of its contributors may be used to
//   endorse or promote products derived from this software without specific prior written
//   permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS &AS IS& AND ANY EXPRESS
// OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY
// AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
// DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER
// IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT
// OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;
using System;
using System.IO;
using System.Windows.Forms;

namespace AlternateEditor
{
	public class Editor : AbstractViewContent
	{
		RichTextBox rtb = new RichTextBox();
		
		public override Control Control {
			get {
				return rtb;
			}
		}
		
		public override bool IsReadOnly {
			get {
				return rtb.ReadOnly;
			}
		}
		
		public Editor(OpenedFile file)
		{			
			Files.Add(file);
			OnFileNameChanged(file);
			file.ForceInitializeView(this);

			rtb.Dock = DockStyle.Fill;
			rtb.TextChanged += TextChanged;			
		}
		
		public override void RedrawContent()
		{
			rtb.Refresh();
		}
		
		public override void Dispose()
		{
			rtb.Dispose();
		}

		public override void Save(OpenedFile file, Stream stream)
		{
			rtb.SaveFile(stream, RichTextBoxStreamType.PlainText);
			TitleName = file.FileName;
		}
		
		public override void Load(OpenedFile file, Stream stream)
		{
			rtb.LoadFile(stream, RichTextBoxStreamType.PlainText);
			TitleName = file.FileName;
		}
		
		void TextChanged(object source, EventArgs e)
		{
			if (PrimaryFile != null) {
				PrimaryFile.MakeDirty();
			}
		}
	}
}
