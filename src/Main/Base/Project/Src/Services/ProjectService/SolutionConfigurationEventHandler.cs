// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krueger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.Core 
{
	public delegate void SolutionConfigurationEventHandler(object sender, SolutionConfigurationEventArgs e);
	
	public class SolutionConfigurationEventArgs : EventArgs
	{ 
		string   configuration;
		Solution solution;
		
		public string Configuration {
			get {
				return configuration;
			}
		}
		public Solution Solution {
			get {
				return solution;
			}
		}
		
		public SolutionConfigurationEventArgs(Solution solution, string configuration)
		{
			this.solution      = solution;
			this.configuration = configuration;
		}
		
	}
}
