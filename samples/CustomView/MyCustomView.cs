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
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace CustomView
{
	public class MyCustomView : AbstractViewContent
	{
		Panel panel     = new Panel();
		Label testLabel = new Label();
		
		public MyCustomView()
		{
			testLabel.Text     = "Hello World!";
			testLabel.Location = new Point(8, 8);
			panel.Controls.Add(testLabel);
			
			TitleName = "My Custom View";
		}	
		
		/// <summary>
		/// The control that will be displayed in SharpDevelop.
		/// </summary>
		public override Control Control {
			get {
				return panel;
			}
		}
		
		// must be overriden, but *may* be useless for
		// 'custom'  views
		public override bool IsReadOnly {
			get {
				return false;
			}
		}
		
		public override void Load(OpenedFile file, Stream stream)
		{
		}

		public override void Save(OpenedFile file, Stream stream)
		{
		}
		
		// the redraw should get new add-in tree information
		// and update the view, the language or layout manager
		// may have changed.
		public override void RedrawContent()
		{
		}
		
		// The Dispose must be overriden, there is no default implementation
		// (because in this case I wouldn't override dipose, I would forget it ...)
		public override void Dispose()
		{
			testLabel.Dispose();
			panel.Dispose();
		}
	}
}
