using System;
using System.IO;
using System.util;
using iTextSharp.text;
using iTextSharp.text.rtf;
using iTextSharp.text.rtf.document;
/*
 * $Id: RtfFont.cs,v 1.13 2008/05/16 19:31:11 psoares33 Exp $
 * 
 *
 * Copyright 2001, 2002, 2003, 2004 by Mark Hall
 *
 * The contents of this file are subject to the Mozilla Public License Version 1.1
 * (the "License"); you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at http://www.mozilla.org/MPL/
 *
 * Software distributed under the License is distributed on an "AS IS" basis,
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. See the License
 * for the specific language governing rights and limitations under the License.
 *
 * The Original Code is 'iText, a free JAVA-PDF library'.
 *
 * The Initial Developer of the Original Code is Bruno Lowagie. Portions created by
 * the Initial Developer are Copyright (C) 1999, 2000, 2001, 2002 by Bruno Lowagie.
 * All Rights Reserved.
 * Co-Developer of the code is Paulo Soares. Portions created by the Co-Developer
 * are Copyright (C) 2000, 2001, 2002 by Paulo Soares. All Rights Reserved.
 *
 * Contributor(s): all the names of the contributors are added in the source code
 * where applicable.
 *
 * Alternatively, the contents of this file may be used under the terms of the
 * LGPL license (the ?GNU LIBRARY GENERAL PUBLIC LICENSE?), in which case the
 * provisions of LGPL are applicable instead of those above.  If you wish to
 * allow use of your version of this file only under the terms of the LGPL
 * License and not to allow others to use your version of this file under
 * the MPL, indicate your decision by deleting the provisions above and
 * replace them with the notice and other provisions required by the LGPL.
 * If you do not delete the provisions above, a recipient may use your version
 * of this file under either the MPL or the GNU LIBRARY GENERAL PUBLIC LICENSE.
 *
 * This library is free software; you can redistribute it and/or modify it
 * under the terms of the MPL as stated above or under the terms of the GNU
 * Library General Public License as published by the Free Software Foundation;
 * either version 2 of the License, or any later version.
 *
 * This library is distributed in the hope that it will be useful, but WITHOUT
 * ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS
 * FOR A PARTICULAR PURPOSE. See the GNU Library general Public License for more
 * details.
 *
 * If you didn't download this code from the following link, you should check if
 * you aren't using an obsolete version:
 * http://www.lowagie.com/iText/
 */

namespace iTextSharp.text.rtf.style {

    /**
    * The RtfFont class stores one font for an rtf document. It extends Font,
    * so can be set as a font, to allow adding of fonts with arbitrary names.
    * BaseFont fontname handling contributed by Craig Fleming. Various fixes
    * Renaud Michel, Werner Daehn.
    *
    * Version: $Id: RtfFont.cs,v 1.13 2008/05/16 19:31:11 psoares33 Exp $
    * @author Mark Hall (Mark.Hall@mail.room3b.eu)
    * @author Craig Fleming (rythos@rhana.dhs.org)
    * @author Renaud Michel (r.michel@immedia.be)
    * @author Werner Daehn (Werner.Daehn@BusinessObjects.com)
    * @author Lidong Liu (tmslld@gmail.com)
    */
    public class RtfFont : Font, IRtfExtendedElement {
        /**
        * Constant for the font family to use ("froman")
        */
        private static byte[] FONT_FAMILY = DocWriter.GetISOBytes("\\froman");
        /**
        * Constant for the charset
        */
        private static byte[] FONT_CHARSET = DocWriter.GetISOBytes("\\fcharset");
        /**
        * Constant for the font size
        */
        public static byte[] FONT_SIZE = DocWriter.GetISOBytes("\\fs");
        /**
        * Constant for the bold flag
        */
        private static byte[] FONT_BOLD = DocWriter.GetISOBytes("\\b");
        /**
        * Constant for the italic flag
        */
        private static byte[] FONT_ITALIC = DocWriter.GetISOBytes("\\i");
        /**
        * Constant for the underline flag
        */
        private static byte[] FONT_UNDERLINE = DocWriter.GetISOBytes("\\ul");
        /**
        * Constant for the strikethrough flag
        */
        private static byte[] FONT_STRIKETHROUGH = DocWriter.GetISOBytes("\\strike");
        /**
        * Constant for the double strikethrough flag
        */
        private static byte[] FONT_DOUBLE_STRIKETHROUGH = DocWriter.GetISOBytes("\\striked");
        /**
        * Constant for the shadow flag
        */
        private static byte[] FONT_SHADOW = DocWriter.GetISOBytes("\\shad");
        /**
        * Constant for the outline flag
        */
        private static byte[] FONT_OUTLINE = DocWriter.GetISOBytes("\\outl");
        /**
        * Constant for the embossed flag
        */
        private static byte[] FONT_EMBOSSED = DocWriter.GetISOBytes("\\embo");
        /**
        * Constant for the engraved flag
        */
        private static byte[] FONT_ENGRAVED = DocWriter.GetISOBytes("\\impr");
        /**
        * Constant for hidden text flag
        */
        private static byte[] FONT_HIDDEN = DocWriter.GetISOBytes("\\v");
        
