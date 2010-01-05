using System;
using System.Text;
using System.util;
/*
 * $Id: XmpSchema.cs,v 1.5 2008/05/13 11:26:16 psoares33 Exp $
 * 
 *
 * Copyright 2005 by Bruno Lowagie.
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
 * the Initial Developer are Copyright (C) 1999-2005 by Bruno Lowagie.
 * All Rights Reserved.
 * Co-Developer of the code is Paulo Soares. Portions created by the Co-Developer
 * are Copyright (C) 2000-2005 by Paulo Soares. All Rights Reserved.
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
 * of this file under either the MPL or the GNU LIBRARY GENERAL PUBLIC LICENSE 
 *
 * This library is free software; you can redistribute it and/or modify it
 * under the terms of the MPL as stated above or under the terms of the GNU
 * Library General Public License as published by the Free Software Foundation;
 * either version 2 of the License, or any later version.
 *
 * This library is distributed in the hope that it will be useful, but WITHOUT
 * ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS
 * FOR A PARTICULAR PURPOSE. See the GNU LIBRARY GENERAL PUBLIC LICENSE for more
 * details.
 *
 * If you didn't download this code from the following link, you should check if
 * you aren't using an obsolete version:
 * http://www.lowagie.com/iText/
 */

namespace iTextSharp.text.xml.xmp {

    /**
    * Abstract superclass of the XmpSchemas supported by iText.
    */
    public abstract class XmpSchema : Properties {
        /** the namesspace */
        protected String xmlns;
        
        /** Constructs an XMP schema. 
        * @param xmlns
        */
        public XmpSchema(String xmlns) : base() {
            this.xmlns = xmlns;
        }
        /**
        * The String representation of the contents.
        * @return a String representation.
        */
        public override String ToString() {
            StringBuilder buf = new StringBuilder();
            foreach (object key in Keys) {
                Process(buf, key);
            }
            return buf.ToString();
        }
        /**
        * Processes a property
        * @param buf
        * @param p
        */
        protected void Process(StringBuilder buf, Object p) {
            buf.Append('<');
            buf.Append(p);
            buf.Append('>');
            buf.Append(this[p.ToString()]);
            buf.Append("</");
            buf.Append(p);
            buf.Append('>');
        }

        /**
        * @return Returns the xmlns.
        */
        public String Xmlns {
            get {
                return xmlns;
            }
        }

        /**
        * @param key
        * @param value
        * @return the previous property (null if there wasn't one)
        */
        public void AddProperty(String key, String value) {
            this[key] = value;
        }
        
        public override string this[string key] {
            set {
                base[key] = Escape(value);
            }
        }
        
        public void SetProperty(string key, XmpArray value) {
            base[key] = value.ToString();
        }
        
        /**
        * @see java.util.Properties#setProperty(java.lang.String, java.lang.String)
        * 
        * @param key
        * @param value
        * @return the previous property (null if there wasn't one)
        */
        public void SetProperty(String key, LangAlt value) {
            base[key] = value.ToString();
        }
        
        /**
        * @param content
        * @return
        */
        public static String Escape(String content) {
            StringBuilder buf = new StringBuilder();
            for (int i = 0; i < content.Length; i++) {
                switch (content[i]) {
                case '<':
                    buf.Append("&lt;");
                    break;
                case '>':
                    buf.Append("&gt;");
                    break;
                case '\'':
                    buf.Append("&apos;");
                    break;
                case '\"':
                    buf.Append("&quot;");
                    break;
                case '&':
                    buf.Append("&amp;");
                    break;
                default:
                    buf.Append(content[i]);
                    break;
                }
            }
            return buf.ToString();
        }
    }
}