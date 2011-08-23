// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;

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
		
		public object GetService(Type serviceType)
		{
			return null;
		}
	}
}
