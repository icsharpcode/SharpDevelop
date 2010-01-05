using System;
using System.Collections;
using System.util;
using iTextSharp.text;
using iTextSharp.text.factories;

/*
 * $Id: PdfPageLabels.cs,v 1.9 2008/05/13 11:25:21 psoares33 Exp $
 * 
 *
 * Copyright 2001, 2002 Paulo Soares
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

    /** Page labels are used to identify each
     * page visually on the screen or in print.
     * @author  Paulo Soares (psoares@consiste.pt)
     */
    public class PdfPageLabels {

        /** Logical pages will have the form 1,2,3,...
         */    
        public const int DECIMAL_ARABIC_NUMERALS = 0;
        /** Logical pages will have the form I,II,III,IV,...
         */    
        public const int UPPERCASE_ROMAN_NUMERALS = 1;
        /** Logical pages will have the form i,ii,iii,iv,...
         */    
        public const int LOWERCASE_ROMAN_NUMERALS = 2;
        /** Logical pages will have the form of uppercase letters
         * (A to Z for the first 26 pages, AA to ZZ for the next 26, and so on)
         */    
        public const int UPPERCASE_LETTERS = 3;
        /** Logical pages will have the form of uppercase letters
         * (a to z for the first 26 pages, aa to zz for the next 26, and so on)
         */    
        public const int LOWERCASE_LETTERS = 4;
        /** No logical page numbers are generated but fixed text may
         * still exist
         */    
        public const int EMPTY = 5;
        /** Dictionary values to set the logical page styles
         */    
        internal static PdfName[] numberingStyle = {PdfName.D, PdfName.R,
                    new PdfName("r"), PdfName.A, new PdfName("a")};
        /** The sequence of logical pages. Will contain at least a value for page 1
         */    
        internal Hashtable map;
    
        /** Creates a new PdfPageLabel with a default logical page 1
         */
        public PdfPageLabels() {
            map = new Hashtable();
            AddPageLabel(1, DECIMAL_ARABIC_NUMERALS, null, 1);
        }

        /** Adds or replaces a page label.
         * @param page the real page to start the numbering. First page is 1
         * @param numberStyle the numbering style such as LOWERCASE_ROMAN_NUMERALS
         * @param text the text to prefix the number. Can be <CODE>null</CODE> or empty
         * @param firstPage the first logical page number
         */    
        public void AddPageLabel(int page, int numberStyle, string text, int firstPage) {
            if (page < 1 || firstPage < 1)
                throw new ArgumentException("In a page label the page numbers must be greater or equal to 1.");
            PdfDictionary dic = new PdfDictionary();
            if (numberStyle >= 0 && numberStyle < numberingStyle.Length)
                dic.Put(PdfName.S, numberingStyle[numberStyle]);
            if (text != null)
                dic.Put(PdfName.P, new PdfString(text, PdfObject.TEXT_UNICODE));
            if (firstPage != 1)
                dic.Put(PdfName.ST, new PdfNumber(firstPage));
            map[page - 1] = dic;
        }

        /** Adds or replaces a page label. The first logical page has the default
         * of 1.
         * @param page the real page to start the numbering. First page is 1
         * @param numberStyle the numbering style such as LOWERCASE_ROMAN_NUMERALS
         * @param text the text to prefix the number. Can be <CODE>null</CODE> or empty
         */    
        public void AddPageLabel(int page, int numberStyle, string text) {
            AddPageLabel(page, numberStyle, text, 1);
        }
    
        /** Adds or replaces a page label. There is no text prefix and the first
         * logical page has the default of 1.
         * @param page the real page to start the numbering. First page is 1
         * @param numberStyle the numbering style such as LOWERCASE_ROMAN_NUMERALS
         */    
        public void AddPageLabel(int page, int numberStyle) {
            AddPageLabel(page, numberStyle, null, 1);
        }
    
        /** Adds or replaces a page label.
        */
        public void AddPageLabel(PdfPageLabelFormat format) {
            AddPageLabel(format.physicalPage, format.numberStyle, format.prefix, format.logicalPage);
        }

        /** Removes a page label. The first page lagel can not be removed, only changed.
         * @param page the real page to remove
         */    
        public void RemovePageLabel(int page) {
            if (page <= 1)
                return;
            map.Remove(page - 1);
        }

        /** Gets the page label dictionary to insert into the document.
         * @return the page label dictionary
         */    
        internal PdfDictionary GetDictionary(PdfWriter writer) {
            return PdfNumberTree.WriteTree(map, writer);
        }

        /**
        * Retrieves the page labels from a PDF as an array of String objects.
        * @param reader a PdfReader object that has the page labels you want to retrieve
        * @return  a String array or <code>null</code> if no page labels are present
        */
        public static String[] GetPageLabels(PdfReader reader) {
            
            int n = reader.NumberOfPages;
            
            PdfDictionary dict = reader.Catalog;
            PdfDictionary labels = (PdfDictionary)PdfReader.GetPdfObjectRelease(dict.Get(PdfName.PAGELABELS));
            if (labels == null)
                return null;
            
            String[] labelstrings = new String[n];
            Hashtable numberTree = PdfNumberTree.ReadTree(labels);
            
            int pagecount = 1;
            String prefix = "";
            char type = 'D';
            for (int i = 0; i < n; i++) {
                if (numberTree.ContainsKey(i)) {
                    PdfDictionary d = (PdfDictionary)PdfReader.GetPdfObjectRelease((PdfObject)numberTree[i]);
                    if (d.Contains(PdfName.ST)) {
                        pagecount = ((PdfNumber)d.Get(PdfName.ST)).IntValue;
                    }
                    else {
                        pagecount = 1;
                    }
                    if (d.Contains(PdfName.P)) {
                        prefix = ((PdfString)d.Get(PdfName.P)).ToUnicodeString();
                    }
                    if (d.Contains(PdfName.S)) {
                        type = ((PdfName)d.Get(PdfName.S)).ToString()[1];
                    }
                }
                switch (type) {
                default:
                    labelstrings[i] = prefix + pagecount;
                    break;
                case 'R':
                    labelstrings[i] = prefix + RomanNumberFactory.GetUpperCaseString(pagecount);
                    break;
                case 'r':
                    labelstrings[i] = prefix + RomanNumberFactory.GetLowerCaseString(pagecount);
                    break;
                case 'A':
                    labelstrings[i] = prefix + RomanAlphabetFactory.GetUpperCaseString(pagecount);
                    break;
                case 'a':
                    labelstrings[i] = prefix + RomanAlphabetFactory.GetLowerCaseString(pagecount);
                    break;
                }
                pagecount++;
            }
            return labelstrings;
        }

        /**
        * Retrieves the page labels from a PDF as an array of {@link PdfPageLabelFormat} objects.
        * @param reader a PdfReader object that has the page labels you want to retrieve
        * @return  a PdfPageLabelEntry array, containing an entry for each format change
        * or <code>null</code> if no page labels are present
        */
        public static PdfPageLabelFormat[] GetPageLabelFormats(PdfReader reader) {
            PdfDictionary dict = reader.Catalog;
            PdfDictionary labels = (PdfDictionary)PdfReader.GetPdfObjectRelease(dict.Get(PdfName.PAGELABELS));
            if (labels == null) 
                return null;
            Hashtable numberTree = PdfNumberTree.ReadTree(labels);
            int[] numbers = new int[numberTree.Count];
            numberTree.Keys.CopyTo(numbers, 0);
            Array.Sort(numbers);
            PdfPageLabelFormat[] formats = new PdfPageLabelFormat[numberTree.Count];
            String prefix;
            int numberStyle;
            int pagecount;
            for (int k = 0; k < numbers.Length; ++k) {
                int key = numbers[k];
                PdfDictionary d = (PdfDictionary)PdfReader.GetPdfObjectRelease((PdfObject)numberTree[key]);
                if (d.Contains(PdfName.ST)) {
                    pagecount = ((PdfNumber)d.Get(PdfName.ST)).IntValue;
                } else {
                    pagecount = 1;
                }
                if (d.Contains(PdfName.P)) {
                    prefix = ((PdfString)d.Get(PdfName.P)).ToUnicodeString();
                } else {
                    prefix = "";
                }
                if (d.Contains(PdfName.S)) {
                    char type = ((PdfName)d.Get(PdfName.S)).ToString()[1];
                    switch (type) {
                        case 'R': numberStyle = UPPERCASE_ROMAN_NUMERALS; break;
                        case 'r': numberStyle = LOWERCASE_ROMAN_NUMERALS; break;
                        case 'A': numberStyle = UPPERCASE_LETTERS; break;
                        case 'a': numberStyle = LOWERCASE_LETTERS; break;
                        default: numberStyle = DECIMAL_ARABIC_NUMERALS; break;
                    }
                } else {
                    numberStyle = EMPTY;
                }
                formats[k] = new PdfPageLabelFormat(key + 1, numberStyle, prefix, pagecount);
            }
            return formats;
        }

        public class PdfPageLabelFormat {
            
            public int physicalPage;
            public int numberStyle;
            public String prefix;
            public int logicalPage;
            
            /** Creates a page label format.
            * @param physicalPage the real page to start the numbering. First page is 1
            * @param numberStyle the numbering style such as LOWERCASE_ROMAN_NUMERALS
            * @param prefix the text to prefix the number. Can be <CODE>null</CODE> or empty
            * @param logicalPage the first logical page number
            */
            public PdfPageLabelFormat(int physicalPage, int numberStyle, String prefix, int logicalPage) {
                this.physicalPage = physicalPage;
                this.numberStyle = numberStyle;
                this.prefix = prefix;
                this.logicalPage = logicalPage;
            }
        }
    }
}