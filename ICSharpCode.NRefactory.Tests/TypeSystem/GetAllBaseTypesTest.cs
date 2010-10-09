// Copyright (c) 2010 AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under MIT X11 license (for details please see \doc\license.txt)

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ICSharpCode.NRefactory.TypeSystem.Implementation;
using NUnit.Framework;

namespace ICSharpCode.NRefactory.TypeSystem
{
	[TestFixture]
	public class GetAllBaseTypesTest
	{
		IProjectContent mscorlib = CecilLoaderTests.Mscorlib;
		ITypeResolveContext context = CecilLoaderTests.Mscorlib;
		
		IType[] GetAllBaseTypes(Type type)
		{
			return type.ToTypeReference().Resolve(context).GetAllBaseTypes(context).OrderBy(t => t.DotNetName).ToArray();
		}
		
		IType[] GetTypes(params Type[] types)
		{
			return types.Select(t => t.ToTypeReference().Resolve(context)).OrderBy(t => t.DotNetName).ToArray();;
		}
		
		[Test]
		public void ObjectBaseTypes()
		{
			Assert.AreEqual(GetTypes(typeof(object)), GetAllBaseTypes(typeof(object)));
		}
		
		[Test]
		public void StringBaseTypes()
		{
			Assert.AreEqual(GetTypes(typeof(string), typeof(object), typeof(IComparable), typeof(ICloneable), typeof(IConvertible),
			                         typeof(IComparable<string>), typeof(IEquatable<string>), typeof(IEnumerable<char>), typeof(IEnumerable)),
			                GetAllBaseTypes(typeof(string)));
		}
		
		[Test]
		public void ClassDerivingFromItself()
		{
			// class C : C {}
			DefaultTypeDefinition c = new DefaultTypeDefinition(mscorlib, string.Empty, "C");
			c.BaseTypes.Add(c);
			Assert.AreEqual(new [] { c }, c.GetAllBaseTypes(context).ToArray());
		}
		
		[Test]
		public void TwoClassesDerivingFromEachOther()
		{
			// class C1 : C2 {} class C2 : C1 {}
			DefaultTypeDefinition c1 = new DefaultTypeDefinition(mscorlib, string.Empty, "C1");
			DefaultTypeDefinition c2 = new DefaultTypeDefinition(mscorlib, string.Empty, "C2");
			c1.BaseTypes.Add(c2);
			c2.BaseTypes.Add(c1);
			Assert.AreEqual(new [] { c1, c2 }, c1.GetAllBaseTypes(context).ToArray());
		}
		
		[Test]
		public void ClassDerivingFromParameterizedVersionOfItself()
		{
			// class C<X> : C<C<X>> {}
			DefaultTypeDefinition c = new DefaultTypeDefinition(mscorlib, string.Empty, "C");
			c.TypeParameters.Add(new DefaultTypeParameter(c, 0, "X"));
			c.BaseTypes.Add(new ParameterizedType(c, new [] { new ParameterizedType(c, new [] { c.TypeParameters[0] }) }));
			Assert.AreEqual(new [] { c }, c.GetAllBaseTypes(context).ToArray());
		}
		
		
		[Test]
		public void ClassDerivingFromTwoInstanciationsOfIEnumerable()
		{
			// class C : IEnumerable<int>, IEnumerable<uint> {}
			DefaultTypeDefinition c = new DefaultTypeDefinition(mscorlib, string.Empty, "C");
			c.BaseTypes.Add(typeof(IEnumerable<int>).ToTypeReference());
			c.BaseTypes.Add(typeof(IEnumerable<uint>).ToTypeReference());
			Assert.AreEqual(new [] {
			                	c,
			                	c.BaseTypes[0].Resolve(context),
			                	c.BaseTypes[1].Resolve(context),
			                	mscorlib.GetClass(typeof(IEnumerable)),
			                	mscorlib.GetClass(typeof(object))
			                },
			                c.GetAllBaseTypes(context).OrderBy(t => t.DotNetName).ToArray());
		}
	}
}
