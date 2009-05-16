// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

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
