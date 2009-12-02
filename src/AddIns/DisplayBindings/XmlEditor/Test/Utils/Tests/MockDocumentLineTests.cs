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
	public class MockDocumentLineTests
	{
		MockDocumentLine documentLine;
		
		[SetUp]
		public void Init()
		{
			documentLine = new MockDocumentLine();
		}
		
		[Test]
		public void CanSetAndGetLineNumberProperty()
		{
			documentLine.LineNumber = 2;
			Assert.AreEqual(2, documentLine.LineNumber);
		}
	}
}
