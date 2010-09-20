// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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

		public override void LoadChildren()
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

		public override DropEffect CanDrop(IDataObject data, DropEffect requestedEffect)
		{
			var paths = data.GetData(typeof(string[])) as string[];
			if (paths != null) {
				return requestedEffect == DropEffect.Link ? DropEffect.Move : requestedEffect;
			}
			return DropEffect.None;
		}

		public override void Drop(IDataObject data, int index, DropEffect finalEffect)
		{
			var paths = data.GetData(typeof(string[])) as string[];
			if (paths != null) {
				for (int i = 0; i < paths.Length; i++) {
					var p = paths[i];
					if (File.Exists(p)) {
						Children.Insert(index + i, new FileNode(p));
					}
					else {
						Children.Insert(index + i, new FolderNode(p));
					}
				}
			}
		}
	}
}
