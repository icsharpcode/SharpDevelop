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
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;
using Microsoft.Win32;
using ResourceEditor.ViewModels;

namespace ResourceEditor.Commands
{
	class AddNewFileCommand : ResourceItemCommand
	{
		public override void ExecuteWithResourceItems(System.Collections.Generic.IEnumerable<ResourceItem> resourceItems)
		{
//			if (editor.ResourceList.WriteProtected) {
//				return;
//			}
			
			var editor = ResourceEditor;
			OpenFileDialog fdiag = new OpenFileDialog();
			fdiag.AddExtension = true;
			fdiag.Filter = StringParser.Parse("${res:SharpDevelop.FileFilter.AllFiles}|*.*");
			fdiag.Multiselect = true;
			fdiag.CheckFileExists = true;
				
			if ((bool)fdiag.ShowDialog()) {
				foreach (string filename in fdiag.FileNames) {
					string oresname = Path.ChangeExtension(Path.GetFileName(filename), null);
					if (oresname == "")
						oresname = "new";
						
					string resname = oresname;
						
					int i = 0;
					TestName:
					if (editor.ContainsResourceName(resname)) {
						if (i == 10) {
							continue;
						}
						i++;
						resname = oresname + "_" + i;
						goto TestName;
					}
						
					object tmp = LoadResource(filename);
					if (tmp == null) {
						continue;
					}
					editor.ResourceItems.Add(new ResourceItem(editor, resname, tmp));
				}
			}
		}
		
		object LoadResource(string name)
		{
			switch (Path.GetExtension(name).ToUpperInvariant()) {
				case ".CUR":
					try {
						return new System.Windows.Forms.Cursor(name);
					} catch {
						return null;
					}
				case ".ICO":
					try {
						return new System.Drawing.Icon(name);
					} catch {
						return null;
					}
				default:
					// Try to read a bitmap
					try {
						return new System.Drawing.Bitmap(name);
					} catch {
					}
					
					// Try to read a serialized object
					try {
						Stream r = File.Open(name, FileMode.Open);
						try {
							BinaryFormatter c = new BinaryFormatter();
							object o = c.Deserialize(r);
							r.Close();
							return o;
						} catch {
							r.Close();
						}
					} catch {
					}
					
					// Try to read a byte array
					try {
						FileStream s = new FileStream(name, FileMode.Open);
						BinaryReader r = new BinaryReader(s);
						Byte[] d = new Byte[(int)s.Length];
						d = r.ReadBytes((int)s.Length);
						s.Close();
						return d;
					} catch (Exception) {
						string message = ResourceService.GetString("ResourceEditor.Messages.CantLoadResource");
						MessageService.ShowWarning(message + " " + name + ".");
					}
					break;
			}
			return null;
		}
	}
}
