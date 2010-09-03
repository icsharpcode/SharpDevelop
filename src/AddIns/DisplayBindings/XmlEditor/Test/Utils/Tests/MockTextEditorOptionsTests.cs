// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
