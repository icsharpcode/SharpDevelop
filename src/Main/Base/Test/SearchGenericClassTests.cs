// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

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
		ProjectContentRegistry projectContentRegistry = ParserService.DefaultProjectContentRegistry;
		
		#region Helper methods
		ICompilationUnit Prepare(LanguageProperties language)
		{
			DefaultProjectContent pc = new DefaultProjectContent();
			pc.ReferencedContents.Add(projectContentRegistry.Mscorlib);
			pc.Language = language;
			DefaultCompilationUnit cu = new DefaultCompilationUnit(pc);
			if (language == LanguageProperties.VBNet) {
				cu.Usings.Add(CreateUsing(pc, "syStEm.coLLectIons"));
				pc.DefaultImports = new DefaultUsing(pc);
				pc.DefaultImports.Usings.Add("syStEm");
				pc.DefaultImports.Usings.Add("syStEm.coLLEctionS.GeNeRic");
			} else {
				cu.Usings.Add(CreateUsing(pc, "System"));
				cu.Usings.Add(CreateUsing(pc, "System.Collections"));
				cu.Usings.Add(CreateUsing(pc, "System.Collections.Generic"));
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
			ICompilationUnit cu = Prepare(LanguageProperties.CSharp);
			IReturnType c = cu.ProjectContent.SearchType(new SearchTypeRequest(type, typeParameterCount, null, cu, 1, 1)).Result;
			Assert.IsNotNull(c, type + "not found");
			return c;
		}
		
		IReturnType SearchTypeVB(string type, int typeParameterCount)
		{
			ICompilationUnit cu = Prepare(LanguageProperties.VBNet);
			IReturnType c = cu.ProjectContent.SearchType(new SearchTypeRequest(type, typeParameterCount, null, cu, 1, 1)).Result;
			Assert.IsNotNull(c, type + "not found");
			return c;
		}
		
		void CheckType(string shortName, string vbShortName, string fullType, int typeParameterCount)
		{
			IReturnType type = SearchType(shortName, typeParameterCount);
			Assert.AreEqual(fullType, type.FullyQualifiedName);
			Assert.AreEqual(typeParameterCount, type.TypeParameterCount);
			type = SearchTypeVB(vbShortName, typeParameterCount);
			Assert.AreEqual(fullType, type.FullyQualifiedName);
			Assert.AreEqual(typeParameterCount, type.TypeParameterCount);
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
