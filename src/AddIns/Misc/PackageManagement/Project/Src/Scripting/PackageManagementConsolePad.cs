// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Windows.Threading;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.PackageManagement.Scripting
{
	public class PackageManagementConsolePad : AbstractPadContent
	{
		PackageManagementConsoleView view;
		PackageManagementConsoleViewModel viewModel;
		
		public override object Control {
			get {
				if (view == null) {
					view = new PackageManagementConsoleView();
					viewModel = view.DataContext as PackageManagementConsoleViewModel;
				}
				return view;
			}
		}
		
		public override void Dispose()
		{
			if (viewModel != null) {
				while (!viewModel.ShutdownConsole()) {
					DoEvents();
				}
				viewModel = null;
			}
		}
		
		/// <summary>
		/// Allow package management console thread to finish. Can be busy if solution was open when the user
		/// closed SharpDevelop.
		/// </summary>
		void DoEvents()
		{
			DispatcherFrame frame = new DispatcherFrame();
			Dispatcher.CurrentDispatcher.BeginInvoke(
				DispatcherPriority.Background,
				new DispatcherOperationCallback(ExitFrame),
				frame);
			Dispatcher.PushFrame(frame);
		}
		
		object ExitFrame(object frame)
		{
			var dispatcherFrame = frame as DispatcherFrame;
			dispatcherFrame.Continue = false;
			return null;
		}
	}
}
