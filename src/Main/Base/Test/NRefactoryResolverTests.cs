// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.IO;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Dom.NRefactoryResolver;
using ICSharpCode.SharpDevelop.Project;
using NUnit.Framework;

namespace ICSharpCode.SharpDevelop.Tests
{
	[TestFixture]
	public class NRefactoryResolverTests
	{
		ProjectContentRegistry projectContentRegistry = ParserService.DefaultProjectContentRegistry;
		
		#region Test helper methods
		ICompilationUnit Parse(string fileName, string fileContent)
		{
			ICSharpCode.NRefactory.IParser p = ICSharpCode.NRefactory.ParserFactory.CreateParser(ICSharpCode.NRefactory.SupportedLanguage.CSharp, new StringReader(fileContent));
			p.ParseMethodBodies = false;
			p.Parse();
			DefaultProjectContent pc = new DefaultProjectContent();
			pc.ReferencedContents.Add(projectContentRegistry.Mscorlib);
			pc.ReferencedContents.Add(projectContentRegistry.GetProjectContentForReference("System.Windows.Forms", "System.Windows.Forms"));
			HostCallback.GetCurrentProjectContent = delegate {
				return pc;
			};
			lastPC = pc;
			NRefactoryASTConvertVisitor visitor = new NRefactoryASTConvertVisitor(pc);
			visitor.VisitCompilationUnit(p.CompilationUnit, null);
			visitor.Cu.FileName = fileName;
			Assert.AreEqual(0, p.Errors.Count, "Parse error preparing compilation unit");
			visitor.Cu.ErrorsDuringCompile = p.Errors.Count > 0;
			foreach (IClass c in visitor.Cu.Classes) {
				pc.AddClassToNamespaceList(c);
			}
			
			return visitor.Cu;
		}
		
		public IProjectContent lastPC;
		
		ICompilationUnit ParseVB(string fileName, string fileContent)
		{
			ICSharpCode.NRefactory.IParser p = ICSharpCode.NRefactory.ParserFactory.CreateParser(ICSharpCode.NRefactory.SupportedLanguage.VBNet, new StringReader(fileContent));
			p.ParseMethodBodies = false;
			p.Parse();
			DefaultProjectContent pc = new DefaultProjectContent();
			HostCallback.GetCurrentProjectContent = delegate {
				return pc;
			};
			pc.ReferencedContents.Add(projectContentRegistry.Mscorlib);
			pc.ReferencedContents.Add(projectContentRegistry.GetProjectContentForReference("System.Windows.Forms", "System.Windows.Forms"));
			pc.Language = LanguageProperties.VBNet;
			lastPC = pc;
			NRefactoryASTConvertVisitor visitor = new NRefactoryASTConvertVisitor(pc);
			visitor.VisitCompilationUnit(p.CompilationUnit, null);
			visitor.Cu.FileName = fileName;
			Assert.AreEqual(0, p.Errors.Count, "Parse error preparing compilation unit");
			visitor.Cu.ErrorsDuringCompile = p.Errors.Count > 0;
			foreach (IClass c in visitor.Cu.Classes) {
				pc.AddClassToNamespaceList(c);
			}
			
			return visitor.Cu;
		}
		
		void AddCompilationUnit(ICompilationUnit parserOutput, string fileName)
		{
			HostCallback.GetParseInformation = ParserService.GetParseInformation;
			ParserService.UpdateParseInformation(parserOutput, fileName, false, false);
		}
		
		public ResolveResult Resolve(string program, string expression, int line)
		{
			AddCompilationUnit(Parse("a.cs", program), "a.cs");
			
			NRefactoryResolver resolver = new NRefactoryResolver(lastPC, LanguageProperties.CSharp);
			return resolver.Resolve(new ExpressionResult(expression),
			                        line, 0,
			                        "a.cs",
			                        program);
		}
		
