// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.SharpDevelop.DefaultEditor;
using System;
using System.Windows.Forms;
using ICSharpCode.Core;
using ICSharpCode.Core.WinForms;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.SharpDevelop.Commands
{
	public class WordCount : AbstractMenuCommand
	{
		public override void Run()
		{
			using (WordCountDialog wcd = new WordCountDialog()) {
				wcd.ShowDialog(WorkbenchSingleton.MainWin32Window);
			}
		}
	}
}
