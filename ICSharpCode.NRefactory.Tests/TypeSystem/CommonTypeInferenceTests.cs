// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under MIT X11 license (for details please see \doc\license.txt)

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ICSharpCode.NRefactory.CSharp.Resolver;
using NUnit.Framework;

namespace ICSharpCode.NRefactory.TypeSystem
{
	[TestFixture]
	public class CommonTypeInferenceTests
	{
		CommonTypeInference cti = new CommonTypeInference(CecilLoaderTests.Mscorlib, new Conversions(CecilLoaderTests.Mscorlib));
		
		IType[] Resolve(params Type[] types)
		{
			IType[] r = new IType[types.Length];
			for (int i = 0; i < types.Length; i++) {
				r[i] = types[i].ToTypeReference().Resolve(CecilLoaderTests.Mscorlib);
				Assert.AreNotSame(r[i], SharedTypes.UnknownType);
			}
			Array.Sort(r, (a,b)=>a.ReflectionName.CompareTo(b.ReflectionName));
			return r;
		}
		
		IType[] CommonBaseTypes(params Type[] types)
		{
			return cti.CommonBaseTypes(Resolve(types)).OrderBy(r => r.ReflectionName).ToArray();
		}
		
		[Test]
		public void ListOfStringAndObject()
		{
			Assert.AreEqual(
				Resolve(typeof(IList), typeof(IEnumerable<object>)),
				CommonBaseTypes(typeof(List<string>), typeof(List<object>)));
		}
		
		[Test]
		public void ListOfListOfStringAndObject()
		{
			Assert.AreEqual(
				Resolve(typeof(IList), typeof(IEnumerable<IList>), typeof(IEnumerable<IEnumerable<object>>)),
				CommonBaseTypes(typeof(List<List<string>>), typeof(List<List<object>>)));
		}
		
		[Test]
		public void ShortAndInt()
		{
			Assert.AreEqual(
				Resolve(typeof(int)),
				CommonBaseTypes(typeof(short), typeof(int)));
		}
		
		[Test]
		public void ListOfShortAndInt()
		{
			Assert.AreEqual(
				Resolve(typeof(IList)),
				CommonBaseTypes(typeof(List<short>), typeof(List<int>)));
		}
		
		[Test]
		public void StringAndVersion()
		{
			Assert.AreEqual(
				Resolve(typeof(ICloneable), typeof(IComparable)),
				CommonBaseTypes(typeof(string), typeof(Version)));
		}
	}
}
