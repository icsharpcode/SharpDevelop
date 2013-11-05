// Copyright (c) 2010-2013 AlphaSierraPapa for the SharpDevelop Team
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
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ICSharpCode.NRefactory.CSharp.Resolver;
using ICSharpCode.NRefactory.CSharp.TypeSystem;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.TypeSystem.Implementation;
using NUnit.Framework;

namespace ICSharpCode.NRefactory.CSharp.Refactoring
{
	[TestFixture]
	public class TypeSystemAstBuilderTests
	{
		const string mainProgram = @"
using System;
using System.Collections.Generic;
using OtherNS;

class Base<T> {
	public class Nested<X> { }
	public class Sibling { }
}
class Derived<T, S> : Base<S> { }

namespace NS {
	using R = global::System.Reflection;
	using L = List<char>;
	
	class System { }
}
namespace OtherNS {
	class Array { }
}
";
		
		IProjectContent pc;
		ICompilation compilation;
		ITypeDefinition baseClass, derivedClass, nestedClass, siblingClass, systemClass;
		CSharpUnresolvedFile unresolvedFile;
		
		[SetUp]
		public void SetUp()
		{
			Init(mainProgram);
			
			baseClass = compilation.RootNamespace.GetTypeDefinition("Base", 1);
			nestedClass = baseClass.NestedTypes.Single(t => t.Name == "Nested");
			siblingClass = baseClass.NestedTypes.Single(t => t.Name == "Sibling");
			derivedClass = compilation.RootNamespace.GetTypeDefinition("Derived", 2);
			systemClass = compilation.RootNamespace.GetChildNamespace("NS").GetTypeDefinition("System", 0);
		}
		
		void Init(string program)
		{
			pc = new CSharpProjectContent();
			pc = pc.SetAssemblyName("MyAssembly");
			unresolvedFile = SyntaxTree.Parse(program, "program.cs").ToTypeSystem();
			pc = pc.AddOrUpdateFiles(unresolvedFile);
			pc = pc.AddAssemblyReferences(new [] { CecilLoaderTests.Mscorlib });
			
			compilation = pc.CreateCompilation();
		}
		
		TypeSystemAstBuilder CreateBuilder(ITypeDefinition currentTypeDef = null)
		{
			UsingScope usingScope = currentTypeDef != null ? unresolvedFile.GetUsingScope(currentTypeDef.Region.Begin) : unresolvedFile.RootUsingScope;
			return new TypeSystemAstBuilder(new CSharpResolver(
				new CSharpTypeResolveContext(compilation.MainAssembly, usingScope.Resolve(compilation), currentTypeDef)));
		}
		
		string TypeToString(IType type, ITypeDefinition currentTypeDef = null, Action<TypeSystemAstBuilder> builderAction = null)
		{
			var builder = CreateBuilder(currentTypeDef);
			if (builderAction != null)
				builderAction (builder);
			AstType node = builder.ConvertType(type);
			return node.ToString();
		}
		
		[Test]
		public void PrimitiveTypeNames()
		{
			Assert.AreEqual("void", TypeToString(compilation.FindType(KnownTypeCode.Void)));
			Assert.AreEqual("int", TypeToString(compilation.FindType(KnownTypeCode.Int32)));
			Assert.AreEqual("decimal", TypeToString(compilation.FindType(KnownTypeCode.Decimal)));
		}
		
		[Test]
		public void SystemType()
		{
			Assert.AreEqual("Type", TypeToString(compilation.FindType(KnownTypeCode.Type)));
		}
		
		[Test]
		public void ListOfNSSystem()
		{
			var type = new ParameterizedType(compilation.FindType(typeof(List<>)).GetDefinition(), new[] { systemClass });
			Assert.AreEqual("List<NS.System>", TypeToString(type));
			Assert.AreEqual("List<System>", TypeToString(type, systemClass));
		}
		
		[Test]
		public void NonGenericIEnumerable()
		{
			Assert.AreEqual("System.Collections.IEnumerable", TypeToString(compilation.FindType(typeof(IEnumerable))));
		}
		
		[Test]
		public void NonGenericIEnumerableWithSystemNamespaceCollision()
		{
			Assert.AreEqual("global::System.Collections.IEnumerable", TypeToString(compilation.FindType(typeof(IEnumerable)), systemClass));
		}
		
		[Test]
		public void AliasedNamespace()
		{
			var type = compilation.FindType(typeof(System.Reflection.Assembly));
			Assert.AreEqual("R.Assembly", TypeToString(type, systemClass));
		}
		
		[Test]
		public void AliasedType()
		{
			var type = new ParameterizedType(compilation.FindType(typeof(List<>)).GetDefinition(), new[] { compilation.FindType(KnownTypeCode.Char) });
			Assert.AreEqual("List<char>", TypeToString(type));
			Assert.AreEqual("L", TypeToString(type, systemClass));
		}
		
