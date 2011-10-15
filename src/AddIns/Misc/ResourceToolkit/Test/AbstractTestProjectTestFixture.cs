// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
