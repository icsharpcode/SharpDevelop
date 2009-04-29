// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using NUnit.Framework;

namespace ICSharpCode.AvalonEdit.Tests.Utils
{
	[TestFixture]
	public class IndentationStringTests
	{
		[Test]
		public void IndentWithSingleTab()
		{
			var options = new TextEditorOptions { IndentationSize = 4, ConvertTabsToSpaces = false };
			Assert.AreEqual("\t", options.IndentationString);
			Assert.AreEqual("\t", options.GetIndentationString(2));
			Assert.AreEqual("\t", options.GetIndentationString(3));
			Assert.AreEqual("\t", options.GetIndentationString(4));
			Assert.AreEqual("\t", options.GetIndentationString(5));
			Assert.AreEqual("\t", options.GetIndentationString(6));
		}
		
		[Test]
		public void IndentWith4Spaces()
		{
			var options = new TextEditorOptions { IndentationSize = 4, ConvertTabsToSpaces = true };
			Assert.AreEqual("    ", options.IndentationString);
			Assert.AreEqual("   ", options.GetIndentationString(2));
			Assert.AreEqual("  ", options.GetIndentationString(3));
			Assert.AreEqual(" ", options.GetIndentationString(4));
			Assert.AreEqual("    ", options.GetIndentationString(5));
			Assert.AreEqual("   ", options.GetIndentationString(6));
		}
	}
}
