using System;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.rtf.document;
using iTextSharp.text.rtf.text;
using iTextSharp.text.rtf.parser;
/*
 * $Id: RtfWriter2.cs,v 1.11 2008/05/23 17:24:11 psoares33 Exp $
 * 
 *
 * Copyright 2001, 2002, 2003, 2004 by Mark Hall
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
 * LGPL license (the ?GNU LIBRARY GENERAL PUBLIC LICENSE?), in which case the
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

namespace iTextSharp.text.rtf {

    /**
    * The RtfWriter allows the creation of rtf documents via the iText system
    *
    * Version: $Id: RtfWriter2.cs,v 1.11 2008/05/23 17:24:11 psoares33 Exp $
    * @author Mark Hall (Mark.Hall@mail.room3b.eu)
    */
    public class RtfWriter2 : DocWriter {
        /**
        * The RtfDocument this RtfWriter is creating
        */
        private RtfDocument rtfDoc = null;
        
        /**
        * Constructs a new RtfWriter that listens to the specified Document and
        * writes its output to the Stream.
        * 
        * @param doc The Document that this RtfWriter listens to
        * @param os The Stream to write to
        */
        protected RtfWriter2(Document doc, Stream os) : base(doc, os) {
            doc.AddDocListener(this);
            rtfDoc = new RtfDocument();
        }

        /**
        * Static method to generate RtfWriters
        * 
        * @param doc The Document that this RtfWriter listens to
        * @param os The Stream to write to
        * @return The new RtfWriter
        */
        public static RtfWriter2 GetInstance(Document doc, Stream os) {
            return new RtfWriter2(doc, os);
        }

        /**
        * Sets the header to use
        * 
        * @param hf The HeaderFooter to use
        */
        public override HeaderFooter Header {
            set {
                this.rtfDoc.GetDocumentHeader().SetHeader(value);
            }
        }
        
        /**
        * Resets the header
        */
        public override void ResetHeader() {
            this.rtfDoc.GetDocumentHeader().SetHeader(null);
        }
        
        /**
        * Sets the footer to use
        * 
        * @param hf The HeaderFooter to use
        */
        public override HeaderFooter Footer {
            set {
                this.rtfDoc.GetDocumentHeader().SetFooter(value);
            }
        }
        
        /**
        * Resets the footer
        */
        public override void ResetFooter() {
            this.rtfDoc.GetDocumentHeader().SetFooter(null);
        }

        /**
        * This method is not supported in the RtfWriter
        * @param i Unused
        */
        public override int PageCount {
            set {}
        }
        
        /**
        * This method is not supported in the RtfWriter
        */
        public override void ResetPageCount() {
        }

        /**
        * Opens the RtfDocument
        */
        public override void Open() {
            base.Open();
            this.rtfDoc.Open();
        }
        
        /**
        * Closes the RtfDocument. This causes the document to be written
        * to the specified Stream
        */
        public override void Close() {
            if (open) {
                rtfDoc.WriteDocument(os);
                base.Close();
                this.rtfDoc = new RtfDocument();
            }
        }

        /**
        * Adds an Element to the Document
        *
        * @param element The element to be added
        * @return <code>false</code>
        * @throws DocumentException
        */
        public override  bool Add(IElement element) {
            if (pause) {
                return false;
            }
            IRtfBasicElement[] rtfElements = rtfDoc.GetMapper().MapElement(element);
            if(rtfElements.Length != 0) {
                for (int i = 0; i < rtfElements.Length; i++) {
                    if (rtfElements[i] != null) {
                        rtfDoc.Add(rtfElements[i]);
                    }
                }
                return true;
            } else {
                return false;
            }
        }
        
        /**
        * Adds a page break
        *
        * @return <code>false</code>
        */
        public override bool NewPage() {
            rtfDoc.Add(new RtfNewPage(rtfDoc));
            return true;
        }

        /**
        * Sets the page margins
        *
        * @param left The left margin
        * @param right The right margin
        * @param top The top margin
        * @param bottom The bottom margin
        * @return <code>false</code>
        */
        public override bool SetMargins(float left, float right, float top, float bottom) {
            rtfDoc.GetDocumentHeader().GetPageSetting().SetMarginLeft((int) (left * RtfElement.TWIPS_FACTOR));
            rtfDoc.GetDocumentHeader().GetPageSetting().SetMarginRight((int) (right * RtfElement.TWIPS_FACTOR));
            rtfDoc.GetDocumentHeader().GetPageSetting().SetMarginTop((int) (top * RtfElement.TWIPS_FACTOR));
            rtfDoc.GetDocumentHeader().GetPageSetting().SetMarginBottom((int) (bottom * RtfElement.TWIPS_FACTOR));
            return true;
        }
        
        /**
        * Sets the size of the page
        *
        * @param rect A Rectangle representing the page
        * @return <code>false</code>
        */
        public override bool SetPageSize(Rectangle rect) {
            rtfDoc.GetDocumentHeader().GetPageSetting().SetPageSize(rect);
            return true;
        }
        
        /**
        * Whether to automagically generate table of contents entries when
        * adding Chapters or Sections.
        * 
        * @param autogenerate Whether to automatically generate TOC entries
        */
        public void SetAutogenerateTOCEntries(bool autogenerate) {
            this.rtfDoc.SetAutogenerateTOCEntries(autogenerate);
        }
        
        /**
        * Gets the RtfDocumentSettings that specify how the rtf document is generated.
        * 
        * @return The current RtfDocumentSettings.
        */
        public RtfDocumentSettings GetDocumentSettings() {
            return this.rtfDoc.GetDocumentSettings();
        }

        /**
        * Adds the complete RTF document to the current RTF document being generated.
        * It will parse the font and color tables and correct the font and color references
        * so that the imported RTF document retains its formattings.
        * 
        * @param documentSource The Stream to read the RTF document from.
        * @throws IOException On errors reading the RTF document.
        * @throws DocumentException On errors adding to this RTF document.
        */
        public void ImportRtfDocument(Stream documentSource) {
            ImportRtfDocument(documentSource, null);
        }
        
        /**
        * Adds the complete RTF document to the current RTF document being generated.
        * It will parse the font and color tables and correct the font and color references
        * so that the imported RTF document retains its formattings.
        * Uses new RtfParser object.
        * 
        * @param documentSource The Stream to read the RTF document from.
        * @param eventListeners The array of event listeners. May be null
        * @throws IOException
        * @throws DocumentException
        * 
        * @see com.lowagie.text.rtf.parser.RtfParser
        * @see com.lowagie.text.rtf.parser.RtfParser#importRtfDocument(Reader, RtfDocument)
        * @since 2.0.8
        * @author Howard Shank
        */
        public void ImportRtfDocument(Stream documentSource, IEventListener[] events ) {
            if(!this.open) {
                throw new DocumentException("The document must be open to import RTF documents.");
            }
            RtfParser rtfImport = new RtfParser();
            if(events != null) {
                for(int idx=0;idx<events.Length;idx++) {
                    rtfImport.AddListener(events[idx]);
                }
            }
            rtfImport.ImportRtfDocument(documentSource, this.rtfDoc);
        }
        
        /**
        * Adds a fragment of an RTF document to the current RTF document being generated.
        * Since this fragment doesn't contain font or color tables, all fonts and colors
        * are mapped to the default font and color. If the font and color mappings are
        * known, they can be specified via the mappings parameter.
        * 
        * @param documentSource The Stream to read the RTF fragment from.
        * @param mappings The RtfImportMappings that contain font and color mappings to apply to the fragment.
        * @throws IOException On errors reading the RTF fragment.
        * @throws DocumentException On errors adding to this RTF fragment.
        */
        public void ImportRtfFragment(Stream documentSource, RtfImportMappings mappings) {
            ImportRtfFragment(documentSource, mappings, null);
        }
        
        /**
        * Adds a fragment of an RTF document to the current RTF document being generated.
        * Since this fragment doesn't contain font or color tables, all fonts and colors
        * are mapped to the default font and color. If the font and color mappings are
        * known, they can be specified via the mappings parameter.
        * Uses new RtfParser object.
        * 
        * @param documentSource The Stream to read the RTF fragment from.
        * @param mappings The RtfImportMappings that contain font and color mappings to apply to the fragment.
        * @param eventListeners The array of event listeners. May be null
        * @throws IOException On errors reading the RTF fragment.
        * @throws DocumentException On errors adding to this RTF fragment.
        * 
        * @see com.lowagie.text.rtf.parser.RtfImportMappings
        * @see com.lowagie.text.rtf.parser.RtfParser
        * @see com.lowagie.text.rtf.parser.RtfParser#importRtfFragment(Reader, RtfDocument, com.lowagie.text.rtf.parser.RtfImportMappings)
        * @since 2.0.8
        * @author Howard Shank
        */
        public void ImportRtfFragment(Stream documentSource, RtfImportMappings mappings, IEventListener[] events ) {
            if(!this.open) {
                throw new DocumentException("The document must be open to import RTF fragments.");
            }
            RtfParser rtfImport = new RtfParser();
            if(events != null) {
                for(int idx=0;idx<events.Length;idx++) {
                    rtfImport.AddListener(events[idx]);
                }
            }
            rtfImport.ImportRtfFragment(documentSource, this.rtfDoc, mappings);
        }
    }
}