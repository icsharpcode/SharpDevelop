using System;
using System.Globalization;
using System.Reflection;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Collections;
using System.util;
using iTextSharp.text.xml.simpleparser;

/*
 * $Id: BaseFont.cs,v 1.17 2008/05/13 11:25:17 psoares33 Exp $
 * 
 *
 * Copyright 2000-2006 by Paulo Soares.
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
 * LGPL license (the "GNU LIBRARY GENERAL PUBLIC LICENSE"), in which case the
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

namespace iTextSharp.text.pdf {
    /// <summary>
    /// Summary description for BaseFont.
    /// </summary>
    public abstract class BaseFont {
        /** This is a possible value of a base 14 type 1 font */
        public const string COURIER = "Courier";
    
        /** This is a possible value of a base 14 type 1 font */
        public const string COURIER_BOLD = "Courier-Bold";
    
        /** This is a possible value of a base 14 type 1 font */
        public const string COURIER_OBLIQUE = "Courier-Oblique";
    
        /** This is a possible value of a base 14 type 1 font */
        public const string COURIER_BOLDOBLIQUE = "Courier-BoldOblique";
    
        /** This is a possible value of a base 14 type 1 font */
        public const string HELVETICA = "Helvetica";
    
        /** This is a possible value of a base 14 type 1 font */
        public const string HELVETICA_BOLD = "Helvetica-Bold";
    
        /** This is a possible value of a base 14 type 1 font */
        public const string HELVETICA_OBLIQUE = "Helvetica-Oblique";
    
        /** This is a possible value of a base 14 type 1 font */
        public const string HELVETICA_BOLDOBLIQUE = "Helvetica-BoldOblique";
    
        /** This is a possible value of a base 14 type 1 font */
        public const string SYMBOL = "Symbol";
    
        /** This is a possible value of a base 14 type 1 font */
        public const string TIMES_ROMAN = "Times-Roman";
    
        /** This is a possible value of a base 14 type 1 font */
        public const string TIMES_BOLD = "Times-Bold";
    
        /** This is a possible value of a base 14 type 1 font */
        public const string TIMES_ITALIC = "Times-Italic";
    
        /** This is a possible value of a base 14 type 1 font */
        public const string TIMES_BOLDITALIC = "Times-BoldItalic";
    
        /** This is a possible value of a base 14 type 1 font */
        public const string ZAPFDINGBATS = "ZapfDingbats";
    
        /** The maximum height above the baseline reached by glyphs in this
         * font, excluding the height of glyphs for accented characters.
         */    
        public const int ASCENT = 1;    
        /** The y coordinate of the top of flat capital letters, measured from
         * the baseline.
         */    
        public const int CAPHEIGHT = 2;
        /** The maximum depth below the baseline reached by glyphs in this
         * font. The value is a negative number.
         */    
        public const int DESCENT = 3;
        /** The angle, expressed in degrees counterclockwise from the vertical,
         * of the dominant vertical strokes of the font. The value is
         * negative for fonts that slope to the right, as almost all italic fonts do.
         */    
        public const int ITALICANGLE = 4;
        /** The lower left x glyph coordinate.
         */    
        public const int BBOXLLX = 5;
        /** The lower left y glyph coordinate.
         */    
        public const int BBOXLLY = 6;
        /** The upper right x glyph coordinate.
         */    
        public const int BBOXURX = 7;
        /** The upper right y glyph coordinate.
         */    
        public const int BBOXURY = 8;
        /** java.awt.Font property */
        public const int AWT_ASCENT = 9;
        /** java.awt.Font property */
        public const int AWT_DESCENT = 10;
        /** java.awt.Font property */
        public const int AWT_LEADING = 11;
        /** java.awt.Font property */
        public const int AWT_MAXADVANCE = 12;    
        /**
        * The undeline position. Usually a negative value.
        */
        public const int UNDERLINE_POSITION = 13;
        /**
        * The underline thickness.
        */
        public const int UNDERLINE_THICKNESS = 14;
        /**
        * The strikethrough position.
        */
        public const int STRIKETHROUGH_POSITION = 15;
        /**
        * The strikethrough thickness.
        */
        public const int STRIKETHROUGH_THICKNESS = 16;
        /**
        * The recommended vertical size for subscripts for this font.
        */
        public const int SUBSCRIPT_SIZE = 17;
        /**
        * The recommended vertical offset from the baseline for subscripts for this font. Usually a negative value.
        */
        public const int SUBSCRIPT_OFFSET = 18;
        /**
        * The recommended vertical size for superscripts for this font.
        */
        public const int SUPERSCRIPT_SIZE = 19;
        /**
        * The recommended vertical offset from the baseline for superscripts for this font.
        */
        public const int SUPERSCRIPT_OFFSET = 20;
    
        /** The font is Type 1.
         */    
        public const int FONT_TYPE_T1 = 0;
        /** The font is True Type with a standard encoding.
         */    
        public const int FONT_TYPE_TT = 1;
        /** The font is CJK.
         */    
        public const int FONT_TYPE_CJK = 2;
        /** The font is True Type with a Unicode encoding.
         */    
        public const int FONT_TYPE_TTUNI = 3;
        /** A font already inside the document.
        */    
        public const int FONT_TYPE_DOCUMENT = 4;
        /** A Type3 font.
        */    
        public const int FONT_TYPE_T3 = 5;

        /** The Unicode encoding with horizontal writing.
         */    
        public const string IDENTITY_H = "Identity-H";
        /** The Unicode encoding with vertical writing.
         */    
        public const string IDENTITY_V = "Identity-V";
    
        /** A possible encoding. */    
        public const string CP1250 = "Cp1250";
    
        /** A possible encoding. */    
        public const string CP1252 = "Cp1252";
    
        /** A possible encoding. */    
        public const string CP1257 = "Cp1257";
    
        /** A possible encoding. */    
        public const string WINANSI = "Cp1252";
    
        /** A possible encoding. */    
        public const string MACROMAN = "MacRoman";
    
        public static readonly int[] CHAR_RANGE_LATIN = {0, 0x17f, 0x2000, 0x206f, 0x20a0, 0x20cf, 0xfb00, 0xfb06};
        public static readonly int[] CHAR_RANGE_ARABIC = {0, 0x7f, 0x0600, 0x067f, 0x20a0, 0x20cf, 0xfb50, 0xfbff, 0xfe70, 0xfeff};
        public static readonly int[] CHAR_RANGE_HEBREW = {0, 0x7f, 0x0590, 0x05ff, 0x20a0, 0x20cf, 0xfb1d, 0xfb4f};
        public static readonly int[] CHAR_RANGE_CYRILLIC = {0, 0x7f, 0x0400, 0x052f, 0x2000, 0x206f, 0x20a0, 0x20cf};
    
        /** if the font has to be embedded */
        public const bool EMBEDDED = true;
    
        /** if the font doesn't have to be embedded */
        public const bool NOT_EMBEDDED = false;
        /** if the font has to be cached */
        public const bool CACHED = true;
        /** if the font doesn't have to be cached */
        public const bool NOT_CACHED = false;
    
        /** The path to the font resources. */    
        public const string RESOURCE_PATH = "iTextSharp.text.pdf.fonts.";
        /** The fake CID code that represents a newline. */    
        public const char CID_NEWLINE = '\u7fff';

        protected ArrayList subsetRanges;

        /** The font type.
         */    
        internal int fontType;
        /** a not defined character in a custom PDF encoding */
        public const string notdef = ".notdef";
    
        /** table of characters widths for this encoding */
        protected int[] widths = new int[256];
    
        /** encoding names */
        protected string[] differences = new string[256];
        /** same as differences but with the unicode codes */
        protected char[] unicodeDifferences = new char[256];
        protected int[][] charBBoxes = new int[256][];
        /** encoding used with this font */
        protected string encoding;
    
        /** true if the font is to be embedded in the PDF */
        protected bool embedded;
    
        /**
         * true if the font must use it's built in encoding. In that case the
         * <CODE>encoding</CODE> is only used to map a char to the position inside
         * the font, not to the expected char name.
         */
        protected bool fontSpecific = true;
    
        /** cache for the fonts already used. */
        protected static Hashtable fontCache = new Hashtable();
    
        /** list of the 14 built in fonts. */
        protected static Hashtable BuiltinFonts14 = new Hashtable();
    
        /** Forces the output of the width array. Only matters for the 14
         * built-in fonts.
         */
        protected bool forceWidthsOutput = false;
    
        /** Converts <CODE>char</CODE> directly to <CODE>byte</CODE>
         * by casting.
         */
        protected bool directTextToByte = false;
    
        /** Indicates if all the glyphs and widths for that particular
         * encoding should be included in the document.
         */
        protected bool subset = true;
    
        protected bool fastWinansi = false;

        /**
        * Custom encodings use this map to key the Unicode character
        * to the single byte code.
        */
        protected IntHashtable specialMap;

        protected static internal ArrayList resourceSearch = ArrayList.Synchronized(new ArrayList());

        private static Random random = new Random();

        static BaseFont() {
            BuiltinFonts14.Add(COURIER, PdfName.COURIER);
            BuiltinFonts14.Add(COURIER_BOLD, PdfName.COURIER_BOLD);
            BuiltinFonts14.Add(COURIER_BOLDOBLIQUE, PdfName.COURIER_BOLDOBLIQUE);
            BuiltinFonts14.Add(COURIER_OBLIQUE, PdfName.COURIER_OBLIQUE);
            BuiltinFonts14.Add(HELVETICA, PdfName.HELVETICA);
            BuiltinFonts14.Add(HELVETICA_BOLD, PdfName.HELVETICA_BOLD);
            BuiltinFonts14.Add(HELVETICA_BOLDOBLIQUE, PdfName.HELVETICA_BOLDOBLIQUE);
            BuiltinFonts14.Add(HELVETICA_OBLIQUE, PdfName.HELVETICA_OBLIQUE);
            BuiltinFonts14.Add(SYMBOL, PdfName.SYMBOL);
            BuiltinFonts14.Add(TIMES_ROMAN, PdfName.TIMES_ROMAN);
            BuiltinFonts14.Add(TIMES_BOLD, PdfName.TIMES_BOLD);
            BuiltinFonts14.Add(TIMES_BOLDITALIC, PdfName.TIMES_BOLDITALIC);
            BuiltinFonts14.Add(TIMES_ITALIC, PdfName.TIMES_ITALIC);
            BuiltinFonts14.Add(ZAPFDINGBATS, PdfName.ZAPFDINGBATS);
        }
    
        /** Generates the PDF stream with the Type1 and Truetype fonts returning
         * a PdfStream.
         */
        internal class StreamFont : PdfStream {
        
            /** Generates the PDF stream with the Type1 and Truetype fonts returning
             * a PdfStream.
             * @param contents the content of the stream
             * @param lengths an array of int that describes the several lengths of each part of the font
             */
            internal StreamFont(byte[] contents, int[] lengths) {
                bytes = contents;
                Put(PdfName.LENGTH, new PdfNumber(bytes.Length));
                for (int k = 0; k < lengths.Length; ++k) {
                    Put(new PdfName("Length" + (k + 1)), new PdfNumber(lengths[k]));
                }
                FlateCompress();
            }
        
            internal StreamFont(byte[] contents, string subType) {
                bytes = contents;
                Put(PdfName.LENGTH, new PdfNumber(bytes.Length));
                if (subType != null)
                    Put(PdfName.SUBTYPE, new PdfName(subType));
                FlateCompress();
            }
        }
    
        /**
         *Creates new BaseFont
         */
        protected BaseFont() {
        }
    
        /**
        * Creates a new font. This will always be the default Helvetica font (not embedded).
        * This method is introduced because Helvetica is used in many examples.
        * @return  a BaseFont object (Helvetica, Winansi, not embedded)
        * @throws  IOException         This shouldn't occur ever
        * @throws  DocumentException   This shouldn't occur ever
        * @since   2.1.1 
        */
        public static BaseFont CreateFont() {
            return CreateFont(BaseFont.HELVETICA, BaseFont.WINANSI, BaseFont.NOT_EMBEDDED);
        }
                /**
        * Creates a new font. This font can be one of the 14 built in types,
        * a Type1 font referred to by an AFM or PFM file, a TrueType font (simple or collection) or a CJK font from the
        * Adobe Asian Font Pack. TrueType fonts and CJK fonts can have an optional style modifier
        * appended to the name. These modifiers are: Bold, Italic and BoldItalic. An
        * example would be "STSong-Light,Bold". Note that this modifiers do not work if
        * the font is embedded. Fonts in TrueType collections are addressed by index such as "msgothic.ttc,1".
        * This would get the second font (indexes start at 0), in this case "MS PGothic".
        * <P>
        * The fonts are cached and if they already exist they are extracted from the cache,
        * not parsed again.
        * <P>
        * Besides the common encodings described by name, custom encodings 
        * can also be made. These encodings will only work for the single byte fonts
        * Type1 and TrueType. The encoding string starts with a '#'
        * followed by "simple" or "full". If "simple" there is a decimal for the first character position and then a list
        * of hex values representing the Unicode codes that compose that encoding.<br>
        * The "simple" encoding is recommended for TrueType fonts
        * as the "full" encoding risks not matching the character with the right glyph
        * if not done with care.<br>
        * The "full" encoding is specially aimed at Type1 fonts where the glyphs have to be
        * described by non standard names like the Tex math fonts. Each group of three elements
        * compose a code position: the one byte code order in decimal or as 'x' (x cannot be the space), the name and the Unicode character
        * used to access the glyph. The space must be assigned to character position 32 otherwise
        * text justification will not work.
        * <P>
        * Example for a "simple" encoding that includes the Unicode
        * character space, A, B and ecyrillic:
        * <PRE>
        * "# simple 32 0020 0041 0042 0454"
        * </PRE>
        * <P>
        * Example for a "full" encoding for a Type1 Tex font:
        * <PRE>
        * "# full 'A' nottriangeqlleft 0041 'B' dividemultiply 0042 32 space 0020"
        * </PRE>
        * <P>
        * This method calls:<br>
        * <PRE>
        * createFont(name, encoding, embedded, true, null, null);
        * </PRE>
        * @param name the name of the font or it's location on file
        * @param encoding the encoding to be applied to this font
        * @param embedded true if the font is to be embedded in the PDF
        * @return returns a new font. This font may come from the cache
        * @throws DocumentException the font is invalid
        * @throws IOException the font file could not be read
        */
        public static BaseFont CreateFont(string name, string encoding, bool embedded) {
            return CreateFont(name, encoding, embedded, true, null, null);
        }
    
        /** Creates a new font. This font can be one of the 14 built in types,
        * a Type1 font referred to by an AFM or PFM file, a TrueType font (simple or collection) or a CJK font from the
        * Adobe Asian Font Pack. TrueType fonts and CJK fonts can have an optional style modifier
        * appended to the name. These modifiers are: Bold, Italic and BoldItalic. An
        * example would be "STSong-Light,Bold". Note that this modifiers do not work if
        * the font is embedded. Fonts in TrueType collections are addressed by index such as "msgothic.ttc,1".
        * This would get the second font (indexes start at 0), in this case "MS PGothic".
        * <P>
        * The fonts may or may not be cached depending on the flag <CODE>cached</CODE>.
        * If the <CODE>byte</CODE> arrays are present the font will be
        * read from them instead of the name. A name is still required to identify
        * the font type.
        * <P>
        * Besides the common encodings described by name, custom encodings 
        * can also be made. These encodings will only work for the single byte fonts
        * Type1 and TrueType. The encoding string starts with a '#'
        * followed by "simple" or "full". If "simple" there is a decimal for the first character position and then a list
        * of hex values representing the Unicode codes that compose that encoding.<br>
        * The "simple" encoding is recommended for TrueType fonts
        * as the "full" encoding risks not matching the character with the right glyph
        * if not done with care.<br>
        * The "full" encoding is specially aimed at Type1 fonts where the glyphs have to be
        * described by non standard names like the Tex math fonts. Each group of three elements
        * compose a code position: the one byte code order in decimal or as 'x' (x cannot be the space), the name and the Unicode character
        * used to access the glyph. The space must be assigned to character position 32 otherwise
        * text justification will not work.
        * <P>
        * Example for a "simple" encoding that includes the Unicode
        * character space, A, B and ecyrillic:
        * <PRE>
        * "# simple 32 0020 0041 0042 0454"
        * </PRE>
        * <P>
        * Example for a "full" encoding for a Type1 Tex font:
        * <PRE>
        * "# full 'A' nottriangeqlleft 0041 'B' dividemultiply 0042 32 space 0020"
        * </PRE>
        * @param name the name of the font or it's location on file
        * @param encoding the encoding to be applied to this font
        * @param embedded true if the font is to be embedded in the PDF
        * @param cached true if the font comes from the cache or is added to
        * the cache if new, false if the font is always created new
        * @param ttfAfm the true type font or the afm in a byte array
        * @param pfb the pfb in a byte array
        * @return returns a new font. This font may come from the cache but only if cached
        * is true, otherwise it will always be created new
        * @throws DocumentException the font is invalid
        * @throws IOException the font file could not be read
        */
        public static BaseFont CreateFont(string name, string encoding, bool embedded, bool cached, byte[] ttfAfm, byte[] pfb) {
            return CreateFont(name, encoding, embedded, cached, ttfAfm, pfb, false);
        }

        /** Creates a new font. This font can be one of the 14 built in types,
        * a Type1 font referred to by an AFM or PFM file, a TrueType font (simple or collection) or a CJK font from the
        * Adobe Asian Font Pack. TrueType fonts and CJK fonts can have an optional style modifier
        * appended to the name. These modifiers are: Bold, Italic and BoldItalic. An
        * example would be "STSong-Light,Bold". Note that this modifiers do not work if
        * the font is embedded. Fonts in TrueType collections are addressed by index such as "msgothic.ttc,1".
        * This would get the second font (indexes start at 0), in this case "MS PGothic".
        * <P>
        * The fonts may or may not be cached depending on the flag <CODE>cached</CODE>.
        * If the <CODE>byte</CODE> arrays are present the font will be
        * read from them instead of the name. A name is still required to identify
        * the font type.
        * <P>
        * Besides the common encodings described by name, custom encodings 
        * can also be made. These encodings will only work for the single byte fonts
        * Type1 and TrueType. The encoding string starts with a '#'
        * followed by "simple" or "full". If "simple" there is a decimal for the first character position and then a list
        * of hex values representing the Unicode codes that compose that encoding.<br>
        * The "simple" encoding is recommended for TrueType fonts
        * as the "full" encoding risks not matching the character with the right glyph
        * if not done with care.<br>
        * The "full" encoding is specially aimed at Type1 fonts where the glyphs have to be
        * described by non standard names like the Tex math fonts. Each group of three elements
        * compose a code position: the one byte code order in decimal or as 'x' (x cannot be the space), the name and the Unicode character
        * used to access the glyph. The space must be assigned to character position 32 otherwise
        * text justification will not work.
        * <P>
        * Example for a "simple" encoding that includes the Unicode
        * character space, A, B and ecyrillic:
        * <PRE>
        * "# simple 32 0020 0041 0042 0454"
        * </PRE>
        * <P>
        * Example for a "full" encoding for a Type1 Tex font:
        * <PRE>
        * "# full 'A' nottriangeqlleft 0041 'B' dividemultiply 0042 32 space 0020"
        * </PRE>
        * @param name the name of the font or it's location on file
        * @param encoding the encoding to be applied to this font
        * @param embedded true if the font is to be embedded in the PDF
        * @param cached true if the font comes from the cache or is added to
        * the cache if new, false if the font is always created new
        * @param ttfAfm the true type font or the afm in a byte array
        * @param pfb the pfb in a byte array
        * @param noThrow if true will not throw an exception if the font is not recognized and will return null, if false will throw
        * an exception if the font is not recognized. Note that even if true an exception may be thrown in some circumstances.
        * This parameter is useful for FontFactory that may have to check many invalid font names before finding the right one
        * @return returns a new font. This font may come from the cache but only if cached
        * is true, otherwise it will always be created new
        * @throws DocumentException the font is invalid
        * @throws IOException the font file could not be read
        */
        public static BaseFont CreateFont(string name, string encoding, bool embedded, bool cached, byte[] ttfAfm, byte[] pfb, bool noThrow) {
            string nameBase = GetBaseName(name);
            encoding = NormalizeEncoding(encoding);
            bool isBuiltinFonts14 = BuiltinFonts14.ContainsKey(name);
            bool isCJKFont = isBuiltinFonts14 ? false : CJKFont.IsCJKFont(nameBase, encoding);
            if (isBuiltinFonts14 || isCJKFont)
                embedded = false;
            else if (encoding.Equals(IDENTITY_H) || encoding.Equals(IDENTITY_V))
                embedded = true;
            BaseFont fontFound = null;
            BaseFont fontBuilt = null;
            string key = name + "\n" + encoding + "\n" + embedded;
            if (cached) {
                lock (fontCache) {
                    fontFound = (BaseFont)fontCache[key];
                }
                if (fontFound != null)
                    return fontFound;
            }
            if (isBuiltinFonts14 || name.ToLower(System.Globalization.CultureInfo.InvariantCulture).EndsWith(".afm") || name.ToLower(System.Globalization.CultureInfo.InvariantCulture).EndsWith(".pfm")) {
                fontBuilt = new Type1Font(name, encoding, embedded, ttfAfm, pfb);
                fontBuilt.fastWinansi = encoding.Equals(CP1252);
            }
            else if (nameBase.ToLower(System.Globalization.CultureInfo.InvariantCulture).EndsWith(".ttf") || nameBase.ToLower(System.Globalization.CultureInfo.InvariantCulture).EndsWith(".otf") || nameBase.ToLower(System.Globalization.CultureInfo.InvariantCulture).IndexOf(".ttc,") > 0) {
                if (encoding.Equals(IDENTITY_H) || encoding.Equals(IDENTITY_V))
                    fontBuilt = new TrueTypeFontUnicode(name, encoding, embedded, ttfAfm);
                else {
                    fontBuilt = new TrueTypeFont(name, encoding, embedded, ttfAfm);
                    fontBuilt.fastWinansi = encoding.Equals(CP1252);
                }
            }
            else if (isCJKFont)
                fontBuilt = new CJKFont(name, encoding, embedded);
            else if (noThrow)
                return null;
            else
                throw new DocumentException("Font '" + name + "' with '" + encoding + "' is not recognized.");
            if (cached) {
                lock (fontCache) {
                    fontFound = (BaseFont)fontCache[key];
                    if (fontFound != null)
                        return fontFound;
                    fontCache.Add(key, fontBuilt);
                }
            }
            return fontBuilt;
        }
    
        /**
        * Creates a font based on an existing document font. The created font font may not
        * behave as expected, depending on the encoding or subset.
        * @param fontRef the reference to the document font
        * @return the font
        */    
        public static BaseFont CreateFont(PRIndirectReference fontRef) {
            return new DocumentFont(fontRef);
        }
        
        /**
         * Gets the name without the modifiers Bold, Italic or BoldItalic.
         * @param name the full name of the font
         * @return the name without the modifiers Bold, Italic or BoldItalic
         */
        protected static string GetBaseName(string name) {
            if (name.EndsWith(",Bold"))
                return name.Substring(0, name.Length - 5);
            else if (name.EndsWith(",Italic"))
                return name.Substring(0, name.Length - 7);
            else if (name.EndsWith(",BoldItalic"))
                return name.Substring(0, name.Length - 11);
            else
                return name;
        }
    
        /**
         * Normalize the encoding names. "winansi" is changed to "Cp1252" and
         * "macroman" is changed to "MacRoman".
         * @param enc the encoding to be normalized
         * @return the normalized encoding
         */
        protected static string NormalizeEncoding(string enc) {
            if (enc.Equals("winansi") || enc.Equals(""))
                return CP1252;
            else if (enc.Equals("macroman"))
                return MACROMAN;
            int n = IanaEncodings.GetEncodingNumber(enc);
            if (n == 1252)
                return CP1252;
            if (n == 10000)
                return MACROMAN;
            return enc;
        }
    
        /**
         * Creates the <CODE>widths</CODE> and the <CODE>differences</CODE> arrays
         * @throws UnsupportedEncodingException the encoding is not supported
         */
        protected void CreateEncoding() {
            if (encoding.StartsWith("#")) {
                specialMap = new IntHashtable();
                StringTokenizer tok = new StringTokenizer(encoding.Substring(1), " ,\t\n\r\f");
                if (tok.NextToken().Equals("full")) {
                    while (tok.HasMoreTokens()) {
                        String order = tok.NextToken();
                        String name = tok.NextToken();
                        char uni = (char)int.Parse(tok.NextToken(), NumberStyles.HexNumber);
                        int orderK;
                        if (order.StartsWith("'"))
                            orderK = (int)order[1];
                        else
                            orderK = int.Parse(order);
                        orderK %= 256;
                        specialMap[(int)uni] = orderK;
                        differences[orderK] = name;
                        unicodeDifferences[orderK] = uni;
                        widths[orderK] = GetRawWidth((int)uni, name);
                        charBBoxes[orderK] = GetRawCharBBox((int)uni, name);
                    }
                }
                else {
                    int k = 0;
                    if (tok.HasMoreTokens())
                        k = int.Parse(tok.NextToken());
                    while (tok.HasMoreTokens() && k < 256) {
                        String hex = tok.NextToken();
                        int uni = int.Parse(hex, NumberStyles.HexNumber) % 0x10000;
                        String name = GlyphList.UnicodeToName(uni);
                        if (name != null) {
                            specialMap[uni] = k;
                            differences[k] = name;
                            unicodeDifferences[k] = (char)uni;
                            widths[k] = GetRawWidth(uni, name);
                            charBBoxes[k] = GetRawCharBBox(uni, name);
                            ++k;
                        }
                    }
                }
                for (int k = 0; k < 256; ++k) {
                    if (differences[k] == null) {
                        differences[k] = notdef;
                    }
                }
            }
            else if (fontSpecific) {
                for (int k = 0; k < 256; ++k) {
                    widths[k] = GetRawWidth(k, null);
                    charBBoxes[k] = GetRawCharBBox(k, null);
                }
            }
            else {
                string s;
                string name;
                char c;
                byte[] b = new byte[1];
                for (int k = 0; k < 256; ++k) {
                    b[0] = (byte)k;
                    s = PdfEncodings.ConvertToString(b, encoding);
                    if (s.Length > 0) {
                        c = s[0];
                    }
                    else {
                        c = '?';
                    }
                    name = GlyphList.UnicodeToName((int)c);
                    if (name == null)
                        name = notdef;
                    differences[k] = name;
                    this.UnicodeDifferences[k] = c;
                    widths[k] = GetRawWidth((int)c, name);
                    charBBoxes[k] = GetRawCharBBox((int)c, name);
                }
            }
        }
    
        /**
         * Gets the width from the font according to the Unicode char <CODE>c</CODE>
         * or the <CODE>name</CODE>. If the <CODE>name</CODE> is null it's a symbolic font.
         * @param c the unicode char
         * @param name the glyph name
         * @return the width of the char
         */
        internal abstract int GetRawWidth(int c, string name);
    
        /**
         * Gets the kerning between two Unicode chars.
         * @param char1 the first char
         * @param char2 the second char
         * @return the kerning to be applied
         */
        public abstract int GetKerning(int char1, int char2);
    
        /**
        * Sets the kerning between two Unicode chars.
        * @param char1 the first char
        * @param char2 the second char
        * @param kern the kerning to apply in normalized 1000 units
        * @return <code>true</code> if the kerning was applied, <code>false</code> otherwise
        */
        public abstract bool SetKerning(int char1, int char2, int kern);

        /**
         * Gets the width of a <CODE>char</CODE> in normalized 1000 units.
         * @param char1 the unicode <CODE>char</CODE> to get the width of
         * @return the width in normalized 1000 units
         */
        public virtual int GetWidth(int char1) {
            if (fastWinansi) {
                if (char1 < 128 || (char1 >= 160 && char1 <= 255))
                    return widths[char1];
                else
                    return widths[PdfEncodings.winansi[char1]];
            }
            else {
                int total = 0;
                byte[] mbytes = ConvertToBytes((char)char1);
                for (int k = 0; k < mbytes.Length; ++k)
                    total += widths[0xff & mbytes[k]];
                return total;
            }
        }
    
        /**
         * Gets the width of a <CODE>string</CODE> in normalized 1000 units.
         * @param text the <CODE>string</CODE> to get the witdth of
         * @return the width in normalized 1000 units
         */
        public virtual int GetWidth(string text) {
            int total = 0;
            if (fastWinansi) {
                int len = text.Length;
                for (int k = 0; k < len; ++k) {
                    char char1 = text[k];
                    if (char1 < 128 || (char1 >= 160 && char1 <= 255))
                        total += widths[char1];
                    else
                        total += widths[PdfEncodings.winansi[char1]];
                }
                return total;
            }
            else {
                byte[] mbytes = ConvertToBytes(text);
                for (int k = 0; k < mbytes.Length; ++k)
                    total += widths[0xff & mbytes[k]];
            }
            return total;
        }
    
        /**
        * Gets the descent of a <CODE>String</CODE> in normalized 1000 units. The descent will always be
        * less than or equal to zero even if all the characters have an higher descent.
        * @param text the <CODE>String</CODE> to get the descent of
        * @return the dexcent in normalized 1000 units
        */
        public int GetDescent(String text) {
            int min = 0;
            char[] chars = text.ToCharArray();
            for (int k = 0; k < chars.Length; ++k) {
                int[] bbox = GetCharBBox(chars[k]);
                if (bbox != null && bbox[1] < min)
                    min = bbox[1];
            }
            return min;
        }
            
        /**
        * Gets the ascent of a <CODE>String</CODE> in normalized 1000 units. The ascent will always be
        * greater than or equal to zero even if all the characters have a lower ascent.
        * @param text the <CODE>String</CODE> to get the ascent of
        * @return the ascent in normalized 1000 units
        */
        public int GetAscent(String text) {
            int max = 0;
            char[] chars = text.ToCharArray();
            for (int k = 0; k < chars.Length; ++k) {
                int[] bbox = GetCharBBox(chars[k]);
                if (bbox != null && bbox[3] > max)
                    max = bbox[3];
            }
            return max;
        }

        /**
        * Gets the descent of a <CODE>String</CODE> in points. The descent will always be
        * less than or equal to zero even if all the characters have an higher descent.
        * @param text the <CODE>String</CODE> to get the descent of
        * @param fontSize the size of the font
        * @return the dexcent in points
        */
        public float GetDescentPoint(String text, float fontSize)
        {
            return (float)GetDescent(text) * 0.001f * fontSize;
        }
            
        /**
        * Gets the ascent of a <CODE>String</CODE> in points. The ascent will always be
        * greater than or equal to zero even if all the characters have a lower ascent.
        * @param text the <CODE>String</CODE> to get the ascent of
        * @param fontSize the size of the font
        * @return the ascent in points
        */
        public float GetAscentPoint(String text, float fontSize)
        {
            return (float)GetAscent(text) * 0.001f * fontSize;
        }
        
        /**
        * Gets the width of a <CODE>String</CODE> in points taking kerning
        * into account.
        * @param text the <CODE>String</CODE> to get the witdth of
        * @param fontSize the font size
        * @return the width in points
        */
        public float GetWidthPointKerned(String text, float fontSize) {
            float size = (float)GetWidth(text) * 0.001f * fontSize;
            if (!HasKernPairs())
                return size;
            int len = text.Length - 1;
            int kern = 0;
            char[] c = text.ToCharArray();
            for (int k = 0; k < len; ++k) {
                kern += GetKerning(c[k], c[k + 1]);
            }
            return size + kern * 0.001f * fontSize;
        }
        
        /**
         * Gets the width of a <CODE>string</CODE> in points.
         * @param text the <CODE>string</CODE> to get the witdth of
         * @param fontSize the font size
         * @return the width in points
         */
        public float GetWidthPoint(string text, float fontSize) {
            return (float)GetWidth(text) * 0.001f * fontSize;
        }
    
        /**
         * Gets the width of a <CODE>char</CODE> in points.
         * @param char1 the <CODE>char</CODE> to get the witdth of
         * @param fontSize the font size
         * @return the width in points
         */
        public float GetWidthPoint(int char1, float fontSize) {
            return GetWidth(char1) * 0.001f * fontSize;
        }
    
        /**
         * Converts a <CODE>string</CODE> to a </CODE>byte</CODE> array according
         * to the font's encoding.
         * @param text the <CODE>string</CODE> to be converted
         * @return an array of <CODE>byte</CODE> representing the conversion according to the font's encoding
         */
        internal virtual byte[] ConvertToBytes(string text) {
            if (directTextToByte)
                return PdfEncodings.ConvertToBytes(text, null);
            if (specialMap != null) {
                byte[] b = new byte[text.Length];
                int ptr = 0;
                int length = text.Length;
                for (int k = 0; k < length; ++k) {
                    char c = text[k];
                    if (specialMap.ContainsKey((int)c))
                        b[ptr++] = (byte)specialMap[(int)c];
                }
                if (ptr < length) {
                    byte[] b2 = new byte[ptr];
                    System.Array.Copy(b, 0, b2, 0, ptr);
                    return b2;
                }
                else
                    return b;
            }
            return PdfEncodings.ConvertToBytes(text, encoding);
        }
    
        /**
        * Converts a <CODE>char</CODE> to a </CODE>byte</CODE> array according
        * to the font's encoding.
        * @param text the <CODE>String</CODE> to be converted
        * @return an array of <CODE>byte</CODE> representing the conversion according to the font's encoding
        */
        internal virtual byte[] ConvertToBytes(int char1) {
            if (directTextToByte)
                return PdfEncodings.ConvertToBytes((char)char1, null);
            if (specialMap != null) {
                if (specialMap.ContainsKey(char1))
                    return new byte[]{(byte)specialMap[(int)char1]};
                else
                    return new byte[0];
            }
            return PdfEncodings.ConvertToBytes((char)char1, encoding);
        }
        
        /** Outputs to the writer the font dictionaries and streams.
         * @param writer the writer for this document
         * @param ref the font indirect reference
         * @param params several parameters that depend on the font type
         * @throws IOException on error
         * @throws DocumentException error in generating the object
         */
        internal abstract void WriteFont(PdfWriter writer, PdfIndirectReference piRef, Object[] oParams);
    
        /** Gets the encoding used to convert <CODE>string</CODE> into <CODE>byte[]</CODE>.
         * @return the encoding name
         */
        public string Encoding {
            get {
                return encoding;
            }
        }
    
        /** Gets the font parameter identified by <CODE>key</CODE>. Valid values
         * for <CODE>key</CODE> are <CODE>ASCENT</CODE>, <CODE>CAPHEIGHT</CODE>, <CODE>DESCENT</CODE>,
         * <CODE>ITALICANGLE</CODE>, <CODE>BBOXLLX</CODE>, <CODE>BBOXLLY</CODE>, <CODE>BBOXURX</CODE>
         * and <CODE>BBOXURY</CODE>.
         * @param key the parameter to be extracted
         * @param fontSize the font size in points
         * @return the parameter in points
         */
        public abstract float GetFontDescriptor(int key, float fontSize);
    
        /** Gets the font type. The font types can be: FONT_TYPE_T1,
         * FONT_TYPE_TT, FONT_TYPE_CJK and FONT_TYPE_TTUNI.
         * @return the font type
         */
        public int FontType {
            get {
                return fontType;
            }

            set {
                fontType = value;
            }
        }
    
        /** Gets the embedded flag.
         * @return <CODE>true</CODE> if the font is embedded.
         */
        public bool IsEmbedded() {
            return embedded;
        }
    
        /** Gets the symbolic flag of the font.
         * @return <CODE>true</CODE> if the font is symbolic
         */
        public bool IsFontSpecific() {
            return fontSpecific;
        }
    
        /** Creates a unique subset prefix to be added to the font name when the font is embedded and subset.
         * @return the subset prefix
         */
        internal static string CreateSubsetPrefix() {
            char[] s = new char[7];
            lock (random) {
                for (int k = 0; k < 6; ++k)
                    s[k] = (char)(random.Next('A', 'Z' + 1));
            }
            s[6] = '+';
            return new String(s);
        }
    
        /** Gets the Unicode character corresponding to the byte output to the pdf stream.
         * @param index the byte index
         * @return the Unicode character
         */
        internal char GetUnicodeDifferences(int index) {
            return unicodeDifferences[index];
        }
    
        /** Gets the postscript font name.
         * @return the postscript font name
         */
        public abstract string PostscriptFontName {
            get;
            set;
        }
    
        /** Gets the full name of the font. If it is a True Type font
         * each array element will have {Platform ID, Platform Encoding ID,
         * Language ID, font name}. The interpretation of this values can be
         * found in the Open Type specification, chapter 2, in the 'name' table.<br>
         * For the other fonts the array has a single element with {"", "", "",
         * font name}.
         * @return the full name of the font
         */
        public abstract string[][] FullFontName {
            get;
        }
    
        /** Gets all the entries of the names-table. If it is a True Type font
        * each array element will have {Name ID, Platform ID, Platform Encoding ID,
        * Language ID, font name}. The interpretation of this values can be
        * found in the Open Type specification, chapter 2, in the 'name' table.<br>
        * For the other fonts the array has a single element with {"4", "", "", "",
        * font name}.
        * @return the full name of the font
        */
        public abstract string[][] AllNameEntries{
            get;
        }

        /** Gets the full name of the font. If it is a True Type font
         * each array element will have {Platform ID, Platform Encoding ID,
         * Language ID, font name}. The interpretation of this values can be
         * found in the Open Type specification, chapter 2, in the 'name' table.<br>
         * For the other fonts the array has a single element with {"", "", "",
         * font name}.
         * @param name the name of the font
         * @param encoding the encoding of the font
         * @param ttfAfm the true type font or the afm in a byte array
         * @throws DocumentException on error
         * @throws IOException on error
         * @return the full name of the font
         */    
        public static string[][] GetFullFontName(string name, string encoding, byte[] ttfAfm) {
            string nameBase = GetBaseName(name);
            BaseFont fontBuilt = null;
            if (nameBase.ToLower(System.Globalization.CultureInfo.InvariantCulture).EndsWith(".ttf") || nameBase.ToLower(System.Globalization.CultureInfo.InvariantCulture).EndsWith(".otf") || nameBase.ToLower(System.Globalization.CultureInfo.InvariantCulture).IndexOf(".ttc,") > 0)
                fontBuilt = new TrueTypeFont(name, CP1252, false, ttfAfm, true);
            else
                fontBuilt = CreateFont(name, encoding, false, false, ttfAfm, null);
            return fontBuilt.FullFontName;
        }
    
        /** Gets all the names from the font. Only the required tables are read.
        * @param name the name of the font
        * @param encoding the encoding of the font
        * @param ttfAfm the true type font or the afm in a byte array
        * @throws DocumentException on error
        * @throws IOException on error
        * @return an array of Object[] built with {getPostscriptFontName(), GetFamilyFontName(), GetFullFontName()}
        */    
        public static Object[] GetAllFontNames(String name, String encoding, byte[] ttfAfm) {
            String nameBase = GetBaseName(name);
            BaseFont fontBuilt = null;
            if (nameBase.ToLower(System.Globalization.CultureInfo.InvariantCulture).EndsWith(".ttf") || nameBase.ToLower(System.Globalization.CultureInfo.InvariantCulture).EndsWith(".otf") || nameBase.ToLower(System.Globalization.CultureInfo.InvariantCulture).IndexOf(".ttc,") > 0)
                fontBuilt = new TrueTypeFont(name, CP1252, false, ttfAfm, true);
            else
                fontBuilt = CreateFont(name, encoding, false, false, ttfAfm, null);
            return new Object[]{fontBuilt.PostscriptFontName, fontBuilt.FamilyFontName, fontBuilt.FullFontName};
        }

        /** Gets all the entries of the namestable from the font. Only the required tables are read.
        * @param name the name of the font
        * @param encoding the encoding of the font
        * @param ttfAfm the true type font or the afm in a byte array
        * @throws DocumentException on error
        * @throws IOException on error
        * @return an array of Object[] built with {getPostscriptFontName(), getFamilyFontName(), getFullFontName()}
        */
        public static String[][] GetAllNameEntries(String name, String encoding, byte[] ttfAfm) {
            String nameBase = GetBaseName(name);
            BaseFont fontBuilt = null;
            if (nameBase.ToLower(System.Globalization.CultureInfo.InvariantCulture).EndsWith(".ttf") || nameBase.ToLower(System.Globalization.CultureInfo.InvariantCulture).EndsWith(".otf") || nameBase.ToLower(System.Globalization.CultureInfo.InvariantCulture).IndexOf(".ttc,") > 0)
                fontBuilt = new TrueTypeFont(name, CP1252, false, ttfAfm, true);
            else
                fontBuilt = CreateFont(name, encoding, false, false, ttfAfm, null);
            return fontBuilt.AllNameEntries;
        }
        
        /** Gets the family name of the font. If it is a True Type font
         * each array element will have {Platform ID, Platform Encoding ID,
         * Language ID, font name}. The interpretation of this values can be
         * found in the Open Type specification, chapter 2, in the 'name' table.<br>
         * For the other fonts the array has a single element with {"", "", "",
         * font name}.
         * @return the family name of the font
         */
        public abstract string[][] FamilyFontName {
            get;
        }
    
        /** Gets the code pages supported by the font. This has only meaning
         * with True Type fonts.
         * @return the code pages supported by the font
         */
        public virtual string[] CodePagesSupported {
            get {
                return new string[0];
            }
        }
    
        /** Enumerates the postscript font names present inside a
         * True Type Collection.
         * @param ttcFile the file name of the font
         * @throws DocumentException on error
         * @throws IOException on error
         * @return the postscript font names
         */    
        public static string[] EnumerateTTCNames(string ttcFile) {
            return new EnumerateTTC(ttcFile).Names;
        }

        /** Enumerates the postscript font names present inside a
         * True Type Collection.
         * @param ttcArray the font as a <CODE>byte</CODE> array
         * @throws DocumentException on error
         * @throws IOException on error
         * @return the postscript font names
         */    
        public static string[] EnumerateTTCNames(byte[] ttcArray) {
            return new EnumerateTTC(ttcArray).Names;
        }
    
        /** Gets the font width array.
         * @return the font width array
         */    
        public int[] Widths {
            get {
                return widths;
            }
        }

        /** Gets the array with the names of the characters.
         * @return the array with the names of the characters
         */    
        public string[] Differences {
            get {
                return differences;
            }
        }

        /** Gets the array with the unicode characters.
         * @return the array with the unicode characters
         */    
        public char[] UnicodeDifferences {
            get {
                return unicodeDifferences;
            }
        }
    
        /** Set to <CODE>true</CODE> to force the generation of the
         * widths array.
         * @param forceWidthsOutput <CODE>true</CODE> to force the generation of the
         * widths array
         */
        public bool ForceWidthsOutput {
            set {
                this.forceWidthsOutput = value;
            }
            get {
                return forceWidthsOutput;
            }
        }
    
        /** Sets the conversion of <CODE>char</CODE> directly to <CODE>byte</CODE>
         * by casting. This is a low level feature to put the bytes directly in
         * the content stream without passing through string.GetBytes().
         * @param directTextToByte New value of property directTextToByte.
         */
        public bool DirectTextToByte {
            set {
                this.directTextToByte = value;
            }
            get {
                return directTextToByte;
            }
        }
    
        /** Indicates if all the glyphs and widths for that particular
        * encoding should be included in the document. When set to <CODE>true</CODE>
        * only the glyphs used will be included in the font. When set to <CODE>false</CODE>
        * and {@link #addSubsetRange(int[])} was not called the full font will be included
        * otherwise just the characters ranges will be included.
        * @param subset new value of property subset
        */
        public bool Subset {
            set {
                this.subset = value;
            }
            get {
                return subset;
            }
        }

        public static void AddToResourceSearch (object obj) {
            if (obj is Assembly) {
                resourceSearch.Add(obj);
            }
            else if (obj is string) {
                string f = (string)obj;
                if (Directory.Exists(f) || File.Exists(f))
                    resourceSearch.Add(obj);
            }
        }

        /** Gets the font resources.
         * @param key the name of the resource
         * @return the <CODE>Stream</CODE> to get the resource or
         * <CODE>null</CODE> if not found
         */    
        public static Stream GetResourceStream(string key) {
            Stream istr = null;
            // Try to use resource loader to load the properties file.
            try {
                Assembly assm = Assembly.GetExecutingAssembly();
                istr = assm.GetManifestResourceStream(key);
            }
            catch {
            }
            if (istr != null)
                return istr;
            for (int k = 0; k < resourceSearch.Count; ++k) {
                object obj = resourceSearch[k];
                try {
                    if (obj is Assembly) {
                        istr = ((Assembly)obj).GetManifestResourceStream(key);
                        if (istr != null)
                            return istr;
                    }
                    else if (obj is string) {
                        string dir = (string)obj;
                        try {
                            istr = Assembly.LoadFrom(dir).GetManifestResourceStream(key);
                        }
                        catch {
                        }
                        if (istr != null)
                            return istr;
                        string modkey = key.Replace('.', '/');
                        string fullPath = Path.Combine(dir, modkey);
                        if (File.Exists(fullPath)) {
                            return new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read);
                        }
                        int idx = modkey.LastIndexOf('/');
                        if (idx >= 0) {
                            modkey = modkey.Substring(0, idx) + "." + modkey.Substring(idx + 1);
                            fullPath = Path.Combine(dir, modkey);
                            if (File.Exists(fullPath))
                                return new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read);
                        }
                    }
                }
                catch {
                }
            }

            return istr;
        }
    
        /** Gets the Unicode equivalent to a CID.
         * The (inexistent) CID <FF00> is translated as '\n'. 
         * It has only meaning with CJK fonts with Identity encoding.
         * @param c the CID code
         * @return the Unicode equivalent
         */    
        public virtual int GetUnicodeEquivalent(int c) {
            return c;
        }
    
        /** Gets the CID code given an Unicode.
         * It has only meaning with CJK fonts.
         * @param c the Unicode
         * @return the CID equivalent
         */    
        public virtual int GetCidCode(int c) {
            return c;
        }

        /** Checks if the font has any kerning pairs.
        * @return <CODE>true</CODE> if the font has any kerning pairs
        */    
        public abstract bool HasKernPairs();
        
        /**
        * Checks if a character exists in this font.
        * @param c the character to check
        * @return <CODE>true</CODE> if the character has a glyph,
        * <CODE>false</CODE> otherwise
        */    
        public virtual bool CharExists(int c) {
            byte[] b = ConvertToBytes(c);
            return b.Length > 0;
        }
        
        /**
        * Sets the character advance.
        * @param c the character
        * @param advance the character advance normalized to 1000 units
        * @return <CODE>true</CODE> if the advance was set,
        * <CODE>false</CODE> otherwise
        */    
        public virtual bool SetCharAdvance(int c, int advance) {
            byte[] b = ConvertToBytes(c);
            if (b.Length == 0)
                return false;
            widths[0xff & b[0]] = advance;
            return true;
        }
        
        private static void AddFont(PRIndirectReference fontRef, IntHashtable hits, ArrayList fonts) {
            PdfObject obj = PdfReader.GetPdfObject(fontRef);
            if (obj == null || !obj.IsDictionary())
                return;
            PdfDictionary font = (PdfDictionary)obj;
            PdfName subtype = (PdfName)PdfReader.GetPdfObject(font.Get(PdfName.SUBTYPE));
            if (!PdfName.TYPE1.Equals(subtype) && !PdfName.TRUETYPE.Equals(subtype))
                return;
            PdfName name = (PdfName)PdfReader.GetPdfObject(font.Get(PdfName.BASEFONT));
            fonts.Add(new Object[]{PdfName.DecodeName(name.ToString()), fontRef});
            hits[fontRef.Number] = 1;
        }
        
        private static void RecourseFonts(PdfDictionary page, IntHashtable hits, ArrayList fonts, int level) {
            ++level;
            if (level > 50) // in case we have an endless loop
                return;
            PdfDictionary resources = (PdfDictionary)PdfReader.GetPdfObject(page.Get(PdfName.RESOURCES));
            if (resources == null)
                return;
            PdfDictionary font = (PdfDictionary)PdfReader.GetPdfObject(resources.Get(PdfName.FONT));
            if (font != null) {
                foreach (PdfName key in font.Keys) {
                    PdfObject ft = font.Get(key);        
                    if (ft == null || !ft.IsIndirect())
                        continue;
                    int hit = ((PRIndirectReference)ft).Number;
                    if (hits.ContainsKey(hit))
                        continue;
                    AddFont((PRIndirectReference)ft, hits, fonts);
                }
            }
            PdfDictionary xobj = (PdfDictionary)PdfReader.GetPdfObject(resources.Get(PdfName.XOBJECT));
            if (xobj != null) {
                foreach (PdfName key in xobj.Keys) {
                    RecourseFonts((PdfDictionary)PdfReader.GetPdfObject(xobj.Get(key)), hits, fonts, level);
                }
            }
        }
        
        /**
        * Gets a list of all document fonts. Each element of the <CODE>ArrayList</CODE>
        * contains a <CODE>Object[]{String,PRIndirectReference}</CODE> with the font name
        * and the indirect reference to it.
        * @param reader the document where the fonts are to be listed from
        * @return the list of fonts and references
        */    
        public static ArrayList GetDocumentFonts(PdfReader reader) {
            IntHashtable hits = new IntHashtable();
            ArrayList fonts = new ArrayList();
            int npages = reader.NumberOfPages;
            for (int k = 1; k <= npages; ++k)
                RecourseFonts(reader.GetPageN(k), hits, fonts, 1);
            return fonts;
        }
        
        /**
        * Gets a list of the document fonts in a particular page. Each element of the <CODE>ArrayList</CODE>
        * contains a <CODE>Object[]{String,PRIndirectReference}</CODE> with the font name
        * and the indirect reference to it.
        * @param reader the document where the fonts are to be listed from
        * @param page the page to list the fonts from
        * @return the list of fonts and references
        */    
        public static ArrayList GetDocumentFonts(PdfReader reader, int page) {
            IntHashtable hits = new IntHashtable();
            ArrayList fonts = new ArrayList();
            RecourseFonts(reader.GetPageN(page), hits, fonts, 1);
            return fonts;
        }
        
        /**
        * Gets the smallest box enclosing the character contours. It will return
        * <CODE>null</CODE> if the font has not the information or the character has no
        * contours, as in the case of the space, for example. Characters with no contours may
        * also return [0,0,0,0].
        * @param c the character to get the contour bounding box from
        * @return an array of four floats with the bounding box in the format [llx,lly,urx,ury] or
        * <code>null</code>
        */    
        public virtual int[] GetCharBBox(int c) {
            byte[] b = ConvertToBytes(c);
            if (b.Length == 0)
                return null;
            else
                return charBBoxes[b[0] & 0xff];
        }
        
        protected abstract int[] GetRawCharBBox(int c, String name);

        /**
        * iText expects Arabic Diactrics (tashkeel) to have zero advance but some fonts,
        * most notably those that come with Windows, like times.ttf, have non-zero
        * advance for those characters. This method makes those character to have zero
        * width advance and work correctly in the iText Arabic shaping and reordering
        * context.
        */    
        public void CorrectArabicAdvance() {
            for (char c = '\u064b'; c <= '\u0658'; ++c)
                SetCharAdvance(c, 0);
            SetCharAdvance('\u0670', 0);
            for (char c = '\u06d6'; c <= '\u06dc'; ++c)
                SetCharAdvance(c, 0);
            for (char c = '\u06df'; c <= '\u06e4'; ++c)
                SetCharAdvance(c, 0);
            for (char c = '\u06e7'; c <= '\u06e8'; ++c)
                SetCharAdvance(c, 0);
            for (char c = '\u06ea'; c <= '\u06ed'; ++c)
                SetCharAdvance(c, 0);
        }

        /**
        * Adds a character range when subsetting. The range is an <CODE>int</CODE> array
        * where the first element is the start range inclusive and the second element is the
        * end range inclusive. Several ranges are allowed in the same array.
        * @param range the character range
        */
        public void AddSubsetRange(int[] range) {
            if (subsetRanges == null)
                subsetRanges = new ArrayList();
            subsetRanges.Add(range);
        }
    }
}