using System;
using System.IO;
using System.Text;
using System.Collections;

/*
 * $Id: TrueTypeFontUnicode.cs,v 1.12 2008/05/13 11:25:23 psoares33 Exp $
 * 
 *
 * Copyright 2001, 2002 Paulo Soares
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

    /** Represents a True Type font with Unicode encoding. All the character
     * in the font can be used directly by using the encoding Identity-H or
     * Identity-V. This is the only way to represent some character sets such
     * as Thai.
     * @author  Paulo Soares (psoares@consiste.pt)
     */
    internal class TrueTypeFontUnicode : TrueTypeFont, IComparer {
    
        /** <CODE>true</CODE> if the encoding is vertical.
         */    
        bool vertical = false;

        /** Creates a new TrueType font addressed by Unicode characters. The font
         * will always be embedded.
         * @param ttFile the location of the font on file. The file must end in '.ttf'.
         * The modifiers after the name are ignored.
         * @param enc the encoding to be applied to this font
         * @param emb true if the font is to be embedded in the PDF
         * @param ttfAfm the font as a <CODE>byte</CODE> array
         * @throws DocumentException the font is invalid
         * @throws IOException the font file could not be read
         */
        internal TrueTypeFontUnicode(string ttFile, string enc, bool emb, byte[] ttfAfm) {
            string nameBase = GetBaseName(ttFile);
            string ttcName = GetTTCName(nameBase);
            if (nameBase.Length < ttFile.Length) {
                style = ttFile.Substring(nameBase.Length);
            }
            encoding = enc;
            embedded = emb;
            fileName = ttcName;
            ttcIndex = "";
            if (ttcName.Length < nameBase.Length)
                ttcIndex = nameBase.Substring(ttcName.Length + 1);
            FontType = FONT_TYPE_TTUNI;
            if ((fileName.ToLower(System.Globalization.CultureInfo.InvariantCulture).EndsWith(".ttf") || fileName.ToLower(System.Globalization.CultureInfo.InvariantCulture).EndsWith(".otf") || fileName.ToLower(System.Globalization.CultureInfo.InvariantCulture).EndsWith(".ttc")) && ((enc.Equals(IDENTITY_H) || enc.Equals(IDENTITY_V)) && emb)) {
                Process(ttfAfm);
                if (os_2.fsType == 2)
                    throw new DocumentException(fileName + style + " cannot be embedded due to licensing restrictions.");
                // Sivan
                if ((cmap31 == null && !fontSpecific) || (cmap10 == null && fontSpecific))
                    directTextToByte=true;
                    //throw new DocumentException(fileName + " " + style + " does not contain an usable cmap.");
                if (fontSpecific) {
                    fontSpecific = false;
                    String tempEncoding = encoding;
                    encoding = "";
                    CreateEncoding();
                    encoding = tempEncoding;
                    fontSpecific = true;
                }
            }
            else
                throw new DocumentException(fileName + " " + style + " is not a TTF font file.");
            vertical = enc.EndsWith("V");
        }
    
        /**
        * Gets the width of a <CODE>char</CODE> in normalized 1000 units.
        * @param char1 the unicode <CODE>char</CODE> to get the width of
        * @return the width in normalized 1000 units
        */
        public override int GetWidth(int char1) {
            if (vertical)
                return 1000;
            if (fontSpecific) {
                if ((char1 & 0xff00) == 0 || (char1 & 0xff00) == 0xf000)
                    return GetRawWidth(char1 & 0xff, null);
                else
                    return 0;
            }
            else {
                return GetRawWidth(char1, encoding);
            }
        }
        
        /**
         * Gets the width of a <CODE>string</CODE> in normalized 1000 units.
         * @param text the <CODE>string</CODE> to get the witdth of
         * @return the width in normalized 1000 units
         */
        public override int GetWidth(string text) {
            if (vertical)
                return text.Length * 1000;
            int total = 0;
            if (fontSpecific) {
                char[] cc = text.ToCharArray();
                int len = cc.Length;
                for (int k = 0; k < len; ++k) {
                    char c = cc[k];
                    if ((c & 0xff00) == 0 || (c & 0xff00) == 0xf000)
                        total += GetRawWidth(c & 0xff, null);
                }
            }
            else {
                int len = text.Length;
                for (int k = 0; k < len; ++k) {
                    if (Utilities.IsSurrogatePair(text, k)) {
                        total += GetRawWidth(Utilities.ConvertToUtf32(text, k), encoding);
                        ++k;
                    }
                    else
                        total += GetRawWidth(text[k], encoding);
                }
            }
            return total;
        }

        /** Creates a ToUnicode CMap to allow copy and paste from Acrobat.
         * @param metrics metrics[0] contains the glyph index and metrics[2]
         * contains the Unicode code
         * @throws DocumentException on error
         * @return the stream representing this CMap or <CODE>null</CODE>
         */    
        private PdfStream GetToUnicode(Object[] metrics) {
            if (metrics.Length == 0)
                return null;
            StringBuilder buf = new StringBuilder(
                "/CIDInit /ProcSet findresource begin\n" +
                "12 dict begin\n" +
                "begincmap\n" +
                "/CIDSystemInfo\n" +
                "<< /Registry (TTX+0)\n" +
                "/Ordering (T42UV)\n" +
                "/Supplement 0\n" +
                ">> def\n" +
                "/CMapName /TTX+0 def\n" +
                "/CMapType 2 def\n" +
                "1 begincodespacerange\n" +
                "<0000><FFFF>\n" +
                "endcodespacerange\n");
            int size = 0;
            for (int k = 0; k < metrics.Length; ++k) {
                if (size == 0) {
                    if (k != 0) {
                        buf.Append("endbfrange\n");
                    }
                    size = Math.Min(100, metrics.Length - k);
                    buf.Append(size).Append(" beginbfrange\n");
                }
                --size;
                int[] metric = (int[])metrics[k];
                string fromTo = ToHex(metric[0]);
                buf.Append(fromTo).Append(fromTo).Append(ToHex(metric[2])).Append('\n');
            }
            buf.Append(
                "endbfrange\n" +
                "endcmap\n" +
                "CMapName currentdict /CMap defineresource pop\n" +
                "end end\n");
            string s = buf.ToString();
            PdfStream stream = new PdfStream(PdfEncodings.ConvertToBytes(s, null));
            stream.FlateCompress();
            return stream;
        }
    
        /** Gets an hex string in the format "&lt;HHHH&gt;".
         * @param n the number
         * @return the hex string
         */    
        internal static string ToHex(int n) {
            if (n < 0x10000)
                return "<" + System.Convert.ToString(n, 16).PadLeft(4, '0') + ">";
            n -= 0x10000;
            int high = (n / 0x400) + 0xd800;
            int low = (n % 0x400) + 0xdc00;
            return "[<" + System.Convert.ToString(high, 16).PadLeft(4, '0') + System.Convert.ToString(low, 16).PadLeft(4, '0') + ">]";
        }
    
        /** Generates the CIDFontTyte2 dictionary.
         * @param fontDescriptor the indirect reference to the font descriptor
         * @param subsetPrefix the subset prefix
         * @param metrics the horizontal width metrics
         * @return a stream
         */    
        private PdfDictionary GetCIDFontType2(PdfIndirectReference fontDescriptor, string subsetPrefix, Object[] metrics) {
            PdfDictionary dic = new PdfDictionary(PdfName.FONT);
            // sivan; cff
            if (cff) {
                dic.Put(PdfName.SUBTYPE, PdfName.CIDFONTTYPE0);
                dic.Put(PdfName.BASEFONT, new PdfName(subsetPrefix + fontName+"-"+encoding));
            }
            else {
                dic.Put(PdfName.SUBTYPE, PdfName.CIDFONTTYPE2);
                dic.Put(PdfName.BASEFONT, new PdfName(subsetPrefix + fontName));
            }
            dic.Put(PdfName.FONTDESCRIPTOR, fontDescriptor);
            if (!cff)
                dic.Put(PdfName.CIDTOGIDMAP,PdfName.IDENTITY);
            PdfDictionary cdic = new PdfDictionary();
            cdic.Put(PdfName.REGISTRY, new PdfString("Adobe"));
            cdic.Put(PdfName.ORDERING, new PdfString("Identity"));
            cdic.Put(PdfName.SUPPLEMENT, new PdfNumber(0));
            dic.Put(PdfName.CIDSYSTEMINFO, cdic);
            if (!vertical) {
                dic.Put(PdfName.DW, new PdfNumber(1000));
                StringBuilder buf = new StringBuilder("[");
                int lastNumber = -10;
                bool firstTime = true;
                for (int k = 0; k < metrics.Length; ++k) {
                    int[] metric = (int[])metrics[k];
                    if (metric[1] == 1000)
                        continue;
                    int m = metric[0];
                    if (m == lastNumber + 1) {
                        buf.Append(' ').Append(metric[1]);
                    }
                    else {
                        if (!firstTime) {
                            buf.Append(']');
                        }
                        firstTime = false;
                        buf.Append(m).Append('[').Append(metric[1]);
                    }
                    lastNumber = m;
                }
                if (buf.Length > 1) {
                    buf.Append("]]");
                    dic.Put(PdfName.W, new PdfLiteral(buf.ToString()));
                }
            }
            return dic;
        }
    
        /** Generates the font dictionary.
         * @param descendant the descendant dictionary
         * @param subsetPrefix the subset prefix
         * @param toUnicode the ToUnicode stream
         * @return the stream
         */    
        private PdfDictionary GetFontBaseType(PdfIndirectReference descendant, string subsetPrefix, PdfIndirectReference toUnicode) {
            PdfDictionary dic = new PdfDictionary(PdfName.FONT);

            dic.Put(PdfName.SUBTYPE, PdfName.TYPE0);
            // The PDF Reference manual advises to add -encoding to CID font names
            if (cff)
                dic.Put(PdfName.BASEFONT, new PdfName(subsetPrefix + fontName+"-"+encoding));
            else
                dic.Put(PdfName.BASEFONT, new PdfName(subsetPrefix + fontName));
            dic.Put(PdfName.ENCODING, new PdfName(encoding));
            dic.Put(PdfName.DESCENDANTFONTS, new PdfArray(descendant));
            if (toUnicode != null)
                dic.Put(PdfName.TOUNICODE, toUnicode);  
            return dic;
        }

        /** The method used to sort the metrics array.
         * @param o1 the first element
         * @param o2 the second element
         * @return the comparisation
         */    
        public int Compare(Object o1, Object o2) {
            int m1 = ((int[])o1)[0];
            int m2 = ((int[])o2)[0];
            if (m1 < m2)
                return -1;
            if (m1 == m2)
                return 0;
            return 1;
        }

        private static readonly byte[] rotbits = {(byte)0x80,(byte)0x40,(byte)0x20,(byte)0x10,(byte)0x08,(byte)0x04,(byte)0x02,(byte)0x01};

        /** Outputs to the writer the font dictionaries and streams.
         * @param writer the writer for this document
         * @param ref the font indirect reference
         * @param parms several parameters that depend on the font type
         * @throws IOException on error
         * @throws DocumentException error in generating the object
         */
        internal override void WriteFont(PdfWriter writer, PdfIndirectReference piref, Object[] parms) {
            Hashtable longTag = (Hashtable)parms[0];
            AddRangeUni(longTag, true, subset);
            ArrayList tmp = new ArrayList();
            foreach (object o in longTag.Values) {
                tmp.Add(o);
            }
            Object[] metrics = tmp.ToArray();
            Array.Sort(metrics, this);
            PdfIndirectReference ind_font = null;
            PdfObject pobj = null;
            PdfIndirectObject obj = null;
            PdfIndirectReference cidset = null;
            if (writer.PDFXConformance == PdfWriter.PDFA1A || writer.PDFXConformance == PdfWriter.PDFA1B) {
                PdfStream stream;
                if (metrics.Length == 0) {
                    stream = new PdfStream(new byte[]{(byte)0x80});
                }
                else {
                    int top = ((int[])metrics[metrics.Length - 1])[0];
                    byte[] bt = new byte[top / 8 + 1];
                    for (int k = 0; k < metrics.Length; ++k) {
                        int v = ((int[])metrics[k])[0];
                        bt[v / 8] |= rotbits[v % 8];
                    }
                    stream = new PdfStream(bt);
                    stream.FlateCompress();
                }
                cidset = writer.AddToBody(stream).IndirectReference;
            }
            // sivan: cff
            if (cff) {
                RandomAccessFileOrArray rf2 = new RandomAccessFileOrArray(rf);
                byte[] b = new byte[cffLength];
                try {
                    rf2.ReOpen();
                    rf2.Seek(cffOffset);
                    rf2.ReadFully(b);
                } finally {
                    try {
                        rf2.Close();
                    } catch  {
                        // empty on purpose
                    }
                }
                if (subset || subsetRanges != null) {
                    CFFFontSubset cffs = new CFFFontSubset(new RandomAccessFileOrArray(b),longTag);
                    b = cffs.Process( (cffs.GetNames())[0] );
                }
                
                pobj = new StreamFont(b, "CIDFontType0C");
                obj = writer.AddToBody(pobj);
                ind_font = obj.IndirectReference;
            } else {
                byte[] b;
                if (subset || directoryOffset != 0) {
                    TrueTypeFontSubSet sb = new TrueTypeFontSubSet(fileName, new RandomAccessFileOrArray(rf), longTag, directoryOffset, false, false);
                    b = sb.Process();
                }
                else {
                    b = GetFullFont();
                }
                int[] lengths = new int[]{b.Length};
                pobj = new StreamFont(b, lengths);
                obj = writer.AddToBody(pobj);
                ind_font = obj.IndirectReference;
            }
            String subsetPrefix = "";
            if (subset)
                subsetPrefix = CreateSubsetPrefix();
            PdfDictionary dic = GetFontDescriptor(ind_font, subsetPrefix, cidset);
            obj = writer.AddToBody(dic);
            ind_font = obj.IndirectReference;

            pobj = GetCIDFontType2(ind_font, subsetPrefix, metrics);
            obj = writer.AddToBody(pobj);
            ind_font = obj.IndirectReference;

            pobj = GetToUnicode(metrics);
            PdfIndirectReference toUnicodeRef = null;
            if (pobj != null) {
                obj = writer.AddToBody(pobj);
                toUnicodeRef = obj.IndirectReference;
            }

            pobj = GetFontBaseType(ind_font, subsetPrefix, toUnicodeRef);
            writer.AddToBody(pobj, piref);
        }

        /** A forbidden operation. Will throw a null pointer exception.
         * @param text the text
         * @return always <CODE>null</CODE>
         */    
        internal override byte[] ConvertToBytes(string text) {
            return null;
        }

        internal override byte[] ConvertToBytes(int char1) {
            return null;
        }

        /** Gets the glyph index and metrics for a character.
         * @param c the character
         * @return an <CODE>int</CODE> array with {glyph index, width}
         */    
        public override int[] GetMetricsTT(int c) {
            if (cmapExt != null)
                return (int[])cmapExt[c];
            Hashtable map = null;
            if (fontSpecific)
                map = cmap10;
            else
                map = cmap31;
            if (map == null)
                return null;
            if (fontSpecific) {
                if ((c & 0xffffff00) == 0 || (c & 0xffffff00) == 0xf000)
                    return (int[])map[c & 0xff];
                else
                    return null;
            }
            else
                return (int[])map[c];
        }

        /**
        * Checks if a character exists in this font.
        * @param c the character to check
        * @return <CODE>true</CODE> if the character has a glyph,
        * <CODE>false</CODE> otherwise
        */
        public override bool CharExists(int c) {
            return GetMetricsTT(c) != null;
        }
        
        /**
        * Sets the character advance.
        * @param c the character
        * @param advance the character advance normalized to 1000 units
        * @return <CODE>true</CODE> if the advance was set,
        * <CODE>false</CODE> otherwise
        */
        public override bool SetCharAdvance(int c, int advance) {
            int[] m = GetMetricsTT(c);
            if (m == null)
                return false;
            m[1] = advance;
            return true;
        }
        
        public override int[] GetCharBBox(int c) {
            if (bboxes == null)
                return null;
            int[] m = GetMetricsTT(c);
            if (m == null)
                return null;
            return bboxes[m[0]];
        }
    }
}