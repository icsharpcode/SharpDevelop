// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="siegfriedpammer@gmail.com"/>
//     <version>$Revision$</version>
// </file>
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
