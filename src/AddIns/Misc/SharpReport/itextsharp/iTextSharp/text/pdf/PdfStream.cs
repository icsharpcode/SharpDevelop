using System;
using System.IO;
using System.Collections;

using System.util.zlib;

/*
 * $Id: PdfStream.cs,v 1.12 2008/05/13 11:25:23 psoares33 Exp $
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
     * <CODE>PdfStream</CODE> is the Pdf stream object.
     * <P>
     * A stream, like a string, is a sequence of characters. However, an application can
     * read a small portion of a stream at a time, while a string must be read in its entirety.
     * For this reason, objects with potentially large amounts of data, such as images and
     * page descriptions, are represented as streams.<BR>
     * A stream consists of a dictionary that describes a sequence of characters, followed by
     * the keyword <B>stream</B>, followed by zero or more lines of characters, followed by
     * the keyword <B>endstream</B>.<BR>
     * All streams must be <CODE>PdfIndirectObject</CODE>s. The stream dictionary must be a direct
     * object. The keyword <B>stream</B> that follows the stream dictionary should be followed by
     * a carriage return and linefeed or just a linefeed.<BR>
     * Remark: In this version only the FLATEDECODE-filter is supported.<BR>
     * This object is described in the 'Portable Document Format Reference Manual version 1.3'
     * section 4.8 (page 41-53).<BR>
     *
     * @see        PdfObject
     * @see        PdfDictionary
     */

    public class PdfStream : PdfDictionary {
    
        // membervariables
    
        /** is the stream compressed? */
        protected bool compressed = false;
    
        protected MemoryStream streamBytes = null;
    
        protected Stream inputStream;
        protected PdfIndirectReference iref;
        protected int inputStreamLength = -1;
        protected PdfWriter writer;
        protected int rawLength;
        
        internal static byte[] STARTSTREAM = DocWriter.GetISOBytes("stream\n");
        internal static byte[] ENDSTREAM = DocWriter.GetISOBytes("\nendstream");
        internal static int SIZESTREAM = STARTSTREAM.Length + ENDSTREAM.Length;

        // constructors
    
        /**
         * Constructs a <CODE>PdfStream</CODE>-object.
         *
         * @param        bytes            content of the new <CODE>PdfObject</CODE> as an array of <CODE>byte</CODE>.
         */
 
        public PdfStream(byte[] bytes) : base() {
            type = STREAM;
            this.bytes = bytes;
            rawLength = bytes.Length;
            Put(PdfName.LENGTH, new PdfNumber(bytes.Length));
        }
  
        /**
         * Creates an efficient stream. No temporary array is ever created. The <CODE>InputStream</CODE>
         * is totally consumed but is not closed. The general usage is:
         * <p>
         * <pre>
         * InputStream in = ...;
         * PdfStream stream = new PdfStream(in, writer);
         * stream.FlateCompress();
         * writer.AddToBody(stream);
         * stream.WriteLength();
         * in.Close();
         * </pre>
         * @param inputStream the data to write to this stream
         * @param writer the <CODE>PdfWriter</CODE> for this stream
         */    
        public PdfStream(Stream inputStream, PdfWriter writer) {
            type = STREAM;
            this.inputStream = inputStream;
            this.writer = writer;
            iref = writer.PdfIndirectReference;
            Put(PdfName.LENGTH, iref);
        }

        /**
         * Constructs a <CODE>PdfStream</CODE>-object.
         */
    
        protected PdfStream() : base() {
            type = STREAM;
        }
    
        // methods overriding some methods of PdfObject
    
        /**
         * Writes the stream length to the <CODE>PdfWriter</CODE>.
         * <p>
         * This method must be called and can only be called if the contructor {@link #PdfStream(InputStream,PdfWriter)}
         * is used to create the stream.
         * @throws IOException on error
         * @see #PdfStream(InputStream,PdfWriter)
         */
        public void WriteLength() {
            if (inputStream == null)
                throw new PdfException("WriteLength() can only be called in a contructed PdfStream(InputStream,PdfWriter).");
            if (inputStreamLength == -1)
                throw new PdfException("WriteLength() can only be called after output of the stream body.");
            writer.AddToBody(new PdfNumber(inputStreamLength), iref, false);
        }
    
        public int RawLength {
            get {
                return rawLength;
            }
        }

        // methods
    
        /**
         * Compresses the stream.
         *
         * @throws PdfException if a filter is allready defined
         */
    
        public void FlateCompress() {
            if (!Document.Compress)
                return;
            // check if the flateCompress-method has allready been
            if (compressed) {
                return;
            }
            if (inputStream != null) {
                compressed = true;
                return;
            }
            // check if a filter allready exists
            PdfObject filter = PdfReader.GetPdfObject(Get(PdfName.FILTER));
            if (filter != null) {
                if (filter.IsName()) {
                    if (PdfName.FLATEDECODE.Equals(filter))
                        return;
                }
                else if (filter.IsArray()) {
                    if (((PdfArray) filter).Contains(PdfName.FLATEDECODE))
                        return;
                }
                else {
                    throw new PdfException("Stream could not be compressed: filter is not a name or array.");
                }
            }
            // compress
            MemoryStream stream = new MemoryStream();
            ZDeflaterOutputStream zip = new ZDeflaterOutputStream(stream);
            if (streamBytes != null)
                streamBytes.WriteTo(zip);
            else
                zip.Write(bytes, 0, bytes.Length);
            //zip.Close();
            zip.Finish();
            // update the object
            streamBytes = stream;
            bytes = null;
            Put(PdfName.LENGTH, new PdfNumber(streamBytes.Length));
            if (filter == null) {
                Put(PdfName.FILTER, PdfName.FLATEDECODE);
            }
            else {
                PdfArray filters = new PdfArray(filter);
                filters.Add(PdfName.FLATEDECODE);
                Put(PdfName.FILTER, filters);
            }
            compressed = true;
        }

        protected virtual void SuperToPdf(PdfWriter writer, Stream os) {
            base.ToPdf(writer, os);
        }
    
        public override void ToPdf(PdfWriter writer, Stream os) {
            if (inputStream != null && compressed)
                Put(PdfName.FILTER, PdfName.FLATEDECODE);
            PdfEncryption crypto = null;
            if (writer != null)
                crypto = writer.Encryption;
            if (crypto != null) {
                PdfObject filter = Get(PdfName.FILTER);
                if (filter != null) {
                    if (PdfName.CRYPT.Equals(filter))
                        crypto = null;
                    else if (filter.IsArray()) {
                        ArrayList af = ((PdfArray)filter).ArrayList;
                        if (af.Count > 0 && PdfName.CRYPT.Equals(af[0]))
                            crypto = null;
                    }
                }
            }
            PdfObject nn = Get(PdfName.LENGTH);
            if (crypto != null && nn != null && nn.IsNumber()) {
                int sz = ((PdfNumber)nn).IntValue;
                Put(PdfName.LENGTH, new PdfNumber(crypto.CalculateStreamSize(sz)));
                SuperToPdf(writer, os);
                Put(PdfName.LENGTH, nn);
            }
            else
                SuperToPdf(writer, os);
            os.Write(STARTSTREAM, 0, STARTSTREAM.Length);
            if (inputStream != null) {
                rawLength = 0;
                ZDeflaterOutputStream def = null;
                OutputStreamCounter osc = new OutputStreamCounter(os);
                OutputStreamEncryption ose = null;
                Stream fout = osc;
                if (crypto != null)
                    fout = ose = crypto.GetEncryptionStream(fout);
                if (compressed)    
                    fout = def = new ZDeflaterOutputStream(fout);
                
                byte[] buf = new byte[4192];
                while (true) {
                    int n = inputStream.Read(buf, 0, buf.Length);
                    if (n <= 0)
                        break;
                    fout.Write(buf, 0, n);
                    rawLength += n;
                }
                if (def != null)
                    def.Finish();
                if (ose != null)
                    ose.Finish();
                inputStreamLength = osc.Counter;
            }
            else {
                if (crypto == null) {
                    if (streamBytes != null)
                        streamBytes.WriteTo(os);
                    else
                        os.Write(bytes, 0, bytes.Length);
                }
                else {
                    byte[] b;
                    if (streamBytes != null) {
                        b = crypto.EncryptByteArray(streamBytes.ToArray());
                    }
                    else {
                        b = crypto.EncryptByteArray(bytes);
                    }
                    os.Write(b, 0, b.Length);
                }
            }
            os.Write(ENDSTREAM, 0, ENDSTREAM.Length);
        }
    
        /**
        * Writes the data content to an <CODE>Stream</CODE>.
        * @param os the destination to write to
        * @throws IOException on error
        */    
        public void WriteContent(Stream os) {
            if (streamBytes != null)
                streamBytes.WriteTo(os);
            else if (bytes != null)
                os.Write(bytes, 0, bytes.Length);
        }

        /**
        * @see com.lowagie.text.pdf.PdfObject#toString()
        */
        public override String ToString() {
            if (Get(PdfName.TYPE) == null) return "Stream";
            return "Stream of type: " + Get(PdfName.TYPE);
        }
    }
}
