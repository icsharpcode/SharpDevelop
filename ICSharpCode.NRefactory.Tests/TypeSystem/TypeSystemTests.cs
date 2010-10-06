// Copyright (c) 2010 AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under MIT X11 license (for details please see \doc\license.txt)

using System;
using System.Linq;
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
		
		[Test]
		public void SimplePublicClassTest()
		{
			ITypeDefinition c = testCasePC.GetClass(typeof(SimplePublicClass).FullName, 0, StringComparer.Ordinal);
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
			ITypeDefinition c = testCasePC.GetClass(typeof(SimplePublicClass).FullName, 0, StringComparer.Ordinal);
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
		[Ignore]
		public void DynamicTypeInGenerics()
		{
			ITypeDefinition testClass = testCasePC.GetClass(typeof(DynamicTest).FullName, 0, StringComparer.Ordinal);
			/*CSharpAmbience a = new CSharpAmbience();
			a.ConversionFlags = ConversionFlags.ShowReturnType | ConversionFlags.ShowParameterList;
			Assert.AreEqual("List<dynamic> DynamicGenerics1(Action<object, dynamic[], object>)", a.Convert(testClass.Methods.Single(me => me.Name == "DynamicGenerics1")));
			Assert.AreEqual("void DynamicGenerics2(Action<object, dynamic, object>)", a.Convert(testClass.Methods.Single(me => me.Name == "DynamicGenerics2")));
			Assert.AreEqual("void DynamicGenerics3(Action<int, dynamic, object>)", a.Convert(testClass.Methods.Single(me => me.Name == "DynamicGenerics3")));
			Assert.AreEqual("void DynamicGenerics4(Action<int[], dynamic, object>)", a.Convert(testClass.Methods.Single(me => me.Name == "DynamicGenerics4")));
			Assert.AreEqual("void DynamicGenerics5(Action<Int32*[], dynamic, object>)", a.Convert(testClass.Methods.Single(me => me.Name == "DynamicGenerics5")));*/
		}
	}
}
