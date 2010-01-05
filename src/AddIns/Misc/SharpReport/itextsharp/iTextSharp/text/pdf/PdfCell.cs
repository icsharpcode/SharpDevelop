using System;
using System.Collections;

using iTextSharp.text;

/*
 * $Id: PdfCell.cs,v 1.10 2008/05/13 11:25:19 psoares33 Exp $
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
     * A <CODE>PdfCell</CODE> is the PDF translation of a <CODE>Cell</CODE>.
     * <P>
     * A <CODE>PdfCell</CODE> is an <CODE>ArrayList</CODE> of <CODE>PdfLine</CODE>s.
     *
     * @see     iTextSharp.text.Rectangle
     * @see     iTextSharp.text.Cell
     * @see     PdfLine
     * @see     PdfTable
     */

    public class PdfCell : Rectangle {
    
        // membervariables
    
        /** These are the PdfLines in the Cell. */
        private ArrayList lines;
    
        /** These are the PdfLines in the Cell. */
        private PdfLine line;
    
        /** These are the Images in the Cell. */
        private ArrayList images;
    
        /** This is the leading of the lines. */
        private float leading;
    
        /** This is the number of the row the cell is in. */
        private int rownumber;
    
        /** This is the rowspan of the cell. */
        private int rowspan;
    
        /** This is the cellspacing of the cell. */
        private float cellspacing;
    
        /** This is the cellpadding of the cell. */
        private float cellpadding;
    
        /** Indicates if this cell belongs to the header of a <CODE>PdfTable</CODE> */
        private bool header = false;
    
        /**
        * This is the total height of the content of the cell.  Note that the actual cell
        * height may be larger due to another cell on the row *
        */
        private float contentHeight = 0.0f;

        /**
        * Indicates that the largest ascender height should be used to
        * determine the height of the first line. Setting this to true can help
        * with vertical alignment problems. */
        private bool useAscender;

        /**
        * Indicates that the largest descender height should be added to the height of
        * the last line (so characters like y don't dip into the border). */
        private bool useDescender;

        /**
        * Adjusts the cell contents to compensate for border widths.
        */
        private bool useBorderPadding;

        private int verticalAlignment;

        private PdfLine firstLine;
        private PdfLine lastLine;
    
        // constructors
    
        /**
         * Constructs a <CODE>PdfCell</CODE>-object.
         *
         * @param   cell        the original <CODE>Cell</CODE>
         * @param   rownumber   the number of the <CODE>Row</CODE> the <CODE>Cell</CODE> was in.
         * @param   left        the left border of the <CODE>PdfCell</CODE>
         * @param   right       the right border of the <CODE>PdfCell</CODE>
         * @param   top         the top border of the <CODE>PdfCell</CODE>
         * @param   cellspacing the cellspacing of the <CODE>Table</CODE>
         * @param   cellpadding the cellpadding of the <CODE>Table</CODE>
         */
    
        public PdfCell(Cell cell, int rownumber, float left, float right, float top, float cellspacing, float cellpadding) : base(left, top, right, top) {
            // copying the other Rectangle attributes from class Cell
            CloneNonPositionParameters(cell);
            this.cellpadding = cellpadding;
            this.cellspacing = cellspacing;
            this.verticalAlignment = cell.VerticalAlignment;
            this.useAscender = cell.UseAscender;
            this.useDescender = cell.UseDescender;
            this.useBorderPadding = cell.UseBorderPadding;
        
            // initialisation of some parameters
            PdfChunk chunk;
            PdfChunk overflow;
            lines = new ArrayList();
            images = new ArrayList();
            leading = cell.Leading;
            int alignment = cell.HorizontalAlignment;
            left += cellspacing + cellpadding;
            right -= cellspacing + cellpadding;
        
            left += GetBorderWidthInside(LEFT_BORDER);
            right -= GetBorderWidthInside(RIGHT_BORDER);
            contentHeight = 0;
            rowspan = cell.Rowspan;
        
            ArrayList allActions;
            int aCounter;
            // we loop over all the elements of the cell
            foreach (IElement ele in cell.Elements) {
                switch (ele.Type) {
                    case Element.JPEG:
                    case Element.JPEG2000:
                    case Element.IMGRAW:
                    case Element.IMGTEMPLATE:
                        AddImage((Image)ele, left, right, 0.4f * leading, alignment);
                        break;
                        // if the element is a list
                    case Element.LIST:
                        if (line != null && line.Size > 0) {
                            line.ResetAlignment();
                            AddLine(line);
                        }
                        // we loop over all the listitems
						AddList((List)ele, left, right, alignment);
                        line = new PdfLine(left, right, alignment, leading);
                        break;
                        // if the element is something else
                    default:
                        allActions = new ArrayList();
                        ProcessActions(ele, null, allActions);
                        aCounter = 0;

                        float currentLineLeading = leading;
                        float currentLeft = left;
                        float currentRight = right;
                        if (ele is Phrase) {
                            currentLineLeading = ((Phrase) ele).Leading;
                        }
                        if (ele is Paragraph) {
                            Paragraph p = (Paragraph) ele;
                            currentLeft += p.IndentationLeft;
                            currentRight -= p.IndentationRight;
                        }
                        if (line == null) {
                            line = new PdfLine(currentLeft, currentRight, alignment, currentLineLeading);
                        }
                        // we loop over the chunks
                        ArrayList chunks = ele.Chunks;
                        if (chunks.Count == 0) {
                            AddLine(line); // add empty line - all cells need some lines even if they are empty
                            line = new PdfLine(currentLeft, currentRight, alignment, currentLineLeading);
                        }
                        else {
                            foreach (Chunk c in chunks) {
                                chunk = new PdfChunk(c, (PdfAction)allActions[aCounter++]);
                                while ((overflow = line.Add(chunk)) != null) {
                                    AddLine(line);
                                    line = new PdfLine(currentLeft, currentRight, alignment, currentLineLeading);
                                    chunk = overflow;
                                }
                            }
                        }
                        // if the element is a paragraph, section or chapter, we reset the alignment and add the line
                        switch (ele.Type) {
                            case Element.PARAGRAPH:
                            case Element.SECTION:
                            case Element.CHAPTER:
                                line.ResetAlignment();
                                FlushCurrentLine();
                                break;
                        }
                        break;
                }
            }
            FlushCurrentLine();
            if (lines.Count > cell.MaxLines) {
                while (lines.Count > cell.MaxLines) {
                    RemoveLine(lines.Count - 1);
                }
                if (cell.MaxLines > 0) {
                    String more = cell.ShowTruncation;
                    if (more != null && more.Length > 0) {
                        // Denote that the content has been truncated
                        lastLine = (PdfLine) lines[lines.Count - 1];
                        if (lastLine.Size >= 0) {
                            PdfChunk lastChunk = lastLine.GetChunk(lastLine.Size - 1);
                            float moreWidth = new PdfChunk(more, lastChunk).Width;
                            while (lastChunk.ToString().Length > 0 && lastChunk.Width + moreWidth > right - left) {
                                // Remove characters to leave room for the 'more' indicator
                                lastChunk.Value = lastChunk.ToString().Substring(0, lastChunk.Length - 1);
                            }
                            lastChunk.Value = lastChunk.ToString() + more;
                        } else {
                            lastLine.Add(new PdfChunk(new Chunk(more), null));
                        }
                    }
                }
            }
            // we set some additional parameters
            if (useDescender && lastLine != null) {
                contentHeight -= lastLine.Descender;
            }

            // adjust first line height so that it touches the top
            if (lines.Count > 0) {
                firstLine = (PdfLine) lines[0];
                float firstLineRealHeight = FirstLineRealHeight;
                contentHeight -= firstLine.Height;
                firstLine.height = firstLineRealHeight;
                contentHeight += firstLineRealHeight;
            }

            float newBottom = top - contentHeight - (2f * Cellpadding) - (2f * Cellspacing);
            newBottom -= GetBorderWidthInside(TOP_BORDER) + GetBorderWidthInside(BOTTOM_BORDER);
            Bottom = newBottom;

            this.rownumber = rownumber;
        }
    
        private void AddList(List list, float left, float right, int alignment) {
            PdfChunk chunk;
            PdfChunk overflow;
            ArrayList allActions = new ArrayList();
            ProcessActions(list, null, allActions);
            int aCounter = 0;
            foreach (IElement ele in list.Items) {
                switch (ele.Type) {
                    case Element.LISTITEM:
                        ListItem item = (ListItem)ele;
                        line = new PdfLine(left + item.IndentationLeft, right, alignment, item.Leading);
                        line.ListItem = item;
                        foreach (Chunk c in item.Chunks) {
                            chunk = new PdfChunk(c, (PdfAction)(allActions[aCounter++]));
                            while ((overflow = line.Add(chunk)) != null) { 
                                AddLine(line);
                                line = new PdfLine(left + item.IndentationLeft, right, alignment, item.Leading);
                                chunk = overflow;
                            }
                            line.ResetAlignment();
                            AddLine(line);
                            line = new PdfLine(left + item.IndentationLeft, right, alignment, leading);
                        }
                        break;
                    case Element.LIST:
                        List sublist = (List)ele;
                        AddList(sublist, left + sublist.IndentationLeft, right, alignment);
                        break;
                }
            }
        }

        // overriding of the Rectangle methods
    
        /**
         * Returns the lower left x-coordinaat.
         *
         * @return      the lower left x-coordinaat
         */
    
        public override float Left {
            get {
                return base.GetLeft(cellspacing);
            }
        }
    
        /**
         * Returns the upper right x-coordinate.
         *
         * @return      the upper right x-coordinate
         */
    
        public override float Right {
            get {
                return base.GetRight(cellspacing);
            }
        }
    
        /**
         * Returns the upper right y-coordinate.
         *
         * @return      the upper right y-coordinate
         */
    
        public override float Top {
            get {
                return base.GetTop(cellspacing);
            }
        }
    
        /**
         * Returns the lower left y-coordinate.
         *
         * @return      the lower left y-coordinate
         */
    
        public override float Bottom {
            get {
                return base.GetBottom(cellspacing);
            }
            set {
                base.Bottom = value;
                float firstLineRealHeight = FirstLineRealHeight;

                float totalHeight = ury - value; // can't use top (already compensates for cellspacing)
                float nonContentHeight = (Cellpadding * 2f) + (Cellspacing * 2f);
                nonContentHeight += GetBorderWidthInside(TOP_BORDER) + GetBorderWidthInside(BOTTOM_BORDER);

                float interiorHeight = totalHeight - nonContentHeight;
                float extraHeight = 0.0f;

                switch (verticalAlignment) {
                    case Element.ALIGN_BOTTOM:
                        extraHeight = interiorHeight - contentHeight;
                        break;
                    case Element.ALIGN_MIDDLE:
                        extraHeight = (interiorHeight - contentHeight) / 2.0f;
                        break;
                    default:    // ALIGN_TOP
                        extraHeight = 0f;
                        break;
                }

                extraHeight += Cellpadding + Cellspacing;
                extraHeight += GetBorderWidthInside(TOP_BORDER);
                if (firstLine != null) {
                    firstLine.height = firstLineRealHeight + extraHeight;
                }
            }
        }
    
        // methods
    
        private void AddLine(PdfLine line) {
            lines.Add(line);
            contentHeight += line.Height;
            lastLine = line;
            this.line = null;
        }

        private PdfLine RemoveLine(int index) {
            PdfLine oldLine = (PdfLine)lines[index];
            lines.RemoveAt(index);
            contentHeight -= oldLine.Height;
            if (index == 0) {
                if (lines.Count > 0) {
                    firstLine = (PdfLine) lines[0];
                    float firstLineRealHeight = FirstLineRealHeight;
                    contentHeight -= firstLine.Height;
                    firstLine.height = firstLineRealHeight;
                    contentHeight += firstLineRealHeight;
                }
            }
            return oldLine;
        }

        private void FlushCurrentLine() {
            if (line != null && line.Size > 0) {
                AddLine(line);
            }
        }

        /**
        * Calculates what the height of the first line should be so that the content will be
        * flush with the top.  For text, this is the height of the ascender.  For an image,
        * it is the actual height of the image.
        * @return the real height of the first line
        */
        private float FirstLineRealHeight {
            get {
                float firstLineRealHeight = 0f;
                if (firstLine != null) {
                    PdfChunk chunk = firstLine.GetChunk(0);
                    if (chunk != null) {
                        Image image = chunk.Image;
                        if (image != null) {
                            firstLineRealHeight = firstLine.GetChunk(0).Image.ScaledHeight;
                        } else {
                            firstLineRealHeight = useAscender ? firstLine.Ascender : leading;
                        }
                    }
                }
                return firstLineRealHeight;
            }
        }

        /**
        * Gets the amount of the border for the specified side that is inside the Rectangle.
        * For non-variable width borders this is only 1/2 the border width on that side.  This
        * always returns 0 if {@link #useBorderPadding} is false;
        * @param side the side to check.  One of the side constants in {@link com.lowagie.text.Rectangle}
        * @return the borderwidth inside the cell
        */
        private float GetBorderWidthInside(int side) {
            float width = 0f;
            if (useBorderPadding) {
                switch (side) {
                    case iTextSharp.text.Rectangle.LEFT_BORDER:
                        width = BorderWidthLeft;
                        break;

                    case iTextSharp.text.Rectangle.RIGHT_BORDER:
                        width = BorderWidthRight;
                        break;

                    case iTextSharp.text.Rectangle.TOP_BORDER:
                        width = BorderWidthTop;
                        break;

                    default:    // default and BOTTOM
                        width = BorderWidthBottom;
                        break;
                }
                // non-variable (original style) borders overlap the rectangle (only 1/2 counts)
                if (!UseVariableBorders) {
                    width = width / 2f;
                }
            }
            return width;
        }


        /**
        * Adds an image to this Cell.
        *
        * @param i           the image to add
        * @param left        the left border
        * @param right       the right border
        * @param extraHeight extra height to add above image
        * @param alignment   horizontal alignment (constant from Element class)
        * @return the height of the image
        */

        private float AddImage(Image i, float left, float right, float extraHeight, int alignment) {
            Image image = Image.GetInstance(i);
            if (image.ScaledWidth > right - left) {
                image.ScaleToFit(right - left, float.MaxValue);
            }
            FlushCurrentLine();
            if (line == null) {
                line = new PdfLine(left, right, alignment, leading);
            }
            PdfLine imageLine = line;

            // left and right in chunk is relative to the start of the line
            right = right - left;
            left = 0f;

            if ((image.Alignment & Image.RIGHT_ALIGN) == Image.RIGHT_ALIGN) { // fix Uwe Zimmerman
                left = right - image.ScaledWidth;
            } else if ((image.Alignment & Image.MIDDLE_ALIGN) == Image.MIDDLE_ALIGN) {
                left = left + ((right - left - image.ScaledWidth) / 2f);
            }
            Chunk imageChunk = new Chunk(image, left, 0);
            imageLine.Add(new PdfChunk(imageChunk, null));
            AddLine(imageLine);
            return imageLine.Height;
        }
    
        /**
         * Gets the lines of a cell that can be drawn between certain limits.
         * <P>
         * Remark: all the lines that can be drawn are removed from the object!
         *
         * @param   top     the top of the part of the table that can be drawn
         * @param   bottom  the bottom of the part of the table that can be drawn
         * @return  an <CODE>ArrayList</CODE> of <CODE>PdfLine</CODE>s
         */
    
        public ArrayList GetLines(float top, float bottom) {
            float lineHeight;
            float currentPosition = Math.Min(this.Top, top);
            this.Top = currentPosition + cellspacing;
            ArrayList result = new ArrayList();
        
            // if the bottom of the page is higher than the top of the cell: do nothing
            if (Top < bottom) {
                return result;
            }
            
            // we loop over the lines
            int size = lines.Count;
            bool aboveBottom = true;
            for (int i = 0; i < size && aboveBottom; i++) {
                line = (PdfLine) lines[i];
                lineHeight = line.Height;
                currentPosition -= lineHeight;
                // if the currentPosition is higher than the bottom, we add the line to the result
                if (currentPosition > (bottom + cellpadding + GetBorderWidthInside(BOTTOM_BORDER))) { // bugfix by Tom Ring and Veerendra Namineni
                    result.Add(line);
                }
                else {
                    aboveBottom = false;
                }
            }
            // if the bottom of the cell is higher than the bottom of the page, the cell is written, so we can remove all lines
            float difference = 0f;
            if (!header) {
                if (aboveBottom) {
                    lines = new ArrayList();
                    contentHeight = 0f;
                }
                else {
                    size = result.Count;
                    for (int i = 0; i < size; i++) {
                        line = RemoveLine(0);
                        difference += line.Height;
                    }
                }
            }
            if (difference > 0) {
                foreach (Image image in images) {
                    image.SetAbsolutePosition(image.AbsoluteX, image.AbsoluteY - difference - leading);
                }
            }
            return result;
        }
    
        /**
         * Gets the images of a cell that can be drawn between certain limits.
         * <P>
         * Remark: all the lines that can be drawn are removed from the object!
         *
         * @param   top     the top of the part of the table that can be drawn
         * @param   bottom  the bottom of the part of the table that can be drawn
         * @return  an <CODE>ArrayList</CODE> of <CODE>Image</CODE>s
         */
    
        public ArrayList GetImages(float top, float bottom) {
        
            // if the bottom of the page is higher than the top of the cell: do nothing
            if (this.Top < bottom) {
                return new ArrayList();
            }
            top = Math.Min(this.Top, top);
            // initialisations
            float height;
            ArrayList result = new ArrayList();
            // we loop over the images
            ArrayList remove = new ArrayList();
            foreach (Image image in images) {
                height = image.AbsoluteY;
                // if the currentPosition is higher than the bottom, we add the line to the result
                if (top - height > (bottom + cellpadding)) {
                    image.SetAbsolutePosition(image.AbsoluteX, top - height);
                    result.Add(image);
                    remove.Add(image);
                }
            }
            foreach (Image image in remove) {
                images.Remove(image);
            }
            return result;
        }
    
        /**
         * Checks if this cell belongs to the header of a <CODE>PdfTable</CODE>.
         *
         * @return  <CODE>void</CODE>
         */
    
        internal void SetHeader() {
            header = true;
        }
    
        /**
         * Indicates that this cell belongs to the header of a <CODE>PdfTable</CODE>.
         */
    
        internal bool Header {
            get {
                return header;
            }
        }
    
        /**
         * Checks if the cell may be removed.
         * <P>
         * Headers may allways be removed, even if they are drawn only partially:
         * they will be repeated on each following page anyway!
         *
         * @return  <CODE>true</CODE> if all the lines are allready drawn; <CODE>false</CODE> otherwise.
         */
    
        internal bool MayBeRemoved() {
            return (header || (lines.Count == 0 && images.Count == 0));
        }
    
        /**
         * Returns the number of lines in the cell.
         *
         * @return  a value
         */
    
        public int Size {
            get {
                return lines.Count;
            }
        }
    
        /**
         * Returns the total height of all the lines in the cell.
         *
         * @return  a value
         */
    
        private float RemainingLinesHeight() {
            if (lines.Count == 0) return 0;
            float result = 0;
            int size = lines.Count;
            PdfLine line;
            for (int i = 0; i < size; i++) {
                line = (PdfLine) lines[i];
                result += line.Height;
            }
            return result;
        }
    
        /**
         * Returns the height needed to draw the remaining text.
         *
         * @return  a height
         */
    
        public float RemainingHeight {
            get {
                float result = 0f;
                foreach (Image image in images) {
                    result += image.ScaledHeight;
                }
                return RemainingLinesHeight() + cellspacing + 2 * cellpadding + result;
            }
        }
    
        // methods to retrieve membervariables
    
        /**
         * Gets the leading of a cell.
         *
         * @return  the leading of the lines is the cell.
         */
    
        public float Leading {
            get {
                return leading;
            }
        }
    
        /**
         * Gets the number of the row this cell is in..
         *
         * @return  a number
         */
    
        public int Rownumber {
            get {
                return rownumber;
            }
        }
    
        /**
         * Gets the rowspan of a cell.
         *
         * @return  the rowspan of the cell
         */
    
        public int Rowspan {
            get {
                return rowspan;
            }
        }
    
        /**
         * Gets the cellspacing of a cell.
         *
         * @return  a value
         */
    
        public float Cellspacing {
            get {
                return cellspacing;
            }
        }
    
        /**
         * Gets the cellpadding of a cell..
         *
         * @return  a value
         */
    
        public float Cellpadding {
            get {
                return cellpadding;
            }
        }
    
        /**
         * Processes all actions contained in the cell.
         */
    
        protected void ProcessActions(IElement element, PdfAction action, ArrayList allActions) {
            if (element.Type == Element.ANCHOR) {
                string url = ((Anchor)element).Reference;
                if (url != null) {
                    action = new PdfAction(url);
                }
            }
            switch (element.Type) {
                case Element.PHRASE:
                case Element.SECTION:
                case Element.ANCHOR:
                case Element.CHAPTER:
                case Element.LISTITEM:
                case Element.PARAGRAPH:
                    foreach (IElement ele in ((ArrayList)element)) {
                        ProcessActions(ele, action, allActions);
                    }
                    break;
                case Element.CHUNK:
                    allActions.Add(action);
                    break;
                case Element.LIST:
                    foreach (IElement ele in ((List)element).Items) {
                        ProcessActions(ele, action, allActions);
                    }
                    break;
                default:
                    int n = element.Chunks.Count;
                    while (n-- > 0)
                        allActions.Add(action);
                    break;
            }
        }

        /**
        * This is the number of the group the cell is in.
        */
        private int groupNumber;

        /**
        * Gets the number of the group this cell is in..
        *
        * @return   a number
        */

        public int GroupNumber {
            get {
                return groupNumber;
            }
            set {
                groupNumber = value;
            }
        }

        /**
        * Gets a Rectangle that is altered to fit on the page.
        *
        * @param    top     the top position
        * @param    bottom  the bottom position
        * @return   a <CODE>Rectangle</CODE>
        */

        public Rectangle Rectangle(float top, float bottom) {
            Rectangle tmp = new Rectangle(Left, Bottom, Right, Top);
            tmp.CloneNonPositionParameters(this);
            if (Top > top) {
                tmp.Top = top;
                tmp.Border = border - (border & TOP_BORDER);
            }
            if (Bottom < bottom) {
                tmp.Bottom = bottom;
                tmp.Border = border - (border & BOTTOM_BORDER);
            }
            return tmp;
        }

        /**
        * Gets the value of {@link #useAscender}
        * @return useAscender
        */
        public bool UseAscender {
            get {
                return useAscender;
            }
            set {
                useAscender = value;
            }
        }

        /**
        * Gets the value of {@link #useDescender}
        * @return useDescender
        */
        public bool UseDescender {
            get {
                return useDescender;
            }
            set {
                useDescender = value;
            }
        }

        /**
        * Sets the value of {@link #useBorderPadding}.
        * @param use adjust layour for borders if true
        */
        public bool UseBorderPadding {
            set {
                useBorderPadding = value;
            }
            get {
                return useBorderPadding;
            }
        }
    }
}