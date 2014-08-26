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
using System.Drawing;
using System.IO;

using System.Linq;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;
using Microsoft.Win32;
using ResourceEditor.ViewModels;

namespace ResourceEditor.Commands
{
	class SaveEntryAsCommand : ResourceItemCommand
	{
		public override System.Collections.Generic.IEnumerable<ResourceItemEditorType> AllowedTypes {
			get {
				return new ResourceItemEditorType[] {
					ResourceItemEditorType.Bitmap,
					ResourceItemEditorType.Icon,
					ResourceItemEditorType.Cursor,
					ResourceItemEditorType.Binary
				};
			}
		}
		
		public override void ExecuteWithResourceItems(System.Collections.Generic.IEnumerable<ResourceEditor.ViewModels.ResourceItem> resourceItems)
		{
			var firstSelectedItem = resourceItems.First();
			
			var sdialog = new Microsoft.Win32.SaveFileDialog();
			sdialog.AddExtension = true;
			sdialog.FileName = firstSelectedItem.Name;
			
			if (firstSelectedItem.ResourceValue is System.Drawing.Bitmap) {
				sdialog.Filter = StringParser.Parse("${res:SharpDevelop.FileFilter.ImageFiles} (*.png)|*.png");
				sdialog.DefaultExt = ".png";
			} else if (firstSelectedItem.ResourceValue is System.Drawing.Icon) {
				sdialog.Filter = StringParser.Parse("${res:SharpDevelop.FileFilter.Icons}|*.ico");
				sdialog.DefaultExt = ".ico";
			} else if (firstSelectedItem.ResourceValue is System.Windows.Forms.Cursor) {
				sdialog.Filter = StringParser.Parse("${res:SharpDevelop.FileFilter.CursorFiles} (*.cur)|*.cur");
				sdialog.DefaultExt = ".cur";
			} else if (firstSelectedItem.ResourceValue is byte[]) {
				sdialog.Filter = StringParser.Parse("${res:SharpDevelop.FileFilter.BinaryFiles} (*.*)|*.*");
				sdialog.DefaultExt = ".bin";
			} else {
				return;
			}
			
			bool? dr = sdialog.ShowDialog();
			if (!dr.HasValue || !dr.Value) {
				return;
			}
			
			try {
				if (firstSelectedItem.ResourceValue is Icon) {
					FileStream fstr = new FileStream(sdialog.FileName, FileMode.Create);
					((Icon) firstSelectedItem.ResourceValue).Save(fstr);
					fstr.Close();
				} else if (firstSelectedItem.ResourceValue is Image) {
					Image img = (Image) firstSelectedItem.ResourceValue;
					img.Save(sdialog.FileName);
				} else {
					FileStream fstr = new FileStream(sdialog.FileName, FileMode.Create);
					BinaryWriter wr = new BinaryWriter(fstr);
					wr.Write((byte[]) firstSelectedItem.ResourceValue);
					fstr.Close();
				}
			} catch (Exception ex) {
				SD.MessageService.ShowWarning("Can't save resource to " + sdialog.FileName + ": " + ex.Message);
			}
		}
	}
}
