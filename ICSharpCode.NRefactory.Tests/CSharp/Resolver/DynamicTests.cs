using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.NRefactory.Semantics;
using ICSharpCode.NRefactory.TypeSystem;
using NUnit.Framework;

namespace ICSharpCode.NRefactory.CSharp.Resolver {
	[TestFixture]
	public class DynamicTests : ResolverTestBase {
		[Test]
		public void AccessToDynamicMember() {
			string program = @"using System;
class TestClass {
	void F() {
		dynamic obj = null;
		$obj.SomeProperty$ = 10;
	}
}";
			var rr = Resolve<DynamicMemberResolveResult>(program);
			Assert.That(rr.Type.Kind, Is.EqualTo(TypeKind.Dynamic));
			Assert.That(rr.Target is LocalResolveResult && ((LocalResolveResult)rr.Target).Variable.Name == "obj");
			Assert.That(rr.Member, Is.EqualTo("SomeProperty"));
		}

		[Test]
		public void DynamicInvocation() {
			string program = @"using System;
class TestClass {
	void F() {
		dynamic obj = null;
		int a = 0;
		string b = null;
		$obj.SomeMethod(a, b)$;
	}
}";
			var rr = Resolve<DynamicInvocationResolveResult>(program);
			Assert.That(rr.Type.Kind, Is.EqualTo(TypeKind.Dynamic));
			Assert.That(rr.Target, Is.InstanceOf<DynamicMemberResolveResult>());
			var dynamicMember = (DynamicMemberResolveResult)rr.Target;
			Assert.That(dynamicMember.Target is LocalResolveResult && ((LocalResolveResult)dynamicMember.Target).Variable.Name == "obj");
			Assert.That(dynamicMember.Member, Is.EqualTo("SomeMethod"));
			Assert.That(rr.Arguments.Count, Is.EqualTo(2));
			Assert.That(rr.Arguments[0].Name, Is.Null);
			Assert.That(rr.Arguments[0].Value is LocalResolveResult && ((LocalResolveResult)rr.Arguments[0].Value).Variable.Name == "a");
			Assert.That(rr.Arguments[1].Name, Is.Null);
			Assert.That(rr.Arguments[1].Value is LocalResolveResult && ((LocalResolveResult)rr.Arguments[1].Value).Variable.Name == "b");
		}

		[Test]
		public void DynamicInvocationWithNamedArguments() {
			string program = @"using System;
class TestClass {
	void F() {
		dynamic obj = null;
		int a = 0, x = 0;
		string b = null;
		$obj.SomeMethod(x, param1: a, param2: b)$;
	}
}";
			var rr = Resolve<DynamicInvocationResolveResult>(program);
			Assert.That(rr.Type.Kind, Is.EqualTo(TypeKind.Dynamic));
			Assert.That(rr.Target, Is.InstanceOf<DynamicMemberResolveResult>());
			var dynamicMember = (DynamicMemberResolveResult)rr.Target;
			Assert.That(dynamicMember.Target is LocalResolveResult && ((LocalResolveResult)dynamicMember.Target).Variable.Name == "obj");
			Assert.That(dynamicMember.Member, Is.EqualTo("SomeMethod"));
			Assert.That(rr.Arguments.Count, Is.EqualTo(3));
			Assert.That(rr.Arguments[0].Name, Is.Null);
			Assert.That(rr.Arguments[0].Value is LocalResolveResult && ((LocalResolveResult)rr.Arguments[0].Value).Variable.Name == "x");
			Assert.That(rr.Arguments[1].Name, Is.EqualTo("param1"));
			Assert.That(rr.Arguments[1].Value is LocalResolveResult && ((LocalResolveResult)rr.Arguments[1].Value).Variable.Name == "a");
			Assert.That(rr.Arguments[2].Name, Is.EqualTo("param2"));
			Assert.That(rr.Arguments[2].Value is LocalResolveResult && ((LocalResolveResult)rr.Arguments[2].Value).Variable.Name == "b");
		}

		[Test]
		public void TwoDynamicInvocationsInARow() {
			string program = @"using System;
class TestClass {
	void F() {
		dynamic obj = null;
		int a = 0, b = 0;
		$obj.SomeMethod(a)(b)$;
	}
}";
			var rr = Resolve<DynamicInvocationResolveResult>(program);
			Assert.That(rr.Type.Kind, Is.EqualTo(TypeKind.Dynamic));
			Assert.That(rr.Target, Is.InstanceOf<DynamicInvocationResolveResult>());
			var innerInvocation = (DynamicInvocationResolveResult)rr.Target;
			Assert.That(innerInvocation.Target, Is.InstanceOf<DynamicMemberResolveResult>());
			var dynamicMember = (DynamicMemberResolveResult)innerInvocation.Target;
			Assert.That(dynamicMember.Target is LocalResolveResult && ((LocalResolveResult)dynamicMember.Target).Variable.Name == "obj");
			Assert.That(dynamicMember.Member, Is.EqualTo("SomeMethod"));
			Assert.That(innerInvocation.Arguments.Count, Is.EqualTo(1));
			Assert.That(innerInvocation.Arguments[0].Name, Is.Null);
			Assert.That(innerInvocation.Arguments[0].Value is LocalResolveResult && ((LocalResolveResult)innerInvocation.Arguments[0].Value).Variable.Name == "a");
			Assert.That(rr.Arguments.Count, Is.EqualTo(1));
			Assert.That(rr.Arguments[0].Name, Is.Null);
			Assert.That(rr.Arguments[0].Value is LocalResolveResult && ((LocalResolveResult)rr.Arguments[0].Value).Variable.Name == "b");
		}
	}
}
