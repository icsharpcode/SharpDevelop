using System;
using System.Collections;
using System.Text;
using System.util;
using iTextSharp.text;

/*
 * Copyright 2005 by Michael Niedermair.
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
namespace iTextSharp.text.pdf.events {

    /**
    * Class for an index.
    * 
    * @author Michael Niedermair
    */
    public class IndexEvents : PdfPageEventHelper {

        /**
        * keeps the indextag with the pagenumber
        */
        private Hashtable indextag = new Hashtable();

        /**
        * All the text that is passed to this event, gets registered in the indexentry.
        * 
        * @see com.lowagie.text.pdf.PdfPageEventHelper#onGenericTag(
        *      com.lowagie.text.pdf.PdfWriter, com.lowagie.text.Document,
        *      com.lowagie.text.Rectangle, java.lang.String)
        */
        public override void OnGenericTag(PdfWriter writer, Document document,
                Rectangle rect, String text) {
            indextag[text] = writer.PageNumber;
        }

        // --------------------------------------------------------------------
        /**
        * indexcounter
        */
        private long indexcounter = 0;

        /**
        * the list for the index entry
        */
        private ArrayList indexentry = new ArrayList();

        /**
        * Create an index entry.
        *
        * @param text  The text for the Chunk.
        * @param in1   The first level.
        * @param in2   The second level.
        * @param in3   The third level.
        * @return Returns the Chunk.
        */
        public Chunk Create(String text, String in1, String in2,
                String in3) {

            Chunk chunk = new Chunk(text);
            String tag = "idx_" + (indexcounter++);
            chunk.SetGenericTag(tag);
            chunk.SetLocalDestination(tag);
            Entry entry = new Entry(in1, in2, in3, tag, this);
            indexentry.Add(entry);
            return chunk;
        }

        /**
        * Create an index entry.
        *
        * @param text  The text for the Chunk.
        * @param in1   The first level.
        * @return Returns the Chunk.
        */
        public Chunk Create(String text, String in1) {
            return Create(text, in1, "", "");
        }

        /**
        * Create an index entry.
        *
        * @param text  The text for the Chunk.
        * @param in1   The first level.
        * @param in2   The second level.
        * @return Returns the Chunk.
        */
        public Chunk Create(String text, String in1, String in2) {
            return Create(text, in1, in2, "");
        }

        /**
        * Create an index entry.
        *
        * @param text  The text.
        * @param in1   The first level.
        * @param in2   The second level.
        * @param in3   The third level.
        */
        public void Create(Chunk text, String in1, String in2,
                String in3) {

            String tag = "idx_" + (indexcounter++);
            text.SetGenericTag(tag);
            text.SetLocalDestination(tag);
            Entry entry = new Entry(in1, in2, in3, tag, this);
            indexentry.Add(entry);
        }

        /**
        * Create an index entry.
        *
        * @param text  The text.
        * @param in1   The first level.
        */
        public void Create(Chunk text, String in1) {
            Create(text, in1, "", "");
        }

        /**
        * Create an index entry.
        *
        * @param text  The text.
        * @param in1   The first level.
        * @param in2   The second level.
        */
        public void Create(Chunk text, String in1, String in2) {
            Create(text, in1, in2, "");
        }

        private class ISortIndex : IComparer {
        
            public int Compare(object arg0, object arg1) {
                Entry en1 = (Entry) arg0;
                Entry en2 = (Entry) arg1;

                int rt = 0;
                if (en1.GetIn1() != null && en2.GetIn1() != null) {
                    if ((rt = Util.CompareToIgnoreCase(en1.GetIn1(),en2.GetIn1())) == 0) {
                        // in1 equals
                        if (en1.GetIn2() != null && en2.GetIn2() != null) {
                            if ((rt = Util.CompareToIgnoreCase(en1.GetIn2(), en2.GetIn2())) == 0) {
                                // in2 equals
                                if (en1.GetIn3() != null && en2.GetIn3() != null) {
                                    rt = Util.CompareToIgnoreCase(en1.GetIn3(), en2.GetIn3());
                                }
                            }
                        }
                    }
                }
                return rt;
            }
        }

        /**
        * Comparator for sorting the index
        */
        private IComparer comparator = new ISortIndex();

        /**
        * Set the comparator.
        * @param aComparator The comparator to set.
        */
        public void SetComparator(IComparer aComparator) {
            comparator = aComparator;
        }

        /**
        * Returns the sorted list with the entries and the collected page numbers.
        * @return Returns the sorted list with the entries and teh collected page numbers.
        */
        public ArrayList GetSortedEntries() {

            Hashtable grouped = new Hashtable();

            for (int i = 0; i < indexentry.Count; i++) {
                Entry e = (Entry) indexentry[i];
                String key = e.GetKey();

                Entry master = (Entry) grouped[key];
                if (master != null) {
                    master.AddPageNumberAndTag(e.GetPageNumber(), e.GetTag());
                } else {
                    e.AddPageNumberAndTag(e.GetPageNumber(), e.GetTag());
                    grouped[key] = e;
                }
            }

            // copy to a list and sort it
            ArrayList sorted = new ArrayList(grouped.Values);
            sorted.Sort(0, sorted.Count, comparator);
            return sorted;
        }

        // --------------------------------------------------------------------
        /**
        * Class for an index entry.
        * <p>
        * In the first step, only in1, in2,in3 and tag are used.
        * After the collections of the index entries, pagenumbers are used.
        * </p>
        */
        public class Entry {

            /**
            * first level
            */
            private String in1;

            /**
            * second level
            */
            private String in2;

            /**
            * third level
            */
            private String in3;

            /**
            * the tag
            */
            private String tag;

            /**
            * the lsit of all page numbers.
            */
            private ArrayList pagenumbers = new ArrayList();

            /**
            * the lsit of all tags.
            */
            private ArrayList tags = new ArrayList();
            private IndexEvents parent;

            /**
            * Create a new object.
            * @param aIn1   The first level.
            * @param aIn2   The second level.
            * @param aIn3   The third level.
            * @param aTag   The tag.
            */
            public Entry(String aIn1, String aIn2, String aIn3,
                    String aTag, IndexEvents parent) {
                in1 = aIn1;
                in2 = aIn2;
                in3 = aIn3;
                tag = aTag;
                this.parent = parent;
            }

            /**
            * Returns the in1.
            * @return Returns the in1.
            */
            public String GetIn1() {
                return in1;
            }

            /**
            * Returns the in2.
            * @return Returns the in2.
            */
            public String GetIn2() {
                return in2;
            }

            /**
            * Returns the in3.
            * @return Returns the in3.
            */
            public String GetIn3() {
                return in3;
            }

            /**
            * Returns the tag.
            * @return Returns the tag.
            */
            public String GetTag() {
                return tag;
            }

            /**
            * Returns the pagenumer for this entry.
            * @return Returns the pagenumer for this entry.
            */
            public int GetPageNumber() {
                int rt = -1;
                object i = parent.indextag[tag];
                if (i != null) {
                    rt = (int)i;
                }
                return rt;
            }

            /**
            * Add a pagenumber.
            * @param number    The page number.
            * @param tag
            */
            public void AddPageNumberAndTag(int number, String tag) {
                pagenumbers.Add(number);
                tags.Add(tag);
            }

            /**
            * Returns the key for the map-entry.
            * @return Returns the key for the map-entry.
            */
            public String GetKey() {
                return in1 + "!" + in2 + "!" + in3;
            }

            /**
            * Returns the pagenumbers.
            * @return Returns the pagenumbers.
            */
            public ArrayList GetPagenumbers() {
                return pagenumbers;
            }

            /**
            * Returns the tags.
            * @return Returns the tags.
            */
            public ArrayList GetTags() {
                return tags;
            }

            /**
            * print the entry (only for test)
            * @return the toString implementation of the entry
            */
            public override String ToString() {
                StringBuilder buf = new StringBuilder();
                buf.Append(in1).Append(' ');
                buf.Append(in2).Append(' ');
                buf.Append(in3).Append(' ');
                for (int i = 0; i < pagenumbers.Count; i++) {
                    buf.Append(pagenumbers[i]).Append(' ');
                }
                return buf.ToString();
            }
        }
    }
}