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
		public abstract object Content {
			get;
		}
		
		public virtual void RedrawContent()
		{
		}
		
		public virtual void Dispose()
		{
		}
		
		protected bool IsVisible {
			get {
				if (WorkbenchSingleton.Workbench == null || WorkbenchSingleton.Workbench.WorkbenchLayout == null)
					return false;
				PadDescriptor d = WorkbenchSingleton.Workbench.GetPad(GetType());
				if (d == null)
					return false;
				return WorkbenchSingleton.Workbench.WorkbenchLayout.IsVisible(d);
			}
		}
	}
}