		public ResolveResult ResolveVB(string program, string expression, int line)
		{
			AddCompilationUnit(ParseVB("a.vb", program), "a.vb");
			
			NRefactoryResolver resolver = new NRefactoryResolver(lastPC, LanguageProperties.VBNet);
			return resolver.Resolve(new ExpressionResult(expression),
			                        line, 0,
			                        "a.vb",
			                        program);
		}
		
		public T Resolve<T>(string program, string expression, int line) where T : ResolveResult
		{
			ResolveResult rr = Resolve(program, expression, line);
			Assert.IsNotNull(rr, "Resolve returned null");
			Assert.IsTrue(rr is T, "result is " + typeof(T).Name);
			return (T)rr;
		}
		
		public T ResolveVB<T>(string program, string expression, int line) where T : ResolveResult
		{
			ResolveResult rr = ResolveVB(program, expression, line);
			Assert.IsNotNull(rr, "Resolve returned null");
			Assert.IsTrue(rr is T, "result is " + typeof(T).Name);
			return (T)rr;
		}
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
			ResolveResult result = ResolveVB<LocalResolveResult>(program, "a", 4);
			Assert.AreEqual("System.String", result.ResolvedType.FullyQualifiedName);
			
			result = ResolveVB<LocalResolveResult>(program, "b", 4);
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
			ResolveResult result = ResolveVB<LocalResolveResult>(program, "c", 4);
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
			ResolveResult result = ResolveVB<LocalResolveResult>(program, "t", 4);
			
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
			ResolveResult result = Resolve<MemberResolveResult>(arrayListConflictProgram, "arrayList", 4);
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
			ResolveResult result = Resolve<LocalResolveResult>(program, "a", 4);
			ArrayList arr = result.GetCompletionData(lastPC);
			Assert.IsNotNull(arr, "arr");
			bool m1 = false;
			bool m2 = false;
			bool disp = false;
			bool getType = false;
			foreach (IMethod m in arr) {
				if (m.Name == "Method1")
					m1 = true;
				if (m.Name == "Method2")
					m2 = true;
				if (m.Name == "Dispose")
					disp = true;
				if (m.Name == "GetType")
					getType = true;
			}
			Assert.IsTrue(m1, "Method1 not found");
			Assert.IsTrue(m2, "Method2 not found");
			Assert.IsTrue(disp, "Dispose not found");
			Assert.IsTrue(getType, "GetType not found");
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
			ResolveResult result = Resolve<MemberResolveResult>(program, "TargetMethod()", 3);
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
			ResolveResult result = Resolve<MemberResolveResult>(program, "this.TargetMethod()", 3);
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
			MemberResolveResult result = Resolve<MemberResolveResult>(program, "TestEvent(this, EventArgs.Empty)", 4);
			Assert.AreEqual("A.TestEvent", result.ResolvedMember.FullyQualifiedName);
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
			Assert.AreSame(VoidReturnType.Instance, result.ResolvedType, result.ResolvedType.ToString());
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
			MemberResolveResult result = Resolve<MemberResolveResult>(program, "this.TestEvent(this, EventArgs.Empty)", 4);
			Assert.AreEqual("A.TestEvent", result.ResolvedMember.FullyQualifiedName);
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
			ResolveResult result = Resolve<LocalResolveResult>(program, "eh(this, new ResolveEventArgs())", 5);
			Assert.AreEqual("eh", (result as LocalResolveResult).Field.Name);
			
			result = Resolve<MemberResolveResult>(program, "eh(this, new ResolveEventArgs()).GetType(\"bla\")", 5);
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
			ResolveResult result = Resolve<MemberResolveResult>(program, "Multiply(1, 1)", 3);
			Assert.AreEqual("System.Int32", result.ResolvedType.FullyQualifiedName, "'Multiply(1,1)'");
			
			result = Resolve<MemberResolveResult>(program, "Multiply(1.0, 1.0)", 3);
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
			MemberResolveResult result = Resolve<MemberResolveResult>(program, "new A()", 3);
			IMethod m = (IMethod)result.ResolvedMember;
			Assert.IsFalse(m.IsStatic, "new A() is static");
			Assert.AreEqual(0, m.Parameters.Count, "new A() parameter count");
			