        /**
        * Constant for a plain font
        */
        public const int STYLE_NONE = 0;
        /**
        * Constant for a bold font
        */
        public const int STYLE_BOLD = 1;
        /**
        * Constant for an italic font
        */
        public const int STYLE_ITALIC = 2;
        /**
        * Constant for an underlined font
        */
        public const int STYLE_UNDERLINE = 4;
        /**
        * Constant for a strikethrough font
        */
        public const int STYLE_STRIKETHROUGH = 8;
        /**
        * Constant for a double strikethrough font
        */
        public const int STYLE_DOUBLE_STRIKETHROUGH = 16;
        /**
        * Constant for a shadowed font
        */
        public const int STYLE_SHADOW = 32;
        /**
        * Constant for an outlined font
        */
        public const int STYLE_OUTLINE = 64;
        /**
        * Constant for an embossed font
        */
        public const int STYLE_EMBOSSED = 128;
        /**
        * Constant for an engraved font
        */
        public const int STYLE_ENGRAVED = 256;
        /**
        * Constant for a font that hides the actual text.
        */
        public const int STYLE_HIDDEN = 512;

        /**
        * The font name. Defaults to "Times New Roman"
        */
        private String fontName = "Times New Roman";
        /**
        * The font size. Defaults to 10
        */
        private int fontSize = 10;
        /**
        * The font style. Defaults to STYLE_NONE
        */
        private int fontStyle = STYLE_NONE;
        /**
        * The number of this font
        */
        private int fontNumber = 0;
        /**
        * The colour of this font
        */
        private RtfColor color = null;
        /**
        * The character set to use for this font
        */
        private int charset = 0;
        /**
        * The RtfDocument this RtfFont belongs to.
        */
        protected RtfDocument document = null;
        
        /**
        * Constructs a RtfFont with the given font name and all other properties
        * at their default values.
        * 
        * @param fontName The font name to use
        */
        public RtfFont(String fontName) : base(Font.UNDEFINED, Font.UNDEFINED, Font.UNDEFINED, null) {
            this.fontName = fontName;
        }
        
        /**
        * Constructs a RtfFont with the given font name and font size and all other
        * properties at their default values.
        * 
        * @param fontName The font name to use
        * @param size The font size to use
        */
        public RtfFont(String fontName, float size) : base(Font.UNDEFINED, size, Font.UNDEFINED, null) {
            this.fontName = fontName;
        }
        
        /**
        * Constructs a RtfFont with the given font name, font size and font style and the
        * default color.
        * 
        * @param fontName The font name to use
        * @param size The font size to use
        * @param style The font style to use
        */
        public RtfFont(String fontName, float size, int style) : base(Font.UNDEFINED, size, style, null) {
            this.fontName = fontName;
        }
        
