// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Xml;

using ICSharpCode.Core;

namespace ICSharpCode.XmlEditor
{
	/// <summary>
	/// This user control holds both the XmlTreeViewControl and the
	/// attributes property grid in a split container. This is separate from
	/// the XmlTreeView class so we can use the forms designer to design this control.
	/// </summary>
	public class XmlTreeViewContainerControl : System.Windows.Forms.UserControl, IXmlTreeView, IOwnerState
	{
		XmlTreeEditor editor;
		bool dirty;
		bool errorMessageTextBoxVisible;
		bool attributesGridVisible = true;
		bool textBoxVisible;
		
		[Flags]
		public enum XmlTreeViewContainerControlState {
			Nothing             = 0,
			ElementSelected     = 1,
			RootElementSelected = 2,
			AttributeSelected   = 4
		}
		
		public event EventHandler DirtyChanged;
		
		public XmlTreeViewContainerControl()
		{
			InitializeComponent();
			InitImages();
		}
		
		public Enum InternalState {
			get {
				XmlTreeViewContainerControlState state = XmlTreeViewContainerControlState.Nothing;
				if (SelectedElement != null) {
					state |= XmlTreeViewContainerControlState.ElementSelected;
					if (SelectedElement == DocumentElement) {
						state |= XmlTreeViewContainerControlState.RootElementSelected;
					}
				}
				if (SelectedAttribute != null) {
					state |= XmlTreeViewContainerControlState.AttributeSelected;
				}
				return state;
			}
		}
		
		public PropertyGrid AttributesGrid {
			get {
				return attributesGrid;
			}
		}
		
		/// <summary>
		/// Gets or sets whether the xml document needs saving.
		/// </summary>
		public bool IsDirty {
			get {
				return dirty;
			}
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
			get {
				return errorMessageTextBox.Text;
			}
			set {
				errorMessageTextBox.Text = value;
			}
		}
		
		/// <summary>
		/// Gets or sets whether the error message is visible. When visible the
		/// error message text box replaces the property grid.
		/// </summary>
		public bool IsErrorMessageTextBoxVisible {
			get {
				return errorMessageTextBoxVisible;
			}
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
		
		public XmlTreeViewControl TreeView {
			get {
				return xmlElementTreeView;
			}
		}
		
		public void ShowXmlIsNotWellFormedMessage(XmlException ex)
		{
			xmlElementTreeView.Clear();
			ErrorMessage = ex.Message;
			IsErrorMessageTextBoxVisible = true;
		}
		
		public XmlElement DocumentElement {
			get {
				return xmlElementTreeView.DocumentElement;
			}
			set {
				xmlElementTreeView.DocumentElement = value;
			}
		}
		
		/// <summary>
		/// Displays the specified xml as a tree.
		/// </summary>
		public void LoadXml(string xml, XmlCompletionDataProvider completionDataProvider)
		{
			textBox.Clear();
			IsAttributesGridVisible = true;
			ClearAttributes();
			
			editor = new XmlTreeEditor(this, completionDataProvider);
			editor.LoadXml(xml);
			
			// Expand document element node.
			if (xmlElementTreeView.Nodes.Count > 0) {
				xmlElementTreeView.Nodes[0].Expand();
			}
		}
		
		/// <summary>
		/// Gets the xml document created from the loaded Xml.
		/// </summary>
		public XmlDocument Document {
			get {
				return editor.Document;
			}
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
		/// Gets the text node text currently on display.
		/// </summary>
		public string TextContent {
			get {
				return textBox.Text;
			}
		}
		
		/// <summary>
		/// Gets the element currently selected.
		/// </summary>
		public XmlElement SelectedElement {
			get {
				return xmlElementTreeView.SelectedElement;
			}
		}
		
		/// <summary>
		/// Gets the element text node currently selected.
		/// </summary>
		public XmlText SelectedTextNode {
			get {
				return xmlElementTreeView.SelectedTextNode;
			}
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
			using (AddAttributeDialog addAttributeDialog = new AddAttributeDialog(attributes)) {
				if (addAttributeDialog.ShowDialog() == DialogResult.OK) {
					return addAttributeDialog.AttributeNames;
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
			using (AddElementDialog addElementDialog = new AddElementDialog(elements)) {
				if (addElementDialog.ShowDialog() == DialogResult.OK) {
					return addElementDialog.ElementNames;
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
			editor.AddChildElement();
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
			this.xmlElementTreeView.DocumentElement = null;
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
		/// Creates an image list that will be used for the XmlTreeViewControl.
		/// </summary>
		void InitImages()
		{
			if (components == null) {
				components = new Container();
			}
			ImageList images = new ImageList(components);
			Image xmlElementImage = Image.FromStream(GetType().Assembly.GetManifestResourceStream("ICSharpCode.XmlEditor.Resources.XmlElementTreeNodeIcon.png"));
			images.Images.Add(XmlElementTreeNode.XmlElementTreeNodeImageKey, xmlElementImage);
			Image xmlTextImage = Image.FromStream(GetType().Assembly.GetManifestResourceStream("ICSharpCode.XmlEditor.Resources.XmlTextTreeNodeIcon.png"));
			images.Images.Add(XmlTextTreeNode.XmlTextTreeNodeImageKey, xmlTextImage);
			xmlElementTreeView.ImageList = images;
		}
		
		void XmlElementTreeViewAfterSelect(object sender, TreeViewEventArgs e)
		{
			if (xmlElementTreeView.IsTextNodeSelected) {
				editor.SelectedTextNodeChanged();
			} else {
				editor.SelectedElementChanged();
			}
		}
		
		void TextBoxTextChanged(object sender, EventArgs e)
		{
			bool previousIsDirty = dirty;
			editor.TextContentChanged();
			OnXmlChanged(previousIsDirty);
		}
		
		void AttributesGridPropertyValueChanged(object s, PropertyValueChangedEventArgs e)
		{
			bool previousIsDirty = dirty;
			editor.AttributeValueChanged();
			OnXmlChanged(previousIsDirty);
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
			get {
				return textBoxVisible;
			}
			set {
				textBoxVisible = value;
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
