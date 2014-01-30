// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Xml;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.WinForms;

namespace ICSharpCode.XmlEditor
{
	/// <summary>
	/// This user control holds both the XmlTreeViewControl and the
	/// attributes property grid in a split container. This is separate from
	/// the XmlTreeView class so we can use the forms designer to design this control.
	/// </summary>
	public class XmlTreeViewContainerControl : System.Windows.Forms.UserControl, IXmlTreeView, IOwnerState, IClipboardHandler
	{
		XmlTreeEditor editor;
		bool dirty;
		bool errorMessageTextBoxVisible;
		bool attributesGridVisible = true;
		
		[Flags]
		internal enum XmlTreeViewContainerControlStates {
			None                = 0,
			ElementSelected     = 1,
			RootElementSelected = 2,
			AttributeSelected   = 4,
			TextNodeSelected    = 8,
			CommentSelected     = 16
		}
		
		public event EventHandler DirtyChanged;
		
		public XmlTreeViewContainerControl()
			: this(new XmlSchemaCompletionCollection(), null)
		{
		}
		
		public XmlTreeViewContainerControl(XmlSchemaCompletionCollection schemas, XmlSchemaCompletion defaultSchema)
		{
			InitializeComponent();
			InitImages();
			editor = new XmlTreeEditor(this, schemas, defaultSchema);
		}
		
		/// <summary>
		/// Gets the current XmlTreeViewContainerControlState.
		/// </summary>
		public Enum InternalState {
			get {
				XmlTreeViewContainerControlStates state = XmlTreeViewContainerControlStates.None;
				if (SelectedElement != null) {
					state |= XmlTreeViewContainerControlStates.ElementSelected;
					if (SelectedElement == Document.DocumentElement) {
						state |= XmlTreeViewContainerControlStates.RootElementSelected;
					}
				}
				if (SelectedAttribute != null) {
					state |= XmlTreeViewContainerControlStates.AttributeSelected;
				}
				if (SelectedTextNode != null) {
					state = XmlTreeViewContainerControlStates.TextNodeSelected;
				}
				if (SelectedComment != null) {
					state = XmlTreeViewContainerControlStates.CommentSelected;
				}
				return state;
			}
		}
		
		/// <summary>
		/// Gets the property grid that displays attributes for the
		/// selected xml element.
		/// </summary>
		public PropertyGrid AttributesGrid {
			get { return attributesGrid; }
		}
		
		/// <summary>
		/// Gets or sets whether the xml document needs saving.
		/// </summary>
		public bool IsDirty {
			get { return dirty; }
			set {
				bool previousDirty = dirty;
				dirty = value;
				OnXmlChanged(previousDirty);
			}
		}
		
		/// <summary>
		/// Gets or sets the error message to display.
		/// </summary>
		public string ErrorMessage {
			get { return errorMessageTextBox.Text; }
			set { errorMessageTextBox.Text = value; }
		}
		
		/// <summary>
		/// Gets or sets whether the error message is visible. When visible the
		/// error message text box replaces the property grid.
		/// </summary>
		public bool IsErrorMessageTextBoxVisible {
			get { return errorMessageTextBoxVisible; }
			set {
				errorMessageTextBoxVisible = value;
				if (value) {
					errorMessageTextBox.BringToFront();
					errorMessageTextBox.TabStop = true;;
					IsAttributesGridVisible = false;
					IsTextBoxVisible = false;
				} else {
					errorMessageTextBox.SendToBack();
					errorMessageTextBox.TabStop = false;
				}
			}
		}
		
		/// <summary>
		/// Gets the XmlTreeView in the container.
		/// </summary>
		public XmlTreeViewControl TreeView {
			get { return xmlElementTreeView; }
		}
		
		public void ShowXmlIsNotWellFormedMessage(XmlException ex)
		{
			ShowErrorMessage(ex.Message);
		}
		
