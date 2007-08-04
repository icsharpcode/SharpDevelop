// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Text;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.DefaultEditor.Commands;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Dom.CSharp;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Tests.Utils;
using NUnit.Framework;

namespace ICSharpCode.SharpDevelop.Tests
{
	/// <summary>
	/// SD2-1199. Tests the available overrides for the 
	/// System.Collections.ObjectModel.Collection class.
	/// </summary>
	[TestFixture]
	public class CollectionClassOverridesTestFixture
	{
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			if (!PropertyService.Initialized) {
				PropertyService.InitializeService(String.Empty, String.Empty, String.Empty);
			}
		}
		
		/// <summary>
		/// This shows how to get the list of overridable methods in the
		/// Collection class using reflection only.
		/// </summary>
		public void GetMethodsThroughReflection()
		{
			Assembly a = Assembly.Load("mscorlib");
			Type t = a.GetType("System.Collections.ObjectModel.Collection`1");
			
			List<string> methodNames = new List<string>();
			BindingFlags bindingFlags = BindingFlags.Instance  |
				BindingFlags.NonPublic |
				BindingFlags.DeclaredOnly |
				BindingFlags.Public;
			
			foreach (MethodInfo m in t.GetMethods(bindingFlags)) {
				if (m.IsVirtual && !m.IsSpecialName && !m.IsFinal) {
					methodNames.Add(m.Name);
				}
			}
			
			List<string> expectedMethodNames = new List<string>();
			expectedMethodNames.Add("ClearItems");
			expectedMethodNames.Add("InsertItem");
			expectedMethodNames.Add("RemoveItem");
			expectedMethodNames.Add("SetItem");
			
			StringBuilder sb = new StringBuilder();
			foreach (string s in methodNames.ToArray()) {
				sb.AppendLine(s);
			}
			Assert.AreEqual(expectedMethodNames.ToArray(), methodNames.ToArray(), sb.ToString());
		}
		
		/// <summary>
		/// Tests that the IsSealed property is set for methods that are
		/// flagged as final. The ReflectionMethod class was not setting
		/// this correctly.
		/// </summary>
		[Test]
		public void ExpectedMethodsFromProjectContent()
		{
			ProjectContentRegistry registry = new ProjectContentRegistry();
			IProjectContent mscorlibProjectContent = registry.Mscorlib;
			IClass c = mscorlibProjectContent.GetClass("System.Collections.ObjectModel.Collection", 1);
			
			List<string> methodNames = new List<string>();
			foreach (IMethod m in c.Methods) {
				if (m.IsVirtual && !m.IsSealed) {
					methodNames.Add(m.Name);
				}
			}
			
			List<string> expectedMethodNames = new List<string>();
			expectedMethodNames.Add("ClearItems");
			expectedMethodNames.Add("InsertItem");
			expectedMethodNames.Add("RemoveItem");
			expectedMethodNames.Add("SetItem");
			
			methodNames.Sort();
			expectedMethodNames.Sort();
			
			Assert.AreEqual(expectedMethodNames.ToArray(), methodNames.ToArray());
		}
		
		/// <summary>
		/// Tests that the OverrideMethodsCodeGenerator returns the correct
		/// methods for the System.Collections.ObjectModel.Collection type.
		/// </summary>
		[Test]
		public void CodeGeneratorMethods()
		{
			ProjectContentRegistry registry = new ProjectContentRegistry();
			IProjectContent mscorlibProjectContent = registry.Mscorlib;
			IClass collectionClass = mscorlibProjectContent.GetClass("System.Collections.ObjectModel.Collection", 1);
			
			DefaultProjectContent projectContent = new DefaultProjectContent();
			DefaultCompilationUnit unit = new DefaultCompilationUnit(projectContent);
			DefaultClass c = new DefaultClass(unit, "MyCollection");
			c.BaseTypes.Add(new DefaultReturnType(collectionClass));
			
			MockProject project = new MockProject();
			ProjectService.CurrentProject = project;
			
			OverrideMethodsCodeGenerator codeGenerator = new OverrideMethodsCodeGenerator();
			codeGenerator.Initialize(c);
			
			List<string> methods = new List<string>();
			foreach (object o in codeGenerator.Content) {
				methods.Add(o.ToString());
			}
			
			List<string> expectedMethods = new List<string>();
			expectedMethods.Add("ClearItems");
			expectedMethods.Add("Equals");
			expectedMethods.Add("Finalize");
			expectedMethods.Add("GetHashCode");
			expectedMethods.Add("InsertItem");
			expectedMethods.Add("RemoveItem");
			expectedMethods.Add("SetItem");
			expectedMethods.Add("ToString");
			
			Assert.AreEqual(expectedMethods.ToArray(), methods.ToArray());
		}
	}
}

