using System;
using System.Collections;
using System.util;

/*
 * $Id: Row.cs,v 1.10 2008/05/13 11:25:12 psoares33 Exp $
 * 
 *
 * Copyright 1999, 2000, 2001, 2002 by Bruno Lowagie.
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
 * of this file under either the MPL or the GNU LIBRARY GENERAL PUBLIC LICENSE
 *
 * This library is free software; you can redistribute it and/or modify it
 * under the terms of the MPL as stated above or under the terms of the GNU
 * Library General Public License as published by the Free Software Foundation;
 * either version 2 of the License, or any later version.
 *
 * This library is distributed in the hope that it will be useful, but WITHOUT
 * ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS
 * FOR A PARTICULAR PURPOSE. See the GNU LIBRARY GENERAL PUBLIC LICENSE for more
 * details.
 *
 * If you didn't download this code from the following link, you should check if
 * you aren't using an obsolete version:
 * http://www.lowagie.com/iText/
 */

namespace iTextSharp.text {
    /// <summary>
    /// A Row is part of a Table
    /// and contains some Cells.
    /// </summary>
    /// <remarks>
    /// All Rows are constructed by a Table-object.
    /// You don't have to construct any Row yourself.
    /// In fact you can't construct a Row outside the package.
    /// <P/>
    /// Since a Cell can span several rows and/or columns
    /// a row can contain reserved space without any content.
    /// </remarks>
    /// <seealso cref="T:iTextSharp.text.Element"/>
    /// <seealso cref="T:iTextSharp.text.Cell"/>
    /// <seealso cref="T:iTextSharp.text.Table"/>
    public class Row : IElement {
    
        // membervariables
    
        /// <summary> id of a null element in a Row</summary>
        public static int NULL = 0;
    
        /// <summary> id of the Cell element in a Row</summary>
        public static int CELL = 1;
    
        /// <summary> id of the Table element in a Row</summary>
        public static int TABLE = 2;
    
        /// <summary> This is the number of columns in the Row. </summary>
        protected int columns;
    
        /// <summary> This is a valid position the Row. </summary>
        protected int currentColumn;
    
        /// <summary> This is the array that keeps track of reserved cells. </summary>
        protected bool[] reserved;
    
        /// <summary> This is the array of Objects (Cell or Table). </summary>
        protected Object[] cells;
    
        /// <summary> This is the horizontal alignment. </summary>
        protected int horizontalAlignment;
    
        // constructors
    
        /// <summary>
        /// Constructs a Row with a certain number of columns.
        /// </summary>
        /// <param name="columns">a number of columns</param>
        internal Row(int columns) {
            this.columns = columns;
            reserved = new bool[columns];
            cells = new Object[columns];
            currentColumn = 0;
        }
    
        // implementation of the Element-methods
    
        /// <summary>
        /// Processes the element by adding it (or the different parts) to a
        /// IElementListener.
        /// </summary>
        /// <param name="listener">an IElementListener</param>
        /// <returns>true if the element was processed successfully</returns>
        public bool Process(IElementListener listener) {
            try {
                return listener.Add(this);
            }
            catch (DocumentException) {
                return false;
            }
        }
    
        /// <summary>
        /// Gets the type of the text element.
        /// </summary>
        /// <value>a type</value>
        public int Type {
            get {
                return Element.ROW;
            }
        }
    
