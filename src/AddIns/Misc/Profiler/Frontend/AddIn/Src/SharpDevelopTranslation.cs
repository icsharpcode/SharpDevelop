// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="siegfriedpammer@gmail.com"/>
//     <version>$Revision$</version>
// </file>
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
