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
		public abstract object Control {
			get;
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
		
		protected bool IsVisible {
			get {
				PadDescriptor d = this.PadDescriptor;
				if (d != null)
					return WorkbenchSingleton.Workbench.WorkbenchLayout.IsVisible(d);
				else
					return false;
			}
		}
	}
}
