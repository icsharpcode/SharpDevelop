// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.RubyBinding;
using ICSharpCode.SharpDevelop.Dom;
using NUnit.Framework;

namespace RubyBinding.Tests.Parsing
{
	/// <summary>
	/// A string of the form "require #" would cause a null reference exception since the ruby ast walker tries
	/// to use the MethodCall's Argument which is null.
	/// </summary>
	[TestFixture]
	public class ParseRequireFollowedByCommentTestFixture
	{
		[Test]
		public void ParseDoesNotThrowNullReferenceException()
		{
			string ruby = "require #";
			
			DefaultProjectContent projectContent = new DefaultProjectContent();
			RubyParser parser = new RubyParser();
			ICompilationUnit unit = null;
		
			Assert.DoesNotThrow(delegate { unit = parser.Parse(projectContent, @"C:\test.rb", ruby); });
			Assert.IsNotNull(unit);
		}
	}
}
