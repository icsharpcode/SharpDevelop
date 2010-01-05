using System;

/*
 * $Id: HtmlTags.cs,v 1.4 2008/05/13 11:25:15 psoares33 Exp $
 * 
 *
 * Copyright 2001, 2002 by Bruno Lowagie.
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

namespace iTextSharp.text.html {

    /**
     * A class that contains all the possible tagnames and their attributes.
     */

    public class HtmlTags {
    
        /** the root tag. */
        public const string HTML = "html";
    
        /** the head tag */
        public const string HEAD = "head";
    
        /** This is a possible HTML attribute for the HEAD tag. */
        public const string CONTENT = "content";
    
        /** the meta tag */
        public const string META = "meta";
    
        /** attribute of the root tag */
        public const string SUBJECT = "subject";
    
        /** attribute of the root tag */
        public const string KEYWORDS = "keywords";
    
        /** attribute of the root tag */
        public const string AUTHOR = "author";
    
        /** the title tag. */
        public const string TITLE = "title";
    
        /** the script tag. */
        public const string SCRIPT = "script";

        /** This is a possible HTML attribute for the SCRIPT tag. */
        public const string LANGUAGE = "language";

        /** This is a possible value for the LANGUAGE attribute. */
        public const string JAVASCRIPT = "JavaScript";

        /** the body tag. */
        public const string BODY = "body";
    
        /** This is a possible HTML attribute for the BODY tag */
        public const string JAVASCRIPT_ONLOAD = "onLoad";

        /** This is a possible HTML attribute for the BODY tag */
        public const string JAVASCRIPT_ONUNLOAD = "onUnLoad";

        /** This is a possible HTML attribute for the BODY tag. */
        public const string TOPMARGIN = "topmargin";
    
        /** This is a possible HTML attribute for the BODY tag. */
        public const string BOTTOMMARGIN = "bottommargin";
    
        /** This is a possible HTML attribute for the BODY tag. */
        public const string LEFTMARGIN = "leftmargin";
    
        /** This is a possible HTML attribute for the BODY tag. */
        public const string RIGHTMARGIN = "rightmargin";
    
        // Phrases, Anchors, Lists and Paragraphs
    
        /** the chunk tag */
        public const string CHUNK = "font";
    
        /** the phrase tag */
        public const string CODE = "code";
    
        /** the phrase tag */
        public const string VAR = "var";
    
        /** the anchor tag */
        public const string ANCHOR = "a";
    
        /** the list tag */
        public const string ORDEREDLIST = "ol";
    
        /** the list tag */
        public const string UNORDEREDLIST = "ul";
    
        /** the listitem tag */
        public const string LISTITEM = "li";
    
        /** the paragraph tag */
        public const string PARAGRAPH = "p";
    
        /** attribute of anchor tag */
        public const string NAME = "name";
    
        /** attribute of anchor tag */
        public const string REFERENCE = "href";
    
        /** attribute of anchor tag */
        public static string[] H = {"h1", "h2", "h3", "h4", "h5", "h6"};
    
        // Chunks
    
        /** attribute of the chunk tag */
        public const string FONT = "face";
    
        /** attribute of the chunk tag */
        public const string SIZE = "point-size";
    
        /** attribute of the chunk/table/cell tag */
        public const string COLOR = "color";
    
        /** some phrase tag */
        public const string EM = "em";
    
        /** some phrase tag */
        public const string I = "i";
    
        /** some phrase tag */
        public const string STRONG = "strong";
    
        /** some phrase tag */
        public const string B = "b";
    
        /** some phrase tag */
        public const string S = "s";
    
        /** some phrase tag */
        public const string U = "u";
    
        /** some phrase tag */
        public const string SUB = "sub";
    
        /** some phrase tag */
        public const string SUP = "sup";
    
        /** the possible value of a tag */
        public const string HORIZONTALRULE = "hr";
    
        // tables/cells
    
        /** the table tag */
        public const string TABLE = "table";
    
        /** the cell tag */
        public const string ROW = "tr";
    
        /** the cell tag */
        public const string CELL = "td";
    
        /** attribute of the cell tag */
        public const string HEADERCELL = "th";
    
        /** attribute of the table tag */
        public const string COLUMNS = "cols";
    
        /** attribute of the table tag */
        public const string CELLPADDING = "cellpadding";
    
        /** attribute of the table tag */
        public const string CELLSPACING = "cellspacing";
    
        /** attribute of the cell tag */
        public const string COLSPAN = "colspan";
    
        /** attribute of the cell tag */
        public const string ROWSPAN = "rowspan";
    
        /** attribute of the cell tag */
        public const string NOWRAP = "nowrap";
    
        /** attribute of the table/cell tag */
        public const string BORDERWIDTH = "border";
    
        /** attribute of the table/cell tag */
        public const string WIDTH = "width";
    
        /** attribute of the table/cell tag */
        public const string BACKGROUNDCOLOR = "bgcolor";
    
        /** attribute of the table/cell tag */
        public const string BORDERCOLOR = "bordercolor";
    
        /** attribute of paragraph/image/table tag */
        public const string ALIGN = "align";
    
        /** attribute of chapter/section/paragraph/table/cell tag */
        public const string LEFT = "left";
    
        /** attribute of chapter/section/paragraph/table/cell tag */
        public const string RIGHT = "right";
    
        /** attribute of the cell tag */
        public const string HORIZONTALALIGN = "align";
    
        /** attribute of the cell tag */
        public const string VERTICALALIGN = "valign";
    
        /** attribute of the table/cell tag */
        public const string TOP = "top";
    
        /** attribute of the table/cell tag */
        public const string BOTTOM = "bottom";
    
        // Misc
    
        /** the image tag */
        public const string IMAGE = "img";
    
        /** attribute of the image tag */
        public const string URL = "src";
    
        /** attribute of the image tag */
        public const string ALT = "alt";
    
        /** attribute of the image tag */
        public const string PLAINWIDTH = "width";
    
        /** attribute of the image tag */
        public const string PLAINHEIGHT = "height";
    
        /** the newpage tag */
        public const string NEWLINE = "br";
    
        // alignment attribute values
    
        /** the possible value of an alignment attribute */
        public const string ALIGN_LEFT = "Left";
    
        /** the possible value of an alignment attribute */
        public const string ALIGN_CENTER = "Center";
    
        /** the possible value of an alignment attribute */
        public const string ALIGN_RIGHT = "Right";
    
        /** the possible value of an alignment attribute */
        public const string ALIGN_JUSTIFIED = "Justify";
    
        /** the possible value of an alignment attribute */
        public const string ALIGN_TOP = "Top";
    
        /** the possible value of an alignment attribute */
        public const string ALIGN_MIDDLE = "Middle";
    
        /** the possible value of an alignment attribute */
        public const string ALIGN_BOTTOM = "Bottom";
    
        /** the possible value of an alignment attribute */
        public const string ALIGN_BASELINE = "Baseline";
    
        /** the possible value of an alignment attribute */
        public const string DEFAULT = "Default";

        /** The DIV tag. */
        public const string DIV = "div";

        /** The SPAN tag. */
        public const string SPAN = "span";
        /** The LINK tag. */
        public const string LINK = "link";
        
        /** This is a possible HTML attribute for the LINK tag. */
        public const string TEXT_CSS = "text/css";

        /** This is a possible HTML attribute for the LINK tag. */
        public const string REL = "rel";

        /** This is used for inline css style information */
        public const string STYLE = "style";

        /** This is a possible HTML attribute for the LINK tag. */
        public const string TYPE = "type";

        /** This is a possible HTML attribute. */
        public const string STYLESHEET = "stylesheet";
    }
}