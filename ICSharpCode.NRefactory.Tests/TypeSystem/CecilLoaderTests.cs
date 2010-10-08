// Copyright (c) 2010 AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under MIT X11 license (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
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
	}
}
