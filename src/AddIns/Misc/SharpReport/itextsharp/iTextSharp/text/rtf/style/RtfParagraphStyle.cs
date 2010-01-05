using System;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.rtf;
using iTextSharp.text.rtf.document;
using iTextSharp.text.rtf.text;

namespace iTextSharp.text.rtf.style {

    /**
    * The RtfParagraphStyle stores all style/formatting attributes of a RtfParagraph.
    * Additionally it also supports the style name system available in RTF. The RtfParagraphStyle
    * is a Font and can thus be used as such. To use the stylesheet functionality
    * it needs to be set as the font of a Paragraph. Otherwise it will work like a
    * RtfFont. It also supports inheritance of styles.
    * 
    * @version $Revision$
    * @author Mark Hall (Mark.Hall@mail.room3b.eu)
    */
    public class RtfParagraphStyle : RtfFont {

        /**
        * Constant for left alignment
        */
        public static byte[] ALIGN_LEFT = DocWriter.GetISOBytes("\\ql");
        /**
        * Constant for right alignment
        */
        public static byte[] ALIGN_RIGHT = DocWriter.GetISOBytes("\\qr");
        /**
        * Constant for center alignment
        */
        public static byte[] ALIGN_CENTER = DocWriter.GetISOBytes("\\qc");
        /**
        * Constant for justified alignment
        */
        public static byte[] ALIGN_JUSTIFY = DocWriter.GetISOBytes("\\qj");
        /**
        * Constant for the first line indentation
        */
        public static byte[] FIRST_LINE_INDENT = DocWriter.GetISOBytes("\\fi");
        /**
        * Constant for left indentation
        */
        public static byte[] INDENT_LEFT = DocWriter.GetISOBytes("\\li");
        /**
        * Constant for right indentation
        */
        public static byte[] INDENT_RIGHT = DocWriter.GetISOBytes("\\ri");
        /**
        * Constant for keeping the paragraph together on one page
        */
        public static byte[] KEEP_TOGETHER = DocWriter.GetISOBytes("\\keep");
        /**
        * Constant for keeping the paragraph toghether with the next one on one page
        */
        public static byte[] KEEP_TOGETHER_WITH_NEXT = DocWriter.GetISOBytes("\\keepn");
        /**
        * Constant for the space after the paragraph.
        */
        public static byte[] SPACING_AFTER = DocWriter.GetISOBytes("\\sa");
        /**
        * Constant for the space before the paragraph.
        */
        public static byte[] SPACING_BEFORE = DocWriter.GetISOBytes("\\sb");

        /**
        * The NORMAL/STANDARD style.
        */
        public static RtfParagraphStyle STYLE_NORMAL = new RtfParagraphStyle("Normal", "Arial", 12, Font.NORMAL, Color.BLACK);
        /**
        * The style for level 1 headings.
        */
        public static RtfParagraphStyle STYLE_HEADING_1 = new RtfParagraphStyle("heading 1", "Normal");
        /**
        * The style for level 2 headings.
        */
        public static RtfParagraphStyle STYLE_HEADING_2 = new RtfParagraphStyle("heading 2", "Normal");
        /**
        * The style for level 3 headings.
        */
        public static RtfParagraphStyle STYLE_HEADING_3 = new RtfParagraphStyle("heading 3", "Normal");

        /**
        * Initialises the properties of the styles.
        */
        static RtfParagraphStyle() {
            STYLE_HEADING_1.Size = 16;
            STYLE_HEADING_1.SetStyle(Font.BOLD);
            STYLE_HEADING_2.Size = 14;
            STYLE_HEADING_2.SetStyle(Font.BOLDITALIC);
            STYLE_HEADING_3.Size = 13;
            STYLE_HEADING_3.SetStyle(Font.BOLD);
        }
        
