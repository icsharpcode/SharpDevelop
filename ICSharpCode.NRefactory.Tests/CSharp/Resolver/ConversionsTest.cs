// Copyright (c) 2010 AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under MIT X11 license (for details please see \doc\license.txt)

using System;
using System.Collections;
using System.Collections.Generic;
using ICSharpCode.NRefactory.TypeSystem;
using NUnit.Framework;

namespace ICSharpCode.NRefactory.CSharp.Resolver
{
	[TestFixture]
	public class ConversionsTest
	{
		IProjectContent mscorlib = CecilLoaderTests.Mscorlib;
		Conversions conversions = new Conversions(CecilLoaderTests.Mscorlib);
		
		bool ImplicitConversion(Type from, Type to)
		{
			IType from2 = from.ToTypeReference().Resolve(mscorlib);
			IType to2 = to.ToTypeReference().Resolve(mscorlib);
			return conversions.ImplicitConversion(from2, to2);
		}
		
		bool ImplicitConversion(Type from, IType to)
		{
			IType from2 = from.ToTypeReference().Resolve(mscorlib);
			return conversions.ImplicitConversion(from2, to);
		}
		
		bool ImplicitConversion(IType from, Type to)
		{
			IType to2 = to.ToTypeReference().Resolve(mscorlib);
			return conversions.ImplicitConversion(from, to2);
		}
		
		[Test]
		public void IdentityConversions()
		{
			Assert.IsTrue(ImplicitConversion(typeof(char), typeof(char)));
			Assert.IsTrue(ImplicitConversion(typeof(string), typeof(string)));
			Assert.IsTrue(conversions.ImplicitConversion(SharedTypes.Dynamic, SharedTypes.Dynamic));
			Assert.IsTrue(conversions.ImplicitConversion(SharedTypes.UnknownType, SharedTypes.UnknownType));
			Assert.IsTrue(conversions.ImplicitConversion(SharedTypes.Null, SharedTypes.Null));
		}
		
		[Test]
		public void PrimitiveConversions()
		{
			Assert.IsTrue(ImplicitConversion(typeof(char), typeof(ushort)));
			Assert.IsFalse(ImplicitConversion(typeof(byte), typeof(char)));
			Assert.IsTrue(ImplicitConversion(typeof(int), typeof(long)));
			Assert.IsFalse(ImplicitConversion(typeof(long), typeof(int)));
			Assert.IsTrue(ImplicitConversion(typeof(int), typeof(float)));
			Assert.IsFalse(ImplicitConversion(typeof(bool), typeof(float)));
			Assert.IsTrue(ImplicitConversion(typeof(float), typeof(double)));
			Assert.IsFalse(ImplicitConversion(typeof(float), typeof(decimal)));
		}
		
		[Test]
		public void NullableConversions()
		{
			Assert.IsTrue(ImplicitConversion(typeof(char), typeof(ushort?)));
			Assert.IsFalse(ImplicitConversion(typeof(byte), typeof(char?)));
			Assert.IsTrue(ImplicitConversion(typeof(int), typeof(long?)));
			Assert.IsFalse(ImplicitConversion(typeof(long), typeof(int?)));
			Assert.IsTrue(ImplicitConversion(typeof(int), typeof(float?)));
			Assert.IsFalse(ImplicitConversion(typeof(bool), typeof(float?)));
			Assert.IsTrue(ImplicitConversion(typeof(float), typeof(double?)));
			Assert.IsFalse(ImplicitConversion(typeof(float), typeof(decimal?)));
		}
		
		[Test]
		public void NullableConversions2()
		{
			Assert.IsTrue(ImplicitConversion(typeof(char?), typeof(ushort?)));
			Assert.IsFalse(ImplicitConversion(typeof(byte?), typeof(char?)));
			Assert.IsTrue(ImplicitConversion(typeof(int?), typeof(long?)));
			Assert.IsFalse(ImplicitConversion(typeof(long?), typeof(int?)));
			Assert.IsTrue(ImplicitConversion(typeof(int?), typeof(float?)));
			Assert.IsFalse(ImplicitConversion(typeof(bool?), typeof(float?)));
			Assert.IsTrue(ImplicitConversion(typeof(float?), typeof(double?)));
			Assert.IsFalse(ImplicitConversion(typeof(float?), typeof(decimal?)));
		}
		
