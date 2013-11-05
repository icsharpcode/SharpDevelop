// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;

using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.TypeSystem.Implementation;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Parser;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.UnitTesting
{
	public class NUnitTestFramework : ITestFramework
	{
		static readonly ITypeReference testAttributeRef = new GetClassTypeReference("NUnit.Framework", "TestAttribute", 0);
		static readonly ITypeReference testCaseAttributeRef = new GetClassTypeReference("NUnit.Framework", "TestCaseAttribute", 0);
		static readonly ITypeReference testFixtureAttributeRef = new GetClassTypeReference("NUnit.Framework", "TestFixtureAttribute", 0);
		
		/// <summary>
		/// Determines whether the project is a test project. A project
		/// is considered to be a test project if it contains a reference
		/// to the NUnit.Framework assembly.
		/// </summary>
		public bool IsTestProject(IProject project)
		{
			if (project == null)
				return false;
			return testAttributeRef.Resolve(SD.ParserService.GetCompilation(project).TypeResolveContext).Kind != TypeKind.Unknown;
		}
		
		public ITestProject CreateTestProject(ITestSolution parentSolution, IProject project)
		{
			return new NUnitTestProject(project);
		}
		
		public static bool IsTestMethod(IMethod method)
		{
			if (method == null || method.SymbolKind != SymbolKind.Method)
				return false;
			var testAttribute = testAttributeRef.Resolve(method.Compilation);
			var testCaseAttribute = testCaseAttributeRef.Resolve(method.Compilation);
			foreach (var attr in method.Attributes) {
				if (attr.AttributeType.Equals(testAttribute) || attr.AttributeType.Equals(testCaseAttribute))
					return true;
			}
			return false;
		}
		
		public static bool IsTestClass(ITypeDefinition type)
		{
			if (type == null)
				return false;
			if (type.NestedTypes.Any(IsTestClass))
				return true;
			if (type.IsAbstract && !type.IsStatic)
				return false;
			var testFixtureAttribute = testFixtureAttributeRef.Resolve(type.Compilation);
			if (type.Attributes.Any(attr => attr.AttributeType.Equals(testFixtureAttribute)))
				return true;
			return type.Methods.Any(IsTestMethod);
		}
	}
}
