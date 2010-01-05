using System;
using System.IO;
using System.Collections;
using iTextSharp.text;
using iTextSharp.text.rtf;
/*
 * $Id: RtfInfoGroup.cs,v 1.6 2008/05/16 19:30:51 psoares33 Exp $
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
    * The RtfInfoGroup stores information group elements. 
    * 
    * @version $Id: RtfInfoGroup.cs,v 1.6 2008/05/16 19:30:51 psoares33 Exp $
    * @author Mark Hall (Mark.Hall@mail.room3b.eu)
    * @author Thomas Bickel (tmb99@inode.at)
    */
    public class RtfInfoGroup : RtfElement {
        /**
        * Information group starting tag
        */
        private static byte[] INFO_GROUP = DocWriter.GetISOBytes("\\info");
        
        /**
        * Constant for the password element
        * @since 2.1.1
        * @author Howard Shank (hgshank@yahoo.com)
        */
        private static byte[] INFO_PASSWORD = DocWriter.GetISOBytes("\\*\\password");

        /**
        * The RtfInfoElements that belong to this RtfInfoGroup
        */
        ArrayList infoElements = null;
        
        /**
        * Constructs a RtfInfoGroup belonging to a RtfDocument
        * 
        * @param doc The RtfDocument this RtfInfoGroup belongs to
        */
        public RtfInfoGroup(RtfDocument doc) : base(doc) {
            infoElements = new ArrayList();
        }
        
        /**
        * Adds an RtfInfoElement to the RtfInfoGroup
        * 
        * @param infoElement The RtfInfoElement to add
        */
        public void Add(RtfInfoElement infoElement) {
            this.infoElements.Add(infoElement);
        }
        
        /**
        * Writes the RTF information group and its elements.
        */    
        public override void WriteContent(Stream result) {
            result.Write(OPEN_GROUP, 0, OPEN_GROUP.Length);
            result.Write(INFO_GROUP, 0, INFO_GROUP.Length);
            for (int i = 0; i < infoElements.Count; i++) {
                RtfInfoElement infoElement = (RtfInfoElement) infoElements[i];
                infoElement.WriteContent(result);
            }
            
            // handle document protection
            if (document.GetDocumentSettings().IsDocumentProtected()) {
                byte[] t;
                result.Write(OPEN_GROUP, 0, OPEN_GROUP.Length);
                result.Write(INFO_PASSWORD, 0, INFO_PASSWORD.Length);
                result.Write(DELIMITER, 0, DELIMITER.Length);
                result.Write(t = document.GetDocumentSettings().GetProtectionHashBytes(), 0, t.Length);
                result.Write(CLOSE_GROUP, 0, CLOSE_GROUP.Length);
            }
            result.Write(CLOSE_GROUP, 0, CLOSE_GROUP.Length);
            result.WriteByte((byte)'\n');
        }
    }
}