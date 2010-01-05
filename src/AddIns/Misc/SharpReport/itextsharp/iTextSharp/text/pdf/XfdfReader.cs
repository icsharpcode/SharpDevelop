using System;
using System.IO;
using System.Collections;
using iTextSharp.text.xml.simpleparser;
/*
 *
 * Copyright 2004 by Leonard Rosenthol.
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

namespace iTextSharp.text.pdf {
    /**
    * Reads a XFDF.
    * @author Leonard Rosenthol (leonardr@pdfsages.com)
    */
    public class XfdfReader : ISimpleXMLDocHandler {
        // stuff used during parsing to handle state
        private bool foundRoot = false;
        private Stackr fieldNames = new Stackr();
        private Stackr fieldValues = new Stackr();

        // storage for the field list and their values
        internal Hashtable fields;
        
        // storage for the path to referenced PDF, if any
        internal String fileSpec;
        
    /** Reads an XFDF form.
        * @param filename the file name of the form
        * @throws IOException on error
        */    
        public XfdfReader(String filename) {
            FileStream fin = null;
            try {
                fin = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read);
                SimpleXMLParser.Parse(this, fin);
            }
            finally {
                try{if (fin != null) fin.Close();}catch{}
            }
        }
        
        /** Reads an XFDF form.
        * @param xfdfIn the byte array with the form
        * @throws IOException on error
        */    
        public XfdfReader(byte[] xfdfIn) {
            SimpleXMLParser.Parse( this, new MemoryStream(xfdfIn));
    }
        
        /** Gets all the fields. The map is keyed by the fully qualified
        * field name and the value is a merged <CODE>PdfDictionary</CODE>
        * with the field content.
        * @return all the fields
        */    
        public Hashtable Fields {
            get {
                return fields;
            }
        }
        
        /** Gets the field value.
        * @param name the fully qualified field name
        * @return the field's value
        */    
        public String GetField(String name) {
            return (String)fields[name];
        }
        
        /** Gets the field value or <CODE>null</CODE> if the field does not
        * exist or has no value defined.
        * @param name the fully qualified field name
        * @return the field value or <CODE>null</CODE>
        */    
        public String GetFieldValue(String name) {
            String field = (String)fields[name];
            if (field == null)
                return null;
            else
                return field;
        }
        
        /** Gets the PDF file specification contained in the FDF.
        * @return the PDF file specification contained in the FDF
        */    
        public String FileSpec {
            get {
                return fileSpec;
            }
        }

        /**
        * Called when a start tag is found.
        * @param tag the tag name
        * @param h the tag's attributes
        */    
        public void StartElement(String tag, Hashtable h) {
            if ( !foundRoot ) {
                if (!tag.Equals("xfdf"))
                    throw new Exception("Root element is not Bookmark.");
                else 
                    foundRoot = true;
            }

            if ( tag.Equals("xfdf") ){
                
            } else if ( tag.Equals("f") ) {
                fileSpec = (String)h[ "href" ];
            } else if ( tag.Equals("fields") ) {
                fields = new Hashtable();     // init it!
            } else if ( tag.Equals("field") ) {
                String  fName = (String) h[ "name" ];
                fieldNames.Push( fName );
            } else if ( tag.Equals("value") ) {
                fieldValues.Push("");
            }
        }
        /**
        * Called when an end tag is found.
        * @param tag the tag name
        */    
        public void EndElement(String tag) {
            if ( tag.Equals("value") ) {
                String  fName = "";
                for (int k = 0; k < fieldNames.Count; ++k) {
                    fName += "." + (String)fieldNames[k];
                }
                if (fName.StartsWith("."))
                    fName = fName.Substring(1);
                String  fVal = (String) fieldValues.Pop();
                fields[fName] = fVal;
            }
            else if (tag.Equals("field") ) {
                if (fieldNames.Count != 0)
                    fieldNames.Pop();
            }
        }
        
        /**
        * Called when the document starts to be parsed.
        */    
        public void StartDocument()
        {
            fileSpec = "";  // and this too...
        }
        /**
        * Called after the document is parsed.
        */    
        public void EndDocument()
        {
            
        }
        /**
        * Called when a text element is found.
        * @param str the text element, probably a fragment.
        */    
        public void Text(String str)
        {
            if (fieldNames.Count == 0 || fieldValues.Count == 0)
                return;
            
            String val = (String)fieldValues.Pop();
            val += str;
            fieldValues.Push(val);
        }

        internal class Stackr : ArrayList {
            internal void Push(object obj) {
                Add(obj);
            }

            internal object Pop() {
                if (Count == 0)
                    throw new InvalidOperationException("The stack is empty.");
                object obj = this[Count - 1];
                RemoveAt(Count - 1);
                return obj;
            }
        }
    }
}