// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Mime;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using ICSharpCode.Core.WinForms;

namespace ICSharpCode.SharpDevelop.Logging
{
	sealed class SDTraceListener : DefaultTraceListener
	{
		[Conditional("DEBUG")]
		public static void Install()
		{
			Debug.Listeners.Clear();
			Debug.Listeners.Add(new SDTraceListener());
		}
		
		public SDTraceListener()
		{
			base.AssertUiEnabled = false;
		}
		
		HashSet<string> ignoredStacks = new HashSet<string>();
		bool dialogIsOpen;
		
		public override void Fail(string message)
		{
			this.Fail(message, null);
		}
		
		public override void Fail(string message, string detailMessage)
		{
			base.Fail(message, detailMessage); // let base class write the assert to the debug console
			if (dialogIsOpen)
				return;
			string stackTrace = "";
			try {
				stackTrace = new StackTrace(true).ToString();
			} catch {}
			lock (ignoredStacks) {
				if (ignoredStacks.Contains(stackTrace))
					return;
			}
			message = message + Environment.NewLine + detailMessage + Environment.NewLine + stackTrace;
			dialogIsOpen = true;
			CustomDialog inputBox = new CustomDialog("Assertion Failed", message.TakeStartEllipsis(750), -1, 2, new[] { "Show Stacktrace", "Debug", "Ignore", "Ignore All" });
			try {
				while (true) { // show the dialog repeatedly until an option other than 'Show Stacktrace' is selected
					if (SD.MainThread.InvokeRequired) {
						inputBox.ShowDialog();
					} else {
						inputBox.ShowDialog(SD.WinForms.MainWin32Window);
					}
					switch (inputBox.Result) {
						case 0:
							ExceptionBox.ShowErrorBox(null, message);
							break; // show the custom dialog again
						case 1:
							Debugger.Break();
							return;
						case 2:
							return;
						case 3:
							lock (ignoredStacks) {
								ignoredStacks.Add(stackTrace);
							}
							return;
					}
				}
			} finally {
				dialogIsOpen = false;
				inputBox.Dispose();
			}
		}
	}
}
