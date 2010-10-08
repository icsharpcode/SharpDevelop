// Copyright (c) 2010 AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under MIT X11 license (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.NRefactory.TypeSystem.Implementation;
using NUnit.Framework;

namespace ICSharpCode.NRefactory.TypeSystem
{
	[TestFixture]
	public class CecilLoaderTests : TypeSystemTests
	{
		public static readonly IProjectContent Mscorlib = new CecilLoader().LoadAssemblyFile(typeof(object).Assembly.Location);
		ITypeResolveContext ctx = Mscorlib;
		
		[TestFixtureSetUp]
		public void SetUp()
		{
			// use "IncludeInternalMembers" so that Cecil results match C# parser results
			CecilLoader loader = new CecilLoader() { IncludeInternalMembers = true };
			testCasePC = loader.LoadAssemblyFile(typeof(TestCase.SimplePublicClass).Assembly.Location);
		}
		
		[Test]
		public void InheritanceTest()
		{
			ITypeDefinition c = Mscorlib.GetClass(typeof(SystemException));
			ITypeDefinition c2 = Mscorlib.GetClass(typeof(Exception));
			Assert.IsNotNull(c, "c is null");
			Assert.IsNotNull(c2, "c2 is null");
			//Assert.AreEqual(3, c.BaseTypes.Count); // Inherited interfaces are not reported by Cecil
			// which matches the behaviour of our C#/VB parsers
			Assert.AreEqual("System.Exception", c.BaseTypes[0].Resolve(ctx).FullName);
			Assert.AreSame(c2, c.BaseTypes[0]);
			
			string[] superTypes = c.GetAllBaseTypes(ctx).Select(t => t.ToString()).ToArray();
			Assert.AreEqual(new string[] {
			                	"System.SystemException", "System.Exception", "System.Object",
			                	"System.Runtime.Serialization.ISerializable", "System.Runtime.InteropServices._Exception"
			                }, superTypes);
		}
		
		[Test]
		public void GenericPropertyTest()
		{
			ITypeDefinition c = Mscorlib.GetClass(typeof(Comparer<>));
			IProperty def = c.Properties.Single(p => p.Name == "Default");
			ParameterizedType pt = (ParameterizedType)def.ReturnType.Resolve(ctx);
			Assert.AreEqual("System.Collections.Generic.Comparer", pt.FullName);
			Assert.AreSame(c.TypeParameters[0], pt.TypeArguments[0]);
		}
		
		[Test]
		public void PointerTypeTest()
		{
			ITypeDefinition c = Mscorlib.GetClass(typeof(IntPtr));
			IMethod toPointer = c.Methods.Single(p => p.Name == "ToPointer");
			Assert.AreEqual("System.Void*", toPointer.ReturnType.Resolve(ctx).DotNetName);
			Assert.IsInstanceOf(typeof(PointerType), toPointer.ReturnType.Resolve(ctx));
			Assert.AreEqual("System.Void", toPointer.ReturnType.Resolve(ctx).GetElementType().FullName);
		}
		
		[Test]
		public void DateTimeDefaultConstructor()
		{
			ITypeDefinition c = Mscorlib.GetClass(typeof(DateTime));
			Assert.IsFalse(c.Methods.Any(m => m.IsConstructor && m.Parameters.Count == 0)); // struct ctor isn't declared
			// but it is implicit:
			Assert.IsTrue(c.GetConstructors(ctx).Any(m => m.Parameters.Count == 0));
		}
		
		[Test]
		public void NoEncodingInfoDefaultConstructor()
		{
			ITypeDefinition c = Mscorlib.GetClass(typeof(EncodingInfo));
			// EncodingInfo only has an internal constructor
			Assert.IsFalse(c.Methods.Any(m => m.IsConstructor));
			// and no implicit ctor should be added:
			Assert.AreEqual(0, c.GetConstructors(ctx).Count);
		}
		
		[Test]
		public void StaticModifierTest()
		{
			ITypeDefinition c = Mscorlib.GetClass(typeof(Environment));
			Assert.IsNotNull(c, "System.Environment not found");
			Assert.IsTrue(c.IsAbstract, "class should be abstract");
			Assert.IsTrue(c.IsSealed, "class should be sealed");
			Assert.IsTrue(c.IsStatic, "class should be static");
		}
		
		[Test]
		public void InnerClassReferenceTest()
		{
			ITypeDefinition c = Mscorlib.GetClass(typeof(Environment));
			Assert.IsNotNull(c, "System.Environment not found");
			ITypeReference rt = c.Methods.First(m => m.Name == "GetFolderPath").Parameters[0].Type;
			Assert.AreSame(c.InnerClasses.Single(ic => ic.Name == "SpecialFolder"), rt.Resolve(ctx));
		}
		
		[Test]
		public void InnerClassesTest()
		{
			ITypeDefinition c = Mscorlib.GetClass(typeof(Environment.SpecialFolder));
			Assert.IsNotNull(c, "c is null");
			Assert.AreEqual("System.Environment.SpecialFolder", c.FullName);
			Assert.AreEqual("System.Environment+SpecialFolder", c.DotNetName);
		}
		
		[Test]
		public void VoidTest()
		{
			ITypeDefinition c = Mscorlib.GetClass(typeof(void));
			Assert.IsNotNull(c, "System.Void not found");
			Assert.AreEqual(0, c.GetMethods(ctx).Count);
			Assert.AreEqual(
				new string[] {
					"[System.SerializableAttribute]",
					"[System.Runtime.InteropServices.StructLayoutAttribute(0, Size=1)]",
					"[System.Runtime.InteropServices.ComVisibleAttribute(true)]"
				},
				c.Attributes.Select(a => a.ToString()).ToArray());
		}
	}
}
