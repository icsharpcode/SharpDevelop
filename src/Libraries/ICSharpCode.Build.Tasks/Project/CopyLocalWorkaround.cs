// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace ICSharpCode.Build.Tasks
{
	/// <summary>
	/// When compiling an AnyCPU project targeting .NET 4 without having the SDK/targeting pack installed,
	/// MSBuild incorrectly assumes "Copy local" for a number of assemblies.
	/// </summary>
	public class CopyLocalWorkaround : Task
	{
		public ITaskItem[] Assemblies { get; set; }
		
		[Output]
		public ITaskItem[] OutputAssemblies { get; set; }
		
		public ITaskItem[] AssemblyFiles { get; set; }
		
		[Output]
		public ITaskItem[] OutputAssemblyFiles { get; set; }
		
		public override bool Execute()
		{
			// mark known assemblies as copy local=false
			List<ITaskItem> outputAssemblies = new List<ITaskItem>();
			foreach (ITaskItem item in this.Assemblies) {
				if (string.IsNullOrEmpty(item.GetMetadata("Private"))) {
					string assemblyName = item.ItemSpec;
					foreach (string knownAssembly in KnownFrameworkAssemblies.FullAssemblyNames) {
						if (knownAssembly.StartsWith(assemblyName, StringComparison.OrdinalIgnoreCase)) {
							if (assemblyName.IndexOf(',') >= 0 || knownAssembly.IndexOf(',') == assemblyName.Length) {
								// either matching full name, or matching short name
								item.SetMetadata("Private", "False");
								outputAssemblies.Add(item);
								break;
							}
						}
					}
				}
			}
			this.OutputAssemblies = outputAssemblies.ToArray();
			
			// mark mscorlib as copy local=false
			List<ITaskItem> outputAssemblyFiles = new List<ITaskItem>();
			if (this.AssemblyFiles != null) {
				foreach (ITaskItem item in this.AssemblyFiles) {
					if (string.IsNullOrEmpty(item.GetMetadata("Private"))) {
						if (item.ItemSpec.EndsWith("\\mscorlib.dll", StringComparison.OrdinalIgnoreCase)) {
							item.SetMetadata("Private", "False");
							outputAssemblyFiles.Add(item);
						}
					}
				}
			}
			this.OutputAssemblyFiles = outputAssemblyFiles.ToArray();
			return true;
		}
	}
}
