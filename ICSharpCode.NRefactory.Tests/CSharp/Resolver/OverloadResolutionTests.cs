// Copyright (c) 2010 AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under MIT X11 license (for details please see \doc\license.txt)

using System;
using System.Linq;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.TypeSystem.Implementation;
using NUnit.Framework;

namespace ICSharpCode.NRefactory.CSharp.Resolver
{
	[TestFixture]
	public class OverloadResolutionTests
	{
		readonly ITypeResolveContext context = CecilLoaderTests.Mscorlib;
		readonly DefaultTypeDefinition dummyClass = new DefaultTypeDefinition(CecilLoaderTests.Mscorlib, string.Empty, "DummyClass");
		
		ResolveResult[] MakeArgumentList(params Type[] argumentTypes)
		{
			return argumentTypes.Select(t => new ResolveResult(t.ToTypeReference().Resolve(context))).ToArray();
		}
		
		DefaultMethod MakeMethod(params Type[] parameterTypes)
		{
			DefaultMethod m = new DefaultMethod(dummyClass, "Method");
			foreach (var type in parameterTypes) {
				m.Parameters.Add(new DefaultParameter(type.ToTypeReference(), string.Empty));
			}
			return m;
		}
		
		DefaultMethod MakeParamsMethod(params Type[] parameterTypes)
		{
			DefaultMethod m = MakeMethod(parameterTypes);
			((DefaultParameter)m.Parameters.Last()).IsParams = true;
			return m;
		}
		
		[Test]
		public void PreferIntOverUInt()
		{
			OverloadResolution r = new OverloadResolution(context, MakeArgumentList(typeof(ushort)));
			var c1 = MakeMethod(typeof(int));
			Assert.AreEqual(OverloadResolutionErrors.None, r.AddCandidate(c1));
			Assert.AreEqual(OverloadResolutionErrors.None, r.AddCandidate(MakeMethod(typeof(uint))));
			Assert.IsFalse(r.IsAmbiguous);
			Assert.AreSame(c1, r.BestCandidate);
		}
		
		[Test]
		public void NullableIntAndNullableUIntIsAmbiguous()
		{
			OverloadResolution r = new OverloadResolution(context, MakeArgumentList(typeof(ushort?)));
			Assert.AreEqual(OverloadResolutionErrors.None, r.AddCandidate(MakeMethod(typeof(int?))));
			Assert.AreEqual(OverloadResolutionErrors.None, r.AddCandidate(MakeMethod(typeof(uint?))));
			Assert.AreEqual(OverloadResolutionErrors.AmbiguousMatch, r.BestCandidateErrors);
			
			// then adding a matching overload solves the ambiguity:
			Assert.AreEqual(OverloadResolutionErrors.None, r.AddCandidate(MakeMethod(typeof(ushort?))));
			Assert.AreEqual(OverloadResolutionErrors.None, r.BestCandidateErrors);
			Assert.IsNull(r.BestCandidateAmbiguousWith);
		}
		
		[Test]
		public void ParamsMethodMatchesEmptyArgumentList()
		{
			OverloadResolution r = new OverloadResolution(context, MakeArgumentList());
			Assert.AreEqual(OverloadResolutionErrors.None, r.AddCandidate(MakeParamsMethod(typeof(int[]))));
			Assert.IsTrue(r.BestCandidateIsExpandedForm);
		}
		
		[Test]
		public void ParamsMethodMatchesOneArgumentInExpandedForm()
		{
			OverloadResolution r = new OverloadResolution(context, MakeArgumentList(typeof(int)));
			Assert.AreEqual(OverloadResolutionErrors.None, r.AddCandidate(MakeParamsMethod(typeof(int[]))));
			Assert.IsTrue(r.BestCandidateIsExpandedForm);
		}
		
		[Test]
		public void ParamsMethodMatchesInUnexpandedForm()
		{
			OverloadResolution r = new OverloadResolution(context, MakeArgumentList(typeof(int[])));
			Assert.AreEqual(OverloadResolutionErrors.None, r.AddCandidate(MakeParamsMethod(typeof(int[]))));
			Assert.IsFalse(r.BestCandidateIsExpandedForm);
		}
		
		[Test]
		public void LessArgumentsPassedToParamsIsBetter()
		{
			OverloadResolution r = new OverloadResolution(context, MakeArgumentList(typeof(int), typeof(int), typeof(int)));
			Assert.AreEqual(OverloadResolutionErrors.None, r.AddCandidate(MakeParamsMethod(typeof(int[]))));
			Assert.AreEqual(OverloadResolutionErrors.None, r.AddCandidate(MakeParamsMethod(typeof(int), typeof(int[]))));
			Assert.IsFalse(r.IsAmbiguous);
			Assert.AreEqual(2, r.BestCandidate.Parameters.Count);
		}
		
		[Test]
		public void CallInvalidParamsDeclaration()
		{
			OverloadResolution r = new OverloadResolution(context, MakeArgumentList(typeof(int[,])));
			Assert.AreEqual(OverloadResolutionErrors.ArgumentTypeMismatch, r.AddCandidate(MakeParamsMethod(typeof(int))));
			Assert.IsFalse(r.BestCandidateIsExpandedForm);
		}
	}
}
