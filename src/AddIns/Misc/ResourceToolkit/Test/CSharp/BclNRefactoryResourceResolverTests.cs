// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;

using Hornung.ResourceToolkit;
using Hornung.ResourceToolkit.Resolver;
using NUnit.Framework;

namespace ResourceToolkit.Tests.CSharp
{
	[TestFixture]
	public sealed class BclNRefactoryResourceResolverTests : AbstractCSharpResourceResolverTestFixture
	{
		readonly IResourceResolver resolver = new NRefactoryResourceResolver();
		
		IResourceResolver Resolver {
			get { return this.resolver; }
		}
		
		// ********************************************************************************************************************************
		
		#region === Tests with local variables ===
		
		const string CodeLocalSRMDirectInitFullName = @"class A
{
	void B()
	{
		System.Resources.ResourceManager mgr = new System.Resources.ResourceManager(""Test.TestResources"", System.Reflection.Assembly.GetExecutingAssembly());
		mgr.GetString(""TestKey"");
		mgr[""TestKey2""];
	}
	
	void C()
	{
		mgr.GetString(""TestKey"");
		mgr[""TestKey2""];
	}
}
";
		
		[Test]
		public void LocalSRMDirectInitFullNameGetString()
		{
			ResourceResolveResult rrr = Resolve(CodeLocalSRMDirectInitFullName, 5, 18, null);
			TestHelper.CheckReference(rrr, "Test.TestResources", "TestKey", "A", "A.B");
		}
		
		[Test]
		public void LocalSRMDirectInitFullNameNoIndexer()
		{
			ResourceResolveResult rrr = Resolve(CodeLocalSRMDirectInitFullName, 6, 7, null);
			TestHelper.CheckNoReference(rrr);
		}
		
		[Test]
		public void LocalSRMDirectInitFullNameOutOfScope()
		{
			ResourceResolveResult rrr = Resolve(CodeLocalSRMDirectInitFullName, 11, 18, null);
			TestHelper.CheckNoReference(rrr);
			
			rrr = Resolve(CodeLocalSRMDirectInitFullName, 12, 7, null);
			TestHelper.CheckNoReference(rrr);
		}
		
		// ********************************************************************************************************************************
		
		const string CodeLocalSRMDirectInitFullNameGetStringCompletion = @"class A
{
	void B()
	{
		System.Resources.ResourceManager mgr = new System.Resources.ResourceManager(""Test.TestResources"", System.Reflection.Assembly.GetExecutingAssembly());
		mgr.GetString
	}
}
";
		
		[Test]
		public void LocalSRMDirectInitFullNameGetStringIncomplete()
		{
			ResourceResolveResult rrr = Resolve(CodeLocalSRMDirectInitFullNameGetStringCompletion, 5, 15, null);
			TestHelper.CheckNoReference(rrr);
		}
		
		[Test]
		public void LocalSRMDirectInitFullNameGetStringCompletion()
		{
			ResourceResolveResult rrr = Resolve(CodeLocalSRMDirectInitFullNameGetStringCompletion, 5, 15, '(');
			TestHelper.CheckReference(rrr, "Test.TestResources", null, "A", "A.B");
		}
		
		[Test]
		public void LocalSRMDirectInitFullNameGetStringNoCompletion()
		{
			ResourceResolveResult rrr = Resolve(CodeLocalSRMDirectInitFullNameGetStringCompletion, 5, 15, '[');
			TestHelper.CheckNoReference(rrr);
		}
		
		const string CodeLocalSRMDirectInitFullNameGetStringCompletionBug1 = @"class A
{
	void B()
	{
		System.Resources.ResourceManager mgr = new System.Resources.ResourceManager(""Test.TestResources"", System.Reflection.Assembly.GetExecutingAssembly());
		mgr.GetString(""TestKey"").Replace
	}
}
";
		
		[Test]
		public void LocalSRMDirectInitFullNameGetStringCompletionBug1()
		{
			ResourceResolveResult rrr = Resolve(CodeLocalSRMDirectInitFullNameGetStringCompletionBug1, 5, 34, '(');
			TestHelper.CheckNoReference(rrr);
		}
		