			result = Resolve<MemberResolveResult>(program, "new A(10)", 3);
			m = (IMethod)result.ResolvedMember;
			Assert.AreEqual(1, m.Parameters.Count, "new A(10) parameter count");
			Assert.AreEqual("intVal", m.Parameters[0].Name, "new A(10) parameter");
			
			result = Resolve<MemberResolveResult>(program, "new A(11.1)", 3);
			m = (IMethod)result.ResolvedMember;
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
			MemberResolveResult result = Resolve<MemberResolveResult>(program, "new A()", 3);
			IMethod m = (IMethod)result.ResolvedMember;
			Assert.IsNotNull(m);
		}
		
		[Test]
		public void ValueInsideSetterTest()
		{
			string program = @"class A {
	public string Property {
		set {
			
		}
	}
}
";
			LocalResolveResult result = Resolve<LocalResolveResult>(program, "value", 4);
			Assert.AreEqual("System.String", result.ResolvedType.FullyQualifiedName);
			MemberResolveResult mrr = Resolve<MemberResolveResult>(program, "value.ToString()", 4);
			Assert.AreEqual("System.String.ToString", mrr.ResolvedMember.FullyQualifiedName);
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
			ResolveResult result = Resolve<LocalResolveResult>(program, "e", 5);
			Assert.AreEqual("System.EventArgs", result.ResolvedType.FullyQualifiedName);
		}
		
		[Test]
		public void DefaultTypeCSharp()
		{
			string program = @"class A {
	void Method() {
		
	} }
