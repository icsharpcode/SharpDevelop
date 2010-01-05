using System;
using System.Collections;
using System.Security.Cryptography;
using System.Net;
using System.Text;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf.intern;
using iTextSharp.text.pdf.interfaces;
using System.util;
using System.util.zlib;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Cms;
using Org.BouncyCastle.X509;
/*
 * $Id: PdfReader.cs,v 1.50 2008/05/13 11:25:23 psoares33 Exp $
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
    /** Reads a PDF document.
    * @author Paulo Soares (psoares@consiste.pt)
    * @author Kazuya Ujihara
    */
    public class PdfReader : IPdfViewerPreferences {
        
        static PdfName[] pageInhCandidates = {
            PdfName.MEDIABOX, PdfName.ROTATE, PdfName.RESOURCES, PdfName.CROPBOX
        };

        static byte[] endstream = PdfEncodings.ConvertToBytes("endstream", null);
        static byte[] endobj = PdfEncodings.ConvertToBytes("endobj", null);
        protected internal PRTokeniser tokens;
        // Each xref pair is a position
        // type 0 -> -1, 0
        // type 1 -> offset, 0
        // type 2 -> index, obj num
        protected internal int[] xref;
        protected internal Hashtable objStmMark;
        protected internal IntHashtable objStmToOffset;
        protected internal bool newXrefType;
        private ArrayList xrefObj;
        PdfDictionary rootPages;
        protected internal PdfDictionary trailer;
        protected internal PdfDictionary catalog;
        protected internal PageRefs pageRefs;
        protected internal PRAcroForm acroForm = null;
        protected internal bool acroFormParsed = false;
        protected internal bool encrypted = false;
        protected internal bool rebuilt = false;
        protected internal int freeXref;
        protected internal bool tampered = false;
        protected internal int lastXref;
        protected internal int eofPos;
        protected internal char pdfVersion;
        protected internal PdfEncryption decrypt;
        protected internal byte[] password = null; //added by ujihara for decryption
        protected ICipherParameters certificateKey = null; //added by Aiken Sam for certificate decryption
        protected X509Certificate certificate = null; //added by Aiken Sam for certificate decryption
        private bool ownerPasswordUsed;
        protected internal ArrayList strings = new ArrayList();
        protected internal bool sharedStreams = true;
        protected internal bool consolidateNamedDestinations = false;
        protected internal int rValue;
        protected internal int pValue;
        private int objNum;
        private int objGen;
        private int fileLength;
        private bool hybridXref;
        private int lastXrefPartial = -1;
        private bool partial;
        private PRIndirectReference cryptoRef;
        private PdfViewerPreferencesImp viewerPreferences = new PdfViewerPreferencesImp();
        private bool encryptionError;

        /**
        * Holds value of property appendable.
        */
        private bool appendable;
        
        protected internal PdfReader() {
        }
        
        /** Reads and parses a PDF document.
        * @param filename the file name of the document
        * @throws IOException on error
        */
        public PdfReader(String filename) : this(filename, null) {
        }
        
        /** Reads and parses a PDF document.
        * @param filename the file name of the document
        * @param ownerPassword the password to read the document
        * @throws IOException on error
        */    
        public PdfReader(String filename, byte[] ownerPassword) {
            password = ownerPassword;
            tokens = new PRTokeniser(filename);
            ReadPdf();
        }
        
        /** Reads and parses a PDF document.
        * @param pdfIn the byte array with the document
        * @throws IOException on error
        */
        public PdfReader(byte[] pdfIn) : this(pdfIn, null) {
        }
        
        /** Reads and parses a PDF document.
        * @param pdfIn the byte array with the document
        * @param ownerPassword the password to read the document
        * @throws IOException on error
        */
        public PdfReader(byte[] pdfIn, byte[] ownerPassword) {
            password = ownerPassword;
            tokens = new PRTokeniser(pdfIn);
            ReadPdf();
        }
        
        /** Reads and parses a PDF document.
        * @param filename the file name of the document
        * @param certificate the certificate to read the document
        * @param certificateKey the private key of the certificate
        * @param certificateKeyProvider the security provider for certificateKey
        * @throws IOException on error
        */
        public PdfReader(String filename, X509Certificate certificate, ICipherParameters certificateKey) {
            this.certificate = certificate;
            this.certificateKey = certificateKey;
            tokens = new PRTokeniser(filename);
            ReadPdf();
        }        

    /** Reads and parses a PDF document.
        * @param url the Uri of the document
        * @throws IOException on error
        */
        public PdfReader(Uri url) : this(url, null) {
        }
        
        /** Reads and parses a PDF document.
        * @param url the Uri of the document
        * @param ownerPassword the password to read the document
        * @throws IOException on error
        */
        public PdfReader(Uri url, byte[] ownerPassword) {
            password = ownerPassword;
            tokens = new PRTokeniser(new RandomAccessFileOrArray(url));
            ReadPdf();
        }
        
        /**
        * Reads and parses a PDF document.
        * @param is the <CODE>InputStream</CODE> containing the document. The stream is read to the
        * end but is not closed
        * @param ownerPassword the password to read the document
        * @throws IOException on error
        */
        public PdfReader(Stream isp, byte[] ownerPassword) {
            password = ownerPassword;
            tokens = new PRTokeniser(new RandomAccessFileOrArray(isp));
            ReadPdf();
        }
        
        /**
        * Reads and parses a PDF document.
        * @param isp the <CODE>InputStream</CODE> containing the document. The stream is read to the
        * end but is not closed
        * @throws IOException on error
        */
        public PdfReader(Stream isp) : this(isp, null) {
        }
        
        /**
        * Reads and parses a pdf document. Contrary to the other constructors only the xref is read
        * into memory. The reader is said to be working in "partial" mode as only parts of the pdf
        * are read as needed. The pdf is left open but may be closed at any time with
        * <CODE>PdfReader.Close()</CODE>, reopen is automatic.
        * @param raf the document location
        * @param ownerPassword the password or <CODE>null</CODE> for no password
        * @throws IOException on error
        */    
        public PdfReader(RandomAccessFileOrArray raf, byte[] ownerPassword) {
            password = ownerPassword;
            partial = true;
            tokens = new PRTokeniser(raf);
            ReadPdfPartial();
        }
        
        /** Creates an independent duplicate.
        * @param reader the <CODE>PdfReader</CODE> to duplicate
        */    
        public PdfReader(PdfReader reader) {
            this.appendable = reader.appendable;
            this.consolidateNamedDestinations = reader.consolidateNamedDestinations;
            this.encrypted = reader.encrypted;
            this.rebuilt = reader.rebuilt;
            this.sharedStreams = reader.sharedStreams;
            this.tampered = reader.tampered;
            this.password = reader.password;
            this.pdfVersion = reader.pdfVersion;
            this.eofPos = reader.eofPos;
            this.freeXref = reader.freeXref;
            this.lastXref = reader.lastXref;
            this.tokens = new PRTokeniser(reader.tokens.SafeFile);
            if (reader.decrypt != null)
                this.decrypt = new PdfEncryption(reader.decrypt);
            this.pValue = reader.pValue;
            this.rValue = reader.rValue;
            this.xrefObj = new ArrayList(reader.xrefObj);
            for (int k = 0; k < reader.xrefObj.Count; ++k) {
                this.xrefObj[k] = DuplicatePdfObject((PdfObject)reader.xrefObj[k], this);
            }
            this.pageRefs = new PageRefs(reader.pageRefs, this);
            this.trailer = (PdfDictionary)DuplicatePdfObject(reader.trailer, this);
            this.catalog = (PdfDictionary)GetPdfObject(trailer.Get(PdfName.ROOT));
            this.rootPages = (PdfDictionary)GetPdfObject(catalog.Get(PdfName.PAGES));
            this.fileLength = reader.fileLength;
            this.partial = reader.partial;
            this.hybridXref = reader.hybridXref;
            this.objStmToOffset = reader.objStmToOffset;
            this.xref = reader.xref;
            this.cryptoRef = (PRIndirectReference)DuplicatePdfObject(reader.cryptoRef, this);
            this.ownerPasswordUsed = reader.ownerPasswordUsed;
        }
                                                                                      
        /** Gets a new file instance of the original PDF
        * document.
        * @return a new file instance of the original PDF document
        */
        public RandomAccessFileOrArray SafeFile {
            get {
                return tokens.SafeFile;
            }
        }
        
        protected internal PdfReaderInstance GetPdfReaderInstance(PdfWriter writer) {
            return new PdfReaderInstance(this, writer);
        }
        
        /** Gets the number of pages in the document.
        * @return the number of pages in the document
        */
        public int NumberOfPages {
            get {
                return pageRefs.Size;
            }
        }
        
        /** Returns the document's catalog. This dictionary is not a copy,
        * any changes will be reflected in the catalog.
        * @return the document's catalog
        */
        public PdfDictionary Catalog {
            get {
                return catalog;
            }
        }
        
        /** Returns the document's acroform, if it has one.
        * @return the document's acroform
        */
        public PRAcroForm AcroForm {
            get {
                if (!acroFormParsed) {
                    acroFormParsed = true;
                    PdfObject form = catalog.Get(PdfName.ACROFORM);
                    if (form != null) {
                        try {
                            acroForm = new PRAcroForm(this);
                            acroForm.ReadAcroForm((PdfDictionary)GetPdfObject(form));
                        }
                        catch {
                            acroForm = null;
                        }
                    }
                }
                return acroForm;
            }
        }
        /**
        * Gets the page rotation. This value can be 0, 90, 180 or 270.
        * @param index the page number. The first page is 1
        * @return the page rotation
        */
        public int GetPageRotation(int index) {
            return GetPageRotation(pageRefs.GetPageNRelease(index));
        }
        
        internal int GetPageRotation(PdfDictionary page) {
            PdfNumber rotate = (PdfNumber)GetPdfObject(page.Get(PdfName.ROTATE));
            if (rotate == null)
                return 0;
            else {
                int n = rotate.IntValue;
                n %= 360;
                return n < 0 ? n + 360 : n;
            }
        }
        /** Gets the page size, taking rotation into account. This
        * is a <CODE>Rectangle</CODE> with the value of the /MediaBox and the /Rotate key.
        * @param index the page number. The first page is 1
        * @return a <CODE>Rectangle</CODE>
        */
        public Rectangle GetPageSizeWithRotation(int index) {
            return GetPageSizeWithRotation(pageRefs.GetPageNRelease(index));
        }
        
        /**
        * Gets the rotated page from a page dictionary.
        * @param page the page dictionary
        * @return the rotated page
        */    
        public Rectangle GetPageSizeWithRotation(PdfDictionary page) {
            Rectangle rect = GetPageSize(page);
            int rotation = GetPageRotation(page);
            while (rotation > 0) {
                rect = rect.Rotate();
                rotation -= 90;
            }
            return rect;
        }
        
        /** Gets the page size without taking rotation into account. This
        * is the value of the /MediaBox key.
        * @param index the page number. The first page is 1
        * @return the page size
        */
        public Rectangle GetPageSize(int index) {
            return GetPageSize(pageRefs.GetPageNRelease(index));
        }
        
        /**
        * Gets the page from a page dictionary
        * @param page the page dictionary
        * @return the page
        */    
        public Rectangle GetPageSize(PdfDictionary page) {
            PdfArray mediaBox = (PdfArray)GetPdfObject(page.Get(PdfName.MEDIABOX));
            return GetNormalizedRectangle(mediaBox);
        }
        
        /** Gets the crop box without taking rotation into account. This
        * is the value of the /CropBox key. The crop box is the part
        * of the document to be displayed or printed. It usually is the same
        * as the media box but may be smaller. If the page doesn't have a crop
        * box the page size will be returned.
        * @param index the page number. The first page is 1
        * @return the crop box
        */
        public Rectangle GetCropBox(int index) {
            PdfDictionary page = pageRefs.GetPageNRelease(index);
            PdfArray cropBox = (PdfArray)GetPdfObjectRelease(page.Get(PdfName.CROPBOX));
            if (cropBox == null)
                return GetPageSize(page);
            return GetNormalizedRectangle(cropBox);
        }
        
        /** Gets the box size. Allowed names are: "crop", "trim", "art", "bleed" and "media".
        * @param index the page number. The first page is 1
        * @param boxName the box name
        * @return the box rectangle or null
        */
        public Rectangle GetBoxSize(int index, String boxName) {
            PdfDictionary page = pageRefs.GetPageNRelease(index);
            PdfArray box = null;
            if (boxName.Equals("trim"))
                box = (PdfArray)GetPdfObjectRelease(page.Get(PdfName.TRIMBOX));
            else if (boxName.Equals("art"))
                box = (PdfArray)GetPdfObjectRelease(page.Get(PdfName.ARTBOX));
            else if (boxName.Equals("bleed"))
                box = (PdfArray)GetPdfObjectRelease(page.Get(PdfName.BLEEDBOX));
            else if (boxName.Equals("crop"))
                box = (PdfArray)GetPdfObjectRelease(page.Get(PdfName.CROPBOX));
            else if (boxName.Equals("media"))
                box = (PdfArray)GetPdfObjectRelease(page.Get(PdfName.MEDIABOX));
            if (box == null)
                return null;
            return GetNormalizedRectangle(box);
        }
        
        /** Returns the content of the document information dictionary as a <CODE>Hashtable</CODE>
        * of <CODE>String</CODE>.
        * @return content of the document information dictionary
        */
        public Hashtable Info {
            get {
                Hashtable map = new Hashtable();
                PdfDictionary info = (PdfDictionary)GetPdfObject(trailer.Get(PdfName.INFO));
                if (info == null)
                    return map;
                foreach (PdfName key in info.Keys) {
                    PdfObject obj = GetPdfObject(info.Get(key));
                    if (obj == null)
                        continue;
                    String value = obj.ToString();
                    switch (obj.Type) {
                        case PdfObject.STRING: {
                            value = ((PdfString)obj).ToUnicodeString();
                            break;
                        }
                        case PdfObject.NAME: {
                            value = PdfName.DecodeName(value);
                            break;
                        }
                    }
                    map[PdfName.DecodeName(key.ToString())] = value;
                }
                return map;
            }
        }
        
        /** Normalizes a <CODE>Rectangle</CODE> so that llx and lly are smaller than urx and ury.
        * @param box the original rectangle
        * @return a normalized <CODE>Rectangle</CODE>
        */    
        public static Rectangle GetNormalizedRectangle(PdfArray box) {
            ArrayList rect = box.ArrayList;
            float llx = ((PdfNumber)GetPdfObjectRelease((PdfObject)rect[0])).FloatValue;
            float lly = ((PdfNumber)GetPdfObjectRelease((PdfObject)rect[1])).FloatValue;
            float urx = ((PdfNumber)GetPdfObjectRelease((PdfObject)rect[2])).FloatValue;
            float ury = ((PdfNumber)GetPdfObjectRelease((PdfObject)rect[3])).FloatValue;
            return new Rectangle(Math.Min(llx, urx), Math.Min(lly, ury),
            Math.Max(llx, urx), Math.Max(lly, ury));
        }
        
        protected internal virtual void ReadPdf() {
            try {
                fileLength = tokens.File.Length;
                pdfVersion = tokens.CheckPdfHeader();
                try {
                    ReadXref();
                }
                catch (Exception e) {
                    try {
                        rebuilt = true;
                        RebuildXref();
                        lastXref = -1;
                    }
                    catch (Exception ne) {
                        throw new IOException("Rebuild failed: " + ne.Message + "; Original message: " + e.Message);
                    }
                }
                try {
                    ReadDocObj();
                }
                catch (Exception ne) {
                    if (rebuilt || encryptionError)
                        throw ne;
                    rebuilt = true;
                    encrypted = false;
                    RebuildXref();
                    lastXref = -1;
                    ReadDocObj();
                }
                
                strings.Clear();
                ReadPages();
                EliminateSharedStreams();
                RemoveUnusedObjects();
            }
            finally {
                try {
                    tokens.Close();
                }
                catch {
                    // empty on purpose
                }
            }
        }
        
        protected internal void ReadPdfPartial() {
            try {
                fileLength = tokens.File.Length;
                pdfVersion = tokens.CheckPdfHeader();
                try {
                    ReadXref();
                }
                catch (Exception e) {
                    try {
                        rebuilt = true;
                        RebuildXref();
                        lastXref = -1;
                    }
                    catch (Exception ne) {
                        throw new IOException("Rebuild failed: " + ne.Message + "; Original message: " + e.Message);
                    }
                }
                ReadDocObjPartial();
                ReadPages();
            }
            catch (IOException e) {
                try{tokens.Close();}catch{}
                throw e;
            }
        }
        
        private bool EqualsArray(byte[] ar1, byte[] ar2, int size) {
            for (int k = 0; k < size; ++k) {
                if (ar1[k] != ar2[k])
                    return false;
            }
            return true;
        }
        
        /**
        * @throws IOException
        */
        private void ReadDecryptedDocObj() {
            if (encrypted)
                return;
            PdfObject encDic = trailer.Get(PdfName.ENCRYPT);
            if (encDic == null || encDic.ToString().Equals("null"))
                return;
            encryptionError = true;
            byte[] encryptionKey = null;
       	
            encrypted = true;
            PdfDictionary enc = (PdfDictionary)GetPdfObject(encDic);
            
            String s;
            PdfObject o;
            
            PdfArray documentIDs = (PdfArray)GetPdfObject(trailer.Get(PdfName.ID));
            byte[] documentID = null;
            if (documentIDs != null) {
                o = (PdfObject)documentIDs.ArrayList[0];
                strings.Remove(o);
                s = o.ToString();
                documentID = DocWriter.GetISOBytes(s);
                if (documentIDs.Size > 1)
                    strings.Remove(documentIDs.ArrayList[1]);
            }
            // just in case we have a broken producer
            if (documentID == null)
                documentID = new byte[0];
            
            byte[] uValue = null;
            byte[] oValue = null;
            int cryptoMode = PdfWriter.STANDARD_ENCRYPTION_40;
            int lengthValue = 0;  
            
            PdfObject filter = GetPdfObjectRelease(enc.Get(PdfName.FILTER));

            if (filter.Equals(PdfName.STANDARD)) {                   
                s = enc.Get(PdfName.U).ToString();
                strings.Remove(enc.Get(PdfName.U));
                uValue = DocWriter.GetISOBytes(s);
                s = enc.Get(PdfName.O).ToString();
                strings.Remove(enc.Get(PdfName.O));
                oValue = DocWriter.GetISOBytes(s);
                
                o = enc.Get(PdfName.R);
                if (!o.IsNumber()) throw new IOException("Illegal R value.");
                rValue = ((PdfNumber)o).IntValue;
                if (rValue != 2 && rValue != 3 && rValue != 4) throw new IOException("Unknown encryption type (" + rValue + ")");
                
                o = enc.Get(PdfName.P);
                if (!o.IsNumber()) throw new IOException("Illegal P value.");
                pValue = ((PdfNumber)o).IntValue;
                
                if ( rValue == 3 ){
                    o = enc.Get(PdfName.LENGTH);
                    if (!o.IsNumber())
                        throw new IOException("Illegal Length value.");
                    lengthValue = ((PdfNumber)o).IntValue;
                    if (lengthValue > 128 || lengthValue < 40 || lengthValue % 8 != 0)
                        throw new IOException("Illegal Length value.");
                    cryptoMode = PdfWriter.STANDARD_ENCRYPTION_128;
                }
                else if (rValue == 4) {
                    lengthValue = 128;
                    PdfDictionary dic = (PdfDictionary)enc.Get(PdfName.CF);
                    if (dic == null)
                        throw new IOException("/CF not found (encryption)");
                    dic = (PdfDictionary)dic.Get(PdfName.STDCF);
                    if (dic == null)
                        throw new IOException("/StdCF not found (encryption)");
                    if (PdfName.V2.Equals(dic.Get(PdfName.CFM)))
                        cryptoMode = PdfWriter.STANDARD_ENCRYPTION_128;
                    else if (PdfName.AESV2.Equals(dic.Get(PdfName.CFM)))
                        cryptoMode = PdfWriter.ENCRYPTION_AES_128;
                    else
                        throw new IOException("No compatible encryption found");
                    PdfObject em = enc.Get(PdfName.ENCRYPTMETADATA);
                    if (em != null && em.ToString().Equals("false"))
                        cryptoMode |= PdfWriter.DO_NOT_ENCRYPT_METADATA;
                } else {
                    cryptoMode = PdfWriter.STANDARD_ENCRYPTION_40;
                }
            } else if (filter.Equals(PdfName.PUBSEC)) {
                bool foundRecipient = false;
                byte[] envelopedData = null;
                PdfArray recipients = null;

                o = enc.Get(PdfName.V);
                if (!o.IsNumber()) throw new IOException("Illegal V value.");
                int vValue = ((PdfNumber)o).IntValue;
                if (vValue != 1 && vValue != 2 && vValue != 4)
                    throw new IOException("Unknown encryption type V = " + rValue);

                if ( vValue == 2 ){
                    o = enc.Get(PdfName.LENGTH);
                    if (!o.IsNumber())
                        throw new IOException("Illegal Length value.");
                    lengthValue = ((PdfNumber)o).IntValue;
                    if (lengthValue > 128 || lengthValue < 40 || lengthValue % 8 != 0)
                        throw new IOException("Illegal Length value.");
                    cryptoMode = PdfWriter.STANDARD_ENCRYPTION_128;                
                    recipients = (PdfArray)enc.Get(PdfName.RECIPIENTS);                                
                } else if (vValue == 4) {
                    PdfDictionary dic = (PdfDictionary)enc.Get(PdfName.CF);
                    if (dic == null)
                        throw new IOException("/CF not found (encryption)");
                    dic = (PdfDictionary)dic.Get(PdfName.DEFAULTCRYPTFILER);
                    if (dic == null)
                        throw new IOException("/DefaultCryptFilter not found (encryption)");
                    if (PdfName.V2.Equals(dic.Get(PdfName.CFM)))
                    {
                        cryptoMode = PdfWriter.STANDARD_ENCRYPTION_128;
                        lengthValue = 128;
                    }
                    else if (PdfName.AESV2.Equals(dic.Get(PdfName.CFM)))
                    {
                        cryptoMode = PdfWriter.ENCRYPTION_AES_128;
                        lengthValue = 128;
                    }
                    else
                        throw new IOException("No compatible encryption found");
                    PdfObject em = dic.Get(PdfName.ENCRYPTMETADATA);
                    if (em != null && em.ToString().Equals("false"))
                        cryptoMode |= PdfWriter.DO_NOT_ENCRYPT_METADATA;
                    
                    recipients = (PdfArray)dic.Get(PdfName.RECIPIENTS);                                    
                } else {
                    cryptoMode = PdfWriter.STANDARD_ENCRYPTION_40;
                    lengthValue = 40;                
                    recipients = (PdfArray)enc.Get(PdfName.RECIPIENTS);                
                }

                for (int i = 0; i<recipients.Size; i++)
                {
                    PdfObject recipient = (PdfObject)recipients.ArrayList[i];
                    strings.Remove(recipient);
                    
                    CmsEnvelopedData data = null;
                    data = new CmsEnvelopedData(recipient.GetBytes());
                
                    foreach (RecipientInformation recipientInfo in data.GetRecipientInfos().GetRecipients()) {
                        if (recipientInfo.RecipientID.Match(certificate) && !foundRecipient) {
    
                        envelopedData = recipientInfo.GetContent(certificateKey);
                        foundRecipient = true;                         
                        }
                    }                        
                }
                
                if (!foundRecipient || envelopedData == null)
                {
                    throw new IOException("Bad certificate and key.");
                }            

                SHA1 sh = new SHA1CryptoServiceProvider();

                sh.TransformBlock(envelopedData, 0, 20, envelopedData, 0);
                for (int i=0; i<recipients.Size; i++)
                {
                byte[] encodedRecipient = ((PdfObject)recipients.ArrayList[i]).GetBytes();  
                sh.TransformBlock(encodedRecipient, 0, encodedRecipient.Length, encodedRecipient, 0);
                }
                if ((cryptoMode & PdfWriter.DO_NOT_ENCRYPT_METADATA) != 0)
                    sh.TransformBlock(PdfEncryption.metadataPad, 0, PdfEncryption.metadataPad.Length, PdfEncryption.metadataPad, 0);
                sh.TransformFinalBlock(envelopedData, 0, 0);        
                encryptionKey = sh.Hash;
            }
            decrypt = new PdfEncryption();
            decrypt.SetCryptoMode(cryptoMode, lengthValue);
            
            if (filter.Equals(PdfName.STANDARD))
            {
                //check by owner password
                decrypt.SetupByOwnerPassword(documentID, password, uValue, oValue, pValue);
                if (!EqualsArray(uValue, decrypt.userKey, (rValue == 3 || rValue == 4) ? 16 : 32)) {
                    //check by user password
                    decrypt.SetupByUserPassword(documentID, password, oValue, pValue);
                    if (!EqualsArray(uValue, decrypt.userKey, (rValue == 3 || rValue == 4) ? 16 : 32)) {
                        throw new BadPasswordException();
                    }
                }
                else
                    ownerPasswordUsed = true;
            } else if (filter.Equals(PdfName.PUBSEC)) {   
                decrypt.SetupByEncryptionKey(encryptionKey, lengthValue); 
                ownerPasswordUsed = true;
            }
            for (int k = 0; k < strings.Count; ++k) {
                PdfString str = (PdfString)strings[k];
                str.Decrypt(this);
            }
            if (encDic.IsIndirect()) {
                cryptoRef = (PRIndirectReference)encDic;
                xrefObj[cryptoRef.Number] = null;
            }
            encryptionError = false;
        }
        
        /**
        * @param obj
        * @return a PdfObject
        */
        public static PdfObject GetPdfObjectRelease(PdfObject obj) {
            PdfObject obj2 = GetPdfObject(obj);
            ReleaseLastXrefPartial(obj);
            return obj2;
        }

        
        /**
        * Reads a <CODE>PdfObject</CODE> resolving an indirect reference
        * if needed.
        * @param obj the <CODE>PdfObject</CODE> to read
        * @return the resolved <CODE>PdfObject</CODE>
        */    
        public static PdfObject GetPdfObject(PdfObject obj) {
            if (obj == null)
                return null;
            if (!obj.IsIndirect())
                return obj;
            PRIndirectReference refi = (PRIndirectReference)obj;
            int idx = refi.Number;
            bool appendable = refi.Reader.appendable;
            obj = refi.Reader.GetPdfObject(idx);
            if (obj == null) {
                return null;
            }
            else {
                if (appendable) {
                    switch (obj.Type) {
                        case PdfObject.NULL:
                            obj = new PdfNull();
                            break;
                        case PdfObject.BOOLEAN:
                            obj = new PdfBoolean(((PdfBoolean)obj).BooleanValue);
                            break;
                        case PdfObject.NAME:
                            obj = new PdfName(obj.GetBytes());
                            break;
                    }
                    obj.IndRef = refi;
                }
                return obj;
            }
        }
        
        /**
        * Reads a <CODE>PdfObject</CODE> resolving an indirect reference
        * if needed. If the reader was opened in partial mode the object will be released
        * to save memory.
        * @param obj the <CODE>PdfObject</CODE> to read
        * @param parent
        * @return a PdfObject
        */    
        public static PdfObject GetPdfObjectRelease(PdfObject obj, PdfObject parent) {
            PdfObject obj2 = GetPdfObject(obj, parent);
            ReleaseLastXrefPartial(obj);
            return obj2;
        }
        
        /**
        * @param obj
        * @param parent
        * @return a PdfObject
        */
        public static PdfObject GetPdfObject(PdfObject obj, PdfObject parent) {
            if (obj == null)
                return null;
            if (!obj.IsIndirect()) {
                PRIndirectReference refi = null;
                if (parent != null && (refi = parent.IndRef) != null && refi.Reader.Appendable) {
                    switch (obj.Type) {
                        case PdfObject.NULL:
                            obj = new PdfNull();
                            break;
                        case PdfObject.BOOLEAN:
                            obj = new PdfBoolean(((PdfBoolean)obj).BooleanValue);
                            break;
                        case PdfObject.NAME:
                            obj = new PdfName(obj.GetBytes());
                            break;
                    }
                    obj.IndRef = refi;
                }
                return obj;
            }
            return GetPdfObject(obj);
        }
        
        /**
        * @param idx
        * @return a PdfObject
        */
        public PdfObject GetPdfObjectRelease(int idx) {
            PdfObject obj = GetPdfObject(idx);
            ReleaseLastXrefPartial();
            return obj;
        }
        
        /**
        * @param idx
        * @return aPdfObject
        */
        public PdfObject GetPdfObject(int idx) {
            lastXrefPartial = -1;
            if (idx < 0 || idx >= xrefObj.Count)
                return null;
            PdfObject obj = (PdfObject)xrefObj[idx];
            if (!partial || obj != null)
                return obj;
            if (idx * 2 >= xref.Length)
                return null;
            obj = ReadSingleObject(idx);
            lastXrefPartial = -1;
            if (obj != null)
                lastXrefPartial = idx;
            return obj;
        }

        /**
        * 
        */
        public void ResetLastXrefPartial() {
            lastXrefPartial = -1;
        }
        
        /**
        * 
        */
        public void ReleaseLastXrefPartial() {
            if (partial && lastXrefPartial != -1) {
                xrefObj[lastXrefPartial] = null;
                lastXrefPartial = -1;
            }
        }

        /**
        * @param obj
        */
        public static void ReleaseLastXrefPartial(PdfObject obj) {
            if (obj == null)
                return;
            if (!obj.IsIndirect())
                return;
            PRIndirectReference refi = (PRIndirectReference)obj;
            PdfReader reader = refi.Reader;
            if (reader.partial && reader.lastXrefPartial != -1 && reader.lastXrefPartial == refi.Number) {
                reader.xrefObj[reader.lastXrefPartial] = null;
            }
            reader.lastXrefPartial = -1;
        }

        private void SetXrefPartialObject(int idx, PdfObject obj) {
            if (!partial || idx < 0)
                return;
            xrefObj[idx] = obj;
        }
        
        /**
        * @param obj
        * @return an indirect reference
        */
        public PRIndirectReference AddPdfObject(PdfObject obj) {
            xrefObj.Add(obj);
            return new PRIndirectReference(this, xrefObj.Count - 1);
        }
        
        protected internal void ReadPages() {
            catalog = (PdfDictionary)GetPdfObject(trailer.Get(PdfName.ROOT));
            rootPages = (PdfDictionary)GetPdfObject(catalog.Get(PdfName.PAGES));
            pageRefs = new PageRefs(this);
        }
        
        protected internal void ReadDocObjPartial() {
            xrefObj = ArrayList.Repeat(null, xref.Length / 2);
            ReadDecryptedDocObj();
            if (objStmToOffset != null) {
                int[] keys = objStmToOffset.GetKeys();
                for (int k = 0; k < keys.Length; ++k) {
                    int n = keys[k];
                    objStmToOffset[n] = xref[n * 2];
                    xref[n * 2] = -1;
                }
            }
        }

        protected internal PdfObject ReadSingleObject(int k) {
            strings.Clear();
            int k2 = k * 2;
            int pos = xref[k2];
            if (pos < 0)
                return null;
            if (xref[k2 + 1] > 0)
                pos = objStmToOffset[xref[k2 + 1]];
            if (pos == 0)
                return null;
            tokens.Seek(pos);
            tokens.NextValidToken();
            if (tokens.TokenType != PRTokeniser.TK_NUMBER)
                tokens.ThrowError("Invalid object number.");
            objNum = tokens.IntValue;
            tokens.NextValidToken();
            if (tokens.TokenType != PRTokeniser.TK_NUMBER)
                tokens.ThrowError("Invalid generation number.");
            objGen = tokens.IntValue;
            tokens.NextValidToken();
            if (!tokens.StringValue.Equals("obj"))
                tokens.ThrowError("Token 'obj' expected.");
            PdfObject obj;
            try {
                obj = ReadPRObject();
                for (int j = 0; j < strings.Count; ++j) {
                    PdfString str = (PdfString)strings[j];
                    str.Decrypt(this);
                }
                if (obj.IsStream()) {
                    CheckPRStreamLength((PRStream)obj);
                }
            }
            catch {
                obj = null;
            }
            if (xref[k2 + 1] > 0) {
                obj = ReadOneObjStm((PRStream)obj, xref[k2]);
            }
            xrefObj[k] = obj;
            return obj;
        }
        
        protected internal PdfObject ReadOneObjStm(PRStream stream, int idx) {
            int first = ((PdfNumber)GetPdfObject(stream.Get(PdfName.FIRST))).IntValue;
            byte[] b = GetStreamBytes(stream, tokens.File);
            PRTokeniser saveTokens = tokens;
            tokens = new PRTokeniser(b);
            try {
                int address = 0;
                bool ok = true;
                ++idx;
                for (int k = 0; k < idx; ++k) {
                    ok = tokens.NextToken();
                    if (!ok)
                        break;
                    if (tokens.TokenType != PRTokeniser.TK_NUMBER) {
                        ok = false;
                        break;
                    }
                    ok = tokens.NextToken();
                    if (!ok)
                        break;
                    if (tokens.TokenType != PRTokeniser.TK_NUMBER) {
                        ok = false;
                        break;
                    }
                    address = tokens.IntValue + first;
                }
                if (!ok)
                    throw new IOException("Error reading ObjStm");
                tokens.Seek(address);
                return ReadPRObject();
            }
            finally {
                tokens = saveTokens;
            }
        }

        /**
        * @return the percentage of the cross reference table that has been read
        */
        public double DumpPerc() {
            int total = 0;
            for (int k = 0; k < xrefObj.Count; ++k) {
                if (xrefObj[k] != null)
                    ++total;
            }
            return (total * 100.0 / xrefObj.Count);
        }
        
        protected internal void ReadDocObj() {
            ArrayList streams = new ArrayList();
            xrefObj = ArrayList.Repeat(null, xref.Length / 2);
            for (int k = 2; k < xref.Length; k += 2) {
                int pos = xref[k];
                if (pos <= 0 || xref[k + 1] > 0)
                    continue;
                tokens.Seek(pos);
                tokens.NextValidToken();
                if (tokens.TokenType != PRTokeniser.TK_NUMBER)
                    tokens.ThrowError("Invalid object number.");
                objNum = tokens.IntValue;
                tokens.NextValidToken();
                if (tokens.TokenType != PRTokeniser.TK_NUMBER)
                    tokens.ThrowError("Invalid generation number.");
                objGen = tokens.IntValue;
                tokens.NextValidToken();
                if (!tokens.StringValue.Equals("obj"))
                    tokens.ThrowError("Token 'obj' expected.");
                PdfObject obj;
                try {
                    obj = ReadPRObject();
                    if (obj.IsStream()) {
                        streams.Add(obj);
                    }
                }
                catch {
                    obj = null;
                }
                xrefObj[k / 2] = obj;
            }
            for (int k = 0; k < streams.Count; ++k) {
                CheckPRStreamLength((PRStream)streams[k]);
            }
            ReadDecryptedDocObj();
            if (objStmMark != null) {
                foreach (DictionaryEntry entry  in objStmMark) {
                    int n = (int)entry.Key;
                    IntHashtable h = (IntHashtable)entry.Value;
                    ReadObjStm((PRStream)xrefObj[n], h);
                    xrefObj[n] = null;
                }
                objStmMark = null;
            }
            xref = null;
        }
        
        private void CheckPRStreamLength(PRStream stream) {
            int fileLength = tokens.Length;
            int start = stream.Offset;
            bool calc = false;
            int streamLength = 0;
            PdfObject obj = GetPdfObjectRelease(stream.Get(PdfName.LENGTH));
            if (obj != null && obj.Type == PdfObject.NUMBER) {
                streamLength = ((PdfNumber)obj).IntValue;
                if (streamLength + start > fileLength - 20)
                    calc = true;
                else {
                    tokens.Seek(start + streamLength);
                    String line = tokens.ReadString(20);
                    if (!line.StartsWith("\nendstream") &&
                    !line.StartsWith("\r\nendstream") &&
                    !line.StartsWith("\rendstream") &&
                    !line.StartsWith("endstream"))
                        calc = true;
                }
            }
            else
                calc = true;
            if (calc) {
                byte[] tline = new byte[16];
                tokens.Seek(start);
                while (true) {
                    int pos = tokens.FilePointer;
                    if (!tokens.ReadLineSegment(tline))
                        break;
                    if (Equalsn(tline, endstream)) {
                        streamLength = pos - start;
                        break;
                    }
                    if (Equalsn(tline, endobj)) {
                        tokens.Seek(pos - 16);
                        String s = tokens.ReadString(16);
                        int index = s.IndexOf("endstream");
                        if (index >= 0)
                            pos = pos - 16 + index;
                        streamLength = pos - start;
                        break;
                    }
                }
            }
            stream.Length = streamLength;
        }
        
        protected internal void ReadObjStm(PRStream stream, IntHashtable map) {
            int first = ((PdfNumber)GetPdfObject(stream.Get(PdfName.FIRST))).IntValue;
            int n = ((PdfNumber)GetPdfObject(stream.Get(PdfName.N))).IntValue;
            byte[] b = GetStreamBytes(stream, tokens.File);
            PRTokeniser saveTokens = tokens;
            tokens = new PRTokeniser(b);
            try {
                int[] address = new int[n];
                int[] objNumber = new int[n];
                bool ok = true;
                for (int k = 0; k < n; ++k) {
                    ok = tokens.NextToken();
                    if (!ok)
                        break;
                    if (tokens.TokenType != PRTokeniser.TK_NUMBER) {
                        ok = false;
                        break;
                    }
                    objNumber[k] = tokens.IntValue;
                    ok = tokens.NextToken();
                    if (!ok)
                        break;
                    if (tokens.TokenType != PRTokeniser.TK_NUMBER) {
                        ok = false;
                        break;
                    }
                    address[k] = tokens.IntValue + first;
                }
                if (!ok)
                    throw new IOException("Error reading ObjStm");
                for (int k = 0; k < n; ++k) {
                    if (map.ContainsKey(k)) {
                        tokens.Seek(address[k]);
                        PdfObject obj = ReadPRObject();
                        xrefObj[objNumber[k]] = obj;
                    }
                }            
            }
            finally {
                tokens = saveTokens;
            }
        }
        
        /**
        * Eliminates the reference to the object freeing the memory used by it and clearing
        * the xref entry.
        * @param obj the object. If it's an indirect reference it will be eliminated
        * @return the object or the already erased dereferenced object
        */    
        public static PdfObject KillIndirect(PdfObject obj) {
            if (obj == null || obj.IsNull())
                return null;
            PdfObject ret = GetPdfObjectRelease(obj);
            if (obj.IsIndirect()) {
                PRIndirectReference refi = (PRIndirectReference)obj;
                PdfReader reader = refi.Reader;
                int n = refi.Number;
                reader.xrefObj[n] = null;
                if (reader.partial)
                    reader.xref[n * 2] = -1;
            }
            return ret;
        }
        
        private void EnsureXrefSize(int size) {
            if (size == 0)
                return;
            if (xref == null)
                xref = new int[size];
            else {
                if (xref.Length < size) {
                    int[] xref2 = new int[size];
                    Array.Copy(xref, 0, xref2, 0, xref.Length);
                    xref = xref2;
                }
            }
        }
        
        protected internal void ReadXref() {
            hybridXref = false;
            newXrefType = false;
            tokens.Seek(tokens.Startxref);
            tokens.NextToken();
            if (!tokens.StringValue.Equals("startxref"))
                throw new IOException("startxref not found.");
            tokens.NextToken();
            if (tokens.TokenType != PRTokeniser.TK_NUMBER)
                throw new IOException("startxref is not followed by a number.");
            int startxref = tokens.IntValue;
            lastXref = startxref;
            eofPos = tokens.FilePointer;
            try {
                if (ReadXRefStream(startxref)) {
                    newXrefType = true;
                    return;
                }
            }
            catch {}
            xref = null;
            tokens.Seek(startxref);
            trailer = ReadXrefSection();
            PdfDictionary trailer2 = trailer;
            while (true) {
                PdfNumber prev = (PdfNumber)trailer2.Get(PdfName.PREV);
                if (prev == null)
                    break;
                tokens.Seek(prev.IntValue);
                trailer2 = ReadXrefSection();
            }
        }
        
        protected internal PdfDictionary ReadXrefSection() {
            tokens.NextValidToken();
            if (!tokens.StringValue.Equals("xref"))
                tokens.ThrowError("xref subsection not found");
            int start = 0;
            int end = 0;
            int pos = 0;
            int gen = 0;
            while (true) {
                tokens.NextValidToken();
                if (tokens.StringValue.Equals("trailer"))
                    break;
                if (tokens.TokenType != PRTokeniser.TK_NUMBER)
                    tokens.ThrowError("Object number of the first object in this xref subsection not found");
                start = tokens.IntValue;
                tokens.NextValidToken();
                if (tokens.TokenType != PRTokeniser.TK_NUMBER)
                    tokens.ThrowError("Number of entries in this xref subsection not found");
                end = tokens.IntValue + start;
                if (start == 1) { // fix incorrect start number
                    int back = tokens.FilePointer;
                    tokens.NextValidToken();
                    pos = tokens.IntValue;
                    tokens.NextValidToken();
                    gen = tokens.IntValue;
                    if (pos == 0 && gen == 65535) {
                        --start;
                        --end;
                    }
                    tokens.Seek(back);
                }
                EnsureXrefSize(end * 2);
                for (int k = start; k < end; ++k) {
                    tokens.NextValidToken();
                    pos = tokens.IntValue;
                    tokens.NextValidToken();
                    gen = tokens.IntValue;
                    tokens.NextValidToken();
                    int p = k * 2;
                    if (tokens.StringValue.Equals("n")) {
                        if (xref[p] == 0 && xref[p + 1] == 0) {
    //                        if (pos == 0)
    //                            tokens.ThrowError("File position 0 cross-reference entry in this xref subsection");
                            xref[p] = pos;
                        }
                    }
                    else if (tokens.StringValue.Equals("f")) {
                        if (xref[p] == 0 && xref[p + 1] == 0)
                            xref[p] = -1;
                    }
                    else
                        tokens.ThrowError("Invalid cross-reference entry in this xref subsection");
                }
            }
            PdfDictionary trailer = (PdfDictionary)ReadPRObject();
            PdfNumber xrefSize = (PdfNumber)trailer.Get(PdfName.SIZE);
            EnsureXrefSize(xrefSize.IntValue * 2);
            PdfObject xrs = trailer.Get(PdfName.XREFSTM);
            if (xrs != null && xrs.IsNumber()) {
                int loc = ((PdfNumber)xrs).IntValue;
                try {
                    ReadXRefStream(loc);
                    newXrefType = true;
                    hybridXref = true;
                }
                catch (IOException e) {
                    xref = null;
                    throw e;
                }
            }
            return trailer;
        }
        
        protected internal bool ReadXRefStream(int ptr) {
            tokens.Seek(ptr);
            int thisStream = 0;
            if (!tokens.NextToken())
                return false;
            if (tokens.TokenType != PRTokeniser.TK_NUMBER)
                return false;
            thisStream = tokens.IntValue;
            if (!tokens.NextToken() || tokens.TokenType != PRTokeniser.TK_NUMBER)
                return false;
            if (!tokens.NextToken() || !tokens.StringValue.Equals("obj"))
                return false;
            PdfObject objecto = ReadPRObject();
            PRStream stm = null;
            if (objecto.IsStream()) {
                stm = (PRStream)objecto;
                if (!PdfName.XREF.Equals(stm.Get(PdfName.TYPE)))
                    return false;
            }
            else
                return false;
            if (trailer == null) {
                trailer = new PdfDictionary();
                trailer.Merge(stm);
            }
            stm.Length = ((PdfNumber)stm.Get(PdfName.LENGTH)).IntValue;
            int size = ((PdfNumber)stm.Get(PdfName.SIZE)).IntValue;
            PdfArray index;
            PdfObject obj = stm.Get(PdfName.INDEX);
            if (obj == null) {
                index = new PdfArray();
                index.Add(new int[]{0, size});
            }
            else
                index = (PdfArray)obj;
            PdfArray w = (PdfArray)stm.Get(PdfName.W);
            int prev = -1;
            obj = stm.Get(PdfName.PREV);
            if (obj != null)
                prev = ((PdfNumber)obj).IntValue;
            // Each xref pair is a position
            // type 0 -> -1, 0
            // type 1 -> offset, 0
            // type 2 -> index, obj num
            EnsureXrefSize(size * 2);
            if (objStmMark == null && !partial)
                objStmMark = new Hashtable();
            if (objStmToOffset == null && partial)
                objStmToOffset = new IntHashtable();
            byte[] b = GetStreamBytes(stm, tokens.File);
            int bptr = 0;
            ArrayList wa = w.ArrayList;
            int[] wc = new int[3];
            for (int k = 0; k < 3; ++k)
                wc[k] = ((PdfNumber)wa[k]).IntValue;
            ArrayList sections = index.ArrayList;
            for (int idx = 0; idx < sections.Count; idx += 2) {
                int start = ((PdfNumber)sections[idx]).IntValue;
                int length = ((PdfNumber)sections[idx + 1]).IntValue;
                EnsureXrefSize((start + length) * 2);
                while (length-- > 0) {
                    int type = 1;
                    if (wc[0] > 0) {
                        type = 0;
                        for (int k = 0; k < wc[0]; ++k)
                            type = (type << 8) + (b[bptr++] & 0xff);
                    }
                    int field2 = 0;
                    for (int k = 0; k < wc[1]; ++k)
                        field2 = (field2 << 8) + (b[bptr++] & 0xff);
                    int field3 = 0;
                    for (int k = 0; k < wc[2]; ++k)
                        field3 = (field3 << 8) + (b[bptr++] & 0xff);
                    int baseb = start * 2;
                    if (xref[baseb] == 0 && xref[baseb + 1] == 0) {
                        switch (type) {
                            case 0:
                                xref[baseb] = -1;
                                break;
                            case 1:
                                xref[baseb] = field2;
                                break;
                            case 2:
                                xref[baseb] = field3;
                                xref[baseb + 1] = field2;
                                if (partial) {
                                    objStmToOffset[field2] = 0;
                                }
                                else {
                                    IntHashtable seq = (IntHashtable)objStmMark[field2];
                                    if (seq == null) {
                                        seq = new IntHashtable();
                                        seq[field3] = 1;
                                        objStmMark[field2] = seq;
                                    }
                                    else
                                        seq[field3] = 1;
                                }
                                break;
                        }
                    }
                    ++start;
                }
            }
            thisStream *= 2;
            if (thisStream < xref.Length)
                xref[thisStream] = -1;
                
            if (prev == -1)
                return true;
            return ReadXRefStream(prev);
        }
        
        protected internal void RebuildXref() {
            hybridXref = false;
            newXrefType = false;
            tokens.Seek(0);
            int[][] xr = new int[1024][];
            int top = 0;
            trailer = null;
            byte[] line = new byte[64];
            for (;;) {
                int pos = tokens.FilePointer;
                if (!tokens.ReadLineSegment(line))
                    break;
                if (line[0] == 't') {
                    if (!PdfEncodings.ConvertToString(line, null).StartsWith("trailer"))
                        continue;
                    tokens.Seek(pos);
                    tokens.NextToken();
                    pos = tokens.FilePointer;
                    try {
                        PdfDictionary dic = (PdfDictionary)ReadPRObject();
                        if (dic.Get(PdfName.ROOT) != null)
                            trailer = dic;
                        else
                            tokens.Seek(pos);
                    }
                    catch {
                        tokens.Seek(pos);
                    }
                }
                else if (line[0] >= '0' && line[0] <= '9') {
                    int[] obj = PRTokeniser.CheckObjectStart(line);
                    if (obj == null)
                        continue;
                    int num = obj[0];
                    int gen = obj[1];
                    if (num >= xr.Length) {
                        int newLength = num * 2;
                        int[][] xr2 = new int[newLength][];
                        Array.Copy(xr, 0, xr2, 0, top);
                        xr = xr2;
                    }
                    if (num >= top)
                        top = num + 1;
                    if (xr[num] == null || gen >= xr[num][1]) {
                        obj[0] = pos;
                        xr[num] = obj;
                    }
                }
            }
            if (trailer == null)
                throw new IOException("trailer not found.");
            xref = new int[top * 2];
            for (int k = 0; k < top; ++k) {
                int[] obj = xr[k];
                if (obj != null)
                    xref[k * 2] = obj[0];
            }
        }
        
        protected internal PdfDictionary ReadDictionary() {
            PdfDictionary dic = new PdfDictionary();
            while (true) {
                tokens.NextValidToken();
                if (tokens.TokenType == PRTokeniser.TK_END_DIC)
                    break;
                if (tokens.TokenType != PRTokeniser.TK_NAME)
                    tokens.ThrowError("Dictionary key is not a name.");
                PdfName name = new PdfName(tokens.StringValue, false);
                PdfObject obj = ReadPRObject();
                int type = obj.Type;
                if (-type == PRTokeniser.TK_END_DIC)
                    tokens.ThrowError("Unexpected '>>'");
                if (-type == PRTokeniser.TK_END_ARRAY)
                    tokens.ThrowError("Unexpected ']'");
                dic.Put(name, obj);
            }
            return dic;
        }
        
        protected internal PdfArray ReadArray() {
            PdfArray array = new PdfArray();
            while (true) {
                PdfObject obj = ReadPRObject();
                int type = obj.Type;
                if (-type == PRTokeniser.TK_END_ARRAY)
                    break;
                if (-type == PRTokeniser.TK_END_DIC)
                    tokens.ThrowError("Unexpected '>>'");
                array.Add(obj);
            }
            return array;
        }
        
        protected internal PdfObject ReadPRObject() {
            tokens.NextValidToken();
            int type = tokens.TokenType;
            switch (type) {
                case PRTokeniser.TK_START_DIC: {
                    PdfDictionary dic = ReadDictionary();
                    int pos = tokens.FilePointer;
                    // be careful in the trailer. May not be a "next" token.
                    if (tokens.NextToken() && tokens.StringValue.Equals("stream")) {
                        int ch = tokens.Read();
                        if (ch != '\n')
                            ch = tokens.Read();
                        if (ch != '\n')
                            tokens.BackOnePosition(ch);
                        PRStream stream = new PRStream(this, tokens.FilePointer);
                        stream.Merge(dic);
                        stream.ObjNum = objNum;
                        stream.ObjGen = objGen;
                        return stream;
                    }
                    else {
                        tokens.Seek(pos);
                        return dic;
                    }
                }
                case PRTokeniser.TK_START_ARRAY:
                    return ReadArray();
                case PRTokeniser.TK_NUMBER:
                    return new PdfNumber(tokens.StringValue);
                case PRTokeniser.TK_STRING:
                    PdfString str = new PdfString(tokens.StringValue, null).SetHexWriting(tokens.IsHexString());
                    str.SetObjNum(objNum, objGen);
                    if (strings != null)
                        strings.Add(str);
                    return str;
                case PRTokeniser.TK_NAME:
                    return new PdfName(tokens.StringValue, false);
                case PRTokeniser.TK_REF:
                    int num = tokens.Reference;
                    PRIndirectReference refi = new PRIndirectReference(this, num, tokens.Generation);
                    return refi;
                default:
                    String sv = tokens.StringValue;
                    if ("null".Equals(sv))
                        return PdfNull.PDFNULL;
                    else if ("true".Equals(sv))
                        return PdfBoolean.PDFTRUE;
                    else if ("false".Equals(sv))
                        return PdfBoolean.PDFFALSE;
                    return new PdfLiteral(-type, tokens.StringValue);
            }
        }
        
        /** Decodes a stream that has the FlateDecode filter.
        * @param in the input data
        * @return the decoded data
        */    
        public static byte[] FlateDecode(byte[] inp) {
            byte[] b = FlateDecode(inp, true);
            if (b == null)
                return FlateDecode(inp, false);
            return b;
        }
        
        /**
        * @param in
        * @param dicPar
        * @return a byte array
        */
        public static byte[] DecodePredictor(byte[] inp, PdfObject dicPar) {
            if (dicPar == null || !dicPar.IsDictionary())
                return inp;
            PdfDictionary dic = (PdfDictionary)dicPar;
            PdfObject obj = GetPdfObject(dic.Get(PdfName.PREDICTOR));
            if (obj == null || !obj.IsNumber())
                return inp;
            int predictor = ((PdfNumber)obj).IntValue;
            if (predictor < 10)
                return inp;
            int width = 1;
            obj = GetPdfObject(dic.Get(PdfName.COLUMNS));
            if (obj != null && obj.IsNumber())
                width = ((PdfNumber)obj).IntValue;
            int colors = 1;
            obj = GetPdfObject(dic.Get(PdfName.COLORS));
            if (obj != null && obj.IsNumber())
                colors = ((PdfNumber)obj).IntValue;
            int bpc = 8;
            obj = GetPdfObject(dic.Get(PdfName.BITSPERCOMPONENT));
            if (obj != null && obj.IsNumber())
                bpc = ((PdfNumber)obj).IntValue;
            MemoryStream dataStream = new MemoryStream(inp);
            MemoryStream fout = new MemoryStream(inp.Length);
            int bytesPerPixel = colors * bpc / 8;
            int bytesPerRow = (colors*width*bpc + 7)/8;
            byte[] curr = new byte[bytesPerRow];
            byte[] prior = new byte[bytesPerRow];
            
            // Decode the (sub)image row-by-row
            while (true) {
                // Read the filter type byte and a row of data
                int filter = 0;
                try {
                    filter = dataStream.ReadByte();
                    if (filter < 0) {
                        return fout.ToArray();
                    }
                    int tot = 0;
                    while (tot < bytesPerRow) {
                        int n = dataStream.Read(curr, tot, bytesPerRow - tot);
                        if (n <= 0)
                            return fout.ToArray();
                        tot += n;
                    }
                } catch {
                    return fout.ToArray();
                }
                
                switch (filter) {
                    case 0: //PNG_FILTER_NONE
                        break;
                    case 1: //PNG_FILTER_SUB
                        for (int i = bytesPerPixel; i < bytesPerRow; i++) {
                            curr[i] += curr[i - bytesPerPixel];
                        }
                        break;
                    case 2: //PNG_FILTER_UP
                        for (int i = 0; i < bytesPerRow; i++) {
                            curr[i] += prior[i];
                        }
                        break;
                    case 3: //PNG_FILTER_AVERAGE
                        for (int i = 0; i < bytesPerPixel; i++) {
                            curr[i] += (byte)(prior[i] / 2);
                        }
                        for (int i = bytesPerPixel; i < bytesPerRow; i++) {
                            curr[i] += (byte)(((curr[i - bytesPerPixel] & 0xff) + (prior[i] & 0xff))/2);
                        }
                        break;
                    case 4: //PNG_FILTER_PAETH
                        for (int i = 0; i < bytesPerPixel; i++) {
                            curr[i] += prior[i];
                        }

                        for (int i = bytesPerPixel; i < bytesPerRow; i++) {
                            int a = curr[i - bytesPerPixel] & 0xff;
                            int b = prior[i] & 0xff;
                            int c = prior[i - bytesPerPixel] & 0xff;

                            int p = a + b - c;
                            int pa = Math.Abs(p - a);
                            int pb = Math.Abs(p - b);
                            int pc = Math.Abs(p - c);

                            int ret;

                            if ((pa <= pb) && (pa <= pc)) {
                                ret = a;
                            } else if (pb <= pc) {
                                ret = b;
                            } else {
                                ret = c;
                            }
                            curr[i] += (byte)(ret);
                        }
                        break;
                    default:
                        // Error -- uknown filter type
                        throw new Exception("PNG filter unknown.");
                }
                fout.Write(curr, 0, curr.Length);
                
                // Swap curr and prior
                byte[] tmp = prior;
                prior = curr;
                curr = tmp;
            }        
        }
        
        /** A helper to FlateDecode.
        * @param in the input data
        * @param strict <CODE>true</CODE> to read a correct stream. <CODE>false</CODE>
        * to try to read a corrupted stream
        * @return the decoded data
        */    
        public static byte[] FlateDecode(byte[] inp, bool strict) {
            MemoryStream stream = new MemoryStream(inp);
            ZInflaterInputStream zip = new ZInflaterInputStream(stream);
            MemoryStream outp = new MemoryStream();
            byte[] b = new byte[strict ? 4092 : 1];
            try {
                int n;
                while ((n = zip.Read(b, 0, b.Length)) > 0) {
                    outp.Write(b, 0, n);
                }
                zip.Close();
                outp.Close();
                return outp.ToArray();
            }
            catch {
                if (strict)
                    return null;
                return outp.ToArray();
            }
        }
        
        /** Decodes a stream that has the ASCIIHexDecode filter.
        * @param in the input data
        * @return the decoded data
        */    
        public static byte[] ASCIIHexDecode(byte[] inp) {
            MemoryStream outp = new MemoryStream();
            bool first = true;
            int n1 = 0;
            for (int k = 0; k < inp.Length; ++k) {
                int ch = inp[k] & 0xff;
                if (ch == '>')
                    break;
                if (PRTokeniser.IsWhitespace(ch))
                    continue;
                int n = PRTokeniser.GetHex(ch);
                if (n == -1)
                    throw new ArgumentException("Illegal character in ASCIIHexDecode.");
                if (first)
                    n1 = n;
                else
                    outp.WriteByte((byte)((n1 << 4) + n));
                first = !first;
            }
            if (!first)
                outp.WriteByte((byte)(n1 << 4));
            return outp.ToArray();
        }
        
        /** Decodes a stream that has the ASCII85Decode filter.
        * @param in the input data
        * @return the decoded data
        */    
        public static byte[] ASCII85Decode(byte[] inp) {
            MemoryStream outp = new MemoryStream();
            int state = 0;
            int[] chn = new int[5];
            for (int k = 0; k < inp.Length; ++k) {
                int ch = inp[k] & 0xff;
                if (ch == '~')
                    break;
                if (PRTokeniser.IsWhitespace(ch))
                    continue;
                if (ch == 'z' && state == 0) {
                    outp.WriteByte(0);
                    outp.WriteByte(0);
                    outp.WriteByte(0);
                    outp.WriteByte(0);
                    continue;
                }
                if (ch < '!' || ch > 'u')
                    throw new ArgumentException("Illegal character in ASCII85Decode.");
                chn[state] = ch - '!';
                ++state;
                if (state == 5) {
                    state = 0;
                    int rx = 0;
                    for (int j = 0; j < 5; ++j)
                        rx = rx * 85 + chn[j];
                    outp.WriteByte((byte)(rx >> 24));
                    outp.WriteByte((byte)(rx >> 16));
                    outp.WriteByte((byte)(rx >> 8));
                    outp.WriteByte((byte)rx);
                }
            }
            int r = 0;
            // We'll ignore the next two lines for the sake of perpetuating broken PDFs
//            if (state == 1)
//                throw new ArgumentException("Illegal length in ASCII85Decode.");
            if (state == 2) {
                r = chn[0] * 85 * 85 * 85 * 85 + chn[1] * 85 * 85 * 85 + 85 * 85 * 85  + 85 * 85 + 85;
                outp.WriteByte((byte)(r >> 24));
            }
            else if (state == 3) {
                r = chn[0] * 85 * 85 * 85 * 85 + chn[1] * 85 * 85 * 85  + chn[2] * 85 * 85 + 85 * 85 + 85;
                outp.WriteByte((byte)(r >> 24));
                outp.WriteByte((byte)(r >> 16));
            }
            else if (state == 4) {
                r = chn[0] * 85 * 85 * 85 * 85 + chn[1] * 85 * 85 * 85  + chn[2] * 85 * 85  + chn[3] * 85 + 85;
                outp.WriteByte((byte)(r >> 24));
                outp.WriteByte((byte)(r >> 16));
                outp.WriteByte((byte)(r >> 8));
            }
            return outp.ToArray();
        }
        
        /** Decodes a stream that has the LZWDecode filter.
        * @param in the input data
        * @return the decoded data
        */    
        public static byte[] LZWDecode(byte[] inp) {
            MemoryStream outp = new MemoryStream();
            LZWDecoder lzw = new LZWDecoder();
            lzw.Decode(inp, outp);
            return outp.ToArray();
        }
        
        /** Checks if the document had errors and was rebuilt.
        * @return true if rebuilt.
        *
        */
        public bool IsRebuilt() {
            return this.rebuilt;
        }
        
        /** Gets the dictionary that represents a page.
        * @param pageNum the page number. 1 is the first
        * @return the page dictionary
        */    
        public PdfDictionary GetPageN(int pageNum) {
            PdfDictionary dic = pageRefs.GetPageN(pageNum);
            if (dic == null)
                return null;
            if (appendable)
                dic.IndRef = pageRefs.GetPageOrigRef(pageNum);
            return dic;
        }
        
        /**
        * @param pageNum
        * @return a Dictionary object
        */
        public PdfDictionary GetPageNRelease(int pageNum) {
            PdfDictionary dic = GetPageN(pageNum);
            pageRefs.ReleasePage(pageNum);
            return dic;
        }
        
        /**
        * @param pageNum
        */
        public void ReleasePage(int pageNum) {
            pageRefs.ReleasePage(pageNum);
        }
        
        /**
        * 
        */
        public void ResetReleasePage() {
            pageRefs.ResetReleasePage();
        }

        /** Gets the page reference to this page.
        * @param pageNum the page number. 1 is the first
        * @return the page reference
        */    
        public PRIndirectReference GetPageOrigRef(int pageNum) {
            return pageRefs.GetPageOrigRef(pageNum);
        }
        
        /** Gets the contents of the page.
        * @param pageNum the page number. 1 is the first
        * @param file the location of the PDF document
        * @throws IOException on error
        * @return the content
        */    
        public byte[] GetPageContent(int pageNum, RandomAccessFileOrArray file) {
            PdfDictionary page = GetPageNRelease(pageNum);
            if (page == null)
                return null;
            PdfObject contents = GetPdfObjectRelease(page.Get(PdfName.CONTENTS));
            if (contents == null)
                return new byte[0];
            MemoryStream bout = null;
            if (contents.IsStream()) {
                return GetStreamBytes((PRStream)contents, file);
            }
            else if (contents.IsArray()) {
                PdfArray array = (PdfArray)contents;
                ArrayList list = array.ArrayList;
                bout = new MemoryStream();
                for (int k = 0; k < list.Count; ++k) {
                    PdfObject item = GetPdfObjectRelease((PdfObject)list[k]);
                    if (item == null || !item.IsStream())
                        continue;
                    byte[] b = GetStreamBytes((PRStream)item, file);
                    bout.Write(b, 0, b.Length);
                    if (k != list.Count - 1)
                        bout.WriteByte((byte)'\n');
                }
                return bout.ToArray();
            }
            else
                return new byte[0];
        }
        
        /** Gets the contents of the page.
        * @param pageNum the page number. 1 is the first
        * @throws IOException on error
        * @return the content
        */    
        public byte[] GetPageContent(int pageNum) {
            RandomAccessFileOrArray rf = SafeFile;
            try {
                rf.ReOpen();
                return GetPageContent(pageNum, rf);
            }
            finally {
                try{rf.Close();}catch{}
            }
        }
        
        protected internal void KillXref(PdfObject obj) {
            if (obj == null)
                return;
            if ((obj is PdfIndirectReference) && !obj.IsIndirect())
                return;
            switch (obj.Type) {
                case PdfObject.INDIRECT: {
                    int xr = ((PRIndirectReference)obj).Number;
                    obj = (PdfObject)xrefObj[xr];
                    xrefObj[xr] = null;
                    freeXref = xr;
                    KillXref(obj);
                    break;
                }
                case PdfObject.ARRAY: {
                    ArrayList t = ((PdfArray)obj).ArrayList;
                    for (int i = 0; i < t.Count; ++i)
                        KillXref((PdfObject)t[i]);
                    break;
                }
                case PdfObject.STREAM:
                case PdfObject.DICTIONARY: {
                    PdfDictionary dic = (PdfDictionary)obj;
                    foreach (PdfName key in dic.Keys){
                        KillXref(dic.Get(key));
                    }
                    break;
                }
            }
        }
        
        /** Sets the contents of the page.
        * @param content the new page content
        * @param pageNum the page number. 1 is the first
        * @throws IOException on error
        */    
        public void SetPageContent(int pageNum, byte[] content) {
            PdfDictionary page = GetPageN(pageNum);
            if (page == null)
                return;
            PdfObject contents = page.Get(PdfName.CONTENTS);
            freeXref = -1;
            KillXref(contents);
            if (freeXref == -1) {
                xrefObj.Add(null);
                freeXref = xrefObj.Count - 1;
            }
            page.Put(PdfName.CONTENTS, new PRIndirectReference(this, freeXref));
            xrefObj[freeXref] = new PRStream(this, content);
        }
        
        /** Get the content from a stream applying the required filters.
        * @param stream the stream
        * @param file the location where the stream is
        * @throws IOException on error
        * @return the stream content
        */    
        public static byte[] GetStreamBytes(PRStream stream, RandomAccessFileOrArray file) {
            PdfObject filter = GetPdfObjectRelease(stream.Get(PdfName.FILTER));
            byte[] b = GetStreamBytesRaw(stream, file);
            ArrayList filters = new ArrayList();
            if (filter != null) {
                if (filter.IsName())
                    filters.Add(filter);
                else if (filter.IsArray())
                    filters = ((PdfArray)filter).ArrayList;
            }
            ArrayList dp = new ArrayList();
            PdfObject dpo = GetPdfObjectRelease(stream.Get(PdfName.DECODEPARMS));
            if (dpo == null || (!dpo.IsDictionary() && !dpo.IsArray()))
                dpo = GetPdfObjectRelease(stream.Get(PdfName.DP));
            if (dpo != null) {
                if (dpo.IsDictionary())
                    dp.Add(dpo);
                else if (dpo.IsArray())
                    dp = ((PdfArray)dpo).ArrayList;
            }
            String name;
            for (int j = 0; j < filters.Count; ++j) {
                name = ((PdfName)GetPdfObjectRelease((PdfObject)filters[j])).ToString();
                if (name.Equals("/FlateDecode") || name.Equals("/Fl")) {
                    b = FlateDecode(b);
                    PdfObject dicParam = null;
                    if (j < dp.Count) {
                        dicParam = (PdfObject)dp[j];
                        b = DecodePredictor(b, dicParam);
                    }
                }
                else if (name.Equals("/ASCIIHexDecode") || name.Equals("/AHx"))
                    b = ASCIIHexDecode(b);
                else if (name.Equals("/ASCII85Decode") || name.Equals("/A85"))
                    b = ASCII85Decode(b);
                else if (name.Equals("/LZWDecode")) {
                    b = LZWDecode(b);
                    PdfObject dicParam = null;
                    if (j < dp.Count) {
                        dicParam = (PdfObject)dp[j];
                        b = DecodePredictor(b, dicParam);
                    }
                }
                else if (name.Equals("/Crypt")) {
                }
                else
                    throw new IOException("The filter " + name + " is not supported.");
            }
            return b;
        }
        
        /** Get the content from a stream applying the required filters.
        * @param stream the stream
        * @throws IOException on error
        * @return the stream content
        */    
        public static byte[] GetStreamBytes(PRStream stream) {
            RandomAccessFileOrArray rf = stream.Reader.SafeFile;
            try {
                rf.ReOpen();
                return GetStreamBytes(stream, rf);
            }
            finally {
                try{rf.Close();}catch{}
            }
        }
        
        /** Get the content from a stream as it is without applying any filter.
        * @param stream the stream
        * @param file the location where the stream is
        * @throws IOException on error
        * @return the stream content
        */    
        public static byte[] GetStreamBytesRaw(PRStream stream, RandomAccessFileOrArray file) {
            PdfReader reader = stream.Reader;
            byte[] b;
            if (stream.Offset < 0)
                b = stream.GetBytes();
            else {
                b = new byte[stream.Length];
                file.Seek(stream.Offset);
                file.ReadFully(b);
                PdfEncryption decrypt = reader.Decrypt;
                if (decrypt != null) {
                    PdfObject filter = GetPdfObjectRelease(stream.Get(PdfName.FILTER));
                    ArrayList filters = new ArrayList();
                    if (filter != null) {
                        if (filter.IsName())
                            filters.Add(filter);
                        else if (filter.IsArray())
                            filters = ((PdfArray)filter).ArrayList;
                    }
                    bool skip = false;
                    for (int k = 0; k < filters.Count; ++k) {
                        PdfObject obj = GetPdfObjectRelease((PdfObject)filters[k]);
                        if (obj != null && obj.ToString().Equals("/Crypt")) {
                            skip = true;
                            break;
                        }
                    }
                    if (!skip) {
                        decrypt.SetHashKey(stream.ObjNum, stream.ObjGen);
                        b = decrypt.DecryptByteArray(b);
                    }
                }
            }
            return b;
        }
        
        /** Get the content from a stream as it is without applying any filter.
        * @param stream the stream
        * @throws IOException on error
        * @return the stream content
        */    
        public static byte[] GetStreamBytesRaw(PRStream stream) {
            RandomAccessFileOrArray rf = stream.Reader.SafeFile;
            try {
                rf.ReOpen();
                return GetStreamBytesRaw(stream, rf);
            }
            finally {
                try{rf.Close();}catch{}
            }
        }

        /** Eliminates shared streams if they exist. */    
        public void EliminateSharedStreams() {
            if (!sharedStreams)
                return;
            sharedStreams = false;
            if (pageRefs.Size == 1)
                return;
            ArrayList newRefs = new ArrayList();
            ArrayList newStreams = new ArrayList();
            IntHashtable visited = new IntHashtable();
            for (int k = 1; k <= pageRefs.Size; ++k) {
                PdfDictionary page = pageRefs.GetPageN(k);
                if (page == null)
                    continue;
                PdfObject contents = GetPdfObject(page.Get(PdfName.CONTENTS));
                if (contents == null)
                    continue;
                if (contents.IsStream()) {
                    PRIndirectReference refi = (PRIndirectReference)page.Get(PdfName.CONTENTS);
                    if (visited.ContainsKey(refi.Number)) {
                        // need to duplicate
                        newRefs.Add(refi);
                        newStreams.Add(new PRStream((PRStream)contents, null));
                    }
                    else
                        visited[refi.Number] = 1;
                }
                else if (contents.IsArray()) {
                    PdfArray array = (PdfArray)contents;
                    ArrayList list = array.ArrayList;
                    for (int j = 0; j < list.Count; ++j) {
                        PRIndirectReference refi = (PRIndirectReference)list[j];
                        if (visited.ContainsKey(refi.Number)) {
                            // need to duplicate
                            newRefs.Add(refi);
                            newStreams.Add(new PRStream((PRStream)GetPdfObject(refi), null));
                        }
                        else
                            visited[refi.Number] = 1;
                    }
                }
            }
            if (newStreams.Count == 0)
                return;
            for (int k = 0; k < newStreams.Count; ++k) {
                xrefObj.Add(newStreams[k]);
                PRIndirectReference refi = (PRIndirectReference)newRefs[k];
                refi.SetNumber(xrefObj.Count - 1, 0);
            }
        }
                
        /**
        * Sets the tampered state. A tampered PdfReader cannot be reused in PdfStamper.
        * @param tampered the tampered state
        */            
        public bool Tampered {
            get {
                return tampered;
            }
            set {
                tampered = value;
                pageRefs.KeepPages();            }
        }

        /** Gets the XML metadata.
        * @throws IOException on error
        * @return the XML metadata
        */
        public byte[] Metadata {
            get {
                PdfObject obj = GetPdfObject(catalog.Get(PdfName.METADATA));
                if (!(obj is PRStream))
                    return null;
                RandomAccessFileOrArray rf = SafeFile;
                byte[] b = null;
                try {
                    rf.ReOpen();
                    b = GetStreamBytes((PRStream)obj, rf);
                }
                finally {
                    try {
                        rf.Close();
                    }
                    catch{
                        // empty on purpose
                    }
                }
                return b;
            }
        }
        
        /**
        * Gets the byte address of the last xref table.
        * @return the byte address of the last xref table
        */    
        public int LastXref {
            get {
                return lastXref;
            }
        }
        
        /**
        * Gets the number of xref objects.
        * @return the number of xref objects
        */    
        public int XrefSize {
            get {
                return xrefObj.Count;
            }
        }
        
        /**
        * Gets the byte address of the %%EOF marker.
        * @return the byte address of the %%EOF marker
        */    
        public int EofPos {
            get {
                return eofPos;
            }
        }
        
        /**
        * Gets the PDF version. Only the last version char is returned. For example
        * version 1.4 is returned as '4'.
        * @return the PDF version
        */    
        public char PdfVersion {
            get {
                return pdfVersion;
            }
        }
        
        /**
        * Returns <CODE>true</CODE> if the PDF is encrypted.
        * @return <CODE>true</CODE> if the PDF is encrypted
        */    
        public bool IsEncrypted() {
            return encrypted;
        }
        
        /**
        * Gets the encryption permissions. It can be used directly in
        * <CODE>PdfWriter.SetEncryption()</CODE>.
        * @return the encryption permissions
        */    
        public int Permissions {
            get {
                return pValue;
            }
        }
        
        /**
        * Returns <CODE>true</CODE> if the PDF has a 128 bit key encryption.
        * @return <CODE>true</CODE> if the PDF has a 128 bit key encryption
        */    
        public bool Is128Key() {
            return rValue == 3;
        }
        
        /**
        * Gets the trailer dictionary
        * @return the trailer dictionary
        */    
        public PdfDictionary Trailer {
            get {
                return trailer;
            }
        }
        
        internal PdfEncryption Decrypt {
            get {
                return decrypt;
            }
        }
        
        internal static bool Equalsn(byte[] a1, byte[] a2) {
            int length = a2.Length;
            for (int k = 0; k < length; ++k) {
                if (a1[k] != a2[k])
                    return false;
            }
            return true;
        }
        
        internal static bool ExistsName(PdfDictionary dic, PdfName key, PdfName value) {
            PdfObject type = GetPdfObjectRelease(dic.Get(key));
            if (type == null || !type.IsName())
                return false;
            PdfName name = (PdfName)type;
            return name.Equals(value);
        }
        
        internal static String GetFontName(PdfDictionary dic) {
            if (dic == null)
                return null;
            PdfObject type = GetPdfObjectRelease(dic.Get(PdfName.BASEFONT));
            if (type == null || !type.IsName())
                return null;
            return PdfName.DecodeName(type.ToString());
        }
        
        internal static String GetSubsetPrefix(PdfDictionary dic) {
            if (dic == null)
                return null;
            String s = GetFontName(dic);
            if (s == null)
                return null;
            if (s.Length < 8 || s[6] != '+')
                return null;
            for (int k = 0; k < 6; ++k) {
                char c = s[k];
                if (c < 'A' || c > 'Z')
                    return null;
            }
            return s;
        }
        
        /** Finds all the font subsets and changes the prefixes to some
        * random values.
        * @return the number of font subsets altered
        */    
        public int ShuffleSubsetNames() {
            int total = 0;
            for (int k = 1; k < xrefObj.Count; ++k) {
                PdfObject obj = GetPdfObjectRelease(k);
                if (obj == null || !obj.IsDictionary())
                    continue;
                PdfDictionary dic = (PdfDictionary)obj;
                if (!ExistsName(dic, PdfName.TYPE, PdfName.FONT))
                    continue;
                if (ExistsName(dic, PdfName.SUBTYPE, PdfName.TYPE1)
                    || ExistsName(dic, PdfName.SUBTYPE, PdfName.MMTYPE1)
                    || ExistsName(dic, PdfName.SUBTYPE, PdfName.TRUETYPE)) {
                    String s = GetSubsetPrefix(dic);
                    if (s == null)
                        continue;
                    String ns = BaseFont.CreateSubsetPrefix() + s.Substring(7);
                    PdfName newName = new PdfName(ns);
                    dic.Put(PdfName.BASEFONT, newName);
                    SetXrefPartialObject(k, dic);
                    ++total;
                    PdfDictionary fd = (PdfDictionary)GetPdfObject(dic.Get(PdfName.FONTDESCRIPTOR));
                    if (fd == null)
                        continue;
                    fd.Put(PdfName.FONTNAME, newName);
                }
                else if (ExistsName(dic, PdfName.SUBTYPE, PdfName.TYPE0)) {
                    String s = GetSubsetPrefix(dic);
                    PdfArray arr = (PdfArray)GetPdfObject(dic.Get(PdfName.DESCENDANTFONTS));
                    if (arr == null)
                        continue;
                    ArrayList list = arr.ArrayList;
                    if (list.Count == 0)
                        continue;
                    PdfDictionary desc = (PdfDictionary)GetPdfObject((PdfObject)list[0]);
                    String sde = GetSubsetPrefix(desc);
                    if (sde == null)
                        continue;
                    String ns = BaseFont.CreateSubsetPrefix();
                    if (s != null)
                        dic.Put(PdfName.BASEFONT, new PdfName(ns + s.Substring(7)));
                    SetXrefPartialObject(k, dic);
                    PdfName newName = new PdfName(ns + sde.Substring(7));
                    desc.Put(PdfName.BASEFONT, newName);
                    ++total;
                    PdfDictionary fd = (PdfDictionary)GetPdfObject(desc.Get(PdfName.FONTDESCRIPTOR));
                    if (fd == null)
                        continue;
                    fd.Put(PdfName.FONTNAME, newName);
                }
            }
            return total;
        }
        
        /** Finds all the fonts not subset but embedded and marks them as subset.
        * @return the number of fonts altered
        */    
        public int CreateFakeFontSubsets() {
            int total = 0;
            for (int k = 1; k < xrefObj.Count; ++k) {
                PdfObject obj = GetPdfObjectRelease(k);
                if (obj == null || !obj.IsDictionary())
                    continue;
                PdfDictionary dic = (PdfDictionary)obj;
                if (!ExistsName(dic, PdfName.TYPE, PdfName.FONT))
                    continue;
                if (ExistsName(dic, PdfName.SUBTYPE, PdfName.TYPE1)
                    || ExistsName(dic, PdfName.SUBTYPE, PdfName.MMTYPE1)
                    || ExistsName(dic, PdfName.SUBTYPE, PdfName.TRUETYPE)) {
                    String s = GetSubsetPrefix(dic);
                    if (s != null)
                        continue;
                    s = GetFontName(dic);
                    if (s == null)
                        continue;
                    String ns = BaseFont.CreateSubsetPrefix() + s;
                    PdfDictionary fd = (PdfDictionary)GetPdfObjectRelease(dic.Get(PdfName.FONTDESCRIPTOR));
                    if (fd == null)
                        continue;
                    if (fd.Get(PdfName.FONTFILE) == null && fd.Get(PdfName.FONTFILE2) == null
                        && fd.Get(PdfName.FONTFILE3) == null)
                        continue;
                    fd = (PdfDictionary)GetPdfObject(dic.Get(PdfName.FONTDESCRIPTOR));
                    PdfName newName = new PdfName(ns);
                    dic.Put(PdfName.BASEFONT, newName);
                    fd.Put(PdfName.FONTNAME, newName);
                    SetXrefPartialObject(k, dic);
                    ++total;
                }
            }
            return total;
        }
        
        private static PdfArray GetNameArray(PdfObject obj) {
            if (obj == null)
                return null;
            obj = GetPdfObjectRelease(obj);
            if (obj == null)
                return null;
            if (obj.IsArray())
                return (PdfArray)obj;
            else if (obj.IsDictionary()) {
                PdfObject arr2 = GetPdfObjectRelease(((PdfDictionary)obj).Get(PdfName.D));
                if (arr2 != null && arr2.IsArray())
                    return (PdfArray)arr2;
            }
            return null;
        }

        /**
        * Gets all the named destinations as an <CODE>Hashtable</CODE>. The key is the name
        * and the value is the destinations array.
        * @return gets all the named destinations
        */    
        public Hashtable GetNamedDestination() {
            Hashtable names = GetNamedDestinationFromNames();
            Hashtable names2 = GetNamedDestinationFromStrings(); 
            foreach (DictionaryEntry ie in names2)
                names[ie.Key] = ie.Value;
            return names;
        }
        
        /**
        * Gets the named destinations from the /Dests key in the catalog as an <CODE>Hashtable</CODE>. The key is the name
        * and the value is the destinations array.
        * @return gets the named destinations
        */    
        public Hashtable GetNamedDestinationFromNames() {
            Hashtable names = new Hashtable();
            if (catalog.Get(PdfName.DESTS) != null) {
                PdfDictionary dic = (PdfDictionary)GetPdfObjectRelease(catalog.Get(PdfName.DESTS));
                if (dic == null)
                    return names;
                foreach (PdfName key in dic.Keys) {
                    String name = PdfName.DecodeName(key.ToString());
                    PdfArray arr = GetNameArray(dic.Get(key));
                    if (arr != null)
                        names[name] = arr;
                }
            }
            return names;
        }

        /**
        * Gets the named destinations from the /Names key in the catalog as an <CODE>Hashtable</CODE>. The key is the name
        * and the value is the destinations array.
        * @return gets the named destinations
        */    
        public Hashtable GetNamedDestinationFromStrings() {
            if (catalog.Get(PdfName.NAMES) != null) {
                PdfDictionary dic = (PdfDictionary)GetPdfObjectRelease(catalog.Get(PdfName.NAMES));
                if (dic != null) {
                    dic = (PdfDictionary)GetPdfObjectRelease(dic.Get(PdfName.DESTS));
                    if (dic != null) {
                        Hashtable names = PdfNameTree.ReadTree(dic);
                        object[] keys = new object[names.Count];
                        names.Keys.CopyTo(keys, 0);
                        foreach (object key in keys) {
                            PdfArray arr = GetNameArray((PdfObject)names[key]);
                            if (arr != null)
                                names[key] = arr;
                            else
                                names.Remove(key);
                        }
                        return names;
                    }
                }
            }
            return new Hashtable();
        }
        
        private bool ReplaceNamedDestination(PdfObject obj, Hashtable names) {
            obj = GetPdfObject(obj);
            int objIdx = lastXrefPartial;
            ReleaseLastXrefPartial();
            if (obj != null && obj.IsDictionary()) {
                PdfObject ob2 = GetPdfObjectRelease(((PdfDictionary)obj).Get(PdfName.DEST));
                String name = null;
                if (ob2 != null) {
                    if (ob2.IsName())
                        name = PdfName.DecodeName(ob2.ToString());
                    else if (ob2.IsString())
                        name = ob2.ToString();
                    if (name != null) {
                        PdfArray dest = (PdfArray)names[name];
                        if (dest != null) {
                            ((PdfDictionary)obj).Put(PdfName.DEST, dest);
                            SetXrefPartialObject(objIdx, obj);
                            return true;
                        }
                    }
                }
                else if ((ob2 = GetPdfObject(((PdfDictionary)obj).Get(PdfName.A))) != null) {
                    int obj2Idx = lastXrefPartial;
                    ReleaseLastXrefPartial();
                    PdfDictionary dic = (PdfDictionary)ob2;
                    PdfName type = (PdfName)GetPdfObjectRelease(dic.Get(PdfName.S));
                    if (PdfName.GOTO.Equals(type)) {
                        PdfObject ob3 = GetPdfObjectRelease(dic.Get(PdfName.D));
                        if (ob3 != null) {
                            if (ob3.IsName())
                                name = PdfName.DecodeName(ob3.ToString());
                            else if (ob3.IsString())
                                name = ob3.ToString();
                        }
                        if (name != null) {
                            PdfArray dest = (PdfArray)names[name];
                            if (dest != null) {
                                dic.Put(PdfName.D, dest);
                                SetXrefPartialObject(obj2Idx, ob2);
                                SetXrefPartialObject(objIdx, obj);
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }
        
        /**
        * Removes all the fields from the document.
        */    
        public void RemoveFields() {
            pageRefs.ResetReleasePage();
            for (int k = 1; k <= pageRefs.Size; ++k) {
                PdfDictionary page = pageRefs.GetPageN(k);
                PdfArray annots = (PdfArray)GetPdfObject(page.Get(PdfName.ANNOTS));
                if (annots == null) {
                    pageRefs.ReleasePage(k);
                    continue;
                }
                ArrayList arr = annots.ArrayList;
                for (int j = 0; j < arr.Count; ++j) {
                    PdfObject obj = GetPdfObjectRelease((PdfObject)arr[j]);
                    if (obj == null || !obj.IsDictionary())
                        continue;
                    PdfDictionary annot = (PdfDictionary)obj;
                    if (PdfName.WIDGET.Equals(annot.Get(PdfName.SUBTYPE)))
                        arr.RemoveAt(j--);
                }
                if (arr.Count == 0)
                    page.Remove(PdfName.ANNOTS);
                else
                    pageRefs.ReleasePage(k);
            }
            catalog.Remove(PdfName.ACROFORM);
            pageRefs.ResetReleasePage();
        }
        
        /**
        * Removes all the annotations and fields from the document.
        */    
        public void RemoveAnnotations() {
            pageRefs.ResetReleasePage();
            for (int k = 1; k <= pageRefs.Size; ++k) {
                PdfDictionary page = pageRefs.GetPageN(k);
                if (page.Get(PdfName.ANNOTS) == null)
                    pageRefs.ReleasePage(k);
                else
                    page.Remove(PdfName.ANNOTS);
            }
            catalog.Remove(PdfName.ACROFORM);
            pageRefs.ResetReleasePage();
        }
        
        public ArrayList GetLinks(int page) {
            pageRefs.ResetReleasePage();
            ArrayList result = new ArrayList();
            PdfDictionary pageDic = pageRefs.GetPageN(page);
            if (pageDic.Get(PdfName.ANNOTS) != null) {
                PdfArray annots = (PdfArray)GetPdfObject(pageDic.Get(PdfName.ANNOTS));
                ArrayList arr = annots.ArrayList;
                for (int j = 0; j < arr.Count; ++j) {
                    PdfDictionary annot = (PdfDictionary)GetPdfObjectRelease((PdfObject)arr[j]);
                  
                    if (PdfName.LINK.Equals(annot.Get(PdfName.SUBTYPE))) {
                        result.Add(new PdfAnnotation.PdfImportedLink(annot));
                    }
                }
            }
            pageRefs.ReleasePage(page);
            pageRefs.ResetReleasePage();
            return result;
        }

        private void IterateBookmarks(PdfObject outlineRef, Hashtable names) {
            while (outlineRef != null) {
                ReplaceNamedDestination(outlineRef, names);
                PdfDictionary outline = (PdfDictionary)GetPdfObjectRelease(outlineRef);
                PdfObject first = outline.Get(PdfName.FIRST);
                if (first != null) {
                    IterateBookmarks(first, names);
                }
                outlineRef = outline.Get(PdfName.NEXT);
            }
        }
        
        /** Replaces all the local named links with the actual destinations. */    
        public void ConsolidateNamedDestinations() {
            if (consolidateNamedDestinations)
                return;
            consolidateNamedDestinations = true;
            Hashtable names = GetNamedDestination();
            if (names.Count == 0)
                return;
            for (int k = 1; k <= pageRefs.Size; ++k) {
                PdfDictionary page = pageRefs.GetPageN(k);
                PdfObject annotsRef;
                PdfArray annots = (PdfArray)GetPdfObject(annotsRef = page.Get(PdfName.ANNOTS));
                int annotIdx = lastXrefPartial;
                ReleaseLastXrefPartial();
                if (annots == null) {
                    pageRefs.ReleasePage(k);
                    continue;
                }
                ArrayList list = annots.ArrayList;
                bool commitAnnots = false;
                for (int an = 0; an < list.Count; ++an) {
                    PdfObject objRef = (PdfObject)list[an];
                    if (ReplaceNamedDestination(objRef, names) && !objRef.IsIndirect())
                        commitAnnots = true;
                }
                if (commitAnnots)
                    SetXrefPartialObject(annotIdx,  annots);
                if (!commitAnnots || annotsRef.IsIndirect())
                    pageRefs.ReleasePage(k);
            }
            PdfDictionary outlines = (PdfDictionary)GetPdfObjectRelease(catalog.Get(PdfName.OUTLINES));
            if (outlines == null)
                return;
            IterateBookmarks(outlines.Get(PdfName.FIRST), names);
        }
        
        protected internal static PdfDictionary DuplicatePdfDictionary(PdfDictionary original, PdfDictionary copy, PdfReader newReader) {
            if (copy == null)
                copy = new PdfDictionary();
            foreach (PdfName key in original.Keys) {
                copy.Put(key, DuplicatePdfObject(original.Get(key), newReader));
            }
            return copy;
        }
        
        protected internal static PdfObject DuplicatePdfObject(PdfObject original, PdfReader newReader) {
            if (original == null)
                return null;
            switch (original.Type) {
                case PdfObject.DICTIONARY: {
                    return DuplicatePdfDictionary((PdfDictionary)original, null, newReader);
                }
                case PdfObject.STREAM: {
                    PRStream org = (PRStream)original;
                    PRStream stream = new PRStream(org, null, newReader);
                    DuplicatePdfDictionary(org, stream, newReader);
                    return stream;
                }
                case PdfObject.ARRAY: {
                    ArrayList list = ((PdfArray)original).ArrayList;
                    PdfArray arr = new PdfArray();
                    foreach (PdfObject ob in list) {
                        arr.Add(DuplicatePdfObject(ob, newReader));
                    }
                    return arr;
                }
                case PdfObject.INDIRECT: {
                    PRIndirectReference org = (PRIndirectReference)original;
                    return new PRIndirectReference(newReader, org.Number, org.Generation);
                }
                default:
                    return original;
            }
        }
        
        /**
        * Closes the reader
        */
        public void Close() {
            if (!partial)
                return;
            tokens.Close();
        }
        
        protected internal void RemoveUnusedNode(PdfObject obj, bool[] hits) {
            Stack state = new Stack();
            state.Push(obj);
            while (state.Count != 0) {
                Object current = state.Pop();
                if (current == null)
                    continue;
                ArrayList ar = null;
                PdfDictionary dic = null;
                PdfName[] keys = null;
                Object[] objs = null;
                int idx = 0;
                if (current is PdfObject) {
                    obj = (PdfObject)current;
                    switch (obj.Type) {
                        case PdfObject.DICTIONARY:
                        case PdfObject.STREAM:
                            dic = (PdfDictionary)obj;
                            keys = new PdfName[dic.Size];
                            dic.Keys.CopyTo(keys, 0);
                            break;
                        case PdfObject.ARRAY:
                            ar = ((PdfArray)obj).ArrayList;
                            break;
                        case PdfObject.INDIRECT:
                            PRIndirectReference refi = (PRIndirectReference)obj;
                            int num = refi.Number;
                            if (!hits[num]) {
                                hits[num] = true;
                                state.Push(GetPdfObjectRelease(refi));
                            }
                            continue;
                        default:
                            continue;
                    }
                }
                else {
                    objs = (Object[])current;
                    if (objs[0] is ArrayList) {
                        ar = (ArrayList)objs[0];
                        idx = (int)objs[1];
                    }
                    else {
                        keys = (PdfName[])objs[0];
                        dic = (PdfDictionary)objs[1];
                        idx = (int)objs[2];
                    }
                }
                if (ar != null) {
                    for (int k = idx; k < ar.Count; ++k) {
                        PdfObject v = (PdfObject)ar[k];
                        if (v.IsIndirect()) {
                            int num = ((PRIndirectReference)v).Number;
                            if (num >= xrefObj.Count || (!partial && xrefObj[num] == null)) {
                                ar[k] = PdfNull.PDFNULL;
                                continue;
                            }
                        }
                        if (objs == null)
                            state.Push(new Object[]{ar, k + 1});
                        else {
                            objs[1] = k + 1;
                            state.Push(objs);
                        }
                        state.Push(v);
                        break;
                    }
                }
                else {
                    for (int k = idx; k < keys.Length; ++k) {
                        PdfName key = keys[k];
                        PdfObject v = dic.Get(key);
                        if (v.IsIndirect()) {
                            int num = ((PRIndirectReference)v).Number;
                            if (num >= xrefObj.Count || (!partial && xrefObj[num] == null)) {
                                dic.Put(key, PdfNull.PDFNULL);
                                continue;
                            }
                        }
                        if (objs == null)
                            state.Push(new Object[]{keys, dic, k + 1});
                        else {
                            objs[2] = k + 1;
                            state.Push(objs);
                        }
                        state.Push(v);
                        break;
                    }
                }
            }
        }

        /** Removes all the unreachable objects.
        * @return the number of indirect objects removed
        */    
        public int RemoveUnusedObjects() {
            bool[] hits = new bool[xrefObj.Count];
            RemoveUnusedNode(trailer, hits);
            int total = 0;
            if (partial) {
                for (int k = 1; k < hits.Length; ++k) {
                    if (!hits[k]) {
                        xref[k * 2] = -1;
                        xref[k * 2 + 1] = 0;
                        xrefObj[k] = null;
                        ++total;
                    }
                }
            }
            else {
                for (int k = 1; k < hits.Length; ++k) {
                    if (!hits[k]) {
                        xrefObj[k] = null;
                        ++total;
                    }
                }
            }
            return total;
        }
        
        /** Gets a read-only version of <CODE>AcroFields</CODE>.
        * @return a read-only version of <CODE>AcroFields</CODE>
        */    
        public AcroFields AcroFields {
            get {
                return new AcroFields(this, null);
            }
        }
        
        /**
        * Gets the global document JavaScript.
        * @param file the document file
        * @throws IOException on error
        * @return the global document JavaScript
        */    
        public String GetJavaScript(RandomAccessFileOrArray file) {
            PdfDictionary names = (PdfDictionary)GetPdfObjectRelease(catalog.Get(PdfName.NAMES));
            if (names == null)
                return null;
            PdfDictionary js = (PdfDictionary)GetPdfObjectRelease(names.Get(PdfName.JAVASCRIPT));
            if (js == null)
                return null;
            Hashtable jscript = PdfNameTree.ReadTree(js);
            String[] sortedNames = new String[jscript.Count];
            jscript.Keys.CopyTo(sortedNames, 0);
            Array.Sort(sortedNames);
            StringBuilder buf = new StringBuilder();
            for (int k = 0; k < sortedNames.Length; ++k) {
                PdfDictionary j = (PdfDictionary)GetPdfObjectRelease((PdfIndirectReference)jscript[sortedNames[k]]);
                if (j == null)
                    continue;
                PdfObject obj = GetPdfObjectRelease(j.Get(PdfName.JS));
                if (obj != null) {
                    if (obj.IsString())
                        buf.Append(((PdfString)obj).ToUnicodeString()).Append('\n');
                    else if (obj.IsStream()) {
                        byte[] bytes = GetStreamBytes((PRStream)obj, file);
                        if (bytes.Length >= 2 && bytes[0] == (byte)254 && bytes[1] == (byte)255)
                            buf.Append(PdfEncodings.ConvertToString(bytes, PdfObject.TEXT_UNICODE));
                        else
                            buf.Append(PdfEncodings.ConvertToString(bytes, PdfObject.TEXT_PDFDOCENCODING));
                        buf.Append('\n');    
                    }
                }
            }
            return buf.ToString();
        }
        
        /**
        * Gets the global document JavaScript.
        * @throws IOException on error
        * @return the global document JavaScript
        */    
        public String JavaScript {
            get {
                RandomAccessFileOrArray rf = SafeFile;
                try {
                    rf.ReOpen();
                    return GetJavaScript(rf);
                }
                finally {
                    try{rf.Close();}catch{}
                }
            }
        }
        
        /**
        * Selects the pages to keep in the document. The pages are described as
        * ranges. The page ordering can be changed but
        * no page repetitions are allowed. Note that it may be very slow in partial mode.
        * @param ranges the comma separated ranges as described in {@link SequenceList}
        */    
        public void SelectPages(String ranges) {
            SelectPages(SequenceList.Expand(ranges, NumberOfPages));
        }
        
        /**
        * Selects the pages to keep in the document. The pages are described as a
        * <CODE>List</CODE> of <CODE>Integer</CODE>. The page ordering can be changed but
        * no page repetitions are allowed. Note that it may be very slow in partial mode.
        * @param pagesToKeep the pages to keep in the document
        */    
        public void SelectPages(ArrayList pagesToKeep) {
            pageRefs.SelectPages(pagesToKeep);
            RemoveUnusedObjects();
        }

        /** Sets the viewer preferences as the sum of several constants.
        * @param preferences the viewer preferences
        * @see PdfViewerPreferences#setViewerPreferences
        */
        public virtual int ViewerPreferences {
            set {
                this.viewerPreferences.ViewerPreferences = value;
                SetViewerPreferences(this.viewerPreferences);
            }
        }
        
        /** Adds a viewer preference
        * @param key a key for a viewer preference
        * @param value a value for the viewer preference
        * @see PdfViewerPreferences#addViewerPreference
        */
        public virtual void AddViewerPreference(PdfName key, PdfObject value) {
            this.viewerPreferences.AddViewerPreference(key, value);
            SetViewerPreferences(this.viewerPreferences);
        }
        
        internal virtual void SetViewerPreferences(PdfViewerPreferencesImp vp) {
            vp.AddToCatalog(catalog);
        }

        /**
        * @return an int that contains the Viewer Preferences.
        */
        public virtual int SimpleViewerPreferences {
            get {
                return PdfViewerPreferencesImp.GetViewerPreferences(catalog).PageLayoutAndMode;
            }
        }
        
        public bool Appendable {
            set {
                appendable = value;
                if (appendable)
                    GetPdfObject(trailer.Get(PdfName.ROOT));
            }
            get {
                return appendable;
            }
        }
        
        /**
        * Getter for property newXrefType.
        * @return Value of property newXrefType.
        */
        public bool IsNewXrefType() {
            return newXrefType;
        }    
        
        /**
        * Getter for property fileLength.
        * @return Value of property fileLength.
        */
        public int FileLength {
            get {
                return fileLength;
            }
        }
        
        /**
        * Getter for property hybridXref.
        * @return Value of property hybridXref.
        */
        public bool IsHybridXref() {
            return hybridXref;
        }
        
        public class PageRefs {
            private PdfReader reader;
            private IntHashtable refsp;
            private ArrayList refsn;
            private ArrayList pageInh;
            private int lastPageRead = -1;
            private int sizep;
            private bool keepPages;
            
            internal PageRefs(PdfReader reader) {
                this.reader = reader;
                if (reader.partial) {
                    refsp = new IntHashtable();
                    PdfNumber npages = (PdfNumber)PdfReader.GetPdfObjectRelease(reader.rootPages.Get(PdfName.COUNT));
                    sizep = npages.IntValue;
                }
                else {
                    ReadPages();
                }
            }
            
            internal PageRefs(PageRefs other, PdfReader reader) {
                this.reader = reader;
                this.sizep = other.sizep;
                if (other.refsn != null) {
                    refsn = new ArrayList(other.refsn);
                    for (int k = 0; k < refsn.Count; ++k) {
                        refsn[k] = DuplicatePdfObject((PdfObject)refsn[k], reader);
                    }
                }
                else
                    this.refsp = (IntHashtable)other.refsp.Clone();
            }
            
            internal int Size {
                get {
                    if (refsn != null)
                        return refsn.Count;
                    else
                        return sizep;
                }
            }
            
            internal void ReadPages() {
                if (refsn != null)
                    return;
                refsp = null;
                refsn = new ArrayList();
                pageInh = new ArrayList();
                IteratePages((PRIndirectReference)reader.catalog.Get(PdfName.PAGES));
                pageInh = null;
                reader.rootPages.Put(PdfName.COUNT, new PdfNumber(refsn.Count));
            }
            
            internal void ReReadPages() {
                refsn = null;
                ReadPages();
            }
            
            /** Gets the dictionary that represents a page.
            * @param pageNum the page number. 1 is the first
            * @return the page dictionary
            */    
            public PdfDictionary GetPageN(int pageNum) {
                PRIndirectReference refi = GetPageOrigRef(pageNum);
                return (PdfDictionary)PdfReader.GetPdfObject(refi);
            }

            /**
            * @param pageNum
            * @return a dictionary object
            */
            public PdfDictionary GetPageNRelease(int pageNum) {
                PdfDictionary page = GetPageN(pageNum);
                ReleasePage(pageNum);
                return page;
            }

            /**
            * @param pageNum
            * @return an indirect reference
            */
            public PRIndirectReference GetPageOrigRefRelease(int pageNum) {
                PRIndirectReference refi = GetPageOrigRef(pageNum);
                ReleasePage(pageNum);
                return refi;
            }
            
            /** Gets the page reference to this page.
            * @param pageNum the page number. 1 is the first
            * @return the page reference
            */    
            public PRIndirectReference GetPageOrigRef(int pageNum) {
                --pageNum;
                if (pageNum < 0 || pageNum >= Size)
                    return null;
                if (refsn != null)
                    return (PRIndirectReference)refsn[pageNum];
                else {
                    int n = refsp[pageNum];
                    if (n == 0) {
                        PRIndirectReference refi = GetSinglePage(pageNum);
                        if (reader.lastXrefPartial == -1)
                            lastPageRead = -1;
                        else
                            lastPageRead = pageNum;
                        reader.lastXrefPartial = -1;
                        refsp[pageNum] = refi.Number;
                        if (keepPages)
                            lastPageRead = -1;
                        return refi;
                    }
                    else {
                        if (lastPageRead != pageNum)
                            lastPageRead = -1;
                        if (keepPages)
                            lastPageRead = -1;
                        return new PRIndirectReference(reader, n);
                    }
                }
            }
            
            internal void KeepPages() {
                if (refsp == null || keepPages)
                    return;
                keepPages = true;
                refsp.Clear();
            }

            /**
            * @param pageNum
            */
            public void ReleasePage(int pageNum) {
                if (refsp == null)
                    return;
                --pageNum;
                if (pageNum < 0 || pageNum >= Size)
                    return;
                if (pageNum != lastPageRead)
                    return;
                lastPageRead = -1;
                reader.lastXrefPartial = refsp[pageNum];
                reader.ReleaseLastXrefPartial();
                refsp.Remove(pageNum);
            }
            
            /**
            * 
            */
            public void ResetReleasePage() {
                if (refsp == null)
                    return;
                lastPageRead = -1;
            }
            
            internal void InsertPage(int pageNum, PRIndirectReference refi) {
                --pageNum;
                if (refsn != null) {
                    if (pageNum >= refsn.Count)
                        refsn.Add(refi);
                    else
                        refsn.Insert(pageNum, refi);
                }
                else {
                    ++sizep;
                    lastPageRead = -1;
                    if (pageNum >= Size) {
                        refsp[Size] = refi.Number;
                    }
                    else {
                        IntHashtable refs2 = new IntHashtable((refsp.Size + 1) * 2);
                        for (IntHashtable.IntHashtableIterator it = refsp.GetEntryIterator(); it.HasNext();) {
                            IntHashtable.IntHashtableEntry entry = (IntHashtable.IntHashtableEntry)it.Next();
                            int p = entry.Key;
                            refs2[p >= pageNum ? p + 1 : p] = entry.Value;
                        }
                        refs2[pageNum] = refi.Number;
                        refsp = refs2;
                    }
                }
            }
            
            private void PushPageAttributes(PdfDictionary nodePages) {
                PdfDictionary dic = new PdfDictionary();
                if (pageInh.Count != 0) {
                    dic.Merge((PdfDictionary)pageInh[pageInh.Count - 1]);
                }
                for (int k = 0; k < pageInhCandidates.Length; ++k) {
                    PdfObject obj = nodePages.Get(pageInhCandidates[k]);
                    if (obj != null)
                        dic.Put(pageInhCandidates[k], obj);
                }
                pageInh.Add(dic);
            }

            private void PopPageAttributes() {
                pageInh.RemoveAt(pageInh.Count - 1);
            }

            private void IteratePages(PRIndirectReference rpage) {
                PdfDictionary page = (PdfDictionary)GetPdfObject(rpage);
                PdfArray kidsPR = (PdfArray)GetPdfObject(page.Get(PdfName.KIDS));
                if (kidsPR == null) {
                    page.Put(PdfName.TYPE, PdfName.PAGE);
                    PdfDictionary dic = (PdfDictionary)pageInh[pageInh.Count - 1];
                    foreach (PdfName key in dic.Keys) {
                        if (page.Get(key) == null)
                            page.Put(key, dic.Get(key));
                    }
                    if (page.Get(PdfName.MEDIABOX) == null) {
                        PdfArray arr = new PdfArray(new float[]{0,0,PageSize.LETTER.Right,PageSize.LETTER.Top});
                        page.Put(PdfName.MEDIABOX, arr);
                    }
                    refsn.Add(rpage);
                }
                else {
                    page.Put(PdfName.TYPE, PdfName.PAGES);
                    PushPageAttributes(page);
                    ArrayList kids = kidsPR.ArrayList;
                    for (int k = 0; k < kids.Count; ++k){
                        PdfObject obj = (PdfObject)kids[k];
                        if (!obj.IsIndirect()) {
                            while (k < kids.Count)
                                kids.RemoveAt(k);
                            break;
                        }
                        IteratePages((PRIndirectReference)obj);
                    }
                    PopPageAttributes();
                }
            }
            
            protected internal PRIndirectReference GetSinglePage(int n) {
                PdfDictionary acc = new PdfDictionary();
                PdfDictionary top = reader.rootPages;
                int baseb = 0;
                while (true) {
                    for (int k = 0; k < pageInhCandidates.Length; ++k) {
                        PdfObject obj = top.Get(pageInhCandidates[k]);
                        if (obj != null)
                            acc.Put(pageInhCandidates[k], obj);
                    }
                    PdfArray kids = (PdfArray)PdfReader.GetPdfObjectRelease(top.Get(PdfName.KIDS));
                    for (ListIterator it = new ListIterator(kids.ArrayList); it.HasNext();) {
                        PRIndirectReference refi = (PRIndirectReference)it.Next();
                        PdfDictionary dic = (PdfDictionary)GetPdfObject(refi);
                        int last = reader.lastXrefPartial;
                        PdfObject count = GetPdfObjectRelease(dic.Get(PdfName.COUNT));
                        reader.lastXrefPartial = last;
                        int acn = 1;
                        if (count != null && count.Type == PdfObject.NUMBER)
                            acn = ((PdfNumber)count).IntValue;
                        if (n < baseb + acn) {
                            if (count == null) {
                                dic.MergeDifferent(acc);
                                return refi;
                            }
                            reader.ReleaseLastXrefPartial();
                            top = dic;
                            break;
                        }
                        reader.ReleaseLastXrefPartial();
                        baseb += acn;
                    }
                }
            }

            internal void SelectPages(ArrayList pagesToKeep) {
                IntHashtable pg = new IntHashtable();
                ArrayList finalPages = new ArrayList();
                int psize = Size;
                foreach (int p in pagesToKeep) {
                    if (p >= 1 && p <= psize && !pg.ContainsKey(p)) {
                        pg[p] = 1;
                        finalPages.Add(p);
                    }
                }
                if (reader.partial) {
                    for (int k = 1; k <= psize; ++k) {
                        GetPageOrigRef(k);
                        ResetReleasePage();
                    }
                }
                PRIndirectReference parent = (PRIndirectReference)reader.catalog.Get(PdfName.PAGES);
                PdfDictionary topPages = (PdfDictionary)PdfReader.GetPdfObject(parent);
                ArrayList newPageRefs = new ArrayList(finalPages.Count);
                PdfArray kids = new PdfArray();
                foreach (int p in finalPages) {
                    PRIndirectReference pref = GetPageOrigRef(p);
                    ResetReleasePage();
                    kids.Add(pref);
                    newPageRefs.Add(pref);
                    GetPageN(p).Put(PdfName.PARENT, parent);
                }
                AcroFields af = reader.AcroFields;
                bool removeFields = (af.Fields.Count > 0);
                for (int k = 1; k <= psize; ++k) {
                    if (!pg.ContainsKey(k)) {
                        if (removeFields)
                            af.RemoveFieldsFromPage(k);
                        PRIndirectReference pref = GetPageOrigRef(k);
                        int nref = pref.Number;
                        reader.xrefObj[nref] = null;
                        if (reader.partial) {
                            reader.xref[nref * 2] = -1;
                            reader.xref[nref * 2 + 1] = 0;
                        }
                    }
                }
                topPages.Put(PdfName.COUNT, new PdfNumber(finalPages.Count));
                topPages.Put(PdfName.KIDS, kids);
                refsp = null;
                refsn = newPageRefs;
            }
        }

        internal PdfIndirectReference GetCryptoRef() {
            if (cryptoRef == null)
                return null;
            return new PdfIndirectReference(0, cryptoRef.Number, cryptoRef.Generation);
        }

        /**
        * Removes any usage rights that this PDF may have. Only Adobe can grant usage rights
        * and any PDF modification with iText will invalidate them. Invalidated usage rights may
        * confuse Acrobat and it's advisabe to remove them altogether.
        */
        public void RemoveUsageRights() {
            PdfDictionary perms = (PdfDictionary)GetPdfObject(catalog.Get(PdfName.PERMS));
            if (perms == null)
                return;
            perms.Remove(PdfName.UR);
            perms.Remove(PdfName.UR3);
            if (perms.Size == 0)
                catalog.Remove(PdfName.PERMS);
        }

        /**
        * Gets the certification level for this document. The return values can be <code>PdfSignatureAppearance.NOT_CERTIFIED</code>, 
        * <code>PdfSignatureAppearance.CERTIFIED_NO_CHANGES_ALLOWED</code>,
        * <code>PdfSignatureAppearance.CERTIFIED_FORM_FILLING</code> and
        * <code>PdfSignatureAppearance.CERTIFIED_FORM_FILLING_AND_ANNOTATIONS</code>.
        * <p>
        * No signature validation is made, use the methods availabe for that in <CODE>AcroFields</CODE>.
        * </p>
        * @return gets the certification level for this document
        */
        public int GetCertificationLevel() {
            PdfDictionary dic = (PdfDictionary)GetPdfObject(catalog.Get(PdfName.PERMS));
            if (dic == null)
                return PdfSignatureAppearance.NOT_CERTIFIED;
            dic = (PdfDictionary)GetPdfObject(dic.Get(PdfName.DOCMDP));
            if (dic == null)
                return PdfSignatureAppearance.NOT_CERTIFIED;
            PdfArray arr = (PdfArray)GetPdfObject(dic.Get(PdfName.REFERENCE));
            if (arr == null || arr.Size == 0)
                return PdfSignatureAppearance.NOT_CERTIFIED;
            dic = (PdfDictionary)GetPdfObject((PdfObject)(arr.ArrayList[0]));
            if (dic == null)
                return PdfSignatureAppearance.NOT_CERTIFIED;
            dic = (PdfDictionary)GetPdfObject(dic.Get(PdfName.TRANSFORMPARAMS));
            if (dic == null)
                return PdfSignatureAppearance.NOT_CERTIFIED;
            PdfNumber p = (PdfNumber)GetPdfObject(dic.Get(PdfName.P));
            if (p == null)
                return PdfSignatureAppearance.NOT_CERTIFIED;
            return p.IntValue;
        }

        /**
        * Checks if the document was opened with the owner password so that the end application
        * can decide what level of access restrictions to apply. If the document is not encrypted
        * it will return <CODE>true</CODE>.
        * @return <CODE>true</CODE> if the document was opened with the owner password or if it's not encrypted,
        * <CODE>false</CODE> if the document was opened with the user password
        */
        public bool IsOpenedWithFullPermissions {
            get {
                return !encrypted || ownerPasswordUsed;
            }
        } 

        public int GetCryptoMode() {
    	    if (decrypt == null) 
    		    return -1;
    	    else 
    		    return decrypt.GetCryptoMode();
        }
        
        public bool IsMetadataEncrypted() {
    	    if (decrypt == null) 
    		    return false; 
    	    else 
    		    return decrypt.IsMetadataEncrypted();
        }
        
        public byte[] ComputeUserPassword() {
    	    if (!encrypted || !ownerPasswordUsed) return null;
    	    return decrypt.ComputeUserPassword(password);
        }
    }
}