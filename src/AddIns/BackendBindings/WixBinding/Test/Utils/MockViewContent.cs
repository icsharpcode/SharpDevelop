// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.TextEditor;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace WixBinding.Tests.Utils
{
	/// <summary>
	/// Mock IViewContent class.
	/// </summary>
	public class MockViewContent : IViewContent
	{		
		string fileName = String.Empty;
		string untitledFileName = String.Empty;
		bool untitled = false;
		List<ISecondaryViewContent> secondaryViews = new List<ISecondaryViewContent>();
		
		public MockViewContent()
		{
		}
		
		#region IViewContent

		public event EventHandler TitleNameChanged;
		public event EventHandler Saving;
		public event SaveEventHandler Saved;
		public event EventHandler DirtyChanged;
		public event EventHandler FileNameChanged;
		
		public string UntitledName {
			get {
				return untitledFileName;
			}
			set {
				untitledFileName = value;
			}
		}
		
		public string TitleName {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
		
		public string FileName {
			get {
				return fileName;
			}
			set {
				fileName = value;
			}
		}
		
		public bool IsUntitled {
			get {
				return untitled;
			}
			set {
				untitled = value;
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
		
		public List<ISecondaryViewContent> SecondaryViewContents {
			get {
				return secondaryViews;
			}
		}
		
		public Control Control {
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
		}
		
		protected virtual void OnTitleNameChanged(EventArgs e)
		{
			if (TitleNameChanged != null) {
				TitleNameChanged(this, e);
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
		
		protected virtual void OnDirtyChanged(EventArgs e)
		{
			if (DirtyChanged != null) {
				DirtyChanged(this, e);
			}
		}
		
		protected virtual void OnFileNameChanged(EventArgs e)
		{
			if (FileNameChanged != null) {
				FileNameChanged(this, e);
			}
		}
		
		#endregion
	}
}

