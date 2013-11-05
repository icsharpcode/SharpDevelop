// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Dom;
using Microsoft.Build.Evaluation;
using Rhino.Mocks;

namespace ICSharpCode.SharpDevelop.Project
{
	public class MockSolution 
	{
		public static ISolution Create()
		{
			ISolution solution = MockRepository.GenerateStub<ISolution>();
			solution.Stub(s => s.MSBuildProjectCollection).Return(new ProjectCollection());
			solution.Stub(s => s.Projects).Return(new SimpleModelCollection<IProject>());
			return solution;
		}
	}
}
