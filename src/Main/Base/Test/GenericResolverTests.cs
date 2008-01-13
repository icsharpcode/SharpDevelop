// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.SharpDevelop.Dom;
using NUnit.Framework;

namespace ICSharpCode.SharpDevelop.Tests
{
	[TestFixture]
	public class GenericResolverTests
	{
		#region Test helper methods
		NRefactoryResolverTests nrrt = new NRefactoryResolverTests();
		
		ResolveResult Resolve(string program, string expression, int line)
		{
			return nrrt.Resolve(program, expression, line);
		}
		
		RR Resolve<RR>(string program, string expression, int line) where RR : ResolveResult
		{
			return nrrt.Resolve<RR>(program, expression, line);
		}
		
		RR Resolve<RR>(string program, string expression, int line, int column) where RR : ResolveResult
		{
			return nrrt.Resolve<RR>(program, expression, line, column, ExpressionContext.Default);
		}
		
		ResolveResult ResolveVB(string program, string expression, int line)
		{
			return nrrt.ResolveVB(program, expression, line);
		}
		#endregion
		
		#region Generic references
		const string listProgram = @"using System.Collections.Generic;
class TestClass {
	void Method() {
		List<TestClass> list = new List<TestClass>();
		
	}
	
	T CloneIt<T>(T source) where T : ICloneable {
		if (source == null) return new TestClass();
		return source.Clone();
	}
	
	public int PublicField;
}
";
		
		[Test]
		public void ListAddTest()
		{
			ResolveResult result = Resolve(listProgram, "list.Add(new A())", 5);
			Assert.IsNotNull(result);
			Assert.IsTrue(result is MemberResolveResult);
			IMethod m = (IMethod)((MemberResolveResult)result).ResolvedMember;
			Assert.AreEqual(1, m.Parameters.Count);
			Assert.AreEqual("TestClass", m.Parameters[0].ReturnType.FullyQualifiedName);
		}
		
		[Test]
		public void ListAddRangeTest()
		{
			ResolveResult result = Resolve(listProgram, "list.AddRange(new A[0])", 5);
			Assert.IsNotNull(result);
			Assert.IsTrue(result is MemberResolveResult);
			IMethod m = (IMethod)((MemberResolveResult)result).ResolvedMember;
			Assert.AreEqual(1, m.Parameters.Count);
			Assert.IsTrue(m.Parameters[0].ReturnType is ConstructedReturnType);
			Assert.AreEqual("System.Collections.Generic.IEnumerable", m.Parameters[0].ReturnType.FullyQualifiedName);
			Assert.AreEqual("TestClass", ((ConstructedReturnType)m.Parameters[0].ReturnType).TypeArguments[0].FullyQualifiedName);
		}
		
		[Test]
		public void ListToArrayTest()
		{
			ResolveResult result = Resolve(listProgram, "list.ToArray()", 5);
			Assert.IsNotNull(result);
			Assert.IsTrue(result is MemberResolveResult);
			IMethod m = (IMethod)((MemberResolveResult)result).ResolvedMember;
			Assert.AreEqual("TestClass", m.ReturnType.FullyQualifiedName);
			Assert.AreEqual(1, m.ReturnType.CastToArrayReturnType().ArrayDimensions);
		}
		
		[Test]
		public void ClassReferenceTest()
		{
			ResolveResult result = Resolve(listProgram, "List<string>", 5);
			Assert.IsNotNull(result);
			Assert.IsTrue(result is TypeResolveResult);
			Assert.AreEqual("System.Collections.Generic.List", ((TypeResolveResult)result).ResolvedClass.FullyQualifiedName);
			Assert.IsTrue(result.ResolvedType is ConstructedReturnType);
			Assert.AreEqual("System.String", ((ConstructedReturnType)result.ResolvedType).TypeArguments[0].FullyQualifiedName);
		}
		
		[Test]
		public void GenericMethodCallTest()
		{
			ResolveResult result = Resolve(listProgram, "CloneIt<TestClass>(null)", 5);
			Assert.IsNotNull(result);
			Assert.IsTrue(result is MemberResolveResult);
			Assert.AreEqual("TestClass", result.ResolvedType.FullyQualifiedName);
			MemberResolveResult mrr = (MemberResolveResult) result;
			Assert.AreEqual("TestClass.CloneIt", mrr.ResolvedMember.FullyQualifiedName);
		}
		
		[Test]
		public void FieldReferenceOnGenericMethodTest()
		{
			ResolveResult result = Resolve(listProgram, "CloneIt<TestClass>(null).PublicField", 5);
			Assert.IsNotNull(result);
			Assert.IsTrue(result is MemberResolveResult);
			Assert.AreEqual("System.Int32", result.ResolvedType.FullyQualifiedName);
			MemberResolveResult mrr = (MemberResolveResult) result;
			Assert.AreEqual("TestClass.PublicField", mrr.ResolvedMember.FullyQualifiedName);
		}
		
