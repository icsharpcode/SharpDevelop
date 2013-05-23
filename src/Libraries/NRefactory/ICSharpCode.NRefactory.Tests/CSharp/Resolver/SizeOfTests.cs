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
using ICSharpCode.NRefactory.Semantics;
using NUnit.Framework;
using ICSharpCode.NRefactory.TypeSystem;

namespace ICSharpCode.NRefactory.CSharp.Resolver
{
	[TestFixture]
	public class SizeOfTests : ResolverTestBase
	{
		[Test]
		public void SizeOfPrimitiveTypes()
		{
			foreach (var t in new[] { new { t = "sbyte", n = 1 },
			                          new { t = "byte", n = 1 },
			                          new { t = "short", n = 2 },
			                          new { t = "ushort", n = 2 },
			                          new { t = "int", n = 4 },
			                          new { t = "uint", n = 4 },
			                          new { t = "long", n = 8 },
			                          new { t = "ulong", n = 8 },
			                          new { t = "char", n = 2 },
			                          new { t = "float", n = 4 },
			                          new { t = "double", n = 8 },
			                          new { t = "bool", n = 1 }
			        }) {
				string program = @"using System;
					class TestClass {
						static void Main() {
							public int s = $sizeof(" + t.t + @")$;
						}
					}";
				var rr = Resolve<SizeOfResolveResult>(program);
				Assert.IsTrue(rr.Type.IsKnownType(KnownTypeCode.Int32));
				Assert.IsFalse(rr.IsError);
				Assert.AreEqual(t.n, rr.ConstantValue);
				Assert.IsTrue(Type.GetType(rr.ReferencedType.FullName).IsPrimitive);
			}
		}
		
		[Test]
		public void SizeOfEnum()
		{
			string program = @"
enum TestEnum {}
class TestClass {
	static void Main() {
		int s = $sizeof(TestEnum)$;
	}
}";
			var rr = Resolve<SizeOfResolveResult>(program);
			Assert.IsTrue(rr.Type.IsKnownType(KnownTypeCode.Int32));
			Assert.IsFalse(rr.IsError);
			Assert.AreEqual(4, rr.ConstantValue);
			Assert.AreEqual("TestEnum", rr.ReferencedType.Name);

			program = @"
enum TestEnum2 : short {}
class TestClass {
	static void Main() {
		int s = $sizeof(TestEnum2)$;
	}
}";
			rr = Resolve<SizeOfResolveResult>(program);
			Assert.IsTrue(rr.Type.IsKnownType(KnownTypeCode.Int32));
			Assert.IsFalse(rr.IsError);
			Assert.AreEqual(2, rr.ConstantValue);
			Assert.AreEqual("TestEnum2", rr.ReferencedType.Name);
		}
		
		[Test]
		public void SizeOfStructIsNotAConstant()
		{
			string program = @"
struct MyStruct {}
class TestClass {
	static void Main() {
		int s = $sizeof(MyStruct)$;
	}
}";
			var rr = Resolve<SizeOfResolveResult>(program);
			Assert.IsTrue(rr.Type.IsKnownType(KnownTypeCode.Int32));
			Assert.IsFalse(rr.IsError);
			Assert.IsNull(rr.ConstantValue);
			Assert.AreEqual("MyStruct", rr.ReferencedType.Name);
		}

		[Test]
		public void SizeOfReferenceTypeIsAnError() {
			string program = @"
class MyClass {}
class TestClass {
	static void Main() {
		int s = $sizeof(MyClass)$;
	}
}";
			var rr = Resolve<SizeOfResolveResult>(program);
			Assert.IsTrue(rr.Type.IsKnownType(KnownTypeCode.Int32));
			Assert.IsTrue(rr.IsError);
			Assert.IsNull(rr.ConstantValue);
			Assert.AreEqual("MyClass", rr.ReferencedType.Name);
		}
	}
}
