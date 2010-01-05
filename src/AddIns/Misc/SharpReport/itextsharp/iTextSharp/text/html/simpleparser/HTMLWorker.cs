using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Globalization;
using System.util;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.xml.simpleparser;
/*
 * Copyright 2004 Paulo Soares
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

namespace iTextSharp.text.html.simpleparser {

    public class HTMLWorker : ISimpleXMLDocHandler, IDocListener {
        
        protected ArrayList objectList;
        protected IDocListener document;
        private Paragraph currentParagraph;
        private ChainedProperties cprops = new ChainedProperties();
        private Stack stack = new Stack();
        private bool pendingTR = false;
        private bool pendingTD = false;
        private bool pendingLI = false;
        private StyleSheet style = new StyleSheet();
        private bool isPRE = false;
        private Stack tableState = new Stack();
        private bool skipText = false;
        private Hashtable interfaceProps;
        private FactoryProperties factoryProperties = new FactoryProperties();
        
        /** Creates a new instance of HTMLWorker */
        public HTMLWorker(IDocListener document) {
            this.document = document;
        }
        
        public StyleSheet Style {
            set {
                style = value;
            }
            get {
                return style;
            }
        }
        
        public Hashtable InterfaceProps {
            set {
                interfaceProps = value;
                FontFactoryImp ff = null;
                if (interfaceProps != null)
                    ff = (FontFactoryImp)interfaceProps["font_factory"];
                if (ff != null)
                    factoryProperties.FontImp = ff;
            }
            get {
                return interfaceProps;
            }
        }

        public void Parse(TextReader reader) {
            SimpleXMLParser.Parse(this, null, reader, true);
        }
        
        public static ArrayList ParseToList(TextReader reader, StyleSheet style) {
            return ParseToList(reader, style, null);
        }

        public static ArrayList ParseToList(TextReader reader, StyleSheet style, Hashtable interfaceProps) {
            HTMLWorker worker = new HTMLWorker(null);
            if (style != null)
                worker.Style = style;
            worker.document = worker;
            worker.InterfaceProps = interfaceProps;
            worker.objectList = new ArrayList();
            worker.Parse(reader);
            return worker.objectList;
        }
        
        public virtual void EndDocument() {
            foreach (IElement e in stack)
                document.Add(e);
            if (currentParagraph != null)
                document.Add(currentParagraph);
            currentParagraph = null;
        }
        
        public virtual void StartDocument() {
            Hashtable h = new Hashtable();
            style.ApplyStyle("body", h);
            cprops.AddToChain("body", h);
        }
        
        public virtual void StartElement(String tag, Hashtable h) {
            if (!tagsSupported.ContainsKey(tag))
                return;
            style.ApplyStyle(tag, h);
            String follow = (String)FactoryProperties.followTags[tag];
            if (follow != null) {
                Hashtable prop = new Hashtable();
                prop[follow] = null;
                cprops.AddToChain(follow, prop);
                return;
            }
            FactoryProperties.InsertStyle(h);
            if (tag.Equals("a")) {
                cprops.AddToChain(tag, h);
                if (currentParagraph == null)
                    currentParagraph = new Paragraph();
                stack.Push(currentParagraph);
                currentParagraph = new Paragraph();
                return;
            }
            if (tag.Equals("br")) {
                if (currentParagraph == null)
                    currentParagraph = new Paragraph();
                currentParagraph.Add(factoryProperties.CreateChunk("\n", cprops));
                return;
            }
            if (tag.Equals("font") || tag.Equals("span")) {
                cprops.AddToChain(tag, h);
                return;
            }
            if (tag.Equals("img")) {
                String src = (String)h["src"];
                if (src == null)
                    return;
                cprops.AddToChain(tag, h);
                Image img = null;
                if (interfaceProps != null) {
                    IImageProvider ip = (IImageProvider)interfaceProps["img_provider"];
                    if (ip != null)
                        img = ip.GetImage(src, h, cprops, document);
                    if (img == null) {
                        Hashtable images = (Hashtable)interfaceProps["img_static"];
                        if (images != null) {
                            Image tim = (Image)images[src];
                            if (tim != null)
                                img = Image.GetInstance(tim);
                        } else {
                            if (!src.StartsWith("http")) { // relative src references only
                                String baseurl = (String)interfaceProps["img_baseurl"];
                                if (baseurl != null) {
                                    src = baseurl + src;
                                    img = Image.GetInstance(src);
                                }
                            }
                        }
                    }
                }
                if (img == null) {
                    if (!src.StartsWith("http")) {
                        String path = cprops["image_path"];
                        if (path == null)
                            path = "";
                        src = Path.Combine(path, src);
                    }
                    img = Image.GetInstance(src);
                }
                String align = (String)h["align"];
                String width = (String)h["width"];
                String height = (String)h["height"];
                String before = cprops["before"];
                String after = cprops["after"];
                if (before != null)
                    img.SpacingBefore = float.Parse(before, System.Globalization.NumberFormatInfo.InvariantInfo);
                if (after != null)
                    img.SpacingAfter = float.Parse(after, System.Globalization.NumberFormatInfo.InvariantInfo);
                float wp = LengthParse(width, (int)img.Width);
                float lp = LengthParse(height, (int)img.Height);
                if (wp > 0 && lp > 0)
                    img.ScalePercent(wp > lp ? lp : wp);
                else if (wp > 0)
                    img.ScalePercent(wp);
                else if (lp > 0)
                    img.ScalePercent(lp);
                img.WidthPercentage = 0;
                if (align != null) {
                    EndElement("p");
                    int ralign = Image.MIDDLE_ALIGN;
                    if (Util.EqualsIgnoreCase(align, "left"))
                        ralign = Image.LEFT_ALIGN;
                    else if (Util.EqualsIgnoreCase(align, "right"))
                        ralign = Image.RIGHT_ALIGN;
                    img.Alignment = ralign;
                    IImg i = null;
                    bool skip = false;
                    if (interfaceProps != null) {
                        i = (IImg)interfaceProps["img_interface"];
                        if (i != null)
                            skip = i.Process(img, h, cprops, document);
                    }
                    if (!skip)
                        document.Add(img);
                    cprops.RemoveChain(tag);
                }
                else {
                    cprops.RemoveChain(tag);
                    if (currentParagraph == null)
                        currentParagraph = FactoryProperties.CreateParagraph(cprops);
                    currentParagraph.Add(new Chunk(img, 0, 0));
                }
                return;
            }

            EndElement("p");
            if (tag.Equals("h1") || tag.Equals("h2") || tag.Equals("h3") || tag.Equals("h4") || tag.Equals("h5") || tag.Equals("h6")) {
                if (!h.ContainsKey("size")) {
                    int v = 7 - int.Parse(tag.Substring(1));
                    h["size"] = v.ToString();
                }
                cprops.AddToChain(tag, h);
                return;
            }
            if (tag.Equals("ul")) {
                if (pendingLI)
                    EndElement("li");
                skipText = true;
                cprops.AddToChain(tag, h);
                List list = new List(false, 10);
                list.SetListSymbol("\u2022");
                stack.Push(list);
                return;
            }
            if (tag.Equals("ol")) {
                if (pendingLI)
                    EndElement("li");
                skipText = true;
                cprops.AddToChain(tag, h);
                List list = new List(true, 10);
                stack.Push(list);
                return;
            }
            if (tag.Equals("li")) {
                if (pendingLI)
                    EndElement("li");
                skipText = false;
                pendingLI = true;
                cprops.AddToChain(tag, h);
                stack.Push(FactoryProperties.CreateListItem(cprops));
                return;
            }
            if (tag.Equals("div") || tag.Equals("body")) {
                cprops.AddToChain(tag, h);
                return;
            }
            if (tag.Equals("pre")) {
                if (!h.ContainsKey("face")) {
                    h["face"] = "Courier";
                }
                cprops.AddToChain(tag, h);
                isPRE = true;
                return;
            }
            if (tag.Equals("p")) {
                cprops.AddToChain(tag, h);
                currentParagraph = FactoryProperties.CreateParagraph(h);
                return;
            }
            if (tag.Equals("tr")) {
                if (pendingTR)
                    EndElement("tr");
                skipText = true;
                pendingTR = true;
                cprops.AddToChain("tr", h);
                return;
            }
            if (tag.Equals("td") || tag.Equals("th")) {
                if (pendingTD)
                    EndElement(tag);
                skipText = false;
                pendingTD = true;
                cprops.AddToChain("td", h);
                stack.Push(new IncCell(tag, cprops));
                return;
            }
            if (tag.Equals("table")) {
                cprops.AddToChain("table", h);
                IncTable table = new IncTable(h);
                stack.Push(table);
                tableState.Push(new bool[]{pendingTR, pendingTD});
                pendingTR = pendingTD = false;
                skipText = true;
                return;
            }
        }
        
        public virtual void EndElement(String tag) {
            if (!tagsSupported.ContainsKey(tag))
                return;
            String follow = (String)FactoryProperties.followTags[tag];
            if (follow != null) {
                cprops.RemoveChain(follow);
                return;
            }
            if (tag.Equals("font") || tag.Equals("span")) {
                cprops.RemoveChain(tag);
                return;
            }
            if (tag.Equals("a")) {
                if (currentParagraph == null)
                    currentParagraph = new Paragraph();
                IALink i = null;
                bool skip = false;
                if (interfaceProps != null) {
                    i = (IALink)interfaceProps["alink_interface"];
                    if (i != null)
                        skip = i.Process(currentParagraph, cprops);
                }
                if (!skip) {
                    String href = cprops["href"];
                    if (href != null) {
                        ArrayList chunks = currentParagraph.Chunks;
                        for (int k = 0; k < chunks.Count; ++k) {
                            Chunk ck = (Chunk)chunks[k];
                            ck.SetAnchor(href);
                        }
                    }
                }
                Paragraph tmp = (Paragraph)stack.Pop();
                Phrase tmp2 = new Phrase();
                tmp2.Add(currentParagraph);
                tmp.Add(tmp2);
                currentParagraph = tmp;
                cprops.RemoveChain("a");
                return;
            }
            if (tag.Equals("br")) {
                return;
            }
            if (currentParagraph != null) {
                if (stack.Count == 0)
                    document.Add(currentParagraph);
                else {
                    Object obj = stack.Pop();
                    if (obj is ITextElementArray) {
                        ITextElementArray current = (ITextElementArray)obj;
                        current.Add(currentParagraph);
                    }
                    stack.Push(obj);
                }
            }
            currentParagraph = null;
            if (tag.Equals("ul") || tag.Equals("ol")) {
                if (pendingLI)
                    EndElement("li");
                skipText = false;
                cprops.RemoveChain(tag);
                if (stack.Count == 0)
                    return;
                Object obj = stack.Pop();
                if (!(obj is List)) {
                    stack.Push(obj);
                    return;
                }
                if (stack.Count == 0)
                    document.Add((IElement)obj);
                else
                    ((ITextElementArray)stack.Peek()).Add(obj);
                return;
            }
            if (tag.Equals("li")) {
                pendingLI = false;
                skipText = true;
                cprops.RemoveChain(tag);
                if (stack.Count == 0)
                    return;
                Object obj = stack.Pop();
                if (!(obj is ListItem)) {
                    stack.Push(obj);
                    return;
                }
                if (stack.Count == 0) {
                    document.Add((IElement)obj);
                    return;
                }
                Object list = stack.Pop();
                if (!(list is List)) {
                    stack.Push(list);
                    return;
                }
                ListItem item = (ListItem)obj;
                ((List)list).Add(item);
                ArrayList cks = item.Chunks;
                if (cks.Count > 0)
                    item.ListSymbol.Font = ((Chunk)cks[0]).Font;
                stack.Push(list);
                return;
            }
            if (tag.Equals("div") || tag.Equals("body")) {
                cprops.RemoveChain(tag);
                return;
            }
            if (tag.Equals("pre")) {
                cprops.RemoveChain(tag);
                isPRE = false;
                return;
            }
            if (tag.Equals("p")) {
                cprops.RemoveChain(tag);
                return;
            }
            if (tag.Equals("h1") || tag.Equals("h2") || tag.Equals("h3") || tag.Equals("h4") || tag.Equals("h5") || tag.Equals("h6")) {
                cprops.RemoveChain(tag);
                return;
            }
            if (tag.Equals("table")) {
                if (pendingTR)
                    EndElement("tr");
                cprops.RemoveChain("table");
                IncTable table = (IncTable) stack.Pop();
                PdfPTable tb = table.BuildTable();
                tb.SplitRows = true;
                if (stack.Count == 0)
                    document.Add(tb);
                else
                    ((ITextElementArray)stack.Peek()).Add(tb);
                bool[] state = (bool[])tableState.Pop();
                pendingTR = state[0];
                pendingTD = state[1];
                skipText = false;
                return;
            }
            if (tag.Equals("tr")) {
                if (pendingTD)
                    EndElement("td");
                pendingTR = false;
                cprops.RemoveChain("tr");
                ArrayList cells = new ArrayList();
                IncTable table = null;
                while (true) {
                    Object obj = stack.Pop();
                    if (obj is IncCell) {
                        cells.Add(((IncCell)obj).Cell);
                    }
                    if (obj is IncTable) {
                        table = (IncTable)obj;
                        break;
                    }
                }
                table.AddCols(cells);
                table.EndRow();
                stack.Push(table);
                skipText = true;
                return;
            }
            if (tag.Equals("td") || tag.Equals("th")) {
                pendingTD = false;
                cprops.RemoveChain("td");
                skipText = true;
                return;
            }
        }
        
        public virtual void Text(String str) {
            if (skipText)
                return;
            String content = str;
            if (isPRE) {
                if (currentParagraph == null)
                    currentParagraph = FactoryProperties.CreateParagraph(cprops);
                currentParagraph.Add(factoryProperties.CreateChunk(content, cprops));
                return;
            }
            if (content.Trim().Length == 0 && content.IndexOf(' ') < 0) {
                return;
            }
            
            StringBuilder buf = new StringBuilder();
            int len = content.Length;
            char character;
            bool newline = false;
            for (int i = 0; i < len; i++) {
                switch (character = content[i]) {
                    case ' ':
                        if (!newline) {
                            buf.Append(character);
                        }
                        break;
                    case '\n':
                        if (i > 0) {
                            newline = true;
                            buf.Append(' ');
                        }
                        break;
                    case '\r':
                        break;
                    case '\t':
                        break;
                    default:
                        newline = false;
                        buf.Append(character);
                        break;
                }
            }
            if (currentParagraph == null)
                currentParagraph = FactoryProperties.CreateParagraph(cprops);
            currentParagraph.Add(factoryProperties.CreateChunk(buf.ToString(), cprops));
        }
        
        public bool Add(IElement element) {
            objectList.Add(element);
            return true;
        }
        
        public void ClearTextWrap() {
        }
        
        public void Close() {
        }
        
        public bool NewPage() {
            return true;
        }
        
        public void Open() {
        }
        
        public void ResetFooter() {
        }
        
        public void ResetHeader() {
        }
        
        public void ResetPageCount() {
        }
        
        public bool SetMarginMirroring(bool marginMirroring) {
            return true;
        }
        
        public bool SetMargins(float marginLeft, float marginRight, float marginTop, float marginBottom) {
            return true;
        }
        
        public bool SetPageSize(Rectangle pageSize) {
            return true;
        }
        
        public const String tagsSupportedString = "ol ul li a pre font span br p div body table td th tr i b u sub sup em strong s strike"
            + " h1 h2 h3 h4 h5 h6 img";
        
        public static Hashtable tagsSupported = new Hashtable();
        
        static HTMLWorker() {
            StringTokenizer tok = new StringTokenizer(tagsSupportedString);
            while (tok.HasMoreTokens())
                tagsSupported[tok.NextToken()] = null;
        }
    
        public HeaderFooter Footer {
            set {
            }
        }
    
        public HeaderFooter Header {
            set {
            }
        }
    
        public int PageCount {
            set {
            }
        }

        private static float LengthParse(String txt, int c) {
            if (txt == null)
                return -1;
            if (txt.EndsWith("%")) {
                float vf = float.Parse(txt.Substring(0, txt.Length - 1), System.Globalization.NumberFormatInfo.InvariantInfo);
                return vf;
            }
            if (txt.EndsWith("px")) {
                float vf = float.Parse(txt.Substring(0, txt.Length - 2), System.Globalization.NumberFormatInfo.InvariantInfo);
                return vf;
            }
            int v = int.Parse(txt);
            return (float)v / c * 100f;
        }
    }
}