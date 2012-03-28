// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Text;
using System.Threading;
using ICSharpCode.Core;
using ICSharpCode.NRefactory.Editor;

namespace ICSharpCode.SharpDevelop
{
	/// <summary>
	/// File service.
	/// </summary>
	public interface IFileService
	{
		/// <summary>
		/// Gets the default file encoding.
		/// This property is thread-safe.
		/// </summary>
		Encoding DefaultFileEncoding { get; }
		
		/// <summary>
		/// Gets the content of the specified file.
		/// If the file is currently open in SharpDevelop, retrieves a snapshot
		/// of the editor content.
		/// </summary>
		/// <remarks>
		/// This method is thread-safe. This method involves waiting for the main thread, so using it while
		/// holding a lock can lead to deadlocks.
		/// </remarks>
		ITextSource GetFileContent(FileName fileName);
		
		/// <inheritdoc cref="GetParseableFileContent(FileName)"/>
		ITextSource GetFileContent(string fileName);
		
		/// <summary>
		/// Gets the file content for a file that is currently open.
		/// Returns null if the file is not open.
		/// </summary>
		/// <remarks>
		/// This method is thread-safe. This method involves waiting for the main thread, so using it while
		/// holding a lock can lead to deadlocks.
		/// </remarks>
		ITextSource GetFileContentForOpenFile(FileName fileName);
		
		/// <summary>
		/// Gets the file content from disk, ignoring open files.
		/// </summary>
		/// <remarks>
		/// This method is thread-safe.
		/// </remarks>
		ITextSource GetFileContentFromDisk(FileName fileName, CancellationToken cancellationToken = default(CancellationToken));
	}
}
