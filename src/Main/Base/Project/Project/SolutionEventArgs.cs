// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.SharpDevelop.Project
{
	public class SolutionEventArgs : EventArgs
	{
		readonly ISolution solution;
		
		public ISolution Solution {
			get { return solution; }
		}
		
		public SolutionEventArgs(ISolution solution)
		{
			this.solution = solution;
		}
	}
	
	public class SolutionClosingEventArgs : SolutionEventArgs
	{
		readonly bool allowCancel;
		
		public bool AllowCancel {
			get { return allowCancel; }
		}
		
		public bool Cancel { get; set; }
		
		public SolutionClosingEventArgs(ISolution solution, bool allowCancel)
			: base(solution)
		{
			this.allowCancel = allowCancel;
		}
	}
}
