using System;
using System.Collections;
using System.util;
using iTextSharp.text.factories;

/*
 * $Id: Chapter.cs,v 1.10 2008/05/13 11:25:08 psoares33 Exp $
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
 *
 */

namespace iTextSharp.text 
{
    /// <summary>
    /// A Chapter is a special Section.
    /// </summary>
    /// <remarks>
    /// A chapter number has to be created using a Paragraph as title
    /// and an int as chapter number. The chapter number is shown be
    /// default. If you don't want to see the chapter number, you have to set the
    /// numberdepth to 0.
    /// </remarks>
    /// <example>
    /// <code>
    /// Paragraph title2 = new Paragraph("This is Chapter 2", FontFactory.GetFont(FontFactory.HELVETICA, 18, Font.BOLDITALIC, new Color(0, 0, 255)));
    /// <strong>Chapter chapter2 = new Chapter(title2, 2);
    /// chapter2.SetNumberDepth(0);</strong>
    /// Paragraph someText = new Paragraph("This is some text");
    /// <strong>chapter2.Add(someText);</strong>
    /// Paragraph title21 = new Paragraph("This is Section 1 in Chapter 2", FontFactory.GetFont(FontFactory.HELVETICA, 16, Font.BOLD, new Color(255, 0, 0)));
    /// Section section1 = <strong>chapter2.AddSection(title21);</strong>
    /// Paragraph someSectionText = new Paragraph("This is some silly paragraph in a chapter and/or section. It contains some text to test the functionality of Chapters and Section.");
    /// section1.Add(someSectionText);
    /// </code>
    /// </example>
    public class Chapter : Section 
    {
    
        // constructors
    
        /**
        * Constructs a new <CODE>Chapter</CODE>.
        * @param   number      the Chapter number
        */
        
        public Chapter(int number) : base (null, 1) {
            numbers = new ArrayList();
            numbers.Add(number);
            triggerNewPage = true;
        }

        /// <summary>
        /// Constructs a new Chapter.
        /// </summary>
        /// <param name="title">the Chapter title (as a Paragraph)</param>
        /// <param name="number">the Chapter number</param>
        /// <overoads>
        /// Has three overloads.
        /// </overoads>
        public Chapter(Paragraph title, int number) : base(title, 1) 
        {
            numbers = new ArrayList();
            numbers.Add(number);
            triggerNewPage = true;
        }
    
        /// <summary>
        /// Constructs a new Chapter.
        /// </summary>
        /// <param name="title">the Chapter title (as a string)</param>
        /// <param name="number">the Chapter number</param>
        /// <overoads>
        /// Has three overloads.
        /// </overoads>
        public Chapter(string title, int number) : this(new Paragraph(title), number) {}
    
        // implementation of the Element-methods
    
        /// <summary>
        /// Gets the type of the text element.
        /// </summary>
        /// <value>a type</value>
        public override int Type {
            get {
                return Element.CHAPTER;
            }
        }
    
        /**
        * @see com.lowagie.text.Element#isNestable()
        * @since   iText 2.0.8
        */
        public override bool IsNestable() {
            return false;
        }
    }
}
