// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Itai Bar-Haim"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows.Forms;

using ClassDiagram;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Commands;
using ICSharpCode.SharpDevelop.Dom;

namespace ClassDiagramAddin
{
	public class SaveToBitmapCommand : ClassDiagramAddinCommand
	{
		public override void Run()
		{
			SaveFileDialog sfd = new SaveFileDialog();
			sfd.Title = "Save Diagram To Bitmap";
			sfd.Filter = "All Image Formats|*.bmp; *.png; *.jpg; *.jpeg; *.gif; *.tif; *.tiff|Bitmap files|*.bmp|Portable Network Graphics|*.png|JPEG files|*.jpg;*.jpeg|Graphics Interchange Format|*.gif|Tagged Image File|*.tif; *.tiff";
			sfd.OverwritePrompt = true;
			sfd.DefaultExt = ".png";
			if (sfd.ShowDialog() != DialogResult.Cancel)
			{
				Canvas.SaveToImage(sfd.FileName);
			}
		}
	}
}
