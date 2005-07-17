using System;
using System.Collections;
using System.IO;
using NUnit.Framework;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Dom.NRefactoryResolver;

namespace ICSharpCode.SharpDevelop.Tests
{
	[TestFixture]
	public class NRefactoryResolverTests
	{
		#region Test helper methods
		ICompilationUnit Parse(string fileName, string fileContent)
		{
			ICSharpCode.NRefactory.Parser.IParser p = ICSharpCode.NRefactory.Parser.ParserFactory.CreateParser(ICSharpCode.NRefactory.Parser.SupportedLanguages.CSharp, new StringReader(fileContent));
			p.ParseMethodBodies = false;
			p.Parse();
			DefaultProjectContent pc = new DefaultProjectContent();
			pc.ReferencedContents.Add(corLib);
			ParserService.ForceProjectContent(pc);
			lastPC = pc;
			NRefactoryASTConvertVisitor visitor = new NRefactoryASTConvertVisitor(pc);
			visitor.Visit(p.CompilationUnit, null);
			visitor.Cu.FileName = fileName;
			visitor.Cu.ErrorsDuringCompile = p.Errors.count > 0;
			foreach (IClass c in visitor.Cu.Classes) {
				pc.AddClassToNamespaceList(c);
			}
			
			return visitor.Cu;
		}
		
		public IProjectContent lastPC;
		
		ICompilationUnit ParseVB(string fileName, string fileContent)
		{
			ICSharpCode.NRefactory.Parser.IParser p = ICSharpCode.NRefactory.Parser.ParserFactory.CreateParser(ICSharpCode.NRefactory.Parser.SupportedLanguages.VBNet, new StringReader(fileContent));
			p.ParseMethodBodies = false;
			p.Parse();
			DefaultProjectContent pc = new DefaultProjectContent();
			ParserService.ForceProjectContent(pc);
			pc.ReferencedContents.Add(corLib);
			pc.Language = LanguageProperties.VBNet;
			lastPC = pc;
			NRefactoryASTConvertVisitor visitor = new NRefactoryASTConvertVisitor(pc);
			visitor.Visit(p.CompilationUnit, null);
			visitor.Cu.FileName = fileName;
			visitor.Cu.ErrorsDuringCompile = p.Errors.count > 0;
			foreach (IClass c in visitor.Cu.Classes) {
				pc.AddClassToNamespaceList(c);
			}
			
			return visitor.Cu;
		}
		
		void AddCompilationUnit(ICompilationUnit parserOutput, string fileName)
		{
			ParserService.UpdateParseInformation(parserOutput, fileName, false, false);
		}
		
		public ResolveResult Resolve(string program, string expression, int line)
		{
			AddCompilationUnit(Parse("a.cs", program), "a.cs");
			
			NRefactoryResolver resolver = new NRefactoryResolver(ICSharpCode.NRefactory.Parser.SupportedLanguages.CSharp);
			return resolver.Resolve(expression,
			                        line, 0,
			                        "a.cs",
			                        program);
		}
		
		public ResolveResult ResolveVB(string program, string expression, int line)
		{
			AddCompilationUnit(ParseVB("a.vb", program), "a.vb");
			
			NRefactoryResolver resolver = new NRefactoryResolver(ICSharpCode.NRefactory.Parser.SupportedLanguages.VBNet);
			return resolver.Resolve(expression,
			                        line, 0,
			                        "a.vb",
			                        program);
		}
		
		IProjectContent corLib = ProjectContentRegistry.GetMscorlibContent();
		#endregion
		
		#region Test for old issues (Fidalgo)
		// Issue SD-291
		[Test]
		public void VBNetMultipleVariableDeclarationsTest()
		{
			string program = @"Class X
	Shared Sub Main
		Dim a, b As String
		
	End Sub
End Class
";
			ResolveResult result = ResolveVB(program, "a", 4);
			Assert.IsNotNull(result, "result");
			Assert.IsTrue(result is LocalResolveResult, "result is LocalResolveResult");
			Assert.AreEqual("System.String", result.ResolvedType.FullyQualifiedName);
			
			result = ResolveVB(program, "b", 4);
			Assert.IsNotNull(result, "result");
			Assert.IsTrue(result is LocalResolveResult, "result is LocalResolveResult");
			Assert.AreEqual("System.String", result.ResolvedType.FullyQualifiedName);
		}
		
