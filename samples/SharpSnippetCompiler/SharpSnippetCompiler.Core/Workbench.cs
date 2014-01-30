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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.SharpSnippetCompiler.Core
{
	public class Workbench : IWorkbench
	{
		readonly static string viewContentPath = "/SharpDevelop/Workbench/Pads";

		Window mainWindow;
		List<PadDescriptor> padDescriptors = new List<PadDescriptor>();
		List<IViewContent> views = new List<IViewContent>();
		IWorkbenchLayout workbenchLayout;
		IViewContent activeViewContent;
		object activeContent;
		StatusBarService statusBarService = new StatusBarService();
		
		public event EventHandler ActiveWorkbenchWindowChanged;
		public event EventHandler ActiveViewContentChanged;
		public event EventHandler ActiveContentChanged;
		public event ViewContentEventHandler ViewOpened;
		public event ViewContentEventHandler ViewClosed;

		public Workbench(Window mainWindow)
		{
			this.mainWindow = mainWindow;
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
			get { return views; }
		}
		
		public ICollection<IViewContent> PrimaryViewContents {
			get { return views.AsReadOnly(); }
		}
		
		public IList<IWorkbenchWindow> WorkbenchWindowCollection {
			get {
				throw new NotImplementedException();
			}
		}
		
		public IList<PadDescriptor> PadContentCollection {
			get { return padDescriptors; }
		}
		
		public IWorkbenchWindow ActiveWorkbenchWindow {
			get {
				throw new NotImplementedException();
			}
		}
		
		public IViewContent ActiveViewContent {
			get { return activeViewContent; }
			set { activeViewContent = value; }
		}
		
		public object ActiveContent {
			get { return activeContent; }
			set { activeContent = value; }
		}
		
		public IWorkbenchLayout WorkbenchLayout {
			get { return workbenchLayout; }
			set { workbenchLayout = value; }
		}
		
		public bool IsActiveWindow {
			get {
				throw new NotImplementedException();
			}
		}
		
		public void ShowView(IViewContent content)
		{
			views.Add(content);
			OnViewOpened(new ViewContentEventArgs(content));
		}
		
		public void ShowView(IViewContent content, bool switchToOpenedView)
		{
			throw new NotImplementedException();
		}
		
		public void ShowPad(PadDescriptor content)
		{
			throw new NotImplementedException();
		}

		public void UnloadPad(PadDescriptor content)
		{
			throw new NotImplementedException();
		}
		
		public PadDescriptor GetPad(Type type)
		{
			foreach (PadDescriptor pad in padDescriptors) {
				if (pad.Class == type.FullName) {
					return pad;
				}
			}
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
			Console.WriteLine("Workbench.SetMemento not implemented");
		}
		
		public void Initialize()
		{
			SynchronizingObject = new WpfSynchronizeInvoke(mainWindow.Dispatcher);
			try {
				List<PadDescriptor> contents = AddInTree.GetTreeNode(viewContentPath).BuildChildItems<PadDescriptor>(this);
				foreach (PadDescriptor content in contents) {
					if (content != null) {
						padDescriptors.Add(content);
					}
				}
			} catch (TreePathNotFoundException) {}
		}
		
		protected virtual void OnActiveWorkbenchWindowChanged(EventArgs e)
		{
			if (ActiveWorkbenchWindowChanged != null) {
				ActiveWorkbenchWindowChanged(this, e);
			}
		}

		protected virtual void OnActiveViewContentChanged(EventArgs e)
		{
			if (ActiveViewContentChanged != null) {
				ActiveViewContentChanged(this, e);
			}
		}

		protected virtual void OnActiveContentChanged(EventArgs e)
		{
			if (ActiveContentChanged != null) {
				ActiveContentChanged(this, e);
			}
		}

		protected virtual void OnViewOpened(ViewContentEventArgs e)
		{
			if (ViewOpened != null) {
				ViewOpened(this, e);
			}
		}

		protected virtual void OnViewClosed(ViewContentEventArgs e)
		{
			if (ViewClosed != null) {
				ViewClosed(this, e);
			}
		}
		
		public IWin32Window MainWin32Window {
			get {
				throw new NotImplementedException();
			}
		}
		
		public ISynchronizeInvoke SynchronizingObject { get; private set; }
		
		public Window MainWindow {
			get { return mainWindow; }
		}
		
		public IStatusBarService StatusBar {
			get { return statusBarService; }
		}
		
		public bool FullScreen { get; set; }
		
		public bool CloseAllSolutionViews()
		{
			return true;
		}
	}
}
