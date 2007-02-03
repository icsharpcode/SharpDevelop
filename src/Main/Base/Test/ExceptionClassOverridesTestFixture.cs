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
	/// Tests the available overrides for the 
	/// System.Exception class.
	/// </summary>
	[TestFixture]
	public class ExceptionClassOverridesTestFixture
	{
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			if (!PropertyService.Initialized) {
				PropertyService.InitializeService(String.Empty, String.Empty, String.Empty);
			}
		}
		
		/// <summary>
		/// This shows how to get the list of overridable properties in the
		/// Exception class using reflection only.
		/// </summary>
		public void GetPropertiesThroughReflection()
		{
			Assembly a = Assembly.Load("mscorlib");
			Type t = a.GetType("System.Exception");
			
			List<string> propertyNames = new List<string>();
			BindingFlags bindingFlags = BindingFlags.Instance  |
				BindingFlags.NonPublic |
				BindingFlags.DeclaredOnly |
				BindingFlags.Public;
			
			foreach (PropertyInfo p in t.GetProperties(bindingFlags)) {
				MethodInfo m = p.GetGetMethod(true);
				if (m.IsVirtual && !m.IsPrivate && !m.IsFinal) {
					propertyNames.Add(p.Name);
				}
			}
			
			List<string> expectedPropertyNames = new List<string>();
			expectedPropertyNames.Add("Data");
			expectedPropertyNames.Add("HelpLink");
			expectedPropertyNames.Add("Message");
			expectedPropertyNames.Add("Source");
			expectedPropertyNames.Add("StackTrace");
			
			StringBuilder sb = new StringBuilder();
			foreach (string s in propertyNames.ToArray()) {
				sb.AppendLine(s);
			}
			Assert.AreEqual(expectedPropertyNames.ToArray(), propertyNames.ToArray(), sb.ToString());
		}
		
		/// <summary>
		/// Tests that the IsSealed property is set for properties that are
		/// flagged as final. The ReflectionProperty class was not setting
		/// this correctly.
		/// </summary>
		[Test]
		public void ExpectedPropertiesFromProjectContent()
		{
			ProjectContentRegistry registry = new ProjectContentRegistry();
			IProjectContent mscorlibProjectContent = registry.Mscorlib;
			IClass c = mscorlibProjectContent.GetClass("System.Exception", 0);
			
			List<string> propertyNames = new List<string>();
			foreach (IProperty p in c.Properties) {
				if (p.IsVirtual && !p.IsSealed) {
					propertyNames.Add(p.Name);
				}
			}
			propertyNames.Sort();
			
			List<string> expectedPropertyNames = new List<string>();
			expectedPropertyNames.Add("Data");
			expectedPropertyNames.Add("HelpLink");
			expectedPropertyNames.Add("Message");
			expectedPropertyNames.Add("Source");
			expectedPropertyNames.Add("StackTrace");
			
			Assert.AreEqual(expectedPropertyNames.ToArray(), propertyNames.ToArray());
		}
		
		/// <summary>
		/// Tests that the OverridePropertyCodeGenerator returns the correct
		/// methods for the System.Exception type.
		/// </summary>
		[Test]
		public void CodeGeneratorProperties()
		{
			ProjectContentRegistry registry = new ProjectContentRegistry();
			IProjectContent mscorlibProjectContent = registry.Mscorlib;
			IClass exceptionClass = mscorlibProjectContent.GetClass("System.Exception", 0);
			
			DefaultProjectContent projectContent = new DefaultProjectContent();
			DefaultCompilationUnit unit = new DefaultCompilationUnit(projectContent);
			DefaultClass c = new DefaultClass(unit, "MyException");
			c.BaseTypes.Add(new DefaultReturnType(exceptionClass));
			
			MockProject project = new MockProject();
			ProjectService.CurrentProject = project;
			
			OverridePropertiesCodeGenerator codeGenerator = new OverridePropertiesCodeGenerator();
			codeGenerator.Initialize(c);
			
			List<string> properties = new List<string>();
			foreach (object o in codeGenerator.Content) {
				properties.Add(o.ToString());
			}
			
			List<string> expectedProperties = new List<string>();
			expectedProperties.Add("Data");
			expectedProperties.Add("HelpLink");
			expectedProperties.Add("Message");
			expectedProperties.Add("Source");
			expectedProperties.Add("StackTrace");
			
			Assert.AreEqual(expectedProperties.ToArray(), properties.ToArray());
		}
	}
}

