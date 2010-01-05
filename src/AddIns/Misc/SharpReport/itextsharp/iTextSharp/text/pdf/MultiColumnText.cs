using System;
using System.Collections;
using iTextSharp.text;
/*
 * $Id: MultiColumnText.cs,v 1.13 2008/05/13 11:25:18 psoares33 Exp $
 * 
 *
 * Copyright 2004 Steve Appling
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

namespace iTextSharp.text.pdf {
    /**
    * Formats content into one or more columns bounded by a
    * rectangle.  The columns may be simple rectangles or
    * more complicated shapes. Add all of the columns before
    * adding content. Column continuation is supported. A MultiColumnText object may be added to
    * a document using <CODE>Document.add</CODE>.
    * @author Steve Appling
    */
    public class MultiColumnText : IElement {

        /** special constant for automatic calculation of height */
        public const float AUTOMATIC = -1f;

        /**
        * total desiredHeight of columns.  If <CODE>AUTOMATIC</CODE>, this means fill pages until done.
        * This may be larger than one page
        */
        private float desiredHeight;

        /**
        * total height of element written out so far
        */
        private float totalHeight;

        /**
        * true if all the text could not be written out due to height restriction
        */
        private bool overflow;

        /**
        * Top of the columns - y position on starting page.
        * If <CODE>AUTOMATIC</CODE>, it means current y position when added to document
        */
        private float top;

        /**
        * used to store the y position of the bottom of the page
        */
        private float pageBottom;

        /**
        * ColumnText object used to do all the real work.  This same object is used for all columns
        */
        private ColumnText columnText;

        /**
        * Array of <CODE>ColumnDef</CODE> objects used to define the columns
        */
        private ArrayList columnDefs;

        /**
        * true if all columns are simple (rectangular)
        */
        private bool simple = true;

        private int currentColumn = 0;
        
        private float nextY = AUTOMATIC;
        
        private bool columnsRightToLeft = false;
        
        private PdfDocument document;
        /**
        * Default constructor.  Sets height to <CODE>AUTOMATIC</CODE>.
        * Columns will repeat on each page as necessary to accomodate content length.
        */
        public MultiColumnText() : this(AUTOMATIC) {
        }

        /**
        * Construct a MultiColumnText container of the specified height.
        * If height is <CODE>AUTOMATIC</CODE>, fill complete pages until done.
        * If a specific height is used, it may span one or more pages.
        *
        * @param height
        */
        public MultiColumnText(float height) {
            columnDefs = new ArrayList();
            desiredHeight = height;
            top = AUTOMATIC;
            // canvas will be set later
            columnText = new ColumnText(null);
            totalHeight = 0f;
        }

        /**
        * Construct a MultiColumnText container of the specified height
        * starting at the specified Y position.
        *
        * @param height
        * @param top
        */
        public MultiColumnText(float top, float height) {
            columnDefs = new ArrayList();
            desiredHeight = height;
            this.top = top;
            nextY = top;
            // canvas will be set later
            columnText = new ColumnText(null);
            totalHeight = 0f;
        }

        /**
        * Indicates that all of the text did not fit in the
        * specified height. Note that isOverflow will return
        * false before the MultiColumnText object has been
        * added to the document.  It will always be false if
        * the height is AUTOMATIC.
        *
        * @return true if there is still space left in the column
        */
        public bool IsOverflow() {
            return overflow;
        }

        /**
        * Copy the parameters from the specified ColumnText to use
        * when rendering.  Parameters like <CODE>setArabicOptions</CODE>
        * must be set in this way.
        *
        * @param sourceColumn
        */
        public void UseColumnParams(ColumnText sourceColumn) {
            // note that canvas will be overwritten later
            columnText.SetSimpleVars(sourceColumn);
        }

        /**
        * Add a new column.  The parameters are limits for each column
        * wall in the format of a sequence of points (x1,y1,x2,y2,...).
        *
        * @param left  limits for left column
        * @param right limits for right column
        */
        public void AddColumn(float[] left, float[] right) {
            ColumnDef nextDef = new ColumnDef(left, right, this);
            simple = nextDef.IsSimple();
            columnDefs.Add(nextDef);
        }

        /**
        * Add a simple rectangular column with specified left
        * and right x position boundaries.
        *
        * @param left  left boundary
        * @param right right boundary
        */
        public void AddSimpleColumn(float left, float right) {
            ColumnDef newCol = new ColumnDef(left, right, this);
            columnDefs.Add(newCol);
        }

        /**
        * Add the specified number of evenly spaced rectangular columns.
        * Columns will be seperated by the specified gutterWidth.
        *
        * @param left        left boundary of first column
        * @param right       right boundary of last column
        * @param gutterWidth width of gutter spacing between columns
        * @param numColumns  number of columns to add
        */
        public void AddRegularColumns(float left, float right, float gutterWidth, int numColumns) {
            float currX = left;
            float width = right - left;
            float colWidth = (width - (gutterWidth * (numColumns - 1))) / numColumns;
            for (int i = 0; i < numColumns; i++) {
                AddSimpleColumn(currX, currX + colWidth);
                currX += colWidth + gutterWidth;
            }
        }

        /**
        * Add an element to be rendered in a column.
        * Note that you can only add a <CODE>Phrase</CODE>
        * or a <CODE>Chunk</CODE> if the columns are
        * not all simple.  This is an underlying restriction in
        * {@link com.lowagie.text.pdf.ColumnText}
        *
        * @param element element to add
        * @throws DocumentException if element can't be added
        */
        public void AddElement(IElement element) {
            if (simple) {
                columnText.AddElement(element);
            } else if (element is Phrase) {
                columnText.AddText((Phrase) element);
            } else if (element is Chunk) {
                columnText.AddText((Chunk) element);
            } else {
                throw new DocumentException("Can't add " + element.GetType().ToString() + " to MultiColumnText with complex columns");
            }
        }


        /**
        * Write out the columns.  After writing, use
        * {@link #isOverflow()} to see if all text was written.
        * @param canvas PdfContentByte to write with
        * @param document document to write to (only used to get page limit info)
        * @param documentY starting y position to begin writing at
        * @return the current height (y position) after writing the columns
        * @throws DocumentException on error
        */
        public float Write(PdfContentByte canvas, PdfDocument document, float documentY) {
            this.document = document;
            columnText.Canvas = canvas;
            if (columnDefs.Count == 0) {
                throw new DocumentException("MultiColumnText has no columns");
            }
            overflow = false;
            pageBottom = document.Bottom;
            float currentHeight = 0;
            bool done = false;
            while (!done) {
                if (top == AUTOMATIC) {
                    top = document.GetVerticalPosition(true);  
                }
                else if (nextY == AUTOMATIC) {
                    nextY = document.GetVerticalPosition(true); // RS - 07/07/2005 - - Get current doc writing position for top of columns on new page.
                }

                ColumnDef currentDef = (ColumnDef) columnDefs[CurrentColumn];
                columnText.YLine = top;

                float[] left = currentDef.ResolvePositions(Rectangle.LEFT_BORDER);
                float[] right = currentDef.ResolvePositions(Rectangle.RIGHT_BORDER);
                if (document.IsMarginMirroring() && document.PageNumber % 2 == 0){
                    float delta = document.RightMargin - document.Left;
                    left = (float[])left.Clone();
                    right = (float[])right.Clone();
                    for (int i = 0; i < left.Length; i += 2) {
                        left[i] -= delta;
                    }
                    for (int i = 0; i < right.Length; i += 2) {
                        right[i] -= delta;
                    }
                }
                currentHeight = Math.Max(currentHeight, GetHeight(left, right));

                if (currentDef.IsSimple()) {
                    columnText.SetSimpleColumn(left[2], left[3], right[0], right[1]);
                } else {
                    columnText.SetColumns(left, right);
                }

                int result = columnText.Go();
                if ((result & ColumnText.NO_MORE_TEXT) != 0) {
                    done = true;
                    top = columnText.YLine;
                } else if (ShiftCurrentColumn()) {
                    top = nextY;
                } else {  // check if we are done because of height
                    totalHeight += currentHeight;

                    if ((desiredHeight != AUTOMATIC) && (totalHeight >= desiredHeight)) {
                        overflow = true;
                        break;
                    } else {  // need to start new page and reset the columns
                        documentY = nextY;
                        NewPage();
                        currentHeight = 0;
                    }
                }
            }
            if (desiredHeight == AUTOMATIC && columnDefs.Count == 1) {
                currentHeight = documentY - columnText.YLine;
            }
            return currentHeight;
        }

        private void NewPage() {
            ResetCurrentColumn();
            if (desiredHeight == AUTOMATIC) {
                top = nextY = AUTOMATIC;
            }
            else {
                top = nextY;
            }
            totalHeight = 0;
            if (document != null) {
                document.NewPage();
            }
        }
        
        /**
        * Figure out the height of a column from the border extents
        *
        * @param left  left border
        * @param right right border
        * @return height
        */
        private float GetHeight(float[] left, float[] right) {
            float max = float.MinValue;
            float min = float.MaxValue;
            for (int i = 0; i < left.Length; i += 2) {
                min = Math.Min(min, left[i + 1]);
                max = Math.Max(max, left[i + 1]);
            }
            for (int i = 0; i < right.Length; i += 2) {
                min = Math.Min(min, right[i + 1]);
                max = Math.Max(max, right[i + 1]);
            }
            return max - min;
        }


        /**
        * Processes the element by adding it to an
        * <CODE>ElementListener</CODE>.
        *
        * @param   listener    an <CODE>ElementListener</CODE>
        * @return  <CODE>true</CODE> if the element was processed successfully
        */
        public bool Process(IElementListener listener) {
            try {
                return listener.Add(this);
            } catch (DocumentException) {
                return false;
            }
        }

        /**
        * Gets the type of the text element.
        *
        * @return  a type
        */

        public int Type {
            get {
                return Element.MULTI_COLUMN_TEXT;
            }
        }

        /**
        * Returns null - not used
        *
        * @return  null
        */

        public ArrayList Chunks {
            get {
                return null;
            }
        }

        /**
        * @see com.lowagie.text.Element#isContent()
        * @since   iText 2.0.8
        */
        public bool IsContent() {
            return true;
        }

        /**
        * @see com.lowagie.text.Element#isNestable()
        * @since   iText 2.0.8
        */
        public bool IsNestable() {
            return false;
        }
  
        /**
        * Calculates the appropriate y position for the bottom
        * of the columns on this page.
        *
        * @return the y position of the bottom of the columns
        */
        private float GetColumnBottom() {
            if (desiredHeight == AUTOMATIC) {
                return pageBottom;
            } else {
                return Math.Max(top - (desiredHeight - totalHeight), pageBottom);
            }
        }

        /**
        * Moves the text insertion point to the beginning of the next column, issuing a page break if
        * needed.
        * @throws DocumentException on error
        */    
        public void NextColumn() {
            currentColumn = (currentColumn + 1) % columnDefs.Count;
            top = nextY;
            if (currentColumn == 0) {
                NewPage();
            }
        }

        /**
        * Gets the current column.
        * @return the current column
        */
        public int CurrentColumn {
            get {
                if (columnsRightToLeft) {
                    return (columnDefs.Count - currentColumn - 1);
                } 
                return currentColumn;
            }
        }
        
        /**
        * Resets the current column.
        */
        public void ResetCurrentColumn() {
            currentColumn = 0;
        }
        
        /**
        * Shifts the current column.
        * @return true if the currentcolumn has changed
        */
        public bool ShiftCurrentColumn() {
            if (currentColumn + 1 < columnDefs.Count) {
                currentColumn++;
                return true;
            }
            return false;
        }
        
        /**
        * Sets the direction of the columns.
        * @param direction true = right2left; false = left2right
        */
        public void SetColumnsRightToLeft(bool direction) {
            columnsRightToLeft = direction;
        }
        
        /** Sets the ratio between the extra word spacing and the extra character spacing
        * when the text is fully justified.
        * Extra word spacing will grow <CODE>spaceCharRatio</CODE> times more than extra character spacing.
        * If the ratio is <CODE>PdfWriter.NO_SPACE_CHAR_RATIO</CODE> then the extra character spacing
        * will be zero.
        * @param spaceCharRatio the ratio between the extra word spacing and the extra character spacing
        */
        public float SpaceCharRatio {
            set {
                columnText.SpaceCharRatio = value;
            }
        }

        /** Sets the run direction. 
        * @param runDirection the run direction
        */    
        public int RunDirection {
            set {
                columnText.RunDirection = value;
            }
        }
        
        /** Sets the arabic shaping options. The option can be AR_NOVOWEL,
        * AR_COMPOSEDTASHKEEL and AR_LIG.
        * @param arabicOptions the arabic shaping options
        */
        public int ArabicOptions {
            set {
                columnText.ArabicOptions = value;
            }
        }

        /** Sets the default alignment
        * @param alignment the default alignment
        */
        public int Alignment {
            set {
                columnText.Alignment = value;
            }
        }
        
        public override string ToString() {
            return base.ToString();
        }

        /**
        * Inner class used to define a column
        */
        internal class ColumnDef {
            private float[] left;
            private float[] right;
            private MultiColumnText mc;

            internal ColumnDef(float[] newLeft, float[] newRight, MultiColumnText mc) {
                this.mc = mc;
                left = newLeft;
                right = newRight;
            }

            internal ColumnDef(float leftPosition, float rightPosition, MultiColumnText mc) {
                this.mc = mc;
                left = new float[4];
                left[0] = leftPosition; // x1
                left[1] = mc.top;          // y1
                left[2] = leftPosition; // x2
                if (mc.desiredHeight == AUTOMATIC || mc.top == AUTOMATIC) {
                    left[3] = AUTOMATIC;
                } else {
                    left[3] = mc.top - mc.desiredHeight;
                }

                right = new float[4];
                right[0] = rightPosition; // x1
                right[1] = mc.top;           // y1
                right[2] = rightPosition; // x2
                if (mc.desiredHeight == AUTOMATIC || mc.top == AUTOMATIC) {
                    right[3] = AUTOMATIC;
                } else {
                    right[3] = mc.top - mc.desiredHeight;
                }
            }

            /**
            * Resolves the positions for the specified side of the column
            * into real numbers once the top of the column is known.
            *
            * @param side either <CODE>Rectangle.LEFT_BORDER</CODE>
            *             or <CODE>Rectangle.RIGHT_BORDER</CODE>
            * @return the array of floats for the side
            */
            internal float[] ResolvePositions(int side) {
                if (side == Rectangle.LEFT_BORDER) {
                    return ResolvePositions(left);
                } else {
                    return ResolvePositions(right);
                }
            }

            internal float[] ResolvePositions(float[] positions) {
                if (!IsSimple()) {
                    return positions;
                }
                if (mc.top == AUTOMATIC) {
                    // this is bad - must be programmer error
                    throw new Exception("resolvePositions called with top=AUTOMATIC (-1).  " +
                            "Top position must be set befure lines can be resolved");
                }
                positions[1] = mc.top;
                positions[3] = mc.GetColumnBottom();
                return positions;
            }

            /**
            * Checks if column definition is a simple rectangle
            * @return true if it is a simple column 
            */
            internal bool IsSimple() {
                return (left.Length == 4 && right.Length == 4) && (left[0] == left[2] && right[0] == right[2]);
            }

        }
    }
}
