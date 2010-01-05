using System;
using System.Text;
using System.Collections;

using iTextSharp.text;

/*
 * $Id: PdfLine.cs,v 1.7 2008/05/13 11:25:21 psoares33 Exp $
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
     * <CODE>PdfLine</CODE> defines an array with <CODE>PdfChunk</CODE>-objects
     * that fit into 1 line.
     */

    public class PdfLine {
    
        // membervariables
    
        /** The arraylist containing the chunks. */
        protected internal ArrayList line;
    
        /** The left indentation of the line. */
        protected internal float left;
    
        /** The width of the line. */
        protected internal float width;
    
        /** The alignment of the line. */
        protected internal int alignment;
    
        /** The heigth of the line. */
        protected internal float height;
    
        /** The listsymbol (if necessary). */
        protected internal Chunk listSymbol = null;
    
        /** The listsymbol (if necessary). */
        protected internal float symbolIndent;
    
        /** <CODE>true</CODE> if the chunk splitting was caused by a newline. */
        protected internal bool newlineSplit = false;
    
        /** The original width. */
        protected internal float originalWidth;
    
        protected internal bool isRTL = false;
    
        // constructors
    
        /**
         * Constructs a new <CODE>PdfLine</CODE>-object.
         *
         * @param    left        the limit of the line at the left
         * @param    right        the limit of the line at the right
         * @param    alignment    the alignment of the line
         * @param    height        the height of the line
         */
    
        internal PdfLine(float left, float right, int alignment, float height) {
            this.left = left;
            this.width = right - left;
            this.originalWidth = this.width;
            this.alignment = alignment;
            this.height = height;
            this.line = new ArrayList();
        }
    
        /**
        * Creates a PdfLine object.
        * @param left              the left offset
        * @param originalWidth     the original width of the line
        * @param remainingWidth    bigger than 0 if the line isn't completely filled
        * @param alignment         the alignment of the line
        * @param newlineSplit      was the line splitted (or does the paragraph end with this line)
        * @param line              an array of PdfChunk objects
        * @param isRTL             do you have to read the line from Right to Left?
        */
        internal PdfLine(float left, float originalWidth, float remainingWidth, int alignment, bool newlineSplit, ArrayList line, bool isRTL) {
            this.left = left;
            this.originalWidth = originalWidth;
            this.width = remainingWidth;
            this.alignment = alignment;
            this.line = line;
            this.newlineSplit = newlineSplit;
            this.isRTL = isRTL;
        }
    
        // methods
    
        /**
         * Adds a <CODE>PdfChunk</CODE> to the <CODE>PdfLine</CODE>.
         *
         * @param        chunk        the <CODE>PdfChunk</CODE> to add
         * @return        <CODE>null</CODE> if the chunk could be added completely; if not
         *                a <CODE>PdfChunk</CODE> containing the part of the chunk that could
         *                not be added is returned
         */
    
        internal PdfChunk Add(PdfChunk chunk) {
            // nothing happens if the chunk is null.
            if (chunk == null || chunk.ToString().Equals("")) {
                return null;
            }
        
            // we split the chunk to be added
            PdfChunk overflow = chunk.Split(width);
            newlineSplit = (chunk.IsNewlineSplit() || overflow == null);
            //        if (chunk.IsNewlineSplit() && alignment == Element.ALIGN_JUSTIFIED)
            //            alignment = Element.ALIGN_LEFT;
            if (chunk.IsTab()) {
                Object[] tab = (Object[])chunk.GetAttribute(Chunk.TAB);
                float tabPosition = (float)tab[1];
                bool newline = (bool)tab[2];
                if (newline && tabPosition < originalWidth - width) {
                    return chunk;
                }
                width = originalWidth - tabPosition;
                chunk.AdjustLeft(left);
                AddToLine(chunk);
            }
            // if the length of the chunk > 0 we add it to the line
            else if (chunk.Length > 0) {
                if (overflow != null)
                    chunk.TrimLastSpace();
                width -= chunk.Width;
                AddToLine(chunk);
            }
        
                // if the length == 0 and there were no other chunks added to the line yet,
                // we risk to end up in an endless loop trying endlessly to add the same chunk
            else if (line.Count < 1) {
                chunk = overflow;
                overflow = chunk.Truncate(width);
                width -= chunk.Width;
                if (chunk.Length > 0) {
                    AddToLine(chunk);
                    return overflow;
                }
                    // if the chunck couldn't even be truncated, we add everything, so be it
                else {
                    if (overflow != null)
                        AddToLine(chunk);
                    return null;
                }
            }
            else {
                width += ((PdfChunk)(line[line.Count - 1])).TrimLastSpace();
            }
            return overflow;
        }
    
        private void AddToLine(PdfChunk chunk) {
            if (chunk.ChangeLeading && chunk.IsImage()) {
                float f = chunk.Image.ScaledHeight + chunk.ImageOffsetY;
                if (f > height) height = f;
            }
            line.Add(chunk);
        }

        // methods to retrieve information
    
        /**
         * Returns the number of chunks in the line.
         *
         * @return    a value
         */
    
        public int Size {
            get {
                return line.Count;
            }
        }
    
        /**
         * Returns an iterator of <CODE>PdfChunk</CODE>s.
         *
         * @return    an <CODE>Iterator</CODE>
         */
    
        public IEnumerator GetEnumerator() {
            return line.GetEnumerator();
        }
    
        /**
         * Returns the height of the line.
         *
         * @return    a value
         */
    
        internal float Height {
            get {
                return height;
            }
        }
    
        /**
         * Returns the left indentation of the line taking the alignment of the line into account.
         *
         * @return    a value
         */
    
        internal  float IndentLeft {
            get {
                if (isRTL) {
                    switch (alignment) {
                        case Element.ALIGN_LEFT:
                            return left + width;
                        case Element.ALIGN_CENTER:
                            return left + (width / 2f);
                        default:
                            return left;
                    }
                }
                else {
                    switch (alignment) {
                        case Element.ALIGN_RIGHT:
                            return left + width;
                        case Element.ALIGN_CENTER:
                            return left + (width / 2f);
                        default:
                            return left;
                    }
                }
            }
        }
    
        /**
         * Checks if this line has to be justified.
         *
         * @return    <CODE>true</CODE> if the alignment equals <VAR>ALIGN_JUSTIFIED</VAR> and there is some width left.
         */
    
        public bool HasToBeJustified() {
            return ((alignment == Element.ALIGN_JUSTIFIED || alignment == Element.ALIGN_JUSTIFIED_ALL) && width != 0);
        }
    
        /**
         * Resets the alignment of this line.
         * <P>
         * The alignment of the last line of for instance a <CODE>Paragraph</CODE>
         * that has to be justified, has to be reset to <VAR>ALIGN_LEFT</VAR>.
         */
    
        public void ResetAlignment() {
            if (alignment == Element.ALIGN_JUSTIFIED) {
                alignment = Element.ALIGN_LEFT;
            }
        }
    
        /** Adds extra indentation to the left (for Paragraph.setFirstLineIndent). */
        internal void SetExtraIndent(float extra) {
            left += extra;
            width -= extra;
        }

        /**
         * Returns the width that is left, after a maximum of characters is added to the line.
         *
         * @return    a value
         */
    
        internal float WidthLeft {
            get {
                return width;
            }
        }
    
        /**
         * Returns the number of space-characters in this line.
         *
         * @return    a value
         */
    
        internal int NumberOfSpaces {
            get {
                string str = ToString();
                int length = str.Length;
                int numberOfSpaces = 0;
                for (int i = 0; i < length; i++) {
                    if (str[i] == ' ') {
                        numberOfSpaces++;
                    }
                }
                return numberOfSpaces;
            }
        }
    
        /**
         * Sets the listsymbol of this line.
         * <P>
         * This is only necessary for the first line of a <CODE>ListItem</CODE>.
         *
         * @param listItem the list symbol
         */
    
        public ListItem ListItem {
            set {
                this.listSymbol = value.ListSymbol;
                this.symbolIndent = value.IndentationLeft;
            }
        }
    
        /**
         * Returns the listsymbol of this line.
         *
         * @return    a <CODE>PdfChunk</CODE> if the line has a listsymbol; <CODE>null</CODE> otherwise
         */
    
        public Chunk ListSymbol {
            get {
                return listSymbol;
            }
        }
    
        /**
         * Return the indentation needed to show the listsymbol.
         *
         * @return    a value
         */
    
        public float ListIndent {
            get {
                return symbolIndent;
            }
        }
    
        /**
         * Get the string representation of what is in this line.
         *
         * @return    a <CODE>string</CODE>
         */
    
        public override string ToString() {
            StringBuilder tmp = new StringBuilder();
            foreach (PdfChunk c in line) {
                tmp.Append(c.ToString());
            }
            return tmp.ToString();
        }
    
        public int GetLineLengthUtf32() {
            int total = 0;
            foreach (PdfChunk c in line) {
                total += c.LengthUtf32;
            }
            return total;
        }
    
        /**
         * Checks if a newline caused the line split.
         * @return <CODE>true</CODE> if a newline caused the line split
         */
        public bool NewlineSplit {
            get {
                return newlineSplit && (alignment != Element.ALIGN_JUSTIFIED_ALL);
            }
        }
    
        /**
         * Gets the index of the last <CODE>PdfChunk</CODE> with metric attributes
         * @return the last <CODE>PdfChunk</CODE> with metric attributes
         */
        public int LastStrokeChunk {
            get {
                int lastIdx = line.Count - 1;
                for (; lastIdx >= 0; --lastIdx) {
                    PdfChunk chunk = (PdfChunk)line[lastIdx];
                    if (chunk.IsStroked())
                        break;
                }
                return lastIdx;
            }
        }
    
        /**
         * Gets a <CODE>PdfChunk</CODE> by index.
         * @param idx the index
         * @return the <CODE>PdfChunk</CODE> or null if beyond the array
         */
        public PdfChunk GetChunk(int idx) {
            if (idx < 0 || idx >= line.Count)
                return null;
            return (PdfChunk)line[idx];
        }
    
        /**
         * Gets the original width of the line.
         * @return the original width of the line
         */
        public float OriginalWidth {
            get {
                return originalWidth;
            }
        }
    
        /**
         * Gets the maximum size of all the fonts used in this line
         * including images.
         * @return maximum size of all the fonts used in this line
         */
        internal float MaxSizeSimple {
            get {
                float maxSize = 0;
                for (int k = 0; k < line.Count; ++k) {
                    PdfChunk chunk = (PdfChunk)line[k];
                    if (!chunk.IsImage()) {
                        maxSize = Math.Max(chunk.Font.Size, maxSize);
                    }
                    else {
                        maxSize = Math.Max(chunk.Image.ScaledHeight + chunk.ImageOffsetY , maxSize);
                    }
                }
                return maxSize;
            }
        }
    
        internal bool RTL {
            get {
                return isRTL;
            }
        }
    
        /**
        * Gets the number of separators in the line.
        * @return  the number of separators in the line
        * @since   2.1.2
        */
        internal int GetSeparatorCount() {
            int s = 0;
            foreach (PdfChunk ck in line) {
                if (ck.IsTab()) {
                    return 0;
                }
                if (ck.IsHorizontalSeparator()) {
                    s++;
                }
            }
            return s;
        }

        public float GetWidthCorrected(float charSpacing, float wordSpacing) {
            float total = 0;
            for (int k = 0; k < line.Count; ++k) {
                PdfChunk ck = (PdfChunk)line[k];
                total += ck.GetWidthCorrected(charSpacing, wordSpacing);
            }
            return total;
        }

        /**
        * Gets the maximum size of the ascender for all the fonts used
        * in this line.
        * @return maximum size of all the ascenders used in this line
        */
        public float Ascender {
            get {
                float ascender = 0;
                foreach (PdfChunk ck in line) {
                    if (ck.IsImage())
                        ascender = Math.Max(ascender, ck.Image.ScaledHeight + ck.ImageOffsetY);
                    else {
                        PdfFont font = ck.Font;
                        ascender = Math.Max(ascender, font.Font.GetFontDescriptor(BaseFont.ASCENT, font.Size));
                    }
                }
                return ascender;
            }
        }

        /**
        * Gets the biggest descender for all the fonts used 
        * in this line.  Note that this is a negative number.
        * @return maximum size of all the ascenders used in this line
        */
        public float Descender {
            get {
                float descender = 0;
                foreach (PdfChunk ck in line) {
                    if (ck.IsImage())
                        descender = Math.Min(descender, ck.ImageOffsetY);
                    else {
                        PdfFont font = ck.Font;
                        descender = Math.Min(descender, font.Font.GetFontDescriptor(BaseFont.DESCENT, font.Size));
                    }
                }
                return descender;
            }
        }
    }
}