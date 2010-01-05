using System;
using System.IO;
using iTextSharp.text;
/*
 * $Id: RtfTOCEntry.cs,v 1.6 2008/05/23 17:24:26 psoares33 Exp $
 * 
 *
 * Copyright 2004 by Mark Hall
 * Uses code Copyright 2002
 *   <a href="mailto:Steffen.Stundzig@smb-tec.com">Steffen.Stundzig@smb-tec.com</a> 
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

namespace iTextSharp.text.rtf.field {

    /**
    * The RtfTOCEntry is used together with the RtfTableOfContents to generate a table of
    * contents. Add the RtfTOCEntry in those locations in the document where table of
    * contents entries should link to 
    * 
    * @version $Version:$
    * @author Mark Hall (Mark.Hall@mail.room3b.eu)
    * @author <a href="mailto:Steffen.Stundzig@smb-tec.com">Steffen.Stundzig@smb-tec.com</a> 
    */
    public class RtfTOCEntry : RtfField {

        /**
        * Constant for the beginning of hidden text
        */
        private static byte[] TEXT_HIDDEN_ON = DocWriter.GetISOBytes("\\v");
        /**
        * Constant for the end of hidden text
        */
        private static byte[] TEXT_HIDDEN_OFF = DocWriter.GetISOBytes("\\v0");
        /**
        * Constant for a TOC entry with page numbers
        */
        private static byte[] TOC_ENTRY_PAGE_NUMBER = DocWriter.GetISOBytes("\\tc");
        /**
        * Constant for a TOC entry without page numbers
        */
        private static byte[] TOC_ENTRY_NO_PAGE_NUMBER = DocWriter.GetISOBytes("\\tcn");
        
        /**
        * The entry text of this RtfTOCEntry
        */
        private String entry = "";
        /**
        * Whether to show page numbers in the table of contents
        */
        private bool showPageNumber = true;
        
        /**
        * Constructs a RtfTOCEntry with a certain entry text.
        * 
        * @param entry The entry text to display
        * @param font The Font to use
        */
        public RtfTOCEntry(String entry) : base(null, new Font()) {
            if (entry != null) {
                this.entry = entry;
            }
        }
        
        /**
        * Writes the content of the RtfTOCEntry
        */ 
        public override void WriteContent(Stream result) {       
            result.Write(TEXT_HIDDEN_ON, 0, TEXT_HIDDEN_ON.Length);
            result.Write(OPEN_GROUP, 0, OPEN_GROUP.Length);
            if (this.showPageNumber) {
                result.Write(TOC_ENTRY_PAGE_NUMBER, 0, TOC_ENTRY_PAGE_NUMBER.Length);
            } else {
                result.Write(TOC_ENTRY_NO_PAGE_NUMBER, 0, TOC_ENTRY_NO_PAGE_NUMBER.Length);
            }
            result.Write(DELIMITER, 0, DELIMITER.Length);
            this.document.FilterSpecialChar(result, this.entry, true, false);
            result.Write(CLOSE_GROUP, 0, CLOSE_GROUP.Length);
            result.Write(TEXT_HIDDEN_OFF, 0, TEXT_HIDDEN_OFF.Length);
        }
        
        /**
        * Sets whether to display a page number in the table of contents, or not
        * 
        * @param showPageNumber Whether to display a page number or not
        */
        public void SetShowPageNumber(bool showPageNumber) {
            this.showPageNumber = showPageNumber;
        }
        
        /**
        * unused
        */
        protected override void WriteFieldInstContent(Stream outp) {
        }

        /*
        * unused
        * @see com.lowagie.text.rtf.field.RtfField#writeFieldResultContent(java.io.OutputStream)
        */
        protected override void WriteFieldResultContent(Stream outp) {
        }
    }
}