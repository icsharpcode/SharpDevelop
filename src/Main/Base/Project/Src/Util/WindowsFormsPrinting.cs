// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using ICSharpCode.Core;
using System;
using System.Drawing.Printing;
using System.Windows.Forms;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.SharpDevelop.Util
{
	/// <summary>
	/// Allows printing using the IPrintable interface.
	/// </summary>
	public class WindowsFormsPrinting
	{
		public static void Print(IPrintable printable)
		{
			using (PrintDocument pdoc = printable.PrintDocument) {
				if (pdoc != null) {
					using (PrintDialog ppd = new PrintDialog()) {
						ppd.Document  = pdoc;
						ppd.AllowSomePages = true;
						if (ppd.ShowDialog(ICSharpCode.SharpDevelop.Gui.WorkbenchSingleton.MainWin32Window) == DialogResult.OK) { // fixed by Roger Rubin
							pdoc.Print();
						}
					}
				} else {
					MessageService.ShowError("${res:ICSharpCode.SharpDevelop.Commands.Print.CreatePrintDocumentError}");
				}
			}
		}
		
		public static void PrintPreview(IPrintable printable)
		{
			using (PrintDocument pdoc = printable.PrintDocument) {
				if (pdoc != null) {
					PrintPreviewDialog ppd = new PrintPreviewDialog();
					ppd.TopMost   = true;
					ppd.Document  = pdoc;
					ppd.Show(WorkbenchSingleton.MainWin32Window);
				} else {
					MessageService.ShowError("${res:ICSharpCode.SharpDevelop.Commands.Print.CreatePrintDocumentError}");
				}
			}
		}
	}
}
