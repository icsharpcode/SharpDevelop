// Copyright (c) 2010 AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under MIT X11 license (for details please see \doc\license.txt)

using System;
using ICSharpCode.NRefactory.TypeSystem.Implementation;
using NUnit.Framework;

namespace ICSharpCode.NRefactory.TypeSystem
{
	[TestFixture]
	public class CecilLoaderTests : TypeSystemTests
	{
		[TestFixtureSetUp]
		public void SetUp()
		{
			testCasePC = CecilLoader.LoadAssemblyFile(typeof(TestCase.SimplePublicClass).Assembly.Location);
		}
	}
}
