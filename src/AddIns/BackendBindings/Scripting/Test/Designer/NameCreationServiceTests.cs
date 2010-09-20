// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.ComponentModel.Design.Serialization;
using System.Windows.Forms;

using ICSharpCode.Scripting;
using ICSharpCode.Scripting.Tests.Utils;
using NUnit.Framework;

namespace ICSharpCode.Scripting.Tests.Designer
{
	[TestFixture]
	public class NameCreationServiceTestFixture
	{
		ScriptingNameCreationService nameCreationService;
		MockDesignerLoaderHost host;
		
		[SetUp]
		public void Init()
		{
			host = new MockDesignerLoaderHost();
			host.Container.Add(new Button(), "button1");
			nameCreationService = new ScriptingNameCreationService(host);
		}
		
		[Test]
		public void IsValidNameReturnsFalseIfComponentInContainerAlreadyUsesName()
		{
			Assert.IsFalse(nameCreationService.IsValidName("button1"));
		}
		
		[Test]
		public void IsValidNameReturnsTrueIfNameNotInUse()
		{
			Assert.IsTrue(nameCreationService.IsValidName("unknown"));
		}
		
		[Test]
		public void NullReferenceNotThrownWhenHostContainerIsNull()
		{
			host.Container = null;
			Assert.IsTrue(nameCreationService.IsValidName("button1"));
		}
		
		[Test]
		public void CreateNameReturnsTypeNameFollowedByNumberOne()
		{
			Assert.AreEqual("button1", nameCreationService.CreateName(null, typeof(Button)));
		}
		
		[Test]
		public void CreateNameReturnsTypeNameFollowedByNumberTwoIfNameExistsInContainer()
		{
			Assert.AreEqual("button2", nameCreationService.CreateName(host.Container, typeof(Button)));
		}
		
		[Test]
		public void ValidateNameThrowsExceptionIfNameExistsInContainer()
		{
			Exception ex = Assert.Throws<Exception>(delegate { nameCreationService.ValidateName("button1"); });
			Assert.AreEqual("Invalid name button1", ex.Message);
		}
		
		[Test]
		public void ValidateNameDoesNotThrowExceptionIfNameDoesNotExistInContainer()
		{
			Assert.DoesNotThrow(delegate { nameCreationService.ValidateName("button2"); });
		}
		
		[Test]
		public void IsValidReturnsFalseWhenNameIsNull()
		{
			Assert.IsFalse(nameCreationService.IsValidName(null));
		}
		
		[Test]
		public void IsValidReturnsFalseWhenNameIsEmptyString()
		{
			Assert.IsFalse(nameCreationService.IsValidName(String.Empty));
		}
		
		[Test]
		public void IsValidReturnsFalseWhenNameContainsNonLetterOrDigit()
		{
			Assert.IsFalse(nameCreationService.IsValidName("a%b"));
		}
		
		[Test]
		public void IsValidReturnsFalseWhenFirstCharacterOfNameIsNumber()
		{
			Assert.IsFalse(nameCreationService.IsValidName("1abc"));
		}
		
		[Test]
		public void IsValidReturnsTrueWhenFirstCharacterOfNameIsUnderscore()
		{
			Assert.IsTrue(nameCreationService.IsValidName("_abc"));
		}
	}
}
