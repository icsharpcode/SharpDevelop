// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml;

using ICSharpCode.Core;
using ICSharpCode.XmlEditor;

namespace ICSharpCode.WixBinding
{
	/// <summary>
	/// Creates or loads a dialog from a Wix XML document.
	/// </summary>
	public class WixDialog
	{
		WixBinaries binaries;
		XmlElement dialogElement;
		WixDocument document;
		Button acceptButton;
		Button cancelButton;
		static Nullable<double> installerUnit;
		const int lineHeight = 2;
		WixNamespaceManager namespaceManager;
		List<PictureBox> pictureBoxesAdded = new List<PictureBox>();
		
		/// <summary>
		/// Default component creator class. This is defined so the 
		/// </summary>
		class ComponentCreator : IComponentCreator
		{
			public IComponent CreateComponent(Type componentClass, string name)
			{
				object createdObject = componentClass.Assembly.CreateInstance(componentClass.FullName);
				IComponent component = (IComponent)createdObject;
				return component;
			}
		}
		
		/// <summary>
		/// Creates a new instance of the Wix Dialog class.
		/// </summary>
		/// <param name="dialogElement">The dialog XML element loaded from
		/// the Wix document</param>
		public WixDialog(WixDocument document, XmlElement dialogElement) 
			: this(document, dialogElement, null)
		{
		}
		
		/// <summary>
		/// Creates a new instance of the Wix Dialog class.
		/// </summary>
		/// <param name="dialogElement">The dialog XML element loaded from
		/// the Wix document</param>
		public WixDialog(WixDocument document, XmlElement dialogElement, WixBinaries binaries)
		{
			this.document = document;
			this.dialogElement = dialogElement;
			this.binaries = binaries;
			namespaceManager = new WixNamespaceManager(dialogElement.OwnerDocument.NameTable);
		}

		/// <summary>
		/// Gets the Windows Installer User Interface unit. This is the factor that 
		/// is used to convert dialog widths and heights specified in the Wix document
		/// to pixels.
		/// </summary>
		/// <remarks>
		/// According to the Windows Installer documentation 
		/// (http://msdn.microsoft.com/library/en-us/msi/setup/installer_units.asp)
		/// this factor "is approximately equal to one-twelfth (1/12) the height of 
		/// the 10-point MS Sans Serif font size."</remarks>
		public static double InstallerUnit {
			get {
				if (!installerUnit.HasValue) {
					installerUnit = GetInstallerUnit();
				}
				return installerUnit.Value;
			}
		}
		
		/// <summary>
		/// Converts the specified template unit (width or height) into pixels.
		/// </summary>
		public static int ConvertTemplateUnitsToPixels(int templateUnits)
		{
			return Convert.ToInt32(templateUnits * InstallerUnit);
		}
		
		/// <summary>
		/// Converts the specified pixels to template units.
		/// </summary>
		public static int ConvertPixelsToTemplateUnits(int pixels)
		{
			return Convert.ToInt32(pixels / InstallerUnit);
		}
		
		/// <summary>
		/// Gets the Wix control type for the specified WinForm control.
		/// </summary>
		/// <returns><see langword="null"/> if no match is found.</returns>
		public static string GetControlTypeName(Type controlType)
		{
			if (controlType == typeof(Button)) {
				return "PushButton";
			} else if (controlType == typeof(TextBox)) {
				return "Edit";
			} else if (controlType == typeof(Label)) {
				return "Text";
			} else if (controlType == typeof(CheckBox)) {
				return "CheckBox";
			} else if (controlType == typeof(RichTextBox)) {
				return "ScrollableText";
			} else if (controlType == typeof(ComboBox)) {
				return "ComboBox";
			} else if (controlType == typeof(GroupBox)) {
				return "GroupBox";
			} else if (controlType == typeof(PictureBox)) {
				return "Bitmap";
			} else if (controlType == typeof(ListBox)) {
				return "ListBox";
			} else if (controlType == typeof(ListView)) {
				return "ListView";
			} else if (controlType == typeof(ProgressBar)) {
				return "ProgressBar";
			} else if (controlType == typeof(MaskedTextBox)) {
				return "MaskedEdit";
			} else if (controlType == typeof(TreeView)) {
				return "SelectionTree";
			} else if (controlType == typeof(RadioButtonGroupBox)) {
				return "RadioButtonGroup";
			} else {
				return null;
			}
		}
		
		/// <summary>
		/// Creates a dialog based on the dialog XML element.
		/// </summary>
		public Form CreateDialog()
		{
			return CreateDialog(new ComponentCreator());
		}
		
		/// <summary>
		/// Creates a dialog based on the dialog XML element.
		/// </summary>
		/// <param name="componentCreator">The object that will be used to create
		/// all components. This can be used to link the dialog creation with
		/// a forms designer.</param>
		public Form CreateDialog(IComponentCreator componentCreator)
		{
			if (componentCreator == null) {
				throw new ArgumentNullException("componentCreator");
			}
			
			Form dialog = CreateForm(componentCreator);
			
			foreach (Control control in CreateControls(componentCreator)) {
				dialog.Controls.Add(control);
			}
			
			// Add accept button.
			if (acceptButton != null) {
				dialog.AcceptButton = acceptButton;
			}
			
			// Add cancel button.
			if (cancelButton != null) {
				dialog.CancelButton = cancelButton;
			}
			
			// Make sure bitmaps appear behind all other controls.
			foreach (PictureBox pictureBox in pictureBoxesAdded) {
				pictureBox.SendToBack();
			}
			
			return dialog;
		}
		
