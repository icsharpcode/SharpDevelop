// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows.Forms;

namespace ICSharpCode.SharpDevelop.Gui
{
	public abstract class AbstractBaseViewContent : IBaseViewContent
	{
		IWorkbenchWindow workbenchWindow = null;
		
		public abstract Control Control {
			get;
		}
		
		public virtual IWorkbenchWindow WorkbenchWindow {
			get {
				return workbenchWindow;
			}
			set {
				workbenchWindow = value;
				OnWorkbenchWindowChanged(EventArgs.Empty);
			}
		}
		
		public virtual string TabPageText {
			get {
				return "Abstract Content";
			}
		}
		
		public virtual void SwitchedTo()
		{
		}
		
		public virtual void Selected()
		{
		}
		
		public virtual void Deselected()
		{
		}
		
		public virtual void RedrawContent()
		{
		}
		
		public virtual void Dispose()
		{
			workbenchWindow = null;
		}
		
		protected virtual void OnWorkbenchWindowChanged(EventArgs e)
		{
			if (WorkbenchWindowChanged != null) {
				WorkbenchWindowChanged(this, e);
			}
		}
		
		public event EventHandler WorkbenchWindowChanged;
	}
}
