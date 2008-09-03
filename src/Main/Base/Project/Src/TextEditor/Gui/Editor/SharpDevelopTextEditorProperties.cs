// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="none" email=""/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Drawing;
using System.Drawing.Text;
using System.Text;

using ICSharpCode.Core;
using ICSharpCode.Core.WinForms;
using ICSharpCode.SharpDevelop.Gui.OptionPanels;
using ICSharpCode.TextEditor.Document;

namespace ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor
{
	public sealed class SharpDevelopTextEditorProperties : ITextEditorProperties
	{
		static SharpDevelopTextEditorProperties textEditorProperties;
		Properties properties;
		FontContainer fontContainer;
		
		public static SharpDevelopTextEditorProperties Instance {
			get {
				if (textEditorProperties == null) {
					textEditorProperties = new SharpDevelopTextEditorProperties();
				}
				return textEditorProperties;
			}
		}
		
		private SharpDevelopTextEditorProperties()
		{
			properties = PropertyService.Get("ICSharpCode.TextEditor.Document.Document.DefaultDocumentAggregatorProperties", new Properties());
			fontContainer = new FontContainer(FontContainer.ParseFont(properties.Get("DefaultFont", WinFormsResourceService.DefaultMonospacedFont.ToString())));
			properties.PropertyChanged += new PropertyChangedEventHandler(CheckFontChange);
		}
		
		void CheckFontChange(object sender, PropertyChangedEventArgs e)
		{
			if (e.Key == "DefaultFont") {
				fontContainer.DefaultFont = FontContainer.ParseFont(e.NewValue.ToString());
			}
		}
		
		public int TabIndent {
			get {
				return properties.Get("TabIndent", 4);
			}
			set {
				// FIX: don't allow to set tab size to zero as this will cause divide by zero exceptions in the text control.
				// Zero isn't a setting that makes sense, anyway.
				if (value < 1) value = 1;
				properties.Set("TabIndent", value);
			}
		}
		
		public int IndentationSize {
			get { return properties.Get("IndentationSize", 4); }
			set {
				if (value < 1) value = 1;
				properties.Set("IndentationSize", value);
				indentationString = null;
			}
		}
		
		string indentationString;
		
		public string IndentationString {
			get {
				if (indentationString == null) {
					if (ConvertTabsToSpaces)
						return new string(' ', IndentationSize);
					else
						return "\t";
				}
				return indentationString;
			}
		}
		
		public IndentStyle IndentStyle {
			get {
				return properties.Get("IndentStyle", IndentStyle.Smart);
			}
			set {
				properties.Set("IndentStyle", value);
			}
		}
		
		public DocumentSelectionMode DocumentSelectionMode {
			get {
				return properties.Get("DocumentSelectionMode", DocumentSelectionMode.Normal);
			}
			set {
				properties.Set("DocumentSelectionMode", value);
			}
		}
		
		public bool CaretLine {
			get {
				return properties.Get("CaretLine", false);
			}
			set {
				properties.Set("CaretLine", value);
			}
		}

		public bool ShowQuickClassBrowserPanel {
			get {
				return properties.Get("ShowQuickClassBrowserPanel", true);
			}
			set {
				properties.Set("ShowQuickClassBrowserPanel", value);
			}
		}
		
