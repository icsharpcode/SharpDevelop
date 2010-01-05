using System;
using System.IO;
using System.Collections;
using iTextSharp.text.pdf;
using iTextSharp.text.xml.simpleparser;
/*
 * $Id: XmpWriter.cs,v 1.10 2008/05/13 11:26:16 psoares33 Exp $
 * 
 *
 * Copyright 2005 by Bruno Lowagie.
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
 * the Initial Developer are Copyright (C) 1999-2005 by Bruno Lowagie.
 * All Rights Reserved.
 * Co-Developer of the code is Paulo Soares. Portions created by the Co-Developer
 * are Copyright (C) 2000-2005 by Paulo Soares. All Rights Reserved.
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
 * of this file under either the MPL or the GNU LIBRARY GENERAL PUBLIC LICENSE 
 *
 * This library is free software; you can redistribute it and/or modify it
 * under the terms of the MPL as stated above or under the terms of the GNU
 * Library General Public License as published by the Free Software Foundation;
 * either version 2 of the License, or any later version.
 *
 * This library is distributed in the hope that it will be useful, but WITHOUT
 * ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS
 * FOR A PARTICULAR PURPOSE. See the GNU LIBRARY GENERAL PUBLIC LICENSE for more
 * details.
 *
 * If you didn't download this code from the following link, you should check if
 * you aren't using an obsolete version:
 * http://www.lowagie.com/iText/
 */

namespace iTextSharp.text.xml.xmp {

    /**
    * With this class you can create an Xmp Stream that can be used for adding
    * Metadata to a PDF Dictionary. Remark that this class doesn't cover the
    * complete XMP specification. 
    */
    public class XmpWriter {

        /** A possible charset for the XMP. */
        public const String UTF8 = "UTF-8";
        /** A possible charset for the XMP. */
        public const String UTF16 = "UTF-16";
        /** A possible charset for the XMP. */
        public const String UTF16BE = "UTF-16BE";
        /** A possible charset for the XMP. */
        public const String UTF16LE = "UTF-16LE";
        
        /** String used to fill the extra space. */
        public const String EXTRASPACE = "                                                                                                   \n";
        
        /** You can add some extra space in the XMP packet; 1 unit in this variable represents 100 spaces and a newline. */
        protected int extraSpace;
        
        /** The writer to which you can write bytes for the XMP stream. */
        protected StreamWriter writer;
        
        /** The about string that goes into the rdf:Description tags. */
        protected String about;
        
        /** The end attribute. */
        protected char end = 'w';
        
        /**
        * Creates an XmpWriter. 
        * @param os
        * @param utfEncoding
        * @param extraSpace
        * @throws IOException
        */
        public XmpWriter(Stream os, string utfEncoding, int extraSpace) {
            this.extraSpace = extraSpace;
            writer = new StreamWriter(os, new EncodingNoPreamble(IanaEncodings.GetEncodingEncoding(utfEncoding)));
            writer.Write("<?xpacket begin=\"\uFEFF\" id=\"W5M0MpCehiHzreSzNTczkc9d\"?>\n");
            writer.Write("<x:xmpmeta xmlns:x=\"adobe:ns:meta/\">\n");
            writer.Write("<rdf:RDF xmlns:rdf=\"http://www.w3.org/1999/02/22-rdf-syntax-ns#\">\n");
            about = "";
        }
        
        /**
        * Creates an XmpWriter.
        * @param os
        * @throws IOException
        */
        public XmpWriter(Stream os) : this(os, UTF8, 20) {
        }
        
        /** Sets the XMP to read-only */
        public void SetReadOnly() {
            end = 'r';
        }
        
        /**
        * @param about The about to set.
        */
        public String About {
            set {
                this.about = value;
            }
        }
        
        /**
        * Adds an rdf:Description.
        * @param xmlns
        * @param content
        * @throws IOException
        */
        public void AddRdfDescription(String xmlns, String content) {
            writer.Write("<rdf:Description rdf:about=\"");
            writer.Write(about);
            writer.Write("\" ");
            writer.Write(xmlns);
            writer.Write(">");
            writer.Write(content);
            writer.Write("</rdf:Description>\n");
        }
        
        /**
        * Adds an rdf:Description.
        * @param s
        * @throws IOException
        */
        public void AddRdfDescription(XmpSchema s) {
            writer.Write("<rdf:Description rdf:about=\"");
            writer.Write(about);
            writer.Write("\" ");
            writer.Write(s.Xmlns);
            writer.Write(">");
            writer.Write(s.ToString());
            writer.Write("</rdf:Description>\n");
        }
        
