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
