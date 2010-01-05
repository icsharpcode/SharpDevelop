using System;
using System.util;
using System.Collections;
using iTextSharp.text;

/*
 * $Id: HtmlTagMap.cs,v 1.4 2008/05/13 11:25:15 psoares33 Exp $
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
    * The <CODE>Tags</CODE>-class maps several XHTML-tags to iText-objects.
    */

    public class HtmlTagMap : Hashtable {
        
    /**
    * Constructs an HtmlTagMap.
    */
        
        public HtmlTagMap() {
            HtmlPeer peer;
            
            peer = new HtmlPeer(ElementTags.ITEXT, HtmlTags.HTML);
            this[peer.Alias] = peer;
            
            peer = new HtmlPeer(ElementTags.PHRASE, HtmlTags.SPAN);
            this[peer.Alias] = peer;
            
            peer = new HtmlPeer(ElementTags.CHUNK, HtmlTags.CHUNK);
            peer.AddAlias(ElementTags.FONT, HtmlTags.FONT);
            peer.AddAlias(ElementTags.SIZE, HtmlTags.SIZE);
            peer.AddAlias(ElementTags.COLOR, HtmlTags.COLOR);
            this[peer.Alias] = peer;
            
            peer = new HtmlPeer(ElementTags.ANCHOR, HtmlTags.ANCHOR);
            peer.AddAlias(ElementTags.NAME, HtmlTags.NAME);
            peer.AddAlias(ElementTags.REFERENCE, HtmlTags.REFERENCE);
            this[peer.Alias] = peer;
            
            peer = new HtmlPeer(ElementTags.PARAGRAPH, HtmlTags.PARAGRAPH);
            peer.AddAlias(ElementTags.ALIGN, HtmlTags.ALIGN);
            this[peer.Alias] = peer;
            
            peer = new HtmlPeer(ElementTags.PARAGRAPH, HtmlTags.DIV);
            peer.AddAlias(ElementTags.ALIGN, HtmlTags.ALIGN);
            this[peer.Alias] = peer;
            
            peer = new HtmlPeer(ElementTags.PARAGRAPH, HtmlTags.H[0]);
            peer.AddValue(ElementTags.SIZE, "20");
            this[peer.Alias] = peer;
            
            peer = new HtmlPeer(ElementTags.PARAGRAPH, HtmlTags.H[1]);
            peer.AddValue(ElementTags.SIZE, "18");
            this[peer.Alias] = peer;
            
            peer = new HtmlPeer(ElementTags.PARAGRAPH, HtmlTags.H[2]);
            peer.AddValue(ElementTags.SIZE, "16");
            this[peer.Alias] = peer;
            
            peer = new HtmlPeer(ElementTags.PARAGRAPH, HtmlTags.H[3]);
            peer.AddValue(ElementTags.SIZE, "14");
            this[peer.Alias] = peer;
            
            peer = new HtmlPeer(ElementTags.PARAGRAPH, HtmlTags.H[4]);
            peer.AddValue(ElementTags.SIZE, "12");
            this[peer.Alias] = peer;
            
            peer = new HtmlPeer(ElementTags.PARAGRAPH, HtmlTags.H[5]);
            peer.AddValue(ElementTags.SIZE, "10");
            this[peer.Alias] = peer;
            
            peer = new HtmlPeer(ElementTags.LIST, HtmlTags.ORDEREDLIST);
            peer.AddValue(ElementTags.NUMBERED, "true");
            peer.AddValue(ElementTags.SYMBOLINDENT, "20");
            this[peer.Alias] = peer;
            
            peer = new HtmlPeer(ElementTags.LIST, HtmlTags.UNORDEREDLIST);
            peer.AddValue(ElementTags.NUMBERED, "false");
            peer.AddValue(ElementTags.SYMBOLINDENT, "20");
            this[peer.Alias] = peer;
            
            peer = new HtmlPeer(ElementTags.LISTITEM, HtmlTags.LISTITEM);
            this[peer.Alias] = peer;
            
            peer = new HtmlPeer(ElementTags.PHRASE, HtmlTags.I);
            peer.AddValue(ElementTags.STYLE, Markup.CSS_VALUE_ITALIC);
            this[peer.Alias] = peer;
            
            peer = new HtmlPeer(ElementTags.PHRASE, HtmlTags.EM);
            peer.AddValue(ElementTags.STYLE, Markup.CSS_VALUE_ITALIC);
            this[peer.Alias] = peer;
            
            peer = new HtmlPeer(ElementTags.PHRASE, HtmlTags.B);
            peer.AddValue(ElementTags.STYLE, Markup.CSS_VALUE_BOLD);
            this[peer.Alias] = peer;
            
            peer = new HtmlPeer(ElementTags.PHRASE, HtmlTags.STRONG);
            peer.AddValue(ElementTags.STYLE, Markup.CSS_VALUE_BOLD);
            this[peer.Alias] = peer;
            
            peer = new HtmlPeer(ElementTags.PHRASE, HtmlTags.S);
            peer.AddValue(ElementTags.STYLE, Markup.CSS_VALUE_LINETHROUGH);
            this[peer.Alias] = peer;
            
            peer = new HtmlPeer(ElementTags.PHRASE, HtmlTags.CODE);
            peer.AddValue(ElementTags.FONT, FontFactory.COURIER);
            this[peer.Alias] = peer;
            
            peer = new HtmlPeer(ElementTags.PHRASE, HtmlTags.VAR);
            peer.AddValue(ElementTags.FONT, FontFactory.COURIER);
            peer.AddValue(ElementTags.STYLE, Markup.CSS_VALUE_ITALIC);
            this[peer.Alias] = peer;
            
            peer = new HtmlPeer(ElementTags.PHRASE, HtmlTags.U);
            peer.AddValue(ElementTags.STYLE, Markup.CSS_VALUE_UNDERLINE);
            this[peer.Alias] = peer;
            
            peer = new HtmlPeer(ElementTags.CHUNK, HtmlTags.SUP);
            peer.AddValue(ElementTags.SUBSUPSCRIPT, "6.0");
            this[peer.Alias] = peer;
            
            peer = new HtmlPeer(ElementTags.CHUNK, HtmlTags.SUB);
            peer.AddValue(ElementTags.SUBSUPSCRIPT, "-6.0");
            this[peer.Alias] = peer;
            
            peer = new HtmlPeer(ElementTags.HORIZONTALRULE, HtmlTags.HORIZONTALRULE);
            this[peer.Alias] = peer;
            
            peer = new HtmlPeer(ElementTags.TABLE, HtmlTags.TABLE);
            peer.AddAlias(ElementTags.WIDTH, HtmlTags.WIDTH);
            peer.AddAlias(ElementTags.BACKGROUNDCOLOR, HtmlTags.BACKGROUNDCOLOR);
            peer.AddAlias(ElementTags.BORDERCOLOR, HtmlTags.BORDERCOLOR);
            peer.AddAlias(ElementTags.COLUMNS, HtmlTags.COLUMNS);
            peer.AddAlias(ElementTags.CELLPADDING, HtmlTags.CELLPADDING);
            peer.AddAlias(ElementTags.CELLSPACING, HtmlTags.CELLSPACING);
            peer.AddAlias(ElementTags.BORDERWIDTH, HtmlTags.BORDERWIDTH);
            peer.AddAlias(ElementTags.ALIGN, HtmlTags.ALIGN);
            this[peer.Alias] = peer;
            
            peer = new HtmlPeer(ElementTags.ROW, HtmlTags.ROW);
            this[peer.Alias] = peer;
            
            peer = new HtmlPeer(ElementTags.CELL, HtmlTags.CELL);
            peer.AddAlias(ElementTags.WIDTH, HtmlTags.WIDTH);
            peer.AddAlias(ElementTags.BACKGROUNDCOLOR, HtmlTags.BACKGROUNDCOLOR);
            peer.AddAlias(ElementTags.BORDERCOLOR, HtmlTags.BORDERCOLOR);
            peer.AddAlias(ElementTags.COLSPAN, HtmlTags.COLSPAN);
            peer.AddAlias(ElementTags.ROWSPAN, HtmlTags.ROWSPAN);
            peer.AddAlias(ElementTags.NOWRAP, HtmlTags.NOWRAP);
            peer.AddAlias(ElementTags.HORIZONTALALIGN, HtmlTags.HORIZONTALALIGN);
            peer.AddAlias(ElementTags.VERTICALALIGN, HtmlTags.VERTICALALIGN);
            peer.AddValue(ElementTags.HEADER, "false");
            this[peer.Alias] = peer;
            
            peer = new HtmlPeer(ElementTags.CELL, HtmlTags.HEADERCELL);
            peer.AddAlias(ElementTags.WIDTH, HtmlTags.WIDTH);
            peer.AddAlias(ElementTags.BACKGROUNDCOLOR, HtmlTags.BACKGROUNDCOLOR);
            peer.AddAlias(ElementTags.BORDERCOLOR, HtmlTags.BORDERCOLOR);
            peer.AddAlias(ElementTags.COLSPAN, HtmlTags.COLSPAN);
            peer.AddAlias(ElementTags.ROWSPAN, HtmlTags.ROWSPAN);
            peer.AddAlias(ElementTags.NOWRAP, HtmlTags.NOWRAP);
            peer.AddAlias(ElementTags.HORIZONTALALIGN, HtmlTags.HORIZONTALALIGN);
            peer.AddAlias(ElementTags.VERTICALALIGN, HtmlTags.VERTICALALIGN);
            peer.AddValue(ElementTags.HEADER, "true");
            this[peer.Alias] = peer;
            
            peer = new HtmlPeer(ElementTags.IMAGE, HtmlTags.IMAGE);
            peer.AddAlias(ElementTags.URL, HtmlTags.URL);
            peer.AddAlias(ElementTags.ALT, HtmlTags.ALT);
            peer.AddAlias(ElementTags.PLAINWIDTH, HtmlTags.PLAINWIDTH);
            peer.AddAlias(ElementTags.PLAINHEIGHT, HtmlTags.PLAINHEIGHT);
            this[peer.Alias] = peer;
            
            peer = new HtmlPeer(ElementTags.NEWLINE, HtmlTags.NEWLINE);
            this[peer.Alias] = peer;
        }
        
        /**
        * Checks if this is the root tag.
        * @param tag a tagvalue
        * @return true if tag is HTML or html
        */
        public static bool IsHtml(String tag) {
            return Util.EqualsIgnoreCase(HtmlTags.HTML, tag);
        }

        /**
        * Checks if this is the head tag.
        * @param tag a tagvalue
        * @return true if tag is HEAD or head
        */
        public static bool IsHead(String tag) {
            return Util.EqualsIgnoreCase(HtmlTags.HEAD, tag);
        }

        /**
        * Checks if this is the meta tag.
        * @param tag a tagvalue
        * @return true if tag is META or meta
        */
        public static bool IsMeta(String tag) {
            return Util.EqualsIgnoreCase(HtmlTags.META, tag);
        }

        /**
        * Checks if this is the linl tag.
        * @param tag a tagvalue
        * @return true if tag is LINK or link
        */
        public static bool IsLink(String tag) {
            return Util.EqualsIgnoreCase(HtmlTags.LINK, tag);
        }

        /**
        * Checks if this is the title tag.
        * @param tag a tagvalue
        * @return true if tag is TITLE or title
        */
        public static bool IsTitle(String tag) {
            return Util.EqualsIgnoreCase(HtmlTags.TITLE, tag);
        }

        /**
        * Checks if this is the root tag.
        * @param tag a tagvalue
        * @return true if tag is BODY or body
        */
        public static bool IsBody(String tag) {
            return Util.EqualsIgnoreCase(HtmlTags.BODY, tag);
        }

        /**
        * Checks if this is a special tag.
        * @param tag a tagvalue
        * @return true if tag is a HTML, HEAD, META, LINK or BODY tag (case insensitive)
        */
        public static bool IsSpecialTag(String tag) {
            return IsHtml(tag) || IsHead(tag) || IsMeta(tag) || IsLink(tag) || IsBody(tag);
        }
    }
}