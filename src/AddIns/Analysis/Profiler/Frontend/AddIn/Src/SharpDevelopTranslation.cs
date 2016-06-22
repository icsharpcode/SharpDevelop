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

using ICSharpCode.Core;
using System;
using ICSharpCode.Profiler.Controls;

namespace ICSharpCode.Profiler.AddIn
{
	/// <summary>
	/// Description of SharpDevelopTranslation.
	/// </summary>
	public class SharpDevelopTranslation : ControlsTranslation
	{
		public override string WaitBarText {
			get { return StringParser.Parse("${res:AddIns.Profiler.ProfilingView.WaitBarText}"); }
		}
		
		public override string NameColumnText {
			get { return StringParser.Parse("${res:AddIns.Profiler.ProfilingView.NameColumnText}"); }
		}
		
		public override string CallCountColumnText {
			get { return StringParser.Parse("${res:AddIns.Profiler.ProfilingView.CallCountColumnText}"); }
		}
		
		public override string CallsText {
			get { return StringParser.Parse("${res:AddIns.Profiler.ProfilingView.CallsText}"); }
		}
		
		public override string CpuCyclesText {
			get { return StringParser.Parse("${res:AddIns.Profiler.ProfilingView.CpuCyclesText}"); }
		}
		
		public override string ExecuteQueryText {
			get { return StringParser.Parse("${res:AddIns.Profiler.ProfilingView.ExecuteQueryText}"); }
		}
		
		public override string ExpandHotPathSubtreeText {
			get { return StringParser.Parse("${res:AddIns.Profiler.ProfilingView.ExpandHotPathSubtreeText}"); }
		}
		
		public override string SearchLabelText {
			get { return StringParser.Parse("${res:AddIns.Profiler.ProfilingView.SearchLabelText}"); }
		}
		
		public override string ShowQueryBarText {
			get { return StringParser.Parse("${res:AddIns.Profiler.ProfilingView.ShowQueryBarText}"); }
		}
		
		public override string TimePercentageOfParentColumnText {
			get { return StringParser.Parse("${res:AddIns.Profiler.ProfilingView.TimePercentageOfParentColumnText}"); }
		}
		
		public override string TimeSpentColumnText {
			get { return StringParser.Parse("${res:AddIns.Profiler.ProfilingView.TimeSpentColumnText}"); }
		}
		
		public override string TimeSpentPerCallColumnText {
			get { return StringParser.Parse("${res:AddIns.Profiler.ProfilingView.TimeSpentPerCallColumnText}"); }
		}
		
		public override string TimeSpentSelfColumnText {
			get { return StringParser.Parse("${res:AddIns.Profiler.ProfilingView.TimeSpentSelfColumnText}"); }
		}
		
		public override string TimeSpentSelfPerCallColumnText {
			get { return StringParser.Parse("${res:AddIns.Profiler.ProfilingView.TimeSpentSelfPerCallColumnText}"); }
		}
		
		public override string TimeText {
			get { return StringParser.Parse("${res:AddIns.Profiler.ProfilingView.TimeText}"); }
		}
	}
}
