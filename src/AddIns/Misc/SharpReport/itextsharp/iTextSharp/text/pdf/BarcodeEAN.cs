using System;
using iTextSharp.text;
using System.Collections;

/*
 * Copyright 2002 by Paulo Soares.
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
    /** Generates barcodes in several formats: EAN13, EAN8, UPCA, UPCE,
     * supplemental 2 and 5. The default parameters are:
     * <pre>
     *x = 0.8f;
     *font = BaseFont.CreateFont("Helvetica", "winansi", false);
     *size = 8;
     *baseline = size;
     *barHeight = size * 3;
     *guardBars = true;
     *codeType = EAN13;
     *code = "";
     * </pre>
     *
     * @author Paulo Soares (psoares@consiste.pt)
     */
    public class BarcodeEAN : Barcode {
        
        /** The bar positions that are guard bars.*/    
        private static readonly int[] GUARD_EMPTY = {};
        /** The bar positions that are guard bars.*/    
        private static readonly int[] GUARD_UPCA = {0, 2, 4, 6, 28, 30, 52, 54, 56, 58};
        /** The bar positions that are guard bars.*/    
        private static readonly int[] GUARD_EAN13 = {0, 2, 28, 30, 56, 58};
        /** The bar positions that are guard bars.*/    
        private static readonly int[] GUARD_EAN8 = {0, 2, 20, 22, 40, 42};
        /** The bar positions that are guard bars.*/    
        private static readonly int[] GUARD_UPCE = {0, 2, 28, 30, 32};
        /** The x coordinates to place the text.*/
        private static readonly float[] TEXTPOS_EAN13 = {6.5f, 13.5f, 20.5f, 27.5f, 34.5f, 41.5f, 53.5f, 60.5f, 67.5f, 74.5f, 81.5f, 88.5f};
        /** The x coordinates to place the text.*/
        private static readonly float[] TEXTPOS_EAN8 = {6.5f, 13.5f, 20.5f, 27.5f, 39.5f, 46.5f, 53.5f, 60.5f};
        /** The basic bar widths.*/
        private static readonly byte[][] BARS = 
        {
            new byte[] {3, 2, 1, 1}, // 0
            new byte[] {2, 2, 2, 1}, // 1
            new byte[] {2, 1, 2, 2}, // 2
            new byte[] {1, 4, 1, 1}, // 3
            new byte[] {1, 1, 3, 2}, // 4
            new byte[] {1, 2, 3, 1}, // 5
            new byte[] {1, 1, 1, 4}, // 6
            new byte[] {1, 3, 1, 2}, // 7
            new byte[] {1, 2, 1, 3}, // 8
            new byte[] {3, 1, 1, 2}  // 9
        };
        
        /** The total number of bars for EAN13.*/
        private const int TOTALBARS_EAN13 = 11 + 12 * 4;
        /** The total number of bars for EAN8.*/
        private const int TOTALBARS_EAN8 = 11 + 8 * 4;
        /** The total number of bars for UPCE.*/
        private const int TOTALBARS_UPCE = 9 + 6 * 4;
        /** The total number of bars for supplemental 2.*/
        private const int TOTALBARS_SUPP2 = 13;
        /** The total number of bars for supplemental 5.*/
        private const int TOTALBARS_SUPP5 = 31;
        /** Marker for odd parity.*/
        private const byte ODD = 0;
        /** Marker for even parity.*/
        private const byte EVEN = 1;
        
        /** Sequence of parities to be used with EAN13.*/
        private static readonly byte[][] PARITY13 =
        {
            new byte[] {ODD, ODD,  ODD,  ODD,  ODD,  ODD},  // 0
            new byte[] {ODD, ODD,  EVEN, ODD,  EVEN, EVEN}, // 1
            new byte[] {ODD, ODD,  EVEN, EVEN, ODD,  EVEN}, // 2
            new byte[] {ODD, ODD,  EVEN, EVEN, EVEN, ODD},  // 3
            new byte[] {ODD, EVEN, ODD,  ODD,  EVEN, EVEN}, // 4
            new byte[] {ODD, EVEN, EVEN, ODD,  ODD,  EVEN}, // 5
            new byte[] {ODD, EVEN, EVEN, EVEN, ODD,  ODD},  // 6
            new byte[] {ODD, EVEN, ODD,  EVEN, ODD,  EVEN}, // 7
            new byte[] {ODD, EVEN, ODD,  EVEN, EVEN, ODD},  // 8
            new byte[] {ODD, EVEN, EVEN, ODD,  EVEN, ODD}   // 9
        };
        
        /** Sequence of parities to be used with supplemental 2.*/
        private static readonly byte[][] PARITY2 =
        {
            new byte[] {ODD,  ODD},   // 0
            new byte[] {ODD,  EVEN},  // 1
            new byte[] {EVEN, ODD},   // 2
            new byte[] {EVEN, EVEN}   // 3
        };
        
        /** Sequence of parities to be used with supplemental 2.*/
        private static readonly byte[][] PARITY5 =
        {
            new byte[] {EVEN, EVEN, ODD,  ODD,  ODD},  // 0
            new byte[] {EVEN, ODD,  EVEN, ODD,  ODD},  // 1
            new byte[] {EVEN, ODD,  ODD,  EVEN, ODD},  // 2
            new byte[] {EVEN, ODD,  ODD,  ODD,  EVEN}, // 3
            new byte[] {ODD,  EVEN, EVEN, ODD,  ODD},  // 4
            new byte[] {ODD,  ODD,  EVEN, EVEN, ODD},  // 5
            new byte[] {ODD,  ODD,  ODD,  EVEN, EVEN}, // 6
            new byte[] {ODD,  EVEN, ODD,  EVEN, ODD},  // 7
            new byte[] {ODD,  EVEN, ODD,  ODD,  EVEN}, // 8
            new byte[] {ODD,  ODD,  EVEN, ODD,  EVEN}  // 9
        };
        
        /** Sequence of parities to be used with UPCE.*/
        private static readonly byte[][] PARITYE =
        {
            new byte[] {EVEN, EVEN, EVEN, ODD,  ODD,  ODD},  // 0
            new byte[] {EVEN, EVEN, ODD,  EVEN, ODD,  ODD},  // 1
            new byte[] {EVEN, EVEN, ODD,  ODD,  EVEN, ODD},  // 2
            new byte[] {EVEN, EVEN, ODD,  ODD,  ODD,  EVEN}, // 3
            new byte[] {EVEN, ODD,  EVEN, EVEN, ODD,  ODD},  // 4
            new byte[] {EVEN, ODD,  ODD,  EVEN, EVEN, ODD},  // 5
            new byte[] {EVEN, ODD,  ODD,  ODD,  EVEN, EVEN}, // 6
            new byte[] {EVEN, ODD,  EVEN, ODD,  EVEN, ODD},  // 7
            new byte[] {EVEN, ODD,  EVEN, ODD,  ODD,  EVEN}, // 8
            new byte[] {EVEN, ODD,  ODD,  EVEN, ODD,  EVEN}  // 9
        };
        
        /** Creates new BarcodeEAN */
        public BarcodeEAN() {
            x = 0.8f;
            font = BaseFont.CreateFont("Helvetica", "winansi", false);
            size = 8;
            baseline = size;
            barHeight = size * 3;
            guardBars = true;
            codeType = EAN13;
            code = "";
        }
        
        /** Calculates the EAN parity character.
        * @param code the code
        * @return the parity character
        */    
        public static int CalculateEANParity(string code) {
            int mul = 3;
            int total = 0;
            for (int k = code.Length - 1; k >= 0; --k) {
                int n = code[k] - '0';
                total += mul * n;
                mul ^= 2;
            }
            return (10 - (total % 10)) % 10;
        }
        
        /** Converts an UPCA code into an UPCE code. If the code can not
        * be converted a <CODE>null</CODE> is returned.
        * @param text the code to convert. It must have 12 numeric characters
        * @return the 8 converted digits or <CODE>null</CODE> if the
        * code could not be converted
        */    
        static public string ConvertUPCAtoUPCE(string text) {
            if (text.Length != 12 || !(text.StartsWith("0") || text.StartsWith("1")))
                return null;
            if (text.Substring(3, 3).Equals("000") || text.Substring(3, 3).Equals("100")
                || text.Substring(3, 3).Equals("200")) {
                    if (text.Substring(6, 2).Equals("00"))
                        return text.Substring(0, 1) + text.Substring(1, 2) + text.Substring(8, 3) + text.Substring(3, 1) + text.Substring(11);
            }
            else if (text.Substring(4, 2).Equals("00")) {
                if (text.Substring(6, 3).Equals("000"))
                    return text.Substring(0, 1) + text.Substring(1, 3) + text.Substring(9, 2) + "3" + text.Substring(11);
            }
            else if (text.Substring(5, 1).Equals("0")) {
                if (text.Substring(6, 4).Equals("0000"))
                    return text.Substring(0, 1) + text.Substring(1, 4) + text.Substring(10, 1) + "4" + text.Substring(11);
            }
            else if (text[10] >= '5') {
                if (text.Substring(6, 4).Equals("0000"))
                    return text.Substring(0, 1) + text.Substring(1, 5) + text.Substring(10, 1) + text.Substring(11);
            }
            return null;
        }
        
        /** Creates the bars for the barcode EAN13 and UPCA.
        * @param _code the text with 13 digits
        * @return the barcode
        */    
        public static byte[] GetBarsEAN13(string _code) {
            int[] code = new int[_code.Length];
            for (int k = 0; k < code.Length; ++k)
                code[k] = _code[k] - '0';
            byte[] bars = new byte[TOTALBARS_EAN13];
            int pb = 0;
            bars[pb++] = 1;
            bars[pb++] = 1;
            bars[pb++] = 1;
            byte[] sequence = PARITY13[code[0]];
            for (int k = 0; k < sequence.Length; ++k) {
                int c = code[k + 1];
                byte[] stripes = BARS[c];
                if (sequence[k] == ODD) {
                    bars[pb++] = stripes[0];
                    bars[pb++] = stripes[1];
                    bars[pb++] = stripes[2];
                    bars[pb++] = stripes[3];
                }
                else {
                    bars[pb++] = stripes[3];
                    bars[pb++] = stripes[2];
                    bars[pb++] = stripes[1];
                    bars[pb++] = stripes[0];
                }
            }
            bars[pb++] = 1;
            bars[pb++] = 1;
            bars[pb++] = 1;
            bars[pb++] = 1;
            bars[pb++] = 1;
            for (int k = 7; k < 13; ++k) {
                int c = code[k];
                byte[] stripes = BARS[c];
                bars[pb++] = stripes[0];
                bars[pb++] = stripes[1];
                bars[pb++] = stripes[2];
                bars[pb++] = stripes[3];
            }
            bars[pb++] = 1;
            bars[pb++] = 1;
            bars[pb++] = 1;
            return bars;
        }
        
        /** Creates the bars for the barcode EAN8.
        * @param _code the text with 8 digits
        * @return the barcode
        */    
        public static byte[] GetBarsEAN8(string _code) {
            int[] code = new int[_code.Length];
            for (int k = 0; k < code.Length; ++k)
                code[k] = _code[k] - '0';
            byte[] bars = new byte[TOTALBARS_EAN8];
            int pb = 0;
            bars[pb++] = 1;
            bars[pb++] = 1;
            bars[pb++] = 1;
            for (int k = 0; k < 4; ++k) {
                int c = code[k];
                byte[] stripes = BARS[c];
                bars[pb++] = stripes[0];
                bars[pb++] = stripes[1];
                bars[pb++] = stripes[2];
                bars[pb++] = stripes[3];
            }
            bars[pb++] = 1;
            bars[pb++] = 1;
            bars[pb++] = 1;
            bars[pb++] = 1;
            bars[pb++] = 1;
            for (int k = 4; k < 8; ++k) {
                int c = code[k];
                byte[] stripes = BARS[c];
                bars[pb++] = stripes[0];
                bars[pb++] = stripes[1];
                bars[pb++] = stripes[2];
                bars[pb++] = stripes[3];
            }
            bars[pb++] = 1;
            bars[pb++] = 1;
            bars[pb++] = 1;
            return bars;
        }
        
        /** Creates the bars for the barcode UPCE.
        * @param _code the text with 8 digits
        * @return the barcode
        */    
        public static byte[] GetBarsUPCE(string _code) {
            int[] code = new int[_code.Length];
            for (int k = 0; k < code.Length; ++k)
                code[k] = _code[k] - '0';
            byte[] bars = new byte[TOTALBARS_UPCE];
            bool flip = (code[0] != 0);
            int pb = 0;
            bars[pb++] = 1;
            bars[pb++] = 1;
            bars[pb++] = 1;
            byte[] sequence = PARITYE[code[code.Length - 1]];
            for (int k = 1; k < code.Length - 1; ++k) {
                int c = code[k];
                byte[] stripes = BARS[c];
                if (sequence[k - 1] == (flip ? EVEN : ODD)) {
                    bars[pb++] = stripes[0];
                    bars[pb++] = stripes[1];
                    bars[pb++] = stripes[2];
                    bars[pb++] = stripes[3];
                }
                else {
                    bars[pb++] = stripes[3];
                    bars[pb++] = stripes[2];
                    bars[pb++] = stripes[1];
                    bars[pb++] = stripes[0];
                }
            }
            bars[pb++] = 1;
            bars[pb++] = 1;
            bars[pb++] = 1;
            bars[pb++] = 1;
            bars[pb++] = 1;
            bars[pb++] = 1;
            return bars;
        }

        /** Creates the bars for the barcode supplemental 2.
        * @param _code the text with 2 digits
        * @return the barcode
        */    
        public static byte[] GetBarsSupplemental2(string _code) {
            int[] code = new int[2];
            for (int k = 0; k < code.Length; ++k)
                code[k] = _code[k] - '0';
            byte[] bars = new byte[TOTALBARS_SUPP2];
            int pb = 0;
            int parity = (code[0] * 10 + code[1]) % 4;
            bars[pb++] = 1;
            bars[pb++] = 1;
            bars[pb++] = 2;
            byte[] sequence = PARITY2[parity];
            for (int k = 0; k < sequence.Length; ++k) {
                if (k == 1) {
                    bars[pb++] = 1;
                    bars[pb++] = 1;
                }
                int c = code[k];
                byte[] stripes = BARS[c];
                if (sequence[k] == ODD) {
                    bars[pb++] = stripes[0];
                    bars[pb++] = stripes[1];
                    bars[pb++] = stripes[2];
                    bars[pb++] = stripes[3];
                }
                else {
                    bars[pb++] = stripes[3];
                    bars[pb++] = stripes[2];
                    bars[pb++] = stripes[1];
                    bars[pb++] = stripes[0];
                }
            }
            return bars;
        }   

        /** Creates the bars for the barcode supplemental 5.
        * @param _code the text with 5 digits
        * @return the barcode
        */    
        public static byte[] GetBarsSupplemental5(string _code) {
            int[] code = new int[5];
            for (int k = 0; k < code.Length; ++k)
                code[k] = _code[k] - '0';
            byte[] bars = new byte[TOTALBARS_SUPP5];
            int pb = 0;
            int parity = (((code[0] + code[2] + code[4]) * 3) + ((code[1] + code[3]) * 9)) % 10;
            bars[pb++] = 1;
            bars[pb++] = 1;
            bars[pb++] = 2;
            byte[] sequence = PARITY5[parity];
            for (int k = 0; k < sequence.Length; ++k) {
                if (k != 0) {
                    bars[pb++] = 1;
                    bars[pb++] = 1;
                }
                int c = code[k];
                byte[] stripes = BARS[c];
                if (sequence[k] == ODD) {
                    bars[pb++] = stripes[0];
                    bars[pb++] = stripes[1];
                    bars[pb++] = stripes[2];
                    bars[pb++] = stripes[3];
                }
                else {
                    bars[pb++] = stripes[3];
                    bars[pb++] = stripes[2];
                    bars[pb++] = stripes[1];
                    bars[pb++] = stripes[0];
                }
            }
            return bars;
        }   
        
        /** Gets the maximum area that the barcode and the text, if
        * any, will occupy. The lower left corner is always (0, 0).
        * @return the size the barcode occupies.
        */    
        public override Rectangle BarcodeSize {
            get {
                float width = 0;
                float height = barHeight;
                if (font != null) {
                    if (baseline <= 0)
                        height += -baseline + size;
                    else
                        height += baseline - font.GetFontDescriptor(BaseFont.DESCENT, size);
                }
                switch (codeType) {
                    case BarcodeEAN.EAN13:
                        width = x * (11 + 12 * 7);
                        if (font != null) {
                            width += font.GetWidthPoint(code[0], size);
                        }
                        break;
                    case EAN8:
                        width = x * (11 + 8 * 7);
                        break;
                    case UPCA:
                        width = x * (11 + 12 * 7);
                        if (font != null) {
                            width += font.GetWidthPoint(code[0], size) + font.GetWidthPoint(code[11], size);
                        }
                        break;
                    case UPCE:
                        width = x * (9 + 6 * 7);
                        if (font != null) {
                            width += font.GetWidthPoint(code[0], size) + font.GetWidthPoint(code[7], size);
                        }
                        break;
                    case SUPP2:
                        width = x * (6 + 2 * 7);
                        break;
                    case SUPP5:
                        width = x * (4 + 5 * 7 + 4 * 2);
                        break;
                    default:
                        throw new ArgumentException("Invalid code type.");
                }
                return new Rectangle(width, height);
            }
        }
        
        /** Places the barcode in a <CODE>PdfContentByte</CODE>. The
        * barcode is always placed at coodinates (0, 0). Use the
        * translation matrix to move it elsewhere.<p>
        * The bars and text are written in the following colors:<p>
        * <P><TABLE BORDER=1>
        * <TR>
        *    <TH><P><CODE>barColor</CODE></TH>
        *    <TH><P><CODE>textColor</CODE></TH>
        *    <TH><P>Result</TH>
        *    </TR>
        * <TR>
        *    <TD><P><CODE>null</CODE></TD>
        *    <TD><P><CODE>null</CODE></TD>
        *    <TD><P>bars and text painted with current fill color</TD>
        *    </TR>
        * <TR>
        *    <TD><P><CODE>barColor</CODE></TD>
        *    <TD><P><CODE>null</CODE></TD>
        *    <TD><P>bars and text painted with <CODE>barColor</CODE></TD>
        *    </TR>
        * <TR>
        *    <TD><P><CODE>null</CODE></TD>
        *    <TD><P><CODE>textColor</CODE></TD>
        *    <TD><P>bars painted with current color<br>text painted with <CODE>textColor</CODE></TD>
        *    </TR>
        * <TR>
        *    <TD><P><CODE>barColor</CODE></TD>
        *    <TD><P><CODE>textColor</CODE></TD>
        *    <TD><P>bars painted with <CODE>barColor</CODE><br>text painted with <CODE>textColor</CODE></TD>
        *    </TR>
        * </TABLE>
        * @param cb the <CODE>PdfContentByte</CODE> where the barcode will be placed
        * @param barColor the color of the bars. It can be <CODE>null</CODE>
        * @param textColor the color of the text. It can be <CODE>null</CODE>
        * @return the dimensions the barcode occupies
        */    
        public override Rectangle PlaceBarcode(PdfContentByte cb, Color barColor, Color textColor) {
            Rectangle rect = this.BarcodeSize;
            float barStartX = 0;
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
            switch (codeType) {
                case EAN13:
                case UPCA:
                case UPCE:
                    if (font != null)
                        barStartX += font.GetWidthPoint(code[0], size);
                    break;
            }
            byte[] bars = null;
            int[] guard = GUARD_EMPTY;
            switch (codeType) {
                case EAN13:
                    bars = GetBarsEAN13(code);
                    guard = GUARD_EAN13;
                    break;
                case EAN8:
                    bars = GetBarsEAN8(code);
                    guard = GUARD_EAN8;
                    break;
                case UPCA:
                    bars = GetBarsEAN13("0" + code);
                    guard = GUARD_UPCA;
                    break;
                case UPCE:
                    bars = GetBarsUPCE(code);
                    guard = GUARD_UPCE;
                    break;
                case SUPP2:
                    bars = GetBarsSupplemental2(code);
                    break;
                case SUPP5:
                    bars = GetBarsSupplemental5(code);
                    break;
            }
            float keepBarX = barStartX;
            bool print = true;
            float gd = 0;
            if (font != null && baseline > 0 && guardBars) {
                gd = baseline / 2;
            }
            if (barColor != null)
                cb.SetColorFill(barColor);
            for (int k = 0; k < bars.Length; ++k) {
                float w = bars[k] * x;
                if (print) {
                    if (Array.BinarySearch(guard, k) >= 0)
                        cb.Rectangle(barStartX, barStartY - gd, w - inkSpreading, barHeight + gd);
                    else
                        cb.Rectangle(barStartX, barStartY, w - inkSpreading, barHeight);
                }
                print = !print;
                barStartX += w;
            }
            cb.Fill();
            if (font != null) {
                if (textColor != null)
                    cb.SetColorFill(textColor);
                cb.BeginText();
                cb.SetFontAndSize(font, size);
                switch (codeType) {
                    case EAN13:
                        cb.SetTextMatrix(0, textStartY);
                        cb.ShowText(code.Substring(0, 1));
                        for (int k = 1; k < 13; ++k) {
                            string c = code.Substring(k, 1);
                            float len = font.GetWidthPoint(c, size);
                            float pX = keepBarX + TEXTPOS_EAN13[k - 1] * x - len / 2;
                            cb.SetTextMatrix(pX, textStartY);
                            cb.ShowText(c);
                        }
                        break;
                    case EAN8:
                        for (int k = 0; k < 8; ++k) {
                            string c = code.Substring(k, 1);
                            float len = font.GetWidthPoint(c, size);
                            float pX = TEXTPOS_EAN8[k] * x - len / 2;
                            cb.SetTextMatrix(pX, textStartY);
                            cb.ShowText(c);
                        }
                        break;
                    case UPCA:
                        cb.SetTextMatrix(0, textStartY);
                        cb.ShowText(code.Substring(0, 1));
                        for (int k = 1; k < 11; ++k) {
                            string c = code.Substring(k, 1);
                            float len = font.GetWidthPoint(c, size);
                            float pX = keepBarX + TEXTPOS_EAN13[k] * x - len / 2;
                            cb.SetTextMatrix(pX, textStartY);
                            cb.ShowText(c);
                        }
                        cb.SetTextMatrix(keepBarX + x * (11 + 12 * 7), textStartY);
                        cb.ShowText(code.Substring(11, 1));
                        break;
                    case UPCE:
                        cb.SetTextMatrix(0, textStartY);
                        cb.ShowText(code.Substring(0, 1));
                        for (int k = 1; k < 7; ++k) {
                            string c = code.Substring(k, 1);
                            float len = font.GetWidthPoint(c, size);
                            float pX = keepBarX + TEXTPOS_EAN13[k - 1] * x - len / 2;
                            cb.SetTextMatrix(pX, textStartY);
                            cb.ShowText(c);
                        }
                        cb.SetTextMatrix(keepBarX + x * (9 + 6 * 7), textStartY);
                        cb.ShowText(code.Substring(7, 1));
                        break;
                    case SUPP2:
                    case SUPP5:
                        for (int k = 0; k < code.Length; ++k) {
                            string c = code.Substring(k, 1);
                            float len = font.GetWidthPoint(c, size);
                            float pX = (7.5f + (9 * k)) * x - len / 2;
                            cb.SetTextMatrix(pX, textStartY);
                            cb.ShowText(c);
                        }
                        break;
                }
                cb.EndText();
            }
            return rect;
        }

        public override System.Drawing.Image CreateDrawingImage(System.Drawing.Color foreground, System.Drawing.Color background) {
            int width = 0;
            byte[] bars = null;
            switch (codeType) {
                case EAN13:
                    bars = GetBarsEAN13(code);
                    width = 11 + 12 * 7;
                    break;
                case EAN8:
                    bars = GetBarsEAN8(code);
                    width = 11 + 8 * 7;
                    break;
                case UPCA:
                    bars = GetBarsEAN13("0" + code);
                    width = 11 + 12 * 7;
                    break;
                case UPCE:
                    bars = GetBarsUPCE(code);
                    width = 9 + 6 * 7;
                    break;
                case SUPP2:
                    bars = GetBarsSupplemental2(code);
                    width = 6 + 2 * 7;
                    break;
                case SUPP5:
                    bars = GetBarsSupplemental5(code);
                    width = 4 + 5 * 7 + 4 * 2;
                    break;
                default:
                    throw new InvalidOperationException("Invalid code type.");
            }
            int height = (int)barHeight;
            System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(width, height);
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
    }
}
