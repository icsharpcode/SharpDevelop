// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.TypeSystem.Implementation;
using ICSharpCode.SharpDevelop.Parser;
using ICSharpCode.SharpDevelop.Project;
using Rhino.Mocks;

namespace UnitTesting.Tests.Utils
{
	public class MockClass
	{
		public static ITypeDefinition CreateMockClassWithoutAnyAttributes()
		{
			var project = MockRepository.GenerateStub<IProject>();
			var assembly = MockMethod.CreateMockAssemblyForProject(project);
			var typeDefinition = MockRepository.GenerateStrictMock<ITypeDefinition>();
			typeDefinition.Stub(td => td.ParentAssembly).Return(assembly);
			typeDefinition.Stub(td => td.Name).Return("TestFixture");
			typeDefinition.Stub(td => td.Namespace).Return("MyTests");
			typeDefinition.Stub(td => td.ReflectionName).Return("MyTests.TestFixture");
			return typeDefinition;
		}
	}
	
	public class MockMethod
	{
		public static IAssembly CreateMockAssemblyForProject(IProject project)
		{
			var assembly = MockRepository.GenerateStrictMock<IAssembly>();
			var compilation = MockRepository.GenerateStrictMock<ICompilation>();
			var solutionSnapshot = MockRepository.GenerateStrictMock<ISolutionSnapshotWithProjectMapping>();
			
			assembly.Stub(a => a.Compilation).Return(compilation);
			compilation.Stub(c => c.SolutionSnapshot).Return(solutionSnapshot);
			solutionSnapshot.Stub(s => s.GetProject(assembly)).Return(project);
			return assembly;
		}
		
		public static IMethod CreateResolvedMethod(string name = "MyMethod")
		{
			var project = MockRepository.GenerateStub<IProject>();
			var assembly = CreateMockAssemblyForProject(project);
			var method = MockRepository.GenerateStrictMock<IMethod>();
			method.Stub(m => m.ParentAssembly).Return(assembly);
			method.Stub(m => m.Name).Return(name);
			method.Stub(m => m.ReflectionName).Return("MyTests.TestFixture." + name);
			return method;
		}
		
		public static IUnresolvedMethod CreateUnresolvedMethod(string name = "MyMethod")
		{
			return new DefaultUnresolvedMethod { Name = name };
		}
	}
}
