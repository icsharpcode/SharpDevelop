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
using System.Linq;
using System.Text;
using System.IO;
using System.Windows;

namespace ICSharpCode.TreeView.Demo
{
	public class FolderNode : FileSystemNode
	{
		public FolderNode(string path)
		{
			this.path = path;
			LazyLoading = true;
		}

		string path;

		public override object Text
		{
			get
			{
				var name = Path.GetFileName(path);
				if (name == "") return path;
				return name;
			}
		}

		public override object Icon
		{
			get
			{
				return Window1.LoadIcon("Folder.png");
			}
		}

		public override object ExpandedIcon
		{
			get
			{
				return Window1.LoadIcon("FolderOpened.png");
			}
		}

		public override bool IsCheckable
		{
			get
			{
				return true;
			}
		}

		public override string FullPath
		{
			get { return path; }
		}

		protected override void LoadChildren()
		{
			try {
				foreach (var p in Directory.GetDirectories(path)
				         .OrderBy(d => Path.GetDirectoryName(d))) {
					Children.Add(new FolderNode(p));
				}
				foreach (var p in Directory.GetFiles(path)
				         .OrderBy(f => Path.GetFileName(f))) {
					Children.Add(new FileNode(p));
				}
			}
			catch {
			}
		}
		
		public override bool CanPaste(IDataObject data)
		{
			return data.GetDataPresent(DataFormats.FileDrop);
		}
		
		public override void Paste(IDataObject data)
		{
			var paths = data.GetData(DataFormats.FileDrop) as string[];
			if (paths != null) {
				foreach (var p in paths) {
					if (File.Exists(p)) {
						Children.Add(new FileNode(p));
					} else {
						Children.Add(new FolderNode(p));
					}
				}
			}
		}
		
		public override void Drop(DragEventArgs e, int index)
		{
			var paths = e.Data.GetData(DataFormats.FileDrop) as string[];
			if (paths != null) {
				foreach (var p in paths) {
					if (File.Exists(p)) {
						Children.Insert(index++, new FileNode(p));
					} else {
						Children.Insert(index++, new FolderNode(p));
					}
				}
			}
		}
	}
}
