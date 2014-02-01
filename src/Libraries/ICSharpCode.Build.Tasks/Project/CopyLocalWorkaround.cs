// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
			if (this.Assemblies != null) {
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
			}
			
			if (this.AssemblyFiles != null) {
				// mark mscorlib as copy local=false
				List<ITaskItem> outputAssemblyFiles = new List<ITaskItem>();
				foreach (ITaskItem item in this.AssemblyFiles) {
					if (string.IsNullOrEmpty(item.GetMetadata("Private"))) {
						if (item.ItemSpec.EndsWith("\\mscorlib.dll", StringComparison.OrdinalIgnoreCase)) {
							item.SetMetadata("Private", "False");
							outputAssemblyFiles.Add(item);
						}
					}
				}
				this.OutputAssemblyFiles = outputAssemblyFiles.ToArray();
			}
			return true;
		}
	}
}
