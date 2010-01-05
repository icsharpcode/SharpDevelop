using System;
using System.Text;
using System.Collections;
using System.Net;
using System.IO;

/*
 * Copyright 2003 by Paulo Soares.
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
    /** Reads an FDF form and makes the fields available
    * @author Paulo Soares (psoares@consiste.pt)
    */
    public class FdfReader : PdfReader {
        
        internal Hashtable fields;
        internal String fileSpec;
        internal PdfName encoding;
        
        /** Reads an FDF form.
        * @param filename the file name of the form
        * @throws IOException on error
        */    
        public FdfReader(String filename) : base(filename) {
        }
        
        /** Reads an FDF form.
        * @param pdfIn the byte array with the form
        * @throws IOException on error
        */    
        public FdfReader(byte[] pdfIn) : base(pdfIn) {
        }
        
        /** Reads an FDF form.
        * @param url the URL of the document
        * @throws IOException on error
        */    
        public FdfReader(Uri url) : base(url) {
        }
        
        /** Reads an FDF form.
        * @param is the <CODE>InputStream</CODE> containing the document. The stream is read to the
        * end but is not closed
        * @throws IOException on error
        */    
        public FdfReader(Stream isp) : base(isp) {
        }
        
        protected internal override void ReadPdf() {
            fields = new Hashtable();
            try {
                tokens.CheckFdfHeader();
                RebuildXref();
                ReadDocObj();
            }
            finally {
                try {
                    tokens.Close();
                }
                catch  {
                    // empty on purpose
                }
            }
            ReadFields();
        }
        
        protected virtual void KidNode(PdfDictionary merged, String name) {
            PdfArray kids = (PdfArray)GetPdfObject(merged.Get(PdfName.KIDS));
            if (kids == null || kids.ArrayList.Count == 0) {
                if (name.Length > 0)
                    name = name.Substring(1);
                fields[name] = merged;
            }
            else {
                merged.Remove(PdfName.KIDS);
                ArrayList ar = kids.ArrayList;
                for (int k = 0; k < ar.Count; ++k) {
                    PdfDictionary dic = new PdfDictionary();
                    dic.Merge(merged);
                    PdfDictionary newDic = (PdfDictionary)GetPdfObject((PdfObject)ar[k]);
                    PdfString t = (PdfString)GetPdfObject(newDic.Get(PdfName.T));
                    String newName = name;
                    if (t != null)
                        newName += "." + t.ToUnicodeString();
                    dic.Merge(newDic);
                    dic.Remove(PdfName.T);
                    KidNode(dic, newName);
                }
            }
        }
        
        protected virtual void ReadFields() {
            catalog = (PdfDictionary)GetPdfObject(trailer.Get(PdfName.ROOT));
            PdfDictionary fdf = (PdfDictionary)GetPdfObject(catalog.Get(PdfName.FDF));
            if (fdf == null)
                return;
            PdfString fs = (PdfString)GetPdfObject(fdf.Get(PdfName.F));
            if (fs != null)
                fileSpec = fs.ToUnicodeString();
            PdfArray fld = (PdfArray)GetPdfObject(fdf.Get(PdfName.FIELDS));
            if (fld == null)
                return;
            encoding = (PdfName)GetPdfObject(fdf.Get(PdfName.ENCODING));
            PdfDictionary merged = new PdfDictionary();
            merged.Put(PdfName.KIDS, fld);
            KidNode(merged, "");
        }

        /** Gets all the fields. The map is keyed by the fully qualified
        * field name and the value is a merged <CODE>PdfDictionary</CODE>
        * with the field content.
        * @return all the fields
        */    
        public Hashtable Fields {
            get {
                return fields;
            }
        }
        
        /** Gets the field dictionary.
        * @param name the fully qualified field name
        * @return the field dictionary
        */    
        public PdfDictionary GetField(String name) {
            return (PdfDictionary)fields[name];
        }
        
        /** Gets the field value or <CODE>null</CODE> if the field does not
        * exist or has no value defined.
        * @param name the fully qualified field name
        * @return the field value or <CODE>null</CODE>
        */    
        public String GetFieldValue(String name) {
            PdfDictionary field = (PdfDictionary)fields[name];
            if (field == null)
                return null;
            PdfObject v = GetPdfObject(field.Get(PdfName.V));
            if (v == null)
                return null;
            if (v.IsName())
                return PdfName.DecodeName(((PdfName)v).ToString());
            else if (v.IsString()) {
                PdfString vs = (PdfString)v;
                if (encoding == null || vs.Encoding != null)
                    return vs.ToUnicodeString();
                byte[] b = vs.GetBytes();
                if (b.Length >= 2 && b[0] == (byte)254 && b[1] == (byte)255)
                    return vs.ToUnicodeString();
                try {
                    if (encoding.Equals(PdfName.SHIFT_JIS))
                        return Encoding.GetEncoding(932).GetString(b);
                    else if (encoding.Equals(PdfName.UHC))
                        return Encoding.GetEncoding(949).GetString(b);
                    else if (encoding.Equals(PdfName.GBK))
                        return Encoding.GetEncoding(936).GetString(b);
                    else if (encoding.Equals(PdfName.BIGFIVE))
                        return Encoding.GetEncoding(950).GetString(b);
                }
                catch  {
                }
                return vs.ToUnicodeString();
            }
            return null;
        }
        
        /** Gets the PDF file specification contained in the FDF.
        * @return the PDF file specification contained in the FDF
        */    
        public String FileSpec {
            get {
                return fileSpec;
            }
        }
    }
}