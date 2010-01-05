using System;
using iTextSharp.text;
/*
 * $Id: Barcode.cs,v 1.4 2006/07/31 13:51:38 psoares33 Exp $
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
    /** Base class containing properties and methods commom to all
     * barcode types.
     *
     * @author Paulo Soares (psoares@consiste.pt)
     */
    public abstract class Barcode {
        /** A type of barcode */
        public const int EAN13 = 1;
        /** A type of barcode */
        public const int EAN8 = 2;
        /** A type of barcode */
        public const int UPCA = 3;
        /** A type of barcode */
        public const int UPCE = 4;
        /** A type of barcode */
        public const int SUPP2 = 5;
        /** A type of barcode */
        public const int SUPP5 = 6;
        /** A type of barcode */
        public const int POSTNET = 7;
        /** A type of barcode */
        public const int PLANET = 8;
        /** A type of barcode */
        public const int CODE128 = 9;
        /** A type of barcode */
        public const int CODE128_UCC = 10;
        /** A type of barcode */
        public const int CODE128_RAW = 11;
        /** A type of barcode */
        public const int CODABAR = 12;

        /** The minimum bar width.
         */
        protected float x;    

        /** The bar multiplier for wide bars or the distance between
         * bars for Postnet and Planet.
         */
        protected float n;
    
        /** The text font. <CODE>null</CODE> if no text.
         */
        protected BaseFont font;

        /** The size of the text or the height of the shorter bar
         * in Postnet.
         */    
        protected float size;
    
        /** If positive, the text distance under the bars. If zero or negative,
         * the text distance above the bars.
         */
        protected float baseline;
    
        /** The height of the bars.
         */
        protected float barHeight;
    
        /** The text Element. Can be <CODE>Element.ALIGN_LEFT</CODE>,
         * <CODE>Element.ALIGN_CENTER</CODE> or <CODE>Element.ALIGN_RIGHT</CODE>.
         */
        protected int textAlignment;
    
        /** The optional checksum generation.
         */
        protected bool generateChecksum;
    
        /** Shows the generated checksum in the the text.
         */
        protected bool checksumText;
    
        /** Show the start and stop character '*' in the text for
         * the barcode 39 or 'ABCD' for codabar.
         */
        protected bool startStopText;
    
        /** Generates extended barcode 39.
         */
        protected bool extended;
    
        /** The code to generate.
         */
        protected string code = "";
    
        /** Show the guard bars for barcode EAN.
         */
        protected bool guardBars;
    
        /** The code type.
         */
        protected int codeType;
    
        /** The ink spreading. */
        protected float inkSpreading = 0;

        /** Gets the minimum bar width.
         * @return the minimum bar width
         */
        public float X {
            get {
                return x;
            }

            set {
                this.x = value;
            }
        }
    
        /** Gets the bar multiplier for wide bars.
         * @return the bar multiplier for wide bars
         */
        public float N {
            get {
                return n;
            }

            set {
                this.n = value;
            }
        }
    
        /** Gets the text font. <CODE>null</CODE> if no text.
         * @return the text font. <CODE>null</CODE> if no text
         */
        public BaseFont Font {
            get {
                return font;
            }

            set {
                this.font = value;
            }
        }
    
        /** Gets the size of the text.
         * @return the size of the text
         */
        public float Size {
            get {
                return size;
            }

            set {
                this.size = value;
            }
        }
    
        /** Gets the text baseline.
         * If positive, the text distance under the bars. If zero or negative,
         * the text distance above the bars.
         * @return the baseline.
         */
        public float Baseline {
            get {
                return baseline;
            }

            set {
                this.baseline = value;
            }
        }
    
        /** Gets the height of the bars.
         * @return the height of the bars
         */
        public float BarHeight {
            get {
                return barHeight;
            }

            set {
                this.barHeight = value;
            }
        }
    
        /** Gets the text Element. Can be <CODE>Element.ALIGN_LEFT</CODE>,
         * <CODE>Element.ALIGN_CENTER</CODE> or <CODE>Element.ALIGN_RIGHT</CODE>.
         * @return the text alignment
         */
        public int TextAlignment{
            get {
                return textAlignment;
            }

            set {
                this.textAlignment = value;
            }
        }
    
        /** The property for the optional checksum generation.
         */
        public bool GenerateChecksum {
            set {
                this.generateChecksum = value;
            }
            get {
                return generateChecksum;
            }
        }
    
        /** Sets the property to show the generated checksum in the the text.
         * @param checksumText new value of property checksumText
         */
        public bool ChecksumText {
            set {
                this.checksumText = value;
            }
            get {
                return checksumText;
            }
        }
    
        /** Gets the property to show the start and stop character '*' in the text for
         * the barcode 39.
         * @param startStopText new value of property startStopText
         */
        public bool StartStopText {
            set {
                this.startStopText = value;
            }
            get {
                return startStopText;
            }
        }
    
        /** Sets the property to generate extended barcode 39.
         * @param extended new value of property extended
         */
        public bool Extended {
            set {
                this.extended = value;
            }
            get {
                return extended;
            }
        }
    
        /** Gets the code to generate.
         * @return the code to generate
         */
        public virtual string Code {
            get {
                return code;
            }

            set {
                this.code = value;
            }
        }
    
        /** Sets the property to show the guard bars for barcode EAN.
         * @param guardBars new value of property guardBars
         */
        public bool GuardBars {
            set {
                this.guardBars = value;
            }
            get {
                return guardBars;
            }
        }
    
        /** Gets the code type.
         * @return the code type
         */
        public int CodeType {
            get {
                return codeType;
            }

            set {
                this.codeType = value;
            }
        }
    
        /** Gets the maximum area that the barcode and the text, if
         * any, will occupy. The lower left corner is always (0, 0).
         * @return the size the barcode occupies.
         */    
        public abstract Rectangle BarcodeSize {
            get;
        }

        public float InkSpreading {
            set {
                inkSpreading = value;
            }
            get {
                return inkSpreading;
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
        public abstract Rectangle PlaceBarcode(PdfContentByte cb, Color barColor, Color textColor);
    
        /** Creates a template with the barcode.
         * @param cb the <CODE>PdfContentByte</CODE> to create the template. It
         * serves no other use
         * @param barColor the color of the bars. It can be <CODE>null</CODE>
         * @param textColor the color of the text. It can be <CODE>null</CODE>
         * @return the template
         * @see #placeBarcode(PdfContentByte cb, Color barColor, Color textColor)
         */    
        public PdfTemplate CreateTemplateWithBarcode(PdfContentByte cb, Color barColor, Color textColor) {
            PdfTemplate tp = cb.CreateTemplate(0, 0);
            Rectangle rect = PlaceBarcode(tp, barColor, textColor);
            tp.BoundingBox = rect;
            return tp;
        }
    
        /** Creates an <CODE>Image</CODE> with the barcode.
         * @param cb the <CODE>PdfContentByte</CODE> to create the <CODE>Image</CODE>. It
         * serves no other use
         * @param barColor the color of the bars. It can be <CODE>null</CODE>
         * @param textColor the color of the text. It can be <CODE>null</CODE>
         * @return the <CODE>Image</CODE>
         * @see #placeBarcode(PdfContentByte cb, Color barColor, Color textColor)
         */    
        public Image CreateImageWithBarcode(PdfContentByte cb, Color barColor, Color textColor) {
            return Image.GetInstance(CreateTemplateWithBarcode(cb, barColor, textColor));
        }

        /**
        * The alternate text to be used, if present.
        */
        protected String altText;

        /**
        * Sets the alternate text. If present, this text will be used instead of the
        * text derived from the supplied code.
        * @param altText the alternate text
        */
        public String AltText {
            set {
                altText = value;
            }
            get {
                return altText;
            }
        }

        public abstract System.Drawing.Image CreateDrawingImage(System.Drawing.Color foreground, System.Drawing.Color background);
    }
}
