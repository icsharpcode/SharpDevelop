using System;
using System.Collections;
using System.IO;
/*
 * $Id: PdfPages.cs,v 1.3 2008/05/13 11:25:21 psoares33 Exp $
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
    * <CODE>PdfPages</CODE> is the PDF Pages-object.
    * <P>
    * The Pages of a document are accessible through a tree of nodes known as the Pages tree.
    * This tree defines the ordering of the pages in the document.<BR>
    * This object is described in the 'Portable Document Format Reference Manual version 1.3'
    * section 6.3 (page 71-73)
    *
    * @see        PdfPageElement
    * @see        PdfPage
    */

    public class PdfPages {
        
        private ArrayList pages = new ArrayList();
        private ArrayList parents = new ArrayList();
        private int leafSize = 10;
        private PdfWriter writer;
        private PdfIndirectReference topParent;
        
        // constructors
        
    /**
    * Constructs a <CODE>PdfPages</CODE>-object.
    */
        
        internal PdfPages(PdfWriter writer) {
            this.writer = writer;
        }
        
        internal void AddPage(PdfDictionary page) {
            if ((pages.Count % leafSize) == 0)
                parents.Add(writer.PdfIndirectReference);
            PdfIndirectReference parent = (PdfIndirectReference)parents[parents.Count - 1];
            page.Put(PdfName.PARENT, parent);
            PdfIndirectReference current = writer.CurrentPage;
            writer.AddToBody(page, current);
            pages.Add(current);
        }
        
        internal PdfIndirectReference AddPageRef(PdfIndirectReference pageRef) {
            if ((pages.Count % leafSize) == 0)
                parents.Add(writer.PdfIndirectReference);
            pages.Add(pageRef);
            return (PdfIndirectReference)parents[parents.Count - 1];
        }
        
        // returns the top parent to include in the catalog
        internal PdfIndirectReference WritePageTree() {
            if (pages.Count == 0)
                throw new IOException("The document has no pages.");
            int leaf = 1;
            ArrayList tParents = parents;
            ArrayList tPages = pages;
            ArrayList nextParents = new ArrayList();
            while (true) {
                leaf *= leafSize;
                int stdCount = leafSize;
                int rightCount = tPages.Count % leafSize;
                if (rightCount == 0)
                    rightCount = leafSize;
                for (int p = 0; p < tParents.Count; ++p) {
                    int count;
                    int thisLeaf = leaf;
                    if (p == tParents.Count - 1) {
                        count = rightCount;
                        thisLeaf = pages.Count % leaf;
                        if (thisLeaf == 0)
                            thisLeaf = leaf;
                    }
                    else
                        count = stdCount;
                    PdfDictionary top = new PdfDictionary(PdfName.PAGES);
                    top.Put(PdfName.COUNT, new PdfNumber(thisLeaf));
                    PdfArray kids = new PdfArray();
                    ArrayList intern = kids.ArrayList;
                    intern.AddRange(tPages.GetRange(p * stdCount, count));
                    top.Put(PdfName.KIDS, kids);
                    if (tParents.Count > 1) {
                        if ((p % leafSize) == 0)
                            nextParents.Add(writer.PdfIndirectReference);
                        top.Put(PdfName.PARENT, (PdfIndirectReference)nextParents[p / leafSize]);
                    }
                    writer.AddToBody(top, (PdfIndirectReference)tParents[p]);
                }
                if (tParents.Count == 1) {
                    topParent = (PdfIndirectReference)tParents[0];
                    return topParent;
                }
                tPages = tParents;
                tParents = nextParents;
                nextParents = new ArrayList();
            }
        }
        
        internal PdfIndirectReference TopParent {
            get {
                return topParent;
            }
        }
        
        internal void SetLinearMode(PdfIndirectReference topParent) {
            if (parents.Count > 1)
                throw new Exception("Linear page mode can only be called with a single parent.");
            if (topParent != null) {
                this.topParent = topParent;
                parents.Clear();
                parents.Add(topParent);
            }
            leafSize = 10000000;
        }

        internal void AddPage(PdfIndirectReference page) {
            pages.Add(page);
        }

        internal int ReorderPages(int[] order) {
            if (order == null)
                return pages.Count;
            if (parents.Count > 1)
                throw new DocumentException("Page reordering requires a single parent in the page tree. Call PdfWriter.SetLinearMode() after open.");
            if (order.Length != pages.Count)
                throw new DocumentException("Page reordering requires an array with the same size as the number of pages.");
            int max = pages.Count;
            bool[] temp = new bool[max];
            for (int k = 0; k < max; ++k) {
                int p = order[k];
                if (p < 1 || p > max)
                    throw new DocumentException("Page reordering requires pages between 1 and " + max + ". Found " + p + ".");
                if (temp[p - 1])
                    throw new DocumentException("Page reordering requires no page repetition. Page " + p + " is repeated.");
                temp[p - 1] = true;
            }
            Object[] copy = pages.ToArray();
            for (int k = 0; k < max; ++k) {
                pages[k] = copy[order[k] - 1];
            }
            return max;
        }
    }
}