        /**
        * Constructs a RtfFont with the given font name, font size, font style and
        * color.
        * 
        * @param fontName The font name to use
        * @param size the font size to use
        * @param style The font style to use
        * @param color The font color to use
        */
        public RtfFont(String fontName, float size, int style, Color color) : base(Font.UNDEFINED, size, style, color) {
            this.fontName = fontName;
        }
        
        /**
        * Constructs a RtfFont with the given font name, font size, font style, colour
        * and charset. This can be used when generating non latin-1 text.
        * 
        * @param fontName The font name to use
        * @param size the font size to use
        * @param style The font style to use
        * @param color The font color to use
        * @param charset The charset of the font content
        */
        public RtfFont(String fontName, float size, int style, Color color, int charset) : this(fontName, size, style, color){
            this.charset = charset;
        }

        /**
        * Special constructor for the default font
        *
        * @param doc The RtfDocument this font appears in
        * @param fontNumber The id of this font
        */
        protected internal RtfFont(RtfDocument doc, int fontNumber) {
            this.document = doc;
            this.fontNumber = fontNumber;
            color = new RtfColor(doc, 0, 0, 0);
        }

        /**
        * Constructs a RtfFont from a com.lowagie.text.Font
        * @param doc The RtfDocument this font appears in
        * @param font The Font to use as a base
        */
        public RtfFont(RtfDocument doc, Font font) {
            this.document = doc;
            if (font != null) {
                if (font is RtfFont) {
                    this.fontName = ((RtfFont) font).GetFontName();
                    this.charset = ((RtfFont) font).GetCharset();
                } else {
                    SetToDefaultFamily(font.Familyname);
                }
                if (font.BaseFont != null) {
                    String[][] fontNames = font.BaseFont.FullFontName;
                    for (int i = 0; i < fontNames.Length; i++) {
                        if (fontNames[i][2].Equals("0")) {
                            this.fontName = fontNames[i][3];
                            break;
                        } else if (fontNames[i][2].Equals("1033") || fontNames[i][2].Equals("")) {
                            this.fontName = fontNames[i][3];
                        }
                    }
                }
                Size = font.Size;
                SetStyle(font.Style);
                Color = font.Color;
            }
            if (Util.EqualsIgnoreCase(this.fontName, "unknown")) {
                return;
            }

            if (document != null) {
                SetRtfDocument(document);
            }
        }

        /**
        * Writes the font definition
        */
        public virtual void WriteDefinition(Stream result) {
            byte[] t;
            result.Write(FONT_FAMILY, 0, FONT_FAMILY.Length);
            result.Write(FONT_CHARSET, 0, FONT_CHARSET.Length);
            result.Write(t = IntToByteArray(charset), 0, t.Length);
            result.Write(RtfElement.DELIMITER, 0, RtfElement.DELIMITER.Length);
            document.FilterSpecialChar(result, fontName, true, false);
        }

