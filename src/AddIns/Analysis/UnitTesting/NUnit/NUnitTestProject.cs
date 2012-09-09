// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.TypeSystem.Implementation;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.UnitTesting
{
	/// <summary>
	/// NUnit test project.
	/// </summary>
	public class NUnitTestProject : TestProjectBase
	{
		public NUnitTestProject(IProject project) : base(project)
		{
		}
		
		public override Task RunTestsAsync(IEnumerable<ITest> tests, TestExecutionOptions options, IProgressMonitor progressMonitor)
		{
			throw new NotImplementedException();
		}
		
		public override void UpdateTestClass(ITest test, ITypeDefinition typeDefinition)
		{
			((NUnitTestClass)test).UpdateTestClass(typeDefinition);
		}
		
		public override bool IsTestClass(ITypeDefinition typeDefinition)
		{
			return NUnitTestFramework.IsTestClass(typeDefinition);
		}
		
		public override ITest GetTestForEntity(IEntity entity)
		{
			if (entity.DeclaringTypeDefinition != null) {
				ITest testClass = GetTestForEntity(entity.DeclaringTypeDefinition);
				throw new NotImplementedException();
			} else if (entity is ITypeDefinition) {
				// top-level type definition
				ITypeDefinition typeDef = (ITypeDefinition)entity;
				return GetTestClass(new FullNameAndTypeParameterCount(typeDef.Namespace, typeDef.Name, typeDef.TypeParameterCount));
			} else {
				return null;
			}
		}
		
		public override ITest CreateTestClass(ITypeDefinition typeDefinition)
		{
			if (NUnitTestFramework.IsTestClass(typeDefinition))
				return new NUnitTestClass(this, typeDefinition);
			else
				return null;
		}
	}
}
