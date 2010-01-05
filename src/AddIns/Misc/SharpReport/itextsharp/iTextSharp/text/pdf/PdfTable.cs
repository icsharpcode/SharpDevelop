using System;
using System.Collections;

using iTextSharp.text;

/*
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
     * <CODE>PdfTable</CODE> is an object that contains the graphics and text of a table.
     *
     * @see     iTextSharp.text.Table
     * @see     iTextSharp.text.Row
     * @see     iTextSharp.text.Cell
     * @see     PdfCell
     */

    public class PdfTable : Rectangle {
    
        // membervariables
    
        /** this is the number of columns in the table. */
        private int columns;
    
        /** this is the ArrayList with all the cell of the table header. */
        private ArrayList headercells;
    
        /** this is the ArrayList with all the cells in the table. */
        private ArrayList cells;
    
        /** Original table used to build this object*/
        protected Table table;
        
        /** Cached column widths. */
        protected float[] positions;
    
        // constructors
    
        /**
         * Constructs a <CODE>PdfTable</CODE>-object.
         *
         * @param   table   a <CODE>Table</CODE>
         * @param   left    the left border on the page
         * @param   right   the right border on the page
         * @param   top     the start position of the top of the table
         */
    
        internal PdfTable(Table table, float left, float right, float top) : base(left, top, right, top) {
            // constructs a Rectangle (the bottomvalue will be changed afterwards)
            this.table = table;
            table.Complete();

            // copying the attributes from class Table
            CloneNonPositionParameters(table);

            this.columns = table.Columns;
            positions = table.GetWidths(left, right - left);
            
            // initialisation of some parameters
            Left = positions[0];
            Right = positions[positions.Length - 1];
            
            headercells = new ArrayList();
            cells = new ArrayList();

            UpdateRowAdditionsInternal();
        }

        // methods

        /**
        * Updates the table row additions in the underlying table object and deletes all table rows, 
        * in order to preserve memory and detect future row additions.
        * <p><b>Pre-requisite</b>: the object must have been built with the parameter <code>supportUpdateRowAdditions</code> equals to true.
        */
        
        internal void UpdateRowAdditions() {
            table.Complete();
            UpdateRowAdditionsInternal();
            table.DeleteAllRows();
        }
        
        /**
        * Updates the table row additions in the underlying table object
        */
        
        private void UpdateRowAdditionsInternal() {
            // correct table : fill empty cells/ parse table in table
            int prevRows = Rows;
            int rowNumber = 0;
            int groupNumber = 0;
            bool groupChange;
            int firstDataRow = table.LastHeaderRow + 1;
            Cell cell;
            PdfCell currentCell;
            ArrayList newCells = new ArrayList();
            int rows = table.Size + 1;
            float[] offsets = new float[rows];
            for (int i = 0; i < rows; i++) {
                offsets[i] = Bottom;
            }
            
            // loop over all the rows
            foreach (Row row in table) {
                groupChange = false;
                if (row.IsEmpty()) {
                    if (rowNumber < rows - 1 && offsets[rowNumber + 1] > offsets[rowNumber]) offsets[rowNumber + 1] = offsets[rowNumber];
                }
                else {
                    for (int i = 0; i < row.Columns; i++) {
                        cell = (Cell) row.GetCell(i);
                        if (cell != null) {
                            currentCell = new PdfCell(cell, rowNumber+prevRows, positions[i], positions[i + cell.Colspan], offsets[rowNumber], Cellspacing, Cellpadding);
                            if (rowNumber < firstDataRow) {
                                currentCell.SetHeader();
                                headercells.Add(currentCell);
                                if (!table.NotAddedYet)
                                    continue;
                            }
                            try {
                                if (offsets[rowNumber] - currentCell.Height - Cellpadding < offsets[rowNumber + currentCell.Rowspan]) {
                                    offsets[rowNumber + currentCell.Rowspan] = offsets[rowNumber] - currentCell.Height - Cellpadding;
                                }
                            }
                            catch (ArgumentOutOfRangeException) {
                                if (offsets[rowNumber] - currentCell.Height < offsets[rows - 1]) {
                                    offsets[rows - 1] = offsets[rowNumber] - currentCell.Height;
                                }
                            }
                            currentCell.GroupNumber = groupNumber;
                            groupChange |= cell.GroupChange;
                            newCells.Add(currentCell);
                        }
                    }
                }
                rowNumber++;
                if ( groupChange ) groupNumber++;
            }
            
            // loop over all the cells
            int n = newCells.Count;
            for (int i = 0; i < n; i++) {
                currentCell = (PdfCell) newCells[i];
                try {
                    currentCell.Bottom = offsets[currentCell.Rownumber-prevRows + currentCell.Rowspan];
                }
                catch (ArgumentOutOfRangeException) {
                    currentCell.Bottom = offsets[rows - 1];
                }
            }
            cells.AddRange(newCells);
            Bottom = offsets[rows - 1];
        }

        /**
        * Get the number of rows
        */
        
        internal int Rows {
            get {
                return cells.Count == 0 ? 0 : ((PdfCell)cells[cells.Count-1]).Rownumber+1;
            }
        }

        /** @see com.lowagie.text.Element#type() */
        public override int Type {
            get {
                return Element.TABLE;
            }
        }
    
        /**
         * Returns the arraylist with the cells of the table header.
         *
         * @return  an <CODE>ArrayList</CODE>
         */
    
        internal ArrayList HeaderCells {
            get {
                return headercells;
            }
        }
    
        /**
         * Checks if there is a table header.
         *
         * @return  an <CODE>ArrayList</CODE>
         */
    
        internal bool HasHeader() {
            return headercells.Count > 0;
        }
    
        /**
         * Returns the arraylist with the cells of the table.
         *
         * @return  an <CODE>ArrayList</CODE>
         */
    
        internal ArrayList Cells {
            get {
                return cells;
            }
        }
    
        /**
         * Returns the number of columns of the table.
         *
         * @return  the number of columns
         */
    
        internal int Columns {
            get {
                return columns;
            }
        }
    
        /**
         * Returns the cellpadding of the table.
         *
         * @return  the cellpadding
         */
    
        internal float Cellpadding {
            get {
                return table.Cellpadding;
            }
        }
    
        /**
         * Returns the cellspacing of the table.
         *
         * @return  the cellspacing
         */
    
        internal float Cellspacing {
            get {
                return table.Cellspacing;
            }
        }

        /**
        * Checks if this <CODE>Table</CODE> has to fit a page.
        *
        * @return  true if the table may not be split
        */

        public bool HasToFitPageTable() {
            return table.TableFitsPage;
        }

        /**
        * Checks if the cells of this <CODE>Table</CODE> have to fit a page.
        *
        * @return  true if the cells may not be split
        */
        
        public bool HasToFitPageCells() {
            return table.CellsFitPage;
        }

        /**
        * Gets the offset of this table.
        *
        * @return  the space between this table and the previous element.
        */
        public float Offset {
            get {
                return table.Offset;
            }
        }
    }
}