		// ********************************************************************************************************************************
		
		const string CodeLocalIndexerRMDirectInit = @"class IndexerRM : System.Resources.ResourceManager
{
	public IndexerRM(string name, System.Reflection.Assembly assembly) : base(name, assembly)
	{
	}
	public string this[string key] {
		get { return this.GetString(key); }
	}
}

class A
{
	void B()
	{
		IndexerRM mgr = new IndexerRM(""Test.TestResources"", System.Reflection.Assembly.GetExecutingAssembly());
		mgr[""TestKey""];
		mgr
	}
}
";
		
		[Test]
		public void LocalIndexerRMDirectInit()
		{
			ResourceResolveResult rrr = Resolve(CodeLocalIndexerRMDirectInit, 15, 7, null);
			TestHelper.CheckReference(rrr, "Test.TestResources", "TestKey", "A", "A.B");
		}
		
		[Test]
		public void LocalIndexerRMDirectInitCompletionBug1()
		{
			ResourceResolveResult rrr = Resolve(CodeLocalIndexerRMDirectInit, 15, 16, '[');
			TestHelper.CheckNoReference(rrr);
		}
		
		[Test]
		public void LocalIndexerRMDirectInitCompletion()
		{
			ResourceResolveResult rrr = Resolve(CodeLocalIndexerRMDirectInit, 16, 5, '[');
			TestHelper.CheckReference(rrr, "Test.TestResources", null, "A", "A.B");
		}
		
		[Test]
		public void LocalIndexerRMDirectInitNoCompletion()
		{
			ResourceResolveResult rrr = Resolve(CodeLocalIndexerRMDirectInit, 16, 5, null);
			TestHelper.CheckNoReference(rrr);
		}
		
		[Test]
		public void LocalIndexerRMDirectInitNoCompletionWrongChar()
		{
			ResourceResolveResult rrr = Resolve(CodeLocalIndexerRMDirectInit, 16, 5, '(');
			TestHelper.CheckNoReference(rrr);
		}
		
		// ********************************************************************************************************************************
		
		const string CodeLocalIndexerRMBug1 = @"class IndexerRM : System.Resources.ResourceManager
{
	public IndexerRM(string name, System.Reflection.Assembly assembly) : base(name, assembly)
	{
	}
	public string this[string key] {
		get { return this.GetString(key); }
	}
}

class A
{
	void B()
	{
		IndexerRM mgr = new IndexerRM(""Test.TestResources"", System.Reflection.Assembly.GetExecutingAssembly());
		DoSomething(mgr[""TestKey""], ""["");
	}
}
";
		
		[Test]
		public void LocalIndexerRMBug1FirstRef()
		{
			ResourceResolveResult rrr = Resolve(CodeLocalIndexerRMBug1, 15, 19, null);
			TestHelper.CheckReference(rrr, "Test.TestResources", "TestKey", "A", "A.B");
		}
		
		[Test]
		public void LocalIndexerRMBug1SecondRef()
		{
			ResourceResolveResult rrr = Resolve(CodeLocalIndexerRMBug1, 15, 31, null);
			TestHelper.CheckNoReference(rrr);
		}
		
		// ********************************************************************************************************************************
		
		const string CodeLocalSRMDirectInitAlias = @"using SRM = System.Resources.ResourceManager;
class A
{
	void B()
	{
		SRM mgr = new SRM(""Test.TestResources"", System.Reflection.Assembly.GetExecutingAssembly());
		mgr.GetString(""TestKey"");
		mgr[""TestKey2""];
	}
}
";
		
		[Test]
		public void LocalSRMDirectInitAliasGetString()
		{
			ResourceResolveResult rrr = Resolve(CodeLocalSRMDirectInitAlias, 6, 18, null);
			TestHelper.CheckReference(rrr, "Test.TestResources", "TestKey", "A", "A.B");
		}
		
		[Test]
		public void LocalSRMDirectInitAliasNoIndexer()
		{
			ResourceResolveResult rrr = Resolve(CodeLocalSRMDirectInitAlias, 7, 7, null);
			TestHelper.CheckNoReference(rrr);
		}
		
