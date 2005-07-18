/*
 * Created by SharpDevelop.
 * User: Daniel Grunwald
 * Date: 18.07.2005
 * Time: 20:29
 */

using System;
using System.Collections.Generic;
using NUnit.Framework;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.SharpDevelop.Tests
{
	[TestFixture]
	public class SearchClassTests
	{
		#region Helper methods
		ICompilationUnit Prepare(LanguageProperties language)
		{
			DefaultProjectContent pc = new DefaultProjectContent();
			pc.ReferencedContents.Add(ProjectContentRegistry.GetMscorlibContent());
			pc.Language = language;
			DefaultCompilationUnit cu = new DefaultCompilationUnit(pc);
			cu.Usings.Add(CreateUsing(pc, "System"));
			return cu;
		}
		
		IUsing CreateUsing(IProjectContent pc, string @namespace)
		{
			DefaultUsing @using = new DefaultUsing(pc);
			@using.Usings.Add(@namespace);
			return @using;
		}
		
		IClass SearchType(string type)
		{
			ICompilationUnit cu = Prepare(LanguageProperties.CSharp);
			return cu.ProjectContent.SearchType(type, null, 1, 1);
		}
		
		IClass SearchTypeVB(string type)
		{
			ICompilationUnit cu = Prepare(LanguageProperties.VBNet);
			return cu.ProjectContent.SearchType(type, null, 1, 1);
		}
		
		string SearchNamespace(string @namespace)
		{
			ICompilationUnit cu = Prepare(LanguageProperties.CSharp);
			return cu.ProjectContent.SearchNamespace(@namespace, null, 1, 1);
		}
		
		string SearchNamespaceVB(string @namespace)
		{
			ICompilationUnit cu = Prepare(LanguageProperties.VBNet);
			return cu.ProjectContent.SearchNamespace(@namespace, null, 1, 1);
		}
		#endregion
		
		[Test]
		public void SearchFullyQualifiedClass()
		{
			Assert.AreEqual("System.Reflection.Assembly", SearchType("System.Reflection.Assembly").FullyQualifiedName);
		}
		
		[Test]
		public void SearchFullyQualifiedClassVB()
		{
			Assert.AreEqual("System.Reflection.Assembly", SearchTypeVB("SYStem.RefleCtion.asSembly").FullyQualifiedName);
		}
		
		[Test]
		public void SearchFullyQualifiedNamespace()
		{
			Assert.AreEqual("System.Collections.Generic", SearchNamespace("System.Collections.Generic"));
		}
		
		[Test]
		public void SearchFullyQualifiedNamespaceVB()
		{
			Assert.AreEqual("System.Collections.Generic", SearchNamespace("SyStem.COllEctions.GeNEric"));
		}
		
		[Test]
		public void SearchEnvironment()
		{
			Assert.AreEqual("System.Environment", SearchType("Environment").FullyQualifiedName);
		}
		
		[Test]
		public void SearchEnvironmentVB()
		{
			Assert.AreEqual("System.Environment", SearchType("EnVIroNmEnt").FullyQualifiedName);
		}
	}
}
