// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

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
		
		public async void Run()
		{
			SvcUtilMessageView.ClearText();
			
			var commandLine = new SvcUtilCommandLine(Options);
			commandLine.Command = GetSvcUtilPath();
			using (ProcessRunner processRunner = new ProcessRunner()) {
				this.ExitCode = await processRunner.RunInOutputPadAsync(SvcUtilMessageView.Category, commandLine.Command, ProcessRunner.CommandLineToArgumentArray(commandLine.Arguments));
			}
			if (ProcessExited != null) {
				ProcessExited(this, new EventArgs());
			}
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
		
		void LineReceived(object sender, LineReceivedEventArgs e)
		{
			SvcUtilMessageView.AppendLine(e.Line);
		}
	}
}
