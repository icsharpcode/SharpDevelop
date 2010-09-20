// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using NUnit.Framework;

namespace NRefactoryToBooConverter.Tests
{
	[TestFixture]
	public class GlobalTests : TestHelper
	{
		[Test]
		public void EmptyFile()
		{
			Test("", "");
		}
		
		[Test]
		public void EmptyNamespace()
		{
			Test("namespace A.B {}", "namespace A.B");
		}
		
		[Test]
		public void SimpleUsing()
		{
			Test("using System.Windows.Forms;", "import System.Windows.Forms");
		}
		
		[Test]
		public void AliasUsing()
		{
			Test("using WinForms = System.Windows.Forms;", "import System.Windows.Forms as WinForms");
		}
		
		[Test]
		public void UsingOutsideNamespace()
		{
			Test("using System.Windows.Forms;\nnamespace A.B { } ", "namespace A.B\nimport System.Windows.Forms");
		}
		
		[Test]
		public void UsingInsideNamespace()
		{
			Test("namespace A.B { using System.Windows.Forms; } ", "namespace A.B\nimport System.Windows.Forms");
		}
		
		[Test]
		public void ClassDeclaration()
		{
			Test("public class Test {}", "public class Test:\n\tpass");
		}
		
		[Test]
		public void ClassDeclarationWithAttribute()
		{
			Test("[TestFixture] class Test {}", "[TestFixture]\ninternal class Test:\n\tpass");
		}
		
		[Test]
		public void ClassDeclarationWithMultipleAttributes()
		{
			Test("[TestFixture] [OtherA] class Test {}", "[TestFixture]\n[OtherA]\ninternal class Test:\n\tpass");
		}
		
		[Test]
		public void ClassDeclarationWithBaseType()
		{
			Test("public class TestException : Exception {}", "public class TestException(Exception):\n\tpass");
		}
		
		[Test]
		public void ClassDeclarationWithMultipleBaseTypes()
		{
			Test("public class TestException : Exception, IDisposable {}", "public class TestException(Exception, IDisposable):\n\tpass");
		}
		
		[Test]
		public void InnerClassDeclaration()
		{
			Test("public class Test { class Inner {} }", "public class Test:\n\tprivate class Inner:\n\t\tpass");
		}
		
		[Test]
		public void InterfaceDeclaration()
		{
			Test("public interface Test {}", "public interface Test:\n\tpass");
		}
		
		[Test]
		public void InterfaceDeclarationWithAttribute()
		{
			Test("[TestFixture] interface Test {}", "[TestFixture]\ninternal interface Test:\n\tpass");
		}
		
		[Test]
		public void InterfaceDeclarationWithBaseType()
		{
			Test("public interface ExtendedDisposable : IDisposable {}", "public interface ExtendedDisposable(IDisposable):\n\tpass");
		}
		
		[Test]
		public void EnumerationDeclaration()
		{
			Test("public enum Test {}", "public enum Test:\n\tpass");
		}
		
		[Test]
		public void EnumerationDeclarationWithAttribute()
		{
			Test("[TestFixture] enum Test {}", "[TestFixture]\ninternal enum Test:\n\tpass");
		}
		
		[Test]
		public void StructureDeclaration()
		{
			Test("public struct Test {}", "public struct Test:\n\tpass");
		}
		
		[Test]
		public void StructureDeclarationWithAttribute()
		{
			Test("[TestFixture] struct Test {}", "[TestFixture]\ninternal struct Test:\n\tpass");
		}
		
		[Test]
		public void StructureDeclarationWithBaseType()
		{
			Test("public struct Bla : ValueType {}", "public struct Bla(ValueType):\n\tpass");
		}
	}
}
