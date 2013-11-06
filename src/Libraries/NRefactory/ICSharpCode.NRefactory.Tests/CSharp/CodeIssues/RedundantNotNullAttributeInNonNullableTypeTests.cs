// 
// RedundantNotNullAttributeInNonNullableTypeTests.cs
//
// Author:
//       Luís Reis <luiscubal@gmail.com>
// 
// Copyright (c) 2013 Luís Reis
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using NUnit.Framework;
using ICSharpCode.NRefactory.CSharp.Refactoring;

namespace ICSharpCode.NRefactory.CSharp.CodeIssues
{
	[TestFixture]
	public class RedundantNotNullAttributeTests : InspectionActionTestBase
	{
		[Test]
		public void TestValidReturn() {
			TestWrongContext<RedundantNotNullAttributeInNonNullableTypeIssue>(@"namespace JetBrains.Annotations
{
	[System.AttributeUsage(System.AttributeTargets.Method)]
	class NotNullAttribute : System.Attribute
	{
	}
}
class TestClass
{
	[JetBrains.Annotations.NotNull]
	object NotNull() {
		return 1;
	}
}");
		}

		[Test]
		public void TestRedundantReturn() {
			Test<RedundantNotNullAttributeInNonNullableTypeIssue>(@"namespace JetBrains.Annotations
{
	[System.AttributeUsage(System.AttributeTargets.Method)]
	class NotNullAttribute : System.Attribute
	{
	}
}
class TestClass
{
	[JetBrains.Annotations.NotNull]
	int NotNull() {
		return 1;
	}
}", @"namespace JetBrains.Annotations
{
	[System.AttributeUsage(System.AttributeTargets.Method)]
	class NotNullAttribute : System.Attribute
	{
	}
}
class TestClass
{
	int NotNull() {
		return 1;
	}
}");
		}

		[Test]
		public void TestValidField() {
			TestWrongContext<RedundantNotNullAttributeInNonNullableTypeIssue>(@"namespace JetBrains.Annotations
{
	[System.AttributeUsage(System.AttributeTargets.Field)]
	class NotNullAttribute : System.Attribute
	{
	}
}
class TestClass
{
	[JetBrains.Annotations.NotNull]
	object NotNullField;
}");
		}

		[Test]
		public void TestValidNullable() {
			TestWrongContext<RedundantNotNullAttributeInNonNullableTypeIssue>(@"namespace JetBrains.Annotations
{
	[System.AttributeUsage(System.AttributeTargets.Field)]
	class NotNullAttribute : System.Attribute
	{
	}
}
class TestClass
{
	[JetBrains.Annotations.NotNull]
	int? NotNullField;
}");
		}

		[Test]
		public void TestRedundantField() {
			Test<RedundantNotNullAttributeInNonNullableTypeIssue>(@"namespace JetBrains.Annotations
{
	[System.AttributeUsage(System.AttributeTargets.Field)]
	class NotNullAttribute : System.Attribute
	{
	}
}
class TestClass
{
	[JetBrains.Annotations.NotNull]
	int NotNullField;
}", @"namespace JetBrains.Annotations
{
	[System.AttributeUsage(System.AttributeTargets.Field)]
	class NotNullAttribute : System.Attribute
	{
	}
}
class TestClass
{
	int NotNullField;
}");
		}

		[Test]
		public void TestRedundantFixedField() {
			Test<RedundantNotNullAttributeInNonNullableTypeIssue>(@"namespace JetBrains.Annotations
{
	[System.AttributeUsage(System.AttributeTargets.Field)]
	class NotNullAttribute : System.Attribute
	{
	}
}
unsafe struct TestClass
{
	[JetBrains.Annotations.NotNull]
	fixed object NotNullField[1024];
}", @"namespace JetBrains.Annotations
{
	[System.AttributeUsage(System.AttributeTargets.Field)]
	class NotNullAttribute : System.Attribute
	{
	}
}
unsafe struct TestClass
{
	fixed object NotNullField[1024];
}");
		}

		[Test]
		public void TestRedundantProperty() {
			Test<RedundantNotNullAttributeInNonNullableTypeIssue>(@"namespace JetBrains.Annotations
{
	[System.AttributeUsage(System.AttributeTargets.Property)]
	class NotNullAttribute : System.Attribute
	{
	}
}
class TestClass
{
	[JetBrains.Annotations.NotNull]
	int NotNullField { get; set; }
}", @"namespace JetBrains.Annotations
{
	[System.AttributeUsage(System.AttributeTargets.Property)]
	class NotNullAttribute : System.Attribute
	{
	}
}
class TestClass
{
	int NotNullField { get; set; }
}");
		}

		[Test]
		public void TestRedundantParameter() {
			Test<RedundantNotNullAttributeInNonNullableTypeIssue>(@"namespace JetBrains.Annotations
{
	[System.AttributeUsage(System.AttributeTargets.Parameter)]
	class NotNullAttribute : System.Attribute
	{
	}
}
class TestClass
{
	void Foo([JetBrains.Annotations.NotNull]int x) {}
}", @"namespace JetBrains.Annotations
{
	[System.AttributeUsage(System.AttributeTargets.Parameter)]
	class NotNullAttribute : System.Attribute
	{
	}
}
class TestClass
{
	void Foo(int x) {}
}");
		}
	}
}

