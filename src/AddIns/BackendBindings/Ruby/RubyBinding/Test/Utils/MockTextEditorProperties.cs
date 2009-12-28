// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Drawing;
using System.Drawing.Text;
using System.Text;
using ICSharpCode.TextEditor.Document;

namespace RubyBinding.Tests.Utils
{
	public class MockTextEditorProperties : ITextEditorProperties
	{
		public MockTextEditorProperties()
		{
			FontContainer = new FontContainer(SystemFonts.MenuFont);
		}
		
		public bool CaretLine { get; set; }
		
		public bool AutoInsertCurlyBracket { get; set; }
		
		public bool HideMouseCursor { get; set; }
		
		public bool IsIconBarVisible { get; set; }
		
		public bool AllowCaretBeyondEOL { get; set; }
		
		public bool ShowMatchingBracket { get; set; }
		
		public bool CutCopyWholeLine { get; set; }
		
		public TextRenderingHint TextRenderingHint { get; set; }
		
		public bool MouseWheelScrollDown { get; set; }
		
		public bool MouseWheelTextZoom { get; set; }
		
		public string LineTerminator { get; set; }
		
		public LineViewerStyle LineViewerStyle { get; set; }
		
		public bool ShowInvalidLines { get; set; }
		
		public int VerticalRulerRow { get; set; }
		
		public bool ShowSpaces { get; set; }
		
		public bool ShowTabs { get; set; }
		
		public bool ShowEOLMarker { get; set; }
		
		public bool ConvertTabsToSpaces { get; set; }
		
		public bool ShowHorizontalRuler { get; set; }
		
		public bool ShowVerticalRuler { get; set; }
		
		public Encoding Encoding { get; set; }
		
		public bool EnableFolding{ get; set; }
		
		public bool ShowLineNumbers { get; set; }
		
		public int TabIndent { get; set; }
		
		public int IndentationSize { get; set; }
		
		public IndentStyle IndentStyle{ get; set; }
		
		public DocumentSelectionMode DocumentSelectionMode { get; set; }
		
		public Font Font { get; set; }
		
		public FontContainer FontContainer { get; set; }
		
		public BracketMatchingStyle BracketMatchingStyle { get; set; }
		
		public bool SupportReadOnlySegments { get; set; }
	}
}
