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

namespace ICSharpCode.Profiler.Controls
{
	/// <summary>
	/// Description of ControlsTranslation.
	/// </summary>
	public class ControlsTranslation
	{
		public virtual string WaitBarText {
			get {
				return "Refreshing view, please wait ...";
			}
		}
		
		public virtual string NameColumnText {
			get {
				return "Name";
			}
		}
		
		public virtual string CallCountColumnText {
			get {
				return "Call count";
			}
		}
		
		public virtual string TimeSpentColumnText {
			get {
				return "Time spent";
			}
		}
		
		public virtual string TimeSpentSelfColumnText {
			get {
				return "Time spent (self)";
			}
		}
		
		public virtual string TimeSpentPerCallColumnText {
			get {
				return "Time spent/call";
			}
		}
		
		public virtual string TimeSpentSelfPerCallColumnText {
			get {
				return "Time spent (self)/call";
			}
		}
		
		public virtual string TimePercentageOfParentColumnText {
			get {
				return "% of parent";
			}
		}
		
		public virtual string SearchLabelText {
			get {
				return "Search:";
			}
		}
		
		public virtual string ShowQueryBarText {
			get {
				return "Show query bar";
			}
		}
		
		public virtual string ExecuteQueryText {
			get {
				return "Execute query";
			}
		}
		
		public virtual string ExpandHotPathSubtreeText {
			get {
				return "Expand selected hot path";
			}
		}
		
		public virtual string CpuCyclesText {
			get {
				return "CPU cycles:";
			}
		}
		
		public virtual string TimeText {
			get {
				return "Time:";
			}
		}
		
		public virtual string CallsText {
			get {
				return "Calls:";
			}
		}
	}
}