";
			ResolveResult result = Resolve<TypeResolveResult>(program, "int", 3);
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
			ResolveResult result = ResolveVB<TypeResolveResult>(program, "inTeGer", 3);
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
			ResolveResult result = Resolve<TypeResolveResult>(program, "int", 4);
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
			ResolveResult result = Resolve<TypeResolveResult>(program, "Activator", 5);
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
			ResolveResult result = Resolve<TypeResolveResult>(program, "System.Activator", 4);
			Assert.AreEqual("System.Activator", result.ResolvedType.FullyQualifiedName);
		}
		
		[Test]
		public void InnerClassTest()
		{
			string program = @"using System;
class A {
	
}
";
			ResolveResult result = Resolve<TypeResolveResult>(program, "Environment.SpecialFolder", 3);
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
			ResolveResult result = Resolve<TypeResolveResult>(program, "Activator", 4);
			Assert.AreEqual("Testnamespace.Activator", result.ResolvedType.FullyQualifiedName);
		}
		
		[Test]
		public void ParentNamespaceTypeLookup()
		{
			// Classes in the current namespace are preferred over classes from
			// imported namespaces
			string program = @"using System;
namespace Root {
  class Alpha {}
}
namespace Root.Child {
  class Beta {
  
  }
}
";
			ResolveResult result = Resolve<TypeResolveResult>(program, "Alpha", 7);
			Assert.AreEqual("Root.Alpha", result.ResolvedType.FullyQualifiedName);
		}
		
		[Test]
		public void ParentNamespaceCtrlSpace()
		{
			// Classes in the current namespace are preferred over classes from
			// imported namespaces
			string program = @"using System;
namespace Root {
  class Alpha {}
}
namespace Root.Child {
  class Beta {
  
  }
}
";
			AddCompilationUnit(Parse("a.cs", program), "a.cs");
			
			NRefactoryResolver resolver = new NRefactoryResolver(lastPC, LanguageProperties.CSharp);
			ArrayList m = resolver.CtrlSpace(7, 0, "a.cs", program, ExpressionContext.Default);
			Assert.IsTrue(TypeExists(m, "Beta"), "Meta must exist");
			Assert.IsTrue(TypeExists(m, "Alpha"), "Alpha must exist");
		}
		
		bool TypeExists(ArrayList m, string name)
		{
			foreach (object o in m) {
				IClass c = o as IClass;
				if (c != null && c.Name == name)
					return true;
			}
			return false;
		}
		
		[Test]
		public void ImportedSubnamespaceTestCSharp()
		{
			// using an import in this way is not possible in C#
			string program = @"using System;
class TestClass {
	void Test() {
		Collections.ArrayList a;
		
	}
}
";
			ResolveResult result = Resolve(program, "Collections.ArrayList", 4);
			Assert.IsNull(result, "Collections.ArrayList should not resolve");
			LocalResolveResult local = Resolve<LocalResolveResult>(program, "a", 5);
			Assert.IsNull(local.ResolvedType, "the full type should not be resolved");
		}
		
		[Test]
		public void ImportedSubnamespaceTestVBNet()
		{
			// using an import this way IS possible in VB.NET
			string program = @"Imports System
Class TestClass
	Sub Test()
		Dim a As Collections.ArrayList
		
	End Sub
End Class
";
			TypeResolveResult type = ResolveVB<TypeResolveResult>(program, "Collections.ArrayList", 4);
			Assert.AreEqual("System.Collections.ArrayList", type.ResolvedClass.FullyQualifiedName, "TypeResolveResult");
			LocalResolveResult local = ResolveVB<LocalResolveResult>(program, "a", 5);
			Assert.AreEqual("System.Collections.ArrayList", local.ResolvedType.FullyQualifiedName,
			                "the full type should be resolved");
		}
		
		[Test]
		public void ImportAliasTest()
		{
			string program = @"using COL = System.Collections;
class TestClass {
	void Test() {
		COL.ArrayList a;
		
	}
}
";
			TypeResolveResult type = Resolve<TypeResolveResult>(program, "COL.ArrayList", 4);
			Assert.IsNotNull(type, "COL.ArrayList should resolve to a type");
			Assert.AreEqual("System.Collections.ArrayList", type.ResolvedClass.FullyQualifiedName, "TypeResolveResult");
			LocalResolveResult local = Resolve<LocalResolveResult>(program, "a", 5);
			Assert.AreEqual("System.Collections.ArrayList", local.ResolvedType.FullyQualifiedName,
			                "the full type should be resolved");
		}
		
		[Test]
		public void ImportAliasNamespaceResolveTest()
		{
			NamespaceResolveResult ns;
			string program = "using COL = System.Collections;\r\nclass A {\r\n\r\n}\r\n";
			ns = Resolve<NamespaceResolveResult>(program, "COL", 3);
			Assert.AreEqual("System.Collections", ns.Name, "COL");
			ns = Resolve<NamespaceResolveResult>(program, "COL.Generic", 3);
			Assert.AreEqual("System.Collections.Generic", ns.Name, "COL.Generic");
		}
		
		[Test]
		public void ImportAliasClassResolveTest()
		{
			string program = @"using COL = System.Collections.ArrayList;
class TestClass {
	void Test() {
		COL a = new COL();
		
	}
}
";
			TypeResolveResult rr = Resolve<TypeResolveResult>(program, "COL", 4);
			Assert.AreEqual("System.Collections.ArrayList", rr.ResolvedClass.FullyQualifiedName, "COL");
			LocalResolveResult lr = Resolve<LocalResolveResult>(program, "a", 5);
			Assert.AreEqual("System.Collections.ArrayList", lr.ResolvedType.FullyQualifiedName, "a");
		}
		#endregion
		
		#region Import class tests
		const string importClassProgram = @"Imports System
Imports System.Math

Class TestClass
	Sub Main()
		
	End Sub