        /**
        * Flushes and closes the XmpWriter.
        * @throws IOException
        */
        public void Close() {
            writer.Write("</rdf:RDF>");
            writer.Write("</x:xmpmeta>\n");
            for (int i = 0; i < extraSpace; i++) {
                writer.Write(EXTRASPACE);
            }
            writer.Write("<?xpacket end=\"" + end + "\"?>");
            writer.Flush();
            writer.Close();
        }
        
        /**
        * @param os
        * @param info
        * @throws IOException
        */
        public XmpWriter(Stream os, PdfDictionary info, int PdfXConformance) : this(os) {
            if (info != null) {
                DublinCoreSchema dc = new DublinCoreSchema();
                PdfSchema p = new PdfSchema();
                XmpBasicSchema basic = new XmpBasicSchema();
                PdfObject obj;
                foreach (PdfName key in info.Keys) {
                    obj = info.Get(key);
                    if (obj == null)
                        continue;
                    if (PdfName.TITLE.Equals(key)) {
                        dc.AddTitle(((PdfString)obj).ToUnicodeString());
                    }
                    if (PdfName.AUTHOR.Equals(key)) {
                        dc.AddAuthor(((PdfString)obj).ToUnicodeString());
                    }
                    if (PdfName.SUBJECT.Equals(key)) {
                        dc.AddSubject(((PdfString)obj).ToUnicodeString());
                        dc.AddDescription(((PdfString)obj).ToUnicodeString());
                    }
                    if (PdfName.KEYWORDS.Equals(key)) {
                        p.AddKeywords(((PdfString)obj).ToUnicodeString());
                    }
                    if (PdfName.CREATOR.Equals(key)) {
                        basic.AddCreatorTool(((PdfString)obj).ToUnicodeString());
                    }
                    if (PdfName.PRODUCER.Equals(key)) {
                        p.AddProducer(((PdfString)obj).ToUnicodeString());
                    }
                    if (PdfName.CREATIONDATE.Equals(key)) {
                        basic.AddCreateDate(((PdfDate)obj).GetW3CDate());
                    }
                    if (PdfName.MODDATE.Equals(key)) {
                        basic.AddModDate(((PdfDate)obj).GetW3CDate());
                    }
                }
                if (dc.Count > 0) AddRdfDescription(dc);
                if (p.Count > 0) AddRdfDescription(p);
                if (basic.Count > 0) AddRdfDescription(basic);
                if (PdfXConformance == PdfWriter.PDFA1A || PdfXConformance == PdfWriter.PDFA1B) {
                    PdfA1Schema a1 = new PdfA1Schema();
                    if (PdfXConformance == PdfWriter.PDFA1A)
                        a1.AddConformance("A");
                    else
                        a1.AddConformance("B");
                    AddRdfDescription(a1);
                }
            }
        }
        
        /**
        * @param os
        * @param info
        * @throws IOException
        */
        public XmpWriter(Stream os, Hashtable info) : this(os) {
            if (info != null) {
                DublinCoreSchema dc = new DublinCoreSchema();
                PdfSchema p = new PdfSchema();
                XmpBasicSchema basic = new XmpBasicSchema();
                String value;
                foreach (DictionaryEntry entry in info) {
                    String key = (String)entry.Key;
                    value = (String)entry.Value;
                    if (value == null)
                        continue;
                    if ("Title".Equals(key)) {
                        dc.AddTitle(value);
                    }
                    if ("Author".Equals(key)) {
                        dc.AddAuthor(value);
                    }
                    if ("Subject".Equals(key)) {
                        dc.AddSubject(value);
                        dc.AddDescription(value);
                    }
                    if ("Keywords".Equals(key)) {
                        p.AddKeywords(value);
                    }
                    if ("Creator".Equals(key)) {
                        basic.AddCreatorTool(value);
                    }
                    if ("Producer".Equals(key)) {
                        p.AddProducer(value);
                    }
                    if ("CreationDate".Equals(key)) {
                        basic.AddCreateDate(PdfDate.GetW3CDate(value));
                    }
                    if ("ModDate".Equals(key)) {
                        basic.AddModDate(PdfDate.GetW3CDate(value));
                    }
                }
                if (dc.Count > 0) AddRdfDescription(dc);
                if (p.Count > 0) AddRdfDescription(p);
                if (basic.Count > 0) AddRdfDescription(basic);
            }
        }
    }
}