// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Dom.CSharp;
using ICSharpCode.SharpDevelop.Dom.ReflectionLayer;
using NUnit.Framework;

[assembly: ICSharpCode.SharpDevelop.Tests.TypeTestAttribute(
	42, typeof(System.Action<>), typeof(IDictionary<string, IList<TestAttribute>>))]

namespace ICSharpCode.SharpDevelop.Tests
{
	public class TypeTestAttribute : Attribute
	{
		public TypeTestAttribute(int a1, Type a2, Type a3) {}
	}
	
	[TestFixture]
	public class ReflectionLayerTests : ReflectionOrCecilLayerTests
	{
		public ReflectionLayerTests()
		{
			mscorlib = AssemblyParserService.DefaultProjectContentRegistry.Mscorlib;
		}
		
		protected override IClass GetClass(Type type)
		{
			ICompilationUnit cu = new ReflectionProjectContent("TestName", "testlocation", new DomAssemblyName[0], AssemblyParserService.DefaultProjectContentRegistry).AssemblyCompilationUnit;
			((ReflectionProjectContent)cu.ProjectContent).AddReferencedContent(mscorlib);
			IClass c = new ReflectionClass(cu, type, type.FullName, null);
			cu.ProjectContent.AddClassToNamespaceList(c);
			return c;
		}
		
		protected override IEnumerable<IAttribute> GetAssemblyAttributes(Assembly assembly)
		{
			var pc = new ReflectionProjectContent("TestName", "testlocation", new DomAssemblyName[0], AssemblyParserService.DefaultProjectContentRegistry);
			pc.AddAssemblyAttributes(assembly);
			return pc.GetAssemblyAttributes();
		}
	}
	
	[TestFixture]
	public class ReflectionWithRoundTripLayerTests : ReflectionOrCecilLayerTests
	{
		public ReflectionWithRoundTripLayerTests()
		{
			mscorlib = AssemblyParserService.DefaultProjectContentRegistry.Mscorlib;
			
			MemoryStream memory = new MemoryStream();
			DomPersistence.WriteProjectContent((ReflectionProjectContent)mscorlib, memory);
			memory.Position = 0;
			mscorlib = DomPersistence.LoadProjectContent(memory, AssemblyParserService.DefaultProjectContentRegistry);
		}
		
		protected override IClass GetClass(Type type)
		{
			ICompilationUnit cu = new ReflectionProjectContent("TestName", "testlocation", new DomAssemblyName[0], AssemblyParserService.DefaultProjectContentRegistry).AssemblyCompilationUnit;
			IClass c = new ReflectionClass(cu, type, type.FullName, null);
			cu.ProjectContent.AddClassToNamespaceList(c);
			
			MemoryStream memory = new MemoryStream();
			DomPersistence.WriteProjectContent((ReflectionProjectContent)c.ProjectContent, memory);
			
			memory.Position = 0;
			ReflectionProjectContent loadedPC = DomPersistence.LoadProjectContent(memory, AssemblyParserService.DefaultProjectContentRegistry);
			loadedPC.AddReferencedContent(mscorlib);
			return loadedPC.Classes.Single();
		}
		
		protected override IEnumerable<IAttribute> GetAssemblyAttributes(Assembly assembly)
		{
			var pc = new ReflectionProjectContent("TestName", "testlocation", new DomAssemblyName[0], AssemblyParserService.DefaultProjectContentRegistry);
			pc.AddAssemblyAttributes(assembly);
			
			MemoryStream memory = new MemoryStream();
			DomPersistence.WriteProjectContent(pc, memory);
			
			memory.Position = 0;
			return DomPersistence.LoadProjectContent(memory, AssemblyParserService.DefaultProjectContentRegistry).GetAssemblyAttributes();
		}
	}
	
	[TestFixture]
	public class CecilLayerTests : ReflectionOrCecilLayerTests
	{
		public CecilLayerTests()
		{
			mscorlib = CecilReader.LoadAssembly(typeof(object).Assembly.Location, AssemblyParserService.DefaultProjectContentRegistry);
		}
		
		IProjectContent LoadAssembly(Assembly assembly)
		{
			var pc = CecilReader.LoadAssembly(assembly.Location, AssemblyParserService.DefaultProjectContentRegistry);
			Assert.IsNotNull(pc);
			pc.AddReferencedContent(mscorlib);
			return pc;
		}
		
