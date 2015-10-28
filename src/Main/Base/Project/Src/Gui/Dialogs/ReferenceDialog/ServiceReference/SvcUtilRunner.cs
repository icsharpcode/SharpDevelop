// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
				this.ExitCode = await processRunner.RunInOutputPadAsync(SvcUtilMessageView.Category, commandLine.Command, commandLine.GetArguments());
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
