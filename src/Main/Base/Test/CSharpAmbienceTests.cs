// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using ICSharpCode.Core;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.TypeSystem.Implementation;
using ICSharpCode.SharpDevelop.Parser;
using NUnit.Framework;

namespace ICSharpCode.SharpDevelop.Tests
{
	[TestFixture]
	public class CSharpAmbienceTests
	{
		IProjectContent mscorlib;
		IProjectContent myLib;
		CompositeTypeResolveContext compositeContext;
		CSharpAmbience ambience;
		
		public CSharpAmbienceTests()
		{
			ambience = new CSharpAmbience();
			mscorlib = AssemblyParserService.GetAssembly(FileName.Create(typeof(object).Assembly.Location));
			var loader = new CecilLoader();
			loader.IncludeInternalMembers = true;
			myLib = loader.LoadAssemblyFile(typeof(CSharpAmbienceTests).Assembly.Location);
			compositeContext = new CompositeTypeResolveContext(new[] { mscorlib, myLib });
		}
		
		#region ITypeDefinition tests
		[Test]
		public void GenericType()
		{
			var typeDef = mscorlib.GetTypeDefinition(typeof(Dictionary<,>));
			ambience.ConversionFlags = ConversionFlags.UseFullyQualifiedMemberNames | ConversionFlags.ShowTypeParameterList;
			string result = ambience.ConvertEntity(typeDef, mscorlib);
			
			Assert.AreEqual("System.Collections.Generic.Dictionary<TKey, TValue>", result);
		}
		
		[Test]
		public void GenericTypeShortName()
		{
			var typeDef = mscorlib.GetTypeDefinition(typeof(Dictionary<,>));
			ambience.ConversionFlags = ConversionFlags.ShowTypeParameterList;
			string result = ambience.ConvertEntity(typeDef, mscorlib);
			
			Assert.AreEqual("Dictionary<TKey, TValue>", result);
		}
		
		[Test]
		public void SimpleType()
		{
			var typeDef = mscorlib.GetTypeDefinition(typeof(Object));
			ambience.ConversionFlags = ConversionFlags.UseFullyQualifiedMemberNames | ConversionFlags.ShowTypeParameterList;
			string result = ambience.ConvertEntity(typeDef, mscorlib);
			
			Assert.AreEqual("System.Object", result);
		}
		
		[Test]
		public void SimpleTypeDefinition()
		{
			var typeDef = mscorlib.GetTypeDefinition(typeof(Object));
			ambience.ConversionFlags = ConversionFlags.All & ~(ConversionFlags.UseFullyQualifiedMemberNames);
			string result = ambience.ConvertEntity(typeDef, mscorlib);
			
			Assert.AreEqual("public class Object", result);
		}
		
		[Test]
		public void SimpleTypeDefinitionWithoutModifiers()
		{
			var typeDef = mscorlib.GetTypeDefinition(typeof(Object));
			ambience.ConversionFlags = ConversionFlags.All & ~(ConversionFlags.UseFullyQualifiedMemberNames | ConversionFlags.ShowModifiers | ConversionFlags.ShowAccessibility);
			string result = ambience.ConvertEntity(typeDef, mscorlib);
			
			Assert.AreEqual("class Object", result);
		}
		
		[Test]
		public void GenericTypeDefinitionFull()
		{
			var typeDef = mscorlib.GetTypeDefinition(typeof(List<>));
			ambience.ConversionFlags = ConversionFlags.All;
			string result = ambience.ConvertEntity(typeDef, mscorlib);
			
			Assert.AreEqual("public class System.Collections.Generic.List<T>", result);
		}
		
		[Test]
		public void SimpleTypeShortName()
		{
			var typeDef = mscorlib.GetTypeDefinition(typeof(Object));
			ambience.ConversionFlags = ConversionFlags.ShowTypeParameterList;
			string result = ambience.ConvertEntity(typeDef, mscorlib);
			
			Assert.AreEqual("Object", result);
		}
		
		[Test]
		public void GenericTypeWithNested()
		{
			var typeDef = mscorlib.GetTypeDefinition(typeof(List<>.Enumerator));
			ambience.ConversionFlags = ConversionFlags.UseFullyQualifiedMemberNames | ConversionFlags.ShowTypeParameterList;
			string result = ambience.ConvertEntity(typeDef, mscorlib);
			
			Assert.AreEqual("System.Collections.Generic.List<T>.Enumerator", result);
		}
		
		[Test]
		public void GenericTypeWithNestedShortName()
		{
			var typeDef = mscorlib.GetTypeDefinition(typeof(List<>.Enumerator));
			ambience.ConversionFlags = ConversionFlags.ShowTypeParameterList;
			string result = ambience.ConvertEntity(typeDef, mscorlib);
			
			Assert.AreEqual("List<T>.Enumerator", result);
		}
		#endregion
		
		#region IField tests
		[Test]
		public void SimpleField()
		{
			var field = typeof(CSharpAmbienceTests.MyClass).ToTypeReference().Resolve(myLib)
				.GetDefinition().Fields.Single(f => f.Name == "test");
			ambience.ConversionFlags = ConversionFlags.All;
			string result = ambience.ConvertEntity(field, compositeContext);
			
			Assert.AreEqual("private int ICSharpCode.SharpDevelop.Tests.CSharpAmbienceTests.MyClass.test", result);
		}
		
		[Test]
		public void SimpleConstField()
		{
			var field = typeof(CSharpAmbienceTests.MyClass).ToTypeReference().Resolve(myLib)
				.GetDefinition().Fields.Single(f => f.Name == "TEST");
			ambience.ConversionFlags = ConversionFlags.All;
			string result = ambience.ConvertEntity(field, compositeContext);
			
			Assert.AreEqual("private const int ICSharpCode.SharpDevelop.Tests.CSharpAmbienceTests.MyClass.TEST", result);
		}
		
		[Test]
		public void SimpleFieldWithoutModifiers()
		{
			var field = typeof(CSharpAmbienceTests.MyClass).ToTypeReference().Resolve(myLib)
				.GetDefinition().Fields.Single(f => f.Name == "test");
			ambience.ConversionFlags = ConversionFlags.All & ~(ConversionFlags.UseFullyQualifiedMemberNames | ConversionFlags.ShowModifiers | ConversionFlags.ShowAccessibility);
			string result = ambience.ConvertEntity(field, compositeContext);
			
			Assert.AreEqual("int test", result);
		}
		#endregion
		
		#region Test types
		#pragma warning disable 169
		
		class MyClass
		{
			const int TEST = 0;
			int test;
		}
		#endregion
	}
}
