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
using System.Diagnostics;
using System.IO;
using System.Linq;
using ICSharpCode.Core;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.CSharp.Resolver;
using ICSharpCode.NRefactory.CSharp.TypeSystem;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.NRefactory.ConsistencyCheck
{
	// Adapters from SD to NR.ConsistencyCheck solution model
	public class Solution
	{
		public readonly List<CSharpProject> Projects = new List<CSharpProject>();
		
		public Solution(ICSharpCode.SharpDevelop.Project.Solution solution)
		{
			foreach (var p in solution.Projects.OfType<MSBuildBasedProject>()) {
				if (p.FileName.ToString().EndsWith(".csproj", StringComparison.OrdinalIgnoreCase))
					Projects.Add(new CSharpProject(p));
			}
			var snapshot = SD.ParserService.GetCurrentSolutionSnapshot();
			foreach (var p in Projects) {
				p.Compilation = snapshot.GetCompilation(p.IProject);
			}
		}
		
		public IEnumerable<CSharpFile> AllFiles {
			get {
				return Projects.SelectMany(p => p.Files);
			}
		}
	}
	
	public class CSharpProject
	{
		public readonly MSBuildBasedProject IProject;
		public readonly CompilerSettings CompilerSettings = new CompilerSettings();
		public readonly List<CSharpFile> Files = new List<CSharpFile>();
		public ICompilation Compilation;
		
		public CSharpProject(MSBuildBasedProject project)
		{
			this.IProject = project;
			
			CompilerSettings.AllowUnsafeBlocks = GetBoolProperty("AllowUnsafeBlocks") ?? false;
			CompilerSettings.CheckForOverflow = GetBoolProperty("CheckForOverflowUnderflow") ?? false;
			string defineConstants = project.GetEvaluatedProperty("DefineConstants");
			foreach (string symbol in defineConstants.Split(new char[] { ';', ',' }, StringSplitOptions.RemoveEmptyEntries))
				this.CompilerSettings.ConditionalSymbols.Add(symbol.Trim());
			
			// Parse the C# code files
			foreach (var item in project.GetItemsOfType(ItemType.Compile)) {
				var file = new CSharpFile(this, FileName.Create(item.FileName));
				Files.Add(file);
			}
		}
		
		bool? GetBoolProperty(string propertyName)
		{
			string val = IProject.GetEvaluatedProperty(propertyName);
			bool result;
			if (bool.TryParse(val, out result))
				return result;
			else
				return null;
		}
	}
	
	public class CSharpFile
	{
		public readonly CSharpProject Project;
		public readonly string FileName;
		public readonly string OriginalText;
		
		public SyntaxTree SyntaxTree;
		public CSharpUnresolvedFile UnresolvedTypeSystemForFile;
		
		public CSharpFile(CSharpProject project, FileName fileName)
		{
			this.Project = project;
			this.FileName = fileName;
			this.OriginalText = File.ReadAllText(fileName);
			CSharpParser p = new CSharpParser(project.CompilerSettings);
			this.SyntaxTree = p.Parse(this.OriginalText, fileName);
			this.UnresolvedTypeSystemForFile = SD.ParserService.GetExistingUnresolvedFile(fileName, null, project.IProject) as CSharpUnresolvedFile;
			if (this.UnresolvedTypeSystemForFile == null)
				throw new InvalidOperationException("LoadSolutionProjectsThread not yet finished?");
		}
		
		public CSharpAstResolver CreateResolver()
		{
			return new CSharpAstResolver(Project.Compilation, SyntaxTree, UnresolvedTypeSystemForFile);
		}
	}
	
	sealed class Timer : IDisposable
	{
		Stopwatch w = Stopwatch.StartNew();
		
		public Timer(string title)
		{
			Console.Write(title);
		}
		
		public void Dispose()
		{
			Console.WriteLine(w.Elapsed);
		}
	}
}