		protected override IClass GetClass(Type type)
		{
			IClass c = LoadAssembly(type.Assembly).GetClassByReflectionName(type.FullName, false);
			Assert.IsNotNull(c);
			return c;
		}
		
		protected override IEnumerable<IAttribute> GetAssemblyAttributes(Assembly assembly)
		{
			return LoadAssembly(assembly).GetAssemblyAttributes();
		}
	}
	
	public abstract class ReflectionOrCecilLayerTests
	{
		protected IProjectContent mscorlib;
		
		[Test]
		public void InheritanceTest()
		{
			IClass c = mscorlib.GetClass("System.SystemException", 0);
			IClass c2 = mscorlib.GetClass("System.Exception", 0);
			Assert.IsNotNull(c, "c is null");
			Assert.IsNotNull(c2, "c2 is null");
			//Assert.AreEqual(3, c.BaseTypes.Count); // Inherited interfaces are not reported by Cecil
			// which matches the behaviour of our C#/VB parsers
			Assert.AreEqual("System.Exception", c.BaseTypes[0].FullyQualifiedName);
			Assert.AreSame(c2, c.BaseClass);
			
			List<IClass> subClasses = new List<IClass>();
			foreach (IClass subClass in c.ClassInheritanceTree) {
				subClasses.Add(subClass);
			}
			Assert.AreEqual(5, subClasses.Count, "ClassInheritanceTree length");
			Assert.AreEqual("System.SystemException", subClasses[0].FullyQualifiedName);
			Assert.AreEqual("System.Exception", subClasses[1].FullyQualifiedName);
			if (subClasses[2].FullyQualifiedName == "System.Object") {
				Assert.AreEqual("System.Object", subClasses[2].FullyQualifiedName);
				Assert.AreEqual("System.Runtime.Serialization.ISerializable", subClasses[3].FullyQualifiedName);
				Assert.AreEqual("System.Runtime.InteropServices._Exception", subClasses[4].FullyQualifiedName);
			} else {
				Assert.AreEqual("System.Runtime.Serialization.ISerializable", subClasses[2].FullyQualifiedName);
				Assert.AreEqual("System.Runtime.InteropServices._Exception", subClasses[3].FullyQualifiedName);
				Assert.AreEqual("System.Object", subClasses[4].FullyQualifiedName);
			}
		}
		
		[Test]
		public void GenericPropertyTest()
		{
			IClass c = mscorlib.GetClass("System.Collections.Generic.Comparer", 1);
			IProperty def = c.Properties.First(p => p.Name == "Default");
			ConstructedReturnType crt = def.ReturnType.CastToConstructedReturnType();
			Assert.AreEqual("System.Collections.Generic.Comparer", crt.FullyQualifiedName);
			Assert.IsTrue(crt.TypeArguments[0].IsGenericReturnType);
		}
		
		[Test]
		public void PointerTypeTest()
		{
			IClass c = mscorlib.GetClass("System.IntPtr", 1);
			IMethod toPointer = c.Methods.First(p => p.Name == "ToPointer");
			Assert.AreEqual("System.Void*", toPointer.ReturnType.DotNetName);
			PointerReturnType prt = toPointer.ReturnType.CastToDecoratingReturnType<PointerReturnType>();
			Assert.AreEqual("System.Void", prt.BaseType.FullyQualifiedName);
		}
		
		[Test]
		public void DateTimeDefaultConstructor()
		{
			IClass c = mscorlib.GetClass("System.DateTime", 0);
			Assert.IsFalse(c.Methods.Any(p => p.IsConstructor && p.Parameters.Count == 0));
			Assert.IsTrue(c.AddDefaultConstructorIfRequired);
		}
		
		[Test]
		public void NoEncodingInfoDefaultConstructor()
		{
			IClass c = mscorlib.GetClass("System.Text.EncodingInfo", 0);
			Assert.IsFalse(c.Methods.Any(p => p.IsConstructor)); // EncodingInfo only has an internal constructor
			Assert.IsFalse(c.AddDefaultConstructorIfRequired);
		}
		
		[Test]
		public void ParameterComparisonTest()
		{
			DefaultParameter p1 = new DefaultParameter("a", mscorlib.GetClass("System.String", 0).DefaultReturnType, DomRegion.Empty);
			DefaultParameter p2 = new DefaultParameter("b", new GetClassReturnType(mscorlib, "System.String", 0), DomRegion.Empty);
			IList<IParameter> a1 = new List<IParameter>();
			IList<IParameter> a2 = new List<IParameter>();
			a1.Add(p1);
			a2.Add(p2);
			Assert.AreEqual(0, DiffUtility.Compare(a1, a2));
		}
		
