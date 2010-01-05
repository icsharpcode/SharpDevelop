using System;
using System.IO;
using System.Net;
using iTextSharp.text.pdf.collection;
/*
 * Copyright 2003 Paulo Soares
 *
 * The contents of this file are subject to the Mozilla Public License Version 1.1
 * (the "License"); you may not use this file except inp compliance with the License.
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
 * Contributor(s): all the names of the contributors are added inp the source code
 * where applicable.
 *
 * Alternatively, the contents of this file may be used under the terms of the
 * LGPL license (the "GNU LIBRARY GENERAL PUBLIC LICENSE"), inp which case the
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
 * This library is distributed inp the hope that it will be useful, but WITHOUT
 * ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS
 * FOR A PARTICULAR PURPOSE. See the GNU Library general Public License for more
 * details.
 *
 * If you didn't download this code from the following link, you should check if
 * you aren't using an obsolete version:
 * http://www.lowagie.com/iText/
 */

namespace iTextSharp.text.pdf {
    /** Specifies a file or an URL. The file can be extern or embedded.
    *
    * @author Paulo Soares (psoares@consiste.pt)
    */
    public class PdfFileSpecification : PdfDictionary {
        protected PdfWriter writer;
        protected PdfIndirectReference refi;
        
        /** Creates a new instance of PdfFileSpecification. The static methods are preferred. */
        public PdfFileSpecification() : base(PdfName.FILESPEC) {
        }
        
        /**
        * Creates a file specification of type URL.
        * @param writer the <CODE>PdfWriter</CODE>
        * @param url the URL
        * @return the file specification
        */    
        public static PdfFileSpecification Url(PdfWriter writer, String url) {
            PdfFileSpecification fs = new PdfFileSpecification();
            fs.writer = writer;
            fs.Put(PdfName.FS, PdfName.URL);
            fs.Put(PdfName.F, new PdfString(url));
            return fs;
        }

        /**
        * Creates a file specification with the file embedded. The file may
        * come from the file system or from a byte array. The data is flate compressed.
        * @param writer the <CODE>PdfWriter</CODE>
        * @param filePath the file path
        * @param fileDisplay the file information that is presented to the user
        * @param fileStore the byte array with the file. If it is not <CODE>null</CODE>
        * it takes precedence over <CODE>filePath</CODE>
        * @throws IOException on error
        * @return the file specification
        */    
        public static PdfFileSpecification FileEmbedded(PdfWriter writer, String filePath, String fileDisplay, byte[] fileStore) {
            return FileEmbedded(writer, filePath, fileDisplay, fileStore, true);
        }
        
        
        /**
        * Creates a file specification with the file embedded. The file may
        * come from the file system or from a byte array.
        * @param writer the <CODE>PdfWriter</CODE>
        * @param filePath the file path
        * @param fileDisplay the file information that is presented to the user
        * @param fileStore the byte array with the file. If it is not <CODE>null</CODE>
        * it takes precedence over <CODE>filePath</CODE>
        * @param compress sets the compression on the data. Multimedia content will benefit little
        * from compression
        * @throws IOException on error
        * @return the file specification
        */    
        public static PdfFileSpecification FileEmbedded(PdfWriter writer, String filePath, String fileDisplay, byte[] fileStore, bool compress) {
            return FileEmbedded(writer, filePath, fileDisplay, fileStore, compress, null, null);
        }
        
