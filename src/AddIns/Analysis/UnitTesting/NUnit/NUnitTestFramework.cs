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
