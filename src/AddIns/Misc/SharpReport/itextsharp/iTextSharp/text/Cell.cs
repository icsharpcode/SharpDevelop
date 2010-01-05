using System;
using System.Collections;
using System.util;

using iTextSharp.text.html;
using iTextSharp.text.pdf;
using iTextSharp.text.factories;

/*
 * $Id: Cell.cs,v 1.17 2008/05/13 11:25:08 psoares33 Exp $
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

namespace iTextSharp.text {
    /// <summary>
    /// A Cell is a Rectangle containing other Elements.
    /// </summary>
    /// <remarks>
    /// A Cell is a Rectangle containing other
    /// Elements.
    /// <p/>
    /// A Cell must be added to a Table.
    /// The Table will place the Cell in
    /// a Row.
    /// </remarks>
    /// <example>
    /// <code>
    /// Table table = new Table(3);
    /// table.SetBorderWidth(1);
    /// table.SetBorderColor(new Color(0, 0, 255));
    /// table.SetCellpadding(5);
    /// table.SetCellspacing(5);
    /// <strong>Cell cell = new Cell("header");
    /// cell.SetHeader(true);
    /// cell.SetColspan(3);</strong>
    /// table.AddCell(cell);
    /// <strong>cell = new Cell("example cell with colspan 1 and rowspan 2");
    /// cell.SetRowspan(2);
    /// cell.SetBorderColor(new Color(255, 0, 0));</strong>
    /// table.AddCell(cell);
    /// table.AddCell("1.1");
    /// table.AddCell("2.1");
    /// table.AddCell("1.2");
    /// table.AddCell("2.2");
    /// </code>
    /// </example>
    /// <seealso cref="T:iTextSharp.text.Rectangle"/>
    /// <seealso cref="T:iTextSharp.text.Element"/>
    /// <seealso cref="T:iTextSharp.text.Table"/>
    /// <seealso cref="T:iTextSharp.text.Row"/>
    public class Cell : Rectangle, ITextElementArray {

        // static membervariable

        public static Cell DummyCell {
            get {
                Cell cell = new Cell(true);
                cell.Colspan = 3;
                cell.Border = NO_BORDER;
                return cell;
            }
        }

        // membervariables

        ///<summary> This is the ArrayList of Elements. </summary>
        protected ArrayList arrayList = null;

        ///<summary> This is the horizontal Element. </summary>
        protected int horizontalAlignment = Element.ALIGN_UNDEFINED;

        ///<summary> This is the vertical Element. </summary>
        protected int verticalAlignment = Element.ALIGN_UNDEFINED;

        ///<summary> This is the vertical Element. </summary>
        protected float width;
        protected bool percentage = false;

        ///<summary> This is the colspan. </summary>
        protected int colspan = 1;

        ///<summary> This is the rowspan. </summary>
        protected int rowspan = 1;

        ///<summary> This is the leading. </summary>
        float leading = float.NaN;

        ///<summary> Is this Cell a header? </summary>
        protected bool header;

        /// <summary>
        /// Indicates that the largest ascender height should be used to determine the
        /// height of the first line.  Note that this only has an effect when rendered
        /// to PDF.  Setting this to true can help with vertical alignment problems.
        /// </summary>
        protected bool useAscender = false;

        /// <summary>
        /// Indicates that the largest descender height should be added to the height of
        /// the last line (so characters like y don't dip into the border).   Note that
        /// this only has an effect when rendered to PDF.
        /// </summary>
        protected bool useDescender = false;

        /// <summary>
        /// Adjusts the cell contents to compensate for border widths.  Note that
        /// this only has an effect when rendered to PDF.
        /// </summary>
        protected bool useBorderPadding;

        ///<summary> Will the element have to be wrapped? </summary>
        protected bool noWrap;

        // constructors

        /**
         * Constructs an empty Cell.
         */
        /// <summary>
        /// Constructs an empty Cell.
        /// </summary>
        /// <overloads>
        /// Has five overloads.
        /// </overloads>
        public Cell() : base(0, 0, 0, 0) {
            // creates a Rectangle with BY DEFAULT a border of 0.5
            
            this.Border = UNDEFINED;
            this.BorderWidth = 0.5F;
            
            // initializes the arraylist and adds an element
            arrayList = new ArrayList();
        }

        /// <summary>
        /// Constructs an empty Cell (for internal use only).
        /// </summary>
        /// <param name="dummy">a dummy value</param>
        public Cell(bool dummy) : this() {
            arrayList.Add(new Paragraph(0));
        }

        /// <summary>
        /// Constructs a Cell with a certain content.
        /// </summary>
        /// <remarks>
        /// The string will be converted into a Paragraph.
        /// </remarks>
        /// <param name="content">a string</param>
        public Cell(string content) : this() {
            AddElement(new Paragraph(content));
        }

        /// <summary>
        /// Constructs a Cell with a certain Element.
        /// </summary>
        /// <remarks>
        /// if the element is a ListItem, Row or
        /// Cell, an exception will be thrown.
        /// </remarks>
        /// <param name="element">the element</param>
        public Cell(IElement element) : this() {
            if (element is Phrase) {
                Leading = ((Phrase)element).Leading;
            }
            AddElement(element);
        }

        // implementation of the Element-methods

        /// <summary>
        /// Processes the element by adding it (or the different parts) to an
        /// IElementListener.
        /// </summary>
        /// <param name="listener">an IElementListener</param>
        /// <returns>true if the element was processed successfully</returns>
        public override bool Process(IElementListener listener) {
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
        public override int Type {
            get {
                return Element.CELL;
            }
        }

        /// <summary>
        /// Gets all the chunks in this element.
        /// </summary>
        /// <value>an ArrayList</value>
        public override ArrayList Chunks {
            get {
                ArrayList tmp = new ArrayList();
                foreach (IElement ele in arrayList) {
                    tmp.AddRange(ele.Chunks);
                }
                return tmp;
            }
        }

        // methods to set the membervariables

        /**
         * Adds an element to this Cell.
         * <P>
         * Remark: you can't add ListItems, Rows, Cells,
         * JPEGs, GIFs or PNGs to a Cell.
         *
         * @param element The Element to add
         * @throws BadElementException if the method was called with a ListItem, Row or Cell
         */
        /// <summary>
        /// Adds an element to this Cell.
        /// </summary>
        /// <remarks>
        /// You can't add ListItems, Rows, Cells,
        /// JPEGs, GIFs or PNGs to a Cell.
        /// </remarks>
        /// <param name="element">the Element to add</param>
        public void AddElement(IElement element) {
            if (IsTable()) {
                Table table = (Table) arrayList[0];
                Cell tmp = new Cell(element);
                tmp.Border = NO_BORDER;
                tmp.Colspan = table.Columns;
                table.AddCell(tmp);
                return;
            }
            switch (element.Type) {
                case Element.LISTITEM:
                case Element.ROW:
                case Element.CELL:
                    throw new BadElementException("You can't add listitems, rows or cells to a cell.");
                case Element.JPEG:
                case Element.IMGRAW:
                case Element.IMGTEMPLATE:
                    arrayList.Add(element);
                    break;
                case Element.LIST:
                    if (float.IsNaN(this.Leading)) {
                        leading = ((List) element).TotalLeading;
                    }
                    if (((List) element).IsEmpty()) return;
                    arrayList.Add(element);
                    return;
                case Element.ANCHOR:
                case Element.PARAGRAPH:
                case Element.PHRASE:
                    if (float.IsNaN(leading)) {
                        leading = ((Phrase) element).Leading;
                    }
                    if (((Phrase) element).IsEmpty()) return;
                    arrayList.Add(element);
                    return;
                case Element.CHUNK:
                    if (((Chunk) element).IsEmpty()) return;
                    arrayList.Add(element);
                    return;
                case Element.TABLE:
                    Table table = new Table(3);
                    float[] widths = new float[3];
                    widths[1] = ((Table)element).Width;

                    switch (((Table)element).Alignment) {
                        case Element.ALIGN_LEFT:
                            widths[0] = 0f;
                            widths[2] = 100f - widths[1];
                            break;
                        case Element.ALIGN_CENTER:
                            widths[0] = (100f - widths[1]) / 2f;
                            widths[2] = widths[0];
                            break;
                        case Element.ALIGN_RIGHT:
                            widths[0] = 100f - widths[1];
                            widths[2] = 0f;
                            break;
                    }
                    table.Widths = widths;
                    Cell tmp;
                    if (arrayList.Count == 0) {
                        table.AddCell(Cell.DummyCell);
                    }
                    else {
                        tmp = new Cell();
                        tmp.Border = NO_BORDER;
                        tmp.Colspan = 3;
                        foreach (IElement ele in arrayList) {
                            tmp.Add(ele);
                        }
                        table.AddCell(tmp);
                    }
                    tmp = new Cell();
                    tmp.Border = NO_BORDER;
                    table.AddCell(tmp);
                    table.InsertTable((Table)element);
                    tmp = new Cell();
                    tmp.Border = NO_BORDER;
                    table.AddCell(tmp);
                    table.AddCell(Cell.DummyCell);
                    Clear();
                    arrayList.Add(table);
                    return;
                default:
                    arrayList.Add(element);
                    break;
            }
        }

        /// <summary>
        /// Add an Object to this cell.
        /// </summary>
        /// <param name="o">the object to add</param>
        /// <returns>always true</returns>
        public bool Add(Object o) {
            try {
                this.AddElement((IElement) o);
                return true;
            }
            catch (BadElementException bee) {
                throw new Exception(bee.Message);
            }
            catch {
                throw new Exception("You can only add objects that implement the Element interface.");
            }
        }

        /// <summary>
        /// Sets the alignment of this cell.
        /// </summary>
        /// <param name="alignment">the new alignment as a string</param>
        public void SetHorizontalAlignment(string alignment) {
            if (Util.EqualsIgnoreCase(alignment, ElementTags.ALIGN_CENTER)) {
                this.HorizontalAlignment = Element.ALIGN_CENTER;
                return;
            }
            if (Util.EqualsIgnoreCase(alignment, ElementTags.ALIGN_RIGHT)) {
                this.HorizontalAlignment = Element.ALIGN_RIGHT;
                return;
            }
            if (Util.EqualsIgnoreCase(alignment, ElementTags.ALIGN_JUSTIFIED)) {
                this.HorizontalAlignment = Element.ALIGN_JUSTIFIED;
                return;
            }
            if (Util.EqualsIgnoreCase(alignment, ElementTags.ALIGN_JUSTIFIED_ALL)) {
                this.HorizontalAlignment = Element.ALIGN_JUSTIFIED_ALL;
                return;
            }
            this.HorizontalAlignment = Element.ALIGN_LEFT;
        }

        /// <summary>
        /// Sets the alignment of this paragraph.
        /// </summary>
        /// <param name="alignment">the new alignment as a string</param>
        public void SetVerticalAlignment(string alignment) {
            if (Util.EqualsIgnoreCase(alignment, ElementTags.ALIGN_MIDDLE)) {
                this.VerticalAlignment = Element.ALIGN_MIDDLE;
                return;
            }
            if (Util.EqualsIgnoreCase(alignment, ElementTags.ALIGN_BOTTOM)) {
                this.VerticalAlignment = Element.ALIGN_BOTTOM;
                return;
            }
            if (Util.EqualsIgnoreCase(alignment, ElementTags.ALIGN_BASELINE)) {
                this.VerticalAlignment = Element.ALIGN_BASELINE;
                return;
            }
            this.VerticalAlignment = Element.ALIGN_TOP;
        }

        /// <summary>
        /// Sets the width.
        /// </summary>
        /// <value>the new value</value>
        public override float Width {
            set {
                width = value;
            }
            get {
                return width;
            }
        }

        /**
        * Sets the width.
        * It can be an absolute value "100" or a percentage "20%"
        *
        * @param   value   the new value
        */
        public void SetWidth(String value) {
            if (value.EndsWith("%")) {
                value = value.Substring(0, value.Length - 1);
                percentage = true;
            }
            width = int.Parse(value);
        }
        
        /**
        * Gets the width as a String.
        *
        * @return  a value
        */
        public String GetWidthAsString() {
            String w = width.ToString(System.Globalization.CultureInfo.InvariantCulture);
            if (w.EndsWith(".0")) w = w.Substring(0, w.Length - 2);
            if (percentage) w += "%";
            return w;
        }

        // methods to retrieve information

        /// <summary>
        /// Gets the number of Elements in the Cell.
        /// </summary>
        /// <value>a size</value>
        public int Size {
            get {
                return arrayList.Count;
            }
        }

        /// <summary>
        /// Checks if the Cell is empty.
        /// </summary>
        /// <returns>false if there are non-empty Elements in the Cell.</returns>
        public bool IsEmpty() {
            switch (this.Size) {
                case 0:
                    return true;
                case 1:
                    IElement element = (IElement)arrayList[0];
                switch (element.Type) {
                    case Element.CHUNK:
                        return ((Chunk) element).IsEmpty();
                    case Element.ANCHOR:
                    case Element.PHRASE:
                    case Element.PARAGRAPH:
                        return ((Phrase) element).IsEmpty();
                    case Element.LIST:
                        return ((List) element).IsEmpty();                        
                }
                    return false;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Makes sure there is at least 1 object in the Cell.
        /// Otherwise it might not be shown in the table.
        /// </summary>
        internal void Fill() {
            if (this.Size == 0) arrayList.Add(new Paragraph(0));
        }

        /// <summary>
        /// Checks if the Cell is empty.
        /// </summary>
        /// <returns>false if there are non-empty Elements in the Cell.</returns>
        public bool IsTable() {
            return (this.Size == 1) && (((IElement)arrayList[0]).Type == Element.TABLE);
        }

        /// <summary>
        /// Gets Elements.
        /// </summary>
        /// <value>an ArrayList</value>
        public ArrayList Elements {
            get {
                return arrayList;
            }
        }

        /// <summary>
        /// Gets/Sets the horizontal Element.
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

        /// <summary>
        /// Gets/sets the vertical Element.
        /// </summary>
        /// <value>a value</value>
        public int VerticalAlignment {
            get {
                return verticalAlignment;
            }

            set {
                verticalAlignment = value;
            }
        }

        /**
         * Gets the colspan.
         *
         * @return    a value
         */
        /// <summary>
        /// Gets/sets the colspan.
        /// </summary>
        /// <value>a value</value>
        public int Colspan {
            get {
                return colspan;
            }

            set {
                colspan = value;
            }
        }

        /// <summary>
        /// Gets/sets the rowspan.
        /// </summary>
        /// <value>a value</value>
        public int Rowspan {
            get {
                return rowspan;
            }

            set {
                rowspan = value;
            }
        }

        /// <summary>
        /// Gets/sets the leading.
        /// </summary>
        /// <value>a value</value>
        public float Leading {
            get {
                if (float.IsNaN(leading)) {
                    return 16;
                }
                return leading;
            }

            set {
                leading = value;
            }
        }

        /// <summary>
        /// Gets/sets header
        /// </summary>
        /// <value>a value</value>
        public bool Header {
            get {
                return header;
            }

            set {
                header = value;
            }
        }

        /**
         * Get nowrap.
         *
         * @return    a value
         */
        /// <summary>
        /// Get/set nowrap.
        /// </summary>
        /// <value>a value</value>
        public bool NoWrap {
            get {
                return (maxLines == 1);
            }

            set {
                maxLines = 1;
            }
        }

        /// <summary>
        /// Clears all the Elements of this Cell.
        /// </summary>
        public void Clear() {
            arrayList.Clear();
        }

        /// <summary>
        /// This property throws an Exception.
        /// </summary>
        /// <value>none</value>
        public override float Top {
            get {
                throw new Exception("Dimensions of a Cell can't be calculated. See the FAQ.");
            }

            set {
                throw new Exception("Dimensions of a Cell can't be calculated. See the FAQ.");
            }
        }

        /// <summary>
        /// This property throws an Exception.
        /// </summary>
        /// <value>none</value>
        public override float Bottom {
            get {
                throw new Exception("Dimensions of a Cell can't be calculated. See the FAQ.");
            }

            set {
                throw new Exception("Dimensions of a Cell can't be calculated. See the FAQ.");
            }
        }

        /// <summary>
        /// This property throws an Exception.
        /// </summary>
        /// <value>none</value>
        public override float Left {
            get {
                throw new Exception("Dimensions of a Cell can't be calculated. See the FAQ.");
            }

            set {
                throw new Exception("Dimensions of a Cell can't be calculated. See the FAQ.");
            }
        }

        /// <summary>
        /// This property throws an Exception.
        /// </summary>
        /// <value>none</value>
        public override float Right {
            get {
                throw new Exception("Dimensions of a Cell can't be calculated. See the FAQ.");
            }

            set {
                throw new Exception("Dimensions of a Cell can't be calculated. See the FAQ.");
            }
        }

        /// <summary>
        /// This method throws an Exception.
        /// </summary>
        /// <param name="margin">new value</param>
        /// <returns>none</returns>
        public float GetTop(int margin) {
            throw new Exception("Dimensions of a Cell can't be calculated. See the FAQ.");
        }

        /// <summary>
        /// This method throws an Exception.
        /// </summary>
        /// <param name="margin">new value</param>
        /// <returns>none</returns>
        public float GetBottom(int margin) {
            throw new Exception("Dimensions of a Cell can't be calculated. See the FAQ.");
        }

        /// <summary>
        /// This method throws an Exception.
        /// </summary>
        /// <param name="margin">new value</param>
        /// <returns>none</returns>
        public float GetLeft(int margin) {
            throw new Exception("Dimensions of a Cell can't be calculated. See the FAQ.");
        }

        /// <summary>
        /// This method throws an Exception.
        /// </summary>
        /// <param name="margin">new value</param>
        /// <returns>none</returns>
        public float GetRight(int margin) {
            throw new Exception("Dimensions of a Cell can't be calculated. See the FAQ.");
        }

        /// <summary>
        /// Checks if a given tag corresponds with this object.
        /// </summary>
        /// <param name="tag">the given tag</param>
        /// <returns>true if the tag corresponds</returns>
        public static bool IsTag(string tag) {
            return ElementTags.CELL.Equals(tag);
        }

        
        ///<summary>Does this <CODE>Cell</CODE> force a group change? </summary>
        protected bool groupChange = true;

        /// <summary>
        /// Does this <CODE>Cell</CODE> force a group change?
        /// </summary>
        public bool GroupChange {
            get {
                return groupChange;
            }

            set {
                groupChange = value;
            }
        }

        /// <summary>
        /// get/set maxLines value
        /// </summary>
        public int MaxLines {
            get {
                return maxLines;
            }

            set {
                maxLines = value;
            }
        }

        /// <summary>
        /// Maximum number of lines allowed in the cell.  
        /// The default value of this property is not to limit the maximum number of lines
        /// (contributed by dperezcar@fcc.es)
        /// </summary>
        protected int maxLines = int.MaxValue;

        /// <summary>
        /// get/set showTruncation value
        /// </summary>
        public string ShowTruncation {
            get {
                return showTruncation;
            }

            set {
                showTruncation = value;
            }
        }

        /// <summary>
        /// If a truncation happens due to the {@link #maxLines} property, then this text will 
        /// be added to indicate a truncation has happened.
        /// Default value is null, and means avoiding marking the truncation.  
        /// A useful value of this property could be e.g. "..."
        /// (contributed by dperezcar@fcc.es)
        /// </summary>
        private string showTruncation;

        /// <summary>
        /// get/set useAscender value
        /// </summary>
        public bool UseAscender {
            get {
                return useAscender;
            }

            set {
                useAscender = value;
            }
        }

        /// <summary>
        /// get/set useDescender value
        /// </summary>
        public bool UseDescender {
            get {
                return useDescender;
            }

            set {
                useDescender = value;
            }
        }

        /// <summary>
        /// get/set useBorderPadding value
        /// </summary>
        public bool UseBorderPadding {
            get {
                return useBorderPadding;
            }

            set {
                useBorderPadding = value;
            }
        }
        /**
        * Creates a PdfPCell based on this Cell object.
        * @return a PdfPCell
        * @throws BadElementException
        */
        public PdfPCell CreatePdfPCell() {
            if (rowspan > 1) throw new BadElementException("PdfPCells can't have a rowspan > 1");
            if (IsTable()) return new PdfPCell(((Table)arrayList[0]).CreatePdfPTable());
            PdfPCell cell = new PdfPCell();
            cell.VerticalAlignment = verticalAlignment;
            cell.HorizontalAlignment = horizontalAlignment;
            cell.Colspan = colspan;
            cell.UseBorderPadding = useBorderPadding;
            cell.UseDescender = useDescender;
            cell.SetLeading(Leading, 0);
            cell.CloneNonPositionParameters(this);
            cell.NoWrap = noWrap;
            foreach (IElement i in Elements) {
                if (i.Type == Element.PHRASE || i.Type == Element.PARAGRAPH) {
                    Paragraph p = new Paragraph((Phrase)i);
                    p.Alignment = horizontalAlignment;
                    cell.AddElement(p);
                }
                else
                    cell.AddElement(i);
            }
            return cell;
        }
    }
}
