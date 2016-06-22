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
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Utils;

namespace ICSharpCode.SharpDevelop.Project
{
	/// <summary>
	/// Interface for reporting build results in real-time.
	/// </summary>
	/// <remarks>
	/// Implementations of this interface must be thread-safe.
	/// Project-specific build engines use this interface to report results to the main build engine,
	/// and the main build engine uses this interface to report the combined results to the IDE.
	/// </remarks>
	public interface IBuildFeedbackSink
	{
		/// <summary>
		/// Reports an build error/warning/message by adding it to the error list.
		/// This member is thread-safe.
		/// </summary>
		void ReportError(BuildError error);
		
		/// <summary>
		/// Reports a build message.
		/// This member is thread-safe.
		/// </summary>
		void ReportMessage(RichText message);
	}
}
