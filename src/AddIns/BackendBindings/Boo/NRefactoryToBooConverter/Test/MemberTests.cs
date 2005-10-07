#region license
// Copyright (c) 2005, Daniel Grunwald (daniel@danielgrunwald.de)
// All rights reserved.
//
// NRefactoryToBoo is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
// 
// NRefactoryToBoo is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with NRefactoryToBoo; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
#endregion

using System;
using NUnit.Framework;

namespace NRefactoryToBooConverter.Tests
{
	[TestFixture]
	public class MemberTests : TestHelper
	{
		[Test]
		public void EnumerationMember()
		{
			Test("public enum Trool { No, Maybe, Yes }", "public enum Trool:\n\tNo\n\tMaybe\n\tYes");
		}
		
		[Test]
		public void EnumerationMemberWithAttribute()
		{
			Test("public enum Trool { No, [XXX] Maybe, Yes }", "public enum Trool:\n\tNo\n\t[XXX]\n\tMaybe\n\tYes");
		}
		
		[Test]
		public void EnumerationMemberWithValue()
		{
			Test("public enum Trool { No = 0, Maybe = 2, Yes = 4 }", "public enum Trool:\n\tNo = 0\n\tMaybe = 2\n\tYes = 4");
		}
		
		[Test]
		public void Field()
		{
			TestInClass("MyType o;", "private o as MyType");
		}
		
		[Test]
		public void MultipleFields()
		{
			TestInClass("MyType a, b, c;", "private a as MyType\nprivate b as MyType\nprivate c as MyType");
		}
		
		[Test]
		public void PrimitiveField()
		{
			TestInClass("int num;", "private num as System.Int32");
		}
		
		[Test]
		public void ArrayField()
		{
			TestInClass("Field[] Core;", "private Core as (Field)");
		}
		
		[Test]
		public void FieldWithModifier()
		{
			TestInClass("public static int num;", "public static num as System.Int32");
		}
		
		[Test]
		public void FullyQualifiedField()
		{
			TestInClass("System.IDisposable d;", "private d as System.IDisposable");
		}
		
		[Test]
		public void FieldWithInitializer()
		{
			TestInClass("MyType o = null;", "private o as MyType = null");
		}
		
		[Test]
		public void Method()
		{
			TestInClass("void Main() {}", "private final def Main() as System.Void:\n\tpass");
		}
		
		[Test]
		public void MethodWithAttribute()
		{
			TestInClass("[Test] void Main() {}", "[Test]\nprivate final def Main() as System.Void:\n\tpass");
		}
		
		[Test]
		public void MethodWithParameters()
		{
			TestInClass("void Main(int a, MyType b) {}", "private final def Main(a as System.Int32, b as MyType) as System.Void:\n\tpass");
		}
		
		[Test]
		public void MethodWithRefParameters()
		{
			TestInClass("void Main(ref int a, out MyType b) {}", "private final def Main(ref a as System.Int32, ref b as MyType) as System.Void:\n\tpass");
		}
		
		[Test]
		public void MethodWithParamsParameters()
		{
			TestInClass("void Main(int a, params string[] args) {}", "private final def Main(a as System.Int32, *args as (System.String)) as System.Void:\n\tpass");
		}
		
		[Test]
		public void MethodWithReturnType()
		{
			TestInClass("MyType Main() {}", "private final def Main() as MyType:\n\tpass");
		}
		
		[Test]
		public void MethodWithModifier()
		{
			TestInClass("public static void Run() {}", "public static def Run() as System.Void:\n\tpass");
		}
		
		[Test]
		public void StaticMethodInStaticClass()
		{
			Test("public static class MainClass { public static void Run() {} }",
			     "public static class MainClass:\n\tpublic static def Run() as System.Void:\n\t\tpass");
		}
		
		[Test]
		public void Constructor()
		{
			TestInClass("ClassName() {}", "private def constructor():\n\tpass");
		}
		
		[Test]
		public void ConstructorWithAttribute()
		{
			TestInClass("[Test] ClassName() {}", "[Test]\nprivate def constructor():\n\tpass");
		}
		
		[Test]
		public void ConstructorWithParameters()
		{
			TestInClass("ClassName(int a, MyType b) {}", "private def constructor(a as System.Int32, b as MyType):\n\tpass");
		}
		
		[Test]
		public void ConstructorWithModifier()
		{
			TestInClass("public static ClassName() {}", "public static def constructor():\n\tpass");
		}
		
		[Test]
		public void ConstructorWithThisCall()
		{
			TestInClass("public ClassName() : this(5) {}", "public def constructor():\n\tself(5)");
		}
		
		[Test]
		public void ConstructorWithBaseCall()
		{
			TestInClass("public ClassName() : base(5) {}", "public def constructor():\n\tsuper(5)");
		}
		
		[Test]
		public void Destructor()
		{
			TestInClass("~ClassName() {}", "def destructor():\n\tpass");
		}
		
		[Test]
		public void DestructorWithAttribute()
		{
			TestInClass("[Test] ~ClassName() {}", "[Test]\ndef destructor():\n\tpass");
		}
		
		[Test]
		public void ReadOnlyProperty()
		{
			TestInClass("public string Text { get { } }", "public Text as System.String:\n\tget:\n\t\tpass");
		}
		
		[Test]
		public void WriteOnlyProperty()
		{
			TestInClass("public string Text { set { } }", "public Text as System.String:\n\tset:\n\t\tpass");
		}
		
		[Test]
		public void Property()
		{
			TestInClass("public string Text { get {} set { } }", "public Text as System.String:\n\tget:\n\t\tpass\n\tset:\n\t\tpass");
		}
		
		[Test]
		public void PropertyWithAttributes()
		{
			TestInClass("[AA] public string Text { [BB] get {} [CC] set { } }",
			            "[AA]\npublic Text as System.String:\n\t[BB]\n\tget:\n\t\tpass\n\t[CC]\n\tset:\n\t\tpass");
		}
		
		[Test]
		public void ReadOnlyIndexer()
		{
			TestInClassWithIndexer("public string this[int index] { get { } }", "public Indexer(index as System.Int32) as System.String:\n\tget:\n\t\tpass");
		}
		
		[Test]
		public void WriteOnlyIndexer()
		{
			TestInClassWithIndexer("public string this[int index] { set { } }", "public Indexer(index as System.Int32) as System.String:\n\tset:\n\t\tpass");
		}
		
		[Test]
		public void Indexer()
		{
			TestInClassWithIndexer("public string this[int index] { get {} set { } }", "public Indexer(index as System.Int32) as System.String:\n\tget:\n\t\tpass\n\tset:\n\t\tpass");
		}
		
		[Test]
		public void IndexerWithAttributes()
		{
			TestInClassWithIndexer("[AA] public string this[int index] { [BB] get {} [CC] set { } }",
			                       "[AA]\npublic Indexer(index as System.Int32) as System.String:\n\t[BB]\n\tget:\n\t\tpass\n\t[CC]\n\tset:\n\t\tpass");
		}
		
		[Test]
		public void Event()
		{
			TestInClass("public event EventHandler Closed;", "public event Closed as EventHandler");
		}
		
		[Test]
		public void EventWithAttribute()
		{
			TestInClass("[LookHere] event EventHandler Closed;", "[LookHere]\nprivate event Closed as EventHandler");
		}
	}
}
