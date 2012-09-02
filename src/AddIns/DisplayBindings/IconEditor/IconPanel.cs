// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;

namespace ICSharpCode.IconEditor
{
	public partial class IconPanel
	{
		Size iconSize;
		int colorDepth;
		IconEntry entry;
		Bitmap maskBitmap;
		Bitmap bitmap;
		
		public Size IconSize {
			get { return iconSize; }
		}
		
		public int ColorDepth {
			get { return colorDepth; }
		}
		
		public IconEntry Entry {
			get {
				return entry;
			}
			set {
				if (maskBitmap != null) {
					maskBitmap.Dispose();
					maskBitmap = null;
				}
				if (bitmap != null) {
					bitmap.Dispose();
					bitmap = null;
				}
				entry = value;
				if (entry != null) {
					bitmap = entry.GetImage();
					if (entry.Type == IconEntryType.Classic) {
						maskBitmap = FixBitmap(entry.GetMaskImage());
						bitmap = FixBitmap(bitmap);
					}
					
					exportANDMaskToolStripMenuItem.Visible = entry.Type == IconEntryType.Classic;
					exportXORMaskToolStripMenuItem.Visible = entry.Type == IconEntryType.Classic;
					compressedToolStripMenuItem.Checked = entry.IsCompressed;
					compressedToolStripMenuItem.Enabled = entry.IsCompressed || entry.ColorDepth == 32;
				}
				this.ContextMenuStrip = (entry != null) ? contextMenuStrip : emptyContextMenuStrip;
				UpdateSize();
				Invalidate();
			}
		}
		
		public event EventHandler EntryChanged = delegate {};
		
		Bitmap FixBitmap(Bitmap src)
		{
			if (src == null)
				return src;
			Bitmap dest = new Bitmap(src.Width, src.Height);
			using (Graphics g = Graphics.FromImage(dest)) {
				g.DrawImageUnscaled(src, 0, 0);
			}
			src.Dispose();
			return dest;
		}
		
		public IconPanel(Size size, int colorDepth)
		{
			this.iconSize = size;
			this.colorDepth = colorDepth;
			
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			UpdateSize();
		}
		
		void UpdateSize()
		{
			Size newClientSize = new Size(LimitSizeIfNoEntry(iconSize.Width) + 2, LimitSizeIfNoEntry(iconSize.Height) + 2);
			if (this.ClientSize != newClientSize)
				this.ClientSize = newClientSize;
		}
		
		int LimitSizeIfNoEntry(int size)
		{
			if (entry != null)
				return size;
			else
				return Math.Min(size, 32);
		}
		
		void IconPanelPaint(object sender, PaintEventArgs e)
		{
			const int drawOffset = 1;
			if (maskBitmap != null) {
				IntPtr destDC = e.Graphics.GetHdc();
				{ // AND blitting
					IntPtr memDC = Gdi32.CreateCompatibleDC(destDC);
					IntPtr srcHBitmap = maskBitmap.GetHbitmap();
					IntPtr oldHBitmap = Gdi32.SelectObject(memDC, srcHBitmap);
					Gdi32.BitBlt(destDC, drawOffset, drawOffset, iconSize.Width, iconSize.Height, memDC, 0, 0, Gdi32.SRCAND);
					
					// TODO: review if the objects get destroyed correctly
					Gdi32.SelectObject(memDC, oldHBitmap);
					Gdi32.DeleteObject(srcHBitmap);
					Gdi32.DeleteDC(memDC);
					Gdi32.DeleteDC(oldHBitmap);
				}
				{ // XOR blitting
					IntPtr memDC = Gdi32.CreateCompatibleDC(destDC);
					IntPtr srcHBitmap = bitmap.GetHbitmap();
					IntPtr oldHBitmap = Gdi32.SelectObject(memDC, srcHBitmap);
					Gdi32.BitBlt(destDC, drawOffset, drawOffset, iconSize.Width, iconSize.Height, memDC, 0, 0, Gdi32.SRCINVERT);
					
					Gdi32.SelectObject(memDC, oldHBitmap);
					Gdi32.DeleteObject(srcHBitmap);
					Gdi32.DeleteDC(memDC);
					Gdi32.DeleteDC(oldHBitmap);
				}
				Gdi32.DeleteDC(destDC);
			} else if (bitmap != null) {
				e.Graphics.DrawImageUnscaled(bitmap, drawOffset, drawOffset);
			}
		}
		
		/// <summary>
		/// Helper class containing Gdi32 API functions
		/// </summary>
		private static class Gdi32
		{
			/// <summary>dest = source AND dest</summary>
			public const int SRCAND = 0x008800C6;
			/// <summary>dest = source XOR dest</summary>
			public const int SRCINVERT = 0x00660046;
			/// <summary>dest = source</summary>
			public const int SRCCOPY = 0x00CC0020; // BitBlt dwRop parameter
			[DllImport("gdi32.dll")]
			public static extern bool BitBlt(IntPtr hObject,int nXDest,int nYDest,
			                                 int nWidth,int nHeight,IntPtr hObjectSource,
			                                 int nXSrc,int nYSrc,int dwRop);
			
			[DllImport("gdi32.dll")]
			public static extern IntPtr CreateCompatibleBitmap(IntPtr hDC,int nWidth,
			                                                   int nHeight);
			[DllImport("gdi32.dll")]
			public static extern IntPtr CreateCompatibleDC(IntPtr hDC);
			[DllImport("gdi32.dll")]
			public static extern bool DeleteDC(IntPtr hDC);
			[DllImport("gdi32.dll")]
			public static extern bool DeleteObject(IntPtr hObject);
			[DllImport("gdi32.dll")]
			public static extern IntPtr SelectObject(IntPtr hDC,IntPtr hObject);
		}
		
