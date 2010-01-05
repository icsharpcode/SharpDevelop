using System;
using System.util;
using System.Collections;

/*
 * $Id: HeaderFooter.cs,v 1.6 2008/05/13 11:25:10 psoares33 Exp $
 * 
 *
 * Copyright 1999, 2000, 2001, 2002 by Bruno Lowagie.
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

namespace iTextSharp.text {
    /// <summary>
    /// A HeaderFooter-object is a Rectangle with text
    /// that can be put above and/or below every page.
    /// </summary>
    /// <example>
    /// <code>
    /// <strong>HeaderFooter header = new HeaderFooter(new Phrase("This is a header."), false);
    /// HeaderFooter footer = new HeaderFooter(new Phrase("This is page "), new Phrase("."));</strong>
    /// document.SetHeader(header);
    /// document.SetFooter(footer);
    /// </code>
    /// </example>
    public class HeaderFooter : Rectangle {
    
        // membervariables
    
        /// <summary> Does the page contain a pagenumber? </summary>
        private bool numbered;
    
        /// <summary> This is the Phrase that comes before the pagenumber. </summary>
        private Phrase before = null;
    
        /// <summary> This is number of the page. </summary>
        private int pageN;
    
        /// <summary> This is the Phrase that comes after the pagenumber. </summary>
        private Phrase after = null;
    
        /// <summary> This is alignment of the header/footer. </summary>
        private int alignment;
    
        // constructors
    
        /// <summary>
        /// Constructs a HeaderFooter-object.
        /// </summary>
        /// <param name="before">the Phrase before the pagenumber</param>
        /// <param name="after">the Phrase after the pagenumber</param>
        public HeaderFooter(Phrase before, Phrase after) : base(0, 0, 0, 0) {
            this.Border = TOP_BORDER + BOTTOM_BORDER;
            this.BorderWidth = 1;
        
            numbered = true;
            this.before = before;
            this.after = after;
        }
    
        /// <summary>
        /// Constructs a Header-object with a pagenumber at the end.
        /// </summary>
        /// <param name="before">the Phrase before the pagenumber</param>
        /// <param name="numbered">true if the page has to be numbered</param>
        public HeaderFooter(Phrase before, bool numbered) : base(0, 0, 0, 0) {
            this.Border = TOP_BORDER + BOTTOM_BORDER;
            this.BorderWidth = 1;
        
            this.numbered = numbered;
            this.before = before;
        }

        public HeaderFooter(Properties attributes) : base(0, 0, 0, 0) {
            string value;
            
            if ((value = attributes.Remove(ElementTags.NUMBERED)) != null) {
                this.numbered = bool.Parse(value);
            }
            if ((value = attributes.Remove(ElementTags.ALIGN)) != null) {
                this.SetAlignment(value);
            }
            if ((value = attributes.Remove("border")) != null) {
                this.Border = int.Parse(value);
            } else {
                this.Border = TOP_BORDER + BOTTOM_BORDER;
            }
        }
    
        // methods
    
        /// <summary>
        /// Checks if the HeaderFooter contains a page number.
        /// </summary>
        /// <returns>true if the page has to be numbered</returns>
        public bool IsNumbered() {
            return numbered;
        }
    
        /// <summary>
        /// Get/set the part that comes before the pageNumber.
        /// </summary>
        /// <value>a Phrase</value>
        public Phrase Before {
            get {
                return before;
            }

            set {
                this.before = value;
            }
        }
    
        /// <summary>
        /// Get/set the part that comes after the pageNumber.
        /// </summary>
        /// <value>a Phrase</value>
        public Phrase After {
            get {
                return after;
            }

            set {
                this.after = value;
            }
        }
    
        /// <summary>
        /// Sets the page number.
        /// </summary>
        /// <value>the new page number</value>
        public int PageNumber {
            set {
                this.pageN = value;
            }
        }
    
        /// <summary>
        /// Sets the Element.
        /// </summary>
        /// <value>the new alignment</value>
        public int Alignment{
            set {
                this.alignment = value;
            }
            get {
                return this.alignment;
            }
        }

        /// <summary>
        /// Sets the alignment of this HeaderFooter.
        /// </summary>
        /// <param name="alignment">the new alignment as a string</param>
        public void SetAlignment(string alignment) {
            if (Util.EqualsIgnoreCase(alignment, ElementTags.ALIGN_CENTER)) {
                this.alignment = Element.ALIGN_CENTER;
                return;
            }
            if (Util.EqualsIgnoreCase(alignment, ElementTags.ALIGN_RIGHT)) {
                this.alignment = Element.ALIGN_RIGHT;
                return;
            }
            if (Util.EqualsIgnoreCase(alignment, ElementTags.ALIGN_JUSTIFIED)) {
                this.alignment = Element.ALIGN_JUSTIFIED;
                return;
            }
            this.alignment = Element.ALIGN_LEFT;
        }
    
        // methods to retrieve the membervariables
    
        /// <summary>
        /// Gets the Paragraph that can be used as header or footer.
        /// </summary>
        /// <returns>a Paragraph</returns>
        public Paragraph Paragraph {
            get {
                Paragraph paragraph = new Paragraph(before.Leading);
                paragraph.Add(before);
                if (numbered) {
                    paragraph.AddSpecial(new Chunk(pageN.ToString(), before.Font));
                }
                if (after != null) {
                    paragraph.AddSpecial(after);
                }
                paragraph.Alignment = alignment;
                return paragraph;
            }
        }
    }
}
