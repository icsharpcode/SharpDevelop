// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.SharpDevelop.Project
{
	/// <summary>
	/// Interface for reporting build results in real-time.
	/// Project-specific build engines use this interface to report results to the main build engine.
	/// </summary>
	public interface IBuildFeedbackSink
	{
		/// <summary>
		/// Gets the progress monitor associated with this build.
		/// Does not return null.
		/// This member is thread-safe.
		/// </summary>
		Gui.IProgressMonitor ProgressMonitor { get; }
		
		/// <summary>
		/// Reports an build error by adding it to the error list.
		/// This member is thread-safe.
		/// </summary>
		void ReportError(BuildError error);
		
		/// <summary>
		/// Reports a build message.
		/// This member is thread-safe.
		/// </summary>
		void ReportMessage(string message);
		
		/// <summary>
		/// Notifies the build engine that the build of a project has finished.
		/// You should not call any methods after the Done() call.
		/// This member is thread-safe.
		/// </summary>
		void Done(bool success);
	}
}
