// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="none" email=""/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Drawing;
using System.Text;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui.OptionPanels;
using ICSharpCode.TextEditor.Document;

namespace ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor
{
	public class SharpDevelopTextEditorProperties : ITextEditorProperties
	{
		static Properties properties;
		static FontContainer fontContainer;
		
		static SharpDevelopTextEditorProperties()
		{
			Properties properties2 = ((Properties)PropertyService.Get("ICSharpCode.TextEditor.Document.Document.DefaultDocumentAggregatorProperties", new Properties()));
			fontContainer = new FontContainer(FontContainer.ParseFont(properties2.Get("DefaultFont", ResourceService.DefaultMonospacedFont.ToString())));
			properties2.PropertyChanged += new PropertyChangedEventHandler(CheckFontChange);
		}
		
		static void CheckFontChange(object sender, PropertyChangedEventArgs e)
		{
			if (e.Key == "DefaultFont") {
				fontContainer.DefaultFont = FontContainer.ParseFont(e.NewValue.ToString());
			}
		}
		
		public SharpDevelopTextEditorProperties()
		{
			properties = ((Properties)PropertyService.Get("ICSharpCode.TextEditor.Document.Document.DefaultDocumentAggregatorProperties", new Properties()));
		}
		
		public int TabIndent {
			get {
				return properties.Get("TabIndent", 4);

			}
			set {
				properties.Set("TabIndent", value);
			}
		}
		public IndentStyle IndentStyle {
			get {
				return (IndentStyle)properties.Get("IndentStyle", IndentStyle.Smart);
			}
			set {
				properties.Set("IndentStyle", value);
			}
		}
		
		public DocumentSelectionMode DocumentSelectionMode {
			get {
				return (DocumentSelectionMode)properties.Get("DocumentSelectionMode", DocumentSelectionMode.Normal);
			}
			set {
				properties.Set("DocumentSelectionMode", value);
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
				properties.Get("ShowTabs", value);
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
			}
		}
		public bool UseAntiAliasedFont {
			get {
				return properties.Get("UseAntiAliasFont", false);
			}
			set {
				properties.Set("UseAntiAliasFont", value);
			}
		}
		public bool CreateBackupCopy {
			get {
				return properties.Get("CreateBackupCopy", false);
			}
			set {
				properties.Set("CreateBackupCopy", value);
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
				return Encoding.GetEncoding(properties.Get("Encoding", 65001));
			}
			set {
				properties.Set("Encoding", value.CodePage);
			}
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
				return (LineViewerStyle)properties.Get("LineViewerStyle", LineViewerStyle.None);
			}
			set {
				properties.Set("LineViewerStyle", value);
			}
		}
		public string LineTerminator {
			get {
				LineTerminatorStyle lineTerminatorStyle = (LineTerminatorStyle)PropertyService.Get("SharpDevelop.LineTerminatorStyle", LineTerminatorStyle.Windows);
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
		
		public Font Font {
			get {
				return fontContainer.DefaultFont;
			}
			set {
				properties.Set("DefaultFont", value.ToString());
				fontContainer.DefaultFont = value;
			}
		}
		FontContainer ITextEditorProperties.FontContainer {
			get {
				return fontContainer;
			}
		}
		public static FontContainer FontContainer {
			get {
				return fontContainer;
			}
		}
		
		public BracketMatchingStyle  BracketMatchingStyle {
			get {
				return (BracketMatchingStyle)properties.Get("BracketMatchingStyle", BracketMatchingStyle.After);
			}
			set {
				properties.Set("BracketMatchingStyle", value);
			}
		}
		
		bool useCustomLine = false;
		public bool UseCustomLine {
			get {
				return useCustomLine;
			}
			set {
				useCustomLine = value;
			}
		}

		/*
		<Property key="DoubleBuffer" value="True" />
        <Property key="ShowErrors" value="True" />
        <Property key="" value="True" />
        <Property key="AutoInsertTemplates" value="True" />
        <Property key="IndentationSize" value="4" />		 * */
	}
}