		DefaultMethod GetMethod(IClass c, string name)
		{
			IMethod result = c.Methods.FirstOrDefault(m => m.Name == name);
			Assert.IsNotNull(result, "Method " + name + " not found");
			return (DefaultMethod)result;
		}
		
		[Test]
		public void GenericDocumentationTagNamesTest()
		{
			DefaultClass c = (DefaultClass)mscorlib.GetClass("System.Collections.Generic.List", 1);
			Assert.AreEqual("T:System.Collections.Generic.List`1",
			                c.DocumentationTag);
			Assert.AreEqual("M:System.Collections.Generic.List`1.Add(`0)",
			                GetMethod(c, "Add").DocumentationTag);
			Assert.AreEqual("M:System.Collections.Generic.List`1.AddRange(System.Collections.Generic.IEnumerable{`0})",
			                GetMethod(c, "AddRange").DocumentationTag);
			Assert.AreEqual("M:System.Collections.Generic.List`1.ConvertAll``1(System.Converter{`0,``0})",
			                GetMethod(c, "ConvertAll").DocumentationTag);
		}
		
		[Test]
		public void StaticModifierTest()
		{
			IClass c = mscorlib.GetClass("System.Environment", 0);
			Assert.IsNotNull(c, "System.Environment not found");
			Assert.IsTrue(c.IsAbstract, "class should be abstract");
			Assert.IsTrue(c.IsSealed, "class should be sealed");
			Assert.IsTrue(c.IsStatic, "class should be static");
		}
		
		[Test]
		public void InnerClassReferenceTest()
		{
			IClass c = mscorlib.GetClass("System.Environment", 0);
			Assert.IsNotNull(c, "System.Environment not found");
			IReturnType rt = GetMethod(c, "GetFolderPath").Parameters[0].ReturnType;
			Assert.IsNotNull(rt, "ReturnType is null");
			Assert.AreEqual("System.Environment.SpecialFolder", rt.FullyQualifiedName);
			IClass inner = rt.GetUnderlyingClass();
			Assert.IsNotNull(inner, "UnderlyingClass");
			Assert.AreEqual("System.Environment.SpecialFolder", inner.FullyQualifiedName);
		}
		
		[Test]
		public void InnerClassesTest()
		{
			IClass c = mscorlib.GetClass("System.Environment.SpecialFolder", 0);
			Assert.IsNotNull(c, "c is null");
			Assert.AreEqual("System.Environment.SpecialFolder", c.FullyQualifiedName);
		}
		
		[Test]
		public void VoidTest()
		{
			IClass c = mscorlib.GetClass("System.Void", 0);
			Assert.IsNotNull(c, "System.Void not found");
			Assert.AreSame(c.DefaultReturnType, mscorlib.SystemTypes.Void, "pc.SystemTypes.Void is c.DefaultReturnType");
		}
		
		[Test]
		public void NestedClassInGenericClassTest()
		{
			IClass dictionary = mscorlib.GetClass("System.Collections.Generic.Dictionary", 2);
			Assert.IsNotNull(dictionary);
			IClass valueCollection = mscorlib.GetClass("System.Collections.Generic.Dictionary.ValueCollection", 2);
			Assert.IsNotNull(valueCollection);
			var dictionaryRT = new ConstructedReturnType(dictionary.DefaultReturnType, new[] { mscorlib.SystemTypes.String, mscorlib.SystemTypes.Int32 });
			IProperty valueProperty = dictionaryRT.GetProperties().Find(p => p.Name == "Values");
			Assert.AreSame(valueCollection, valueProperty.ReturnType.GetUnderlyingClass());
		}
		
		[Test]
		public void ValueCollectionCountModifiers()
		{
			IClass valueCollection = mscorlib.GetClass("System.Collections.Generic.Dictionary.ValueCollection", 2);
			Assert.AreEqual(ModifierEnum.Public | ModifierEnum.Sealed, valueCollection.Modifiers);
			IProperty count = valueCollection.Properties.Single(p => p.Name == "Count");
			Assert.AreEqual(ModifierEnum.Public | ModifierEnum.Sealed, count.Modifiers);
		}
		
