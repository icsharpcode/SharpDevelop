// Copyright (c) 2010 AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under MIT X11 license (for details please see \doc\license.txt)

using System;
using ICSharpCode.NRefactory.TypeSystem;
using NUnit.Framework;

namespace ICSharpCode.NRefactory.CSharp.Resolver
{
	/// <summary>
	/// Base class with helper functions for resolver unit tests.
	/// </summary>
	public abstract class ResolverTestBase
	{
		protected readonly IProjectContent mscorlib = CecilLoaderTests.Mscorlib;
		protected readonly ITypeResolveContext context = CecilLoaderTests.Mscorlib;
		protected CSharpResolver resolver;
		
		[SetUp]
		public virtual void SetUp()
		{
			resolver = new CSharpResolver(CecilLoaderTests.Mscorlib);
			resolver.UsingScope = MakeUsingScope("");
		}
		
		protected UsingScope MakeUsingScope(string namespaceName)
		{
			UsingScope u = new UsingScope(mscorlib);
			if (!string.IsNullOrEmpty(namespaceName)) {
				foreach (string element in namespaceName.Split('.')) {
					u = new UsingScope(u, string.IsNullOrEmpty(u.NamespaceName) ? element : u.NamespaceName + "." + element);
				}
			}
			return u;
		}
		
		/// <summary>
		/// Adds a using to the current top-level using scope.
		/// </summary>
		protected void AddUsing(string namespaceName)
		{
			string[] nameParts = namespaceName.Split('.');
			ITypeOrNamespaceReference r = new SimpleTypeOrNamespaceReference(nameParts[0], new ITypeReference[0], resolver.CurrentTypeDefinition, resolver.UsingScope, true);
			for (int i = 1; i < nameParts.Length; i++) {
				throw new NotImplementedException();
			}
			resolver.UsingScope.Usings.Add(r);
		}
		
		protected IType ResolveType(Type type)
		{
			IType t = type.ToTypeReference().Resolve(context);
			if (t == SharedTypes.UnknownType)
				throw new InvalidOperationException("Could not resolve type");
			return t;
		}
		
		protected ConstantResolveResult MakeConstant(object value)
		{
			if (value == null)
				return new ConstantResolveResult(SharedTypes.Null, null);
			IType type = ResolveType(value.GetType());
			if (type.IsEnum())
				value = Convert.ChangeType(value, Enum.GetUnderlyingType(value.GetType()));
			return new ConstantResolveResult(type, value);
		}
		
		protected ResolveResult MakeResult(Type type)
		{
			return new ResolveResult(ResolveType(type));
		}
		
		protected void AssertConstant(object expectedValue, ResolveResult rr)
		{
			Assert.IsFalse(rr.IsError, rr.ToString() + " is an error");
			Assert.IsTrue(rr.IsCompileTimeConstant, rr.ToString() + " is not a compile-time constant");
			Type expectedType = expectedValue.GetType();
			Assert.AreEqual(ResolveType(expectedType), rr.Type, "ResolveResult.Type is wrong");
			if (expectedType.IsEnum) {
				Assert.AreEqual(Enum.GetUnderlyingType(expectedType), rr.ConstantValue.GetType(), "ResolveResult.ConstantValue has wrong Type");
				Assert.AreEqual(Convert.ChangeType(expectedValue, Enum.GetUnderlyingType(expectedType)), rr.ConstantValue);
			} else {
				Assert.AreEqual(expectedType, rr.ConstantValue.GetType(), "ResolveResult.ConstantValue has wrong Type");
				Assert.AreEqual(expectedValue, rr.ConstantValue);
			}
		}
		
		protected void AssertType(Type expectedType, ResolveResult rr)
		{
			Assert.IsFalse(rr.IsError, rr.ToString() + " is an error");
			Assert.IsFalse(rr.IsCompileTimeConstant, rr.ToString() + " is a compile-time constant");
			Assert.AreEqual(expectedType.ToTypeReference().Resolve(context), rr.Type);
		}
		
		protected void AssertError(Type expectedType, ResolveResult rr)
		{
			Assert.IsTrue(rr.IsError, rr.ToString() + " is not an error, but an error was expected");
			Assert.IsFalse(rr.IsCompileTimeConstant, rr.ToString() + " is a compile-time constant");
			Assert.AreEqual(expectedType.ToTypeReference().Resolve(context), rr.Type);
		}
	}
}
