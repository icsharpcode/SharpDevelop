// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision: 2760 $</version>
// </file>

using ICSharpCode.SharpDevelop.Editor;
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
		ICompletionItemList list;
		
		[SetUp]
		public void Init()
		{
			XmlSchemaCompletionDataCollection schemas = new XmlSchemaCompletionDataCollection();
			list = new XmlCompletionDataProvider(schemas, null, null).GenerateCompletionData("", '<');
		}
		
		/// <summary>
		/// A space character should be treated as a normal character.
		/// </summary>
		[Test]
		public void SpaceChar()
		{
			Assert.AreEqual(CompletionDataProviderKeyResult.NormalKey, list.ProcessInput(' '));
		}
		
		[Test]
		public void TabChar()
		{
			Assert.AreEqual(CompletionDataProviderKeyResult.InsertionKey, list.ProcessInput('\t'));
		}		

		[Test]
		public void ReturnChar()
		{
			Assert.AreEqual(CompletionDataProviderKeyResult.InsertionKey, list.ProcessInput((char)Keys.Return));
		}		
	}
}