		/// <summary>
		/// Updates the dialog XmlElement with any changes that have been made to the
		/// dialog.
		/// </summary>
		/// <param name="dialog">The modified dialog.</param>
		/// <returns>The modified dialog XmlElement.</returns>
		public XmlElement UpdateDialogElement(Form dialog)
		{
			UpdateDialogAttributes(dialog);
			
			acceptButton = (Button)dialog.AcceptButton;
			cancelButton = (Button)dialog.CancelButton;
			
			List<string> controlIds = UpdateControls(dialog.Controls);
			RemoveMissingControls(controlIds);

			return dialogElement;
		}
		
		/// <summary>
		/// Gets the Windows Installer User Interface Unit.
		/// </summary>
		/// <remarks>
		/// This is the size of a 10 point MS Sans Serif font divided by the
		/// magic number 12.</remarks>
		static double GetInstallerUnit()
		{
			using (Font msSansSerifFont = new Font("MS Sans Serif", 10)) {
				return msSansSerifFont.Height / 12.0;
			}
		}
		
		/// <summary>
		/// Gets the size in pixels based on the information in the 
		/// Wix element. 
		/// </summary>
		/// <param name="element">
		/// Either a dialog element or a control element. Both of these use
		/// width and height attributes.
		/// </param>
		static Size GetControlSize(XmlElement element)
		{
			int templateWidth = GetAttributeAsInteger(element, "Width");
			int templateHeight = GetAttributeAsInteger(element, "Height");
			
			int width = ConvertTemplateUnitsToPixels(templateWidth);
			int height = ConvertTemplateUnitsToPixels(templateHeight);

			return new Size(width, height);
		}
		
		/// <summary>
		/// Gets the attribute value as an integer.
		/// </summary>
		/// <param name="name">The name of the attribute.</param>
		static int GetAttributeAsInteger(XmlElement element, string name)
		{
			XmlAttribute attribute = element.Attributes[name];
			if (attribute == null) {
				string message = String.Format(StringParser.Parse("${res:ICSharpCode.WixBinding.WixDialog.RequiredAttributeMissingMessage}"), name);
				throw new WixDialogException(message, element.Name, element.GetAttribute("Id"));
			}
			int attributeValue;
			if (!Int32.TryParse(attribute.Value, out attributeValue)) {
				string message = String.Format(StringParser.Parse("${res:ICSharpCode.WixBinding.WixDialog.IllegalAttributeIntegerValue}"), name, attribute.Value);
				throw new WixDialogException(message, element.Name, element.GetAttribute("Id"));
			}
			return attributeValue;
		}
		
		/// <summary>
		/// Creates a form from the dialog element and sets its properties, but 
		/// does not add any controls to it.
		/// </summary>
		/// <remarks>
		/// We set the ClientSize of the form rather than the Size since the Height
		/// and Width attributes seem to refer to the client area height and width.
		/// </remarks>
		Form CreateForm(IComponentCreator componentCreator)
		{
			string dialogId = dialogElement.GetAttribute("Id");
			IComponent formComponent = componentCreator.CreateComponent(typeof(Form), dialogId);
			Form dialog = (Form)formComponent;
			dialog.Name = dialogId;
			dialog.Text = dialogElement.GetAttribute("Title");
			dialog.FormBorderStyle = FormBorderStyle.FixedDialog;
			dialog.ClientSize = GetControlSize(dialogElement);
			dialog.MaximizeBox = false;
			dialog.MinimizeBox = IsMinimizeButtonEnabled(dialogElement);
			
			return dialog;
		}
		
		/// <summary>
		/// Creates a list of controls that need to be added to the dialog.
		/// </summary>
		List<Control> CreateControls(IComponentCreator componentCreator)
		{
			List<Control> controls = new List<Control>();
			foreach (XmlElement controlElement in dialogElement.SelectNodes("w:Control", namespaceManager)) {
				switch (controlElement.GetAttribute("Type")) {
					case "PushButton":
						controls.Add(CreateButton(controlElement, componentCreator));
						break;
					case "Text":
						controls.Add(CreateLabel(controlElement, componentCreator));
						break;
					case "Line":
						controls.Add(CreateLine(controlElement, componentCreator));
						break;
					case "CheckBox":
						controls.Add(CreateControl(typeof(CheckBox), controlElement, componentCreator));
						break;
					case "ScrollableText":
						controls.Add(CreateControl(typeof(RichTextBox), controlElement, componentCreator));
						break;
					case "ComboBox":
						controls.Add(CreateComboBox(controlElement, componentCreator));
						break;
					case "Edit":
					case "PathEdit":
						controls.Add(CreateControl(typeof(TextBox), controlElement, componentCreator));
						break;
					case "GroupBox":
						controls.Add(CreateControl(typeof(GroupBox), controlElement, componentCreator));
						break;
					case "RadioButtonGroup":
						controls.Add(CreateRadioButtonGroup(controlElement, componentCreator));
						break;
					case "Bitmap":
					case "Icon":
						controls.Add(CreatePictureBox(controlElement, componentCreator));
						break;
					case "ListBox":
						controls.Add(CreateListBox(controlElement, componentCreator));
						break;
					case "DirectoryList":
						controls.Add(CreateControl(typeof(ListBox), controlElement, componentCreator));
						break;
					case "ListView":
						controls.Add(CreateListView(controlElement, componentCreator));
						break;
					case "ProgressBar":
						controls.Add(CreateControl(typeof(ProgressBar), controlElement, componentCreator));
						break;
					case "MaskedEdit":
						controls.Add(CreateControl(typeof(MaskedTextBox), controlElement, componentCreator));
						break;
					case "SelectionTree":
						controls.Add(CreateControl(typeof(TreeView), controlElement, componentCreator));
						break;
				}
			}
			return controls;
		}
		