		// ********************************************************************************************************************************
		
		const string CodeLocalSRMDeferredInitUsing = @"using System.Resources;
class A
{
	void B()
	{
		ResourceManager mgr;
		
		mgr = new ResourceManager(""Test.TestResources"", System.Reflection.Assembly.GetExecutingAssembly());
		mgr.GetString(""TestKey"");
	}
}
";
		
		[Test]
		public void LocalSRMDeferredInitUsingGetString()
		{
			ResourceResolveResult rrr = Resolve(CodeLocalSRMDeferredInitUsing, 8, 18, null);
			TestHelper.CheckReference(rrr, "Test.TestResources", "TestKey", "A", "A.B");
		}
		
		// ********************************************************************************************************************************
		
		const string CodeLocalCRMDeferredInitUsingApplyResources = @"using System.ComponentModel;
class A
{
	void B()
	{
		ComponentResourceManager mgr;
		
		mgr = new ComponentResourceManager(typeof(A));
		mgr.ApplyResources(this, ""$this"");
	}
}
";
		
		[Test]
		public void LocalCRMDeferredInitUsingApplyResources()
		{
			ResourceResolveResult rrr = Resolve(CodeLocalCRMDeferredInitUsingApplyResources, 8, 20, null);
			TestHelper.CheckReference(rrr, "A", null, "A", "A.B");
			Assert.That(rrr, Is.InstanceOf(typeof(ResourcePrefixResolveResult)));
			ResourcePrefixResolveResult rprr = (ResourcePrefixResolveResult)rrr;
			Assert.That(rprr.Prefix, Is.EqualTo("$this"), "Resource key prefix not detected correctly.");
		}
		#endregion
		
		// ********************************************************************************************************************************
		
		#region === Tests with instance fields ===
		
		const string CodeInstanceFieldSRMDirectInitUsing = @"using System.Resources;
class A
{
	ResourceManager mgr = new ResourceManager(""Test.TestResources"", System.Reflection.Assembly.GetExecutingAssembly());
	
	void B()
	{
		this.mgr.GetString(""TestKey"");
		mgr.GetString(""TestKey"");
	}
}
";
		
		[Test]
		public void InstanceFieldSRMDirectInitUsingThisGetString()
		{
			ResourceResolveResult rrr = Resolve(CodeInstanceFieldSRMDirectInitUsing, 7, 22, null);
			TestHelper.CheckReference(rrr, "Test.TestResources", "TestKey", "A", "A.B");
		}
		
		[Test]
		public void InstanceFieldSRMDirectInitUsingGetString()
		{
			ResourceResolveResult rrr = Resolve(CodeInstanceFieldSRMDirectInitUsing, 8, 17, null);
			TestHelper.CheckReference(rrr, "Test.TestResources", "TestKey", "A", "A.B");
		}
		
		// ********************************************************************************************************************************
		
		const string CodeInstanceFieldSRMDeferredInitThisUsing = @"using System.Resources;
class A
{
	ResourceManager mgr;
	public A()
	{
		this.mgr = new ResourceManager(""Test.TestResources"", System.Reflection.Assembly.GetExecutingAssembly());
	}
	void B()
	{
		this.mgr.GetString(""TestKey"");
	}
}
";
		
		[Test]
		public void InstanceFieldSRMDeferredInitThisUsingThisGetString()
		{
			ResourceResolveResult rrr = Resolve(CodeInstanceFieldSRMDeferredInitThisUsing, 10, 22, null);
			TestHelper.CheckReference(rrr, "Test.TestResources", "TestKey", "A", "A.B");
		}
		
		const string CodeInstanceFieldSRMDeferredInitUsing = @"using System.Resources;
class A
{
	ResourceManager mgr;
	public A()
	{
		mgr = new ResourceManager(""Test.TestResources"", System.Reflection.Assembly.GetExecutingAssembly());
	}
	void B()
	{
		this.mgr.GetString(""TestKey"");
	}
}
";
		
