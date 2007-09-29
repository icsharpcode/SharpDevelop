// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Reflection;
using NUnit.Framework;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop.Tests
{
	[TestFixture]
	public class GetElementByReflectionNameTests
	{
		IProjectContent mscorlib = ParserService.DefaultProjectContentRegistry.Mscorlib;
		
		void TestClass(Type type)
		{
			Assert.AreSame(typeof(object).Assembly, type.Assembly);
			IClass c = (IClass)mscorlib.GetElement(type.FullName);
			Assert.AreEqual(type.FullName, c.DotNetName);
		}
		
		[Test]
		public void TestClasses()
		{
			TestClass(typeof(object));
			TestClass(typeof(Environment.SpecialFolder));
			TestClass(typeof(Nullable));
			TestClass(typeof(Nullable<>));
		}
		
		void TestMember(string fullName)
		{
			AbstractMember d = (AbstractMember)mscorlib.GetElement(fullName);
			Assert.AreEqual(fullName, d.DocumentationTag.Substring(2));
		}
		
		[Test]
		public void TestConstructor()
		{
			TestMember("System.Collections.ObjectModel.KeyedCollection`2.#ctor");
			TestMember("System.Collections.ObjectModel.KeyedCollection`2.#ctor(System.Collections.Generic.IEqualityComparer{`0},System.Int32)");
			TestMember("System.Runtime.InteropServices.CurrencyWrapper.#ctor(System.Decimal)");
			TestMember("System.Runtime.InteropServices.CurrencyWrapper.#ctor(System.Object)");
		}
	}
}
