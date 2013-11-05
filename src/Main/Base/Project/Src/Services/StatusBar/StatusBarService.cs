// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Windows;

using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Gui
{
	[SDService("SD.StatusBar")]
	public interface IStatusBarService
	{
		//bool Visible { get; set; }
		
		/// <summary>
		/// Sets the caret position shown in the status bar.
		/// </summary>
		/// <param name="x">column number</param>
		/// <param name="y">line number</param>
		/// <param name="charOffset">character number</param>
		void SetCaretPosition(int x, int y, int charOffset);
		//void SetInsertMode(bool insertMode);
		
		/// <summary>
		/// Sets the message shown in the left-most pane in the status bar.
		/// </summary>
		/// <param name="message">The message text.</param>
		/// <param name="highlighted">Whether to highlight the text</param>
		/// <param name="icon">Icon to show next to the text</param>
		void SetMessage(string message, bool highlighted = false, IImage icon = null);
		
		/// <summary>
		/// Creates a new <see cref="IProgressMonitor"/> that can be used to report
		/// progress to the status bar.
		/// </summary>
		/// <param name="cancellationToken">Cancellation token to use for
		/// <see cref="IProgressMonitor.CancellationToken"/></param>
		/// <returns>The new IProgressMonitor instance. This return value must be disposed
		/// once the background task has completed.</returns>
		IProgressMonitor CreateProgressMonitor(CancellationToken cancellationToken = default(CancellationToken));
		
		/// <summary>
		/// Shows progress for the specified ProgressCollector in the status bar.
		/// </summary>
		void AddProgress(ProgressCollector progress);
	}
}
