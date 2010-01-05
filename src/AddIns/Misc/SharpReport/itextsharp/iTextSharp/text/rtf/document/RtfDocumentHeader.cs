using System;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.rtf;
using iTextSharp.text.rtf.document.output;
using iTextSharp.text.rtf.list;
using ST = iTextSharp.text.rtf.style;
using iTextSharp.text.rtf.style;
using HF = iTextSharp.text.rtf.headerfooter;
using iTextSharp.text.rtf.headerfooter;
//using iTextSharp.text.rtf;
/*
 * $Id: RtfDocumentHeader.cs,v 1.11 2008/05/16 19:30:51 psoares33 Exp $
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
    * The RtfDocumentHeader contains all classes required for the generation of
    * the document header area.
    * 
    * @version $Id: RtfDocumentHeader.cs,v 1.11 2008/05/16 19:30:51 psoares33 Exp $
    * @author Mark Hall (Mark.Hall@mail.room3b.eu)
    * @author Thomas Bickel (tmb99@inode.at)
    */
    public class RtfDocumentHeader : RtfElement {
        /**
        * Constant for the title page
        */
        private static byte[] TITLE_PAGE = DocWriter.GetISOBytes("\\titlepg");
        /**
        * Constant for facing pages
        */
        private static byte[] FACING_PAGES = DocWriter.GetISOBytes("\\facingp");
        
        /**
        * The code page to use
        */
        private RtfCodePage codePage = null;
        /**
        * Stores all the colors used in the document
        */
        private RtfColorList colorList = null;
        /**
        * Stores all the fonts used in the document
        */
        private RtfFontList fontList = null;
        /**
        * Manages List tables
        */
        private RtfListTable listTable = null;
        /**
        * Stores all paragraph styles used in the document.
        */
        private RtfStylesheetList stylesheetList = null;
        /**
        * Generator string in document
        */
        private RtfGenerator generator = null;
        /**
        * The information group with author/subject/keywords/title/producer/creationdate data
        */
        private RtfInfoGroup infoGroup = null;
        /**
        * The protection settings
        * @since 2.1.1
        * @author Howard Shank (hgshank@yahoo.com)
        */
        private RtfProtectionSetting protectionSetting = null;
        /**
        * The page settings
        */
        private RtfPageSetting pageSetting = null;
        /**
        * The current RtfHeaderFooterGroup for the header
        */
        private HeaderFooter header = null;
        /**
        * The current RtfHeaderFooterGroup for the footer
        */
        private HeaderFooter footer = null;

        /**
        * Constructs a RtfDocumentHeader for a RtfDocument
        * 
        * @param doc The RtfDocument this RtfDocumentHeader belongs to
        */
        protected internal RtfDocumentHeader(RtfDocument doc) : base(doc)  {
        }

        /**
        * Initialises the RtfDocumentHeader.
        */
        protected internal void Init() {
            this.codePage = new RtfCodePage(this.document);
            this.colorList = new RtfColorList(this.document);
            this.fontList = new RtfFontList(this.document);
            this.listTable = new RtfListTable(this.document);
            this.stylesheetList = new RtfStylesheetList(this.document);
            this.infoGroup = new RtfInfoGroup(this.document);
            this.protectionSetting = new RtfProtectionSetting(this.document);
            this.pageSetting = new RtfPageSetting(this.document);
            this.header = new RtfHeaderFooterGroup(this.document, HF.RtfHeaderFooter.TYPE_HEADER);
            this.footer = new RtfHeaderFooterGroup(this.document, HF.RtfHeaderFooter.TYPE_FOOTER);
            this.generator = new RtfGenerator(this.document);
        }
        
        /**
        * Write the contents of the document header area.
        */    
        public override void WriteContent(Stream result) {
            try {
                // This is so that all colour, font and similar information is processed once, before
                // the header section is written.
                WriteSectionDefinition(new RtfNilOutputStream());
                
                this.codePage.WriteDefinition(result);
                this.fontList.WriteDefinition(result);
                this.colorList.WriteDefinition(result);
                this.stylesheetList.WriteDefinition(result);
                this.listTable.WriteDefinition(result);
                this.generator.WriteContent(result);
                this.infoGroup.WriteContent(result);
                this.protectionSetting.WriteDefinition(result);
                this.pageSetting.WriteDefinition(result);
                
                WriteSectionDefinition(result);
            } catch (IOException) {
            }
        }        

        /**
        * Writes the section definition data
        * @param result
        */
        public void WriteSectionDefinition(Stream result) {
            try {
                RtfHeaderFooterGroup header = ConvertHeaderFooter(this.header, HF.RtfHeaderFooter.TYPE_HEADER);
                RtfHeaderFooterGroup footer = ConvertHeaderFooter(this.footer, HF.RtfHeaderFooter.TYPE_FOOTER);
                if (header.HasTitlePage() || footer.HasTitlePage()) {
                    result.Write(TITLE_PAGE, 0, TITLE_PAGE.Length);
                    header.SetHasTitlePage();
                    footer.SetHasTitlePage();
                }
                if (header.HasFacingPages() || footer.HasFacingPages()) {
                    result.Write(FACING_PAGES, 0, FACING_PAGES.Length);
                    header.SetHasFacingPages();
                    footer.SetHasFacingPages();
                }
                footer.WriteContent(result);
                header.WriteContent(result);
                pageSetting.WriteSectionDefinition(result);
            } catch (IOException) {
            }
        }
        
        /**
        * Gets the number of the specified RtfFont
        *
        * @param font The RtfFont for which to get the number
        * @return The number of the font
        */
        public int GetFontNumber(ST.RtfFont font) {
            return this.fontList.GetFontNumber(font);
        }

        /**
        * Gets the number of the specified RtfColor
        * 
        * @param color The RtfColor for which to get the number
        * @return The number of the color
        */
        public int GetColorNumber(ST.RtfColor color) {
            return this.colorList.GetColorNumber(color);
        }
        
        /**
        * Gets the number of the specified RtfList
        * 
        * @param list The RtfList for which to get the number
        * @return The number of the list
        */
        public int GetListNumber(RtfList list) {
            return this.listTable.GetListNumber(list);
        }
        
        /**
        * Gets the RtfParagraphStyle with the given style name.
        * 
        * @param styleName The style name of the RtfParagraphStyle to get. 
        * @return The RtfParagraphStyle with the given style name or null.
        */
        public RtfParagraphStyle GetRtfParagraphStyle(String styleName) {
            return this.stylesheetList.GetRtfParagraphStyle(styleName);
        }

        /**
        * Removes a RtfList from the list table
        * 
        * @param list The RtfList to remove
        */
        public void FreeListNumber(RtfList list) {
            this.listTable.FreeListNumber(list);
        }
        
        /**
        * Gets the RtfPageSetting object of this RtfDocument
        * 
        * @return The RtfPageSetting object
        */
        public RtfPageSetting GetPageSetting() {
            return this.pageSetting;
        }
        
        /**
        * Adds an RtfInfoElement to the list of RtfInfoElements
        * 
        * @param rtfInfoElement The RtfInfoElement to add
        */
        public void AddInfoElement(RtfInfoElement rtfInfoElement) {
            this.infoGroup.Add(rtfInfoElement);
        }
        
        /**
        * Sets the current header to use
        * 
        * @param header The HeaderFooter to use as header
        */
        public void SetHeader(HeaderFooter header) {
            this.header = header;
        }
        
        /**
        * Sets the current footer to use
        * 
        * @param footer The HeaderFooter to use as footer
        */
        public void SetFooter(HeaderFooter footer) {
            this.footer = footer;
        }

        /**
        * Registers the RtfParagraphStyle for further use in the document.
        * 
        * @param rtfParagraphStyle The RtfParagraphStyle to register.
        */
        public void RegisterParagraphStyle(RtfParagraphStyle rtfParagraphStyle) {
            this.stylesheetList.RegisterParagraphStyle(rtfParagraphStyle);
        }

        /**
        * Converts a HeaderFooter into a RtfHeaderFooterGroup. Depending on which class
        * the HeaderFooter is, the correct RtfHeaderFooterGroup is created.
        * 
        * @param hf The HeaderFooter to convert.
        * @param type Whether the conversion is being done on a footer or header
        * @return The converted RtfHeaderFooterGroup.
        * @see com.lowagie.text.rtf.headerfooter.RtfHeaderFooter
        * @see com.lowagie.text.rtf.headerfooter.RtfHeaderFooterGroup
        */
        private RtfHeaderFooterGroup ConvertHeaderFooter(HeaderFooter hf, int type) {
            if (hf != null) {
                if (hf is RtfHeaderFooterGroup) {
                    return new RtfHeaderFooterGroup(this.document, (RtfHeaderFooterGroup) hf, type);
                } else if (hf is RtfHeaderFooter) {
                    return new RtfHeaderFooterGroup(this.document, (RtfHeaderFooter) hf, type);
                } else {
                    return new RtfHeaderFooterGroup(this.document, hf, type);
                }
            } else {
                return new RtfHeaderFooterGroup(this.document, type);
            }
        }
    }
}