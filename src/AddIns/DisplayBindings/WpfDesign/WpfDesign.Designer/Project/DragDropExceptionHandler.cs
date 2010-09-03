// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Threading;

namespace ICSharpCode.WpfDesign.Designer
{
	/// <summary>
	/// When the designer is hosted in a Windows.Forms application, exceptions in
	/// drag'n'drop handlers are silently ignored.
	/// Applications hosting the designer should listen to the event and provide their own exception handling
	/// method. If no event listener is registered, exceptions will call Environment.FailFast.
	/// </summary>
	public static class DragDropExceptionHandler
	{
		/// <summary>
		/// Event that occors when an unhandled exception occurs during drag'n'drop operators.
		/// </summary>
		public static event ThreadExceptionEventHandler UnhandledException;
		
		/// <summary>
		/// Raises the UnhandledException event, or calls Environment.FailFast if no event handlers are present.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate", Justification = "We're raising an event")]
		public static void RaiseUnhandledException(Exception exception)
		{
			if (exception == null)
				throw new ArgumentNullException("exception");
			ThreadExceptionEventHandler eh = UnhandledException;
			if (eh != null)
				eh(null, new ThreadExceptionEventArgs(exception));
			else
				Environment.FailFast(exception.ToString());
		}
	}
}