		[Test]
		public void MathAcosModifiers()
		{
			IClass math = mscorlib.GetClass("System.Math", 0);
			Assert.AreEqual(ModifierEnum.Public | ModifierEnum.Abstract | ModifierEnum.Sealed | ModifierEnum.Static, math.Modifiers);
			IMethod acos = math.Methods.Single(p => p.Name == "Acos");
			Assert.AreEqual(ModifierEnum.Public | ModifierEnum.Static, acos.Modifiers);
		}
		
		[Test]
		public void EncodingModifiers()
		{
			IClass encoding = mscorlib.GetClass("System.Text.Encoding", 0);
			Assert.AreEqual(ModifierEnum.Public | ModifierEnum.Abstract, encoding.Modifiers);
			IMethod getDecoder = encoding.Methods.Single(p => p.Name == "GetDecoder");
			Assert.AreEqual(ModifierEnum.Public | ModifierEnum.Virtual, getDecoder.Modifiers);
			IMethod getMaxByteCount = encoding.Methods.Single(p => p.Name == "GetMaxByteCount");
			Assert.AreEqual(ModifierEnum.Public | ModifierEnum.Abstract, getMaxByteCount.Modifiers);
			IProperty encoderFallback = encoding.Properties.Single(p => p.Name == "EncoderFallback");
			Assert.AreEqual(ModifierEnum.Public, encoderFallback.Modifiers);
		}
		
		[Test]
		public void UnicodeEncodingModifiers()
		{
			IClass encoding = mscorlib.GetClass("System.Text.UnicodeEncoding", 0);
			Assert.AreEqual(ModifierEnum.Public, encoding.Modifiers);
			IMethod getDecoder = encoding.Methods.Single(p => p.Name == "GetDecoder");
			Assert.AreEqual(ModifierEnum.Public | ModifierEnum.Override, getDecoder.Modifiers);
		}
		
		[Test]
		public void UTF32EncodingModifiers()
		{
			IClass encoding = mscorlib.GetClass("System.Text.UTF32Encoding", 0);
			Assert.AreEqual(ModifierEnum.Public | ModifierEnum.Sealed, encoding.Modifiers);
			IMethod getDecoder = encoding.Methods.Single(p => p.Name == "GetDecoder");
			Assert.AreEqual(ModifierEnum.Public | ModifierEnum.Override, getDecoder.Modifiers);
		}
		
		public class TestClass<A, B> where A : B {
			public void TestMethod<K, V>(string param) where V: K where K: IComparable {}
			
			public void GetIndex<T>(T element) where T: IEquatable<T> {}
			
			public int Property { get; protected set; }
			public dynamic ReadOnlyPropertyWithPrivateSetter { get; private set; }
			
			public List<dynamic> DynamicGenerics1(Action<object, dynamic[], object> param) { return null; }
			public void DynamicGenerics2(Action<object, dynamic, object> param) { }
			public void DynamicGenerics3(Action<int, dynamic, object> param) { }
			public void DynamicGenerics4(Action<int[], dynamic, object> param) { }
			public void DynamicGenerics5(Action<int*[], dynamic, object> param) { }
		}
		
		protected abstract IClass GetClass(Type type);
		protected abstract IEnumerable<IAttribute> GetAssemblyAttributes(Assembly assembly);
		
		IClass testClass;
		
		[TestFixtureSetUp]
		public void FixtureSetUp()
		{
			testClass = GetClass(typeof(TestClass<,>));
		}
		
		[Test]
		public void TestClassTypeParameters()
		{
			Assert.AreSame(testClass, testClass.TypeParameters[0].Class);
			Assert.AreSame(testClass, testClass.TypeParameters[1].Class);
			Assert.AreSame(testClass.TypeParameters[1], ((GenericReturnType)testClass.TypeParameters[0].Constraints[0]).TypeParameter);
		}
		
		[Test]
		public void TestMethod()
		{
			IMethod m = testClass.Methods.Single(me => me.Name == "TestMethod");
			Assert.AreEqual("K", m.TypeParameters[0].Name);
			Assert.AreEqual("V", m.TypeParameters[1].Name);
			Assert.AreSame(m, m.TypeParameters[0].Method);
			Assert.AreSame(m, m.TypeParameters[1].Method);
			
			Assert.AreEqual("IComparable", m.TypeParameters[0].Constraints[0].Name);
			GenericReturnType kConst = (GenericReturnType)m.TypeParameters[1].Constraints[0];
			Assert.AreSame(m.TypeParameters[0], kConst.TypeParameter);
		}
		
