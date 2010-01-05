using System;
using System.Collections;
using iTextSharp.text;
using iTextSharp.text.rtf.document;
using iTextSharp.text.rtf.list;
using iTextSharp.text.rtf.style;
/*
 * $Id: RtfImportMgr.cs,v 1.3 2008/05/16 19:31:07 psoares33 Exp $
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
namespace iTextSharp.text.rtf.parser {

    /**
    * The RtfImportHeader stores the docment header information from
    * an RTF document that is being imported. Currently font and
    * color settings are stored. The RtfImportHeader maintains a mapping
    * from font and color numbers from the imported RTF document to
    * the RTF document that is the target of the import. This guarantees
    * that the merged document has the correct font and color settings.
    * It also handles other list based items that need mapping, for example
    * stylesheets and lists.
    * 
    * @author Mark Hall (Mark.Hall@mail.room3b.eu)
    * @author Howard Shank (hgshank@yahoo.com)
    */
    public class RtfImportMgr {
        //TODO: Add list, stylesheet, info, etc. mappings
        /**
        * The Hashtable storing the font number mappings.
        */
        private Hashtable importFontMapping = null;
        /**
        * The Hashtable storing the color number mapings.
        */
        private Hashtable importColorMapping = null;
        /**
        * The Hashtable storing the Stylesheet List number mapings.
        */
        private Hashtable importStylesheetListMapping = null;
        /**
        * The Hashtable storing the List number mapings.
        */
        private Hashtable importListMapping = null;
        /**
        * The RtfDocument to get font and color numbers from.
        */
        private RtfDocument rtfDoc = null;
        /**
        * The Document.
        * Used for conversions, but not imports.
        */
        private Document doc = null;


        /**
        * Constructs a new RtfImportHeader.
        * 
        * @param rtfDoc The RtfDocument to get font and color numbers from.
        */
        public RtfImportMgr(RtfDocument rtfDoc, Document doc) {
            this.rtfDoc = rtfDoc;
            this.doc = doc;
            this.importFontMapping = new Hashtable();
            this.importColorMapping = new Hashtable();
            this.importStylesheetListMapping = new Hashtable();
            this.importListMapping = new Hashtable();
        }

        /**
        * Imports a font. The font name is looked up in the RtfDocumentHeader and
        * then the mapping from original font number to actual font number is added.
        * 
        * @param fontNr The original font number.
        * @param fontName The font name to look up.
        */
        public bool ImportFont(String fontNr, String fontName) {
            RtfFont rtfFont = new RtfFont(fontName);
            if (rtfFont != null){
                rtfFont.SetRtfDocument(this.rtfDoc);
                this.importFontMapping[fontNr] = this.rtfDoc.GetDocumentHeader().GetFontNumber(rtfFont).ToString();
                return true;
            } else {
                return false;
            }
        }
        /**
        * Imports a font. The font name is looked up in the RtfDocumentHeader and
        * then the mapping from original font number to actual font number is added.
        * 
        * @param fontNr The original font number.
        * @param fontName The font name to look up.
        * @param charset The characterset to use for the font.
        */
        public bool ImportFont(String fontNr, String fontName, int charset) {
            RtfFont rtfFont = new RtfFont(fontName);
            if (charset>= 0)
                rtfFont.SetCharset(charset);
            if (rtfFont != null){
                rtfFont.SetRtfDocument(this.rtfDoc);
                this.importFontMapping[fontNr] = this.rtfDoc.GetDocumentHeader().GetFontNumber(rtfFont).ToString();
                return true;
            } else {
                return false;
            }
        }
        /**
        * Imports a font. The font name is looked up in the RtfDocumentHeader and
        * then the mapping from original font number to actual font number is added.
        * 
        * @param fontNr The original font number.
        * @param fontName The font name to look up.
        * @param charset The characterset to use for the font.
        */
        public bool ImportFont(String fontNr, String fontName, String fontFamily, int charset) {
            RtfFont rtfFont = new RtfFont(fontName);

            if (charset>= 0)
                rtfFont.SetCharset(charset);
            if (fontFamily != null && fontFamily.Length > 0)
                rtfFont.SetFamily(fontFamily);
            if (rtfFont != null){
                rtfFont.SetRtfDocument(this.rtfDoc);
                this.importFontMapping[fontNr] = this.rtfDoc.GetDocumentHeader().GetFontNumber(rtfFont).ToString();
                return true;
            } else {
                return false;
            }
        }
        /**
        * Performs the mapping from the original font number to the actual
        * font number in the resulting RTF document. If the font number was not
        * seen during import (thus no mapping) then 0 is returned, guaranteeing
        * that the font number is always valid.
        * 
        * @param fontNr The font number to map.
        * @return The mapped font number.
        */
        public String MapFontNr(String fontNr) {
            if (this.importFontMapping.ContainsKey(fontNr)) {
                return (String) this.importFontMapping[fontNr];
            } else {
                return "0";
            }
        }

        /**
        * Imports a color value. The color number for the color defined
        * by its red, green and blue values is determined and then the
        * resulting mapping is added.
        * 
        * @param colorNr The original color number.
        * @param color The color to import.
        */
        public void ImportColor(String colorNr, Color color) {
            RtfColor rtfColor = new RtfColor(this.rtfDoc, color);
            this.importColorMapping[colorNr] = rtfColor.GetColorNumber().ToString();
        }

        /**
        * Performs the mapping from the original font number to the actual font
        * number used in the RTF document. If the color number was not
        * seen during import (thus no mapping) then 0 is returned, guaranteeing
        * that the color number is always valid.
        * 
        * @param colorNr The color number to map.
        * @return The mapped color number
        */
        public String MapColorNr(String colorNr) {
            if (this.importColorMapping.ContainsKey(colorNr)) {
                return (String) this.importColorMapping[colorNr];
            } else {
                return "0";
            }
        }

        /**
        * Imports a List value. The List number for the List defined
        * is determined and then the resulting mapping is added.
        */
        public void ImportList(String listNr, List list) {
            RtfList rtfList = new RtfList(this.rtfDoc, list);

            //if(rtfList != null){
            //rtfList.SetRtfDocument(this.rtfDoc);
            this.importStylesheetListMapping[listNr] = this.rtfDoc.GetDocumentHeader().GetListNumber(rtfList).ToString();
    //      return true;
    //      } else {
    //      return false;
    //      }
        }

        /**
        * Performs the mapping from the original list number to the actual
        * list number in the resulting RTF document. If the list number was not
        * seen during import (thus no mapping) then 0 is returned, guaranteeing
        * that the list number is always valid.
        */
        public String MapListNr(String listNr) {
            if (this.importListMapping.ContainsKey(listNr)) {
                return (String) this.importListMapping[listNr];
            } else {
                return "0";
            }
        }

        /**
        * Imports a stylesheet list value. The stylesheet number for the stylesheet defined
        * is determined and then the resulting mapping is added.
        */
        public bool ImportStylesheetList(String listNr, List listIn) {
            RtfList rtfList = new RtfList(this.rtfDoc, listIn);

            if (rtfList != null){
                rtfList.SetRtfDocument(this.rtfDoc);
                //this.importStylesheetListMapping[listNr] = Integer.ToString(this.rtfDoc.GetDocumentHeader().GetRtfParagraphStyle(styleName)(rtfList));
                return true;
            } else {
                return false;
            }
        }
        /**
        * Performs the mapping from the original stylesheet number to the actual
        * stylesheet number in the resulting RTF document. If the stylesheet number was not
        * seen during import (thus no mapping) then 0 is returned, guaranteeing
        * that the stylesheet number is always valid.
        */
        public String MapStylesheetListNr(String listNr) {
            if (this.importStylesheetListMapping.ContainsKey(listNr)) {
                return (String) this.importStylesheetListMapping[listNr];
            } else {
                return "0";
            }
        }

    }
}