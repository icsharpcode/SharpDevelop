// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using ICSharpCode.FormsDesigner;
using ICSharpCode.WixBinding;
using NUnit.Framework;
using System;
using System.ComponentModel;
using System.Windows.Forms;
using WixBinding.Tests.Utils;

namespace WixBinding.Tests.DialogLoading
{
	/// <summary>
	/// Unit tests for the XmlDesignerLoader.NameCreationService.ValidName method.
	/// </summary>
	[TestFixture]
	public class NameCreationServiceValidNameTests
	{
		XmlDesignerNameCreationService nameCreationService;
		MockDesignerLoaderHost loaderHost;
		
		[SetUp]
		public void Init()
		{
			loaderHost = new MockDesignerLoaderHost();
			nameCreationService = new XmlDesignerNameCreationService(loaderHost);
		}
		
		[Test]
		public void NullName()
		{
			Assert.IsFalse(nameCreationService.IsValidName(null));
		}
		
		[Test]
		public void EmptyString()
		{
			Assert.IsFalse(nameCreationService.IsValidName(String.Empty));
		}
		
		[Test]
		public void FirstCharIsDigit()
		{
			Assert.IsFalse(nameCreationService.IsValidName("8"));
		}
		
		[Test]
		public void FirstCharIsUnderscore()
		{
			Assert.IsTrue(nameCreationService.IsValidName("_"));
		}
		
		[Test]
		public void FirstCharIsNonDigit()
		{
			Assert.IsFalse(nameCreationService.IsValidName("a*"));
		}
		
		
		[Test]
		public void SecondCharIsUnderscore()
		{
			Assert.IsTrue(nameCreationService.IsValidName("a_"));
		}
		
		[Test]
		public void SecondCharIsNonDigit()
		{
			Assert.IsFalse(nameCreationService.IsValidName("a$"));
		}
		
		[Test]
		[ExpectedException(typeof(Exception), ExpectedMessage = "Invalid name 9")]
		public void ValidateNameThrowsExceptionWhenFirstCharIsDigit()
		{
			nameCreationService.ValidateName("9");
		}
		
		[Test]
		public void FirstTextBoxName()
		{
			Assert.AreEqual("textBox1", nameCreationService.CreateName(typeof(TextBox)));
		}
		
		[Test]
		public void SecondTextBoxName()
		{
			Component component = new Component();
			loaderHost.Container.Add(component, "textBox1");
			Assert.AreEqual("textBox2", nameCreationService.CreateName(typeof(TextBox)));
		}
	}
}