        /**
        * Creates a file specification with the file embedded. The file may
        * come from the file system or from a byte array.
        * @param writer the <CODE>PdfWriter</CODE>
        * @param filePath the file path
        * @param fileDisplay the file information that is presented to the user
        * @param fileStore the byte array with the file. If it is not <CODE>null</CODE>
        * it takes precedence over <CODE>filePath</CODE>
        * @param compress sets the compression on the data. Multimedia content will benefit little
        * from compression
        * @param mimeType the optional mimeType
        * @param fileParameter the optional extra file parameters such as the creation or modification date
        * @throws IOException on error
        * @return the file specification
        */    
        public static PdfFileSpecification FileEmbedded(PdfWriter writer, String filePath, String fileDisplay, byte[] fileStore, bool compress, String mimeType, PdfDictionary fileParameter) {
            PdfFileSpecification fs = new PdfFileSpecification();
            fs.writer = writer;
            fs.Put(PdfName.F, new PdfString(fileDisplay));
            PdfStream stream;
            Stream inp = null;
            PdfIndirectReference refi;
            PdfIndirectReference refFileLength;
            try {
                refFileLength = writer.PdfIndirectReference;
                if (fileStore == null) {
                    if (File.Exists(filePath)) {
                        inp = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                    }
                    else {
                        if (filePath.StartsWith("file:/") || filePath.StartsWith("http://") || filePath.StartsWith("https://")) {
                            WebRequest w = WebRequest.Create(filePath);
                            inp = w.GetResponse().GetResponseStream();
                        }
                        else {
                            inp = BaseFont.GetResourceStream(filePath);
                            if (inp == null)
                                throw new IOException(filePath + " not found as file or resource.");
                        }
                    }
                    stream = new PdfStream(inp, writer);
                }
                else
                    stream = new PdfStream(fileStore);
                stream.Put(PdfName.TYPE, PdfName.EMBEDDEDFILE);
                if (compress)
                    stream.FlateCompress();
                stream.Put(PdfName.PARAMS, refFileLength);
                if (mimeType != null)
                    stream.Put(PdfName.SUBTYPE, new PdfName(mimeType));
                refi = writer.AddToBody(stream).IndirectReference;
                if (fileStore == null) {
                    stream.WriteLength();
                }
                PdfDictionary param = new PdfDictionary();
                if (fileParameter != null)
                    param.Merge(fileParameter);
                param.Put(PdfName.SIZE, new PdfNumber(stream.RawLength));
                writer.AddToBody(param, refFileLength);
            }
            finally {
                if (inp != null)
                    try{inp.Close();}catch{}
            }
            PdfDictionary f = new PdfDictionary();
            f.Put(PdfName.F, refi);
            fs.Put(PdfName.EF, f);
            return fs;
        }
        
        /**
        * Creates a file specification for an external file.
        * @param writer the <CODE>PdfWriter</CODE>
        * @param filePath the file path
        * @return the file specification
        */
        public static PdfFileSpecification FileExtern(PdfWriter writer, String filePath) {
            PdfFileSpecification fs = new PdfFileSpecification();
            fs.writer = writer;
            fs.Put(PdfName.F, new PdfString(filePath));
            return fs;
        }
        
        /**
        * Gets the indirect reference to this file specification.
        * Multiple invocations will retrieve the same value.
        * @throws IOException on error
        * @return the indirect reference
        */    
        public PdfIndirectReference Reference {
            get {
                if (refi != null)
                    return refi;
                refi = writer.AddToBody(this).IndirectReference;
                return refi;
            }
        }
        
        /**
        * Sets the file name (the key /F) string as an hex representation
        * to support multi byte file names. The name must have the slash and
        * backslash escaped according to the file specification rules
        * @param fileName the file name as a byte array
        */    
        public byte[] MultiByteFileName {
            set {
                Put(PdfName.F, new PdfString(value).SetHexWriting(true));
            }
        }

        /**
        * Adds the unicode file name (the key /UF). This entry was introduced
        * in PDF 1.7. The filename must have the slash and backslash escaped
        * according to the file specification rules.
        * @param filename  the filename
        * @param unicode   if true, the filename is UTF-16BE encoded; otherwise PDFDocEncoding is used;
        */    
        public void SetUnicodeFileName(String filename, bool unicode) {
            Put(PdfName.UF, new PdfString(filename, unicode ? PdfObject.TEXT_UNICODE : PdfObject.TEXT_PDFDOCENCODING));
        }
        
        /**
        * Sets a flag that indicates whether an external file referenced by the file
        * specification is volatile. If the value is true, applications should never
        * cache a copy of the file.
        * @param volatile_file if true, the external file should not be cached
        */
        public bool Volatile {
            set {
                Put(PdfName.V, new PdfBoolean(value));
            }
        }
        
        /**
        * Adds a description for the file that is specified here.
        * @param description   some text
        * @param unicode       if true, the text is added as a unicode string
        */
        public void AddDescription(String description, bool unicode) {
            Put(PdfName.DESC, new PdfString(description, unicode ? PdfObject.TEXT_UNICODE : PdfObject.TEXT_PDFDOCENCODING));
        }
        
        /**
        * Adds the Collection item dictionary.
        */
        public void AddCollectionItem(PdfCollectionItem ci) {
            Put(PdfName.CI, ci);
        }
    }
}
