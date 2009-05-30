// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision: 2164 $</version>
// </file>

using System;
using System.IO;
using System.Windows.Forms;
using System.Xml;

using ICSharpCode.XmlEditor;
using NUnit.Framework;
using XmlEditor.Tests.Utils;

namespace XmlEditor.Tests.Tree
{
	/// <summary>
	/// Tests the various menu commands that can be used on the
	/// Xml Tree (e.g. InsertElementBeforeCommand).
	/// </summary>
	[TestFixture]
	public class MenuCommandsTestFixture
	{
		DerivedXmlTreeViewContainerControl treeViewContainer;
		XmlDocument doc;
		XmlTreeViewControl treeView;
		XmlElement bodyElement;
		XmlElementTreeNode htmlTreeNode;
		XmlElementTreeNode bodyTreeNode;
		
		[SetUp]
		public void Init()
		{
			treeViewContainer = new DerivedXmlTreeViewContainerControl();
							
			XmlTextReader reader = ResourceManager.GetXhtmlStrictSchema();
			XmlSchemaCompletionData xhtmlSchema = new XmlSchemaCompletionData(reader);
			XmlSchemaCompletionDataCollection schemas = new XmlSchemaCompletionDataCollection();
			XmlCompletionDataProvider provider = new XmlCompletionDataProvider(schemas, xhtmlSchema, String.Empty);
			
			treeViewContainer.LoadXml("<html><body></body></html>", provider);
			doc = treeViewContainer.Document;
			treeView = treeViewContainer.TreeView;
			
			htmlTreeNode = (XmlElementTreeNode)treeView.Nodes[0];
			htmlTreeNode.Expanding();
			bodyTreeNode = (XmlElementTreeNode)htmlTreeNode.Nodes[0];
			
			bodyElement = (XmlElement)doc.SelectSingleNode("/html/body");
		}
		
		[TearDown]
		public void TearDown()
		{
			if (treeViewContainer != null) {
				treeViewContainer.Dispose();
			}
		}
		
		/// <summary>
		/// Nothing should happen if the owner is the XmlTreeViewContainerControl.
		/// </summary>
		[Test]
		public void InsertElementBeforeWithUnknownOwner()
		{
			InsertElementBeforeCommand command = new InsertElementBeforeCommand();
			command.Owner = this;
			command.Run();
		}
		
		/// <summary>
		/// Nothing should happen if the owner is null.
		/// </summary>
		[Test]
		public void InsertElementBeforeWithNullOwner()
		{
			InsertElementBeforeCommand command = new InsertElementBeforeCommand();
			command.Run();
		}
		
		/// <summary>
		/// Here we produce dummy data from our mock AddElementDialog
		/// and check that the InsertElementBeforeCommand inserts
		/// the correct element
		/// </summary>
		[Test]
		public void InsertElementBefore()
		{
			treeView.SelectedNode = bodyTreeNode;
			treeViewContainer.AddElementDialogElementNamesReturned.Add("head");
			InsertElementBeforeCommand command = new InsertElementBeforeCommand();
			command.Owner = treeViewContainer;
			command.Run();
			
			XmlElement headElement = (XmlElement)doc.SelectSingleNode("/html/head");
			Assert.IsNotNull(headElement, "Expected a new head element to be inserted.");
			Assert.AreSame(headElement, doc.DocumentElement.FirstChild);
		}
		
		/// <summary>
		/// Nothing should happen if the owner is null.
		/// </summary>
		[Test]
		public void InsertElementAfterWithNullOwner()
		{
			InsertElementAfterCommand command = new InsertElementAfterCommand();
			command.Run();
		}
		
		[Test]
		public void InsertElementAfter()
		{
			treeView.SelectedNode = bodyTreeNode;
			treeViewContainer.AddElementDialogElementNamesReturned.Add("afterBody");
			InsertElementAfterCommand command = new InsertElementAfterCommand();
			command.Owner = treeViewContainer;
			command.Run();
			
			XmlElement newElement = (XmlElement)doc.SelectSingleNode("/html/afterBody");
			Assert.IsNotNull(newElement, "Expected a new element to be inserted.");
			Assert.AreSame(newElement, doc.DocumentElement.ChildNodes[1]);
		}
		
