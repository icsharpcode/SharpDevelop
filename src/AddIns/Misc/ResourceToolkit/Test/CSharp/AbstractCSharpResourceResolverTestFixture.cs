// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;

using CSharpBinding;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Internal.Templates;
using ICSharpCode.SharpDevelop.Project;
using NUnit.Framework;

namespace ResourceToolkit.Tests.CSharp
{
	public abstract class AbstractCSharpResourceResolverTestFixture : AbstractResourceResolverTestFixture
	{
		protected override string DefaultFileName {
			get { return "a.cs"; }
		}
		
		protected override IProject CreateTestProject()
		{
			ProjectCreateInformation info = new ProjectCreateInformation();
			info.ProjectName = "Test";
			info.RootNamespace = "Test";
			info.OutputProjectFileName = Path.Combine(Path.GetTempPath(), "Test.csproj");
			info.Solution = this.Solution;
			
			CSharpProject p = new CSharpProject(info);
			return p;
		}
	}
}
