// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using ICSharpCode.XmlEditor;
using NUnit.Framework;
using System;

namespace XmlEditor.Tests.XPath
{
	/// <summary>
	/// Tests the XmlEncoder class's Encode method which
	/// makes sure that a parameter being used in an xpath string is
	/// correctly encoded.
	/// </summary>
	[TestFixture]
	public class EncodeXPathParameterTests
	{
		[Test]
		public void SingleQuote()
		{
			Assert.AreEqual("&apos;", XmlEncoder.Encode("'", '\''));
		}
		
		[Test]
		public void Ampersand()
		{
			Assert.AreEqual("&amp;", XmlEncoder.Encode("&", '\''));
		}
	}
}
