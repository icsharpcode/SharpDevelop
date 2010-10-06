// Copyright (c) 2010 AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under MIT X11 license (for details please see \doc\license.txt)

using System;
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
	}
}
