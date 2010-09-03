// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using ICSharpCode.SharpDevelop.Internal.Templates;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.WixBinding;
using NUnit.Framework;
using System;
using WixBinding.Tests.Utils;

namespace WixBinding.Tests.Project
{
	/// <summary>
	/// Tests that the WixProjectNodeBuilder can build project nodes for a
	/// WixProject.
	/// </summary>
	[TestFixture]
	public class WixNodeBuilderCanBuildWixProjectTestFixture
	{	
		[Test]
		public void CanBuildWixProject()
		{
			WixProject p = WixBindingTestsHelper.CreateEmptyWixProject();
			WixProjectNodeBuilder builder = new WixProjectNodeBuilder();
			Assert.IsTrue(builder.CanBuildProjectTree(p));
		}
	}
}
