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

using Hornung.ResourceToolkit;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Project;
using NUnit.Framework;

namespace ResourceToolkit.Tests
{
	public abstract class AbstractTestProjectTestFixture : IDisposable
	{
		Solution solution;
		IProject project;
		DefaultProjectContent defaultPC;
		
		[SetUp]
		public void SetUp()
		{
			this.DoSetUp();
		}
		
		[TearDown]
		public void TearDown()
		{
			this.DoTearDown();
		}
		
		public void Dispose()
		{
			this.Dispose(true);
		}
		
		protected virtual void Dispose(bool disposing)
		{
			if (disposing) {
				this.DoTearDown();
			}
		}
		
		protected virtual void DoSetUp()
		{
			TestHelper.InitializeParsers();
			
			this.solution = new Solution(new MockProjectChangeWatcher());
			this.project = this.CreateTestProject();
			ProjectService.CurrentProject = this.project;
			
			DefaultProjectContent pc = this.CreateNewProjectContent(this.project);
			HostCallback.GetCurrentProjectContent = delegate {
				return pc;
			};
			ResourceResolverService.SetProjectContentUnitTestOnly(this.project, pc);
			this.defaultPC = pc;
		}
		
		protected virtual void DoTearDown()
		{
			if (this.defaultPC != null) {
				this.defaultPC.Dispose();
				this.defaultPC = null;
			}
			if (this.project != null) {
				this.project.Dispose();
				this.project = null;
			}
			if (this.solution != null) {
				this.solution.Dispose();
				this.solution = null;
			}
		}
		
		protected abstract IProject CreateTestProject();
		
		protected IProject Project {
			get { return this.project; }
		}
		
		protected DefaultProjectContent DefaultProjectContent {
			get { return this.defaultPC; }
		}
		
		protected Solution Solution {
			get { return this.solution; }
		}
		
		protected virtual DefaultProjectContent CreateNewProjectContent(IProject project)
		{
			DefaultProjectContent pc = new DefaultProjectContent();
			
			pc.ReferencedContents.Add(AssemblyParserService.DefaultProjectContentRegistry.Mscorlib);
			pc.ReferencedContents.Add(AssemblyParserService.DefaultProjectContentRegistry.GetProjectContentForReference("System", typeof(Uri).Module.FullyQualifiedName));
			
			if (project != null) {
				if (project.LanguageProperties != null) {
					pc.Language = project.LanguageProperties;
				}
			}
			
			return pc;
		}
	}
}