		/// <summary>
		/// Creates a control of the given type. This is a common routine that creates 
		/// and sets up various control properties that the Wix Control element defines and
		/// are common across control types.
		/// </summary>
		/// <param name="type">The type of control to create (e.g. Button)</param>
		/// <param name="controlElement">The Wix Control element.</param>
		/// <param name="componentCreator">The object to use to create the control.</param>
		Control CreateControl(Type type, XmlElement controlElement, IComponentCreator componentCreator)
		{
			string name = controlElement.GetAttribute("Id");
			return CreateControl(type, controlElement, componentCreator, name);
		}

		/// <summary>
		/// Creates a control of the given type. This is a common routine that creates 
		/// and sets up various control properties that the Wix Control element defines and
		/// are common across control types.
		/// </summary>
		/// <param name="type">The type of control to create (e.g. Button)</param>
		/// <param name="controlElement">The Wix Control element.</param>
		/// <param name="componentCreator">The object to use to create the control.</param>
		/// <param name="name">The name of the component to create.</param>
		Control CreateControl(Type type, XmlElement controlElement, IComponentCreator componentCreator, string name)
		{
			Control control = (Control)componentCreator.CreateComponent(type, name);
			
			// Disabling controls does not seem to work. In the designer they are still
			// enabled. The XML forms designer has the same problem even back as far as
			// SharpDevelop 1.1
			control.Enabled = !IsDisabled(controlElement);
			control.Name = name;
			control.Text = GetControlText(controlElement);
			control.Location = GetControlLocation(controlElement);
			control.Size = GetControlSize(controlElement);
			control.Font = GetFont(control.Text, control.Font);
			
			return control;
		}
		
		/// <summary>
		/// Creates a new button control based on the xml element and also sets the
		/// stores the accept button and cancel button if it is defined.
		/// </summary>
		Button CreateButton(XmlElement controlElement, IComponentCreator componentCreator)
		{
			Button button = (Button)CreateControl(typeof(Button), controlElement, componentCreator);
			if (IsAcceptButton(controlElement)) {
				acceptButton = button;
			} else if (IsCancelButton(controlElement)) {
				cancelButton = button;
			}
			return button;
		}
		
		/// <summary>
		/// Creates a horizontal line based on the Control xml element.
		/// </summary>
		Label CreateLine(XmlElement controlElement, IComponentCreator componentCreator)
		{
			Label label = (Label)CreateControl(typeof(Label), controlElement, componentCreator);
			label.BorderStyle = BorderStyle.Fixed3D;
			label.Height = ConvertTemplateUnitsToPixels(lineHeight);
			return label;
		}
		
		/// <summary>
		/// Creates a picture box containing a bitmap.
		/// </summary>
		PictureBox CreatePictureBox(XmlElement controlElement, IComponentCreator componentCreator)
		{
			PictureBox pictureBox = (PictureBox)CreateControl(typeof(PictureBox), controlElement, componentCreator);
			pictureBox.Image = GetBitmapFromId(controlElement.GetAttribute("Text"));
			pictureBox.SizeMode = PictureBoxSizeMode.CenterImage;
			pictureBoxesAdded.Add(pictureBox);
			return pictureBox;
		}
		
		/// <summary>
		/// Creates a label.
		/// </summary>
		Label CreateLabel(XmlElement controlElement, IComponentCreator componentCreator)
		{
			Label label = (Label)CreateControl(typeof(Label), controlElement, componentCreator);
			
			if (IsYes(controlElement.GetAttribute("Transparent"))) {
				// Setting the alpha to 0 does not work, the labels are not made transparent.
				label.BackColor = Color.FromArgb(0, label.BackColor);
			}
			return label;
		}
		
		/// <summary>
		/// Creates a radio button group.
		/// </summary>
		RadioButtonGroupBox CreateRadioButtonGroup(XmlElement controlElement, IComponentCreator componentCreator)
		{
			// Create radio button group box.
			RadioButtonGroupBox radioButtonGroup = (RadioButtonGroupBox)CreateControl(typeof(RadioButtonGroupBox), controlElement, componentCreator);
			string property = controlElement.GetAttribute("Property");
			radioButtonGroup.PropertyName = property;
			
			// Add radio buttons.
			int radioButtonCount = 0;
			foreach (XmlElement radioButtonElement in GetRadioButtonElements(property)) {
				++radioButtonCount;
				string radioButtonName = String.Concat(property, "RadioButton", radioButtonCount.ToString());
				radioButtonGroup.Controls.Add(CreateRadioButton(radioButtonElement, componentCreator, radioButtonName));
			}
			return radioButtonGroup;
		}
		
