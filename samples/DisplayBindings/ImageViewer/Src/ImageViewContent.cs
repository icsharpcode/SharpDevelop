/*
 * Created by SharpDevelop.
 * User: Daniel Grunwald
 * Date: 19.10.2005
 * Time: 21:43
 */

using System;
using System.IO;
using System.Windows.Forms;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;

namespace ImageViewer
{
	public class ImageViewContent : AbstractViewContent
	{
		PictureBox box = new PictureBox();
		public override Control Control {
			get {
				return box;
			}
		}
		public override string TabPageText {
			get {
				return "Image Viewer";
			}
		}
		public override void Load(string fileName) {
			this.IsDirty = false;
			this.FileName = fileName;
			this.TitleName = Path.GetFileName(fileName);
			box.SizeMode = PictureBoxSizeMode.Zoom;
			box.Load(fileName);
		}
		
		public override void Save(string fileName) {
			this.IsDirty = false;
			this.FileName = fileName;
			this.TitleName = Path.GetFileName(fileName);
			box.Image.Save(fileName);
		}
		
		public override bool IsReadOnly {
			get {
				return (File.GetAttributes(this.FileName)
				        & FileAttributes.ReadOnly) != 0;
			}
		}
	}
}
