// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Dom;
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
			PropertyService.InitializeServiceForUnitTests();
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
	}
}
