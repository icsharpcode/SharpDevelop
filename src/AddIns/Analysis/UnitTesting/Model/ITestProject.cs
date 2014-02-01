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
		/// Gets whether the project needs to be compiled before the tests can be run.
		/// </summary>
		bool IsBuildNeededBeforeTestRun { get; }
		
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
