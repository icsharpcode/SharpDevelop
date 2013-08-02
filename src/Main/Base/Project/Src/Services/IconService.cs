// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

using ICSharpCode.Core;
using ICSharpCode.Core.WinForms;

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
			Bitmap bmp;
			try {
				bmp = WinFormsResourceService.GetBitmap(name);
			} catch (ResourceNotFoundException ex) {
				LoggingService.Warn(ex);
				bmp = null;
			}
			if (bmp != null) {
				return bmp;
			}
			
			return WinFormsResourceService.GetBitmap("Icons.16x16.MiscFiles");
		}
		
		public static Icon GetIcon(string name)
		{
			Icon icon = WinFormsResourceService.GetIcon(name);
			if (icon != null) {
				return icon;
			}
			return WinFormsResourceService.GetIcon("Icons.16x16.MiscFiles");
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
