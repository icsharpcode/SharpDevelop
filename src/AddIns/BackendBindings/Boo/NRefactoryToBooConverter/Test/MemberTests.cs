// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
		public void ConstantField()
		{
			TestInClass("public const int num = 3;", "public static final num as System.Int32 = 3");
		}
		
		[Test]
		public void FullyQualifiedField()
		{
			TestInClass("System.IDisposable d;", "private d as System.IDisposable");
		}
		
		[Test]
		public void GenericField()
		{
			TestInClass("Dictionary<Dictionary<T, List<K>>, List<J>> d;",
			            "private d as Dictionary[of Dictionary[of T, List[of K]], List[of J]]");
		}
		
		[Test]
		public void FieldWithInitializer()
		{
			TestInClass("MyType o = null;", "private o as MyType = null");
		}
		
		[Test]
		public void Method()
		{
			TestInClass("void Main() {}", "private def Main() as System.Void:\n\tpass");
		}
		
		[Test]
		public void ExplicitInterfaceImplementationMethod()
		{
			TestInClass("void IDisposable.Dispose() {}", "def IDisposable.Dispose() as System.Void:\n\tpass");
		}
		
		[Test]
		public void MethodWithAttribute()
		{
			TestInClass("[Test] void Main() {}", "[Test]\nprivate def Main() as System.Void:\n\tpass");
		}
		
		[Test]
		public void MethodWithParameters()
		{
			TestInClass("void Main(int a, MyType b) {}", "private def Main(a as System.Int32, b as MyType) as System.Void:\n\tpass");
		}
		
		[Test]
		public void MethodWithRefParameters()
		{
			TestInClass("void Main(ref int a, out MyType b) {}", "private def Main(ref a as System.Int32, ref b as MyType) as System.Void:\n\tpass");
		}
		
		[Test]
		public void MethodWithParamsParameters()
		{
			TestInClass("void Main(int a, params string[] args) {}", "private def Main(a as System.Int32, *args as (System.String)) as System.Void:\n\tpass");
		}
		
		[Test]
		public void MethodWithReturnType()
		{
			TestInClass("MyType Main() {}", "private def Main() as MyType:\n\tpass");
		}
		
		[Test]
		public void MethodWithModifier()
		{
			TestInClass("public static void Run() {}", "public static def Run() as System.Void:\n\tpass");
		}
		
		[Test]
		public void AbstractMethod()
		{
			TestInClass("public abstract void Run();", "public abstract def Run() as System.Void:\n\tpass");
		}
		
		[Test]
//		[Ignore("Fix requires change to Boo.Lang.Compiler.dll")]
		public void AbstractMethodInInterface()
		{
			TestInInterface("void Run();", "def Run() as System.Void");
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
		public void PropertyWithProtectedSetter()
		{
			TestInClass("public string Text { get {} protected set { } }",
			            "public Text as System.String:\n\tget:\n\t\tpass\n\tprotected set:\n\t\tpass");
		}
		
		[Test]
		public void AbstractProperty()
		{
			TestInClass("public abstract string Prop { get; }", "public abstract Prop as System.String:\n\tget:\n\t\tpass");
		}
		
		[Test]
//		[Ignore("Fix requires change to Boo.Lang.Compiler.dll")]
		public void AbstractPropertyInInterface()
		{
			TestInInterface("string Prop { get; }", "Prop as System.String:\n\tget");
		}
		
		[Test]
		public void ReadOnlyIndexer()
		{
			TestInClass("public string this[int index] { get { } }", "public self[index as System.Int32] as System.String:\n\tget:\n\t\tpass");
		}
		
		[Test]
		public void WriteOnlyIndexer()
		{
			TestInClass("public string this[int index] { set { } }", "public self[index as System.Int32] as System.String:\n\tset:\n\t\tpass");
		}
		
		[Test]
		public void Indexer()
		{
			TestInClass("public string this[int index] { get {} set { } }", "public self[index as System.Int32] as System.String:\n\tget:\n\t\tpass\n\tset:\n\t\tpass");
		}
		
		[Test]
		public void IndexerWithAttributes()
		{
			TestInClass("[AA] public string this[int index] { [BB] get {} [CC] set { } }",
			            "[AA]\npublic self[index as System.Int32] as System.String:\n\t[BB]\n\tget:\n\t\tpass\n\t[CC]\n\tset:\n\t\tpass");
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
		
		[Test]
//		[Ignore("Fix requires change to Boo.Lang.Compiler.dll")]
		public void EventInInterface()
		{
			TestInInterface("event EventHandler Closed;", "event Closed as EventHandler");
		}
		
		[Test]
		public void PInvoke()
		{
			TestInClass("[DllImport(\"User32.dll\", CharSet = CharSet.Auto)]\n" +
			            "static extern IntPtr MessageBox(int etc);",
			            "[DllImport('User32.dll', CharSet: CharSet.Auto)]\n" +
			            "private static def MessageBox(etc as System.Int32) as IntPtr:\n" +
			            "\tpass");
		}
	}
}
