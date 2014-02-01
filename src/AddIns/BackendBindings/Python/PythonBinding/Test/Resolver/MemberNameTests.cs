// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
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
using ICSharpCode.PythonBinding;
using NUnit.Framework;

namespace PythonBinding.Tests.Resolver
{
	[TestFixture]
	public class MemberNameTests
	{
		[Test]
		public void MemberNameIsEqualReturnsTrueWhenNameAndTypeAreSame()
		{
			MemberName lhs = new MemberName("type", "member");
			MemberName rhs = new MemberName("type", "member");
			
			Assert.IsTrue(lhs.Equals(rhs));
		}
		
		[Test]
		public void MemberNameIsEqualsReturnsFalseWhenMemberNameIsNull()
		{
			MemberName lhs = new MemberName("type", "Member");
			Assert.IsFalse(lhs.Equals(null));
		}
		
		[Test]
		public void MemberNamePropertyReturnsMemberName()
		{
			MemberName methodName = new MemberName("type", "method");
			Assert.AreEqual("method", methodName.Name);
		}
		
		[Test]
		public void MemberNameTypePropertyReturnsType()
		{
			MemberName methodName = new MemberName("type", "method");
			Assert.AreEqual("type", methodName.Type);
		}
		
		[Test]
		public void MemberNameIsEqualReturnsFalseWhenMemberNameIsDifferent()
		{
			MemberName lhs = new MemberName("type", "method1");
			MemberName rhs = new MemberName("type", "method2");
			
			Assert.IsFalse(lhs.Equals(rhs));
		}
		
		[Test]
		public void MemberNameIsEqualReturnsFalseWhenTypeNameIsDifferent()
		{
			MemberName lhs = new MemberName("type1", "method");
			MemberName rhs = new MemberName("type2", "method");
			
			Assert.IsFalse(lhs.Equals(rhs));
		}
		
		[Test]
		public void MemberNameToStringShowsTypeNameAndMemberName()
		{
			MemberName methodName = new MemberName("type", "method");
			string expectedText = "Type: type, Member: method";
			Assert.AreEqual(expectedText, methodName.ToString());
		}
		
		[Test]
		public void CreateMemberNameWithNullStringReturnsMemberNameWithEmptyTypeAndMemberName()
		{
			MemberName methodName = new MemberName(null);
			MemberName expectedMemberName = new MemberName(String.Empty, String.Empty);
			Assert.AreEqual(expectedMemberName, methodName);
		}
		
		[Test]
		public void CreateMemberNameWithEmptyStringReturnsMemberNameWithEmptyTypeAndMemberName()
		{
			MemberName methodName = new MemberName(String.Empty);
			MemberName expectedMemberName = new MemberName(String.Empty, String.Empty);
			Assert.AreEqual(expectedMemberName, methodName);
		}
		
		[Test]
		public void CreateMemberNameWithSystemDotConsoleDotWriteLineReturnsMemberNameWriteLineAndTypeNameSystemDotConsole()
		{
			MemberName methodName = new MemberName("System.Console.WriteLine");
			MemberName expectedMemberName = new MemberName("System.Console", "WriteLine");
			
			Assert.AreEqual(expectedMemberName, methodName);
		}
		
		[Test]
		public void CreateMemberNameWithExpressionWithoutDotCharReturnsMemberNameOfEmptyStringAndExpressionAsTypeName()
		{
			MemberName methodName = new MemberName("test");
			MemberName expectedMemberName = new MemberName("test", String.Empty);
			Assert.AreEqual(expectedMemberName, methodName);
		}
		
		[Test]
		public void HasNameReturnsFalseForMemberNameWithEmptyStringForMemberName()
		{
			MemberName memberName = new MemberName("System", String.Empty);
			Assert.IsFalse(memberName.HasName);
		}
		
		[Test]
		public void HasNameReturnsTrueForMemberNameWithNonEmptyStringForMemberName()
		{
			MemberName memberName = new MemberName("System", "Console");
			Assert.IsTrue(memberName.HasName);
		}
	}
}
