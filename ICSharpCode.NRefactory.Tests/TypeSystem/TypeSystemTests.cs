// Copyright (c) 2010 AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under MIT X11 license (for details please see \doc\license.txt)

using System;
using System.Linq;
using ICSharpCode.NRefactory.TypeSystem.Implementation;
using ICSharpCode.NRefactory.TypeSystem.TestCase;
using NUnit.Framework;

namespace ICSharpCode.NRefactory.TypeSystem
{
	/// <summary>
	/// Base class for the type system tests.
	/// Test fixtures for specific APIs (Cecil, C# Parser) derive from this class.
	/// </summary>
	public abstract class TypeSystemTests
	{
		protected IProjectContent testCasePC;
		
		ITypeResolveContext ctx;
		
		[SetUpAttribute]
		public void SetUp()
		{
			ctx = AggregateTypeResolveContext.Combine(testCasePC, CecilLoaderTests.Mscorlib);
		}
		
		ITypeDefinition GetClass(Type type)
		{
			return testCasePC.GetClass(type.FullName, type.GetGenericArguments().Length, StringComparer.Ordinal);
		}
		
		[Test]
		public void SimplePublicClassTest()
		{
			ITypeDefinition c = testCasePC.GetClass(typeof(SimplePublicClass));
			Assert.AreEqual(typeof(SimplePublicClass).Name, c.Name);
			Assert.AreEqual(typeof(SimplePublicClass).FullName, c.FullName);
			Assert.AreEqual(typeof(SimplePublicClass).Namespace, c.Namespace);
			Assert.AreEqual(typeof(SimplePublicClass).FullName, c.DotNetName);
			
			Assert.AreEqual(Accessibility.Public, c.Accessibility);
			Assert.IsFalse(c.IsAbstract);
			Assert.IsFalse(c.IsSealed);
			Assert.IsFalse(c.IsStatic);
			Assert.IsFalse(c.IsShadowing);
		}
		
		[Test]
		public void SimplePublicClassMethodTest()
		{
			ITypeDefinition c = testCasePC.GetClass(typeof(SimplePublicClass));
			Assert.AreEqual(2, c.Methods.Count);
			
			IMethod method = c.Methods.Single(m => m.Name == "Method");
			Assert.AreEqual(typeof(SimplePublicClass).FullName + ".Method", method.FullName);
			Assert.AreSame(c, method.DeclaringType);
			Assert.AreEqual(Accessibility.Public, method.Accessibility);
			Assert.AreEqual(EntityType.Method, method.EntityType);
			Assert.IsFalse(method.IsVirtual);
			Assert.IsFalse(method.IsStatic);
			Assert.IsTrue(method.IsFrozen);
			Assert.AreEqual(0, method.Parameters.Count);
			Assert.AreEqual(0, method.Attributes.Count);
		}
		
		[Test]
		public void DynamicType()
		{
			ITypeDefinition testClass = testCasePC.GetClass(typeof(DynamicTest));
			Assert.AreSame(SharedTypes.Dynamic, testClass.Properties.Single().ReturnType.Resolve(ctx));
		}
		
		[Test]
		[Ignore]
		public void DynamicTypeInGenerics()
		{
			ITypeDefinition testClass = testCasePC.GetClass(typeof(DynamicTest));
			/*CSharpAmbience a = new CSharpAmbience();
			a.ConversionFlags = ConversionFlags.ShowReturnType | ConversionFlags.ShowParameterList;
			Assert.AreEqual("List<dynamic> DynamicGenerics1(Action<object, dynamic[], object>)", a.Convert(testClass.Methods.Single(me => me.Name == "DynamicGenerics1")));
			Assert.AreEqual("void DynamicGenerics2(Action<object, dynamic, object>)", a.Convert(testClass.Methods.Single(me => me.Name == "DynamicGenerics2")));
			Assert.AreEqual("void DynamicGenerics3(Action<int, dynamic, object>)", a.Convert(testClass.Methods.Single(me => me.Name == "DynamicGenerics3")));
			Assert.AreEqual("void DynamicGenerics4(Action<int[], dynamic, object>)", a.Convert(testClass.Methods.Single(me => me.Name == "DynamicGenerics4")));
			Assert.AreEqual("void DynamicGenerics5(Action<Int32*[], dynamic, object>)", a.Convert(testClass.Methods.Single(me => me.Name == "DynamicGenerics5")));*/
		}
		
