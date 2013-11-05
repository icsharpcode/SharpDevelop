// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop 
{
	public delegate void SolutionConfigurationEventHandler(object sender, SolutionConfigurationEventArgs e);
	
	public class SolutionConfigurationEventArgs : EventArgs
	{ 
		string   configuration;
		ISolution solution;
		
		public string Configuration {
			get {
				return configuration;
			}
		}
		public ISolution Solution {
			get {
				return solution;
			}
		}
		
		public SolutionConfigurationEventArgs(ISolution solution, string configuration)
		{
			this.solution      = solution;
			this.configuration = configuration;
		}
		
	}
}
