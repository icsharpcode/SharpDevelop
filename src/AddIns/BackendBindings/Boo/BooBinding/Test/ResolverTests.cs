// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections;
using System.Reflection;
using NUnit.Framework;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Project;
using Grunwald.BooBinding.CodeCompletion;

namespace Grunwald.BooBinding.Tests
{
	[TestFixture]
	public class ResolverTests
	{
		#region Helper
		T Resolve<T>(string code) where T : ResolveResult
		{
			return Resolve<T>(code, "/*1*/");
		}
		
		T Resolve<T>(string code, string marker) where T : ResolveResult
		{
			return Resolve<T>(normalProg, code, marker);
		}
		
		T Resolve<T>(string prog, string code, string marker) where T : ResolveResult
		{
			ResolveResult rr = Resolve(prog, new ExpressionResult(code), marker);
			Assert.IsNotNull(rr, "Resolve must not return null");
			Assert.IsInstanceOf(typeof(T), rr, "Resolve must return instance of type " + typeof(T).Name);
			return (T)rr;
		}
		
		IProjectContent booLangPC;
		
		public ResolverTests() {
			booLangPC = new ReflectionProjectContent(Assembly.Load("Boo.Lang"), "Boo.Lang.dll", AssemblyParserService.DefaultProjectContentRegistry);
			booLangPC.ReferencedContents.Add(AssemblyParserService.DefaultProjectContentRegistry.Mscorlib);
		}
		
		const string fileName = "tempFile.boo";
		DefaultProjectContent lastPC;
		
		void Register(string prog)
		{
			DefaultProjectContent pc = new DefaultProjectContent();
			lastPC = pc;
			HostCallback.GetCurrentProjectContent = delegate { return pc; };
			pc.ReferencedContents.Add(AssemblyParserService.DefaultProjectContentRegistry.Mscorlib);
			pc.ReferencedContents.Add(AssemblyParserService.DefaultProjectContentRegistry.GetProjectContentForReference("System.Windows.Forms", typeof(System.Windows.Forms.Form).Module.FullyQualifiedName));
			pc.ReferencedContents.Add(booLangPC);
			ICompilationUnit cu = new BooParser().Parse(pc, fileName, new StringTextBuffer(prog));
			ParserService.RegisterParseInformation(fileName, cu);
			cu.Classes.ForEach(pc.AddClassToNamespaceList);
		}
		
		void GetPos(string prog, string marker, out int line, out int column)
		{
			int index = prog.IndexOf(marker);
			line = 1;
			column = 0;
			for (int i = 0; i < index; i++) {
				column++;
				if (prog[i]=='\n') {
					line++;
					column = 0;
				}
			}
		}
		
		ResolveResult Resolve(string prog, ExpressionResult er, string marker)
		{
			Register(prog);
			int line, column;
			GetPos(prog, marker, out line, out column);
			er.Region = new DomRegion(line, column);
			
			BooResolver r = new BooResolver();
			return r.Resolve(er, ParserService.GetParseInformation(fileName), prog);
		}
		#endregion
		
		#region Basic tests
		const string normalProg =
			"import System\n" +
			"def MyMethod(arg as string):\n" +
			"\tlocalVar = arg\n" +
			"\t/*1*/\n" +
			"\tclosure = { e as string | arg.IndexOf(e) /*inClosure*/ }\n" +
			"\tindex = closure('.')\n" +
			"\t/*2*/\n" +
			"\tclosure2 = def(e as DateTime):\n" +
			"\t\treturn e.Year\n" +
			"\trecursiveClosure = def(myObject):/*inRecursiveClosure*/\n" +
			"\t\treturn recursiveClosure(myObject)\n" +
			"\t/*3*/\n";
		
		[Test]
		public void MethodParameter()
		{
			LocalResolveResult rr = Resolve<LocalResolveResult>("arg");
			Assert.IsTrue(rr.IsParameter);
			Assert.AreEqual("System.String", rr.ResolvedType.FullyQualifiedName);
		}
		
		[Test]
		public void LocalVariable()
		{
			LocalResolveResult rr = Resolve<LocalResolveResult>("localVar");
			Assert.IsFalse(rr.IsParameter);
			Assert.AreEqual("System.String", rr.ResolvedType.FullyQualifiedName);
		}
		
		[Test]
		public void NullCoalescing()
		{
			ResolveResult rr = Resolve<ResolveResult>("localVar or arg");
			Assert.AreEqual("System.String", rr.ResolvedType.FullyQualifiedName);
		}
		
