// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Gui;

namespace WixBinding.Tests.Utils
{
	public class MockWorkbenchWindow : IWorkbenchWindow
	{
		bool selectWindowMethodCalled;
		
		public MockWorkbenchWindow()
		{
		}
		
		public string Title {
			get {
				throw new NotImplementedException();
			}
		}
		
		public bool IsDisposed {
			get {
				throw new NotImplementedException();
			}
		}
		
		public IViewContent ActiveViewContent {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
		
		public System.Windows.Media.ImageSource Icon {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
		
		public System.Collections.Generic.IList<IViewContent> ViewContents {
			get {
				throw new NotImplementedException();
			}
		}
		
		public void SwitchView(int viewNumber)
		{
			throw new NotImplementedException();
		}
		
		public bool CloseWindow(bool force)
		{
			throw new NotImplementedException();
		}
		
		public void SelectWindow()
		{
			selectWindowMethodCalled = true;
		}
		
		public bool SelectWindowMethodCalled {
			get { return selectWindowMethodCalled; }
		}
		
		public event EventHandler ActiveViewContentChanged;
		
		protected virtual void OnActiveViewContentChanged(EventArgs e)
		{
			if (ActiveViewContentChanged != null) {
				ActiveViewContentChanged(this, e);
			}
		}
		
		public event EventHandler TitleChanged;
		
		protected virtual void OnTitleChanged(EventArgs e)
		{
			if (TitleChanged != null) {
				TitleChanged(this, e);
			}
		}
	}
}