		// Issue SD-258
		[Test]
		public void VBNetForeachLoopVariableTest()
		{
			string program = @"Class Test
	Shared Sub Main()
		For Each c As String In MyColl
			
		Next
	End Sub
End Class
";
			ResolveResult result = ResolveVB(program, "c", 4);
			Assert.IsNotNull(result, "result");
			Assert.IsTrue(result is LocalResolveResult, "result is LocalResolveResult");
			Assert.AreEqual("System.String", result.ResolvedType.FullyQualifiedName);
		}
		
		// Issue SD-265
		[Test]
		public void VBNetStaticMembersOnInstanceTest()
		{
			string program = @"Class X
	Sub Z()
		Dim a As String
		
	End Sub
End Class
";
			ResolveResult result = ResolveVB(program, "a", 4);
			Assert.IsNotNull(result, "result");
			ArrayList arr = result.GetCompletionData(lastPC);
			Assert.IsNotNull(arr, "arr");
			foreach (object o in arr) {
				if (o is IMember) {
					if (((IMember)o).FullyQualifiedName == "System.String.Empty")
						return;
				}
			}
			Assert.Fail("Static member empty not found on string instance!");
		}
		
		// Issue SD-217
		[Test]
		public void VBNetLocalArrayLookupTest()
		{
			string program = @"Module Main
	Sub Main()
		Dim t As String()
		
	End Sub
End Module
";
			ResolveResult result = ResolveVB(program, "t", 4);
			Assert.IsNotNull(result, "result");
			Assert.IsTrue(result is LocalResolveResult, "result is LocalResolveResult");
			
			ArrayList arr = result.GetCompletionData(lastPC);
			Assert.IsNotNull(arr, "arr");
			foreach (object o in arr) {
				if (o is IMember) {
					if (((IMember)o).FullyQualifiedName == "System.Array.Length")
						return;
				}
			}
			Assert.Fail("Length not found on array instance (resolve result was " + result.ResolvedType.ToString() + ")");
		}
		#endregion
		
		#region Simple Tests
		const string arrayListConflictProgram = @"using System.Collections;
class A {
	void Test() {
		
	}
	
	ArrayList arrayList;
	public ArrayList ArrayList {
		get {
			return arrayList;
		}
	}
}
";
		
		[Test]
		public void PropertyTypeConflictTest()
		{
			ResolveResult result = Resolve(arrayListConflictProgram, "arrayList", 4);
			Assert.IsTrue(result is MemberResolveResult);
			Assert.AreEqual("System.Collections.ArrayList", result.ResolvedType.FullyQualifiedName);
		}
		
		[Test]
		public void PropertyTypeConflictCompletionResultTest()
		{
			ResolveResult result = Resolve(arrayListConflictProgram, "ArrayList", 4);
			// CC should offer both static and non-static results
			ArrayList list = result.GetCompletionData(lastPC);
			bool ok = false;
			foreach (object o in list) {
				IMethod method = o as IMethod;
				if (method != null && method.Name == "AddRange")
					ok = true;
			}
			Assert.IsTrue(ok, "AddRange should exist");
			ok = false;
			foreach (object o in list) {
				IMethod method = o as IMethod;
				if (method != null && method.Name == "Adapter")
					ok = true;
			}
			Assert.IsTrue(ok, "Adapter should exist");
		}
		
