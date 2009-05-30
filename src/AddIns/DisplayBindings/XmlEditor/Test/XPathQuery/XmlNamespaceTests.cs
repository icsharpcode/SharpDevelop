// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision: 1662 $</version>
// </file>

using ICSharpCode.XmlEditor;
using NUnit.Framework;
using System;

namespace XmlEditor.Tests.XPathQuery
{
	[TestFixture]
	public class XmlNamespaceTests
	{
		[Test]
		public void SimpleNamespace()
		{
			XmlNamespace ns = new XmlNamespace("s", "http://sharpdevelop.com");
			Assert.AreEqual("s", ns.Prefix);
			Assert.AreEqual("http://sharpdevelop.com", ns.Uri);
		}
	}
}
