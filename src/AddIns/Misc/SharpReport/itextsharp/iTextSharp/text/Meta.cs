using System;
using System.Text;
using System.Collections;
using System.util;

/*
 * $Id: Meta.cs,v 1.6 2008/05/13 11:25:12 psoares33 Exp $
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
    /// This is an Element that contains
    /// some meta information about the document.
    /// </summary>
    /// <remarks>
    /// An object of type Meta can not be constructed by the user.
    /// Userdefined meta information should be placed in a Header-object.
    /// Meta is reserved for: Subject, Keywords, Author, Title, Producer
    /// and Creationdate information.
    /// </remarks>
    /// <seealso cref="T:iTextSharp.text.Element"/>
    /// <seealso cref="T:iTextSharp.text.Header"/>
    public class Meta : IElement {
    
        // membervariables
    
        ///<summary> This is the type of Meta-information this object contains. </summary>
        private int type;
    
        ///<summary> This is the content of the Meta-information. </summary>
        private StringBuilder content;

        // constructors
    
        /// <summary>
        /// Constructs a Meta.
        /// </summary>
        /// <param name="type">the type of meta-information</param>
        /// <param name="content">the content</param>
        public Meta(int type, string content) {
            this.type = type;
            this.content = new StringBuilder(content);
        }
    
        /// <summary>
        /// Constructs a Meta.
        /// </summary>
        /// <param name="tag">the tagname of the meta-information</param>
        /// <param name="content">the content</param>
        public Meta(string tag, string content) {
            this.type = Meta.GetType(tag);
            this.content = new StringBuilder(content);
        }
    
        // implementation of the Element-methods
    
        /// <summary>
        /// Processes the element by adding it (or the different parts) to a
        /// IElementListener.
        /// </summary>
        /// <param name="listener">the IElementListener</param>
        /// <returns>true if the element was processed successfully</returns>
        public bool Process(IElementListener listener) {
            try {
                return listener.Add(this);
            }
            catch (DocumentException) {
                return false;
            }
        }
    
        /// <summary>
        /// Gets the type of the text element.
        /// </summary>
        /// <value>a type</value>
        public int Type {
            get {
                return type;
            }
        }
    
        /// <summary>
        /// Gets all the chunks in this element.
        /// </summary>
        /// <value>an ArrayList</value>
        public ArrayList Chunks {
            get {
                return new ArrayList();
            }
        }
    
        /**
        * @see com.lowagie.text.Element#isContent()
        * @since   iText 2.0.8
        */
        public bool IsContent() {
            return false;
        }

        /**
        * @see com.lowagie.text.Element#isNestable()
        * @since   iText 2.0.8
        */
        public bool IsNestable() {
            return false;
        }

        // methods
    
        /// <summary>
        /// appends some text to this Meta.
        /// </summary>
        /// <param name="str">a string</param>
        /// <returns>a StringBuilder</returns>
        public StringBuilder Append(string str) {
            return content.Append(str);
        }
    
        // methods to retrieve information
    
        /// <summary>
        /// Returns the content of the meta information.
        /// </summary>
        /// <value>a string</value>
        public string Content {
            get {
                return content.ToString();
            }
        }
    
        /// <summary>
        /// Returns the name of the meta information.
        /// </summary>
        /// <value>a string</value>
        public virtual string Name {
            get {
                switch (type) {
                    case Element.SUBJECT:
                        return ElementTags.SUBJECT;
                    case Element.KEYWORDS:
                        return ElementTags.KEYWORDS;
                    case Element.AUTHOR:
                        return ElementTags.AUTHOR;
                    case Element.TITLE:
                        return ElementTags.TITLE;
                    case Element.PRODUCER:
                        return ElementTags.PRODUCER;
                    case Element.CREATIONDATE:
                        return ElementTags.CREATIONDATE;
                    default:
                        return ElementTags.UNKNOWN;
                }
            }
        }
    
        /// <summary>
        /// Returns the name of the meta information.
        /// </summary>
        /// <param name="tag">name to match</param>
        /// <returns>a string</returns>
        public static int GetType(string tag) {
            if (ElementTags.SUBJECT.Equals(tag)) {
                return Element.SUBJECT;
            }
            if (ElementTags.KEYWORDS.Equals(tag)) {
                return Element.KEYWORDS;
            }
            if (ElementTags.AUTHOR.Equals(tag)) {
                return Element.AUTHOR;
            }
            if (ElementTags.TITLE.Equals(tag)) {
                return Element.TITLE;
            }
            if (ElementTags.PRODUCER.Equals(tag)) {
                return Element.PRODUCER;
            }
            if (ElementTags.CREATIONDATE.Equals(tag)) {
                return Element.CREATIONDATE;
            }
            return Element.HEADER;
        }
    
    
        public override string ToString() {
            return base.ToString();
        }
    }
}
