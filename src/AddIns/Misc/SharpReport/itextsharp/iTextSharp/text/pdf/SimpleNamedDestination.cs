using System;
using System.IO;
using System.Collections;
using System.Text;
using System.util;
using iTextSharp.text.xml.simpleparser;
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
    *
    * @author Paulo Soares (psoares@consiste.pt)
    */
    public sealed class SimpleNamedDestination : ISimpleXMLDocHandler {
        
        private Hashtable xmlNames;
        private Hashtable xmlLast;

        private SimpleNamedDestination() {
        }
        
        public static Hashtable GetNamedDestination(PdfReader reader, bool fromNames) {
            IntHashtable pages = new IntHashtable();
            int numPages = reader.NumberOfPages;
            for (int k = 1; k <= numPages; ++k)
                pages[reader.GetPageOrigRef(k).Number] = k;
            Hashtable names = fromNames ? reader.GetNamedDestinationFromNames() : reader.GetNamedDestinationFromStrings();
            String[] keys = new String[names.Count];
            names.Keys.CopyTo(keys, 0);
            foreach (String name in keys) {
                ArrayList arr = ((PdfArray)names[name]).ArrayList;
                StringBuilder s = new StringBuilder();
                try {
                    s.Append(pages[((PdfIndirectReference)arr[0]).Number]);
                    s.Append(' ').Append(arr[1].ToString().Substring(1));
                    for (int k = 2; k < arr.Count; ++k)
                        s.Append(' ').Append(arr[k].ToString());
                    names[name] = s.ToString();
                }
                catch {
                    names.Remove(name);
                }
            }
            return names;
        }
        
        /**
        * Exports the destinations to XML. The DTD for this XML is:
        * <p>
        * <pre>
        * &lt;?xml version='1.0' encoding='UTF-8'?&gt;
        * &lt;!ELEMENT Name (#PCDATA)&gt;
        * &lt;!ATTLIST Name
        *    Page CDATA #IMPLIED
        * &gt;
        * &lt;!ELEMENT Destination (Name)*&gt;
        * </pre>
        * @param names the names
        * @param outp the export destination. The stream is not closed
        * @param encoding the encoding according to IANA conventions
        * @param onlyASCII codes above 127 will always be escaped with &amp;#nn; if <CODE>true</CODE>,
        * whatever the encoding
        * @throws IOException on error
        */
        public static void ExportToXML(Hashtable names, Stream outp, String encoding, bool onlyASCII) {
            StreamWriter wrt = new StreamWriter(outp, IanaEncodings.GetEncodingEncoding(encoding));
            ExportToXML(names, wrt, encoding, onlyASCII);
        }
        
        /**
        * Exports the bookmarks to XML.
        * @param names the names
        * @param wrt the export destination. The writer is not closed
        * @param encoding the encoding according to IANA conventions
        * @param onlyASCII codes above 127 will always be escaped with &amp;#nn; if <CODE>true</CODE>,
        * whatever the encoding
        * @throws IOException on error
        */
        public static void ExportToXML(Hashtable names, TextWriter wrt, String encoding, bool onlyASCII) {
            wrt.Write("<?xml version=\"1.0\" encoding=\"");
            wrt.Write(SimpleXMLParser.EscapeXML(encoding, onlyASCII));
            wrt.Write("\"?>\n<Destination>\n");
            foreach (String key in names.Keys) {
                String value = (String)names[key];
                wrt.Write("  <Name Page=\"");
                wrt.Write(SimpleXMLParser.EscapeXML(value, onlyASCII));
                wrt.Write("\">");
                wrt.Write(SimpleXMLParser.EscapeXML(EscapeBinaryString(key), onlyASCII));
                wrt.Write("</Name>\n");
            }
            wrt.Write("</Destination>\n");
            wrt.Flush();
        }
        
        /**
        * Import the names from XML.
        * @param inp the XML source. The stream is not closed
        * @throws IOException on error
        * @return the names
        */
        public static Hashtable ImportFromXML(Stream inp) {
            SimpleNamedDestination names = new SimpleNamedDestination();
            SimpleXMLParser.Parse(names, inp);
            return names.xmlNames;
        }
        
        /**
        * Import the names from XML.
        * @param inp the XML source. The reader is not closed
        * @throws IOException on error
        * @return the names
        */
        public static Hashtable ImportFromXML(TextReader inp) {
            SimpleNamedDestination names = new SimpleNamedDestination();
            SimpleXMLParser.Parse(names, inp);
            return names.xmlNames;
        }

        internal static PdfArray CreateDestinationArray(String value, PdfWriter writer) {
            PdfArray ar = new PdfArray();
            StringTokenizer tk = new StringTokenizer(value);
            int n = int.Parse(tk.NextToken());
            ar.Add(writer.GetPageReference(n));
            if (!tk.HasMoreTokens()) {
                ar.Add(PdfName.XYZ);
                ar.Add(new float[]{0, 10000, 0});
            }
            else {
                String fn = tk.NextToken();
                if (fn.StartsWith("/"))
                    fn = fn.Substring(1);
                ar.Add(new PdfName(fn));
                for (int k = 0; k < 4 && tk.HasMoreTokens(); ++k) {
                    fn = tk.NextToken();
                    if (fn.Equals("null"))
                        ar.Add(PdfNull.PDFNULL);
                    else
                        ar.Add(new PdfNumber(fn));
                }
            }
            return ar;
        }
        
        public static PdfDictionary OutputNamedDestinationAsNames(Hashtable names, PdfWriter writer) {
            PdfDictionary dic = new PdfDictionary();
            foreach (String key in names.Keys) {
                try {
                    String value = (String)names[key];
                    PdfArray ar = CreateDestinationArray(value, writer);
                    PdfName kn = new PdfName(key);
                    dic.Put(kn, ar);
                }
                catch {
                    // empty on purpose
                }            
            }
            return dic;
        }
        
        public static PdfDictionary OutputNamedDestinationAsStrings(Hashtable names, PdfWriter writer) {
            Hashtable n2 = new Hashtable();
            foreach (String key in names.Keys) {
                try {
                    String value = (String)names[key];
                    PdfArray ar = CreateDestinationArray(value, writer);
                    n2[key] = writer.AddToBody(ar).IndirectReference;
                }
                catch {
                    // empty on purpose
                }
            }
            return PdfNameTree.WriteTree(n2, writer);
        }
        
        public static String EscapeBinaryString(String s) {
            StringBuilder buf = new StringBuilder();
            char[] cc = s.ToCharArray();
            int len = cc.Length;
            for (int k = 0; k < len; ++k) {
                char c = cc[k];
                if (c < ' ') {
                    buf.Append('\\');
                    ((int)c).ToString("", System.Globalization.CultureInfo.InvariantCulture);
                    String octal = "00" + Convert.ToString((int)c, 8);
                    buf.Append(octal.Substring(octal.Length - 3));
                }
                else if (c == '\\')
                    buf.Append("\\\\");
                else
                    buf.Append(c);
            }
            return buf.ToString();
        }
        
        public static String UnEscapeBinaryString(String s) {
            StringBuilder buf = new StringBuilder();
            char[] cc = s.ToCharArray();
            int len = cc.Length;
            for (int k = 0; k < len; ++k) {
                char c = cc[k];
                if (c == '\\') {
                    if (++k >= len) {
                        buf.Append('\\');
                        break;
                    }
                    c = cc[k];
                    if (c >= '0' && c <= '7') {
                        int n = c - '0';
                        ++k;
                        for (int j = 0; j < 2 && k < len; ++j) {
                            c = cc[k];
                            if (c >= '0' && c <= '7') {
                                ++k;
                                n = n * 8 + c - '0';
                            }
                            else {
                                break;
                            }
                        }
                        --k;
                        buf.Append((char)n);
                    }
                    else
                        buf.Append(c);
                }
                else
                    buf.Append(c);
            }
            return buf.ToString();
        }
        
        public void EndDocument() {
        }
        
        public void EndElement(String tag) {
            if (tag.Equals("Destination")) {
                if (xmlLast == null && xmlNames != null)
                    return;
                else
                    throw new ArgumentException("Destination end tag out of place.");
            }
            if (!tag.Equals("Name"))
                throw new ArgumentException("Invalid end tag - " + tag);
            if (xmlLast == null || xmlNames == null)
                throw new ArgumentException("Name end tag out of place.");
            if (!xmlLast.ContainsKey("Page"))
                throw new ArgumentException("Page attribute missing.");
            xmlNames[UnEscapeBinaryString((String)xmlLast["Name"])] = xmlLast["Page"];
            xmlLast = null;
        }
        
        public void StartDocument() {
        }
        
        public void StartElement(String tag, Hashtable h) {
            if (xmlNames == null) {
                if (tag.Equals("Destination")) {
                    xmlNames = new Hashtable();
                    return;
                }
                else
                    throw new ArgumentException("Root element is not Destination.");
            }
            if (!tag.Equals("Name"))
                throw new ArgumentException("Tag " + tag + " not allowed.");
            if (xmlLast != null)
                throw new ArgumentException("Nested tags are not allowed.");
            xmlLast = new Hashtable(h);
            xmlLast["Name"] = "";
        }
        
        public void Text(String str) {
            if (xmlLast == null)
                return;
            String name = (String)xmlLast["Name"];
            name += str;
            xmlLast["Name"] = name;
        }    
    }
}