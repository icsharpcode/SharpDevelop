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
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Workbench;

namespace ICSharpCode.Scripting.Tests.Utils
{
	public class MockViewContent : IViewContent
	{
		OpenedFile primaryFile = new MockOpenedFile();
		FileName fileName;
		List<IViewContent> secondaryViewContents = new List<IViewContent>();
		
		public MockViewContent()
		{
		}
		
		public event EventHandler InfoTipChanged;
		public event EventHandler TitleNameChanged;
		public event EventHandler TabPageTextChanged;
		public event EventHandler Disposed;
		public event EventHandler IsDirtyChanged;
		
		public string TitleName {
			get {
				throw new NotImplementedException();
			}
		}
		
		public string InfoTip {
			get {
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
		
		public bool CloseWithSolution {
			get {
				throw new NotImplementedException();
			}
		}
		
		public ICollection<IViewContent> SecondaryViewContents {
			get { return secondaryViewContents;	}
		}
		
		public object Control {
			get {
				throw new NotImplementedException();
			}
		}
		
		public object InitiallyFocusedControl {
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
			get { return primaryFile; }
			set { primaryFile = value; }
		}
		
		public FileName PrimaryFileName {
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
		
		protected virtual void OnInfoTipChanged(EventArgs e)
		{
			if (InfoTipChanged != null) {
				InfoTipChanged(this, e);
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
		
		public virtual object GetService(Type serviceType)
		{
			return null;
		}
	}
}