		[Test]
		public void InheritedInterfaceResolveTest()
		{
			string program = @"using System;
class A {
	void Method(IInterface1 a) {
		
	}
}
interface IInterface1 : IInterface2, IDisposable {
	void Method1();
}
interface IInterface2 {
	void Method2();
}
";
			ResolveResult result = Resolve(program, "a", 4);
			Assert.IsNotNull(result, "result");
			Assert.IsTrue(result is LocalResolveResult, "result is LocalResolveResult");
			ArrayList arr = result.GetCompletionData(lastPC);
			Assert.IsNotNull(arr, "arr");
			bool m1 = false;
			bool m2 = false;
			bool disp = false;
			foreach (IMethod m in arr) {
				if (m.Name == "Method1")
					m1 = true;
				if (m.Name == "Method2")
					m2 = true;
				if (m.Name == "Dispose")
					disp = true;
			}
			Assert.IsTrue(m1, "Method1 not found");
			Assert.IsTrue(m2, "Method2 not found");
			Assert.IsTrue(disp, "Dispose not found");
		}
		
		[Test]
		public void InvalidMethodCallTest()
		{
			string program = @"class A {
	void Method(string b) {
		
	}
}
";
			ResolveResult result = Resolve(program, "b.ThisMethodDoesNotExistOnString()", 3);
			Assert.IsNull(result, "result");
		}
		
