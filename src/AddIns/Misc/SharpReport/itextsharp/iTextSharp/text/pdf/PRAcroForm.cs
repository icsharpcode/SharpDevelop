using System;
using System.Collections;
/*
 * $Id: PRAcroForm.cs,v 1.5 2008/05/13 11:25:23 psoares33 Exp $
 * 
 *
 * Copyright 2001, 2002 by Paulo Soares.
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
 * This class written by Mark Thompson, Copyright (C) 2002 by Mark Thompson.
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
    * This class captures an AcroForm on input. Basically, it extends Dictionary
    * by indexing the fields of an AcroForm
    * @author Mark Thompson
    */

    public class PRAcroForm : PdfDictionary {
        
        /**
        * This class holds the information for a single field
        */
        public class FieldInformation {
            internal String name;
            internal PdfDictionary info;
            internal PRIndirectReference refi;
            
            internal FieldInformation(String name, PdfDictionary info, PRIndirectReference refi) {
                this.name = name; this.info = info; this.refi = refi;
            }
            public String Name {
                get {
                    return name; 
                }
            }
            public PdfDictionary Info {
                get {
                    return info; 
                }
            }
            public PRIndirectReference Ref {
                get {
                    return refi; 
                }
            }
        }

        internal ArrayList fields;
        internal ArrayList stack;
        internal Hashtable fieldByName;
        internal PdfReader reader;
        
        /**
        * Constructor
        * @param reader reader of the input file
        */
        public PRAcroForm(PdfReader reader) {
            this.reader = reader;
            fields = new ArrayList();
            fieldByName = new Hashtable();
            stack = new ArrayList();
        }
        /**
        * Number of fields found
        * @return size
        */
        public new int Size {
            get {
                return fields.Count;
            }
        }
        
        public ArrayList Fields {
            get {
                return fields;
            }
        }
        
        public FieldInformation GetField(String name) {
            return (FieldInformation)fieldByName[name];
        }
        
        /**
        * Given the title (/T) of a reference, return the associated reference
        * @param name a string containing the path
        * @return a reference to the field, or null
        */
        public PRIndirectReference GetRefByName(String name) {
            FieldInformation fi = (FieldInformation)fieldByName[name];
            if (fi == null) return null;
            return fi.Ref;
        }
        /**
        * Read, and comprehend the acroform
        * @param root the docment root
        */
        public void ReadAcroForm(PdfDictionary root) {
            if (root == null)
                return;
            hashMap = root.hashMap;
            PushAttrib(root);
            PdfArray fieldlist = (PdfArray)PdfReader.GetPdfObjectRelease(root.Get(PdfName.FIELDS));
            IterateFields(fieldlist, null, null);
        }
        
        /**
        * After reading, we index all of the fields. Recursive.
        * @param fieldlist An array of fields
        * @param fieldDict the last field dictionary we encountered (recursively)
        * @param title the pathname of the field, up to this point or null
        */
        protected void IterateFields(PdfArray fieldlist, PRIndirectReference fieldDict, String title) {
            foreach (PRIndirectReference refi in fieldlist.ArrayList) {
                PdfDictionary dict = (PdfDictionary) PdfReader.GetPdfObjectRelease(refi);
                
                // if we are not a field dictionary, pass our parent's values
                PRIndirectReference myFieldDict = fieldDict;
                String myTitle = title;
                PdfString tField = (PdfString)dict.Get(PdfName.T);
                bool isFieldDict = tField != null;
                
                if (isFieldDict) {
                    myFieldDict = refi;
                    if (title == null) myTitle = tField.ToString();
                    else myTitle = title + '.' + tField.ToString();
                }
                
                PdfArray kids = (PdfArray)dict.Get(PdfName.KIDS);
                if (kids != null) {
                    PushAttrib(dict);
                    IterateFields(kids, myFieldDict, myTitle);
                    stack.RemoveAt(stack.Count - 1);   // pop
                }
                else {          // leaf node
                    if (myFieldDict != null) {
                        PdfDictionary mergedDict = (PdfDictionary)stack[stack.Count - 1];
                        if (isFieldDict)
                            mergedDict = MergeAttrib(mergedDict, dict);
                        
                        mergedDict.Put(PdfName.T, new PdfString(myTitle));
                        FieldInformation fi = new FieldInformation(myTitle, mergedDict, myFieldDict);
                        fields.Add(fi);
                        fieldByName[myTitle] = fi;
                    }
                }
            }
        }
        /**
        * merge field attributes from two dictionaries
        * @param parent one dictionary
        * @param child the other dictionary
        * @return a merged dictionary
        */
        protected PdfDictionary MergeAttrib(PdfDictionary parent, PdfDictionary child) {
            PdfDictionary targ = new PdfDictionary();
            if (parent != null) targ.Merge(parent);
            
            foreach (PdfName key in child.Keys) {
                if (key.Equals(PdfName.DR) || key.Equals(PdfName.DA) ||
                key.Equals(PdfName.Q)  || key.Equals(PdfName.FF) ||
                key.Equals(PdfName.DV) || key.Equals(PdfName.V)
                || key.Equals(PdfName.FT)
                || key.Equals(PdfName.F)) {
                    targ.Put(key,child.Get(key));
                }
            }
            return targ;
        }
        /**
        * stack a level of dictionary. Merge in a dictionary from this level
        */
        protected void PushAttrib(PdfDictionary dict) {
            PdfDictionary dic = null;
            if (stack.Count != 0) {
                dic = (PdfDictionary)stack[stack.Count - 1];
            }
            dic = MergeAttrib(dic, dict);
            stack.Add(dic);
        }
    }
}
