// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using ICSharpCode.Core;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.TypeSystem.Implementation;
using ICSharpCode.NRefactory.Utils;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Parser;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.UnitTesting
{
	/// <summary>
	/// Represents a project that has a reference to a unit testing
	/// framework assembly. Currently only NUnit is supported.
	/// </summary>
	public class TestProject
	{
		IProject project;
		IRegisteredTestFrameworks testFrameworks;
		readonly ObservableCollection<TestClass> testClasses;
		
		public TestProject(IProject project)
		{
			this.project = project;
			this.testFrameworks = TestService.RegisteredTestFrameworks;
			project.ParseInformationUpdated += project_ParseInformationUpdated;
			var compilation = SD.ParserService.GetCompilation(project);
			var classes = project.ProjectContent
				.Resolve(compilation.TypeResolveContext)
				.GetAllTypeDefinitions()
				.Where(td => td.HasTests(compilation))
				.Select(g => new TestClass(testFrameworks, g.ReflectionName, g));
			testClasses = new ObservableCollection<TestClass>(classes);
		}

		void project_ParseInformationUpdated(object sender, ParseInformationEventArgs e)
		{
			var context = new SimpleTypeResolveContext(SD.ParserService.GetCompilation(project).MainAssembly);
			IEnumerable<ITypeDefinition> @new;
			if (e.NewParsedFile != null)
				@new = e.NewParsedFile.TopLevelTypeDefinitions.Select(utd => utd.Resolve(context).GetDefinition()).Where(x => x != null && x.HasTests(SD.ParserService.GetCompilation(project)));
			else
				@new = Enumerable.Empty<ITypeDefinition>();
			testClasses.UpdateTestClasses(testFrameworks, testClasses.Where(tc => tc.Parts.Any(td => td.ParsedFile.FileName == e.OldParsedFile.FileName)).Select(tc => new DefaultResolvedTypeDefinition(context, tc.Parts.ToArray())).ToList(), @new.ToList());
		}
		
		public IProject Project {
			get { return project; }
		}
		
		public ObservableCollection<TestClass> TestClasses {
			get { return testClasses; }
		}
		
		public void UpdateTestResult(TestResult result)
		{
			
		}
	}
}