        /**
        * No modification has taken place when compared to the RtfParagraphStyle this RtfParagraphStyle
        * is based on. These modification markers are used to determine what needs to be
        * inherited and what not from the parent RtfParagraphStyle.
        */
        private const int MODIFIED_NONE = 0;
        /**
        * The alignment has been modified.
        */
        private const int MODIFIED_ALIGNMENT = 1;
        /**
        * The left indentation has been modified.
        */
        private const int MODIFIED_INDENT_LEFT = 2;
        /**
        * The right indentation has been modified.
        */
        private const int MODIFIED_INDENT_RIGHT = 4;
        /**
        * The spacing before a paragraph has been modified.
        */
        private const int MODIFIED_SPACING_BEFORE = 8;
        /**
        * The spacing after a paragraph has been modified.
        */
        private const int MODIFIED_SPACING_AFTER = 16;
        /**
        * The font name has been modified.
        */
        private const int MODIFIED_FONT_NAME = 32;
        /**
        * The font style has been modified.
        */
        private const int MODIFIED_FONT_SIZE = 64;
        /**
        * The font size has been modified.
        */
        private const int MODIFIED_FONT_STYLE = 128;
        /**
        * The font colour has been modified.
        */
        private const int MODIFIED_FONT_COLOR = 256;
        /**
        * The line leading has been modified. 
        */
        private const int MODIFIED_LINE_LEADING = 512;
        /**
        * The paragraph keep together setting has been modified.
        */
        private const int MODIFIED_KEEP_TOGETHER = 1024;
        /**
        * The paragraph keep together with next setting has been modified.
        */
        private const int MODIFIED_KEEP_TOGETHER_WITH_NEXT = 2048;
        
        /**
        * The alignment of the paragraph.
        */
        private int alignment = Element.ALIGN_LEFT;
        /**
        * The indentation for the first line
        */
        private int firstLineIndent = 0;
        /**
        * The left indentation of the paragraph.
        */
        private int indentLeft = 0;
        /**
        * The right indentation of the paragraph.
        */
        private int indentRight = 0;
        /**
        * The spacing before a paragraph.
        */
        private int spacingBefore = 0;
        /**
        * The spacing after a paragraph.
        */
        private int spacingAfter = 0;
        /**
        * The line leading of the paragraph.
        */
        private int lineLeading = 0;
        /**
        * Whether this RtfParagraph must stay on one page.
        */
        private bool keepTogether = false;
        /**
        * Whether this RtfParagraph must stay on the same page as the next paragraph.
        */
        private bool keepTogetherWithNext = false;
        /**
        * The name of this RtfParagraphStyle.
        */
        private String styleName = "";
        /**
        * The name of the RtfParagraphStyle this RtfParagraphStyle is based on.
        */
        private String basedOnName = null;
        /**
        * The RtfParagraphStyle this RtfParagraphStyle is based on.
        */
        private RtfParagraphStyle baseStyle = null;
        /**
        * Which properties have been modified when compared to the base style.
        */
        private int modified = MODIFIED_NONE;
        /**
        * The number of this RtfParagraphStyle in the stylesheet list.
        */
        private int styleNumber = -1;
        
        /**
        * Constructs a new RtfParagraphStyle with the given attributes.
        * 
        * @param styleName The name of this RtfParagraphStyle.
        * @param fontName The name of the font to use for this RtfParagraphStyle.
        * @param fontSize The size of the font to use for this RtfParagraphStyle.
        * @param fontStyle The style of the font to use for this RtfParagraphStyle.
        * @param fontColor The colour of the font to use for this RtfParagraphStyle.
        */
        public RtfParagraphStyle(String styleName, String fontName, int fontSize, int fontStyle, Color fontColor) : base(null, new RtfFont(fontName, fontSize, fontStyle, fontColor)) {
            this.styleName = styleName;
        }
        
        /**
        * Constructs a new RtfParagraphStyle that is based on an existing RtfParagraphStyle.
        * 
        * @param styleName The name of this RtfParagraphStyle.
        * @param basedOnName The name of the RtfParagraphStyle this RtfParagraphStyle is based on.
        */
        public RtfParagraphStyle(String styleName, String basedOnName) : base(null, new Font()) {
            this.styleName = styleName;
            this.basedOnName = basedOnName;
        }
        