		/// <summary>
		/// Gets the radio button elements for the specified radio button group.
		/// </summary>
		XmlNodeList GetRadioButtonElements(string property)
		{
			string xpath = String.Concat("//w:RadioButtonGroup[@Property='", XmlEncode(property), "']/w:RadioButton");
			return dialogElement.SelectNodes(xpath, namespaceManager);
		}
		
		/// <summary>
		/// Creates a radio button control.
		/// </summary>
		RadioButton CreateRadioButton(XmlElement radioButtonElement, IComponentCreator componentCreator, string name)
		{
			RadioButton radioButton = (RadioButton)CreateControl(typeof(RadioButton), radioButtonElement, componentCreator, name);
			return radioButton;
		}
		
		/// <summary>
		/// Gets the control location from the control element.
		/// </summary>
		static Point GetControlLocation(XmlElement controlElement)
		{
			int templateX = GetAttributeAsInteger(controlElement, "X");
			int templateY = GetAttributeAsInteger(controlElement, "Y");
			
			int x = ConvertTemplateUnitsToPixels(templateX);
			int y = ConvertTemplateUnitsToPixels(templateY);

			return new Point(x, y);
		}
		
		/// <summary>
		/// Checks that the control element specifies the dialog's accept button.
		/// </summary>
		static bool IsAcceptButton(XmlElement controlElement)
		{
			return IsYes(controlElement.GetAttribute("Default"));
		}
		
		/// <summary>
		/// Checks that the control element specifies the dialog's cancel button.
		/// </summary>
		static bool IsCancelButton(XmlElement controlElement)
		{
			return IsYes(controlElement.GetAttribute("Cancel"));
		}
		
		/// <summary>
		/// Determines whether the control should be disabled.
		/// </summary>
		static bool IsDisabled(XmlElement controlElement)
		{
			return IsYes(controlElement.GetAttribute("Disabled"));
		}
		
		/// <summary>
		/// Checks that the YesNoType string contains yes.
		/// </summary>
		static bool IsYes(string s)
		{
			return s == "yes";
		}
		
		/// <summary>
		/// Gets the text to be used for the control. If the Control has a child
		/// Text element then this is used for the text, otherwise the Control's 
		/// Text attribute is used.
		/// </summary>
		string GetControlText(XmlElement controlElement)
		{
			XmlElement textElement = (XmlElement)controlElement.SelectSingleNode("w:Text", namespaceManager);
			if (textElement != null) {
				return textElement.InnerText;
			}
			return controlElement.GetAttribute("Text");
		}
		
		/// <summary>
		/// Determines whether the dialog minimize box should be disabled.
		/// </summary>
		static bool IsMinimizeButtonEnabled(XmlElement dialogElement)
		{
			return !IsYes(dialogElement.GetAttribute("NoMinimize"));
		}
		
		/// <summary>
		/// Looks for font information in the specified string and returns a new 
		/// font based on this information, otherwise it returns the default font.
		/// </summary>
		/// <remarks>
		/// A text font is typically indicated by having a marker at the beginning of the
		/// text string which is of the form {\VerdanaBold10}. Sometimes the backslash is 
		/// replaced with the ampersand xml marker. These markers map to TextStyle elements
		/// in the Wix document that give you the actual font information.
		/// </remarks>
		Font GetFont(string text, Font defaultFont)
		{
			// Property references a text style?
			string textStyleName = String.Empty;
			Match propertyMatch = Regex.Match(text, @"^\[(.*?)\]");
			if (propertyMatch.Success) {
				string propertyName = propertyMatch.Groups[1].Value;
				string propertyValue = document.GetProperty(propertyName);
				if (propertyValue.Length > 0) {
					textStyleName = ParseTextStyleName(propertyValue);
				}
			}
			
			// Font defined by text style?
			if (textStyleName.Length == 0) {
				textStyleName = ParseTextStyleName(text);
			}
			
			// Default UI font style set?
			if (textStyleName.Length == 0) {
				textStyleName = GetDefaultUIFontStyle();
			}
			
			// Look for text style.
			if (textStyleName.Length > 0) {
				return GetTextStyleFont(textStyleName, defaultFont);
			}
			
			return defaultFont;
		}
		
		/// <summary>
		/// Parses the text style reference.
		/// </summary>
		/// <remarks>
		/// A text style is of the form {\VerdanaBold10} or {&amp;VerdanaBold10}.
		/// </remarks>
		/// <returns>Returns the text style without the surround brackets and
		/// backslash or ampersand.</returns>
		static string ParseTextStyleName(string text)
		{
			Match m = Regex.Match(text, @"^{[&|\\](.*?)}");
			if (m.Success) {
				return m.Groups[1].Value;
			}
			return String.Empty;
		}
		
