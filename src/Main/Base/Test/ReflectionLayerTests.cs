// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Reflection;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Dom.ReflectionLayer;
using NUnit.Framework;

namespace ICSharpCode.SharpDevelop.Tests
{
	[TestFixture]
	public class ReflectionLayerTests : ReflectionOrCecilLayerTests
	{
		public ReflectionLayerTests()
		{
			pc = ParserService.DefaultProjectContentRegistry.Mscorlib;
		}
		
		protected override IClass GetClass(Type type)
		{
			ICompilationUnit cu = new ReflectionProjectContent("TestName", "testlocation", new DomAssemblyName[0], ParserService.DefaultProjectContentRegistry).AssemblyCompilationUnit;
			IClass c = new ReflectionClass(cu, type, type.FullName, null);
			cu.ProjectContent.AddClassToNamespaceList(c);
			return c;
		}
	}
	
	[TestFixture]
	public class ReflectionWithRoundTripLayerTests : ReflectionOrCecilLayerTests
	{
		public ReflectionWithRoundTripLayerTests()
		{
			pc = ParserService.DefaultProjectContentRegistry.Mscorlib;
			
			MemoryStream memory = new MemoryStream();
			DomPersistence.WriteProjectContent((ReflectionProjectContent)pc, memory);
			memory.Position = 0;
			pc = DomPersistence.LoadProjectContent(memory, ParserService.DefaultProjectContentRegistry);
		}
		
		protected override IClass GetClass(Type type)
		{
			ICompilationUnit cu = new ReflectionProjectContent("TestName", "testlocation", new DomAssemblyName[0], ParserService.DefaultProjectContentRegistry).AssemblyCompilationUnit;
			IClass c = new ReflectionClass(cu, type, type.FullName, null);
			cu.ProjectContent.AddClassToNamespaceList(c);
			
			MemoryStream memory = new MemoryStream();
			DomPersistence.WriteProjectContent((ReflectionProjectContent)c.ProjectContent, memory);
			
			memory.Position = 0;
			return DomPersistence.LoadProjectContent(memory, ParserService.DefaultProjectContentRegistry).Classes.Single();
		}
	}
	
	[TestFixture]
	public class CecilLayerTests : ReflectionOrCecilLayerTests
	{
		public CecilLayerTests()
		{
			pc = CecilReader.LoadAssembly(typeof(object).Assembly.Location, ParserService.DefaultProjectContentRegistry);;
		}
		
		protected override IClass GetClass(Type type)
		{
			IProjectContent pc = CecilReader.LoadAssembly(type.Assembly.Location, ParserService.DefaultProjectContentRegistry);
			IClass c = pc.GetClassByReflectionName(type.FullName, false);
			Assert.IsNotNull(c);
			return c;
		}
	}
	
	public abstract class ReflectionOrCecilLayerTests
	{
		protected IProjectContent pc;
		
		[Test]
		public void InheritanceTest()
		{
			IClass c = pc.GetClass("System.SystemException", 0);
			IClass c2 = pc.GetClass("System.Exception", 0);
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
			IClass c = pc.GetClass("System.Collections.Generic.Comparer", 1);
			IProperty def = c.Properties.First(p => p.Name == "Default");
			ConstructedReturnType crt = def.ReturnType.CastToConstructedReturnType();
			Assert.AreEqual("System.Collections.Generic.Comparer", crt.FullyQualifiedName);
			Assert.IsTrue(crt.TypeArguments[0].IsGenericReturnType);
		}
		
		[Test]
		public void PointerTypeTest()
		{
			IClass c = pc.GetClass("System.IntPtr", 1);
			IMethod toPointer = c.Methods.First(p => p.Name == "ToPointer");
			Assert.AreEqual("System.Void*", toPointer.ReturnType.DotNetName);
			PointerReturnType prt = toPointer.ReturnType.CastToDecoratingReturnType<PointerReturnType>();
			Assert.AreEqual("System.Void", prt.BaseType.FullyQualifiedName);
		}
		
