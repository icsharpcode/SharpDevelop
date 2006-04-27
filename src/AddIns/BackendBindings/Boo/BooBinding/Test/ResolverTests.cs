/*
 * Created by SharpDevelop.
 * User: Daniel Grunwald
 * Date: 23.04.2006
 * Time: 11:33
 */

using System;
using System.Reflection;
using NUnit.Framework;
using ICSharpCode.Core;
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
			ResolveResult rr = Resolve(prog, new ExpressionResult(code), marker);
			Assert.IsNotNull(rr, "Resolve must not return null");
			Assert.IsInstanceOfType(typeof(T), rr, "Resolve must return instance of type " + typeof(T).Name);
			return (T)rr;
		}
		
		T ResolveReg<T>(string code) where T : ResolveResult
		{
			return ResolveReg<T>(code, "/*1*/");
		}
		
		T ResolveReg<T>(string code, string marker) where T : ResolveResult
		{
			ResolveResult rr = Resolve(regressionProg, new ExpressionResult(code), marker);
			Assert.IsNotNull(rr, "Resolve must not return null");
			Assert.IsInstanceOfType(typeof(T), rr, "Resolve must return instance of type " + typeof(T).Name);
			return (T)rr;
		}
		
		IProjectContent booLangPC;
		
		public ResolverTests() {
			booLangPC = new ReflectionProjectContent(Assembly.Load("Boo.Lang"), "Boo.Lang.dll");
			booLangPC.ReferencedContents.Add(ProjectContentRegistry.Mscorlib);
		}
		
		const string fileName = "tempFile.boo";
		
		void Register(string prog)
		{
			DefaultProjectContent pc = new DefaultProjectContent();
			ParserService.ForceProjectContent(pc);
			pc.ReferencedContents.Add(ProjectContentRegistry.Mscorlib);
			pc.ReferencedContents.Add(ProjectContentRegistry.WinForms);
			pc.ReferencedContents.Add(booLangPC);
			ICompilationUnit cu = new BooParser().Parse(pc, fileName, prog);
			ParserService.UpdateParseInformation(cu, fileName, false, false);
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
			
			BooResolver r = new BooResolver();
			return r.Resolve(er, line, column, fileName, prog);
		}
		#endregion
		
		#region Basic tests
		const string prog =
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
			
			Assert.IsNull(Resolve(prog, new ExpressionResult("e"), "/*1*/"));
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
			"\t/*1*/\n";
		
		[Test]
		public void Boo629VariableScope()
		{
			LocalResolveResult rr = ResolveReg<LocalResolveResult>("boo629");
			Assert.AreEqual("System.String", rr.ResolvedType.FullyQualifiedName);
		}
		
		[Test]
		public void Boo640ConditionalAssignment()
		{
			LocalResolveResult rr = ResolveReg<LocalResolveResult>("boo640b");
			Assert.AreEqual("System.Reflection.FieldInfo", rr.ResolvedType.FullyQualifiedName);
			rr = ResolveReg<LocalResolveResult>("boo640a", "/*640*/");
			Assert.AreEqual("System.Object", rr.ResolvedType.FullyQualifiedName);
			Assert.IsNull(Resolve(regressionProg, new ExpressionResult("boo640a"), "/*1*/"));
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
			System.Collections.ArrayList ar;
			ar = r.CtrlSpace(line, column, fileName, prog, ExpressionContext.Default);
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
	}
}
