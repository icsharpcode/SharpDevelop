// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;

using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.TypeSystem.Implementation;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.UnitTesting
{
	public class NUnitTestFramework : ITestFramework
	{
		public bool IsBuildNeededBeforeTestRun {
			get { return true; }
		}
		
		public ITestRunner CreateTestRunner()
		{
			return new NUnitTestRunner();
		}
		
		public ITestRunner CreateTestDebugger()
		{
			return new NUnitTestDebugger();
		}
		
		static readonly ITypeReference testAttribute = new GetClassTypeReference("NUnit.Framework", "TestAttribute", 0);
		static readonly ITypeReference testCaseAttribute = new GetClassTypeReference("NUnit.Framework", "TestCaseAttribute", 0);
		
		/// <summary>
		/// Determines whether the project is a test project. A project
		/// is considered to be a test project if it contains a reference
		/// to the NUnit.Framework assembly.
		/// </summary>
		public bool IsTestProject(IProject project)
		{
			if (project == null)
				throw new ArgumentNullException("project");
			if (project.ProjectContent == null)
				return false;
			return testAttribute.Resolve(SD.ParserService.GetCompilation(project).TypeResolveContext).Kind != TypeKind.Unknown;
		}
		
		public bool IsTestMethod(IMethod method, ICompilation compilation)
		{
			if (method == null)
				throw new ArgumentNullException("method");
			var testAttribute = NUnitTestFramework.testAttribute.Resolve(compilation.TypeResolveContext);
			return IsTestCase(method, compilation) || method.Attributes.Any(a => a.AttributeType.Equals(testAttribute));
		}
		
		public bool IsTestCase(IMethod method, ICompilation compilation)
		{
			if (method == null)
				throw new ArgumentNullException("method");
			var testCaseAttribute = NUnitTestFramework.testCaseAttribute.Resolve(compilation.TypeResolveContext);
			return method.Attributes.Any(a => a.AttributeType.Equals(testCaseAttribute));
		}
		
		public bool IsTestClass(ITypeDefinition type, ICompilation compilation)
		{
			if (type == null)
				throw new ArgumentNullException("type");
			var testAttribute = NUnitTestFramework.testAttribute.Resolve(compilation.TypeResolveContext);
			var testCaseAttribute = NUnitTestFramework.testCaseAttribute.Resolve(compilation.TypeResolveContext);
			return type.Methods.Any(m => m.Attributes.Any(a => a.AttributeType.Equals(testAttribute) || a.AttributeType.Equals(testCaseAttribute)));
		}
		
		public IEnumerable<IMethod> GetTestMethodsFor(ITypeDefinition typeDefinition)
		{
			throw new NotImplementedException();
		}
	}
}