		/// <summary>
		/// Looks up the font information for the specified text style name
		/// in the Wix document associated with the dialog element.
		/// </summary>
		Font GetTextStyleFont(string name, Font defaultFont)
		{
			string xpath = String.Concat("//w:TextStyle[@Id='", XmlEncode(name), "']");
			XmlElement textStyleElement = (XmlElement)dialogElement.SelectSingleNode(xpath, namespaceManager);
			if (textStyleElement != null) {
				return CreateTextStyleFont(textStyleElement);
			}
			return defaultFont;
		}
		
		/// <summary>
		/// Creates a Font from the text style element.
		/// </summary>
		static Font CreateTextStyleFont(XmlElement textStyleElement)
		{
			string name = textStyleElement.GetAttribute("FaceName");
			float size = float.Parse(textStyleElement.GetAttribute("Size"));
			FontStyle style = GetFontStyle(textStyleElement);                         
			return new Font(name, size, style);
		}
		
		/// <summary>
		/// Gets the font style from the text style element.
		/// </summary>
		static FontStyle GetFontStyle(XmlElement textStyleElement)
		{
			if (IsYes(textStyleElement.GetAttribute("Bold"))) {
				return FontStyle.Bold;
			}
			return FontStyle.Regular;
		}
		
		/// <summary>
		/// Gets the font style specified in the DefaultUIFont property.
		/// </summary>
		string GetDefaultUIFontStyle()
		{
			XmlElement propertyElement = (XmlElement)dialogElement.SelectSingleNode("//w:Property[@Id='DefaultUIFont']", namespaceManager);
			if (propertyElement != null) {
				return propertyElement.InnerText;
			}
			return String.Empty;
		}
		
		/// <summary>
		/// Updates the xml elements for the current state of the controls.
		/// </summary>
		/// <returns>A list of the control ids that have been updated.</returns>
		List<string> UpdateControls(Control.ControlCollection controls)
		{
			Point location = new Point(0, 0);
			return UpdateChildControls(location, controls);
		}
		
		/// <summary>
		/// Updates the xml elements for the current state of the child controls.
		/// </summary>
		/// <param name="parentLocation">Location used to offset the contained 
		/// controls. Controls that are added to the dialog xml use a location
		/// relative to the dialog control not the parent.</param>
		/// <returns>A list of the control ids that have been updated.</returns>
		List<string> UpdateChildControls(Point parentLocation, Control.ControlCollection controls)
		{
			List<string> controlIds = new List<string>();
			
			foreach (Control control in controls) {
				controlIds.Add(control.Name);
				if (control is Button) {
					UpdateButtonElement(parentLocation, (Button)control);
				} else if (control is RadioButtonGroupBox) {
					UpdateRadioButtonGroupElement(parentLocation, (RadioButtonGroupBox)control);
				} else if (control is ListBox) {
					UpdateListBoxElement(parentLocation, (ListBox)control);
				} else if (control is ComboBox) {
					UpdateComboBoxElement(parentLocation, (ComboBox)control);
				} else if (control is ListView) {
					UpdateListViewElement(parentLocation, (ListView)control);
				} else if (control is GroupBox) {
					controlIds.AddRange(UpdateGroupBoxElement(parentLocation, (GroupBox)control));
				} else {					
					UpdateControlElement(parentLocation, control);
				}
			}
			
			return controlIds;			
		}
		
		/// <summary>
		/// Modifies the dialog element's attributes due to changes in the 
		/// dialog.
		/// </summary>
		void UpdateDialogAttributes(Form dialog)
		{
			dialogElement.SetAttribute("Title", dialog.Text);
			int height = ConvertPixelsToTemplateUnits(dialog.ClientSize.Height);
			dialogElement.SetAttribute("Height", height.ToString());
			int width = ConvertPixelsToTemplateUnits(dialog.ClientSize.Width);
			dialogElement.SetAttribute("Width", width.ToString());
			
			if (dialog.MinimizeBox) {
				if (dialogElement.HasAttribute("NoMinimize")) {
					dialogElement.RemoveAttribute("NoMinimize");
				}
			} else {
				dialogElement.SetAttribute("NoMinimize", "yes");
			}
		}
		
		/// <summary>
		/// Updates the associated control element with the control's new settings.
		/// The location of the control is offset using the parent's location.
		/// </summary>
		XmlElement UpdateControlElement(Point parentLocation, Control control)
		{
			XmlElement controlElement = UpdateControlElement(control);
			if (controlElement != null) {
				Point location = new Point(parentLocation.X + control.Location.X, parentLocation.Y + control.Location.Y);
				UpdateElementLocation(controlElement, location);
			}
			return controlElement;
		}
		
		/// <summary>
		/// Updates the associated control element with the control's new settings.
		/// </summary>
		/// <remarks>
		/// If the control text is an empty string we must remove the Text attribute
		/// otherwise the Wix compiler will produce errors. If we have a child
		/// Text element however we can just set its inner text to be an empty
		/// string and do not have to remove the element.
		/// </remarks>
		XmlElement UpdateControlElement(Control control)
		{
			string xpath = String.Concat("w:Control[@Id='", control.Name, "']");
			XmlElement controlElement = (XmlElement)dialogElement.SelectSingleNode(xpath, namespaceManager);
			if (controlElement == null) {
				string typeName = GetControlTypeName(control.GetType());
				if (typeName == null) {
					return null;
				}
				controlElement = AppendControlElement(control.Name, typeName);
			}
			
			UpdateElementSize(controlElement, control.Size);
			UpdateElementLocation(controlElement, control.Location);
			UpdateElementText(controlElement, control.Text);
			
			return controlElement;
		}
		
