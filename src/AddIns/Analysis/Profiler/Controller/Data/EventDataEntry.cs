// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