		[Test]
		public void TypeInferredGenericMethodCallTest()
		{
			ResolveResult result = Resolve(listProgram, "CloneIt(new TestClass())", 5);
			Assert.IsNotNull(result);
			Assert.IsTrue(result is MemberResolveResult);
			Assert.AreEqual("TestClass", result.ResolvedType.FullyQualifiedName);
			MemberResolveResult mrr = (MemberResolveResult) result;
			Assert.AreEqual("TestClass.CloneIt", mrr.ResolvedMember.FullyQualifiedName);
		}
		
		[Test]
		public void FieldReferenceOnTypeInferredGenericMethodCallTest()
		{
			ResolveResult result = Resolve(listProgram, "CloneIt(new TestClass()).PublicField", 5);
			Assert.IsNotNull(result);
			Assert.IsTrue(result is MemberResolveResult);
			Assert.AreEqual("System.Int32", result.ResolvedType.FullyQualifiedName);
			MemberResolveResult mrr = (MemberResolveResult) result;
			Assert.AreEqual("TestClass.PublicField", mrr.ResolvedMember.FullyQualifiedName);
		}
		
		[Test]
		public void ImportAliasClassResolveTest()
		{
			string program = @"using COL = System.Collections.Generic.List<string>;
class TestClass {
	void Test() {
		COL a = new COL();
		
	}
}
";
			TypeResolveResult rr = Resolve(program, "COL", 4) as TypeResolveResult;
			Assert.AreEqual("System.Collections.Generic.List", rr.ResolvedClass.FullyQualifiedName, "COL");
			Assert.AreEqual("System.Collections.Generic.List{System.String}", rr.ResolvedType.DotNetName, "COL");
			LocalResolveResult lr = Resolve(program, "a", 5) as LocalResolveResult;
			Assert.AreEqual("System.Collections.Generic.List{System.String}", lr.ResolvedType.DotNetName, "a");
		}
		
		[Test]
		public void InheritFromGenericClass()
		{
			string program = @"using System;
class BaseClass<T> {
	public T value;
}
class DerivedClass : BaseClass<string> {
	
}";
			MemberResolveResult rr = Resolve(program, "value", 6) as MemberResolveResult;
			Assert.AreEqual("System.String", rr.ResolvedType.FullyQualifiedName);
		}
		
		[Test]
		public void CrossTypeParametersInheritance()
		{
			string program = @"using System;
class BaseClass<A,B> {
	public A a;
	public B b;
}
class DerivedClass<A,B> : BaseClass<B,A> {
	
}";
			MemberResolveResult rr = Resolve(program, "a", 7) as MemberResolveResult;
			Assert.AreEqual("B", rr.ResolvedType.Name);
			rr = Resolve(program, "b", 7) as MemberResolveResult;
			Assert.AreEqual("A", rr.ResolvedType.Name);
		}
		#endregion
		
		#region CodeCompletion inside generic classes
		const string genericClass = @"using System;
public class GenericClass<T> where T : IDisposable {
	void Method<G>(T par1, G par2) where G : IConvertible, IFormattable {
		T var1; G var2;
		
	}
}
";
		
		[Test]
		public void ClassTypeParameterResolveType()
		{
			ResolveResult rr = Resolve(genericClass, "T", 5);
			Assert.IsNotNull(rr);
			Assert.IsTrue(rr is TypeResolveResult);
			Assert.IsNull((rr as TypeResolveResult).ResolvedClass);
			Assert.IsTrue(rr.ResolvedType is GenericReturnType);
		}
		
		[Test]
		public void ClassTypeParameterResolveVariable()
		{
			ResolveResult rr = Resolve(genericClass, "var1", 5);
			Assert.IsNotNull(rr);
			Assert.IsTrue(rr is LocalResolveResult);
			Assert.IsTrue(rr.ResolvedType is GenericReturnType);
		}
		
		[Test]
		public void ClassTypeParameterResolveParameter()
		{
			ResolveResult rr = Resolve(genericClass, "par1", 5);
			Assert.IsNotNull(rr);
			Assert.IsTrue(rr is LocalResolveResult);
			Assert.IsTrue(rr.ResolvedType is GenericReturnType);
		}
		
		[Test]
		public void MethodTypeParameterResolveType()
		{
			ResolveResult rr = Resolve(genericClass, "G", 5);
			Assert.IsNotNull(rr);
			Assert.IsTrue(rr is TypeResolveResult);
			Assert.IsNull((rr as TypeResolveResult).ResolvedClass);
			Assert.IsTrue(rr.ResolvedType is GenericReturnType);
		}
		
		[Test]
		public void MethodTypeParameterResolveVariable()
		{
			ResolveResult rr = Resolve(genericClass, "var2", 5);
			Assert.IsNotNull(rr);
			Assert.IsTrue(rr is LocalResolveResult);
			Assert.IsTrue(rr.ResolvedType is GenericReturnType);
		}
		
