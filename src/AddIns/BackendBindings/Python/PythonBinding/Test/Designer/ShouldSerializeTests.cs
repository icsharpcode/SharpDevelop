// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Reflection;
using System.Windows.Forms;

using ICSharpCode.PythonBinding;
using NUnit.Framework;

namespace PythonBinding.Tests.Designer
{
	/// <summary>
	/// Tests the PythonForm.ShouldSerialize method which determines whether a form's property
	/// should be serialized.
	/// </summary>
	[TestFixture]
	public class ShouldSerializeTests
	{
		PythonForm pythonForm;
		Form form;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			form = new Form();
		}
		
		[TestFixtureTearDown]
		public void TearDownFixture()
		{
			form.Dispose();
		}
		
		[SetUp]
		public void Init()
		{
			pythonForm = new PythonForm();
		}
		
		[Test]
		public void ShouldNotSerializePropertyWithFalseBrowseAttribute()
		{
			AssertShouldNotSerialize("LayoutEngine", form);
		}
		
		[Test]
		public void ShouldNotSerializeEmptyStringTextProperty()
		{
			form.Text = String.Empty;
			AssertShouldNotSerialize("Text", form);
		}
		
		void AssertShouldSerialize(string propertyName, Form form)
		{
			AssertShouldSerialize(propertyName, form, true);
		}
		
		void AssertShouldNotSerialize(string propertyName, Form form)
		{
			AssertShouldSerialize(propertyName, form, false);
		}
		
		void AssertShouldSerialize(string propertyName, Form form, bool isTrue)
		{
			PropertyDescriptor property = TypeDescriptor.GetProperties(form).Find(propertyName, true);
			Assert.AreEqual(isTrue, pythonForm.ShouldSerialize(form, property));
		}
	}
}
