// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop.Gui
{
	public static class SingleInstanceHelper
	{
		const int CUSTOM_MESSAGE = NativeMethods.WM_USER + 2;
		const int RESULT_FILES_HANDLED = 2;
		const int RESULT_PROJECT_IS_OPEN = 3;
		
		public static bool OpenFilesInPreviousInstance(string[] fileList)
		{
			LoggingService.Info("Trying to pass arguments to previous instance...");
			int currentProcessId = Process.GetCurrentProcess().Id;
			string currentFile = Assembly.GetEntryAssembly().Location;
			int number = new Random().Next();
			string fileName = Path.Combine(Path.GetTempPath(), "sd" + number + ".tmp");
			try {
				File.WriteAllLines(fileName, fileList);
				List<IntPtr> alternatives = new List<IntPtr>();
				foreach (Process p in Process.GetProcessesByName("SharpDevelop")) {
					if (p.Id == currentProcessId) continue;
					
					if (FileUtility.IsEqualFileName(currentFile, p.MainModule.FileName)) {
						IntPtr hWnd = p.MainWindowHandle;
						if (hWnd != IntPtr.Zero) {
							long result = NativeMethods.SendMessage(hWnd, CUSTOM_MESSAGE, new IntPtr(number), IntPtr.Zero).ToInt64();
							if (result == RESULT_FILES_HANDLED) {
								return true;
							} else if (result == RESULT_PROJECT_IS_OPEN) {
								alternatives.Add(hWnd);
							}
						}
					}
				}
				foreach (IntPtr hWnd in alternatives) {
					if (NativeMethods.SendMessage(hWnd, CUSTOM_MESSAGE, new IntPtr(number), new IntPtr(1)).ToInt64()== RESULT_FILES_HANDLED) {
						return true;
					}
				}
				return false;
			} finally {
				File.Delete(fileName);
			}
		}
		
		internal static IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
		{
			if (msg != CUSTOM_MESSAGE) {
				return IntPtr.Zero;
			}
			handled = true;
			long fileNumber = wParam.ToInt64();
			long openEvenIfProjectIsOpened = lParam.ToInt64();
			LoggingService.Info("Receiving custom message...");
			if (openEvenIfProjectIsOpened == 0 && ProjectService.OpenSolution != null) {
				return new IntPtr(RESULT_PROJECT_IS_OPEN);
			} else {
				try {
					WorkbenchSingleton.SafeThreadAsyncCall(
						delegate { NativeMethods.SetForegroundWindow(WorkbenchSingleton.MainWin32Window.Handle) ; }
					);
					string tempFileName = Path.Combine(Path.GetTempPath(), "sd" + fileNumber + ".tmp");
					foreach (string file in File.ReadAllLines(tempFileName)) {
						WorkbenchSingleton.SafeThreadAsyncCall(
							delegate(string openFileName) { FileService.OpenFile(openFileName); }
							, file
						);
					}
				} catch (Exception ex) {
					LoggingService.Warn(ex);
				}
				return new IntPtr(RESULT_FILES_HANDLED);
			}
		}
	}
}
