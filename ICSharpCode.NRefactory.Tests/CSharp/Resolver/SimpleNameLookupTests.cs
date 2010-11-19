// Copyright (c) 2010 AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under MIT X11 license (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using ICSharpCode.NRefactory.TypeSystem;

namespace ICSharpCode.NRefactory.CSharp.Resolver
{
	[TestFixture]
	public class SimpleNameLookupTests : ResolverTestBase
	{
		[Test]
		public void SimpleNameLookupWithoutContext()
		{
			// nothing should be found without specifying any UsingScope - however, the resolver also must not crash
			resolver.UsingScope = null;
			Assert.IsTrue(resolver.ResolveSimpleName("System", new IType[0]).IsError);
		}
		
		[Test]
		public void SimpleNamespaceLookup()
		{
			NamespaceResolveResult nrr = (NamespaceResolveResult)resolver.ResolveSimpleName("System", new IType[0]);
			Assert.AreEqual("System", nrr.NamespaceName);
			Assert.AreSame(SharedTypes.UnknownType, nrr.Type);
		}
		
		[Test]
		public void NamespaceInParentNamespaceLookup()
		{
			resolver.UsingScope = MakeUsingScope("System.Collections.Generic");
			NamespaceResolveResult nrr = (NamespaceResolveResult)resolver.ResolveSimpleName("Text", new IType[0]);
			Assert.AreEqual("System.Text", nrr.NamespaceName);
		}
		
		[Test]
		public void NamespacesAreNotImported()
		{
			AddUsing("System");
			Assert.IsTrue(resolver.ResolveSimpleName("Collections", new IType[0]).IsError);
		}
		
		[Test]
		public void ImportedType()
		{
			AddUsing("System");
			TypeResolveResult trr = (TypeResolveResult)resolver.ResolveSimpleName("String", new IType[0]);
			Assert.AreEqual("System.String", trr.Type.FullName);
		}
		
		[Test]
		public void GlobalIsUnknownIdentifier()
		{
			Assert.IsTrue(resolver.ResolveSimpleName("global", new IType[0]).IsError);
		}
		
		[Test]
		public void GlobalIsAlias()
		{
			NamespaceResolveResult nrr = (NamespaceResolveResult)resolver.ResolveAlias("global");
			Assert.AreEqual("", nrr.NamespaceName);
		}
		
		[Test]
		public void AliasToImportedType()
		{
			AddUsing("System");
			AddUsingAlias("x", "String");
			TypeResolveResult trr = (TypeResolveResult)resolver.ResolveSimpleName("x", new IType[0]);
			// Unknown type (as String isn't looked up in System)
			Assert.AreSame(SharedTypes.UnknownType, trr.Type);
		}
		
		[Test]
		public void AliasToImportedType2()
		{
			AddUsing("System");
			resolver.UsingScope = new UsingScope(resolver.UsingScope, "SomeNamespace");
			AddUsingAlias("x", "String");
			TypeResolveResult trr = (TypeResolveResult)resolver.ResolveSimpleName("x", new IType[0]);
			Assert.AreEqual("System.String", trr.Type.FullName);
		}
		
		[Test]
		public void AliasOperatorOnTypeAlias()
		{
			AddUsingAlias("x", "System.String");
			Assert.IsTrue(resolver.ResolveAlias("x").IsError);
		}
		
		[Test]
		public void AliasOperatorOnNamespaceAlias()
		{
			AddUsingAlias("x", "System.Collections.Generic");
			NamespaceResolveResult nrr = (NamespaceResolveResult)resolver.ResolveAlias("x");
			Assert.AreEqual("System.Collections.Generic", nrr.NamespaceName);
		}
		
		[Test]
		public void AliasOperatorOnNamespace()
		{
			Assert.IsTrue(resolver.ResolveAlias("System").IsError);
		}
		
		[Test]
		public void FindClassInCurrentNamespace()
		{
			resolver.UsingScope = MakeUsingScope("System.Collections");
			TypeResolveResult trr = (TypeResolveResult)resolver.ResolveSimpleName("String", new IType[0]);
			Assert.AreEqual("System.String", trr.Type.FullName);
		}
		
		[Test]
		public void FindNeighborNamespace()
		{
			resolver.UsingScope = MakeUsingScope("System.Collections");
			NamespaceResolveResult nrr = (NamespaceResolveResult)resolver.ResolveSimpleName("Text", new IType[0]);
			Assert.AreEqual("System.Text", nrr.NamespaceName);
		}
		
		[Test]
		public void FindTypeParameters()
		{
			resolver.UsingScope = MakeUsingScope("System.Collections.Generic");
			resolver.CurrentTypeDefinition = context.GetClass(typeof(List<>));
			resolver.CurrentMember = resolver.CurrentTypeDefinition.Methods.Single(m => m.Name == "ConvertAll");
			
			TypeResolveResult trr;
			trr = (TypeResolveResult)resolver.ResolveSimpleName("TOutput", new IType[0]);
			Assert.AreSame(((IMethod)resolver.CurrentMember).TypeParameters[0], trr.Type);
			
			trr = (TypeResolveResult)resolver.ResolveSimpleName("T", new IType[0]);
			Assert.AreSame(resolver.CurrentTypeDefinition.TypeParameters[0], trr.Type);
		}
	}
}
