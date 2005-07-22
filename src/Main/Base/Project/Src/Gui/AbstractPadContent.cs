// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Drawing;
using System.Windows.Forms;

using ICSharpCode.Core;

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
		
		public bool IsVisible {
			get {
				return Control.Visible && Control.Width > 0;
			}
		}
	}
}
