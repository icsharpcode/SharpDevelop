// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.UnitTesting
{
	/// <summary>
	/// A test that represents the whole solution.
	/// </summary>
	public interface ITestSolution : ITest
	{
		/// <summary>
		/// Gets the test project for the specified project.
		/// Returns null if the project is not a test project.
		/// </summary>
		ITestProject GetTestProject(IProject project);
		
		/// <summary>
		/// Gets the tests for the specified entity.
		/// Returns an empty collection if the entity is not a unit test.
		/// </summary>
		IEnumerable<ITest> GetTestsForEntity(IEntity entity);
	}
}
