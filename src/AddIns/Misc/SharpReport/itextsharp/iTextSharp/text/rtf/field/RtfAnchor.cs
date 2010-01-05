using System;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.rtf.text;
using iTextSharp.text.rtf.document;
/*
 * $Id: RtfAnchor.cs,v 1.7 2008/05/16 19:30:54 psoares33 Exp $
 * 
 *
 * Copyright 2004 by Mark Hall
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
    * 
    * @version $Version:$
    * @author Mark Hall (Mark.Hall@mail.room3b.eu)
    */
    public class RtfAnchor : RtfField {

        /**
        * Constant for a hyperlink
        */
        private static byte[] HYPERLINK = DocWriter.GetISOBytes("HYPERLINK");
        
        /**
        * The url of this RtfAnchor
        */
        private String url = "";
        /**
        * The RtfPhrase to display for the url
        */
        private new RtfPhrase content = null;

        /**
        * Constructs a RtfAnchor based on a RtfField
        * 
        * @param doc The RtfDocument this RtfAnchor belongs to
        * @param anchor The Anchor this RtfAnchor is based on
        */
        public RtfAnchor(RtfDocument doc, Anchor anchor) : base(doc) {
            this.url = anchor.Reference;
            this.content = new RtfPhrase(doc, anchor);
        }
        
        /**
        * Write the field instructions for this RtfAnchor. Sets the field
        * type to HYPERLINK and then writes the url.
        * 
        * @return The field instructions for this RtfAnchor
        * @throws IOException
        */
        protected override void WriteFieldInstContent(Stream result) {
            result.Write(HYPERLINK, 0, HYPERLINK.Length);
            result.Write(DELIMITER, 0, DELIMITER.Length);
            this.document.FilterSpecialChar(result, url, true, true);
        }
        
        /**
        * Write the field result for this RtfAnchor. Writes the content
        * of the RtfPhrase.
        */
        protected override void WriteFieldResultContent(Stream outp) {
            content.WriteContent(outp);
        }
    }
}