// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
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
