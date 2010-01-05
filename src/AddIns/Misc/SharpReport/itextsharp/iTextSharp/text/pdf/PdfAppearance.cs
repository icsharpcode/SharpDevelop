using System;
using System.Collections;

/*
 * Copyright 2002 Paulo Soares
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
 * LGPL license (the "GNU LIBRARY GENERAL PUBLIC LICENSE"), in which case the
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

namespace iTextSharp.text.pdf {

    /**
     * Implements the appearance stream to be used with form fields..
     */

    public class PdfAppearance : PdfTemplate {

        public static Hashtable stdFieldFontNames = new Hashtable();

        static PdfAppearance() {
            stdFieldFontNames["Courier-BoldOblique"] = new PdfName("CoBO");
            stdFieldFontNames["Courier-Bold"] = new PdfName("CoBo");
            stdFieldFontNames["Courier-Oblique"] = new PdfName("CoOb");
            stdFieldFontNames["Courier"] = new PdfName("Cour");
            stdFieldFontNames["Helvetica-BoldOblique"] = new PdfName("HeBO");
            stdFieldFontNames["Helvetica-Bold"] = new PdfName("HeBo");
            stdFieldFontNames["Helvetica-Oblique"] = new PdfName("HeOb");
            stdFieldFontNames["Helvetica"] = PdfName.HELV;
            stdFieldFontNames["Symbol"] = new PdfName("Symb");
            stdFieldFontNames["Times-BoldItalic"] = new PdfName("TiBI");
            stdFieldFontNames["Times-Bold"] = new PdfName("TiBo");
            stdFieldFontNames["Times-Italic"] = new PdfName("TiIt");
            stdFieldFontNames["Times-Roman"] = new PdfName("TiRo");
            stdFieldFontNames["ZapfDingbats"] = PdfName.ZADB;
            stdFieldFontNames["HYSMyeongJo-Medium"] = new PdfName("HySm");
            stdFieldFontNames["HYGoThic-Medium"] = new PdfName("HyGo");
            stdFieldFontNames["HeiseiKakuGo-W5"] = new PdfName("KaGo");
            stdFieldFontNames["HeiseiMin-W3"] = new PdfName("KaMi");
            stdFieldFontNames["MHei-Medium"] = new PdfName("MHei");
            stdFieldFontNames["MSung-Light"] = new PdfName("MSun");
            stdFieldFontNames["STSong-Light"] = new PdfName("STSo");
            stdFieldFontNames["MSungStd-Light"] = new PdfName("MSun");
            stdFieldFontNames["STSongStd-Light"] = new PdfName("STSo");
            stdFieldFontNames["HYSMyeongJoStd-Medium"] = new PdfName("HySm");
            stdFieldFontNames["KozMinPro-Regular"] = new PdfName("KaMi");
        }
        
        /**
        *Creates a <CODE>PdfAppearance</CODE>.
        */
    
        internal PdfAppearance() : base() {
            separator = ' ';
        }
    
        internal PdfAppearance(PdfIndirectReference iref) {
            thisReference = iref;
        }
        /**
         * Creates new PdfTemplate
         *
         * @param wr the <CODE>PdfWriter</CODE>
         */
    
        internal PdfAppearance(PdfWriter wr) : base(wr) {
            separator = ' ';
        }
    
        /**
         * Creates a new appearance to be used with form fields.
         *
         * @param width the bounding box width
         * @param height the bounding box height
         * @return the appearance created
         */
        public static PdfAppearance CreateAppearance(PdfWriter writer, float width, float height) {
            return CreateAppearance(writer, width, height, null);
        }
        
        internal static PdfAppearance CreateAppearance(PdfWriter writer, float width, float height, PdfName forcedName) {
            PdfAppearance template = new PdfAppearance(writer);
            template.Width = width;
            template.Height = height;
            writer.AddDirectTemplateSimple(template, forcedName);
            return template;
        }

        /**
        * Set the font and the size for the subsequent text writing.
        *
        * @param bf the font
        * @param size the font size in points
        */
        public override void SetFontAndSize(BaseFont bf, float size) {
            CheckWriter();
            state.size = size;
            if (bf.FontType == BaseFont.FONT_TYPE_DOCUMENT) {
                state.fontDetails = new FontDetails(null, ((DocumentFont)bf).IndirectReference, bf);
            }
            else
                state.fontDetails = writer.AddSimple(bf);
            PdfName psn = (PdfName)stdFieldFontNames[bf.PostscriptFontName];
            if (psn == null) {
                if (bf.Subset && bf.FontType == BaseFont.FONT_TYPE_TTUNI)
                    psn = state.fontDetails.FontName;
                else {
                    psn = new PdfName(bf.PostscriptFontName);
                    state.fontDetails.Subset = false;
                }
            }
            PageResources prs = PageResources;
            prs.AddFont(psn, state.fontDetails.IndirectReference);
            content.Append(psn.GetBytes()).Append(' ').Append(size).Append(" Tf").Append_i(separator);
        }

        public override PdfContentByte Duplicate {
            get {
                PdfAppearance tpl = new PdfAppearance();
                tpl.writer = writer;
                tpl.pdf = pdf;
                tpl.thisReference = thisReference;
                tpl.pageResources = pageResources;
                tpl.bBox = new Rectangle(bBox);
                tpl.group = group;
                tpl.layer = layer;
                if (matrix != null) {
                    tpl.matrix = new PdfArray(matrix);
                }
                tpl.separator = separator;
                return tpl;
            }
        }
    }
}