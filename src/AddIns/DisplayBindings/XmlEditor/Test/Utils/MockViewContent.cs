// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Gui;

namespace XmlEditor.Tests.Utils
{
	public class MockViewContent : IViewContent
	{
		public event EventHandler TabPageTextChanged;
		
		protected virtual void OnTabPageTextChanged(EventArgs e)
		{
			if (TabPageTextChanged != null) {
				TabPageTextChanged(this, e);
			}
		}
		
		public event EventHandler TitleNameChanged;
		
		protected virtual void OnTitleNameChanged(EventArgs e)
		{
			if (TitleNameChanged != null) {
				TitleNameChanged(this, e);
			}
		}
		
		public event EventHandler InfoTipChanged;
		
		protected virtual void OnInfoTipChanged(EventArgs e)
		{
			if (InfoTipChanged != null) {
				InfoTipChanged(this, e);
			}
		}
		
		public event EventHandler Disposed;
		
		protected virtual void OnDisposed(EventArgs e)
		{
			if (Disposed != null) {
				Disposed(this, e);
			}
		}
		
		public event EventHandler IsDirtyChanged;
		
		protected virtual void OnIsDirtyChanged(EventArgs e)
		{
			if (IsDirtyChanged != null) {
				IsDirtyChanged(this, e);
			}
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
		
		public System.Collections.Generic.IList<ICSharpCode.SharpDevelop.OpenedFile> Files {
			get {
				throw new NotImplementedException();
			}
		}
		
		public ICSharpCode.SharpDevelop.OpenedFile PrimaryFile {
			get {
				throw new NotImplementedException();
			}
		}
		
		public ICSharpCode.Core.FileName PrimaryFileName {
			get {
				throw new NotImplementedException();
			}
		}
		
		public bool IsDisposed {
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
			get { throw new NotImplementedException(); }
		}
		
		public System.Collections.Generic.ICollection<IViewContent> SecondaryViewContents {
			get {
				throw new NotImplementedException();
			}
		}
		
		public bool IsDirty {
			get {
				throw new NotImplementedException();
			}
		}
		
		public void Save(ICSharpCode.SharpDevelop.OpenedFile file, System.IO.Stream stream)
		{
			throw new NotImplementedException();
		}
		
		public void Load(ICSharpCode.SharpDevelop.OpenedFile file, System.IO.Stream stream)
		{
			throw new NotImplementedException();
		}
		
		public ICSharpCode.SharpDevelop.INavigationPoint BuildNavPoint()
		{
			throw new NotImplementedException();
		}
		
		public bool SupportsSwitchFromThisWithoutSaveLoad(ICSharpCode.SharpDevelop.OpenedFile file, IViewContent newView)
		{
			throw new NotImplementedException();
		}
		
		public bool SupportsSwitchToThisWithoutSaveLoad(ICSharpCode.SharpDevelop.OpenedFile file, IViewContent oldView)
		{
			throw new NotImplementedException();
		}
		
		public void SwitchFromThisWithoutSaveLoad(ICSharpCode.SharpDevelop.OpenedFile file, IViewContent newView)
		{
			throw new NotImplementedException();
		}
		
		public void SwitchToThisWithoutSaveLoad(ICSharpCode.SharpDevelop.OpenedFile file, IViewContent oldView)
		{
			throw new NotImplementedException();
		}
		
		public void Dispose()
		{
			throw new NotImplementedException();
		}
		
		public object GetService(Type serviceType)
		{
			return null;
		}
	}
}
