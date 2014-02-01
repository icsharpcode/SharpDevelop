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
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.WinForms;

namespace ICSharpCode.SharpDevelop
{
	public static class IconService
	{
		static Dictionary<string, string> extensionHashtable   = new Dictionary<string, string>();
		static Dictionary<string, string> projectFileHashtable = new Dictionary<string, string>();
		
		readonly static char[] separators = {Path.DirectorySeparatorChar, Path.VolumeSeparatorChar};
		
		static IconService()
		{
			try {
				InitializeIcons(AddInTree.GetTreeNode("/Workspace/Icons"));
			} catch (TreePathNotFoundException) {
				
			}
		}
		
		public static Bitmap GetGhostBitmap(string name)
		{
			return GetGhostBitmap(GetBitmap(name));
		}
		
		public static Bitmap GetGhostBitmap(Bitmap bitmap)
		{
			ColorMatrix clrMatrix = new ColorMatrix(new float[][] {
			                                        	new float[] {1, 0, 0, 0, 0},
			                                        	new float[] {0, 1, 0, 0, 0},
			                                        	new float[] {0, 0, 1, 0, 0},
			                                        	new float[] {0, 0, 0, 0.5f, 0},
			                                        	new float[] {0, 0, 0, 0, 1}
			                                        });
			
			ImageAttributes imgAttributes = new ImageAttributes();
			imgAttributes.SetColorMatrix(clrMatrix,
			                             ColorMatrixFlag.Default,
			                             ColorAdjustType.Bitmap);
			
			Bitmap ghostBitmap = new Bitmap(bitmap.Width, bitmap.Height, PixelFormat.Format32bppArgb);
			
			using (Graphics g = Graphics.FromImage(ghostBitmap)) {
				g.FillRectangle(SystemBrushes.Window, new Rectangle(0, 0, bitmap.Width, bitmap.Height));
				g.DrawImage(bitmap, new Rectangle(0, 0, bitmap.Width, bitmap.Height), 0, 0,bitmap.Width, bitmap.Height, GraphicsUnit.Pixel, imgAttributes);
			}
			
			return ghostBitmap;
		}
		
		public static Bitmap GetBitmap(string name)
		{
			Bitmap bmp = null;
			try {
				bmp = FileIconService.GetBitmap(name);
				if (bmp == null) {
					bmp = SD.ResourceService.GetBitmap(name);
				}
			} catch (ResourceNotFoundException ex) {
				LoggingService.Warn(ex);
			} catch (FileNotFoundException ex) {
				LoggingService.Warn(ex);
			}
			if (bmp != null) {
				return bmp;
			}
			return SD.ResourceService.GetBitmap("Icons.16x16.MiscFiles");
		}
		
		public static System.Windows.Media.ImageSource GetImageSource(string name)
		{
			System.Windows.Media.ImageSource img;
			try {
				img = SD.ResourceService.GetImageSource(name);
			} catch (ResourceNotFoundException ex) {
				LoggingService.Warn(ex);
				img = null;
			}
			if (img != null) {
				return img;
			}
			
			return SD.ResourceService.GetImageSource("Icons.16x16.MiscFiles");
		}
		
		public static string GetImageForProjectType(string projectType)
		{
			if (projectFileHashtable.ContainsKey(projectType)) {
				return projectFileHashtable[projectType];
			}
			return "Icons.16x16.SolutionIcon";
		}
		
		public static string GetImageForFile(string fileName)
		{
			string extension = Path.GetExtension(fileName).ToUpperInvariant();
			if (extension.Length == 0) extension = ".TXT";
			if (extensionHashtable.ContainsKey(extension)) {
				return extensionHashtable[extension];
			}
			return "Icons.16x16.MiscFiles";
		}
		
		public static bool HasImageForFile(string fileName)
		{
			string extension = Path.GetExtension(fileName).ToUpperInvariant();
			return extensionHashtable.ContainsKey(extension);
		}
		
		static void InitializeIcons(AddInTreeNode treeNode)
		{
			extensionHashtable[".PRJX"] = "Icons.16x16.SolutionIcon";
			
			extensionHashtable[".CMBX"] = "Icons.16x16.CombineIcon";
			extensionHashtable[".SLN"]  = "Icons.16x16.CombineIcon";
			
			foreach (IconDescriptor iconCodon in treeNode.BuildChildItems<IconDescriptor>(null)) {
				string imageName = iconCodon.Resource != null ? iconCodon.Resource : iconCodon.Id;
				
				if (iconCodon.Extensions != null) {
					foreach (string ext in iconCodon.Extensions) {
						extensionHashtable[ext.ToUpperInvariant()] = imageName;
					}
				}
				
				if (iconCodon.Language != null) {
					projectFileHashtable[iconCodon.Language] = imageName;
				}
			}
		}
	}
}
