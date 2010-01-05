using System;
using System.Collections;
using iTextSharp.text.pdf;
using iTextSharp.text;
using iTextSharp.text.pdf.interfaces;

/*
 * $Id: PdfXConformanceImp.cs,v 1.3 2007/06/05 15:00:44 psoares33 Exp $
 *
 * Copyright 2006 Bruno Lowagie (based on code by Paulo Soares)
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

namespace iTextSharp.text.pdf.intern {

    public class PdfXConformanceImp : IPdfXConformance {

        /** A key for an aspect that can be checked for PDF/X Conformance. */
        public const int PDFXKEY_COLOR = 1;
        /** A key for an aspect that can be checked for PDF/X Conformance. */
        public const int PDFXKEY_CMYK = 2;
        /** A key for an aspect that can be checked for PDF/X Conformance. */
        public const int PDFXKEY_RGB = 3;
        /** A key for an aspect that can be checked for PDF/X Conformance. */
        public const int PDFXKEY_FONT = 4;
        /** A key for an aspect that can be checked for PDF/X Conformance. */
        public const int PDFXKEY_IMAGE = 5;
        /** A key for an aspect that can be checked for PDF/X Conformance. */
        public const int PDFXKEY_GSTATE = 6;
        /** A key for an aspect that can be checked for PDF/X Conformance. */
        public const int PDFXKEY_LAYER = 7;

        /**
        * The value indicating if the PDF has to be in conformance with PDF/X.
        */
        protected internal int pdfxConformance = PdfWriter.PDFXNONE;
        
        /**
        * @see com.lowagie.text.pdf.interfaces.PdfXConformance#setPDFXConformance(int)
        */
        public int PDFXConformance {
            set {
                this.pdfxConformance = value;
            }
            get {
                return pdfxConformance;
            }
        }

        /**
        * Checks if the PDF/X Conformance is necessary.
        * @return true if the PDF has to be in conformance with any of the PDF/X specifications
        */
        public bool IsPdfX() {
            return pdfxConformance != PdfWriter.PDFXNONE;
        }
        /**
        * Checks if the PDF has to be in conformance with PDF/X-1a:2001
        * @return true of the PDF has to be in conformance with PDF/X-1a:2001
        */
        public bool IsPdfX1A2001() {
            return pdfxConformance == PdfWriter.PDFX1A2001;
        }
        /**
        * Checks if the PDF has to be in conformance with PDF/X-3:2002
        * @return true of the PDF has to be in conformance with PDF/X-3:2002
        */
        public bool IsPdfX32002() {
            return pdfxConformance == PdfWriter.PDFX32002;
        }
        
        /**
        * Checks if the PDF has to be in conformance with PDFA1
        * @return true of the PDF has to be in conformance with PDFA1
        */
        public bool IsPdfA1() {
    	    return pdfxConformance == PdfWriter.PDFA1A || pdfxConformance == PdfWriter.PDFA1B;
        }
        
        /**
        * Checks if the PDF has to be in conformance with PDFA1A
        * @return true of the PDF has to be in conformance with PDFA1A
        */
        public bool IsPdfA1A() {
    	    return pdfxConformance == PdfWriter.PDFA1A;
        }

        public void CompleteInfoDictionary(PdfDictionary info) {
            if (IsPdfX() && !IsPdfA1()) {
                if (info.Get(PdfName.GTS_PDFXVERSION) == null) {
                    if (IsPdfX1A2001()) {
                        info.Put(PdfName.GTS_PDFXVERSION, new PdfString("PDF/X-1:2001"));
                        info.Put(new PdfName("GTS_PDFXConformance"), new PdfString("PDF/X-1a:2001"));
                    }
                    else if (IsPdfX32002())
                        info.Put(PdfName.GTS_PDFXVERSION, new PdfString("PDF/X-3:2002"));
                }
                if (info.Get(PdfName.TITLE) == null) {
                    info.Put(PdfName.TITLE, new PdfString("Pdf document"));
                }
                if (info.Get(PdfName.CREATOR) == null) {
                    info.Put(PdfName.CREATOR, new PdfString("Unknown"));
                }
                if (info.Get(PdfName.TRAPPED) == null) {
                    info.Put(PdfName.TRAPPED, new PdfName("False"));
                }
            }
        }
        
        public void CompleteExtraCatalog(PdfDictionary extraCatalog) {
            if (IsPdfX() && !IsPdfA1()) {
                if (extraCatalog.Get(PdfName.OUTPUTINTENTS) == null) {
                    PdfDictionary outp = new PdfDictionary(PdfName.OUTPUTINTENT);
                    outp.Put(PdfName.OUTPUTCONDITION, new PdfString("SWOP CGATS TR 001-1995"));
                    outp.Put(PdfName.OUTPUTCONDITIONIDENTIFIER, new PdfString("CGATS TR 001"));
                    outp.Put(PdfName.REGISTRYNAME, new PdfString("http://www.color.org"));
                    outp.Put(PdfName.INFO, new PdfString(""));
                    outp.Put(PdfName.S, PdfName.GTS_PDFX);
                    extraCatalog.Put(PdfName.OUTPUTINTENTS, new PdfArray(outp));
                }
            }
        }
        
        /**
        * Business logic that checks if a certain object is in conformance with PDF/X.
        * @param writer    the writer that is supposed to write the PDF/X file
        * @param key       the type of PDF/X conformance that has to be checked
        * @param obj1      the object that is checked for conformance
        */
        public static void CheckPDFXConformance(PdfWriter writer, int key, Object obj1) {
            if (writer == null || !writer.IsPdfX())
                return;
            int conf = writer.PDFXConformance;
            switch (key) {
                case PDFXKEY_COLOR:
                    switch (conf) {
                        case PdfWriter.PDFX1A2001:
                            if (obj1 is ExtendedColor) {
                                ExtendedColor ec = (ExtendedColor)obj1;
                                switch (ec.Type) {
                                    case ExtendedColor.TYPE_CMYK:
                                    case ExtendedColor.TYPE_GRAY:
                                        return;
                                    case ExtendedColor.TYPE_RGB:
                                        throw new PdfXConformanceException("Colorspace RGB is not allowed.");
                                    case ExtendedColor.TYPE_SEPARATION:
                                        SpotColor sc = (SpotColor)ec;
                                        CheckPDFXConformance(writer, PDFXKEY_COLOR, sc.PdfSpotColor.AlternativeCS);
                                        break;
                                    case ExtendedColor.TYPE_SHADING:
                                        ShadingColor xc = (ShadingColor)ec;
                                        CheckPDFXConformance(writer, PDFXKEY_COLOR, xc.PdfShadingPattern.Shading.ColorSpace);
                                        break;
                                    case ExtendedColor.TYPE_PATTERN:
                                        PatternColor pc = (PatternColor)ec;
                                        CheckPDFXConformance(writer, PDFXKEY_COLOR, pc.Painter.DefaultColor);
                                        break;
                                }
                            }
                            else if (obj1 is Color)
                                throw new PdfXConformanceException("Colorspace RGB is not allowed.");
                            break;
                    }
                    break;
                case PDFXKEY_CMYK:
                    break;
                case PDFXKEY_RGB:
                    if (conf == PdfWriter.PDFX1A2001)
                        throw new PdfXConformanceException("Colorspace RGB is not allowed.");
                    break;
                case PDFXKEY_FONT:
                    if (!((BaseFont)obj1).IsEmbedded())
                        throw new PdfXConformanceException("All the fonts must be embedded.");
                    break;
                case PDFXKEY_IMAGE:
                    PdfImage image = (PdfImage)obj1;
                    if (image.Get(PdfName.SMASK) != null)
                        throw new PdfXConformanceException("The /SMask key is not allowed in images.");
                    switch (conf) {
                        case PdfWriter.PDFX1A2001:
                            PdfObject cs = image.Get(PdfName.COLORSPACE);
                            if (cs == null)
                                return;
                            if (cs.IsName()) {
                                if (PdfName.DEVICERGB.Equals(cs))
                                    throw new PdfXConformanceException("Colorspace RGB is not allowed.");
                            }
                            else if (cs.IsArray()) {
                                if (PdfName.CALRGB.Equals(((PdfArray)cs).ArrayList[0]))
                                    throw new PdfXConformanceException("Colorspace CalRGB is not allowed.");
                            }
                            break;
                    }
                    break;
                case PDFXKEY_GSTATE:
                    PdfDictionary gs = (PdfDictionary)obj1;
                    PdfObject obj = gs.Get(PdfName.BM);
                    if (obj != null && !PdfGState.BM_NORMAL.Equals(obj) && !PdfGState.BM_COMPATIBLE.Equals(obj))
                        throw new PdfXConformanceException("Blend mode " + obj.ToString() + " not allowed.");
                    obj = gs.Get(PdfName.CA);
                    double v = 0.0;
                    if (obj != null && (v = ((PdfNumber)obj).DoubleValue) != 1.0)
                        throw new PdfXConformanceException("Transparency is not allowed: /CA = " + v);
                    obj = gs.Get(PdfName.ca_);
                    v = 0.0;
                    if (obj != null && (v = ((PdfNumber)obj).DoubleValue) != 1.0)
                        throw new PdfXConformanceException("Transparency is not allowed: /ca = " + v);
                    break;
                case PDFXKEY_LAYER:
                    throw new PdfXConformanceException("Layers are not allowed.");
            }
        }
    }
}