		public void ShowErrorMessage(string message)
		{
			xmlElementTreeView.Clear();
			ErrorMessage = message;
			IsErrorMessageTextBoxVisible = true;
		}
		
		public void LoadXml(string xml)
		{
			textBox.Clear();
			IsAttributesGridVisible = true;
			ClearAttributes();
			
			editor.LoadXml(xml);
			
			ExpandRootDocumentElementNode();
		}
		
		void ExpandRootDocumentElementNode()
		{
			if (xmlElementTreeView.Nodes.Count > 0) {
				xmlElementTreeView.Nodes[0].Expand();
			}	
		}
		
		/// <summary>
		/// Gets or sets the xml document to be shown in this
		/// container control.
		/// </summary>
		public XmlDocument Document {
			get { return editor.Document; }
			set { xmlElementTreeView.Document = value; }
		}
		
		/// <summary>
		/// Shows the attributes.
		/// </summary>
		public void ShowAttributes(XmlAttributeCollection attributes)
		{
			IsAttributesGridVisible = true;
			attributesGrid.SelectedObject = new XmlAttributeTypeDescriptor(attributes);
		}
		
		/// <summary>
		/// Clears all the attributes currently on display.
		/// </summary>
		public void ClearAttributes()
		{
			attributesGrid.SelectedObject = null;
		}
		
		/// <summary>
		/// Shows the xml element's text content after the user has
		/// selected the text node.
		/// </summary>
		public void ShowTextContent(string text)
		{
			IsTextBoxVisible = true;
			textBox.Text = text;
		}
		
		/// <summary>
		/// Gets or sets the text of the text node or 
		/// comment node currently on display.
		/// </summary>
		public string TextContent {
			get { return textBox.Text.Replace("\n", "\r\n"); }
			set { textBox.Text = value; }
		}
		
		/// <summary>
		/// Gets the currently selected node based on what is selected in 
		/// the tree. This does not return the selected attribute.
		/// </summary>
		public XmlNode SelectedNode {
			get {
				XmlElement selectedElement = SelectedElement;
				if (selectedElement != null) {
					return selectedElement;
				}
				
				XmlText selectedTextNode = SelectedTextNode;
				if (selectedTextNode != null) {
					return selectedTextNode;
				}
				
				return SelectedComment;
			}
		}
		
		/// <summary>
		/// Gets the element currently selected.
		/// </summary>
		public XmlElement SelectedElement {
			get { return xmlElementTreeView.SelectedElement; }
		}
		
		/// <summary>
		/// Gets the text node currently selected.
		/// </summary>
		public XmlText SelectedTextNode {
			get { return xmlElementTreeView.SelectedTextNode; }
		}
		
		/// <summary>
		/// Gets the comment node currently selected.
		/// </summary>
		public XmlComment SelectedComment {
			get { return xmlElementTreeView.SelectedComment; }
		}
		
		/// <summary>
		/// Gets the name of the attribute currently selected.
		/// </summary>
		public string SelectedAttribute {
			get {
				GridItem gridItem = attributesGrid.SelectedGridItem;
				if (IsAttributesGridVisible && gridItem != null && gridItem.PropertyDescriptor != null) {
					return gridItem.PropertyDescriptor.Name;
				}
				return null;
			}
		}
		
		/// <summary>
		/// Shows the add attribute dialog so the user can add a new
		/// attribute to the XML tree.
		/// </summary>
		public void AddAttribute()
		{
			editor.AddAttribute();
		}
		
		/// <summary>
		/// Shows the add attribute dialog so the user can choose one or more
		/// new attributes to be added to the selected element.
		/// </summary>
		/// <param name="attributes">The list of attributes the user
		/// can choose from.</param>
		/// <returns>The attributes selected by the user.</returns>
		public string[] SelectNewAttributes(string[] attributes)
		{
			using (IAddXmlNodeDialog addAttributeDialog = CreateAddAttributeDialog(attributes)) {
				if (addAttributeDialog.ShowDialog() == DialogResult.OK) {
					return addAttributeDialog.GetNames();
				}
				return new string[0];
			}
		}
		
