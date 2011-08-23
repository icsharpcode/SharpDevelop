// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.SharpDevelop.Gui;

namespace XmlEditor.Tests.Utils
{
	public class MockWorkbench : IWorkbench
	{
		List<IViewContent> viewContentCollection = new List<IViewContent>();
		
		public MockWorkbench()
		{
		}
		
		public event EventHandler ActiveWorkbenchWindowChanged;
		
		protected virtual void OnActiveWorkbenchWindowChanged(EventArgs e)
		{
			if (ActiveWorkbenchWindowChanged != null) {
				ActiveWorkbenchWindowChanged(this, e);
			}
		}
		
		public event EventHandler ActiveViewContentChanged;
		
		protected virtual void OnActiveViewContentChanged(EventArgs e)
		{
			if (ActiveViewContentChanged != null) {
				ActiveViewContentChanged(this, e);
			}
		}
		
		public event EventHandler ActiveContentChanged;
		
		protected virtual void OnActiveContentChanged(EventArgs e)
		{
			if (ActiveContentChanged != null) {
				ActiveContentChanged(this, e);
			}
		}
		
		public event ViewContentEventHandler ViewOpened;
		
		protected virtual void OnViewOpened(ViewContentEventArgs e)
		{
			if (ViewOpened != null) {
				ViewOpened(this, e);
			}
		}
		
		public event ViewContentEventHandler ViewClosed;
		
		protected virtual void OnViewClosed(ViewContentEventArgs e)
		{
			if (ViewClosed != null) {
				ViewClosed(this, e);
			}
		}
		
		public System.Windows.Forms.IWin32Window MainWin32Window {
			get {
				throw new NotImplementedException();
			}
		}
		
		public System.ComponentModel.ISynchronizeInvoke SynchronizingObject {
			get {
				throw new NotImplementedException();
			}
		}
		
		public System.Windows.Window MainWindow {
			get {
				throw new NotImplementedException();
			}
		}
		
		public IStatusBarService StatusBar {
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
			get { return viewContentCollection; }
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
		
		public IList<ICSharpCode.SharpDevelop.PadDescriptor> PadContentCollection {
			get {
				throw new NotImplementedException();
			}
		}
		
		public IWorkbenchWindow ActiveWorkbenchWindow {
			get {
				throw new NotImplementedException();
			}
		}
		
		public IViewContent ActiveViewContent {
			get {
				throw new NotImplementedException();
			}
		}
		
		public object ActiveContent {
			get {
				throw new NotImplementedException();
			}
		}
		
		public IWorkbenchLayout WorkbenchLayout {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
		
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
			throw new NotImplementedException();
		}
		
		public void ShowView(IViewContent content, bool switchToOpenedView)
		{
			throw new NotImplementedException();
		}
		
		public void ShowPad(ICSharpCode.SharpDevelop.PadDescriptor content)
		{
			throw new NotImplementedException();
		}
		
		public void UnloadPad(ICSharpCode.SharpDevelop.PadDescriptor content)
		{
			throw new NotImplementedException();
		}
		
		public ICSharpCode.SharpDevelop.PadDescriptor GetPad(Type type)
		{
			throw new NotImplementedException();
		}
		
		public void CloseAllViews()
		{
			throw new NotImplementedException();
		}
		
		public ICSharpCode.Core.Properties CreateMemento()
		{
			throw new NotImplementedException();
		}
		
		public void SetMemento(ICSharpCode.Core.Properties memento)
		{
			throw new NotImplementedException();
		}
		
		public bool FullScreen {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
		
		public bool CloseAllSolutionViews()
		{
			throw new NotImplementedException();
		}
	}
}
