// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Reflection;

using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Project;
using NUnit.Framework;

namespace ICSharpCode.SharpDevelop.Tests
{
	[TestFixture]
	public class GetElementByReflectionNameTests
	{
		IProjectContent mscorlib = AssemblyParserService.DefaultProjectContentRegistry.Mscorlib;
		
		void TestClass(Type type)
		{
			Assert.AreSame(typeof(object).Assembly, type.Assembly);
			IClass c = (IClass)mscorlib.GetClassByReflectionName(type.FullName, false);
			Assert.AreEqual(type.FullName, c.DotNetName);
		}
		
		[Test]
		public void TestClasses()
		{
			TestClass(typeof(object));
			TestClass(typeof(Nullable));
			TestClass(typeof(Nullable<>));
		}
		
		[Test]
		public void TestNestedClass()
		{
			TestClass(typeof(Environment.SpecialFolder));
			TestClass(typeof(Dictionary<,>.ValueCollection));
		}
		
		void TestMember(string className, string memberName)
		{
			IClass c = mscorlib.GetClassByReflectionName(className, false);
			Assert.IsNotNull(c);
			AbstractMember m = (AbstractMember)DefaultProjectContent.GetMemberByReflectionName(c, memberName);
			Assert.AreEqual(className + "." + memberName, m.DocumentationTag.Substring(2));
		}
		
		[Test]
		public void TestConstructor()
		{
			TestMember("System.Collections.ObjectModel.KeyedCollection`2", "#ctor");
			TestMember("System.Collections.ObjectModel.KeyedCollection`2", "#ctor(System.Collections.Generic.IEqualityComparer{`0},System.Int32)");
			TestMember("System.Runtime.InteropServices.CurrencyWrapper", "#ctor(System.Decimal)");
			TestMember("System.Runtime.InteropServices.CurrencyWrapper", "#ctor(System.Object)");
		}
	}
}
