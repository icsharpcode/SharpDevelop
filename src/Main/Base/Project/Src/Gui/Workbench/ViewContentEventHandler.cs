// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;
using System.Collections;
using System.Drawing;
using System.Diagnostics;
using System.CodeDom.Compiler;
using System.Windows.Forms;
using System.ComponentModel;
using System.Xml;

using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Gui
{
	public delegate void ViewContentEventHandler(object sender, ViewContentEventArgs e);
		
	public class ViewContentEventArgs : System.EventArgs
	{
		IViewContent content;
		
		public IViewContent Content {
			get {
				return content;
			}
			set {
				content = value;
			}
		}
		
		public ViewContentEventArgs(IViewContent content)
		{
			this.content = content;
		}
	}
}
