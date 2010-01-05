using System;
using iTextSharp.text;
/*
 * $Id: BarcodeCodabar.cs,v 1.7 2007/02/22 20:48:38 psoares33 Exp $
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

namespace iTextSharp.text.pdf
{
    /** Implements the code codabar. The default parameters are:
    * <pre>
    *x = 0.8f;
    *n = 2;
    *font = BaseFont.CreateFont("Helvetica", "winansi", false);
    *size = 8;
    *baseline = size;
    *barHeight = size * 3;
    *textAlignment = Element.ALIGN_CENTER;
    *generateChecksum = false;
    *checksumText = false;
    *startStopText = false;
    * </pre>
    *
    * @author Paulo Soares (psoares@consiste.pt)
    */
    public class BarcodeCodabar : Barcode{

        /** The bars to generate the code.
        */    
        private static readonly byte[][] BARS = new byte[][] {
            new byte[]{0,0,0,0,0,1,1}, // 0
            new byte[]{0,0,0,0,1,1,0}, // 1
            new byte[]{0,0,0,1,0,0,1}, // 2
            new byte[]{1,1,0,0,0,0,0}, // 3
            new byte[]{0,0,1,0,0,1,0}, // 4
            new byte[]{1,0,0,0,0,1,0}, // 5
            new byte[]{0,1,0,0,0,0,1}, // 6
            new byte[]{0,1,0,0,1,0,0}, // 7
            new byte[]{0,1,1,0,0,0,0}, // 8
            new byte[]{1,0,0,1,0,0,0}, // 9
            new byte[]{0,0,0,1,1,0,0}, // -
            new byte[]{0,0,1,1,0,0,0}, // $
            new byte[]{1,0,0,0,1,0,1}, // :
            new byte[]{1,0,1,0,0,0,1}, // /
            new byte[]{1,0,1,0,1,0,0}, // .
            new byte[]{0,0,1,0,1,0,1}, // +
            new byte[]{0,0,1,1,0,1,0}, // a
            new byte[]{0,1,0,1,0,0,1}, // b
            new byte[]{0,0,0,1,0,1,1}, // c
            new byte[]{0,0,0,1,1,1,0}  // d
        };
     
        /** The index chars to <CODE>BARS</CODE>.
        */    
        private const string CHARS = "0123456789-$:/.+ABCD";
        
        private const int START_STOP_IDX = 16;    
        /** Creates a new BarcodeCodabar.
        */    
        public BarcodeCodabar() {
            x = 0.8f;
            n = 2;
            font = BaseFont.CreateFont("Helvetica", "winansi", false);
            size = 8;
            baseline = size;
            barHeight = size * 3;
            textAlignment = Element.ALIGN_CENTER;
            generateChecksum = false;
            checksumText = false;
            startStopText = false;
            codeType = CODABAR;
        }
        
        /** Creates the bars.
        * @param text the text to create the bars
        * @return the bars
        */    
        public static byte[] GetBarsCodabar(String text) {
            text = text.ToUpper(System.Globalization.CultureInfo.InvariantCulture);
            int len = text.Length;
            if (len < 2)
                throw new ArgumentException("Codabar must have at least a start and stop character.");
            if (CHARS.IndexOf(text[0]) < START_STOP_IDX || CHARS.IndexOf(text[len - 1]) < START_STOP_IDX)
                throw new ArgumentException("Codabar must have one of 'ABCD' as start/stop character.");
            byte[] bars = new byte[text.Length * 8 - 1];
            for (int k = 0; k < len; ++k) {
                int idx = CHARS.IndexOf(text[k]);
                if (idx >= START_STOP_IDX && k > 0 && k < len - 1)
                    throw new ArgumentException("In codabar, start/stop characters are only allowed at the extremes.");
                if (idx < 0)
                    throw new ArgumentException("The character '" + text[k] + "' is illegal in codabar.");
                Array.Copy(BARS[idx], 0, bars, k * 8, 7);
            }
            return bars;
        }
        
        public static String CalculateChecksum(String code) {
            if (code.Length < 2)
                return code;
            String text = code.ToUpper(System.Globalization.CultureInfo.InvariantCulture);
            int sum = 0;
            int len = text.Length;
            for (int k = 0; k < len; ++k)
                sum += CHARS.IndexOf(text[k]);
            sum = (sum + 15) / 16 * 16 - sum;
            return code.Substring(0, len - 1) + CHARS[sum] + code.Substring(len - 1);
        }
        
        /** Gets the maximum area that the barcode and the text, if
        * any, will occupy. The lower left corner is always (0, 0).
        * @return the size the barcode occupies.
        */    
        public override Rectangle BarcodeSize {
            get {
                float fontX = 0;
                float fontY = 0;
                String text = code;
                if (generateChecksum && checksumText)
                    text = CalculateChecksum(code);
                if (!startStopText)
                    text = text.Substring(1, text.Length - 2);
                if (font != null) {
                    if (baseline > 0)
                        fontY = baseline - font.GetFontDescriptor(BaseFont.DESCENT, size);
                    else
                        fontY = -baseline + size;
                    fontX = font.GetWidthPoint(altText != null ? altText : text, size);
                }
                text = code;
                if (generateChecksum)
                    text = CalculateChecksum(code);
                byte[] bars = GetBarsCodabar(text);
                int wide = 0;
                for (int k = 0; k < bars.Length; ++k) {
                    wide += (int)bars[k];
                }
                int narrow = bars.Length - wide;
                float fullWidth = x * (narrow + wide * n);
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
            String fullCode = code;
            if (generateChecksum && checksumText)
                fullCode = CalculateChecksum(code);
            if (!startStopText)
                fullCode = fullCode.Substring(1, fullCode.Length - 2);
            float fontX = 0;
            if (font != null) {
                fontX = font.GetWidthPoint(fullCode = altText != null ? altText : fullCode, size);
            }
            byte[] bars = GetBarsCodabar(generateChecksum ? CalculateChecksum(code) : code);
            int wide = 0;
            for (int k = 0; k < bars.Length; ++k) {
                wide += (int)bars[k];
            }
            int narrow = bars.Length - wide;
            float fullWidth = x * (narrow + wide * n);
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
            bool print = true;
            if (barColor != null)
                cb.SetColorFill(barColor);
            for (int k = 0; k < bars.Length; ++k) {
                float w = (bars[k] == 0 ? x : x * n);
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
            return BarcodeSize;
        }

        public override System.Drawing.Image CreateDrawingImage(System.Drawing.Color foreground, System.Drawing.Color background) {
            String fullCode = code;
            if (generateChecksum && checksumText)
                fullCode = CalculateChecksum(code);
            if (!startStopText)
                fullCode = fullCode.Substring(1, fullCode.Length - 2);
            byte[] bars = GetBarsCodabar(generateChecksum ? CalculateChecksum(code) : code);
            int wide = 0;
            for (int k = 0; k < bars.Length; ++k) {
                wide += (int)bars[k];
            }
            int narrow = bars.Length - wide;
            int fullWidth = narrow + wide * (int)n;
            int height = (int)barHeight;
            System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(fullWidth, height);
            for (int h = 0; h < height; ++h) {
                bool print = true;
                int ptr = 0;
                for (int k = 0; k < bars.Length; ++k) {
                    int w = (bars[k] == 0 ? 1 : (int)n);
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