        /**
        * Constructs a RtfParagraphStyle from another RtfParagraphStyle.
        * 
        * INTERNAL USE ONLY
        * 
        * @param doc The RtfDocument this RtfParagraphStyle belongs to.
        * @param style The RtfParagraphStyle to copy settings from.
        */
        public RtfParagraphStyle(RtfDocument doc, RtfParagraphStyle style) : base(doc, style) {
            this.document = doc;
            this.styleName = style.GetStyleName();
            this.alignment = style.GetAlignment();
            this.firstLineIndent = (int)(style.GetFirstLineIndent() * RtfElement.TWIPS_FACTOR);
            this.indentLeft = (int) (style.GetIndentLeft() * RtfElement.TWIPS_FACTOR);
            this.indentRight = (int) (style.GetIndentRight() * RtfElement.TWIPS_FACTOR);
            this.spacingBefore = (int) (style.GetSpacingBefore() * RtfElement.TWIPS_FACTOR);
            this.spacingAfter = (int) (style.GetSpacingAfter() * RtfElement.TWIPS_FACTOR);
            this.lineLeading = (int) (style.GetLineLeading() * RtfElement.TWIPS_FACTOR);
            this.keepTogether = style.GetKeepTogether();
            this.keepTogetherWithNext = style.GetKeepTogetherWithNext();
            this.basedOnName = style.basedOnName;
            this.modified = style.modified;
            this.styleNumber = style.GetStyleNumber();

            if (this.document != null) {
                SetRtfDocument(this.document);
            }
        }

        /**
        * Gets the name of this RtfParagraphStyle.
        * 
        * @return The name of this RtfParagraphStyle.
        */
        public String GetStyleName() {
            return this.styleName;
        }
        
        /**
        * Gets the name of the RtfParagraphStyle this RtfParagraphStyle is based on.
        * 
        * @return The name of the base RtfParagraphStyle.
        */
        public String GetBasedOnName() {
            return this.basedOnName;
        }
        
        /**
        * Gets the alignment of this RtfParagraphStyle.
        * 
        * @return The alignment of this RtfParagraphStyle.
        */
        public int GetAlignment() {
            return this.alignment;
        }

        /**
        * Sets the alignment of this RtfParagraphStyle.
        * 
        * @param alignment The alignment to use.
        */
        public void SetAlignment(int alignment) {
            this.modified = this.modified | MODIFIED_ALIGNMENT;
            this.alignment = alignment;
        }
        
        /**
        * Gets the first line indentation of this RtfParagraphStyle.
        * 
        * @return The first line indentation of this RtfParagraphStyle.
        */
        public int GetFirstLineIndent() {
            return this.firstLineIndent;
        }
        
        /**
        * Sets the first line indententation of this RtfParagraphStyle. It
        * is relative to the left indentation.
        * 
        * @param firstLineIndent The first line indentation to use.
        */
        public void SetFirstLineIndent(int firstLineIndent) {
            this.firstLineIndent = firstLineIndent;
        }

        /**
        * Gets the left indentation of this RtfParagraphStyle.
        * 
        * @return The left indentation of this RtfParagraphStyle.
        */
        public int GetIndentLeft() {
            return this.indentLeft;
        }

        /**
        * Sets the left indentation of this RtfParagraphStyle.
        * 
        * @param indentLeft The left indentation to use.
        */
        public void SetIndentLeft(int indentLeft) {
            this.modified = this.modified | MODIFIED_INDENT_LEFT;
            this.indentLeft = indentLeft;
        }
        
        /**
        * Gets the right indentation of this RtfParagraphStyle.
        * 
        * @return The right indentation of this RtfParagraphStyle.
        */
        public int GetIndentRight() {
            return this.indentRight;
        }

        /**
        * Sets the right indentation of this RtfParagraphStyle.
        * 
        * @param indentRight The right indentation to use.
        */
        public void SetIndentRight(int indentRight) {
            this.modified = this.modified | MODIFIED_INDENT_RIGHT;
            this.indentRight = indentRight;
        }
        
        /**
        * Gets the space before the paragraph of this RtfParagraphStyle..
        * 
        * @return The space before the paragraph.
        */
        public int GetSpacingBefore() {
            return this.spacingBefore;
        }

