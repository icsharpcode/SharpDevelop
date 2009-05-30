// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision: 2760 $</version>
// </file>

using System;
using System.Windows.Forms;

using ICSharpCode.TextEditor.Gui.CompletionWindow;
using ICSharpCode.XmlEditor;
using NUnit.Framework;

namespace XmlEditor.Tests.Completion
{
	/// <summary>
	/// Tests the XmlCompletionDataProvider's ProcessKey method.
	/// </summary>
	[TestFixture]
	public class ProcessKeyTests
	{
		XmlCompletionDataProvider provider;		
		
		[SetUp]
		public void Init()
		{
			XmlSchemaCompletionDataCollection schemas = new XmlSchemaCompletionDataCollection();
			provider = new XmlCompletionDataProvider(schemas, null, null);
		}
		
		/// <summary>
		/// A space character should be treated as a normal character.
		/// </summary>
		[Test]
		public void SpaceChar()
		{
			Assert.AreEqual(CompletionDataProviderKeyResult.NormalKey, provider.ProcessKey(' '));
		}
		
		[Test]
		public void TabChar()
		{
			Assert.AreEqual(CompletionDataProviderKeyResult.InsertionKey, provider.ProcessKey('\t'));
		}		

		[Test]
		public void ReturnChar()
		{
			Assert.AreEqual(CompletionDataProviderKeyResult.InsertionKey, provider.ProcessKey((char)Keys.Return));
		}		
	}
}
