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
		public static readonly IProjectContent Mscorlib = new CecilLoader().LoadAssemblyFile(typeof(object).Assembly.Location);
		
		[TestFixtureSetUp]
		public void SetUp()
		{
			// use "IncludeInternalMembers" so that Cecil results match C# parser results
			CecilLoader loader = new CecilLoader() { IncludeInternalMembers = true };
			testCasePC = loader.LoadAssemblyFile(typeof(TestCase.SimplePublicClass).Assembly.Location);
		}
	}
}
