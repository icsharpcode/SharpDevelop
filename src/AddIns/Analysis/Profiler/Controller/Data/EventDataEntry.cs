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
using System.Linq;

namespace ICSharpCode.Profiler.Controller.Data
{
	/// <summary>
	/// Represents an entry in the list of events collected by the profiler.
	/// </summary>
	public class EventDataEntry
	{
		/// <summary>
		/// The id of the dataset this entry belongs to.
		/// </summary>
		public int DataSetId { get; set; }
		
		/// <summary>
		/// The type of this event entry.
		/// </summary>
		public EventType Type { get; set; }
		
		/// <summary>
		/// The id of NameMapping of this event entry.
		/// </summary>
		public int NameId { get; set; }
		
		/// <summary>
		/// Additional data collected by the profiler.
		/// </summary>
		public string Data { get; set; }
	}
	
	/// <summary>
	/// Defines kinds of events that can be handled by the profiler.
	/// </summary>
	public enum EventType : int
	{
		/// <summary>
		/// Recorded event was an exception thrown by the profilee.
		/// </summary>
		Exception = 0,
		/// <summary>
		/// Recorded event was a call to a Console.Write*/Read* method.
		/// </summary>
		Console = 1,
		/// <summary>
		/// Recorded event was fired by Windows Forms controls.
		/// </summary>
		WindowsForms = 2,
		/// <summary>
		/// Recorded event was fired by Windows Presentation Foundation controls.
		/// </summary>
		WindowsPresentationFoundation = 3
	}
}