        /**
        * Writes the font beginning
        *
        * @return A byte array with the font start data
        */
        public virtual void WriteBegin(Stream result) {
            byte[] t;
            if(this.fontNumber != Font.UNDEFINED) {
                result.Write(RtfFontList.FONT_NUMBER, 0, RtfFontList.FONT_NUMBER.Length);
                result.Write(t = IntToByteArray(fontNumber), 0, t.Length);
            }
            if(this.fontSize != Font.UNDEFINED) {
                result.Write(FONT_SIZE, 0, FONT_SIZE.Length);
                result.Write(t = IntToByteArray(fontSize * 2), 0, t.Length);
            }
            if (this.fontStyle != UNDEFINED) {
                if ((fontStyle & STYLE_BOLD) == STYLE_BOLD) {
                    result.Write(FONT_BOLD, 0, FONT_BOLD.Length);
                }
                if ((fontStyle & STYLE_ITALIC) == STYLE_ITALIC) {
                    result.Write(FONT_ITALIC, 0, FONT_ITALIC.Length);
                }
                if ((fontStyle & STYLE_UNDERLINE) == STYLE_UNDERLINE) {
                    result.Write(FONT_UNDERLINE, 0, FONT_UNDERLINE.Length);
                }
                if ((fontStyle & STYLE_STRIKETHROUGH) == STYLE_STRIKETHROUGH) {
                    result.Write(FONT_STRIKETHROUGH, 0, FONT_STRIKETHROUGH.Length);
                }
                if ((fontStyle & STYLE_HIDDEN) == STYLE_HIDDEN) {
                    result.Write(FONT_HIDDEN, 0, FONT_HIDDEN.Length);
                }
                if ((fontStyle & STYLE_DOUBLE_STRIKETHROUGH) == STYLE_DOUBLE_STRIKETHROUGH) {
                    result.Write(FONT_DOUBLE_STRIKETHROUGH, 0, FONT_DOUBLE_STRIKETHROUGH.Length);
                    result.Write(t = IntToByteArray(1), 0, t.Length);
                }
                if ((fontStyle & STYLE_SHADOW) == STYLE_SHADOW) {
                    result.Write(FONT_SHADOW, 0, FONT_SHADOW.Length);
                }
                if ((fontStyle & STYLE_OUTLINE) == STYLE_OUTLINE) {
                    result.Write(FONT_OUTLINE, 0, FONT_OUTLINE.Length);
                }
                if ((fontStyle & STYLE_EMBOSSED) == STYLE_EMBOSSED) {
                    result.Write(FONT_EMBOSSED, 0, FONT_EMBOSSED.Length);
                }
                if ((fontStyle & STYLE_ENGRAVED) == STYLE_ENGRAVED) {
                    result.Write(FONT_ENGRAVED, 0, FONT_ENGRAVED.Length);
                }
            }
            if(color != null) {
                color.WriteBegin(result);
            }
        }

        /**
        * Write the font end
        *
        */
        public virtual void WriteEnd(Stream result) {
            byte[] t;
            if (this.fontStyle != UNDEFINED) {
                if ((fontStyle & STYLE_BOLD) == STYLE_BOLD) {
                    result.Write(FONT_BOLD, 0, FONT_BOLD.Length);
                    result.Write(t = IntToByteArray(0), 0, t.Length);
                }
                if ((fontStyle & STYLE_ITALIC) == STYLE_ITALIC) {
                    result.Write(FONT_ITALIC, 0, FONT_ITALIC.Length);
                    result.Write(t = IntToByteArray(0), 0, t.Length);
                }
                if ((fontStyle & STYLE_UNDERLINE) == STYLE_UNDERLINE) {
                    result.Write(FONT_UNDERLINE, 0, FONT_UNDERLINE.Length);
                    result.Write(t = IntToByteArray(0), 0, t.Length);
                }
                if ((fontStyle & STYLE_STRIKETHROUGH) == STYLE_STRIKETHROUGH) {
                    result.Write(FONT_STRIKETHROUGH, 0, FONT_STRIKETHROUGH.Length);
                    result.Write(t = IntToByteArray(0), 0, t.Length);
                }
                if ((fontStyle & STYLE_HIDDEN) == STYLE_HIDDEN) {
                    result.Write(FONT_HIDDEN, 0, FONT_HIDDEN.Length);
                    result.Write(t = IntToByteArray(0), 0, t.Length);
                }
                if ((fontStyle & STYLE_DOUBLE_STRIKETHROUGH) == STYLE_DOUBLE_STRIKETHROUGH) {
                    result.Write(FONT_DOUBLE_STRIKETHROUGH, 0, FONT_DOUBLE_STRIKETHROUGH.Length);
                    result.Write(t = IntToByteArray(0), 0, t.Length);
                }
                if ((fontStyle & STYLE_SHADOW) == STYLE_SHADOW) {
                    result.Write(FONT_SHADOW, 0, FONT_SHADOW.Length);
                    result.Write(t = IntToByteArray(0), 0, t.Length);
                }
                if ((fontStyle & STYLE_OUTLINE) == STYLE_OUTLINE) {
                    result.Write(FONT_OUTLINE, 0, FONT_OUTLINE.Length);
                    result.Write(t = IntToByteArray(0), 0, t.Length);
                }
                if ((fontStyle & STYLE_EMBOSSED) == STYLE_EMBOSSED) {
                    result.Write(FONT_EMBOSSED, 0, FONT_EMBOSSED.Length);
                    result.Write(t = IntToByteArray(0), 0, t.Length);
                }
                if ((fontStyle & STYLE_ENGRAVED) == STYLE_ENGRAVED) {
                    result.Write(FONT_ENGRAVED, 0, FONT_ENGRAVED.Length);
                    result.Write(t = IntToByteArray(0), 0, t.Length);
                }
            }
        }