		[Test]
		public void InvalidConstructorCallTest()
		{
			string program = @"class A {
	void Method() {
		
	}
}
";
			ResolveResult result = Resolve(program, "new ThisClassDoesNotExist()", 3);
			Assert.IsNull(result);
		}
		
		[Test]
		public void MethodCallTest()
		{
			string program = @"class A {
	void Method() {
		
	}
	
	int TargetMethod() {
		return 3;
	}
}
";
			ResolveResult result = Resolve(program, "TargetMethod()", 3);
			Assert.IsNotNull(result);
			Assert.IsTrue(result is MemberResolveResult);
			Assert.AreEqual("System.Int32", result.ResolvedType.FullyQualifiedName, "'TargetMethod()'");
		}
		
		[Test]
		public void ThisMethodCallTest()
		{
			string program = @"class A {
	void Method() {
		
	}
	
	int TargetMethod() {
		return 3;
	}
}
";
			ResolveResult result = Resolve(program, "this.TargetMethod()", 3);
			Assert.IsNotNull(result);
			Assert.IsTrue(result is MemberResolveResult);
			Assert.AreEqual("System.Int32", result.ResolvedType.FullyQualifiedName, "'this.TargetMethod()'");
		}
		
		[Test]
		public void EventCallTest()
		{
			string program = @"using System;
class A {
	void Method() {
		
	}
	
	public event EventHandler TestEvent;
}
";
			ResolveResult result = Resolve(program, "TestEvent(this, EventArgs.Empty)", 4);
			Assert.IsNotNull(result);
			Assert.IsTrue(result is MemberResolveResult);
			Assert.AreEqual("A.TestEvent", (result as MemberResolveResult).ResolvedMember.FullyQualifiedName);
		}
		
		[Test]
		public void VoidTest()
		{
			string program = @"using System;
class A {
	void TestMethod() {
		
	}
}
";
			ResolveResult result = Resolve(program, "TestMethod()", 4);
			Assert.IsNotNull(result);
			Assert.AreSame(ReflectionReturnType.Void, result.ResolvedType, result.ResolvedType.ToString());
			Assert.AreEqual(0, result.GetCompletionData(lastPC).Count);
		}
		
		[Test]
		public void ThisEventCallTest()
		{
			string program = @"using System;
class A {
	void Method() {
		
	}
	
	public event EventHandler TestEvent;
}
";
			ResolveResult result = Resolve(program, "this.TestEvent(this, EventArgs.Empty)", 4);
			Assert.IsNotNull(result);
			Assert.IsTrue(result is MemberResolveResult);
			Assert.AreEqual("A.TestEvent", (result as MemberResolveResult).ResolvedMember.FullyQualifiedName);
		}
		
		[Test]
		public void DelegateCallTest()
		{
			string program = @"using System.Reflection;
class A {
	void Method() {
		ModuleResolveEventHandler eh = SomeClass.SomeProperty;
		
	}
}
";
			ResolveResult result = Resolve(program, "eh(this, new ResolveEventArgs())", 5);
			Assert.IsNotNull(result);
			Assert.IsTrue(result is LocalResolveResult);
			Assert.AreEqual("eh", (result as LocalResolveResult).Field.Name);
			
			result = Resolve(program, "eh(this, new ResolveEventArgs()).GetType(\"bla\")", 5);
			Assert.IsNotNull(result);
			Assert.IsTrue(result is MemberResolveResult);
			Assert.AreEqual("System.Reflection.Module.GetType", (result as MemberResolveResult).ResolvedMember.FullyQualifiedName);
		}
		
		[Test]
		public void OverloadLookupTest()
		{
			string program = @"class A {
	void Method() {
		
	}
	
	int Multiply(int a, int b) { return a * b; }
	double Multiply(double a, double b) { return a * b; }
}
";
			ResolveResult result = Resolve(program, "Multiply(1, 1)", 3);
			Assert.IsNotNull(result);
			Assert.IsTrue(result is MemberResolveResult, "'Multiply(1,1)' is MemberResolveResult");
			Assert.AreEqual("System.Int32", result.ResolvedType.FullyQualifiedName, "'Multiply(1,1)'");
			
			result = Resolve(program, "Multiply(1.0, 1.0)", 3);
			Assert.IsNotNull(result);
			Assert.IsTrue(result is MemberResolveResult, "'Multiply(1.0,1.0)' is MemberResolveResult");
			Assert.AreEqual("System.Double", result.ResolvedType.FullyQualifiedName, "'Multiply(1.0,1.0)'");
		}
		
		[Test]
		public void CTorOverloadLookupTest()
		{
			string program = @"class A {
	void Method() {
		
	}
	
	static A() {}
	A() {}
	A(int intVal) {}
	A(double dblVal) {}
}
";
			ResolveResult result = Resolve(program, "new A()", 3);
			Assert.IsNotNull(result);
			IMethod m = (IMethod)((MemberResolveResult)result).ResolvedMember;
			Assert.IsFalse(m.IsStatic, "new A() is static");
			Assert.AreEqual(0, m.Parameters.Count, "new A() parameter count");
			
			result = Resolve(program, "new A(10)", 3);
			Assert.IsNotNull(result);
			m = (IMethod)((MemberResolveResult)result).ResolvedMember;
			Assert.AreEqual(1, m.Parameters.Count, "new A(10) parameter count");
			Assert.AreEqual("intVal", m.Parameters[0].Name, "new A(10) parameter");
			
			result = Resolve(program, "new A(11.1)", 3);
			Assert.IsNotNull(result);
			m = (IMethod)((MemberResolveResult)result).ResolvedMember;
			Assert.AreEqual(1, m.Parameters.Count, "new A(11.1) parameter count");
			Assert.AreEqual("dblVal", m.Parameters[0].Name, "new A(11.1) parameter");
		}
		
		[Test]
		public void DefaultCTorOverloadLookupTest()
		{
			string program = @"class A {
	void Method() {
		
	}
}
";
			ResolveResult result = Resolve(program, "new A()", 3);
			Assert.IsNotNull(result);
			IMethod m = (IMethod)((MemberResolveResult)result).ResolvedMember;
			Assert.IsNotNull(m);
		}
		
		[Test]
		public void AnonymousMethodParameters()
		{
			string program = @"using System;
class A {
	void Method() {
		SomeEvent += delegate(object sender, EventArgs e) {
			
		};
	} }
";
			ResolveResult result = Resolve(program, "e", 5);
			Assert.IsNotNull(result);
			Assert.IsTrue(result is LocalResolveResult);
			Assert.AreEqual("System.EventArgs", result.ResolvedType.FullyQualifiedName);
		}
		
		[Test]
		public void DefaultTypeCSharp()
		{
			string program = @"class A {
	void Method() {
		
	} }