		[Test]
		public void NullLiteralConversions()
		{
			Assert.IsTrue(ImplicitConversion(SharedTypes.Null, typeof(int?)));
			Assert.IsTrue(ImplicitConversion(SharedTypes.Null, typeof(char?)));
			Assert.IsFalse(ImplicitConversion(SharedTypes.Null, typeof(int)));
			Assert.IsTrue(ImplicitConversion(SharedTypes.Null, typeof(object)));
			Assert.IsTrue(conversions.ImplicitConversion(SharedTypes.Null, SharedTypes.Dynamic));
			Assert.IsTrue(ImplicitConversion(SharedTypes.Null, typeof(string)));
			Assert.IsTrue(ImplicitConversion(SharedTypes.Null, typeof(int[])));
		}
		
		[Test]
		public void SimpleReferenceConversions()
		{
			Assert.IsTrue(ImplicitConversion(typeof(string), typeof(object)));
			Assert.IsTrue(ImplicitConversion(typeof(BitArray), typeof(ICollection)));
			Assert.IsTrue(ImplicitConversion(typeof(IList), typeof(IEnumerable)));
			Assert.IsFalse(ImplicitConversion(typeof(object), typeof(string)));
			Assert.IsFalse(ImplicitConversion(typeof(ICollection), typeof(BitArray)));
			Assert.IsFalse(ImplicitConversion(typeof(IEnumerable), typeof(IList)));
		}
		
		[Test]
		public void SimpleDynamicConversions()
		{
			Assert.IsTrue(ImplicitConversion(typeof(string), SharedTypes.Dynamic));
			Assert.IsTrue(ImplicitConversion(SharedTypes.Dynamic, typeof(string)));
			Assert.IsTrue(ImplicitConversion(typeof(int), SharedTypes.Dynamic));
			Assert.IsTrue(ImplicitConversion(SharedTypes.Dynamic, typeof(int)));
		}
		
		[Test]
		public void ArrayConversions()
		{
			Assert.IsTrue(ImplicitConversion(typeof(string[]), typeof(object[])));
			Assert.IsTrue(ImplicitConversion(typeof(string[,]), typeof(object[,])));
			Assert.IsFalse(ImplicitConversion(typeof(string[]), typeof(object[,])));
			Assert.IsFalse(ImplicitConversion(typeof(object[]), typeof(string[])));
			
			Assert.IsTrue(ImplicitConversion(typeof(string[]), typeof(IList<string>)));
			Assert.IsFalse(ImplicitConversion(typeof(string[,]), typeof(IList<string>)));
			Assert.IsTrue(ImplicitConversion(typeof(string[]), typeof(IList<object>)));
			
			Assert.IsTrue(ImplicitConversion(typeof(string[]), typeof(Array)));
			Assert.IsTrue(ImplicitConversion(typeof(string[]), typeof(ICloneable)));
			Assert.IsFalse(ImplicitConversion(typeof(Array), typeof(string[])));
			Assert.IsFalse(ImplicitConversion(typeof(object), typeof(object[])));
		}
		
		[Test]
		public void VarianceConversions()
		{
			Assert.IsTrue(ImplicitConversion(typeof(List<string>), typeof(IEnumerable<object>)));
			Assert.IsFalse(ImplicitConversion(typeof(List<object>), typeof(IEnumerable<string>)));
			Assert.IsTrue(ImplicitConversion(typeof(IEnumerable<string>), typeof(IEnumerable<object>)));
			Assert.IsFalse(ImplicitConversion(typeof(ICollection<string>), typeof(ICollection<object>)));
			
			Assert.IsTrue(ImplicitConversion(typeof(Comparer<object>), typeof(IComparer<string>)));
			Assert.IsTrue(ImplicitConversion(typeof(Comparer<object>), typeof(IComparer<Array>)));
			Assert.IsFalse(ImplicitConversion(typeof(Comparer<object>), typeof(Comparer<string>)));
			
			Assert.IsFalse(ImplicitConversion(typeof(List<object>), typeof(IEnumerable<string>)));
			Assert.IsTrue(ImplicitConversion(typeof(IEnumerable<string>), typeof(IEnumerable<object>)));
			
			Assert.IsTrue(ImplicitConversion(typeof(Func<ICollection, ICollection>), typeof(Func<IList, IEnumerable>)));
			Assert.IsTrue(ImplicitConversion(typeof(Func<IEnumerable, IList>), typeof(Func<ICollection, ICollection>)));
			Assert.IsFalse(ImplicitConversion(typeof(Func<ICollection, ICollection>), typeof(Func<IEnumerable, IList>)));
			Assert.IsFalse(ImplicitConversion(typeof(Func<IList, IEnumerable>), typeof(Func<ICollection, ICollection>)));
		}
	}
}
