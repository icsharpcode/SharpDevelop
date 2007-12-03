// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;

namespace PythonBinding.Tests.Utils
{
	/// <summary>
	/// Dummy IWorkbenchWindow class.
	/// </summary>
	public class MockWorkbenchWindow : IWorkbenchWindow
	{
		IViewContent viewContent;
		
		public MockWorkbenchWindow()
		{
		}
		
		public event EventHandler WindowSelected;
		public event EventHandler WindowDeselected;
		public event EventHandler TitleChanged;
		public event EventHandler CloseEvent;
		public event EventHandler ActiveViewContentChanged;
	
		public string Title {
			get {
				throw new NotImplementedException();
			}
			set {
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
				return viewContent;
			}
			set {
				viewContent = value;
			}
		}
		
		public bool CloseWindow(bool force)
		{
			throw new NotImplementedException();
		}
		
		public void SelectWindow()
		{
			throw new NotImplementedException();
		}
		
		public void RedrawContent()
		{
			throw new NotImplementedException();
		}
		
		public void SwitchView(int viewNumber)
		{
			throw new NotImplementedException();
		}
		
		public void OnWindowDeselected(EventArgs e)
		{
			if (WindowDeselected != null) {
				WindowDeselected(this, e);
			}
		}	
		
		public void OnWindowSelected(EventArgs e)
		{
			if (WindowSelected != null) {
				WindowSelected(this, e);
			}
		}		
						
		public Icon Icon {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
		
		public IList<IViewContent> ViewContents {
			get {
				throw new NotImplementedException();
			}
		}
		
		protected virtual void OnTitleChanged(EventArgs e)
		{
			if (TitleChanged != null) {
				TitleChanged(this, e);
			}
		}
				
		protected virtual void OnCloseEvent(EventArgs e)
		{
			if (CloseEvent != null) {
				CloseEvent(this, e);
			}
		}
		
		protected virtual void OnActiveViewContentChanged(EventArgs e)
		{
			if (ActiveViewContentChanged != null) {
				ActiveViewContentChanged(this, e);
			}
		}		
	}
}