		[Test]
		public void InnerClassEnum()
		{
			TypeResolveResult trr = Resolve<TypeResolveResult>("Environment.SpecialFolder");
			Assert.AreEqual("System.Environment.SpecialFolder", trr.ResolvedClass.FullyQualifiedName);
			
			MemberResolveResult mrr = Resolve<MemberResolveResult>("Environment.SpecialFolder.Desktop");
			Assert.AreEqual("System.Environment.SpecialFolder.Desktop", mrr.ResolvedMember.FullyQualifiedName);
		}
		
		[Test]
		public void ClosureParameter()
		{
			LocalResolveResult rr = Resolve<LocalResolveResult>("e", "/*inClosure*/");
			Assert.AreEqual("System.String", rr.ResolvedType.FullyQualifiedName);
			
			Assert.IsNull(Resolve(normalProg, new ExpressionResult("e"), "/*1*/"));
		}
		
		[Test]
		public void ClosureCall()
		{
			LocalResolveResult rr = Resolve<LocalResolveResult>("closure('.')", "/*2*/");
			Assert.IsFalse(rr.IsParameter);
			Assert.AreEqual("closure", rr.Field.Name);
			Assert.AreEqual("System.Int32", rr.ResolvedType.FullyQualifiedName);
		}
		
		[Test]
		public void ClosureCall2()
		{
			LocalResolveResult rr = Resolve<LocalResolveResult>("closure2(DateTime.Now)", "/*3*/");
			Assert.IsFalse(rr.IsParameter);
			Assert.AreEqual("closure2", rr.Field.Name);
			Assert.AreEqual("System.Int32", rr.ResolvedType.FullyQualifiedName);
		}
		
		[Test]
		public void RecursiveClosure()
		{
			// Code-completion cannot work here, test if SharpDevelop is correctly
			// preventing the StackOverflow.
			LocalResolveResult rr = Resolve<LocalResolveResult>("recursiveClosure", "/*3*/");
			Assert.IsFalse(rr.IsParameter);
			Assert.AreEqual("delegate(myObject:Object):?", rr.ResolvedType.FullyQualifiedName);
		}
		
		[Test]
		public void ClosureTypelessArgument()
		{
			LocalResolveResult rr = Resolve<LocalResolveResult>("myObject", "/*inRecursiveClosure*/");
			Assert.AreEqual("System.Object", rr.ResolvedType.FullyQualifiedName);
		}
		
		[Test]
		public void EqualityOperator()
		{
			ResolveResult rr = Resolve<ResolveResult>("0 == 0");
			Assert.AreEqual("System.Boolean", rr.ResolvedType.FullyQualifiedName);
			rr = Resolve<ResolveResult>("0 != 1");
			Assert.AreEqual("System.Boolean", rr.ResolvedType.FullyQualifiedName);
			rr = Resolve<ResolveResult>("null is null");
			Assert.AreEqual("System.Boolean", rr.ResolvedType.FullyQualifiedName);
			rr = Resolve<ResolveResult>("object() is not null");
			Assert.AreEqual("System.Boolean", rr.ResolvedType.FullyQualifiedName);
		}
		
		[Test]
		public void ClassMethodAmbiguity()
		{
			string prog =
				"class Test:\n" +
				"\tdef constructor():\n" +
				"\t\tpass\n" +
				"class OtherClass:\n" +
				"\tdef Test():\n" +
				"\t\t/*mark*/\n" +
				"\t\tpass\n";
			MemberResolveResult rr = Resolve<MemberResolveResult>(prog, "Test()", "/*mark*/");
			Assert.AreEqual("OtherClass.Test", rr.ResolvedMember.FullyQualifiedName);
		}
		#endregion
		
		#region Regression
		const string regressionProg =
			"import System\n" +
			"import System.Reflection\n" +
			"def MyMethod(arg as string):\n" +
			"\tif true:\n" +
			"\t\tboo629 = 'hello'\n" +
			"\tfor boo640a in [1, 2, 3]:\n" +
			"\t\tif boo640b = boo640a as FieldInfo: /*640*/\n" +
			"\t\t\tprint boo640b\n" +
			"\t\n" +
			"\tprint 'end of method'\n" +
			"\t/*1*/\n";
		
		[Test]
		public void MyMethodCompletion()
		{
			MethodGroupResolveResult rr = Resolve<MethodGroupResolveResult>(regressionProg, "MyMethod", "/*1*/");
			var arr = rr.GetCompletionData(lastPC);
			Assert.IsNotNull(arr);
			bool beginInvoke = false;
			bool invoke = false;
			foreach (IMember m in arr) {
				if (m.Name == "BeginInvoke") beginInvoke = true;
				if (m.Name == "Invoke") invoke = true;
			}
			Assert.IsTrue(beginInvoke, "beginInvoke");
			Assert.IsTrue(invoke, "invoke");
		}
		
		[Test]
		public void Boo629VariableScope()
		{
			LocalResolveResult rr = Resolve<LocalResolveResult>(regressionProg, "boo629", "/*1*/");
			Assert.AreEqual("System.String", rr.ResolvedType.FullyQualifiedName);
		}
		