		/// <summary>
		/// Removes the currently selected attribute.
		/// </summary>
		public void RemoveAttribute()
		{
			editor.RemoveAttribute();
		}
		
		/// <summary>
		/// Shows the add element dialog so the user can choose one or more
		/// new elements to be added to the selected element.
		/// </summary>
		/// <param name="attributes">The list of elements the user
		/// can choose from.</param>
		/// <returns>The elements selected by the user.</returns>
		public string[] SelectNewElements(string[] elements)
		{
			using (IAddXmlNodeDialog addElementDialog = CreateAddElementDialog(elements)) {
				if (addElementDialog.ShowDialog() == DialogResult.OK) {
					return addElementDialog.GetNames();
				}
				return new string[0];
			}
		}
		
		/// <summary>
		/// Appends a new child element to the currently selected element.
		/// </summary>
		public void AppendChildElement(XmlElement element)
		{
			xmlElementTreeView.AppendChildElement(element);
		}
		
		/// <summary>
		/// Adds a new child element to the selected element.
		/// </summary>
		public void AddChildElement()
		{
			editor.AppendChildElement();
		}
		
		/// <summary>
		/// Inserts an element before the currently selected element.
		/// </summary>
		public void InsertElementBefore()
		{
			editor.InsertElementBefore();
		}

		/// <summary>
		/// Inserts the specified element before the currently selected
		/// element.
		/// </summary>
		public void InsertElementBefore(XmlElement element)
		{
			xmlElementTreeView.InsertElementBefore(element);
		}
		
		/// <summary>
		/// Inserts an element after the currently selected element.
		/// </summary>
		public void InsertElementAfter()
		{
			editor.InsertElementAfter();
		}
		
		/// <summary>
		/// Inserts the specified element after the currently selected
		/// element.
		/// </summary>
		public void InsertElementAfter(XmlElement element)
		{
			xmlElementTreeView.InsertElementAfter(element);
		}
		
		/// <summary>
		/// Removes the specified element from the tree.
		/// </summary>
		public void RemoveElement(XmlElement element)
		{
			xmlElementTreeView.RemoveElement(element);
		}
		
		/// <summary>
		/// Appends a new text node to the currently selected
		/// element.
		/// </summary>
		public void AppendChildTextNode(XmlText textNode)
		{
			xmlElementTreeView.AppendChildTextNode(textNode);			
		}
		
		/// <summary>
		/// Appends a new text node to the currently selected
		/// element.
		/// </summary>		
		public void AppendChildTextNode()
		{
			editor.AppendChildTextNode();
		}
		
		/// <summary>
		/// Inserts a new text node before the currently selected
		/// node.
		/// </summary>
		public void InsertTextNodeBefore()
		{
			editor.InsertTextNodeBefore();
		}
		
		/// <summary>
		/// Inserts a new text node before the currently selected
		/// node.
		/// </summary>
		public void InsertTextNodeBefore(XmlText textNode)
		{
			xmlElementTreeView.InsertTextNodeBefore(textNode);
		}
		
		/// <summary>
		/// Inserts a new text node after the currently selected
		/// node.
		/// </summary>
		public void InsertTextNodeAfter()
		{
			editor.InsertTextNodeAfter();
		}
		
		/// <summary>
		/// Inserts a new text node after the currently selected
		/// node.
		/// </summary>
		public void InsertTextNodeAfter(XmlText textNode)
		{
			xmlElementTreeView.InsertTextNodeAfter(textNode);
		}
		
		/// <summary>
		/// Removes the currently selected text node.
		/// </summary>
		public void RemoveTextNode(XmlText textNode)
		{
			xmlElementTreeView.RemoveTextNode(textNode);
		}
		