End Class
";
		
		[Test]
		public void TestImportClassMember()
		{
			MemberResolveResult mrr = ResolveVB<MemberResolveResult>(importClassProgram, "Pi", 6);
			Assert.AreEqual("System.Math.PI", mrr.ResolvedMember.FullyQualifiedName);
			mrr = ResolveVB<MemberResolveResult>(importClassProgram, "Pi.ToString()", 6);
			Assert.AreEqual("System.Double.ToString", mrr.ResolvedMember.FullyQualifiedName);
		}
		
		[Test]
		public void TestImportClassMethod()
		{
			MemberResolveResult mrr = ResolveVB<MemberResolveResult>(importClassProgram, "Sin(3)", 6);
			Assert.AreEqual("System.Math.Sin", mrr.ResolvedMember.FullyQualifiedName);
			mrr = ResolveVB<MemberResolveResult>(importClassProgram, "Sin(3).ToString()", 6);
			Assert.AreEqual("System.Double.ToString", mrr.ResolvedMember.FullyQualifiedName);
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
		
		[Test]
		public void OverriddenMemberVisibilityTest()
		{
			string program = @"using System;
  public abstract class GrandParent {
    protected abstract void OverrideMe();
  }
  public class Parent: GrandParent {
      protected override void OverrideMe() {
      }
  }
  public class Child: Parent {
  }
";
			ResolveResult result = Resolve(program, "(Child)someVar", 6);
			Assert.AreEqual("Child", result.ResolvedType.FullyQualifiedName);
			int count = 0;
			foreach (IMethod m in result.ResolvedType.GetMethods()) {
				if (m.Name == "OverrideMe")
					count += 1;
			}
			Assert.AreEqual(1, count);
			count = 0;
			foreach (object o in result.GetCompletionData(lastPC)) {
				IMethod m = o as IMethod;
				if (m != null && m.Name == "OverrideMe")
					count += 1;
			}
			Assert.AreEqual(1, count);
		}
		#endregion
		
		#region MixedType tests
		const string mixedTypeTestProgram = @"using System;
class A {
	void TestMethod() {
		
	}
	public Project Project { get { return new Project(); } }
	public Project OtherName { get { return new Project(); } }
}
class Project {
  public static string Static;
  public int Instance;
}
namespace OtherName { class Bla { } }
";
		
		[Test]
		public void MixedResolveResultTest()
		{
			ResolveResult result = Resolve(mixedTypeTestProgram, "Project", 4);
			Assert.IsInstanceOfType(typeof(MixedResolveResult), result);
			MixedResolveResult mrr = (MixedResolveResult)result;
			Assert.IsInstanceOfType(typeof(MemberResolveResult), mrr.PrimaryResult);
			Assert.AreEqual("Project", mrr.TypeResult.ResolvedClass.Name);
		}
		
		[Test]
		public void MixedStaticAccessTest()
		{
			ResolveResult result = Resolve(mixedTypeTestProgram, "Project.Static", 4);
			Assert.IsInstanceOfType(typeof(MemberResolveResult), result);
			Assert.AreEqual("Static", (result as MemberResolveResult).ResolvedMember.Name);
		}
		
		[Test]
		public void MixedInstanceAccessTest()
		{
			ResolveResult result = Resolve(mixedTypeTestProgram, "Project.Instance", 4);
			Assert.IsInstanceOfType(typeof(MemberResolveResult), result);
			Assert.AreEqual("Instance", (result as MemberResolveResult).ResolvedMember.Name);
		}
		
		[Test]
		public void NamespaceMixResolveResultTest()
		{
			ResolveResult result = Resolve(mixedTypeTestProgram, "OtherName", 4);
			Assert.IsInstanceOfType(typeof(MemberResolveResult), result);
			Assert.AreEqual("OtherName", (result as MemberResolveResult).ResolvedMember.Name);
		}
		
		[Test]
		public void NamespaceMixMemberAccessTest()
		{
			ResolveResult result = Resolve(mixedTypeTestProgram, "OtherName.Instance", 4);
			Assert.IsInstanceOfType(typeof(MemberResolveResult), result);
			Assert.AreEqual("Instance", (result as MemberResolveResult).ResolvedMember.Name);
		}
		#endregion
	}
}