		[Test]
		public void Boo640ConditionalAssignment()
		{
			LocalResolveResult rr = Resolve<LocalResolveResult>(regressionProg, "boo640b", "/*1*/");
			Assert.AreEqual("System.Reflection.FieldInfo", rr.ResolvedType.FullyQualifiedName);
			rr = Resolve<LocalResolveResult>(regressionProg, "boo640a", "/*640*/");
			Assert.AreEqual("System.Object", rr.ResolvedType.FullyQualifiedName);
			Assert.IsNull(Resolve(regressionProg, new ExpressionResult("boo640a"), "/*1*/"));
		}
		
		[Test]
		public void IndexerRecognition()
		{
			string prog =
				"class Foo:\n" +
				"\tself[index as int]:\n" +
				"\t\tget:\n" +
				"\t\t\treturn true\n" +
				"def example():\n" +
				"\tfoo = Foo()\n" +
				"\tmybool = foo[1] /*mark*/\n" +
				"\tprint mybool\n";
			MemberResolveResult rr = Resolve<MemberResolveResult>(prog, "foo[1]", "/*mark*/");
			Assert.IsTrue(((IProperty)rr.ResolvedMember).IsIndexer);
			Assert.AreEqual("System.Boolean", rr.ResolvedType.FullyQualifiedName);
			LocalResolveResult rr2 = Resolve<LocalResolveResult>(prog, "mybool", "/*mark*/");
			Assert.AreEqual("System.Boolean", rr2.ResolvedType.FullyQualifiedName);
		}
		
		[Test]
		public void InfiniteRecursionGenerator()
		{
			string prog =
				"class Test:\n" +
				"\t_testList = []\n" +
				"\tTestProperty:\n" +
				"\t\tget:\n" +
				"\t\t\tfor testobj as Test in _testList:\n" +
				"\t\t\t\tyield testobj.TestProperty /*mark*/\n";
			MemberResolveResult rr = Resolve<MemberResolveResult>(prog, "testobj.TestProperty", "/*mark*/");
			Assert.AreEqual("Test.TestProperty", rr.ResolvedMember.FullyQualifiedName);
			Assert.AreEqual("System.Collections.Generic.IEnumerable", rr.ResolvedType.FullyQualifiedName);
			// prevent creating self-referring ConstructedReturnType
			Assert.AreEqual("?", rr.ResolvedType.CastToConstructedReturnType().TypeArguments[0].FullyQualifiedName);
		}
		#endregion
		
		#region Nested Classes
		const string nestedClassProg =
			"class Outer:\n" +
			"\tpublic static outerField = 1\n" +
			"\tpublic class Inner:\n/*inner*/" +
			"\t\tpublic innerField = 2\n" +
			"class Derived(Outer):\n/*derived*/" +
			"\tpublic static derivedField = 3\n" +
			"def Method():\n" +
			"\ti as Outer.Inner\n" +
			"\ti2 as Derived.Inner\n" +
			"\t/*1*/";
		
		[Test]
		public void NestedClassTypeResolution()
		{
			TypeResolveResult trr;
			trr = Resolve<TypeResolveResult>(nestedClassProg, "Outer.Inner", "/*1*/");
			Assert.AreEqual("Outer.Inner", trr.ResolvedClass.FullyQualifiedName);
			trr = Resolve<TypeResolveResult>(nestedClassProg, "Inner", "/*inner*/");
			Assert.AreEqual("Outer.Inner", trr.ResolvedClass.FullyQualifiedName);
			trr = Resolve<TypeResolveResult>(nestedClassProg, "Inner", "/*derived*/");
			Assert.AreEqual("Outer.Inner", trr.ResolvedClass.FullyQualifiedName);
			trr = Resolve<TypeResolveResult>(nestedClassProg, "Derived.Inner", "/*1*/");
			Assert.AreEqual("Outer.Inner", trr.ResolvedClass.FullyQualifiedName);
		}
		
		[Test]
		public void NestedClassCtrlSpace()
		{
			CtrlSpace(nestedClassProg.Replace("/*inner*/", "/*mark*/"), "outerField", "innerField", "Inner", "Outer", "Derived");
			CtrlSpace(nestedClassProg.Replace("/*derived*/", "/*mark*/"), "outerField", "derivedField", "Inner", "Outer", "Derived");
		}
		
		[Test]
		public void NestedClassParentStaticField()
		{
			MemberResolveResult mrr = Resolve<MemberResolveResult>(nestedClassProg, "outerField", "/*inner*/");
			Assert.AreEqual("Outer.outerField", mrr.ResolvedMember.FullyQualifiedName);
		}
		
