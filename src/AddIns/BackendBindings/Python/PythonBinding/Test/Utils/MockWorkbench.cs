// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Windows.Forms;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;

namespace PythonBinding.Tests.Utils
{
	/// <summary>
	/// Dummy IWorkbench class.
	/// </summary>
	public class MockWorkbench : IWorkbench
	{
		public event EventHandler ActiveWorkbenchWindowChanged { add {} remove {} }
		public event EventHandler ActiveViewContentChanged { add {} remove {} }
		public event EventHandler ActiveContentChanged { add {} remove {} }
		public event ViewContentEventHandler ViewOpened { add {} remove {} }
		public event ViewContentEventHandler ViewClosed { add {} remove {} }
		
		public IWin32Window MainWin32Window {
			get { return null; }
		}
		
		public System.ComponentModel.ISynchronizeInvoke SynchronizingObject {
			get { return null; }
		}
		
		public System.Windows.Window MainWindow {
			get { return null; }
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
			get {
				throw new NotImplementedException();
			}
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
			get; set;
		}
		
		public IViewContent ActiveViewContent {
			get {
				if (ActiveWorkbenchWindow != null)
					return ActiveWorkbenchWindow.ActiveViewContent;
				else
					return null;
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
			throw new NotImplementedException();
		}
		
		public void CloseAllViews()
		{
			throw new NotImplementedException();
		}
		
		public void RedrawAllComponents()
		{
			throw new NotImplementedException();
		}
		
		public void UpdateRenderer()
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
		
		public bool FullScreen {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
	}
}
