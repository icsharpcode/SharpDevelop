// Copyright (c) AlphaSierraPapa for the SharpDevelop Team
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
using System.IO;
using System.Linq;

using ICSharpCode.NRefactory.Semantics;
using ICSharpCode.NRefactory.TypeSystem;
using NUnit.Framework;

namespace ICSharpCode.NRefactory.CSharp.Resolver
{
	[TestFixture]
	public class MemberLookupTests : ResolverTestBase
	{
		MemberLookup lookup;
		
		public override void SetUp()
		{
			base.SetUp();
			lookup = new MemberLookup(context, null, project);
		}
		
		CSharpParsedFile Parse(string program)
		{
			CompilationUnit cu = new CSharpParser().Parse(new StringReader(program));
			CSharpParsedFile parsedFile = new TypeSystemConvertVisitor(project, "test.cs").Convert(cu);
			project.UpdateProjectContent(null, parsedFile);
			return parsedFile;
		}
		
		[Test]
		public void GroupMethodsByDeclaringType()
		{
			string program = @"
class Base {
	public virtual void Method() {}
}
class Middle : Base {
	public void Method(int p) {}
}
class Derived : Middle {
	public override void Method() {}
}";
			ITypeDefinition derived = Parse(program).TopLevelTypeDefinitions[2];
			var rr = lookup.Lookup(new ResolveResult(derived), "Method", EmptyList<IType>.Instance, true) as MethodGroupResolveResult;
			Assert.AreEqual(2, rr.MethodsGroupedByDeclaringType.Count());
			
			var baseGroup = rr.MethodsGroupedByDeclaringType.ElementAt(0);
			Assert.AreEqual("Base", baseGroup.DeclaringType.ReflectionName);
			Assert.AreEqual(1, baseGroup.Count);
			Assert.AreEqual("Derived.Method", baseGroup[0].FullName);
			
			var middleGroup = rr.MethodsGroupedByDeclaringType.ElementAt(1);
			Assert.AreEqual("Middle", middleGroup.DeclaringType.ReflectionName);
			Assert.AreEqual(1, middleGroup.Count);
			Assert.AreEqual("Middle.Method", middleGroup[0].FullName);
		}
		
		[Test]
		public void MethodInGenericClassOverriddenByConcreteMethod()
		{
			string program = @"
class Base<T> {
	public virtual void Method(T a) {}
}
class Derived : Base<int> {
	public override void Method(int a) {}
	public override void Method(string a) {}
}";
			ITypeDefinition derived = Parse(program).TopLevelTypeDefinitions[1];
			var rr = lookup.Lookup(new ResolveResult(derived), "Method", EmptyList<IType>.Instance, true) as MethodGroupResolveResult;
			Assert.AreEqual(2, rr.MethodsGroupedByDeclaringType.Count());
			
			var baseGroup = rr.MethodsGroupedByDeclaringType.ElementAt(0);
			Assert.AreEqual("Base`1[[System.Int32]]", baseGroup.DeclaringType.ReflectionName);
			Assert.AreEqual(1, baseGroup.Count);
			Assert.AreEqual("Derived.Method", baseGroup[0].FullName);
			Assert.AreEqual("System.Int32", baseGroup[0].Parameters[0].Type.Resolve(context).ReflectionName);
			
			var derivedGroup = rr.MethodsGroupedByDeclaringType.ElementAt(1);
			Assert.AreEqual("Derived", derivedGroup.DeclaringType.ReflectionName);
			Assert.AreEqual(1, derivedGroup.Count);
			Assert.AreEqual("Derived.Method", derivedGroup[0].FullName);
			Assert.AreEqual("System.String", derivedGroup[0].Parameters[0].Type.Resolve(context).ReflectionName);
		}
		
		[Test]
		public void GenericMethod()
		{
			string program = @"
class Base {
	public virtual void Method<T>(T a) {}
}
class Derived : Base {
	public override void Method<S>(S a) {}
}";
			ITypeDefinition derived = Parse(program).TopLevelTypeDefinitions[1];
			var rr = lookup.Lookup(new ResolveResult(derived), "Method", EmptyList<IType>.Instance, true) as MethodGroupResolveResult;
			Assert.AreEqual(1, rr.MethodsGroupedByDeclaringType.Count());
			
			var baseGroup = rr.MethodsGroupedByDeclaringType.ElementAt(0);
			Assert.AreEqual("Base", baseGroup.DeclaringType.ReflectionName);
			Assert.AreEqual(1, baseGroup.Count);
			Assert.AreEqual("Derived.Method", baseGroup[0].FullName);
			Assert.AreEqual("``0", baseGroup[0].Parameters[0].Type.Resolve(context).ReflectionName);
		}
		
		[Test]
		public void TestOuterTemplateParameter()
		{
			string program = @"public class A<T>
{
	public class B
	{
		public T field;
	}
}

public class Foo
{
	public void Bar ()
	{
		A<int>.B baz = new A<int>.B ();
		$baz.field$.ToString ();
	}
}";
			var lrr = Resolve<MemberResolveResult>(program);
			Assert.AreEqual("System.Int32", lrr.Type.FullName);
		}
		
		[Test]
		public void TestOuterTemplateParameterInDerivedClass()
		{
			string program = @"public class A<T>
{
	public class B
	{
		public T field;
	}
}

public class Foo : A<int>.B
{
	public void Bar ()
	{
		$field$.ToString ();
	}
}";
			var lrr = Resolve<MemberResolveResult>(program);
			Assert.AreEqual("System.Int32", lrr.Type.FullName);
		}
		
		[Test]
		public void TestOuterTemplateParameterInDerivedClass2()
		{
			string program = @"public class A<T>
{
	public class B
	{
		public T field;
	}
}

public class Foo : A<int>
{
	public void Bar (B v)
	{
		$v.field$.ToString ();
	}
}";
			var lrr = Resolve<MemberResolveResult>(program);
			Assert.AreEqual("System.Int32", lrr.Type.FullName);
		}
	}
}
