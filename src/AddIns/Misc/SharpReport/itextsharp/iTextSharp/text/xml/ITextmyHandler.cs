using System;
using System.Collections;
using System.util;

using iTextSharp.text;

/*
 * $Id: ITextmyHandler.cs,v 1.5 2008/05/13 11:26:12 psoares33 Exp $
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

namespace iTextSharp.text.xml {

    /// <summary>
    /// The <CODE>iTextmyHandler</CODE>-class maps several XHTML-tags to iText-objects.
    /// </summary>
    public class ITextmyHandler : ITextHandler {
    
        /// <summary>
        /// Constructs a new iTextHandler that will translate all the events
        /// triggered by the parser to actions on the <CODE>Document</CODE>-object.
        /// </summary>
        /// <param name="document">this is the document on which events must be triggered</param>
        /// <param name="myTags">a map of tags</param>
        public ITextmyHandler(IDocListener document, Hashtable myTags) : base(document, myTags) {
        }
    
        /// <summary>
        /// This method gets called when a start tag is encountered.
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="lname"></param>
        /// <param name="name">the name of the tag that is encountered</param>
        /// <param name="attrs">the list of attributes</param>
        public override void StartElement(String uri, String lname, String name, Hashtable attrs) {
            if (myTags.ContainsKey(name)) {
                XmlPeer peer = (XmlPeer) myTags[name];
                HandleStartingTags(peer.Tag, peer.GetAttributes(attrs));
            }
            else {
                Properties attributes = new Properties();
                if (attrs != null) {
                    foreach (string key in attrs.Keys) {
                        attributes.Add(key, (string)attrs[key]);
                    }
                }
                HandleStartingTags(name, attributes);
            }
        }
    
        /// <summary>
        /// This method gets called when an end tag is encountered.
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="lname"></param>
        /// <param name="name">the name of the tag that ends</param>
        public override void EndElement(String uri, String lname, String name) {
            if (myTags.ContainsKey(name)) {
                XmlPeer peer = (XmlPeer) myTags[name];
                HandleEndingTags(peer.Tag);
            }
            else {
                HandleEndingTags(name);
            }
        }
    }
}