		[Test]
		public void InstanceFieldSRMDeferredInitUsingThisGetString()
		{
			ResourceResolveResult rrr = Resolve(CodeInstanceFieldSRMDeferredInitUsing, 10, 22, null);
			TestHelper.CheckReference(rrr, "Test.TestResources", "TestKey", "A", "A.B");
		}
		
		#endregion
		
		// ********************************************************************************************************************************
		
		#region === Tests with static fields ===
		
		const string CodeStaticFieldSRMDirectInitUsing = @"using System.Resources;
class A
{
	static ResourceManager mgr = new ResourceManager(""Test.TestResources"", System.Reflection.Assembly.GetExecutingAssembly());
	
	void B()
	{
		A.mgr.GetString(""TestKey"");
		mgr.GetString(""TestKey"");
	}
}
";
		
		[Test]
		public void StaticFieldSRMDirectInitUsingClassGetString()
		{
			ResourceResolveResult rrr = Resolve(CodeStaticFieldSRMDirectInitUsing, 7, 19, null);
			TestHelper.CheckReference(rrr, "Test.TestResources", "TestKey", "A", "A.B");
		}
		
		[Test]
		public void StaticFieldSRMDirectInitUsingGetString()
		{
			ResourceResolveResult rrr = Resolve(CodeInstanceFieldSRMDirectInitUsing, 8, 17, null);
			TestHelper.CheckReference(rrr, "Test.TestResources", "TestKey", "A", "A.B");
		}
		
		// ********************************************************************************************************************************
		
		const string CodeStaticFieldSRMDeferredInitClassUsing = @"using System.Resources;
class A
{
	static ResourceManager mgr;
	static A()
	{
		A.mgr = new ResourceManager(""Test.TestResources"", System.Reflection.Assembly.GetExecutingAssembly());
	}
	void B()
	{
		A.mgr.GetString(""TestKey"");
	}
}
";
		
		[Test]
		public void StaticFieldSRMDeferredInitClassUsingClassGetString()
		{
			ResourceResolveResult rrr = Resolve(CodeStaticFieldSRMDeferredInitClassUsing, 10, 19, null);
			TestHelper.CheckReference(rrr, "Test.TestResources", "TestKey", "A", "A.B");
		}
		
		const string CodeStaticFieldSRMDeferredInitUsing = @"using System.Resources;
class A
{
	static ResourceManager mgr;
	static A()
	{
		mgr = new ResourceManager(""Test.TestResources"", System.Reflection.Assembly.GetExecutingAssembly());
	}
	void B()
	{
		A.mgr.GetString(""TestKey"");
	}
}
";
		
		[Test]
		public void StaticFieldSRMDeferredInitUsingClassGetString()
		{
			ResourceResolveResult rrr = Resolve(CodeStaticFieldSRMDeferredInitUsing, 10, 19, null);
			TestHelper.CheckReference(rrr, "Test.TestResources", "TestKey", "A", "A.B");
		}
		
		#endregion
		
		// ********************************************************************************************************************************
		
		#region === Tests with instance properties ===
		
		const string CodeInstancePropertySRMFieldDirectInitUsing = @"using System.Resources;
class A
{
	public ResourceManager Resources {
		get { return mgr; }
	}
	
	ResourceManager mgr = new ResourceManager(""Test.TestResources"", System.Reflection.Assembly.GetExecutingAssembly());
	
	void B()
	{
		this.Resources.GetString(""TestKey"");
		Resources.GetString(""TestKey"");
	}
}
";
		
		[Test]
		public void InstancePropertySRMFieldDirectInitUsingThisGetString()
		{
			ResourceResolveResult rrr = Resolve(CodeInstancePropertySRMFieldDirectInitUsing, 11, 28, null);
			TestHelper.CheckReference(rrr, "Test.TestResources", "TestKey", "A", "A.B");
		}
		
		[Test]
		public void InstancePropertySRMFieldDirectInitUsingGetString()
		{
			ResourceResolveResult rrr = Resolve(CodeInstancePropertySRMFieldDirectInitUsing, 12, 23, null);
			TestHelper.CheckReference(rrr, "Test.TestResources", "TestKey", "A", "A.B");
		}
		
