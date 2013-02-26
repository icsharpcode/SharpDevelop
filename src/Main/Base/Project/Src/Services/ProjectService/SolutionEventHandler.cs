// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.ComponentModel;

namespace ICSharpCode.SharpDevelop.Project
{
	public class SolutionEventArgs : EventArgs
	{
		ISolution solution;
		
		public ISolution Solution {
			get {
				return solution;
			}
		}
		
		public SolutionEventArgs(ISolution solution)
		{
			this.solution = solution;
		}
	}
	
	public class SolutionCancelEventArgs : CancelEventArgs
	{
		ISolution solution;
		
		public ISolution Solution {
			get {
				return solution;
			}
		}
		
		public SolutionCancelEventArgs(ISolution solution)
		{
			this.solution = solution;
		}
	}
}
