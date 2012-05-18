// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Util;

namespace ICSharpCode.SharpDevelop.Gui.Dialogs.ReferenceDialog.ServiceReference
{
	public class SvcUtilRunner
	{		
		public SvcUtilRunner(ServiceReferenceGeneratorOptions options)
		{
			this.Options = options;
		}
		
		public event EventHandler ProcessExited;
		
		public ServiceReferenceGeneratorOptions Options { get; private set; }
		
		public int ExitCode { get; private set; }
		
		public void Run()
		{
			SvcUtilMessageView.ClearText();
			
			var commandLine = new SvcUtilCommandLine(Options);
			commandLine.Command = GetSvcUtilPath();
			SvcUtilMessageView.AppendLine(commandLine.ToString());
			ProcessRunner runner = CreateProcessRunner();
			runner.Start(commandLine.Command, commandLine.Arguments);
		}
		
		string GetSvcUtilPath()
		{
			var path = new SvcUtilPath();
			if (!path.Exists) {
				DisplaySvcUtilNotFoundMessage();
			}
			return path.FileName;
		}
		
		void DisplaySvcUtilNotFoundMessage()
		{
			string message =
				"Unable to find svcutil.exe. Please install the Windows SDK or specify the path to svcutil in Tools | Options.";
			SvcUtilMessageView.AppendLine(message);
		}
		
		ProcessRunner CreateProcessRunner()
		{
			var runner = new ProcessRunner();
			runner.LogStandardOutputAndError = false;
			runner.OutputLineReceived += LineReceived;
			runner.ErrorLineReceived += LineReceived;
			runner.ProcessExited += SvcUtilProcessExited;
			return runner;
		}
		
		void LineReceived(object sender, LineReceivedEventArgs e)
		{
			SvcUtilMessageView.AppendLine(e.Line);
		}
		
		void SvcUtilProcessExited(object sender, EventArgs e)
		{
			SvcUtilMessageView.AppendLine("SvcUtil finished.");
			
			var runner = (ProcessRunner)sender;
			ExitCode = runner.ExitCode;
			
			WorkbenchSingleton.SafeThreadAsyncCall(() => OnProcessExited());
		}
		
		void OnProcessExited()
		{
			if (ProcessExited != null) {
				ProcessExited(this, new EventArgs());
			}
		}
	}
}