		[Test]
		public void NestedClassCC()
		{
			LocalResolveResult rr = Resolve<LocalResolveResult>(nestedClassProg, "i", "/*1*/");
			Assert.AreEqual("Outer.Inner", rr.ResolvedType.FullyQualifiedName);
			bool ok = false;
			foreach (object o in rr.GetCompletionData(lastPC)) {
				IMember m = o as IMember;
				if (m != null && m.Name == "innerField")
					ok = true;
			}
			Assert.IsTrue(ok);
			MemberResolveResult mrr = Resolve<MemberResolveResult>(nestedClassProg, "i.innerField", "/*1*/");
			Assert.AreEqual("Outer.Inner.innerField", mrr.ResolvedMember.FullyQualifiedName);
		}
		
		[Test]
		public void NestedClassCC2()
		{
			LocalResolveResult rr = Resolve<LocalResolveResult>(nestedClassProg, "i2", "/*1*/");
			Assert.AreEqual("Outer.Inner", rr.ResolvedType.FullyQualifiedName);
			bool ok = false;
			foreach (object o in rr.GetCompletionData(lastPC)) {
				IMember m = o as IMember;
				if (m != null && m.Name == "innerField")
					ok = true;
			}
			Assert.IsTrue(ok);
			MemberResolveResult mrr = Resolve<MemberResolveResult>(nestedClassProg, "i2.innerField", "/*1*/");
			Assert.AreEqual("Outer.Inner.innerField", mrr.ResolvedMember.FullyQualifiedName);
		}
		#endregion
		
		#region CtrlSpace
		void CtrlSpace(string prog, params string[] expected)
		{
			CtrlSpace(new string[0], prog, expected);
		}
		
		void CtrlSpace(string[] unExpected, string prog, params string[] expected)
		{
			Register(prog);
			int line, column;
			GetPos(prog, "/*mark*/", out line, out column);
			BooResolver r = new BooResolver();
			var ar = r.CtrlSpace(line, column, ParserService.GetParseInformation(fileName), prog, ExpressionContext.Default);
			foreach (string e in unExpected) {
				foreach (object o in ar) {
					if (e.Equals(o))
						Assert.Fail("Didn't expect " + e);
					if (o is IMember && (o as IMember).Name == e) {
						Assert.Fail("Didn't expect " + e);
					}
					if (o is IClass && (o as IClass).Name == e) {
						Assert.Fail("Didn't expect " + e);
					}
				}
			}
			foreach (string e in expected) {
				bool ok = false;
				foreach (object o in ar) {
					if (e.Equals(o)) {
						if (ok) Assert.Fail("double entry " + e);
						ok = true;
					}
					if (o is IMember && (o as IMember).Name == e) {
						if (ok) Assert.Fail("double entry " + e);
						ok = true;
					}
					if (o is IClass && (o as IClass).Name == e) {
						if (ok) Assert.Fail("double entry " + e);
						ok = true;
					}
				}
				if (!ok)
					Assert.Fail("Expected " + e);
			}
		}
		
		[Test]
		public void CtrlSpaceScopeExtension()
		{
			string prog =
				"def Foo():\n" +
				"\tbar = def():\n" +
				"\t\tx = 0\n" +
				"\t\t/*mark*/\n";
			CtrlSpace(prog, "bar", "x");
		}
		
		[Test]
		public void DoubleEntryTest()
		{
			string prog =
				"class MyClass:\n" +
				"\t_myInt = 0\n" +
				"\tdef Foo():\n" +
				"\t\t_myInt = 5\n" +
				"\t\t/*mark*/\n";
			CtrlSpace(prog, "_myInt");
		}
		
		[Test]
		public void LoopInClosureTest()
		{
			string prog =
				"def Foo():\n" +
				"\tfor i in range(5):\n" +
				"\t\tbar = def():\n" +
				"\t\t\tx = 0\n" +
				"\t\t\t/*mark*/\n" +
				"\t\t\tprint x";
			CtrlSpace(prog, "x", "bar", "i");
		}
		#endregion
		
		[Test]
		public void SystemNamespaceInFileWithNamespace()
		{
			string prog =
				"namespace Test\n" +
				"/*mark*/";
			NamespaceResolveResult nrr = Resolve<NamespaceResolveResult>(prog, "System", "/*mark*/");
			Assert.AreEqual("System", nrr.Name);
		}
		
		[Test]
		public void ClassInParentNamespace()
		{
			string prog =
				"namespace System.Collections.CustomSubNamespace\n" +
				"/*mark*/";
			TypeResolveResult trr = Resolve<TypeResolveResult>(prog, "ICollection", "/*mark*/");
			Assert.AreEqual("System.Collections.ICollection", trr.ResolvedClass.FullyQualifiedName);
		}
	}
}
