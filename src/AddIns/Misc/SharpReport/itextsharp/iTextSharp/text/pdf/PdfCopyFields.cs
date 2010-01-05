using System;
using System.Collections;
using System.IO;
using iTextSharp.text.pdf.interfaces;
using Org.BouncyCastle.X509;
/*
 * Copyright 2004 by Paulo Soares.
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
    * Concatenates PDF documents including form fields. The rules for the form field
    * concatenation are the same as in Acrobat. All the documents are kept in memory unlike
    * PdfCopy.
    * @author  Paulo Soares (psoares@consiste.pt)
    */
    public class PdfCopyFields : IPdfViewerPreferences, IPdfEncryptionSettings {
        
        private PdfCopyFieldsImp fc;
        
        /**
        * Creates a new instance.
        * @param os the output stream
        * @throws DocumentException on error
        * @throws IOException on error
        */    
        public PdfCopyFields(Stream os) {
            fc = new PdfCopyFieldsImp(os);
        }
        
        /**
        * Creates a new instance.
        * @param os the output stream
        * @param pdfVersion the pdf version the output will have
        * @throws DocumentException on error
        * @throws IOException on error
        */    
        public PdfCopyFields(Stream os, char pdfVersion) {
            fc = new PdfCopyFieldsImp(os, pdfVersion);
        }
        
        /**
        * Concatenates a PDF document.
        * @param reader the PDF document
        * @throws DocumentException on error
        */    
        public void AddDocument(PdfReader reader) {
            fc.AddDocument(reader);
        }
        
        /**
        * Concatenates a PDF document selecting the pages to keep. The pages are described as a
        * <CODE>List</CODE> of <CODE>Integer</CODE>. The page ordering can be changed but
        * no page repetitions are allowed.
        * @param reader the PDF document
        * @param pagesToKeep the pages to keep
        * @throws DocumentException on error
        */    
        public void AddDocument(PdfReader reader, ArrayList pagesToKeep) {
            fc.AddDocument(reader, pagesToKeep);
        }

        /**
        * Concatenates a PDF document selecting the pages to keep. The pages are described as
        * ranges. The page ordering can be changed but
        * no page repetitions are allowed.
        * @param reader the PDF document
        * @param ranges the comma separated ranges as described in {@link SequenceList}
        * @throws DocumentException on error
        */    
        public void AddDocument(PdfReader reader, String ranges) {
            fc.AddDocument(reader, SequenceList.Expand(ranges, reader.NumberOfPages));
        }

        /** Sets the encryption options for this document. The userPassword and the
        *  ownerPassword can be null or have zero length. In this case the ownerPassword
        *  is replaced by a random string. The open permissions for the document can be
        *  AllowPrinting, AllowModifyContents, AllowCopy, AllowModifyAnnotations,
        *  AllowFillIn, AllowScreenReaders, AllowAssembly and AllowDegradedPrinting.
        *  The permissions can be combined by ORing them.
        * @param userPassword the user password. Can be null or empty
        * @param ownerPassword the owner password. Can be null or empty
        * @param permissions the user permissions
        * @param strength128Bits <code>true</code> for 128 bit key length, <code>false</code> for 40 bit key length
        * @throws DocumentException if the document is already open
        */
        public void SetEncryption(byte[] userPassword, byte[] ownerPassword, int permissions, bool strength128Bits) {
            fc.SetEncryption(userPassword, ownerPassword, permissions, strength128Bits ? PdfWriter.STANDARD_ENCRYPTION_128 : PdfWriter.STANDARD_ENCRYPTION_40);
        }
        
        /**
        * Sets the encryption options for this document. The userPassword and the
        *  ownerPassword can be null or have zero length. In this case the ownerPassword
        *  is replaced by a random string. The open permissions for the document can be
        *  AllowPrinting, AllowModifyContents, AllowCopy, AllowModifyAnnotations,
        *  AllowFillIn, AllowScreenReaders, AllowAssembly and AllowDegradedPrinting.
        *  The permissions can be combined by ORing them.
        * @param strength true for 128 bit key length. false for 40 bit key length
        * @param userPassword the user password. Can be null or empty
        * @param ownerPassword the owner password. Can be null or empty
        * @param permissions the user permissions
        * @throws DocumentException if the document is already open
        */
        public void SetEncryption(bool strength, String userPassword, String ownerPassword, int permissions) {
            SetEncryption(DocWriter.GetISOBytes(userPassword), DocWriter.GetISOBytes(ownerPassword), permissions, strength);
        }
     
        /**
        * Closes the output document.
        */    
        public void Close() {
            fc.Close();
        }

        /**
        * Opens the document. This is usually not needed as AddDocument() will do it
        * automatically.
        */    
        public void Open() {
            fc.OpenDoc();
        }

        /**
        * Adds JavaScript to the global document
        * @param js the JavaScript
        */    
        public void AddJavaScript(String js) {
            fc.AddJavaScript(js, !PdfEncodings.IsPdfDocEncoding(js));
        }

        /**
        * Sets the bookmarks. The list structure is defined in
        * {@link SimpleBookmark}.
        * @param outlines the bookmarks or <CODE>null</CODE> to remove any
        */    
        public ArrayList Outlines {
            set {
                fc.Outlines = value;
            }
        }
        
        /** Gets the underlying PdfWriter.
        * @return the underlying PdfWriter
        */    
        public PdfWriter Writer {
            get {
                return fc;
            }
        }

        /**
        * Gets the 1.5 compression status.
        * @return <code>true</code> if the 1.5 compression is on
        */
        public bool FullCompression {
            get {
                return fc.FullCompression;
            }
        }
        
        /**
        * Sets the document's compression to the new 1.5 mode with object streams and xref
        * streams. It can be set at any time but once set it can't be unset.
        * <p>
        * If set before opening the document it will also set the pdf version to 1.5.
        */
        public void SetFullCompression() {
            fc.SetFullCompression();
        }
    
        /**
        * @see com.lowagie.text.pdf.interfaces.PdfEncryptionSettings#setEncryption(byte[], byte[], int, int)
        */
        public void SetEncryption(byte[] userPassword, byte[] ownerPassword, int permissions, int encryptionType) {
            fc.SetEncryption(userPassword, ownerPassword, permissions, encryptionType);
        }

        /**
        * @see com.lowagie.text.pdf.interfaces.PdfViewerPreferences#addViewerPreference(com.lowagie.text.pdf.PdfName, com.lowagie.text.pdf.PdfObject)
        */
        public void AddViewerPreference(PdfName key, PdfObject value) {
            fc.AddViewerPreference(key, value); 
        }

        /**
        * @see com.lowagie.text.pdf.interfaces.PdfViewerPreferences#setViewerPreferences(int)
        */
        public int ViewerPreferences {
            set {
                fc.ViewerPreferences = value;
            }
        }

        /**
        * @see com.lowagie.text.pdf.interfaces.PdfEncryptionSettings#setEncryption(java.security.cert.Certificate[], int[], int)
        */
        public void SetEncryption(X509Certificate[] certs, int[] permissions, int encryptionType) {
            fc.SetEncryption(certs, permissions, encryptionType);
        }    
    }
}