		/// <summary>
		/// Updates the corresponding tree node's text.
		/// </summary>
		public void UpdateTextNode(XmlText textNode)
		{
			xmlElementTreeView.UpdateTextNode(textNode);
		}
		
		/// <summary>
		/// Updates the corresponding tree node's text.
		/// </summary>
		public void UpdateComment(XmlComment comment)
		{
			xmlElementTreeView.UpdateComment(comment);
		}
		
		/// <summary>
		/// Appends a new child comment node to the currently selected
		/// element.
		/// </summary>
		public void AppendChildComment(XmlComment comment)
		{
			xmlElementTreeView.AppendChildComment(comment);
		}
		
		/// <summary>
		/// Appends a new child comment node to the currently selected
		/// element.
		/// </summary>
		public void AppendChildComment()
		{
			editor.AppendChildComment();
		}
		
		/// <summary>
		/// Removes the specified xml comment from the tree.
		/// </summary>
		public void RemoveComment(XmlComment comment)
		{
			xmlElementTreeView.RemoveComment(comment);
		}
		
		/// <summary>
		/// Inserts the comment before the currently selected node.
		/// </summary>
		public void InsertCommentBefore(XmlComment comment)
		{
			xmlElementTreeView.InsertCommentBefore(comment);
		}
		
		/// <summary>
		/// Inserts a comment before the currently selected node.
		/// </summary>
		public void InsertCommentBefore()
		{
			editor.InsertCommentBefore();
		}
		
		/// <summary>
		/// Inserts the comment after the currently selected node.
		/// </summary>
		public void InsertCommentAfter(XmlComment comment)
		{
			xmlElementTreeView.InsertCommentAfter(comment);
		}
		
		/// <summary>
		/// Inserts a comment after the currently selected node.
		/// </summary>
		public void InsertCommentAfter()
		{
			editor.InsertCommentAfter();
		}
		
		/// <summary>
		/// Updates the view to show that the specified node is going
		/// to be cut.
		/// </summary>
		public void ShowCut(XmlNode node)
		{
			xmlElementTreeView.ShowCut(node);
		}
		
		/// <summary>
		/// Updates the view so that the specified node is not displayed
		/// as being cut.
		/// </summary>
		public void HideCut(XmlNode node)
		{
			xmlElementTreeView.HideCut(node);
		}

		#region IClipboardHandler implementation

		/// <summary>
		/// Gets whether cutting is enabled.
		/// </summary>
		public bool EnableCut {
			get { return editor.IsCutEnabled; }
		}
		
		/// <summary>
		/// Gets whether copying is enabled.
		/// </summary>
		public bool EnableCopy {
			get { return editor.IsCopyEnabled; }
		}
		
		/// <summary>
		/// Gets whether pasting is enabled.
		/// </summary>
		public bool EnablePaste {
			get { return editor.IsPasteEnabled; }
		}
		
		/// <summary>
		/// Gets whether deleting is enabled.
		/// </summary>
		public bool EnableDelete {
			get { return editor.IsDeleteEnabled; }
		}
		
		/// <summary>
		/// Currently not possible to select all tree nodes so this
		/// always returns false.
		/// </summary>
		public bool EnableSelectAll {
			get { return false; }
		}
		
		/// <summary>
		/// Cuts the selected tree node.
		/// </summary>
		public void Cut()
		{
			editor.Cut();
		}
		
		/// <summary>
		/// Copies the selected tree node.
		/// </summary>
		public void Copy()
		{
			editor.Copy();
		}
		
		/// <summary>
		/// Pastes the selected tree node.
		/// </summary>
		public void Paste()
		{
			editor.Paste();
		}
		
		/// <summary>
		/// Deletes the selected tree node.
		/// </summary>
		public void Delete()
		{
			editor.Delete();
		}
		
		/// <summary>
		/// Selects all tree nodes. Currently not supported.
		/// </summary>
		public void SelectAll()
		{
		}
		
