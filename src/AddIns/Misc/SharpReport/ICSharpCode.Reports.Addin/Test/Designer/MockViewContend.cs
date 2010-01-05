/*
 * Erstellt mit SharpDevelop.
 * Benutzer: Peter
 * Datum: 01.05.2009
 * Zeit: 17:53
 * 
 * Sie können diese Vorlage unter Extras > Optionen > Codeerstellung > Standardheader ändern.
 */
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.Reports.Addin.Test.Designer
{
	/// <summary>
	/// Description of MockViewContend.
	/// </summary>
	public class MockViewContent:IViewContent
	{
		
		string fileName = String.Empty;
		List<IViewContent> secondaryViewContents = new List<IViewContent>();
		
		public MockViewContent()
		{
		}
		
		public event EventHandler TitleNameChanged;		
		public event EventHandler TabPageTextChanged;				
		public event EventHandler Disposed;		
		public event EventHandler IsDirtyChanged;
			
		public Control Control
		{
			get {return null;}
		}
		
		public string TitleName {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
				
		public bool IsReadOnly {
			get {
				throw new NotImplementedException();
			}
		}
		
		public bool IsViewOnly {
			get {
				throw new NotImplementedException();
			}
		}
		
		public ICollection<IViewContent> SecondaryViewContents {
			get { return secondaryViewContents;	}
		}
		
		public object Content {
			get {
				throw new NotImplementedException();
			}
		}
		
		public IWorkbenchWindow WorkbenchWindow {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
		
		public string TabPageText {
			get {
				throw new NotImplementedException();
			}
		}
		
		public bool IsDirty {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
		
		public void Save()
		{
			throw new NotImplementedException();
		}
		
		public void Save(string fileName)
		{
			throw new NotImplementedException();
		}
		
		public void Load(string fileName)
		{
			throw new NotImplementedException();
		}
		
		public INavigationPoint BuildNavPoint()
		{
			throw new NotImplementedException();
		}
		
		public void SwitchedTo()
		{
			throw new NotImplementedException();
		}
		
		public void Selected()
		{
			throw new NotImplementedException();
		}
		
		public void Deselecting()
		{
			throw new NotImplementedException();
		}
		
		public void Deselected()
		{
			throw new NotImplementedException();
		}
		
		public void RedrawContent()
		{
			throw new NotImplementedException();
		}
		
		public void Dispose()
		{
			throw new NotImplementedException();
		}
		
		public IList<OpenedFile> Files {
			get {
				throw new NotImplementedException();
			}
		}
		
		public OpenedFile PrimaryFile {
			get {
				MockOpenedFile file = new MockOpenedFile();
				//file.FileName = fileName;
				return file;
			}
		}
		
		public string PrimaryFileName {
			get { return fileName; }
			set { fileName = value; }
		}
		
		public bool IsDisposed {
			get {
				throw new NotImplementedException();
			}
		}
				
		public void Save(OpenedFile file, System.IO.Stream stream)
		{
			throw new NotImplementedException();
		}
		
		public void Load(OpenedFile file, System.IO.Stream stream)
		{
			throw new NotImplementedException();
		}
		
		public bool SupportsSwitchFromThisWithoutSaveLoad(OpenedFile file, IViewContent newView)
		{
			throw new NotImplementedException();
		}
		
		public bool SupportsSwitchToThisWithoutSaveLoad(OpenedFile file, IViewContent oldView)
		{
			throw new NotImplementedException();
		}
		
		public void SwitchFromThisWithoutSaveLoad(OpenedFile file, IViewContent newView)
		{
			throw new NotImplementedException();
		}
		
		public void SwitchToThisWithoutSaveLoad(OpenedFile file, IViewContent oldView)
		{
			throw new NotImplementedException();
		}
								
		protected virtual void OnTitleNameChanged(EventArgs e)
		{
			if (TitleNameChanged != null) {
				TitleNameChanged(this, e);
			}
		}
		
		protected virtual void OnTabPageTextChanged(EventArgs e)
		{
			if (TabPageTextChanged != null) {
				TabPageTextChanged(this, e);
			}
		}
		
		protected virtual void OnDisposed(EventArgs e)
		{
			if (Disposed != null) {
				Disposed(this, e);
			}
		}

		protected virtual void OnIsDirtyChanged(EventArgs e)
		{
			if (IsDirtyChanged != null) {
				IsDirtyChanged(this, e);
			}
		}		
	}
}
