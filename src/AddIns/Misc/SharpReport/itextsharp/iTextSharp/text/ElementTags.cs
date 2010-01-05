using System;
using System.util;

/*
 * $Id: ElementTags.cs,v 1.8 2008/05/13 11:25:10 psoares33 Exp $
 * 
 *
 * Copyright (c) 2001, 2002 Bruno Lowagie.
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
    /// A class that contains all the possible tagnames and their attributes.
    /// </summary>
    public class ElementTags 
    {
    
        /// <summary> the root tag. </summary>
        public const string ITEXT = "itext";
    
        /// <summary> attribute of the root and annotation tag (also a special tag within a chapter or section) </summary>
        public const string TITLE = "title";
    
        /// <summary> attribute of the root tag </summary>
        public const string SUBJECT = "subject";
    
        /// <summary> attribute of the root tag </summary>
        public const string KEYWORDS = "keywords";
    
        /// <summary> attribute of the root tag </summary>
        public const string AUTHOR = "author";
    
        /// <summary> attribute of the root tag </summary>
        public const string CREATIONDATE = "creationdate";
    
        /// <summary> attribute of the root tag </summary>
        public const string PRODUCER = "producer";
    
        // Chapters and Sections
    
        /// <summary> the chapter tag </summary>
        public const string CHAPTER = "chapter";
    
        /// <summary> the section tag </summary>
        public const string SECTION = "section";
    
        /// <summary> attribute of section/chapter tag </summary>
        public const string NUMBERDEPTH = "numberdepth";
    
        /// <summary> attribute of section/chapter tag </summary>
        public const string DEPTH = "depth";
    
        /// <summary> attribute of section/chapter tag </summary>
        public const string NUMBER = "number";
    
        /// <summary> attribute of section/chapter tag </summary>
        public const string INDENT = "indent";
    
        /// <summary> attribute of chapter/section/paragraph/table/cell tag </summary>
        public const string LEFT = "left";
    
        /// <summary> attribute of chapter/section/paragraph/table/cell tag </summary>
        public const string RIGHT = "right";
    
        // Phrases, Anchors, Lists and Paragraphs
    
        /// <summary> the phrase tag </summary>
        public const string PHRASE = "phrase";
    
        /// <summary> the anchor tag </summary>
        public const string ANCHOR = "anchor";
    
        /// <summary> the list tag </summary>
        public const string LIST = "list";
    
        /// <summary> the listitem tag </summary>
        public const string LISTITEM = "listitem";
    
        /// <summary> the paragraph tag </summary>
        public const string PARAGRAPH = "paragraph";
    
        /// <summary> attribute of phrase/paragraph/cell tag </summary>
        public const string LEADING = "leading";
    
        /// <summary> attribute of paragraph/image/table tag </summary>
        public const string ALIGN = "align";
    
        /// <summary> attribute of paragraph </summary>
        public const string KEEPTOGETHER = "keeptogether";
    
        /// <summary> attribute of anchor tag </summary>
        public const string NAME = "name";
    
        /// <summary> attribute of anchor tag </summary>
        public const string REFERENCE = "reference";
    
        /// <summary> attribute of list tag </summary>
        public const string LISTSYMBOL = "listsymbol";
    
        /// <summary> attribute of list tag </summary>
        public const string NUMBERED = "numbered";
    
        /// <summary> attribute of the list tag </summary>
        public const string LETTERED = "lettered";

        /// <summary> attribute of list tag </summary>
        public const string FIRST = "first";
    
        /// <summary> attribute of list tag </summary>
        public const string SYMBOLINDENT = "symbolindent";
    
        /// <summary> attribute of list tag </summary>
        public const string INDENTATIONLEFT = "indentationleft";
    
        /// <summary> attribute of list tag </summary>
        public const string INDENTATIONRIGHT = "indentationright";
    
        // Chunks
    
        /// <summary> the chunk tag </summary>
        public const string IGNORE = "ignore";
    
        /// <summary> the chunk tag </summary>
        public const string ENTITY = "entity";
    
        /// <summary> the chunk tag </summary>
        public const string ID = "id";
    
        /// <summary> the chunk tag </summary>
        public const string CHUNK = "chunk";
    
        /// <summary> attribute of the chunk tag </summary>
        public const string ENCODING = "encoding";
    
        /// <summary> attribute of the chunk tag </summary>
        public const string EMBEDDED = "embedded";
    
        /// <summary> attribute of the chunk/table/cell tag </summary>
        public const string COLOR = "color";
    
        /// <summary> attribute of the chunk/table/cell tag </summary>
        public const string RED = "red";
    
        /// <summary> attribute of the chunk/table/cell tag </summary>
        public const string GREEN = "green";
    
        /// <summary> attribute of the chunk/table/cell tag </summary>
        public const string BLUE = "blue";
    
        /// <summary> attribute of the chunk tag </summary>
        public static readonly string SUBSUPSCRIPT = Chunk.SUBSUPSCRIPT.ToLower(System.Globalization.CultureInfo.InvariantCulture);
    
        /// <summary> attribute of the chunk tag </summary>
        public static readonly string LOCALGOTO = Chunk.LOCALGOTO.ToLower(System.Globalization.CultureInfo.InvariantCulture);
    
        /// <summary> attribute of the chunk tag </summary>
        public static readonly string REMOTEGOTO = Chunk.REMOTEGOTO.ToLower(System.Globalization.CultureInfo.InvariantCulture);
    
        /// <summary> attribute of the chunk tag </summary>
        public static readonly string LOCALDESTINATION = Chunk.LOCALDESTINATION.ToLower(System.Globalization.CultureInfo.InvariantCulture);
    
        /// <summary> attribute of the chunk tag </summary>
        public static readonly string GENERICTAG = Chunk.GENERICTAG.ToLower(System.Globalization.CultureInfo.InvariantCulture);
    
        // tables/cells
    
        /// <summary> the table tag </summary>
        public const string TABLE = "table";
    
        /// <summary> the cell tag </summary>
        public const string ROW = "row";
    
        /// <summary> the cell tag </summary>
        public const string CELL = "cell";
    
        /// <summary> attribute of the table tag </summary>
        public const string COLUMNS = "columns";
    
        /// <summary> attribute of the table tag </summary>
        public const string LASTHEADERROW = "lastHeaderRow";
    
        /// <summary> attribute of the table tag </summary>
        public const string CELLPADDING = "cellpadding";
    
        /// <summary> attribute of the table tag </summary>
        public const string CELLSPACING = "cellspacing";
    
        /// <summary> attribute of the table tag </summary>
        public const string OFFSET = "offset";
    
        /// <summary> attribute of the table tag </summary>
        public const string WIDTHS = "widths";
    
        /// <summary> attribute of the table tag </summary>
        public const string TABLEFITSPAGE = "tablefitspage";
    
        /// <summary> attribute of the table tag </summary>
        public const string CELLSFITPAGE = "cellsfitpage";
    
        /// <summary> attribute of the table tag </summary>
        public const string CONVERT2PDFP = "convert2pdfp";
                
        /// <summary> attribute of the cell tag </summary>
        public const string HORIZONTALALIGN = "horizontalalign";
    
        /// <summary> attribute of the cell tag </summary>
        public const string VERTICALALIGN = "verticalalign";
    
        /// <summary> attribute of the cell tag </summary>
        public const string COLSPAN = "colspan";
    
        /// <summary> attribute of the cell tag </summary>
        public const string ROWSPAN = "rowspan";
    
        /// <summary> attribute of the cell tag </summary>
        public const string HEADER = "header";
    
        /// <summary> attribute of the cell tag </summary>
        public const string FOOTER = "footer";

        /// <summary> attribute of the cell tag </summary>
        public const string NOWRAP = "nowrap";
    
        /// <summary> attribute of the table/cell tag </summary>
        public const string BORDERWIDTH = "borderwidth";
    
        /// <summary> attribute of the table/cell tag </summary>
        public const string TOP = "top";
    
        /// <summary> attribute of the table/cell tag </summary>
        public const string BOTTOM = "bottom";
    
        /// <summary> attribute of the table/cell tag </summary>
        public const string WIDTH = "width";
    
        /// <summary> attribute of the table/cell tag </summary>
        public const string BORDERCOLOR = "bordercolor";
    
        /// <summary> attribute of the table/cell tag </summary>
        public const string BACKGROUNDCOLOR = "backgroundcolor";
    
        /// <summary> attribute of the table/cell tag </summary>
        public const string BGRED = "bgred";
    
        /// <summary> attribute of the table/cell tag </summary>
        public const string BGGREEN = "bggreen";
    
        /// <summary> attribute of the table/cell tag </summary>
        public const string BGBLUE = "bgblue";
    
        /// <summary> attribute of the table/cell tag </summary>
        public const string GRAYFILL = "grayfill";
    
        // Misc
    
        /// <summary> the image tag </summary>
        public const string IMAGE = "image";
    
        /// <summary> the image tag </summary>
        public const string BOOKMARKOPEN = "bookmarkopen";
    
        /// <summary> attribute of the image and annotation tag </summary>
        public const string URL = "url";
    
        /// <summary> attribute of the image tag </summary>
        public const string UNDERLYING = "underlying";
    
        /// <summary> attribute of the image tag </summary>
        public const string TEXTWRAP = "textwrap";
    
        /// <summary> attribute of the image tag </summary>
        public const string ALT = "alt";
    
        /// <summary> attribute of the image tag </summary>
        public const string ABSOLUTEX = "absolutex";
    
        /// <summary> attribute of the image tag </summary>
        public const string ABSOLUTEY = "absolutey";
    
        /// <summary> attribute of the image tag </summary>
        public const string PLAINWIDTH = "plainwidth";
    
        /// <summary> attribute of the image tag </summary>
        public const string PLAINHEIGHT = "plainheight";
    
        /// <summary> attribute of the image tag </summary>
        public const string SCALEDWIDTH = "scaledwidth";
    
        /// <summary> attribute of the image tag </summary>
        public const string SCALEDHEIGHT = "scaledheight";
    
        /// <summary> attribute of the image tag </summary>
        public const string  ROTATION = "rotation";
    
        /// <summary> the newpage tag </summary>
        public const string NEWPAGE = "newpage";
    
        /// <summary> the newpage tag </summary>
        public const string NEWLINE = "newline";
    
        /// <summary> the annotation tag </summary>
        public const string ANNOTATION = "annotation";
    
        /// <summary> attribute of the annotation tag </summary>
        public const string FILE = "file";
    
        /// <summary> attribute of the annotation tag </summary>
        public const string DESTINATION = "destination";
    
        /// <summary> attribute of the annotation tag </summary>
        public const string PAGE = "page";
    
        /// <summary> attribute of the annotation tag </summary>
        public const string NAMED = "named";
    
        /// <summary> attribute of the annotation tag </summary>
        public const string APPLICATION = "application";
    
        /// <summary> attribute of the annotation tag </summary>
        public const string PARAMETERS = "parameters";
    
        /// <summary> attribute of the annotation tag </summary>
        public const string OPERATION = "operation";
    
        /// <summary> attribute of the annotation tag </summary>
        public const string DEFAULTDIR = "defaultdir";
    
        /// <summary> attribute of the annotation tag </summary>
        public const string LLX = "llx";
    
        /// <summary> attribute of the annotation tag </summary>
        public const string LLY = "lly";
    
        /// <summary> attribute of the annotation tag </summary>
        public const string URX = "urx";
    
        /// <summary> attribute of the annotation tag </summary>
        public const string URY = "ury";
    
        /// <summary> attribute of the annotation tag </summary>
        public const string CONTENT = "content";
    
        // alignment attribute values
    
        /// <summary> the possible value of an alignment attribute </summary>
        public const string ALIGN_LEFT = "Left";
    
        /// <summary> the possible value of an alignment attribute </summary>
        public const string ALIGN_CENTER = "Center";
    
        /// <summary> the possible value of an alignment attribute </summary>
        public const string ALIGN_RIGHT = "Right";
    
        /// <summary> the possible value of an alignment attribute </summary>
        public const string ALIGN_JUSTIFIED = "Justify";
    
        /// <summary> the possible value of an alignment attribute </summary>
        public const string ALIGN_JUSTIFIED_ALL = "JustifyAll";
    
        /// <summary> the possible value of an alignment attribute </summary>
        public const string ALIGN_TOP = "Top";
    
        /// <summary> the possible value of an alignment attribute </summary>
        public const string ALIGN_MIDDLE = "Middle";
    
        /// <summary> the possible value of an alignment attribute </summary>
        public const string ALIGN_BOTTOM = "Bottom";
    
        /// <summary> the possible value of an alignment attribute </summary>
        public const string ALIGN_BASELINE = "Baseline";
    
        /// <summary> the possible value of an alignment attribute </summary>
        public const string DEFAULT = "Default";
    
        /// <summary> the possible value of an alignment attribute </summary>
        public const string UNKNOWN = "unknown";
    
        /// <summary> the possible value of an alignment attribute </summary>
        public const string FONT = "font";
    
        /// <summary> the possible value of an alignment attribute </summary>
        public const string SIZE = "size";
    
        /// <summary> the possible value of an alignment attribute </summary>
        public const string STYLE = "fontstyle";
    
        /// <summary> the possible value of a tag </summary>
        public const string HORIZONTALRULE = "horizontalrule";
        /** the possible value of a tag */
        public const string PAGE_SIZE  = "pagesize";

        /** the possible value of a tag */
        public const string ORIENTATION  = "orientation";
    
        /** a possible list attribute */
	    public const String ALIGN_INDENTATION_ITEMS = "alignindent";
    	
	    /** a possible list attribute */
	    public const String AUTO_INDENT_ITEMS = "autoindent";
    	
	    /** a possible list attribute */
	    public const String LOWERCASE = "lowercase";

        // methods
    
        /// <summary>
        /// Translates the alignment value to a String value.
        /// </summary>
        /// <param name="alignment">the alignment value</param>
        /// <returns>the translated value</returns>
        public static string GetAlignment(int alignment) 
        {
            switch (alignment) 
            {
                case Element.ALIGN_LEFT:
                    return ALIGN_LEFT;
                case Element.ALIGN_CENTER:
                    return ALIGN_CENTER;
                case Element.ALIGN_RIGHT:
                    return ALIGN_RIGHT;
                case Element.ALIGN_JUSTIFIED:
                case Element.ALIGN_JUSTIFIED_ALL:
                    return ALIGN_JUSTIFIED;
                case Element.ALIGN_TOP:
                    return ALIGN_TOP;
                case Element.ALIGN_MIDDLE:
                    return ALIGN_MIDDLE;
                case Element.ALIGN_BOTTOM:
                    return ALIGN_BOTTOM;
                case Element.ALIGN_BASELINE:
                    return ALIGN_BASELINE;
                default:
                    return DEFAULT;
            }
        }

    /**
    * Translates a String value to an alignment value.
    * (written by Norman Richards, integrated into iText by Bruno)
    * @param	a String (one of the ALIGN_ constants of this class)
    * @param	an alignment value (one of the ALIGN_ constants of the Element interface) 
    */
        public static int AlignmentValue(String alignment) {
            if (alignment == null) return Element.ALIGN_UNDEFINED;
            if (Util.EqualsIgnoreCase(ALIGN_CENTER, alignment)) {
                return Element.ALIGN_CENTER;
            }
            if (Util.EqualsIgnoreCase(ALIGN_LEFT, alignment)) {
                return Element.ALIGN_LEFT;
            }
            if (Util.EqualsIgnoreCase(ALIGN_RIGHT, alignment)) {
                return Element.ALIGN_RIGHT;
            }
            if (Util.EqualsIgnoreCase(ALIGN_JUSTIFIED, alignment)) {
                return Element.ALIGN_JUSTIFIED;
            }
            if (Util.EqualsIgnoreCase(ALIGN_JUSTIFIED_ALL, alignment)) {
                return Element.ALIGN_JUSTIFIED_ALL;
            }
            if (Util.EqualsIgnoreCase(ALIGN_TOP, alignment)) {
                return Element.ALIGN_TOP;
            }
            if (Util.EqualsIgnoreCase(ALIGN_MIDDLE, alignment)) {
                return Element.ALIGN_MIDDLE;
            }
            if (Util.EqualsIgnoreCase(ALIGN_BOTTOM, alignment)) {
                return Element.ALIGN_BOTTOM;
            }
            if (Util.EqualsIgnoreCase(ALIGN_BASELINE, alignment)) {
                return Element.ALIGN_BASELINE;
            }

            return Element.ALIGN_UNDEFINED;
        }
    
    }
}
