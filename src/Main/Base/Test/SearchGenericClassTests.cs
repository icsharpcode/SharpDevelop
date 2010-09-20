// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using NUnit.Framework;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.SharpDevelop.Tests
{
	[TestFixture]
	public class SearchGenericClassTests
	{
		ProjectContentRegistry projectContentRegistry = AssemblyParserService.DefaultProjectContentRegistry;
		
		#region Helper methods
		// usingMode: 0 = one using-statement for each namespace (correctly cased)
		//            1 = mixture of using statements and default imports (incorrectly cased)
		//            2 = all default imports (incorrectly cased)
		ICompilationUnit Prepare(LanguageProperties language, int usingMode)
		{
			DefaultProjectContent pc = new DefaultProjectContent();
			pc.ReferencedContents.Add(projectContentRegistry.Mscorlib);
			pc.Language = language;
			DefaultCompilationUnit cu = new DefaultCompilationUnit(pc);
			if (usingMode == 1) {
				cu.UsingScope.Usings.Add(CreateUsing(pc, "syStEm.coLLectIons"));
				pc.DefaultImports = new DefaultUsing(pc);
				pc.DefaultImports.Usings.Add("syStEm");
				pc.DefaultImports.Usings.Add("syStEm.coLLEctionS.GeNeRic");
			} else if (usingMode == 2) {
				pc.DefaultImports = new DefaultUsing(pc);
				pc.DefaultImports.Usings.Add("syStEm");
				pc.DefaultImports.Usings.Add("syStEm.coLLEctioNs");
				pc.DefaultImports.Usings.Add("syStEm.coLLEctionS.GeNeRic");
			} else { // usingMode == 0
				cu.UsingScope.Usings.Add(CreateUsing(pc, "System"));
				cu.UsingScope.Usings.Add(CreateUsing(pc, "System.Collections"));
				cu.UsingScope.Usings.Add(CreateUsing(pc, "System.Collections.Generic"));
			}
			return cu;
		}
		
		IUsing CreateUsing(IProjectContent pc, string @namespace)
		{
			DefaultUsing @using = new DefaultUsing(pc);
			@using.Usings.Add(@namespace);
			return @using;
		}
		
		IReturnType SearchType(string type, int typeParameterCount)
		{
			ICompilationUnit cu = Prepare(LanguageProperties.CSharp, 0);
			IReturnType c = cu.ProjectContent.SearchType(new SearchTypeRequest(type, typeParameterCount, null, cu, 1, 1)).Result;
			Assert.IsNotNull(c, type + "not found");
			return c;
		}
		
		IReturnType SearchTypeVB(string type, int typeParameterCount)
		{
			ICompilationUnit cu;
			cu = Prepare(LanguageProperties.VBNet, 0);
			IReturnType c0 = cu.ProjectContent.SearchType(new SearchTypeRequest(type, typeParameterCount, null, cu, 1, 1)).Result;
			Assert.IsNotNull(c0, type + "not found for mode=0");
			
			cu = Prepare(LanguageProperties.VBNet, 1);
			IReturnType c1 = cu.ProjectContent.SearchType(new SearchTypeRequest(type, typeParameterCount, null, cu, 1, 1)).Result;
			Assert.IsNotNull(c1, type + "not found for mode=1");
			
			cu = Prepare(LanguageProperties.VBNet, 2);
			IReturnType c2 = cu.ProjectContent.SearchType(new SearchTypeRequest(type, typeParameterCount, null, cu, 1, 1)).Result;
			Assert.IsNotNull(c2, type + "not found for mode=2");
			
			Assert.IsTrue(c0.Equals(c1) && c0.Equals(c2));
			
			return c0;
		}
		
		void CheckType(string shortName, string vbShortName, string fullType, int typeParameterCount)
		{
			IReturnType type = SearchType(shortName, typeParameterCount);
			Assert.AreEqual(fullType, type.FullyQualifiedName);
			Assert.AreEqual(typeParameterCount, type.TypeArgumentCount);
			type = SearchTypeVB(vbShortName, typeParameterCount);
			Assert.AreEqual(fullType, type.FullyQualifiedName);
			Assert.AreEqual(typeParameterCount, type.TypeArgumentCount);
		}
		#endregion
		
		// EventHandler vs. EventHandler<TEventArgs>
		// both mscorlib, both namespace System
		[Test] public void FindEventHandler() {
			CheckType("EventHandler", "EvEnThAndler", "System.EventHandler", 0);
		}
		[Test] public void FindGenericEventHandler() {
			CheckType("EventHandler", "EvEnThAndler", "System.EventHandler", 1);
		}
		
		
		[Test] public void FindNullableClass() {
			CheckType("Nullable", "NuLLable", "System.Nullable", 0);
		}
		[Test] public void FindNullableStruct() {
			CheckType("Nullable", "NuLLable", "System.Nullable", 1);
		}
		
		// ICollection vs. ICollection<T>
		// both mscorlib, different namespaces
		[Test] public void FindCollection() {
			CheckType("ICollection", "IColLEction", "System.Collections.ICollection", 0);
		}
		[Test] public void FindGenericCollection() {
			CheckType("ICollection", "IColLEction", "System.Collections.Generic.ICollection", 1);
		}
	}
}
