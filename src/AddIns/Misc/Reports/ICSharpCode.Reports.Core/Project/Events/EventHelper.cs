// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.Reports.Core{
	/// <summary>
	/// A helper class witch is used for raising <see cref="System.Events"></see>
	/// seen at:
	/// http://www.codeproject.com/csharp/thehelpertrinity.asp
	/// developed by :Kent Boogaart
	/// </summary>
	internal sealed class EventHelper{
		private EventHelper(){
		}
		/// <summary>
		/// Raises a generic event.
		/// </summary>
		/// <remarks>
		/// This method raises a generic event, passing in the specified event arguments.
		/// </remarks>
		/// <typeparam name="T">
		/// The event arguments type.
		/// </typeparam>
		/// <param name="handler">
		/// The event to be raised.
		/// </param>
		/// <param name="sender">
		/// The sender of the event.
		/// </param>
		/// <param name="e">
		/// The arguments for the event.
		/// </param>
		
		public static void Raise <T>(EventHandler<T> handler, object sender, T e)
			where T: EventArgs{
				// Copy to a temporary variable to be thread-safe.
			EventHandler<T> temp = handler;
		
			if (temp != null) {
				temp(sender, e);
			}
		}

		/// <summary>
		/// Raises a non-generic event.
		/// </summary>
		/// <remarks>
		/// This method raises the specified non-generic event and passes in <c>EventArgs.Empty</c> as the event arguments.
		/// </remarks>
		/// <param name="handler">
		/// The event to be raised.
		/// </param>
		/// <param name="sender">
		/// The sender of the event.
		/// </param>
//		[SuppressMessage("Microsoft.Design", "CA1030", Justification = "False positive - the Raise method overloads are supposed to raise an event on behalf of a client, not on behalf of its declaring class.")]
//		[DebuggerHidden]
		
		public static void Raise(EventHandler handler, object sender){
			// Copy to a temporary variable to be thread-safe.
			EventHandler temp = handler;
			if (temp != null)
			{
				temp(sender, EventArgs.Empty);
			}
		}
	}
}
