// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.IO;
using System.Linq;

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
		public ICompilationUnit Parse(string fileName, string fileContent)
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
			pc.ReferencedContents.Add(projectContentRegistry.GetProjectContentForReference("Microsoft.VisualBasic", "Microsoft.VisualBasic"));
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
		
		ParseInformation AddCompilationUnit(ICompilationUnit parserOutput, string fileName)
		{
			return ParserService.RegisterParseInformation(fileName, parserOutput);
		}
		
		public ResolveResult Resolve(string program, string expression, int line)
		{
			return Resolve(program, expression, line, 0, ExpressionContext.Default);
		}
		
		public ResolveResult Resolve(string program, string expression, int line, int column, ExpressionContext context)
		{
			ParseInformation parseInfo = AddCompilationUnit(Parse("a.cs", program), "a.cs");
			
			NRefactoryResolver resolver = new NRefactoryResolver(LanguageProperties.CSharp);
			ExpressionResult expressionResult = new ExpressionResult(expression, new DomRegion(line, column), context, null);
			return resolver.Resolve(expressionResult, parseInfo, program);
		}
		
		public ResolveResult ResolveVB(string program, string expression, int line)
		{
			ParseInformation parseInfo = AddCompilationUnit(ParseVB("a.vb", program), "a.vb");
			
			NRefactoryResolver resolver = new NRefactoryResolver(LanguageProperties.VBNet);
			ExpressionResult expressionResult = new ExpressionResult(expression, new DomRegion(line, 0), ExpressionContext.Default, null);
			return resolver.Resolve(expressionResult, parseInfo, program);
		}
		
		public T Resolve<T>(string program, string expression, int line) where T : ResolveResult
		{
			ResolveResult rr = Resolve(program, expression, line);
			Assert.IsNotNull(rr, "Resolve returned null (expression=" + expression + ")");
			Assert.AreEqual(typeof(T), rr.GetType());
			return (T)rr;
		}
		
		public T Resolve<T>(string program, string expression, int line, int column, ExpressionContext context) where T : ResolveResult
		{
			ResolveResult rr = Resolve(program, expression, line, column, context);
			Assert.IsNotNull(rr, "Resolve returned null (expression=" + expression + ")");
			Assert.AreEqual(typeof(T), rr.GetType());
			return (T)rr;
		}
		
		public T ResolveVB<T>(string program, string expression, int line) where T : ResolveResult
		{
			ResolveResult rr = ResolveVB(program, expression, line);
			Assert.IsNotNull(rr, "Resolve returned null (expression=" + expression + ")");
			Assert.AreEqual(typeof(T), rr.GetType());
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
		public void GenericObjectCreation()
		{
			string program = @"using System.Collections.Generic;
class A {
	static void Main() {
		
	}
}
";
			MemberResolveResult result = Resolve<MemberResolveResult>(program, "new List<string>()", 4);
			Assert.AreEqual("System.Collections.Generic.List.#ctor", result.ResolvedMember.FullyQualifiedName);
			Assert.AreEqual("System.Collections.Generic.List{System.String}", result.ResolvedType.DotNetName);
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
		public void OverriddenMethodCallTest()
		{
			string program = @"class A {
	void Method() {
		
	}
	
	public abstract int GetRandomNumber();
}
class B : A {
	public override int GetRandomNumber() {
		return 4; // chosen by fair dice roll.
		          // guaranteed to be random
	}
}
";
			MemberResolveResult result = Resolve<MemberResolveResult>(program, "new B().GetRandomNumber()", 3);
			Assert.AreEqual("B.GetRandomNumber", result.ResolvedMember.FullyQualifiedName);
		}
		
		[Test]
		public void OverriddenMethodCallTest2()
		{
			string program = @"class A {
	void Method() {
		
	}
	
	public abstract int GetRandomNumber(string a, A b);
}
class B : A {
	public override int GetRandomNumber(string b, A a) {
		return 4; // chosen by fair dice roll.
		          // guaranteed to be random
	}
}
";
			MemberResolveResult result = Resolve<MemberResolveResult>(program, "new B().GetRandomNumber(\"x\", this)", 3);
			Assert.AreEqual("B.GetRandomNumber", result.ResolvedMember.FullyQualifiedName);
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
		public void MethodGroupResolveTest()
		{
			string program = @"class A {
	void Method() {
		
	}
	
	void TargetMethod(int a) { }
	void TargetMethod<T>(T a) { }
}
";
			MethodGroupResolveResult result = Resolve<MethodGroupResolveResult>(program, "TargetMethod", 3);
			Assert.AreEqual("TargetMethod", result.Name);
			Assert.AreEqual(2, result.Methods[0].Count);
			
			result = Resolve<MethodGroupResolveResult>(program, "TargetMethod<string>", 3);
			Assert.AreEqual("TargetMethod", result.Name);
			Assert.AreEqual(1, result.Methods[0].Count);
			Assert.AreEqual("System.String", result.GetMethodIfSingleOverload().Parameters[0].ReturnType.FullyQualifiedName);
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
			LocalResolveResult result = Resolve<LocalResolveResult>(program, "eh(this, new ResolveEventArgs())", 5);
			Assert.AreEqual("eh", result.Field.Name);
			
			MemberResolveResult mrr = Resolve<MemberResolveResult>(program, "eh(this, new ResolveEventArgs()).GetType(\"bla\")", 5);
			Assert.AreEqual("System.Reflection.Module.GetType", mrr.ResolvedMember.FullyQualifiedName);
		}
		
		[Test]
		public void DelegateReturnedFromMethodCallTest()
		{
			string program = @"using System;
class A {
	void Method() {
		
	}
	abstract Predicate<string> GetHandler();
}
";
			ResolveResult result = Resolve<MemberResolveResult>(program, "GetHandler()(abc)", 4);
			Assert.AreEqual("System.Boolean", result.ResolvedType.FullyQualifiedName);
			
			MemberResolveResult mrr = Resolve<MemberResolveResult>(program, "GetHandler()(abc).ToString()", 4);
			Assert.AreEqual("System.Boolean.ToString", mrr.ResolvedMember.FullyQualifiedName);
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
			Assert.AreEqual("A", result.ResolvedType.FullyQualifiedName);
			
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
			Assert.AreEqual("A", result.ResolvedType.FullyQualifiedName);
			
			ArrayList ar = result.GetCompletionData(result.CallingClass.ProjectContent);
			Assert.IsTrue(ContainsMember(ar, "A.Method"));
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
			
			int valueParameterCount = 0;
			foreach (object o in CtrlSpaceResolveCSharp(program, 4, ExpressionContext.Default)) {
				IField f = o as IField;
				if (f != null && f.Name == "value") {
					valueParameterCount++;
					Assert.IsTrue(f.IsParameter);
					Assert.AreEqual("System.String", f.ReturnType.FullyQualifiedName);
				}
			}
			Assert.IsTrue(valueParameterCount == 1);
		}
		
		[Test]
		public void ValueInsideEventTest()
		{
			string program = @"using System; class A {
	public event EventHandler Ev {
		add {
			
		}
		remove {}
	}
}
";
			LocalResolveResult result = Resolve<LocalResolveResult>(program, "value", 4);
			Assert.AreEqual("System.EventHandler", result.ResolvedType.FullyQualifiedName);
			MemberResolveResult mrr = Resolve<MemberResolveResult>(program, "value.DynamicInvoke(null)", 4);
			Assert.AreEqual("System.Delegate.DynamicInvoke", mrr.ResolvedMember.FullyQualifiedName);
			
			int valueParameterCount = 0;
			foreach (object o in CtrlSpaceResolveCSharp(program, 4, ExpressionContext.Default)) {
				IField f = o as IField;
				if (f != null && f.Name == "value") {
					valueParameterCount++;
					Assert.IsTrue(f.IsParameter);
					Assert.AreEqual("System.EventHandler", f.ReturnType.FullyQualifiedName);
				}
			}
			Assert.IsTrue(valueParameterCount == 1);
		}
		
		[Test]
		public void ValueInsideIndexerSetterTest()
		{
			string program = @"using System; class A {
		public string this[int arg] {
			set {
				
			}
		}
}
";
			LocalResolveResult result = Resolve<LocalResolveResult>(program, "value", 4);
			Assert.AreEqual("System.String", result.ResolvedType.FullyQualifiedName);
			MemberResolveResult mrr = Resolve<MemberResolveResult>(program, "value.ToString()", 4);
			Assert.AreEqual("System.String.ToString", mrr.ResolvedMember.FullyQualifiedName);
			
			int valueParameterCount = 0;
			foreach (object o in CtrlSpaceResolveCSharp(program, 4, ExpressionContext.Default)) {
				IField f = o as IField;
				if (f != null && f.Name == "value") {
					valueParameterCount++;
					Assert.IsTrue(f.IsParameter);
					Assert.AreEqual("System.String", f.ReturnType.FullyQualifiedName);
				}
			}
			Assert.IsTrue(valueParameterCount == 1);
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
		public void LoopVariableScopeTest()
		{
			string program = @"using System;
class TestClass {
	void Test() {
		for (int i = 0; i < 10; i++) {
			
		}
		for (long i = 0; i < 10; i++) {
			
		}
	}
}
";
			LocalResolveResult lr = Resolve<LocalResolveResult>(program, "i", 5);
			Assert.AreEqual("System.Int32", lr.ResolvedType.FullyQualifiedName);
			lr = Resolve<LocalResolveResult>(program, "i", 8);
			Assert.AreEqual("System.Int64", lr.ResolvedType.FullyQualifiedName);
		}
		
		[Test]
		public void ShortMaxValueTest()
		{
			string program = @"using System;
class TestClass {
	void Test() {
		
	}
}
";
			ResolveResult rr = Resolve<MemberResolveResult>(program, "short.MaxValue", 4);
			Assert.AreEqual("System.Int16", rr.ResolvedType.FullyQualifiedName);
			
			rr = Resolve<ResolveResult>(program, "(short.MaxValue)", 4);
			Assert.AreEqual("System.Int16", rr.ResolvedType.FullyQualifiedName);
		}
		
		[Test]
		public void VBNetArrayReturnedFromMethodTest()
		{
			string program = @"Module Main
	Sub Main()
		
	End Sub
	Function F() As String()
	End Function
End Module
";
			MemberResolveResult result = ResolveVB<MemberResolveResult>(program, "F()(0)", 3);
			Assert.AreEqual("System.String", result.ResolvedType.FullyQualifiedName);
			Assert.IsFalse(result.ResolvedType.IsArrayReturnType);
			Assert.IsInstanceOfType(typeof(IProperty), result.ResolvedMember);
			Assert.IsTrue((result.ResolvedMember as IProperty).IsIndexer);
		}
		
		[Test]
		public void ConstructorBaseCall()
		{
			string program = @"using System;
class A {
	public A(int a) {}
}
class B : A {
	public B(int a)
		: base(a)  /*7*/
 	{}
}
class C : B {
	public C(int a)
		: base(a)  /*12*/
 	{}
}
";
			MemberResolveResult mrr = Resolve<MemberResolveResult>(program, "new A(2)", 3);
			IMember aCtor = mrr.ResolvedMember;
			Assert.AreEqual("A.#ctor", mrr.ResolvedMember.FullyQualifiedName);
			
			mrr = Resolve<MemberResolveResult>(program, "new B(2)", 3);
			IMember bCtor = mrr.ResolvedMember;
			Assert.AreEqual("B.#ctor", mrr.ResolvedMember.FullyQualifiedName);
			
			mrr = Resolve<MemberResolveResult>(program, "base(a)", 7);
			Assert.AreEqual("A.#ctor", mrr.ResolvedMember.FullyQualifiedName);
			
			mrr = Resolve<MemberResolveResult>(program, "base(a)", 12);
			Assert.AreEqual("B.#ctor", mrr.ResolvedMember.FullyQualifiedName);
			
			// ensure that the reference pointing to the B ctor is not seen as a reference
			// to the A ctor.
			Assert.IsTrue(Refactoring.RefactoringService.IsReferenceToMember(bCtor, mrr));
			Assert.IsFalse(Refactoring.RefactoringService.IsReferenceToMember(aCtor, mrr));
		}
		
		[Test]
		public void ConstructorCallInCreationContext()
		{
			string program = @"using System;
class A {
	public A(int a) {}
}
";
			MemberResolveResult mrr = Resolve<MemberResolveResult>(program, "new A(2)", 3, 0, ExpressionContext.ObjectCreation);
			Assert.AreEqual("A.#ctor", mrr.ResolvedMember.FullyQualifiedName);
			
			mrr = Resolve<MemberResolveResult>(program, "A(2)", 3, 0, ExpressionContext.ObjectCreation);
			Assert.AreEqual("A.#ctor", mrr.ResolvedMember.FullyQualifiedName);
		}
		
		[Test]
		public void VBIndexerCall()
		{
			string program = @"Imports Microsoft.VisualBasic
Class A
	Sub Method()
		Dim c As Collection
		
	End Sub
End Class
";
			MemberResolveResult result = ResolveVB<MemberResolveResult>(program, "c(3)", 5);
			Assert.AreEqual("Microsoft.VisualBasic.Collection.Item", result.ResolvedMember.FullyQualifiedName);
			IProperty p = (IProperty)result.ResolvedMember;
			Assert.IsTrue(p.IsIndexer);
			
			result = ResolveVB<MemberResolveResult>(program, "c.Item(3)", 5);
			Assert.AreEqual("Microsoft.VisualBasic.Collection.Item", result.ResolvedMember.FullyQualifiedName);
			p = (IProperty)result.ResolvedMember;
			Assert.IsTrue(p.IsIndexer);
		}
		
		[Test]
		public void VBPInvokeCall()
		{
			string program = @"Imports Microsoft.VisualBasic
Class A
	Declare Function GetRectangleArea Lib ""..\..\PinvokeLib.dll"" (ByRef rectangle As MyRectangle) As Integer
	Sub Method(r1 As MyRectangle)
		
	End Sub
End Class
";
			MemberResolveResult result = ResolveVB<MemberResolveResult>(program, "GetRectangleArea(r1)", 5);
			Assert.AreEqual("A.GetRectangleArea", result.ResolvedMember.FullyQualifiedName);
			
			IMethod m = (IMethod)result.ResolvedMember;
			Assert.IsTrue(m.IsStatic);
			Assert.AreEqual("System.Int32", m.ReturnType.FullyQualifiedName);
			Assert.AreEqual(1, m.Parameters.Count);
			Assert.AreEqual("rectangle", m.Parameters[0].Name);
			Assert.IsTrue(m.Parameters[0].IsRef);
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
		
		ArrayList CtrlSpaceResolveCSharp(string program, int line, ExpressionContext context)
		{
			ParseInformation parseInfo = AddCompilationUnit(Parse("a.cs", program), "a.cs");
			
			NRefactoryResolver resolver = new NRefactoryResolver(LanguageProperties.CSharp);
			return resolver.CtrlSpace(line, 0, parseInfo, program, context);
		}
		
		[Test]
		public void ParentNamespaceCtrlSpace()
		{
			string program = @"using System;
namespace Root {
  class Alpha {}
}
namespace Root.Child {
  class Beta {
  
  }
}
";
			ArrayList m = CtrlSpaceResolveCSharp(program, 7, ExpressionContext.Default);
			Assert.IsTrue(TypeExists(m, "Beta"), "Beta must exist");
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
		
		[Test]
		public void ResolveNamespaceSD2_863()
		{
			string program = @"using System;
namespace A.C { class D {} }
namespace A.B.C { class D {} }
namespace A.B {
	class TestClass {
		void Test() {
			
		}
	}
}
";
			NamespaceResolveResult nrr = Resolve<NamespaceResolveResult>(program, "C", 7);
			Assert.AreEqual("A.B.C", nrr.Name, "nrr.Name");
			TypeResolveResult trr = Resolve<TypeResolveResult>(program, "C.D", 7);
			Assert.AreEqual("A.B.C.D", trr.ResolvedClass.FullyQualifiedName, "trr.ResolvedClass.FullyQualifiedName");
		}
		
		[Test]
		public void ResolveTypeSD2_863()
		{
			string program = @"using System;
namespace A { class C {} }
namespace A.B {
	class C {}
	class TestClass {
		void Test() {
			
		}
	}
}
";
			TypeResolveResult trr = Resolve<TypeResolveResult>(program, "C", 7);
			Assert.AreEqual("A.B.C", trr.ResolvedClass.FullyQualifiedName, "trr.ResolvedClass.FullyQualifiedName");
		}
		
		[Test]
		public void VBBaseConstructorCall()
		{
			string program = @"Class A
Inherits B
	Sub New()
		
	End Sub
	Sub Test()
	
	End Sub
End Class
Class B
	Sub New(a As String)
	End Sub
End Class
";
			MemberResolveResult mrr = ResolveVB<MemberResolveResult>(program, "mybase.new(\"bb\")", 4);
			Assert.AreEqual("B", mrr.ResolvedMember.DeclaringType.FullyQualifiedName);
			Assert.IsTrue(((IMethod)mrr.ResolvedMember).IsConstructor);
			
			ResolveResult result = ResolveVB<VBBaseOrThisReferenceInConstructorResolveResult>(program, "mybase", 4);
			Assert.AreEqual("B", result.ResolvedType.FullyQualifiedName);
			Assert.IsTrue(ContainsMember(result.GetCompletionData(result.CallingClass.ProjectContent), mrr.ResolvedMember.FullyQualifiedName));
			
			result = ResolveVB<BaseResolveResult>(program, "mybase", 7);
			Assert.AreEqual("B", result.ResolvedType.FullyQualifiedName);
			Assert.IsFalse(ContainsMember(result.GetCompletionData(result.CallingClass.ProjectContent), mrr.ResolvedMember.FullyQualifiedName));
		}
		
		bool ContainsMember(IEnumerable input, string fullMemberName)
		{
			return input.OfType<IMember>().Any(m => m.FullyQualifiedName == fullMemberName);
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
			Assert.IsTrue(result != null && result.IsValid, "member should be found even though it is not visible!");
		}
		
		[Test]
		public void ProtectedVisibleMemberTest()
		{
			string program = @"using System;
class A : B {
	static void TestMethod(A a) {
		
	}
}
class B {
	protected int member;
}
";
			ResolveResult result = Resolve(program, "a", 4);
			Assert.IsNotNull(result);
			ArrayList cd = result.GetCompletionData(lastPC);
			Assert.IsTrue(MemberExists(cd, "member"), "member should be in completion lookup");
			result = Resolve(program, "a.member", 4);
			Assert.IsTrue(result != null && result.IsValid, "member should be found!");
		}
		
		[Test]
		public void ProtectedMemberVisibleWithImplicitThisReferenceTest()
		{
			string program = @"using System;
class A : B {
	void TestMethod(A a) {
		
	}
}
class B {
	protected int member;
}
";
			ArrayList results = CtrlSpaceResolveCSharp(program, 4, ExpressionContext.Default);
			Assert.IsTrue(MemberExists(results, "member"), "member should be in completion lookup");
			ResolveResult result = Resolve(program, "member", 4);
			Assert.IsTrue(result != null && result.IsValid, "member should be found!");
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
			Assert.IsTrue(result != null && result.IsValid, "member should be found even though it is not visible!");
		}
		
		[Test]
		public void ProtectedMemberInvisibleWhenNotUsingReferenceOfCurrentTypeTest()
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
			Assert.IsFalse(MemberExists(cd, "member"), "member should not be in completion lookup");
			result = Resolve(program, "b.member", 4);
			Assert.IsTrue(result != null && result.IsValid, "member should be found even though it is not visible!");
		}
		
		[Test]
		public void ProtectedMemberVisibleWhenUsingBaseReference()
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
			ResolveResult result = Resolve(program, "base", 4);
			Assert.IsNotNull(result);
			ArrayList cd = result.GetCompletionData(lastPC);
			Assert.IsTrue(MemberExists(cd, "member"), "member should be in completion lookup");
			result = Resolve(program, "base.member", 4);
			Assert.IsTrue(result != null && result.IsValid, "member should be found!");
		}
		
		[Test]
		public void ProtectedMethodInvisibleWhenNotUsingReferenceOfCurrentTypeTest()
		{
			string program = @"using System;
class A : B {
	void TestMethod(B b) {
		
	}
}
class B {
	protected int Method();
}
";
			ResolveResult result = Resolve(program, "b", 4);
			Assert.IsNotNull(result);
			ArrayList cd = result.GetCompletionData(lastPC);
			Assert.IsFalse(MemberExists(cd, "Method"), "member should not be in completion lookup");
			result = Resolve(program, "b.Method()", 4);
			Assert.IsTrue(result != null && result.IsValid, "method should be found even though it is invisible!");
		}
		
		[Test]
		public void ProtectedMethodVisibleWhenUsingBaseReference()
		{
			string program = @"using System;
class A : B {
	void TestMethod(B b) {
		
	}
}
class B {
	protected int Method();
}
";
			ResolveResult result = Resolve(program, "base", 4);
			Assert.IsNotNull(result);
			ArrayList cd = result.GetCompletionData(lastPC);
			Assert.IsTrue(MemberExists(cd, "Method"), "member should be in completion lookup");
			result = Resolve(program, "base.Method()", 4);
			Assert.IsTrue(result != null && result.IsValid, "method should be found!");
		}
		
		[Test]
		public void ProtectedMethodVisibleWithImplicitThisReferenceTest()
		{
			string program = @"using System;
class A : B {
	void TestMethod(A a) {
		
	}
}
class B {
	protected int Method();
}
";
			ArrayList results = CtrlSpaceResolveCSharp(program, 4, ExpressionContext.Default);
			Assert.IsTrue(MemberExists(results, "Method"), "method should be in completion lookup");
			ResolveResult result = Resolve(program, "Method()", 4);
			Assert.IsTrue(result != null && result.IsValid, "method should be found!");
		}
		
		[Test]
		public void ProtectedMethodVisibleWithImplicitThisReferenceConflictsWithClassNameTest()
		{
			string program = @"using System;
class A : B {
	void TestMethod(A a) {
		
	}
}
class B {
	protected int Method();
}
class Method { }
";
			ArrayList results = CtrlSpaceResolveCSharp(program, 4, ExpressionContext.Default);
			Assert.IsTrue(MemberExists(results, "Method"), "method should be in completion lookup");
			ResolveResult result = Resolve(program, "Method()", 4);
			Assert.IsTrue(result != null && result.IsValid, "method should be found!");
		}
		
		bool MemberExists(ArrayList members, string name)
		{
			foreach (object o in members) {
				IMember m = o as IMember;
				if (m != null && m.Name == name) return true;
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
//			foreach (IMethod m in result.ResolvedType.GetMethods()) {
//				if (m.Name == "OverrideMe")
//					count += 1;
//			}
//			Assert.AreEqual(1, count);
			count = 0;
			foreach (object o in result.GetCompletionData(lastPC)) {
				IMethod m = o as IMethod;
				if (m != null && m.Name == "OverrideMe")
					count += 1;
			}
			Assert.AreEqual(1, count);
		}
		
		[Test]
		public void ShadowingTest()
		{
			string program = @"Imports System.Windows.Forms
Class F
  Inherits Form
	Sub M
	  
	End Sub
	Friend WithEvents Shadows cancelButton As Button
End Class
";
			MemberResolveResult result = ResolveVB<MemberResolveResult>(program, "CancelButton", 5);
			Assert.AreEqual("F.cancelButton", result.ResolvedMember.FullyQualifiedName);
			
			result = ResolveVB<MemberResolveResult>(program, "MyBase.CancelButton", 5);
			Assert.AreEqual("System.Windows.Forms.Form.CancelButton", result.ResolvedMember.FullyQualifiedName);
		}
		
		[Test]
		public void PreferExtensionMethodToInaccessibleMethod()
		{
			string program = @"static class Program {
	static void Main() {
		new BaseClass().Test(3);
		Console.ReadKey();
	}
}
class BaseClass {
	private void Test(int a) { }
}
static class Extensions {
	public static void Test(this BaseClass b, object a) { }
}";
			MemberResolveResult result = Resolve<MemberResolveResult>(program, "new BaseClass().Test(3)", 4);
			Assert.AreEqual("Extensions.Test", result.ResolvedMember.FullyQualifiedName);
		}
		#endregion
		
		#region MixedType tests
		const string arrayListMixedTypeProgram = @"using System.Collections;
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
			ResolveResult result = Resolve<MemberResolveResult>(arrayListMixedTypeProgram, "arrayList", 4);
			Assert.AreEqual("System.Collections.ArrayList", result.ResolvedType.FullyQualifiedName);
		}
		
		[Test]
		public void PropertyTypeConflictTestResolveInTypeContext()
		{
			TypeResolveResult result = (TypeResolveResult)Resolve(arrayListMixedTypeProgram, "ArrayList", 4, 0, ExpressionContext.Type);
			Assert.AreEqual("System.Collections.ArrayList", result.ResolvedClass.FullyQualifiedName);
			
			result = (TypeResolveResult)Resolve(arrayListMixedTypeProgram, "ArrayList", 8, 10, ExpressionContext.Type);
			Assert.AreEqual("System.Collections.ArrayList", result.ResolvedClass.FullyQualifiedName);
		}
		
		[Test]
		public void PropertyTypeConflictCompletionResultTest()
		{
			ResolveResult result = Resolve(arrayListMixedTypeProgram, "ArrayList", 4);
			Assert.IsTrue(result is MixedResolveResult);
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
		
		#region Attribute tests
		[Test]
		public void NamespaceInAttributeContext()
		{
			string program = @"using System;
  class Test {

}
";
			NamespaceResolveResult result = Resolve<NamespaceResolveResult>(program, "System", 2, 1, ExpressionContext.Attribute);
			Assert.AreEqual("System", result.Name);
			
			result = Resolve<NamespaceResolveResult>(program, "System.Runtime", 2, 1, ExpressionContext.Attribute);
			Assert.AreEqual("System.Runtime", result.Name);
		}
		
		[Test]
		public void AttributeWithShortName()
		{
			string program = @"using System;
  class Test {

}
";
			
			TypeResolveResult result = Resolve<TypeResolveResult>(program, "Obsolete", 2, 1, ExpressionContext.Attribute);
			Assert.AreEqual("System.ObsoleteAttribute", result.ResolvedClass.FullyQualifiedName);
			
			result = Resolve<TypeResolveResult>(program, "System.Obsolete", 2, 1, ExpressionContext.Attribute);
			Assert.AreEqual("System.ObsoleteAttribute", result.ResolvedClass.FullyQualifiedName);
		}
		
		[Test]
		public void AttributeConstructor()
		{
			string program = @"using System;
  class Test {

}
";
			
			MemberResolveResult result = Resolve<MemberResolveResult>(program, "LoaderOptimization(3)", 2, 1, ExpressionContext.Attribute);
			Assert.AreEqual("System.Byte", ((IMethod)result.ResolvedMember).Parameters[0].ReturnType.FullyQualifiedName);
			
			result = Resolve<MemberResolveResult>(program, "LoaderOptimization(LoaderOptimization.NotSpecified)", 2, 1, ExpressionContext.Attribute);
			Assert.AreEqual("System.LoaderOptimization", ((IMethod)result.ResolvedMember).Parameters[0].ReturnType.FullyQualifiedName);
			
			result = Resolve<MemberResolveResult>(program, "LoaderOptimizationAttribute(0)", 2, 1, ExpressionContext.Attribute);
			Assert.AreEqual("System.Byte", ((IMethod)result.ResolvedMember).Parameters[0].ReturnType.FullyQualifiedName);
		}
		
		[Test]
		public void AttributeArgumentInClassContext1()
		{
			string program = @"using System;
[AttributeUsage(XXX)] class MyAttribute : Attribute {
	public const AttributeTargets XXX = AttributeTargets.All;
}
";
			MemberResolveResult result = Resolve<MemberResolveResult>(program, "XXX", 2, 17, ExpressionContext.Default);
			Assert.AreEqual("MyAttribute.XXX", result.ResolvedMember.FullyQualifiedName);
		}
		
		[Test]
		public void AttributeArgumentInClassContext2()
		{
			string program = @"using System; namespace MyNamespace {
[SomeAttribute(E.A)] class Test {
	
}
enum E { A, B }
}
";
			MemberResolveResult result = Resolve<MemberResolveResult>(program, "E.A", 2, 16, ExpressionContext.Default);
			Assert.AreEqual("MyNamespace.E.A", result.ResolvedMember.FullyQualifiedName);
		}
		#endregion
		
		#region C# 3.0 tests
		[Test]
		public void TypeInferenceTest()
		{
			string program = @"class TestClass {
	static void Test() {
		var a = 3;
		
	}
}
";
			var lrr = Resolve<LocalResolveResult>(program, "a", 4);
			Assert.AreEqual("System.Int32", lrr.ResolvedType.FullyQualifiedName);
		}
		
		[Test]
		public void TypeInferenceCycleTest()
		{
			string program = @"class TestClass {
	static void Test() {
		var a = a;
		
	}
}
";
			var lrr = Resolve<LocalResolveResult>(program, "a", 4);
			Assert.IsNull(lrr.ResolvedType.GetUnderlyingClass());
		}
		
		[Test]
		public void ExtensionMethodsTest()
		{
			string program = @"using XN;
class TestClass {
	static void Test(A a, B b, C c) {
		
	}
}
class A { }
class B {
	public void F(int i) { }
}
class C {
	public void F(object obj) { }
}
namespace XN {
	public static class XC {
		public static void F(this object obj, int i) { }
		public static void F(this object obj, string s) { }
	}
}
";
			MemberResolveResult mrr;
			
			mrr = Resolve<MemberResolveResult>(program, "a.F(1)", 4);
			Assert.AreEqual("XN.XC.F", mrr.ResolvedMember.FullyQualifiedName);
			Assert.AreEqual("System.Int32", ((IMethod)mrr.ResolvedMember).Parameters[1].ReturnType.FullyQualifiedName);
			
			mrr = Resolve<MemberResolveResult>(program, "a.F(\"text\")", 4);
			Assert.AreEqual("XN.XC.F", mrr.ResolvedMember.FullyQualifiedName);
			Assert.AreEqual("System.String", ((IMethod)mrr.ResolvedMember).Parameters[1].ReturnType.FullyQualifiedName);
			
			mrr = Resolve<MemberResolveResult>(program, "b.F(1)", 4);
			Assert.AreEqual("B.F", mrr.ResolvedMember.FullyQualifiedName);
			
			mrr = Resolve<MemberResolveResult>(program, "b.F(\"text\")", 4);
			Assert.AreEqual("XN.XC.F", mrr.ResolvedMember.FullyQualifiedName);
			Assert.AreEqual("System.String", ((IMethod)mrr.ResolvedMember).Parameters[1].ReturnType.FullyQualifiedName);
			
			mrr = Resolve<MemberResolveResult>(program, "c.F(1)", 4);
			Assert.AreEqual("C.F", mrr.ResolvedMember.FullyQualifiedName);
			
			mrr = Resolve<MemberResolveResult>(program, "c.F(\"text\")", 4);
			Assert.AreEqual("C.F", mrr.ResolvedMember.FullyQualifiedName);
		}
		
		[Test]
		public void ExtensionMethodsTest2()
		{
			string program = @"using System; using System.Collections.Generic;
class TestClass {
	static void Test(string[] args) {
		
	}
}
public static class XC {
	public static int ToInt32(this string s) { return int.Parse(s); }
	public static T[] Slice<T>(this T[] source, int index, int count) { throw new NotImplementedException(); }
	public static IEnumerable<T> Filter<T>(this IEnumerable<T> source, Predicate<T> predicate) { throw new NotImplementedException(); }
}
";
			MemberResolveResult mrr;
			
			mrr = Resolve<MemberResolveResult>(program, "\"text\".ToInt32()", 4);
			Assert.AreEqual("XC.ToInt32", mrr.ResolvedMember.FullyQualifiedName);
			
			mrr = Resolve<MemberResolveResult>(program, "args.Slice(1, 2)", 4);
			Assert.AreEqual("XC.Slice", mrr.ResolvedMember.FullyQualifiedName);
			Assert.AreEqual("System.String[]", mrr.ResolvedType.DotNetName);
			
			mrr = Resolve<MemberResolveResult>(program, "args.Filter(delegate { return true; })", 4);
			Assert.AreEqual("XC.Filter", mrr.ResolvedMember.FullyQualifiedName);
			Assert.AreEqual("System.Collections.Generic.IEnumerable{System.String}", mrr.ResolvedType.DotNetName);
		}
		
		[Test]
		public void SimpleLinqTest()
		{
			string program = @"using System;
class TestClass {
	void Test(string[] input) {
		var r = from e in input
			where e.StartsWith(""/"")
			select e.Trim();
		
	}
}
";
			LocalResolveResult lrr = Resolve<LocalResolveResult>(program, "e", 5);
			Assert.AreEqual("System.String", lrr.ResolvedType.FullyQualifiedName);
			lrr = Resolve<LocalResolveResult>(program, "e", 6);
			Assert.AreEqual("System.String", lrr.ResolvedType.FullyQualifiedName);
			
			lrr = Resolve<LocalResolveResult>(program, "r", 7);
			Assert.AreEqual("System.Collections.Generic.IEnumerable", lrr.ResolvedType.FullyQualifiedName);
			Assert.AreEqual("System.String", lrr.ResolvedType.CastToConstructedReturnType().TypeArguments[0].FullyQualifiedName);
		}
		
		[Test]
		public void ParenthesizedLinqTest()
		{
			string program = @"using System; using System.Linq;
class TestClass {
	void Test(string[] input) {
		(from e in input select e.Length)
	}
}
";
			ResolveResult rr = Resolve(program,
			                           "(from e in input select e.Length)",
			                           4, 3, ExpressionContext.Default);
			Assert.IsNotNull(rr);
			Assert.AreEqual("System.Collections.Generic.IEnumerable", rr.ResolvedType.FullyQualifiedName);
			Assert.AreEqual("System.Int32", rr.ResolvedType.CastToConstructedReturnType().TypeArguments[0].FullyQualifiedName);
		}
		
		const string objectInitializerTestProgram = @"using System; using System.Threading;
