// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.UnitTesting
{
	/// <summary>
	/// Represents the root node of a test project.
	/// </summary>
	public interface ITestProject : ITest
	{
		/// <summary>
		/// Gets the SharpDevelop project on which this test project is based.
		/// </summary>
		IProject Project { get; }
		
		/// <summary>
		/// Gets the tests for the specified entity.
		/// Returns an empty list if the entity is not a unit test.
		/// </summary>
		IEnumerable<ITest> GetTestsForEntity(IEntity entity);
		
		/// <summary>
		/// Returns a SharpDevelop <see cref="IBuildable"/> that builds the project
		/// for test execution.
		/// May return null if the project does not require compilation.
		/// </summary>
		IBuildable GetBuildableForTesting();
		
		/// <summary>
		/// Notifies the project that the parse information was changed.
		/// </summary>
		void NotifyParseInformationChanged(IUnresolvedFile oldUnresolvedFile, IUnresolvedFile newUnresolvedFile);
		
		/// <summary>
		/// Runs the specified tests. The specified tests must belong to this project.
		/// </summary>
		ITestRunner CreateTestRunner(TestExecutionOptions options);
		
		void UpdateTestResult(TestResult result);
	}
}
