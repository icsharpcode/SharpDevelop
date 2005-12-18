// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Text;
using System.IO;
using System.Threading;
using System.Drawing;
using System.Drawing.Printing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Diagnostics;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;

using ICSharpCode.SharpDevelop.Gui;
using NSvn.Common;
using NSvn.Core;
using ICSharpCode.Svn.Gui;

namespace ICSharpCode.Svn.Commands
{
	
	public class CommitCommand : SubversionCommand
	{
		protected override bool StartOperation()
		{
			using (CommitDialog commitDialog = new CommitDialog()) {
				commitDialog.LogMessage = AddInOptions.DefaultLogMessage;
				if (commitDialog.ShowDialog(ICSharpCode.SharpDevelop.Gui.WorkbenchSingleton.MainForm) == DialogResult.OK) {
					SvnClient.Instance.LogMessage = commitDialog.LogMessage;
					MessageService.ShowMessage("Not implemented.");
					//SvnClient.Instance.OperationStart("Commit", new ThreadStart(DoCommit));
				}
			}
			return false;
		}
	}
}
