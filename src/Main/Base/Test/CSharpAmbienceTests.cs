// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.Core;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop.Parser;
using NUnit.Framework;

namespace ICSharpCode.SharpDevelop.Tests
{
	[TestFixture]
	public class CSharpAmbienceTests
	{
		IProjectContent mscorlib;
		CSharpAmbience ambience;
		
		[SetUp]
		public void Setup()
		{
			ambience = new CSharpAmbience();
			mscorlib = AssemblyParserService.GetAssembly(FileName.Create(typeof(object).Assembly.Location));
		}
		
		[Test]
		public void GenericType()
		{
			var typeDef = mscorlib.GetTypeDefinition(typeof(Dictionary<,>));
			ambience.ConversionFlags = ConversionFlags.UseFullyQualifiedMemberNames | ConversionFlags.ShowTypeParameterList;
			string result = ambience.ConvertEntity(typeDef);
			
			Assert.AreEqual("System.Collections.Generic.Dictionary<TKey, TValue>", result);
		}
		
		[Test]
		public void GenericTypeShortName()
		{
			var typeDef = mscorlib.GetTypeDefinition(typeof(Dictionary<,>));
			ambience.ConversionFlags = ConversionFlags.ShowTypeParameterList;
			string result = ambience.ConvertEntity(typeDef);
			
			Assert.AreEqual("Dictionary<TKey, TValue>", result);
		}
		
		[Test]
		public void SimpleType()
		{
			var typeDef = mscorlib.GetTypeDefinition(typeof(Object));
			ambience.ConversionFlags = ConversionFlags.UseFullyQualifiedMemberNames | ConversionFlags.ShowTypeParameterList;
			string result = ambience.ConvertEntity(typeDef);
			
			Assert.AreEqual("System.Object", result);
		}
		
		[Test]
		public void SimpleTypeDefinition()
		{
			var typeDef = mscorlib.GetTypeDefinition(typeof(Object));
			ambience.ConversionFlags = ConversionFlags.All & ~(ConversionFlags.UseFullyQualifiedMemberNames);
			string result = ambience.ConvertEntity(typeDef);
			
			Assert.AreEqual("public class Object", result);
		}
		
		[Test]
		public void SimpleTypeDefinitionWithoutModifiers()
		{
			var typeDef = mscorlib.GetTypeDefinition(typeof(Object));
			ambience.ConversionFlags = ConversionFlags.All & ~(ConversionFlags.UseFullyQualifiedMemberNames | ConversionFlags.ShowModifiers | ConversionFlags.ShowAccessibility);
			string result = ambience.ConvertEntity(typeDef);
			
			Assert.AreEqual("class Object", result);
		}
		
		[Test]
		public void GenericTypeDefinitionFull()
		{
			var typeDef = mscorlib.GetTypeDefinition(typeof(List<>));
			ambience.ConversionFlags = ConversionFlags.All;
			string result = ambience.ConvertEntity(typeDef);
			
			Assert.AreEqual("public class System.Collections.Generic.List<T>", result);
		}
		
		[Test]
		public void SimpleTypeShortName()
		{
			var typeDef = mscorlib.GetTypeDefinition(typeof(Object));
			ambience.ConversionFlags = ConversionFlags.ShowTypeParameterList;
			string result = ambience.ConvertEntity(typeDef);
			
			Assert.AreEqual("Object", result);
		}
		
		[Test]
		public void GenericTypeWithNested()
		{
			var typeDef = mscorlib.GetTypeDefinition(typeof(List<>.Enumerator));
			ambience.ConversionFlags = ConversionFlags.UseFullyQualifiedMemberNames | ConversionFlags.ShowTypeParameterList;
			string result = ambience.ConvertEntity(typeDef);
			
			Assert.AreEqual("System.Collections.Generic.List<T>.Enumerator", result);
		}
		
		[Test]
		public void GenericTypeWithNestedShortName()
		{
			var typeDef = mscorlib.GetTypeDefinition(typeof(List<>.Enumerator));
			ambience.ConversionFlags = ConversionFlags.ShowTypeParameterList;
			string result = ambience.ConvertEntity(typeDef);
			
			Assert.AreEqual("List<T>.Enumerator", result);
		}
	}
}