		[Test]
		public void MethodTypeParameterResolveParameter()
		{
			ResolveResult rr = Resolve(genericClass, "par2", 5);
			Assert.IsNotNull(rr);
			Assert.IsTrue(rr is LocalResolveResult);
			Assert.IsTrue(rr.ResolvedType is GenericReturnType);
		}
		
		[Test]
		public void ResolveGenericClassInDeclarationLine()
		{
			const string program = @"using System;
public abstract class Sorter { }
public abstract class Sorter<T> : Sorter, IComparer<T> { }
";
			
			TypeResolveResult trr = Resolve<TypeResolveResult>(program, "Sorter", 3, 37);
			Assert.AreEqual(0, trr.ResolvedClass.TypeParameters.Count);
		}
		#endregion
		
		#region Generic methods
		[Test]
		public void GenericMethodInstanciation()
		{
			string program = @"using System;
using System.Collections.Generic;
class TestClass {
	void Main() {
		
	}
	static T First<T>(IEnumerable<T> input) {
		foreach (T e in input) return e;
		throw new EmptyCollectionException();
	}
}
";
			MemberResolveResult mrr = Resolve<MemberResolveResult>(program, "First(new string[0])", 5);
			Assert.AreEqual("System.String", mrr.ResolvedType.FullyQualifiedName);
			Assert.AreEqual("System.String", mrr.ResolvedMember.ReturnType.FullyQualifiedName);
			
			IMethod genericMethod = mrr.ResolvedMember.DeclaringType.Methods[1];
			Assert.AreEqual("T", genericMethod.ReturnType.FullyQualifiedName);
			
			// ensure that the reference pointing to the specialized method is seen as a reference
			// to the generic method.
			Assert.IsTrue(Refactoring.RefactoringService.IsReferenceToMember(genericMethod, mrr));
		}
		
		[Test]
		public void BothGenericAndNonGenericMethod()
		{
			string program = @"using System;
class TestClass {
	void Main() {
		
	}
	static object GetSomething() {
		return null;
	}
	static T GetSomething<T>() {
		return default(T);
	}
}
";
			
			MemberResolveResult mrr = Resolve<MemberResolveResult>(program, "GetSomething()", 4);
			
			IMethod nonGenericMethod = mrr.ResolvedMember.DeclaringType.Methods[1];
			Assert.AreEqual("System.Object", nonGenericMethod.ReturnType.FullyQualifiedName);
			
			IMethod genericMethod = mrr.ResolvedMember.DeclaringType.Methods[2];
			Assert.AreEqual("T", genericMethod.ReturnType.FullyQualifiedName);
			
			Assert.AreSame(nonGenericMethod, mrr.ResolvedMember);
			
			Assert.IsTrue(Refactoring.RefactoringService.IsReferenceToMember(nonGenericMethod, mrr));
			Assert.IsFalse(Refactoring.RefactoringService.IsReferenceToMember(genericMethod, mrr));
			
			mrr = Resolve<MemberResolveResult>(program, "GetSomething<int>()", 4);
			Assert.AreEqual("System.Int32", mrr.ResolvedType.FullyQualifiedName);
			Assert.AreEqual("System.Int32", mrr.ResolvedMember.ReturnType.FullyQualifiedName);
			
			Assert.IsTrue(Refactoring.RefactoringService.IsReferenceToMember(genericMethod, mrr));
			Assert.IsFalse(Refactoring.RefactoringService.IsReferenceToMember(nonGenericMethod, mrr));
		}
		
		[Test]
		public void OverrideGenericMethodTest()
		{
			string program = @"using System;
class Program {
	public static void Main() {
		D d = new D();
		
		d.T<char, int>('a', 1);
		d.T<int, char>('a', 2);
	}
	protected virtual void T<A, B>(A a, B b) {
	}
	protected virtual void T<X, Y>(Y a, X b) {
	}
}
class D : Program  {
	protected override void T<X, Y>(X a, Y b) {
		// overrides T<A,B> - type arguments are identified by position
	}
}";
			IAmbience ambience = Dom.CSharp.CSharpAmbience.Instance;
			ambience.ConversionFlags = ConversionFlags.UseFullyQualifiedMemberNames | ConversionFlags.ShowTypeParameterList;
			MemberResolveResult mrr;
			
			mrr = Resolve<MemberResolveResult>(program, "d.T<int, char>('a', 2)", 5);
			Assert.AreEqual("Program.T<X, Y>", ambience.Convert((IMethod)mrr.ResolvedMember));
			
			mrr = Resolve<MemberResolveResult>(program, "d.T<char, int>('a', 1)", 6);
			Assert.AreEqual("D.T<X, Y>", ambience.Convert((IMethod)mrr.ResolvedMember));
			
			IMember baseMember = MemberLookupHelper.FindBaseMember(mrr.ResolvedMember);
			Assert.IsNotNull(baseMember);
			Assert.AreEqual("Program.T<A, B>", ambience.Convert((IMethod)baseMember));
		}
		#endregion
	}
}
