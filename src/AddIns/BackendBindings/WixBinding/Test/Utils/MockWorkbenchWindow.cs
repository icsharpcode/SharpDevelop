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
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Workbench;

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
