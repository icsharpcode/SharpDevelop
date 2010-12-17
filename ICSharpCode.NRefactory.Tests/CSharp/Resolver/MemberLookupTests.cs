// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under MIT X11 license (for details please see \doc\license.txt)

using System;
using NUnit.Framework;

namespace ICSharpCode.NRefactory.CSharp.Resolver
{
	[TestFixture]
	public class MemberLookupTests : ResolverTestBase
	{
		[Test]
		public void ShortMaxValueTest()
		{
			string program = @"using System;
class TestClass {
	object a = $short.MaxValue$;
}
";
			MemberResolveResult rr = Resolve<MemberResolveResult>(program);
			Assert.AreEqual("System.Int16", rr.Type.FullName);
			Assert.AreEqual("System.Int16.MaxValue", rr.Member.FullName);
			Assert.AreEqual(short.MaxValue, rr.ConstantValue);
		}
	}
}
