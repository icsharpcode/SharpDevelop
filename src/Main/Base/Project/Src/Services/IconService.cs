// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Runtime.InteropServices;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Resources;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;
using System.Xml;

namespace ICSharpCode.Core
{
	public static class IconService
	{
		static Dictionary<string, string> extensionHashtable   = new Dictionary<string, string>();
		static Dictionary<string, string> projectFileHashtable = new Dictionary<string, string>();
		
		readonly static char[] separators = {Path.DirectorySeparatorChar, Path.VolumeSeparatorChar};
		
		static IconService()
		{
			Thread myThread = new Thread(new ThreadStart(LoadThread));
			myThread.Name = "IconLoader";
			myThread.IsBackground = true;
			myThread.Priority = ThreadPriority.Normal;
			myThread.Start();
		}
		
		static void LoadThread()
		{
			try {
				InitializeIcons(AddInTree.GetTreeNode("/Workspace/Icons"));
			} catch (TreePathNotFoundException) {
				
			}
		}
		public static Bitmap GetGhostBitmap(string name)
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
			
			Bitmap bitmap = GetBitmap(name);
			Bitmap ghostBitmap = new Bitmap(bitmap.Width, bitmap.Height, PixelFormat.Format32bppArgb);
			
			using (Graphics g = Graphics.FromImage(ghostBitmap)) {
				g.FillRectangle(SystemBrushes.Window, new Rectangle(0, 0, bitmap.Width, bitmap.Height));
				g.DrawImage(bitmap, new Rectangle(0, 0, bitmap.Width, bitmap.Height), 0, 0,bitmap.Width, bitmap.Height, GraphicsUnit.Pixel, imgAttributes);
			}
			
			return ghostBitmap;
		}
		
		public static Bitmap GetBitmap(string name)
		{
			Bitmap bmp = ResourceService.GetBitmap(name);
			if (bmp != null) {
				return bmp;
			}
			
			return ResourceService.GetBitmap("Icons.16x16.MiscFiles");
		}
		
		public static Icon GetIcon(string name)
		{
			Icon icon = ResourceService.GetIcon(name);
			if (icon != null) {
				return icon;
			}
			return ResourceService.GetIcon("Icons.16x16.MiscFiles");
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
		
		static void InitializeIcons(AddInTreeNode treeNode)
		{
			extensionHashtable[".PRJX"] = "Icons.16x16.SolutionIcon";
			
			extensionHashtable[".CMBX"] = "Icons.16x16.CombineIcon";
			extensionHashtable[".SLN"]  = "Icons.16x16.CombineIcon";
			
			IconDescriptor[] icons = (IconDescriptor[])treeNode.BuildChildItems(null).ToArray(typeof(IconDescriptor));
			for (int i = 0; i < icons.Length; ++i) {
				IconDescriptor iconCodon = icons[i];
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