		/// <summary>
		/// Updates the X and Y attributes of the xml element based on the 
		/// new location.
		/// </summary>
		static void UpdateElementLocation(XmlElement controlElement, Point location)
		{
			int x = ConvertPixelsToTemplateUnits(location.X);
			controlElement.SetAttribute("X", x.ToString());
			int y = ConvertPixelsToTemplateUnits(location.Y);
			controlElement.SetAttribute("Y", y.ToString());
		}
		
		/// <summary>
		/// Updates the height and width of the xml element.
		/// </summary>
		static void UpdateElementSize(XmlElement controlElement, Size size)
		{
			int height = ConvertPixelsToTemplateUnits(size.Height);
			controlElement.SetAttribute("Height", height.ToString());
			int width = ConvertPixelsToTemplateUnits(size.Width);
			controlElement.SetAttribute("Width", width.ToString());
		}
		
		/// <summary>
		/// Updates the Text child element if it exists otherwise updates the
		/// Text attribute.
		/// </summary>
		void UpdateElementText(XmlElement controlElement, string text)
		{
			XmlElement textElement = (XmlElement)controlElement.SelectSingleNode("w:Text", namespaceManager);
			if (textElement != null) {
				textElement.InnerText = text;
			} else if (text.Length > 0) {
				// Set text if the control text is not an empty string.
				controlElement.SetAttribute("Text", text);
			} else {
				// Remove the Text attribute.
				controlElement.RemoveAttribute("Text");
			}
		}
		
		/// <summary>
		/// Updates the button xml element's state based on the control.
		/// </summary>
		void UpdateButtonElement(Point parentLocation, Button control)
		{
			XmlElement controlElement = UpdateControlElement(parentLocation, control);
			
			// Set accept button.
			if (acceptButton != null) {
				if (acceptButton == control) {
					controlElement.SetAttribute("Default", "yes");
				}
			} else if (controlElement.HasAttribute("Default")) {
				controlElement.SetAttribute("Default", "no");
			}
			
			// Set cancel button.
			if (cancelButton != null) {
				if (cancelButton == control) {
					controlElement.SetAttribute("Cancel", "yes");
				}
			} else if (controlElement.HasAttribute("Cancel")) {
				controlElement.SetAttribute("Cancel", "no");
			}
		}
		
		/// <summary>
		/// Updates the RadioButtonGroup xml elements.
		/// </summary>
		void UpdateRadioButtonGroupElement(Point parentLocation, RadioButtonGroupBox control)
		{
			XmlElement controlElement = UpdateControlElement(parentLocation, control);
			string property = controlElement.GetAttribute("Property");
			string xpath = String.Concat("//w:RadioButtonGroup[@Property='", XmlEncode(property), "']");
			XmlElement radioButtonGroupElement = (XmlElement)controlElement.SelectSingleNode(xpath, namespaceManager);
			if (radioButtonGroupElement == null) {
				radioButtonGroupElement = AppendChildElement(controlElement, "RadioButtonGroup", control.PropertyName);
			}
			
			int radioButtonControlIndex = 0;
			int radioButtonElementIndex = 0;
			
			XmlNodeList radioButtonElements = radioButtonGroupElement.SelectNodes("w:RadioButton", namespaceManager);
			for (; radioButtonElementIndex < radioButtonElements.Count; ++radioButtonElementIndex) {
				RadioButton radioButton = GetNextRadioButton(control.Controls, ref radioButtonControlIndex);
				if (radioButton == null) {
					break;
				}
				XmlElement radioButtonElement = (XmlElement)radioButtonElements[radioButtonElementIndex];
				UpdateRadioButton(radioButtonElement, radioButton);
				++radioButtonControlIndex;
			}
			
			// Remove radio buttons.
			if (radioButtonElementIndex < radioButtonElements.Count) {
				for (; radioButtonElementIndex < radioButtonElements.Count; ++radioButtonElementIndex) {
					radioButtonGroupElement.RemoveChild(radioButtonElements[radioButtonElementIndex]);
				}
			}
			
			// Add new radio buttons.
			if (radioButtonControlIndex < control.Controls.Count) {
				for (; radioButtonControlIndex < control.Controls.Count; ++radioButtonControlIndex) {	
					RadioButton radioButton = control.Controls[radioButtonControlIndex] as RadioButton;
					if (radioButton != null) {
						XmlElement newRadioButtonElement = AppendChildElement(radioButtonGroupElement, "RadioButton");
						UpdateRadioButton(newRadioButtonElement, radioButton);
					}
				}
			}
		}
		
		/// <summary>
		/// Gets the next radio button in the control collection. Non-radio button controls
		/// are ignored.
		/// </summary>
		/// <param name="index">The current control index to start looking for controls.</param>
		static RadioButton GetNextRadioButton(Control.ControlCollection controls, ref int index)
		{
			while (index < controls.Count) {
				RadioButton radioButton = controls[index] as RadioButton;
				if (radioButton != null) {
					return radioButton;
				}
				++index;
			}
			return null;
		}
		
