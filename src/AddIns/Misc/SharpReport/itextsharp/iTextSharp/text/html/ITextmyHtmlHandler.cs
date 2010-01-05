using System;
using System.Collections;
using System.Globalization;
using System.util;
using iTextSharp.text;
using iTextSharp.text.xml;
using iTextSharp.text.pdf;
using iTextSharp.text.factories;

/*
 * $Id: ITextmyHtmlHandler.cs,v 1.9 2008/05/13 11:25:15 psoares33 Exp $
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

    public class ITextmyHtmlHandler : ITextHandler {
        
    /** These are the properties of the body section. */
        private Properties bodyAttributes = new Properties();
        
    /** This is the status of the table border. */
        private bool tableBorder = false;
        
    /**
    * Constructs a new SAXiTextHandler that will translate all the events
    * triggered by the parser to actions on the <CODE>Document</CODE>-object.
    *
    * @param   document    this is the document on which events must be triggered
    */
        
        public ITextmyHtmlHandler(IDocListener document) : base(document, new HtmlTagMap()) {
        }
        
        public ITextmyHtmlHandler(IDocListener document, BaseFont bf) : base(document, new HtmlTagMap(), bf) {
        }

    /**
    * Constructs a new SAXiTextHandler that will translate all the events
    * triggered by the parser to actions on the <CODE>Document</CODE>-object.
    *
    * @param   document    this is the document on which events must be triggered
    * @param htmlTags a tagmap translating HTML tags to iText tags
    */
        
        public ITextmyHtmlHandler(IDocListener document, Hashtable htmlTags) : base(document, htmlTags) {
        }
        
    /**
    * This method gets called when a start tag is encountered.
    *
    * @param   uri         the Uniform Resource Identifier
    * @param   lname       the local name (without prefix), or the empty string if Namespace processing is not being performed.
    * @param   name        the name of the tag that is encountered
    * @param   attrs       the list of attributes
    */
        
        public override void StartElement(String uri, String lname, String name, Hashtable attrs) {
        //System.err.Println("Start: " + name);
            
            // super.handleStartingTags is replaced with handleStartingTags
            // suggestion by Vu Ngoc Tan/Hop
            name = name.ToLower(CultureInfo.InvariantCulture);
            if (HtmlTagMap.IsHtml(name)) {
                // we do nothing
                return;
            }
            if (HtmlTagMap.IsHead(name)) {
                // we do nothing
                return;
            }
            if (HtmlTagMap.IsTitle(name)) {
                // we do nothing
                return;
            }
            if (HtmlTagMap.IsMeta(name)) {
                // we look if we can change the body attributes
                String meta = null;
                String content = null;
                if (attrs != null) {
                    foreach (String attribute in attrs.Keys) {
                        if (Util.EqualsIgnoreCase(attribute, HtmlTags.CONTENT))
                            content = (String)attrs[attribute];
                        else if (Util.EqualsIgnoreCase(attribute, HtmlTags.NAME))
                            meta = (String)attrs[attribute];
                    }
                }
                if (meta != null && content != null) {
                    bodyAttributes.Add(meta, content);
                }
                return;
            }
            if (HtmlTagMap.IsLink(name)) {
                // we do nothing for the moment, in a later version we could extract the style sheet
                return;
            }
            if (HtmlTagMap.IsBody(name)) {
                // maybe we could extract some info about the document: color, margins,...
                // but that's for a later version...
                XmlPeer peer = new XmlPeer(ElementTags.ITEXT, name);
                peer.AddAlias(ElementTags.TOP, HtmlTags.TOPMARGIN);
                peer.AddAlias(ElementTags.BOTTOM, HtmlTags.BOTTOMMARGIN);
                peer.AddAlias(ElementTags.RIGHT, HtmlTags.RIGHTMARGIN);
                peer.AddAlias(ElementTags.LEFT, HtmlTags.LEFTMARGIN);
                bodyAttributes.AddAll(peer.GetAttributes(attrs));
                HandleStartingTags(peer.Tag, bodyAttributes);
                return;
            }
            if (myTags.ContainsKey(name)) {
                XmlPeer peer = (XmlPeer) myTags[name];
                if (ElementTags.TABLE.Equals(peer.Tag) || ElementTags.CELL.Equals(peer.Tag)) {
                    Properties p = peer.GetAttributes(attrs);
                    String value;
                    if (ElementTags.TABLE.Equals(peer.Tag) && (value = p[ElementTags.BORDERWIDTH]) != null) {
                        if (float.Parse(value, System.Globalization.NumberFormatInfo.InvariantInfo) > 0) {
                            tableBorder = true;
                        }
                    }
                    if (tableBorder) {
                        p.Add(ElementTags.LEFT, "true");
                        p.Add(ElementTags.RIGHT, "true");
                        p.Add(ElementTags.TOP, "true");
                        p.Add(ElementTags.BOTTOM, "true");
                    }
                    HandleStartingTags(peer.Tag, p);
                    return;
                }
                HandleStartingTags(peer.Tag, peer.GetAttributes(attrs));
                return;
            }
            Properties attributes = new Properties();
            if (attrs != null) {
                foreach (String attribute in attrs.Keys) {
                    attributes.Add(attribute.ToLower(CultureInfo.InvariantCulture), ((String)attrs[attribute]).ToLower(CultureInfo.InvariantCulture));
                }
            }
            HandleStartingTags(name, attributes);
        }
        
    /**
    * This method gets called when an end tag is encountered.
    *
    * @param   uri         the Uniform Resource Identifier
    * @param   lname       the local name (without prefix), or the empty string if Namespace processing is not being performed.
    * @param   name        the name of the tag that ends
    */
        
        public override void EndElement(String uri, String lname, String name) {
            //System.err.Println("End: " + name);
            name = name.ToLower(CultureInfo.InvariantCulture);
            if (ElementTags.PARAGRAPH.Equals(name)) {
                document.Add((IElement) stack.Pop());
                return;
            }        
            if (HtmlTagMap.IsHead(name)) {
                // we do nothing
                return;
            }
            if (HtmlTagMap.IsTitle(name)) {
                if (currentChunk != null) {
                    bodyAttributes.Add(ElementTags.TITLE, currentChunk.Content);
                }
                return;
            }
            if (HtmlTagMap.IsMeta(name)) {
                // we do nothing
                return;
            }
            if (HtmlTagMap.IsLink(name)) {
                // we do nothing
                return;
            }
            if (HtmlTagMap.IsBody(name)) {
                // we do nothing
                return;
            }
            if (myTags.ContainsKey(name)) {
                XmlPeer peer = (XmlPeer) myTags[name];
                if (ElementTags.TABLE.Equals(peer.Tag)) {
                    tableBorder = false;
                }
                base.HandleEndingTags(peer.Tag);
                return;
            }
            // super.handleEndingTags is replaced with handleEndingTags
            // suggestion by Ken Auer
            HandleEndingTags(name);
        }
    }
}