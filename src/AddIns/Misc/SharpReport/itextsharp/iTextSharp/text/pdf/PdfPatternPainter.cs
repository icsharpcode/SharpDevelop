using System;
using iTextSharp.text;
/*
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
     * Implements the pattern.
     */

    public sealed class PdfPatternPainter : PdfTemplate {
    
        internal float xstep, ystep;
        internal bool stencil = false;
        internal Color defaultColor;
    
        /**
         *Creates a <CODE>PdfPattern</CODE>.
         */
    
        private PdfPatternPainter() : base() {
            type = TYPE_PATTERN;
        }
    
        /**
         * Creates new PdfPattern
         *
         * @param wr the <CODE>PdfWriter</CODE>
         */
    
        internal PdfPatternPainter(PdfWriter wr) : base(wr) {
            type = TYPE_PATTERN;
        }
    
        internal PdfPatternPainter(PdfWriter wr, Color defaultColor) : this(wr) {
            stencil = true;
            if (defaultColor == null)
                this.defaultColor = new Color(System.Drawing.Color.Gray);
            else
                this.defaultColor = defaultColor;
        }
    
        public float XStep {
            get {
                return this.xstep;
            }

            set {
                this.xstep = value;
            }
        }
    
        public float YStep {
            get {
                return this.ystep;
            }

            set {
                this.ystep = value;
            }
        }
    
        public bool IsStencil() {
            return stencil;
        }
    
        public void SetPatternMatrix(float a, float b, float c, float d, float e, float f) {
            SetMatrix(a, b, c, d, e, f);
        }
        /**
         * Gets the stream representing this pattern
         *
         * @return the stream representing this pattern
         */
    
        internal PdfPattern Pattern {
            get {
                return new PdfPattern(this);
            }
        }
    
        /**
         * Gets a duplicate of this <CODE>PdfPatternPainter</CODE>. All
         * the members are copied by reference but the buffer stays different.
         * @return a copy of this <CODE>PdfPatternPainter</CODE>
         */
    
        public override PdfContentByte Duplicate {
            get {
                PdfPatternPainter tpl = new PdfPatternPainter();
                tpl.writer = writer;
                tpl.pdf = pdf;
                tpl.thisReference = thisReference;
                tpl.pageResources = pageResources;
                tpl.bBox = new Rectangle(bBox);
                tpl.xstep = xstep;
                tpl.ystep = ystep;
                tpl.matrix = matrix;
                tpl.stencil = stencil;
                tpl.defaultColor = defaultColor;
                return tpl;
            }
        }
    
        public Color DefaultColor {
            get {
                return defaultColor;
            }
        }
    
        /**
        * @see com.lowagie.text.pdf.PdfContentByte#setGrayFill(float)
        */
        public override void SetGrayFill(float gray) {
            CheckNoColor();
            base.SetGrayFill(gray);
        }
        
        /**
        * @see com.lowagie.text.pdf.PdfContentByte#resetGrayFill()
        */
        public override void ResetGrayFill() {
            CheckNoColor();
            base.ResetGrayFill();
        }
        
        /**
        * @see com.lowagie.text.pdf.PdfContentByte#setGrayStroke(float)
        */
        public override void SetGrayStroke(float gray) {
            CheckNoColor();
            base.SetGrayStroke(gray);
        }
        
        /**
        * @see com.lowagie.text.pdf.PdfContentByte#resetGrayStroke()
        */
        public override void ResetGrayStroke() {
            CheckNoColor();
            base.ResetGrayStroke();
        }
        
        /**
        * @see com.lowagie.text.pdf.PdfContentByte#setRGBColorFillF(float, float, float)
        */
        public override void SetRGBColorFillF(float red, float green, float blue) {
            CheckNoColor();
            base.SetRGBColorFillF(red, green, blue);
        }
        
        /**
        * @see com.lowagie.text.pdf.PdfContentByte#resetRGBColorFill()
        */
        public override void ResetRGBColorFill() {
            CheckNoColor();
            base.ResetRGBColorFill();
        }
        
        /**
        * @see com.lowagie.text.pdf.PdfContentByte#setRGBColorStrokeF(float, float, float)
        */
        public override void SetRGBColorStrokeF(float red, float green, float blue) {
            CheckNoColor();
            base.SetRGBColorStrokeF(red, green, blue);
        }
        
        /**
        * @see com.lowagie.text.pdf.PdfContentByte#resetRGBColorStroke()
        */
        public override void ResetRGBColorStroke() {
            CheckNoColor();
            base.ResetRGBColorStroke();
        }
        
        /**
        * @see com.lowagie.text.pdf.PdfContentByte#setCMYKColorFillF(float, float, float, float)
        */
        public override void SetCMYKColorFillF(float cyan, float magenta, float yellow, float black) {
            CheckNoColor();
            base.SetCMYKColorFillF(cyan, magenta, yellow, black);
        }
        
        /**
        * @see com.lowagie.text.pdf.PdfContentByte#resetCMYKColorFill()
        */
        public override void ResetCMYKColorFill() {
            CheckNoColor();
            base.ResetCMYKColorFill();
        }
        
        /**
        * @see com.lowagie.text.pdf.PdfContentByte#setCMYKColorStrokeF(float, float, float, float)
        */
        public override void SetCMYKColorStrokeF(float cyan, float magenta, float yellow, float black) {
            CheckNoColor();
            base.SetCMYKColorStrokeF(cyan, magenta, yellow, black);
        }
        
        /**
        * @see com.lowagie.text.pdf.PdfContentByte#resetCMYKColorStroke()
        */
        public override void ResetCMYKColorStroke() {
            CheckNoColor();
            base.ResetCMYKColorStroke();
        }
        
        /**
        * @see com.lowagie.text.pdf.PdfContentByte#addImage(com.lowagie.text.Image, float, float, float, float, float, float)
        */
        public override void AddImage(Image image, float a, float b, float c, float d, float e, float f) {
            if (stencil && !image.IsMask())
                CheckNoColor();
            base.AddImage(image, a, b, c, d, e, f);
        }
        
        /**
        * @see com.lowagie.text.pdf.PdfContentByte#setCMYKColorFill(int, int, int, int)
        */
        public override void SetCMYKColorFill(int cyan, int magenta, int yellow, int black) {
            CheckNoColor();
            base.SetCMYKColorFill(cyan, magenta, yellow, black);
        }
        
        /**
        * @see com.lowagie.text.pdf.PdfContentByte#setCMYKColorStroke(int, int, int, int)
        */
        public override void SetCMYKColorStroke(int cyan, int magenta, int yellow, int black) {
            CheckNoColor();
            base.SetCMYKColorStroke(cyan, magenta, yellow, black);
        }
        
        /**
        * @see com.lowagie.text.pdf.PdfContentByte#setRGBColorFill(int, int, int)
        */
        public override void SetRGBColorFill(int red, int green, int blue) {
            CheckNoColor();
            base.SetRGBColorFill(red, green, blue);
        }
        
        /**
        * @see com.lowagie.text.pdf.PdfContentByte#setRGBColorStroke(int, int, int)
        */
        public override void SetRGBColorStroke(int red, int green, int blue) {
            CheckNoColor();
            base.SetRGBColorStroke(red, green, blue);
        }
        
        /**
        * @see com.lowagie.text.pdf.PdfContentByte#setColorStroke(java.awt.Color)
        */
        public override void SetColorStroke(Color color) {
            CheckNoColor();
            base.SetColorStroke(color);
        }
        
        /**
        * @see com.lowagie.text.pdf.PdfContentByte#setColorFill(java.awt.Color)
        */
        public override void SetColorFill(Color color) {
            CheckNoColor();
            base.SetColorFill(color);
        }
        
        /**
        * @see com.lowagie.text.pdf.PdfContentByte#setColorFill(com.lowagie.text.pdf.PdfSpotColor, float)
        */
        public override void SetColorFill(PdfSpotColor sp, float tint) {
            CheckNoColor();
            base.SetColorFill(sp, tint);
        }
        
        /**
        * @see com.lowagie.text.pdf.PdfContentByte#setColorStroke(com.lowagie.text.pdf.PdfSpotColor, float)
        */
        public override void SetColorStroke(PdfSpotColor sp, float tint) {
            CheckNoColor();
            base.SetColorStroke(sp, tint);
        }
        
        /**
        * @see com.lowagie.text.pdf.PdfContentByte#setPatternFill(com.lowagie.text.pdf.PdfPatternPainter)
        */
        public override void SetPatternFill(PdfPatternPainter p) {
            CheckNoColor();
            base.SetPatternFill(p);
        }
        
        /**
        * @see com.lowagie.text.pdf.PdfContentByte#setPatternFill(com.lowagie.text.pdf.PdfPatternPainter, java.awt.Color, float)
        */
        public override void SetPatternFill(PdfPatternPainter p, Color color, float tint) {
            CheckNoColor();
            base.SetPatternFill(p, color, tint);
        }
        
        /**
        * @see com.lowagie.text.pdf.PdfContentByte#setPatternStroke(com.lowagie.text.pdf.PdfPatternPainter, java.awt.Color, float)
        */
        public override void SetPatternStroke(PdfPatternPainter p, Color color, float tint) {
            CheckNoColor();
            base.SetPatternStroke(p, color, tint);
        }
        
        /**
        * @see com.lowagie.text.pdf.PdfContentByte#setPatternStroke(com.lowagie.text.pdf.PdfPatternPainter)
        */
        public override void SetPatternStroke(PdfPatternPainter p) {
            CheckNoColor();
            base.SetPatternStroke(p);
        }
    
        internal void CheckNoColor() {
            if (stencil)
                throw new ArgumentException("Colors are not allowed in uncolored tile patterns.");
        }
    }
}