		// ********************************************************************************************************************************
		
		const string CodeInstancePropertySRMDeferredFieldInitThisUsing = @"using System.Resources;
class A
{
	ResourceManager mgr;
	
	public ResourceManager Resources {
		get { return mgr; }
	}
	
	public A()
	{
		this.mgr = new ResourceManager(""Test.TestResources"", System.Reflection.Assembly.GetExecutingAssembly());
	}
	void B()
	{
		this.Resources.GetString(""TestKey"");
	}
}
";
		
		[Test]
		public void InstancePropertySRMDeferredFieldInitThisUsingThisGetString()
		{
			ResourceResolveResult rrr = Resolve(CodeInstancePropertySRMDeferredFieldInitThisUsing, 15, 28, null);
			TestHelper.CheckReference(rrr, "Test.TestResources", "TestKey", "A", "A.B");
		}
		
		// ********************************************************************************************************************************
		
		const string CodeInstancePropertySRMDeferredPropertyInitThisUsing = @"using System.Resources;
class A
{
	ResourceManager mgr;
	
	public ResourceManager Resources {
		get { return mgr; }
		private set { mgr = value; }
	}
	
	public A()
	{
		this.Resources = new ResourceManager(""Test.TestResources"", System.Reflection.Assembly.GetExecutingAssembly());
	}
	void B()
	{
		this.Resources.GetString(""TestKey"");
	}
}
";
		
		[Test]
		public void InstancePropertySRMDeferredPropertyInitThisUsingThisGetString()
		{
			ResourceResolveResult rrr = Resolve(CodeInstancePropertySRMDeferredPropertyInitThisUsing, 16, 28, null);
			TestHelper.CheckReference(rrr, "Test.TestResources", "TestKey", "A", "A.B");
		}
		
		const string CodeInstanceFieldSRMDeferredWriteOnlyPropertyInitThisUsing = @"using System.Resources;
class A
{
	ResourceManager mgr;
	
	private ResourceManager Resources {
		set { mgr = value; }
	}
	
	public A()
	{
		this.Resources = new ResourceManager(""Test.TestResources"", System.Reflection.Assembly.GetExecutingAssembly());
	}
	void B()
	{
		this.mgr.GetString(""TestKey"");
	}
}
";
		
		[Test]
		public void InstanceFieldSRMDeferredWriteOnlyPropertyInitThisUsingThisGetString()
		{
			ResourceResolveResult rrr = Resolve(CodeInstanceFieldSRMDeferredWriteOnlyPropertyInitThisUsing, 15, 22, null);
			TestHelper.CheckReference(rrr, "Test.TestResources", "TestKey", "A", "A.B");
		}
		
		// ********************************************************************************************************************************
		
		const string CodeInstancePropertySRMInvalidReturnTypeBug = @"using System.Resources;
class A
{
	ResourceManager mgr;
	
	public ResourceManager Resources {
		get { return mgr; }
		private set { mgr = value; }
	}
	
	public A()
	{
		foo bar;
		bar = 1;
		this.Resources = new ResourceManager(""Test.TestResources"", System.Reflection.Assembly.GetExecutingAssembly());
		this.Resources.GetString
	}
}
";
		
		[Test]
		public void InstancePropertySRMInvalidReturnTypeBug()
		{
			ResourceResolveResult rrr = Resolve(CodeInstancePropertySRMInvalidReturnTypeBug, 15, 26, '(');
			TestHelper.CheckReference(rrr, "Test.TestResources", null, "A", "A.#ctor");
		}
		
		#endregion
		
		// ********************************************************************************************************************************
		
		#region === Tests with static properties ===
		
		const string CodeStaticPropertySRMFieldDirectInitUsing = @"using System.Resources;
class A
{
	public static ResourceManager Resources {
		get { return mgr; }
	}
	
	static ResourceManager mgr = new ResourceManager(""Test.TestResources"", System.Reflection.Assembly.GetExecutingAssembly());
	
