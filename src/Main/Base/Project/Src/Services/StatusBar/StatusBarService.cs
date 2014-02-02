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
		/// Sets the selection length in the status bar.
		/// </summary>
		/// <param name="length">selection length</param>
		void SetSelectionSingle(int length);
		
		/// <summary>
		/// Sets rect selection size in the status bar.
		/// </summary>
		/// <param name="rows">vertical size of selecion</param>
		/// <param name="cols">horizontal size of selection</param>
		void SetSelectionMulti(int rows, int cols);
		
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