		[Test]
		public void GetIndex()
		{
			IMethod m = testClass.Methods.Single(me => me.Name == "GetIndex");
			Assert.AreEqual("T", m.TypeParameters[0].Name);
			Assert.AreSame(m, m.TypeParameters[0].Method);
			
			Assert.AreEqual("IEquatable", m.TypeParameters[0].Constraints[0].Name);
			Assert.AreEqual(1, m.TypeParameters[0].Constraints[0].TypeArgumentCount);
			Assert.AreEqual(1, m.TypeParameters[0].Constraints[0].CastToConstructedReturnType().TypeArguments.Count);
			GenericReturnType grt = (GenericReturnType)m.TypeParameters[0].Constraints[0].CastToConstructedReturnType().TypeArguments[0];
			Assert.AreSame(m.TypeParameters[0], grt.TypeParameter);
		}
		
		[Test]
		public void Property()
		{
			IProperty p = testClass.Properties.Single(pr => pr.Name == "Property");
			Assert.AreEqual(ModifierEnum.Public, p.Modifiers);
			Assert.AreEqual(ModifierEnum.None, p.GetterModifiers);
			Assert.AreEqual(ModifierEnum.Protected, p.SetterModifiers);
		}
		
		[Test]
		public void CannotSetReadOnlyProperty()
		{
			IProperty p = testClass.Properties.Single(pr => pr.Name == "ReadOnlyPropertyWithPrivateSetter");
			Assert.AreEqual(ModifierEnum.Public, p.Modifiers);
			Assert.AreEqual(ModifierEnum.None, p.GetterModifiers);
			Assert.IsFalse(p.CanSet);
		}
		
		[Test]
		public void DynamicType()
		{
			IProperty p = testClass.Properties.Single(pr => pr.Name == "ReadOnlyPropertyWithPrivateSetter");
			Assert.IsInstanceOf(typeof(DynamicReturnType), p.ReturnType);
		}
		
		[Test]
		public void DynamicTypeInGenerics()
		{
			CSharpAmbience a = new CSharpAmbience();
			a.ConversionFlags = ConversionFlags.ShowReturnType | ConversionFlags.ShowParameterList;
			Assert.AreEqual("List<dynamic> DynamicGenerics1(Action<object, dynamic[], object>)", a.Convert(testClass.Methods.Single(me => me.Name == "DynamicGenerics1")));
			Assert.AreEqual("void DynamicGenerics2(Action<object, dynamic, object>)", a.Convert(testClass.Methods.Single(me => me.Name == "DynamicGenerics2")));
			Assert.AreEqual("void DynamicGenerics3(Action<int, dynamic, object>)", a.Convert(testClass.Methods.Single(me => me.Name == "DynamicGenerics3")));
			Assert.AreEqual("void DynamicGenerics4(Action<int[], dynamic, object>)", a.Convert(testClass.Methods.Single(me => me.Name == "DynamicGenerics4")));
			Assert.AreEqual("void DynamicGenerics5(Action<Int32*[], dynamic, object>)", a.Convert(testClass.Methods.Single(me => me.Name == "DynamicGenerics5")));
		}
		
		[Test]
		public void AssemblyAttribute()
		{
			var attributes = GetAssemblyAttributes(typeof(TypeTestAttribute).Assembly);
			var typeTest = attributes.First(a => a.AttributeType.FullyQualifiedName == typeof(TypeTestAttribute).FullName);
			Assert.AreEqual(3, typeTest.PositionalArguments.Count);
			// first argument is (int)42
			Assert.AreEqual(42, (int)typeTest.PositionalArguments[0]);
			// second argument is typeof(System.Action<>)
			IReturnType rt = (IReturnType)typeTest.PositionalArguments[1];
			Assert.IsNull(rt.CastToConstructedReturnType()); // rt must not be constructed - it's just an unbound type
			Assert.AreEqual("System.Action", rt.FullyQualifiedName);
			Assert.AreEqual(1, rt.TypeArgumentCount);
			// third argument is typeof(IDictionary<string, IList<TestAttribute>>)
			ConstructedReturnType crt = ((IReturnType)typeTest.PositionalArguments[2]).CastToConstructedReturnType();
			Assert.AreEqual("System.Collections.Generic.IDictionary", crt.FullyQualifiedName);
			Assert.AreEqual("System.String", crt.TypeArguments[0].FullyQualifiedName);
			Assert.AreEqual("System.Collections.Generic.IList{NUnit.Framework.TestAttribute}", crt.TypeArguments[1].DotNetName);
		}
	}
}
