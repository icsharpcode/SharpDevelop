using System;
using System.Collections;
using System.IO;
using System.util.collections;
using iTextSharp.text;
using iTextSharp.text.pdf.intern;
using iTextSharp.text.pdf.draw;
using iTextSharp.text.pdf.collection;
using System.util;
/*
 * 
 * $Id: PdfDocument.cs,v 1.75 2008/05/13 11:25:19 psoares33 Exp $
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
    * <CODE>PdfDocument</CODE> is the class that is used by <CODE>PdfWriter</CODE>
    * to translate a <CODE>Document</CODE> into a PDF with different pages.
    * <P>
    * A <CODE>PdfDocument</CODE> always listens to a <CODE>Document</CODE>
    * and adds the Pdf representation of every <CODE>Element</CODE> that is
    * added to the <CODE>Document</CODE>.
    *
    * @see      com.lowagie.text.Document
    * @see      com.lowagie.text.DocListener
    * @see      PdfWriter
    */

    public class PdfDocument : Document {
        
        /**
        * <CODE>PdfInfo</CODE> is the PDF InfoDictionary.
        * <P>
        * A document's trailer may contain a reference to an Info dictionary that provides information
        * about the document. This optional dictionary may contain one or more keys, whose values
        * should be strings.<BR>
        * This object is described in the 'Portable Document Format Reference Manual version 1.3'
        * section 6.10 (page 120-121)
        */
        
        public class PdfInfo : PdfDictionary {
            
            // constructors
            
            /**
            * Construct a <CODE>PdfInfo</CODE>-object.
            */
            
            internal PdfInfo() {
                AddProducer();
                AddCreationDate();
            }
            
            /**
            * Constructs a <CODE>PdfInfo</CODE>-object.
            *
            * @param        author      name of the author of the document
            * @param        title       title of the document
            * @param        subject     subject of the document
            */
            
            internal PdfInfo(String author, String title, String subject) : base() {
                AddTitle(title);
                AddSubject(subject);
                AddAuthor(author);
            }
            
            /**
            * Adds the title of the document.
            *
            * @param    title       the title of the document
            */
            
            internal void AddTitle(String title) {
                Put(PdfName.TITLE, new PdfString(title, PdfObject.TEXT_UNICODE));
            }
            
            /**
            * Adds the subject to the document.
            *
            * @param    subject     the subject of the document
            */
            
            internal void AddSubject(String subject) {
                Put(PdfName.SUBJECT, new PdfString(subject, PdfObject.TEXT_UNICODE));
            }
            
            /**
            * Adds some keywords to the document.
            *
            * @param    keywords        the keywords of the document
            */
            
            internal void AddKeywords(String keywords) {
                Put(PdfName.KEYWORDS, new PdfString(keywords, PdfObject.TEXT_UNICODE));
            }
            
            /**
            * Adds the name of the author to the document.
            *
            * @param    author      the name of the author
            */
            
            internal void AddAuthor(String author) {
                Put(PdfName.AUTHOR, new PdfString(author, PdfObject.TEXT_UNICODE));
            }
            
            /**
            * Adds the name of the creator to the document.
            *
            * @param    creator     the name of the creator
            */
            
            internal void AddCreator(String creator) {
                Put(PdfName.CREATOR, new PdfString(creator, PdfObject.TEXT_UNICODE));
            }
            
            /**
            * Adds the name of the producer to the document.
            */
            
            internal void AddProducer() {
                // This line may only be changed by Bruno Lowagie or Paulo Soares
                Put(PdfName.PRODUCER, new PdfString(Version));
                // Do not edit the line above!
            }
            
            /**
            * Adds the date of creation to the document.
            */
            
            internal void AddCreationDate() {
                PdfString date = new PdfDate();
                Put(PdfName.CREATIONDATE, date);
                Put(PdfName.MODDATE, date);
            }
            
            internal void Addkey(String key, String value) {
                if (key.Equals("Producer") || key.Equals("CreationDate"))
                    return;
                Put(new PdfName(key), new PdfString(value, PdfObject.TEXT_UNICODE));
            }
        }
        
        /**
        * <CODE>PdfCatalog</CODE> is the PDF Catalog-object.
        * <P>
        * The Catalog is a dictionary that is the root node of the document. It contains a reference
        * to the tree of pages contained in the document, a reference to the tree of objects representing
        * the document's outline, a reference to the document's article threads, and the list of named
        * destinations. In addition, the Catalog indicates whether the document's outline or thumbnail
        * page images should be displayed automatically when the document is viewed and wether some location
        * other than the first page should be shown when the document is opened.<BR>
        * In this class however, only the reference to the tree of pages is implemented.<BR>
        * This object is described in the 'Portable Document Format Reference Manual version 1.3'
        * section 6.2 (page 67-71)
        */
        
        internal class PdfCatalog : PdfDictionary {
            
            internal PdfWriter writer;
            // constructors
            
            /**
            * Constructs a <CODE>PdfCatalog</CODE>.
            *
            * @param        pages       an indirect reference to the root of the document's Pages tree.
            * @param writer the writer the catalog applies to
            */
            
            internal PdfCatalog(PdfIndirectReference pages, PdfWriter writer) : base(CATALOG) {
                this.writer = writer;
                Put(PdfName.PAGES, pages);
            }
            
            /**
            * Adds the names of the named destinations to the catalog.
            * @param localDestinations the local destinations
            * @param documentJavaScript the javascript used in the document
            * @param writer the writer the catalog applies to
            */
            internal void AddNames(k_Tree localDestinations, Hashtable documentLevelJS, Hashtable documentFileAttachment, PdfWriter writer) {
                if (localDestinations.Count == 0 && documentLevelJS.Count == 0 && documentFileAttachment.Count == 0)
                    return;
                PdfDictionary names = new PdfDictionary();
                if (localDestinations.Count > 0) {
                    PdfArray ar = new PdfArray();
                    foreach (DictionaryEntry entry in localDestinations) {
                        String name = (String)entry.Key;
                        Object[] obj = (Object[])entry.Value;
                        PdfIndirectReference refi = (PdfIndirectReference)obj[1];
                        ar.Add(new PdfString(name, null));
                        ar.Add(refi);
                    }
                    PdfDictionary dests = new PdfDictionary();
                    dests.Put(PdfName.NAMES, ar);
                    names.Put(PdfName.DESTS, writer.AddToBody(dests).IndirectReference);
                }
                if (documentLevelJS.Count > 0) {
                    PdfDictionary tree = PdfNameTree.WriteTree(documentLevelJS, writer);
                    names.Put(PdfName.JAVASCRIPT, writer.AddToBody(tree).IndirectReference);
                }
                if (documentFileAttachment.Count > 0) {
                    names.Put(PdfName.EMBEDDEDFILES, writer.AddToBody(PdfNameTree.WriteTree(documentFileAttachment, writer)).IndirectReference);
                }
                Put(PdfName.NAMES, writer.AddToBody(names).IndirectReference);
            }
            
            internal PdfAction OpenAction {
                set {
                    Put(PdfName.OPENACTION, value);
                }
            }
            
            
            /** Sets the document level additional actions.
            * @param actions   dictionary of actions
            */
            internal PdfDictionary AdditionalActions {
                set {
                    Put(PdfName.AA, writer.AddToBody(value).IndirectReference);
                }
            }
        }
        
    // CONSTRUCTING A PdfDocument/PdfWriter INSTANCE

        /**
        * Constructs a new PDF document.
        * @throws DocumentException on error
        */
        internal PdfDocument() {
            AddProducer();
            AddCreationDate();
        }
        
        /** The <CODE>PdfWriter</CODE>. */
        protected internal PdfWriter writer;
        
        /**
        * Adds a <CODE>PdfWriter</CODE> to the <CODE>PdfDocument</CODE>.
        *
        * @param writer the <CODE>PdfWriter</CODE> that writes everything
        *                     what is added to this document to an outputstream.
        * @throws DocumentException on error
        */
        internal void AddWriter(PdfWriter writer) {
            if (this.writer == null) {
                this.writer = writer;
                annotationsImp = new PdfAnnotationsImp(writer);
                return;
            }
            throw new DocumentException("You can only add a writer to a PdfDocument once.");
        }
        
    // LISTENER METHODS START
        
    //  [L0] ElementListener interface
        
        /** This is the PdfContentByte object, containing the text. */
        protected internal PdfContentByte text;
        
        /** This is the PdfContentByte object, containing the borders and other Graphics. */
        protected internal PdfContentByte graphics;
        
        /** This represents the leading of the lines. */
        protected internal float leading = 0;
        
        /**
        * Getter for the current leading.
        * @return  the current leading
        * @since   2.1.2
        */
        public float Leading {
            get {
                return leading;
            }
        }
        /** This is the current height of the document. */
        protected internal float currentHeight = 0;
        
        /**
        * Signals that onParagraph is valid (to avoid that a Chapter/Section title is treated as a Paragraph).
        * @since 2.1.2
        */
        protected bool isSectionTitle = false;
        
        /**
        * Signals that the current leading has to be subtracted from a YMark object.
        * @since 2.1.2
        */
        protected int leadingCount = 0;

        /** This represents the current alignment of the PDF Elements. */
        protected internal int alignment = Element.ALIGN_LEFT;
        
        /** The current active <CODE>PdfAction</CODE> when processing an <CODE>Anchor</CODE>. */
        protected internal PdfAction anchorAction = null;
        
        /**
        * Signals that an <CODE>Element</CODE> was added to the <CODE>Document</CODE>.
        *
        * @param element the element to add
        * @return <CODE>true</CODE> if the element was added, <CODE>false</CODE> if not.
        * @throws DocumentException when a document isn't open yet, or has been closed
        */
        public override bool Add(IElement element) {
            if (writer != null && writer.IsPaused()) {
                return false;
            }
            switch (element.Type) {
                
                // Information (headers)
                case Element.HEADER:
                    info.Addkey(((Meta)element).Name, ((Meta)element).Content);
                    break;
                case Element.TITLE:
                    info.AddTitle(((Meta)element).Content);
                    break;
                case Element.SUBJECT:
                    info.AddSubject(((Meta)element).Content);
                    break;
                case Element.KEYWORDS:
                    info.AddKeywords(((Meta)element).Content);
                    break;
                case Element.AUTHOR:
                    info.AddAuthor(((Meta)element).Content);
                    break;
                case Element.CREATOR:
                    info.AddCreator(((Meta)element).Content);
                    break;
                case Element.PRODUCER:
                    // you can not change the name of the producer
                    info.AddProducer();
                    break;
                case Element.CREATIONDATE:
                    // you can not set the creation date, only reset it
                    info.AddCreationDate();
                    break;
                    
                    // content (text)
                case Element.CHUNK: {
                    // if there isn't a current line available, we make one
                    if (line == null) {
                        CarriageReturn();
                    }
                    
                    // we cast the element to a chunk
                    PdfChunk chunk = new PdfChunk((Chunk) element, anchorAction);
                    // we try to add the chunk to the line, until we succeed
                    {
                        PdfChunk overflow;
                        while ((overflow = line.Add(chunk)) != null) {
                            CarriageReturn();
                            chunk = overflow;
                            chunk.TrimFirstSpace();
                        }
                    }
                    pageEmpty = false;
                    if (chunk.IsAttribute(Chunk.NEWPAGE)) {
                        NewPage();
                    }
                    break;
                }
                case Element.ANCHOR: {
                    leadingCount++;
                    Anchor anchor = (Anchor) element;
                    String url = anchor.Reference;
                    leading = anchor.Leading;
                    if (url != null) {
                        anchorAction = new PdfAction(url);
                    }
                    
                    // we process the element
                    element.Process(this);
                    anchorAction = null;
                    leadingCount--;
                    break;
                }
                case Element.ANNOTATION: {
                    if (line == null) {
                        CarriageReturn();
                    }
                    Annotation annot = (Annotation) element;
                    Rectangle rect = new Rectangle(0, 0);
                    if (line != null)
                        rect = new Rectangle(annot.GetLlx(IndentRight - line.WidthLeft), annot.GetLly(IndentTop - currentHeight), annot.GetUrx(IndentRight - line.WidthLeft + 20), annot.GetUry(IndentTop - currentHeight - 20));
                    PdfAnnotation an = PdfAnnotationsImp.ConvertAnnotation(writer, annot, rect);
                    annotationsImp.AddPlainAnnotation(an);
                    pageEmpty = false;
                    break;
                }
                case Element.PHRASE: {
                    leadingCount++;
                    // we cast the element to a phrase and set the leading of the document
                    leading = ((Phrase) element).Leading;
                    // we process the element
                    element.Process(this);
                    leadingCount--;
                    break;
                }
                case Element.PARAGRAPH: {
                    leadingCount++;
                    // we cast the element to a paragraph
                    Paragraph paragraph = (Paragraph) element;
                    
                    AddSpacing(paragraph.SpacingBefore, leading, paragraph.Font);
                    
                    // we adjust the parameters of the document
                    alignment = paragraph.Alignment;
                    leading = paragraph.TotalLeading;
                    
                    CarriageReturn();
                    // we don't want to make orphans/widows
                    if (currentHeight + line.Height + leading > IndentTop - IndentBottom) {
                        NewPage();
                    }

                    indentation.indentLeft += paragraph.IndentationLeft;
                    indentation.indentRight += paragraph.IndentationRight;
                    
                    CarriageReturn();

                    IPdfPageEvent pageEvent = writer.PageEvent;
                    if (pageEvent != null && !isSectionTitle)
                        pageEvent.OnParagraph(writer, this, IndentTop - currentHeight);
                    
                    // if a paragraph has to be kept together, we wrap it in a table object
                    if (paragraph.KeepTogether) {
                        CarriageReturn();
                        PdfPTable table = new PdfPTable(1);
                        table.WidthPercentage = 100f;
                        PdfPCell cell = new PdfPCell();
                        cell.AddElement(paragraph);
                        cell.Border = Rectangle.NO_BORDER;
                        cell.Padding = 0;
                        table.AddCell(cell);
                        indentation.indentLeft -= paragraph.IndentationLeft;
                        indentation.indentRight -= paragraph.IndentationRight;
                        this.Add(table);
                        indentation.indentLeft += paragraph.IndentationLeft;
                        indentation.indentRight += paragraph.IndentationRight;
                    }
                    else {
                        line.SetExtraIndent(paragraph.FirstLineIndent);
                        element.Process(this);
                        CarriageReturn();
                        AddSpacing(paragraph.SpacingAfter, paragraph.TotalLeading, paragraph.Font);
                    }
                    
                    if (pageEvent != null && !isSectionTitle)
                        pageEvent.OnParagraphEnd(writer, this, IndentTop - currentHeight);
                    
                    alignment = Element.ALIGN_LEFT;
                    indentation.indentLeft -= paragraph.IndentationLeft;
                    indentation.indentRight -= paragraph.IndentationRight;
                    CarriageReturn();
                    leadingCount--;
                    break;
                }
                case Element.SECTION:
                case Element.CHAPTER: {
                    // Chapters and Sections only differ in their constructor
                    // so we cast both to a Section
                    Section section = (Section) element;
                    IPdfPageEvent pageEvent = writer.PageEvent;
                    
                    bool hasTitle = section.NotAddedYet && section.Title != null;
                    
                    // if the section is a chapter, we begin a new page
                    if (section.TriggerNewPage) {
                        NewPage();
                    }

                    if (hasTitle) {
                        float fith = IndentTop - currentHeight;
                        int rotation = pageSize.Rotation;
                        if (rotation == 90 || rotation == 180)
                            fith = pageSize.Height - fith;
                        PdfDestination destination = new PdfDestination(PdfDestination.FITH, fith);
                        while (currentOutline.Level >= section.Depth) {
                            currentOutline = currentOutline.Parent;
                        }
                        PdfOutline outline = new PdfOutline(currentOutline, destination, section.GetBookmarkTitle(), section.BookmarkOpen);
                        currentOutline = outline;
                    }
                    
                    // some values are set
                    CarriageReturn();
                    indentation.sectionIndentLeft += section.IndentationLeft;
                    indentation.sectionIndentRight += section.IndentationRight;                    
                    if (section.NotAddedYet && pageEvent != null)
                        if (element.Type == Element.CHAPTER)
                            pageEvent.OnChapter(writer, this, IndentTop - currentHeight, section.Title);
                        else
                            pageEvent.OnSection(writer, this, IndentTop - currentHeight, section.Depth, section.Title);
                    
                    // the title of the section (if any has to be printed)
                    if (hasTitle) {
                        isSectionTitle = true;
                        Add(section.Title);
                        isSectionTitle = false;
                    }
                    indentation.sectionIndentLeft += section.Indentation;
                    // we process the section
                    element.Process(this);
                    // some parameters are set back to normal again
                    indentation.sectionIndentLeft -= (section.IndentationLeft + section.Indentation);
                    indentation.sectionIndentRight -= section.IndentationRight;
                    
                    if (section.ElementComplete && pageEvent != null)
                        if (element.Type == Element.CHAPTER)
                            pageEvent.OnChapterEnd(writer, this, IndentTop - currentHeight);
                        else
                            pageEvent.OnSectionEnd(writer, this, IndentTop - currentHeight);
                    
                    break;
                }
                case Element.LIST: {
                    // we cast the element to a List
                    List list = (List) element;
                    if (list.Alignindent) {
                        list.NormalizeIndentation();
                    }
                    // we adjust the document
                    indentation.listIndentLeft += list.IndentationLeft;
                    indentation.indentRight += list.IndentationRight;
                    // we process the items in the list
                    element.Process(this);
                    // some parameters are set back to normal again
                    indentation.listIndentLeft -= list.IndentationLeft;
                    indentation.indentRight -= list.IndentationRight;
                    CarriageReturn();
                    break;
                }
                case Element.LISTITEM: {
                    leadingCount++;
                    // we cast the element to a ListItem
                    ListItem listItem = (ListItem) element;
                    
                    AddSpacing(listItem.SpacingBefore, leading, listItem.Font);
                    
                    // we adjust the document
                    alignment = listItem.Alignment;
                    indentation.listIndentLeft += listItem.IndentationLeft;
                    indentation.indentRight += listItem.IndentationRight;
                    leading = listItem.TotalLeading;
                    CarriageReturn();
                    // we prepare the current line to be able to show us the listsymbol
                    line.ListItem = listItem;
                    // we process the item
                    element.Process(this);

                    AddSpacing(listItem.SpacingAfter, listItem.TotalLeading, listItem.Font);
                    
                    // if the last line is justified, it should be aligned to the left
                    if (line.HasToBeJustified()) {
                        line.ResetAlignment();
                    }
                    // some parameters are set back to normal again
                    CarriageReturn();
                    indentation.listIndentLeft -= listItem.IndentationLeft;
                    indentation.indentRight -= listItem.IndentationRight;
                    leadingCount--;
                    break;
                }
                case Element.RECTANGLE: {
                    Rectangle rectangle = (Rectangle) element;
                    graphics.Rectangle(rectangle);
                    pageEmpty = false;
                    break;
                }
                case Element.PTABLE: {
                    PdfPTable ptable = (PdfPTable)element;
                    if (ptable.Size <= ptable.HeaderRows)
                        break; //nothing to do

                    // before every table, we add a new line and flush all lines
                    EnsureNewLine();
                    FlushLines();
                    
                    AddPTable(ptable);
                    pageEmpty = false;
                    NewLine();
                    break;
                }
                case Element.MULTI_COLUMN_TEXT: {
                    EnsureNewLine();
                    FlushLines();
                    MultiColumnText multiText = (MultiColumnText) element;
                    float height = multiText.Write(writer.DirectContent, this, IndentTop - currentHeight);
                    currentHeight += height;
                    text.MoveText(0, -1f* height);
                    pageEmpty = false;
                    break;
                }
                case Element.TABLE : {
                    if (element is SimpleTable) {
                        PdfPTable ptable = ((SimpleTable)element).CreatePdfPTable();
                        if (ptable.Size <= ptable.HeaderRows)
                            break; //nothing to do
                    
                        // before every table, we add a new line and flush all lines
                        EnsureNewLine();
                        FlushLines();
                        AddPTable(ptable);                    
                        pageEmpty = false;
                        break;
                    } else if (element is Table) {

                        try {
                            PdfPTable ptable = ((Table)element).CreatePdfPTable();
                            if (ptable.Size <= ptable.HeaderRows)
                                break; //nothing to do
                            
                            // before every table, we add a new line and flush all lines
                            EnsureNewLine();
                            FlushLines();
                            AddPTable(ptable);                    
                            pageEmpty = false;
                            break;
                        }
                        catch (BadElementException) {
                            // constructing the PdfTable
                            // Before the table, add a blank line using offset or default leading
                            float offset = ((Table)element).Offset;
                            if (float.IsNaN(offset))
                                offset = leading;
                            CarriageReturn();
                            lines.Add(new PdfLine(IndentLeft, IndentRight, alignment, offset));
                            currentHeight += offset;
                            AddPdfTable((Table)element);
                        }
                    } else {
                        return false;
                    }
                    break;
                }
                case Element.JPEG:
                case Element.JPEG2000:
                case Element.IMGRAW:
                case Element.IMGTEMPLATE: {
                    //carriageReturn(); suggestion by Marc Campforts
                    Add((Image) element);
                    break;
                }
                case Element.YMARK: {
                    IDrawInterface zh = (IDrawInterface)element;
                    zh.Draw(graphics, IndentLeft, IndentBottom, IndentRight, IndentTop, IndentTop - currentHeight - (leadingCount > 0 ? leading : 0));
                    pageEmpty = false;
                    break;
                }
                case Element.MARKED: {
                    MarkedObject mo;
                    if (element is MarkedSection) {
                        mo = ((MarkedSection)element).Title;
                        if (mo != null) {
                            mo.Process(this);
                        }
                    }
                    mo = (MarkedObject)element;
                    mo.Process(this);
                    break;
                }
                default:
                    return false;
            }
            lastElementType = element.Type;
            return true;
        }
        
    //  [L1] DocListener interface
        
        /**
        * Opens the document.
        * <P>
        * You have to open the document before you can begin to add content
        * to the body of the document.
        */
        public override void Open() {
            if (!open) {
                base.Open();
                writer.Open();
                rootOutline = new PdfOutline(writer);
                currentOutline = rootOutline;
            }
            InitPage();
        }
        
    //  [L2] DocListener interface
    
        /**
        * Closes the document.
        * <B>
        * Once all the content has been written in the body, you have to close
        * the body. After that nothing can be written to the body anymore.
        */        
        public override void Close() {
            if (close) {
                return;
            }
            bool wasImage = (imageWait != null);
            NewPage();
            if (imageWait != null || wasImage) NewPage();
            if (annotationsImp.HasUnusedAnnotations())
                throw new Exception("Not all annotations could be added to the document (the document doesn't have enough pages).");
            IPdfPageEvent pageEvent = writer.PageEvent;
            if (pageEvent != null)
                pageEvent.OnCloseDocument(writer, this);
            base.Close();
            
            writer.AddLocalDestinations(localDestinations);
            CalculateOutlineCount();
            WriteOutlines();
            
            writer.Close();
        }
    
    //  [L3] DocListener interface

        protected internal int textEmptySize;

        // [C9] Metadata for the page
        /** XMP Metadata for the page. */
        protected byte[] xmpMetadata = null;
        /**
        * Use this method to set the XMP Metadata.
        * @param xmpMetadata The xmpMetadata to set.
        */
        public byte[] XmpMetadata {
            set {
                xmpMetadata = value;
            }
        }
        
        /**
        * Makes a new page and sends it to the <CODE>PdfWriter</CODE>.
        *
        * @return a <CODE>bool</CODE>
        * @throws DocumentException on error
        */
        public override bool NewPage() {
            lastElementType = -1;
            if (writer == null || (writer.DirectContent.Size == 0 && writer.DirectContentUnder.Size == 0 && (pageEmpty || writer.IsPaused()))) {
                SetNewPageSizeAndMargins();
                return false;
            }
            if (!open || close) {
                throw new Exception("The document isn't open.");
            }
            IPdfPageEvent pageEvent = writer.PageEvent;
            if (pageEvent != null)
                pageEvent.OnEndPage(writer, this);
            
            //Added to inform any listeners that we are moving to a new page (added by David Freels)
            base.NewPage();
            
            // the following 2 lines were added by Pelikan Stephan
            indentation.imageIndentLeft = 0;
            indentation.imageIndentRight = 0;
            
            // we flush the arraylist with recently written lines
            FlushLines();
            // we prepare the elements of the page dictionary
            
            // [U1] page size and rotation
            int rotation = pageSize.Rotation;
            
            // [C10]
            if (writer.IsPdfX()) {
                if (thisBoxSize.ContainsKey("art") && thisBoxSize.ContainsKey("trim"))
                    throw new PdfXConformanceException("Only one of ArtBox or TrimBox can exist in the page.");
                if (!thisBoxSize.ContainsKey("art") && !thisBoxSize.ContainsKey("trim")) {
                    if (thisBoxSize.ContainsKey("crop"))
                        thisBoxSize["trim"] = thisBoxSize["crop"];
                    else
                        thisBoxSize["trim"] = new PdfRectangle(pageSize, pageSize.Rotation);
                }
            }
            
            // [M1]
            pageResources.AddDefaultColorDiff(writer.DefaultColorspace);        
            if (writer.RgbTransparencyBlending) {
                PdfDictionary dcs = new PdfDictionary();
                dcs.Put(PdfName.CS, PdfName.DEVICERGB);
                pageResources.AddDefaultColorDiff(dcs);
            }
            PdfDictionary resources = pageResources.Resources;
            
            // we create the page dictionary
            
            PdfPage page = new PdfPage(new PdfRectangle(pageSize, rotation), thisBoxSize, resources, rotation);

            // we complete the page dictionary
            
            // [C9] if there is XMP data to add: add it
            if (xmpMetadata != null) {
                PdfStream xmp = new PdfStream(xmpMetadata);
                xmp.Put(PdfName.TYPE, PdfName.METADATA);
                xmp.Put(PdfName.SUBTYPE, PdfName.XML);
                PdfEncryption crypto = writer.Encryption;
                if (crypto != null && !crypto.IsMetadataEncrypted()) {
                    PdfArray ar = new PdfArray();
                    ar.Add(PdfName.CRYPT);
                    xmp.Put(PdfName.FILTER, ar);
                }
                page.Put(PdfName.METADATA, writer.AddToBody(xmp).IndirectReference);
            }
            
            // [U3] page actions: transition, duration, additional actions
            if (this.transition!=null) {
                page.Put(PdfName.TRANS, this.transition.TransitionDictionary);
                transition = null;
            }
            if (this.duration>0) {
                page.Put(PdfName.DUR,new PdfNumber(this.duration));
                duration = 0;
            }
            if (pageAA != null) {
                page.Put(PdfName.AA, writer.AddToBody(pageAA).IndirectReference);
                pageAA = null;
            }
            
            // [U4] we add the thumbs
            if (thumb != null) {
                page.Put(PdfName.THUMB, thumb);
                thumb = null;
            }
            
            // [U8] we check if the userunit is defined
            if (writer.Userunit > 0f) {
                page.Put(PdfName.USERUNIT, new PdfNumber(writer.Userunit));
            }
            
            // [C5] and [C8] we add the annotations
            if (annotationsImp.HasUnusedAnnotations()) {
                PdfArray array = annotationsImp.RotateAnnotations(writer, pageSize);
                if (array.Size != 0)
                    page.Put(PdfName.ANNOTS, array);
            }
            
            // [F12] we add tag info
            if (writer.IsTagged())
                 page.Put(PdfName.STRUCTPARENTS, new PdfNumber(writer.CurrentPageNumber - 1));
            
            if (text.Size > textEmptySize)
                text.EndText();
            else
                text = null;
            writer.Add(page, new PdfContents(writer.DirectContentUnder, graphics, text, writer.DirectContent, pageSize));
            // we initialize the new page
            InitPage();
            return true;
        }

    //  [L4] DocListener interface

        /**
        * Sets the pagesize.
        *
        * @param pageSize the new pagesize
        * @return <CODE>true</CODE> if the page size was set
        */
        public override bool SetPageSize(Rectangle pageSize) {
            if (writer != null && writer.IsPaused()) {
                return false;
            }
            nextPageSize = new Rectangle(pageSize);
            return true;
        }
        

        /** margin in x direction starting from the left. Will be valid in the next page */
        protected float nextMarginLeft;
        
        /** margin in x direction starting from the right. Will be valid in the next page */
        protected float nextMarginRight;
        
        /** margin in y direction starting from the top. Will be valid in the next page */
        protected float nextMarginTop;
        
        /** margin in y direction starting from the bottom. Will be valid in the next page */
        protected float nextMarginBottom;

        /**
        * Sets the margins.
        *
        * @param    marginLeft      the margin on the left
        * @param    marginRight     the margin on the right
        * @param    marginTop       the margin on the top
        * @param    marginBottom    the margin on the bottom
        * @return   a <CODE>bool</CODE>
        */
        public override bool SetMargins(float marginLeft, float marginRight, float marginTop, float marginBottom) {
            if (writer != null && writer.IsPaused()) {
                return false;
            }
            nextMarginLeft = marginLeft;
            nextMarginRight = marginRight;
            nextMarginTop = marginTop;
            nextMarginBottom = marginBottom;
            return true;
        }
        
    //  [L6] DocListener interface
        
        /**
        * @see com.lowagie.text.DocListener#setMarginMirroring(bool)
        */
        public override bool SetMarginMirroring(bool MarginMirroring) {
            if (writer != null && writer.IsPaused()) {
                return false;
            }
            return base.SetMarginMirroring(MarginMirroring);
        }

    //  [L7] DocListener interface
        
        /**
        * Sets the page number.
        *
        * @param    pageN       the new page number
        */
        public override int PageCount {
            set {
                if (writer != null && writer.IsPaused()) {
                    return;
                }
                base.PageCount = value;
            }
        }
        
    //  [L8] DocListener interface
        
        /**
        * Sets the page number to 0.
        */
        public override void ResetPageCount() {
            if (writer != null && writer.IsPaused()) {
                return;
            }
            base.ResetPageCount();
        }

        /**
        * Changes the header of this document.
        *
        * @param header the new header
        */        
        public override HeaderFooter Header {
            set {
                if (writer != null && writer.IsPaused()) {
                    return;
                }
                base.Header = value;
            }
        }
        
        /**
        * Resets the header of this document.
        */        
        public override void ResetHeader() {
            if (writer != null && writer.IsPaused()) {
                return;
            }
            base.ResetHeader();
        }
        
        /**
        * Changes the footer of this document.
        *
        * @param    footer      the new footer
        */        
        public override HeaderFooter Footer {
            set {
                if (writer != null && writer.IsPaused()) {
                    return;
                }
                base.Footer = value;
            }
        }
        
        /**
        * Resets the footer of this document.
        */        
        public override void ResetFooter() {
            if (writer != null && writer.IsPaused()) {
                return;
            }
            base.ResetFooter();
        }
        
    // DOCLISTENER METHODS END
    
        /** Signals that OnOpenDocument should be called. */
        protected internal bool firstPageEvent = true;
    
        /**
        * Initializes a page.
        * <P>
        * If the footer/header is set, it is printed.
        * @throws DocumentException on error
        */
        
        protected internal void InitPage() {
            // the pagenumber is incremented
            pageN++;
            
            // initialisation of some page objects
            annotationsImp.ResetAnnotations();
            pageResources = new PageResources();

            writer.ResetContent();
            graphics = new PdfContentByte(writer);
            text = new PdfContentByte(writer);
            text.Reset();
            text.BeginText();
            textEmptySize = text.Size;

            markPoint = 0;
            SetNewPageSizeAndMargins();
            imageEnd = -1;
            indentation.imageIndentRight = 0;
            indentation.imageIndentLeft = 0;
            indentation.indentBottom = 0;
            indentation.indentTop = 0;
            currentHeight = 0;
            
            // backgroundcolors, etc...
            thisBoxSize = new Hashtable(boxSize);
            if (pageSize.BackgroundColor != null
            || pageSize.HasBorders()
            || pageSize.BorderColor != null) {
                Add(pageSize);
            }

            float oldleading = leading;
            int oldAlignment = alignment;
            // if there is a footer, the footer is added
            DoFooter();
            // we move to the left/top position of the page
            text.MoveText(Left, Top);
            DoHeader();
            pageEmpty = true;
            // if there is an image waiting to be drawn, draw it
            if (imageWait != null) {
                Add(imageWait);
                imageWait = null;
            }
            leading = oldleading;
            alignment = oldAlignment;
            CarriageReturn();
            
            IPdfPageEvent pageEvent = writer.PageEvent;
            if (pageEvent != null) {
                if (firstPageEvent) {
                    pageEvent.OnOpenDocument(writer, this);
                }
                pageEvent.OnStartPage(writer, this);
            }
            firstPageEvent = false;
        }

        /** The line that is currently being written. */
        protected internal PdfLine line = null;
        
        /** The lines that are written until now. */
        protected internal ArrayList lines = new ArrayList();
        
        /**
        * Adds the current line to the list of lines and also adds an empty line.
        * @throws DocumentException on error
        */
        
        protected internal void NewLine() {
            lastElementType = -1;
            CarriageReturn();
            if (lines != null && lines.Count > 0) {
                lines.Add(line);
                currentHeight += line.Height;
            }
            line = new PdfLine(IndentLeft, IndentRight, alignment, leading);
        }
        
        /**
        * If the current line is not empty or null, it is added to the arraylist
        * of lines and a new empty line is added.
        * @throws DocumentException on error
        */
        
        protected internal void CarriageReturn() {
            // the arraylist with lines may not be null
            if (lines == null) {
                lines = new ArrayList();
            }
            // If the current line is not null
            if (line != null) {
                // we check if the end of the page is reached (bugfix by Francois Gravel)
                if (currentHeight + line.Height + leading < IndentTop - IndentBottom) {
                    // if so nonempty lines are added and the heigt is augmented
                    if (line.Size > 0) {
                        currentHeight += line.Height;
                        lines.Add(line);
                        pageEmpty = false;
                    }
                }
                // if the end of the line is reached, we start a new page
                else {
                    NewPage();
                }
            }
            if (imageEnd > -1 && currentHeight > imageEnd) {
                imageEnd = -1;
                indentation.imageIndentRight = 0;
                indentation.imageIndentLeft = 0;
            }
            // a new current line is constructed
            line = new PdfLine(IndentLeft, IndentRight, alignment, leading);
        }
        
        /**
        * Gets the current vertical page position.
        * @param ensureNewLine Tells whether a new line shall be enforced. This may cause side effects 
        *   for elements that do not terminate the lines they've started because those lines will get
        *   terminated. 
        * @return The current vertical page position.
        */
        public float GetVerticalPosition(bool ensureNewLine) {
            // ensuring that a new line has been started.
            if (ensureNewLine) {
                EnsureNewLine();
            }
            return Top - currentHeight - indentation.indentTop;
        }

        /** Holds the type of the last element, that has been added to the document. */
        protected internal int lastElementType = -1;    

        /**
        * Ensures that a new line has been started. 
        */
        protected internal void EnsureNewLine() {
            if ((lastElementType == Element.PHRASE) || 
                (lastElementType == Element.CHUNK)) {
                NewLine();
                FlushLines();
            }
        }
        
        /**
        * Writes all the lines to the text-object.
        *
        * @return the displacement that was caused
        * @throws DocumentException on error
        */
        protected internal float FlushLines() {
            // checks if the ArrayList with the lines is not null
            if (lines == null) {
                return 0;
            }
            // checks if a new Line has to be made.
            if (line != null && line.Size > 0) {
                lines.Add(line);
                line = new PdfLine(IndentLeft, IndentRight, alignment, leading);
            }
            
            // checks if the ArrayList with the lines is empty
            if (lines.Count == 0) {
                return 0;
            }
            
            // initialisation of some parameters
            Object[] currentValues = new Object[2];
            PdfFont currentFont = null;
            float displacement = 0;

            currentValues[1] = (float)0;
            // looping over all the lines
            foreach (PdfLine l in lines) {
                
                // this is a line in the loop
                
                float moveTextX = l.IndentLeft - IndentLeft + indentation.indentLeft + indentation.listIndentLeft + indentation.sectionIndentLeft;
                text.MoveText(moveTextX, -l.Height);
                // is the line preceeded by a symbol?
                if (l.ListSymbol != null) {
                    ColumnText.ShowTextAligned(graphics, Element.ALIGN_LEFT, new Phrase(l.ListSymbol), text.XTLM - l.ListIndent, text.YTLM, 0);
                }
                
                currentValues[0] = currentFont;
                
                WriteLineToContent(l, text, graphics, currentValues, writer.SpaceCharRatio);
                
                currentFont = (PdfFont)currentValues[0];
                
                displacement += l.Height;
                text.MoveText(-moveTextX, 0);
            }
            lines = new ArrayList();
            return displacement;
        }

        /** The characters to be applied the hanging punctuation. */
        internal const String hangingPunctuation = ".,;:'";
        
        /**
        * Writes a text line to the document. It takes care of all the attributes.
        * <P>
        * Before entering the line position must have been established and the
        * <CODE>text</CODE> argument must be in text object scope (<CODE>beginText()</CODE>).
        * @param line the line to be written
        * @param text the <CODE>PdfContentByte</CODE> where the text will be written to
        * @param graphics the <CODE>PdfContentByte</CODE> where the graphics will be written to
        * @param currentValues the current font and extra spacing values
        * @param ratio
        * @throws DocumentException on error
        */
        internal void WriteLineToContent(PdfLine line, PdfContentByte text, PdfContentByte graphics, Object[] currentValues, float ratio)  {
            PdfFont currentFont = (PdfFont)(currentValues[0]);
            float lastBaseFactor = (float)currentValues[1];
            //PdfChunk chunkz;
            int numberOfSpaces;
            int lineLen;
            bool isJustified;
            float hangingCorrection = 0;
            float hScale = 1;
            float lastHScale = float.NaN;
            float baseWordSpacing = 0;
            float baseCharacterSpacing = 0;
            float glueWidth = 0;
            
            numberOfSpaces = line.NumberOfSpaces;
            lineLen = line.GetLineLengthUtf32();
            // does the line need to be justified?
            isJustified = line.HasToBeJustified() && (numberOfSpaces != 0 || lineLen > 1);
            int separatorCount = line.GetSeparatorCount();
            if (separatorCount > 0) {
                glueWidth = line.WidthLeft / separatorCount;
            }
            else if (isJustified) {
                if (line.NewlineSplit && line.WidthLeft >= (lastBaseFactor * (ratio * numberOfSpaces + lineLen - 1))) {
                    if (line.RTL) {
                        text.MoveText(line.WidthLeft - lastBaseFactor * (ratio * numberOfSpaces + lineLen - 1), 0);
                    }
                    baseWordSpacing = ratio * lastBaseFactor;
                    baseCharacterSpacing = lastBaseFactor;
                }
                else {
                    float width = line.WidthLeft;
                    PdfChunk last = line.GetChunk(line.Size - 1);
                    if (last != null) {
                        String s = last.ToString();
                        char c;
                        if (s.Length > 0 && hangingPunctuation.IndexOf((c = s[s.Length - 1])) >= 0) {
                            float oldWidth = width;
                            width += last.Font.Width(c) * 0.4f;
                            hangingCorrection = width - oldWidth;
                        }
                    }
                    float baseFactor = width / (ratio * numberOfSpaces + lineLen - 1);
                    baseWordSpacing = ratio * baseFactor;
                    baseCharacterSpacing = baseFactor;
                    lastBaseFactor = baseFactor;
                }
            }
            
            int lastChunkStroke = line.LastStrokeChunk;
            int chunkStrokeIdx = 0;
            float xMarker = text.XTLM;
            float baseXMarker = xMarker;
            float yMarker = text.YTLM;
            bool adjustMatrix = false;
            float tabPosition = 0;
            
            // looping over all the chunks in 1 line
            foreach (PdfChunk chunk in line) {
                Color color = chunk.Color;
                hScale = 1;
                
                if (chunkStrokeIdx <= lastChunkStroke) {
                    float width;
                    if (isJustified) {
                        width = chunk.GetWidthCorrected(baseCharacterSpacing, baseWordSpacing);
                    }
                    else {
                        width = chunk.Width;
                    }
                    if (chunk.IsStroked()) {
                        PdfChunk nextChunk = line.GetChunk(chunkStrokeIdx + 1);
                        if (chunk.IsSeparator()) {
                            width = glueWidth;
                            Object[] sep = (Object[])chunk.GetAttribute(Chunk.SEPARATOR);
                            IDrawInterface di = (IDrawInterface)sep[0];
                            bool vertical = (bool)sep[1];
                            float fontSize = chunk.Font.Size;
                            float ascender = chunk.Font.Font.GetFontDescriptor(BaseFont.ASCENT, fontSize);
                            float descender = chunk.Font.Font.GetFontDescriptor(BaseFont.DESCENT, fontSize);
                            if (vertical) {
                                di.Draw(graphics, baseXMarker, yMarker + descender, baseXMarker + line.OriginalWidth, ascender - descender, yMarker);      
                            }
                            else {
                                di.Draw(graphics, xMarker, yMarker + descender, xMarker + width, ascender - descender, yMarker);
                            }
                        }
                        if (chunk.IsTab()) {
                            Object[] tab = (Object[])chunk.GetAttribute(Chunk.TAB);
                            IDrawInterface di = (IDrawInterface)tab[0];
                            tabPosition = (float)tab[1] + (float)tab[3];
                            float fontSize = chunk.Font.Size;
                            float ascender = chunk.Font.Font.GetFontDescriptor(BaseFont.ASCENT, fontSize);
                            float descender = chunk.Font.Font.GetFontDescriptor(BaseFont.DESCENT, fontSize);
                            if (tabPosition > xMarker) {
                                di.Draw(graphics, xMarker, yMarker + descender, tabPosition, ascender - descender, yMarker);
                            }
                            float tmp = xMarker;
                            xMarker = tabPosition;
                            tabPosition = tmp;
                        }
                        if (chunk.IsAttribute(Chunk.BACKGROUND)) {
                            float subtract = lastBaseFactor;
                            if (nextChunk != null && nextChunk.IsAttribute(Chunk.BACKGROUND))
                                subtract = 0;
                            if (nextChunk == null)
                                subtract += hangingCorrection;
                            float fontSize = chunk.Font.Size;
                            float ascender = chunk.Font.Font.GetFontDescriptor(BaseFont.ASCENT, fontSize);
                            float descender = chunk.Font.Font.GetFontDescriptor(BaseFont.DESCENT, fontSize);
                            Object[] bgr = (Object[])chunk.GetAttribute(Chunk.BACKGROUND);
                            graphics.SetColorFill((Color)bgr[0]);
                            float[] extra = (float[])bgr[1];
                            graphics.Rectangle(xMarker - extra[0],
                                yMarker + descender - extra[1] + chunk.TextRise,
                                width - subtract + extra[0] + extra[2],
                                ascender - descender + extra[1] + extra[3]);
                            graphics.Fill();
                            graphics.SetGrayFill(0);
                        }
                        if (chunk.IsAttribute(Chunk.UNDERLINE)) {
                            float subtract = lastBaseFactor;
                            if (nextChunk != null && nextChunk.IsAttribute(Chunk.UNDERLINE))
                                subtract = 0;
                            if (nextChunk == null)
                                subtract += hangingCorrection;
                            Object[][] unders = (Object[][])chunk.GetAttribute(Chunk.UNDERLINE);
                            Color scolor = null;
                            for (int k = 0; k < unders.Length; ++k) {
                                Object[] obj = unders[k];
                                scolor = (Color)obj[0];
                                float[] ps = (float[])obj[1];
                                if (scolor == null)
                                    scolor = color;
                                if (scolor != null)
                                    graphics.SetColorStroke(scolor);
                                float fsize = chunk.Font.Size;
                                graphics.SetLineWidth(ps[0] + fsize * ps[1]);
                                float shift = ps[2] + fsize * ps[3];
                                int cap2 = (int)ps[4];
                                if (cap2 != 0)
                                    graphics.SetLineCap(cap2);
                                graphics.MoveTo(xMarker, yMarker + shift);
                                graphics.LineTo(xMarker + width - subtract, yMarker + shift);
                                graphics.Stroke();
                                if (scolor != null)
                                    graphics.ResetGrayStroke();
                                if (cap2 != 0)
                                    graphics.SetLineCap(0);
                            }
                            graphics.SetLineWidth(1);
                        }
                        if (chunk.IsAttribute(Chunk.ACTION)) {
                            float subtract = lastBaseFactor;
                            if (nextChunk != null && nextChunk.IsAttribute(Chunk.ACTION))
                                subtract = 0;
                            if (nextChunk == null)
                                subtract += hangingCorrection;
                            text.AddAnnotation(new PdfAnnotation(writer, xMarker, yMarker, xMarker + width - subtract, yMarker + chunk.Font.Size, (PdfAction)chunk.GetAttribute(Chunk.ACTION)));
                        }
                        if (chunk.IsAttribute(Chunk.REMOTEGOTO)) {
                            float subtract = lastBaseFactor;
                            if (nextChunk != null && nextChunk.IsAttribute(Chunk.REMOTEGOTO))
                                subtract = 0;
                            if (nextChunk == null)
                                subtract += hangingCorrection;
                            Object[] obj = (Object[])chunk.GetAttribute(Chunk.REMOTEGOTO);
                            String filename = (String)obj[0];
                            if (obj[1] is String)
                                RemoteGoto(filename, (String)obj[1], xMarker, yMarker, xMarker + width - subtract, yMarker + chunk.Font.Size);
                            else
                                RemoteGoto(filename, (int)obj[1], xMarker, yMarker, xMarker + width - subtract, yMarker + chunk.Font.Size);
                        }
                        if (chunk.IsAttribute(Chunk.LOCALGOTO)) {
                            float subtract = lastBaseFactor;
                            if (nextChunk != null && nextChunk.IsAttribute(Chunk.LOCALGOTO))
                                subtract = 0;
                            if (nextChunk == null)
                                subtract += hangingCorrection;
                            LocalGoto((String)chunk.GetAttribute(Chunk.LOCALGOTO), xMarker, yMarker, xMarker + width - subtract, yMarker + chunk.Font.Size);
                        }
                        if (chunk.IsAttribute(Chunk.LOCALDESTINATION)) {
                            float subtract = lastBaseFactor;
                            if (nextChunk != null && nextChunk.IsAttribute(Chunk.LOCALDESTINATION))
                                subtract = 0;
                            if (nextChunk == null)
                                subtract += hangingCorrection;
                            LocalDestination((String)chunk.GetAttribute(Chunk.LOCALDESTINATION), new PdfDestination(PdfDestination.XYZ, xMarker, yMarker + chunk.Font.Size, 0));
                        }
                        if (chunk.IsAttribute(Chunk.GENERICTAG)) {
                            float subtract = lastBaseFactor;
                            if (nextChunk != null && nextChunk.IsAttribute(Chunk.GENERICTAG))
                                subtract = 0;
                            if (nextChunk == null)
                                subtract += hangingCorrection;
                            Rectangle rect = new Rectangle(xMarker, yMarker, xMarker + width - subtract, yMarker + chunk.Font.Size);
                            IPdfPageEvent pev = writer.PageEvent;
                            if (pev != null)
                                pev.OnGenericTag(writer, this, rect, (String)chunk.GetAttribute(Chunk.GENERICTAG));
                        }
                        if (chunk.IsAttribute(Chunk.PDFANNOTATION)) {
                            float subtract = lastBaseFactor;
                            if (nextChunk != null && nextChunk.IsAttribute(Chunk.PDFANNOTATION))
                                subtract = 0;
                            if (nextChunk == null)
                                subtract += hangingCorrection;
                            float fontSize = chunk.Font.Size;
                            float ascender = chunk.Font.Font.GetFontDescriptor(BaseFont.ASCENT, fontSize);
                            float descender = chunk.Font.Font.GetFontDescriptor(BaseFont.DESCENT, fontSize);
                            PdfAnnotation annot = PdfFormField.ShallowDuplicate((PdfAnnotation)chunk.GetAttribute(Chunk.PDFANNOTATION));
                            annot.Put(PdfName.RECT, new PdfRectangle(xMarker, yMarker + descender, xMarker + width - subtract, yMarker + ascender));
                            text.AddAnnotation(annot);
                        }
                        float[] paramsx = (float[])chunk.GetAttribute(Chunk.SKEW);
                        object hs = chunk.GetAttribute(Chunk.HSCALE);
                        if (paramsx != null || hs != null) {
                            float b = 0, c = 0;
                            if (paramsx != null) {
                                b = paramsx[0];
                                c = paramsx[1];
                            }
                            if (hs != null)
                                hScale = (float)hs;
                            text.SetTextMatrix(hScale, b, c, 1, xMarker, yMarker);
                        }
                        if (chunk.IsImage()) {
                            Image image = chunk.Image;
                            float[] matrix = image.Matrix;
                            matrix[Image.CX] = xMarker + chunk.ImageOffsetX - matrix[Image.CX];
                            matrix[Image.CY] = yMarker + chunk.ImageOffsetY - matrix[Image.CY];
                            graphics.AddImage(image, matrix[0], matrix[1], matrix[2], matrix[3], matrix[4], matrix[5]);
                            text.MoveText(xMarker + lastBaseFactor + image.ScaledWidth - text.XTLM, 0);
                        }
                    }
                    xMarker += width;
                    ++chunkStrokeIdx;
                }

                if (chunk.Font.CompareTo(currentFont) != 0) {
                    currentFont = chunk.Font;
                    text.SetFontAndSize(currentFont.Font, currentFont.Size);
                }
                float rise = 0;
                Object[] textRender = (Object[])chunk.GetAttribute(Chunk.TEXTRENDERMODE);
                int tr = 0;
                float strokeWidth = 1;
                Color strokeColor = null;
                object fr = chunk.GetAttribute(Chunk.SUBSUPSCRIPT);
                if (textRender != null) {
                    tr = (int)textRender[0] & 3;
                    if (tr != PdfContentByte.TEXT_RENDER_MODE_FILL)
                        text.SetTextRenderingMode(tr);
                    if (tr == PdfContentByte.TEXT_RENDER_MODE_STROKE || tr == PdfContentByte.TEXT_RENDER_MODE_FILL_STROKE) {
                        strokeWidth = (float)textRender[1];
                        if (strokeWidth != 1)
                            text.SetLineWidth(strokeWidth);
                        strokeColor = (Color)textRender[2];
                        if (strokeColor == null)
                            strokeColor = color;
                        if (strokeColor != null)
                            text.SetColorStroke(strokeColor);
                    }
                }
                if (fr != null)
                    rise = (float)fr;
                if (color != null)
                    text.SetColorFill(color);
                if (rise != 0)
                    text.SetTextRise(rise);
                if (chunk.IsImage()) {
                    adjustMatrix = true;
                }
                else if (chunk.IsHorizontalSeparator()) {
                    PdfTextArray array = new PdfTextArray();
                    array.Add(-glueWidth * 1000f / chunk.Font.Size / hScale);
                    text.ShowText(array);
                }
                else if (chunk.IsTab()) {
                    PdfTextArray array = new PdfTextArray();
                    array.Add((tabPosition - xMarker) * 1000f / chunk.Font.Size / hScale);
                    text.ShowText(array);
                }
                // If it is a CJK chunk or Unicode TTF we will have to simulate the
                // space adjustment.
                else if (isJustified && numberOfSpaces > 0 && chunk.IsSpecialEncoding()) {
                    if (hScale != lastHScale) {
                        lastHScale = hScale;
                        text.SetWordSpacing(baseWordSpacing / hScale);
                        text.SetCharacterSpacing(baseCharacterSpacing / hScale);
                    }
                    String s = chunk.ToString();
                    int idx = s.IndexOf(' ');
                    if (idx < 0)
                        text.ShowText(s);
                    else {
                        float spaceCorrection = - baseWordSpacing * 1000f / chunk.Font.Size / hScale;
                        PdfTextArray textArray = new PdfTextArray(s.Substring(0, idx));
                        int lastIdx = idx;
                        while ((idx = s.IndexOf(' ', lastIdx + 1)) >= 0) {
                            textArray.Add(spaceCorrection);
                            textArray.Add(s.Substring(lastIdx, idx - lastIdx));
                            lastIdx = idx;
                        }
                        textArray.Add(spaceCorrection);
                        textArray.Add(s.Substring(lastIdx));
                        text.ShowText(textArray);
                    }
                }
                else {
                    if (isJustified && hScale != lastHScale) {
                        lastHScale = hScale;
                        text.SetWordSpacing(baseWordSpacing / hScale);
                        text.SetCharacterSpacing(baseCharacterSpacing / hScale);
                    }
                    text.ShowText(chunk.ToString());
                }
                
                if (rise != 0)
                    text.SetTextRise(0);
                if (color != null)
                    text.ResetRGBColorFill();
                if (tr != PdfContentByte.TEXT_RENDER_MODE_FILL)
                    text.SetTextRenderingMode(PdfContentByte.TEXT_RENDER_MODE_FILL);
                if (strokeColor != null)
                    text.ResetRGBColorStroke();
                if (strokeWidth != 1)
                    text.SetLineWidth(1);            
                if (chunk.IsAttribute(Chunk.SKEW) || chunk.IsAttribute(Chunk.HSCALE)) {
                    adjustMatrix = true;
                    text.SetTextMatrix(xMarker, yMarker);
                }
            }
            if (isJustified) {
                text.SetWordSpacing(0);
                text.SetCharacterSpacing(0);
                if (line.NewlineSplit)
                    lastBaseFactor = 0;
            }
            if (adjustMatrix)
                text.MoveText(baseXMarker - text.XTLM, 0);
            currentValues[0] = currentFont;
            currentValues[1] = lastBaseFactor;
        }
        
        protected internal Indentation indentation = new Indentation();
        public class Indentation {
            /** This represents the current indentation of the PDF Elements on the left side. */
            internal float indentLeft = 0;
            
            /** Indentation to the left caused by a section. */
            internal float sectionIndentLeft = 0;
            
            /** This represents the current indentation of the PDF Elements on the left side. */
            internal float listIndentLeft = 0;
            
            /** This is the indentation caused by an image on the left. */
            internal float imageIndentLeft = 0;
            
            /** This represents the current indentation of the PDF Elements on the right side. */
            internal float indentRight = 0;
            
            /** Indentation to the right caused by a section. */
            internal float sectionIndentRight = 0;
            
            /** This is the indentation caused by an image on the right. */
            internal float imageIndentRight = 0;
            
            /** This represents the current indentation of the PDF Elements on the top side. */
            internal float indentTop = 0;
            
            /** This represents the current indentation of the PDF Elements on the bottom side. */
            internal float indentBottom = 0;
        }
        
        /**
        * Gets the indentation on the left side.
        *
        * @return   a margin
        */
        
        protected internal float IndentLeft {
            get {
                return GetLeft(indentation.indentLeft + indentation.listIndentLeft + indentation.imageIndentLeft + indentation.sectionIndentLeft);
            }
        }
        
        /**
        * Gets the indentation on the right side.
        *
        * @return   a margin
        */
        
        protected internal float IndentRight {
            get {
                return GetRight(indentation.indentRight + indentation.sectionIndentRight + indentation.imageIndentRight);
            }
        }
        
        /**
        * Gets the indentation on the top side.
        *
        * @return   a margin
        */
        
        protected internal float IndentTop {
            get {
                return GetTop(indentation.indentTop);
            }
        }
        
        /**
        * Gets the indentation on the bottom side.
        *
        * @return   a margin
        */
        
        protected internal float IndentBottom {
            get {
                return GetBottom(indentation.indentBottom);
            }
        }
        
        /**
        * Adds extra space.
        * This method should probably be rewritten.
        */
        protected internal void AddSpacing(float extraspace, float oldleading, Font f) {
            if (extraspace == 0) return;
            if (pageEmpty) return;
            if (currentHeight + line.Height + leading > IndentTop - IndentBottom) return;
            leading = extraspace;
            CarriageReturn();
            if (f.IsUnderlined() || f.IsStrikethru()) {
                f = new Font(f);
                int style = f.Style;
                style &= ~Font.UNDERLINE;
                style &= ~Font.STRIKETHRU;
                f.SetStyle(Font.UNDEFINED);
                f.SetStyle(style);
            }
            Chunk space = new Chunk(" ", f);
            space.Process(this);
            CarriageReturn();
            leading = oldleading;
        }
        
    //  Info Dictionary and Catalog

        /** some meta information about the Document. */
        protected internal PdfInfo info = new PdfInfo();

        /**
        * Gets the <CODE>PdfInfo</CODE>-object.
        *
        * @return   <CODE>PdfInfo</COPE>
        */
        internal PdfInfo Info {
            get {
                return info;
            }
        }
        
        /**
        * Gets the <CODE>PdfCatalog</CODE>-object.
        *
        * @param pages an indirect reference to this document pages
        * @return <CODE>PdfCatalog</CODE>
        */
        internal PdfCatalog GetCatalog(PdfIndirectReference pages) {
            PdfCatalog catalog = new PdfCatalog(pages, writer);
            
            // [C1] outlines
            if (rootOutline.Kids.Count > 0) {
                catalog.Put(PdfName.PAGEMODE, PdfName.USEOUTLINES);
                catalog.Put(PdfName.OUTLINES, rootOutline.IndirectReference);
            }
            
            // [C2] version
            writer.GetPdfVersion().AddToCatalog(catalog);
            
            // [C3] preferences
            viewerPreferences.AddToCatalog(catalog);
            
            // [C4] pagelabels
            if (pageLabels != null) {
                catalog.Put(PdfName.PAGELABELS, pageLabels.GetDictionary(writer));
            }
            
            // [C5] named objects
            catalog.AddNames(localDestinations, GetDocumentLevelJS(), documentFileAttachment, writer);
            
            // [C6] actions
            if (openActionName != null) {
                PdfAction action = GetLocalGotoAction(openActionName);
                catalog.OpenAction = action;
            }
            else if (openActionAction != null)
                catalog.OpenAction = openActionAction;
            if (additionalActions != null)   {
                catalog.AdditionalActions = additionalActions;
            }
            
            // [C7] portable collections
            if (collection != null) {
                catalog.Put(PdfName.COLLECTION, collection);
            }

            // [C8] AcroForm
            if (annotationsImp.HasValidAcroForm()) {
                catalog.Put(PdfName.ACROFORM, writer.AddToBody(annotationsImp.AcroForm).IndirectReference);
            }
            
            return catalog;
        }

    //  [C1] outlines

        /** This is the root outline of the document. */
        protected internal PdfOutline rootOutline;
        
        /** This is the current <CODE>PdfOutline</CODE> in the hierarchy of outlines. */
        protected internal PdfOutline currentOutline;
        
        /**
        * Adds a named outline to the document .
        * @param outline the outline to be added
        * @param name the name of this local destination
        */
        internal void AddOutline(PdfOutline outline, String name) {
            LocalDestination(name, outline.PdfDestination);
        }
        
        /**
        * Gets the root outline. All the outlines must be created with a parent.
        * The first level is created with this outline.
        * @return the root outline
        */
        public PdfOutline RootOutline {
            get {
                return rootOutline;
            }
        }
            
        internal void CalculateOutlineCount() {
            if (rootOutline.Kids.Count == 0)
                return;
            TraverseOutlineCount(rootOutline);
        }

        internal void TraverseOutlineCount(PdfOutline outline) {
            ArrayList kids = outline.Kids;
            PdfOutline parent = outline.Parent;
            if (kids.Count == 0) {
                if (parent != null) {
                    parent.Count = parent.Count + 1;
                }
            }
            else {
                for (int k = 0; k < kids.Count; ++k) {
                    TraverseOutlineCount((PdfOutline)kids[k]);
                }
                if (parent != null) {
                    if (outline.Open) {
                        parent.Count = outline.Count + parent.Count + 1;
                    }
                    else {
                        parent.Count = parent.Count + 1;
                        outline.Count = -outline.Count;
                    }
                }
            }
        }
        
        internal void WriteOutlines() {
            if (rootOutline.Kids.Count == 0)
                return;
            OutlineTree(rootOutline);
            writer.AddToBody(rootOutline, rootOutline.IndirectReference);
        }
        
        internal void OutlineTree(PdfOutline outline) {
            outline.IndirectReference = writer.PdfIndirectReference;
            if (outline.Parent != null)
                outline.Put(PdfName.PARENT, outline.Parent.IndirectReference);
            ArrayList kids = outline.Kids;
            int size = kids.Count;
            for (int k = 0; k < size; ++k)
                OutlineTree((PdfOutline)kids[k]);
            for (int k = 0; k < size; ++k) {
                if (k > 0)
                    ((PdfOutline)kids[k]).Put(PdfName.PREV, ((PdfOutline)kids[k - 1]).IndirectReference);
                if (k < size - 1)
                    ((PdfOutline)kids[k]).Put(PdfName.NEXT, ((PdfOutline)kids[k + 1]).IndirectReference);
            }
            if (size > 0) {
                outline.Put(PdfName.FIRST, ((PdfOutline)kids[0]).IndirectReference);
                outline.Put(PdfName.LAST, ((PdfOutline)kids[size - 1]).IndirectReference);
            }
            for (int k = 0; k < size; ++k) {
                PdfOutline kid = (PdfOutline)kids[k];
                writer.AddToBody(kid, kid.IndirectReference);
            }
        }
        
    //  [C3] PdfViewerPreferences interface

        /** Contains the Viewer preferences of this PDF document. */
        protected PdfViewerPreferencesImp viewerPreferences = new PdfViewerPreferencesImp();
        /** @see com.lowagie.text.pdf.interfaces.PdfViewerPreferences#setViewerPreferences(int) */
        internal int ViewerPreferences {
            set {
                this.viewerPreferences.ViewerPreferences = value;
            }
        }

        /** @see com.lowagie.text.pdf.interfaces.PdfViewerPreferences#addViewerPreference(com.lowagie.text.pdf.PdfName, com.lowagie.text.pdf.PdfObject) */
        internal void AddViewerPreference(PdfName key, PdfObject value) {
            this.viewerPreferences.AddViewerPreference(key, value);
        }

    //  [C4] Page labels

        protected internal PdfPageLabels pageLabels;

        internal PdfPageLabels PageLabels {
            set {
                this.pageLabels = value;
            }
        }
        
    //  [C5] named objects: local destinations, javascript, embedded files

        /**
        * Implements a link to other part of the document. The jump will
        * be made to a local destination with the same name, that must exist.
        * @param name the name for this link
        * @param llx the lower left x corner of the activation area
        * @param lly the lower left y corner of the activation area
        * @param urx the upper right x corner of the activation area
        * @param ury the upper right y corner of the activation area
        */
        internal void LocalGoto(String name, float llx, float lly, float urx, float ury) {
            PdfAction action = GetLocalGotoAction(name);
            annotationsImp.AddPlainAnnotation(new PdfAnnotation(writer, llx, lly, urx, ury, action));
        }
        
        /**
        * Implements a link to another document.
        * @param filename the filename for the remote document
        * @param name the name to jump to
        * @param llx the lower left x corner of the activation area
        * @param lly the lower left y corner of the activation area
        * @param urx the upper right x corner of the activation area
        * @param ury the upper right y corner of the activation area
        */
        internal void RemoteGoto(String filename, String name, float llx, float lly, float urx, float ury) {
            annotationsImp.AddPlainAnnotation(new PdfAnnotation(writer, llx, lly, urx, ury, new PdfAction(filename, name)));
        }
        
        /**
        * Implements a link to another document.
        * @param filename the filename for the remote document
        * @param page the page to jump to
        * @param llx the lower left x corner of the activation area
        * @param lly the lower left y corner of the activation area
        * @param urx the upper right x corner of the activation area
        * @param ury the upper right y corner of the activation area
        */
        internal void RemoteGoto(String filename, int page, float llx, float lly, float urx, float ury) {
            AddAnnotation(new PdfAnnotation(writer, llx, lly, urx, ury, new PdfAction(filename, page)));
        }
        
        /** Implements an action in an area.
        * @param action the <CODE>PdfAction</CODE>
        * @param llx the lower left x corner of the activation area
        * @param lly the lower left y corner of the activation area
        * @param urx the upper right x corner of the activation area
        * @param ury the upper right y corner of the activation area
        */
        internal void SetAction(PdfAction action, float llx, float lly, float urx, float ury) {
            AddAnnotation(new PdfAnnotation(writer, llx, lly, urx, ury, action));
        }
        
        /**
        * Stores the destinations keyed by name. Value is
        * <CODE>Object[]{PdfAction,PdfIndirectReference,PdfDestintion}</CODE>.
        */
        protected internal k_Tree localDestinations = new k_Tree();

        internal PdfAction GetLocalGotoAction(String name) {
            PdfAction action;
            Object[] obj = (Object[])localDestinations[name];
            if (obj == null)
                obj = new Object[3];
            if (obj[0] == null) {
                if (obj[1] == null) {
                    obj[1] = writer.PdfIndirectReference;
                }
                action = new PdfAction((PdfIndirectReference)obj[1]);
                obj[0] = action;
                localDestinations[name] = obj;
            }
            else {
                action = (PdfAction)obj[0];
            }
            return action;
        }
        
        /**
        * The local destination to where a local goto with the same
        * name will jump to.
        * @param name the name of this local destination
        * @param destination the <CODE>PdfDestination</CODE> with the jump coordinates
        * @return <CODE>true</CODE> if the local destination was added,
        * <CODE>false</CODE> if a local destination with the same name
        * already existed
        */
        internal bool LocalDestination(String name, PdfDestination destination) {
            Object[] obj = (Object[])localDestinations[name];
            if (obj == null)
                obj = new Object[3];
            if (obj[2] != null)
                return false;
            obj[2] = destination;
            localDestinations[name] = obj;
            destination.AddPage(writer.CurrentPage);
            return true;
        }
        
        /**
        * Stores a list of document level JavaScript actions.
        */
        private int jsCounter;
        protected internal Hashtable documentLevelJS = new Hashtable();

        internal void AddJavaScript(PdfAction js) {
            if (js.Get(PdfName.JS) == null)
                throw new ArgumentException("Only JavaScript actions are allowed.");
            documentLevelJS[jsCounter.ToString().PadLeft(16, '0')] = writer.AddToBody(js).IndirectReference;
            jsCounter++;
        }
        
        internal void AddJavaScript(String name, PdfAction js) {
            if (js.Get(PdfName.JS) == null)
                throw new ArgumentException("Only JavaScript actions are allowed.");
            documentLevelJS[name] = writer.AddToBody(js).IndirectReference;
        }

        internal Hashtable GetDocumentLevelJS() {
            return documentLevelJS;
        }

        protected internal Hashtable documentFileAttachment = new Hashtable();

        internal void AddFileAttachment(String description, PdfFileSpecification fs) {
            if (description == null) {
                PdfString desc = (PdfString)fs.Get(PdfName.DESC);
                if (desc == null) {
                    description = ""; 
                }
                else {
                    description = PdfEncodings.ConvertToString(desc.GetBytes(), null);
                }
            }
            fs.AddDescription(description, true);
            if (description.Length == 0)
                description = "Unnamed";
            String fn = PdfEncodings.ConvertToString(new PdfString(description, PdfObject.TEXT_UNICODE).GetBytes(), null);
            int k = 0;
            while (documentFileAttachment.ContainsKey(fn)) {
                ++k;
                fn = PdfEncodings.ConvertToString(new PdfString(description + " " + k, PdfObject.TEXT_UNICODE).GetBytes(), null);
            }
            documentFileAttachment[fn] = fs.Reference;
        }
        
        internal Hashtable GetDocumentFileAttachment() {
            return documentFileAttachment;
        }

    //  [C6] document level actions

        protected internal String openActionName;

        internal void SetOpenAction(String name) {
            openActionName = name;
            openActionAction = null;
        }
        
        protected internal PdfAction openActionAction;

        internal void SetOpenAction(PdfAction action) {
            openActionAction = action;
            openActionName = null;
        }

        protected internal PdfDictionary additionalActions;

        internal void AddAdditionalAction(PdfName actionType, PdfAction action)  {
            if (additionalActions == null)  {
                additionalActions = new PdfDictionary();
            }
            if (action == null)
                additionalActions.Remove(actionType);
            else
                additionalActions.Put(actionType, action);
            if (additionalActions.Size == 0)
                additionalActions = null;
        }
        
    //  [C7] portable collections

        protected internal PdfCollection collection;

        /**
        * Sets the collection dictionary.
        * @param collection a dictionary of type PdfCollection
        */
        public PdfCollection Collection {
            set {
                this.collection = value;
            }
        }

    //  [C8] AcroForm
        
        internal PdfAnnotationsImp annotationsImp;

        /**
        * Gets the AcroForm object.
        * @return the PdfAcroform object of the PdfDocument
        */
        public PdfAcroForm AcroForm {
            get {
                return annotationsImp.AcroForm;
            }
        }
        
        internal int SigFlags {
            set {
                annotationsImp.SigFlags = value;
            }
        }
        
        internal void AddCalculationOrder(PdfFormField formField) {
            annotationsImp.AddCalculationOrder(formField);
        }

        internal void AddAnnotation(PdfAnnotation annot) {
            pageEmpty = false;
            annotationsImp.AddAnnotation(annot);
        }
        
    //	[F12] tagged PDF

        protected int markPoint;

        internal int GetMarkPoint() {
            return markPoint;
        }
        
        internal void IncMarkPoint() {
            ++markPoint;
        }

    //	[U1] page sizes

        /** This is the size of the next page. */
        protected Rectangle nextPageSize = null;
        
        /** This is the size of the several boxes of the current Page. */
        protected Hashtable thisBoxSize = new Hashtable();
        
        /** This is the size of the several boxes that will be used in
        * the next page. */
        protected Hashtable boxSize = new Hashtable();
        
        internal Rectangle CropBoxSize {
            set {
                SetBoxSize("crop", value);
            }
        }
        
        internal void SetBoxSize(String boxName, Rectangle size) {
            if (size == null)
                boxSize.Remove(boxName);
            else
                boxSize[boxName] = new PdfRectangle(size);
        }
        
        protected internal void SetNewPageSizeAndMargins() {
            pageSize = nextPageSize;
            if (marginMirroring && (PageNumber & 1) == 0) {
                marginRight = nextMarginLeft;
                marginLeft = nextMarginRight;
            }
            else {
                marginLeft = nextMarginLeft;
                marginRight = nextMarginRight;
            }
            marginTop = nextMarginTop;
            marginBottom = nextMarginBottom;
        }

        /**
        * Gives the size of a trim, art, crop or bleed box, or null if not defined.
        * @param boxName crop, trim, art or bleed
        */
        internal Rectangle GetBoxSize(String boxName) {
            PdfRectangle r = (PdfRectangle)thisBoxSize[boxName];
            if (r != null) return r.Rectangle;
            return null;
        }
        
    //	[U2] empty pages

        /** This checks if the page is empty. */
        protected internal bool pageEmpty = true;
        
        internal bool PageEmpty {
            set {
                this.pageEmpty = value;
            }
        }


    //	[U3] page actions

        /** The duration of the page */
        protected int duration=-1; // negative values will indicate no duration
        
        /** The page transition */
        protected PdfTransition transition=null; 
        
        /**
        * Sets the display duration for the page (for presentations)
        * @param seconds   the number of seconds to display the page
        */
        internal int Duration {
            set {
                if (value > 0)
                    this.duration=value;
                else
                    this.duration=-1;
            }
        }
        
        /**
        * Sets the transition for the page
        * @param transition   the PdfTransition object
        */
        internal PdfTransition Transition {
            set {
                this.transition=value;
            }
        }

        protected PdfDictionary pageAA = null;

        internal void SetPageAction(PdfName actionType, PdfAction action) {
            if (pageAA == null) {
                pageAA = new PdfDictionary();
            }
            pageAA.Put(actionType, action);
        }
        
    //	[U8] thumbnail images

        protected internal PdfIndirectReference thumb;

        internal Image Thumbnail {
            set {
                thumb = writer.GetImageReference(writer.AddDirectImageSimple(value));
            }
        }

    //	[M0] Page resources contain references to fonts, extgstate, images,...

        /** This are the page resources of the current Page. */
        protected internal PageResources pageResources;
        
        internal PageResources PageResources {
            get {
                return pageResources;
            }
        }
        
    //	[M3] Images

        /** Holds value of property strictImageSequence. */
        protected internal bool strictImageSequence = false;    

        /** Setter for property strictImageSequence.
        * @param strictImageSequence New value of property strictImageSequence.
        *
        */
        internal bool StrictImageSequence {
            set {
                this.strictImageSequence = value;
            }
            get {
                return strictImageSequence;
            }
        }
     
        /** This is the position where the image ends. */
        protected internal float imageEnd = -1;
        
        /**
        * Method added by Pelikan Stephan
        * @see com.lowagie.text.DocListener#clearTextWrap()
        */
        public void ClearTextWrap() {
            float tmpHeight = imageEnd - currentHeight;
            if (line != null) {
                tmpHeight += line.Height;
            }
            if ((imageEnd > -1) && (tmpHeight > 0)) {
                CarriageReturn();
                currentHeight += tmpHeight;
            }
        }
        
        /** This is the image that could not be shown on a previous page. */
        protected internal Image imageWait = null;
        
        /**
        * Adds an image to the document.
        * @param image the <CODE>Image</CODE> to add
        * @throws PdfException on error
        * @throws DocumentException on error
        */        
        protected internal void Add(Image image) {
            
            if (image.HasAbsolutePosition()) {
                graphics.AddImage(image);
                pageEmpty = false;
                return;
            }
            
            // if there isn't enough room for the image on this page, save it for the next page
            if (currentHeight != 0 && IndentTop - currentHeight - image.ScaledHeight < IndentBottom) {
                if (!strictImageSequence && imageWait == null) {
                    imageWait = image;
                    return;
                }
                NewPage();
                if (currentHeight != 0 && IndentTop - currentHeight - image.ScaledHeight < IndentBottom) {
                    imageWait = image;
                    return;
                }
            }
            pageEmpty = false;
            // avoid endless loops
            if (image == imageWait)
                imageWait = null;
            bool textwrap = (image.Alignment & Image.TEXTWRAP) == Image.TEXTWRAP
            && !((image.Alignment & Image.MIDDLE_ALIGN) == Image.MIDDLE_ALIGN);
            bool underlying = (image.Alignment & Image.UNDERLYING) == Image.UNDERLYING;
            float diff = leading / 2;
            if (textwrap) {
                diff += leading;
            }
            float lowerleft = IndentTop - currentHeight - image.ScaledHeight - diff;
            float[] mt = image.Matrix;
            float startPosition = IndentLeft - mt[4];
            if ((image.Alignment & Image.RIGHT_ALIGN) == Image.RIGHT_ALIGN) startPosition = IndentRight - image.ScaledWidth - mt[4];
            if ((image.Alignment & Image.MIDDLE_ALIGN) == Image.MIDDLE_ALIGN) startPosition = IndentLeft + ((IndentRight - IndentLeft - image.ScaledWidth) / 2) - mt[4];
            if (image.HasAbsoluteX()) startPosition = image.AbsoluteX;
            if (textwrap) {
                if (imageEnd < 0 || imageEnd < currentHeight + image.ScaledHeight + diff) {
                    imageEnd = currentHeight + image.ScaledHeight + diff;
                }
                if ((image.Alignment & Image.RIGHT_ALIGN) == Image.RIGHT_ALIGN) {
                    // indentation suggested by Pelikan Stephan
                    indentation.imageIndentRight += image.ScaledWidth + image.IndentationLeft;
                }
                else {
                    // indentation suggested by Pelikan Stephan
                    indentation.imageIndentLeft += image.ScaledWidth + image.IndentationRight;
                }
            }
            else {
                if ((image.Alignment & Image.RIGHT_ALIGN) == Image.RIGHT_ALIGN) startPosition -= image.IndentationRight;
                else if ((image.Alignment & Image.MIDDLE_ALIGN) == Image.MIDDLE_ALIGN) startPosition += image.IndentationLeft - image.IndentationRight;
                else startPosition -= image.IndentationRight;
            }
            graphics.AddImage(image, mt[0], mt[1], mt[2], mt[3], startPosition, lowerleft - mt[5]);
            if (!(textwrap || underlying)) {
                currentHeight += image.ScaledHeight + diff;
                FlushLines();
                text.MoveText(0, - (image.ScaledHeight + diff));
                NewLine();
            }
        }
        
    //	[M4] Adding a PdfPTable

        /** Adds a <CODE>PdfPTable</CODE> to the document.
        * @param ptable the <CODE>PdfPTable</CODE> to be added to the document.
        * @throws DocumentException on error
        */
        internal void AddPTable(PdfPTable ptable) {
            ColumnText ct = new ColumnText(writer.DirectContent);
            if (currentHeight > 0) {
                Paragraph p = new Paragraph();
                p.Leading = 0;
                ct.AddElement(p);
                //if the table prefers to be on a single page, and it wouldn't
                //fit on the current page, start a new page.
                if (ptable.KeepTogether && !FitsPage(ptable, 0f))
                    NewPage();
            }
            ct.AddElement(ptable);
            bool he = ptable.HeadersInEvent;
            ptable.HeadersInEvent = true;
            int loop = 0;
            while (true) {
                ct.SetSimpleColumn(IndentLeft, IndentBottom, IndentRight, IndentTop - currentHeight);
                int status = ct.Go();
                if ((status & ColumnText.NO_MORE_TEXT) != 0) {
                    text.MoveText(0, ct.YLine - IndentTop + currentHeight);
                    currentHeight = IndentTop - ct.YLine;
                    break;
                }
                if (IndentTop - currentHeight == ct.YLine)
                    ++loop;
                else
                    loop = 0;
                if (loop == 3) {
                    Add(new Paragraph("ERROR: Infinite table loop"));
                    break;
                }
                NewPage();
            }
            ptable.HeadersInEvent = he;
        }
        
        internal bool FitsPage(PdfPTable table, float margin) {
            if (!table.LockedWidth) {
                float totalWidth = (IndentRight - IndentLeft) * table.WidthPercentage / 100;
                table.TotalWidth = totalWidth;
            }
            // ensuring that a new line has been started.
            EnsureNewLine();
            return table.TotalHeight <= IndentTop - currentHeight - IndentBottom - margin;
        }
        
    //	[M4'] Adding a Table
        
        protected internal class RenderingContext {
            internal float pagetop = -1;
            internal float oldHeight = -1;

            internal PdfContentByte cellGraphics = null;
            
            internal float lostTableBottom;
            
            internal float maxCellBottom;
            //internal float maxCellHeight;
            
            internal Hashtable rowspanMap;
            internal Hashtable pageMap = new Hashtable();
            /**
            * A PdfPTable
            */
            public PdfTable table;
            
            /**
            * Consumes the rowspan
            * @param c
            * @return a rowspan.
            */
            public int ConsumeRowspan(PdfCell c) {
                if (c.Rowspan == 1) {
                    return 1;
                }
                
                object i = rowspanMap[c];
                if (i == null) {
                    i = c.Rowspan;
                }
                
                i = (int)i - 1;
                rowspanMap[c] = i;

                if ((int)i < 1) {
                    return 1;
                }
                return (int)i;
            }

            /**
            * Looks at the current rowspan.
            * @param c
            * @return the current rowspan
            */
            public int CurrentRowspan(PdfCell c) {
                object i = rowspanMap[c];
                if (i == null) {
                    return c.Rowspan;
                } else {
                    return (int)i;
                }
            }
            
            public int CellRendered(PdfCell cell, int pageNumber) {
                object i = pageMap[cell];
                if (i == null) {
                    i = 1;
                } else {
                    i = (int)i + 1;
                }
                pageMap[cell] = i;

                Hashtable seti = (Hashtable)pageMap[pageNumber];
                
                if (seti == null) {
                    seti = new Hashtable();
                    pageMap[pageNumber] = seti;
                }
                
                seti[cell] = null;
                
                return (int)i;
            }

            public int NumCellRendered(PdfCell cell) {
                object i = pageMap[cell];
                if (i == null) {
                    i = 0;
                } 
                return (int)i;
            }
            
            public bool IsCellRenderedOnPage(PdfCell cell, int pageNumber) {
                Hashtable seti = (Hashtable) pageMap[pageNumber];
                
                if (seti != null) {
                    return seti.ContainsKey(cell);
                }
                
                return false;
            }
        };
        
        /**
        * Adds a new table to 
        * @param table              Table to add.  Rendered rows will be deleted after processing.
        * @param onlyFirstPage      Render only the first full page
        * @throws DocumentException
        */        
        private void AddPdfTable(Table t) {
            // before every table, we flush all lines
            FlushLines();

            PdfTable table = new PdfTable(t, IndentLeft, IndentRight, IndentTop - currentHeight);
            RenderingContext ctx = new RenderingContext();
            ctx.pagetop = IndentTop;
            ctx.oldHeight = currentHeight;
            ctx.cellGraphics = new PdfContentByte(writer);
            ctx.rowspanMap = new Hashtable();
            ctx.table = table;

            // initialisation of parameters
            PdfCell cell;
                            
            // drawing the table
            ArrayList headercells = table.HeaderCells;
            ArrayList cells = table.Cells;
            ArrayList rows = ExtractRows(cells, ctx);
            bool isContinue = false;
            while (cells.Count != 0) {
                // initialisation of some extra parameters;
                ctx.lostTableBottom = 0;
                            
                // loop over the cells
                bool cellsShown = false;

                // draw the cells (line by line)
                ListIterator iterator = new ListIterator(rows);
                  
                bool atLeastOneFits = false;
                while (iterator.HasNext()) {
                    ArrayList row = (ArrayList) iterator.Next();
                    AnalyzeRow(rows, ctx);
                    RenderCells(ctx, row, table.HasToFitPageCells() & atLeastOneFits);
                                    
                    if (!MayBeRemoved(row)) {
                        break;
                    }
                    
                    ConsumeRowspan(row, ctx);
                    iterator.Remove();
                    atLeastOneFits = true;
                }

    //          compose cells array list for subsequent code
                cells.Clear();
                Hashtable opt = new Hashtable();
                foreach (ArrayList row in rows) {
                    foreach (PdfCell cellp in row) {
                        if (!opt.ContainsKey(cellp)) {
                            cells.Add(cellp);
                            opt[cellp] = null;
                        }
                    }
                }
                // we paint the graphics of the table after looping through all the cells
                Rectangle tablerec = new Rectangle(table);
                tablerec.Border = table.Border;
                tablerec.BorderWidth = table.BorderWidth;
                tablerec.BorderColor = table.BorderColor;
                tablerec.BackgroundColor = table.BackgroundColor;
                PdfContentByte under = writer.DirectContentUnder;
                under.Rectangle(tablerec.GetRectangle(Top, IndentBottom));
                under.Add(ctx.cellGraphics);
                // bugfix by Gerald Fehringer: now again add the border for the table
                // since it might have been covered by cell backgrounds
                tablerec.BackgroundColor = null;
                tablerec = tablerec.GetRectangle(Top, IndentBottom);
                tablerec.Border = table.Border;
                under.Rectangle(tablerec);
                // end bugfix
                ctx.cellGraphics = new PdfContentByte(null);
                // if the table continues on the next page
                if (rows.Count != 0) {
                    isContinue = true;
                    graphics.SetLineWidth(table.BorderWidth);
                    if (cellsShown && (table.Border & Rectangle.BOTTOM_BORDER) == Rectangle.BOTTOM_BORDER) {
                        // Draw the bottom line
                                    
                        // the color is set to the color of the element
                        Color tColor = table.BorderColor;
                        if (tColor != null) {
                            graphics.SetColorStroke(tColor);
                        }
                        graphics.MoveTo(table.Left, Math.Max(table.Bottom, IndentBottom));
                        graphics.LineTo(table.Right, Math.Max(table.Bottom, IndentBottom));
                        graphics.Stroke();
                        if (tColor != null) {
                            graphics.ResetRGBColorStroke();
                        }
                    }
                                
                    // old page
                    pageEmpty = false;
                    float difference = ctx.lostTableBottom;
                                
                    // new page
                    NewPage();
                    // G.F.: if something added in page event i.e. currentHeight > 0
                    float heightCorrection = 0;
                    bool somethingAdded = false;
                    if (currentHeight > 0) {
                        heightCorrection = 6;
                        currentHeight += heightCorrection;
                        somethingAdded = true;
                        NewLine();
                        FlushLines();
                        indentation.indentTop = currentHeight - leading;
                        currentHeight = 0;
                    }
                    else {
                        FlushLines();
                    }
                    
                    // this part repeats the table headers (if any)
                    int size = headercells.Count;
                    if (size > 0) {
                        // this is the top of the headersection
                        cell = (PdfCell) headercells[0];
                        float oldTop = cell.GetTop(0);
                        // loop over all the cells of the table header
                        for (int ii = 0; ii < size; ii++) {
                            cell = (PdfCell) headercells[ii];
                            // calculation of the new cellpositions
                            cell.Top = IndentTop - oldTop + cell.GetTop(0);
                            cell.Bottom = IndentTop - oldTop + cell.GetBottom(0);
                            ctx.pagetop = cell.Bottom;
                            // we paint the borders of the cell
                            ctx.cellGraphics.Rectangle(cell.Rectangle(IndentTop, IndentBottom));
                            // we write the text of the cell
                            ArrayList images = cell.GetImages(IndentTop, IndentBottom);
                            foreach (Image image in images) {
                                cellsShown = true;
                                graphics.AddImage(image);
                            }
                            lines = cell.GetLines(IndentTop, IndentBottom);
                            float cellTop = cell.GetTop(IndentTop);
                            text.MoveText(0, cellTop-heightCorrection);
                            float cellDisplacement = FlushLines() - cellTop+heightCorrection;
                            text.MoveText(0, cellDisplacement);
                        }           
                        currentHeight = IndentTop - ctx.pagetop + table.Cellspacing;
                        text.MoveText(0, ctx.pagetop - IndentTop - currentHeight);
                    }
                    else {
                        if (somethingAdded) {
                            ctx.pagetop = IndentTop;
                            text.MoveText(0, -table.Cellspacing);
                        }
                    }
                    ctx.oldHeight = currentHeight - heightCorrection;
                    // calculating the new positions of the table and the cells
                    size = Math.Min(cells.Count, table.Columns);
                    int i = 0;
                    while (i < size) {
                        cell = (PdfCell) cells[i];
                        if (cell.GetTop(-table.Cellspacing) > ctx.lostTableBottom) {
                            float newBottom = ctx.pagetop - difference + cell.Bottom;
                            float neededHeight = cell.RemainingHeight;
                            if (newBottom > ctx.pagetop - neededHeight) {
                                difference += newBottom - (ctx.pagetop - neededHeight);
                            }
                        }
                        i++;
                    }
                    size = cells.Count;
                    table.Top = IndentTop;
                    table.Bottom = ctx.pagetop - difference + table.GetBottom(table.Cellspacing);
                    for (i = 0; i < size; i++) {
                        cell = (PdfCell) cells[i];
                        float newBottom = ctx.pagetop - difference + cell.Bottom;
                        float newTop = ctx.pagetop - difference + cell.GetTop(-table.Cellspacing);
                        if (newTop > IndentTop - currentHeight) {
                            newTop = IndentTop - currentHeight;
                        }
                        cell.Top = newTop ;
                        cell.Bottom = newBottom ;
                    }
                }
            }
                        
            float tableHeight = table.Top - table.Bottom;
            // bugfix by Adauto Martins when have more than two tables and more than one page 
            // If continuation of table in other page (bug report #1460051)
            if (isContinue) {
                currentHeight = tableHeight;
                text.MoveText(0, -(tableHeight - (ctx.oldHeight * 2)));
            }
            else {
                currentHeight = ctx.oldHeight + tableHeight;
                text.MoveText(0, -tableHeight);
            }
            pageEmpty = false;
        }

        protected internal void AnalyzeRow(ArrayList rows, RenderingContext ctx) {
            ctx.maxCellBottom = IndentBottom;

            // determine whether Row(index) is in a rowspan
            int rowIndex = 0;

            ArrayList row = (ArrayList) rows[rowIndex];
            int maxRowspan = 1;
            foreach (PdfCell cell in row) {
                maxRowspan = Math.Max(ctx.CurrentRowspan(cell), maxRowspan);
            }
            rowIndex += maxRowspan;
            
            bool useTop = true;
            if (rowIndex == rows.Count) {
                rowIndex = rows.Count - 1;
                useTop = false;
            }
            
            if (rowIndex < 0 || rowIndex >= rows.Count) return;
            
            row = (ArrayList) rows[rowIndex];
            foreach (PdfCell cell in row) {
                Rectangle cellRect = cell.Rectangle(ctx.pagetop, IndentBottom);
                if (useTop) {
                    ctx.maxCellBottom = Math.Max(ctx.maxCellBottom, cellRect.Top);
                } else {
                    if (ctx.CurrentRowspan(cell) == 1) {
                        ctx.maxCellBottom = Math.Max(ctx.maxCellBottom, cellRect.Bottom);
                    }
                }
            }
        }
        
        protected internal bool MayBeRemoved(ArrayList row) {
            bool mayBeRemoved = true;
            foreach (PdfCell cell in row) {
                mayBeRemoved &= cell.MayBeRemoved();
            }
            return mayBeRemoved;
        }

        protected internal void ConsumeRowspan(ArrayList row, RenderingContext ctx) {
            foreach (PdfCell c in row) {
                ctx.ConsumeRowspan(c);
            }
        }
        
        protected internal ArrayList ExtractRows(ArrayList cells, RenderingContext ctx) {
            PdfCell cell;
            PdfCell previousCell = null;
            ArrayList rows = new ArrayList();
            ArrayList rowCells = new ArrayList();
            
            ListIterator iterator = new ListIterator(cells);
            while (iterator.HasNext()) {
                cell = (PdfCell) iterator.Next();

                bool isAdded = false;

                bool isEndOfRow = !iterator.HasNext();
                bool isCurrentCellPartOfRow = !iterator.HasNext();
                
                if (previousCell != null) {
                    if (cell.Left <= previousCell.Left) {
                        isEndOfRow = true;
                        isCurrentCellPartOfRow = false;
                    }
                }
                
                if (isCurrentCellPartOfRow) {
                    rowCells.Add(cell);
                    isAdded = true;
                }
                
                if (isEndOfRow) {
                    if (rowCells.Count != 0) {
                        // add to rowlist
                        rows.Add(rowCells);
                    }
                    
                    // start a new list for next line
                    rowCells = new ArrayList();                
                }

                if (!isAdded) {
                    rowCells.Add(cell);
                }
                
                previousCell = cell;
            }
            
            if (rowCells.Count != 0) {
                rows.Add(rowCells);
            }
            
            // fill row information with rowspan cells to get complete "scan lines"
            for (int i = rows.Count - 1; i >= 0; i--) {
                ArrayList row = (ArrayList) rows[i];

                // iterator through row
                for (int j = 0; j < row.Count; j++) {
                    PdfCell c = (PdfCell) row[j];
                    int rowspan = c.Rowspan;
                    
                    // fill in missing rowspan cells to complete "scan line"
                    for (int k = 1; k < rowspan && rows.Count < i+k; k++) {
                        ArrayList spannedRow = ((ArrayList) rows[i + k]);
                        if (spannedRow.Count > j)
                            spannedRow.Insert(j, c);
                    }
                }
            }
                    
            return rows;
        }

        protected internal void RenderCells(RenderingContext ctx, ArrayList cells, bool hasToFit) {
            if (hasToFit) {
                foreach (PdfCell cell in cells) {
                    if (!cell.Header) {
                        if (cell.Bottom < IndentBottom) return;
                    }
                }
            }
            foreach (PdfCell cell in cells) {
                if (!ctx.IsCellRenderedOnPage(cell, PageNumber)) {

                    float correction = 0;
                    if (ctx.NumCellRendered(cell) >= 1) {
                        correction = 1.0f;
                    }
                
                    lines = cell.GetLines(ctx.pagetop, IndentBottom - correction);
                    
                    // if there is still text to render we render it
                    if (lines != null && lines.Count > 0) {
                        
                        // we write the text
                        float cellTop = cell.GetTop(ctx.pagetop - ctx.oldHeight);
                        text.MoveText(0, cellTop);
                        float cellDisplacement = FlushLines() - cellTop;
                        
                        text.MoveText(0, cellDisplacement);
                        if (ctx.oldHeight + cellDisplacement > currentHeight) {
                            currentHeight = ctx.oldHeight + cellDisplacement;
                        }

                        ctx.CellRendered(cell, PageNumber);
                    } 
                                
                    float indentBottom = Math.Max(cell.Bottom, IndentBottom);
        
                    Rectangle tableRect = ctx.table.GetRectangle(ctx.pagetop, IndentBottom);
                    
                    indentBottom = Math.Max(tableRect.Bottom, indentBottom);
                    
                    // we paint the borders of the cells
                    Rectangle cellRect = cell.GetRectangle(tableRect.Top, indentBottom);
                    //cellRect.Bottom = cellRect.Bottom;
                    if (cellRect.Height > 0) {
                        ctx.lostTableBottom = indentBottom;
                        ctx.cellGraphics.Rectangle(cellRect);
                    }
        
                    // and additional graphics
                    ArrayList images = cell.GetImages(ctx.pagetop, IndentBottom);
                    foreach (Image image in images) {
                        graphics.AddImage(image);
                    }
                }
            }
        }

        /**
        * Returns the bottomvalue of a <CODE>Table</CODE> if it were added to this document.
        *
        * @param    table   the table that may or may not be added to this document
        * @return   a bottom value
        */        
        internal float GetBottom(Table table) {
            // constructing a PdfTable
            PdfTable tmp = new PdfTable(table, IndentLeft, IndentRight, IndentTop - currentHeight);
            return tmp.Bottom;
        }
        
    //	[M5] header/footer
        protected internal void DoFooter() {
    	    if (footer == null) return;
		    // Begin added by Edgar Leonardo Prieto Perilla
    	    // Avoid footer identation
    	    float tmpIndentLeft = indentation.indentLeft;
    	    float tmpIndentRight = indentation.indentRight;
    	    // Begin added: Bonf (Marc Schneider) 2003-07-29
            float tmpListIndentLeft = indentation.listIndentLeft;
            float tmpImageIndentLeft = indentation.imageIndentLeft;
            float tmpImageIndentRight = indentation.imageIndentRight;
            // End added: Bonf (Marc Schneider) 2003-07-29

            indentation.indentLeft = indentation.indentRight = 0;
            // Begin added: Bonf (Marc Schneider) 2003-07-29
            indentation.listIndentLeft = 0;
            indentation.imageIndentLeft = 0;
            indentation.imageIndentRight = 0;
            // End added: Bonf (Marc Schneider) 2003-07-29
            // End Added by Edgar Leonardo Prieto Perilla
            footer.PageNumber = pageN;
            leading = footer.Paragraph.TotalLeading;
            Add(footer.Paragraph);
            // adding the footer limits the height
            indentation.indentBottom = currentHeight;
            text.MoveText(Left, IndentBottom);
            FlushLines();
            text.MoveText(-Left, -Bottom);
            footer.Top = GetBottom(currentHeight);
            footer.Bottom = Bottom - (0.75f * leading);
            footer.Left = Left;
            footer.Right = Right;
            graphics.Rectangle(footer);
            indentation.indentBottom = currentHeight + leading * 2;
            currentHeight = 0;
            // Begin added by Edgar Leonardo Prieto Perilla
            indentation.indentLeft = tmpIndentLeft;
            indentation.indentRight = tmpIndentRight;
            // Begin added: Bonf (Marc Schneider) 2003-07-29
            indentation.listIndentLeft = tmpListIndentLeft;
            indentation.imageIndentLeft = tmpImageIndentLeft;
            indentation.imageIndentRight = tmpImageIndentRight;
            // End added: Bonf (Marc Schneider) 2003-07-29
            // End added by Edgar Leonardo Prieto Perilla
        }
        
        protected internal void DoHeader() {
            // if there is a header, the header = added
            if (header == null) return;
		    // Begin added by Edgar Leonardo Prieto Perilla
		    // Avoid header identation
		    float tmpIndentLeft = indentation.indentLeft;
		    float tmpIndentRight = indentation.indentRight;
            // Begin added: Bonf (Marc Schneider) 2003-07-29
            float tmpListIndentLeft = indentation.listIndentLeft;
            float tmpImageIndentLeft = indentation.imageIndentLeft;
            float tmpImageIndentRight = indentation.imageIndentRight;
            // End added: Bonf (Marc Schneider) 2003-07-29
            indentation.indentLeft = indentation.indentRight = 0;
            //  Added: Bonf
            indentation.listIndentLeft = 0;
            indentation.imageIndentLeft = 0;
            indentation.imageIndentRight = 0;
            // End added: Bonf
            // Begin added by Edgar Leonardo Prieto Perilla
		    header.PageNumber = pageN;
            leading = header.Paragraph.TotalLeading;
            text.MoveText(0, leading);
            Add(header.Paragraph);
            NewLine();
            indentation.indentTop = currentHeight - leading;
            header.Top = Top + leading;
            header.Bottom = IndentTop + leading * 2 / 3;
            header.Left = Left;
            header.Right = Right;
            graphics.Rectangle(header);
            FlushLines();
            currentHeight = 0;
            // Begin added by Edgar Leonardo Prieto Perilla
            // Restore identation
		    indentation.indentLeft = tmpIndentLeft;
		    indentation.indentRight = tmpIndentRight;
            // Begin added: Bonf (Marc Schneider) 2003-07-29
            indentation.listIndentLeft = tmpListIndentLeft;
            indentation.imageIndentLeft = tmpImageIndentLeft;
            indentation.imageIndentRight = tmpImageIndentRight;
            // End added: Bonf (Marc Schneider) 2003-07-29
		    // End Added by Edgar Leonardo Prieto Perilla
        }
    }
}
