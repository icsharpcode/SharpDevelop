// Copyright (c) AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Linq;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.TypeSystem.Implementation;
using NUnit.Framework;

namespace ICSharpCode.NRefactory.CSharp
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
			mscorlib = CecilLoaderTests.Mscorlib;
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
			var field = typeof(CSharpAmbienceTests.Program).ToTypeReference().Resolve(myLib)
				.GetDefinition().Fields.Single(f => f.Name == "test");
			ambience.ConversionFlags = ConversionFlags.All;
			string result = ambience.ConvertEntity(field, compositeContext);
			
			Assert.AreEqual("private int ICSharpCode.NRefactory.CSharp.CSharpAmbienceTests.Program.test", result);
		}
		
		[Test]
		public void SimpleConstField()
		{
			var field = typeof(CSharpAmbienceTests.Program).ToTypeReference().Resolve(myLib)
				.GetDefinition().Fields.Single(f => f.Name == "TEST2");
			ambience.ConversionFlags = ConversionFlags.All;
			string result = ambience.ConvertEntity(field, compositeContext);
			
			Assert.AreEqual("private const int ICSharpCode.NRefactory.CSharp.CSharpAmbienceTests.Program.TEST2", result);
		}
		
		[Test]
		public void SimpleFieldWithoutModifiers()
		{
			var field = typeof(CSharpAmbienceTests.Program).ToTypeReference().Resolve(myLib)
				.GetDefinition().Fields.Single(f => f.Name == "test");
			ambience.ConversionFlags = ConversionFlags.All & ~(ConversionFlags.UseFullyQualifiedMemberNames | ConversionFlags.ShowModifiers | ConversionFlags.ShowAccessibility);
			string result = ambience.ConvertEntity(field, compositeContext);
			
			Assert.AreEqual("int test", result);
		}
		#endregion
		
		#region Test types
		#pragma warning disable 169
		
		class Test {}
		
		class Program
		{
			int test;
			const int TEST2 = 2;
			
			public int Test { get; set; }
			
			public int this[int index] {
				get {
					return index;
				}
			}
			
			public event EventHandler ProgramChanged;
			
			public event EventHandler SomeEvent {
				add {}
				remove {}
			}
			
			public static bool operator +(Program lhs, Program rhs)
			{
				throw new NotImplementedException();
			}
			
			public static implicit operator Test(Program lhs)
			{
				throw new NotImplementedException();
			}
			
			public static explicit operator int(Program lhs)
			{
				throw new NotImplementedException();
			}
			
			public Program(int x)
			{
				
			}
			
			~Program()
			{
				
			}
			
			public static void Main(string[] args)
			{
				Console.WriteLine("Hello World!");
				
				// TODO: Implement Functionality Here
				
				Console.Write("Press any key to continue . . . ");
				Console.ReadKey(true);
			}
		}
		#endregion
	}
}
