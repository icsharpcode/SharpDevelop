// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

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
