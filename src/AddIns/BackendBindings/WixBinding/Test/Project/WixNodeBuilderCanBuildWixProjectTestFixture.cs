// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.WixBinding;
using NUnit.Framework;
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
		[TestFixtureSetUp]
		public void SetUp()
		{
			SD.InitializeForUnitTests();
		}
		
		[Test]
		public void CanBuildWixProject()
		{
			WixProject p = WixBindingTestsHelper.CreateEmptyWixProject();
			WixProjectNodeBuilder builder = new WixProjectNodeBuilder();
			Assert.IsTrue(builder.CanBuildProjectTree(p));
		}
	}
}
