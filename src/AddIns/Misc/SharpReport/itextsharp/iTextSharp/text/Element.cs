using System;

/*
 * $Id: Element.cs,v 1.7 2008/05/13 11:25:09 psoares33 Exp $
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

namespace iTextSharp.text 
{

    /// <summary>
    /// Interface for a text element.
    /// </summary>
    /// <seealso cref="T:iTextSharp.text.Anchor"/>
    /// <seealso cref="T:iTextSharp.text.Cell"/>
    /// <seealso cref="T:iTextSharp.text.Chapter"/>
    /// <seealso cref="T:iTextSharp.text.Chunk"/>
    /// <seealso cref="T:iTextSharp.text.Gif"/>
    /// <seealso cref="T:iTextSharp.text.Graphic"/>
    /// <seealso cref="T:iTextSharp.text.Header"/>
    /// <seealso cref="T:iTextSharp.text.Image"/>
    /// <seealso cref="T:iTextSharp.text.Jpeg"/>
    /// <seealso cref="T:iTextSharp.text.List"/>
    /// <seealso cref="T:iTextSharp.text.ListItem"/>
    /// <seealso cref="T:iTextSharp.text.Meta"/>
    /// <seealso cref="T:iTextSharp.text.Paragraph"/>
    /// <seealso cref="T:iTextSharp.text.Phrase"/>
    /// <seealso cref="T:iTextSharp.text.Rectangle"/>
    /// <seealso cref="T:iTextSharp.text.Row"/>
    /// <seealso cref="T:iTextSharp.text.Section"/>
    /// <seealso cref="T:iTextSharp.text.Table"/>
    public class Element 
    {
    
        // static membervariables (meta information)
    
        /// <summary> This is a possible type of Element. </summary>
        public const int HEADER = 0;
    
        /// <summary> This is a possible type of Element. </summary>
        public const int TITLE = 1;
    
        /// <summary> This is a possible type of Element. </summary>
        public const int SUBJECT = 2;
    
        /// <summary> This is a possible type of Element. </summary>
        public const int KEYWORDS = 3;
    
        /// <summary> This is a possible type of Element. </summary>
        public const int AUTHOR = 4;
    
        /// <summary> This is a possible type of Element. </summary>
        public const int PRODUCER = 5;
    
        /// <summary> This is a possible type of Element. </summary>
        public const int CREATIONDATE = 6;
    
        /// <summary> This is a possible type of Element. </summary>
        public const int CREATOR = 7;
    
        // static membervariables (content)
    
        /// <summary> This is a possible type of Element. </summary>
        public const int CHUNK = 10;
    
        /// <summary> This is a possible type of Element. </summary>
        public const int PHRASE = 11;
    
        /// <summary> This is a possible type of Element. </summary>
        public const int PARAGRAPH = 12;
    
        /// <summary> This is a possible type of Element </summary>
        public const int SECTION = 13;
    
        /// <summary> This is a possible type of Element </summary>
        public const int LIST = 14;
    
        /// <summary> This is a possible type of Element </summary>
        public const int LISTITEM = 15;
    
        /// <summary> This is a possible type of Element </summary>
        public const int CHAPTER = 16;
    
        /// <summary> This is a possible type of Element </summary>
        public const int ANCHOR = 17;
    
        // static membervariables (tables)
    
        /// <summary> This is a possible type of Element. </summary>
        public const int CELL = 20;
    
        /// <summary> This is a possible type of Element. </summary>
        public const int ROW = 21;
    
        /// <summary> This is a possible type of Element. </summary>
        public const int TABLE = 22;
    
        /// <summary> This is a possible type of Element. </summary>
        public const int PTABLE = 23;
    
        // static membervariables (annotations)
    
        /// <summary> This is a possible type of Element. </summary>
        public const int ANNOTATION = 29;
    
        // static membervariables (geometric figures)
    
        /// <summary> This is a possible type of Element. </summary>
        public const int RECTANGLE = 30;
    
        /// <summary> This is a possible type of Element. </summary>
        public const int JPEG = 32;
    
	    /** This is a possible type of <CODE>Element</CODE>. */
	    public const int JPEG2000 = 33;

        /// <summary> This is a possible type of Element. </summary>
        public const int IMGRAW = 34;
    
        /// <summary> This is a possible type of Element. </summary>
        public const int IMGTEMPLATE = 35;
    
        /// <summary> This is a possible type of <CODE>Element</CODE>. </summary>
        public const int MULTI_COLUMN_TEXT = 40;
	
        /** This is a possible type of <CODE>Element</CODE>. */
        public const int MARKED = 50;
    
        /** This is a possible type of <CODE>Element</CODE>.
        * @since 2.1.2
        */
        public const int YMARK = 55;

        // static membervariables (alignment)
       
        /// <summary>
        /// A possible value for paragraph Element.  This
        /// specifies that the text is aligned to the left
        /// indent and extra whitespace should be placed on
        /// the right.
        /// </summary>
        public const int ALIGN_UNDEFINED = -1;
    
        /// <summary>
        /// A possible value for paragraph Element.  This
        /// specifies that the text is aligned to the left
        /// indent and extra whitespace should be placed on
        /// the right.
        /// </summary>
        public const int ALIGN_LEFT = 0;
    
        /// <summary>
        /// A possible value for paragraph Element.  This
        /// specifies that the text is aligned to the center
        /// and extra whitespace should be placed equally on
        /// the left and right.
        /// </summary>
        public const int ALIGN_CENTER = 1;
    
        /// <summary>
        /// A possible value for paragraph Element.  This
        /// specifies that the text is aligned to the right
        /// indent and extra whitespace should be placed on
        /// the left.
        /// </summary>
        public const int ALIGN_RIGHT = 2;
    
        /// <summary>
        /// A possible value for paragraph Element.  This
        /// specifies that extra whitespace should be spread
        /// out through the rows of the paragraph with the
        /// text lined up with the left and right indent
        /// except on the last line which should be aligned
        /// to the left.
        /// </summary>
        public const int ALIGN_JUSTIFIED = 3;
    
        /// <summary>
        /// A possible value for vertical Element.
        /// </summary>
        public const int ALIGN_TOP = 4;
    
        /// <summary>
        /// A possible value for vertical Element.
        /// </summary>
        public const int ALIGN_MIDDLE = 5;
    
        /// <summary>
        /// A possible value for vertical Element.
        /// </summary>
        public const int ALIGN_BOTTOM = 6;
    
        /// <summary>
        /// A possible value for vertical Element.
        /// </summary>
        public const int ALIGN_BASELINE = 7;

        /// <summary>
        /// Does the same as ALIGN_JUSTIFIED but the last line is also spread out.
        /// </summary>
        public const int ALIGN_JUSTIFIED_ALL = 8;

        // static member variables for CCITT compression
    
        /// <summary>
        /// Pure two-dimensional encoding (Group 4)
        /// </summary>
        public const int CCITTG4 = 0x100;
        /// <summary>
        /// Pure one-dimensional encoding (Group 3, 1-D)
        /// </summary>
        public const int CCITTG3_1D = 0x101;
        /// <summary>
        /// Mixed one- and two-dimensional encoding (Group 3, 2-D)
        /// </summary>
        public const int CCITTG3_2D = 0x102;    
        /// <summary>
        /// A flag indicating whether 1-bits are to be interpreted as black pixels
        /// and 0-bits as white pixels,
        /// </summary>
        public const int CCITT_BLACKIS1 = 1;
        /// <summary>
        /// A flag indicating whether the filter expects extra 0-bits before each
        /// encoded line so that the line begins on a byte boundary.
        /// </summary>
        public const int CCITT_ENCODEDBYTEALIGN = 2;
        /// <summary>
        /// A flag indicating whether end-of-line bit patterns are required to be
        ///  present in the encoding.
        /// </summary>
        public const int CCITT_ENDOFLINE = 4;
        /// <summary>
        /// A flag indicating whether the filter expects the encoded data to be
        /// terminated by an end-of-block pattern, overriding the Rows
        /// parameter. The use of this flag will set the key /EndOfBlock to false.
        /// </summary>
        public const int CCITT_ENDOFBLOCK = 8;
    
    }
}