		/// <summary>
		/// Nothing should happen if the owner is null.
		/// </summary>
		[Test]
		public void AddChildElementWithNullOwner()
		{
			AddChildElementCommand command = new AddChildElementCommand();
			command.Run();
		}
		
		[Test]
		public void AddChildElement()
		{
			treeView.SelectedNode = bodyTreeNode;
			treeViewContainer.AddElementDialogElementNamesReturned.Add("p");
			AddChildElementCommand command = new AddChildElementCommand();
			command.Owner = treeViewContainer;
			command.Run();
			
			XmlElement paragraphElement = (XmlElement)bodyElement.SelectSingleNode("p");
			Assert.IsNotNull(paragraphElement, "Expected a new <p> element to be appended.");
		}
		
		/// <summary>
		/// Tests the XmlTreeViewContainer.SelectNewElements methods
		/// returns an empty string array when the AddElementDialog
		/// is cancelled.
		/// </summary>
		[Test]
		public void AddElementDialogCancelled()
		{
			treeView.SelectedNode = bodyTreeNode;
			treeViewContainer.AddElementDialogElementNamesReturned.Add("p");
			treeViewContainer.AddElementDialogResult = DialogResult.Cancel;
		
			string[] elements = treeViewContainer.SelectNewElements(new string[] {"a"});
			Assert.AreEqual(0, elements.Length);
		}
		
		/// <summary>
		/// Nothing should happen if the owner is null.
		/// </summary>
		[Test]
		public void AddAttributeWithNullOwner()
		{
			AddAttributeCommand command = new AddAttributeCommand();
			command.Run();
		}
		
		[Test]
		public void AddAttribute()
		{
			treeView.SelectedNode = bodyTreeNode;
			treeViewContainer.AddAttributeDialogAttributeNamesReturned.Add("class");
			AddAttributeCommand command = new AddAttributeCommand();
			command.Owner = treeViewContainer;
			command.Run();
			
			Assert.IsTrue(bodyElement.HasAttribute("class"));
		}
		
		/// <summary>
		/// Nothing should happen if the owner is null.
		/// </summary>
		[Test]
		public void RemoveAttributeWithNullOwner()
		{
			RemoveAttributeCommand command = new RemoveAttributeCommand();
			command.Run();
		}
		
		[Test]
		public void RemoveAttribute()
		{
			AddAttribute();
			
			treeView.SelectedNode = bodyTreeNode;
			treeViewContainer.ShowAttributes(treeView.SelectedElement.Attributes);
			
			Assert.IsNotNull(treeViewContainer.AttributesGrid.SelectedGridItem,
				"Sanity check - should have a grid item selected.");
			
			RemoveAttributeCommand command = new RemoveAttributeCommand();
			command.Owner = treeViewContainer;
			command.Run();

			Assert.IsFalse(bodyElement.HasAttribute("class"));
		}
		
		/// <summary>
		/// Tests the XmlTreeViewContainer.SelectNewAttributes methods
		/// returns an empty string array when the AddAttributeDialog
		/// is cancelled.
		/// </summary>
		[Test]
		public void AddAttributeDialogCancelled()
		{
			treeView.SelectedNode = bodyTreeNode;
			treeView.SelectedNode = bodyTreeNode;
			treeViewContainer.AddAttributeDialogAttributeNamesReturned.Add("class");
			treeViewContainer.AddAttributeDialogResult = DialogResult.Cancel;
			AddAttributeCommand command = new AddAttributeCommand();
			
			string[] attributes = treeViewContainer.SelectNewAttributes(new string[] {"a"});
			Assert.AreEqual(0, attributes.Length);
		}
		
