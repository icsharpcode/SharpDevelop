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
		
		ResolveResult Resolve(string prog, ExpressionResult er, string marker)
		{
			const string fileName = "tempFile.boo";
			DefaultProjectContent pc = new DefaultProjectContent();
			ParserService.ForceProjectContent(pc);
			pc.ReferencedContents.Add(ProjectContentRegistry.Mscorlib);
			pc.ReferencedContents.Add(ProjectContentRegistry.WinForms);
			pc.ReferencedContents.Add(booLangPC);
			ICompilationUnit cu = new BooParser().Parse(pc, fileName, prog);
			ParserService.UpdateParseInformation(cu, fileName, false, false);
			cu.Classes.ForEach(pc.AddClassToNamespaceList);
			
			int index = prog.IndexOf(marker);
			int line = 1;
			int column = 0;
			for (int i = 0; i < index; i++) {
				column++;
				if (prog[i]=='\n') {
					line++;
					column = 0;
				}
			}
			
			BooResolver r = new BooResolver();
			return r.Resolve(er, line, column, fileName, prog);
		}
		#endregion
		
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
			"\trecursiveClosure = def():\n" +
			"\t\treturn recursiveClosure()\n" +
			"\t/*3*/\n";
		
		#region Basic tests
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
			Assert.AreEqual("delegate():?", rr.ResolvedType.FullyQualifiedName);
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
	}
}