		[Test]
		public void AliasedTypeWrongTypeArgument()
		{
			var type = new ParameterizedType(compilation.FindType(typeof(List<>)).GetDefinition(), new[] { compilation.FindType(KnownTypeCode.Int32) });
			Assert.AreEqual("List<int>", TypeToString(type, systemClass));
		}
		
		[Test]
		public void UnboundType()
		{
			Assert.AreEqual("Base<>", TypeToString(baseClass));
			Assert.AreEqual("Base<>.Nested<>", TypeToString(nestedClass));
		}

		[Test]
		public void UnboundTypeConvertUnboundTypeArgumentsOption()
		{
			Assert.AreEqual("Base<T>", TypeToString(baseClass, null, builder => builder.ConvertUnboundTypeArguments = true));
			Assert.AreEqual("Base<T>.Nested<X>", TypeToString(nestedClass, null, builder => builder.ConvertUnboundTypeArguments = true));
		}
		
		[Test]
		public void NestedType()
		{
			var type = new ParameterizedType(nestedClass, new[] { compilation.FindType(KnownTypeCode.Char), compilation.FindType(KnownTypeCode.String) });
			Assert.AreEqual("Base<char>.Nested<string>", TypeToString(type));
			// The short form "Nested<string>" refers to "Base<T>.Nested<string>",
			// so we need to use the long form to specify that T=char.
			Assert.AreEqual("Base<char>.Nested<string>", TypeToString(type, baseClass));
			Assert.AreEqual("Base<char>.Nested<string>", TypeToString(type, nestedClass));
			Assert.AreEqual("Base<char>.Nested<string>", TypeToString(type, derivedClass));
		}
		
		[Test]
		public void NestedTypeInCurrentClass()
		{
			var type = new ParameterizedType(nestedClass, new[] { baseClass.TypeParameters[0], compilation.FindType(KnownTypeCode.String) });
			Assert.AreEqual("Nested<string>", TypeToString(type, baseClass));
			Assert.AreEqual("Nested<string>", TypeToString(type, nestedClass));
		}
		
		[Test]
		public void NestedTypeInDerivedClass()
		{
			var type1 = new ParameterizedType(nestedClass, new[] { derivedClass.TypeParameters[0], compilation.FindType(KnownTypeCode.String) });
			// short form "Nested<string>" cannot be used as it would refer to "Base<S>.Nested<string>"
			Assert.AreEqual("Base<T>.Nested<string>", TypeToString(type1, derivedClass));
			
			var type2 = new ParameterizedType(nestedClass, new[] { derivedClass.TypeParameters[1], compilation.FindType(KnownTypeCode.String) });
			Assert.AreEqual("Nested<string>", TypeToString(type2, derivedClass));
		}
		
		[Test]
		public void SiblingClass()
		{
			var type = new ParameterizedType(siblingClass, new[] { baseClass.TypeParameters[0] });
			Assert.AreEqual("Sibling", TypeToString(type, nestedClass));
		}
		
		[Test]
		public void GenericClass()
		{
			var type = new ParameterizedType(nestedClass, new[] { baseClass.TypeParameters[0], compilation.FindType(KnownTypeCode.String) });
			Assert.AreEqual("Nested<string>", TypeToString(type, siblingClass));
		}
		
		[Test]
		public void MultidimensionalArray()
		{
			Assert.AreEqual("byte[][,]", TypeToString(compilation.FindType(typeof(byte[][,]))));
		}
		
		[Test]
		public void Pointer()
		{
			Assert.AreEqual("long*", TypeToString(compilation.FindType(typeof(long*))));
		}
		
		[Test]
		public void NullableType()
		{
			Assert.AreEqual("ulong?", TypeToString(compilation.FindType(typeof(ulong?))));
		}
		
		[Test]
		public void AmbiguousType()
		{
			Assert.AreEqual("System.Array", TypeToString(compilation.FindType(typeof(Array))));
			Assert.AreEqual("OtherNS.Array", TypeToString(compilation.MainAssembly.GetTypeDefinition(new TopLevelTypeName("OtherNS", "Array"))));
		}
		
		[Test]
		public void NestedFooCollidingWithProperty_SameType()
		{
			string program = @"class MainClass {
        public enum Foo { Value1, Value2 }

        public class Test {
            Foo Foo { get; set; }
        }
    }";
			Init(program);
			var foo = compilation.MainAssembly.GetTypeDefinition(new FullTypeName("MainClass+Foo"));
			var test = compilation.MainAssembly.GetTypeDefinition(new FullTypeName("MainClass+Test"));
			Assert.AreEqual("Foo", TypeToString(foo, test));
		}
		
		[Test]
		public void NestedFooCollidingWithProperty_DifferentType()
		{
			string program = @"class MainClass {
        public enum Foo { Value1, Value2 }

        public class Test {
            int Foo { get; set; }
        }
    }";
			Init(program);
			var foo = compilation.MainAssembly.GetTypeDefinition(new FullTypeName("MainClass+Foo"));
			var test = compilation.MainAssembly.GetTypeDefinition(new FullTypeName("MainClass+Test"));
			Assert.AreEqual("MainClass.Foo", TypeToString(foo, test));
		}
	}
}
