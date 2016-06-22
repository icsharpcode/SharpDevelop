// 
// CompileTypeScriptFilesOnBuildFileAction.cs
// 
// Author:
//   Matt Ward <ward.matt@gmail.com>
// 
// Copyright (C) 2013 Matthew Ward
// 
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System;
using System.Collections.Generic;
using System.Linq;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.TypeScriptBinding.Hosting;

namespace ICSharpCode.TypeScriptBinding
{
	public class CompileTypeScriptFilesOnBuildAction : CompileTypeScriptAction
	{
		public void CompileFiles(IEnumerable<IProject> projects)
		{
			ClearOutputWindow();
			foreach (TypeScriptProject project in GetTypeScriptProjects(projects)) {
				if (project.CompileOnBuild) {
					CompileFiles(project);
				}
			}
		}
		
		static IEnumerable<TypeScriptProject> GetTypeScriptProjects(IEnumerable<IProject> projects)
		{
			return projects.OfType<MSBuildBasedProject>()
				.Select(project => new TypeScriptProject(project));
		}
		
		void CompileFiles(TypeScriptProject project)
		{
			FileName[] fileNames = project.GetTypeScriptFileNames().ToArray();
			if (fileNames.Length == 0)
				return;
			
			CompileFiles(project, fileNames);
		}
		
		void CompileFiles(TypeScriptProject project, FileName[] fileNames)
		{
			ReportCompileStarting(project);
			
			bool errors = false;
			TypeScriptContext context = TypeScriptService.ContextProvider.GetContext(fileNames.First());
			var compiler = new LanguageServiceCompiler(context);
			
			project.CreateOutputDirectory();
			
			foreach (FileName fileName in fileNames) {
				UpdateFile(context, fileName);
				LanguageServiceCompilerResult result = compiler.Compile(fileName, project);
				
				if (result.HasErrors) {
					errors = true;
					Report(result.GetError());
				}
			}
			ReportCompileFinished(errors);
		}
		
		void ReportCompileStarting(TypeScriptProject project)
		{
			Report("Compiling TypeScript files for project: {0}", project.Name);
		}
	}
}
