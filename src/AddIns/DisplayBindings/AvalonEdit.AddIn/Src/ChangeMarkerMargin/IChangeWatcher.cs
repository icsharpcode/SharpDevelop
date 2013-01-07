// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Core;
using ICSharpCode.NRefactory.Editor;

namespace ICSharpCode.AvalonEdit.AddIn
{
	public interface IChangeWatcher : IDisposable
	{
		event EventHandler ChangeOccurred;
		
		/// <summary>
		/// Returns the change information for a given line.
		/// Pass 0 to get the changes before the first line.
		/// </summary>
		LineChangeInfo GetChange(int lineNumber);
		void Initialize(IDocument document, FileName fileName);
		string GetOldVersionFromLine(int lineNumber, out int newStartLine, out bool added);
		bool GetNewVersionFromLine(int lineNumber, out int offset, out int length);
		IDocument CurrentDocument { get; }
		IDocument BaseDocument { get; }
	}
}
