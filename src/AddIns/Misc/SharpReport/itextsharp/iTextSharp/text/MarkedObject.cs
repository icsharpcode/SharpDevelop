using System;
using System.Collections;
using System.util;
/*
 * $Id: MarkedObject.cs,v 1.4 2008/05/13 11:25:11 psoares33 Exp $
 * 
 *
 * Copyright 2007 by Bruno Lowagie.
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
 * the Initial Developer are Copyright (C) 1999-2007 by Bruno Lowagie.
 * All Rights Reserved.
 * Co-Developer of the code is Paulo Soares. Portions created by the Co-Developer
 * are Copyright (C) 2000-2007 by Paulo Soares. All Rights Reserved.
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

    /**
    * Wrapper that allows to add properties to 'basic building block' objects.
    * Before iText 1.5 every 'basic building block' implemented the MarkupAttributes interface.
    * By setting attributes, you could add markup to the corresponding XML and/or HTML tag.
    * This functionality was hardly used by anyone, so it was removed, and replaced by
    * the MarkedObject functionality.
    */

    public class MarkedObject : IElement {

        /** The element that is wrapped in a MarkedObject. */
        protected internal IElement element;

        /** Contains extra markupAttributes */
        protected internal Properties markupAttributes = new Properties();
            
        /**
        * This constructor is for internal use only.
        */
        protected MarkedObject() {
            element = null;
        }
        
        /**
        * Creates a MarkedObject.
        */
        public MarkedObject(IElement element) {
            this.element = element;
        }
        
        /**
        * Gets all the chunks in this element.
        *
        * @return  an <CODE>ArrayList</CODE>
        */
        public virtual ArrayList Chunks {
            get {
                return element.Chunks;
            }
        }

        /**
        * Processes the element by adding it (or the different parts) to an
        * <CODE>ElementListener</CODE>.
        *
        * @param       listener        an <CODE>ElementListener</CODE>
        * @return <CODE>true</CODE> if the element was processed successfully
        */
        public virtual bool Process(IElementListener listener) {
            try {
                return listener.Add(element);
            }
            catch (DocumentException) {
                return false;
            }
        }
        
        /**
        * Gets the type of the text element.
        *
        * @return  a type
        */
        public virtual int Type {
            get {
                return Element.MARKED;
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

        /**
        * @return the markupAttributes
        */
        public virtual Properties MarkupAttributes {
            get {
                return markupAttributes;
            }
        }
        
        public virtual void SetMarkupAttribute(String key, String value) {
            markupAttributes.Add(key, value);
        }

    }
}