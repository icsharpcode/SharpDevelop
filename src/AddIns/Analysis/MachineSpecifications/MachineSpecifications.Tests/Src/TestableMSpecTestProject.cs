// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.UnitTesting;

namespace ICSharpCode.MachineSpecifications.Tests
{
	public class TestableMSpecTestProject : MSpecTestProject
	{
		public TestableMSpecTestProject(ITestSolution parentSolution, IProject project)
			: base(parentSolution, project)
		{
		}
		
		public bool CallIsTestClass(ITypeDefinition typeDefinition)
		{
			return base.IsTestClass(typeDefinition);
		}
	}
}