		public bool AllowCaretBeyondEOL {
			get {
				return properties.Get("CursorBehindEOL", false);
			}
			set {
				properties.Set("CursorBehindEOL", value);
			}
		}
		public bool UnderlineErrors {
			get {
				return properties.Get("ShowErrors", true);
			}
			set {
				properties.Set("ShowErrors", value);
			}
		}
		public bool ShowMatchingBracket {
			get {
				return properties.Get("ShowBracketHighlight", true);
			}
			set {
				properties.Set("ShowBracketHighlight", value);
			}
		}
		public bool ShowLineNumbers {
			get {
				return properties.Get("ShowLineNumbers", true);
			}
			set {
				properties.Set("ShowLineNumbers", value);
			}
		}
		public bool ShowSpaces {
			get {
				return properties.Get("ShowSpaces", false);
			}
			set {
				properties.Set("ShowSpaces", value);
			}
		}
		public bool ShowTabs {
			get {
				return properties.Get("ShowTabs", false);
			}
			set {
				properties.Set("ShowTabs", value);
			}
		}
		public bool ShowEOLMarker {
			get {
				return properties.Get("ShowEOLMarkers", false);
			}
			set {
				properties.Set("ShowEOLMarkers", value);
			}
		}
		public bool ShowInvalidLines {
			get {
				return properties.Get("ShowInvalidLines", false);
			}
			set {
				properties.Set("ShowInvalidLines", value);
			}
		}
		public bool IsIconBarVisible {
			get {
				return properties.Get("IconBarVisible", true);
			}
			set {
				properties.Set("IconBarVisible", value);
			}
		}
		public bool EnableFolding {
			get {
				return properties.Get("EnableFolding", true);
			}
			set {
				properties.Set("EnableFolding", value);
			}
		}
		public bool ShowHorizontalRuler {
			get {
				return properties.Get("ShowHRuler", false);
			}
			set {
				properties.Set("ShowHRuler", value);
			}
		}
		public bool ShowVerticalRuler {
			get {
				return properties.Get("ShowVRuler", false);
			}
			set {
				properties.Set("ShowVRuler", value);
			}
		}
		public bool ConvertTabsToSpaces {
			get {
				return properties.Get("TabsToSpaces", false);
			}
			set {
				properties.Set("TabsToSpaces", value);
				indentationString = null;
			}
		}
		public bool MouseWheelScrollDown {
			get {
				return properties.Get("MouseWheelScrollDown", true);
			}
			set {
				properties.Set("MouseWheelScrollDown", value);
			}
		}
		
		public bool MouseWheelTextZoom {
			get {
				return properties.Get("MouseWheelTextZoom", true);
			}
			set {
				properties.Set("MouseWheelTextZoom", value);
			}
		}
		
		public bool HideMouseCursor {
			get {
				return properties.Get("HideMouseCursor", false);
			}
			set {
				properties.Set("HideMouseCursor", value);
			}
		}

		public bool CutCopyWholeLine {
			get {
				return properties.Get("CutCopyWholeLine", true);
			}
			set {
				properties.Set("CutCopyWholeLine", value);
			}
		}

		public Encoding Encoding {
			get {
				return Encoding.GetEncoding(this.EncodingCodePage);
			}
			set {
				this.EncodingCodePage = value.CodePage;
			}
		}
		public int EncodingCodePage {
			get { return properties.Get("Encoding", 65001); }
			set { properties.Set("Encoding", value); }
		}
		
		public int VerticalRulerRow {
			get {
				return properties.Get("VRulerRow", 80);
			}
			set {
				properties.Set("VRulerRow", value);
			}
		}
		public LineViewerStyle LineViewerStyle {
			get {
				return properties.Get("LineViewerStyle", LineViewerStyle.None);
			}
			set {
				properties.Set("LineViewerStyle", value);
			}
		}
		public string LineTerminator {
			get {
				LineTerminatorStyle lineTerminatorStyle = PropertyService.Get("SharpDevelop.LineTerminatorStyle", LineTerminatorStyle.Windows);
				switch (lineTerminatorStyle) {
					case LineTerminatorStyle.Windows:
						return "\r\n";
					case LineTerminatorStyle.Macintosh:
						return "\r";
				}
				return "\n";
			}
			set {
				throw new System.NotImplementedException();
			}
		}
		public bool AutoInsertCurlyBracket {
			get {
				return properties.Get("AutoInsertCurlyBracket", true);
			}
			set {
				properties.Set("AutoInsertCurlyBracket", value);
			}
		}
		public bool AutoInsertTemplates {
			get {
				return properties.Get("AutoInsertTemplates", false);
			}
			set {
				properties.Set("AutoInsertTemplates", value);
			}
		}
		
		public Font Font {
			get {
				return fontContainer.DefaultFont;
			}
			set {
				properties.Set("DefaultFont", value.ToString());
				fontContainer.DefaultFont = value;
			}
		}
		public FontContainer FontContainer {
			get {
				return fontContainer;
			}
		}
		public BracketMatchingStyle  BracketMatchingStyle {
			get {
				return properties.Get("BracketMatchingStyle", BracketMatchingStyle.After);
			}
			set {
				properties.Set("BracketMatchingStyle", value);
			}
		}
		
		public bool SupportReadOnlySegments { get; set; }
		
		public TextRenderingHint TextRenderingHint {
			get {
				return properties.Get("TextRenderingHint", TextRenderingHint.SystemDefault);
			}
			set {
				LoggingService.Debug("Setting TextRenderingHint to " + value);
				properties.Set("TextRenderingHint", value);
			}
		}
	}
}