";
			ResolveResult result = Resolve(program, "int", 3);
			Assert.IsNotNull(result);
			Assert.IsTrue(result is TypeResolveResult);
			Assert.AreEqual("System.Int32", result.ResolvedType.FullyQualifiedName);
		}
		
		[Test]
		public void DefaultTypeVB()
		{
			string program = @"Class A
	Sub Method()
		
	End Sub
End Class
";
			ResolveResult result = ResolveVB(program, "inTeGer", 3);
			Assert.IsNotNull(result);
			Assert.IsTrue(result is TypeResolveResult);
			Assert.AreEqual("System.Int32", result.ResolvedType.FullyQualifiedName);
		}
		
		// PrimitiveTypeOutsideClass and OtherTypeOutsideClass
		// are necessary for delegate declarations and class inheritance
		// (because "outside" is everything before {, so the reference to the
		// base class is outside the class)
		[Test]
		public void PrimitiveTypeOutsideClass()
		{
			string program = @"class A {
	
}

class B {
	
}
";
			ResolveResult result = Resolve(program, "int", 4);
			Assert.IsNotNull(result);
			Assert.IsTrue(result is TypeResolveResult);
			Assert.AreEqual("System.Int32", result.ResolvedType.FullyQualifiedName);
		}
		
		[Test]
		public void OtherTypeOutsideClass()
		{
			string program = @"using System;
class A {
	
}

class B {
	
}
";
			ResolveResult result = Resolve(program, "Activator", 5);
			Assert.IsNotNull(result);
			Assert.IsTrue(result is TypeResolveResult);
			Assert.AreEqual("System.Activator", result.ResolvedType.FullyQualifiedName);
		}

		[Test]
		public void FullyQualifiedTypeOutsideClass()
		{
			string program = @"class A {
	
}

