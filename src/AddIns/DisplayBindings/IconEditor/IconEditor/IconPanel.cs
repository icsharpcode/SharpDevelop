// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace IconEditor
{
	public partial class IconPanel
	{
		Size size;
		int colorDepth;
		IconEntry entry;
		Bitmap maskBitmap;
		Bitmap bitmap;
		
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
				this.ContextMenuStrip = null;
				if (entry != null) {
					bitmap = entry.GetImage();
					if (entry.Type == IconEntryType.Classic) {
						maskBitmap = FixBitmap(entry.GetMaskImage());
						bitmap = FixBitmap(bitmap);
					} else {
						//this.ContextMenuStrip = trueColorContextMenu;
					}
				}
				Invalidate();
			}
		}
		
		Bitmap FixBitmap(Bitmap src)
		{
			Bitmap dest = new Bitmap(src.Width, src.Height);
			using (Graphics g = Graphics.FromImage(dest)) {
				g.DrawImageUnscaled(src, 0, 0);
			}
			src.Dispose();
			return dest;
		}
		
		public IconPanel(Size size, int colorDepth)
		{
			this.size = size;
			this.colorDepth = colorDepth;
			
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			this.ClientSize = new Size(size.Width + 2, size.Height + 2);
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
					Gdi32.BitBlt(destDC, drawOffset, drawOffset, size.Width, size.Height, memDC, 0, 0, Gdi32.SRCAND);
					
					Gdi32.SelectObject(memDC, oldHBitmap);
					Gdi32.DeleteObject(srcHBitmap);
					Gdi32.DeleteDC(memDC);
					Gdi32.DeleteDC(oldHBitmap);
				}
				{ // XOR blitting
					IntPtr memDC = Gdi32.CreateCompatibleDC(destDC);
					IntPtr srcHBitmap = bitmap.GetHbitmap();
					IntPtr oldHBitmap = Gdi32.SelectObject(memDC, srcHBitmap);
					Gdi32.BitBlt(destDC, drawOffset, drawOffset, size.Width, size.Height, memDC, 0, 0, Gdi32.SRCINVERT);
					
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
					bitmap.Save(dlg.FileName, ImageFormat.Png);
				}
			}
		}
		
		void ReplaceWithImageToolStripMenuItemClick(object sender, EventArgs e)
		{
			using (OpenFileDialog dlg = new OpenFileDialog()) {
				dlg.Filter = "Images|*.png;*.bmp;*.gif;*.jpg|All files|*.*";
				if (dlg.ShowDialog() == DialogResult.OK) {
					Bitmap newBitmap = new Bitmap(dlg.FileName);
					// scale to correct size and make it ARGB
					string oldFormat = entry.Width + "x" + entry.Height + "x" + entry.ColorDepth;
					string newFormat = newBitmap.Width + "x" + newBitmap.Height + "x";
					if (newBitmap.Width != entry.Width || newBitmap.Height != entry.Height) {
						MessageBox.Show("The loaded bitmap has the");
					}
					//entry.SetTrueColorImage(newBitmap, entry.IsCompressed);
					newBitmap.Dispose();
					this.Entry = entry; // re-display bitmap
				}
			}
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
				/*
				if (mouseDownLocation.IsEmpty == false) {
					int dx = Math.Abs(e.X - mouseDownLocation.X);
					int dy = Math.Abs(e.Y - mouseDownLocation.Y);
					if (dx > SystemInformation.DragSize.Width || dy > SystemInformation.DragSize.Height)
					{
						mouseDownLocation = Point.Empty;
						this.DoDragDrop(bitmap, DragDropEffects.Copy);
					}
				}
				*/
			}
		}
		
		void IconPanelDragEnter(object sender, DragEventArgs e)
		{
			/*
			if (e.Data.GetDataPresent(typeof(Bitmap)))
				e.Effect = DragDropEffects.Copy;
			else
				e.Effect = DragDropEffects.None;
			*/
		}
		
		void IconPanelDragDrop(object sender, DragEventArgs e)
		{
			/*try {
				Bitmap bmp = (Bitmap)e.Data.GetData(typeof(Bitmap));
				if (bmp != null) {
					entry.SetImage(bmp, entry.IsCompressed);
					this.Entry = entry; // re-display entry
				}
			} catch (Exception ex) {
				MessageBox.Show(ex.ToString());
			}*/
		}
	}
}
