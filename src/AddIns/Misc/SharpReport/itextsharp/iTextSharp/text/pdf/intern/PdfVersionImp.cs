using System;
using iTextSharp.text.pdf;
using iTextSharp.text;
using iTextSharp.text.pdf.interfaces;
/*
 * $Id: PdfVersionImp.cs,v 1.1 2007/02/09 15:34:40 psoares33 Exp $
 *
 * Copyright 2006 Bruno Lowagie
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

    /**
    * Stores the PDF version information,
    * knows how to write a PDF Header,
    * and how to add the version to the catalog (if necessary).
    */

    public class PdfVersionImp : IPdfVersion {

        /** Contains different strings that are part of the header. */
        public static readonly byte[][] HEADER = {
            DocWriter.GetISOBytes("\n"),
            DocWriter.GetISOBytes("%PDF-"),
            DocWriter.GetISOBytes("\n%\u00e2\u00e3\u00cf\u00d3\n")
        };
        /** Indicates if the header was already written. */
        protected bool headerWasWritten = false;
        /** Indicates if we are working in append mode. */
        protected bool appendmode = false;
        /** The version that was or will be written to the header. */
        protected char header_version = PdfWriter.VERSION_1_4;
        /** The version that will be written to the catalog. */
        protected PdfName catalog_version = null;
        
        /**
        * @see com.lowagie.text.pdf.interfaces.PdfVersion#setPdfVersion(char)
        */
        public char PdfVersion {
            set {
                if (headerWasWritten || appendmode) {
                    SetPdfVersion(GetVersionAsName(value));
                }
                else {
                    this.header_version = value;
                }
            }
        }
        
        /**
        * @see com.lowagie.text.pdf.interfaces.PdfVersion#setAtLeastPdfVersion(char)
        */
        public void SetAtLeastPdfVersion(char version) {
            if (version > header_version) {
                PdfVersion = version;
            }
        }
        
        /**
        * @see com.lowagie.text.pdf.interfaces.PdfVersion#setPdfVersion(com.lowagie.text.pdf.PdfName)
        */
        public void SetPdfVersion(PdfName version) {
            if (catalog_version == null || catalog_version.CompareTo(version) < 0) {
                this.catalog_version = version;
            }
        }
        
        /**
        * Sets the append mode.
        */
        public void SetAppendmode(bool appendmode) {
            this.appendmode = appendmode;
        }
        
        /**
        * Writes the header to the OutputStreamCounter.
        * @throws IOException 
        */
        public void WriteHeader(OutputStreamCounter os) {
            if (appendmode) {
                os.Write(HEADER[0], 0, HEADER[0].Length);
            }
            else {
                os.Write(HEADER[1], 0, HEADER[1].Length);
                os.Write(GetVersionAsByteArray(header_version), 0, GetVersionAsByteArray(header_version).Length);
                os.Write(HEADER[2], 0, HEADER[2].Length);
                headerWasWritten = true;
            }
        }
        
        /**
        * Returns the PDF version as a name.
        * @param version    the version character.
        */
        public PdfName GetVersionAsName(char version) {
            switch (version) {
            case PdfWriter.VERSION_1_2:
                return PdfWriter.PDF_VERSION_1_2;
            case PdfWriter.VERSION_1_3:
                return PdfWriter.PDF_VERSION_1_3;
            case PdfWriter.VERSION_1_4:
                return PdfWriter.PDF_VERSION_1_4;
            case PdfWriter.VERSION_1_5:
                return PdfWriter.PDF_VERSION_1_5;
            case PdfWriter.VERSION_1_6:
                return PdfWriter.PDF_VERSION_1_6;
            case PdfWriter.VERSION_1_7:
                return PdfWriter.PDF_VERSION_1_7;
            default:
                return PdfWriter.PDF_VERSION_1_4;
            }
        }
        
        /**
        * Returns the version as a byte[].
        * @param version the version character
        */
        public byte[] GetVersionAsByteArray(char version) {
            return DocWriter.GetISOBytes(GetVersionAsName(version).ToString().Substring(1));
        }

	    /** Adds the version to the Catalog dictionary. */
	    public void AddToCatalog(PdfDictionary catalog) {
		    if(catalog_version != null) {
			    catalog.Put(PdfName.VERSION, catalog_version);
		    }
	    }
    }
}