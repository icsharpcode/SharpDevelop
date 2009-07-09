// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows.Forms;
using ICSharpCode.SharpDevelop.Editor.CodeCompletion;
using ICSharpCode.XmlEditor;
using NUnit.Framework;
using XmlEditor.Tests.Utils;

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
			XmlSchemaCompletionData schema = new XmlSchemaCompletionData(ResourceManager.GetXhtmlStrictSchema());
			XmlSchemaCompletionDataCollection schemas = new XmlSchemaCompletionDataCollection();
			schemas.Add(schema);
			list = new XmlCompletionDataProvider(schemas, schema, "").GenerateCompletionData("", '<');
		}
		
		/// <summary>
		/// A space character should be treated as a normal character.
		/// </summary>
		[Test]
		public void SpaceChar()
		{
			Assert.AreEqual(CompletionItemListKeyResult.NormalKey, list.ProcessInput(' '));
		}
		
		[Test]
		public void TabChar()
		{
			Assert.AreEqual(CompletionItemListKeyResult.InsertionKey, list.ProcessInput('\t'));
		}		

		[Test]
		public void ReturnChar()
		{
			Assert.AreEqual(CompletionItemListKeyResult.InsertionKey, list.ProcessInput((char)Keys.Return));
		}		
	}
}