		[Test]
		public void AssemblyAttribute()
		{
			var attributes = testCasePC.AssemblyAttributes;
			var typeTest = attributes.First(a => a.AttributeType.Resolve(ctx).FullName == typeof(TypeTestAttribute).FullName);
			Assert.AreEqual(3, typeTest.PositionalArguments.Count);
			// first argument is (int)42
			Assert.AreEqual(42, (int)typeTest.PositionalArguments[0].GetValue(ctx));
			// second argument is typeof(System.Action<>)
			IType rt = (IType)typeTest.PositionalArguments[1].GetValue(ctx);
			Assert.IsFalse(rt is ParameterizedType); // rt must not be constructed - it's just an unbound type
			Assert.AreEqual("System.Action", rt.FullName);
			Assert.AreEqual(1, rt.TypeParameterCount);
			// third argument is typeof(IDictionary<string, IList<TestAttribute>>)
			ParameterizedType crt = (ParameterizedType)typeTest.PositionalArguments[2].GetValue(ctx);
			Assert.AreEqual("System.Collections.Generic.IDictionary", crt.FullName);
			Assert.AreEqual("System.String", crt.TypeArguments[0].FullName);
			// ? for NUnit.TestAttribute (because that assembly isn't in ctx)
			Assert.AreEqual("System.Collections.Generic.IList`1[[?]]", crt.TypeArguments[1].DotNetName);
		}
		
		[Test]
		public void TestClassTypeParameters()
		{
			var testClass = testCasePC.GetClass(typeof(GenericClass<,>));
			Assert.AreSame(testClass, testClass.TypeParameters[0].ParentClass);
			Assert.AreSame(testClass, testClass.TypeParameters[1].ParentClass);
			Assert.AreSame(testClass.TypeParameters[1], testClass.TypeParameters[0].Constraints[0].Resolve(ctx));
		}
		
		[Test]
		public void TestMethod()
		{
			var testClass = testCasePC.GetClass(typeof(GenericClass<,>));
			
			IMethod m = testClass.Methods.Single(me => me.Name == "TestMethod");
			Assert.AreEqual("K", m.TypeParameters[0].Name);
			Assert.AreEqual("V", m.TypeParameters[1].Name);
			Assert.AreSame(m, m.TypeParameters[0].ParentMethod);
			Assert.AreSame(m, m.TypeParameters[1].ParentMethod);
			
			Assert.AreEqual("System.IComparable`1[[``1]]", m.TypeParameters[0].Constraints[0].Resolve(ctx).DotNetName);
			Assert.AreSame(m.TypeParameters[0], m.TypeParameters[1].Constraints[0].Resolve(ctx));
		}
		
		[Test]
		public void GetIndex()
		{
			var testClass = testCasePC.GetClass(typeof(GenericClass<,>));
			
			IMethod m = testClass.Methods.Single(me => me.Name == "GetIndex");
			Assert.AreEqual("T", m.TypeParameters[0].Name);
			Assert.AreSame(m, m.TypeParameters[0].ParentMethod);
			
			ParameterizedType constraint = (ParameterizedType)m.TypeParameters[0].Constraints[0].Resolve(ctx);
			Assert.AreEqual("IEquatable", constraint.Name);
			Assert.AreEqual(1, constraint.TypeParameterCount);
			Assert.AreEqual(1, constraint.TypeArguments.Count);
			Assert.AreSame(m.TypeParameters[0], constraint.TypeArguments[0]);
		}
		
		[Test]
		public void PropertyWithProtectedSetter()
		{
			var testClass = testCasePC.GetClass(typeof(PropertyTest));
			IProperty p = testClass.Properties.Single(pr => pr.Name == "PropertyWithProtectedSetter");
			Assert.IsTrue(p.CanGet);
			Assert.IsTrue(p.CanSet);
			Assert.AreEqual(Accessibility.Public, p.Accessibility);
			Assert.AreEqual(Accessibility.Public, p.GetterAccessibility);
			Assert.AreEqual(Accessibility.Protected, p.SetterAccessibility);
		}
		
		[Test]
		public void PropertyWithPrivateSetter()
		{
			var testClass = testCasePC.GetClass(typeof(PropertyTest));
			IProperty p = testClass.Properties.Single(pr => pr.Name == "PropertyWithPrivateSetter");
			Assert.IsTrue(p.CanGet);
			Assert.IsTrue(p.CanSet);
			Assert.AreEqual(Accessibility.Public, p.Accessibility);
			Assert.AreEqual(Accessibility.Public, p.GetterAccessibility);
			Assert.AreEqual(Accessibility.Private, p.SetterAccessibility);
		}
		
		[Test]
		public void Indexer()
		{
			var testClass = testCasePC.GetClass(typeof(PropertyTest));
			IProperty p = testClass.Properties.Single(pr => pr.IsIndexer);
			Assert.IsTrue(p.CanGet);
			Assert.AreEqual(Accessibility.Public, p.Accessibility);
			Assert.AreEqual(Accessibility.Public, p.GetterAccessibility);
			Assert.IsFalse(p.CanSet);
			Assert.AreEqual(Accessibility.None, p.SetterAccessibility);
		}
		
		[Test]
		public void EnumTest()
		{
			var e = testCasePC.GetClass(typeof(MyEnum));
			Assert.AreEqual(ClassType.Enum, e.ClassType);
			Assert.AreEqual(false, e.IsReferenceType);
			Assert.AreEqual("System.Int16", e.BaseTypes[0].Resolve(ctx).DotNetName);
			Assert.AreEqual(new[] { "System.Enum" }, e.GetBaseTypes(ctx).Select(t => t.DotNetName).ToArray());
		}
	}
}
