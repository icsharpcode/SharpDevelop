// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using NUnit.Framework;
using XmlEditor.Tests.Utils;

namespace XmlEditor.Tests.Utils.Tests
{
	[TestFixture]
	public class MockTextEditorOptionsTests
	{
		MockTextEditorOptions options;
		
		[SetUp]
		public void Init()
		{
			options = new MockTextEditorOptions();
		}
		
		[Test]
		public void IndentationStringIsSingleTabByDefault()
		{
			Assert.AreEqual("\t", options.IndentationString);
		}
		
		[Test]
		public void CanSetAndGetIndentationString()
		{
			options.IndentationString = "abc";
			Assert.AreEqual("abc", options.IndentationString);
		}
	}
}