class TestClass {
	static void Test() {
		Rectangle r1 = new Rectangle {
			
		};
		Rectangle r2 = new Rectangle {
			P1 = {
				
			}
		};
		MyCollectionType mct = new MyCollectionType {
			
		};
	}
}
public class Point
{
	int x;
	public int X { get { return x; } set { x = value; } }
	public int Y;
}
public class Rectangle
{
	Point p1 = new Point();
	Point p2 = new Point();
	public Point P1 { get { return p1; } }
	public Point P2 { get { return p2; } }
}
public class MyCollectionType : System.Collections.IEnumerable
{
	public void Add(object o) {}
	public int Field;
	public readonly int ReadOnlyValueTypeField;
	public int ReadOnlyValueTypeProperty { get; }
}
";
		
		[Test]
		public void ObjectInitializerCtrlSpaceCompletion()
		{
			ArrayList results = CtrlSpaceResolveCSharp(objectInitializerTestProgram, 5, ExpressionContext.ObjectInitializer);
			Assert.AreEqual(new[] { "P1", "P2" }, (from IMember p in results orderby p.Name select p.Name).ToArray() );
			
			results = CtrlSpaceResolveCSharp(objectInitializerTestProgram, 9, ExpressionContext.ObjectInitializer);
			Assert.AreEqual(new[] { "X", "Y" }, (from IMember p in results orderby p.Name select p.Name).ToArray() );
			
			results = CtrlSpaceResolveCSharp(objectInitializerTestProgram, 13, ExpressionContext.ObjectInitializer);
			// collection type: expect system types
			Assert.IsTrue(results.OfType<IClass>().Any((IClass c) => c.FullyQualifiedName == "System.Int32"));
			Assert.IsTrue(results.OfType<IClass>().Any((IClass c) => c.FullyQualifiedName == "System.AppDomain"));
			// expect local variables
			Assert.IsTrue(results.OfType<IField>().Any((IField f) => f.IsLocalVariable && f.Name == "r1"));
			// but also expect MyCollectionType.Field
			Assert.IsTrue(results.OfType<IField>().Any((IField f) => f.FullyQualifiedName == "MyCollectionType.Field"));
			Assert.IsFalse(results.OfType<IField>().Any((IField f) => f.FullyQualifiedName == "MyCollectionType.ReadOnlyValueTypeField"));
			Assert.IsFalse(results.OfType<IProperty>().Any((IProperty f) => f.FullyQualifiedName == "MyCollectionType.ReadOnlyValueTypeProperty"));
		}
		
		[Test]
		public void ObjectInitializerCompletion()
		{
			MemberResolveResult mrr = (MemberResolveResult)Resolve(objectInitializerTestProgram, "P2", 5, 1, ExpressionContext.ObjectInitializer);
			Assert.IsNotNull(mrr);
			Assert.AreEqual("Rectangle.P2", mrr.ResolvedMember.FullyQualifiedName);
			
			mrr = (MemberResolveResult)Resolve(objectInitializerTestProgram, "X", 9, 1, ExpressionContext.ObjectInitializer);
			Assert.IsNotNull(mrr);
			Assert.AreEqual("Point.X", mrr.ResolvedMember.FullyQualifiedName);
			
			mrr = (MemberResolveResult)Resolve(objectInitializerTestProgram, "Field", 13, 1, ExpressionContext.ObjectInitializer);
			Assert.IsNotNull(mrr);
			Assert.AreEqual("MyCollectionType.Field", mrr.ResolvedMember.FullyQualifiedName);
			
			LocalResolveResult lrr = (LocalResolveResult)Resolve(objectInitializerTestProgram, "r1", 13, 1, ExpressionContext.ObjectInitializer);
			Assert.IsNotNull(lrr);
			Assert.AreEqual("r1", lrr.Field.Name);
		}
		#endregion
		
		[Test]
		public void ClassWithSameNameAsNamespace()
		{
			string program = @"using System; namespace XX {
	class Test {
		static void X() {
			
		}
	}
	class XX {
		public static void Test() {}
	} }";
			TypeResolveResult trr = Resolve<TypeResolveResult>(program, "XX", 4);
			Assert.AreEqual("XX.XX", trr.ResolvedClass.FullyQualifiedName);

			NamespaceResolveResult nrr = Resolve<NamespaceResolveResult>(program, "global::XX", 4);
			Assert.AreEqual("XX", nrr.Name);
			
			trr = Resolve<TypeResolveResult>(program, "global::XX.XX", 4);
			Assert.AreEqual("XX.XX", trr.ResolvedClass.FullyQualifiedName);
			
			MemberResolveResult mrr = Resolve<MemberResolveResult>(program, "XX.Test()", 4);
			Assert.AreEqual("XX.XX.Test", mrr.ResolvedMember.FullyQualifiedName);
		}
		
		[Test]
		public void InvocableRule()
		{
			string program = @"using System;
	class DerivedClass : BaseClass {
		static void X() {
		
		}
		private static new int Test;
	}
	class BaseClass {
		public static string Test() {}
	}";
			MemberResolveResult mrr = Resolve<MemberResolveResult>(program, "BaseClass.Test()", 4);
			Assert.AreEqual("BaseClass.Test", mrr.ResolvedMember.FullyQualifiedName);
			
			mrr = Resolve<MemberResolveResult>(program, "Test", 4);
			Assert.AreEqual("DerivedClass.Test", mrr.ResolvedMember.FullyQualifiedName);
			
			mrr = Resolve<MemberResolveResult>(program, "DerivedClass.Test", 4);
			Assert.AreEqual("DerivedClass.Test", mrr.ResolvedMember.FullyQualifiedName);
			
			// returns BaseClass.Test because DerivedClass.Test is not invocable
			mrr = Resolve<MemberResolveResult>(program, "DerivedClass.Test()", 4);
			Assert.AreEqual("BaseClass.Test", mrr.ResolvedMember.FullyQualifiedName);
		}
		
		[Test]
		public void InvocableRule2()
		{
			string program = @"using System;
	class DerivedClass : BaseClass {
		static void X() {
		
		}
		private static new int Test;
	}
	delegate string SomeDelegate();
	class BaseClass {
		public static SomeDelegate Test;
	}";
			MemberResolveResult mrr = Resolve<MemberResolveResult>(program, "BaseClass.Test()", 4);
			Assert.AreEqual("BaseClass.Test", mrr.ResolvedMember.FullyQualifiedName);
			
			mrr = Resolve<MemberResolveResult>(program, "Test", 4);
			Assert.AreEqual("DerivedClass.Test", mrr.ResolvedMember.FullyQualifiedName);
			
			mrr = Resolve<MemberResolveResult>(program, "DerivedClass.Test", 4);
			Assert.AreEqual("DerivedClass.Test", mrr.ResolvedMember.FullyQualifiedName);
			
			// returns BaseClass.Test because DerivedClass.Test is not invocable
			mrr = Resolve<MemberResolveResult>(program, "DerivedClass.Test()", 4);
			Assert.AreEqual("BaseClass.Test", mrr.ResolvedMember.FullyQualifiedName);
		}
		
		[Test]
		public void AccessibleRule()
		{
			string program = @"using System;
	class BaseClass {
		static void X() {
		
		}
		public static int Test;
	}
	class DerivedClass : BaseClass {
		private static new int Test;
	}
	";
			// returns BaseClass.Test because DerivedClass.Test is not accessible
			MemberResolveResult mrr = Resolve<MemberResolveResult>(program, "DerivedClass.Test", 4);
			Assert.AreEqual("BaseClass.Test", mrr.ResolvedMember.FullyQualifiedName);
		}
		
		[Test]
		public void FieldHidingProperty()
		{
			string program = @"using System;
	class DerivedClass : BaseClass {
		static void X() {
		
		}
		public static new int Test;
	}
	class BaseClass {
		public static int Test { get { return 0; } }
	}
	";
			MemberResolveResult mrr = Resolve<MemberResolveResult>(program, "Test", 4);
			Assert.AreEqual("DerivedClass.Test", mrr.ResolvedMember.FullyQualifiedName);
			
			mrr = Resolve<MemberResolveResult>(program, "DerivedClass.Test", 4);
			Assert.AreEqual("DerivedClass.Test", mrr.ResolvedMember.FullyQualifiedName);
		}
		
		[Test]
		public void PropertyHidingField()
		{
			string program = @"using System;
	class DerivedClass : BaseClass {
		static void X() {
		
		}
		public static new int Test { get { return 0; } }
	}
	class BaseClass {
		public static int Test;
	}
	";
			MemberResolveResult mrr = Resolve<MemberResolveResult>(program, "Test", 4);
			Assert.AreEqual("DerivedClass.Test", mrr.ResolvedMember.FullyQualifiedName);
			
			mrr = Resolve<MemberResolveResult>(program, "DerivedClass.Test", 4);
			Assert.AreEqual("DerivedClass.Test", mrr.ResolvedMember.FullyQualifiedName);
		}
		
		[Test]
		public void TestOverloadingByRef()
		{
			string program = @"using System;
class Program {
	public static void Main() {
		int a = 42;
		T(a);
		T(ref a);
	}
	static void T(int x) {}
	static void T(ref int y) {}
}";
			
			MemberResolveResult mrr = Resolve<MemberResolveResult>(program, "T(a)", 5);
			Assert.IsFalse(((IMethod)mrr.ResolvedMember).Parameters[0].IsRef);
			
			mrr = Resolve<MemberResolveResult>(program, "T(ref a)", 5);
			Assert.IsTrue(((IMethod)mrr.ResolvedMember).Parameters[0].IsRef);
		}
		
		[Test]
		public void AddedOverload()
		{
			string program = @"class BaseClass {
	static void Main() {
		new DerivedClass().Test(3);
		Console.ReadKey();
	}
	public void Test(int a) { }
}
class DerivedClass : BaseClass {
	public void Test(object a) { }
}";
			MemberResolveResult mrr = Resolve<MemberResolveResult>(program, "new DerivedClass().Test(3);", 4);
			Assert.AreEqual("DerivedClass.Test", (mrr.ResolvedMember).FullyQualifiedName);
		}
		
		[Test]
		public void AddedNonApplicableOverload()
		{
			string program = @"class BaseClass {
	static void Main() {
		new DerivedClass().Test(3);
		Console.ReadKey();
	}
	public void Test(int a) { }
}
class DerivedClass : BaseClass {
	public void Test(string a) { }
}";
			MemberResolveResult mrr = Resolve<MemberResolveResult>(program, "new DerivedClass().Test(3);", 4);
			Assert.AreEqual("BaseClass.Test", (mrr.ResolvedMember).FullyQualifiedName);
			
			mrr = Resolve<MemberResolveResult>(program, "new DerivedClass().Test(\"3\");", 4);
			Assert.AreEqual("DerivedClass.Test", (mrr.ResolvedMember).FullyQualifiedName);
		}
		
		[Test]
		public void OverrideShadowed()
		{
			string program = @"using System;
class BaseClass {
	static void Main() {
		new DerivedClass().Test(3);
		Console.ReadKey();
	}
	public virtual void Test(int a) { }
}
class MiddleClass : BaseClass {
	public void Test(object a) { }
}
class DerivedClass : MiddleClass {
	public override void Test(int a) { }
}";
			
			MemberResolveResult mrr = Resolve<MemberResolveResult>(program, "new DerivedClass().Test(3);", 4);
			Assert.AreEqual("MiddleClass.Test", (mrr.ResolvedMember).FullyQualifiedName);
		}
	}
}
