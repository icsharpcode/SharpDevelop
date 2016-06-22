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