		[Test]
		public void ParameterComparisonTest()
		{
			DefaultParameter p1 = new DefaultParameter("a", pc.GetClass("System.String", 0).DefaultReturnType, DomRegion.Empty);
			DefaultParameter p2 = new DefaultParameter("b", new GetClassReturnType(pc, "System.String", 0), DomRegion.Empty);
			IList<IParameter> a1 = new List<IParameter>();
			IList<IParameter> a2 = new List<IParameter>();
			a1.Add(p1);
			a2.Add(p2);
			Assert.AreEqual(0, DiffUtility.Compare(a1, a2));
		}
		
		DefaultMethod GetMethod(IClass c, string name) {
			IMethod result = c.Methods.FirstOrDefault(delegate(IMethod m) { return m.Name == name; });
			Assert.IsNotNull(result, "Method " + name + " not found");
			return (DefaultMethod)result;
		}
		
		[Test]
		public void GenericDocumentationTagNamesTest()
		{
			DefaultClass c = (DefaultClass)pc.GetClass("System.Collections.Generic.List", 1);
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
		public void InnerClassReferenceTest()
		{
			IClass c = pc.GetClass("System.Environment", 0);
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
			IClass c = pc.GetClass("System.Environment.SpecialFolder", 0);
			Assert.IsNotNull(c, "c is null");
			Assert.AreEqual("System.Environment.SpecialFolder", c.FullyQualifiedName);
		}
		
		[Test]
		public void VoidTest()
		{
			IClass c = pc.GetClass("System.Void", 0);
			Assert.IsNotNull(c, "System.Void not found");
			Assert.AreSame(c.DefaultReturnType, pc.SystemTypes.Void, "pc.SystemTypes.Void is c.DefaultReturnType");
		}
		
		public class TestClass<A, B> where A : B {
			public void TestMethod<K, V>(string param) where V: K where K: IComparable {}
			
			public void GetIndex<T>(T element) where T: IEquatable<T> {}
		}
		
		protected abstract IClass GetClass(Type type);
		
		[Test]
		public void ReflectionParserTest()
		{
			IClass c = GetClass(typeof(TestClass<,>));
			
			CheckClass(c);
		}
		
		void CheckClass(IClass c)
		{
			Assert.AreSame(c, c.TypeParameters[0].Class);
			Assert.AreSame(c, c.TypeParameters[1].Class);
			Assert.AreSame(c.TypeParameters[1], ((GenericReturnType)c.TypeParameters[0].Constraints[0]).TypeParameter);
			
			IMethod m = c.Methods.First(delegate(IMethod me) { return me.Name == "TestMethod"; });
			Assert.IsNotNull(m);
			Assert.AreEqual("K", m.TypeParameters[0].Name);
			Assert.AreEqual("V", m.TypeParameters[1].Name);
			Assert.AreSame(m, m.TypeParameters[0].Method);
			Assert.AreSame(m, m.TypeParameters[1].Method);
			
			Assert.AreEqual("IComparable", m.TypeParameters[0].Constraints[0].Name);
			GenericReturnType kConst = (GenericReturnType)m.TypeParameters[1].Constraints[0];
			Assert.AreSame(m.TypeParameters[0], kConst.TypeParameter);
			
			m = c.Methods.First(delegate(IMethod me) { return me.Name == "GetIndex"; });
			Assert.IsNotNull(m);
			Assert.AreEqual("T", m.TypeParameters[0].Name);
			Assert.AreSame(m, m.TypeParameters[0].Method);
			
			Assert.AreEqual("IEquatable", m.TypeParameters[0].Constraints[0].Name);
			Assert.AreEqual(1, m.TypeParameters[0].Constraints[0].TypeArgumentCount);
			Assert.AreEqual(1, m.TypeParameters[0].Constraints[0].CastToConstructedReturnType().TypeArguments.Count);
			GenericReturnType grt = (GenericReturnType)m.TypeParameters[0].Constraints[0].CastToConstructedReturnType().TypeArguments[0];
			Assert.AreSame(m.TypeParameters[0], grt.TypeParameter);
		}
	}
}
