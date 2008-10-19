// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.Core.WinForms;
using System;
using System.Threading;
using ICSharpCode.SharpDevelop.Gui;

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
		
		static bool clipboardContainsText;
		
		public static bool GetClipboardContainsText()
		{
			WorkbenchSingleton.DebugAssertMainThread();
			if (WorkbenchSingleton.Workbench != null && WorkbenchSingleton.Workbench.IsActiveWindow) {
				UpdateClipboardContainsText();
			}
			return clipboardContainsText;
		}
		
		static Thread updateThread;
		
		static void UpdateClipboardContainsText()
		{
			if (updateThread != null)
				return;
			Thread t = new Thread(new ThreadStart(DoUpdate));
			t.SetApartmentState(ApartmentState.STA);
			t.IsBackground = true;
			updateThread = t;
			t.Start();
			t.Join(50); // wait a few ms in case the clipboard can be accessed without problems
		}
		
		static void DoUpdate()
		{
			try {
				clipboardContainsText = ClipboardWrapper.ContainsText;
			} finally {
				updateThread = null;
			}
		}
	}
}
