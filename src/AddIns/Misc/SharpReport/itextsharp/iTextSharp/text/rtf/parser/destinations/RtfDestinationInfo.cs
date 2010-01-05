using System;
using System.Collections;
using iTextSharp.text;
using iTextSharp.text.rtf.document;
using iTextSharp.text.rtf.parser;
using iTextSharp.text.rtf.parser.ctrlwords;
/*
 * $Id: RtfDestinationInfo.cs,v 1.2 2008/05/13 11:26:00 psoares33 Exp $
 * 
 *
 * Copyright 2007 by Howard Shank (hgshank@yahoo.com)
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
 * the Initial Developer are Copyright (C) 1999-2006 by Bruno Lowagie.
 * All Rights Reserved.
 * Co-Developer of the code is Paulo Soares. Portions created by the Co-Developer
 * are Copyright (C) 2000-2006 by Paulo Soares. All Rights Reserved.
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
 
namespace iTextSharp.text.rtf.parser.destinations {

    /**
    * <code>RtfDestinationInfo</code> handles data destined for the info destination
    * 
    * @author Howard Shank (hgshank@yahoo.com)
    * @since 2.0.8
    */
    public class RtfDestinationInfo : RtfDestination {
        private String elementName = "";
        private String text = "";

        
        public RtfDestinationInfo() : base(null) {
        }
        /**
        * Constructs a new RtfDestinationInfo.
        * 
        * @param parser The RtfParser object.
        */
        public RtfDestinationInfo(RtfParser parser, String elementname) : base(parser) {
            SetToDefaults();
            this.elementName = elementname;
        }
        public override void SetParser(RtfParser parser) {
            this.rtfParser = parser;
            this.SetToDefaults();
        }
        public void SetElementName(String value) {
            this.elementName = value;
        }
        /* (non-Javadoc)
        * @see com.lowagie.text.rtf.parser.destinations.RtfDestination#handleOpenNewGroup()
        */
        public override bool HandleOpeningSubGroup() {
            return true;
        }

        /* (non-Javadoc)
        * @see com.lowagie.text.rtf.direct.RtfDestination#closeDestination()
        */
        public override bool CloseDestination() {
            return true;
        }

        /* (non-Javadoc)
        * @see com.lowagie.text.rtf.direct.RtfDestination#handleGroupEnd()
        */
        public override bool HandleCloseGroup() {
            if (this.text.Length > 0) {       
                Document doc = this.rtfParser.GetDocument();
                if (doc != null) {
                    if (this.elementName.Equals("author")){
                        doc.AddAuthor(this.text);
                    }
                    if (this.elementName.Equals("title")){
                        doc.AddTitle(this.text);
                    }
                    if (this.elementName.Equals("subject")){
                        doc.AddSubject(this.text);
                    }
                } else {
                    RtfDocument rtfDoc = this.rtfParser.GetRtfDocument();
                    if (rtfDoc != null) {
                        if (this.elementName.Equals("author")){
                            Meta meta = new Meta(this.elementName, this.text);
                            RtfInfoElement elem = new RtfInfoElement(rtfDoc, meta);
                            rtfDoc.GetDocumentHeader().AddInfoElement(elem);
                        }
                        if (this.elementName.Equals("title")){
                            Meta meta = new Meta(this.elementName, this.text);
                            RtfInfoElement elem = new RtfInfoElement(rtfDoc, meta);
                            rtfDoc.GetDocumentHeader().AddInfoElement(elem);
                        }
                        if (this.elementName.Equals("subject")){
                            Meta meta = new Meta(this.elementName, this.text);
                            RtfInfoElement elem = new RtfInfoElement(rtfDoc, meta);
                            rtfDoc.GetDocumentHeader().AddInfoElement(elem);
                        }
                    }
                }
                this.SetToDefaults();
            }
            return true;
        }

        /* (non-Javadoc)
        * @see com.lowagie.text.rtf.direct.RtfDestination#handleGroupStart()
        */
        public override bool HandleOpenGroup() {

            return true;
        }
        /* (non-Javadoc)
        * @see com.lowagie.text.rtf.direct.RtfDestination#handleCharacter(char[])
        */
        public override bool HandleCharacter(int ch) {
            this.text += (char)ch;
            return true;
        }

        /* (non-Javadoc)
        * @see com.lowagie.text.rtf.parser.destinations.RtfDestination#handleControlWord(com.lowagie.text.rtf.parser.ctrlwords.RtfCtrlWordData)
        */
        public override bool HandleControlWord(RtfCtrlWordData ctrlWordData) {
            elementName = ctrlWordData.ctrlWord;
            return true;
        }

        /* (non-Javadoc)
        * @see com.lowagie.text.rtf.parser.destinations.RtfDestination#setToDefaults()
        */
        public override void SetToDefaults() {
            this.text = "";
        }

    }
}