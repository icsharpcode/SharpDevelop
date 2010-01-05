using System;
using System.Collections;
/*
 * Copyright 2005 by Paulo Soares.
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
    * Creates a number tree.
    * @author Paulo Soares (psoares@consiste.pt)
    */
    public class PdfNumberTree {
        
        private const int leafSize = 64;
        
        /**
        * Creates a number tree.
        * @param items the item of the number tree. The key is an <CODE>Integer</CODE>
        * and the value is a <CODE>PdfObject</CODE>.
        * @param writer the writer
        * @throws IOException on error
        * @return the dictionary with the number tree.
        */    
        public static PdfDictionary WriteTree(Hashtable items, PdfWriter writer) {
            if (items.Count == 0)
                return null;
            int[] numbers = new int[items.Count];
            items.Keys.CopyTo(numbers, 0);
            Array.Sort(numbers);
            if (numbers.Length <= leafSize) {
                PdfDictionary dic = new PdfDictionary();
                PdfArray ar = new PdfArray();
                for (int k = 0; k < numbers.Length; ++k) {
                    ar.Add(new PdfNumber(numbers[k]));
                    ar.Add((PdfObject)items[numbers[k]]);
                }
                dic.Put(PdfName.NUMS, ar);
                return dic;
            }
            int skip = leafSize;
            PdfIndirectReference[] kids = new PdfIndirectReference[(numbers.Length + leafSize - 1) / leafSize];
            for (int k = 0; k < kids.Length; ++k) {
                int offset = k * leafSize;
                int end = Math.Min(offset + leafSize, numbers.Length);
                PdfDictionary dic = new PdfDictionary();
                PdfArray arr = new PdfArray();
                arr.Add(new PdfNumber(numbers[offset]));
                arr.Add(new PdfNumber(numbers[end - 1]));
                dic.Put(PdfName.LIMITS, arr);
                arr = new PdfArray();
                for (; offset < end; ++offset) {
                    arr.Add(new PdfNumber(numbers[offset]));
                    arr.Add((PdfObject)items[numbers[offset]]);
                }
                dic.Put(PdfName.NUMS, arr);
                kids[k] = writer.AddToBody(dic).IndirectReference;
            }
            int top = kids.Length;
            while (true) {
                if (top <= leafSize) {
                    PdfArray arr = new PdfArray();
                    for (int k = 0; k < top; ++k)
                        arr.Add(kids[k]);
                    PdfDictionary dic = new PdfDictionary();
                    dic.Put(PdfName.KIDS, arr);
                    return dic;
                }
                skip *= leafSize;
                int tt = (numbers.Length + skip - 1 )/ skip;
                for (int k = 0; k < tt; ++k) {
                    int offset = k * leafSize;
                    int end = Math.Min(offset + leafSize, top);
                    PdfDictionary dic = new PdfDictionary();
                    PdfArray arr = new PdfArray();
                    arr.Add(new PdfNumber(numbers[k * skip]));
                    arr.Add(new PdfNumber(numbers[Math.Min((k + 1) * skip, numbers.Length) - 1]));
                    dic.Put(PdfName.LIMITS, arr);
                    arr = new PdfArray();
                    for (; offset < end; ++offset) {
                        arr.Add(kids[offset]);
                    }
                    dic.Put(PdfName.KIDS, arr);
                    kids[k] = writer.AddToBody(dic).IndirectReference;
                }
                top = tt;
            }
        }
        
        private static void IterateItems(PdfDictionary dic, Hashtable items) {
            PdfArray nn = (PdfArray)PdfReader.GetPdfObjectRelease(dic.Get(PdfName.NUMS));
            if (nn != null) {
                ArrayList arr = nn.ArrayList;
                for (int k = 0; k < arr.Count; ++k) {
                    PdfNumber s = (PdfNumber)PdfReader.GetPdfObjectRelease((PdfObject)arr[k++]);
                    items[s.IntValue] = arr[k];
                }
            }
            else if ((nn = (PdfArray)PdfReader.GetPdfObjectRelease(dic.Get(PdfName.KIDS))) != null) {
                ArrayList arr = nn.ArrayList;
                for (int k = 0; k < arr.Count; ++k) {
                    PdfDictionary kid = (PdfDictionary)PdfReader.GetPdfObjectRelease((PdfObject)arr[k]);
                    IterateItems(kid, items);
                }
            }
        }
        
        public static Hashtable ReadTree(PdfDictionary dic) {
            Hashtable items = new Hashtable();
            if (dic != null)
                IterateItems(dic, items);
            return items;
        }
    }
}
