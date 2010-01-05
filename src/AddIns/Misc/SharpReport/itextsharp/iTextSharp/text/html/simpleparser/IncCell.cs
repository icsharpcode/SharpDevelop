using System;
using System.Collections;
using System.util;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.html;
/*
 * Copyright 2004 Paulo Soares
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

namespace iTextSharp.text.html.simpleparser {
    /**
    *
    * @author  psoares
    */
    public class IncCell : ITextElementArray {
        
        private ArrayList chunks = new ArrayList();
        private PdfPCell cell;
        
        /** Creates a new instance of IncCell */
        public IncCell(String tag, ChainedProperties props) {
            cell = new PdfPCell();
            String value = props["colspan"];
            if (value != null)
                cell.Colspan = int.Parse(value);
            value = props["align"];
            if (tag.Equals("th"))
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
            if (value != null) {
                if (Util.EqualsIgnoreCase(value, "center"))
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                else if (Util.EqualsIgnoreCase(value, "right"))
                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                else if (Util.EqualsIgnoreCase(value, "left"))
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                else if (Util.EqualsIgnoreCase(value, "justify"))
                    cell.HorizontalAlignment = Element.ALIGN_JUSTIFIED;
            }
            value = props["valign"];
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            if (value != null) {
                if (Util.EqualsIgnoreCase(value, "top"))
                    cell.VerticalAlignment = Element.ALIGN_TOP;
                else if (Util.EqualsIgnoreCase(value, "bottom"))
                    cell.VerticalAlignment = Element.ALIGN_BOTTOM;
            }
            value = props["border"];
            float border = 0;
            if (value != null)
                border = float.Parse(value, System.Globalization.NumberFormatInfo.InvariantInfo);
            cell.BorderWidth = border;
            value = props["cellpadding"];
            if (value != null)
                cell.Padding = float.Parse(value, System.Globalization.NumberFormatInfo.InvariantInfo);
            cell.UseDescender = true;
            value = props["bgcolor"];
            cell.BackgroundColor = Markup.DecodeColor(value);
        }
        
        public bool Add(Object o) {
            if (!(o is IElement))
                return false;
            cell.AddElement((IElement)o);
            return true;
        }
        
        public ArrayList Chunks {
            get {
                return chunks;
            }
        }
        
        public bool Process(IElementListener listener) {
            return true;
        }
        
        public int Type {
            get {
                return Element.RECTANGLE;
            }
        }
        
        public PdfPCell Cell {
            get {
                return cell;
            }
        }    

        /**
        * @see com.lowagie.text.Element#isContent()
        * @since   iText 2.0.8
        */
        public bool IsContent() {
            return true;
        }

        /**
        * @see com.lowagie.text.Element#isNestable()
        * @since   iText 2.0.8
        */
        public bool IsNestable() {
            return true;
        }
  
        public override string ToString() {
            return base.ToString();
        }
    }
}