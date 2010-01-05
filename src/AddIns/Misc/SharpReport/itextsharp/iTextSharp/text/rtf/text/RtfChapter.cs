using System;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.rtf;
using iTextSharp.text.rtf.document;
/*
 * $Id: RtfChapter.cs,v 1.7 2008/05/16 19:31:23 psoares33 Exp $
 * 
 *
 * Copyright 2001, 2002 by Mark Hall
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
    * The RtfChapter wraps a Chapter element.
    * INTERNAL CLASS
    * 
    * @version $Version:$
    * @author Mark Hall (Mark.Hall@mail.room3b.eu)
    */
    public class RtfChapter : RtfSection {

        /**
        * Constructs a RtfChapter for a given Chapter
        * 
        * @param doc The RtfDocument this RtfChapter belongs to
        * @param chapter The Chapter this RtfChapter is based on
        */
        public RtfChapter(RtfDocument doc, Chapter chapter) : base(doc, chapter) {
        }
        
        /**
        * Writes the RtfChapter and its contents
        */    
        public override void WriteContent(Stream result) {
            byte[] t;
            if (this.document.GetLastElementWritten() != null && !(this.document.GetLastElementWritten() is RtfChapter)) {
                result.Write(t = DocWriter.GetISOBytes("\\page"), 0, t.Length);
            }
            result.Write(t = DocWriter.GetISOBytes("\\sectd"), 0, t.Length);
            document.GetDocumentHeader().WriteSectionDefinition(result);
            if (this.title != null) {
                this.title.WriteContent(result);
            }
            for (int i = 0; i < items.Count; i++) {
                IRtfBasicElement rbe = (IRtfBasicElement)items[i];
                rbe.WriteContent(result);
            }
            result.Write(t = DocWriter.GetISOBytes("\\sect"), 0, t.Length);
        }
    }
}