// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PackageManagement.Scripting;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.PackageManagement
{
	public class PackageManagementWorkbench : IPackageManagementWorkbench
	{
		public void CreateConsolePad()
		{
			PadDescriptor pad = GetConsolePad();
			EnsurePackageManagementConsoleViewModelIsCreated(pad);
		}
		
		PadDescriptor GetConsolePad()
		{
			return SD.Workbench.GetPad(typeof(PackageManagementConsolePad));
		}
		
		void EnsurePackageManagementConsoleViewModelIsCreated(PadDescriptor pad)
		{
			// Force creation of view model.
			object control = pad.PadContent.Control;
		}
		
		public void ShowConsolePad()
		{
			PadDescriptor pad = GetConsolePad();
			pad.BringPadToFront();
		}
		
		public bool InvokeRequired {
			get { return SD.MainThread.InvokeRequired; }
		}
		
		public void SafeThreadAsyncCall<A>(Action<A> method, A arg1)
		{
			SD.MainThread.InvokeAsyncAndForget(() => method(arg1));
		}
		
		public void SafeThreadAsyncCall<A, B>(Action<A, B> method, A arg1, B arg2)
		{
			SD.MainThread.InvokeAsyncAndForget(() => method(arg1, arg2));
		}
		
		public R SafeThreadFunction<R>(Func<R> method)
		{
			return SD.MainThread.InvokeIfRequired(method);
		}
	}
}
