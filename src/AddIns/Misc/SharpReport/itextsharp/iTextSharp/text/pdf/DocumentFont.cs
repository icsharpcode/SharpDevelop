using System;
using System.Collections;
/*
 * Copyright 2004 by Paulo Soares.
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
    public class DocumentFont : BaseFont {
        
        // code, [glyph, width]
        private Hashtable metrics = new Hashtable();
        private String fontName;
        private PRIndirectReference refFont;
        private PdfDictionary font;
        private IntHashtable uni2byte = new IntHashtable();
        private IntHashtable byte2uni = new IntHashtable();
        private float Ascender = 800;
        private float CapHeight = 700;
        private float Descender = -200;
        private float ItalicAngle = 0;
        private float llx = -50;
        private float lly = -200;
        private float urx = 100;
        private float ury = 900;
        private bool isType0 = false;
        
        private BaseFont cjkMirror;
        
        private static String[] cjkNames = {"HeiseiMin-W3", "HeiseiKakuGo-W5", "STSong-Light", "MHei-Medium",
            "MSung-Light", "HYGoThic-Medium", "HYSMyeongJo-Medium", "MSungStd-Light", "STSongStd-Light",
            "HYSMyeongJoStd-Medium", "KozMinPro-Regular"};
            
        private static String[] cjkEncs = {"UniJIS-UCS2-H", "UniJIS-UCS2-H", "UniGB-UCS2-H", "UniCNS-UCS2-H",
            "UniCNS-UCS2-H", "UniKS-UCS2-H", "UniKS-UCS2-H", "UniCNS-UCS2-H", "UniGB-UCS2-H",
            "UniKS-UCS2-H", "UniJIS-UCS2-H"};
            
        private static String[] cjkNames2 = {"MSungStd-Light", "STSongStd-Light", "HYSMyeongJoStd-Medium", "KozMinPro-Regular"};
            
        private static String[] cjkEncs2 = {"UniCNS-UCS2-H", "UniGB-UCS2-H", "UniKS-UCS2-H", "UniJIS-UCS2-H",
            "UniCNS-UTF16-H", "UniGB-UTF16-H", "UniKS-UTF16-H", "UniJIS-UTF16-H"};

        private static int[] stdEnc = {
            0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
            0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
            32,33,34,35,36,37,38,8217,40,41,42,43,44,45,46,47,
            48,49,50,51,52,53,54,55,56,57,58,59,60,61,62,63,
            64,65,66,67,68,69,70,71,72,73,74,75,76,77,78,79,
            80,81,82,83,84,85,86,87,88,89,90,91,92,93,94,95,
            8216,97,98,99,100,101,102,103,104,105,106,107,108,109,110,111,
            112,113,114,115,116,117,118,119,120,121,122,123,124,125,126,0,
            0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
            0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
            0,161,162,163,8260,165,402,167,164,39,8220,171,8249,8250,64257,64258,
            0,8211,8224,8225,183,0,182,8226,8218,8222,8221,187,8230,8240,0,191,
            0,96,180,710,732,175,728,729,168,0,730,184,0,733,731,711,
            8212,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
            0,198,0,170,0,0,0,0,321,216,338,186,0,0,0,0,
            0,230,0,0,0,305,0,0,322,248,339,223,0,0,0,0};

        /** Creates a new instance of DocumentFont */
        internal DocumentFont(PRIndirectReference refFont) {
            encoding = "";
            fontSpecific = false;
            this.refFont = refFont;
            fontType = FONT_TYPE_DOCUMENT;
            font = (PdfDictionary)PdfReader.GetPdfObject(refFont);
            fontName = PdfName.DecodeName(((PdfName)PdfReader.GetPdfObject(font.Get(PdfName.BASEFONT))).ToString());
            PdfName subType = (PdfName)PdfReader.GetPdfObject(font.Get(PdfName.SUBTYPE));
            if (PdfName.TYPE1.Equals(subType) || PdfName.TRUETYPE.Equals(subType))
                DoType1TT();
            else {
                for (int k = 0; k < cjkNames.Length; ++k) {
                    if (fontName.StartsWith(cjkNames[k])) {
                        fontName = cjkNames[k];
                        cjkMirror = BaseFont.CreateFont(fontName, cjkEncs[k], false);
                        return;
                    }
                }
                String enc = PdfName.DecodeName(((PdfName)PdfReader.GetPdfObject(font.Get(PdfName.ENCODING))).ToString());
                for (int k = 0; k < cjkEncs2.Length; ++k) {
                    if (enc.StartsWith(cjkEncs2[k])) {
                        if (k > 3)
                            k -= 4;
                        cjkMirror = BaseFont.CreateFont(cjkNames2[k], cjkEncs2[k], false);
                        return;
                    }
                }
                if (PdfName.TYPE0.Equals(subType) && enc.Equals("Identity-H")) {
                    ProcessType0(font);
                    isType0 = true;
                }
            }
        }
        
        private void ProcessType0(PdfDictionary font) {
            byte[] touni = PdfReader.GetStreamBytes((PRStream)PdfReader.GetPdfObjectRelease(font.Get(PdfName.TOUNICODE)));
            PdfArray df = (PdfArray)PdfReader.GetPdfObjectRelease(font.Get(PdfName.DESCENDANTFONTS));
            PdfDictionary cidft = (PdfDictionary)PdfReader.GetPdfObjectRelease((PdfObject)df.ArrayList[0]);
            PdfNumber dwo = (PdfNumber)PdfReader.GetPdfObjectRelease(cidft.Get(PdfName.DW));
            int dw = 1000;
            if (dwo != null)
                dw = dwo.IntValue;
            IntHashtable widths = ReadWidths((PdfArray)PdfReader.GetPdfObjectRelease(cidft.Get(PdfName.W)));
            PdfDictionary fontDesc = (PdfDictionary)PdfReader.GetPdfObjectRelease(cidft.Get(PdfName.FONTDESCRIPTOR));
            FillFontDesc(fontDesc);
            FillMetrics(touni, widths, dw);
        }
        
        private IntHashtable ReadWidths(PdfArray ws) {
            IntHashtable hh = new IntHashtable();
            if (ws == null)
                return hh;
            ArrayList ar = ws.ArrayList;
            for (int k = 0; k < ar.Count; ++k) {
                int c1 = ((PdfNumber)PdfReader.GetPdfObjectRelease((PdfObject)ar[k])).IntValue;
                PdfObject obj = PdfReader.GetPdfObjectRelease((PdfObject)ar[++k]);
                if (obj.IsArray()) {
                    ArrayList ar2 = ((PdfArray)obj).ArrayList;
                    for (int j = 0; j < ar2.Count; ++j) {
                        int c2 = ((PdfNumber)PdfReader.GetPdfObjectRelease((PdfObject)ar2[j])).IntValue;
                        hh[c1++] = c2;
                    }
                }
                else {
                    int c2 = ((PdfNumber)obj).IntValue;
                    int w = ((PdfNumber)PdfReader.GetPdfObjectRelease((PdfObject)ar[++k])).IntValue;
                    for (; c1 <= c2; ++c1)
                        hh[c1] = w;
                }
            }
            return hh;
        }
        
        private String DecodeString(PdfString ps) {
            if (ps.IsHexWriting())
                return PdfEncodings.ConvertToString(ps.GetBytes(), "UnicodeBigUnmarked");
            else
                return ps.ToUnicodeString();
        }
        
        private void FillMetrics(byte[] touni, IntHashtable widths, int dw) {
            PdfContentParser ps = new PdfContentParser(new PRTokeniser(touni));
            PdfObject ob = null;
            PdfObject last = null;
            while ((ob = ps.ReadPRObject()) != null) {
                if (ob.Type == PdfContentParser.COMMAND_TYPE) {
                    if (ob.ToString().Equals("beginbfchar")) {
                        int n = ((PdfNumber)last).IntValue;
                        for (int k = 0; k < n; ++k) {
                            String cid = DecodeString((PdfString)ps.ReadPRObject());
                            String uni = DecodeString((PdfString)ps.ReadPRObject());
                            if (uni.Length == 1) {
                                int cidc = (int)cid[0];
                                int unic = (int)uni[uni.Length - 1];
                                int w = dw;
                                if (widths.ContainsKey(cidc))
                                    w = widths[cidc];
                                metrics[unic] = new int[]{cidc, w};
                            }
                        }
                    }
                    else if (ob.ToString().Equals("beginbfrange")) {
                        int n = ((PdfNumber)last).IntValue;
                        for (int k = 0; k < n; ++k) {
                            String cid1 = DecodeString((PdfString)ps.ReadPRObject());
                            String cid2 = DecodeString((PdfString)ps.ReadPRObject());
                            int cid1c = (int)cid1[0];
                            int cid2c = (int)cid2[0];
                            PdfObject ob2 = ps.ReadPRObject();
                            if (ob2.IsString()) {
                                String uni = DecodeString((PdfString)ob2);
                                if (uni.Length == 1) {
                                    int unic = (int)uni[uni.Length - 1];
                                    for (; cid1c <= cid2c; cid1c++, unic++) {
                                        int w = dw;
                                        if (widths.ContainsKey(cid1c))
                                            w = widths[cid1c];
                                        metrics[unic] = new int[]{cid1c, w};
                                    }
                                }
                            }
                            else {
                                ArrayList ar = ((PdfArray)ob2).ArrayList;
                                for (int j = 0; j < ar.Count; ++j, ++cid1c) {
                                    String uni = DecodeString((PdfString)ar[j]);
                                    if (uni.Length == 1) {
                                        int unic = (int)uni[uni.Length - 1];
                                        int w = dw;
                                        if (widths.ContainsKey(cid1c))
                                            w = widths[cid1c];
                                        metrics[unic] = new int[]{cid1c, w};
                                    }
                                }
                            }
                        }                        
                    }
                }
                else
                    last = ob;
            }
        }

        private void DoType1TT() {
            PdfObject enc = PdfReader.GetPdfObject(font.Get(PdfName.ENCODING));
            if (enc == null)
                FillEncoding(null);
            else {
                if (enc.IsName())
                    FillEncoding((PdfName)enc);
                else {
                    PdfDictionary encDic = (PdfDictionary)enc;
                    enc = PdfReader.GetPdfObject(encDic.Get(PdfName.BASEENCODING));
                    if (enc == null)
                        FillEncoding(null);
                    else
                        FillEncoding((PdfName)enc);
                    PdfArray diffs = (PdfArray)PdfReader.GetPdfObject(encDic.Get(PdfName.DIFFERENCES));
                    if (diffs != null) {
                        ArrayList dif = diffs.ArrayList;
                        int currentNumber = 0;
                        for (int k = 0; k < dif.Count; ++k) {
                            PdfObject obj = (PdfObject)dif[k];
                            if (obj.IsNumber())
                                currentNumber = ((PdfNumber)obj).IntValue;
                            else {
                                int[] c = GlyphList.NameToUnicode(PdfName.DecodeName(((PdfName)obj).ToString()));
                                if (c != null && c.Length > 0) {
                                    if (byte2uni.ContainsKey(currentNumber)) {
                                        uni2byte.Remove(byte2uni[currentNumber]);
                                    }
                                    uni2byte[c[0]] = currentNumber;
                                    byte2uni[currentNumber] = c[0];
                                }
                                ++currentNumber;
                            }
                        }
                    }
                }
            }
            PdfArray newWidths = (PdfArray)PdfReader.GetPdfObject(font.Get(PdfName.WIDTHS));
            PdfNumber first = (PdfNumber)PdfReader.GetPdfObject(font.Get(PdfName.FIRSTCHAR));
            PdfNumber last = (PdfNumber)PdfReader.GetPdfObject(font.Get(PdfName.LASTCHAR));
            if (BuiltinFonts14.ContainsKey(fontName)) {
                BaseFont bf;
                    bf = BaseFont.CreateFont(fontName, WINANSI, false);
                int[] e = uni2byte.ToOrderedKeys();
                for (int k = 0; k < e.Length; ++k) {
                    int n = uni2byte[e[k]];
                    widths[n] = bf.GetRawWidth(n, GlyphList.UnicodeToName(e[k]));
                }
                Ascender = bf.GetFontDescriptor(ASCENT, 1000);
                CapHeight = bf.GetFontDescriptor(CAPHEIGHT, 1000);
                Descender = bf.GetFontDescriptor(DESCENT, 1000);
                ItalicAngle = bf.GetFontDescriptor(ITALICANGLE, 1000);
                llx = bf.GetFontDescriptor(BBOXLLX, 1000);
                lly = bf.GetFontDescriptor(BBOXLLY, 1000);
                urx = bf.GetFontDescriptor(BBOXURX, 1000);
                ury = bf.GetFontDescriptor(BBOXURY, 1000);
            }
            if (first != null && last != null && newWidths != null) {
                int f = first.IntValue;
                ArrayList ar = newWidths.ArrayList;
                for (int k = 0; k < ar.Count; ++k) {
                    widths[f + k] = ((PdfNumber)ar[k]).IntValue;
                }
            }
            FillFontDesc((PdfDictionary)PdfReader.GetPdfObject(font.Get(PdfName.FONTDESCRIPTOR)));
        }
        
        private void FillFontDesc(PdfDictionary fontDesc) {
            if (fontDesc == null)
                return;
            PdfNumber v = (PdfNumber)PdfReader.GetPdfObject(fontDesc.Get(PdfName.ASCENT));
            if (v != null)
                Ascender = v.FloatValue;
            v = (PdfNumber)PdfReader.GetPdfObject(fontDesc.Get(PdfName.CAPHEIGHT));
            if (v != null)
                CapHeight = v.FloatValue;
            v = (PdfNumber)PdfReader.GetPdfObject(fontDesc.Get(PdfName.DESCENT));
            if (v != null)
                Descender = v.FloatValue;
            v = (PdfNumber)PdfReader.GetPdfObject(fontDesc.Get(PdfName.ITALICANGLE));
            if (v != null)
                ItalicAngle = v.FloatValue;
            PdfArray bbox = (PdfArray)PdfReader.GetPdfObject(fontDesc.Get(PdfName.FONTBBOX));
            if (bbox != null) {
                ArrayList ar = bbox.ArrayList;
                llx = ((PdfNumber)ar[0]).FloatValue;
                lly = ((PdfNumber)ar[1]).FloatValue;
                urx = ((PdfNumber)ar[2]).FloatValue;
                ury = ((PdfNumber)ar[3]).FloatValue;
                if (llx > urx) {
                    float t = llx;
                    llx = urx;
                    urx = t;
                }
                if (lly > ury) {
                    float t = lly;
                    lly = ury;
                    ury = t;
                }
            }
        }
        
        private void FillEncoding(PdfName encoding) {
            if (PdfName.MAC_ROMAN_ENCODING.Equals(encoding) || PdfName.WIN_ANSI_ENCODING.Equals(encoding)) {
                byte[] b = new byte[256];
                for (int k = 0; k < 256; ++k)
                    b[k] = (byte)k;
                String enc = WINANSI;
                if (PdfName.MAC_ROMAN_ENCODING.Equals(encoding))
                    enc = MACROMAN;
                String cv = PdfEncodings.ConvertToString(b, enc);
                char[] arr = cv.ToCharArray();
                for (int k = 0; k < 256; ++k) {
                    uni2byte[arr[k]] = k;
                    byte2uni[k] = arr[k];
                }
            }
            else {
                for (int k = 0; k < 256; ++k) {
                    uni2byte[stdEnc[k]] = k;
                    byte2uni[k] = stdEnc[k];
                }
            }
        }
        
        /** Gets the family name of the font. If it is a True Type font
        * each array element will have {Platform ID, Platform Encoding ID,
        * Language ID, font name}. The interpretation of this values can be
        * found in the Open Type specification, chapter 2, in the 'name' table.<br>
        * For the other fonts the array has a single element with {"", "", "",
        * font name}.
        * @return the family name of the font
        *
        */
        public override string[][] FamilyFontName {
            get {
                return FullFontName;
            }
        }
        
        /** Gets the font parameter identified by <CODE>key</CODE>. Valid values
        * for <CODE>key</CODE> are <CODE>ASCENT</CODE>, <CODE>CAPHEIGHT</CODE>, <CODE>DESCENT</CODE>,
        * <CODE>ITALICANGLE</CODE>, <CODE>BBOXLLX</CODE>, <CODE>BBOXLLY</CODE>, <CODE>BBOXURX</CODE>
        * and <CODE>BBOXURY</CODE>.
        * @param key the parameter to be extracted
        * @param fontSize the font size in points
        * @return the parameter in points
        *
        */
        public override float GetFontDescriptor(int key, float fontSize) {
            if (cjkMirror != null)
                return cjkMirror.GetFontDescriptor(key, fontSize);
            switch (key) {
                case AWT_ASCENT:
                case ASCENT:
                    return Ascender * fontSize / 1000;
                case CAPHEIGHT:
                    return CapHeight * fontSize / 1000;
                case AWT_DESCENT:
                case DESCENT:
                    return Descender * fontSize / 1000;
                case ITALICANGLE:
                    return ItalicAngle;
                case BBOXLLX:
                    return llx * fontSize / 1000;
                case BBOXLLY:
                    return lly * fontSize / 1000;
                case BBOXURX:
                    return urx * fontSize / 1000;
                case BBOXURY:
                    return ury * fontSize / 1000;
                case AWT_LEADING:
                    return 0;
                case AWT_MAXADVANCE:
                    return (urx - llx) * fontSize / 1000;
            }
            return 0;
        }
        
        /** Gets the full name of the font. If it is a True Type font
        * each array element will have {Platform ID, Platform Encoding ID,
        * Language ID, font name}. The interpretation of this values can be
        * found in the Open Type specification, chapter 2, in the 'name' table.<br>
        * For the other fonts the array has a single element with {"", "", "",
        * font name}.
        * @return the full name of the font
        *
        */
        public override string[][] FullFontName {
            get {
                return new string[][]{new string[]{"", "", "", fontName}};
            }
        }
        
        /** Gets all the entries of the names-table. If it is a True Type font
        * each array element will have {Name ID, Platform ID, Platform Encoding ID,
        * Language ID, font name}. The interpretation of this values can be
        * found in the Open Type specification, chapter 2, in the 'name' table.<br>
        * For the other fonts the array has a single element with {"4", "", "", "",
        * font name}.
        * @return the full name of the font
        */
        public override string[][] AllNameEntries {
            get {
                return new string[][]{new string[]{"4", "", "", "", fontName}};
            }
        }

        /** Gets the kerning between two Unicode chars.
        * @param char1 the first char
        * @param char2 the second char
        * @return the kerning to be applied
        *
        */
        public override int GetKerning(int char1, int char2) {
            return 0;
        }
        
        /** Gets the postscript font name.
        * @return the postscript font name
        *
        */
        public override string PostscriptFontName {
            get {
                return fontName;
            }
            set {
            }
        }
        
        /** Gets the width from the font according to the Unicode char <CODE>c</CODE>
        * or the <CODE>name</CODE>. If the <CODE>name</CODE> is null it's a symbolic font.
        * @param c the unicode char
        * @param name the glyph name
        * @return the width of the char
        *
        */
        internal override int GetRawWidth(int c, String name) {
            return 0;
        }
        
        /** Checks if the font has any kerning pairs.
        * @return <CODE>true</CODE> if the font has any kerning pairs
        *
        */
        public override bool HasKernPairs() {
            return false;
        }
        
        /** Outputs to the writer the font dictionaries and streams.
        * @param writer the writer for this document
        * @param ref the font indirect reference
        * @param params several parameters that depend on the font type
        * @throws IOException on error
        * @throws DocumentException error in generating the object
        *
        */
        internal override void WriteFont(PdfWriter writer, PdfIndirectReference refi, Object[] param) {
        }

        /**
        * Gets the width of a <CODE>char</CODE> in normalized 1000 units.
        * @param char1 the unicode <CODE>char</CODE> to get the width of
        * @return the width in normalized 1000 units
        */
        public override int GetWidth(int char1) {
            if (cjkMirror != null)
                return cjkMirror.GetWidth(char1);
            else if (isType0) {
                int[] ws = (int[])metrics[(int)char1];
                if (ws != null)
                    return ws[1];
                else
                    return 0;
            }
            else
                return base.GetWidth(char1);
        }
        
        public override int GetWidth(String text) {
            if (cjkMirror != null)
                return cjkMirror.GetWidth(text);
            else if (isType0) {
                char[] chars = text.ToCharArray();
                int len = chars.Length;
                int total = 0;
                for (int k = 0; k < len; ++k) {
                    int[] ws = (int[])metrics[(int)chars[k]];
                    if (ws != null)
                        total += ws[1];
                }
                return total;
            }
            else
                return base.GetWidth(text);
        }
        
        internal override byte[] ConvertToBytes(String text) {
            if (cjkMirror != null)
                return PdfEncodings.ConvertToBytes(text, CJKFont.CJK_ENCODING);
            else if (isType0) {
                char[] chars = text.ToCharArray();
                int len = chars.Length;
                byte[] b = new byte[len * 2];
                int bptr = 0;
                for (int k = 0; k < len; ++k) {
                    int[] ws = (int[])metrics[(int)chars[k]];
                    if (ws != null) {
                        int g = ws[0];
                        b[bptr++] = (byte)(g / 256);
                        b[bptr++] = (byte)(g);
                    }
                }
                if (bptr == b.Length)
                    return b;
                else {
                    byte[] nb = new byte[bptr];
                    System.Array.Copy(b, 0, nb, 0, bptr);
                    return nb;
                }
            }
            else {
                char[] cc = text.ToCharArray();
                byte[] b = new byte[cc.Length];
                int ptr = 0;
                for (int k = 0; k < cc.Length; ++k) {
                    if (uni2byte.ContainsKey(cc[k]))
                        b[ptr++] = (byte)uni2byte[cc[k]];
                }
                if (ptr == b.Length)
                    return b;
                else {
                    byte[] b2 = new byte[ptr];
                    System.Array.Copy(b, 0, b2, 0, ptr);
                    return b2;
                }
            }
        }
        
        internal override byte[] ConvertToBytes(int char1) {
            if (cjkMirror != null)
                return PdfEncodings.ConvertToBytes((char)char1, CJKFont.CJK_ENCODING);
            else if (isType0) {
                int[] ws = (int[])metrics[(int)char1];
                if (ws != null) {
                    int g = ws[0];
                    return new byte[]{(byte)(g / 256), (byte)(g)};
                }
                else
                    return new byte[0];
            }
            else {
                if (uni2byte.ContainsKey(char1))
                    return new byte[]{(byte)uni2byte[char1]};
                else
                    return new byte[0];
            }
        }
        
        internal PdfIndirectReference IndirectReference {
            get {
                return refFont;
            }
        }
        
        public override bool CharExists(int c) {
            if (cjkMirror != null)
                return cjkMirror.CharExists(c);
            else if (isType0) {
                return metrics.ContainsKey((int)c);
            }
            else
                return base.CharExists(c);
        }
        
        public override bool SetKerning(int char1, int char2, int kern) {
            return false;
        }
        
        public override int[] GetCharBBox(int c) {
            return null;
        }
        
        protected override int[] GetRawCharBBox(int c, String name) {
            return null;
        }    
    }
}
