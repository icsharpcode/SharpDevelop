// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Tests.Utils;
using NUnit.Framework;

namespace ICSharpCode.SharpDevelop.Tests
{
	/// <summary>
	/// Tests the OverrideCompletionDataProvider GetOverridableProperties.
	/// This property should be added to the IClass interface.
	/// </summary>
	[TestFixture]
	public class OverridablePropertiesTestFixture
	{
		MockClass c;
		MockDefaultReturnType returnType;
		List<IProperty> expectedProperties;
		MockClass declaringType;
		
		[SetUp]
		public void SetUp()
		{
			expectedProperties = new List<IProperty>();
			c = new MockClass("MyClass");
			declaringType = new MockClass("MyDeclaringType");
			returnType = new MockDefaultReturnType();
			c.DefaultReturnType = returnType;
		}
	
		IProperty[] GetOverridableProperties(IClass baseClass)
		{
			return OverrideCompletionDataProvider.GetOverridableProperties(new MockClass("DerivedClass") { BaseType = baseClass.DefaultReturnType });
		}
		
		/// <summary>
		/// Add one overridable property to the return type and this
		/// should be returned in the list of overridable properties.
		/// </summary>
		[Test]
		public void OneOverridablePropertyReturned()
		{
			MockProperty property = new MockProperty("IsRunning");
			property.DeclaringType = declaringType;
			property.IsOverridable = true;
			returnType.Properties.Add(property);
			
			expectedProperties.Add(property);

			IProperty[] properties = GetOverridableProperties(c);
			
			AssertArePropertiesEqual(expectedProperties, properties);
		}
		
		/// <summary>
		/// Make sure that an overridable property is not returned when
		/// it is part of the class being considered.
		/// </summary>
		[Test]
		public void OverridablePropertyPartOfClass()
		{
			MockProperty property = new MockProperty("IsRunning");
			property.DeclaringType = c;
			property.IsOverridable = true;
			returnType.Properties.Add(property);
			
			IProperty[] properties = OverrideCompletionDataProvider.GetOverridableProperties(c);
			
			AssertArePropertiesEqual(expectedProperties, properties);
		}
		
		/// <summary>
		/// An overridable but const property should not be returned.
		/// </summary>
		[Test]
		public void OverridableConstPropertyNotReturned()
		{
			MockProperty property = new MockProperty("IsRunning");
			property.DeclaringType = declaringType;
			property.IsOverridable = true;
			property.IsConst = true;
			returnType.Properties.Add(property);
			
			IProperty[] properties = GetOverridableProperties(c);
			
			AssertArePropertiesEqual(expectedProperties, properties);
		}
		
		/// <summary>
		/// An overridable but private property should not be returned.
		/// </summary>
		[Test]
		public void OverridablePrivatePropertyNotReturned()
		{
			MockProperty property = new MockProperty("Run");
			property.DeclaringType = declaringType;
			property.IsOverridable = true;
			property.IsPrivate = true;
			returnType.Properties.Add(property);
			
			IProperty[] properties = GetOverridableProperties(c);
			
			AssertArePropertiesEqual(expectedProperties, properties);
		}
		
		[Test]
		[ExpectedException(typeof(ArgumentException))]
		public void NullArgument()
		{
			OverrideCompletionDataProvider.GetOverridableProperties(null);
		}
		
		void AssertArePropertiesEqual(List<IProperty> expectedProperties, IProperty[] properties)
		{
			// Get a list of expected property names.
			List<string> expectedPropertyNames = new List<string>();
			foreach (IProperty expectedProperty in expectedProperties) {
				expectedPropertyNames.Add(expectedProperty.Name);
			}
			
			// Get a list of actual property names.
			List<string> propertyNames = new List<string>();
			foreach (IProperty p in properties) {
				propertyNames.Add(p.Name);
			}
			
			// Compare the two arrays.
			Assert.AreEqual(expectedPropertyNames.ToArray(), propertyNames.ToArray());
		}
	}
}
