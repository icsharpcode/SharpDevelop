using System;
using System.Collections;
/*
 * Copyright 2005 by Paulo Soares.
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

    /**
    * A class to support Type3 fonts.
    */
    public class Type3Font : BaseFont {
        
        private bool[] usedSlot;
        private IntHashtable widths3 = new IntHashtable();
        private Hashtable char2glyph = new Hashtable();
        private PdfWriter writer;
        private float llx = float.NaN, lly, urx, ury;
        private PageResources pageResources = new PageResources();
        private bool colorized;
        
        /**
        * Creates a Type3 font.
        * @param writer the writer
        * @param chars an array of chars corresponding to the glyphs used (not used, prisent for compability only)
        * @param colorized if <CODE>true</CODE> the font may specify color, if <CODE>false</CODE> no color commands are allowed
        * and only images as masks can be used
        */    
        public Type3Font(PdfWriter writer, char[] chars, bool colorized) : this(writer, colorized) {
        }
        
        /**
        * Creates a Type3 font. This implementation assumes that the /FontMatrix is
        * [0.001 0 0 0.001 0 0] or a 1000-unit glyph coordinate system.
        * <p>
        * An example:
        * <p>
        * <pre>
        * Document document = new Document(PageSize.A4);
        * PdfWriter writer = PdfWriter.getInstance(document, new FileOutputStream("type3.pdf"));
        * document.open();
        * Type3Font t3 = new Type3Font(writer, false);
        * PdfContentByte g = t3.defineGlyph('a', 1000, 0, 0, 750, 750);
        * g.rectangle(0, 0, 750, 750);
        * g.fill();
        * g = t3.defineGlyph('b', 1000, 0, 0, 750, 750);
        * g.moveTo(0, 0);
        * g.lineTo(375, 750);
        * g.lineTo(750, 0);
        * g.fill();
        * Font f = new Font(t3, 12);
        * document.add(new Paragraph("ababab", f));
        * document.close();
        * </pre>
        * @param writer the writer
        * @param colorized if <CODE>true</CODE> the font may specify color, if <CODE>false</CODE> no color commands are allowed
        * and only images as masks can be used
        */    
        public Type3Font(PdfWriter writer, bool colorized) {
            this.writer = writer;
            this.colorized = colorized;
            fontType = FONT_TYPE_T3;
            usedSlot = new bool[256];
        }
        
        /**
        * Defines a glyph. If the character was already defined it will return the same content
        * @param c the character to match this glyph.
        * @param wx the advance this character will have
        * @param llx the X lower left corner of the glyph bounding box. If the <CODE>colorize</CODE> option is
        * <CODE>true</CODE> the value is ignored
        * @param lly the Y lower left corner of the glyph bounding box. If the <CODE>colorize</CODE> option is
        * <CODE>true</CODE> the value is ignored
        * @param urx the X upper right corner of the glyph bounding box. If the <CODE>colorize</CODE> option is
        * <CODE>true</CODE> the value is ignored
        * @param ury the Y upper right corner of the glyph bounding box. If the <CODE>colorize</CODE> option is
        * <CODE>true</CODE> the value is ignored
        * @return a content where the glyph can be defined
        */    
        public PdfContentByte DefineGlyph(char c, float wx, float llx, float lly, float urx, float ury) {
            if (c == 0 || c > 255)
                throw new ArgumentException("The char " + (int)c + " doesn't belong in this Type3 font");
            usedSlot[c] = true;
            Type3Glyph glyph = (Type3Glyph)char2glyph[c];
            if (glyph != null)
                return glyph;
            widths3[c] = (int)wx;
            if (!colorized) {
                if (float.IsNaN(this.llx)) {
                    this.llx = llx;
                    this.lly = lly;
                    this.urx = urx;
                    this.ury = ury;
                }
                else {
                    this.llx = Math.Min(this.llx, llx);
                    this.lly = Math.Min(this.lly, lly);
                    this.urx = Math.Max(this.urx, urx);
                    this.ury = Math.Max(this.ury, ury);
                }
            }
            glyph = new Type3Glyph(writer, pageResources, wx, llx, lly, urx, ury, colorized);
            char2glyph[c] = glyph;
            return glyph;
        }
        
        public override String[][] FamilyFontName {
            get {
                return FullFontName;
            }
        }
        
        public override float GetFontDescriptor(int key, float fontSize) {
            return 0;
        }
        
        public override String[][] FullFontName {
            get {
                return new string[][]{new string[]{"", "", "", ""}};
            }
        }
        
        public override string[][] AllNameEntries {
            get {
                return new string[][]{new string[]{"4", "", "", "", ""}};
            }
        }
        
        public override int GetKerning(int char1, int char2) {
            return 0;
        }
        
        public override string PostscriptFontName {
            get {
                return "";
            }
            set {
            }
        }
        
        protected override int[] GetRawCharBBox(int c, String name) {
            return null;
        }
        
        internal override int GetRawWidth(int c, String name) {
            return 0;
        }
        
        public override bool HasKernPairs() {
            return false;
        }
        
        public override bool SetKerning(int char1, int char2, int kern) {
            return false;
        }
                
        internal override void WriteFont(PdfWriter writer, PdfIndirectReference piRef, Object[] oParams) {
            if (this.writer != writer)
                throw new ArgumentException("Type3 font used with the wrong PdfWriter");
            // Get first & lastchar ...
            int firstChar = 0;
            while( firstChar < usedSlot.Length && !usedSlot[firstChar] ) firstChar++;
            
            if ( firstChar == usedSlot.Length ) {
                throw new DocumentException( "No glyphs defined for Type3 font" );
            }
            int lastChar = usedSlot.Length - 1;
            while( lastChar >= firstChar && !usedSlot[lastChar] ) lastChar--;
            
            int[] widths = new int[lastChar - firstChar + 1];
            int[] invOrd = new int[lastChar - firstChar + 1];
            
            int invOrdIndx = 0, w = 0;
            for( int u = firstChar; u<=lastChar; u++, w++ ) {
                if ( usedSlot[u] ) {
                    invOrd[invOrdIndx++] = u;
                    widths[w] = widths3[u];
                }
            }
            PdfArray diffs = new PdfArray();
            PdfDictionary charprocs = new PdfDictionary();
            int last = -1;
            for (int k = 0; k < invOrdIndx; ++k) {
                int c = invOrd[k];
                if (c > last) {
                    last = c;
                    diffs.Add(new PdfNumber(last));
                }
                ++last;
                int c2 = invOrd[k];
                String s = GlyphList.UnicodeToName(c2);
                if (s == null)
                    s = "a" + c2;
                PdfName n = new PdfName(s);
                diffs.Add(n);
                Type3Glyph glyph = (Type3Glyph)char2glyph[(char)c2];
                PdfStream stream = new PdfStream(glyph.ToPdf(null));
                stream.FlateCompress();
                PdfIndirectReference refp = writer.AddToBody(stream).IndirectReference;
                charprocs.Put(n, refp);
            }
            PdfDictionary font = new PdfDictionary(PdfName.FONT);
            font.Put(PdfName.SUBTYPE, PdfName.TYPE3);
            if (colorized)
                font.Put(PdfName.FONTBBOX, new PdfRectangle(0, 0, 0, 0));
            else
                font.Put(PdfName.FONTBBOX, new PdfRectangle(llx, lly, urx, ury));
            font.Put(PdfName.FONTMATRIX, new PdfArray(new float[]{0.001f, 0, 0, 0.001f, 0, 0}));
            font.Put(PdfName.CHARPROCS, writer.AddToBody(charprocs).IndirectReference);
            PdfDictionary encoding = new PdfDictionary();
            encoding.Put(PdfName.DIFFERENCES, diffs);
            font.Put(PdfName.ENCODING, writer.AddToBody(encoding).IndirectReference);
            font.Put(PdfName.FIRSTCHAR, new PdfNumber(firstChar));
            font.Put(PdfName.LASTCHAR, new PdfNumber(lastChar));
            font.Put(PdfName.WIDTHS, writer.AddToBody(new PdfArray(widths)).IndirectReference);
            if (pageResources.HasResources())
                font.Put(PdfName.RESOURCES, writer.AddToBody(pageResources.Resources).IndirectReference);
            writer.AddToBody(font, piRef);
        }
        
        internal override byte[] ConvertToBytes(String text) {
            char[] cc = text.ToCharArray();
            byte[] b = new byte[cc.Length];
            int p = 0;
            for (int k = 0; k < cc.Length; ++k) {
                char c = cc[k];
                if (CharExists(c))
                    b[p++] = (byte)c;
            }
            if (b.Length == p)
                return b;
            byte[] b2 = new byte[p];
            Array.Copy(b, 0, b2, 0, p);
            return b2;
        }
        
        internal override byte[] ConvertToBytes(int char1) {
            if (CharExists(char1))
                return new byte[]{(byte)char1};
            else return new byte[0];
        }
        
        public override int GetWidth(int char1) {
            if (!widths3.ContainsKey(char1))
                throw new ArgumentException("The char " + (int)char1 + " is not defined in a Type3 font");
            return widths3[char1];
        }
        
        public override int GetWidth(String text) {
            char[] c = text.ToCharArray();
            int total = 0;
            for (int k = 0; k < c.Length; ++k)
                total += GetWidth(c[k]);
            return total;
        }
        
        public override int[] GetCharBBox(int c) {
            return null;
        }
        
        public override bool CharExists(int c) {
            if ( c > 0 && c < 256 ) {
                return usedSlot[c];
            } else {
                return false;
            }
        }
        
        public override bool SetCharAdvance(int c, int advance) {
            return false;
        }
        
    }
}