        /// <summary>
        /// Gets all the chunks in this element.
        /// </summary>
        /// <value>an ArrayList</value>
        public ArrayList Chunks {
            get {
                return new ArrayList();
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

        /// <summary>
        /// Deletes a certain column has been deleted.
        /// </summary>
        /// <param name="column">the number of the column to delete</param>
        internal void DeleteColumn(int column) {
            if ((column >= columns) || (column < 0)) {
                throw new Exception("getCell at illegal index : " + column);
            }
            columns--;
            bool[] newReserved = new bool[columns];
                Object[] newCells = new Cell[columns];
        
                    for (int i = 0; i < column; i++) {
                        newReserved[i] = reserved[i];
                        newCells[i] = cells[i];
                        if (newCells[i] != null && (i + ((Cell) newCells[i]).Colspan > column)) {
                            ((Cell) newCells[i]).Colspan = ((Cell) cells[i]).Colspan - 1;
                        }
                    }
            for (int i = column; i < columns; i++) {
                newReserved[i] = reserved[i + 1];
                newCells[i] = cells[i + 1];
            }
            if (cells[column] != null && ((Cell) cells[column]).Colspan > 1) {
                newCells[column] = cells[column];
                ((Cell) newCells[column]).Colspan = ((Cell) newCells[column]).Colspan - 1;
            }
            reserved = newReserved;
            cells = newCells;
        }
    
        // methods
    
        /// <summary>
        /// Adds a Cell to the Row.
        /// </summary>
        /// <param name="element">the element to add (currently only Cells and Tables supported)</param>
        /// <returns>
        /// the column position the Cell was added,
        ///        or -1 if the element couldn't be added.
        /// </returns>
        internal int AddElement(Object element) {
            return AddElement(element, currentColumn);
        }
    
        /// <summary>
        /// Adds an element to the Row at the position given.
        /// </summary>
        /// <param name="element">the element to add. (currently only Cells and Tables supported</param>
        /// <param name="column">the position where to add the cell</param>
        /// <returns>
        /// the column position the Cell was added,
        ///        or -1 if the Cell couldn't be added.
        /// </returns>
        internal int AddElement(Object element, int column) {
            if (element == null) throw new Exception("addCell - null argument");
            if ((column < 0) || (column > columns)) throw new Exception("addCell - illegal column argument");
            if ( !((GetObjectID(element) == CELL) || (GetObjectID(element) == TABLE)) ) throw new ArgumentException("addCell - only Cells or Tables allowed");
        
            int lColspan = ((element is Cell) ? ((Cell)element).Colspan : 1);
        
            if (!Reserve(column, lColspan)) {
                return -1;
            }
        
            cells[column] = element;
            currentColumn += lColspan - 1;
        
            return column;
        }
    
        /// <summary>
        /// Puts Cell to the Row at the position given, doesn't reserve colspan.
        /// </summary>
        /// <param name="aElement">the cell to add.</param>
        /// <param name="column">the position where to add the cell.</param>
        internal void SetElement(Object aElement, int column) {
            if (reserved[column]) throw new ArgumentException("setElement - position already taken");
        
            cells[column] = aElement;
            if (aElement != null) {
                reserved[column] = true;
            }
        }
    
        /// <summary>
        /// Reserves a Cell in the Row.
        /// </summary>
        /// <param name="column">the column that has to be reserved.</param>
        /// <returns>true if the column was reserved, false if not.</returns>
        internal bool Reserve(int column) {
            return Reserve(column, 1);
        }
    
    
        /// <summary>
        /// Reserves a Cell in the Row.
        /// </summary>
        /// <param name="column">the column that has to be reserved.</param>
        /// <param name="size">the number of columns</param>
        /// <returns>true if the column was reserved, false if not.</returns>
        internal bool Reserve(int column, int size) {
            if ((column < 0) || ((column + size) > columns)) throw new Exception("reserve - incorrect column/size");
        
            for (int i=column; i < column + size; i++) {
                if (reserved[i]) {
                    // undo reserve
                    for (int j=i; j >= column; j--) {
                        reserved[j] = false;
                    }
                    return false;
                }
                reserved[i] = true;
            }
            return true;
        }
    
        // methods to retrieve information
    
        /// <summary>
        /// Returns true/false when this position in the Row has been reserved, either filled or through a colspan of an Element.
        /// </summary>
        /// <param name="column">the column.</param>
        /// <returns>true if the column was reserved, false if not.</returns>
        internal bool IsReserved(int column) {
            return reserved[column];
        }
    
        /// <summary>
        /// Returns the type-id of the element in a Row.
        /// </summary>
        /// <param name="column">the column of which you'd like to know the type</param>
        /// <returns>the element id</returns>
        int GetElementID(int column) {
            if (cells[column] == null) return NULL;
            else if (cells[column] is Cell) return CELL;
            else if (cells[column] is Table) return TABLE;
        
            return -1;
        }
    
    
        /// <summary>
        /// Returns the type-id of an Object.
        /// </summary>
        /// <param name="element"></param>
        /// <returns>the object of which you'd like to know the type-id, -1 if invalid</returns>
        int GetObjectID(Object element) {
            if (element == null) return NULL;
            else if (element is Cell) return CELL;
            else if (element is Table) return TABLE;
        
            return -1;
        }
    
    
        /// <summary>
        /// Gets a Cell or Table from a certain column.
        /// </summary>
        /// <param name="column">the column the Cell/Table is in.</param>
        /// <returns>
        /// the Cell,Table or Object if the column was
        /// reserved or null if empty.
        /// </returns>
        public Object GetCell(int column) {
            if ((column < 0) || (column > columns)) {
                throw new Exception("getCell at illegal index :" + column + " max is " + columns);
            }
            return cells[column];
        }
    
        /// <summary>
        /// Checks if the row is empty.
        /// </summary>
        /// <returns>true if none of the columns is reserved.</returns>
        public bool IsEmpty() {
            for (int i = 0; i < columns; i++) {
                if (cells[i] != null) {
                    return false;
                }
            }
            return true;
        }
    
        /// <summary>
        /// Gets the number of columns.
        /// </summary>
        /// <value>a value</value>
        public int Columns {
            get {
                return columns;
            }
        }
    
        /// <summary>
        /// Gets the horizontal Element.
        /// </summary>
        /// <value>a value</value>
        public int HorizontalAlignment {
            get {
                return horizontalAlignment;
            }
            set {
                horizontalAlignment = value;
            }
        }
    
        public override string ToString() {
            return base.ToString();
        }
    }
}
