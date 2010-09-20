// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using ICSharpCode.SharpDevelop.Project;
using System;
using System.Collections.Generic;
using Hornung.ResourceToolkit;
using Hornung.ResourceToolkit.Resolver;
using ICSharpCode.SharpDevelop.Dom;
using NUnit.Framework;

namespace ResourceToolkit.Tests
{
	static class TestHelper
	{
		public static void InitializeResolvers()
		{
			NRefactoryResourceResolver.SetResourceResolversListUnitTestOnly(
				new INRefactoryResourceResolver[] {
					new BclNRefactoryResourceResolver(),
					new ICSharpCodeCoreNRefactoryResourceResolver()
				});
			ResourceResolverService.SetResourceResolversListUnitTestOnly(
				new IResourceResolver[] {
					new NRefactoryResourceResolver(),
					new ICSharpCodeCoreResourceResolver()
				});
		}
		
		public static void InitializeParsers()
		{
			Dictionary<string, IParser> parsers = new Dictionary<string, IParser>();
			parsers.Add(".cs", new CSharpBinding.Parser.TParser());
			parsers.Add(".vb", new ICSharpCode.VBNetBinding.TParser());
			ResourceResolverService.SetParsersUnitTestOnly(parsers);
		}
		
		public static void CheckReference(ResourceResolveResult result, string expectedResourceSetName, string expectedKey, string expectedCallingClassFullName, string expectedCallingMemberFullName)
		{
			Assert.IsNotNull(result, "Resource reference should be detected.");
			
			if (expectedResourceSetName == null) {
				
				Assert.IsNull(result.ResourceSetReference, "Resource set reference should not be detected.");
				
			} else {
				
				Assert.IsNotNull(result.ResourceSetReference, "Resource set reference should be detected.");
				Assert.IsNotNull(result.ResourceSetReference.ResourceSetName, "Resource set name must not be null.");
				Assert.AreEqual(expectedResourceSetName, result.ResourceSetReference.ResourceSetName, "Incorrect resource set name.");
				
			}
			
			if (expectedKey == null) {
				
				Assert.IsNull(result.Key, "Resource key should not be detected.");
				
			} else {
				
				Assert.IsNotNull(result.Key, "Resource key should be detected.");
				Assert.AreEqual(expectedKey, result.Key, "Incorrect resource key.");
				
			}
			
			if (expectedCallingClassFullName == null) {
				
				Assert.IsNull(result.CallingClass, "Calling class should not be detected.");
				
			} else {
				
				Assert.IsNotNull(result.CallingClass, "Calling class should be detected.");
				Assert.AreEqual(expectedCallingClassFullName, result.CallingClass.FullyQualifiedName, "Incorrect calling class.");
				
			}
			
			if (expectedCallingMemberFullName == null) {
				
				Assert.IsNull(result.CallingMember, "Calling member should not be detected.");
				
			} else {
				
				Assert.IsNotNull(result.CallingMember, "Calling member should be detected.");
				Assert.AreEqual(expectedCallingMemberFullName, result.CallingMember.FullyQualifiedName, "Incorrect calling member.");
				
			}
		}
		
		public static void CheckNoReference(ResourceResolveResult result)
		{
			Assert.IsNull(result, "Resource reference should not be detected.");
		}
	}
}
