using System;
using System.Collections;

using iTextSharp.text;
using iTextSharp.text.pdf.events;

/*
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
    /** This is a table that can be put at an absolute position but can also
    * be added to the document as the class <CODE>Table</CODE>.
    * In the last case when crossing pages the table always break at full rows; if a
    * row is bigger than the page it is dropped silently to avoid infinite loops.
    * <P>
    * A PdfPTableEvent can be associated to the table to do custom drawing
    * when the table is rendered.
    * @author Paulo Soares (psoares@consiste.pt)
    */

    public class PdfPTable : ILargeElement{
        
        /** The index of the original <CODE>PdfcontentByte</CODE>.
        */    
        public const int BASECANVAS = 0;
        /** The index of the duplicate <CODE>PdfContentByte</CODE> where the background will be drawn.
        */    
        public const int BACKGROUNDCANVAS = 1;
        /** The index of the duplicate <CODE>PdfContentByte</CODE> where the border lines will be drawn.
        */    
        public const int LINECANVAS = 2;
        /** The index of the duplicate <CODE>PdfContentByte</CODE> where the text will be drawn.
        */    
        public const int TEXTCANVAS = 3;
        
        protected ArrayList rows = new ArrayList();
        protected float totalHeight = 0;
        protected PdfPCell[] currentRow;
        protected int currentRowIdx = 0;
        protected PdfPCell defaultCell = new PdfPCell((Phrase)null);
        protected float totalWidth = 0;
        protected float[] relativeWidths;
        protected float[] absoluteWidths;
        protected IPdfPTableEvent tableEvent;
        
    /** Holds value of property headerRows. */
        protected int headerRows;
        
    /** Holds value of property widthPercentage. */
        protected float widthPercentage = 80;
        
    /** Holds value of property horizontalAlignment. */
        private int horizontalAlignment = Element.ALIGN_CENTER;
        
    /** Holds value of property skipFirstHeader. */
        private bool skipFirstHeader = false;

        protected bool isColspan = false;
        
        protected int runDirection = PdfWriter.RUN_DIRECTION_DEFAULT;

        /**
        * Holds value of property lockedWidth.
        */
        private bool lockedWidth = false;
        
        /**
        * Holds value of property splitRows.
        */
        private bool splitRows = true;
        
    /** The spacing before the table. */
        protected float spacingBefore;
        
    /** The spacing after the table. */
        protected float spacingAfter;
        
        /**
        * Holds value of property extendLastRow.
        */
        private bool extendLastRow;
        
        /**
        * Holds value of property headersInEvent.
        */
        private bool headersInEvent;
        
        /**
        * Holds value of property splitLate.
        */
        private bool splitLate = true;
        
        /**
        * Defines if the table should be kept
        * on one page if possible
        */
        private bool keepTogether;

        /**
        * Indicates if the PdfPTable is complete once added to the document.
        * @since	iText 2.0.8
        */
        protected bool complete = true;
        
		private int footerRows;

        protected PdfPTable() {
        }
        
        /** Constructs a <CODE>PdfPTable</CODE> with the relative column widths.
        * @param relativeWidths the relative column widths
        */    
        public PdfPTable(float[] relativeWidths) {
            if (relativeWidths == null)
                throw new ArgumentNullException("The widths array in PdfPTable constructor can not be null.");
            if (relativeWidths.Length == 0)
                throw new ArgumentException("The widths array in PdfPTable constructor can not have zero length.");
            this.relativeWidths = new float[relativeWidths.Length];
            Array.Copy(relativeWidths, 0, this.relativeWidths, 0, relativeWidths.Length);
            absoluteWidths = new float[relativeWidths.Length];
            CalculateWidths();
            currentRow = new PdfPCell[absoluteWidths.Length];
            keepTogether = false;
        }
        
        /** Constructs a <CODE>PdfPTable</CODE> with <CODE>numColumns</CODE> columns.
        * @param numColumns the number of columns
        */    
        public PdfPTable(int numColumns) {
            if (numColumns <= 0)
                throw new ArgumentException("The number of columns in PdfPTable constructor must be greater than zero.");
            relativeWidths = new float[numColumns];
            for (int k = 0; k < numColumns; ++k)
                relativeWidths[k] = 1;
            absoluteWidths = new float[relativeWidths.Length];
            CalculateWidths();
            currentRow = new PdfPCell[absoluteWidths.Length];
            keepTogether = false;
        }
        
        /** Constructs a copy of a <CODE>PdfPTable</CODE>.
        * @param table the <CODE>PdfPTable</CODE> to be copied
        */    
        public PdfPTable(PdfPTable table) {
            CopyFormat(table);
            for (int k = 0; k < currentRow.Length; ++k) {
                if (table.currentRow[k] == null)
                    break;
                currentRow[k] = new PdfPCell(table.currentRow[k]);
            }
            for (int k = 0; k < table.rows.Count; ++k) {
                PdfPRow row = (PdfPRow)(table.rows[k]);
                if (row != null)
                    row = new PdfPRow(row);
                rows.Add(row);
            }
        }
        
        /**
        * Makes a shallow copy of a table (format without content).
        * @param table
        * @return a shallow copy of the table
        */
        public static PdfPTable ShallowCopy(PdfPTable table) {
            PdfPTable nt = new PdfPTable();
            nt.CopyFormat(table);
            return nt;
        }

        /**
        * Copies the format of the sourceTable without copying the content. 
        * @param sourceTable
        */
        private void CopyFormat(PdfPTable sourceTable) {
            relativeWidths = new float[sourceTable.relativeWidths.Length];
            absoluteWidths = new float[sourceTable.relativeWidths.Length];
            Array.Copy(sourceTable.relativeWidths, 0, relativeWidths, 0, relativeWidths.Length);
            Array.Copy(sourceTable.absoluteWidths, 0, absoluteWidths, 0, relativeWidths.Length);
            totalWidth = sourceTable.totalWidth;
            totalHeight = sourceTable.totalHeight;
            currentRowIdx = 0;
            tableEvent = sourceTable.tableEvent;
            runDirection = sourceTable.runDirection;
            defaultCell = new PdfPCell(sourceTable.defaultCell);
            currentRow = new PdfPCell[sourceTable.currentRow.Length];
            isColspan = sourceTable.isColspan;
            splitRows = sourceTable.splitRows;
            spacingAfter = sourceTable.spacingAfter;
            spacingBefore = sourceTable.spacingBefore;
            headerRows = sourceTable.headerRows;
			footerRows = sourceTable.footerRows;
            lockedWidth = sourceTable.lockedWidth;
            extendLastRow = sourceTable.extendLastRow;
            headersInEvent = sourceTable.headersInEvent;
            widthPercentage = sourceTable.widthPercentage;
            splitLate = sourceTable.splitLate;
            skipFirstHeader = sourceTable.skipFirstHeader;
            horizontalAlignment = sourceTable.horizontalAlignment;
            keepTogether = sourceTable.keepTogether;
            complete = sourceTable.complete;
        }

        /** Sets the relative widths of the table.
        * @param relativeWidths the relative widths of the table.
        * @throws DocumentException if the number of widths is different than the number
        * of columns
        */    
        public void SetWidths(float[] relativeWidths) {
            if (relativeWidths.Length != this.relativeWidths.Length)
                throw new DocumentException("Wrong number of columns.");
            this.relativeWidths = new float[relativeWidths.Length];
            Array.Copy(relativeWidths, 0, this.relativeWidths, 0, relativeWidths.Length);
            absoluteWidths = new float[relativeWidths.Length];
            totalHeight = 0;
            CalculateWidths();
            CalculateHeights();
        }

        /** Sets the relative widths of the table.
        * @param relativeWidths the relative widths of the table.
        * @throws DocumentException if the number of widths is different than the number
        * of columns
        */    
        public void SetWidths(int[] relativeWidths) {
            float[] tb = new float[relativeWidths.Length];
            for (int k = 0; k < relativeWidths.Length; ++k)
                tb[k] = relativeWidths[k];
            SetWidths(tb);
        }

        private void CalculateWidths() {
            if (totalWidth <= 0)
                return;
            float total = 0;
            for (int k = 0; k < absoluteWidths.Length; ++k) {
                total += relativeWidths[k];
            }
            for (int k = 0; k < absoluteWidths.Length; ++k) {
                absoluteWidths[k] = totalWidth * relativeWidths[k] / total;
            }
        }
        
        /** Sets the full width of the table from the absolute column width.
        * @param columnWidth the absolute width of each column
        * @throws DocumentException if the number of widths is different than the number
        * of columns
        */    
        public void SetTotalWidth(float[] columnWidth) {
            if (columnWidth.Length != this.relativeWidths.Length)
                throw new DocumentException("Wrong number of columns.");
            totalWidth = 0;
            for (int k = 0; k < columnWidth.Length; ++k)
                totalWidth += columnWidth[k];
            SetWidths(columnWidth);
        }

        /** Sets the percentage width of the table from the absolute column width.
        * @param columnWidth the absolute width of each column
        * @param pageSize the page size
        * @throws DocumentException
        */    
        public void SetWidthPercentage(float[] columnWidth, Rectangle pageSize) {
            if (columnWidth.Length != this.relativeWidths.Length)
                throw new ArgumentException("Wrong number of columns.");
            float totalWidth = 0;
            for (int k = 0; k < columnWidth.Length; ++k)
                totalWidth += columnWidth[k];
            widthPercentage = totalWidth / (pageSize.Right - pageSize.Left) * 100f;
            SetWidths(columnWidth);
        }

        /** Gets the full width of the table.
        * @return the full width of the table
        */    
        public float TotalWidth {
            get {
                return totalWidth;
            }
            set {
                if (this.totalWidth == value)
                    return;
                this.totalWidth = value;
                totalHeight = 0;
                CalculateWidths();
                CalculateHeights();
            }
        }

        internal void CalculateHeights() {
            if (totalWidth <= 0)
                return;
            totalHeight = 0;
            for (int k = 0; k < rows.Count; ++k) {
                PdfPRow row = (PdfPRow)rows[k];
                if (row != null) {
                    row.SetWidths(absoluteWidths);
                    totalHeight += row.MaxHeights;
                }
            }
        }
        
        /**
        * Calculates the heights of the table.
        */
        public void CalculateHeightsFast() {
            if (totalWidth <= 0)
                return;
            totalHeight = 0;
            for (int k = 0; k < rows.Count; ++k) {
                PdfPRow row = (PdfPRow)rows[k];
                if (row != null)
                    totalHeight += row.MaxHeights;
            }
        }
        
        /** Gets the default <CODE>PdfPCell</CODE> that will be used as
        * reference for all the <CODE>addCell</CODE> methods except
        * <CODE>addCell(PdfPCell)</CODE>.
        * @return default <CODE>PdfPCell</CODE>
        */    
        public PdfPCell DefaultCell {
            get {
                return defaultCell;
            }
        }
        
        /** Adds a cell element.
        * @param cell the cell element
        */    
        public void AddCell(PdfPCell cell) {
            PdfPCell ncell = new PdfPCell(cell);
            int colspan = ncell.Colspan;
            colspan = Math.Max(colspan, 1);
            colspan = Math.Min(colspan, currentRow.Length - currentRowIdx);
            ncell.Colspan = colspan;
            if (colspan != 1)
                isColspan = true;
            int rdir = ncell.RunDirection;
            if (rdir == PdfWriter.RUN_DIRECTION_DEFAULT)
                ncell.RunDirection = runDirection;
            currentRow[currentRowIdx] = ncell;
            currentRowIdx += colspan;
            if (currentRowIdx >= currentRow.Length) {
                if (runDirection == PdfWriter.RUN_DIRECTION_RTL) {
                    PdfPCell[] rtlRow = new PdfPCell[absoluteWidths.Length];
                    int rev = currentRow.Length;
                    for (int k = 0; k < currentRow.Length; ++k) {
                        PdfPCell rcell = currentRow[k];
                        int cspan = rcell.Colspan;
                        rev -= cspan;
                        rtlRow[rev] = rcell;
                        k += cspan - 1;
                    }
                    currentRow = rtlRow;
                }
                PdfPRow row = new PdfPRow(currentRow);
                if (totalWidth > 0) {
                    row.SetWidths(absoluteWidths);
                    totalHeight += row.MaxHeights;
                }
                rows.Add(row);
                currentRow = new PdfPCell[absoluteWidths.Length];
                currentRowIdx = 0;
            }
        }
        
        /** Adds a cell element.
        * @param text the text for the cell
        */    
        public void AddCell(String text) {
            AddCell(new Phrase(text));
        }
        
        /**
        * Adds a nested table.
        * @param table the table to be added to the cell
        */    
        public void AddCell(PdfPTable table) {
            defaultCell.Table = table;
            AddCell(defaultCell);
            defaultCell.Table = null;
        }
        
        /**
        * Adds an Image as Cell.
        * @param image the <CODE>Image</CODE> to add to the table. This image will fit in the cell
        */    
        public void AddCell(Image image) {
            defaultCell.Image = image;
            AddCell(defaultCell);
            defaultCell.Image = null;
        }
        
        /**
        * Adds a cell element.
        * @param phrase the <CODE>Phrase</CODE> to be added to the cell
        */    
        public void AddCell(Phrase phrase) {
            defaultCell.Phrase = phrase;
            AddCell(defaultCell);
            defaultCell.Phrase = null;
        }
        
        /**
        * Writes the selected rows to the document.
        * <P>
        * <CODE>canvases</CODE> is obtained from <CODE>beginWritingRows()</CODE>.
        * @param rowStart the first row to be written, zero index
        * @param rowEnd the last row to be written + 1. If it is -1 all the
        * rows to the end are written
        * @param xPos the x write coodinate
        * @param yPos the y write coodinate
        * @param canvases an array of 4 <CODE>PdfContentByte</CODE> obtained from
        * <CODE>beginWrittingRows()</CODE>
        * @return the y coordinate position of the bottom of the last row
        * @see #beginWritingRows(com.lowagie.text.pdf.PdfContentByte)
        */    
        public float WriteSelectedRows(int rowStart, int rowEnd, float xPos, float yPos, PdfContentByte[] canvases) {
            return WriteSelectedRows(0, -1, rowStart, rowEnd, xPos, yPos, canvases);
        }
        
        /** Writes the selected rows and columns to the document.
        * This method does not clip the columns; this is only important
        * if there are columns with colspan at boundaries.
        * <P>
        * <CODE>canvases</CODE> is obtained from <CODE>beginWritingRows()</CODE>.
        * <P>
        * The table event is only fired for complete rows.
        * @param colStart the first column to be written, zero index
        * @param colEnd the last column to be written + 1. If it is -1 all the
        * columns to the end are written
        * @param rowStart the first row to be written, zero index
        * @param rowEnd the last row to be written + 1. If it is -1 all the
        * rows to the end are written
        * @param xPos the x write coodinate
        * @param yPos the y write coodinate
        * @param canvases an array of 4 <CODE>PdfContentByte</CODE> obtained from
        * <CODE>beginWrittingRows()</CODE>
        * @return the y coordinate position of the bottom of the last row
        * @see #beginWritingRows(com.lowagie.text.pdf.PdfContentByte)
        */    
        public float WriteSelectedRows(int colStart, int colEnd, int rowStart, int rowEnd, float xPos, float yPos, PdfContentByte[] canvases) {
            if (totalWidth <= 0)
                throw new ArgumentException("The table width must be greater than zero.");
            int size = rows.Count;
            if (rowEnd < 0)
                rowEnd = size;
            rowEnd = Math.Min(rowEnd, size);
            if (rowStart < 0)
                rowStart = 0;
            if (rowStart >= rowEnd)
                return yPos;
            if (colEnd < 0)
                colEnd = absoluteWidths.Length;
            colEnd = Math.Min(colEnd, absoluteWidths.Length);
            if (colStart < 0)
                colStart = 0;
            colStart = Math.Min(colStart, absoluteWidths.Length);
            float yPosStart = yPos;
            for (int k = rowStart; k < rowEnd; ++k) {
                PdfPRow row = (PdfPRow)rows[k];
                if (row != null) {
                    row.WriteCells(colStart, colEnd, xPos, yPos, canvases);
                    yPos -= row.MaxHeights;
                }
            }
            if (tableEvent != null && colStart == 0 && colEnd == absoluteWidths.Length) {
                float[] heights = new float[rowEnd - rowStart + 1];
                heights[0] = yPosStart;
                for (int k = rowStart; k < rowEnd; ++k) {
                    PdfPRow row = (PdfPRow)rows[k];
                    float hr = 0;
                    if (row != null)
                        hr = row.MaxHeights;
                    heights[k - rowStart + 1] = heights[k - rowStart] - hr;
                }
                tableEvent.TableLayout(this, GetEventWidths(xPos, rowStart, rowEnd, headersInEvent), heights, headersInEvent ? headerRows : 0, rowStart, canvases);
            }
            return yPos;
        }
        
        /**
        * Writes the selected rows to the document.
        * 
        * @param rowStart the first row to be written, zero index
        * @param rowEnd the last row to be written + 1. If it is -1 all the
        * rows to the end are written
        * @param xPos the x write coodinate
        * @param yPos the y write coodinate
        * @param canvas the <CODE>PdfContentByte</CODE> where the rows will
        * be written to
        * @return the y coordinate position of the bottom of the last row
        */    
        public float WriteSelectedRows(int rowStart, int rowEnd, float xPos, float yPos, PdfContentByte canvas) {
            return WriteSelectedRows(0, -1, rowStart, rowEnd, xPos, yPos, canvas);
        }
        
        /**
        * Writes the selected rows to the document.
        * This method clips the columns; this is only important
        * if there are columns with colspan at boundaries.
        * <P>
        * The table event is only fired for complete rows.
        * 
        * @param colStart the first column to be written, zero index
        * @param colEnd the last column to be written + 1. If it is -1 all the
        * @param rowStart the first row to be written, zero index
        * @param rowEnd the last row to be written + 1. If it is -1 all the
        * rows to the end are written
        * @param xPos the x write coodinate
        * @param yPos the y write coodinate
        * @param canvas the <CODE>PdfContentByte</CODE> where the rows will
        * be written to
        * @return the y coordinate position of the bottom of the last row
        */    
        public float WriteSelectedRows(int colStart, int colEnd, int rowStart, int rowEnd, float xPos, float yPos, PdfContentByte canvas) {
            if (colEnd < 0)
                colEnd = absoluteWidths.Length;
            colEnd = Math.Min(colEnd, absoluteWidths.Length);
            if (colStart < 0)
                colStart = 0;
            colStart = Math.Min(colStart, absoluteWidths.Length);
            if (colStart != 0 || colEnd != absoluteWidths.Length) {
                float w = 0;
                for (int k = colStart; k < colEnd; ++k)
                    w += absoluteWidths[k];
                canvas.SaveState();
                float lx = 0;
                float rx = 0;
                if (colStart == 0)
                    lx = 10000;
                if (colEnd == absoluteWidths.Length)
                    rx = 10000;
                canvas.Rectangle(xPos - lx, -10000, w + lx + rx, 20000);
                canvas.Clip();
                canvas.NewPath();
            }
            PdfContentByte[] canvases = BeginWritingRows(canvas);
            float y = WriteSelectedRows(colStart, colEnd, rowStart, rowEnd, xPos, yPos, canvases);
            EndWritingRows(canvases);
            if (colStart != 0 || colEnd != absoluteWidths.Length)
                canvas.RestoreState();
            return y;
        }
        
        /** Gets and initializes the 4 layers where the table is written to. The text or graphics are added to
        * one of the 4 <CODE>PdfContentByte</CODE> returned with the following order:<p>
        * <ul>
        * <li><CODE>PdfPtable.BASECANVAS</CODE> - the original <CODE>PdfContentByte</CODE>. Anything placed here
        * will be under the table.
        * <li><CODE>PdfPtable.BACKGROUNDCANVAS</CODE> - the layer where the background goes to.
        * <li><CODE>PdfPtable.LINECANVAS</CODE> - the layer where the lines go to.
        * <li><CODE>PdfPtable.TEXTCANVAS</CODE> - the layer where the text go to. Anything placed here
        * will be over the table.
        * </ul><p>
        * The layers are placed in sequence on top of each other.
        * @param canvas the <CODE>PdfContentByte</CODE> where the rows will
        * be written to
        * @return an array of 4 <CODE>PdfContentByte</CODE>
        * @see #writeSelectedRows(int, int, float, float, PdfContentByte[])
        */    
        public static PdfContentByte[] BeginWritingRows(PdfContentByte canvas) {
            return new PdfContentByte[]{
                canvas,
                canvas.Duplicate,
                canvas.Duplicate,
                canvas.Duplicate,
            };
        }
        
        /** Finishes writing the table.
        * @param canvases the array returned by <CODE>beginWritingRows()</CODE>
        */    
        public static void EndWritingRows(PdfContentByte[] canvases) {
            PdfContentByte canvas = canvases[BASECANVAS];
            canvas.SaveState();
            canvas.Add(canvases[BACKGROUNDCANVAS]);
            canvas.RestoreState();
            canvas.SaveState();
            canvas.SetLineCap(2);
            canvas.ResetRGBColorStroke();
            canvas.Add(canvases[LINECANVAS]);
            canvas.RestoreState();
            canvas.Add(canvases[TEXTCANVAS]);
        }
        
        /** Gets the number of rows in this table.
        * @return the number of rows in this table
        */    
        public int Size {
            get {
                return rows.Count;
            }
        }
        
        /** Gets the total height of the table.
        * @return the total height of the table
        */    
        public float TotalHeight {
            get {
                return totalHeight;
            }
        }
        
        /** Gets the height of a particular row.
        * @param idx the row index (starts at 0)
        * @return the height of a particular row
        */    
        public float GetRowHeight(int idx) {
            if (totalWidth <= 0 || idx < 0 || idx >= rows.Count)
                return 0;
            PdfPRow row = (PdfPRow)rows[idx];
            if (row == null)
                return 0;
            return row.MaxHeights;
        }
        
        /** Gets the height of the rows that constitute the header as defined by
        * <CODE>setHeaderRows()</CODE>.
        * @return the height of the rows that constitute the header and footer
        */    
        public float HeaderHeight {
            get {
                float total = 0;
                int size = Math.Min(rows.Count, headerRows);
                for (int k = 0; k < size; ++k) {
                    PdfPRow row = (PdfPRow)rows[k];
                    if (row != null)
                        total += row.MaxHeights;
                }
                return total;
            }
        }
        
        /** Gets the height of the rows that constitute the header as defined by
        * <CODE>setFooterRows()</CODE>.
        * @return the height of the rows that constitute the footer
        * @since 2.1.1
        */    
        public float FooterHeight {
            get {
                float total = 0;
                int start = Math.Min(0, headerRows - footerRows);
                int size = Math.Min(rows.Count, footerRows);
                for (int k = start; k < size; ++k) {
                    PdfPRow row = (PdfPRow)rows[k];
                    if (row != null)
                        total += row.MaxHeights;
                }
                return total;
            }
        }
        
        /** Deletes a row from the table.
        * @param rowNumber the row to be deleted
        * @return <CODE>true</CODE> if the row was deleted
        */    
        public bool DeleteRow(int rowNumber) {
            if (rowNumber < 0 || rowNumber >= rows.Count) {
                return false;
            }
            if (totalWidth > 0) {
                PdfPRow row = (PdfPRow)rows[rowNumber];
                if (row != null)
                    totalHeight -= row.MaxHeights;
            }
            rows.RemoveAt(rowNumber);
            return true;
        }
        
        /** Deletes the last row in the table.
        * @return <CODE>true</CODE> if the last row was deleted
        */    
        public bool DeleteLastRow() {
            return DeleteRow(rows.Count - 1);
        }
        
        /**
        * Removes all of the rows except headers
        */
        public void DeleteBodyRows() {
            ArrayList rows2 = new ArrayList();
            for (int k = 0; k < headerRows; ++k)
                rows2.Add(rows[k]);
            rows = rows2;
            totalHeight = 0;
            if (totalWidth > 0)
                totalHeight = HeaderHeight;
        }

        /** Returns the number of columns.
        * @return  the number of columns.
        * @since   2.1.1
        */
        public int NumberOfColumns {
            get {
                return relativeWidths.Length;
            }
        }
        public int HeaderRows {
            get {
                return headerRows;
            }
            set {
                headerRows = value;
                if (headerRows < 0)
                    headerRows = 0;
            }
        }
        
        public int FooterRows {
            get {
                return footerRows;
            }
            set {
                footerRows = value;
                if (footerRows < 0)
                    footerRows = 0;
            }
        }
        
		/**
        * Gets all the chunks in this element.
        *
        * @return    an <CODE>ArrayList</CODE>
        */
        public ArrayList Chunks {
            get {
                return new ArrayList();
            }
        }
        
        /**
        * Gets the type of the text element.
        *
        * @return    a type
        */
        public int Type {
            get {
                return Element.PTABLE;
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
            return true;
        }
  
        /**
        * Processes the element by adding it (or the different parts) to an
        * <CODE>ElementListener</CODE>.
        *
        * @param    listener    an <CODE>ElementListener</CODE>
        * @return    <CODE>true</CODE> if the element was processed successfully
        */
        public bool Process(IElementListener listener) {
            try {
                return listener.Add(this);
            }
            catch (DocumentException) {
                return false;
            }
        }
                        
        public float WidthPercentage {
            get {
                return widthPercentage;
            }
            set {
                widthPercentage = value;
            }
        }

        public int HorizontalAlignment {
            get {
                return horizontalAlignment;
            }
            set {
                horizontalAlignment = value;
            }
        }

        /**
        * Gets a row with a given index
        * (added by Jin-Hsia Yang).
        * @param idx
        * @return the row at position idx
        */
        public PdfPRow GetRow(int idx) {
            return (PdfPRow)rows[idx];
        }

        /**
        * Gets an arraylist with all the rows in the table.
        * @return an arraylist
        */
        public ArrayList Rows {
            get {
                return rows;
            }
        }

        public IPdfPTableEvent TableEvent {
            get {
                return tableEvent;
            }
            set {
                if (value == null) this.tableEvent = null;
                else if (this.tableEvent == null) this.tableEvent = value;
                else if (this.tableEvent is PdfPTableEventForwarder) ((PdfPTableEventForwarder)this.tableEvent).AddTableEvent(value);
                else {
                    PdfPTableEventForwarder forward = new PdfPTableEventForwarder();
                    forward.AddTableEvent(this.tableEvent);
                    forward.AddTableEvent(value);
                    this.tableEvent = forward;
                }
            }
        }

        /** Gets the absolute sizes of each column width.
        * @return he absolute sizes of each column width
        */    
        public float[] AbsoluteWidths {
            get {
                return absoluteWidths;
            }
        }
        
        internal float [][] GetEventWidths(float xPos, int firstRow, int lastRow, bool includeHeaders) {
            if (includeHeaders) {
                firstRow = Math.Max(firstRow, headerRows);
                lastRow = Math.Max(lastRow, headerRows);
            }
            float[][] widths = new float[(includeHeaders ? headerRows : 0) + lastRow - firstRow][];
            if (isColspan) {
                int n = 0;
                if (includeHeaders) {
                    for (int k = 0; k < headerRows; ++k) {
                        PdfPRow row = (PdfPRow)rows[k];
                        if (row == null)
                            ++n;
                        else
                            widths[n++] = row.GetEventWidth(xPos);
                    }
                }
                for (; firstRow < lastRow; ++firstRow) {
                        PdfPRow row = (PdfPRow)rows[firstRow];
                        if (row == null)
                            ++n;
                        else
                            widths[n++] = row.GetEventWidth(xPos);
                }
            }
            else {
                float[] width = new float[absoluteWidths.Length + 1];
                width[0] = xPos;
                for (int k = 0; k < absoluteWidths.Length; ++k)
                    width[k + 1] = width[k] + absoluteWidths[k];
                for (int k = 0; k < widths.Length; ++k)
                    widths[k] = width;
            }
            return widths;
        }

        public bool SkipFirstHeader {
            get {
                return skipFirstHeader;
            }
            set {
                skipFirstHeader = value;
            }
        }

        public int RunDirection {
            get {
                return runDirection;
            }
            set {
                if (value < PdfWriter.RUN_DIRECTION_DEFAULT || value > PdfWriter.RUN_DIRECTION_RTL)
                    throw new ArgumentException("Invalid run direction: " + value);
                runDirection = value;
            }
        }
        
        public bool LockedWidth {
            get {
                return lockedWidth;
            }
            set {
                lockedWidth = value;
            }
        }
                
        public bool SplitRows {
            get {
                return splitRows;
            }
            set {
                splitRows = value;
            }
        }

        public float SpacingBefore {
            get {
                return spacingBefore;
            }
            set {
                spacingBefore = value;
            }
        }
        
        public float SpacingAfter {
            get {
                return spacingAfter;
            }
            set {
                spacingAfter = value;
            }
        }

        public bool ExtendLastRow {
            get {
                return extendLastRow;
            }
            set {
                extendLastRow = value;
            }
        }
        
        public bool HeadersInEvent {
            get {
                return headersInEvent;
            }
            set {
                headersInEvent = value;
            }
        }
        public bool SplitLate {
            get {
                return splitLate;
            }
            set {
                splitLate = value;
            }
        }

        /**
        * If true the table will be kept on one page if it fits, by forcing a 
        * new page if it doesn't fit on the current page. The default is to
        * split the table over multiple pages.
        *
        * @param p_KeepTogether whether to try to keep the table on one page
        */
        public bool KeepTogether {
            set {
                keepTogether = value;
            }
            get {
                return keepTogether;
            }
        }

        /**
        * Completes the current row with the default cell. An incomplete row will be dropped
        * but calling this method will make sure that it will be present in the table.
        */
        public void CompleteRow() {
            while (currentRowIdx > 0) {
                AddCell(defaultCell);
            }
        }

        /**
        * @since   iText 2.0.8
        * @see com.lowagie.text.LargeElement#flushContent()
        */
        public void FlushContent() {
            DeleteBodyRows();
            SkipFirstHeader = true;
        }

        /**
        * @since   iText 2.0.8
        * @see com.lowagie.text.LargeElement#isComplete()
        */
        public bool ElementComplete {
            get {
                return complete;
            }
            set {
                complete = value;
            }
        }
    }
}