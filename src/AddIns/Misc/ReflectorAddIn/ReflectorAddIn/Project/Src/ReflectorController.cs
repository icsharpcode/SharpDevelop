// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Christian Hornung" email="chhornung@googlemail.com"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.SharpDevelop.Dom;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.Remoting;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;

namespace ReflectorAddIn
{
	/// <summary>
	/// Controls .NET Reflector.
	/// </summary>
	public static class ReflectorController
	{
		#region Connecting
		
		public static IntPtr FindReflector()
		{
			IntPtr result = IntPtr.Zero;
			NativeMethods.EnumWindows(
				(hWnd, lParam) => {
					if (NativeMethods.GetWindowText(hWnd, 100).StartsWith("Red Gate's .NET Reflector", StringComparison.Ordinal)) {
						result = hWnd;
						return false; // stop enumeration
					} else {
						return true; // continue enumeration
					}
				}, IntPtr.Zero);
			return result;
		}
		
		/// <summary>
		/// Ensures that an instance of Reflector is running and connects to it.
		/// </summary>
		/// <returns>The <see cref="IReflectorService"/> that can be used to control the running instance of Reflector.</returns>
		public static IntPtr FindOrStartReflector()
		{
			IntPtr hwnd = FindReflector();
			if (hwnd != IntPtr.Zero) return hwnd;
			
			// Get Reflector path and set it up
			string reflectorExeFullPath = WorkbenchSingleton.SafeThreadFunction<string>(ReflectorSetupHelper.GetReflectorExeFullPathInteractive);
			if (reflectorExeFullPath == null)
				return IntPtr.Zero;
			
			// start Reflector
			ProcessStartInfo psi = new ProcessStartInfo(reflectorExeFullPath);
			psi.WorkingDirectory = Path.GetDirectoryName(psi.FileName);
			using (Process p = Process.Start(psi)) {
				try {
					p.WaitForInputIdle(10000);
				} catch (InvalidOperationException) {
					// can happen if Reflector is configured to run elevated
				}
			}
			
			for (int retryCount = 0; retryCount < 10; retryCount++) {
				hwnd = FindReflector();
				if (hwnd != IntPtr.Zero) {
					return hwnd;
				}
				Thread.Sleep(500);
			}
			MessageService.ShowError("${res:ReflectorAddIn.ReflectorRemotingFailed}");
			return IntPtr.Zero;
		}
		
		#endregion
		
		// ********************************************************************************************************************************
		
		public static void TryGoTo(string assemblyName, AbstractEntity entity)
		{
			string dotnetName = entity.DocumentationTag.Replace('+', '.');
			string selectCommand;
			if (entity is IClass) {
				selectCommand = "SelectTypeDeclaration";
			} else if (entity is IEvent) {
				selectCommand = "SelectEventDeclaration";
			} else if (entity is IProperty) {
				selectCommand = "SelectPropertyDeclaration";
			} else if (entity is IField) {
				selectCommand = "SelectFieldDeclaration";
			} else {
				selectCommand = "SelectMethodDeclaration";
			}
			ThreadPool.QueueUserWorkItem(state => DoTryGoto(assemblyName, selectCommand, dotnetName));
		}
		
		static void DoTryGoto(string assemblyName, string selectCommand, string dotnetName)
		{
			IntPtr hwnd = FindOrStartReflector();
			if (hwnd != IntPtr.Zero) {
				NativeMethods.SetForegroundWindow(hwnd);
				if (Send(hwnd, "LoadAssembly\n" + assemblyName)) {
					Send(hwnd, selectCommand + "\n" + dotnetName);
				}
			}
		}
		
		unsafe static bool Send(IntPtr hWnd, string message)
		{
			CopyDataStruct lParam = new CopyDataStruct();
			lParam.Padding = IntPtr.Zero;
			lParam.Size = message.Length * 2;
			fixed (char *buffer = message) {
				lParam.Buffer = new IntPtr(buffer);
				return NativeMethods.SendMessage(hWnd, 0x4a, IntPtr.Zero, ref lParam) != IntPtr.Zero;
			}
		}
	}
}
