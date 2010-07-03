// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

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
