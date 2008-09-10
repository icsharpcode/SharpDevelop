// Copyright (c) 2005 Daniel Grunwald
// Licensed under the terms of the "BSD License", see doc/license.txt

using System;
using System.Text;
using System.Windows.Forms;

using ICSharpCode.Core;

namespace Base
{
	/// <summary>
	/// Base ViewContent for files.
	/// </summary>
	public abstract class FileViewContent : IViewContent
	{
		public abstract Control Control {
			get;
		}
		
		string fileName;
		public event EventHandler FileNameChanged;
		
		public string FileName {
			get {
				return fileName;
			}
			set {
				if (fileName != value) {
					fileName = value;
					ChangeTitleToFileName();
					if (FileNameChanged != null) {
						FileNameChanged(this, EventArgs.Empty);
					}
				}
			}
		}
		
		protected virtual void ChangeTitleToFileName()
		{
			this.Title = System.IO.Path.GetFileName(this.FileName);
		}
		
		string title = "Untitled";
		public event EventHandler TitleChanged;
		
		public string Title {
			get {
				return title;
			}
			set {
				if (title != value) {
					title = value;
					
					if (TitleChanged != null) {
						TitleChanged(this, EventArgs.Empty);
					}
				}
			}
		}
		
		bool dirty;
		public event EventHandler DirtyChanged;
		
		public bool Dirty {
			get {
				return dirty;
			}
			set {
				if (dirty != value) {
					dirty = value;
					
					if (DirtyChanged != null) {
						DirtyChanged(this, EventArgs.Empty);
					}
				}
			}
		}
		
		public bool Save()
		{
			if (fileName == null) {
				return SaveAs();
			} else {
				if (Save(fileName)) {
					Dirty = false;
					return true;
				} else {
					return false;
				}
			}
		}
		
		public virtual bool SaveAs()
		{
			return ShowSaveAsDialog(GetFileFilter("/Workspace/FileFilter"), ".txt");
		}
		
		public static string GetFileFilter(string addInTreePath)
		{
			StringBuilder b = new StringBuilder();
			b.Append("All known file types|");
			foreach (string filter in AddInTree.BuildItems<string>(addInTreePath, null, true)) {
				b.Append(filter.Substring(filter.IndexOf('|') + 1));
				b.Append(';');
			}
			foreach (string filter in AddInTree.BuildItems<string>(addInTreePath, null, true)) {
				b.Append('|');
				b.Append(filter);
			}
			b.Append("|All files|*.*");
			return b.ToString();
		}
		
		protected bool ShowSaveAsDialog(string filter, string defaultExtension)
		{
			using (SaveFileDialog dlg = new SaveFileDialog()) {
				dlg.Filter = filter;
				dlg.DefaultExt = defaultExtension;
				if (dlg.ShowDialog() == DialogResult.OK) {
					FileName = dlg.FileName;
					if (Save(dlg.FileName)) {
						Dirty = false;
						return true;
					}
				}
			}
			return false;
		}
		
		protected abstract bool Save(string fileName);
		
		public virtual bool Close()
		{
			if (this.Dirty) {
				DialogResult res = MessageBox.Show("The file was modified. Do you want to save it?", "Modified file", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button3);
				if (res == DialogResult.Yes)
					return Save();
				else if (res == DialogResult.No)
					return true; // close without saving
				else
					return false;
			} else {
				return true;
			}
		}
	}
}
