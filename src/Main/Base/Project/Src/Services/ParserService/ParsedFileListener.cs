// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.NRefactory.TypeSystem;

namespace ICSharpCode.SharpDevelop.Parser
{
	/// <summary>
	/// Listener for parse info changes.
	/// Caution: The callback is invoked within the parser service's lock. Beware of deadlocks!
	/// The method is called on the thread that performed the parse operation, which might be the main thread
	/// or a background thread.
	/// If possible, use the <see cref="ParserService.ParseInformationUpdated"/> event instead, which is called on the main thread
	/// and after the parser service released its lock.
	/// </summary>
	public delegate void ParsedFileListener(IParsedFile oldFile, IParsedFile newFile);
}
