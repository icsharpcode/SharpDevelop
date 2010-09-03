// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
