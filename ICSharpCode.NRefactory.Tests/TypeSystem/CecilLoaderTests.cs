// Copyright (c) 2010 AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under MIT X11 license (for details please see \doc\license.txt)

using System;
using ICSharpCode.NRefactory.TypeSystem.Implementation;
using NUnit.Framework;

namespace ICSharpCode.NRefactory.TypeSystem
{
	[TestFixture]
	public class CecilLoaderTests : TypeSystemTests
	{
		public static readonly IProjectContent Mscorlib = new CecilLoader().LoadAssemblyFile(typeof(object).Assembly.Location);
		
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
			ITypeResolveContext ctx = Mscorlib;
			ITypeDefinition c = Mscorlib.GetClass(typeof(SystemException));
			ITypeDefinition c2 = Mscorlib.GetClass(typeof(Exception));
			Assert.IsNotNull(c, "c is null");
			Assert.IsNotNull(c2, "c2 is null");
			//Assert.AreEqual(3, c.BaseTypes.Count); // Inherited interfaces are not reported by Cecil
			// which matches the behaviour of our C#/VB parsers
			Assert.AreEqual("System.Exception", c.BaseTypes[0].Resolve(ctx).FullName);
			Assert.AreSame(c2, c.BaseTypes[0]);
			
			/*
			List<ITypeDefinition> subClasses = new List<ITypeDefinition>();
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
			}*/
		}
	}
}
