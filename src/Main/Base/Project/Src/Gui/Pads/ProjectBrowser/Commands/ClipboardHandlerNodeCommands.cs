// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;
using System.Threading;
using System.Drawing;
using System.Drawing.Printing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Diagnostics;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.SharpDevelop.Project.Commands
{
	public class CutProjectBrowserNode : AbstractMenuCommand
	{
		public override bool IsEnabled {
			get {
				return ProjectBrowserPad.Instance.EnableCut;
			}
		}
		
		public override void Run()
		{
			ProjectBrowserPad.Instance.Cut();
		}
	}
	
	public class CopyProjectBrowserNode : AbstractMenuCommand
	{
		public override bool IsEnabled {
			get {
				return ProjectBrowserPad.Instance.EnableCopy;
			}
		}
		
		public override void Run()
		{
			ProjectBrowserPad.Instance.Copy();
		}
	}
	
	public class PasteProjectBrowserNode : AbstractMenuCommand
	{
		public override bool IsEnabled {
			get {
				return ProjectBrowserPad.Instance.EnablePaste;
			}
		}
		
		public override void Run()
		{
			ProjectBrowserPad.Instance.Paste();
		}
	}
	
	public class DeleteProjectBrowserNode : AbstractMenuCommand
	{
		public override bool IsEnabled {
			get {
				return ProjectBrowserPad.Instance.EnableDelete;
			}
		}
		public override void Run()
		{
			ProjectBrowserPad.Instance.Delete();
		}
	}
	
}
