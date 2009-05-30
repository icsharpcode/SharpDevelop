// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision: 1864 $</version>
// </file>

using ICSharpCode.XmlEditor;
using NUnit.Framework;
using System;

namespace XmlEditor.Tests.XPathQuery
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
