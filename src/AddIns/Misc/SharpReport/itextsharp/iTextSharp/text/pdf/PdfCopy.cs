using System;
using System.Collections;
using System.IO;
using iTextSharp.text;
/*
 * $Id: PdfCopy.cs,v 1.24 2008/05/13 11:25:19 psoares33 Exp $
 * 
 *
 * Copyright 1999, 2000, 2001, 2002 Bruno Lowagie
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
 * This module by Mark Thompson. Copyright (C) 2002 Mark Thompson
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
    * Make copies of PDF documents. Documents can be edited after reading and
    * before writing them out.
    * @author Mark Thompson
    */

    public class PdfCopy : PdfWriter {
        /**
        * This class holds information about indirect references, since they are
        * renumbered by iText.
        */
        internal class IndirectReferences {
            PdfIndirectReference theRef;
            bool hasCopied;
            internal IndirectReferences(PdfIndirectReference refi) {
                theRef = refi;
                hasCopied = false;
            }
            internal void SetCopied() { hasCopied = true; }
            internal bool Copied {
                get {
                    return hasCopied; 
                }
            }
            internal PdfIndirectReference Ref {
                get {
                    return theRef; 
                }
            }
        };
        protected Hashtable indirects;
        protected Hashtable indirectMap;
        protected int currentObjectNum = 1;
        protected PdfReader reader;
        protected PdfIndirectReference acroForm;
        protected int[] namePtr = {0};
        /** Holds value of property rotateContents. */
        private bool rotateContents = true;
        protected internal PdfArray fieldArray;
        protected internal Hashtable fieldTemplates;

        /**
        * A key to allow us to hash indirect references
        */
        protected class RefKey {
            internal int num;
            internal int gen;
            internal RefKey(int num, int gen) {
                this.num = num;
                this.gen = gen;
            }
            internal RefKey(PdfIndirectReference refi) {
                num = refi.Number;
                gen = refi.Generation;
            }
            internal RefKey(PRIndirectReference refi) {
                num = refi.Number;
                gen = refi.Generation;
            }
            public override int GetHashCode() {
                return (gen<<16)+num;
            }
            public override bool Equals(Object o) {
                if (!(o is RefKey)) return false;
                RefKey other = (RefKey)o;
                return this.gen == other.gen && this.num == other.num;
            }
            public override String ToString() {
                return "" + num + " " + gen;
            }
        }
        
        /**
        * Constructor
        * @param document
        * @param os outputstream
        */
        public PdfCopy(Document document, Stream os) : base(new PdfDocument(), os) {
            document.AddDocListener(pdf);
            pdf.AddWriter(this);
            indirectMap = new Hashtable();
        }

        /** Checks if the content is automatically adjusted to compensate
        * the original page rotation.
        * @return the auto-rotation status
        */    
        /** Flags the content to be automatically adjusted to compensate
        * the original page rotation. The default is <CODE>true</CODE>.
        * @param rotateContents <CODE>true</CODE> to set auto-rotation, <CODE>false</CODE>
        * otherwise
        */    
        public bool RotateContents {
            set {
                rotateContents = value;
            }
            get {
                return rotateContents;
            }
        }

        /**
        * Grabs a page from the input document
        * @param reader the reader of the document
        * @param pageNumber which page to get
        * @return the page
        */
        public override PdfImportedPage GetImportedPage(PdfReader reader, int pageNumber) {
            if (currentPdfReaderInstance != null) {
                if (currentPdfReaderInstance.Reader != reader) {
                    try {
                        currentPdfReaderInstance.Reader.Close();
                        currentPdfReaderInstance.ReaderFile.Close();
                    }
                    catch (IOException) {
                        // empty on purpose
                    }
                    currentPdfReaderInstance = reader.GetPdfReaderInstance(this);
                }
            }
            else {
                currentPdfReaderInstance = reader.GetPdfReaderInstance(this);
            }
            return currentPdfReaderInstance.GetImportedPage(pageNumber);            
        }
        
        
        /**
        * Translate a PRIndirectReference to a PdfIndirectReference
        * In addition, translates the object numbers, and copies the
        * referenced object to the output file.
        * NB: PRIndirectReferences (and PRIndirectObjects) really need to know what
        * file they came from, because each file has its own namespace. The translation
        * we do from their namespace to ours is *at best* heuristic, and guaranteed to
        * fail under some circumstances.
        */
        protected virtual PdfIndirectReference CopyIndirect(PRIndirectReference inp) {
            PdfIndirectReference theRef;
            RefKey key = new RefKey(inp);
            IndirectReferences iRef = (IndirectReferences)indirects[key] ;
            if (iRef != null) {
                theRef = iRef.Ref;
                if (iRef.Copied) {
                    return theRef;
                }
            }
            else {
                theRef = body.PdfIndirectReference;
                iRef = new IndirectReferences(theRef);
                indirects[key] =  iRef;
            }
            PdfObject obj = PdfReader.GetPdfObjectRelease(inp);
            if (obj != null && obj.IsDictionary()) {
                PdfObject type = PdfReader.GetPdfObjectRelease(((PdfDictionary)obj).Get(PdfName.TYPE));
                if (type != null && PdfName.PAGE.Equals(type)) {
                    return theRef;
                }
            }
            iRef.SetCopied();
            obj = CopyObject(obj);
            AddToBody(obj, theRef);
            return theRef;
        }
        
        /**
        * Translate a PRDictionary to a PdfDictionary. Also translate all of the
        * objects contained in it.
        */
        protected PdfDictionary CopyDictionary(PdfDictionary inp) {
            PdfDictionary outp = new PdfDictionary();
            PdfObject type = PdfReader.GetPdfObjectRelease(inp.Get(PdfName.TYPE));
            
            foreach (PdfName key in inp.Keys) {
                PdfObject value = inp.Get(key);
                if (type != null && PdfName.PAGE.Equals(type)) {
                    if (!key.Equals(PdfName.B) && !key.Equals(PdfName.PARENT))
                        outp.Put(key, CopyObject(value));
                }
                else
                    outp.Put(key, CopyObject(value));
            }
            return outp;
        }
        
        /**
        * Translate a PRStream to a PdfStream. The data part copies itself.
        */
        protected PdfStream CopyStream(PRStream inp) {
            PRStream outp = new PRStream(inp, null);
            
            foreach (PdfName key in inp.Keys) {
                PdfObject value = inp.Get(key);
                outp.Put(key, CopyObject(value));
            }
            
            return outp;
        }
        
        
        /**
        * Translate a PRArray to a PdfArray. Also translate all of the objects contained
        * in it
        */
        protected PdfArray CopyArray(PdfArray inp) {
            PdfArray outp = new PdfArray();
            
            foreach (PdfObject value in inp.ArrayList) {
                outp.Add(CopyObject(value));
            }
            return outp;
        }
        
        /**
        * Translate a PR-object to a Pdf-object
        */
        protected PdfObject CopyObject(PdfObject inp) {
            if (inp == null)
                return PdfNull.PDFNULL;
            switch (inp.Type) {
                case PdfObject.DICTIONARY:
                    return CopyDictionary((PdfDictionary)inp);
                case PdfObject.INDIRECT:
                    return CopyIndirect((PRIndirectReference)inp);
                case PdfObject.ARRAY:
                    return CopyArray((PdfArray)inp);
                case PdfObject.NUMBER:
                case PdfObject.NAME:
                case PdfObject.STRING:
                case PdfObject.NULL:
                case PdfObject.BOOLEAN:
                case 0:
                    return inp;
                case PdfObject.STREAM:
                    return CopyStream((PRStream)inp);
                    //                return in;
                default:
                    if (inp.Type < 0) {
                        String lit = ((PdfLiteral)inp).ToString();
                        if (lit.Equals("true") || lit.Equals("false")) {
                            return new PdfBoolean(lit);
                        }
                        return new PdfLiteral(lit);
                    }
                    return null;
            }
        }
        
        /**
        * convenience method. Given an importedpage, set our "globals"
        */
        protected int SetFromIPage(PdfImportedPage iPage) {
            int pageNum = iPage.PageNumber;
            PdfReaderInstance inst = currentPdfReaderInstance = iPage.PdfReaderInstance;
            reader = inst.Reader;
            SetFromReader(reader);
            return pageNum;
        }
        
        /**
        * convenience method. Given a reader, set our "globals"
        */
        protected void SetFromReader(PdfReader reader) {
            this.reader = reader;
            indirects = (Hashtable)indirectMap[reader] ;
            if (indirects == null) {
                indirects = new Hashtable();
                indirectMap[reader] = indirects;
                PdfDictionary catalog = reader.Catalog;
                PRIndirectReference refi = null;
                PdfObject o = catalog.Get(PdfName.ACROFORM);
                if (o == null || o.Type != PdfObject.INDIRECT)
                    return;
                refi = (PRIndirectReference)o;
                if (acroForm == null) acroForm = body.PdfIndirectReference;
                indirects[new RefKey(refi)] =  new IndirectReferences(acroForm);
            }
        }
        /**
        * Add an imported page to our output
        * @param iPage an imported page
        * @throws IOException, BadPdfFormatException
        */
        public void AddPage(PdfImportedPage iPage) {
            int pageNum = SetFromIPage(iPage);
            
            PdfDictionary thePage = reader.GetPageN(pageNum);
            PRIndirectReference origRef = reader.GetPageOrigRef(pageNum);
            reader.ReleasePage(pageNum);
            RefKey key = new RefKey(origRef);
            PdfIndirectReference pageRef;
            IndirectReferences iRef = (IndirectReferences)indirects[key] ;
            if (iRef != null && !iRef.Copied) {
                pageReferences.Add(iRef.Ref);
                iRef.SetCopied();
            }
            pageRef = CurrentPage;
            if (iRef == null) {
                iRef = new IndirectReferences(pageRef);
                indirects[key] = iRef;
            }
            iRef.SetCopied();
            PdfDictionary newPage = CopyDictionary(thePage);
            root.AddPage(newPage);
            ++currentPageNumber;
        }
        
        /**
        * Copy the acroform for an input document. Note that you can only have one,
        * we make no effort to merge them.
        * @param reader The reader of the input file that is being copied
        * @throws IOException, BadPdfFormatException
        */
        public void CopyAcroForm(PdfReader reader) {
            SetFromReader(reader);
            
            PdfDictionary catalog = reader.Catalog;
            PRIndirectReference hisRef = null;
            PdfObject o = catalog.Get(PdfName.ACROFORM);
            if (o != null && o.Type == PdfObject.INDIRECT)
                hisRef = (PRIndirectReference)o;
            if (hisRef == null) return; // bugfix by John Engla
            RefKey key = new RefKey(hisRef);
            PdfIndirectReference myRef;
            IndirectReferences iRef = (IndirectReferences)indirects[key] ;
            if (iRef != null) {
                acroForm = myRef = iRef.Ref;
            }
            else {
                acroForm = myRef = body.PdfIndirectReference;
                iRef = new IndirectReferences(myRef);
                indirects[key] =  iRef;
            }
            if (! iRef.Copied) {
                iRef.SetCopied();
                PdfDictionary theForm = CopyDictionary((PdfDictionary)PdfReader.GetPdfObject(hisRef));
                AddToBody(theForm, myRef);
            }
        }
        
        /*
        * the getCatalog method is part of PdfWriter.
        * we wrap this so that we can extend it
        */
        protected override PdfDictionary GetCatalog(PdfIndirectReference rootObj) {
            PdfDictionary theCat = pdf.GetCatalog(rootObj);
            if (fieldArray == null) {
                if (acroForm != null) theCat.Put(PdfName.ACROFORM, acroForm);
            }
            else
                AddFieldResources(theCat);
            WriteOutlines(theCat, false);
            return theCat;
        }
        
        private void AddFieldResources(PdfDictionary catalog) {
            if (fieldArray == null)
                return;
            PdfDictionary acroForm = new PdfDictionary();
            catalog.Put(PdfName.ACROFORM, acroForm);
            acroForm.Put(PdfName.FIELDS, fieldArray);
            acroForm.Put(PdfName.DA, new PdfString("/Helv 0 Tf 0 g "));
            if (fieldTemplates.Count == 0)
                return;
            PdfDictionary dr = new PdfDictionary();
            acroForm.Put(PdfName.DR, dr);
            foreach (PdfTemplate template in fieldTemplates.Keys) {
                PdfFormField.MergeResources(dr, (PdfDictionary)template.Resources);
            }
            if (dr.Get(PdfName.ENCODING) == null)
                dr.Put(PdfName.ENCODING, PdfName.WIN_ANSI_ENCODING);
            PdfDictionary fonts = (PdfDictionary)PdfReader.GetPdfObject(dr.Get(PdfName.FONT));
            if (fonts == null) {
                fonts = new PdfDictionary();
                dr.Put(PdfName.FONT, fonts);
            }
            if (!fonts.Contains(PdfName.HELV)) {
                PdfDictionary dic = new PdfDictionary(PdfName.FONT);
                dic.Put(PdfName.BASEFONT, PdfName.HELVETICA);
                dic.Put(PdfName.ENCODING, PdfName.WIN_ANSI_ENCODING);
                dic.Put(PdfName.NAME, PdfName.HELV);
                dic.Put(PdfName.SUBTYPE, PdfName.TYPE1);
                fonts.Put(PdfName.HELV, AddToBody(dic).IndirectReference);
            }
            if (!fonts.Contains(PdfName.ZADB)) {
                PdfDictionary dic = new PdfDictionary(PdfName.FONT);
                dic.Put(PdfName.BASEFONT, PdfName.ZAPFDINGBATS);
                dic.Put(PdfName.NAME, PdfName.ZADB);
                dic.Put(PdfName.SUBTYPE, PdfName.TYPE1);
                fonts.Put(PdfName.ZADB, AddToBody(dic).IndirectReference);
            }
        }
        
        /**
        * Signals that the <CODE>Document</CODE> was closed and that no other
        * <CODE>Elements</CODE> will be added.
        * <P>
        * The pages-tree is built and written to the outputstream.
        * A Catalog is constructed, as well as an Info-object,
        * the referencetable is composed and everything is written
        * to the outputstream embedded in a Trailer.
        */
        
        public override void Close() {
            if (open) {
                PdfReaderInstance ri = currentPdfReaderInstance;
                pdf.Close();
                base.Close();
                if (ri != null) {
                    try {
                        ri.Reader.Close();
                        ri.ReaderFile.Close();
                    }
                    catch (IOException) {
                        // empty on purpose
                    }
                }
            }
        }

        public override void AddAnnotation(PdfAnnotation annot) {  }
        internal override PdfIndirectReference Add(PdfPage page, PdfContents contents) { return null; }

        public override void FreeReader(PdfReader reader) {
            indirectMap.Remove(reader);
            if (currentPdfReaderInstance != null) {
                if (currentPdfReaderInstance.Reader == reader) {
                    try {
                        currentPdfReaderInstance.Reader.Close();
                        currentPdfReaderInstance.ReaderFile.Close();
                    }
                    catch (IOException) {
                        // empty on purpose
                    }
                    currentPdfReaderInstance = null;
                }
            }
        }

        /**
        * Create a page stamp. New content and annotations, including new fields, are allowed.
        * The fields added cannot have parents in another pages. This method modifies the PdfReader instance.<p>
        * The general usage to stamp something in a page is:
        * <p>
        * <pre>
        * PdfImportedPage page = copy.getImportedPage(reader, 1);
        * PdfCopy.PageStamp ps = copy.createPageStamp(page);
        * ps.addAnnotation(PdfAnnotation.createText(copy, new Rectangle(50, 180, 70, 200), "Hello", "No Thanks", true, "Comment"));
        * PdfContentByte under = ps.getUnderContent();
        * under.addImage(img);
        * PdfContentByte over = ps.getOverContent();
        * over.beginText();
        * over.setFontAndSize(bf, 18);
        * over.setTextMatrix(30, 30);
        * over.showText("total page " + totalPage);
        * over.endText();
        * ps.alterContents();
        * copy.addPage(page);
        * </pre>
        * @param iPage an imported page
        * @return the <CODE>PageStamp</CODE>
        */
        public PageStamp CreatePageStamp(PdfImportedPage iPage) {
            int pageNum = iPage.PageNumber;
            PdfReader reader = iPage.PdfReaderInstance.Reader;
            PdfDictionary pageN = reader.GetPageN(pageNum);
            return new PageStamp(reader, pageN, this);
        }

        public class PageStamp {
            
            PdfDictionary pageN;
            PdfCopy.StampContent under;
            PdfCopy.StampContent over;
            PageResources pageResources;
            PdfReader reader;
            PdfCopy cstp;
            
            internal PageStamp(PdfReader reader, PdfDictionary pageN, PdfCopy cstp) {
                this.pageN = pageN;
                this.reader = reader;
                this.cstp = cstp;
            }
            
            public PdfContentByte GetUnderContent(){
                if (under == null) {
                    if (pageResources == null) {
                        pageResources = new PageResources();
                        PdfDictionary resources = (PdfDictionary)PdfReader.GetPdfObject(pageN.Get(PdfName.RESOURCES));
                        pageResources.SetOriginalResources(resources, cstp.namePtr);
                    }
                    under = new PdfCopy.StampContent(cstp, pageResources);
                }
                return under;
            }
            
            public PdfContentByte GetOverContent(){
                if (over == null) {
                    if (pageResources == null) {
                        pageResources = new PageResources();
                        PdfDictionary resources = (PdfDictionary)PdfReader.GetPdfObject(pageN.Get(PdfName.RESOURCES));
                        pageResources.SetOriginalResources(resources, cstp.namePtr);
                    }
                    over = new PdfCopy.StampContent(cstp, pageResources);
                }
                return over;
            }

            public void AlterContents() {
                if (over == null && under == null)
                    return;
                PdfArray ar = null;
                PdfObject content = PdfReader.GetPdfObject(pageN.Get(PdfName.CONTENTS), pageN);
                if (content == null) {
                    ar = new PdfArray();
                    pageN.Put(PdfName.CONTENTS, ar);
                }
                else if (content.IsArray()) {
                    ar = (PdfArray)content;
                }
                else if (content.IsStream()) {
                    ar = new PdfArray();
                    ar.Add(pageN.Get(PdfName.CONTENTS));
                    pageN.Put(PdfName.CONTENTS, ar);
                }
                else {
                    ar = new PdfArray();
                    pageN.Put(PdfName.CONTENTS, ar);
                }
                ByteBuffer out_p = new ByteBuffer();
                if (under != null) {
                    out_p.Append(PdfContents.SAVESTATE);
                    ApplyRotation(pageN, out_p);
                    out_p.Append(under.InternalBuffer);
                    out_p.Append(PdfContents.RESTORESTATE);
                }
                if (over != null)
                    out_p.Append(PdfContents.SAVESTATE);
                PdfStream stream = new PdfStream(out_p.ToByteArray());
                stream.FlateCompress();
                PdfIndirectReference ref1 = cstp.AddToBody(stream).IndirectReference;
                ar.AddFirst(ref1);
                out_p.Reset();
                if (over != null) {
                    out_p.Append(' ');
                    out_p.Append(PdfContents.RESTORESTATE);
                    out_p.Append(PdfContents.SAVESTATE);
                    ApplyRotation(pageN, out_p);
                    out_p.Append(over.InternalBuffer);
                    out_p.Append(PdfContents.RESTORESTATE);
                    stream = new PdfStream(out_p.ToByteArray());
                    stream.FlateCompress();
                    ar.Add(cstp.AddToBody(stream).IndirectReference);
                }
                pageN.Put(PdfName.RESOURCES, pageResources.Resources);
            }

            void ApplyRotation(PdfDictionary pageN, ByteBuffer out_p) {
                if (!cstp.rotateContents)
                    return;
                Rectangle page = reader.GetPageSizeWithRotation(pageN);
                int rotation = page.Rotation;
                switch (rotation) {
                    case 90:
                        out_p.Append(PdfContents.ROTATE90);
                        out_p.Append(page.Top);
                        out_p.Append(' ').Append('0').Append(PdfContents.ROTATEFINAL);
                        break;
                    case 180:
                        out_p.Append(PdfContents.ROTATE180);
                        out_p.Append(page.Right);
                        out_p.Append(' ');
                        out_p.Append(page.Top);
                        out_p.Append(PdfContents.ROTATEFINAL);
                        break;
                    case 270:
                        out_p.Append(PdfContents.ROTATE270);
                        out_p.Append('0').Append(' ');
                        out_p.Append(page.Right);
                        out_p.Append(PdfContents.ROTATEFINAL);
                        break;
                }
            }

            private void AddDocumentField(PdfIndirectReference refi) {
                if (cstp.fieldArray == null)
                    cstp.fieldArray = new PdfArray();
                cstp.fieldArray.Add(refi);
            }

            private void ExpandFields(PdfFormField field, ArrayList allAnnots) {
                allAnnots.Add(field);
                ArrayList kids = field.Kids;
                if (kids != null) {
                    for (int k = 0; k < kids.Count; ++k)
                        ExpandFields((PdfFormField)kids[k], allAnnots);
                }
            }

            public void AddAnnotation(PdfAnnotation annot) {
                ArrayList allAnnots = new ArrayList();
                if (annot.IsForm()) {
                    PdfFormField field = (PdfFormField)annot;
                    if (field.Parent != null)
                        return;
                    ExpandFields(field, allAnnots);
                    if (cstp.fieldTemplates == null)
                        cstp.fieldTemplates = new Hashtable();
                }
                else
                    allAnnots.Add(annot);
                for (int k = 0; k < allAnnots.Count; ++k) {
                    annot = (PdfAnnotation)allAnnots[k];
                    if (annot.IsForm()) {
                        if (!annot.IsUsed()) {
                            Hashtable templates = annot.Templates;
                            if (templates != null) {
                                foreach (object tpl in templates.Keys) {
                                    cstp.fieldTemplates[tpl] = null;
                                }
                            }
                        }
                        PdfFormField field = (PdfFormField)annot;
                        if (field.Parent == null)
                            AddDocumentField(field.IndirectReference);
                    }
                    if (annot.IsAnnotation()) {
                        PdfObject pdfobj = PdfReader.GetPdfObject(pageN.Get(PdfName.ANNOTS), pageN);
                        PdfArray annots = null;
                        if (pdfobj == null || !pdfobj.IsArray()) {
                            annots = new PdfArray();
                            pageN.Put(PdfName.ANNOTS, annots);
                        }
                        else 
                            annots = (PdfArray)pdfobj;
                        annots.Add(annot.IndirectReference);
                        if (!annot.IsUsed()) {
                            PdfRectangle rect = (PdfRectangle)annot.Get(PdfName.RECT);
                            if (rect != null && (rect.Left != 0 || rect.Right != 0 || rect.Top != 0 || rect.Bottom != 0)) {
                                int rotation = reader.GetPageRotation(pageN);
                                Rectangle pageSize = reader.GetPageSizeWithRotation(pageN);
                                switch (rotation) {
                                    case 90:
                                        annot.Put(PdfName.RECT, new PdfRectangle(
                                            pageSize.Top - rect.Bottom,
                                            rect.Left,
                                            pageSize.Top - rect.Top,
                                            rect.Right));
                                        break;
                                    case 180:
                                        annot.Put(PdfName.RECT, new PdfRectangle(
                                            pageSize.Right - rect.Left,
                                            pageSize.Top - rect.Bottom,
                                            pageSize.Right - rect.Right,
                                            pageSize.Top - rect.Top));
                                        break;
                                    case 270:
                                        annot.Put(PdfName.RECT, new PdfRectangle(
                                            rect.Bottom,
                                            pageSize.Right - rect.Left,
                                            rect.Top,
                                            pageSize.Right - rect.Right));
                                        break;
                                }
                            }
                        }
                    }
                    if (!annot.IsUsed()) {
                        annot.SetUsed();
                        cstp.AddToBody(annot, annot.IndirectReference);
                    }
                }
            }
        }

        public class StampContent : PdfContentByte {
            PageResources pageResources;
            
            /** Creates a new instance of StampContent */
            internal StampContent(PdfWriter writer, PageResources pageResources) : base(writer) {
                this.pageResources = pageResources;
            }

            /**
            * Gets a duplicate of this <CODE>PdfContentByte</CODE>. All
            * the members are copied by reference but the buffer stays different.
            *
            * @return a copy of this <CODE>PdfContentByte</CODE>
            */
            public override PdfContentByte Duplicate {
                get {
                    return new PdfCopy.StampContent(writer, pageResources);
                }
            }

            internal override PageResources PageResources {
                get {
                    return pageResources;
                }
            }
        }
    }
}
