using System;
using System.IO;
using System.Collections;
using System.Text;
using iTextSharp.text;
using iTextSharp.text.rtf.document;
using iTextSharp.text.rtf.field;
using FD = iTextSharp.text.rtf.field;
using iTextSharp.text.rtf;
/*
 * $Id: RtfSection.cs,v 1.9 2008/05/16 19:31:24 psoares33 Exp $
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

namespace iTextSharp.text.rtf.text {

    /**
    * The RtfSection wraps a Section element.
    * INTERNAL CLASS
    * 
    * @version $Version:$
    * @author Mark Hall (Mark.Hall@mail.room3b.eu)
    */
    public class RtfSection : RtfElement {

        /**
        * The title paragraph of this RtfSection
        */
        protected RtfParagraph title = null;
        /**
        * The sub-items of this RtfSection
        */
        protected ArrayList items = null;
        
        /**
        * Constructs a RtfSection for a given Section. If the autogenerateTOCEntries
        * property of the RtfDocument is set and the title is not empty then a TOC entry
        * is generated for the title.
        *  
        * @param doc The RtfDocument this RtfSection belongs to
        * @param section The Section this RtfSection is based on
        */
        public RtfSection(RtfDocument doc, Section section) : base(doc) {
            items = new ArrayList();
            try {
                if (section.Title != null) {
                    this.title = (RtfParagraph) doc.GetMapper().MapElement(section.Title)[0];
                }
                if (document.GetAutogenerateTOCEntries()) {
                    StringBuilder titleText = new StringBuilder();
                    foreach (IElement element in section.Title) {
                        if (element.Type == Element.CHUNK) {
                            titleText.Append(((Chunk) element).Content);
                        }
                    }
                    if (titleText.ToString().Trim().Length > 0) {
                        FD.RtfTOCEntry tocEntry = new FD.RtfTOCEntry(titleText.ToString());
                        tocEntry.SetRtfDocument(this.document);
                        this.items.Add(tocEntry);
                    }
                }
                foreach (IElement element in section) {
                    IRtfBasicElement[] rtfElements = doc.GetMapper().MapElement(element);
                    for (int i = 0; i < rtfElements.Length; i++) {
                        if (rtfElements[i] != null) {
                            items.Add(rtfElements[i]);
                        }
                    }
                }
                UpdateIndentation(section.IndentationLeft, section.IndentationRight, section.Indentation);
            } catch (DocumentException) {
            }
        }
        
        /**
        * Write this RtfSection and its contents
        */    
        public override void WriteContent(Stream result) {
            result.Write(RtfParagraph.PARAGRAPH, 0, RtfParagraph.PARAGRAPH.Length);
            if (this.title != null) {
                this.title.WriteContent(result);
            }
            foreach (IRtfBasicElement rbe in items) {
                rbe.WriteContent(result);
            }
        }
        
        /**
        * Sets whether this RtfSection is in a table. Sets the correct inTable setting for all
        * child elements.
        * 
        * @param inTable <code>True</code> if this RtfSection is in a table, <code>false</code> otherwise
        */
        public override void SetInTable(bool inTable) {
            base.SetInTable(inTable);
            for (int i = 0; i < this.items.Count; i++) {
                ((IRtfBasicElement) this.items[i]).SetInTable(inTable);
            }
        }
        
        /**
        * Sets whether this RtfSection is in a header. Sets the correct inTable setting for all
        * child elements.
        * 
        * @param inHeader <code>True</code> if this RtfSection is in a header, <code>false</code> otherwise
        */
        public override void SetInHeader(bool inHeader) {
            base.SetInHeader(inHeader);
            for (int i = 0; i < this.items.Count; i++) {
                ((IRtfBasicElement) this.items[i]).SetInHeader(inHeader);
            }
        }

        /**
        * Updates the left, right and content indentation of all RtfParagraph and RtfSection
        * elements that this RtfSection contains.
        * 
        * @param indentLeft The left indentation to add.
        * @param indentRight The right indentation to add.
        * @param indentContent The content indentation to add.
        */
        private void UpdateIndentation(float indentLeft, float indentRight, float indentContent) {
            if(this.title != null) {
                this.title.SetIndentLeft((int) (this.title.GetIndentLeft() + indentLeft * RtfElement.TWIPS_FACTOR));
                this.title.SetIndentRight((int) (this.title.GetIndentRight() + indentRight * RtfElement.TWIPS_FACTOR));
            }
            for(int i = 0; i < this.items.Count; i++) {
                IRtfBasicElement rtfElement = (IRtfBasicElement) this.items[i];
                if(rtfElement is RtfSection) {
                    ((RtfSection) rtfElement).UpdateIndentation(indentLeft + indentContent, indentRight, 0);
                } else if(rtfElement is RtfParagraph) {
                    ((RtfParagraph) rtfElement).SetIndentLeft((int) (((RtfParagraph) rtfElement).GetIndentLeft() + (indentLeft + indentContent) * RtfElement.TWIPS_FACTOR));
                    ((RtfParagraph) rtfElement).SetIndentRight((int) (((RtfParagraph) rtfElement).GetIndentRight() + indentRight * RtfElement.TWIPS_FACTOR));
                }
            }
        }
    }
}