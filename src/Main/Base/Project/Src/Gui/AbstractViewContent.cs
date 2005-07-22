// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;
using System.Windows.Forms;

namespace ICSharpCode.SharpDevelop.Gui
{
	public abstract class AbstractViewContent : AbstractBaseViewContent, IViewContent
	{
		string untitledName = String.Empty;
		string titleName    = null;
		string fileName     = null;
		
		bool   isDirty  = false;
		bool   isViewOnly = false;
		
		public virtual string UntitledName {
			get {
				return untitledName;
			}
			set {
				untitledName = value;
			}
		}
		
		public virtual string TitleName {
			get {
				return IsUntitled ? untitledName : titleName;
			}
			set {
				titleName = value;
				OnTitleNameChanged(EventArgs.Empty);
			}
		}
		
		public virtual string FileName {
			get {
				return fileName;
			}
			set {
				fileName = value;
				OnFileNameChanged(EventArgs.Empty);
			}
		}
		
		public virtual bool CreateAsSubViewContent {
			get {
				return false;
			}
		}

		public AbstractViewContent()
		{
		}
		
		public AbstractViewContent(string titleName)
		{
			this.titleName = titleName;
		}
		
		public AbstractViewContent(string titleName, string fileName)
		{
			this.titleName = titleName;
			this.fileName  = fileName;
		}
		
		public virtual bool IsUntitled {
			get {
				return titleName == null;
			}
		}
		
		public virtual bool IsDirty {
			get {
				return isDirty;
			}
			set {
				isDirty = value;
				OnDirtyChanged(EventArgs.Empty);
			}
		}
		
		public virtual bool IsReadOnly {
			get {
				return false;
			}
		}		
		
		public virtual bool IsViewOnly {
			get {
				return isViewOnly;
			}
			set {
				isViewOnly = value;
			}
		}
		
		public virtual void Save()
		{
			if (IsDirty) {
				Save(fileName);
			}
		}
		
		public virtual void Save(string fileName)
		{
			throw new System.NotImplementedException();
		}
		
		public virtual void Load(string fileName)
		{
			throw new System.NotImplementedException();
		}
		
		/// <summary>
		/// Sets the file name to <param name="fileName"/> and the title to the file name without path. Sets dirty == false too.
		/// </summary>
		/// <param name="fileName">The name of the file currently inside the content.</param>
		protected void SetTitleAndFileName(string fileName)
		{
			TitleName = Path.GetFileName(fileName);
			FileName  = fileName;
			IsDirty   = false;
		}
		
		protected virtual void OnDirtyChanged(EventArgs e)
		{
			if (DirtyChanged != null) {
				DirtyChanged(this, e);
			}
		}
		
		protected virtual void OnTitleNameChanged(EventArgs e)
		{
			if (TitleNameChanged != null) {
				TitleNameChanged(this, e);
			}
		}
		
		protected virtual void OnFileNameChanged(EventArgs e)
		{
			if (FileNameChanged != null) {
				FileNameChanged(this, e);
			}
		}
		
		
		protected virtual void OnSaving(EventArgs e)
		{
			if (Saving != null) {
				Saving(this, e);
			}
		}
		
		protected virtual void OnSaved(SaveEventArgs e)
		{
			if (Saved != null) {
				Saved(this, e);
			}
		}
		
		public event EventHandler TitleNameChanged;
		public event EventHandler FileNameChanged;
		public event EventHandler DirtyChanged;
		public event EventHandler     Saving;
		public event SaveEventHandler Saved;
	}
}