        /**
        * Sets the space before the paragraph of this RtfParagraphStyle.
        * 
        * @param spacingBefore The space before to use.
        */
        public void SetSpacingBefore(int spacingBefore) {
            this.modified = this.modified | MODIFIED_SPACING_BEFORE;
            this.spacingBefore = spacingBefore;
        }
        
        /**
        * Gets the space after the paragraph of this RtfParagraphStyle.
        * 
        * @return The space after the paragraph.
        */
        public int GetSpacingAfter() {
            return this.spacingAfter;
        }
        
        /**
        * Sets the space after the paragraph of this RtfParagraphStyle.
        * 
        * @param spacingAfter The space after to use.
        */
        public void SetSpacingAfter(int spacingAfter) {
            this.modified = this.modified | MODIFIED_SPACING_AFTER;
            this.spacingAfter = spacingAfter;
        }
        
        /**
        * Sets the font name of this RtfParagraphStyle.
        * 
        * @param fontName The font name to use 
        */
        public override void SetFontName(String fontName) {
            this.modified = this.modified | MODIFIED_FONT_NAME;
            base.SetFontName(fontName);
        }
        
        /**
        * Sets the font size of this RtfParagraphStyle.
        * 
        * @param fontSize The font size to use.
        */
        public override float Size {
            set {
                this.modified = this.modified | MODIFIED_FONT_SIZE;
                base.Size = value;
            }
        }
        
        /**
        * Sets the font style of this RtfParagraphStyle.
        * 
        * @param fontStyle The font style to use.
        */
        public override void SetStyle(int fontStyle) {
            this.modified = this.modified | MODIFIED_FONT_STYLE;
            base.SetStyle(fontStyle);
        }
        
        /**
        * Sets the colour of this RtfParagraphStyle.
        * 
        * @param color The Color to use.
        */
        public void SetColor(Color color) {
            this.modified = this.modified | MODIFIED_FONT_COLOR;
            base.Color = color;
        }
        
        /**
        * Gets the line leading of this RtfParagraphStyle.
        * 
        * @return The line leading of this RtfParagraphStyle.
        */
        public int GetLineLeading() {
            return this.lineLeading;
        }
        
        /**
        * Sets the line leading of this RtfParagraphStyle.
        * 
        * @param lineLeading The line leading to use.
        */
        public void SetLineLeading(int lineLeading) {
            this.lineLeading = lineLeading;
            this.modified = this.modified | MODIFIED_LINE_LEADING;
        }
        
        /**
        * Gets whether the lines in the paragraph should be kept together in
        * this RtfParagraphStyle.
        * 
        * @return Whether the lines in the paragraph should be kept together.
        */
        public bool GetKeepTogether() {
            return this.keepTogether;
        }
        
        /**
        * Sets whether the lines in the paragraph should be kept together in
        * this RtfParagraphStyle.
        * 
        * @param keepTogether Whether the lines in the paragraph should be kept together.
        */
        public void SetKeepTogether(bool keepTogether) {
            this.keepTogether = keepTogether;
            this.modified = this.modified | MODIFIED_KEEP_TOGETHER;
        }
        
        /**
        * Gets whether the paragraph should be kept toggether with the next in
        * this RtfParagraphStyle.
        * 
        * @return Whether the paragraph should be kept together with the next.
        */
        public bool GetKeepTogetherWithNext() {
            return this.keepTogetherWithNext;
        }
        
        /**
        * Sets whether the paragraph should be kept together with the next in
        * this RtfParagraphStyle.
        * 
        * @param keepTogetherWithNext Whether the paragraph should be kept together with the next.
        */
        public void SetKeepTogetherWithNext(bool keepTogetherWithNext) {
            this.keepTogetherWithNext = keepTogetherWithNext;
            this.modified = this.modified | MODIFIED_KEEP_TOGETHER_WITH_NEXT;
        }
        
