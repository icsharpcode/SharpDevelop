// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