		/// <summary>
		/// Updates the radio button's xml element based on the new values set in the
		/// control.
		/// </summary>
		void UpdateRadioButton(XmlElement radioButtonElement, RadioButton radioButton)
		{
			UpdateElementLocation(radioButtonElement, radioButton.Location);
			UpdateElementSize(radioButtonElement, radioButton.Size);
			UpdateElementText(radioButtonElement, radioButton.Text);
		}
		
		/// <summary>
		/// Updates the group box xml element and also any controls that have
		/// been added to the group box.
		/// </summary>
		/// <returns>
		/// A list of control ids that are contained inside the group box.
		/// </returns>
		List<string> UpdateGroupBoxElement(Point parentLocation, GroupBox groupBox)
		{
			UpdateControlElement(parentLocation, groupBox);
			Point groupBoxLocation = new Point(parentLocation.X + groupBox.Location.X, parentLocation.Y + groupBox.Location.Y);
			return UpdateChildControls(groupBoxLocation, groupBox.Controls);
		}
		
		/// <summary>
		/// Removes any controls in the control collection if they are missing from the
		/// control ids list.
		/// </summary>
		void RemoveMissingControls(List<string> controlIds)
		{
			List<XmlElement> controlElementsToRemove = new List<XmlElement>();
			
			foreach (XmlElement controlElement in dialogElement.SelectNodes("w:Control", namespaceManager)) {
				string id = controlElement.GetAttribute("Id");
				if (!controlIds.Contains(id)) {
					controlElementsToRemove.Add(controlElement);
				}
			}
			
			foreach (XmlElement controlElementToRemove in controlElementsToRemove) {
				dialogElement.RemoveChild(controlElementToRemove);
			}
		}
		
		/// <summary>
		/// Appends a new control element to the dialog xml. 
		/// </summary>
		XmlElement AppendControlElement(string id, string type)
		{
			XmlElement controlElement = AppendChildElement(dialogElement, "Control");
			controlElement.SetAttribute("Id", id);
			controlElement.SetAttribute("Type", type);
			return controlElement;
		}
		
		/// <summary>
		/// Creates a list box.
		/// </summary>
		ListBox CreateListBox(XmlElement controlElement, IComponentCreator componentCreator)
		{
			// Create list box.
			ListBox listBox = (ListBox)CreateControl(typeof(ListBox), controlElement, componentCreator);

			// Add list box items.
			AddListItems(controlElement, listBox.Items);
			
			return listBox;
		}
		
		/// <summary>
		/// Creates a list view.
		/// </summary>
		ListView CreateListView(XmlElement controlElement, IComponentCreator componentCreator)
		{
			// Create list view.
			ListView listView = (ListView)CreateControl(typeof(ListView), controlElement, componentCreator);
			listView.View = View.List;
			
			// Add list view items.
			AddListItems(controlElement, listView.Items);
			
			return listView;
		}
		
		/// <summary>
		/// Creates a combo box.
		/// </summary>
		ComboBox CreateComboBox(XmlElement controlElement, IComponentCreator componentCreator)
		{
			// Create combo box.
			ComboBox comboBox = (ComboBox)CreateControl(typeof(ComboBox), controlElement, componentCreator);
			comboBox.Text = controlElement.GetAttribute("Property");

			// Add combo box items.
			AddListItems(controlElement, comboBox.Items);
			
			return comboBox;
		}
		
		/// <summary>
		/// Adds ListItems to the control. The control can be a ListView, ComboBox or
		/// ListBox.
		/// </summary>
		void AddListItems(XmlElement controlElement, IList items)
		{
			string property = controlElement.GetAttribute("Property");
			string elementName = controlElement.GetAttribute("Type");

			// Add list items.
			string xpath = String.Concat("//w:", elementName, "[@Property='", XmlEncode(property), "']/w:ListItem");
			foreach (XmlElement itemElement in dialogElement.SelectNodes(xpath, namespaceManager)) {
				items.Add(GetControlText(itemElement));
			}			
		}
		
		/// <summary>
		/// Updates the list box xml element.
		/// </summary>
		void UpdateListBoxElement(Point parentLocation, ListBox control)
		{
			XmlElement controlElement = UpdateControlElement(parentLocation, control);
			string property = controlElement.GetAttribute("Property");
			string xpath = String.Concat("//w:ListBox[@Property='", XmlEncode(property), "']");
			XmlElement listBoxElement = (XmlElement)controlElement.SelectSingleNode(xpath, namespaceManager);
			if (listBoxElement == null) {
				listBoxElement = AppendChildElement(controlElement, "ListBox", property);
			}
			
			int listItemIndex = 0;
			int listItemElementIndex = 0;
			
			XmlNodeList listItemElements = listBoxElement.SelectNodes("w:ListItem", namespaceManager);
			for (; (listItemElementIndex < listItemElements.Count) && (listItemIndex < control.Items.Count); ++listItemElementIndex) {
				XmlElement listItemElement = (XmlElement)listItemElements[listItemElementIndex];
				UpdateElementText(listItemElement, (string)control.Items[listItemIndex]);
				++listItemIndex;
			}
			
			// Remove list items.
			if (listItemElementIndex < listItemElements.Count) {
				for (; listItemElementIndex < listItemElements.Count; ++listItemElementIndex) {
					listBoxElement.RemoveChild(listItemElements[listItemElementIndex]);
				}
			}
			
			// Add new list items.
			if (listItemIndex < control.Items.Count) {
				for (; listItemIndex < control.Items.Count; ++listItemIndex) {	
					XmlElement newListItemElement = AppendChildElement(listBoxElement, "ListItem");
					UpdateElementText(newListItemElement, (string)control.Items[listItemIndex]);
				}
			}
		}
		