        /**
        * Handles the inheritance of paragraph style settings. All settings that
        * have not been modified will be inherited from the base RtfParagraphStyle.
        * If this RtfParagraphStyle is not based on another one, then nothing happens.
        */
        public void HandleInheritance() {
            if (this.basedOnName != null && this.document.GetDocumentHeader().GetRtfParagraphStyle(this.basedOnName) != null) {
                this.baseStyle = this.document.GetDocumentHeader().GetRtfParagraphStyle(this.basedOnName);
                this.baseStyle.HandleInheritance();
                if (!((this.modified & MODIFIED_ALIGNMENT) == MODIFIED_ALIGNMENT)) {
                    this.alignment = this.baseStyle.GetAlignment();
                }
                if (!((this.modified & MODIFIED_INDENT_LEFT) == MODIFIED_INDENT_LEFT)) {
                    this.indentLeft = this.baseStyle.GetIndentLeft();
                }
                if (!((this.modified & MODIFIED_INDENT_RIGHT) == MODIFIED_INDENT_RIGHT)) {
                    this.indentRight = this.baseStyle.GetIndentRight();
                }
                if (!((this.modified & MODIFIED_SPACING_BEFORE) == MODIFIED_SPACING_BEFORE)) {
                    this.spacingBefore = this.baseStyle.GetSpacingBefore();
                }
                if (!((this.modified & MODIFIED_SPACING_AFTER) == MODIFIED_SPACING_AFTER)) {
                    this.spacingAfter = this.baseStyle.GetSpacingAfter();
                }
                if (!((this.modified & MODIFIED_FONT_NAME) == MODIFIED_FONT_NAME)) {
                    SetFontName(this.baseStyle.GetFontName());
                }
                if (!((this.modified & MODIFIED_FONT_SIZE) == MODIFIED_FONT_SIZE)) {
                    Size = this.baseStyle.GetFontSize();
                }
                if (!((this.modified & MODIFIED_FONT_STYLE) == MODIFIED_FONT_STYLE)) {
                    SetStyle(this.baseStyle.GetFontStyle());
                }
                if (!((this.modified & MODIFIED_FONT_COLOR) == MODIFIED_FONT_COLOR)) {
                    SetColor(this.baseStyle.Color);
                }
                if (!((this.modified & MODIFIED_LINE_LEADING) == MODIFIED_LINE_LEADING)) {
                    SetLineLeading(this.baseStyle.GetLineLeading());
                }
                if (!((this.modified & MODIFIED_KEEP_TOGETHER) == MODIFIED_KEEP_TOGETHER)) {
                    SetKeepTogether(this.baseStyle.GetKeepTogether());
                }
                if (!((this.modified & MODIFIED_KEEP_TOGETHER_WITH_NEXT) == MODIFIED_KEEP_TOGETHER_WITH_NEXT)) {
                    SetKeepTogetherWithNext(this.baseStyle.GetKeepTogetherWithNext());
                }
            }
        }
        
        /**
        * Writes the settings of this RtfParagraphStyle.
        * 
        */
        private void WriteParagraphSettings(Stream result) {
            byte[] t;
            if (this.keepTogether) {
                result.Write(t = RtfParagraphStyle.KEEP_TOGETHER, 0, t.Length);
            }
            if (this.keepTogetherWithNext) {
                result.Write(t = RtfParagraphStyle.KEEP_TOGETHER_WITH_NEXT, 0, t.Length);
            }
            switch (alignment) {
                case Element.ALIGN_LEFT:
                    result.Write(t = RtfParagraphStyle.ALIGN_LEFT, 0, t.Length);
                    break;
                case Element.ALIGN_RIGHT:
                    result.Write(t = RtfParagraphStyle.ALIGN_RIGHT, 0, t.Length);
                    break;
                case Element.ALIGN_CENTER:
                    result.Write(t = RtfParagraphStyle.ALIGN_CENTER, 0, t.Length);
                    break;
                case Element.ALIGN_JUSTIFIED:
                case Element.ALIGN_JUSTIFIED_ALL:
                    result.Write(t = RtfParagraphStyle.ALIGN_JUSTIFY, 0, t.Length);
                    break;
            }
            result.Write(t = FIRST_LINE_INDENT, 0, t.Length);
            result.Write(t = IntToByteArray(this.firstLineIndent), 0, t.Length);
            result.Write(t = RtfParagraphStyle.INDENT_LEFT, 0, t.Length);
            result.Write(t = IntToByteArray(indentLeft), 0, t.Length);
            result.Write(t = RtfParagraphStyle.INDENT_RIGHT, 0, t.Length);
            result.Write(t = IntToByteArray(indentRight), 0, t.Length);
            if (this.spacingBefore > 0) {
                result.Write(t = RtfParagraphStyle.SPACING_BEFORE, 0, t.Length);
                result.Write(t = IntToByteArray(this.spacingBefore), 0, t.Length);
            }
            if (this.spacingAfter > 0) {
                result.Write(t = RtfParagraphStyle.SPACING_AFTER, 0, t.Length);
                result.Write(t = IntToByteArray(this.spacingAfter), 0, t.Length);
            }
            if (this.lineLeading > 0) {
                result.Write(t = RtfParagraph.LINE_SPACING, 0, t.Length);
                result.Write(t = IntToByteArray(this.lineLeading), 0, t.Length);
            }            
        }
        
