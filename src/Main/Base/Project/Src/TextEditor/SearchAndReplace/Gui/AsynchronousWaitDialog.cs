// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace SearchAndReplace
{
	/// <summary>
	/// Shows an wait dialog on a separate thread if the action takes longer than 200ms.
	/// Usage:
	/// using (AsynchronousWaitDialog.ShowWaitDialog()) {
	///   long_running_action();
	/// }
	/// </summary>
	public sealed partial class AsynchronousWaitDialog
	{
		class WaitHandle : IDisposable
		{
			bool disposed;
			AsynchronousWaitDialog dlg;
			
			[STAThread]
			public void Run()
			{
				Thread.Sleep(500);
				lock (this) {
					if (disposed)
						return;
					dlg = new AsynchronousWaitDialog();
					dlg.CreateControl();
				}
				Application.Run(dlg);
			}
			
			public void Dispose()
			{
				lock (this) {
					disposed = true;
					if (dlg != null) {
						dlg.BeginInvoke(new MethodInvoker(dlg.Close));
					}
				}
			}
		}
		
		public static IDisposable ShowWaitDialog()
		{
			WaitHandle h = new WaitHandle();
			Thread thread = new Thread(h.Run);
			thread.Name = "AsynchronousWaitDialog thread";
			thread.Start();
			
			Thread.Sleep(0); // allow new thread to start
			return h;
		}
		
		private AsynchronousWaitDialog()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
		}
	}
}
