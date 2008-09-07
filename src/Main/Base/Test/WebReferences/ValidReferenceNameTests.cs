// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;
using NUnit.Framework;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop.Tests.WebReferences
{
	/// <summary>
	/// Tests the WebReference.IsValidReferenceName method.
	/// </summary>
	[TestFixture]
	public class ValidReferenceNameTests
	{
		[Test]
		public void EmptyStringIsNotAValidReferenceName()
		{
			Assert.IsFalse(WebReference.IsValidReferenceName(String.Empty));
		}
		
		[Test]
		public void ReferenceNameCannotContainABackslash()
		{
			Assert.IsFalse(WebReference.IsValidReferenceName("ab\\c"));
		}
		
		[Test]
		public void ValidReferenceName()
		{
			Assert.IsTrue(WebReference.IsValidReferenceName("a"));
		}
		
		[Test]
		public void NoInvalidDirectoryPathCharacters()
		{
			char[] ch = Path.GetInvalidPathChars();
			string name = new string(ch);
			Assert.IsFalse(WebReference.IsValidReferenceName(name));
		}
	}
}