        /**
        * Writes the definition of this RtfParagraphStyle for the stylesheet list.
        */
        public override void WriteDefinition(Stream result) {
            byte[] t;
            result.Write(t = DocWriter.GetISOBytes("{"), 0, t.Length);
            result.Write(t = DocWriter.GetISOBytes("\\style"), 0, t.Length);
            result.Write(t = DocWriter.GetISOBytes("\\s"), 0, t.Length);
            result.Write(t = IntToByteArray(this.styleNumber), 0, t.Length);
            result.Write(t = RtfElement.DELIMITER, 0, t.Length);
            WriteParagraphSettings(result);
            base.WriteBegin(result);
            result.Write(t = RtfElement.DELIMITER, 0, t.Length);
            result.Write(t = DocWriter.GetISOBytes(this.styleName), 0, t.Length);
            result.Write(t = DocWriter.GetISOBytes(";"), 0, t.Length);
            result.Write(t = DocWriter.GetISOBytes("}"), 0, t.Length);
            if (this.document.GetDocumentSettings().IsOutputDebugLineBreaks()) {
                result.WriteByte((byte)'\n');
            }
        }
        
        /**
        * Writes the start information of this RtfParagraphStyle.
        *
        * @param result The <code>OutputStream</code> to write to.
        * @throws IOException On i/o errors.
        */
        public override void WriteBegin(Stream result) {
            byte[] t;
            result.Write(t = DocWriter.GetISOBytes("\\s"), 0, t.Length);
            result.Write(t = IntToByteArray(this.styleNumber), 0, t.Length);
            WriteParagraphSettings(result);
        }
        
        /**
        * Unused
        */
        public override void WriteEnd(Stream result) {
        }
        
        /**
        * unused
        */
        public override void WriteContent(Stream outp) {       
        }
                
        /**
        * Tests whether two RtfParagraphStyles are equal. Equality
        * is determined via the name.
        */
        public override bool Equals(Object o) {
            if (!(o is RtfParagraphStyle)) {
                return false;
            }
            RtfParagraphStyle paragraphStyle = (RtfParagraphStyle) o;
            bool result = this.GetStyleName().Equals(paragraphStyle.GetStyleName());
            return result;
        }
        
        /**
        * Gets the hash code of this RtfParagraphStyle.
        */
        public override int GetHashCode() {
            return this.styleName.GetHashCode();
        }
        
        /**
        * Gets the number of this RtfParagraphStyle in the stylesheet list.
        * 
        * @return The number of this RtfParagraphStyle in the stylesheet list.
        */
        private int GetStyleNumber() {
            return this.styleNumber;
        }
        
        /**
        * Sets the number of this RtfParagraphStyle in the stylesheet list.
        * 
        * @param styleNumber The number to use.
        */
        protected internal void SetStyleNumber(int styleNumber) {
            this.styleNumber = styleNumber;
        }
    }
}