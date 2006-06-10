// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace ICSharpCode.SharpDevelop.Gui
{
	public abstract class AbstractViewContent : AbstractBaseViewContent, IViewContent
	{
		string untitledName = String.Empty;
		string titleName    = null;
		string fileName     = null;

		bool   isViewOnly = false;

		List<ISecondaryViewContent> secondaryViewContents = new List<ISecondaryViewContent>();
		
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

		/// <summary>
		/// Sets the file name to <paramref name="fileName"/> and the title to the file name without path. Sets dirty == false too.
		/// </summary>
		/// <param name="fileName">The name of the file currently inside the content.</param>
		protected void SetTitleAndFileName(string fileName)
		{
			TitleName = Path.GetFileName(fileName);
			FileName  = fileName;
			IsDirty   = false;
		}
						
		public event EventHandler FileNameChanged;
		
		protected virtual void OnFileNameChanged(EventArgs e)
		{
			if (FileNameChanged != null) {
				FileNameChanged(this, e);
			}
		}
		
		#region IViewContent implementation
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
				return titleName;
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
		
		public virtual bool IsUntitled {
			get {
				return titleName == null;
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
	
		/// <summary>
		/// Gets the list of secondary view contents attached to this view content.
		/// </summary>
		public List<ISecondaryViewContent> SecondaryViewContents {
			get {
				return secondaryViewContents;
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
		
		public event EventHandler TitleNameChanged;
		public event EventHandler     Saving;
		public event SaveEventHandler Saved;

		protected virtual void OnTitleNameChanged(EventArgs e)
		{
			if (TitleNameChanged != null) {
				TitleNameChanged(this, e);
			}
		}

		protected virtual void OnSaving(EventArgs e)
		{
			foreach (ISecondaryViewContent svc in SecondaryViewContents) {
				svc.NotifyBeforeSave();
			}
			if (Saving != null) {
				Saving(this, e);
			}
		}
		
		protected virtual void OnSaved(SaveEventArgs e)
		{
			foreach (ISecondaryViewContent svc in SecondaryViewContents) {
				svc.NotifyAfterSave(e.Successful);
			}
			if (Saved != null) {
				Saved(this, e);
			}
		}

		#region IBaseViewContent implementation
		// handled in AbstractBaseViewContent

		#region IDisposable implementation
		public override void Dispose()
		{
			foreach (ISecondaryViewContent svc in secondaryViewContents) {
				svc.Dispose();
			}
			base.Dispose();
		}
		#endregion
		#endregion
		
		#region ICanBeDirty implementation
		public virtual bool IsDirty {
			get {
				return isDirty;
			}
			set {
				if (isDirty != value) {
					isDirty = value;
					OnDirtyChanged(EventArgs.Empty);
				}
			}
		}
		bool   isDirty  = false;
		
		public event EventHandler DirtyChanged;

		protected virtual void OnDirtyChanged(EventArgs e)
		{
			if (DirtyChanged != null) {
				DirtyChanged(this, e);
			}
		}		
		#endregion
		#endregion			
	}
}
