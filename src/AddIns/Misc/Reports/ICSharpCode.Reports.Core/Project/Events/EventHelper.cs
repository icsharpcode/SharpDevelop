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
