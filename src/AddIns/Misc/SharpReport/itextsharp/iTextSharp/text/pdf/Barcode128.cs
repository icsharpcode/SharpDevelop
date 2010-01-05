using System;
using System.Text;
using iTextSharp.text;
/*
 * $Id: Barcode128.cs,v 1.6 2007/10/24 16:31:54 psoares33 Exp $
 *
 * Copyright 2002-2006 by Paulo Soares.
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
    /** Implements the code 128 and UCC/EAN-128. Other symbologies are allowed in raw mode.<p>
     * The code types allowed are:<br>
     * <ul>
     * <li><b>CODE128</b> - plain barcode 128.
     * <li><b>CODE128_UCC</b> - support for UCC/EAN-128 with a full list of AI.
     * <li><b>CODE128_RAW</b> - raw mode. The code attribute has the actual codes from 0
     *     to 105 followed by '&#92;uffff' and the human readable text.
     * </ul>
     * The default parameters are:
     * <pre>
     * x = 0.8f;
     * font = BaseFont.CreateFont("Helvetica", "winansi", false);
     * size = 8;
     * baseline = size;
     * barHeight = size * 3;
     * textint= Element.ALIGN_CENTER;
     * codeType = CODE128;
     * </pre>
     *
     * @author Paulo Soares (psoares@consiste.pt)
     */
    public class Barcode128 : Barcode {

        /** The bars to generate the code.
        */    
        private static readonly byte[][] BARS = 
        {
            new byte[] {2, 1, 2, 2, 2, 2},
            new byte[] {2, 2, 2, 1, 2, 2},
            new byte[] {2, 2, 2, 2, 2, 1},
            new byte[] {1, 2, 1, 2, 2, 3},
            new byte[] {1, 2, 1, 3, 2, 2},
            new byte[] {1, 3, 1, 2, 2, 2},
            new byte[] {1, 2, 2, 2, 1, 3},
            new byte[] {1, 2, 2, 3, 1, 2},
            new byte[] {1, 3, 2, 2, 1, 2},
            new byte[] {2, 2, 1, 2, 1, 3},
            new byte[] {2, 2, 1, 3, 1, 2},
            new byte[] {2, 3, 1, 2, 1, 2},
            new byte[] {1, 1, 2, 2, 3, 2},
            new byte[] {1, 2, 2, 1, 3, 2},
            new byte[] {1, 2, 2, 2, 3, 1},
            new byte[] {1, 1, 3, 2, 2, 2},
            new byte[] {1, 2, 3, 1, 2, 2},
            new byte[] {1, 2, 3, 2, 2, 1},
            new byte[] {2, 2, 3, 2, 1, 1},
            new byte[] {2, 2, 1, 1, 3, 2},
            new byte[] {2, 2, 1, 2, 3, 1},
            new byte[] {2, 1, 3, 2, 1, 2},
            new byte[] {2, 2, 3, 1, 1, 2},
            new byte[] {3, 1, 2, 1, 3, 1},
            new byte[] {3, 1, 1, 2, 2, 2},
            new byte[] {3, 2, 1, 1, 2, 2},
            new byte[] {3, 2, 1, 2, 2, 1},
            new byte[] {3, 1, 2, 2, 1, 2},
            new byte[] {3, 2, 2, 1, 1, 2},
            new byte[] {3, 2, 2, 2, 1, 1},
            new byte[] {2, 1, 2, 1, 2, 3},
            new byte[] {2, 1, 2, 3, 2, 1},
            new byte[] {2, 3, 2, 1, 2, 1},
            new byte[] {1, 1, 1, 3, 2, 3},
            new byte[] {1, 3, 1, 1, 2, 3},
            new byte[] {1, 3, 1, 3, 2, 1},
            new byte[] {1, 1, 2, 3, 1, 3},
            new byte[] {1, 3, 2, 1, 1, 3},
            new byte[] {1, 3, 2, 3, 1, 1},
            new byte[] {2, 1, 1, 3, 1, 3},
            new byte[] {2, 3, 1, 1, 1, 3},
            new byte[] {2, 3, 1, 3, 1, 1},
            new byte[] {1, 1, 2, 1, 3, 3},
            new byte[] {1, 1, 2, 3, 3, 1},
            new byte[] {1, 3, 2, 1, 3, 1},
            new byte[] {1, 1, 3, 1, 2, 3},
            new byte[] {1, 1, 3, 3, 2, 1},
            new byte[] {1, 3, 3, 1, 2, 1},
            new byte[] {3, 1, 3, 1, 2, 1},
            new byte[] {2, 1, 1, 3, 3, 1},
            new byte[] {2, 3, 1, 1, 3, 1},
            new byte[] {2, 1, 3, 1, 1, 3},
            new byte[] {2, 1, 3, 3, 1, 1},
            new byte[] {2, 1, 3, 1, 3, 1},
            new byte[] {3, 1, 1, 1, 2, 3},
            new byte[] {3, 1, 1, 3, 2, 1},
            new byte[] {3, 3, 1, 1, 2, 1},
            new byte[] {3, 1, 2, 1, 1, 3},
            new byte[] {3, 1, 2, 3, 1, 1},
            new byte[] {3, 3, 2, 1, 1, 1},
            new byte[] {3, 1, 4, 1, 1, 1},
            new byte[] {2, 2, 1, 4, 1, 1},
            new byte[] {4, 3, 1, 1, 1, 1},
            new byte[] {1, 1, 1, 2, 2, 4},
            new byte[] {1, 1, 1, 4, 2, 2},
            new byte[] {1, 2, 1, 1, 2, 4},
            new byte[] {1, 2, 1, 4, 2, 1},
            new byte[] {1, 4, 1, 1, 2, 2},
            new byte[] {1, 4, 1, 2, 2, 1},
            new byte[] {1, 1, 2, 2, 1, 4},
            new byte[] {1, 1, 2, 4, 1, 2},
            new byte[] {1, 2, 2, 1, 1, 4},
            new byte[] {1, 2, 2, 4, 1, 1},
            new byte[] {1, 4, 2, 1, 1, 2},
            new byte[] {1, 4, 2, 2, 1, 1},
            new byte[] {2, 4, 1, 2, 1, 1},
            new byte[] {2, 2, 1, 1, 1, 4},
            new byte[] {4, 1, 3, 1, 1, 1},
            new byte[] {2, 4, 1, 1, 1, 2},
            new byte[] {1, 3, 4, 1, 1, 1},
            new byte[] {1, 1, 1, 2, 4, 2},
            new byte[] {1, 2, 1, 1, 4, 2},
            new byte[] {1, 2, 1, 2, 4, 1},
            new byte[] {1, 1, 4, 2, 1, 2},
            new byte[] {1, 2, 4, 1, 1, 2},
            new byte[] {1, 2, 4, 2, 1, 1},
            new byte[] {4, 1, 1, 2, 1, 2},
            new byte[] {4, 2, 1, 1, 1, 2},
            new byte[] {4, 2, 1, 2, 1, 1},
            new byte[] {2, 1, 2, 1, 4, 1},
            new byte[] {2, 1, 4, 1, 2, 1},
            new byte[] {4, 1, 2, 1, 2, 1},
            new byte[] {1, 1, 1, 1, 4, 3},
            new byte[] {1, 1, 1, 3, 4, 1},
            new byte[] {1, 3, 1, 1, 4, 1},
            new byte[] {1, 1, 4, 1, 1, 3},
            new byte[] {1, 1, 4, 3, 1, 1},
            new byte[] {4, 1, 1, 1, 1, 3},
            new byte[] {4, 1, 1, 3, 1, 1},
            new byte[] {1, 1, 3, 1, 4, 1},
            new byte[] {1, 1, 4, 1, 3, 1},
            new byte[] {3, 1, 1, 1, 4, 1},
            new byte[] {4, 1, 1, 1, 3, 1},
            new byte[] {2, 1, 1, 4, 1, 2},
            new byte[] {2, 1, 1, 2, 1, 4},
            new byte[] {2, 1, 1, 2, 3, 2}
        };
        
        /** The stop bars.
        */    
        private static readonly byte[] BARS_STOP = {2, 3, 3, 1, 1, 1, 2};
        /** The charset code change.
        */
        public const char CODE_AB_TO_C = (char)99;
        /** The charset code change.
        */
        public const char CODE_AC_TO_B = (char)100;
        /** The charset code change.
        */
        public const char CODE_BC_TO_A = (char)101;
        /** The code for UCC/EAN-128.
        */
        public const char FNC1_INDEX = (char)102;
        /** The start code.
        */
        public const char START_A = (char)103;
        /** The start code.
        */
        public const char START_B = (char)104;
        /** The start code.
        */
        public const char START_C = (char)105;

        public const char FNC1 = '\u00ca';
        public const char DEL = '\u00c3';
        public const char FNC3 = '\u00c4';
        public const char FNC2 = '\u00c5';
        public const char SHIFT = '\u00c6';
        public const char CODE_C = '\u00c7';
        public const char CODE_A = '\u00c8';
        public const char FNC4 = '\u00c8';
        public const char STARTA = '\u00cb';
        public const char STARTB = '\u00cc';
        public const char STARTC = '\u00cd';
        
        private static IntHashtable ais = new IntHashtable();
        
        /** Creates new Barcode128 */
        public Barcode128() {
            x = 0.8f;
            font = BaseFont.CreateFont("Helvetica", "winansi", false);
            size = 8;
            baseline = size;
            barHeight = size * 3;
            textAlignment = Element.ALIGN_CENTER;
            codeType = CODE128;
        }

        /**
        * Removes the FNC1 codes in the text.
        * @param code the text to clean
        * @return the cleaned text
        */    
        public static String RemoveFNC1(String code) {
            int len = code.Length;
            StringBuilder buf = new StringBuilder(len);
            for (int k = 0; k < len; ++k) {
                char c = code[k];
                if (c >= 32 && c <= 126)
                    buf.Append(c);
            }
            return buf.ToString();
        }
        
        /**
        * Gets the human readable text of a sequence of AI.
        * @param code the text
        * @return the human readable text
        */    
        public static String GetHumanReadableUCCEAN(String code) {
            StringBuilder buf = new StringBuilder();
            String fnc1 = FNC1.ToString();
            try {
                while (true) {
                    if (code.StartsWith(fnc1)) {
                        code = code.Substring(1);
                        continue;
                    }
                    int n = 0;
                    int idlen = 0;
                    for (int k = 2; k < 5; ++k) {
                        if (code.Length < k)
                            break;
                        if ((n = ais[int.Parse(code.Substring(0, k))]) != 0) {
                            idlen = k;
                            break;
                        }
                    }
                    if (idlen == 0)
                        break;
                    buf.Append('(').Append(code.Substring(0, idlen)).Append(')');
                    code = code.Substring(idlen);
                    if (n > 0) {
                        n -= idlen;
                        if (code.Length <= n)
                            break;
                        buf.Append(RemoveFNC1(code.Substring(0, n)));
                        code = code.Substring(n);
                    }
                    else {
                        int idx = code.IndexOf(FNC1);
                        if (idx < 0)
                            break;
                        buf.Append(code.Substring(0,idx));
                        code = code.Substring(idx + 1);
                    }
                }
            }
            catch {
                //empty
            }
            buf.Append(RemoveFNC1(code));
            return buf.ToString();
        }
        
        /** Returns <CODE>true</CODE> if the next <CODE>numDigits</CODE>
        * starting from index <CODE>textIndex</CODE> are numeric skipping any FNC1.
        * @param text the text to check
        * @param textIndex where to check from
        * @param numDigits the number of digits to check
        * @return the check result
        */    
        internal static bool IsNextDigits(string text, int textIndex, int numDigits) {
            int len = text.Length;
            while (textIndex < len && numDigits > 0) {
                if (text[textIndex] == FNC1) {
                    ++textIndex;
                    continue;
                }
                int n = Math.Min(2, numDigits);
                if (textIndex + n > len)
                    return false;
                while (n-- > 0) {
                    char c = text[textIndex++];
                    if (c < '0' || c > '9')
                        return false;
                    --numDigits;
                }
            }
            return numDigits == 0;
        }
        
        /** Packs the digits for charset C also considering FNC1. It assumes that all the parameters
        * are valid.
        * @param text the text to pack
        * @param textIndex where to pack from
        * @param numDigits the number of digits to pack. It is always an even number
        * @return the packed digits, two digits per character
        */    
        internal static String GetPackedRawDigits(String text, int textIndex, int numDigits) {
            String outs = "";
            int start = textIndex;
            while (numDigits > 0) {
                if (text[textIndex] == FNC1) {
                    outs += FNC1_INDEX;
                    ++textIndex;
                    continue;
                }
                numDigits -= 2;
                int c1 = text[textIndex++] - '0';
                int c2 = text[textIndex++] - '0';
                outs += (char)(c1 * 10 + c2);
            }
            return (char)(textIndex - start) + outs;
        }
        
        /** Converts the human readable text to the characters needed to
        * create a barcode. Some optimization is done to get the shortest code.
        * @param text the text to convert
        * @param ucc <CODE>true</CODE> if it is an UCC/EAN-128. In this case
        * the character FNC1 is added
        * @return the code ready to be fed to GetBarsCode128Raw()
        */    
        public static string GetRawText(string text, bool ucc) {
            String outs = "";
            int tLen = text.Length;
            if (tLen == 0) {
                outs += START_B;
                if (ucc)
                    outs += FNC1_INDEX;
                return outs;
            }
            int c = 0;
            for (int k = 0; k < tLen; ++k) {
                c = text[k];
                if (c > 127 && c != FNC1)
                    throw new ArgumentException("There are illegal characters for barcode 128 in '" + text + "'.");
            }
            c = text[0];
            char currentCode = START_B;
            int index = 0;
            if (IsNextDigits(text, index, 2)) {
                currentCode = START_C;
                outs += currentCode;
                if (ucc)
                    outs += FNC1_INDEX;
                String out2 = GetPackedRawDigits(text, index, 2);
                index += (int)out2[0];
                outs += out2.Substring(1);
            }
            else if (c < ' ') {
                currentCode = START_A;
                outs += currentCode;
                if (ucc)
                    outs += FNC1_INDEX;
                outs += (char)(c + 64);
                ++index;
            }
            else {
                outs += currentCode;
                if (ucc)
                    outs += FNC1_INDEX;
                if (c == FNC1)
                    outs += FNC1_INDEX;
                else
                    outs += (char)(c - ' ');
                ++index;
            }
            while (index < tLen) {
                switch (currentCode) {
                    case START_A:
                        {
                            if (IsNextDigits(text, index, 4)) {
                                currentCode = START_C;
                                outs += CODE_AB_TO_C;
                                String out2 = GetPackedRawDigits(text, index, 4);
                                index += (int)out2[0];
                                outs += out2.Substring(1);
                            }
                            else {
                                c = text[index++];
                                if (c == FNC1)
                                    outs += FNC1_INDEX;
                                else if (c > '_') {
                                    currentCode = START_B;
                                    outs += CODE_AC_TO_B;
                                    outs += (char)(c - ' ');
                                }
                                else if (c < ' ')
                                    outs += (char)(c + 64);
                                else
                                    outs += (char)(c - ' ');
                            }
                        }
                        break;
                    case START_B:
                        {
                            if (IsNextDigits(text, index, 4)) {
                                currentCode = START_C;
                                outs += CODE_AB_TO_C;
                                String out2 = GetPackedRawDigits(text, index, 4);
                                index += (int)out2[0];
                                outs += out2.Substring(1);
                            }
                            else {
                                c = text[index++];
                                if (c == FNC1)
                                    outs += FNC1_INDEX;
                                else if (c < ' ') {
                                    currentCode = START_A;
                                    outs += CODE_BC_TO_A;
                                    outs += (char)(c + 64);
                                }
                                else {
                                    outs += (char)(c - ' ');
                                }
                            }
                        }
                        break;
                    case START_C:
                        {
                            if (IsNextDigits(text, index, 2)) {
                                String out2 = GetPackedRawDigits(text, index, 2);
                                index += (int)out2[0];
                                outs += out2.Substring(1);
                            }
                            else {
                                c = text[index++];
                                if (c == FNC1)
                                    outs += FNC1_INDEX;
                                else if (c < ' ') {
                                    currentCode = START_A;
                                    outs += CODE_BC_TO_A;
                                    outs += (char)(c + 64);
                                }
                                else {
                                    currentCode = START_B;
                                    outs += CODE_AC_TO_B;
                                    outs += (char)(c - ' ');
                                }
                            }
                        }
                        break;
                }
            }
            return outs;
        }
        
        /** Generates the bars. The input has the actual barcodes, not
        * the human readable text.
        * @param text the barcode
        * @return the bars
        */    
        public static byte[] GetBarsCode128Raw(string text) {
            int k;
            int idx = text.IndexOf('\uffff');
            if (idx >= 0)
                text = text.Substring(0, idx);
            int chk = text[0];
            for (k = 1; k < text.Length; ++k)
                chk += k * text[k];
            chk = chk % 103;
            text += (char)chk;
            byte[] bars = new byte[(text.Length + 1) * 6 + 7];
            for (k = 0; k < text.Length; ++k)
                Array.Copy(BARS[text[k]], 0, bars, k * 6, 6);
            Array.Copy(BARS_STOP, 0, bars, k * 6, 7);
            return bars;
        }
        
        /** Gets the maximum area that the barcode and the text, if
        * any, will occupy. The lower left corner is always (0, 0).
        * @return the size the barcode occupies.
        */
        public override Rectangle BarcodeSize {
            get {
                float fontX = 0;
                float fontY = 0;
                string fullCode;
                if (font != null) {
                    if (baseline > 0)
                        fontY = baseline - font.GetFontDescriptor(BaseFont.DESCENT, size);
                    else
                        fontY = -baseline + size;
                    if (codeType == CODE128_RAW) {
                        int idx = code.IndexOf('\uffff');
                        if (idx < 0)
                            fullCode = "";
                        else
                            fullCode = code.Substring(idx + 1);
                    }
                    else if (codeType == CODE128_UCC)
                        fullCode = GetHumanReadableUCCEAN(code);
                    else
                        fullCode = RemoveFNC1(code);
                    fontX = font.GetWidthPoint(altText != null ? altText : fullCode, size);
                }
                if (codeType == CODE128_RAW) {
                    int idx = code.IndexOf('\uffff');
                    if (idx >= 0)
                        fullCode = code.Substring(0, idx);
                    else
                        fullCode = code;
                }
                else {
                    fullCode = GetRawText(code, codeType == CODE128_UCC);
                }
                int len = fullCode.Length;
                float fullWidth = (len + 2) * 11 * x + 2 * x;
                fullWidth = Math.Max(fullWidth, fontX);
                float fullHeight = barHeight + fontY;
                return new Rectangle(fullWidth, fullHeight);
            }
        }
        
        /** Places the barcode in a <CODE>PdfContentByte</CODE>. The
        * barcode is always placed at coodinates (0, 0). Use the
        * translation matrix to move it elsewhere.<p>
        * The bars and text are written in the following colors:<p>
        * <P><TABLE BORDER=1>
        * <TR>
        *   <TH><P><CODE>barColor</CODE></TH>
        *   <TH><P><CODE>textColor</CODE></TH>
        *   <TH><P>Result</TH>
        *   </TR>
        * <TR>
        *   <TD><P><CODE>null</CODE></TD>
        *   <TD><P><CODE>null</CODE></TD>
        *   <TD><P>bars and text painted with current fill color</TD>
        *   </TR>
        * <TR>
        *   <TD><P><CODE>barColor</CODE></TD>
        *   <TD><P><CODE>null</CODE></TD>
        *   <TD><P>bars and text painted with <CODE>barColor</CODE></TD>
        *   </TR>
        * <TR>
        *   <TD><P><CODE>null</CODE></TD>
        *   <TD><P><CODE>textColor</CODE></TD>
        *   <TD><P>bars painted with current color<br>text painted with <CODE>textColor</CODE></TD>
        *   </TR>
        * <TR>
        *   <TD><P><CODE>barColor</CODE></TD>
        *   <TD><P><CODE>textColor</CODE></TD>
        *   <TD><P>bars painted with <CODE>barColor</CODE><br>text painted with <CODE>textColor</CODE></TD>
        *   </TR>
        * </TABLE>
        * @param cb the <CODE>PdfContentByte</CODE> where the barcode will be placed
        * @param barColor the color of the bars. It can be <CODE>null</CODE>
        * @param textColor the color of the text. It can be <CODE>null</CODE>
        * @return the dimensions the barcode occupies
        */
        public override Rectangle PlaceBarcode(PdfContentByte cb, Color barColor, Color textColor) {
            string fullCode;
            if (codeType == CODE128_RAW) {
                int idx = code.IndexOf('\uffff');
                if (idx < 0)
                    fullCode = "";
                else
                    fullCode = code.Substring(idx + 1);
            }
            else if (codeType == CODE128_UCC)
                fullCode = GetHumanReadableUCCEAN(code);
            else
                fullCode = RemoveFNC1(code);
            float fontX = 0;
            if (font != null) {
                fontX = font.GetWidthPoint(fullCode = altText != null ? altText : fullCode, size);
            }
            string bCode;
            if (codeType == CODE128_RAW) {
                int idx = code.IndexOf('\uffff');
                if (idx >= 0)
                    bCode = code.Substring(0, idx);
                else
                    bCode = code;
            }
            else {
                bCode = GetRawText(code, codeType == CODE128_UCC);
            }
            int len = bCode.Length;
            float fullWidth = (len + 2) * 11 * x + 2 * x;
            float barStartX = 0;
            float textStartX = 0;
            switch (textAlignment) {
                case Element.ALIGN_LEFT:
                    break;
                case Element.ALIGN_RIGHT:
                    if (fontX > fullWidth)
                        barStartX = fontX - fullWidth;
                    else
                        textStartX = fullWidth - fontX;
                    break;
                default:
                    if (fontX > fullWidth)
                        barStartX = (fontX - fullWidth) / 2;
                    else
                        textStartX = (fullWidth - fontX) / 2;
                    break;
            }
            float barStartY = 0;
            float textStartY = 0;
            if (font != null) {
                if (baseline <= 0)
                    textStartY = barHeight - baseline;
                else {
                    textStartY = -font.GetFontDescriptor(BaseFont.DESCENT, size);
                    barStartY = textStartY + baseline;
                }
            }
            byte[] bars = GetBarsCode128Raw(bCode);
            bool print = true;
            if (barColor != null)
                cb.SetColorFill(barColor);
            for (int k = 0; k < bars.Length; ++k) {
                float w = bars[k] * x;
                if (print)
                    cb.Rectangle(barStartX, barStartY, w - inkSpreading, barHeight);
                print = !print;
                barStartX += w;
            }
            cb.Fill();
            if (font != null) {
                if (textColor != null)
                    cb.SetColorFill(textColor);
                cb.BeginText();
                cb.SetFontAndSize(font, size);
                cb.SetTextMatrix(textStartX, textStartY);
                cb.ShowText(fullCode);
                cb.EndText();
            }
            return this.BarcodeSize;
        }    

        public override System.Drawing.Image CreateDrawingImage(System.Drawing.Color foreground, System.Drawing.Color background) {
            String bCode;
            if (codeType == CODE128_RAW) {
                int idx = code.IndexOf('\uffff');
                if (idx >= 0)
                    bCode = code.Substring(0, idx);
                else
                    bCode = code;
            }
            else {
                bCode = GetRawText(code, codeType == CODE128_UCC);
            }
            int len = bCode.Length;
            int fullWidth = (len + 2) * 11 + 2;
            byte[] bars = GetBarsCode128Raw(bCode);
            int height = (int)barHeight;
            System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(fullWidth, height);
            for (int h = 0; h < height; ++h) {
                bool print = true;
                int ptr = 0;
                for (int k = 0; k < bars.Length; ++k) {
                    int w = bars[k];
                    System.Drawing.Color c = background;
                    if (print)
                        c = foreground;
                    print = !print;
                    for (int j = 0; j < w; ++j)
                        bmp.SetPixel(ptr++, h, c);
                }
            }
            return bmp;
        }

        /**
        * Sets the code to generate. If it's an UCC code and starts with '(' it will
        * be split by the AI. This code in UCC mode is valid:
        * <p>
        * <code>(01)00000090311314(10)ABC123(15)060916</code>
        * @param code the code to generate
        */
        public override string Code {
            set {
                string code = value;
                if (CodeType == Barcode128.CODE128_UCC && code.StartsWith("(")) {
                    int idx = 0;
                    String ret = "";
                    while (idx >= 0) {
                        int end = code.IndexOf(')', idx);
                        if (end < 0)
                            throw new ArgumentException("Badly formed UCC string: " + code);
                        String sai = code.Substring(idx + 1, end - (idx + 1));
                        if (sai.Length < 2)
                            throw new ArgumentException("AI too short: (" + sai + ")");
                        int ai = int.Parse(sai);
                        int len = ais[ai];
                        if (len == 0)
                            throw new ArgumentException("AI not found: (" + sai + ")");
                        sai = ai.ToString();
                        if (sai.Length == 1)
                            sai = "0" + sai;
                        idx = code.IndexOf('(', end);
                        int next = (idx < 0 ? code.Length : idx);
                        ret += sai + code.Substring(end + 1, next - (end + 1));
                        if (len < 0) {
                            if (idx >= 0)
                                ret += FNC1;
                        }
                        else if (next - end - 1 + sai.Length != len)
                            throw new ArgumentException("Invalid AI length: (" + sai + ")");
                    }
                    base.Code = ret;
                }
                else
                    base.Code = code;
            }
        }
        
        static Barcode128 () {
            ais[0] = 20;
            ais[1] = 16;
            ais[2] = 16;
            ais[10] = -1;
            ais[11] = 9;
            ais[12] = 8;
            ais[13] = 8;
            ais[15] = 8;
            ais[17] = 8;
            ais[20] = 4;
            ais[21] = -1;
            ais[22] = -1;
            ais[23] = -1;
            ais[240] = -1;
            ais[241] = -1;
            ais[250] = -1;
            ais[251] = -1;
            ais[252] = -1;
            ais[30] = -1;
            for (int k = 3100; k < 3700; ++k)
                ais[k] = 10;
            ais[37] = -1;
            for (int k = 3900; k < 3940; ++k)
                ais[k] = -1;
            ais[400] = -1;
            ais[401] = -1;
            ais[402] = 20;
            ais[403] = -1;
            for (int k = 410; k < 416; ++k)
                ais[k] = 16;
            ais[420] = -1;
            ais[421] = -1;
            ais[422] = 6;
            ais[423] = -1;
            ais[424] = 6;
            ais[425] = 6;
            ais[426] = 6;
            ais[7001] = 17;
            ais[7002] = -1;
            for (int k = 7030; k < 7040; ++k)
                ais[k] = -1;
            ais[8001] = 18;
            ais[8002] = -1;
            ais[8003] = -1;
            ais[8004] = -1;
            ais[8005] = 10;
            ais[8006] = 22;
            ais[8007] = -1;
            ais[8008] = -1;
            ais[8018] = 22;
            ais[8020] = -1;
            ais[8100] = 10;
            ais[8101] = 14;
            ais[8102] = 6;
            for (int k = 90; k < 100; ++k)
                ais[k] = -1;
        }
    }
}
