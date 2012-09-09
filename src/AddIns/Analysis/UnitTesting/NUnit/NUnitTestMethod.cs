// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.NRefactory.TypeSystem;

namespace ICSharpCode.UnitTesting
{
	/// <summary>
	/// NUnit test method.
	/// </summary>
	public class NUnitTestMethod : TestBase
	{
		ITestProject parentProject;
		IUnresolvedMethod method;
		
		public NUnitTestMethod(ITestProject parentProject, IUnresolvedMethod method)
		{
			if (parentProject == null)
				throw new ArgumentNullException("parentProject");
			if (method == null)
				throw new ArgumentNullException("method");
			this.parentProject = parentProject;
			this.method = method;
		}
		
		public override ITestProject ParentProject {
			get { return parentProject; }
		}
		
		public override string DisplayName {
			get { return method.Name; }
		}
		
		public string Name {
			get { return method.Name; }
		}
		
		public string FixtureReflectionName {
			get { return method.DeclaringTypeDefinition.ReflectionName; }
		}
		
		public void UpdateTestResult(TestResult result)
		{
			this.Result = result.ResultType;
		}
	}
}
