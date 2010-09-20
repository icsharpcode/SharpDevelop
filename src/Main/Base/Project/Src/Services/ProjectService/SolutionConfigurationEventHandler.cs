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
