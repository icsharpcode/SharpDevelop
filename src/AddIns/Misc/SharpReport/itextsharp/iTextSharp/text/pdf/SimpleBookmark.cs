using System;
using System.IO;
using System.Collections;
using System.Text;
using System.util;
using iTextSharp.text.xml.simpleparser;

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
    /**
    * Bookmark processing in a simple way. It has some limitations, mainly the only
    * action types supported are GoTo, GoToR, URI and Launch.
    * <p>
    * The list structure is composed by a number of Hashtable, keyed by strings, one Hashtable
    * for each bookmark.
    * The element values are all strings with the exception of the key "Kids" that has
    * another list for the child bookmarks.
    * <p>
    * All the bookmarks have a "Title" with the
    * bookmark title and optionally a "Style" that can be "bold", "italic" or a
    * combination of both. They can also have a "Color" key with a value of three
    * floats separated by spaces. The key "Open" can have the values "true" or "false" and
    * signals the open status of the children. It's "true" by default.
    * <p>
    * The actions and the parameters can be:
    * <ul>
    * <li>"Action" = "GoTo" - "Page" | "Named"
    * <ul>
    * <li>"Page" = "3 XYZ 70 400 null" - page number followed by a destination (/XYZ is also accepted)
    * <li>"Named" = "named_destination"
    * </ul>
    * <li>"Action" = "GoToR" - "Page" | "Named" | "NamedN", "File", ["NewWindow"]
    * <ul>
    * <li>"Page" = "3 XYZ 70 400 null" - page number followed by a destination (/XYZ is also accepted)
    * <li>"Named" = "named_destination_as_a_string"
    * <li>"NamedN" = "named_destination_as_a_name"
    * <li>"File" - "the_file_to_open"
    * <li>"NewWindow" - "true" or "false"
    * </ul>
    * <li>"Action" = "URI" - "URI"
    * <ul>
    * <li>"URI" = "http://sf.net" - URI to jump to
    * </ul>
    * <li>"Action" = "Launch" - "File"
    * <ul>
    * <li>"File" - "the_file_to_open_or_execute"
    * </ul>
    * @author Paulo Soares (psoares@consiste.pt)
    */
    public sealed class SimpleBookmark : ISimpleXMLDocHandler {
        
        private ArrayList topList;
        private Stack attr = new Stack();
        
        /** Creates a new instance of SimpleBookmark */
        private SimpleBookmark() {
        }
        
        private static ArrayList BookmarkDepth(PdfReader reader, PdfDictionary outline, IntHashtable pages) {
            ArrayList list = new ArrayList();
            while (outline != null) {
                Hashtable map = new Hashtable();
                PdfString title = (PdfString)PdfReader.GetPdfObjectRelease(outline.Get(PdfName.TITLE));
                map["Title"] = title.ToUnicodeString();
                PdfArray color = (PdfArray)PdfReader.GetPdfObjectRelease(outline.Get(PdfName.C));
                if (color != null && color.ArrayList.Count == 3) {
                    ByteBuffer outp = new ByteBuffer();
                    ArrayList arr = color.ArrayList;
                    outp.Append(((PdfNumber)arr[0]).FloatValue).Append(' ');
                    outp.Append(((PdfNumber)arr[1]).FloatValue).Append(' ');
                    outp.Append(((PdfNumber)arr[2]).FloatValue);
                    map["Color"] = PdfEncodings.ConvertToString(outp.ToByteArray(), null);
                }
                PdfNumber style = (PdfNumber)PdfReader.GetPdfObjectRelease(outline.Get(PdfName.F));
                if (style != null) {
                    int f = style.IntValue;
                    String s = "";
                    if ((f & 1) != 0)
                        s += "italic ";
                    if ((f & 2) != 0)
                        s += "bold ";
                    s = s.Trim();
                    if (s.Length != 0) 
                        map["Style"] = s;
                }
                PdfNumber count = (PdfNumber)PdfReader.GetPdfObjectRelease(outline.Get(PdfName.COUNT));
                if (count != null && count.IntValue < 0)
                    map["Open"] = "false";
                try {
                    PdfObject dest = PdfReader.GetPdfObjectRelease(outline.Get(PdfName.DEST));
                    if (dest != null) {
                        MapGotoBookmark(map, dest, pages); //changed by ujihara 2004-06-13
                    }
                    else {
                        PdfDictionary action = (PdfDictionary)PdfReader.GetPdfObjectRelease(outline.Get(PdfName.A));
                        if (action != null) {
                            if (PdfName.GOTO.Equals(PdfReader.GetPdfObjectRelease(action.Get(PdfName.S)))) {
                                dest = PdfReader.GetPdfObjectRelease(action.Get(PdfName.D));
                                if (dest != null) {
                                    MapGotoBookmark(map, dest, pages);
                                }
                            }
                            else if (PdfName.URI.Equals(PdfReader.GetPdfObjectRelease(action.Get(PdfName.S)))) {
                                map["Action"] = "URI";
                                map["URI"] = ((PdfString)PdfReader.GetPdfObjectRelease(action.Get(PdfName.URI))).ToUnicodeString();
                            }
                            else if (PdfName.GOTOR.Equals(PdfReader.GetPdfObjectRelease(action.Get(PdfName.S)))) {
                                dest = PdfReader.GetPdfObjectRelease(action.Get(PdfName.D));
                                if (dest != null) {
                                    if (dest.IsString())
                                        map["Named"] = dest.ToString();
                                    else if (dest.IsName())
                                        map["NamedN"] = PdfName.DecodeName(dest.ToString());
                                    else if (dest.IsArray()) {
                                        ArrayList arr = ((PdfArray)dest).ArrayList;
                                        StringBuilder s = new StringBuilder();
                                        s.Append(arr[0].ToString());
                                        s.Append(' ').Append(arr[1].ToString());
                                        for (int k = 2; k < arr.Count; ++k)
                                            s.Append(' ').Append(arr[k].ToString());
                                        map["Page"] = s.ToString();
                                    }
                                }
                                map["Action"] = "GoToR";
                                PdfObject file = PdfReader.GetPdfObjectRelease(action.Get(PdfName.F));
                                if (file != null) {
                                    if (file.IsString())
                                        map["File"] = ((PdfString)file).ToUnicodeString();
                                    else if (file.IsDictionary()) {
                                        file = PdfReader.GetPdfObject(((PdfDictionary)file).Get(PdfName.F));
                                        if (file.IsString())
                                            map["File"] = ((PdfString)file).ToUnicodeString();
                                    }
                                }
                                PdfObject newWindow = PdfReader.GetPdfObjectRelease(action.Get(PdfName.NEWWINDOW));
                                if (newWindow != null)
                                    map["NewWindow"] = newWindow.ToString();
                            }
                            else if (PdfName.LAUNCH.Equals(PdfReader.GetPdfObjectRelease(action.Get(PdfName.S)))) {
                                map["Action"] = "Launch";
                                PdfObject file = PdfReader.GetPdfObjectRelease(action.Get(PdfName.F));
                                if (file == null)
                                    file = PdfReader.GetPdfObjectRelease(action.Get(PdfName.WIN));
                                if (file != null) {
                                    if (file.IsString())
                                        map["File"] = ((PdfString)file).ToUnicodeString();
                                    else if (file.IsDictionary()) {
                                        file = PdfReader.GetPdfObjectRelease(((PdfDictionary)file).Get(PdfName.F));
                                        if (file.IsString())
                                            map["File"] = ((PdfString)file).ToUnicodeString();
                                    }
                                }
                            }
                        }
                    }
                }
                catch  {
                    //empty on purpose
                }
                PdfDictionary first = (PdfDictionary)PdfReader.GetPdfObjectRelease(outline.Get(PdfName.FIRST));
                if (first != null) {
                    map["Kids"] = BookmarkDepth(reader, first, pages);
                }
                list.Add(map);
                outline = (PdfDictionary)PdfReader.GetPdfObjectRelease(outline.Get(PdfName.NEXT));
            }
            return list;
        }
        
        private static void MapGotoBookmark(Hashtable map, PdfObject dest, IntHashtable pages) 
        {
            if (dest.IsString())
                map["Named"] = dest.ToString();
            else if (dest.IsName())
                map["Named"] = PdfName.DecodeName(dest.ToString());
            else if (dest.IsArray()) 
                map["Page"] = MakeBookmarkParam((PdfArray)dest, pages); //changed by ujihara 2004-06-13
            map["Action"] = "GoTo";
        }

        private static String MakeBookmarkParam(PdfArray dest, IntHashtable pages)
        {
            ArrayList arr = dest.ArrayList;
            StringBuilder s = new StringBuilder();
            s.Append(pages[GetNumber((PdfIndirectReference)arr[0])]); //changed by ujihara 2004-06-13
            s.Append(' ').Append(arr[1].ToString().Substring(1));
            for (int k = 2; k < arr.Count; ++k)
                s.Append(' ').Append(arr[k].ToString());
            return s.ToString();
        }
        
        /**
        * Gets number of indirect. If type of directed indirect is PAGES, it refers PAGE object through KIDS.
        * (Contributed by Kazuya Ujihara)
        * @param indirect 
        * 2004-06-13
        */
        private static int GetNumber(PdfIndirectReference indirect)
        {
            PdfDictionary pdfObj = (PdfDictionary)PdfReader.GetPdfObjectRelease(indirect);
            if (pdfObj.Contains(PdfName.TYPE) && pdfObj.Get(PdfName.TYPE).Equals(PdfName.PAGES) && pdfObj.Contains(PdfName.KIDS)) 
            {
                PdfArray kids = (PdfArray)pdfObj.Get(PdfName.KIDS);
                indirect = (PdfIndirectReference)kids.ArrayList[0];
            }
            return indirect.Number;
        }
        
        /**
        * Gets a <CODE>List</CODE> with the bookmarks. It returns <CODE>null</CODE> if
        * the document doesn't have any bookmarks.
        * @param reader the document
        * @return a <CODE>List</CODE> with the bookmarks or <CODE>null</CODE> if the
        * document doesn't have any
        */    
        public static ArrayList GetBookmark(PdfReader reader) {
            PdfDictionary catalog = reader.Catalog;
            PdfObject obj = PdfReader.GetPdfObjectRelease(catalog.Get(PdfName.OUTLINES));
            if (obj == null || !obj.IsDictionary())
                return null;
            PdfDictionary outlines = (PdfDictionary)obj;
            IntHashtable pages = new IntHashtable();
            int numPages = reader.NumberOfPages;
            for (int k = 1; k <= numPages; ++k) {
                pages[reader.GetPageOrigRef(k).Number] = k;
                reader.ReleasePage(k);
            }
            return BookmarkDepth(reader, (PdfDictionary)PdfReader.GetPdfObjectRelease(outlines.Get(PdfName.FIRST)), pages);
        }
        
        /**
        * Removes the bookmark entries for a number of page ranges. The page ranges
        * consists of a number of pairs with the start/end page range. The page numbers
        * are inclusive.
        * @param list the bookmarks
        * @param pageRange the page ranges, always in pairs.
        */    
        public static void EliminatePages(ArrayList list, int[] pageRange) {
            if (list == null)
                return;

            for (ListIterator it = new ListIterator(list); it.HasNext();) {
                Hashtable map = (Hashtable)it.Next();
                bool hit = false;
                if ("GoTo".Equals(map["Action"])) {
                    String page = (String)map["Page"];
                    if (page != null) {
                        page = page.Trim();
                        int idx = page.IndexOf(' ');
                        int pageNum;
                        if (idx < 0)
                            pageNum = int.Parse(page);
                        else
                            pageNum = int.Parse(page.Substring(0, idx));
                        int len = pageRange.Length & 0x7ffffffe;
                        for (int k = 0; k < len; k += 2) {
                            if (pageNum >= pageRange[k] && pageNum <= pageRange[k + 1]) {
                                hit = true;
                                break;
                            }
                        }
                    }
                }
                ArrayList kids = (ArrayList)map["Kids"];
                if (kids != null) {
                    EliminatePages(kids, pageRange);
                    if (kids.Count == 0) {
                        map.Remove("Kids");
                        kids = null;
                    }
                }
                if (hit) {
                    if (kids == null)
                        it.Remove();
                    else {
                        map.Remove("Action");
                        map.Remove("Page");
                        map.Remove("Named");
                    }
                }
            }
        }
        
        /**
        * For the pages in range add the <CODE>pageShift</CODE> to the page number.
        * The page ranges
        * consists of a number of pairs with the start/end page range. The page numbers
        * are inclusive.
        * @param list the bookmarks
        * @param pageShift the number to add to the pages in range
        * @param pageRange the page ranges, always in pairs. It can be <CODE>null</CODE>
        * to include all the pages
        */    
        public static void ShiftPageNumbers(ArrayList list, int pageShift, int[] pageRange) {
            if (list == null)
                return;
            foreach (Hashtable map in list) {
                if ("GoTo".Equals(map["Action"])) {
                    String page = (String)map["Page"];
                    if (page != null) {
                        page = page.Trim();
                        int idx = page.IndexOf(' ');
                        int pageNum;
                        if (idx < 0)
                            pageNum = int.Parse(page);
                        else
                            pageNum = int.Parse(page.Substring(0, idx));
                        bool hit = false;
                        if (pageRange == null)
                            hit = true;
                        else {
                            int len = pageRange.Length & 0x7ffffffe;
                            for (int k = 0; k < len; k += 2) {
                                if (pageNum >= pageRange[k] && pageNum <= pageRange[k + 1]) {
                                    hit = true;
                                    break;
                                }
                            }
                        }
                        if (hit) {
                            if (idx < 0)
                                page = (pageNum + pageShift) + "";
                            else
                                page = (pageNum + pageShift) + page.Substring(idx);
                        }
                        map["Page"] = page;
                    }
                }
                ArrayList kids = (ArrayList)map["Kids"];
                if (kids != null)
                    ShiftPageNumbers(kids, pageShift, pageRange);
            }
        }
        
        internal static void CreateOutlineAction(PdfDictionary outline, Hashtable map, PdfWriter writer, bool namedAsNames) {
            try {
                String action = (String)map["Action"];
                if ("GoTo".Equals(action)) {
                    String p;
                    if ((p = (String)map["Named"]) != null) {
                        if (namedAsNames)
                            outline.Put(PdfName.DEST, new PdfName(p));
                        else
                            outline.Put(PdfName.DEST, new PdfString(p, null));
                    }
                    else if ((p = (String)map["Page"]) != null) {
                        PdfArray ar = new PdfArray();
                        StringTokenizer tk = new StringTokenizer(p);
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
                        outline.Put(PdfName.DEST, ar);
                    }
                }
                else if ("GoToR".Equals(action)) {
                    String p;
                    PdfDictionary dic = new PdfDictionary();
                    if ((p = (String)map["Named"]) != null)
                        dic.Put(PdfName.D, new PdfString(p, null));
                    else if ((p = (String)map["NamedN"]) != null)
                        dic.Put(PdfName.D, new PdfName(p));
                    else if ((p = (String)map["Page"]) != null){
                        PdfArray ar = new PdfArray();
                        StringTokenizer tk = new StringTokenizer(p);
                        ar.Add(new PdfNumber(tk.NextToken()));
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
                        dic.Put(PdfName.D, ar);
                    }
                    String file = (String)map["File"];
                    if (dic.Size > 0 && file != null) {
                        dic.Put(PdfName.S,  PdfName.GOTOR);
                        dic.Put(PdfName.F, new PdfString(file));
                        String nw = (String)map["NewWindow"];
                        if (nw != null) {
                            if (nw.Equals("true"))
                                dic.Put(PdfName.NEWWINDOW, PdfBoolean.PDFTRUE);
                            else if (nw.Equals("false"))
                                dic.Put(PdfName.NEWWINDOW, PdfBoolean.PDFFALSE);
                        }
                        outline.Put(PdfName.A, dic);
                    }
                }
                else if ("URI".Equals(action)) {
                    String uri = (String)map["URI"];
                    if (uri != null) {
                        PdfDictionary dic = new PdfDictionary();
                        dic.Put(PdfName.S, PdfName.URI);
                        dic.Put(PdfName.URI, new PdfString(uri));
                        outline.Put(PdfName.A, dic);
                    }
                }
                else if ("Launch".Equals(action)) {
                    String file = (String)map["File"];
                    if (file != null) {
                        PdfDictionary dic = new PdfDictionary();
                        dic.Put(PdfName.S, PdfName.LAUNCH);
                        dic.Put(PdfName.F, new PdfString(file));
                        outline.Put(PdfName.A, dic);
                    }
                }
            }
            catch  {
                // empty on purpose
            }
        }

        public static Object[] IterateOutlines(PdfWriter writer, PdfIndirectReference parent, ArrayList kids, bool namedAsNames) {
            PdfIndirectReference[] refs = new PdfIndirectReference[kids.Count];
            for (int k = 0; k < refs.Length; ++k)
                refs[k] = writer.PdfIndirectReference;
            int ptr = 0;
            int count = 0;
            foreach (Hashtable map in kids) {
                Object[] lower = null;
                ArrayList subKid = (ArrayList)map["Kids"];
                if (subKid != null && subKid.Count > 0)
                    lower = IterateOutlines(writer, refs[ptr], subKid, namedAsNames);
                PdfDictionary outline = new PdfDictionary();
                ++count;
                if (lower != null) {
                    outline.Put(PdfName.FIRST, (PdfIndirectReference)lower[0]);
                    outline.Put(PdfName.LAST, (PdfIndirectReference)lower[1]);
                    int n = (int)lower[2];
                    if ("false".Equals(map["Open"])) {
                        outline.Put(PdfName.COUNT, new PdfNumber(-n));
                    }
                    else {
                        outline.Put(PdfName.COUNT, new PdfNumber(n));
                        count += n;
                    }
                }
                outline.Put(PdfName.PARENT, parent);
                if (ptr > 0)
                    outline.Put(PdfName.PREV, refs[ptr - 1]);
                if (ptr < refs.Length - 1)
                    outline.Put(PdfName.NEXT, refs[ptr + 1]);
                outline.Put(PdfName.TITLE, new PdfString((String)map["Title"], PdfObject.TEXT_UNICODE));
                String color = (String)map["Color"];
                if (color != null) {
                    try {
                        PdfArray arr = new PdfArray();
                        StringTokenizer tk = new StringTokenizer(color);
                        for (int k = 0; k < 3; ++k) {
                            float f = float.Parse(tk.NextToken(), System.Globalization.NumberFormatInfo.InvariantInfo);
                            if (f < 0) f = 0;
                            if (f > 1) f = 1;
                            arr.Add(new PdfNumber(f));
                        }
                        outline.Put(PdfName.C, arr);
                    } catch {} //in case it's malformed
                }
                String style = (String)map["Style"];
                if (style != null) {
                    style = style.ToLower(System.Globalization.CultureInfo.InvariantCulture);
                    int bits = 0;
                    if (style.IndexOf("italic") >= 0)
                        bits |= 1;
                    if (style.IndexOf("bold") >= 0)
                        bits |= 2;
                    if (bits != 0)
                        outline.Put(PdfName.F, new PdfNumber(bits));
                }
                CreateOutlineAction(outline, map, writer, namedAsNames);
                writer.AddToBody(outline, refs[ptr]);
                ++ptr;
            }
            return new Object[]{refs[0], refs[refs.Length - 1], count};
        }
        
        /**
        * Exports the bookmarks to XML. Only of use if the generation is to be include in
        * some other XML document.
        * @param list the bookmarks
        * @param out the export destination. The writer is not closed
        * @param indent the indentation level. Pretty printing significant only
        * @param onlyASCII codes above 127 will always be escaped with &amp;#nn; if <CODE>true</CODE>,
        * whatever the encoding
        * @throws IOException on error
        */
        public static void ExportToXMLNode(ArrayList list, TextWriter outp, int indent, bool onlyASCII) {
            String dep = "";
            for (int k = 0; k < indent; ++k)
                dep += "  ";
            foreach (Hashtable map in list) {
                String title = null;
                outp.Write(dep);
                outp.Write("<Title ");
                ArrayList kids = null;
                foreach (DictionaryEntry entry in map) {
                    String key = (String)entry.Key;
                    if (key.Equals("Title")) {
                        title = (String)entry.Value;
                        continue;
                    }
                    else if (key.Equals("Kids")) {
                        kids = (ArrayList)entry.Value;
                        continue;
                    }
                    else {
                        outp.Write(key);
                        outp.Write("=\"");
                        String value = (String)entry.Value;
                        if (key.Equals("Named") || key.Equals("NamedN"))
                            value = EscapeBinaryString(value);
                        outp.Write(SimpleXMLParser.EscapeXML(value, onlyASCII));
                        outp.Write("\" ");
                    }
                }
                outp.Write(">");
                if (title == null)
                    title = "";
                outp.Write(SimpleXMLParser.EscapeXML(title, onlyASCII));
                if (kids != null) {
                    outp.Write("\n");
                    ExportToXMLNode(kids, outp, indent + 1, onlyASCII);
                    outp.Write(dep);
                }
                outp.Write("</Title>\n");
            }
        }
        
        /**
        * Exports the bookmarks to XML. The DTD for this XML is:
        * <p>
        * <pre>
        * &lt;?xml version='1.0' encoding='UTF-8'?&gt;
        * &lt;!ELEMENT Title (#PCDATA|Title)*&gt;
        * &lt;!ATTLIST Title
        *    Action CDATA #IMPLIED
        *    Open CDATA #IMPLIED
        *    Page CDATA #IMPLIED
        *    URI CDATA #IMPLIED
        *    File CDATA #IMPLIED
        *    Named CDATA #IMPLIED
        *    NamedN CDATA #IMPLIED
        *    NewWindow CDATA #IMPLIED
        *    Style CDATA #IMPLIED
        *    Color CDATA #IMPLIED
        * &gt;
        * &lt;!ELEMENT Bookmark (Title)*&gt;
        * </pre>
        * @param list the bookmarks
        * @param out the export destination. The stream is not closed
        * @param encoding the encoding according to IANA conventions
        * @param onlyASCII codes above 127 will always be escaped with &amp;#nn; if <CODE>true</CODE>,
        * whatever the encoding
        * @throws IOException on error
        */    
        public static void ExportToXML(ArrayList list, Stream outp, String encoding, bool onlyASCII) {
            StreamWriter wrt = new StreamWriter(outp, IanaEncodings.GetEncodingEncoding(encoding));
            ExportToXML(list, wrt, encoding, onlyASCII);
        }
        
        /**
        * Exports the bookmarks to XML.
        * @param list the bookmarks
        * @param wrt the export destination. The writer is not closed
        * @param encoding the encoding according to IANA conventions
        * @param onlyASCII codes above 127 will always be escaped with &amp;#nn; if <CODE>true</CODE>,
        * whatever the encoding
        * @throws IOException on error
        */
        public static void ExportToXML(ArrayList list, TextWriter wrt, String encoding, bool onlyASCII) {
            wrt.Write("<?xml version=\"1.0\" encoding=\"");
            wrt.Write(SimpleXMLParser.EscapeXML(encoding, onlyASCII));
            wrt.Write("\"?>\n<Bookmark>\n");
            ExportToXMLNode(list, wrt, 1, onlyASCII);
            wrt.Write("</Bookmark>\n");
            wrt.Flush();
        }
        
        /**
        * Import the bookmarks from XML.
        * @param in the XML source. The stream is not closed
        * @throws IOException on error
        * @return the bookmarks
        */    
        public static ArrayList ImportFromXML(Stream inp) {
            SimpleBookmark book = new SimpleBookmark();
            SimpleXMLParser.Parse(book, inp);
            return book.topList;
        }
        
        /**
        * Import the bookmarks from XML.
        * @param in the XML source. The reader is not closed
        * @throws IOException on error
        * @return the bookmarks
        */
        public static ArrayList ImportFromXML(TextReader inp) {
            SimpleBookmark book = new SimpleBookmark();
            SimpleXMLParser.Parse(book, inp);
            return book.topList;
        }
        
        public static String EscapeBinaryString(String s) {
            StringBuilder buf = new StringBuilder();
            char[] cc = s.ToCharArray();
            int len = cc.Length;
            for (int k = 0; k < len; ++k) {
                char c = cc[k];
                if (c < ' ') {
                    buf.Append('\\');
                    int v = (int)c;
                    string octal = "";
                    do {
                        int x = v % 8;
                        octal = x.ToString() + octal;
                        v /= 8;
                    } while (v > 0);
                    buf.Append(octal.PadLeft(3, '0'));
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
            if (tag.Equals("Bookmark")) {
                if (attr.Count == 0)
                    return;
                else
                    throw new Exception("Bookmark end tag out of place.");
            }
            if (!tag.Equals("Title"))
                throw new Exception("Invalid end tag - " + tag);
            Hashtable attributes = (Hashtable)attr.Pop();
            String title = (String)attributes["Title"];
            attributes["Title"] = title.Trim();
            String named = (String)attributes["Named"];
            if (named != null)
                attributes["Named"] = UnEscapeBinaryString(named);
            named = (String)attributes["NamedN"];
            if (named != null)
                attributes["NamedN"] = UnEscapeBinaryString(named);
            if (attr.Count == 0)
                topList.Add(attributes);
            else {
                Hashtable parent = (Hashtable)attr.Peek();
                ArrayList kids = (ArrayList)parent["Kids"];
                if (kids == null) {
                    kids = new ArrayList();
                    parent["Kids"] = kids;
                }
                kids.Add(attributes);
            }
        }
        
        public void StartDocument() {
        }
        
        public void StartElement(String tag, Hashtable h) {
            if (topList == null) {
                if (tag.Equals("Bookmark")) {
                    topList = new ArrayList();
                    return;
                }
                else
                    throw new Exception("Root element is not Bookmark: " + tag);
            }
            if (!tag.Equals("Title"))
                throw new Exception("Tag " + tag + " not allowed.");
            Hashtable attributes = new Hashtable(h);
            attributes["Title"] = "";
            attributes.Remove("Kids");
            attr.Push(attributes);
        }
        
        public void Text(String str) {
            if (attr.Count == 0)
                return;
            Hashtable attributes = (Hashtable)attr.Peek();
            String title = (String)attributes["Title"];
            title += str;
            attributes["Title"] = title;
        }    
    }
}
