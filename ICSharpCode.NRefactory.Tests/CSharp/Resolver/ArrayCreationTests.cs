// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under MIT license (for details please see \doc\license.txt)

using System;
using ICSharpCode.NRefactory.TypeSystem;
using NUnit.Framework;

namespace ICSharpCode.NRefactory.CSharp.Resolver
{
	[TestFixture]
	[Ignore("Parser produces incorrect positions")]
	public class ArrayCreationTests : ResolverTestBase
	{
		[Test]
		public void SimpleArrayCreation()
		{
			string program = @"using System.Collections.Generic;
class A {
	static void Main() {
		var a = $new int[] { 42 }$;
	}
}
";
			var result = Resolve(program);
			Assert.AreEqual("System.Int32[]", result.Type.ReflectionName);
		}
		
		[Test]
		public void NestedArrayCreation()
		{
			string program = @"using System.Collections.Generic;
class A {
	static void Main() {
		var a = $new int[2][,][,,]$;
	}
}
";
			var result = Resolve(program);
			// a one-dimensional array of two-dimensional arrays of three-dimensional arrays
			ArrayType a1 = (ArrayType)result.Type;
			Assert.AreEqual(1, a1.Dimensions);
			ArrayType a2 = (ArrayType)a1.ElementType;
			Assert.AreEqual(2, a2.Dimensions);
			ArrayType a3 = (ArrayType)a2.ElementType;
			Assert.AreEqual(3, a3.Dimensions);
			Assert.AreEqual("System.Int32", a3.ElementType.ReflectionName);
		}
		
		[Test]
		public void InferredType()
		{
			string program = @"using System.Collections.Generic;
class A {
	static void Main() {
		var a = $new [] { 1, 1L }$;
	}
}
";
			var result = Resolve(program);
			Assert.AreEqual("System.Int64[]", result.Type.ReflectionName);
		}
		
		[Test]
		public void InferredType2D()
		{
			string program = @"using System.Collections.Generic;
class A {
	static void Main() {
		var a = $new [,] { { 1 }, { 1L } }$;
	}
}
";
			var result = Resolve(program);
			Assert.AreEqual("System.Int64[,]", result.Type.ReflectionName);
		}
		
		[Test]
		public void ArrayInitializerExpression()
		{
			string program = @"using System.Collections.Generic;
class A {
	int[] a = ${ 1 }$;
}
";
			var result = Resolve(program);
			Assert.AreEqual("System.Int32[]", result.Type.ReflectionName);
		}
	}
}
