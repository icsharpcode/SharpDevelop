using System;
using System.Collections;
using System.Text;
using System.IO;
using Org.BouncyCastle.X509;
using Org.BouncyCastle.Crypto;
using iTextSharp.text;
/*
 * $Id: PdfSignatureAppearance.cs,v 1.13 2008/04/17 15:32:39 psoares33 Exp $
 * 
 * Copyright 2004-2006 by Paulo Soares.
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
    * This class takes care of the cryptographic options and appearances that form a signature.
    */
    public class PdfSignatureAppearance {
    
        /**
        * Enumeration representing the different rendering options of a signature
        */  
        public enum SignatureRender {
            Description,
            NameAndDescription,
            GraphicAndDescription
        }  
        
        /**
        * The self signed filter.
        */
        public static PdfName SELF_SIGNED = PdfName.ADOBE_PPKLITE;
        /**
        * The VeriSign filter.
        */
        public static PdfName VERISIGN_SIGNED = PdfName.VERISIGN_PPKVS;
        /**
        * The Windows Certificate Security.
        */
        public static PdfName WINCER_SIGNED = PdfName.ADOBE_PPKMS;

        public const int NOT_CERTIFIED = 0;
        public const int CERTIFIED_NO_CHANGES_ALLOWED = 1;
        public const int CERTIFIED_FORM_FILLING = 2;
        public const int CERTIFIED_FORM_FILLING_AND_ANNOTATIONS = 3;

        private const float TOP_SECTION = 0.3f;
        private const float MARGIN = 2;
        private Rectangle rect;
        private Rectangle pageRect;
        private PdfTemplate[] app = new PdfTemplate[5];
        private PdfTemplate frm; 
        private PdfStamperImp writer;
        private String layer2Text;
        private String reason;
        private String location;
        private DateTime signDate;
        private int page = 1;
        private String fieldName;
        private ICipherParameters privKey;
        private X509Certificate[] certChain;
        private object[] crlList;
        private PdfName filter;
        private bool newField;
        private ByteBuffer sigout;
        private Stream originalout;
        private string tempFile;
        private PdfDictionary cryptoDictionary;
        private PdfStamper stamper;
        private bool preClosed = false;
        private PdfSigGenericPKCS sigStandard;
        private int[] range;
        private FileStream raf;
        private byte[] bout;
        private int boutLen;
        private byte[] externalDigest;
        private byte[] externalRSAdata;
        private String digestEncryptionAlgorithm;
        private Hashtable exclusionLocations;
            
        internal PdfSignatureAppearance(PdfStamperImp writer) {
            this.writer = writer;
            signDate = DateTime.Now;
            fieldName = GetNewSigName();
        }		

        /**
        * Gets the rendering mode for this signature .
        * @return the rectangle rendering mode for this signature. 
        */    
        private SignatureRender render = SignatureRender.Description;

        public SignatureRender Render {
            get {
                return render;
            }
            set {
                render = value;
            }
        }

        /**
        * Sets the Image object to render when Render is set to SignatureRender.GraphicAndDescription
        * @param image rendered. If <CODE>null</CODE> the mode is defaulted
        * to SignatureRender.Description
        */    
        private Image signatureGraphic = null;

        public Image SignatureGraphic {
            get {
                return signatureGraphic;
            }
            set {
                signatureGraphic = value;
            }
        }
        
        /**
        * Sets the signature text identifying the signer.
        * @param text the signature text identifying the signer. If <CODE>null</CODE> or not set
        * a standard description will be used
        */    
        public string Layer2Text {
            get {
                return layer2Text;
            }
            set {
                layer2Text = value;
            }
        }
        
        /**
        * Sets the text identifying the signature status.
        * @param text the text identifying the signature status. If <CODE>null</CODE> or not set
        * the description "Signature Not Verified" will be used
        */    
        public string Layer4Text {
            get {
                return layer4Text;
            }
            set {
                layer4Text = value;
            }
        }
        
        /**
        * Gets the rectangle representing the signature dimensions.
        * @return the rectangle representing the signature dimensions. It may be <CODE>null</CODE>
        * or have zero width or height for invisible signatures
        */    
        public Rectangle Rect {
            get {
                return rect;
            }
        }
        
        /**
        * Gets the visibility status of the signature.
        * @return the visibility status of the signature
        */    
        public bool IsInvisible() {
            return (rect == null || rect.Width == 0 || rect.Height == 0);
        }
        
        /**
        * Sets the cryptographic parameters.
        * @param privKey the private key
        * @param certChain the certificate chain
        * @param crlList the certificate revocation list. It may be <CODE>null</CODE>
        * @param filter the crytographic filter type. It can be SELF_SIGNED, VERISIGN_SIGNED or WINCER_SIGNED
        */    
        public void SetCrypto(ICipherParameters privKey, X509Certificate[] certChain, object[] crlList, PdfName filter) {
            this.privKey = privKey;
            this.certChain = certChain;
            this.crlList = crlList;
            this.filter = filter;
        }
        
        /**
        * Sets the signature to be visible. It creates a new visible signature field.
        * @param pageRect the position and dimension of the field in the page
        * @param page the page to place the field. The fist page is 1
        * @param fieldName the field name or <CODE>null</CODE> to generate automatically a new field name
        */    
        public void SetVisibleSignature(Rectangle pageRect, int page, String fieldName) {
            if (fieldName != null) {
                if (fieldName.IndexOf('.') >= 0)
                    throw new ArgumentException("Field names cannot contain a dot.");
                AcroFields af = writer.AcroFields;
                AcroFields.Item item = af.GetFieldItem(fieldName);
                if (item != null)
                    throw new ArgumentException("The field " + fieldName + " already exists.");
                this.fieldName = fieldName;
            }
            if (page < 1 || page > writer.reader.NumberOfPages)
                throw new ArgumentException("Invalid page number: " + page);
            this.pageRect = new Rectangle(pageRect);
            this.pageRect.Normalize();
            rect = new Rectangle(this.pageRect.Width, this.pageRect.Height);
            this.page = page;
            newField = true;
        }
        
        /**
        * Sets the signature to be visible. An empty signature field with the same name must already exist.
        * @param fieldName the existing empty signature field name
        */    
        public void SetVisibleSignature(String fieldName) {
            AcroFields af = writer.AcroFields;
            AcroFields.Item item = af.GetFieldItem(fieldName);
            if (item == null)
                throw new ArgumentException("The field " + fieldName + " does not exist.");
            PdfDictionary merged = (PdfDictionary)item.merged[0];
            if (!PdfName.SIG.Equals(PdfReader.GetPdfObject(merged.Get(PdfName.FT))))
                throw new ArgumentException("The field " + fieldName + " is not a signature field.");
            this.fieldName = fieldName;
            PdfArray r = (PdfArray)PdfReader.GetPdfObject(merged.Get(PdfName.RECT));
            ArrayList ar = r.ArrayList;
            float llx = ((PdfNumber)PdfReader.GetPdfObject((PdfObject)ar[0])).FloatValue;
            float lly = ((PdfNumber)PdfReader.GetPdfObject((PdfObject)ar[1])).FloatValue;
            float urx = ((PdfNumber)PdfReader.GetPdfObject((PdfObject)ar[2])).FloatValue;
            float ury = ((PdfNumber)PdfReader.GetPdfObject((PdfObject)ar[3])).FloatValue;
            pageRect = new Rectangle(llx, lly, urx, ury);
            pageRect.Normalize();
            page = (int)item.page[0];
            int rotation = writer.reader.GetPageRotation(page);
            Rectangle pageSize = writer.reader.GetPageSizeWithRotation(page);
            switch (rotation) {
                case 90:
                    pageRect = new Rectangle(
                    pageRect.Bottom,
                    pageSize.Top - pageRect.Left,
                    pageRect.Top,
                    pageSize.Top - pageRect.Right);
                    break;
                case 180:
                    pageRect = new Rectangle(
                    pageSize.Right - pageRect.Left,
                    pageSize.Top - pageRect.Bottom,
                    pageSize.Right - pageRect.Right,
                    pageSize.Top - pageRect.Top);
                    break;
                case 270:
                    pageRect = new Rectangle(
                    pageSize.Right - pageRect.Bottom,
                    pageRect.Left,
                    pageSize.Right - pageRect.Top,
                    pageRect.Right);
                    break;
            }
            if (rotation != 0)
                pageRect.Normalize();
            rect = new Rectangle(this.pageRect.Width, this.pageRect.Height);
        }
        
        /**
        * Gets a template layer to create a signature appearance. The layers can go from 0 to 4.
        * <p>
        * Consult <A HREF="http://partners.adobe.com/asn/developer/pdfs/tn/PPKAppearances.pdf">PPKAppearances.pdf</A>
        * for further details.
        * @param layer the layer
        * @return a template
        */    
        public PdfTemplate GetLayer(int layer) {
            if (layer < 0 || layer >= app.Length)
                return null;
            PdfTemplate t = app[layer];
            if (t == null) {
                t = app[layer] = new PdfTemplate(writer);
                t.BoundingBox = rect;
                writer.AddDirectTemplateSimple(t, new PdfName("n" + layer));
            }
            return t;
        }
        
        /**
        * Gets the template that aggregates all appearance layers. This corresponds to the /FRM resource.
        * <p>
        * Consult <A HREF="http://partners.adobe.com/asn/developer/pdfs/tn/PPKAppearances.pdf">PPKAppearances.pdf</A>
        * for further details.
        * @return the template that aggregates all appearance layers
        */    
        public PdfTemplate GetTopLayer() {
            if (frm == null) {
                frm = new PdfTemplate(writer);
                frm.BoundingBox = rect;
                writer.AddDirectTemplateSimple(frm, new PdfName("FRM"));
            }
            return frm;
        }
        
        /**
        * Gets the main appearance layer.
        * <p>
        * Consult <A HREF="http://partners.adobe.com/asn/developer/pdfs/tn/PPKAppearances.pdf">PPKAppearances.pdf</A>
        * for further details.
        * @return the main appearance layer
        * @throws DocumentException on error
        * @throws IOException on error
        */    
        public PdfTemplate GetAppearance() {
            if (IsInvisible()) {
                PdfTemplate t = new PdfTemplate(writer);
                t.BoundingBox = new Rectangle(0, 0);
                writer.AddDirectTemplateSimple(t, null);
                return t;
            }
            if (app[0] == null) {
                PdfTemplate t = app[0] = new PdfTemplate(writer);
                t.BoundingBox = new Rectangle(100, 100);
                writer.AddDirectTemplateSimple(t, new PdfName("n0"));
                t.SetLiteral("% DSBlank\n");
            }
            if (app[1] == null && !acro6Layers) {
                PdfTemplate t = app[1] = new PdfTemplate(writer);
                t.BoundingBox = new Rectangle(100, 100);
                writer.AddDirectTemplateSimple(t, new PdfName("n1"));
                t.SetLiteral(questionMark);
            }
            if (app[2] == null) {
                String text;
                if (layer2Text == null) {
                    StringBuilder buf = new StringBuilder();
                    buf.Append("Digitally signed by ").Append(PdfPKCS7.GetSubjectFields((X509Certificate)certChain[0]).GetField("CN")).Append('\n');
                    buf.Append("Date: ").Append(signDate.ToString("yyyy.MM.dd HH:mm:ss zzz"));
                    if (reason != null)
                        buf.Append('\n').Append("Reason: ").Append(reason);
                    if (location != null)
                        buf.Append('\n').Append("Location: ").Append(location);
                    text = buf.ToString();
                }
                else
                    text = layer2Text;
                PdfTemplate t = app[2] = new PdfTemplate(writer);
                t.BoundingBox = rect;
                writer.AddDirectTemplateSimple(t, new PdfName("n2"));
                if (image != null) {
                    if (imageScale == 0) {
                        t.AddImage(image, rect.Width, 0, 0, rect.Height, 0, 0);
                    }
                    else {
                        float usableScale = imageScale;
                        if (imageScale < 0)
                            usableScale = Math.Min(rect.Width / image.Width, rect.Height / image.Height);
                        float w = image.Width * usableScale;
                        float h = image.Height * usableScale;
                        float x = (rect.Width - w) / 2;
                        float y = (rect.Height - h) / 2;
                        t.AddImage(image, w, 0, 0, h, x, y);
                    }
                }
                Font font;
                if (layer2Font == null)
                    font = new Font();
                else
                    font = new Font(layer2Font);
                float size = font.Size;

                Rectangle dataRect = null;
                Rectangle signatureRect = null;

                if (Render == SignatureRender.NameAndDescription || 
                    (Render == SignatureRender.GraphicAndDescription && this.SignatureGraphic != null)) {
                    // origin is the bottom-left
                    signatureRect = new Rectangle(
                        MARGIN, 
                        MARGIN, 
                        rect.Width / 2 - MARGIN,
                        rect.Height - MARGIN);
                    dataRect = new Rectangle(
                        rect.Width / 2 +  MARGIN / 2, 
                        MARGIN, 
                        rect.Width - MARGIN / 2,
                        rect.Height - MARGIN);

                    if (rect.Height > rect.Width) {
                        signatureRect = new Rectangle(
                            MARGIN, 
                            rect.Height / 2, 
                            rect.Width - MARGIN,
                            rect.Height);
                        dataRect = new Rectangle(
                            MARGIN, 
                            MARGIN, 
                            rect.Width - MARGIN,
                            rect.Height / 2 - MARGIN);
                    }
                }
                else {
                    dataRect = new Rectangle(
                        MARGIN, 
                        MARGIN, 
                        rect.Width - MARGIN,
                        rect.Height * (1 - TOP_SECTION) - MARGIN);
                }

                if (Render == SignatureRender.NameAndDescription) {
                    string signedBy = iTextSharp.text.pdf.PdfPKCS7.GetSubjectFields(this.certChain[0]).GetField("CN");
                    Rectangle sr2 = new Rectangle(signatureRect.Width - MARGIN, signatureRect.Height - MARGIN );
                    float signedSize = FitText(font, signedBy, sr2, -1, runDirection);

                    ColumnText ct2 = new ColumnText(t);
                    ct2.RunDirection = runDirection;
                    ct2.SetSimpleColumn(new Phrase(signedBy, font), signatureRect.Left, signatureRect.Bottom, signatureRect.Right, signatureRect.Top, signedSize, Element.ALIGN_LEFT);

                    ct2.Go();
                }
                else if (Render == SignatureRender.GraphicAndDescription) {
                    ColumnText ct2 = new ColumnText(t);
                    ct2.RunDirection = runDirection;
                    ct2.SetSimpleColumn(signatureRect.Left, signatureRect.Bottom, signatureRect.Right, signatureRect.Top, 0, Element.ALIGN_RIGHT);

                    Image im = Image.GetInstance(SignatureGraphic);
                    im.ScaleToFit(signatureRect.Width, signatureRect.Height);

                    Paragraph p = new Paragraph();
                    // must calculate the point to draw from to make image appear in middle of column
                    float x = 0;
                    // experimentation found this magic number to counteract Adobe's signature graphic, which
                    // offsets the y co-ordinate by 15 units
                    float y = -im.ScaledHeight + 15;

                    x = x + (signatureRect.Width - im.ScaledWidth) / 2;
                    y = y - (signatureRect.Height - im.ScaledHeight) / 2;
                    p.Add(new Chunk(im, x + (signatureRect.Width - im.ScaledWidth) / 2, y, false));
                    ct2.AddElement(p);
                    ct2.Go();
                }

                if (size <= 0) {
                    Rectangle sr = new Rectangle(dataRect.Width, dataRect.Height);
                    size = FitText(font, text, sr, 12, runDirection);
                }
                ColumnText ct = new ColumnText(t);
                ct.RunDirection = runDirection;
                ct.SetSimpleColumn(new Phrase(text, font), dataRect.Left, dataRect.Bottom, dataRect.Right, dataRect.Top, size, Element.ALIGN_LEFT);
                ct.Go();

            }
            if (app[3] == null && !acro6Layers) {
                PdfTemplate t = app[3] = new PdfTemplate(writer);
                t.BoundingBox = new Rectangle(100, 100);
                writer.AddDirectTemplateSimple(t, new PdfName("n3"));
                t.SetLiteral("% DSBlank\n");
            }
            if (app[4] == null && !acro6Layers) {
                PdfTemplate t = app[4] = new PdfTemplate(writer);
                t.BoundingBox = new Rectangle(0, rect.Height * (1 - TOP_SECTION), rect.Right, rect.Top);
                writer.AddDirectTemplateSimple(t, new PdfName("n4"));
                Font font;
                if (layer2Font == null)
                    font = new Font();
                else
                    font = new Font(layer2Font);
                float size = font.Size;
                String text = "Signature Not Verified";
                if (layer4Text != null)
                    text = layer4Text;
                Rectangle sr = new Rectangle(rect.Width - 2 * MARGIN, rect.Height * TOP_SECTION - 2 * MARGIN);
                size = FitText(font, text, sr, 15, runDirection);
                ColumnText ct = new ColumnText(t);
                ct.RunDirection = runDirection;
                ct.SetSimpleColumn(new Phrase(text, font), MARGIN, 0, rect.Width - MARGIN, rect.Height - MARGIN, size, Element.ALIGN_LEFT);
                ct.Go();
            }
            int rotation = writer.reader.GetPageRotation(page);
            Rectangle rotated = new Rectangle(rect);
            int n = rotation;
            while (n > 0) {
                rotated = rotated.Rotate();
                n -= 90;
            }
            if (frm == null) {
                frm = new PdfTemplate(writer);
                frm.BoundingBox = rotated;
                writer.AddDirectTemplateSimple(frm, new PdfName("FRM"));
                float scale = Math.Min(rect.Width, rect.Height) * 0.9f;
                float x = (rect.Width - scale) / 2;
                float y = (rect.Height - scale) / 2;
                scale /= 100;
                if (rotation == 90)
                    frm.ConcatCTM(0, 1, -1, 0, rect.Height, 0);
                else if (rotation == 180)
                    frm.ConcatCTM(-1, 0, 0, -1, rect.Width, rect.Height);
                else if (rotation == 270)
                    frm.ConcatCTM(0, -1, 1, 0, 0, rect.Width);
                frm.AddTemplate(app[0], 0, 0);
                if (!acro6Layers)
                    frm.AddTemplate(app[1], scale, 0, 0, scale, x, y);
                frm.AddTemplate(app[2], 0, 0);
                if (!acro6Layers) {
                    frm.AddTemplate(app[3], scale, 0, 0, scale, x, y);
                    frm.AddTemplate(app[4], 0, 0);
                }
            }
            PdfTemplate napp = new PdfTemplate(writer);
            napp.BoundingBox = rotated;
            writer.AddDirectTemplateSimple(napp, null);
            napp.AddTemplate(frm, 0, 0);
            return napp;
        }
        
        /**
        * Fits the text to some rectangle adjusting the font size as needed.
        * @param font the font to use
        * @param text the text
        * @param rect the rectangle where the text must fit
        * @param maxFontSize the maximum font size
        * @param runDirection the run direction
        * @return the calculated font size that makes the text fit
        */    
        public static float FitText(Font font, String text, Rectangle rect, float maxFontSize, int runDirection) {
            ColumnText ct = null;
            int status = 0;
            if (maxFontSize <= 0) {
                int cr = 0;
                int lf = 0;
                char[] t = text.ToCharArray();
                for (int k = 0; k < t.Length; ++k) {
                    if (t[k] == '\n')
                        ++lf;
                    else if (t[k] == '\r')
                        ++cr;
                }
                int minLines = Math.Max(cr, lf) + 1;
                maxFontSize = Math.Abs(rect.Height) / minLines - 0.001f;
            }
            font.Size = maxFontSize;
            Phrase ph = new Phrase(text, font);
            ct = new ColumnText(null);
            ct.SetSimpleColumn(ph, rect.Left, rect.Bottom, rect.Right, rect.Top, maxFontSize, Element.ALIGN_LEFT);
            ct.RunDirection = runDirection;
            status = ct.Go(true);
            if ((status & ColumnText.NO_MORE_TEXT) != 0)
                return maxFontSize;
            float precision = 0.1f;
            float min = 0;
            float max = maxFontSize;
            float size = maxFontSize;
            for (int k = 0; k < 50; ++k) { //just in case it doesn't converge
                size = (min + max) / 2;
                ct = new ColumnText(null);
                font.Size = size;
                ct.SetSimpleColumn(new Phrase(text, font), rect.Left, rect.Bottom, rect.Right, rect.Top, size, Element.ALIGN_LEFT);
                ct.RunDirection = runDirection;
                status = ct.Go(true);
                if ((status & ColumnText.NO_MORE_TEXT) != 0) {
                    if (max - min < size * precision)
                        return size;
                    min = size;
                }
                else
                    max = size;
            }
            return size;
        }
        
        /**
        * Sets the digest/signature to an external calculated value.
        * @param digest the digest. This is the actual signature
        * @param RSAdata the extra data that goes into the data tag in PKCS#7
        * @param digestEncryptionAlgorithm the encryption algorithm. It may must be <CODE>null</CODE> if the <CODE>digest</CODE>
        * is also <CODE>null</CODE>. If the <CODE>digest</CODE> is not <CODE>null</CODE>
        * then it may be "RSA" or "DSA"
        */    
        public void SetExternalDigest(byte[] digest, byte[] RSAdata, String digestEncryptionAlgorithm) {
            externalDigest = digest;
            externalRSAdata = RSAdata;
            this.digestEncryptionAlgorithm = digestEncryptionAlgorithm;
        }

        /**
        * Sets the signing reason.
        * @param reason the signing reason
        */    
        public string Reason {
            get {
                return reason;
            }
            set {
                reason = value;
            }
        }

        /**
        * Sets the signing location.
        * @param location the signing location
        */    
        public string Location {
            get {
                return location;
            }
            set {
                location = value;
            }
        }
            
        /**
        * Gets the private key.
        * @return the private key
        */    
        public ICipherParameters PrivKey {
            get {
                return privKey;
            }
        }
        
        /**
        * Gets the certificate chain.
        * @return the certificate chain
        */    
        public X509Certificate[] CertChain {
            get {
                return this.certChain;
            }
        }
        
        /**
        * Gets the certificate revocation list.
        * @return the certificate revocation list
        */    
        public object[] CrlList {
            get {
                return this.crlList;
            }
        }
        
        /**
        * Gets the filter used to sign the document.
        * @return the filter used to sign the document
        */    
        public PdfName Filter {
            get {
                return filter;
            }
        }
        
        /**
        * Checks if a new field was created.
        * @return <CODE>true</CODE> if a new field was created, <CODE>false</CODE> if signing
        * an existing field or if the signature is invisible
        */    
        public bool IsNewField() {
            return this.newField;
        }
        
        /**
        * Gets the page number of the field.
        * @return the page number of the field
        */    
        public int Page {
            get {
                return page;
            }
        }
        
        /**
        * Gets the field name.
        * @return the field name
        */    
        public String FieldName {
            get {
                return fieldName;
            }
        }
        
        /**
        * Gets the rectangle that represent the position and dimension of the signature in the page.
        * @return the rectangle that represent the position and dimension of the signature in the page
        */    
        public Rectangle PageRect {
            get {
                return pageRect;
            }
        }
        
        /**
        * Gets the signature date.
        * @return the signature date
        */    
        public DateTime SignDate {
            get {
                return signDate;
            }
            set {
                signDate = value;
            }
        }
        
        internal ByteBuffer Sigout {
            get {
                return sigout;
            }
            set {
                sigout = value;
            }
        }
                
        internal Stream Originalout {
            get {
                return originalout;
            }
            set {
                originalout = value;
            }
        }
        
        /**
        * Gets the temporary file.
        * @return the temporary file or <CODE>null</CODE> is the document is created in memory
        */    
        public string TempFile {
            get {
                return tempFile;
            }
        }

        internal void SetTempFile(string tempFile) {
            this.tempFile = tempFile;
        }

        /**
        * Gets a new signature fied name that doesn't clash with any existing name.
        * @return a new signature fied name
        */    
        public String GetNewSigName() {
            AcroFields af = writer.AcroFields;
            String name = "Signature";
            int step = 0;
            bool found = false;
            while (!found) {
                ++step;
                String n1 = name + step;
                if (af.GetFieldItem(n1) != null)
                    continue;
                n1 += ".";
                found = true;
                foreach (String fn in af.Fields.Keys) {
                    if (fn.StartsWith(n1)) {
                        found = false;
                        break;
                    }
                }
            }
            name += step;
            return name;
        }
        
        /**
        * This is the first method to be called when using external signatures. The general sequence is:
        * PreClose(), GetDocumentBytes() and Close().
        * <p>
        * If calling PreClose() <B>dont't</B> call PdfStamper.Close().
        * <p>
        * No external signatures are allowed if this methos is called.
        * @throws IOException on error
        * @throws DocumentException on error
        */    
        public void PreClose() {
            PreClose(null);
        }
        /**
        * This is the first method to be called when using external signatures. The general sequence is:
        * PreClose(), GetDocumentBytes() and Close().
        * <p>
        * If calling PreClose() <B>dont't</B> call PdfStamper.Close().
        * <p>
        * If using an external signature <CODE>exclusionSizes</CODE> must contain at least
        * the <CODE>PdfName.CONTENTS</CODE> key with the size that it will take in the
        * document. Note that due to the hex string coding this size should be
        * byte_size*2+2.
        * @param exclusionSizes a <CODE>Hashtable</CODE> with names and sizes to be excluded in the signature
        * calculation. The key is a <CODE>PdfName</CODE> and the value an
        * <CODE>Integer</CODE>. At least the <CODE>PdfName.CONTENTS</CODE> must be present
        * @throws IOException on error
        * @throws DocumentException on error
        */    
        public void PreClose(Hashtable exclusionSizes) {
            if (preClosed)
                throw new DocumentException("Document already pre closed.");
            preClosed = true;
            AcroFields af = writer.AcroFields;
            String name = FieldName;
            bool fieldExists = !(IsInvisible() || IsNewField());
            PdfIndirectReference refSig = writer.PdfIndirectReference;
            writer.SigFlags = 3;
            if (fieldExists) {
                ArrayList widgets = af.GetFieldItem(name).widgets;
                PdfDictionary widget = (PdfDictionary)widgets[0];
                writer.MarkUsed(widget);
                widget.Put(PdfName.P, writer.GetPageReference(Page));
                widget.Put(PdfName.V, refSig);
                PdfObject obj = PdfReader.GetPdfObjectRelease(widget.Get(PdfName.F));
                int flags = 0;
                if (obj != null && obj.IsNumber())
                    flags = ((PdfNumber)obj).IntValue;
                flags |= PdfAnnotation.FLAGS_LOCKED;
                widget.Put(PdfName.F, new PdfNumber(flags));
                PdfDictionary ap = new PdfDictionary();
                ap.Put(PdfName.N, GetAppearance().IndirectReference);
                widget.Put(PdfName.AP, ap);
            }
            else {
                PdfFormField sigField = PdfFormField.CreateSignature(writer);
                sigField.FieldName = name;
                sigField.Put(PdfName.V, refSig);
                sigField.Flags = PdfAnnotation.FLAGS_PRINT | PdfAnnotation.FLAGS_LOCKED;

                int pagen = Page;
                if (!IsInvisible())
                    sigField.SetWidget(PageRect, null);
                else
                    sigField.SetWidget(new Rectangle(0, 0), null);
                sigField.SetAppearance(PdfAnnotation.APPEARANCE_NORMAL, GetAppearance());
                sigField.Page = pagen;
                writer.AddAnnotation(sigField, pagen);
            }

            exclusionLocations = new Hashtable();
            if (cryptoDictionary == null) {
                if (PdfName.ADOBE_PPKLITE.Equals(Filter))
                    sigStandard = new PdfSigGenericPKCS.PPKLite();
                else if (PdfName.ADOBE_PPKMS.Equals(Filter))
                    sigStandard = new PdfSigGenericPKCS.PPKMS();
                else if (PdfName.VERISIGN_PPKVS.Equals(Filter))
                    sigStandard = new PdfSigGenericPKCS.VeriSign();
                else
                    throw new ArgumentException("Unknown filter: " + Filter);
                sigStandard.SetExternalDigest(externalDigest, externalRSAdata, digestEncryptionAlgorithm);
                if (Reason != null)
                    sigStandard.Reason = Reason;
                if (Location != null)
                    sigStandard.Location = Location;
                if (Contact != null)
                    sigStandard.Contact = Contact;
                sigStandard.Put(PdfName.M, new PdfDate(SignDate));
                sigStandard.SetSignInfo(PrivKey, CertChain, CrlList);
                PdfString contents = (PdfString)sigStandard.Get(PdfName.CONTENTS);
                PdfLiteral lit = new PdfLiteral((contents.ToString().Length + (PdfName.ADOBE_PPKLITE.Equals(Filter)?0:64)) * 2 + 2);
                exclusionLocations[PdfName.CONTENTS] = lit;
                sigStandard.Put(PdfName.CONTENTS, lit);
                lit = new PdfLiteral(80);
                exclusionLocations[PdfName.BYTERANGE] = lit;
                sigStandard.Put(PdfName.BYTERANGE, lit);
                if (certificationLevel > 0)
                    AddDocMDP(sigStandard);
                if (signatureEvent != null)
                    signatureEvent.GetSignatureDictionary(sigStandard);
                writer.AddToBody(sigStandard, refSig, false);
            }
            else {
                PdfLiteral lit = new PdfLiteral(80);
                exclusionLocations[PdfName.BYTERANGE] = lit;
                cryptoDictionary.Put(PdfName.BYTERANGE, lit);
                foreach (DictionaryEntry entry in exclusionSizes) {
                    PdfName key = (PdfName)entry.Key;
                    int v = (int)entry.Value;
                    lit = new PdfLiteral(v);
                    exclusionLocations[key] = lit;
                    cryptoDictionary.Put(key, lit);
                }
                if (certificationLevel > 0)
                    AddDocMDP(cryptoDictionary);
                if (signatureEvent != null)
                    signatureEvent.GetSignatureDictionary(cryptoDictionary);
                writer.AddToBody(cryptoDictionary, refSig, false);
            }
            if (certificationLevel > 0) {
                // add DocMDP entry to root
                PdfDictionary docmdp = new PdfDictionary();
                docmdp.Put(new PdfName("DocMDP"), refSig);
                writer.reader.Catalog.Put(new PdfName("Perms"), docmdp);
            }
            writer.Close(stamper.MoreInfo);
            
            range = new int[exclusionLocations.Count * 2];
            int byteRangePosition = ((PdfLiteral)exclusionLocations[PdfName.BYTERANGE]).Position;
            exclusionLocations.Remove(PdfName.BYTERANGE);
            int idx = 1;
            foreach (PdfLiteral lit in exclusionLocations.Values) {
                int n = lit.Position;
                range[idx++] = n;
                range[idx++] = lit.PosLength + n;
            }
            Array.Sort(range, 1, range.Length - 2);
            for (int k = 3; k < range.Length - 2; k += 2)
                range[k] -= range[k - 1];
            
            if (tempFile == null) {
                bout = sigout.Buffer;
                boutLen = sigout.Size;
                range[range.Length - 1] = boutLen - range[range.Length - 2];
                ByteBuffer bf = new ByteBuffer();
                bf.Append('[');
                for (int k = 0; k < range.Length; ++k)
                    bf.Append(range[k]).Append(' ');
                bf.Append(']');
                Array.Copy(bf.Buffer, 0, bout, byteRangePosition, bf.Size);
            }
            else {
                try {
                    raf = new FileStream(tempFile, FileMode.Open, FileAccess.ReadWrite);
                    int boutLen = (int)raf.Length;
                    range[range.Length - 1] = boutLen - range[range.Length - 2];
                    ByteBuffer bf = new ByteBuffer();
                    bf.Append('[');
                    for (int k = 0; k < range.Length; ++k)
                        bf.Append(range[k]).Append(' ');
                    bf.Append(']');
                    raf.Seek(byteRangePosition, SeekOrigin.Begin);
                    raf.Write(bf.Buffer, 0, bf.Size);
                }
                catch (IOException e) {
                    try{raf.Close();}catch{}
                    try{File.Delete(tempFile);}catch{}
                    throw e;
                }
            }
        }
        
        /**
        * This is the last method to be called when using external signatures. The general sequence is:
        * PreClose(), GetDocumentBytes() and Close().
        * <p>
        * <CODE>update</CODE> is a <CODE>PdfDictionary</CODE> that must have exactly the
        * same keys as the ones provided in {@link #preClose(Hashtable)}.
        * @param update a <CODE>PdfDictionary</CODE> with the key/value that will fill the holes defined
        * in {@link #preClose(Hashtable)}
        * @throws DocumentException on error
        * @throws IOException on error
        */    
        public void Close(PdfDictionary update) {
            try {
                if (!preClosed)
                    throw new DocumentException("preClose() must be called first.");
                ByteBuffer bf = new ByteBuffer();
                foreach (PdfName key in update.Keys) {
                    PdfObject obj = update.Get(key);
                    PdfLiteral lit = (PdfLiteral)exclusionLocations[key];
                    if (lit == null)
                        throw new ArgumentException("The key " + key.ToString() + " didn't reserve space in PreClose().");
                    bf.Reset();
                    obj.ToPdf(null, bf);
                    if (bf.Size > lit.PosLength)
                        throw new ArgumentException("The key " + key.ToString() + " is too big. Is " + bf.Size + ", reserved " + lit.PosLength);
                    if (tempFile == null)
                        Array.Copy(bf.Buffer, 0, bout, lit.Position, bf.Size);
                    else {
                        raf.Seek(lit.Position, SeekOrigin.Begin);
                        raf.Write(bf.Buffer, 0, bf.Size);
                    }
                }
                if (update.Size != exclusionLocations.Count)
                    throw new ArgumentException("The update dictionary has less keys than required.");
                if (tempFile == null) {
                    originalout.Write(bout, 0, boutLen);
                }
                else {
                    if (originalout != null) {
                        raf.Seek(0, SeekOrigin.Begin);
                        int length = (int)raf.Length;
                        byte[] buf = new byte[8192];
                        while (length > 0) {
                            int r = raf.Read(buf, 0, Math.Min(buf.Length, length));
                            if (r < 0)
                                throw new EndOfStreamException("Unexpected EOF");
                            originalout.Write(buf, 0, r);
                            length -= r;
                        }
                    }
                }
            }
            finally {
                if (tempFile != null) {
                    try{raf.Close();}catch{}
                    if (originalout != null)
                        try{File.Delete(tempFile);}catch{}
                }
                if (originalout != null)
                    try{originalout.Close();}catch{}
            }
        }
        
        private void AddDocMDP(PdfDictionary crypto) {
            PdfDictionary reference = new PdfDictionary();
            PdfDictionary transformParams = new PdfDictionary();
            transformParams.Put(PdfName.P, new PdfNumber(certificationLevel));
            transformParams.Put(PdfName.V, new PdfName("1.2"));
            transformParams.Put(PdfName.TYPE, PdfName.TRANSFORMPARAMS);
            reference.Put(PdfName.TRANSFORMMETHOD, PdfName.DOCMDP);
            reference.Put(PdfName.TYPE, PdfName.SIGREF);
            reference.Put(PdfName.TRANSFORMPARAMS, transformParams);
            reference.Put(new PdfName("DigestValue"), new PdfString("aa"));
            PdfArray loc = new PdfArray();
            loc.Add(new PdfNumber(0));
            loc.Add(new PdfNumber(0));
            reference.Put(new PdfName("DigestLocation"), loc);
            reference.Put(new PdfName("DigestMethod"), new PdfName("MD5"));
            reference.Put(PdfName.DATA, writer.reader.Trailer.Get(PdfName.ROOT));
            PdfArray types = new PdfArray();
            types.Add(reference);
            crypto.Put(PdfName.REFERENCE, types);
        }

        /**
        * Gets the document bytes that are hashable when using external signatures. The general sequence is:
        * PreClose(), GetRangeStream() and Close().
        * <p>
        * @return the document bytes that are hashable
        */    
        public Stream RangeStream {
            get {
                return new PdfSignatureAppearance.FRangeStream(raf, bout, range);
            }
        }
        
        /**
        * Gets the user made signature dictionary. This is the dictionary at the /V key.
        * @return the user made signature dictionary
        */    
        public PdfDictionary CryptoDictionary {
            get {
                return cryptoDictionary;
            }
            set {
                cryptoDictionary = value;
            }
        }
        
        /**
        * Gets the <CODE>PdfStamper</CODE> associated with this instance.
        * @return the <CODE>PdfStamper</CODE> associated with this instance
        */    
        public PdfStamper Stamper {
            get {
                return stamper;
            }
        }
        
        internal void SetStamper(PdfStamper stamper) {
            this.stamper = stamper;
        }
        
        /**
        * Checks if the document is in the process of closing.
        * @return <CODE>true</CODE> if the document is in the process of closing,
        * <CODE>false</CODE> otherwise
        */    
        public bool IsPreClosed() {
            return preClosed;
        }
        
        /**
        * Gets the instance of the standard signature dictionary. This instance
        * is only available after pre close.
        * <p>
        * The main use is to insert external signatures.
        * @return the instance of the standard signature dictionary
        */    
        public PdfSigGenericPKCS SigStandard {
            get {
                return sigStandard;
            }
        }
        
        /**
        * Sets the signing contact.
        * @param contact the signing contact
        */
        public string Contact {
            get {
                return contact;
            }
            set {
                contact = value;
            }
        }
        
        /**
        * Sets the n2 and n4 layer font. If the font size is zero, auto-fit will be used.
        * @param layer2Font the n2 and n4 font
        */
        public Font Layer2Font {
            get {
                return layer2Font;
            }
            set {
                layer2Font = value;
            }
        }
        
        /**
        * Acrobat 6.0 and higher recomends that only layer n2 and n4 be present. This method sets that mode.
        * @param acro6Layers if <code>true</code> only the layers n2 and n4 will be present
        */
        public bool Acro6Layers {
            get {
                return acro6Layers;
            }
            set {
                acro6Layers = value;
            }
        }
        
        /** Sets the run direction in the n2 and n4 layer. 
        * @param runDirection the run direction
        */    
        public int RunDirection {
            set {
                if (value < PdfWriter.RUN_DIRECTION_DEFAULT || value > PdfWriter.RUN_DIRECTION_RTL)
                    throw new ArgumentException("Invalid run direction: " + runDirection);
                this.runDirection = value;
            }
            get {
                return runDirection;
            }
        }
        
        /**
        * Sets the signature event to allow modification of the signature dictionary.
        * @param signatureEvent the signature event
        */
        public ISignatureEvent SignatureEvent {
            get {
                return signatureEvent;
            }
            set {
                signatureEvent = value;
            }
        }
        /**
        * Gets the background image for the layer 2.
        * @return the background image for the layer 2
        */
        public Image GetImage() {
            return this.image;
        }
        
        /**
        * Sets the background image for the layer 2.
        * @param image the background image for the layer 2
        */
        public Image Image {
            get {
                return image;
            }
            set {
                image = value;
            }
        }
        
        /**
        * Sets the scaling to be applied to the background image. If it's zero the image
        * will fully fill the rectangle. If it's less than zero the image will fill the rectangle but
        * will keep the proportions. If it's greater than zero that scaling will be applied.
        * In any of the cases the image will always be centered. It's zero by default.
        * @param imageScale the scaling to be applied to the background image
        */
        public float ImageScale {
            get {
                return imageScale;
            }
            set {
                imageScale = value;
            }
        }

        /**
        * Commands to draw a yellow question mark in a stream content
        */    
        public const String questionMark = 
            "% DSUnknown\n" +
            "q\n" +
            "1 G\n" +
            "1 g\n" +
            "0.1 0 0 0.1 9 0 cm\n" +
            "0 J 0 j 4 M []0 d\n" +
            "1 i \n" +
            "0 g\n" +
            "313 292 m\n" +
            "313 404 325 453 432 529 c\n" +
            "478 561 504 597 504 645 c\n" +
            "504 736 440 760 391 760 c\n" +
            "286 760 271 681 265 626 c\n" +
            "265 625 l\n" +
            "100 625 l\n" +
            "100 828 253 898 381 898 c\n" +
            "451 898 679 878 679 650 c\n" +
            "679 555 628 499 538 435 c\n" +
            "488 399 467 376 467 292 c\n" +
            "313 292 l\n" +
            "h\n" +
            "308 214 170 -164 re\n" +
            "f\n" +
            "0.44 G\n" +
            "1.2 w\n" +
            "1 1 0.4 rg\n" +
            "287 318 m\n" +
            "287 430 299 479 406 555 c\n" +
            "451 587 478 623 478 671 c\n" +
            "478 762 414 786 365 786 c\n" +
            "260 786 245 707 239 652 c\n" +
            "239 651 l\n" +
            "74 651 l\n" +
            "74 854 227 924 355 924 c\n" +
            "425 924 653 904 653 676 c\n" +
            "653 581 602 525 512 461 c\n" +
            "462 425 441 402 441 318 c\n" +
            "287 318 l\n" +
            "h\n" +
            "282 240 170 -164 re\n" +
            "B\n" +
            "Q\n";
        
        /**
        * Holds value of property contact.
        */
        private String contact;
        
        /**
        * Holds value of property layer2Font.
        */
        private Font layer2Font;
        
        /**
        * Holds value of property layer4Text.
        */
        private String layer4Text;
        
        /**
        * Holds value of property acro6Layers.
        */
        private bool acro6Layers;
        
        /**
        * Holds value of property runDirection.
        */
        private int runDirection = PdfWriter.RUN_DIRECTION_NO_BIDI;
        
        /**
        * Holds value of property signatureEvent.
        */
        private ISignatureEvent signatureEvent;
        
        /**
        * Holds value of property image.
        */
        private Image image;
        
        /**
        * Holds value of property imageScale.
        */
        private float imageScale;
        
        /**
        *
        */    
        public class FRangeStream : Stream {
            private byte[] b = new byte[1];
            private FileStream raf;
            private byte[] bout;
            private int[] range;
            private int rangePosition = 0;
            
            internal FRangeStream(FileStream raf, byte[] bout, int[] range) {
                this.raf = raf;
                this.bout = bout;
                this.range = range;
            }
            
            /**
            * @see java.io.Stream#read()
            */
            public override int ReadByte() {
                int n = Read(b, 0, 1);
                if (n != 1)
                    return -1;
                return b[0] & 0xff;
            }
            
            /**
            * @see java.io.Stream#read(byte[], int, int)
            */
            public override int Read(byte[] b, int off, int len) {
                if (b == null) {
                    throw new ArgumentNullException();
                } else if ((off < 0) || (off > b.Length) || (len < 0) ||
                ((off + len) > b.Length) || ((off + len) < 0)) {
                    throw new ArgumentOutOfRangeException();
                } else if (len == 0) {
                    return 0;
                }
                if (rangePosition >= range[range.Length - 2] + range[range.Length - 1]) {
                    return -1;
                }
                for (int k = 0; k < range.Length; k += 2) {
                    int start = range[k];
                    int end = start + range[k + 1];
                    if (rangePosition < start)
                        rangePosition = start;
                    if (rangePosition >= start && rangePosition < end) {
                        int lenf = Math.Min(len, end - rangePosition);
                        if (raf == null)
                            Array.Copy(bout, rangePosition, b, off, lenf);
                        else {
                            raf.Seek(rangePosition, SeekOrigin.Begin);
                            ReadFully(b, off, lenf);
                        }
                        rangePosition += lenf;
                        return lenf;
                    }
                }
                return -1;
            }
        
            private void ReadFully(byte[] b, int offset, int count) {
                while (count > 0) {
                    int n = raf.Read(b, offset, count);
                    if (n <= 0)
                        throw new IOException("Insufficient data.");
                    count -= n;
                    offset += n;
                }
            }

            public override bool CanRead {
                get {
                    return true;
                }
            }
        
            public override bool CanSeek {
                get {
                    return false;
                }
            }
        
            public override bool CanWrite {
                get {
                    return false;
                }
            }
        
            public override long Length {
                get {
                    return 0;
                }
            }
        
            public override long Position {
                get {
                    return 0;
                }
                set {
                }
            }
        
            public override void Flush() {
            }
        
            public override long Seek(long offset, SeekOrigin origin) {
                return 0;
            }
        
            public override void SetLength(long value) {
            }
        
            public override void Write(byte[] buffer, int offset, int count) {
            }
        
            public override void WriteByte(byte value) {
            }
        }
        
        /**
        * An interface to retrieve the signature dictionary for modification.
        */    
        public interface ISignatureEvent {
            /**
            * Allows modification of the signature dictionary.
            * @param sig the signature dictionary
            */        
            void GetSignatureDictionary(PdfDictionary sig);
        }

        private int certificationLevel = NOT_CERTIFIED;

        /**
        * Sets the document type to certified instead of simply signed.
        * @param certificationLevel the values can be: <code>NOT_CERTIFIED</code>, <code>CERTIFIED_NO_CHANGES_ALLOWED</code>,
        * <code>CERTIFIED_FORM_FILLING</code> and <code>CERTIFIED_FORM_FILLING_AND_ANNOTATIONS</code>
        */
        public int CertificationLevel {
            get {
                return certificationLevel;
            }
            set {
                certificationLevel = value;
            }
        }
    }
}