		/// <summary>
		/// Updates the combo box xml element.
		/// </summary>
		void UpdateComboBoxElement(Point parentLocation, ComboBox control)
		{
			XmlElement controlElement = UpdateControlElement(parentLocation, control);
			string property = controlElement.GetAttribute("Property");
			string xpath = String.Concat("//w:ComboBox[@Property='", XmlEncode(property), "']");
			XmlElement comboBoxElement = (XmlElement)controlElement.SelectSingleNode(xpath, namespaceManager);
			if (comboBoxElement == null) {
				comboBoxElement = AppendChildElement(controlElement, "ComboBox", property);
			}
			
			int listItemIndex = 0;
			int listItemElementIndex = 0;
			
			XmlNodeList listItemElements = comboBoxElement.SelectNodes("w:ListItem", namespaceManager);
			for (; (listItemElementIndex < listItemElements.Count) && (listItemIndex < control.Items.Count); ++listItemElementIndex) {
				XmlElement listItemElement = (XmlElement)listItemElements[listItemElementIndex];
				UpdateElementText(listItemElement, (string)control.Items[listItemIndex]);
				++listItemIndex;
			}
			
			// Remove list items.
			if (listItemElementIndex < listItemElements.Count) {
				for (; listItemElementIndex < listItemElements.Count; ++listItemElementIndex) {
					comboBoxElement.RemoveChild(listItemElements[listItemElementIndex]);
				}
			}
			
			// Add new list items.
			if (listItemIndex < control.Items.Count) {
				for (; listItemIndex < control.Items.Count; ++listItemIndex) {	
					XmlElement newListItemElement = AppendChildElement(comboBoxElement, "ListItem");
					UpdateElementText(newListItemElement, (string)control.Items[listItemIndex]);
				}
			}
		}
		
		/// <summary>
		/// Updates the list view xml element.
		/// </summary>
		void UpdateListViewElement(Point parentLocation, ListView control)
		{
			XmlElement controlElement = UpdateControlElement(parentLocation, control);
			string property = controlElement.GetAttribute("Property");
			string xpath = String.Concat("//w:ListView[@Property='", XmlEncode(property), "']");
			XmlElement listViewElement = (XmlElement)controlElement.SelectSingleNode(xpath, namespaceManager);
			if (listViewElement == null) {
				listViewElement = AppendChildElement(controlElement, "ListView", property);
			}
			
			int listItemIndex = 0;
			int listItemElementIndex = 0;
			
			XmlNodeList listItemElements = listViewElement.SelectNodes("w:ListItem", namespaceManager);
			for (; (listItemElementIndex < listItemElements.Count) && (listItemIndex < control.Items.Count); ++listItemElementIndex) {
				XmlElement listItemElement = (XmlElement)listItemElements[listItemElementIndex];
				UpdateElementText(listItemElement, control.Items[listItemIndex].Text);
				++listItemIndex;
			}
			
			// Remove list items.
			if (listItemElementIndex < listItemElements.Count) {
				for (; listItemElementIndex < listItemElements.Count; ++listItemElementIndex) {
					listViewElement.RemoveChild(listItemElements[listItemElementIndex]);
				}
			}
			
			// Add new list items.
			if (listItemIndex < control.Items.Count) {
				for (; listItemIndex < control.Items.Count; ++listItemIndex) {	
					XmlElement newListItemElement = AppendChildElement(listViewElement, "ListItem");
					UpdateElementText(newListItemElement, control.Items[listItemIndex].Text);
				}
			}
		}		
		
		/// <summary>
		/// Appends a new child element to the specified parent element and sets its
		/// Property attribute.
		/// </summary>
		XmlElement AppendChildElement(XmlElement parentElement, string name, string property)
		{
			XmlElement element = AppendChildElement(parentElement, name);
			element.SetAttribute("Property", property);
			return element;
		}
		
		/// <summary>
		/// Appends a child element to the specified parent.
		/// </summary>
		XmlElement AppendChildElement(XmlElement parentElement, string name)
		{
			XmlElement element = dialogElement.OwnerDocument.CreateElement(name, WixNamespaceManager.Namespace);
			parentElement.AppendChild(element);
			return element;
		}
	
		Bitmap GetBitmapFromId(string id)
		{			
			if (binaries != null) {
				string fileName = binaries.GetBinaryFileName(id);
				return document.LoadBitmapWithFileName(fileName);
			} 
			return document.LoadBitmapWithId(id);
		}
		
		static string XmlEncode(string item)
		{
			char quoteChar = '\'';
			return XmlEncoder.Encode(item, quoteChar);
		}
	}
}
