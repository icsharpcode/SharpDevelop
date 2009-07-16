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
	public abstract class AbstractPadContent : IPadContent
	{
		public abstract Control Control {
			get;
		}
		
		public virtual void RedrawContent()
		{
		}
		
		public virtual void Dispose()
		{
		}
		
		public void BringToFront()
		{
			PadDescriptor d = this.PadDescriptor;
			if (d != null)
				d.BringPadToFront();
		}
		
		protected virtual PadDescriptor PadDescriptor {
			get {
				if (WorkbenchSingleton.Workbench == null || WorkbenchSingleton.Workbench.WorkbenchLayout == null)
					return null;
				return WorkbenchSingleton.Workbench.GetPad(GetType());
			}
		}
		
		public bool IsVisible {
			get {
				Control ctl = this.Control;
				return ctl.Visible && ctl.Width > 0 && ctl.Height > 0;
			}
		}
	}
}
