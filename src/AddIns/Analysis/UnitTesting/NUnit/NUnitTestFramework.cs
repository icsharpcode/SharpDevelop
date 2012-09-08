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
				throw new ArgumentNullException("project");
			if (project.ProjectContent == null)
				return false;
			return testAttributeRef.Resolve(SD.ParserService.GetCompilation(project).TypeResolveContext).Kind != TypeKind.Unknown;
		}
		
		public bool IsTestMember(IMember member)
		{
			if (member == null)
				throw new ArgumentNullException("member");
			if (member.EntityType != EntityType.Method)
				return false;
			var testAttribute = testAttributeRef.Resolve(member.Compilation);
			var testCaseAttribute = testCaseAttributeRef.Resolve(member.Compilation);
			foreach (var attr in member.Attributes) {
				if (attr.AttributeType.Equals(testAttribute) || attr.AttributeType.Equals(testCaseAttribute))
					return true;
			}
			return false;
		}
		
		public bool IsTestClass(ITypeDefinition type)
		{
			if (type == null)
				throw new ArgumentNullException("type");
			if (type.IsAbstract)
				return false;
			var testFixtureAttribute = testFixtureAttributeRef.Resolve(type.Compilation);
			if (type.Attributes.Any(attr => attr.AttributeType.Equals(testFixtureAttributeRef)))
				return true;
			else
				return type.Methods.Any(IsTestMember);
		}
		
		public IEnumerable<TestMember> GetTestMembersFor(TestProject project, ITypeDefinition typeDefinition)
		{
			return typeDefinition.Methods.Where(IsTestMember).Select(m => new TestMember(project, m.UnresolvedMember));
		}
	}
}
