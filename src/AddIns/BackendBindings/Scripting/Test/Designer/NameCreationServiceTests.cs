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

//using System;
//using System.ComponentModel.Design.Serialization;
//using System.Windows.Forms;
//
//using ICSharpCode.Scripting;
//using ICSharpCode.Scripting.Tests.Utils;
//using NUnit.Framework;
//
//namespace ICSharpCode.Scripting.Tests.Designer
//{
//	[TestFixture]
//	public class NameCreationServiceTestFixture
//	{
//		ScriptingNameCreationService nameCreationService;
//		MockDesignerLoaderHost host;
//		
//		[SetUp]
//		public void Init()
//		{
//			host = new MockDesignerLoaderHost();
//			host.Container.Add(new Button(), "button1");
//			nameCreationService = new ScriptingNameCreationService(host);
//		}
//		
//		[Test]
//		public void IsValidNameReturnsFalseIfComponentInContainerAlreadyUsesName()
//		{
//			Assert.IsFalse(nameCreationService.IsValidName("button1"));
//		}
//		
//		[Test]
//		public void IsValidNameReturnsTrueIfNameNotInUse()
//		{
//			Assert.IsTrue(nameCreationService.IsValidName("unknown"));
//		}
//		
//		[Test]
//		public void NullReferenceNotThrownWhenHostContainerIsNull()
//		{
//			host.Container = null;
//			Assert.IsTrue(nameCreationService.IsValidName("button1"));
//		}
//		
//		[Test]
//		public void CreateNameReturnsTypeNameFollowedByNumberOne()
//		{
//			Assert.AreEqual("button1", nameCreationService.CreateName(null, typeof(Button)));
//		}
//		
//		[Test]
//		public void CreateNameReturnsTypeNameFollowedByNumberTwoIfNameExistsInContainer()
//		{
//			Assert.AreEqual("button2", nameCreationService.CreateName(host.Container, typeof(Button)));
//		}
//		
//		[Test]
//		public void ValidateNameThrowsExceptionIfNameExistsInContainer()
//		{
//			Exception ex = Assert.Throws<Exception>(delegate { nameCreationService.ValidateName("button1"); });
//			Assert.AreEqual("Invalid name button1", ex.Message);
//		}
//		
//		[Test]
//		public void ValidateNameDoesNotThrowExceptionIfNameDoesNotExistInContainer()
//		{
//			Assert.DoesNotThrow(delegate { nameCreationService.ValidateName("button2"); });
//		}
//		
//		[Test]
//		public void IsValidReturnsFalseWhenNameIsNull()
//		{
//			Assert.IsFalse(nameCreationService.IsValidName(null));
//		}
//		
//		[Test]
//		public void IsValidReturnsFalseWhenNameIsEmptyString()
//		{
//			Assert.IsFalse(nameCreationService.IsValidName(String.Empty));
//		}
//		
//		[Test]
//		public void IsValidReturnsFalseWhenNameContainsNonLetterOrDigit()
//		{
//			Assert.IsFalse(nameCreationService.IsValidName("a%b"));
//		}
//		
//		[Test]
//		public void IsValidReturnsFalseWhenFirstCharacterOfNameIsNumber()
//		{
//			Assert.IsFalse(nameCreationService.IsValidName("1abc"));
//		}
//		
//		[Test]
//		public void IsValidReturnsTrueWhenFirstCharacterOfNameIsUnderscore()
//		{
//			Assert.IsTrue(nameCreationService.IsValidName("_abc"));
//		}
//	}
//}
