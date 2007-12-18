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

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;

namespace ImageViewer
{
	public class ImageViewContent : AbstractViewContent
	{
		PictureBox box = new PictureBox();
		ImageFormat format;
		
		public ImageViewContent(OpenedFile file)
		{
			this.TabPageText = "Image Viewer";
			this.Files.Add(file);
			file.ForceInitializeView(this);
		}
		
		public override Control Control {
			get {
				return box;
			}
		}
		
		public override bool IsReadOnly {
			get {
				if (File.Exists(PrimaryFile.FileName)) {
					return (File.GetAttributes(PrimaryFile.FileName)
				    	    & FileAttributes.ReadOnly) != 0;
				}
				return false;
			}
		}
			
		/// <summary>
		/// The load method takes a copy of the image and saves the
		/// image format so the image can be saved later without throwing
		/// a GDI+ exception. This is because the stream is disposed during
		/// the lifetime of the image:
		/// 
		/// http://support.microsoft.com/?id=814675
		/// </summary>
		public override void Load(OpenedFile file, Stream stream)
		{
			Image image = Image.FromStream(stream);
			format = image.RawFormat;
			
			box.SizeMode = PictureBoxSizeMode.Zoom;
			box.Image = new Bitmap(image.Width, image.Height);
			using (Graphics graphics = Graphics.FromImage(box.Image)) {
				graphics.DrawImage(image, 0, 0);
			}
		}
		
		public override void Save(OpenedFile file, Stream stream)
		{
			box.Image.Save(stream, format);
		}
	}
}
