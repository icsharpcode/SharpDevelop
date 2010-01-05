using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Globalization;
using System.util;

using iTextSharp.text;
using iTextSharp.text.pdf;

/*
 * $Id: HtmlWriter.cs,v 1.24 2008/05/13 11:25:15 psoares33 Exp $
 * 
 *
 * Copyright 1999, 2000, 2001, 2002 by Bruno Lowagie.
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

namespace iTextSharp.text.html {
    /**
     * A <CODE>DocWriter</CODE> class for HTML.
     * <P>
     * An <CODE>HtmlWriter</CODE> can be added as a <CODE>DocListener</CODE>
     * to a certain <CODE>Document</CODE> by getting an instance.
     * Every <CODE>Element</CODE> added to the original <CODE>Document</CODE>
     * will be written to the <CODE>Stream</CODE> of this <CODE>HtmlWriter</CODE>.
     * <P>
     * Example:
     * <BLOCKQUOTE><PRE>
     * // creation of the document with a certain size and certain margins
     * Document document = new Document(PageSize.A4, 50, 50, 50, 50);
     * try {
     *    // this will write HTML to the Standard Stream
     *    <STRONG>HtmlWriter.GetInstance(document, System.out);</STRONG>
     *    // this will write HTML to a file called text.html
     *    <STRONG>HtmlWriter.GetInstance(document, new FileOutputStream("text.html"));</STRONG>
     *    // this will write HTML to for instance the Stream of a HttpServletResponse-object
     *    <STRONG>HtmlWriter.GetInstance(document, response.GetOutputStream());</STRONG>
     * }
     * catch (DocumentException de) {
     *    System.err.Println(de.GetMessage());
     * }
     * // this will close the document and all the OutputStreams listening to it
     * <STRONG>document.Close();</CODE>
     * </PRE></BLOCKQUOTE>
     */

    public class HtmlWriter : DocWriter {
    
        // static membervariables (tags)
    
        /** This is a possible HTML-tag. */
        public static byte[] BEGINCOMMENT = GetISOBytes("<!-- ");
    
        /** This is a possible HTML-tag. */
        public static byte[] ENDCOMMENT = GetISOBytes(" -->");
    
        /** This is a possible HTML-tag. */
        public const string NBSP = "&nbsp;";
    
        // membervariables
    
        /** This is the current font of the HTML. */
        protected Stack currentfont = new Stack();
    
        /** This is the standard font of the HTML. */
        protected Font standardfont = new Font();
    
        /** This is a path for images. */
        protected String imagepath = null;
    
        /** Stores the page number. */
        protected int pageN = 0;
    
        /** This is the textual part of a header */
        protected HeaderFooter header = null;
    
        /** This is the textual part of the footer */
        protected HeaderFooter footer = null;
    
        /** Store the markup properties of a MarkedObject. */
        protected Properties markup = new Properties();

        // constructor
    
        /**
         * Constructs a <CODE>HtmlWriter</CODE>.
         *
         * @param doc     The <CODE>Document</CODE> that has to be written as HTML
         * @param os      The <CODE>Stream</CODE> the writer has to write to.
         */
    
        protected HtmlWriter(Document doc, Stream os) : base(doc, os){
        
            document.AddDocListener(this);
            this.pageN = document.PageNumber;
            os.WriteByte(LT);
            byte[] tmp = GetISOBytes(HtmlTags.HTML);
            os.Write(tmp, 0, tmp.Length);
            os.WriteByte(GT);
            os.WriteByte(NEWLINE);
            os.WriteByte(TAB);
            os.WriteByte(LT);
            tmp = GetISOBytes(HtmlTags.HEAD);
            os.Write(tmp, 0, tmp.Length);
            os.WriteByte(GT);
        }
    
        // get an instance of the HtmlWriter
    
        /**
         * Gets an instance of the <CODE>HtmlWriter</CODE>.
         *
         * @param document  The <CODE>Document</CODE> that has to be written
         * @param os  The <CODE>Stream</CODE> the writer has to write to.
         * @return  a new <CODE>HtmlWriter</CODE>
         */
    
        public static HtmlWriter GetInstance(Document document, Stream os) {
            return new HtmlWriter(document, os);
        }
    
        // implementation of the DocListener methods
    
        /**
         * Signals that an new page has to be started.
         *
         * @return  <CODE>true</CODE> if this action succeeded, <CODE>false</CODE> if not.
         * @throws  DocumentException when a document isn't open yet, or has been closed
         */
    
        public override bool NewPage() {
            try {
                WriteStart(HtmlTags.DIV);
                Write(" ");
                Write(HtmlTags.STYLE);
                Write("=\"");
                WriteCssProperty(Markup.CSS_KEY_PAGE_BREAK_BEFORE, Markup.CSS_VALUE_ALWAYS);
                Write("\" /");
                os.WriteByte(GT);
            }
            catch (IOException ioe) {
                throw new DocumentException(ioe.Message);
            }
            return true;
        }
    
        /**
         * Signals that an <CODE>Element</CODE> was added to the <CODE>Document</CODE>.
         *
         * @return  <CODE>true</CODE> if the element was added, <CODE>false</CODE> if not.
         * @throws  DocumentException when a document isn't open yet, or has been closed
         */
    
        public override bool Add(IElement element) {
            if (pause) {
                return false;
            }
            if (open && !element.IsContent()) {
                throw new DocumentException("The document is open; you can only add Elements with content.");
            }
            switch (element.Type) {
                case Element.HEADER:
                    try {
                        Header h = (Header) element;
                        if (HtmlTags.STYLESHEET.Equals(h.Name)) {
                            WriteLink(h);
                        }
                        else if (HtmlTags.JAVASCRIPT.Equals(h.Name)) {
                            WriteJavaScript(h);
                        }
                        else {
                            WriteHeader(h);
                        }
                    }
                    catch (InvalidCastException) {
                    }
                    return true;
                case Element.SUBJECT:
                case Element.KEYWORDS:
                case Element.AUTHOR:
                    Meta meta = (Meta) element;
                    WriteHeader(meta);
                    return true;
                case Element.TITLE:
                    AddTabs(2);
                    WriteStart(HtmlTags.TITLE);
                    os.WriteByte(GT);
                    AddTabs(3);
                    Write(HtmlEncoder.Encode(((Meta)element).Content));
                    AddTabs(2);
                    WriteEnd(HtmlTags.TITLE);
                    return true;
                case Element.CREATOR:
                    WriteComment("Creator: " + HtmlEncoder.Encode(((Meta)element).Content));
                    return true;
                case Element.PRODUCER:
                    WriteComment("Producer: " + HtmlEncoder.Encode(((Meta)element).Content));
                    return true;
                case Element.CREATIONDATE:
                    WriteComment("Creationdate: " + HtmlEncoder.Encode(((Meta)element).Content));
                    return true;
                case Element.MARKED:
                	if (element is MarkedSection) {
                		MarkedSection ms = (MarkedSection)element;
                		AddTabs(1);
                        WriteStart(HtmlTags.DIV);
                        WriteMarkupAttributes(ms.MarkupAttributes);
                        os.WriteByte(GT);
                		MarkedObject mo = ((MarkedSection)element).Title;
                		if (mo != null) {
                            markup = mo.MarkupAttributes;
                			mo.Process(this);
                		}
                		ms.Process(this);
                        WriteEnd(HtmlTags.DIV);
                        return true;
                	}
                	else {
                		MarkedObject mo = (MarkedObject) element;
                		markup = mo.MarkupAttributes;
                    	return mo.Process(this);
                	}
                default:
                    Write(element, 2);
                    return true;
            }
        }
    
        /**
         * Signals that the <CODE>Document</CODE> has been opened and that
         * <CODE>Elements</CODE> can be added.
         * <P>
         * The <CODE>HEAD</CODE>-section of the HTML-document is written.
         */
    
        public override void Open() {
            base.Open();
            WriteComment(Document.Version);
            WriteComment("CreationDate: " + DateTime.Now.ToString());
            AddTabs(1);
            WriteEnd(HtmlTags.HEAD);
            AddTabs(1);
            WriteStart(HtmlTags.BODY);
            if (document.LeftMargin > 0) {
                Write(HtmlTags.LEFTMARGIN, document.LeftMargin.ToString());
            }
            if (document.RightMargin > 0) {
                Write(HtmlTags.RIGHTMARGIN, document.RightMargin.ToString());
            }
            if (document.TopMargin > 0) {
                Write(HtmlTags.TOPMARGIN, document.TopMargin.ToString());
            }
            if (document.BottomMargin > 0) {
                Write(HtmlTags.BOTTOMMARGIN, document.BottomMargin.ToString());
            }
            if (pageSize.BackgroundColor != null) {
                Write(HtmlTags.BACKGROUNDCOLOR, HtmlEncoder.Encode(pageSize.BackgroundColor));
            }
            if (document.JavaScript_onLoad != null) {
                Write(HtmlTags.JAVASCRIPT_ONLOAD, HtmlEncoder.Encode(document.JavaScript_onLoad));
            }
            if (document.JavaScript_onUnLoad != null) {
                Write(HtmlTags.JAVASCRIPT_ONUNLOAD, HtmlEncoder.Encode(document.JavaScript_onUnLoad));
            }
            if (document.HtmlStyleClass != null) {
                Write(Markup.HTML_ATTR_CSS_CLASS, document.HtmlStyleClass);
            }
            os.WriteByte(GT);
            InitHeader(); // line added by David Freels
        }
    
        /**
         * Signals that the <CODE>Document</CODE> was closed and that no other
         * <CODE>Elements</CODE> will be added.
         */
    
        public override void Close() {
            InitFooter(); // line added by David Freels
            AddTabs(1);
            WriteEnd(HtmlTags.BODY);
            os.WriteByte(NEWLINE);
            WriteEnd(HtmlTags.HTML);
            base.Close();
        }
    
        // some protected methods
    
        /**
         * Adds the header to the top of the </CODE>Document</CODE>
         */
    
        protected void InitHeader() {
            if (header != null) {
                Add(header.Paragraph);
            }
        }
    
        /**
         *  Adds the header to the top of the </CODE>Document</CODE>
         */
    
        protected void InitFooter() {
            if (footer != null) {
                // Set the page number. HTML has no notion of a page, so it should always
                // add up to 1
                footer.PageNumber = pageN + 1;
                Add(footer.Paragraph);
            }
        }
    
        /**
         * Writes a Metatag in the header.
         *
         * @param   meta   the element that has to be written
         * @throws  IOException
         */
    
        protected void WriteHeader(Meta meta) {
            AddTabs(2);
            WriteStart(HtmlTags.META);
            switch (meta.Type) {
                case Element.HEADER:
                    Write(HtmlTags.NAME, ((Header) meta).Name);
                    break;
                case Element.SUBJECT:
                    Write(HtmlTags.NAME, HtmlTags.SUBJECT);
                    break;
                case Element.KEYWORDS:
                    Write(HtmlTags.NAME, HtmlTags.KEYWORDS);
                    break;
                case Element.AUTHOR:
                    Write(HtmlTags.NAME, HtmlTags.AUTHOR);
                    break;
            }
            Write(HtmlTags.CONTENT, HtmlEncoder.Encode(meta.Content));
            WriteEnd();
        }
    
        /**
         * Writes a link in the header.
         *
         * @param   header   the element that has to be written
         * @throws  IOException
         */
    
        protected void WriteLink(Header header) {
            AddTabs(2);
            WriteStart(HtmlTags.LINK);
            Write(HtmlTags.REL, header.Name);
            Write(HtmlTags.TYPE, HtmlTags.TEXT_CSS);
            Write(HtmlTags.REFERENCE, header.Content);
            WriteEnd();
        }
    
        /**
         * Writes a JavaScript section or, if the markup attribute HtmlTags.URL is set, a JavaScript reference in the header.
         *
         * @param   header   the element that has to be written
         * @throws  IOException
         */
    
        protected void WriteJavaScript(Header header) {
            AddTabs(2);
            WriteStart(HtmlTags.SCRIPT);
            Write(HtmlTags.LANGUAGE, HtmlTags.JAVASCRIPT);
            if (markup.Count > 0) {
                /* JavaScript reference example:
                 *
                 * <script language="JavaScript" src="/myPath/MyFunctions.js"/>
                 */ 
                WriteMarkupAttributes(markup);
                os.WriteByte(GT);
                WriteEnd(HtmlTags.SCRIPT);
            }
            else {
                /* JavaScript coding convention:
                 *
                 * <script language="JavaScript" type="text/javascript">
                 * <!--
                 * // ... JavaScript methods ...
                 * //-->
                 * </script>
                 */ 
                Write(HtmlTags.TYPE, Markup.HTML_VALUE_JAVASCRIPT);
                os.WriteByte(GT);
                AddTabs(2);
                Write(Encoding.ASCII.GetString(BEGINCOMMENT) + "\n");
                Write(header.Content);
                AddTabs(2);
                Write("//" + Encoding.ASCII.GetString(ENDCOMMENT));
                AddTabs(2);
                WriteEnd(HtmlTags.SCRIPT);
            }
        }
    
        /**
         * Writes some comment.
         * <P>
         * This method writes some comment.
         *
         * @param comment   the comment that has to be written
         * @throws  IOException
         */
    
        protected void WriteComment(String comment) {
            AddTabs(2);
            os.Write(BEGINCOMMENT, 0, BEGINCOMMENT.Length);
            Write(comment);
            os.Write(ENDCOMMENT, 0, ENDCOMMENT.Length);
        }
    
        // public methods
    
        /**
         * Changes the standardfont.
         *
         * @param standardFont  The font
         */
    
        public void SetStandardFont(Font standardFont) {
            this.standardfont = standardFont;
        }
    
        /**
         * Checks if a given font is the same as the font that was last used.
         *
         * @param   font    the font of an object
         * @return  true if the font differs
         */
    
        public bool IsOtherFont(Font font) {
            try {
                Font cFont = (Font) currentfont.Peek();
                if (cFont.CompareTo(font) == 0) return false;
                return true;
            }
            catch (InvalidOperationException) {
                if (standardfont.CompareTo(font) == 0) return false;
                return true;
            }
        }
    
        /**
         * Sets the basepath for images.
         * <P>
         * This is especially useful if you add images using a file,
         * rather than an URL. In PDF there is no problem, since
         * the images are added inline, but in HTML it is sometimes
         * necessary to use a relative path or a special path to some
         * images directory.
         *
         * @param imagepath the new imagepath
         */
    
        public void SetImagepath(String imagepath) {
            this.imagepath = imagepath;
        }
    
        /**
         * Resets the imagepath.
         */
    
        public void ResetImagepath() {
            imagepath = null;
        }
    
        /**
         * Changes the header of this document.
         *
         * @param header    the new header
         */
    
        public void SetHeader(HeaderFooter header) {
            this.header = header;
        }
    
        /**
         * Changes the footer of this document.
         *
         * @param footer    the new footer
         */
    
        public void SetFooter(HeaderFooter footer) {
            this.footer = footer;
        }
    
        /**
         * Signals that a <CODE>String</CODE> was added to the <CODE>Document</CODE>.
         *
         * @return  <CODE>true</CODE> if the string was added, <CODE>false</CODE> if not.
         * @throws  DocumentException when a document isn't open yet, or has been closed
         */
    
        public bool Add(String str) {
            if (pause) {
                return false;
            }
            Write(str);
            return true;
        }
    
        /**
         * Writes the HTML representation of an element.
         *
         * @param   element     the element
         * @param   indent      the indentation
         */
    
        protected void Write(IElement element, int indent) {
            Properties styleAttributes = null;
            switch (element.Type) {
                case Element.MARKED: {
                    try {
                        Add(element);
                    } catch (DocumentException) {
                    }
                    return;
                }
                case Element.CHUNK: {
                    Chunk chunk = (Chunk) element;
                    // if the chunk contains an image, return the image representation
                    Image image = chunk.GetImage();
                    if (image != null) {
                        Write(image, indent);
                        return;
                    }
                
                    if (chunk.IsEmpty()) return;
                    Hashtable attributes = chunk.Attributes;
                    if (attributes != null && attributes[Chunk.NEWPAGE] != null) {
                        return;
                    }
                    bool tag = IsOtherFont(chunk.Font) || markup.Count > 0;
                    if (tag) {
                        // start span tag
                        AddTabs(indent);
                        WriteStart(HtmlTags.SPAN);
                        if (IsOtherFont(chunk.Font)) {
                            Write(chunk.Font, null);
                        }
                        WriteMarkupAttributes(markup);
                        os.WriteByte(GT);
                    }
                    if (attributes != null && attributes[Chunk.SUBSUPSCRIPT] != null) {
                        // start sup or sub tag
                        if ((float)attributes[Chunk.SUBSUPSCRIPT] > 0) {
                            WriteStart(HtmlTags.SUP);
                        }
                        else {
                            WriteStart(HtmlTags.SUB);
                        }
                        os.WriteByte(GT);
                    }
                    // contents
                    Write(HtmlEncoder.Encode(chunk.Content));
                    if (attributes != null && attributes[Chunk.SUBSUPSCRIPT] != null) {
                        // end sup or sub tag
                        os.WriteByte(LT);
                        os.WriteByte(FORWARD);
                        if ((float)attributes[Chunk.SUBSUPSCRIPT] > 0) {
                            Write(HtmlTags.SUP);
                        }
                        else {
                            Write(HtmlTags.SUB);
                        }
                        os.WriteByte(GT);
                    }
                    if (tag) {
                        // end tag
                        WriteEnd(Markup.HTML_TAG_SPAN);
                    }
                    return;
                }
                case Element.PHRASE: {
                    Phrase phrase = (Phrase) element;
                    styleAttributes = new Properties();
                    if (phrase.HasLeading()) styleAttributes[Markup.CSS_KEY_LINEHEIGHT] = phrase.Leading.ToString() + "pt";
                
                    // start tag
                    AddTabs(indent);
                    WriteStart(Markup.HTML_TAG_SPAN);
                    WriteMarkupAttributes(markup);
                    Write(phrase.Font, styleAttributes);
                    os.WriteByte(GT);
                    currentfont.Push(phrase.Font);
                    // contents
                    foreach (IElement i in phrase) {
                        Write(i, indent + 1);
                    }
                    // end tag
                    AddTabs(indent);
                    WriteEnd(Markup.HTML_TAG_SPAN);
                    currentfont.Pop();
                    return;
                }
                case Element.ANCHOR: {
                    Anchor anchor = (Anchor) element;
                    styleAttributes = new Properties();
                    if (anchor.HasLeading()) styleAttributes[Markup.CSS_KEY_LINEHEIGHT] = anchor.Leading.ToString() + "pt";
                
                    // start tag
                    AddTabs(indent);
                    WriteStart(HtmlTags.ANCHOR);
                    if (anchor.Name != null) {
                        Write(HtmlTags.NAME, anchor.Name);
                    }
                    if (anchor.Reference != null) {
                        Write(HtmlTags.REFERENCE, anchor.Reference);
                    }
                    WriteMarkupAttributes(markup);
                    Write(anchor.Font, styleAttributes);
                    os.WriteByte(GT);
                    currentfont.Push(anchor.Font);
                    // contents
                    foreach (IElement i in anchor) {
                        Write(i, indent + 1);
                    }
                    // end tag
                    AddTabs(indent);
                    WriteEnd(HtmlTags.ANCHOR);
                    currentfont.Pop();
                    return;
                }
                case Element.PARAGRAPH: {
                    Paragraph paragraph = (Paragraph) element;
                    styleAttributes = new Properties();
                    if (paragraph.HasLeading()) styleAttributes[Markup.CSS_KEY_LINEHEIGHT] = paragraph.TotalLeading.ToString() + "pt";
                
                    // start tag
                    AddTabs(indent);
                    WriteStart(HtmlTags.DIV);
                    WriteMarkupAttributes(markup);
                    String alignment = HtmlEncoder.GetAlignment(paragraph.Alignment);
                    if (!"".Equals(alignment)) {
                        Write(HtmlTags.ALIGN, alignment);
                    }
                    Write(paragraph.Font, styleAttributes);
                    os.WriteByte(GT);
                    currentfont.Push(paragraph.Font);
                    // contents
                    foreach (IElement i in paragraph) {
                        Write(i, indent + 1);
                    }
                    // end tag
                    AddTabs(indent);
                    WriteEnd(HtmlTags.DIV);
                    currentfont.Pop();
                    return;
                }
                case Element.SECTION:
                case Element.CHAPTER: {
                    // part of the start tag + contents
                    WriteSection((Section) element, indent);
                    return;
                }
                case Element.LIST: {
                    List list = (List) element;
                    // start tag
                    AddTabs(indent);
                    if (list.Numbered) {
                        WriteStart(HtmlTags.ORDEREDLIST);
                    }
                    else {
                        WriteStart(HtmlTags.UNORDEREDLIST);
                    }
                    WriteMarkupAttributes(markup);
                    os.WriteByte(GT);
                    // contents
                    foreach (IElement i in list.Items) {
                        Write(i, indent + 1);
                    }
                    // end tag
                    AddTabs(indent);
                    if (list.Numbered) {
                        WriteEnd(HtmlTags.ORDEREDLIST);
                    }
                    else {
                        WriteEnd(HtmlTags.UNORDEREDLIST);
                    }
                    return;
                }
                case Element.LISTITEM: {
                    ListItem listItem = (ListItem) element;
                    styleAttributes = new Properties();
                    if (listItem.HasLeading()) styleAttributes[Markup.CSS_KEY_LINEHEIGHT] = listItem.TotalLeading.ToString() + "pt";
                
                    // start tag
                    AddTabs(indent);
                    WriteStart(HtmlTags.LISTITEM);
                    WriteMarkupAttributes(markup);
                    Write(listItem.Font, styleAttributes);
                    os.WriteByte(GT);
                    currentfont.Push(listItem.Font);
                    // contents
                    foreach (IElement i in listItem) {
                        Write(i, indent + 1);
                    }
                    // end tag
                    AddTabs(indent);
                    WriteEnd(HtmlTags.LISTITEM);
                    currentfont.Pop();
                    return;
                }
                case Element.CELL: {
                    Cell cell = (Cell) element;
                
                    // start tag
                    AddTabs(indent);
                    if (cell.Header) {
                        WriteStart(HtmlTags.HEADERCELL);
                    }
                    else {
                        WriteStart(HtmlTags.CELL);
                    }
                    WriteMarkupAttributes(markup);
                    if (cell.BorderWidth != Rectangle.UNDEFINED) {
                        Write(HtmlTags.BORDERWIDTH, cell.BorderWidth.ToString());
                    }
                    if (cell.BorderColor != null) {
                        Write(HtmlTags.BORDERCOLOR, HtmlEncoder.Encode(cell.BorderColor));
                    }
                    if (cell.BackgroundColor != null) {
                        Write(HtmlTags.BACKGROUNDCOLOR, HtmlEncoder.Encode(cell.BackgroundColor));
                    }
                    String alignment = HtmlEncoder.GetAlignment(cell.HorizontalAlignment);
                    if (!"".Equals(alignment)) {
                        Write(HtmlTags.HORIZONTALALIGN, alignment);
                    }
                    alignment = HtmlEncoder.GetAlignment(cell.VerticalAlignment);
                    if (!"".Equals(alignment)) {
                        Write(HtmlTags.VERTICALALIGN, alignment);
                    }
                    if (cell.GetWidthAsString() != null) {
                        Write(HtmlTags.WIDTH, cell.GetWidthAsString());
                    }
                    if (cell.Colspan != 1) {
                        Write(HtmlTags.COLSPAN, cell.Colspan.ToString());
                    }
                    if (cell.Rowspan != 1) {
                        Write(HtmlTags.ROWSPAN, cell.Rowspan.ToString());
                    }
                    if (cell.MaxLines == 1) {
                        Write(HtmlTags.STYLE, "white-space: nowrap;");
                    }
                    os.WriteByte(GT);
                    // contents
                    if (cell.IsEmpty()) {
                        Write(NBSP);
                    } else {
                        foreach (IElement i in cell.Elements) {
                            Write(i, indent + 1);
                        }
                    }
                    // end tag
                    AddTabs(indent);
                    if (cell.Header) {
                        WriteEnd(HtmlTags.HEADERCELL);
                    }
                    else {
                        WriteEnd(HtmlTags.CELL);
                    }
                    return;
                }
                case Element.ROW: {
                    Row row = (Row) element;
                
                    // start tag
                    AddTabs(indent);
                    WriteStart(HtmlTags.ROW);
                    WriteMarkupAttributes(markup);
                    os.WriteByte(GT);
                    // contents
                    IElement cell;
                    for (int i = 0; i < row.Columns; i++) {
                        if ((cell = (IElement)row.GetCell(i)) != null) {
                            Write(cell, indent + 1);
                        }
                    }
                    // end tag
                    AddTabs(indent);
                    WriteEnd(HtmlTags.ROW);
                    return;
                }
                case Element.TABLE: {
                    Table table;
                    try {
                        table = (Table) element;
                    }
                    catch (InvalidCastException) {
                        table = ((SimpleTable)element).CreateTable();
                    }
                    table.Complete();
                    // start tag
                    AddTabs(indent);
                    WriteStart(HtmlTags.TABLE);
                    WriteMarkupAttributes(markup);
                    os.WriteByte(SPACE);
                    Write(HtmlTags.WIDTH);
                    os.WriteByte(EQUALS);
                    os.WriteByte(QUOTE);
                    Write(table.Width.ToString(System.Globalization.CultureInfo.InvariantCulture));
                    if (!table.Locked){
                        Write("%");
                    }
                    os.WriteByte(QUOTE);
                    String alignment = HtmlEncoder.GetAlignment(table.Alignment);
                    if (!"".Equals(alignment)) {
                        Write(HtmlTags.ALIGN, alignment);
                    }
                    Write(HtmlTags.CELLPADDING, table.Cellpadding.ToString(System.Globalization.CultureInfo.InvariantCulture));
                    Write(HtmlTags.CELLSPACING, table.Cellspacing.ToString(System.Globalization.CultureInfo.InvariantCulture));
                    if (table.BorderWidth != Rectangle.UNDEFINED) {
                        Write(HtmlTags.BORDERWIDTH, table.BorderWidth.ToString(System.Globalization.CultureInfo.InvariantCulture));
                    }
                    if (table.BorderColor != null) {
                        Write(HtmlTags.BORDERCOLOR, HtmlEncoder.Encode(table.BorderColor));
                    }
                    if (table.BackgroundColor != null) {
                        Write(HtmlTags.BACKGROUNDCOLOR, HtmlEncoder.Encode(table.BackgroundColor));
                    }
                    os.WriteByte(GT);
                    // contents
                    foreach (Row row in table) {
                        Write(row, indent + 1);
                    }
                    // end tag
                    AddTabs(indent);
                    WriteEnd(HtmlTags.TABLE);
                    return;
                }
                case Element.ANNOTATION: {
                    Annotation annotation = (Annotation) element;
                    WriteComment(annotation.Title + ": " + annotation.Content);
                    return;
                }
                case Element.IMGRAW:
                case Element.JPEG:
                case Element.JPEG2000:
                case Element.IMGTEMPLATE: {
                    Image image = (Image) element;
                    if (image.Url == null) {
                        return;
                    }
                
                    // start tag
                    AddTabs(indent);
                    WriteStart(HtmlTags.IMAGE);
                    String path = image.Url.ToString();
                    if (imagepath != null) {
                        if (path.IndexOf('/') > 0) {
                            path = imagepath + path.Substring(path.LastIndexOf('/') + 1);
                        }
                        else {
                            path = imagepath + path;
                        }
                    }
                    Write(HtmlTags.URL, path);
                    if ((image.Alignment & Image.RIGHT_ALIGN) > 0) {
                        Write(HtmlTags.ALIGN, HtmlTags.ALIGN_RIGHT);
                    }
                    else if ((image.Alignment & Image.MIDDLE_ALIGN) > 0) {
                        Write(HtmlTags.ALIGN, HtmlTags.ALIGN_MIDDLE);
                    }
                    else {
                        Write(HtmlTags.ALIGN, HtmlTags.ALIGN_LEFT);
                    }
                    if (image.Alt != null) {
                        Write(HtmlTags.ALT, image.Alt);
                    }
                    Write(HtmlTags.PLAINWIDTH, image.ScaledWidth.ToString());
                    Write(HtmlTags.PLAINHEIGHT, image.ScaledHeight.ToString());
                    WriteMarkupAttributes(markup);
                    WriteEnd();
                    return;
                }
            
                default:
                    return;
            }
        }
    
        /**
         * Writes the HTML representation of a section.
         *
         * @param   section     the section to write
         * @param   indent      the indentation
         */
    
        protected void WriteSection(Section section, int indent) {
            if (section.Title != null) {
                int depth = section.Depth - 1;
                if (depth > 5) {
                    depth = 5;
                }
                Properties styleAttributes = new Properties();
                if (section.Title.HasLeading()) styleAttributes[Markup.CSS_KEY_LINEHEIGHT] = section.Title.TotalLeading.ToString() + "pt";
                // start tag
                AddTabs(indent);
                WriteStart(HtmlTags.H[depth]);
                Write(section.Title.Font, styleAttributes);
                String alignment = HtmlEncoder.GetAlignment(section.Title.Alignment);
                if (!"".Equals(alignment)) {
                    Write(HtmlTags.ALIGN, alignment);
                }
                WriteMarkupAttributes(markup);
                os.WriteByte(GT);
                currentfont.Push(section.Title.Font);
                // contents
                foreach (IElement i in section.Title) {
                    Write(i, indent + 1);
                }
                // end tag
                AddTabs(indent);
                WriteEnd(HtmlTags.H[depth]);
                currentfont.Pop();
            }
            foreach (IElement i in section) {
                Write(i, indent);
            }
        }
    
        /**
         * Writes the representation of a <CODE>Font</CODE>.
         *
         * @param font              a <CODE>Font</CODE>
         * @param styleAttributes   the style of the font
         */
    
        protected void Write(Font font, Properties styleAttributes) {
            if (font == null || !IsOtherFont(font) /*|| styleAttributes == null*/) return;
            Write(" ");
            Write(HtmlTags.STYLE);
            Write("=\"");
            if (styleAttributes != null) {
                foreach (String key in styleAttributes.Keys) {
                    WriteCssProperty(key, styleAttributes[key]);
                }
            }
            if (IsOtherFont(font)) {
                WriteCssProperty(Markup.CSS_KEY_FONTFAMILY, font.Familyname);
            
                if (font.Size != Font.UNDEFINED) {
                    WriteCssProperty(Markup.CSS_KEY_FONTSIZE, font.Size.ToString() + "pt");
                }
                if (font.Color != null) {
                    WriteCssProperty(Markup.CSS_KEY_COLOR, HtmlEncoder.Encode(font.Color));
                }
            
                int fontstyle = font.Style;
                BaseFont bf = font.BaseFont;
                if (bf != null) {
                    String ps = bf.PostscriptFontName.ToLower(CultureInfo.InvariantCulture);
                    if (ps.IndexOf("bold") >= 0) {
                        if (fontstyle == Font.UNDEFINED)
                            fontstyle = 0;
                        fontstyle |= Font.BOLD;
                    }
                    if (ps.IndexOf("italic") >= 0 || ps.IndexOf("oblique") >= 0) {
                        if (fontstyle == Font.UNDEFINED)
                            fontstyle = 0;
                        fontstyle |= Font.ITALIC;
                    }
                }
                if (fontstyle != Font.UNDEFINED && fontstyle != Font.NORMAL) {
                    switch (fontstyle & Font.BOLDITALIC) {
                        case Font.BOLD:
                            WriteCssProperty(Markup.CSS_KEY_FONTWEIGHT, Markup.CSS_VALUE_BOLD);
                            break;
                        case Font.ITALIC:
                            WriteCssProperty(Markup.CSS_KEY_FONTSTYLE, Markup.CSS_VALUE_ITALIC);
                            break;
                        case Font.BOLDITALIC:
                            WriteCssProperty(Markup.CSS_KEY_FONTWEIGHT, Markup.CSS_VALUE_BOLD);
                            WriteCssProperty(Markup.CSS_KEY_FONTSTYLE, Markup.CSS_VALUE_ITALIC);
                            break;
                    }
                
                    // CSS only supports one decoration tag so if both are specified
                    // only one of the two will display
                    if ((fontstyle & Font.UNDERLINE) > 0) {
                        WriteCssProperty(Markup.CSS_KEY_TEXTDECORATION, Markup.CSS_VALUE_UNDERLINE);
                    }
                    if ((fontstyle & Font.STRIKETHRU) > 0) {
                        WriteCssProperty(Markup.CSS_KEY_TEXTDECORATION, Markup.CSS_VALUE_LINETHROUGH);
                    }
                }
            }
            Write("\"");
        }
    
        /**
         * Writes out a CSS property.
         */
        protected void WriteCssProperty(String prop, String value) {
            Write(new StringBuilder(prop).Append(": ").Append(value).Append("; ").ToString());
        }
    }
}