using System;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.rtf;
/*
 * $Id: RtfInfoElement.cs,v 1.6 2008/05/16 19:30:51 psoares33 Exp $
 * 
 *
 * Copyright 2003, 2004 by Mark Hall
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

namespace iTextSharp.text.rtf.document {

    /**
    * Stores one information group element. Valid elements are
    * author, title, subject, keywords, producer and creationdate.
    * 
    * @version $Id: RtfInfoElement.cs,v 1.6 2008/05/16 19:30:51 psoares33 Exp $
    * @author Mark Hall (Mark.Hall@mail.room3b.eu)
    * @author Thomas Bickel (tmb99@inode.at)
    */
    public class RtfInfoElement : RtfElement {

        /**
        * Constant for the author element
        */
        private static byte[] INFO_AUTHOR = DocWriter.GetISOBytes("\\author");
        /**
        * Constant for the subject element
        */
        private static byte[] INFO_SUBJECT = DocWriter.GetISOBytes("\\subject");
        /**
        * Constant for the keywords element
        */
        private static byte[] INFO_KEYWORDS = DocWriter.GetISOBytes("\\keywords");
        /**
        * Constant for the title element
        */
        private static byte[] INFO_TITLE = DocWriter.GetISOBytes("\\title");
        /**
        * Constant for the producer element
        */
        private static byte[] INFO_PRODUCER = DocWriter.GetISOBytes("\\operator");
        /**
        * Constant for the creationdate element
        */
        private static byte[] INFO_CREATION_DATE = DocWriter.GetISOBytes("\\creationdate");

        /**
        * The type of this RtfInfoElement. The values from Element.INFO_ELEMENT_NAME are used.
        */
        private int infoType = -1;
        /**
        * The content of this RtfInfoElement
        */
        private String content = "";
        
        /**
        * Constructs a RtfInfoElement based on the given Meta object
        * 
        * @param doc The RtfDocument this RtfInfoElement belongs to
        * @param meta The Meta object this RtfInfoElement is based on
        */
        public RtfInfoElement(RtfDocument doc, Meta meta) : base(doc) {
            infoType = meta.Type;
            content = meta.Content;
        }
        
        /**
        * Writes the content of one RTF information element.
        */    
        public override void WriteContent(Stream result) {
            result.Write(OPEN_GROUP, 0, OPEN_GROUP.Length);
            switch (infoType) {
                case Element.AUTHOR:
                    result.Write(INFO_AUTHOR, 0, INFO_AUTHOR.Length);
                    break;
                case Element.SUBJECT:
                    result.Write(INFO_SUBJECT, 0, INFO_SUBJECT.Length);
                    break;
                case Element.KEYWORDS:
                    result.Write(INFO_KEYWORDS, 0, INFO_KEYWORDS.Length);
                    break;
                case Element.TITLE:
                    result.Write(INFO_TITLE, 0, INFO_TITLE.Length);
                    break;
                case Element.PRODUCER:
                    result.Write(INFO_PRODUCER, 0, INFO_PRODUCER.Length);
                    break;
                case Element.CREATIONDATE:
                    result.Write(INFO_CREATION_DATE, 0, INFO_CREATION_DATE.Length);
                    break;
                default:
                    result.Write(INFO_AUTHOR, 0, INFO_AUTHOR.Length);
                    break;
            }
            result.Write(DELIMITER, 0, DELIMITER.Length);
            byte[] t;
            if (infoType == Element.CREATIONDATE) {
                t = DocWriter.GetISOBytes(ConvertDate(content));
                result.Write(t, 0, t.Length);
            } else {
                document.FilterSpecialChar(result, content, false, false);
            }
            result.Write(CLOSE_GROUP, 0, CLOSE_GROUP.Length);
        }
        
        /**
        * Converts a date from the format used by iText to the format required by
        * rtf.<br>iText: EEE MMM dd HH:mm:ss zzz yyyy - rtf: \\'yr'yyyy\\'mo'MM\\'dy'dd\\'hr'HH\\'min'mm\\'sec'ss
        * 
        * @param date The date formated by iText
        * @return The date formated for rtf
        */
        private String ConvertDate(String date) {
            DateTime d;
            try {
                d = DateTime.Parse(date);
            } catch {
                d = DateTime.Now;
            }
            return d.ToString("'\\\\yr'yyyy'\\\\mo'MM'\\\\dy'dd'\\\\hr'HH'\\\\min'mm'\\\\sec'ss");
        }
    }
}