		/// <summary>
		/// Nothing should happen if the owner is null.
		/// </summary>
		[Test]
		public void AddChildTextNodeWithNullOwner()
		{
			AddChildTextNodeCommand command = new AddChildTextNodeCommand();
			command.Run();
		}
		
		[Test]
		public void AddChildTextNode()
		{
			treeView.SelectedNode = bodyTreeNode;
			AddChildTextNodeCommand command = new AddChildTextNodeCommand();
			command.Owner = treeViewContainer;
			command.Run();
			
			XmlText textNode = bodyElement.SelectSingleNode("text()") as XmlText;
			Assert.IsNotNull(textNode, "Expected a new text node element to be appended.");
			
			XmlTextTreeNode textTreeNode = bodyTreeNode.Nodes[0] as XmlTextTreeNode;
			Assert.IsNotNull(textTreeNode);
		}
		
		/// <summary>
		/// Makes sure that nothing happens if we try to add a text
		/// node to the currently selected element node when there
		/// is no selected element node.
		/// </summary>
		[Test]
		public void AddChildTextNodeWhenNoElementSelected()
		{
			treeView.SelectedNode = null;
			XmlText newTextNode = doc.CreateTextNode(String.Empty);
			treeView.AppendChildTextNode(newTextNode);
			
			Assert.IsFalse(treeViewContainer.IsDirty);
		}
		
		/// <summary>
		/// Nothing should happen if the owner is null.
		/// </summary>
		[Test]
		public void InsertTextNodeBeforeWithNullOwner()
		{
			InsertTextNodeBeforeCommand command = new InsertTextNodeBeforeCommand();
			command.Run();
		}
		
		[Test]
		public void InsertTextNodeBefore()
		{
			AddChildTextNode();
			
			XmlText textNode = bodyElement.SelectSingleNode("text()") as XmlText;
			textNode.Value = "Original";
			
			XmlTextTreeNode textTreeNode = bodyTreeNode.Nodes[0] as XmlTextTreeNode;
			treeView.SelectedNode = textTreeNode;
			InsertTextNodeBeforeCommand command = new InsertTextNodeBeforeCommand();
			command.Owner = treeViewContainer;
			command.Run();
						
			XmlTextTreeNode insertedTextTreeNode = bodyTreeNode.Nodes[0] as XmlTextTreeNode;
			XmlText insertedTextNode = bodyElement.FirstChild as XmlText;
			Assert.IsNotNull(insertedTextTreeNode);
			Assert.AreEqual(2, bodyTreeNode.Nodes.Count);
			Assert.AreEqual(String.Empty, insertedTextTreeNode.XmlText.Value);
			Assert.IsNotNull(insertedTextNode);
			Assert.AreEqual(2, bodyElement.ChildNodes.Count);
			Assert.AreEqual(String.Empty, insertedTextNode.Value);
		}
		
		/// <summary>
		/// Makes sure that nothing happens if we try to add a text
		/// node when there is no currently selected node.
		/// </summary>
		[Test]
		public void InsertTextNodeWhenNoTreeNodeSelected()
		{
			treeView.SelectedNode = null;
			XmlText newTextNode = doc.CreateTextNode(String.Empty);
			treeView.InsertTextNodeBefore(newTextNode);
			
			XmlText textNode = bodyElement.SelectSingleNode("text()") as XmlText;
			Assert.IsNull(textNode);
		}
		
		[Test]
		public void InsertTextNodeAfterWithNullOwner()
		{
			InsertTextNodeAfterCommand command = new InsertTextNodeAfterCommand();
			command.Run();
		}
		
