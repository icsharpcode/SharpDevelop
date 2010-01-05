using System;
using System.IO;
using System.Collections;

/*
 * $Id: PdfDictionary.cs,v 1.10 2008/05/13 11:25:19 psoares33 Exp $
 * 
 *
 * Copyright 1999, 2000, 2001, 2002 Bruno Lowagie
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
     * <CODE>PdfDictionary</CODE> is the Pdf dictionary object.
     * <P>
     * A dictionary is an associative table containing pairs of objects. The first element
     * of each pair is called the <I>key</I> and the second element is called the <I>value</I>.
     * Unlike dictionaries in the PostScript language, a key must be a <CODE>PdfName</CODE>.
     * A value can be any kind of <CODE>PdfObject</CODE>, including a dictionary. A dictionary is
     * generally used to collect and tie together the attributes of a complex object, with each
     * key-value pair specifying the name and value of an attribute.<BR>
     * A dictionary is represented by two left angle brackets (<<), followed by a sequence of
     * key-value pairs, followed by two right angle brackets (>>).<BR>
     * This object is described in the 'Portable Document Format Reference Manual version 1.3'
     * section 4.7 (page 40-41).
     * <P>
     *
     * @see        PdfObject
     * @see        PdfName
     * @see        BadPdfFormatException
     */

    public class PdfDictionary : PdfObject {
    
        // static membervariables (types of dictionary's)
    
        /** This is a possible type of dictionary */
        public static PdfName FONT = PdfName.FONT;
    
        /** This is a possible type of dictionary */
        public static PdfName OUTLINES = PdfName.OUTLINES;
    
        /** This is a possible type of dictionary */
        public static PdfName PAGE = PdfName.PAGE;
    
        /** This is a possible type of dictionary */
        public static PdfName PAGES = PdfName.PAGES;
    
        /** This is a possible type of dictionary */
        public static PdfName CATALOG = PdfName.CATALOG;
    
        // membervariables
    
        /** This is the type of this dictionary */
        private PdfName dictionaryType = null;
    
        /** This is the hashmap that contains all the values and keys of the dictionary */
        protected internal Hashtable hashMap;
    
        // constructors
    
        /**
         * Constructs an empty <CODE>PdfDictionary</CODE>-object.
         */
    
        public PdfDictionary() : base(DICTIONARY) {
            hashMap = new Hashtable();
        }
    
        /**
         * Constructs a <CODE>PdfDictionary</CODE>-object of a certain type.
         *
         * @param        type    a <CODE>PdfName</CODE>
         */
    
        public PdfDictionary(PdfName type) : this() {
            dictionaryType = type;
            Put(PdfName.TYPE, dictionaryType);
        }
    
        // methods overriding some methods in PdfObject
    
        /**
         * Returns the PDF representation of this <CODE>PdfDictionary</CODE>.
         *
         * @return        an array of <CODE>byte</CODE>
         */
        public override void ToPdf(PdfWriter writer, Stream os) {
            os.WriteByte((byte)'<');
            os.WriteByte((byte)'<');

            // loop over all the object-pairs in the Hashtable
            PdfObject value;
            foreach (PdfName key in hashMap.Keys) {
                value = (PdfObject) hashMap[key];
                key.ToPdf(writer, os);
                int type = value.Type;
                if (type != PdfObject.ARRAY && type != PdfObject.DICTIONARY && type != PdfObject.NAME && type != PdfObject.STRING)
                    os.WriteByte((byte)' ');
                value.ToPdf(writer, os);
            }
            os.WriteByte((byte)'>');
            os.WriteByte((byte)'>');
        }
    
    
        // methods concerning the Hashtable member value
    
        /**
         * Adds a <CODE>PdfObject</CODE> and its key to the <CODE>PdfDictionary</CODE>.
         * If the value is <CODE>null</CODE> or <CODE>PdfNull</CODE> the key is deleted.
         *
         * @param        key        key of the entry (a <CODE>PdfName</CODE>)
         * @param        value    value of the entry (a <CODE>PdfObject</CODE>)
         */
    
        public void Put(PdfName key, PdfObject value) {
            if (value == null || value.IsNull())
                hashMap.Remove(key);
            else
                hashMap[key] = value;
        }
    
        /**
         * Adds a <CODE>PdfObject</CODE> and its key to the <CODE>PdfDictionary</CODE>.
         * If the value is null it does nothing.
         *
         * @param        key        key of the entry (a <CODE>PdfName</CODE>)
         * @param        value    value of the entry (a <CODE>PdfObject</CODE>)
         */
        public void PutEx(PdfName key, PdfObject value) {
            if (value == null)
                return;
            Put(key, value);
        }
    
        /**
         * Removes a <CODE>PdfObject</CODE> and its key from the <CODE>PdfDictionary</CODE>.
         *
         * @param        key        key of the entry (a <CODE>PdfName</CODE>)
         */
    
        public void Remove(PdfName key) {
            hashMap.Remove(key);
        }
    
        /**
         * Gets a <CODE>PdfObject</CODE> with a certain key from the <CODE>PdfDictionary</CODE>.
         *
         * @param        key        key of the entry (a <CODE>PdfName</CODE>)
         * @return        the previous </CODE>PdfObject</CODE> corresponding with the <VAR>key</VAR>
         */
    
        public PdfObject Get(PdfName key) {
            return (PdfObject) hashMap[key];
        }
    
        // methods concerning the type of Dictionary
    
        /**
         *  Checks if a <CODE>Dictionary</CODE> is of the type FONT.
         *
         * @return        <CODE>true</CODE> if it is, <CODE>false</CODE> if it isn't.
         */
    
        public bool IsFont() {
            return FONT.Equals(dictionaryType);
        }
    
        /**
         *  Checks if a <CODE>Dictionary</CODE> is of the type PAGE.
         *
         * @return        <CODE>true</CODE> if it is, <CODE>false</CODE> if it isn't.
         */
    
        public bool IsPage() {
            return PAGE.Equals(dictionaryType);
        }
    
        /**
         *  Checks if a <CODE>Dictionary</CODE> is of the type PAGES.
         *
         * @return        <CODE>true</CODE> if it is, <CODE>false</CODE> if it isn't.
         */
    
        public bool IsPages() {
            return PAGES.Equals(dictionaryType);
        }
    
        /**
         *  Checks if a <CODE>Dictionary</CODE> is of the type CATALOG.
         *
         * @return        <CODE>true</CODE> if it is, <CODE>false</CODE> if it isn't.
         */
    
        public bool IsCatalog() {
            return CATALOG.Equals(dictionaryType);
        }
    
        /**
         *  Checks if a <CODE>Dictionary</CODE> is of the type OUTLINES.
         *
         * @return        <CODE>true</CODE> if it is, <CODE>false</CODE> if it isn't.
         */
    
        public bool IsOutlineTree() {
            return OUTLINES.Equals(dictionaryType);
        }
    
        public void Merge(PdfDictionary other) {
            foreach (object key in other.hashMap.Keys) {
                hashMap[key] = other.hashMap[key];
            }
        }
    
        public void MergeDifferent(PdfDictionary other) {
            foreach (Object key in other.hashMap.Keys) {
                if (!hashMap.ContainsKey(key)) {
                    hashMap[key] = other.hashMap[key];
                }
            }
        }

        public ICollection Keys {
            get {
                return hashMap.Keys;
            }
        }

        public int Size {
            get {
                return hashMap.Count;
            }
        }
    
        public bool Contains(PdfName key) {
            return hashMap.ContainsKey(key);
        }

        public virtual IDictionaryEnumerator GetEnumerator() {
            return hashMap.GetEnumerator();
        }

        public override String ToString() {
            if (Get(PdfName.TYPE) == null) return "Dictionary";
    	    return "Dictionary of type: " + Get(PdfName.TYPE);
        }

        /**
        * This function behaves the same as 'get', but will never return an indirect reference,
        * it will always look such references up and return the actual object.
        * @param key 
        * @return null, or a non-indirect object
        */
        public PdfObject GetDirectObject(PdfName key) {
            return PdfReader.GetPdfObject(Get(key));
        }
        
        /**
        * All the getAs functions will return either null, or the specified object type
        * This function will automatically look up indirect references. There's one obvious
        * exception, the one that will only return an indirect reference.  All direct objects
        * come back as a null.
        * Mark A Storer (2/17/06)
        * @param key
        * @return the appropriate object in its final type, or null
        */
        public PdfDictionary GetAsDict(PdfName key) {
            PdfDictionary dict = null;
            PdfObject orig = GetDirectObject(key);
            if (orig != null && orig.IsDictionary())
                dict = (PdfDictionary) orig;
            return dict;
        }
        
        public PdfArray GetAsArray(PdfName key) {
            PdfArray array = null;
            PdfObject orig = GetDirectObject(key);
            if (orig != null && orig.IsArray())
                array = (PdfArray) orig;
            return array;
        }
        
        public PdfStream GetAsStream(PdfName key) {
            PdfStream stream = null;
            PdfObject orig = GetDirectObject(key);
            if (orig != null && orig.IsStream())
                stream = (PdfStream) orig;
            return stream;
        }
        
        public PdfString GetAsString(PdfName key) {
            PdfString str = null;
            PdfObject orig = GetDirectObject(key);
            if (orig != null && orig.IsString())
                str = (PdfString) orig;
            return str;
        }
        
        public PdfNumber GetAsNumber(PdfName key) {
            PdfNumber number = null;
            PdfObject orig = GetDirectObject(key);
            if (orig != null && orig.IsNumber())
                number = (PdfNumber) orig;
            return number;
        }
        
        public PdfName GetAsName(PdfName key) {
            PdfName name = null;
            PdfObject orig = GetDirectObject(key);
            if (orig != null && orig.IsName())
                name = (PdfName) orig;
            return name;
        }
        
        public PdfBoolean GetAsBoolean(PdfName key) {
            PdfBoolean b = null;
            PdfObject orig = GetDirectObject(key);
            if (orig != null && orig.IsBoolean())
                b = (PdfBoolean)orig;
            return b;
        }
        
        public PdfIndirectReference GetAsIndirectObject( PdfName key ) {
            PdfIndirectReference refi = null;
            PdfObject orig = Get(key); // not getDirect this time.
            if (orig != null && orig.IsIndirect())
                refi = (PdfIndirectReference) orig;
            return refi;
        }
    }
}
