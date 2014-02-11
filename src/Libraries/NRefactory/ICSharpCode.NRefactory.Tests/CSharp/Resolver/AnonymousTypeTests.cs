// Copyright (c) 2010-2013 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Linq;
using System.Linq.Expressions;
using ICSharpCode.NRefactory.Semantics;
using ICSharpCode.NRefactory.TypeSystem;
using NUnit.Framework;

namespace ICSharpCode.NRefactory.CSharp.Resolver
{
	[TestFixture]
	public class AnonymousTypeTests : ResolverTestBase
	{
		const string programStart = @"using System;
using System.Collections.Generic;
using System.Linq;
class Test {
	void M(IEnumerable<string> list1, IEnumerable<int> list2) {
		";
		const string programEnd = " } }";
		
		[Test]
		public void Zip()
		{
			string program = programStart + "$var$ q = list1.Zip(list2, (a,b) => new { a, b });" + programEnd;
			var rr = Resolve<TypeResolveResult>(program);
			Assert.AreEqual("System.Collections.Generic.IEnumerable", rr.Type.FullName);
			var type = (AnonymousType)((ParameterizedType)rr.Type).TypeArguments.Single();
			Assert.AreEqual(TypeKind.Anonymous, type.Kind);
			Assert.AreEqual(2, type.Properties.Count);
			Assert.AreEqual("a", type.Properties[0].Name);
			Assert.AreEqual("b", type.Properties[1].Name);
			Assert.AreEqual("System.String", type.Properties[0].ReturnType.ReflectionName);
			Assert.AreEqual("System.Int32", type.Properties[1].ReturnType.ReflectionName);
		}
		
		[Test]
		public void ZipItem1()
		{
			string program = programStart + "var q = list1.Zip(list2, (a,b) => new { $Item1 = a$, Item2 = b });" + programEnd;
			var rr = Resolve<MemberResolveResult>(program);
			Assert.AreEqual(TypeKind.Anonymous, rr.Member.DeclaringType.Kind);
			Assert.AreEqual("Item1", rr.Member.Name);
			Assert.AreEqual(SymbolKind.Property, rr.Member.SymbolKind);
			Assert.AreEqual("System.String", rr.Member.ReturnType.FullName);
		}
		
		[Test]
		public void ZipAnonymousType()
		{
			string program = programStart + "var q = list1.Zip(list2, (a,b) => $new { a, b }$);" + programEnd;
			var rr = Resolve<InvocationResolveResult>(program);
			Assert.AreEqual(TypeKind.Anonymous, rr.Type.Kind);
			Assert.AreEqual(SymbolKind.Constructor, rr.Member.SymbolKind);
			Assert.AreEqual(rr.Type, rr.Member.DeclaringType);
			Assert.AreEqual(0, rr.Arguments.Count);
			Assert.AreEqual(2, rr.InitializerStatements.Count);
			var init1 = (OperatorResolveResult)rr.InitializerStatements[0];
			Assert.AreEqual(ExpressionType.Assign, init1.OperatorType);
			Assert.IsInstanceOf<MemberResolveResult>(init1.Operands[0]);
			Assert.IsInstanceOf<LocalResolveResult>(init1.Operands[1]);
			
			ResolveResult target = ((MemberResolveResult)init1.Operands[0]).TargetResult;
			Assert.IsInstanceOf<InitializedObjectResolveResult>(target);
			Assert.AreEqual(rr.Type, target.Type);
		}

		[Test]
		public void NestingAnonymousTypesShouldWork()
		{
			string program = @"using System;
class TestClass {
	void F() {
		var o = $new { a = 0, b = 1, c = new { d = 2, e = 3, f = 4 } }$;
	}
}";
			
			var result = Resolve<InvocationResolveResult>(program);
			Assert.That(result.Type.GetProperties().Select(p => p.Name), Is.EquivalentTo(new[] { "a", "b", "c" }));
			Assert.That(result.Type.GetProperties().Single(p => p.Name == "c").ReturnType.GetProperties().Select(p => p.Name), Is.EquivalentTo(new[] { "d", "e", "f" }));
		}

		[Test]
		public void DeclaringTypeIsSetCorrectlyForMembersOfAnonymousType()
		{
			string program = @"using System;
class TestClass {
	void F() {
		var o = $new { Prop = 0 }$;
	}
}";
			var result = Resolve<InvocationResolveResult>(program);
			var prop = result.Type.GetProperties().Single();
			Assert.That(prop.DeclaringType, Is.EqualTo(result.Type));
			Assert.That(prop.Getter.DeclaringType, Is.EqualTo(result.Type));
			Assert.That(prop.Getter.IsAccessor, Is.True);
			Assert.That(prop.Getter.AccessorOwner, Is.EqualTo(prop));
			Assert.That(prop.Setter, Is.Null);
		}
		
		[Test]
		public void CanRoundtripAnonymousTypeThroughTypeReference()
		{
			string program = @"using System;
class TestClass {
	void F() {
		var o = $new { Prop = 0 }$;
	}
}";
			var result = Resolve<InvocationResolveResult>(program);
			Assert.AreEqual(TypeKind.Anonymous, result.Type.Kind);
			IType typeAfterRoundtrip = result.Type.ToTypeReference().Resolve(result.Member.Compilation);
			Assert.AreEqual(result.Type, typeAfterRoundtrip);
		}
		
		[Test]
		public void CanRoundtripAnonymousTypePropertyThroughMemberReference()
		{
			string program = @"using System;
class TestClass {
	void F() {
		var o = $new { Prop = 0 }$;
	}
}";
			var result = Resolve<InvocationResolveResult>(program);
			IProperty prop = result.Type.GetProperties().Single();
			IProperty propAfterRoundtrip = (IProperty)prop.ToReference().Resolve(result.Member.Compilation.TypeResolveContext);
			Assert.AreEqual(prop, propAfterRoundtrip);
		}
		
		[Test]
		public void CanRoundtripAnonymousTypeGetterThroughMemberReference()
		{
			string program = @"using System;
class TestClass {
	void F() {
		var o = $new { Prop = 0 }$;
	}
}";
			var result = Resolve<InvocationResolveResult>(program);
			IMethod getter = result.Type.GetProperties().Single().Getter;
			IMethod getterAfterRoundtrip = (IMethod)getter.ToReference().Resolve(result.Member.Compilation.TypeResolveContext);
			Assert.AreEqual(getter, getterAfterRoundtrip);
		}

		[Test]
		public void AnonymousTypeToString() {
			string program = @"
class TestClass {
	void F() {
		var s = $new { Prop = 0 }.ToString()$;
	}
}";

			var result = Resolve<CSharpInvocationResolveResult>(program);
			Assert.That(result.IsError, Is.False);
			Assert.That(result.TargetResult.Type.Kind == TypeKind.Anonymous);
			Assert.That(result.Member.Name, Is.EqualTo("ToString"));
			Assert.That(result.Member.DeclaringType.Name, Is.EqualTo("Object"));
		}
	}
}
