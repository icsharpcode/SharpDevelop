using System;
using System.IO;
using System.Text;
using System.Collections;
using System.util.collections;
using System.util;
using iTextSharp.text;
using iTextSharp.text.pdf.events;
using iTextSharp.text.pdf.interfaces;
using iTextSharp.text.pdf.intern;
using iTextSharp.text.pdf.collection;
using iTextSharp.text.xml.xmp;
using Org.BouncyCastle.X509;
/*
 * $Id: PdfWriter.cs,v 1.48 2008/05/13 11:25:23 psoares33 Exp $
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
    * A <CODE>DocWriter</CODE> class for PDF.
    * <P>
    * When this <CODE>PdfWriter</CODE> is added
    * to a certain <CODE>PdfDocument</CODE>, the PDF representation of every Element
    * added to this Document will be written to the outputstream.</P>
    */

    public class PdfWriter : DocWriter, 
        IPdfViewerPreferences,
        IPdfEncryptionSettings,
        IPdfVersion,
        IPdfDocumentActions,
        IPdfPageActions,
        IPdfXConformance,
        IPdfRunDirection,
        IPdfAnnotations {
        
    // INNER CLASSES
        
        /**
        * This class generates the structure of a PDF document.
        * <P>
        * This class covers the third section of Chapter 5 in the 'Portable Document Format
        * Reference Manual version 1.3' (page 55-60). It contains the body of a PDF document
        * (section 5.14) and it can also generate a Cross-reference Table (section 5.15).
        *
        * @see      PdfWriter
        * @see      PdfObject
        * @see      PdfIndirectObject
        */
        
        public class PdfBody {
            
            // inner classes
            
            /**
            * <CODE>PdfCrossReference</CODE> is an entry in the PDF Cross-Reference table.
            */
            
            internal class PdfCrossReference : IComparable {
                
                // membervariables
                private int type;
                
                /** Byte offset in the PDF file. */
                private int offset;
                
                private int refnum;
                /** generation of the object. */
                private int generation;
                
                // constructors
                /**
                * Constructs a cross-reference element for a PdfIndirectObject.
                * @param refnum
                * @param    offset      byte offset of the object
                * @param    generation  generationnumber of the object
                */
                
                internal PdfCrossReference(int refnum, int offset, int generation) {
                    type = 0;
                    this.offset = offset;
                    this.refnum = refnum;
                    this.generation = generation;
                }
                
                /**
                * Constructs a cross-reference element for a PdfIndirectObject.
                * @param refnum
                * @param    offset      byte offset of the object
                */
                
                internal PdfCrossReference(int refnum, int offset) {
                    type = 1;
                    this.offset = offset;
                    this.refnum = refnum;
                    this.generation = 0;
                }
                
                internal PdfCrossReference(int type, int refnum, int offset, int generation) {
                    this.type = type;
                    this.offset = offset;
                    this.refnum = refnum;
                    this.generation = generation;
                }
                
                internal int Refnum {
                    get {
                        return refnum;
                    }
                }
                
                /**
                * Returns the PDF representation of this <CODE>PdfObject</CODE>.
                * @param os
                * @throws IOException
                */
                
                public void ToPdf(Stream os) {
                    String s1 = offset.ToString().PadLeft(10, '0');
                    String s2 = generation.ToString().PadLeft(5, '0');
                    ByteBuffer buf = new ByteBuffer(40);
                    if (generation == 65535) {
                        buf.Append(s1).Append(' ').Append(s2).Append(" f \n");
                    }
                    else {
                        buf.Append(s1).Append(' ').Append(s2).Append(" n \n");
                    }
                    os.Write(buf.Buffer, 0, buf.Size);
                }
                
                /**
                * Writes PDF syntax to the Stream
                * @param midSize
                * @param os
                * @throws IOException
                */
                public void ToPdf(int midSize, Stream os) {
                    os.WriteByte((byte)type);
                    while (--midSize >= 0)
                        os.WriteByte((byte)((offset >> (8 * midSize)) & 0xff));
                    os.WriteByte((byte)((generation >> 8) & 0xff));
                    os.WriteByte((byte)(generation & 0xff));
                }
                
                /**
                * @see java.lang.Comparable#compareTo(java.lang.Object)
                */
                public int CompareTo(Object o) {
                    PdfCrossReference other = (PdfCrossReference)o;
                    return (refnum < other.refnum ? -1 : (refnum==other.refnum ? 0 : 1));
                }
                
                /**
                * @see java.lang.Object#equals(java.lang.Object)
                */
                public override bool Equals(Object obj) {
                    if (obj is PdfCrossReference) {
                        PdfCrossReference other = (PdfCrossReference)obj;
                        return (refnum == other.refnum);
                    }
                    else
                        return false;
                }
                
            
                public override int GetHashCode() {
                    return refnum;
                }
            }
            
            // membervariables

            private const int OBJSINSTREAM = 200;
            
            /** array containing the cross-reference table of the normal objects. */
            private k_Tree xrefs;
            private int refnum;
            /** the current byteposition in the body. */
            private int position;
            private PdfWriter writer;
            private ByteBuffer index;
            private ByteBuffer streamObjects;
            private int currentObjNum;
            private int numObj = 0;

            // constructors
            
            /**
            * Constructs a new <CODE>PdfBody</CODE>.
            * @param writer
            */
            internal PdfBody(PdfWriter writer) {
                xrefs = new k_Tree();
                xrefs[new PdfCrossReference(0, 0, 65535)] = null;
                position = writer.Os.Counter;
                refnum = 1;
                this.writer = writer;
            }
            
            // methods

            internal int Refnum {
                set {
                    this.refnum = value;
                }
            }
            
            private PdfWriter.PdfBody.PdfCrossReference AddToObjStm(PdfObject obj, int nObj) {
                if (numObj >= OBJSINSTREAM)
                    FlushObjStm();
                if (index == null) {
                    index = new ByteBuffer();
                    streamObjects = new ByteBuffer();
                    currentObjNum = IndirectReferenceNumber;
                    numObj = 0;
                }
                int p = streamObjects.Size;
                int idx = numObj++;
                PdfEncryption enc = writer.crypto;
                writer.crypto = null;
                obj.ToPdf(writer, streamObjects);
                writer.crypto = enc;
                streamObjects.Append(' ');
                index.Append(nObj).Append(' ').Append(p).Append(' ');
                return new PdfWriter.PdfBody.PdfCrossReference(2, nObj, currentObjNum, idx);
            }
            
            internal void FlushObjStm() {
                if (numObj == 0)
                    return;
                int first = index.Size;
                index.Append(streamObjects);
                PdfStream stream = new PdfStream(index.ToByteArray());
                stream.FlateCompress();
                stream.Put(PdfName.TYPE, PdfName.OBJSTM);
                stream.Put(PdfName.N, new PdfNumber(numObj));
                stream.Put(PdfName.FIRST, new PdfNumber(first));
                Add(stream, currentObjNum);
                index = null;
                streamObjects = null;
                numObj = 0;
            }
            
            /**
            * Adds a <CODE>PdfObject</CODE> to the body.
            * <P>
            * This methods creates a <CODE>PdfIndirectObject</CODE> with a
            * certain number, containing the given <CODE>PdfObject</CODE>.
            * It also adds a <CODE>PdfCrossReference</CODE> for this object
            * to an <CODE>ArrayList</CODE> that will be used to build the
            * Cross-reference Table.
            *
            * @param        object          a <CODE>PdfObject</CODE>
            * @return       a <CODE>PdfIndirectObject</CODE>
            * @throws IOException
            */
            
            internal PdfIndirectObject Add(PdfObject objecta) {
                return Add(objecta, IndirectReferenceNumber);
            }
            
            internal PdfIndirectObject Add(PdfObject objecta, bool inObjStm) {
                return Add(objecta, IndirectReferenceNumber, inObjStm);
            }
            
            /**
            * Gets a PdfIndirectReference for an object that will be created in the future.
            * @return a PdfIndirectReference
            */
            
            internal PdfIndirectReference PdfIndirectReference {
                get {
                    return new PdfIndirectReference(0, IndirectReferenceNumber);
                }
            }
            
            internal int IndirectReferenceNumber {
                get {
                    int n = refnum++;
                    xrefs[new PdfCrossReference(n, 0, 65536)] = null;
                    return n;
                }
            }
            
            /**
            * Adds a <CODE>PdfObject</CODE> to the body given an already existing
            * PdfIndirectReference.
            * <P>
            * This methods creates a <CODE>PdfIndirectObject</CODE> with the number given by
            * <CODE>ref</CODE>, containing the given <CODE>PdfObject</CODE>.
            * It also adds a <CODE>PdfCrossReference</CODE> for this object
            * to an <CODE>ArrayList</CODE> that will be used to build the
            * Cross-reference Table.
            *
            * @param        object          a <CODE>PdfObject</CODE>
            * @param        ref             a <CODE>PdfIndirectReference</CODE>
            * @return       a <CODE>PdfIndirectObject</CODE>
            * @throws IOException
            */
            
            internal PdfIndirectObject Add(PdfObject objecta, PdfIndirectReference refa) {
                return Add(objecta, refa.Number);
            }
            
            internal PdfIndirectObject Add(PdfObject objecta, PdfIndirectReference refa, bool inObjStm) {
                return Add(objecta, refa.Number, inObjStm);
            }
            
            internal PdfIndirectObject Add(PdfObject objecta, int refNumber) {
                return Add(objecta, refNumber, true); // to false
            }
            
            internal PdfIndirectObject Add(PdfObject objecta, int refNumber, bool inObjStm) {
                if (inObjStm && objecta.CanBeInObjStm() && writer.FullCompression) {
                    PdfCrossReference pxref = AddToObjStm(objecta, refNumber);
                    PdfIndirectObject indirect = new PdfIndirectObject(refNumber, objecta, writer);
                    xrefs.Remove(pxref);
                    xrefs[pxref] = null;
                    return indirect;
                }
                else {
                    PdfIndirectObject indirect = new PdfIndirectObject(refNumber, objecta, writer);
                    PdfCrossReference pxref = new PdfCrossReference(refNumber, position);
                    xrefs.Remove(pxref);
                    xrefs[pxref] = null;
                    indirect.WriteTo(writer.Os);
                    position = writer.Os.Counter;
                    return indirect;
                }
            }
            
            /**
            * Returns the offset of the Cross-Reference table.
            *
            * @return       an offset
            */
            
            internal int Offset {
                get {
                    return position;
                }
            }
            
            /**
            * Returns the total number of objects contained in the CrossReferenceTable of this <CODE>Body</CODE>.
            *
            * @return   a number of objects
            */
            
            internal int Size {
                get {
                    k_Iterator it = xrefs.End.Clone();
                    it.Prev();
                    return Math.Max(((PdfCrossReference)((DictionaryEntry)it.Current).Key).Refnum + 1, refnum);
                }
            }
            
            /**
            * Returns the CrossReferenceTable of the <CODE>Body</CODE>.
            * @param os
            * @param root
            * @param info
            * @param encryption
            * @param fileID
            * @param prevxref
            * @throws IOException
            */
            
            internal void WriteCrossReferenceTable(Stream os, PdfIndirectReference root, PdfIndirectReference info, PdfIndirectReference encryption, PdfObject fileID, int prevxref) {
                int refNumber = 0;
                if (writer.FullCompression) {
                    FlushObjStm();
                    refNumber = IndirectReferenceNumber;
                    xrefs[new PdfCrossReference(refNumber, position)] = null;
                }
                PdfCrossReference entry = (PdfCrossReference)((DictionaryEntry)xrefs.Begin.Current).Key;
                int first = entry.Refnum;
                int len = 0;
                ArrayList sections = new ArrayList();
                for (k_Iterator i = xrefs.Begin.Clone(); i != xrefs.End; i.Next()) {
                    entry = (PdfCrossReference)((DictionaryEntry)i.Current).Key;
                    if (first + len == entry.Refnum)
                        ++len;
                    else {
                        sections.Add(first);
                        sections.Add(len);
                        first = entry.Refnum;
                        len = 1;
                    }
                }
                sections.Add(first);
                sections.Add(len);
                if (writer.FullCompression) {
                    int mid = 4;
                    uint mask = 0xff000000;
                    for (; mid > 1; --mid) {
                        if ((mask & position) != 0)
                            break;
                        mask >>= 8;
                    }
                    ByteBuffer buf = new ByteBuffer();
                    
                    for (k_Iterator i = xrefs.Begin.Clone(); i != xrefs.End; i.Next()) {
                        entry = (PdfCrossReference)((DictionaryEntry)i.Current).Key;
                        entry.ToPdf(mid, buf);
                    }
                    PdfStream xr = new PdfStream(buf.ToByteArray());
                    buf = null;
                    xr.FlateCompress();
                    xr.Put(PdfName.SIZE, new PdfNumber(Size));
                    xr.Put(PdfName.ROOT, root);
                    if (info != null) {
                        xr.Put(PdfName.INFO, info);
                    }
                    if (encryption != null)
                        xr.Put(PdfName.ENCRYPT, encryption);
                    if (fileID != null)
                        xr.Put(PdfName.ID, fileID);
                    xr.Put(PdfName.W, new PdfArray(new int[]{1, mid, 2}));
                    xr.Put(PdfName.TYPE, PdfName.XREF);
                    PdfArray idx = new PdfArray();
                    for (int k = 0; k < sections.Count; ++k)
                        idx.Add(new PdfNumber((int)sections[k]));
                    xr.Put(PdfName.INDEX, idx);
                    if (prevxref > 0)
                        xr.Put(PdfName.PREV, new PdfNumber(prevxref));
                    PdfEncryption enc = writer.crypto;
                    writer.crypto = null;
                    PdfIndirectObject indirect = new PdfIndirectObject(refNumber, xr, writer);
                    indirect.WriteTo(writer.Os);
                    writer.crypto = enc;
                }
                else {
                    byte[] tmp = GetISOBytes("xref\n");
                    os.Write(tmp, 0, tmp.Length);
                    k_Iterator i = xrefs.Begin.Clone();
                    for (int k = 0; k < sections.Count; k += 2) {
                        first = (int)sections[k];
                        len = (int)sections[k + 1];
                        tmp = GetISOBytes(first.ToString());
                        os.Write(tmp, 0, tmp.Length);
                        os.WriteByte((byte)' ');
                        tmp = GetISOBytes(len.ToString());
                        os.Write(tmp, 0, tmp.Length);
                        os.WriteByte((byte)'\n');
                        while (len-- > 0) {
                            entry = (PdfCrossReference)((DictionaryEntry)i.Current).Key;
                            entry.ToPdf(os);
                            i.Next();
                        }
                    }
                }
            }
        }
        
        /**
        * <CODE>PdfTrailer</CODE> is the PDF Trailer object.
        * <P>
        * This object is described in the 'Portable Document Format Reference Manual version 1.3'
        * section 5.16 (page 59-60).
        */
        
        internal class PdfTrailer : PdfDictionary {
            
            // membervariables
            
            internal int offset;
            
            // constructors
            
            /**
            * Constructs a PDF-Trailer.
            *
            * @param        size        the number of entries in the <CODE>PdfCrossReferenceTable</CODE>
            * @param        offset      offset of the <CODE>PdfCrossReferenceTable</CODE>
            * @param        root        an indirect reference to the root of the PDF document
            * @param        info        an indirect reference to the info object of the PDF document
            * @param encryption
            * @param fileID
            * @param prevxref
            */
            
            internal PdfTrailer(int size, int offset, PdfIndirectReference root, PdfIndirectReference info, PdfIndirectReference encryption, PdfObject fileID, int prevxref) {
                this.offset = offset;
                Put(PdfName.SIZE, new PdfNumber(size));
                Put(PdfName.ROOT, root);
                if (info != null) {
                    Put(PdfName.INFO, info);
                }
                if (encryption != null)
                    Put(PdfName.ENCRYPT, encryption);
                if (fileID != null)
                    Put(PdfName.ID, fileID);
                if (prevxref > 0)
                    Put(PdfName.PREV, new PdfNumber(prevxref));
            }
            
            /**
            * Returns the PDF representation of this <CODE>PdfObject</CODE>.
            * @param writer
            * @param os
            * @throws IOException
            */
            public override void ToPdf(PdfWriter writer, Stream os) {
                byte[] tmp = GetISOBytes("trailer\n");
                os.Write(tmp, 0, tmp.Length);
                base.ToPdf(null, os);
                tmp = GetISOBytes("\nstartxref\n");
                os.Write(tmp, 0, tmp.Length);
                tmp = GetISOBytes(offset.ToString());
                os.Write(tmp, 0, tmp.Length);
                tmp = GetISOBytes("\n%%EOF\n");
                os.Write(tmp, 0, tmp.Length);
            }
        }

    //  ESSENTIALS 
        
    //  Construct a PdfWriter instance
        
        /**
        * Constructs a <CODE>PdfWriter</CODE>.
        */
        protected PdfWriter() {
            root = new PdfPages(this);
        }
        
        /**
        * Constructs a <CODE>PdfWriter</CODE>.
        * <P>
        * Remark: a PdfWriter can only be constructed by calling the method
        * <CODE>getInstance(Document document, Stream os)</CODE>.
        *
        * @param    document    The <CODE>PdfDocument</CODE> that has to be written
        * @param    os          The <CODE>Stream</CODE> the writer has to write to.
        */
        
        protected PdfWriter(PdfDocument document, Stream os) : base(document, os) {
            root = new PdfPages(this);
            pdf = document;
            directContent = new PdfContentByte(this);
            directContentUnder = new PdfContentByte(this);
        }

        // get an instance of the PdfWriter
        
        /**
        * Use this method to get an instance of the <CODE>PdfWriter</CODE>.
        *
        * @param    document    The <CODE>Document</CODE> that has to be written
        * @param    os  The <CODE>Stream</CODE> the writer has to write to.
        * @return   a new <CODE>PdfWriter</CODE>
        *
        * @throws   DocumentException on error
        */
        
        public static PdfWriter GetInstance(Document document, Stream os)
        {
            PdfDocument pdf = new PdfDocument();
            document.AddDocListener(pdf);
            PdfWriter writer = new PdfWriter(pdf, os);
            pdf.AddWriter(writer);
            return writer;
        }
        
        /**
        * Use this method to get an instance of the <CODE>PdfWriter</CODE>.
        *
        * @return a new <CODE>PdfWriter</CODE>
        * @param document The <CODE>Document</CODE> that has to be written
        * @param os The <CODE>Stream</CODE> the writer has to write to.
        * @param listener A <CODE>DocListener</CODE> to pass to the PdfDocument.
        * @throws DocumentException on error
        */
        
        public static PdfWriter GetInstance(Document document, Stream os, IDocListener listener)
        {
            PdfDocument pdf = new PdfDocument();
            pdf.AddDocListener(listener);
            document.AddDocListener(pdf);
            PdfWriter writer = new PdfWriter(pdf, os);
            pdf.AddWriter(writer);
            return writer;
        }

    //  the PdfDocument instance
        /** the pdfdocument object. */
        protected internal PdfDocument pdf;

        /**
        * Gets the <CODE>PdfDocument</CODE> associated with this writer.
        * @return the <CODE>PdfDocument</CODE>
        */
        internal PdfDocument PdfDocument {
            get {
                return pdf;
            }
        }

        /**
        * Use this method to get the info dictionary if you want to
        * change it directly (add keys and values to the info dictionary).
        * @return the info dictionary
        */    
        public PdfDictionary Info {
            get {
                return ((PdfDocument)document).Info;
            }
        }

        /**
        * Use this method to get the current vertical page position.
        * @param ensureNewLine Tells whether a new line shall be enforced. This may cause side effects 
        *   for elements that do not terminate the lines they've started because those lines will get
        *   terminated. 
        * @return The current vertical page position.
        */
        public float GetVerticalPosition(bool ensureNewLine) {
            return pdf.GetVerticalPosition(ensureNewLine);
        }

    //  the PdfDirectContentByte instances
        
    /*
    * You should see Direct Content as a canvas on which you can draw
    * graphics and text. One canvas goes on top of the page (getDirectContent),
    * the other goes underneath (getDirectContentUnder).
    * You can always the same object throughout your document,
    * even if you have moved to a new page. Whatever you add on
    * the canvas will be displayed on top or under the current page.
    */

        /** The direct content in this document. */
        protected PdfContentByte directContent;
        
        /** The direct content under in this document. */
        protected PdfContentByte directContentUnder;

        /**
        * Use this method to get the direct content for this document.
        * There is only one direct content, multiple calls to this method
        * will allways retrieve the same object.
        * @return the direct content
        */
        public virtual PdfContentByte DirectContent {
            get {
                if (!open)
                    throw new Exception("The document is not open.");
                return directContent;
            }
        }

        /**
        * Use this method to get the direct content under for this document.
        * There is only one direct content, multiple calls to this method
        * will allways retrieve the same object.
        * @return the direct content
        */
        public virtual PdfContentByte DirectContentUnder {
            get {
                if (!open)
                    throw new Exception("The document is not open.");
                return directContentUnder;
            }
        }

        /**
        * Resets all the direct contents to empty.
        * This happens when a new page is started.
        */
        internal void ResetContent() {
            directContent.Reset();
            directContentUnder.Reset();
        }

    //  PDF body
        
    /*
    * A PDF file has 4 parts: a header, a body, a cross-reference table, and a trailer.
    * The body contains all the PDF objects that make up the PDF document.
    * Each element gets a reference (a set of numbers) and the byte position of
    * every object is stored in the cross-reference table.
    * Use these methods only if you know what you're doing.
    */

        /** body of the PDF document */
        protected internal PdfBody body;

        /**
        * Adds the local destinations to the body of the document.
        * @param dest the <CODE>Hashtable</CODE> containing the destinations
        * @throws IOException on error
        */
        internal void AddLocalDestinations(k_Tree dest) {
            foreach (String name in dest.Keys) {
                Object[] obj = (Object[])dest[name];
                PdfDestination destination = (PdfDestination)obj[2];
                if (destination == null)
                    throw new Exception("The name '" + name + "' has no local destination.");
                if (obj[1] == null)
                    obj[1] = PdfIndirectReference;
                AddToBody(destination, (PdfIndirectReference)obj[1]);
            }
        }

        /**
        * Adds an object to the PDF body.
        * @param object
        * @return a PdfIndirectObject
        * @throws IOException
        */
        public PdfIndirectObject AddToBody(PdfObject objecta) {
            PdfIndirectObject iobj = body.Add(objecta);
            return iobj;
        }
        
        /**
        * Adds an object to the PDF body.
        * @param object
        * @param inObjStm
        * @return a PdfIndirectObject
        * @throws IOException
        */
        public PdfIndirectObject AddToBody(PdfObject objecta, bool inObjStm) {
            PdfIndirectObject iobj = body.Add(objecta, inObjStm);
            return iobj;
        }
        
        /**
        * Adds an object to the PDF body.
        * @param object
        * @param ref
        * @return a PdfIndirectObject
        * @throws IOException
        */
        public PdfIndirectObject AddToBody(PdfObject objecta, PdfIndirectReference refa) {
            PdfIndirectObject iobj = body.Add(objecta, refa);
            return iobj;
        }
        
        /**
        * Adds an object to the PDF body.
        * @param object
        * @param ref
        * @param inObjStm
        * @return a PdfIndirectObject
        * @throws IOException
        */
        public PdfIndirectObject AddToBody(PdfObject objecta, PdfIndirectReference refa, bool inObjStm) {
            PdfIndirectObject iobj = body.Add(objecta, refa, inObjStm);
            return iobj;
        }
        
        /**
        * Adds an object to the PDF body.
        * @param object
        * @param refNumber
        * @return a PdfIndirectObject
        * @throws IOException
        */
        public PdfIndirectObject AddToBody(PdfObject objecta, int refNumber) {
            PdfIndirectObject iobj = body.Add(objecta, refNumber);
            return iobj;
        }
        
        /**
        * Adds an object to the PDF body.
        * @param object
        * @param refNumber
        * @param inObjStm
        * @return a PdfIndirectObject
        * @throws IOException
        */
        public PdfIndirectObject AddToBody(PdfObject objecta, int refNumber, bool inObjStm) {
            PdfIndirectObject iobj = body.Add(objecta, refNumber, inObjStm);
            return iobj;
        }

        /**
        * Gets a <CODE>PdfIndirectReference</CODE> for an object that
        * will be created in the future.
        * @return the <CODE>PdfIndirectReference</CODE>
        */
        public PdfIndirectReference PdfIndirectReference {
            get {
                return body.PdfIndirectReference;
            }
        }
        
        internal int IndirectReferenceNumber {
            get {
                return body.IndirectReferenceNumber;
            }
        }

        /**
        * Returns the outputStreamCounter.
        * @return the outputStreamCounter
        */
        internal OutputStreamCounter Os {
            get {
                return os;
            }
        }

    //  PDF Catalog
        
    /*
    * The Catalog is also called the root object of the document.
    * Whereas the Cross-Reference maps the objects number with the
    * byte offset so that the viewer can find the objects, the
    * Catalog tells the viewer the numbers of the objects needed
    * to render the document.
    */

        protected virtual PdfDictionary GetCatalog(PdfIndirectReference rootObj) {
            PdfDictionary catalog = pdf.GetCatalog(rootObj);
            // [F12] tagged PDF
            if (tagged) {
                this.StructureTreeRoot.BuildTree();
                catalog.Put(PdfName.STRUCTTREEROOT, structureTreeRoot.Reference);
                PdfDictionary mi = new PdfDictionary();
                mi.Put(PdfName.MARKED, PdfBoolean.PDFTRUE);
                if (userProperties)
                    mi.Put(PdfName.USERPROPERTIES, PdfBoolean.PDFTRUE);
                catalog.Put(PdfName.MARKINFO, mi);
            }
            // [F13] OCG
            if (documentOCG.Count != 0) {
                FillOCProperties(false);
                catalog.Put(PdfName.OCPROPERTIES, vOCProperties);
            }
            return catalog;
        }

        /** Holds value of property extraCatalog. */
        protected internal PdfDictionary extraCatalog;

        /**
        * Sets extra keys to the catalog.
        * @return the catalog to change
        */    
        public PdfDictionary ExtraCatalog {
            get {
                if (extraCatalog == null)
                    extraCatalog = new PdfDictionary();
                return this.extraCatalog;
            }
        }

    //  PdfPages
        
    /*
    * The page root keeps the complete page tree of the document.
    * There's an entry in the Catalog that refers to the root
    * of the page tree, the page tree contains the references
    * to pages and other page trees.
    */
        
        /** The root of the page tree. */
        protected PdfPages root;
        /** The PdfIndirectReference to the pages. */
        protected ArrayList pageReferences = new ArrayList();
        /** The current page number. */
        protected int currentPageNumber = 1;

        /**
        * Use this method to make sure the page tree has a lineair structure
        * (every leave is attached directly to the root).
        * Use this method to allow page reordering with method reorderPages.
        */    
        public void SetLinearPageMode() {
            root.SetLinearMode(null);
        }

        /**
        * Use this method to reorder the pages in the document.
        * A <CODE>null</CODE> argument value only returns the number of pages to process.
        * It is advisable to issue a <CODE>Document.newPage()</CODE> before using this method.
        * @return the total number of pages
        * @param order an array with the new page sequence. It must have the
        * same size as the number of pages.
        * @throws DocumentException if all the pages are not present in the array
        */
        public int ReorderPages(int[] order) {
            return root.ReorderPages(order);
        }

        /**
        * Use this method to get a reference to a page existing or not.
        * If the page does not exist yet the reference will be created
        * in advance. If on closing the document, a page number greater
        * than the total number of pages was requested, an exception
        * is thrown.
        * @param page the page number. The first page is 1
        * @return the reference to the page
        */
        public virtual PdfIndirectReference GetPageReference(int page) {
            --page;
            if (page < 0)
                throw new ArgumentOutOfRangeException("The page numbers start at 1.");
            PdfIndirectReference refa;
            if (page < pageReferences.Count) {
                refa = (PdfIndirectReference)pageReferences[page];
                if (refa == null) {
                    refa = body.PdfIndirectReference;
                    pageReferences[page] = refa;
                }
            }
            else {
                int empty = page - pageReferences.Count;
                for (int k = 0; k < empty; ++k)
                    pageReferences.Add(null);
                refa = body.PdfIndirectReference;
                pageReferences.Add(refa);
            }
            return refa;
        }

        /**
        * Gets the pagenumber of this document.
        * This number can be different from the real pagenumber,
        * if you have (re)set the page number previously.
        * @return a page number
        */
        public int PageNumber {
            get {
                return pdf.PageNumber;
            }
        }
        
        internal virtual PdfIndirectReference CurrentPage {
            get {
                return GetPageReference(currentPageNumber);
            }
        }

        public virtual int CurrentPageNumber {
            get {
                return currentPageNumber;
            }
        }

        /**
        * Adds some <CODE>PdfContents</CODE> to this Writer.
        * <P>
        * The document has to be open before you can begin to add content
        * to the body of the document.
        *
        * @return a <CODE>PdfIndirectReference</CODE>
        * @param page the <CODE>PdfPage</CODE> to add
        * @param contents the <CODE>PdfContents</CODE> of the page
        * @throws PdfException on error
        */
        internal virtual PdfIndirectReference Add(PdfPage page, PdfContents contents) {
            if (!open) {
                throw new PdfException("The document isn't open.");
            }
            PdfIndirectObject objecta;
            objecta = AddToBody(contents);
            page.Add(objecta.IndirectReference);
            if (group != null) {
                page.Put(PdfName.GROUP, group);
                group = null;
            }
            else if (rgbTransparencyBlending) {
                PdfDictionary pp = new PdfDictionary();
                pp.Put(PdfName.TYPE, PdfName.GROUP);
                pp.Put(PdfName.S, PdfName.TRANSPARENCY);
                pp.Put(PdfName.CS, PdfName.DEVICERGB);
                page.Put(PdfName.GROUP, pp);
            }
            root.AddPage(page);
            currentPageNumber++;
            return null;
        }

    //  page events
        
    /*
    * Page events are specific for iText, not for PDF.
    * Upon specific events (for instance when a page starts
    * or ends), the corresponing method in the page event
    * implementation that is added to the writer is invoked.
    */

        /** The <CODE>PdfPageEvent</CODE> for this document. */
        private IPdfPageEvent pageEvent;

        /**
        * Gets the <CODE>PdfPageEvent</CODE> for this document or <CODE>null</CODE>
        * if none is set.
        * @return the <CODE>PdfPageEvent</CODE> for this document or <CODE>null</CODE>
        * if none is set
        */
        public IPdfPageEvent PageEvent {
            get {
                return pageEvent;
            }
            set {
                if (value == null) this.pageEvent = null;
                else if (this.pageEvent == null) this.pageEvent = value;
                else if (this.pageEvent is PdfPageEventForwarder) ((PdfPageEventForwarder)this.pageEvent).AddPageEvent(value);
                else {
                    PdfPageEventForwarder forward = new PdfPageEventForwarder();
                    forward.AddPageEvent(this.pageEvent);
                    forward.AddPageEvent(value);
                    this.pageEvent = forward;
                }
            }
        }

    //  Open en Close method + method that create the PDF

        /** A number refering to the previous Cross-Reference Table. */
        protected int prevxref = 0;

        /**
        * Signals that the <CODE>Document</CODE> has been opened and that
        * <CODE>Elements</CODE> can be added.
        * <P>
        * When this method is called, the PDF-document header is
        * written to the outputstream.
        */
        public override void Open() {
            base.Open();
            pdf_version.WriteHeader(os);
            body = new PdfBody(this);
            if (pdfxConformance.IsPdfX32002()) {
                PdfDictionary sec = new PdfDictionary();
                sec.Put(PdfName.GAMMA, new PdfArray(new float[]{2.2f,2.2f,2.2f}));
                sec.Put(PdfName.MATRIX, new PdfArray(new float[]{0.4124f,0.2126f,0.0193f,0.3576f,0.7152f,0.1192f,0.1805f,0.0722f,0.9505f}));
                sec.Put(PdfName.WHITEPOINT, new PdfArray(new float[]{0.9505f,1f,1.089f}));
                PdfArray arr = new PdfArray(PdfName.CALRGB);
                arr.Add(sec);
                SetDefaultColorspace(PdfName.DEFAULTRGB, AddToBody(arr).IndirectReference);
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
                if ((currentPageNumber - 1) != pageReferences.Count)
                    throw new Exception("The page " + pageReferences.Count +
                    " was requested but the document has only " + (currentPageNumber - 1) + " pages.");
                pdf.Close();
                AddSharedObjectsToBody();
                // add the root to the body
                PdfIndirectReference rootRef = root.WritePageTree();
                // make the catalog-object and add it to the body
                PdfDictionary catalog = GetCatalog(rootRef);
                // [C9] if there is XMP data to add: add it
                if (xmpMetadata != null) {
                    PdfStream xmp = new PdfStream(xmpMetadata);
                    xmp.Put(PdfName.TYPE, PdfName.METADATA);
                    xmp.Put(PdfName.SUBTYPE, PdfName.XML);
                    if (crypto != null && !crypto.IsMetadataEncrypted()) {
                        PdfArray ar = new PdfArray();
                        ar.Add(PdfName.CRYPT);
                        xmp.Put(PdfName.FILTER, ar);
                    }
                    catalog.Put(PdfName.METADATA, body.Add(xmp).IndirectReference);
                }
                // [C10] make pdfx conformant
                if (IsPdfX()) {
                    pdfxConformance.CompleteInfoDictionary(Info);
                    pdfxConformance.CompleteExtraCatalog(ExtraCatalog);
                }
                // [C11] Output Intents
                if (extraCatalog != null) {
                    catalog.MergeDifferent(extraCatalog);
                }
                
                WriteOutlines(catalog, false);

                // add the Catalog to the body
                PdfIndirectObject indirectCatalog = AddToBody(catalog, false);
                // add the info-object to the body
                PdfIndirectObject infoObj = AddToBody(Info, false);

                // [F1] encryption
                PdfIndirectReference encryption = null;
                PdfObject fileID = null;
                body.FlushObjStm();
                if (crypto != null) {
                    PdfIndirectObject encryptionObject = AddToBody(crypto.GetEncryptionDictionary(), false);
                    encryption = encryptionObject.IndirectReference;
                    fileID = crypto.FileID;
                }
                else
                    fileID = PdfEncryption.CreateInfoId(PdfEncryption.CreateDocumentId());
                
                // write the cross-reference table of the body
                body.WriteCrossReferenceTable(os, indirectCatalog.IndirectReference,
                    infoObj.IndirectReference, encryption,  fileID, prevxref);

                // make the trailer
                // [F2] full compression
                if (fullCompression) {
                    byte[] tmp = GetISOBytes("startxref\n");
                    os.Write(tmp, 0, tmp.Length);
                    tmp = GetISOBytes(body.Offset.ToString());
                    os.Write(tmp, 0, tmp.Length);
                    tmp = GetISOBytes("\n%%EOF\n");
                    os.Write(tmp, 0, tmp.Length);
                }
                else {
                    PdfTrailer trailer = new PdfTrailer(body.Size,
                    body.Offset,
                    indirectCatalog.IndirectReference,
                    infoObj.IndirectReference,
                    encryption,
                    fileID, prevxref);
                    trailer.ToPdf(this, os);
                }
                base.Close();
            }
        }

        protected void AddSharedObjectsToBody() {
            // add the fonts
            foreach (FontDetails details in documentFonts.Values) {
                details.WriteFont(this);
            }
            // add the form XObjects
            foreach (Object[] objs in formXObjects.Values) {
                PdfTemplate template = (PdfTemplate)objs[1];
                if (template != null && template.IndirectReference is PRIndirectReference)
                    continue;
                if (template != null && template.Type == PdfTemplate.TYPE_TEMPLATE) {
                    AddToBody(template.FormXObject, template.IndirectReference);
                }
            }
            // add all the dependencies in the imported pages
            foreach (PdfReaderInstance rd in importedPages.Values) {
                currentPdfReaderInstance = rd;
                currentPdfReaderInstance.WriteAllPages();
            }
            currentPdfReaderInstance = null;
            // add the color
            foreach (ColorDetails color in documentColors.Values) {
                AddToBody(color.GetSpotColor(this), color.IndirectReference);
            }
            // add the pattern
            foreach (PdfPatternPainter pat in documentPatterns.Keys) {
                AddToBody(pat.Pattern, pat.IndirectReference);
            }
            // add the shading patterns
            foreach (PdfShadingPattern shadingPattern in documentShadingPatterns.Keys) {
                shadingPattern.AddToBody();
            }
            // add the shadings
            foreach (PdfShading shading in documentShadings.Keys) {
                shading.AddToBody();
            }
            // add the extgstate
            foreach (DictionaryEntry entry in documentExtGState) {
                PdfDictionary gstate = (PdfDictionary)entry.Key;
                PdfObject[] obj = (PdfObject[])entry.Value;
                AddToBody(gstate, (PdfIndirectReference)obj[1]);
            }
           
            // add the properties
            foreach (DictionaryEntry entry in documentProperties) {
                Object prop = entry.Key;
                PdfObject[] obj = (PdfObject[])entry.Value;
                if (prop is PdfLayerMembership){
                    PdfLayerMembership layer = (PdfLayerMembership)prop;
                    AddToBody(layer.PdfObject, layer.Ref);
                }
                else if ((prop is PdfDictionary) && !(prop is PdfLayer)){
                    AddToBody((PdfDictionary)prop, (PdfIndirectReference)obj[1]);
                }
            }
            foreach (IPdfOCG layer in documentOCG.Keys) {
                AddToBody(layer.PdfObject, layer.Ref);
            }
        }

    // Root data for the PDF document (used when composing the Catalog)
         
    //  [C1] Outlines (bookmarks)
         
        /**
        * Use this method to get the root outline
        * and construct bookmarks.
        * @return the root outline
        */
        public PdfOutline RootOutline {
            get {
                return directContent.RootOutline;
            }
        }

        protected ArrayList newBookmarks;
         
        /**
        * Sets the bookmarks. The list structure is defined in
        * {@link SimpleBookmark}.
        * @param outlines the bookmarks or <CODE>null</CODE> to remove any
        */    
        public ArrayList Outlines {
            set {
                newBookmarks = value;
            }
        }

        protected internal void WriteOutlines(PdfDictionary catalog, bool namedAsNames) {
            if (newBookmarks == null || newBookmarks.Count == 0)
                return;
            PdfDictionary top = new PdfDictionary();
            PdfIndirectReference topRef = this.PdfIndirectReference;
            Object[] kids = SimpleBookmark.IterateOutlines(this, topRef, newBookmarks, namedAsNames);
            top.Put(PdfName.FIRST, (PdfIndirectReference)kids[0]);
            top.Put(PdfName.LAST, (PdfIndirectReference)kids[1]);
            top.Put(PdfName.COUNT, new PdfNumber((int)kids[2]));
            AddToBody(top, topRef);
            catalog.Put(PdfName.OUTLINES, topRef);
        }

    //  [C2] PdfVersion interface
        /** possible PDF version (header) */
        public const char VERSION_1_2 = '2';
        /** possible PDF version (header) */
        public const char VERSION_1_3 = '3';
        /** possible PDF version (header) */
        public const char VERSION_1_4 = '4';
        /** possible PDF version (header) */
        public const char VERSION_1_5 = '5';
        /** possible PDF version (header) */
        public const char VERSION_1_6 = '6';
        /** possible PDF version (header) */
        public const char VERSION_1_7 = '7';
         
        /** possible PDF version (catalog) */
        public static readonly PdfName PDF_VERSION_1_2 = new PdfName("1.2");
        /** possible PDF version (catalog) */
        public static readonly PdfName PDF_VERSION_1_3 = new PdfName("1.3");
        /** possible PDF version (catalog) */
        public static readonly PdfName PDF_VERSION_1_4 = new PdfName("1.4");
        /** possible PDF version (catalog) */
        public static readonly PdfName PDF_VERSION_1_5 = new PdfName("1.5");
        /** possible PDF version (catalog) */
        public static readonly PdfName PDF_VERSION_1_6 = new PdfName("1.6");
        /** possible PDF version (catalog) */
        public static readonly PdfName PDF_VERSION_1_7 = new PdfName("1.7");

        /** Stores the version information for the header and the catalog. */
        protected PdfVersionImp pdf_version = new PdfVersionImp();

        /**
        * @see com.lowagie.text.pdf.interfaces.PdfVersion#setPdfVersion(char)
        */
        public virtual char PdfVersion {
            set {
                pdf_version.PdfVersion = value;
            }
        }
        
        /**
        * @see com.lowagie.text.pdf.interfaces.PdfVersion#setAtLeastPdfVersion(char)
        */
        public void SetAtLeastPdfVersion(char version) {
            pdf_version.SetAtLeastPdfVersion(version);
        }

        /**
        * @see com.lowagie.text.pdf.interfaces.PdfVersion#setPdfVersion(com.lowagie.text.pdf.PdfName)
        */
        public void SetPdfVersion(PdfName version) {
            pdf_version.SetPdfVersion(version);
        }

        /**
        * Returns the version information.
        */
        internal PdfVersionImp GetPdfVersion() {
            return pdf_version;
        }

    //  [C3] PdfViewerPreferences interface

        // page layout (section 13.1.1 of "iText in Action")
        
        /** A viewer preference */
        public const int PageLayoutSinglePage = 1;
        /** A viewer preference */
        public const int PageLayoutOneColumn = 2;
        /** A viewer preference */
        public const int PageLayoutTwoColumnLeft = 4;
        /** A viewer preference */
        public const int PageLayoutTwoColumnRight = 8;
        /** A viewer preference */
        public const int PageLayoutTwoPageLeft = 16;
        /** A viewer preference */
        public const int PageLayoutTwoPageRight = 32;

        // page mode (section 13.1.2 of "iText in Action")
        
        /** A viewer preference */
        public const int PageModeUseNone = 64;
        /** A viewer preference */
        public const int PageModeUseOutlines = 128;
        /** A viewer preference */
        public const int PageModeUseThumbs = 256;
        /** A viewer preference */
        public const int PageModeFullScreen = 512;
        /** A viewer preference */
        public const int PageModeUseOC = 1024;
        /** A viewer preference */
        public const int PageModeUseAttachments = 2048;
        
        // values for setting viewer preferences in iText versions older than 2.x
        
        /** A viewer preference */
        public const int HideToolbar = 1 << 12;
        /** A viewer preference */
        public const int HideMenubar = 1 << 13;
        /** A viewer preference */
        public const int HideWindowUI = 1 << 14;
        /** A viewer preference */
        public const int FitWindow = 1 << 15;
        /** A viewer preference */
        public const int CenterWindow = 1 << 16;
        /** A viewer preference */
        public const int DisplayDocTitle = 1 << 17;

        /** A viewer preference */
        public const int NonFullScreenPageModeUseNone = 1 << 18;
        /** A viewer preference */
        public const int NonFullScreenPageModeUseOutlines = 1 << 19;
        /** A viewer preference */
        public const int NonFullScreenPageModeUseThumbs = 1 << 20;
        /** A viewer preference */
        public const int NonFullScreenPageModeUseOC = 1 << 21;

        /** A viewer preference */
        public const int DirectionL2R = 1 << 22;
        /** A viewer preference */
        public const int DirectionR2L = 1 << 23;

        /** A viewer preference */
        public const int PrintScalingNone = 1 << 24;

        /**
        * Sets the viewer preferences as the sum of several constants.
        * @param preferences the viewer preferences
        * @see PdfViewerPreferences#setViewerPreferences
        */
        public virtual int ViewerPreferences {
            set {
                pdf.ViewerPreferences = value;
            }
        }

        /** Adds a viewer preference
        * @param preferences the viewer preferences
        * @see PdfViewerPreferences#addViewerPreference
        */
        public virtual void AddViewerPreference(PdfName key, PdfObject value) {
            pdf.AddViewerPreference(key, value);
        }

    //  [C4] Page labels
        
        /**
        * Use this method to add page labels
        * @param pageLabels the page labels
        */
        public virtual PdfPageLabels PageLabels {
            set {
                pdf.PageLabels = value;
            }
        }

    //  [C5] named objects: named destinations, javascript, embedded files
         
        /**
        * Use this method to add a JavaScript action at the document level.
        * When the document opens, all this JavaScript runs.
        * @param js The JavaScript action
        */
        public virtual void AddJavaScript(PdfAction js) {
            pdf.AddJavaScript(js);
        }

        /** Adds a JavaScript action at the document level. When the document
        * opens all this JavaScript runs.
        * @param code the JavaScript code
        * @param unicode select JavaScript unicode. Note that the internal
        * Acrobat JavaScript engine does not support unicode,
        * so this may or may not work for you
        */
        public virtual void AddJavaScript(String code, bool unicode) {
            AddJavaScript(PdfAction.JavaScript(code, this, unicode));
        }
        
        /** Adds a JavaScript action at the document level. When the document
        * opens all this JavaScript runs.
        * @param code the JavaScript code
        */
        public virtual void AddJavaScript(String code) {
            AddJavaScript(code, false);
        }

        /**
        * Use this method to add a JavaScript action at the document level.
        * When the document opens, all this JavaScript runs.
        * @param name The name of the JS Action in the name tree
        * @param js The JavaScript action
        */
        public void AddJavaScript(String name, PdfAction js) {
            pdf.AddJavaScript(name, js);
        }
         
        /**
        * Use this method to add a JavaScript action at the document level.
        * When the document opens, all this JavaScript runs.
        * @param name The name of the JS Action in the name tree
        * @param code the JavaScript code
        * @param unicode select JavaScript unicode. Note that the internal
        * Acrobat JavaScript engine does not support unicode,
        * so this may or may not work for you
        */
        public void AddJavaScript(String name, String code, bool unicode) {
            AddJavaScript(name, PdfAction.JavaScript(code, this, unicode));
        }
         
        /**
        * Use this method to adds a JavaScript action at the document level.
        * When the document opens, all this JavaScript runs.
        * @param name The name of the JS Action in the name tree
        * @param code the JavaScript code
        */
        public void AddJavaScript(String name, String code) {
            AddJavaScript(name, code, false);
        }
         
        /** Adds a file attachment at the document level.
        * @param description the file description
        * @param fileStore an array with the file. If it's <CODE>null</CODE>
        * the file will be read from the disk
        * @param file the path to the file. It will only be used if
        * <CODE>fileStore</CODE> is not <CODE>null</CODE>
        * @param fileDisplay the actual file name stored in the pdf
        * @throws IOException on error
        */    
        public virtual void AddFileAttachment(String description, byte[] fileStore, String file, String fileDisplay) {
            AddFileAttachment(description, PdfFileSpecification.FileEmbedded(this, file, fileDisplay, fileStore));
        }

        /** Adds a file attachment at the document level.
        * @param description the file description
        * @param fs the file specification
        */    
        public virtual void AddFileAttachment(String description, PdfFileSpecification fs) {
            pdf.AddFileAttachment(description, fs);
        }

        /** Adds a file attachment at the document level.
        * @param fs the file specification
        */    
        public void AddFileAttachment(PdfFileSpecification fs) {
            pdf.AddFileAttachment(null, fs);
        }

    // [C6] Actions (open and additional)

        /** action value */
        public static PdfName DOCUMENT_CLOSE = PdfName.WC;
        /** action value */
        public static PdfName WILL_SAVE = PdfName.WS;
        /** action value */
        public static PdfName DID_SAVE = PdfName.DS;
        /** action value */
        public static PdfName WILL_PRINT = PdfName.WP;
        /** action value */
        public static PdfName DID_PRINT = PdfName.DP;

        /** When the document opens it will jump to the destination with
        * this name.
        * @param name the name of the destination to jump to
        */
        public virtual void SetOpenAction(String name) {
            pdf.SetOpenAction(name);
        }

        /** When the document opens this <CODE>action</CODE> will be
        * invoked.
        * @param action the action to be invoked
        */
        public virtual void SetOpenAction(PdfAction action) {
            pdf.SetOpenAction(action);
        }

        /** Additional-actions defining the actions to be taken in
        * response to various trigger events affecting the document
        * as a whole. The actions types allowed are: <CODE>DOCUMENT_CLOSE</CODE>,
        * <CODE>WILL_SAVE</CODE>, <CODE>DID_SAVE</CODE>, <CODE>WILL_PRINT</CODE>
        * and <CODE>DID_PRINT</CODE>.
        *
        * @param actionType the action type
        * @param action the action to execute in response to the trigger
        * @throws PdfException on invalid action type
        */
        public virtual void SetAdditionalAction(PdfName actionType, PdfAction action) {
            if (!(actionType.Equals(DOCUMENT_CLOSE) ||
            actionType.Equals(WILL_SAVE) ||
            actionType.Equals(DID_SAVE) ||
            actionType.Equals(WILL_PRINT) ||
            actionType.Equals(DID_PRINT))) {
                throw new PdfException("Invalid additional action type: " + actionType.ToString());
            }
            pdf.AddAdditionalAction(actionType, action);
        }

    //  [C7] portable collections
        /**
        * Sets the Collection dictionary.
        * @param collection a dictionary of type PdfCollection
        */
        public PdfCollection Collection {
            set {
                SetAtLeastPdfVersion(VERSION_1_7);
                pdf.Collection = value;
            }
        }

    //  [C8] AcroForm

        /** signature value */
        public const int SIGNATURE_EXISTS = 1;
        /** signature value */
        public const int SIGNATURE_APPEND_ONLY = 2;

        /** Gets the AcroForm object.
        * @return the <CODE>PdfAcroForm</CODE>
        */
        
        public PdfAcroForm AcroForm {
            get {
                return pdf.AcroForm;
            }
        }

        /** Adds a <CODE>PdfAnnotation</CODE> or a <CODE>PdfFormField</CODE>
        * to the document. Only the top parent of a <CODE>PdfFormField</CODE>
        * needs to be added.
        * @param annot the <CODE>PdfAnnotation</CODE> or the <CODE>PdfFormField</CODE> to add
        */
        public virtual void AddAnnotation(PdfAnnotation annot) {
            pdf.AddAnnotation(annot);
        }
        
        internal virtual void AddAnnotation(PdfAnnotation annot, int page) {
            AddAnnotation(annot);
        }

        /** Adds the <CODE>PdfAnnotation</CODE> to the calculation order
        * array.
        * @param annot the <CODE>PdfAnnotation</CODE> to be added
        */
        public virtual void AddCalculationOrder(PdfFormField annot) {
            pdf.AddCalculationOrder(annot);
        }
        
        /** Set the signature flags.
        * @param f the flags. This flags are ORed with current ones
        */
        public virtual int SigFlags {
            set {
                pdf.SigFlags = value;
            }
        }

    //  [C9] Metadata

        /** XMP Metadata for the document. */
        protected byte[] xmpMetadata = null;

        /**
        * Sets XMP Metadata.
        * @param xmpMetadata The xmpMetadata to set.
        */
        public byte[] XmpMetadata {
            set {
                this.xmpMetadata = value;
            }
            get {
                return this.xmpMetadata;
            }
        }
        
        /**
        * Use this method to set the XMP Metadata for each page.
        * @param xmpMetadata The xmpMetadata to set.
        */
        public byte[] PageXmpMetadata {
            set {
                pdf.XmpMetadata = value;
            }
        }

        /**
        * Creates XMP Metadata based on the metadata in the PdfDocument.
        */
        public void CreateXmpMetadata() {
            XmpMetadata = CreateXmpMetadataBytes();
        }

        /**
        * @return an XmpMetadata byte array
        */
        private byte[] CreateXmpMetadataBytes() {
            MemoryStream baos = new MemoryStream();
            try {
                XmpWriter xmp = new XmpWriter(baos, pdf.Info, pdfxConformance.PDFXConformance);
                xmp.Close();
            }
            catch(IOException) {
            }
            return baos.ToArray();
        }

    // [C10] PDFX Conformance

        /** PDF/X level */
        public const int PDFXNONE = 0;
        /** PDF/X level */
        public const int PDFX1A2001 = 1;
        /** PDF/X level */
        public const int PDFX32002 = 2;
        /** PDFA-1A level. */
        public const int PDFA1A = 3;
        /** PDFA-1B level. */
        public const int PDFA1B = 4;

        /** Stores the PDF/X level. */
        private PdfXConformanceImp pdfxConformance = new PdfXConformanceImp();

        /**
        * Sets the PDFX conformance level. Allowed values are PDFX1A2001 and PDFX32002. It
        * must be called before opening the document.
        * @param pdfxConformance the conformance level
        */    
        public int PDFXConformance {
            set {
                if (pdfxConformance.PDFXConformance == value)
                    return;
                if (pdf.IsOpen())
                    throw new PdfXConformanceException("PDFX conformance can only be set before opening the document.");
                if (crypto != null)
                    throw new PdfXConformanceException("A PDFX conforming document cannot be encrypted.");
                if (value == PDFA1A || value == PDFA1B)
                    PdfVersion = VERSION_1_4;
                else if (value != PDFXNONE)
                    PdfVersion = VERSION_1_3;
                pdfxConformance.PDFXConformance = value;
            }
            get {
                return pdfxConformance.PDFXConformance;
            }
        }

        /** @see com.lowagie.text.pdf.interfaces.PdfXConformance#isPdfX() */
        public bool IsPdfX() {
            return pdfxConformance.IsPdfX();
        }

    //  [C11] Output intents

        /**
        * Sets the values of the output intent dictionary. Null values are allowed to
        * suppress any key.
        * @param outputConditionIdentifier a value
        * @param outputCondition a value
        * @param registryName a value
        * @param info a value
        * @param destOutputProfile a value
        * @throws IOException on error
        */    
        public void SetOutputIntents(String outputConditionIdentifier, String outputCondition, String registryName, String info, byte[] destOutputProfile) {
            PdfDictionary outa = ExtraCatalog; //force the creation
            outa = new PdfDictionary(PdfName.OUTPUTINTENT);
            if (outputCondition != null)
                outa.Put(PdfName.OUTPUTCONDITION, new PdfString(outputCondition, PdfObject.TEXT_UNICODE));
            if (outputConditionIdentifier != null)
                outa.Put(PdfName.OUTPUTCONDITIONIDENTIFIER, new PdfString(outputConditionIdentifier, PdfObject.TEXT_UNICODE));
            if (registryName != null)
                outa.Put(PdfName.REGISTRYNAME, new PdfString(registryName, PdfObject.TEXT_UNICODE));
            if (info != null)
                outa.Put(PdfName.INFO, new PdfString(info, PdfObject.TEXT_UNICODE));
            if (destOutputProfile != null) {
                PdfStream stream = new PdfStream(destOutputProfile);
                stream.FlateCompress();
                outa.Put(PdfName.DESTOUTPUTPROFILE, AddToBody(stream).IndirectReference);
            }
            outa.Put(PdfName.S, PdfName.GTS_PDFX);
            extraCatalog.Put(PdfName.OUTPUTINTENTS, new PdfArray(outa));
        }
        
        /**
        * Copies the output intent dictionary from other document to this one.
        * @param reader the other document
        * @param checkExistence <CODE>true</CODE> to just check for the existence of a valid output intent
        * dictionary, <CODE>false</CODE> to insert the dictionary if it exists
        * @throws IOException on error
        * @return <CODE>true</CODE> if the output intent dictionary exists, <CODE>false</CODE>
        * otherwise
        */    
        public bool SetOutputIntents(PdfReader reader, bool checkExistence) {
            PdfDictionary catalog = reader.Catalog;
            PdfArray outs = (PdfArray)PdfReader.GetPdfObject(catalog.Get(PdfName.OUTPUTINTENTS));
            if (outs == null)
                return false;
            ArrayList arr = outs.ArrayList;
            if (arr.Count == 0)
                return false;
            PdfDictionary outa = (PdfDictionary)PdfReader.GetPdfObject((PdfObject)arr[0]);
            PdfObject obj = PdfReader.GetPdfObject(outa.Get(PdfName.S));
            if (obj == null || !PdfName.GTS_PDFX.Equals(obj))
                return false;
            if (checkExistence)
                return true;
            PRStream stream = (PRStream)PdfReader.GetPdfObject(outa.Get(PdfName.DESTOUTPUTPROFILE));
            byte[] destProfile = null;
            if (stream != null) {
                destProfile = PdfReader.GetStreamBytes(stream);
            }
            SetOutputIntents(GetNameString(outa, PdfName.OUTPUTCONDITIONIDENTIFIER), GetNameString(outa, PdfName.OUTPUTCONDITION),
                GetNameString(outa, PdfName.REGISTRYNAME), GetNameString(outa, PdfName.INFO), destProfile);
            return true;
        }

        private static String GetNameString(PdfDictionary dic, PdfName key) {
            PdfObject obj = PdfReader.GetPdfObject(dic.Get(key));
            if (obj == null || !obj.IsString())
                return null;
            return ((PdfString)obj).ToUnicodeString();
        }
        
    // PDF Objects that have an impact on the PDF body

    //  [F1] PdfEncryptionSettings interface

        // types of encryption
        
        /** Type of encryption */
        public const int STANDARD_ENCRYPTION_40 = 0;
        /** Type of encryption */
        public const int STANDARD_ENCRYPTION_128 = 1;
        /** Type of encryption */
        public const int ENCRYPTION_AES_128 = 2;
        /** Mask to separate the encryption type from the encryption mode. */
        internal const int ENCRYPTION_MASK = 7;
        /** Add this to the mode to keep the metadata in clear text */
        public const int DO_NOT_ENCRYPT_METADATA = 8;
        
        // permissions
        
        /** The operation permitted when the document is opened with the user password
        *
        * @since 2.0.7
        */
        public const int ALLOW_PRINTING = 4 + 2048;

        /** The operation permitted when the document is opened with the user password
        *
        * @since 2.0.7
        */
        public const int ALLOW_MODIFY_CONTENTS = 8;

        /** The operation permitted when the document is opened with the user password
        *
        * @since 2.0.7
        */
        public const int ALLOW_COPY = 16;

        /** The operation permitted when the document is opened with the user password
        *
        * @since 2.0.7
        */
        public const int ALLOW_MODIFY_ANNOTATIONS = 32;

        /** The operation permitted when the document is opened with the user password
        *
        * @since 2.0.7
        */
        public const int ALLOW_FILL_IN = 256;

        /** The operation permitted when the document is opened with the user password
        *
        * @since 2.0.7
        */
        public const int ALLOW_SCREENREADERS = 512;

        /** The operation permitted when the document is opened with the user password
        *
        * @since 2.0.7
        */
        public const int ALLOW_ASSEMBLY = 1024;

        /** The operation permitted when the document is opened with the user password
        *
        * @since 2.0.7
        */
        public const int ALLOW_DEGRADED_PRINTING = 4;
        
        /** @deprecated As of iText 2.0.7, use {@link #ALLOW_PRINTING} instead. Scheduled for removal at or after 2.2.0 */
        public const int AllowPrinting = ALLOW_PRINTING;
        /** @deprecated As of iText 2.0.7, use {@link #ALLOW_MODIFY_CONTENTS} instead. Scheduled for removal at or after 2.2.0 */
        public const int AllowModifyContents = ALLOW_MODIFY_CONTENTS;
        /** @deprecated As of iText 2.0.7, use {@link #ALLOW_COPY} instead. Scheduled for removal at or after 2.2.0 */
        public const int AllowCopy = ALLOW_COPY;
        /** @deprecated As of iText 2.0.7, use {@link #ALLOW_MODIFY_ANNOTATIONS} instead. Scheduled for removal at or after 2.2.0 */
        public const int AllowModifyAnnotations = ALLOW_MODIFY_ANNOTATIONS;
        /** @deprecated As of iText 2.0.7, use {@link #ALLOW_FILL_IN} instead. Scheduled for removal at or after 2.2.0 */
        public const int AllowFillIn = ALLOW_FILL_IN;
        /** @deprecated As of iText 2.0.7, use {@link #ALLOW_SCREENREADERS} instead. Scheduled for removal at or after 2.2.0 */
        public const int AllowScreenReaders = ALLOW_SCREENREADERS;
        /** @deprecated As of iText 2.0.7, use {@link #ALLOW_ASSEMBLY} instead. Scheduled for removal at or after 2.2.0 */
        public const int AllowAssembly = ALLOW_ASSEMBLY;
        /** @deprecated As of iText 2.0.7, use {@link #ALLOW_DEGRADED_PRINTING} instead. Scheduled for removal at or after 2.2.0 */
        public const int AllowDegradedPrinting = ALLOW_DEGRADED_PRINTING;
        
        // Strength of the encryption (kept for historical reasons)
        /** @deprecated As of iText 2.0.7, use {@link #STANDARD_ENCRYPTION_40} instead. Scheduled for removal at or after 2.2.0 */
        public const bool STRENGTH40BITS = false;
        /** @deprecated As of iText 2.0.7, use {@link #STANDARD_ENCRYPTION_128} instead. Scheduled for removal at or after 2.2.0 */
        public const bool STRENGTH128BITS = true;

        /** Contains the business logic for cryptography. */
        protected PdfEncryption crypto;

        internal PdfEncryption Encryption {
            get {
                return crypto;
            }
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
        * @param encryptionType the type of encryption. It can be one of STANDARD_ENCRYPTION_40, STANDARD_ENCRYPTION_128 or ENCRYPTION_AES128.
        * Optionally DO_NOT_ENCRYPT_METADATA can be ored to output the metadata in cleartext
        * @throws DocumentException if the document is already open
        */
        public void SetEncryption(byte[] userPassword, byte[] ownerPassword, int permissions, int encryptionType) {
            if (pdf.IsOpen())
                throw new DocumentException("Encryption can only be added before opening the document.");
            crypto = new PdfEncryption();
            crypto.SetCryptoMode(encryptionType, 0);
            crypto.SetupAllKeys(userPassword, ownerPassword, permissions);
        }
        
        /**
        * Sets the certificate encryption options for this document. An array of one or more public certificates
        * must be provided together with an array of the same size for the permissions for each certificate.
        *  The open permissions for the document can be
        *  AllowPrinting, AllowModifyContents, AllowCopy, AllowModifyAnnotations,
        *  AllowFillIn, AllowScreenReaders, AllowAssembly and AllowDegradedPrinting.
        *  The permissions can be combined by ORing them.
        * Optionally DO_NOT_ENCRYPT_METADATA can be ored to output the metadata in cleartext
        * @param certs the public certificates to be used for the encryption
        * @param permissions the user permissions for each of the certicates
        * @param encryptionType the type of encryption. It can be one of STANDARD_ENCRYPTION_40, STANDARD_ENCRYPTION_128 or ENCRYPTION_AES128.
        * @throws DocumentException if the document is already open
        */
        public void SetEncryption(X509Certificate[] certs, int[] permissions, int encryptionType) {
            if (pdf.IsOpen())
                throw new DocumentException("Encryption can only be added before opening the document.");
            crypto = new PdfEncryption();
            if (certs != null) {
                for (int i=0; i < certs.Length; i++) {
                    crypto.AddRecipient(certs[i], permissions[i]);
                }
            }
            crypto.SetCryptoMode(encryptionType, 0);
            crypto.GetEncryptionDictionary();
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
            SetEncryption(userPassword, ownerPassword, permissions, strength128Bits ? STANDARD_ENCRYPTION_128 : STANDARD_ENCRYPTION_40);
        }
        
        /**
        * Sets the encryption options for this document. The userPassword and the
        *  ownerPassword can be null or have zero length. In this case the ownerPassword
        *  is replaced by a random string. The open permissions for the document can be
        *  AllowPrinting, AllowModifyContents, AllowCopy, AllowModifyAnnotations,
        *  AllowFillIn, AllowScreenReaders, AllowAssembly and AllowDegradedPrinting.
        *  The permissions can be combined by ORing them.
        * @param strength <code>true</code> for 128 bit key length, <code>false</code> for 40 bit key length
        * @param userPassword the user password. Can be null or empty
        * @param ownerPassword the owner password. Can be null or empty
        * @param permissions the user permissions
        * @throws DocumentException if the document is already open
        */
        public void SetEncryption(bool strength, String userPassword, String ownerPassword, int permissions) {
            SetEncryption(GetISOBytes(userPassword), GetISOBytes(ownerPassword), permissions, strength);
        }
        
        /**
        * Sets the encryption options for this document. The userPassword and the
        *  ownerPassword can be null or have zero length. In this case the ownerPassword
        *  is replaced by a random string. The open permissions for the document can be
        *  AllowPrinting, AllowModifyContents, AllowCopy, AllowModifyAnnotations,
        *  AllowFillIn, AllowScreenReaders, AllowAssembly and AllowDegradedPrinting.
        *  The permissions can be combined by ORing them.
        * @param encryptionType the type of encryption. It can be one of STANDARD_ENCRYPTION_40, STANDARD_ENCRYPTION_128 or ENCRYPTION_AES128.
        * Optionally DO_NOT_ENCRYPT_METADATA can be ored to output the metadata in cleartext
        * @param userPassword the user password. Can be null or empty
        * @param ownerPassword the owner password. Can be null or empty
        * @param permissions the user permissions
        * @throws DocumentException if the document is already open
        */
        public void SetEncryption(int encryptionType, String userPassword, String ownerPassword, int permissions) {
            SetEncryption(GetISOBytes(userPassword), GetISOBytes(ownerPassword), permissions, encryptionType);
        }
        
    //  [F2] compression

        /**
        * Holds value of property fullCompression.
        */
        protected bool fullCompression = false;

        /**
        * Gets the 1.5 compression status.
        * @return <code>true</code> if the 1.5 compression is on
        */
        public bool FullCompression {
            get {
                return this.fullCompression;
            }
        }
        
        /**
        * Sets the document's compression to the new 1.5 mode with object streams and xref
        * streams. It can be set at any time but once set it can't be unset.
        * <p>
        * If set before opening the document it will also set the pdf version to 1.5.
        */
        public void SetFullCompression() {
            this.fullCompression = true;
            SetAtLeastPdfVersion(VERSION_1_5);
        }

    //  [F3] adding fonts

        /** The fonts of this document */
        protected Hashtable documentFonts = new Hashtable();

        /** The font number counter for the fonts in the document. */
        protected int fontNumber = 1;

        /**
        * Adds a <CODE>BaseFont</CODE> to the document but not to the page resources.
        * It is used for templates.
        * @param bf the <CODE>BaseFont</CODE> to add
        * @return an <CODE>Object[]</CODE> where position 0 is a <CODE>PdfName</CODE>
        * and position 1 is an <CODE>PdfIndirectReference</CODE>
        */
        internal FontDetails AddSimple(BaseFont bf) {
            if (bf.FontType == BaseFont.FONT_TYPE_DOCUMENT) {
                return new FontDetails(new PdfName("F" + (fontNumber++)), ((DocumentFont)bf).IndirectReference, bf);
            }
            FontDetails ret = (FontDetails)documentFonts[bf];
            if (ret == null) {
                PdfXConformanceImp.CheckPDFXConformance(this, PdfXConformanceImp.PDFXKEY_FONT, bf);
                ret = new FontDetails(new PdfName("F" + (fontNumber++)), body.PdfIndirectReference, bf);
                documentFonts[bf] = ret;
            }
            return ret;
        }
        
        internal void EliminateFontSubset(PdfDictionary fonts) {
            foreach (FontDetails ft in documentFonts.Values) {
                if (fonts.Get(ft.FontName) != null)
                    ft.Subset = false;
            }
        }

    //  [F4] adding (and releasing) form XObjects

        /** The form XObjects in this document. The key is the xref and the value
            is Object[]{PdfName, template}.*/
        protected Hashtable formXObjects = new Hashtable();
        
        /** The name counter for the form XObjects name. */
        protected int formXObjectsCounter = 1;

        /**
        * Adds a template to the document but not to the page resources.
        * @param template the template to add
        * @param forcedName the template name, rather than a generated one. Can be null
        * @return the <CODE>PdfName</CODE> for this template
        */        
        internal PdfName AddDirectTemplateSimple(PdfTemplate template, PdfName forcedName) {
            PdfIndirectReference refa = template.IndirectReference;
            Object[] obj = (Object[])formXObjects[refa];
            PdfName name = null;
            if (obj == null) {
                if (forcedName == null) {
                    name = new PdfName("Xf" + formXObjectsCounter);
                    ++formXObjectsCounter;
                }
                else
                    name = forcedName;
                if (template.Type == PdfTemplate.TYPE_IMPORTED) {
                    // If we got here from PdfCopy we'll have to fill importedPages
                    PdfImportedPage ip = (PdfImportedPage)template;
                    PdfReader r = ip.PdfReaderInstance.Reader;
                    if (!importedPages.ContainsKey(r)) {
                        importedPages[r] = ip.PdfReaderInstance;
                    }
                    template = null;
                }
                formXObjects[refa] = new Object[]{name, template};
            }
            else
                name = (PdfName)obj[0];
            return name;
        }

        /**
        * Releases the memory used by a template by writing it to the output. The template
        * can still be added to any content but changes to the template itself won't have
        * any effect.
        * @param tp the template to release
        * @throws IOException on error
        */    
        public void ReleaseTemplate(PdfTemplate tp) {
            PdfIndirectReference refi = tp.IndirectReference;
            Object[] objs = (Object[])formXObjects[refi];
            if (objs == null || objs[1] == null)
                return;
            PdfTemplate template = (PdfTemplate)objs[1];
            if (template.IndirectReference is PRIndirectReference)
                return;
            if (template.Type == PdfTemplate.TYPE_TEMPLATE) {
                AddToBody(template.FormXObject, template.IndirectReference);
                objs[1] = null;
            }
        }

    //  [F5] adding pages imported form other PDF documents

        protected Hashtable importedPages = new Hashtable();

        /** Gets a page from other PDF document. The page can be used as
        * any other PdfTemplate. Note that calling this method more than
        * once with the same parameters will retrieve the same object.
        * @param reader the PDF document where the page is
        * @param pageNumber the page number. The first page is 1
        * @return the template representing the imported page
        */
        public virtual PdfImportedPage GetImportedPage(PdfReader reader, int pageNumber) {
            PdfReaderInstance inst = (PdfReaderInstance)importedPages[reader];
            if (inst == null) {
                inst = reader.GetPdfReaderInstance(this);
                importedPages[reader] = inst;
            }
            return inst.GetImportedPage(pageNumber);
        }

        /** Writes the reader to the document and frees the memory used by it.
        * The main use is when concatenating multiple documents to keep the
        * memory usage restricted to the current appending document.
        * @param reader the <CODE>PdfReader</CODE> to free
        * @throws IOException on error
        */    
        public virtual void FreeReader(PdfReader reader) {
            currentPdfReaderInstance = (PdfReaderInstance)importedPages[reader];
            if (currentPdfReaderInstance == null)
                return;
            currentPdfReaderInstance.WriteAllPages();
            currentPdfReaderInstance = null;
            importedPages.Remove(reader);
        }

        /** Gets the current document size. This size only includes
        * the data already writen to the output stream, it does not
        * include templates or fonts. It is usefull if used with
        * <CODE>freeReader()</CODE> when concatenating many documents
        * and an idea of the current size is needed.
        * @return the approximate size without fonts or templates
        */    
        public int CurrentDocumentSize {
            get {
                return body.Offset + body.Size * 20 + 0x48;
            }
        }

        protected PdfReaderInstance currentPdfReaderInstance;

        protected internal virtual int GetNewObjectNumber(PdfReader reader, int number, int generation) {
            return currentPdfReaderInstance.GetNewObjectNumber(number, generation);
        }

        internal virtual RandomAccessFileOrArray GetReaderFile(PdfReader reader) {
            return currentPdfReaderInstance.ReaderFile;
        }

    //  [F6] spot colors

        /** The colors of this document */
        protected Hashtable documentColors = new Hashtable();

        /** The color number counter for the colors in the document. */
        protected int colorNumber = 1;

        internal PdfName GetColorspaceName() {
            return new PdfName("CS" + (colorNumber++));
        }

        /**
        * Adds a <CODE>SpotColor</CODE> to the document but not to the page resources.
        * @param spc the <CODE>SpotColor</CODE> to add
        * @return an <CODE>Object[]</CODE> where position 0 is a <CODE>PdfName</CODE>
        * and position 1 is an <CODE>PdfIndirectReference</CODE>
        */
        internal ColorDetails AddSimple(PdfSpotColor spc) {
            ColorDetails ret = (ColorDetails)documentColors[spc];
            if (ret == null) {
                ret = new ColorDetails(GetColorspaceName(), body.PdfIndirectReference, spc);
                documentColors[spc] = ret;
            }
            return ret;
        }

    //  [F7] document patterns

        /** The patterns of this document */
        protected Hashtable documentPatterns = new Hashtable();

        /** The patten number counter for the colors in the document. */
        protected int patternNumber = 1;
        
        internal PdfName AddSimplePattern(PdfPatternPainter painter) {
            PdfName name = (PdfName)documentPatterns[painter];
            if ( name == null ) {
                name = new PdfName("P" + patternNumber);
                ++patternNumber;
                documentPatterns[painter] = name;
            }
            return name;
        }
        
    //  [F8] shading patterns
        
        protected Hashtable documentShadingPatterns = new Hashtable();
        
        internal void AddSimpleShadingPattern(PdfShadingPattern shading) {
            if (!documentShadingPatterns.ContainsKey(shading)) {
                shading.Name = patternNumber;
                ++patternNumber;
                documentShadingPatterns[shading] = null;
                AddSimpleShading(shading.Shading);
            }
        }

    //  [F9] document shadings

        protected Hashtable documentShadings = new Hashtable();

        internal void AddSimpleShading(PdfShading shading) {
            if (!documentShadings.ContainsKey(shading)) {
                documentShadings[shading] = null;
                shading.Name = documentShadings.Count;
            }
        }

    // [F10] extended graphics state (for instance for transparency)

        protected Hashtable documentExtGState = new Hashtable();

        internal PdfObject[] AddSimpleExtGState(PdfDictionary gstate) {
            if (!documentExtGState.ContainsKey(gstate)) {
                PdfXConformanceImp.CheckPDFXConformance(this, PdfXConformanceImp.PDFXKEY_GSTATE, gstate);
                documentExtGState[gstate] = new PdfObject[]{new PdfName("GS" + (documentExtGState.Count + 1)), PdfIndirectReference};
            }
            return (PdfObject[])documentExtGState[gstate];
        }

    //  [F11] adding properties (OCG, marked content)

        protected Hashtable documentProperties = new Hashtable();

        internal PdfObject[] AddSimpleProperty(Object prop, PdfIndirectReference refi) {
            if (!documentProperties.ContainsKey(prop)) {
                if (prop is IPdfOCG)
                    PdfXConformanceImp.CheckPDFXConformance(this, PdfXConformanceImp.PDFXKEY_LAYER, null);
                documentProperties[prop] = new PdfObject[]{new PdfName("Pr" + (documentProperties.Count + 1)), refi};
            }
            return (PdfObject[])documentProperties[prop];
        }

        internal bool PropertyExists(Object prop) {
            return documentProperties.ContainsKey(prop);
        }
        
    //  [F12] tagged PDF
        
        protected bool tagged = false;        
        protected PdfStructureTreeRoot structureTreeRoot;

        /**
        * Mark this document for tagging. It must be called before open.
        */    
        public void SetTagged() {
            if (open)
                throw new ArgumentException("Tagging must be set before opening the document.");
            tagged = true;
        }
        
        /**
        * Check if the document is marked for tagging.
        * @return <CODE>true</CODE> if the document is marked for tagging
        */    
        public bool IsTagged() {
            return tagged;
        }
        
        /**
        * Gets the structure tree root. If the document is not marked for tagging it will return <CODE>null</CODE>.
        * @return the structure tree root
        */    
        public PdfStructureTreeRoot StructureTreeRoot {
            get {
                if (tagged && structureTreeRoot == null)
                    structureTreeRoot = new PdfStructureTreeRoot(this);
                return structureTreeRoot;
            }
        }

    //  [F13] Optional Content Groups    

        protected Hashtable documentOCG = new Hashtable();
        protected ArrayList documentOCGorder = new ArrayList();
        protected PdfOCProperties vOCProperties;
        protected PdfArray OCGRadioGroup = new PdfArray();
        protected PdfArray OCGLocked = new PdfArray();
        
        /**
        * Gets the <B>Optional Content Properties Dictionary</B>. Each call fills the dictionary with the current layer
        * state. It's advisable to only call this method right before close and do any modifications
        * at that time.
        * @return the Optional Content Properties Dictionary
        */    
        public PdfOCProperties OCProperties {
            get {
                FillOCProperties(true);
                return vOCProperties;
            }
        }
        
        /**
        * Sets a collection of optional content groups whose states are intended to follow
        * a "radio button" paradigm. That is, the state of at most one optional
        * content group in the array should be ON at a time: if one group is turned
        * ON, all others must be turned OFF.
        * @param group the radio group
        */    
        public void AddOCGRadioGroup(ArrayList group) {
            PdfArray ar = new PdfArray();
            for (int k = 0; k < group.Count; ++k) {
                PdfLayer layer = (PdfLayer)group[k];
                if (layer.Title == null)
                    ar.Add(layer.Ref);
            }
            if (ar.Size == 0)
                return;
            OCGRadioGroup.Add(ar);
        }
        
        /**
        * Use this method to lock an optional content group.
        * The state of a locked group cannot be changed through the user interface
        * of a viewer application. Producers can use this entry to prevent the visibility
        * of content that depends on these groups from being changed by users.
        * @param layer the layer that needs to be added to the array of locked OCGs
        * @since   2.1.2
        */    
        public void LockLayer(PdfLayer layer) {
            OCGLocked.Add(layer.Ref);
        }
        
        private static void GetOCGOrder(PdfArray order, PdfLayer layer) {
            if (!layer.OnPanel)
                return;
            if (layer.Title == null)
                order.Add(layer.Ref);
            ArrayList children = layer.Children;
            if (children == null)
                return;
            PdfArray kids = new PdfArray();
            if (layer.Title != null)
                kids.Add(new PdfString(layer.Title, PdfObject.TEXT_UNICODE));
            for (int k = 0; k < children.Count; ++k) {
                GetOCGOrder(kids, (PdfLayer)children[k]);
            }
            if (kids.Size > 0)
                order.Add(kids);
        }
        
        private void AddASEvent(PdfName eventa, PdfName category) {
            PdfArray arr = new PdfArray();
            foreach (PdfLayer layer in documentOCG.Keys) {
                PdfDictionary usage = (PdfDictionary)layer.Get(PdfName.USAGE);
                if (usage != null && usage.Get(category) != null)
                    arr.Add(layer.Ref);
            }
            if (arr.Size == 0)
                return;
            PdfDictionary d = (PdfDictionary)vOCProperties.Get(PdfName.D);
            PdfArray arras = (PdfArray)d.Get(PdfName.AS);
            if (arras == null) {
                arras = new PdfArray();
                d.Put(PdfName.AS, arras);
            }
            PdfDictionary asa = new PdfDictionary();
            asa.Put(PdfName.EVENT, eventa);
            asa.Put(PdfName.CATEGORY, new PdfArray(category));
            asa.Put(PdfName.OCGS, arr);
            arras.Add(asa);
        }
        
        protected void FillOCProperties(bool erase) {
            if (vOCProperties == null)
                vOCProperties = new PdfOCProperties();
            if (erase) {
                vOCProperties.Remove(PdfName.OCGS);
                vOCProperties.Remove(PdfName.D);
            }
            if (vOCProperties.Get(PdfName.OCGS) == null) {
                PdfArray gr = new PdfArray();
                foreach (PdfLayer layer in documentOCG.Keys) {
                    gr.Add(layer.Ref);
                }
                vOCProperties.Put(PdfName.OCGS, gr);
            }
            if (vOCProperties.Get(PdfName.D) != null)
                return;
            ArrayList docOrder = new ArrayList(documentOCGorder);
            for (ListIterator it = new ListIterator(docOrder); it.HasNext();) {
                PdfLayer layer = (PdfLayer)it.Next();
                if (layer.Parent != null)
                    it.Remove();
            }
            PdfArray order = new PdfArray();
            foreach (PdfLayer layer in docOrder) {
                GetOCGOrder(order, layer);
            }
            PdfDictionary d = new PdfDictionary();
            vOCProperties.Put(PdfName.D, d);
            d.Put(PdfName.ORDER, order);
            PdfArray grx = new PdfArray();
            foreach (PdfLayer layer in documentOCG.Keys) {
                if (!layer.On)
                    grx.Add(layer.Ref);
            }
            if (grx.Size > 0)
                d.Put(PdfName.OFF, grx);
            if (OCGRadioGroup.Size > 0)
                d.Put(PdfName.RBGROUPS, OCGRadioGroup);
            if (OCGLocked.Size > 0)
                d.Put(PdfName.LOCKED, OCGLocked);
            AddASEvent(PdfName.VIEW, PdfName.ZOOM);
            AddASEvent(PdfName.VIEW, PdfName.VIEW);
            AddASEvent(PdfName.PRINT, PdfName.PRINT);
            AddASEvent(PdfName.EXPORT, PdfName.EXPORT);
            d.Put(PdfName.LISTMODE, PdfName.VISIBLEPAGES);
        }
        
        internal void RegisterLayer(IPdfOCG layer) {
            PdfXConformanceImp.CheckPDFXConformance(this, PdfXConformanceImp.PDFXKEY_LAYER, null);
            if (layer is PdfLayer) {
                PdfLayer la = (PdfLayer)layer;
                if (la.Title == null) {
                    if (!documentOCG.ContainsKey(layer)) {
                        documentOCG[layer] = null;
                        documentOCGorder.Add(layer);
                    }
                }
                else {
                    documentOCGorder.Add(layer);
                }
            }
            else
                throw new ArgumentException("Only PdfLayer is accepted.");
        }
        
    //  User methods to change aspects of the page
        
    //  [U1] page size

        /**
        * Gives the size of the media box.
        * @return a Rectangle
        */
        public Rectangle PageSize {
            get {
                return pdf.PageSize;
            }
        }

        /** Sets the crop box. The crop box should not be rotated even if the
        * page is rotated. This change only takes effect in the next
        * page.
        * @param crop the crop box
        */
        public virtual Rectangle CropBoxSize {
            set {
                pdf.CropBoxSize = value;
            }
        }
        
        /**
        * Sets the page box sizes. Allowed names are: "crop", "trim", "art" and "bleed".
        * @param boxName the box size
        * @param size the size
        */    
        public void SetBoxSize(String boxName, Rectangle size) {
            pdf.SetBoxSize(boxName, size);
        }
        
        /**
        * Gives the size of a trim, art, crop or bleed box, or null if not defined.
        * @param boxName crop, trim, art or bleed
        */
        public Rectangle GetBoxSize(String boxName) {
            return pdf.GetBoxSize(boxName);
        }

    //  [U2] take care of empty pages
        
        /**
        * If you use SetPageEmpty(false), invoking NewPage() after a blank page will add a newPage.
        * @param pageEmpty the state
        */
        public bool PageEmpty {
            set {
                pdf.PageEmpty = value;
            }
        }
        
    //  [U3] page actions (open and close)

        /** action value */
        public static readonly PdfName PAGE_OPEN = PdfName.O;
        /** action value */
        public static readonly PdfName PAGE_CLOSE = PdfName.C;
        
        /** Sets the open and close page additional action.
        * @param actionType the action type. It can be <CODE>PdfWriter.PAGE_OPEN</CODE>
        * or <CODE>PdfWriter.PAGE_CLOSE</CODE>
        * @param action the action to perform
        * @throws PdfException if the action type is invalid
        */    
        public virtual void SetPageAction(PdfName actionType, PdfAction action) {
            if (!actionType.Equals(PAGE_OPEN) && !actionType.Equals(PAGE_CLOSE))
                throw new PdfException("Invalid page additional action type: " + actionType.ToString());
            pdf.SetPageAction(actionType, action);
        }
        
        /**
        * Sets the display duration for the page (for presentations)
        * @param seconds   the number of seconds to display the page
        */
        public virtual int Duration {
            set {
                pdf.Duration = value;
            }
        }
        
        /**
        * Sets the transition for the page
        * @param transition   the Transition object
        */
        public virtual PdfTransition Transition {
            set {
                pdf.Transition = value;
            }
        }
        
    //  [U4] Thumbnail image

        /**
        * Sets the the thumbnail image for the current page.
        * @param image the image
        * @throws PdfException on error
        * @throws DocumentException or error
        */    
        public virtual Image Thumbnail {
            set {
                pdf.Thumbnail = value;
            }
        }

    //  [U5] Transparency groups
        
        /**
        * A group attributes dictionary specifying the attributes
        * of the page’s page group for use in the transparent
        * imaging model
        */
        protected PdfDictionary group;
        
        public PdfDictionary Group {
            get {
                return this.group;
            }
            set {
                group = value;
            }
        }
        
    //  [U6] space char ratio
        
        /** The default space-char ratio. */    
        public const float SPACE_CHAR_RATIO_DEFAULT = 2.5f;
        /** Disable the inter-character spacing. */    
        public const float NO_SPACE_CHAR_RATIO = 10000000f;
        
        /**
        * The ratio between the extra word spacing and the extra character spacing.
        * Extra word spacing will grow <CODE>ratio</CODE> times more than extra character spacing.
        */
        private float spaceCharRatio = SPACE_CHAR_RATIO_DEFAULT;

        /** Sets the ratio between the extra word spacing and the extra character spacing
        * when the text is fully justified.
        * Extra word spacing will grow <CODE>spaceCharRatio</CODE> times more than extra character spacing.
        * If the ratio is <CODE>PdfWriter.NO_SPACE_CHAR_RATIO</CODE> then the extra character spacing
        * will be zero.
        * @param spaceCharRatio the ratio between the extra word spacing and the extra character spacing
        */
        public virtual float SpaceCharRatio {
            set {
                if (value < 0.001f)
                    this.spaceCharRatio = 0.001f;
                else
                    this.spaceCharRatio = value;
            }
            get {
                return spaceCharRatio;
            }
        }

    //  [U7] run direction (doesn't actually do anything)

        /** Use the default run direction. */    
        public const int RUN_DIRECTION_DEFAULT = 0;
        /** Do not use bidirectional reordering. */    
        public const int RUN_DIRECTION_NO_BIDI = 1;
        /** Use bidirectional reordering with left-to-right
        * preferential run direction.
        */    
        public const int RUN_DIRECTION_LTR = 2;
        /** Use bidirectional reordering with right-to-left
        * preferential run direction.
        */    
        public const int RUN_DIRECTION_RTL = 3;
        protected int runDirection = RUN_DIRECTION_NO_BIDI;

        /** Sets the run direction. This is only used as a placeholder
        * as it does not affect anything.
        * @param runDirection the run direction
        */    
        public virtual int RunDirection {
            set {
                if (value < RUN_DIRECTION_NO_BIDI || value > RUN_DIRECTION_RTL)
                    throw new Exception("Invalid run direction: " + value);
                this.runDirection = value;
            }
            get {
                return runDirection;
            }
        }

    //  [U8] user units     

        protected float userunit = 0f;

        /**
        * A UserUnit is a value that defines the default user space unit.
        * The minimum UserUnit is 1 (1 unit = 1/72 inch).
        * The maximum UserUnit is 75,000.
        * Remark that this userunit only works starting with PDF1.6!
        */
        public float Userunit {
            get {
                return userunit;
            }
            set {
                if (value < 1f || value > 75000f) throw new DocumentException("UserUnit should be a value between 1 and 75000.");
                this.userunit = value;
                SetAtLeastPdfVersion(VERSION_1_6);
            }
        }

    // Miscellaneous topics
        
    //  [M1] Color settings

        protected PdfDictionary defaultColorspace = new PdfDictionary();

        /**
        * Gets the default colorspaces.
        * @return the default colorspaces
        */    
        public PdfDictionary DefaultColorspace {
            get {
                return defaultColorspace;
            }
        }

        /**
        * Sets the default colorspace that will be applied to all the document.
        * The colorspace is only applied if another colorspace with the same name
        * is not present in the content.
        * <p>
        * The colorspace is applied immediately when creating templates and at the page
        * end for the main document content.
        * @param key the name of the colorspace. It can be <CODE>PdfName.DEFAULTGRAY</CODE>, <CODE>PdfName.DEFAULTRGB</CODE>
        * or <CODE>PdfName.DEFAULTCMYK</CODE>
        * @param cs the colorspace. A <CODE>null</CODE> or <CODE>PdfNull</CODE> removes any colorspace with the same name
        */    
        public void SetDefaultColorspace(PdfName key, PdfObject cs) {
            if (cs == null || cs.IsNull())
                defaultColorspace.Remove(key);
            defaultColorspace.Put(key, cs);
        }

    //  [M2] spot patterns

        protected Hashtable documentSpotPatterns = new Hashtable();
        protected ColorDetails patternColorspaceRGB;
        protected ColorDetails patternColorspaceGRAY;
        protected ColorDetails patternColorspaceCMYK;
        
        internal ColorDetails AddSimplePatternColorspace(Color color) {
            int type = ExtendedColor.GetType(color);
            if (type == ExtendedColor.TYPE_PATTERN || type == ExtendedColor.TYPE_SHADING)
                throw new Exception("An uncolored tile pattern can not have another pattern or shading as color.");
            switch (type) {
                case ExtendedColor.TYPE_RGB:
                    if (patternColorspaceRGB == null) {
                        patternColorspaceRGB = new ColorDetails(GetColorspaceName(), body.PdfIndirectReference, null);
                        PdfArray array = new PdfArray(PdfName.PATTERN);
                        array.Add(PdfName.DEVICERGB);
                        AddToBody(array, patternColorspaceRGB.IndirectReference);
                    }
                    return patternColorspaceRGB;
                case ExtendedColor.TYPE_CMYK:
                    if (patternColorspaceCMYK == null) {
                        patternColorspaceCMYK = new ColorDetails(GetColorspaceName(), body.PdfIndirectReference, null);
                        PdfArray array = new PdfArray(PdfName.PATTERN);
                        array.Add(PdfName.DEVICECMYK);
                        AddToBody(array, patternColorspaceCMYK.IndirectReference);
                    }
                    return patternColorspaceCMYK;
                case ExtendedColor.TYPE_GRAY:
                    if (patternColorspaceGRAY == null) {
                        patternColorspaceGRAY = new ColorDetails(GetColorspaceName(), body.PdfIndirectReference, null);
                        PdfArray array = new PdfArray(PdfName.PATTERN);
                        array.Add(PdfName.DEVICEGRAY);
                        AddToBody(array, patternColorspaceGRAY.IndirectReference);
                    }
                    return patternColorspaceGRAY;
                case ExtendedColor.TYPE_SEPARATION: {
                    ColorDetails details = AddSimple(((SpotColor)color).PdfSpotColor);
                    ColorDetails patternDetails = (ColorDetails)documentSpotPatterns[details];
                    if (patternDetails == null) {
                        patternDetails = new ColorDetails(GetColorspaceName(), body.PdfIndirectReference, null);
                        PdfArray array = new PdfArray(PdfName.PATTERN);
                        array.Add(details.IndirectReference);
                        AddToBody(array, patternDetails.IndirectReference);
                        documentSpotPatterns[details] = patternDetails;
                    }
                    return patternDetails;
                }
                default:
                    throw new Exception("Invalid color type in PdfWriter.AddSimplePatternColorspace().");
            }
        }
        
    //  [M3] Images

        /** Sets the image sequence to follow the text in strict order.
        * @param strictImageSequence new value of property strictImageSequence
        *
        */
        public bool StrictImageSequence {
            set {
                pdf.StrictImageSequence = value;
            }
            get {
                return pdf.StrictImageSequence;
            }
        }
        
        /**
        * Clears text wrapping around images (if applicable).
        * Method suggested by Pelikan Stephan
        * @throws DocumentException
        */
        public void ClearTextWrap() {
            pdf.ClearTextWrap();
        }

        /** Dictionary, containing all the images of the PDF document */
        protected PdfDictionary imageDictionary = new PdfDictionary();
        
        /** This is the list with all the images in the document. */
        private Hashtable images = new Hashtable();
        
        /**
        * Adds an image to the document but not to the page resources. It is used with
        * templates and <CODE>Document.Add(Image)</CODE>.
        * @param image the <CODE>Image</CODE> to add
        * @return the name of the image added
        * @throws PdfException on error
        * @throws DocumentException on error
        */
        public PdfName AddDirectImageSimple(Image image) {
            return AddDirectImageSimple(image, null);
        }
        
        /**
        * Adds an image to the document but not to the page resources. It is used with
        * templates and <CODE>Document.Add(Image)</CODE>.
        * @param image the <CODE>Image</CODE> to add
        * @param fixedRef the reference to used. It may be <CODE>null</CODE>,
        * a <CODE>PdfIndirectReference</CODE> or a <CODE>PRIndirectReference</CODE>.
        * @return the name of the image added
        * @throws PdfException on error
        * @throws DocumentException on error
        */
        public PdfName AddDirectImageSimple(Image image, PdfIndirectReference fixedRef) {
            PdfName name;
            // if the images is already added, just retrieve the name
            if (images.ContainsKey(image.MySerialId)) {
                name = (PdfName) images[image.MySerialId];
            }
            // if it's a new image, add it to the document
            else {
                if (image.IsImgTemplate()) {
                    name = new PdfName("img" + images.Count);
                    if (image is ImgWMF){
                        ImgWMF wmf = (ImgWMF)image;
                        wmf.ReadWMF(PdfTemplate.CreateTemplate(this, 0, 0));
                    }
                }
                else {
                    PdfIndirectReference dref = image.DirectReference;
                    if (dref != null) {
                        PdfName rname = new PdfName("img" + images.Count);
                        images[image.MySerialId] = rname;
                        imageDictionary.Put(rname, dref);
                        return rname;
                    }
                    Image maskImage = image.ImageMask;
                    PdfIndirectReference maskRef = null;
                    if (maskImage != null) {
                        PdfName mname = (PdfName)images[maskImage.MySerialId];
                        maskRef = GetImageReference(mname);
                    }
                    PdfImage i = new PdfImage(image, "img" + images.Count, maskRef);
                    if (image.HasICCProfile()) {
                        PdfICCBased icc = new PdfICCBased(image.TagICC);
                        PdfIndirectReference iccRef = Add(icc);
                        PdfArray iccArray = new PdfArray();
                        iccArray.Add(PdfName.ICCBASED);
                        iccArray.Add(iccRef);
                        PdfObject colorspace = i.Get(PdfName.COLORSPACE);
                        if (colorspace != null && colorspace.IsArray()) {
                            ArrayList ar = ((PdfArray)colorspace).ArrayList;
                            if (ar.Count > 1 && PdfName.INDEXED.Equals(ar[0]))
                                ar[1] = iccArray;
                            else
                                i.Put(PdfName.COLORSPACE, iccArray);
                        }
                        else
                            i.Put(PdfName.COLORSPACE, iccArray);
                    }
                    Add(i, fixedRef);
                    name = i.Name;
                }
                images[image.MySerialId] = name;
            }
            return name;
        }

        /**
        * Writes a <CODE>PdfImage</CODE> to the outputstream.
        *
        * @param pdfImage the image to be added
        * @return a <CODE>PdfIndirectReference</CODE> to the encapsulated image
        * @throws PdfException when a document isn't open yet, or has been closed
        */        
        internal virtual PdfIndirectReference Add(PdfImage pdfImage, PdfIndirectReference fixedRef) {
            if (! imageDictionary.Contains(pdfImage.Name)) {
                PdfXConformanceImp.CheckPDFXConformance(this, PdfXConformanceImp.PDFXKEY_IMAGE, pdfImage);
                if (fixedRef is PRIndirectReference) {
                    PRIndirectReference r2 = (PRIndirectReference)fixedRef;
                    fixedRef = new PdfIndirectReference(0, GetNewObjectNumber(r2.Reader, r2.Number, r2.Generation));
                }
                if (fixedRef == null)
                    fixedRef = AddToBody(pdfImage).IndirectReference;
                else
                    AddToBody(pdfImage, fixedRef);
                imageDictionary.Put(pdfImage.Name, fixedRef);
                return fixedRef;
            }
            return (PdfIndirectReference)imageDictionary.Get(pdfImage.Name);
        }
        
        /**
        * return the <CODE>PdfIndirectReference</CODE> to the image with a given name.
        *
        * @param name the name of the image
        * @return a <CODE>PdfIndirectReference</CODE>
        */
        internal virtual PdfIndirectReference GetImageReference(PdfName name) {
            return (PdfIndirectReference) imageDictionary.Get(name);
        }
        
        protected virtual PdfIndirectReference Add(PdfICCBased icc) {
            PdfIndirectObject objecta;
            objecta = AddToBody(icc);
            return objecta.IndirectReference;
        }
        
    //  [M4] Old table functionality; do we still need it?

        /**
        * Checks if a <CODE>Table</CODE> fits the current page of the <CODE>PdfDocument</CODE>.
        *
        * @param    table   the table that has to be checked
        * @param    margin  a certain margin
        * @return   <CODE>true</CODE> if the <CODE>Table</CODE> fits the page, <CODE>false</CODE> otherwise.
        */        
        public bool FitsPage(Table table, float margin) {
            return pdf.GetBottom(table) > pdf.IndentBottom + margin;
        }
        
        /**
        * Checks if a <CODE>Table</CODE> fits the current page of the <CODE>PdfDocument</CODE>.
        *
        * @param    table   the table that has to be checked
        * @return   <CODE>true</CODE> if the <CODE>Table</CODE> fits the page, <CODE>false</CODE> otherwise.
        */        
        public bool FitsPage(Table table) {
            return FitsPage(table, 0);
        }
        
        //  [F12] tagged PDF
        /**
        * A flag indicating the presence of structure elements that contain user properties attributes.
        */
        private bool userProperties;

        /**
        * Sets the flag indicating the presence of structure elements that contain user properties attributes.
        * @param userProperties the user properties flag
        */
        public bool UserProperties {
            set {
                userProperties = value;
            }
            get {
                return userProperties;
            }
        }

        /**
        * Holds value of property RGBTranparency.
        */
        private bool rgbTransparencyBlending;

        /**
        * Sets the transparency blending colorspace to RGB. The default blending colorspace is
        * CMYK and will result in faded colors in the screen and in printing. Calling this method
        * will return the RGB colors to what is expected. The RGB blending will be applied to all subsequent pages
        * until other value is set.
        * Note that this is a generic solution that may not work in all cases.
        * @param rgbTransparencyBlending <code>true</code> to set the transparency blending colorspace to RGB, <code>false</code>
        * to use the default blending colorspace
        */
        public bool RgbTransparencyBlending {
            get {
                return this.rgbTransparencyBlending;
            }
            set {
                this.rgbTransparencyBlending = value;
            }
        }
    }
}