		[Test]
		public void InsertTextNodeAfter()
		{
			AddChildTextNode();
			
			XmlText textNode = bodyElement.SelectSingleNode("text()") as XmlText;
			textNode.Value = "OriginalTextNode";
			
			XmlTextTreeNode textTreeNode = bodyTreeNode.Nodes[0] as XmlTextTreeNode;
			treeView.SelectedNode = textTreeNode;
			InsertTextNodeAfterCommand command = new InsertTextNodeAfterCommand();
			command.Owner = treeViewContainer;
			command.Run();
						
			XmlTextTreeNode insertedTextTreeNode = bodyTreeNode.LastNode as XmlTextTreeNode;
			XmlText insertedTextNode = bodyElement.LastChild as XmlText;
			Assert.IsNotNull(insertedTextTreeNode);
			Assert.AreEqual(2, bodyTreeNode.Nodes.Count);
			Assert.AreEqual(String.Empty, insertedTextTreeNode.XmlText.Value);
			Assert.IsNotNull(insertedTextNode);
			Assert.AreEqual(2, bodyElement.ChildNodes.Count);
			Assert.AreEqual(String.Empty, insertedTextNode.Value);
		}
		
		[Test]
		public void RemoveTextNode()
		{
			AddChildTextNode();
			treeView.SelectedNode = bodyTreeNode.Nodes[0];
			treeViewContainer.Delete();

			Assert.IsFalse(bodyElement.HasChildNodes);
			Assert.AreEqual(0, bodyTreeNode.Nodes.Count);
		}
		
		[Test]
		public void AddChildCommentNodeWithNullOwner()
		{
			AddChildCommentCommand command = new AddChildCommentCommand();
			command.Run();
		}
		
		[Test]
		public void AddChildCommentNode()
		{
			treeView.SelectedNode = bodyTreeNode;
			AddChildCommentCommand command = new AddChildCommentCommand();
			command.Owner = treeViewContainer;
			command.Run();
			
			XmlComment comment = bodyElement.SelectSingleNode("comment()") as XmlComment;
			Assert.IsTrue(bodyElement.HasChildNodes);
			Assert.IsNotNull(comment, "Expected a new comment node to be appended.");
			
			XmlCommentTreeNode treeNode = bodyTreeNode.Nodes[0] as XmlCommentTreeNode;
			Assert.IsNotNull(treeNode);
		}
		
		/// <summary>
		/// Removes the selected comment using the delete command.
		/// </summary>
		[Test]
		public void RemoveComment()
		{
			AddChildCommentNode();
			treeView.SelectedNode = bodyTreeNode.Nodes[0];
			treeViewContainer.Delete();

			Assert.IsFalse(bodyElement.HasChildNodes);
			Assert.AreEqual(0, bodyTreeNode.Nodes.Count);
		}
		
		[Test]
		public void InsertCommentBeforeWithNullOwner()
		{
			InsertCommentBeforeCommand command = new InsertCommentBeforeCommand();
			command.Run();
		}
		
		[Test]
		public void InsertCommentAfterWithNullOwner()
		{
			InsertCommentAfterCommand command = new InsertCommentAfterCommand();
			command.Run();
		}
		
		[Test]
		public void InsertCommentBefore()
		{
			treeView.SelectedNode = bodyTreeNode;
			InsertCommentBeforeCommand command = new InsertCommentBeforeCommand();
			command.Owner = treeViewContainer;
			command.Run();
			
			XmlComment comment = bodyElement.PreviousSibling as XmlComment;
			Assert.IsNotNull(comment, "Expected a new comment node to be inserted.");
			
			XmlCommentTreeNode treeNode = bodyTreeNode.PrevNode as XmlCommentTreeNode;
			Assert.IsNotNull(treeNode);
			Assert.AreSame(comment, treeNode.XmlComment);
		}
		
		[Test]
		public void InsertCommentAfter()
		{
			treeView.SelectedNode = bodyTreeNode;
			InsertCommentAfterCommand command = new InsertCommentAfterCommand();
			command.Owner = treeViewContainer;
			command.Run();
			
			XmlComment comment = bodyElement.NextSibling as XmlComment;
			Assert.IsNotNull(comment, "Expected a new comment node to be inserted.");
			
			XmlCommentTreeNode treeNode = bodyTreeNode.NextNode as XmlCommentTreeNode;
			Assert.IsNotNull(treeNode);
			Assert.AreSame(comment, treeNode.XmlComment);
		}
	}
}