		void ExportToolStripMenuItemClick(object sender, EventArgs e)
		{
			using (SaveFileDialog dlg = new SaveFileDialog()) {
				dlg.DefaultExt = "png";
				dlg.Filter = "PNG images|*.png|All files|*.*";
				if (dlg.ShowDialog() == DialogResult.OK) {
					entry.ExportArgbBitmap().Save(dlg.FileName, ImageFormat.Png);
				}
			}
		}
		
		void ExportANDMaskToolStripMenuItemClick(object sender, EventArgs e)
		{
			using (SaveFileDialog dlg = new SaveFileDialog()) {
				dlg.DefaultExt = "bmp";
				dlg.Filter = "BMP images|*.bmp|All files|*.*";
				if (dlg.ShowDialog() == DialogResult.OK) {
					using (var stream = dlg.OpenFile()) {
						entry.GetMaskImageData().CopyTo(stream);
					}
				}
			}
		}
		
		void ExportXORMaskToolStripMenuItemClick(object sender, EventArgs e)
		{
			using (SaveFileDialog dlg = new SaveFileDialog()) {
				dlg.DefaultExt = "bmp";
				dlg.Filter = "BMP images|*.bmp|All files|*.*";
				if (dlg.ShowDialog() == DialogResult.OK) {
					using (var stream = dlg.OpenFile()) {
						entry.GetImageData().CopyTo(stream);
					}
				}
			}
		}
		
		void DeleteToolStripMenuItemClick(object sender, EventArgs e)
		{
			this.Entry = null;
			EntryChanged(this, e);
		}
		
		void ReplaceWithImageToolStripMenuItemClick(object sender, EventArgs e)
		{
			using (OpenFileDialog dlg = new OpenFileDialog()) {
				dlg.Filter = "Images|*.png;*.bmp;*.gif;*.jpg|All files|*.*";
				if (dlg.ShowDialog() == DialogResult.OK) {
					try {
						Bitmap newBitmap = new Bitmap(dlg.FileName);
						SetImage(newBitmap);
					} catch (Exception ex) {
						MessageService.ShowHandledException(ex);
					}
				}
			}
		}
		
		void CompressedToolStripMenuItemClick(object sender, EventArgs e)
		{
			// Toggle compression
			this.Entry = new IconEntry(iconSize.Width, iconSize.Height, colorDepth, entry.ExportArgbBitmap(), !entry.IsCompressed);
			EntryChanged(this, EventArgs.Empty);
		}
		
		void SetImage(Bitmap newBitmap)
		{
			// scale to correct size and make it ARGB
			if (iconSize != newBitmap.Size) {
				int r = MessageService.ShowCustomDialog(
					"Import Image",
					string.Format("The image has size {0}x{1}, but size {2}x{3} is expected.", newBitmap.Width, newBitmap.Height, iconSize.Width, iconSize.Height),
					0, 1, "Convert", "Cancel");
				if (r != 0) {
					return;
				}
			}
			bool? compress = entry != null ? entry.IsCompressed : (bool?)null;
			this.Entry = new IconEntry(iconSize.Width, iconSize.Height, colorDepth, newBitmap, compress);
			EntryChanged(this, EventArgs.Empty);
		}
		
		Point mouseDownLocation;
		
		void IconPanelMouseDown(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left) {
				mouseDownLocation = e.Location;
			}
		}
		
		void IconPanelMouseMove(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left) {
				if (mouseDownLocation.IsEmpty == false && entry != null) {
					int dx = Math.Abs(e.X - mouseDownLocation.X);
					int dy = Math.Abs(e.Y - mouseDownLocation.Y);
					if (dx > SystemInformation.DragSize.Width || dy > SystemInformation.DragSize.Height)
					{
						mouseDownLocation = Point.Empty;
						this.DoDragDrop(entry.ExportArgbBitmap(), DragDropEffects.Copy);
					}
				}
			}
		}
		
		void IconPanelDragEnter(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent(DataFormats.Bitmap)) {
				e.Effect = DragDropEffects.Copy;
			} else if (e.Data.GetDataPresent(DataFormats.FileDrop)) {
				e.Effect = DragDropEffects.None;
				string[] files = e.Data.GetData(DataFormats.FileDrop) as string[];
				if (files != null && files.Length == 1) {
					string ext = Path.GetExtension(files[0]);
					if (ext.Equals(".png", StringComparison.OrdinalIgnoreCase) || ext.Equals(".bmp", StringComparison.OrdinalIgnoreCase)
					    || ext.Equals(".gif", StringComparison.OrdinalIgnoreCase) || ext.Equals(".jpg", StringComparison.OrdinalIgnoreCase))
					{
						e.Effect = DragDropEffects.Copy;
					}
				}
			} else {
				e.Effect = DragDropEffects.None;
			}
		}
		
		void IconPanelDragDrop(object sender, DragEventArgs e)
		{
			try {
				Bitmap bmp = null;
				if (e.Data.GetDataPresent(DataFormats.Bitmap)) {
					bmp = (Bitmap)e.Data.GetData(DataFormats.Bitmap);
				} else if (e.Data.GetDataPresent(DataFormats.FileDrop)) {
					string[] files = e.Data.GetData(DataFormats.FileDrop) as string[];
					if (files != null && files.Length == 1) {
						bmp = new Bitmap(files[0]);
					}
				}
				if (bmp != null) {
					SetImage(bmp);
				}
			} catch (Exception ex) {
				MessageService.ShowHandledException(ex);
			}
		}
	}
}
