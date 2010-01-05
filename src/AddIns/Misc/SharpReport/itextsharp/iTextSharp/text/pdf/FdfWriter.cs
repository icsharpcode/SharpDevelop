using System;
using System.util;
using System.IO;
using System.Collections;

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
    /** Writes an FDF form.
    * @author Paulo Soares (psoares@consiste.pt)
    */
    public class FdfWriter {
        private static readonly byte[] HEADER_FDF = DocWriter.GetISOBytes("%FDF-1.2\n%\u00e2\u00e3\u00cf\u00d3\n");
        Hashtable fields = new Hashtable();

        /** The PDF file associated with the FDF. */
        private String file;
        
        /** Creates a new FdfWriter. */    
        public FdfWriter() {
        }

        /** Writes the content to a stream.
        * @param os the stream
        * @throws DocumentException on error
        * @throws IOException on error
        */    
        public void WriteTo(Stream os) {
            Wrt wrt = new Wrt(os, this);
            wrt.WriteTo();
        }
        
        internal bool SetField(String field, PdfObject value) {
            Hashtable map = fields;
            StringTokenizer tk = new StringTokenizer(field, ".");
            if (!tk.HasMoreTokens())
                return false;
            while (true) {
                String s = tk.NextToken();
                Object obj = map[s];
                if (tk.HasMoreTokens()) {
                    if (obj == null) {
                        obj = new Hashtable();
                        map[s] = obj;
                        map = (Hashtable)obj;
                        continue;
                    }
                    else if (obj is Hashtable)
                        map = (Hashtable)obj;
                    else
                        return false;
                }
                else {
                    if (!(obj is Hashtable)) {
                        map[s] = value;
                        return true;
                    }
                    else
                        return false;
                }
            }
        }
        
        internal void IterateFields(Hashtable values, Hashtable map, String name) {
            foreach (DictionaryEntry entry in map) {
                String s = (String)entry.Key;
                Object obj = entry.Value;
                if (obj is Hashtable)
                    IterateFields(values, (Hashtable)obj, name + "." + s);
                else
                    values[(name + "." + s).Substring(1)] = obj;
            }
        }
        
        /** Removes the field value.
        * @param field the field name
        * @return <CODE>true</CODE> if the field was found and removed,
        * <CODE>false</CODE> otherwise
        */    
        public bool RemoveField(String field) {
            Hashtable map = fields;
            StringTokenizer tk = new StringTokenizer(field, ".");
            if (!tk.HasMoreTokens())
                return false;
            ArrayList hist = new ArrayList();
            while (true) {
                String s = tk.NextToken();
                Object obj = map[s];
                if (obj == null)
                    return false;
                hist.Add(map);
                hist.Add(s);
                if (tk.HasMoreTokens()) {
                    if (obj is Hashtable)
                        map = (Hashtable)obj;
                    else
                        return false;
                }
                else {
                    if (obj is Hashtable)
                        return false;
                    else
                        break;
                }
            }
            for (int k = hist.Count - 2; k >= 0; k -= 2) {
                map = (Hashtable)hist[k];
                String s = (String)hist[k + 1];
                map.Remove(s);
                if (map.Count > 0)
                    break;
            }
            return true;
        }
        
        /** Gets all the fields. The map is keyed by the fully qualified
        * field name and the values are <CODE>PdfObject</CODE>.
        * @return a map with all the fields
        */    
        public Hashtable GetFields() {
            Hashtable values = new Hashtable();
            IterateFields(values, fields, "");
            return values;
        }
        
        /** Gets the field value.
        * @param field the field name
        * @return the field value or <CODE>null</CODE> if not found
        */    
        public String GetField(String field) {
            Hashtable map = fields;
            StringTokenizer tk = new StringTokenizer(field, ".");
            if (!tk.HasMoreTokens())
                return null;
            while (true) {
                String s = tk.NextToken();
                Object obj = map[s];
                if (obj == null)
                    return null;
                if (tk.HasMoreTokens()) {
                    if (obj is Hashtable)
                        map = (Hashtable)obj;
                    else
                        return null;
                }
                else {
                    if (obj is Hashtable)
                        return null;
                    else {
                        if (((PdfObject)obj).IsString())
                            return ((PdfString)obj).ToUnicodeString();
                        else
                            return PdfName.DecodeName(obj.ToString());
                    }
                }
            }
        }
        
        /** Sets the field value as a name.
        * @param field the fully qualified field name
        * @param value the value
        * @return <CODE>true</CODE> if the value was inserted,
        * <CODE>false</CODE> if the name is incompatible with
        * an existing field
        */    
        public bool SetFieldAsName(String field, String value) {
            return SetField(field, new PdfName(value));
        }
        
        /** Sets the field value as a string.
        * @param field the fully qualified field name
        * @param value the value
        * @return <CODE>true</CODE> if the value was inserted,
        * <CODE>false</CODE> if the name is incompatible with
        * an existing field
        */    
        public bool SetFieldAsString(String field, String value) {
            return SetField(field, new PdfString(value, PdfObject.TEXT_UNICODE));
        }
        
        /** Sets all the fields from this <CODE>FdfReader</CODE>
        * @param fdf the <CODE>FdfReader</CODE>
        */    
        public void SetFields(FdfReader fdf) {
            Hashtable map = fdf.Fields;
            foreach (DictionaryEntry entry in map) {
                String key = (String)entry.Key;
                PdfDictionary dic = (PdfDictionary)entry.Value;
                PdfObject v = dic.Get(PdfName.V);
                if (v != null) {
                    SetField(key, v);
                }
            }
        }
        
        /** Sets all the fields from this <CODE>PdfReader</CODE>
        * @param pdf the <CODE>PdfReader</CODE>
        */    
        public void SetFields(PdfReader pdf) {
            SetFields(pdf.AcroFields);
        }
        
        /** Sets all the fields from this <CODE>AcroFields</CODE>
        * @param acro the <CODE>AcroFields</CODE>
        */    
        public void SetFields(AcroFields af) {
            foreach (DictionaryEntry entry in af.Fields) {
                String fn = (String)entry.Key;
                AcroFields.Item item = (AcroFields.Item)entry.Value;
                PdfDictionary dic = (PdfDictionary)item.merged[0];
                PdfObject v = PdfReader.GetPdfObjectRelease(dic.Get(PdfName.V));
                if (v == null)
                    continue;
                PdfObject ft = PdfReader.GetPdfObjectRelease(dic.Get(PdfName.FT));
                if (ft == null || PdfName.SIG.Equals(ft))
                    continue;
                SetField(fn, v);
            }
        }
        
        /** Gets the PDF file name associated with the FDF.
        * @return the PDF file name associated with the FDF
        */
        public String File {
            get {
                return this.file;
            }
            set {
                file = value;
            }
        }
                
        internal class Wrt : PdfWriter {
            private FdfWriter fdf;
           
            internal Wrt(Stream os, FdfWriter fdf) : base(new PdfDocument(), os) {
                this.fdf = fdf;
                this.os.Write(HEADER_FDF, 0, HEADER_FDF.Length);
                body = new PdfBody(this);
            }
            
            internal void WriteTo() {
                PdfDictionary dic = new PdfDictionary();
                dic.Put(PdfName.FIELDS, Calculate(fdf.fields));
                if (fdf.file != null)
                    dic.Put(PdfName.F, new PdfString(fdf.file, PdfObject.TEXT_UNICODE));
                PdfDictionary fd = new PdfDictionary();
                fd.Put(PdfName.FDF, dic);
                PdfIndirectReference refi = AddToBody(fd).IndirectReference;
                byte[] b = GetISOBytes("trailer\n");
                os.Write(b, 0, b.Length);
                PdfDictionary trailer = new PdfDictionary();
                trailer.Put(PdfName.ROOT, refi);
                trailer.ToPdf(null, os);
                b = GetISOBytes("\n%%EOF\n");
                os.Write(b, 0, b.Length);
                os.Close();
            }
            
            
            internal PdfArray Calculate(Hashtable map) {
                PdfArray ar = new PdfArray();
                foreach (DictionaryEntry entry in map) {
                    String key = (String)entry.Key;
                    Object v = entry.Value;
                    PdfDictionary dic = new PdfDictionary();
                    dic.Put(PdfName.T, new PdfString(key, PdfObject.TEXT_UNICODE));
                    if (v is Hashtable) {
                        dic.Put(PdfName.KIDS, Calculate((Hashtable)v));
                    }
                    else {
                        dic.Put(PdfName.V, (PdfObject)v);
                    }
                    ar.Add(dic);
                }
                return ar;
            }
        }
    }
}