class B {
	
}
";
			ResolveResult result = Resolve(program, "System.Activator", 4);
			Assert.IsNotNull(result);
			Assert.IsTrue(result is TypeResolveResult);
			Assert.AreEqual("System.Activator", result.ResolvedType.FullyQualifiedName);
		}
		
		[Test]
		public void InnerClassTest()
		{
			string program = @"using System;
class A {
	
}
";
			ResolveResult result = Resolve(program, "Environment.SpecialFolder", 3);
			Assert.IsNotNull(result);
			Assert.IsTrue(result is TypeResolveResult);
			Assert.AreEqual("System.Environment.SpecialFolder", result.ResolvedType.FullyQualifiedName);
		}
		#endregion
		
		#region Import namespace tests
		[Test]
		public void NamespacePreferenceTest()
		{
			// Classes in the current namespace are preferred over classes from
			// imported namespaces
			string program = @"using System;
namespace Testnamespace {
class A {
	
}

class Activator {
	
}
}
";
			ResolveResult result = Resolve(program, "Activator", 4);
			Assert.IsNotNull(result);
			Assert.IsTrue(result is TypeResolveResult);
			Assert.AreEqual("Testnamespace.Activator", result.ResolvedType.FullyQualifiedName);
		}
		
		[Test]
		public void ImportedSubnamespaceTestCSharp()
		{
			// using an import in this way is not possible in C#
			string program = @"using System;
class A {
	void Test() {
		Collections.ArrayList a;
		
	}
}
";
			ResolveResult result = Resolve(program, "Collections.ArrayList", 4);
			Assert.IsNull(result, "Collections.ArrayList should not resolve");
			LocalResolveResult local = Resolve(program, "a", 5) as LocalResolveResult;
			Assert.IsNotNull(local, "a should resolve to a local variable");
			Assert.AreEqual("Collections.ArrayList", local.ResolvedType.FullyQualifiedName,
			                "the full type should not be resolved");
		}
		
		[Test]
		public void ImportedSubnamespaceTestVBNet()
		{
			// using an import this way IS possible in VB.NET
			string program = @"Imports System
Class A
	Sub Test()
		Dim a As Collections.ArrayList
	
	End Sub
End Class
";
			TypeResolveResult type = ResolveVB(program, "Collections.ArrayList", 4) as TypeResolveResult;
			Assert.IsNotNull(type, "Collections.ArrayList should resolve to a type");
			Assert.AreEqual("System.Collections.ArrayList", type.ResolvedClass.FullyQualifiedName, "TypeResolveResult");
			LocalResolveResult local = ResolveVB(program, "a", 5) as LocalResolveResult;
			Assert.IsNotNull(local, "a should resolve to a local variable");
			Assert.AreEqual("System.Collections.ArrayList", local.ResolvedType.FullyQualifiedName,
			                "the full type should be resolved");
		}
		
		[Test]
		public void ImportAliasTest()
		{
			string program = @"using System.Collections = COL;
class A {
	void Test() {
		COL.ArrayList a;
		
	}
}
";
			TypeResolveResult type = Resolve(program, "COL.ArrayList", 4) as TypeResolveResult;
			Assert.IsNotNull(type, "COL.ArrayList should resolve to a type");
			Assert.AreEqual("System.Collections.ArrayList", type.ResolvedClass.FullyQualifiedName, "TypeResolveResult");
			LocalResolveResult local = Resolve(program, "a", 5) as LocalResolveResult;
			Assert.IsNotNull(local, "a should resolve to a local variable");
			Assert.AreEqual("System.Collections.ArrayList", local.ResolvedType.FullyQualifiedName,
			                "the full type should be resolved");
		}
		
		[Test]
		public void ImportAliasNamespaceResolveTest()
		{
			NamespaceResolveResult ns;
			string program = "using System.Collections = COL;\r\nclass A {\r\n}\r\n";
			ns = Resolve(program, "COL", 3) as NamespaceResolveResult;
			Assert.AreEqual("System.Collections", ns.Name, "COL");
			ns = Resolve(program, "COL.Generic", 3) as NamespaceResolveResult;
			Assert.AreEqual("System.Collections.Generic", ns.Name, "COL.Generic");
		}
		#endregion
		
		#region Visibility tests
		[Test]
		public void PrivateMemberTest()
		{
			string program = @"using System;
class A {
	void TestMethod(B b) {
		
	}
}
class B {
	int member;
}
";
			ResolveResult result = Resolve(program, "b", 4);
			Assert.IsNotNull(result);
			ArrayList cd = result.GetCompletionData(lastPC);
			Assert.IsFalse(MemberExists(cd, "member"), "member should not be in completion lookup");
			result = Resolve(program, "b.member", 4);
			Assert.IsNotNull(result, "member should be found even though it is not visible!");
		}
		
		[Test]
		public void ProtectedVisibleMemberTest()
		{
			string program = @"using System;
class A : B {
	void TestMethod(B b) {
		
	}
}
class B {
	protected int member;
}
";
			ResolveResult result = Resolve(program, "b", 4);
			Assert.IsNotNull(result);
			ArrayList cd = result.GetCompletionData(lastPC);
			Assert.IsTrue(MemberExists(cd, "member"), "member should be in completion lookup");
			result = Resolve(program, "b.member", 4);
			Assert.IsNotNull(result, "member should be found!");
		}
		
		[Test]
		public void ProtectedInvisibleMemberTest()
		{
			string program = @"using System;
class A {
	void TestMethod(B b) {
		
	}
}
class B {
	protected int member;
}
";
			ResolveResult result = Resolve(program, "b", 4);
			Assert.IsNotNull(result);
			ArrayList cd = result.GetCompletionData(lastPC);
			Assert.IsFalse(MemberExists(cd, "member"), "member should not be in completion lookup");
			result = Resolve(program, "b.member", 4);
			Assert.IsNotNull(result, "member should be found even though it is not visible!");
		}
		
		bool MemberExists(ArrayList members, string name)
		{
			foreach (object o in members) {
				IMember m = o as IMember;
				if (m.Name == name) return true;
			}
			return false;
		}
		#endregion
	}
}
