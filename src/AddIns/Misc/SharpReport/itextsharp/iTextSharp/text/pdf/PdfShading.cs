using System;
using iTextSharp.text;
/*
 * Copyright 2002 Paulo Soares
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

    /** Implements the shading dictionary (or stream).
     *
     * @author Paulo Soares (psoares@consiste.pt)
     */
    public class PdfShading {

        protected PdfDictionary shading;
    
        protected PdfWriter writer;
    
        protected int shadingType;
    
        protected ColorDetails colorDetails;
    
        protected PdfName shadingName;
    
        protected PdfIndirectReference shadingReference;
    
        /** Holds value of property bBox. */
        protected float[] bBox;
    
        /** Holds value of property antiAlias. */
        protected bool antiAlias = false;

        private Color cspace;
    
        /** Creates new PdfShading */
        protected PdfShading(PdfWriter writer) {
            this.writer = writer;
        }
    
        protected void SetColorSpace(Color color) {
            cspace = color;
            int type = ExtendedColor.GetType(color);
            PdfObject colorSpace = null;
            switch (type) {
                case ExtendedColor.TYPE_GRAY: {
                    colorSpace = PdfName.DEVICEGRAY;
                    break;
                }
                case ExtendedColor.TYPE_CMYK: {
                    colorSpace = PdfName.DEVICECMYK;
                    break;
                }
                case ExtendedColor.TYPE_SEPARATION: {
                    SpotColor spot = (SpotColor)color;
                    colorDetails = writer.AddSimple(spot.PdfSpotColor);
                    colorSpace = colorDetails.IndirectReference;
                    break;
                }
                case ExtendedColor.TYPE_PATTERN:
                case ExtendedColor.TYPE_SHADING: {
                    ThrowColorSpaceError();
                    break;
                }
                default:
                    colorSpace = PdfName.DEVICERGB;
                    break;
            }
            shading.Put(PdfName.COLORSPACE, colorSpace);
        }
    
        public Color ColorSpace {
            get {
                return cspace;
            }
        }

        public static void ThrowColorSpaceError() {
            throw new ArgumentException("A tiling or shading pattern cannot be used as a color space in a shading pattern");
        }
    
        public static void CheckCompatibleColors(Color c1, Color c2) {
            int type1 = ExtendedColor.GetType(c1);
            int type2 = ExtendedColor.GetType(c2);
            if (type1 != type2)
                throw new ArgumentException("Both colors must be of the same type.");
            if (type1 == ExtendedColor.TYPE_SEPARATION && ((SpotColor)c1).PdfSpotColor != ((SpotColor)c2).PdfSpotColor)
                throw new ArgumentException("The spot color must be the same, only the tint can vary.");
            if (type1 == ExtendedColor.TYPE_PATTERN || type1 == ExtendedColor.TYPE_SHADING)
                ThrowColorSpaceError();
        }
    
        public static float[] GetColorArray(Color color) {
            int type = ExtendedColor.GetType(color);
            switch (type) {
                case ExtendedColor.TYPE_GRAY: {
                    return new float[]{((GrayColor)color).Gray};
                }
                case ExtendedColor.TYPE_CMYK: {
                    CMYKColor cmyk = (CMYKColor)color;
                    return new float[]{cmyk.Cyan, cmyk.Magenta, cmyk.Yellow, cmyk.Black};
                }
                case ExtendedColor.TYPE_SEPARATION: {
                    return new float[]{((SpotColor)color).Tint};
                }
                case ExtendedColor.TYPE_RGB: {
                    return new float[]{color.R / 255f, color.G / 255f, color.B / 255f};
                }
            }
            ThrowColorSpaceError();
            return null;
        }

        public static PdfShading Type1(PdfWriter writer, Color colorSpace, float[] domain, float[] tMatrix, PdfFunction function) {
            PdfShading sp = new PdfShading(writer);
            sp.shading = new PdfDictionary();
            sp.shadingType = 1;
            sp.shading.Put(PdfName.SHADINGTYPE, new PdfNumber(sp.shadingType));
            sp.SetColorSpace(colorSpace);
            if (domain != null)
                sp.shading.Put(PdfName.DOMAIN, new PdfArray(domain));
            if (tMatrix != null)
                sp.shading.Put(PdfName.MATRIX, new PdfArray(tMatrix));
            sp.shading.Put(PdfName.FUNCTION, function.Reference);
            return sp;
        }
    
        public static PdfShading Type2(PdfWriter writer, Color colorSpace, float[] coords, float[] domain, PdfFunction function, bool[] extend) {
            PdfShading sp = new PdfShading(writer);
            sp.shading = new PdfDictionary();
            sp.shadingType = 2;
            sp.shading.Put(PdfName.SHADINGTYPE, new PdfNumber(sp.shadingType));
            sp.SetColorSpace(colorSpace);
            sp.shading.Put(PdfName.COORDS, new PdfArray(coords));
            if (domain != null)
                sp.shading.Put(PdfName.DOMAIN, new PdfArray(domain));
            sp.shading.Put(PdfName.FUNCTION, function.Reference);
            if (extend != null && (extend[0] || extend[1])) {
                PdfArray array = new PdfArray(extend[0] ? PdfBoolean.PDFTRUE : PdfBoolean.PDFFALSE);
                array.Add(extend[1] ? PdfBoolean.PDFTRUE : PdfBoolean.PDFFALSE);
                sp.shading.Put(PdfName.EXTEND, array);
            }
            return sp;
        }

        public static PdfShading Type3(PdfWriter writer, Color colorSpace, float[] coords, float[] domain, PdfFunction function, bool[] extend) {
            PdfShading sp = Type2(writer, colorSpace, coords, domain, function, extend);
            sp.shadingType = 3;
            sp.shading.Put(PdfName.SHADINGTYPE, new PdfNumber(sp.shadingType));
            return sp;
        }
    
        public static PdfShading SimpleAxial(PdfWriter writer, float x0, float y0, float x1, float y1, Color startColor, Color endColor, bool extendStart, bool extendEnd) {
            CheckCompatibleColors(startColor, endColor);
            PdfFunction function = PdfFunction.Type2(writer, new float[]{0, 1}, null, GetColorArray(startColor),
                GetColorArray(endColor), 1);
            return Type2(writer, startColor, new float[]{x0, y0, x1, y1}, null, function, new bool[]{extendStart, extendEnd});
        }
    
        public static PdfShading SimpleAxial(PdfWriter writer, float x0, float y0, float x1, float y1, Color startColor, Color endColor) {
            return SimpleAxial(writer, x0, y0, x1, y1, startColor, endColor, true, true);
        }
    
        public static PdfShading SimpleRadial(PdfWriter writer, float x0, float y0, float r0, float x1, float y1, float r1, Color startColor, Color endColor, bool extendStart, bool extendEnd) {
            CheckCompatibleColors(startColor, endColor);
            PdfFunction function = PdfFunction.Type2(writer, new float[]{0, 1}, null, GetColorArray(startColor),
                GetColorArray(endColor), 1);
            return Type3(writer, startColor, new float[]{x0, y0, r0, x1, y1, r1}, null, function, new bool[]{extendStart, extendEnd});
        }

        public static PdfShading SimpleRadial(PdfWriter writer, float x0, float y0, float r0, float x1, float y1, float r1, Color startColor, Color endColor) {
            return SimpleRadial(writer, x0, y0, r0, x1, y1, r1, startColor, endColor, true, true);
        }

        internal PdfName ShadingName {
            get {
                return shadingName;
            }
        }
    
        internal PdfIndirectReference ShadingReference {
            get {
                if (shadingReference == null)
                    shadingReference = writer.PdfIndirectReference;
                return shadingReference;
            }
        }
    
        internal int Name {
            set {
                shadingName = new PdfName("Sh" + value);
            }
        }
    
        internal void AddToBody() {
            if (bBox != null)
                shading.Put(PdfName.BBOX, new PdfArray(bBox));
            if (antiAlias)
                shading.Put(PdfName.ANTIALIAS, PdfBoolean.PDFTRUE);
            writer.AddToBody(shading, this.ShadingReference);
        }
    
        internal PdfWriter Writer {
            get {
                return writer;
            }
        }
    
        internal ColorDetails ColorDetails {
            get {
                return colorDetails;
            }
        }
    
        public float[] BBox {
            get {
                return bBox;
            }
            set {
                if (value.Length != 4)
                    throw new ArgumentException("BBox must be a 4 element array.");
                this.bBox = value;
            }
        }
    
        public bool AntiAlias {
            set {
                this.antiAlias = value;
            }
            get {
                return antiAlias;
            }
        }
    
    }
}