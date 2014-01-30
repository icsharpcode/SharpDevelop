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
using System.ComponentModel;
using System.Windows;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Workbench;

namespace WixBinding.Tests.Utils
{
	public class MockWorkbench : IWorkbench
	{
		List<IViewContent> viewContents = new List<IViewContent>();
		
		public MockWorkbench()
		{
		}
		
		public IWin32Window MainWin32Window {
			get {
				throw new NotImplementedException();
			}
		}
		
		public Window MainWindow {
			get {
				throw new NotImplementedException();
			}
		}
		
		public string Title {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
		
		public ICollection<IViewContent> ViewContentCollection {
			get { return viewContents; }
		}
		
		public ICollection<IViewContent> PrimaryViewContents {
			get {
				throw new NotImplementedException();
			}
		}
		
		public IList<IWorkbenchWindow> WorkbenchWindowCollection {
			get {
				throw new NotImplementedException();
			}
		}
		
		public IList<PadDescriptor> PadContentCollection {
			get {
				throw new NotImplementedException();
			}
		}
		
		public IWorkbenchWindow ActiveWorkbenchWindow {
			get {
				throw new NotImplementedException();
			}
		}
		
		public IViewContent ActiveViewContent { get; set; }
		
		public IServiceProvider ActiveContent { get; set; }
		
		public bool IsActiveWindow {
			get {
				throw new NotImplementedException();
			}
		}
		
		public void Initialize()
		{
			throw new NotImplementedException();
		}
		
		public void ShowView(IViewContent content)
		{
			ShowView(content, true);
		}
		
		public void ShowView(IViewContent content, bool switchToOpenedView)
		{
			viewContents.Add(content);
		}
		
		public void ActivatePad(PadDescriptor content)
		{
			throw new NotImplementedException();
		}
		
		public PadDescriptor GetPad(Type type)
		{
			return null;
		}
		
		public void CloseAllViews()
		{
			throw new NotImplementedException();
		}
		
		public Properties CreateMemento()
		{
			throw new NotImplementedException();
		}
		
		public void SetMemento(Properties memento)
		{
			throw new NotImplementedException();
		}
		
		public event EventHandler ActiveWorkbenchWindowChanged;
		
		protected virtual void OnActiveWorkbenchWindowChanged(EventArgs e)
		{
			if (ActiveWorkbenchWindowChanged != null) {
				ActiveWorkbenchWindowChanged(this, e);
			}
		}
		
		public event EventHandler ActiveViewContentChanged;
		
		public void RaiseActiveViewContentChangedEvent()
		{
			if (ActiveViewContentChanged != null) {
				ActiveViewContentChanged(this, new EventArgs());
			}
		}
		
		public event EventHandler ActiveContentChanged;
		
		protected virtual void OnActiveContentChanged(EventArgs e)
		{
			if (ActiveContentChanged != null) {
				ActiveContentChanged(this, e);
			}
		}
		
		public event EventHandler<ViewContentEventArgs> ViewOpened;
		
		protected virtual void OnViewOpened(ViewContentEventArgs e)
		{
			if (ViewOpened != null) {
				ViewOpened(this, e);
			}
		}
		
		public event EventHandler<ViewContentEventArgs> ViewClosed;
		
		protected virtual void OnViewClosed(ViewContentEventArgs e)
		{
			if (ViewClosed != null) {
				ViewClosed(this, e);
			}
		}
		
		public bool FullScreen {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
		
		public bool CloseAllSolutionViews(bool force)
		{
			throw new NotImplementedException();
		}
		
		public string CurrentLayoutConfiguration {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
	}
}