	void B()
	{
		A.Resources.GetString(""TestKey"");
		Resources.GetString(""TestKey"");
	}
}
";
		
		[Test]
		public void StaticPropertySRMFieldDirectInitUsingClassGetString()
		{
			ResourceResolveResult rrr = Resolve(CodeStaticPropertySRMFieldDirectInitUsing, 11, 25, null);
			TestHelper.CheckReference(rrr, "Test.TestResources", "TestKey", "A", "A.B");
		}
		
		[Test]
		public void StaticPropertySRMFieldDirectInitUsingGetString()
		{
			ResourceResolveResult rrr = Resolve(CodeStaticPropertySRMFieldDirectInitUsing, 12, 23, null);
			TestHelper.CheckReference(rrr, "Test.TestResources", "TestKey", "A", "A.B");
		}
		
		// ********************************************************************************************************************************
		
		const string CodeStaticPropertySRMDeferredFieldInitClassUsing = @"using System.Resources;
class A
{
	static ResourceManager mgr;
	
	public static ResourceManager Resources {
		get { return mgr; }
	}
	
	static A()
	{
		A.mgr = new ResourceManager(""Test.TestResources"", System.Reflection.Assembly.GetExecutingAssembly());
	}
	void B()
	{
		A.Resources.GetString(""TestKey"");
	}
}
";
		
		[Test]
		public void StaticPropertySRMDeferredFieldInitClassUsingClassGetString()
		{
			ResourceResolveResult rrr = Resolve(CodeStaticPropertySRMDeferredFieldInitClassUsing, 15, 25, null);
			TestHelper.CheckReference(rrr, "Test.TestResources", "TestKey", "A", "A.B");
		}
		
		// ********************************************************************************************************************************
		
		const string CodeStaticPropertySRMDeferredPropertyInitClassUsing = @"using System.Resources;
class A
{
	static ResourceManager mgr;
	
	public static ResourceManager Resources {
		get { return mgr; }
		private set { mgr = value; }
	}
	
	static A()
	{
		A.Resources = new ResourceManager(""Test.TestResources"", System.Reflection.Assembly.GetExecutingAssembly());
	}
	void B()
	{
		A.Resources.GetString(""TestKey"");
	}
}
";
		
		[Test]
		public void StaticPropertySRMDeferredPropertyInitClassUsingClassGetString()
		{
			ResourceResolveResult rrr = Resolve(CodeStaticPropertySRMDeferredPropertyInitClassUsing, 16, 25, null);
			TestHelper.CheckReference(rrr, "Test.TestResources", "TestKey", "A", "A.B");
		}
		
		const string CodeStaticFieldSRMDeferredWriteOnlyPropertyInitClassUsing = @"using System.Resources;
class A
{
	static ResourceManager mgr;
	
	private static ResourceManager Resources {
		set { mgr = value; }
	}
	
	static A()
	{
		A.Resources = new ResourceManager(""Test.TestResources"", System.Reflection.Assembly.GetExecutingAssembly());
	}
	void B()
	{
		A.mgr.GetString(""TestKey"");
	}
}
";
		
		[Test]
		public void StaticFieldSRMDeferredWriteOnlyPropertyInitClassUsingClassGetString()
		{
			ResourceResolveResult rrr = Resolve(CodeStaticFieldSRMDeferredWriteOnlyPropertyInitClassUsing, 15, 19, null);
			TestHelper.CheckReference(rrr, "Test.TestResources", "TestKey", "A", "A.B");
		}
		
		#endregion
		
		// ********************************************************************************************************************************
		// ********************************************************************************************************************************
		
		#region === Tests for specific behavior ===
		
		const string CodeUnescapeResourceKey = @"using System.Resources;
class A {
	void B()
	{
		ResourceManager mgr = new ResourceManager(""Test.TestResources"", System.Reflection.Assembly.GetExecutingAssembly());
		mgr.GetString(""Some\""strange\""key"");
	}
}
";
		
