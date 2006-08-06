/*
 * Created by SharpDevelop.
 * User: Daniel Grunwald
 * Date: 09.05.2006
 * Time: 17:22
 */

using System;
using System.Text;
using System.Windows.Forms;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;

namespace LineCounterAddin
{
	public class ToolCommand1 : AbstractMenuCommand
	{
		public override void Run()
		{
			WorkbenchSingleton.Workbench.ShowView(new LineCounterViewContent());
		}
	}
}
