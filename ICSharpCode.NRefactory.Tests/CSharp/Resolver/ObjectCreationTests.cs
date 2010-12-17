// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under MIT X11 license (for details please see \doc\license.txt)

using System;
using ICSharpCode.NRefactory.TypeSystem;
using NUnit.Framework;

namespace ICSharpCode.NRefactory.CSharp.Resolver
{
	[TestFixture]
	public class ObjectCreationTests : ResolverTestBase
	{
		[Test, Ignore("Resolving type references is not implemented")]
		public void GenericObjectCreation()
		{
			string program = @"using System.Collections.Generic;
class A {
	static void Main() {
		var a = $new List<string>()$;
	}
}
";
			MemberResolveResult result = Resolve<MemberResolveResult>(program);
			Assert.AreEqual("System.Collections.Generic.List.#ctor", result.Member.FullName);
			
			Assert.AreEqual("System.Collections.Generic.List`1{System.String}", result.Type.ReflectionName);
		}
		
		[Test]
		public void NonExistingClass()
		{
			string program = @"class A {
	void Method() {
		var a = $new ThisClassDoesNotExist()$;
	}
}
";
			ResolveResult result = Resolve<ResolveResult>(program);
			Assert.AreSame(SharedTypes.UnknownType, result.Type);
		}
		
		[Test]
		public void NonExistingClassTypeName()
		{
			string program = @"class A {
	void Method() {
		var a = new $ThisClassDoesNotExist$();
	}
}
";
			UnknownIdentifierResolveResult result = Resolve<UnknownIdentifierResolveResult>(program);
			Assert.AreEqual("ThisClassDoesNotExist", result.Identifier);
			Assert.AreSame(SharedTypes.UnknownType, result.Type);
		}
	}
}