		#endregion
		
		/// <summary>
		/// Disposes resources used by the control.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing) {
				if (components != null) {
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}
		
		/// <summary>
		/// Creates a new AddElementDialog.
		/// </summary>
		/// <param name="elementNames">The element names to be listed in the
		/// dialog.</param>
		protected virtual IAddXmlNodeDialog CreateAddElementDialog(string[] elementNames)
		{
			AddXmlNodeDialog dialog = new AddXmlNodeDialog(elementNames);
			dialog.Text = StringParser.Parse("${res:ICSharpCode.XmlEditor.AddElementDialog.Title}");
			dialog.CustomNameLabelText = StringParser.Parse("${res:ICSharpCode.XmlEditor.AddElementDialog.CustomElementLabel}");
			return dialog;
		}
		
		/// <summary>
		/// Creates a new AddAttributeDialog.
		/// </summary>
		/// <param name="attributeNames">The attribute names to be listed in the
		/// dialog.</param>
		protected virtual IAddXmlNodeDialog CreateAddAttributeDialog(string[] attributeNames)
		{
			AddXmlNodeDialog dialog = new AddXmlNodeDialog(attributeNames);
			dialog.Text = StringParser.Parse("${res:ICSharpCode.XmlEditor.AddAttributeDialog.Title}");
			dialog.CustomNameLabelText = StringParser.Parse("${res:ICSharpCode.XmlEditor.AddAttributeDialog.CustomAttributeLabel}");
			return dialog;
		}
		
		/// <summary>
		/// Deletes the selected node.
		/// </summary>
		protected void XmlElementTreeViewDeleteKeyPressed(object source, EventArgs e)
		{
			Delete();
		}
		
		#region Forms Designer generated code

		/// <summary>
		/// Designer variable used to keep track of non-visual components.
		/// </summary>
		System.ComponentModel.IContainer components = null;

