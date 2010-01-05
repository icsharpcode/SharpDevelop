using System;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.rtf;
using iTextSharp.text.rtf.document;
using iTextSharp.text.rtf.text;
using iTextSharp.text.rtf.style;
/*
 * $Id: RtfListItem.cs,v 1.7 2008/05/16 19:31:02 psoares33 Exp $
 * 
 *
 * Copyright 2001, 2002, 2003, 2004, 2005 by Mark Hall
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

namespace iTextSharp.text.rtf.list {

    /**
    * The RtfListItem acts as a wrapper for a ListItem.
    * 
    * @version $Version:$
    * @author Mark Hall (Mark.Hall@mail.room3b.eu)
    */
    public class RtfListItem : RtfParagraph {

        /**
        * The RtfList this RtfListItem belongs to.
        */
        private RtfList parentList = null;
        /**
        * Whether this RtfListItem contains further RtfLists.
        */
        private bool containsInnerList = false;
        
        /**
        * Constructs a RtfListItem for a ListItem belonging to a RtfDocument.
        * 
        * @param doc The RtfDocument this RtfListItem belongs to.
        * @param listItem The ListItem this RtfListItem is based on.
        */
        public RtfListItem(RtfDocument doc, ListItem listItem) : base(doc, listItem) {
        }
        
        /**
        * Writes the content of this RtfListItem.
        */    
        public override void WriteContent(Stream result) {
            byte[] t;
            if (this.paragraphStyle.GetSpacingBefore() > 0) {
                result.Write(RtfParagraphStyle.SPACING_BEFORE, 0, RtfParagraphStyle.SPACING_BEFORE.Length);
                result.Write(t = IntToByteArray(paragraphStyle.GetSpacingBefore()), 0, t.Length);
            }
            if (this.paragraphStyle.GetSpacingAfter() > 0) {
                result.Write(RtfParagraphStyle.SPACING_AFTER, 0, RtfParagraphStyle.SPACING_AFTER.Length);
                result.Write(t = IntToByteArray(this.paragraphStyle.GetSpacingAfter()), 0, t.Length);
            }
            for (int i = 0; i < chunks.Count; i++) {
                IRtfBasicElement rtfElement = (IRtfBasicElement) chunks[i];
                if (rtfElement is RtfChunk) {
                    ((RtfChunk) rtfElement).SetSoftLineBreaks(true);
                } else if (rtfElement is RtfList) {
                    result.Write(RtfParagraph.PARAGRAPH, 0, RtfParagraph.PARAGRAPH.Length);
                    this.containsInnerList = true;
                }
                rtfElement.WriteContent(result);
                if (rtfElement is RtfList) {
                    this.parentList.WriteListBeginning(result);
                    result.Write(t = DocWriter.GetISOBytes("\\tab"), 0, t.Length);
                }
            }
        }

        /**
        * Writes the definition of the first element in this RtfListItem that is
        * an instanceof {@link RtfList} to the given stream.<br> 
        * If this item does not contain a {@link RtfList} element nothing is written
        * and the method returns <code>false</code>.
        * 
        * @param out destination stream
        * @return <code>true</code> if a RtfList definition was written, <code>false</code> otherwise
        * @throws IOException
        * @see {@link RtfList#writeDefinition(OutputStream)}
        */
        public bool WriteDefinition(Stream outp) {
            for (int i = 0; i < chunks.Count; i++) {
                IRtfBasicElement rtfElement = (IRtfBasicElement)chunks[i];
                if (rtfElement is RtfList) {
                    RtfList rl = (RtfList)rtfElement;
                    rl.WriteDefinition(outp);
                    return true;
                }
            }
            return false;
        }

        /**
        * Inherit the list settings from the parent list to RtfLists that
        * are contained in this RtfListItem.
        * 
        * @param listNumber The list number to inherit.
        * @param listLevel The list level to inherit.
        */
        public void InheritListSettings(int listNumber, int listLevel) {
            for (int i = 0; i < chunks.Count; i++) {
                IRtfBasicElement rtfElement = (IRtfBasicElement) chunks[i];
                if (rtfElement is RtfList) {
                    ((RtfList) rtfElement).SetListNumber(listNumber);
                    ((RtfList) rtfElement).SetListLevel(listLevel);
                    ((RtfList) rtfElement).SetParent(this.parentList);
                }
            }
        }
            
        /**
        * Correct the indentation of RtfLists in this RtfListItem by adding left/first line indentation
        * from the parent RtfList. Also calls correctIndentation on all child RtfLists.
        */
        protected internal void CorrectIndentation() {
            for (int i = 0; i < chunks.Count; i++) {
                IRtfBasicElement rtfElement = (IRtfBasicElement) chunks[i];
                if (rtfElement is RtfList) {
                    ((RtfList) rtfElement).CorrectIndentation();
                }
            }
        }
        
        /**
        * Set the parent RtfList.
        * 
        * @param parentList The parent RtfList to use.
        */
        public void SetParent(RtfList parentList) {
            this.parentList = parentList;
        }

        /**
        * Gets whether this RtfListItem contains further RtfLists.
        * 
        * @return Whether this RtfListItem contains further RtfLists.
        */
        public bool IsContainsInnerList() {
            return this.containsInnerList;
        }
    }
}