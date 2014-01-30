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
	public class FileNode : FileSystemNode
	{
		public FileNode(string path)
		{
			this.name = Path.GetFileName(path);
			this.info = new FileInfo(path);
		}

		FileInfo info;
		string name;

		public override object Text
		{
			get
			{
				return name;
			}
		}

		public override object Icon
		{
			get
			{
				return Window1.LoadIcon("File.png");
			}
		}

		public override object ToolTip
		{
			get
			{
				return info.FullName;
			}
		}

		public override bool IsEditable
		{
			get
			{
				return true;
			}
		}

		public override string LoadEditText()
		{
			return name;
		}

		public override bool SaveEditText(string value)
		{
			if (value.Contains("?")) {
				MessageBox.Show("?");
				return false;
			}
			else {
				name = value;
				return true;
			}
		}

		public override long? FileSize
		{
			get { return info.Length; }
		}

		public override DateTime? FileModified
		{
			get { return info.LastWriteTime; }
		}

		public override string FullPath
		{
			get { return info.FullName; }
		}

		public override bool CanPaste(IDataObject data)
		{
			return Parent.CanPaste(data);
		}
		
		public override void Paste(IDataObject data)
		{
			Parent.Paste(data);
		}
	}
}