        /**
        * unused
        */
        public virtual void WriteContent(Stream outp) {       
        }
        
        /**
        * Tests for equality of RtfFonts. RtfFonts are equal if their fontName,
        * fontSize, fontStyle and fontSuperSubscript are equal
        * 
        * @param obj The RtfFont to compare with this RtfFont
        * @return <code>True</code> if the RtfFonts are equal, <code>false</code> otherwise
        */
        public override bool Equals(Object obj) {
            if (!(obj is RtfFont)) {
                return false;
            }
            RtfFont font = (RtfFont) obj;
            bool result = true;
            result = result & this.fontName.Equals(font.GetFontName());
            return result;
        }

        /**
        * Returns the hash code of this RtfFont. The hash code is the hash code of the
        * string containing the font name + font size + "-" + the font style + "-" + the
        * font super/supscript value.
        * 
        * @return The hash code of this RtfFont
        */
        public override int GetHashCode() {
            return (this.fontName + this.fontSize + "-" + this.fontStyle).GetHashCode();
        }
        
        /**
        * Gets the font name of this RtfFont
        * 
        * @return The font name
        */
        public String GetFontName() {
            return this.fontName;
        }

        /**
        * Sets the font name of this RtfFont.
        * 
        * @param fontName The font name to use 
        */
        public virtual void SetFontName(String fontName) {
            this.fontName = fontName;
            if(document != null) {
                this.fontNumber = document.GetDocumentHeader().GetFontNumber(this);
            }
        }
        /**
        * @see com.lowagie.text.Font#getFamilyname()
        */
        public override String Familyname {
            get {
                return this.fontName;
            }
        }
        
        /**
        * @see com.lowagie.text.Font#setFamily(String)
        */
        public override void SetFamily(String family){
            base.SetFamily(family);
            SetToDefaultFamily(family);
        }
        
        /**
        * Sets the correct font name from the family name.
        * 
        * @param familyname The family name to set the name to.
        */
        private void SetToDefaultFamily(String familyname){
            switch (Font.GetFamilyIndex(familyname)) {
                case Font.COURIER:
                    this.fontName = "Courier";
                    break;
                case Font.HELVETICA:
                    this.fontName = "Arial";
                    break;
                case Font.SYMBOL:
                    this.fontName = "Symbol";
                    this.charset = 2;
                    break;
                case Font.TIMES_ROMAN:
                    this.fontName = "Times New Roman";
                    break;
                case Font.ZAPFDINGBATS:
                    this.fontName = "Windings";
                    break;
                default:
                    this.fontName = familyname;
                    break;
            }
        }
        
        /**
        * Gets the font size of this RtfFont
        * 
        * @return The font size
        */
        public int GetFontSize() {
            return this.fontSize;
        }
        
        /**
        * @see com.lowagie.text.Font#setSize(float)
        */
        public override float Size {
            set {
                base.Size = value;
                this.fontSize = (int)Size;
            }
        }

        /**
        * Gets the font style of this RtfFont
        * 
        * @return The font style
        */
        public int GetFontStyle() {
            return this.fontStyle;
        }
        
        /**
        * @see com.lowagie.text.Font#setStyle(int)
        */
        public override void SetStyle(int style){
            base.SetStyle(style);
            this.fontStyle = Style;
        }
        
        /**
        * @see com.lowagie.text.Font#setStyle(String)
        */
        public override void SetStyle(String style) {
            base.SetStyle(style);
            fontStyle = Style;
        }

