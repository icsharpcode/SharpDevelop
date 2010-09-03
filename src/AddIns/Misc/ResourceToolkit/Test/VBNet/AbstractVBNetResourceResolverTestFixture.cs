// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;

using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Internal.Templates;
using ICSharpCode.SharpDevelop.Project;
using NUnit.Framework;
using ICSharpCode.VBNetBinding;

namespace ResourceToolkit.Tests.VBNet
{
	public abstract class AbstractVBNetResourceResolverTestFixture : AbstractResourceResolverTestFixture
	{
		protected override string DefaultFileName {
			get { return "a.vb"; }
		}
		
		protected override IProject CreateTestProject()
		{
			ProjectCreateInformation info = new ProjectCreateInformation();
			info.ProjectName = "Test";
			info.RootNamespace = "Test";
			info.OutputProjectFileName = Path.Combine(Path.GetTempPath(), "Test.vbproj");
			info.Solution = this.Solution;
			
			VBNetProject p = new VBNetProject(info);
			return p;
		}
	}
}