		[Test]
		public void UnescapeResourceKey()
		{
			ResourceResolveResult rrr = Resolve(CodeUnescapeResourceKey, 5, 17, null);
			TestHelper.CheckReference(rrr, "Test.TestResources", "Some\"strange\"key", "A", "A.B");
		}
		
		// ********************************************************************************************************************************
		
		const string CodeSRMTypeOfInit = @"using System.Resources;
namespace Test {
	class A {
		void B()
		{
			ResourceManager mgr = new ResourceManager(typeof(A));
			mgr.GetString(""TestKey"");
		}
	}
}
";
		
		[Test]
		public void SRMTypeOfInit()
		{
			ResourceResolveResult rrr = Resolve(CodeSRMTypeOfInit, 6, 18, null);
			TestHelper.CheckReference(rrr, "Test.A", "TestKey", "Test.A", "Test.A.B");
		}
		
		// ********************************************************************************************************************************
		
		const string CodeEmptyResourceSetName = @"using System.Resources;
class A {
	void B()
	{
		ResourceManager mgr = new ResourceManager("""", System.Reflection.Assembly.GetExecutingAssembly());
		mgr.GetString(""TestKey"");
	}
}
";
		
		[Test]
		public void EmptyResourceSetName()
		{
			ResourceResolveResult rrr = Resolve(CodeEmptyResourceSetName, 5, 17, null);
			TestHelper.CheckNoReference(rrr);
		}
		
		// ********************************************************************************************************************************
		
		[Test]
		public void ResolverSupportsCSharpFiles()
		{
			Assert.IsTrue(this.Resolver.SupportsFile("a.cs"));
		}
		
		[Test]
		public void CSharpFilePatterns()
		{
			List<string> patterns = new List<string>(this.Resolver.GetPossiblePatternsForFile("a.cs"));
			Assert.Contains("GetString", patterns);
			Assert.Contains("GetStream", patterns);
			Assert.Contains("GetObject", patterns);
			Assert.Contains("ApplyResources", patterns);
			Assert.Contains("[", patterns);
			Assert.AreEqual(5, patterns.Count, "Incorrect number of resource access patterns for C# files.");
		}
		
		[Test]
		public void ResolverDoesNotSupportBooFiles()
		{
			Assert.IsFalse(this.Resolver.SupportsFile("a.boo"));
		}
		
		[Test]
		public void UnsupportedFilePatterns()
		{
			IEnumerable<string> list = this.Resolver.GetPossiblePatternsForFile("a.boo");
			Assert.IsNotNull(list, "IResourceResolver.GetPossiblePatternsForFile must not return null.");
			
			List<string> patterns = new List<string>(list);
			Assert.AreEqual(0, patterns.Count);
		}
		
		#endregion
		
		// ********************************************************************************************************************************
		
		#region Cache tests
		
		const string CodeCacheTest = @"class A
{
	void B()
	{
		System.Resources.ResourceManager mgr = new System.Resources.ResourceManager(""Test.TestResources"", System.Reflection.Assembly.GetExecutingAssembly());
		mgr.GetString(""TestKey"");
		mgr.GetString(""TestKey2"");
	}
}
";
		
		[Test]
		public void UseAstCache()
		{
			NRefactoryAstCacheService.EnableCache();
			
			ResourceResolveResult rrr = Resolve(CodeCacheTest, 5, 17, null);
			TestHelper.CheckReference(rrr, "Test.TestResources", "TestKey", "A", "A.B");
			
			rrr = Resolve(CodeCacheTest, 5, 17, null);
			TestHelper.CheckReference(rrr, "Test.TestResources", "TestKey", "A", "A.B");
			
			rrr = Resolve(CodeCacheTest, 6, 17, null);
			TestHelper.CheckReference(rrr, "Test.TestResources", "TestKey2", "A", "A.B");
			
			NRefactoryAstCacheService.DisableCache();
		}
		
		[Test]
		[ExpectedException(typeof(InvalidOperationException), ExpectedMessage="The AST cache is already enabled.")]
		public void AstCacheEnableTwice()
		{
			NRefactoryAstCacheService.EnableCache();
			NRefactoryAstCacheService.EnableCache();
		}
		
		#endregion
	}
}