		/// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor. The Forms designer might
		/// not be able to load this method if it was changed manually.
		/// </summary>
		void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			ICSharpCode.SharpDevelop.Gui.ExtTreeViewComparer extTreeViewComparer1 = new ICSharpCode.SharpDevelop.Gui.ExtTreeViewComparer();
			this.splitContainer = new System.Windows.Forms.SplitContainer();
			this.xmlElementTreeView = new ICSharpCode.XmlEditor.XmlTreeViewControl();
			this.attributesGrid = new System.Windows.Forms.PropertyGrid();
			this.errorMessageTextBox = new System.Windows.Forms.RichTextBox();
			this.textBox = new System.Windows.Forms.RichTextBox();
			this.splitContainer.Panel1.SuspendLayout();
			this.splitContainer.Panel2.SuspendLayout();
			this.splitContainer.SuspendLayout();
			this.SuspendLayout();
			// 
			// splitContainer
			// 
			this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer.Location = new System.Drawing.Point(0, 0);
			this.splitContainer.Name = "splitContainer";
			// 
			// splitContainer.Panel1
			// 
			this.splitContainer.Panel1.Controls.Add(this.xmlElementTreeView);
			// 
			// splitContainer.Panel2
			// 
			this.splitContainer.Panel2.Controls.Add(this.attributesGrid);
			this.splitContainer.Panel2.Controls.Add(this.errorMessageTextBox);
			this.splitContainer.Panel2.Controls.Add(this.textBox);
			this.splitContainer.Size = new System.Drawing.Size(562, 326);
			this.splitContainer.SplitterDistance = 185;
			this.splitContainer.SplitterWidth = 2;
			this.splitContainer.TabIndex = 0;
			this.splitContainer.TabStop = false;
			// 
			// xmlElementTreeView
			// 
			this.xmlElementTreeView.AllowDrop = true;
			this.xmlElementTreeView.CanClearSelection = true;
			this.xmlElementTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
			this.xmlElementTreeView.Document = null;
			this.xmlElementTreeView.DrawMode = System.Windows.Forms.TreeViewDrawMode.OwnerDrawText;
			this.xmlElementTreeView.HideSelection = false;
			this.xmlElementTreeView.ImageIndex = 0;
			this.xmlElementTreeView.IsSorted = false;
			this.xmlElementTreeView.Location = new System.Drawing.Point(0, 0);
			this.xmlElementTreeView.Name = "xmlElementTreeView";
			this.xmlElementTreeView.NodeSorter = extTreeViewComparer1;
			this.xmlElementTreeView.SelectedImageIndex = 0;
			this.xmlElementTreeView.Size = new System.Drawing.Size(185, 326);
			this.xmlElementTreeView.TabIndex = 0;
			this.xmlElementTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.XmlElementTreeViewAfterSelect);
			this.xmlElementTreeView.DeleteKeyPressed += new System.EventHandler(this.XmlElementTreeViewDeleteKeyPressed);
			// 
			// attributesGrid
			// 
			this.attributesGrid.Dock = System.Windows.Forms.DockStyle.Fill;
			this.attributesGrid.HelpVisible = false;
			this.attributesGrid.Location = new System.Drawing.Point(0, 0);
			this.attributesGrid.Name = "attributesGrid";
			this.attributesGrid.PropertySort = System.Windows.Forms.PropertySort.Alphabetical;
			this.attributesGrid.Size = new System.Drawing.Size(375, 326);
			this.attributesGrid.TabIndex = 1;
			this.attributesGrid.ToolbarVisible = false;
			this.attributesGrid.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.AttributesGridPropertyValueChanged);
			// 
			// errorMessageTextBox
			// 
			this.errorMessageTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this.errorMessageTextBox.Location = new System.Drawing.Point(0, 0);
			this.errorMessageTextBox.Name = "errorMessageTextBox";
			this.errorMessageTextBox.Size = new System.Drawing.Size(375, 326);
			this.errorMessageTextBox.TabIndex = 0;
			this.errorMessageTextBox.TabStop = false;
			this.errorMessageTextBox.Text = "";
			// 
			// textBox
			// 
			this.textBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this.textBox.Location = new System.Drawing.Point(0, 0);
			this.textBox.Multiline = true;
			this.textBox.Name = "textBox";
			this.textBox.Size = new System.Drawing.Size(375, 326);
			this.textBox.TabIndex = 2;
			this.textBox.TabStop = false;
			this.textBox.Text = "";
			this.textBox.TextChanged += new System.EventHandler(this.TextBoxTextChanged);
			// 
			// XmlTreeViewContainerControl
			// 
			this.Controls.Add(this.splitContainer);
			this.Name = "XmlTreeViewContainerControl";
			this.Size = new System.Drawing.Size(562, 326);
			this.splitContainer.Panel1.ResumeLayout(false);
			this.splitContainer.Panel2.ResumeLayout(false);
			this.splitContainer.ResumeLayout(false);
			this.ResumeLayout(false);
		}
		private System.Windows.Forms.RichTextBox textBox;
		private System.Windows.Forms.PropertyGrid attributesGrid;
		private System.Windows.Forms.RichTextBox errorMessageTextBox;
		private ICSharpCode.XmlEditor.XmlTreeViewControl xmlElementTreeView;
		private System.Windows.Forms.SplitContainer splitContainer;
		
		#endregion

		/// <summary>
		/// This method is protected only so we can easily test
		/// what happens when this method is called. Triggering
		/// a TextChanged event is difficult to do from unit tests.
		/// You can trigger it it by setting the textBox's Rtf property.
		/// </summary>
		protected void TextBoxTextChanged(object sender, EventArgs e)
		{
			if (editor != null) {
				bool previousIsDirty = dirty;
				editor.TextContentChanged();
				OnXmlChanged(previousIsDirty);
			}
		}
		
		/// <summary>
		/// This method is protected so we can test it.
		/// </summary>
		protected void XmlElementTreeViewAfterSelect(object sender, TreeViewEventArgs e)
		{
			editor.SelectedNodeChanged();
		}
		
		/// <summary>
		/// This method is protected so we can test it.
		/// </summary>
		protected void AttributesGridPropertyValueChanged(object sender, PropertyValueChangedEventArgs e)
		{
			bool previousIsDirty = dirty;
			editor.AttributeValueChanged();
			OnXmlChanged(previousIsDirty);
		}
				
		/// <summary>
		/// Creates an image list that will be used for the XmlTreeViewControl.
		/// </summary>
		void InitImages()
		{
			if (components == null) {
				components = new Container();
			}
			ImageList images = new ImageList(components);
			
			// Add xml element tree node images.
			Image xmlElementImage = Image.FromStream(typeof(XmlTreeViewContainerControl).Assembly.GetManifestResourceStream("ICSharpCode.XmlEditor.Resources.XmlElementTreeNodeIcon.png"));
			images.Images.Add(XmlElementTreeNode.XmlElementTreeNodeImageKey, xmlElementImage);
			images.Images.Add(XmlElementTreeNode.XmlElementTreeNodeGhostImageKey, IconService.GetGhostBitmap(new Bitmap(xmlElementImage)));
			
			// Add text tree node images.
			Image xmlTextImage = Image.FromStream(typeof(XmlTreeViewContainerControl).Assembly.GetManifestResourceStream("ICSharpCode.XmlEditor.Resources.XmlTextTreeNodeIcon.png"));
			images.Images.Add(XmlTextTreeNode.XmlTextTreeNodeImageKey, xmlTextImage);
			images.Images.Add(XmlTextTreeNode.XmlTextTreeNodeGhostImageKey, IconService.GetGhostBitmap(new Bitmap(xmlTextImage)));
			
			// Add comment tree node images.
			Image xmlCommentImage = Image.FromStream(typeof(XmlTreeViewContainerControl).Assembly.GetManifestResourceStream("ICSharpCode.XmlEditor.Resources.XmlCommentTreeNodeIcon.png"));
			images.Images.Add(XmlCommentTreeNode.XmlCommentTreeNodeImageKey, xmlCommentImage);
			images.Images.Add(XmlCommentTreeNode.XmlCommentTreeNodeGhostImageKey, IconService.GetGhostBitmap(new Bitmap(xmlCommentImage)));

			xmlElementTreeView.ImageList = images;
		}
		
		/// <summary>
		/// Raises the dirty changed event if the dirty flag has changed.
		/// </summary>
		void OnXmlChanged(bool previousIsDirty)
		{
			if (previousIsDirty != dirty) {
				OnDirtyChanged();
			}
		}
		
		/// <summary>
		/// Raises the DirtyChanged event.
		/// </summary>
		void OnDirtyChanged()
		{
			if (DirtyChanged != null) {
				DirtyChanged(this, new EventArgs());
			}
		}
		
		/// <summary>
		/// Gets or sets whether the attributes grid is visible.
		/// </summary>
		bool IsAttributesGridVisible {
			get {
				return attributesGridVisible;
			}
			set {
				attributesGridVisible = value;
				if (value) {
					attributesGrid.BringToFront();
					attributesGrid.TabStop = true;
					IsTextBoxVisible = false;
					IsErrorMessageTextBoxVisible = false;
				} else {
					attributesGrid.SendToBack();
					attributesGrid.TabStop = false;
				}
			}
		}
		
		/// <summary>
		/// Gets or sets whether the text node text box is visible.
		/// </summary>
		bool IsTextBoxVisible {
			set {
				if (value) {
					textBox.BringToFront();
					textBox.TabStop = true;
					IsAttributesGridVisible = false;
					IsErrorMessageTextBoxVisible = false;
				} else {
					textBox.SendToBack();
					textBox.TabStop = false;
				}
			}
		}
	}
}
