// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krueger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;

namespace ICSharpCode.SharpDevelop.Project
{
	public delegate void SolutionEventHandler(object sender, SolutionEventArgs e);
	
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
}
