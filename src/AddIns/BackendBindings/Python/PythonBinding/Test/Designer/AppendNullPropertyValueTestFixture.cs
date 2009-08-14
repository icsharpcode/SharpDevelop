// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.ComponentModel;
using ICSharpCode.PythonBinding;
using NUnit.Framework;

namespace PythonBinding.Tests.Designer
{
	/// <summary>
	/// Tests that a null property value does not cause a NullReferenceException in the
	/// PythonControl's AppendProperty method.
	/// </summary>
	[TestFixture]
	public class AppendNullPropertyValueTestFixture
	{
		string fooBar;
		
		public string FooBar {
			get { return fooBar; }
			set { fooBar = value; }
		}
		
		[Test]
		public void GeneratedCode()
		{
			string expectedCode = "self._myObject.FooBar = None\r\n";
			PropertyDescriptor propertyDescriptor = TypeDescriptor.GetProperties(this).Find("FooBar", false);
			
			PythonDesignerComponent designerComponent = new PythonDesignerComponent(null);
			PythonCodeBuilder codeBuilder = new PythonCodeBuilder();
			designerComponent.AppendProperty(codeBuilder, "self._myObject", this, propertyDescriptor);
			string generatedCode = codeBuilder.ToString();
			
			Assert.AreEqual(expectedCode, generatedCode, generatedCode);
		}
	}
}
