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
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Forms;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop.Workbench
{
	static class SingleInstanceHelper
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
					SD.MainThread.InvokeAsyncAndForget(delegate {
						var win32Window = PresentationSource.FromVisual(SD.Workbench.MainWindow) as System.Windows.Interop.IWin32Window;
						if (win32Window != null) {
							NativeMethods.SetForegroundWindow(win32Window.Handle);
						}
					});
					string tempFileName = Path.Combine(Path.GetTempPath(), "sd" + fileNumber + ".tmp");
					foreach (string file in File.ReadAllLines(tempFileName)) {
						SD.MainThread.InvokeAsyncAndForget(delegate {
							SharpDevelop.FileService.OpenFile(file);
						});
					}
				} catch (Exception ex) {
					LoggingService.Warn(ex);
				}
				return new IntPtr(RESULT_FILES_HANDLED);
			}
		}
	}
}
