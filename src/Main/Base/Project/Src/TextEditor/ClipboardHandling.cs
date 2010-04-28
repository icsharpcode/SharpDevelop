// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Runtime.InteropServices;
using System.Threading;

using ICSharpCode.Core.WinForms;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Util;
using Microsoft.Win32.SafeHandles;

namespace ICSharpCode.SharpDevelop.DefaultEditor
{
	/// <summary>
	/// This class fixes SD2-1466: SharpDevelop freezes when debugged application sets clipboard text.
	/// The problem is that Clipboard.ContainsText may wait for the application owning the clipboard,
	/// which in turn may currently wait for SharpDevelop (through the debugger)
	/// </summary>
	static class ClipboardHandling
	{
		public static void Initialize()
		{
			ICSharpCode.TextEditor.TextAreaClipboardHandler.GetClipboardContainsText = GetClipboardContainsText;
			if (WorkbenchSingleton.MainForm != null) {
				WorkbenchSingleton.MainForm.Activated += WorkbenchSingleton_MainForm_Activated;
			} else {
				WorkbenchSingleton.WorkbenchCreated += delegate {
					WorkbenchSingleton.MainForm.Activated += WorkbenchSingleton_MainForm_Activated;
				};
			}
		}

		static void WorkbenchSingleton_MainForm_Activated(object sender, EventArgs e)
		{
			UpdateClipboardContainsText();
		}
		
		static volatile bool clipboardContainsText;
		
		public static bool GetClipboardContainsText()
		{
			WorkbenchSingleton.DebugAssertMainThread();
			if (WorkbenchSingleton.Workbench != null && WorkbenchSingleton.Workbench.IsActiveWindow) {
				UpdateClipboardContainsText();
			}
			return clipboardContainsText;
		}
		
		static WorkerThread workerThread;
		static IAsyncResult currentWorker;
		
		static void UpdateClipboardContainsText()
		{
			if (currentWorker != null && !currentWorker.IsCompleted)
				return;
			if (workerThread == null) {
				workerThread = new WorkerThread();
				Thread t = new Thread(new ThreadStart(workerThread.RunLoop));
				t.SetApartmentState(ApartmentState.STA);
				t.IsBackground = true;
				t.Name = "clipboard access";
				t.Start();
			}
			currentWorker = workerThread.Enqueue(DoUpdate);
			// wait a few ms in case the clipboard can be accessed without problems
			WaitForSingleObject(currentWorker.AsyncWaitHandle.SafeWaitHandle, 50);
			// Using WaitHandle.WaitOne() pumps some Win32 messages.
			// To avoid unintended reentrancy, we need to avoid pumping messages,
			// so we directly call the Win32 WaitForSingleObject function.
			// See SD2-1638 for details.
		}
		
		[DllImport("kernel32", SetLastError=true, ExactSpelling=true)]
		static extern Int32 WaitForSingleObject(SafeWaitHandle handle, Int32 milliseconds);
		
		static void DoUpdate()
		{
			clipboardContainsText = ClipboardWrapper.ContainsText;
		}
	}
}