        /**
        * Gets the charset used for constructing this RtfFont.
        * 
        * @return The charset of this RtfFont.
        */
        public int GetCharset() {
            return charset;
        }

        /**
        * Sets the charset used for constructing this RtfFont.
        * 
        * @param charset The charset to use.
        */
        public void SetCharset(int charset) {
            this.charset = charset;
        }

        /**
        * Gets the font number of this RtfFont
        * 
        * @return The font number
        */
        public int GetFontNumber() {
            return fontNumber;
        }

        /**
        * Sets the RtfDocument this RtfFont belongs to
        * 
        * @param doc The RtfDocument to use
        */
        public void SetRtfDocument(RtfDocument doc) {
            this.document = doc;
            if (document != null) {
                this.fontNumber = document.GetDocumentHeader().GetFontNumber(this);
            }
            if (this.color != null) {
                this.color.SetRtfDocument(this.document);
            }
        }

        /**
        * Unused
        * @param inTable
        */
        public void SetInTable(bool inTable) {
        }
        
        /**
        * Unused
        * @param inHeader
        */
        public void SetInHeader(bool inHeader) {
        }

        /**
        * @see com.lowagie.text.Font#setColor(Color)
        */
        public override Color Color {
            set {
                base.Color = value;
                if(value != null) {
                    this.color = new RtfColor(document, value);
                } else {
                    this.color = null;
                }
            }
        }
        
        /**
        * @see com.lowagie.text.Font#setColor(int, int, int)
        */
        public override void SetColor(int red, int green, int blue) {
            base.SetColor(red,green,blue);
            this.color = new RtfColor(document, red, green, blue);
        }

        /**
        * Transforms an integer into its String representation and then returns the bytes
        * of that string.
        *
        * @param i The integer to convert
        * @return A byte array representing the integer
        */
        protected byte[] IntToByteArray(int i) {
            return DocWriter.GetISOBytes(i.ToString());
        }

        /**
        * Replaces the attributes that are equal to <VAR>null</VAR> with
        * the attributes of a given font.
        *
        * @param font The surrounding font
        * @return A RtfFont
        */
        public override Font Difference(Font font) {
            String dFamilyname = font.Familyname;
            if (dFamilyname == null || dFamilyname.Trim().Equals("") || Util.EqualsIgnoreCase(dFamilyname.Trim(), "unknown")) {
                dFamilyname = this.fontName;
            }

            float dSize = font.Size;
            if (dSize == Font.UNDEFINED) {
                dSize = this.Size;
            }

            int dStyle = Font.UNDEFINED;
            if (this.Style != Font.UNDEFINED && font.Style != Font.UNDEFINED) {
                dStyle = this.Style | font.Style;
            } else if (this.Style != Font.UNDEFINED) {
                dStyle = this.Style;
            } else if (font.Style != Font.UNDEFINED) {
                dStyle = font.Style;
            }

            Color dColor = font.Color;
            if (dColor == null) {
                dColor = this.Color;
            }

            int dCharset = this.charset;
            if(font is RtfFont) {
                dCharset = ((RtfFont)font).GetCharset();
            }
            
            return new RtfFont(dFamilyname, dSize, dStyle, dColor, dCharset);
        }

        /**
        * The <code>RtfFont</code> is never a standard font.
        * 
        * @since 2.1.0
        */
        public override bool IsStandardFont() {
            return false;
        }
        
        /**
        * Compares this <code>RtfFont</code> to either a {@link com.lowagie.text.Font} or
        * an <code>RtfFont</code>.
        * 
        * @since 2.1.0
        */
        public override int CompareTo(Object obj) {
            if (obj == null) {
                return -1;
            }
            if(obj is RtfFont) {
                if(this.GetFontName().CompareTo(((RtfFont) obj).GetFontName()) != 0) {
                    return 1;
                } else {
                    return base.CompareTo(obj);
                }
            } else if (obj is Font) {
                return base.CompareTo(obj);
            } else {
                return -3;
            }
        }
    }
}