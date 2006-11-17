// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;

namespace ICSharpCode.SharpDevelop.Project
{
	public delegate void BuildCallback(BuildResults results);
	
	/// <summary>
	/// Specifies options when starting a build.
	/// </summary>
	public class BuildOptions
	{
		BuildCallback callback;
		BuildTarget target;
		IDictionary<string, string> additionalProperties;
		
		public BuildOptions()
		{
			this.target = BuildTarget.Build;
		}
		
		public BuildOptions(BuildTarget target, BuildCallback callback)
		{
			this.callback = callback;
			this.target = target;
		}
		
		public BuildOptions(BuildTarget target, BuildCallback callback, IDictionary<string, string> additionalProperties)
		{
			this.callback = callback;
			this.target = target;
			this.additionalProperties = additionalProperties;
		}
		
		public BuildCallback Callback {
			get { return callback; }
		}
		
		public BuildTarget Target {
			get { return target; }
		}
		
		public IDictionary<string, string> AdditionalProperties {
			get { return additionalProperties; }
		}
	}
}
