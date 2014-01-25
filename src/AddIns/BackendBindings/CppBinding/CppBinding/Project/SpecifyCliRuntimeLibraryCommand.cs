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
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.CppBinding.Project
{
	public class SpecifyCliRuntimeLibraryCommand : AbstractCommand
	{
		/// <summary>
		/// Adds the runtime library metadata to ClCompile items in project file.
		/// Creates ItemDefinitionGroup sections for Debug and Release configurations.
		/// </summary>
		public override void Run()
		{
			if (!(Owner is MSBuildBasedProject))
				throw new ArgumentException("Project must be an MSBuild based project.");
			MSBuildBasedProject project = (MSBuildBasedProject)Owner;
			
			if ("true".Equals(project.GetEvaluatedProperty("CLRSupport"), StringComparison.OrdinalIgnoreCase))
			{
				SpecifyRuntimeLibrary(project, "Debug", "MultiThreadedDebugDLL");
				SpecifyRuntimeLibrary(project, "Release", "MultiThreadedDLL");
			}
		}
		
		private static void SpecifyRuntimeLibrary(MSBuildBasedProject project, string configuration, string runtime)
		{
			MSBuildItemDefinitionGroup group = new MSBuildItemDefinitionGroup(project, new ConfigurationAndPlatform(configuration, null));
			group.SetElementMetadata("ClCompile", "RuntimeLibrary", runtime);
		}
	}
}
