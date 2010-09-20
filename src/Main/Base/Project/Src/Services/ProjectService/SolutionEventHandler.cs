// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.ComponentModel;

namespace ICSharpCode.SharpDevelop.Project
{
	public class SolutionEventArgs : EventArgs
	{
		Solution solution;
		
		public Solution Solution {
			get {
				return solution;
			}
		}
		
		public SolutionEventArgs(Solution solution)
		{
			this.solution = solution;
		}
	}
	
	public class SolutionCancelEventArgs : CancelEventArgs
	{
		Solution solution;
		
		public Solution Solution {
			get {
				return solution;
			}
		}
		
		public SolutionCancelEventArgs(Solution solution)
		{
			this.solution = solution;
		}
	}
}
