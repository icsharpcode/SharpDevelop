// Copyright (c) 2010 AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under MIT X11 license (for details please see \doc\license.txt)

using System;
using System.Linq;
using ICSharpCode.NRefactory.TypeSystem.Implementation;
using NUnit.Framework;

namespace ICSharpCode.NRefactory.TypeSystem
{
	[TestFixture]
	public class GetMembersTests
	{
		IProjectContent mscorlib = CecilLoaderTests.Mscorlib;
		
		[Test]
		public void EmptyClassHasToString()
		{
			DefaultTypeDefinition c = new DefaultTypeDefinition(mscorlib, string.Empty, "C");
			Assert.AreEqual("System.Object.ToString", c.GetMethods(mscorlib, m => m.Name == "ToString").Single().FullName);
		}
		
		[Test]
		public void MultipleInheritanceTest()
		{
			DefaultTypeDefinition b1 = new DefaultTypeDefinition(mscorlib, string.Empty, "B1");
			b1.ClassType = ClassType.Interface;
			b1.Properties.Add(new DefaultProperty(b1, "P1"));
			
			DefaultTypeDefinition b2 = new DefaultTypeDefinition(mscorlib, string.Empty, "B1");
			b2.ClassType = ClassType.Interface;
			b2.Properties.Add(new DefaultProperty(b1, "P2"));
			
			DefaultTypeDefinition c = new DefaultTypeDefinition(mscorlib, string.Empty, "C");
			c.ClassType = ClassType.Interface;
			c.BaseTypes.Add(b1);
			c.BaseTypes.Add(b2);
			
			Assert.AreEqual(new[] { "P1", "P2" }, c.GetProperties(mscorlib).Select(p => p.Name).ToArray());
			// Test that there's only one copy of ToString():
			Assert.AreEqual(1, c.GetMethods(mscorlib, m => m.Name == "ToString").Count());
		}
	}
}
