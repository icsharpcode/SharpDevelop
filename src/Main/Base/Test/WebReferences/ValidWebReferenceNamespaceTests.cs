// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using NUnit.Framework;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop.Tests.WebReferences
{
	/// <summary>
	/// When the user types in a namespace for the web service the WebReference.IsValidNamespace is called.
	/// This test fixture tests that method.
	/// </summary>
	[TestFixture]
	public class ValidWebReferenceNamespaceTests
	{
		[Test]
		public void ValidWebReferenceName()
		{
			Assert.IsTrue(WebReference.IsValidNamespace("a"));
		}
		
		[Test]
		public void EmptyStringIsNotValid()
		{
			Assert.IsFalse(WebReference.IsValidNamespace(String.Empty));	
		}
		
		[Test]
		public void FirstCharacterCannotBeANumber()
		{
			Assert.IsFalse(WebReference.IsValidNamespace("8abc"));
		}
		
		[Test]
		public void SecondCharacterCanBeANumber()
		{
			Assert.IsTrue(WebReference.IsValidNamespace("a8"));
		}	
		
		[Test]
		public void SecondCharacterCanBeAnUnderscore()
		{
			Assert.IsTrue(WebReference.IsValidNamespace("a_"));
		}		
		
		[Test]
		public void SecondCharacterCanBeADot()
		{
			Assert.IsTrue(WebReference.IsValidNamespace("a.b"));
		}
		
		[Test]
		public void FirstCharacterCanBeAnUnderscore()
		{
			Assert.IsTrue(WebReference.IsValidNamespace("_aa"));
		}
		
		[Test]
		public void InvalidReferenceName()
		{
			Assert.IsFalse(WebReference.IsValidNamespace("Test.10.4.4.4"));
		}
	}
}
