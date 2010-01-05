using System;
using System.IO;
using System.Collections;
using System.util;
using iTextSharp.text;
using iTextSharp.text.rtf;
using iTextSharp.text.rtf.document;
using iTextSharp.text.rtf.style;
using iTextSharp.text.rtf.text;
/*
 * $Id: RtfCell.cs,v 1.14 2008/05/16 19:31:18 psoares33 Exp $
 * 
 *
 * Copyright 2001, 2002, 2003, 2004 by Mark Hall
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
 * LGPL license (the ?GNU LIBRARY GENERAL PUBLIC LICENSE?), in which case the
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

namespace iTextSharp.text.rtf.table {

    /**
    * The RtfCell wraps a Cell, but can also be added directly to a Table.
    * The RtfCell is an extension of Cell, that supports a multitude of different
    * borderstyles.
    * 
    * @version $Id: RtfCell.cs,v 1.14 2008/05/16 19:31:18 psoares33 Exp $
    * @author Mark Hall (Mark.Hall@mail.room3b.eu)
    * @author Steffen Stundzig
    * @author Benoit WIART <b.wiart@proxiad.com>
    * @see com.lowagie.text.rtf.table.RtfBorder
    */
    public class RtfCell : Cell, IRtfExtendedElement {

        /**
        * This cell is not merged
        */
        private const int MERGE_NONE = 0;
        /**
        * This cell is the parent cell of a vertical merge operation
        */
        private const int MERGE_VERT_PARENT = 1;
        /**
        * This cell is a child cell of a vertical merge operation
        */
        private const int MERGE_VERT_CHILD = 2;

        /**
        * The parent RtfRow of this RtfCell
        */
        private RtfRow parentRow = null;
        /**
        * The content of this RtfCell
        */
        private ArrayList content = null;
        /**
        * The right margin of this RtfCell
        */
        private int cellRight = 0;
        /**
        * The width of this RtfCell
        */
        private int cellWidth = 0;
        /**
        * The borders of this RtfCell
        */
        private RtfBorderGroup borders = null;
        /**
        * The background color of this RtfCell
        */
        private new RtfColor backgroundColor = null;
        /**
        * The padding of this RtfCell
        */
        private int cellPadding = 0;
        /**
        * The merge type of this RtfCell
        */
        private int mergeType = MERGE_NONE;
        /**
        * The RtfDocument this RtfCell belongs to
        */
        private RtfDocument document = null;
        /**
        * Whether this RtfCell is in a header
        */
        private bool inHeader = false;
        /**
        * Whether this RtfCell is a placeholder for a removed table cell.
        */
        private bool deleted = false;

        /**
        * Constructs an empty RtfCell
        */
        public RtfCell() : base() {
            this.borders = new RtfBorderGroup();
            verticalAlignment = Element.ALIGN_MIDDLE;
        }
        
        /**
        * Constructs a RtfCell based upon a String
        * 
        * @param content The String to base the RtfCell on
        */
        public RtfCell(String content) : base(content) {
            this.borders = new RtfBorderGroup();
            verticalAlignment = Element.ALIGN_MIDDLE;
        }
        
        /**
        * Constructs a RtfCell based upon an Element
        * 
        * @param element The Element to base the RtfCell on
        * @throws BadElementException If the Element is not valid
        */
        public RtfCell(IElement element) : base(element) {
            this.borders = new RtfBorderGroup();
            verticalAlignment = Element.ALIGN_MIDDLE;
        }
        
        /**
        * Constructs a deleted RtfCell.
        * 
        * @param deleted Whether this RtfCell is actually deleted.
        */
        protected internal RtfCell(bool deleted) : base() {
            this.deleted = deleted;
            verticalAlignment = Element.ALIGN_MIDDLE;
        }
        
        /**
        * Constructs a RtfCell based on a Cell.
        * 
        * @param doc The RtfDocument this RtfCell belongs to
        * @param row The RtfRow this RtfCell lies in
        * @param cell The Cell to base this RtfCell on
        */
        protected internal RtfCell(RtfDocument doc, RtfRow row, Cell cell) {
            this.document = doc;
            this.parentRow = row;
            ImportCell(cell);
        }

        /**
        * Imports the Cell properties into the RtfCell
        * 
        * @param cell The Cell to import
        */
        private void ImportCell(Cell cell) {
            this.content = new ArrayList();
            
            if (cell == null) {
                this.borders = new RtfBorderGroup(this.document, RtfBorder.CELL_BORDER, this.parentRow.GetParentTable().GetBorders());
                return;
            }
            
            this.colspan = cell.Colspan;
            this.rowspan = cell.Rowspan;
            if (cell.Rowspan > 1) {
                this.mergeType = MERGE_VERT_PARENT;
            }
            if (cell is RtfCell) {
                this.borders = new RtfBorderGroup(this.document, RtfBorder.CELL_BORDER, ((RtfCell) cell).GetBorders());
            } else {
                this.borders = new RtfBorderGroup(this.document, RtfBorder.CELL_BORDER, cell.Border, cell.BorderWidth, cell.BorderColor);
            }
            this.verticalAlignment = cell.VerticalAlignment;
            if (cell.BackgroundColor == null) {
                this.backgroundColor = new RtfColor(this.document, 255, 255, 255);
            } else {
                this.backgroundColor = new RtfColor(this.document, cell.BackgroundColor);
            }
            
            this.cellPadding = (int) this.parentRow.GetParentTable().GetCellPadding();
            
            Paragraph container = null;
            foreach (IElement element in cell.Elements) {
                try {
                    // should we wrap it in a paragraph
                    if (!(element is Paragraph) && !(element is List)) {
                        if (container != null) {
                            container.Add(element);
                        } else {
                            container = new Paragraph();
                            container.Alignment = cell.HorizontalAlignment;
                            container.Add(element);
                        }
                    } else {
                        if (container != null) {
                            IRtfBasicElement[] rtfElements = this.document.GetMapper().MapElement(container);
                            for(int i = 0; i < rtfElements.Length; i++) {
                                rtfElements[i].SetInTable(true);
                                this.content.Add(rtfElements[i]);
                            }
                            container = null;
                        }
                        // if horizontal alignment is undefined overwrite
                        // with that of enclosing cell
                        if (element is Paragraph && ((Paragraph) element).Alignment == Element.ALIGN_UNDEFINED) {
                            ((Paragraph) element).Alignment = cell.HorizontalAlignment;
                        }
                        
                        IRtfBasicElement[] rtfElements2 = this.document.GetMapper().MapElement(element);
                        for(int i = 0; i < rtfElements2.Length; i++) {
                            rtfElements2[i].SetInTable(true);
                            this.content.Add(rtfElements2[i]);
                        }
                    }
                } catch (DocumentException) {
                }
            }
            if (container != null) {
                try {
                    IRtfBasicElement[] rtfElements = this.document.GetMapper().MapElement(container);
                    for(int i = 0; i < rtfElements.Length; i++) {
                        rtfElements[i].SetInTable(true);
                        this.content.Add(rtfElements[i]);
                    }
                } catch (DocumentException) {
                }
            }
        }
        
        /**
        * Write the cell definition part of this RtfCell
        * 
        * @return A byte array with the cell definition
        */
        public virtual void WriteDefinition(Stream result) {
            byte[] t;
            if (this.mergeType == MERGE_VERT_PARENT) {
                result.Write(t = DocWriter.GetISOBytes("\\clvmgf"), 0, t.Length);
            } else if (this.mergeType == MERGE_VERT_CHILD) {
                result.Write(t = DocWriter.GetISOBytes("\\clvmrg"), 0, t.Length);
            }
            switch (verticalAlignment) {
                case Element.ALIGN_BOTTOM:
                    result.Write(t = DocWriter.GetISOBytes("\\clvertalb"), 0, t.Length);
                    break;
                case Element.ALIGN_CENTER:
                case Element.ALIGN_MIDDLE:
                    result.Write(t = DocWriter.GetISOBytes("\\clvertalc"), 0, t.Length);
                    break;
                case Element.ALIGN_TOP:
                    result.Write(t = DocWriter.GetISOBytes("\\clvertalt"), 0, t.Length);
                    break;
            }
            this.borders.WriteContent(result);

            if (this.backgroundColor != null) {
                result.Write(t = DocWriter.GetISOBytes("\\clcbpat"), 0, t.Length);
                result.Write(t = IntToByteArray(this.backgroundColor.GetColorNumber()), 0, t.Length);
            }
            result.WriteByte((byte)'\n');
            
            result.Write(t = DocWriter.GetISOBytes("\\clftsWidth3"), 0, t.Length);
            result.WriteByte((byte)'\n');
            
            result.Write(t = DocWriter.GetISOBytes("\\clwWidth"), 0, t.Length);
            result.Write(t = IntToByteArray(this.cellWidth), 0, t.Length);
            result.WriteByte((byte)'\n');
            
            if (this.cellPadding > 0) {
                result.Write(t = DocWriter.GetISOBytes("\\clpadl"), 0, t.Length);
                result.Write(t = IntToByteArray(this.cellPadding / 2), 0, t.Length);
                result.Write(t = DocWriter.GetISOBytes("\\clpadt"), 0, t.Length);
                result.Write(t = IntToByteArray(this.cellPadding / 2), 0, t.Length);
                result.Write(t = DocWriter.GetISOBytes("\\clpadr"), 0, t.Length);
                result.Write(t = IntToByteArray(this.cellPadding / 2), 0, t.Length);
                result.Write(t = DocWriter.GetISOBytes("\\clpadb"), 0, t.Length);
                result.Write(t = IntToByteArray(this.cellPadding / 2), 0, t.Length);
                result.Write(t = DocWriter.GetISOBytes("\\clpadfl3"), 0, t.Length);
                result.Write(t = DocWriter.GetISOBytes("\\clpadft3"), 0, t.Length);
                result.Write(t = DocWriter.GetISOBytes("\\clpadfr3"), 0, t.Length);
                result.Write(t = DocWriter.GetISOBytes("\\clpadfb3"), 0, t.Length);
            }
            result.Write(t = DocWriter.GetISOBytes("\\cellx"), 0, t.Length);
            result.Write(t = IntToByteArray(this.cellRight), 0, t.Length);
        }
        
        /**
        * Write the content of this RtfCell
        */    
        public virtual void WriteContent(Stream result) {
            byte[] t;
            if (this.content.Count == 0) {
                result.Write(RtfParagraph.PARAGRAPH_DEFAULTS, 0, RtfParagraph.PARAGRAPH_DEFAULTS.Length);
                if (this.parentRow.GetParentTable().GetTableFitToPage()) {
                    result.Write(RtfParagraphStyle.KEEP_TOGETHER_WITH_NEXT, 0, RtfParagraphStyle.KEEP_TOGETHER_WITH_NEXT.Length);
                }
                result.Write(RtfParagraph.IN_TABLE, 0, RtfParagraph.IN_TABLE.Length);
            } else {
                for (int i = 0; i < this.content.Count; i++) {
                    IRtfBasicElement rtfElement = (IRtfBasicElement) this.content[i];
                    if (rtfElement is RtfParagraph) {
                        ((RtfParagraph) rtfElement).SetKeepTogetherWithNext(this.parentRow.GetParentTable().GetTableFitToPage());
                    }
                    rtfElement.WriteContent(result);
                    if (rtfElement is RtfParagraph && i < (this.content.Count - 1)) {
                        result.Write(RtfParagraph.PARAGRAPH, 0, RtfParagraph.PARAGRAPH.Length);
                    }
                }
            }
            result.Write(t = DocWriter.GetISOBytes("\\cell"), 0, t.Length);
        }

        /**
        * Sets the right margin of this cell. Used in merge operations
        * 
        * @param cellRight The right margin to use
        */
        protected internal void SetCellRight(int cellRight) {
            this.cellRight = cellRight;
        }
        
        /**
        * Gets the right margin of this RtfCell
        * 
        * @return The right margin of this RtfCell.
        */
        protected internal int GetCellRight() {
            return this.cellRight;
        }
        
        /**
        * Sets the cell width of this RtfCell. Used in merge operations.
        * 
        * @param cellWidth The cell width to use
        */
        protected internal void SetCellWidth(int cellWidth) {
            this.cellWidth = cellWidth;
        }
        
        /**
        * Gets the cell width of this RtfCell
        * 
        * @return The cell width of this RtfCell
        */
        protected internal int GetCellWidth() {
            return this.cellWidth;
        }
        
        /**
        * Gets the cell padding of this RtfCell
        * 
        * @return The cell padding of this RtfCell
        */
        protected internal int GetCellpadding() {
            return this.cellPadding;
        }

        /**
        * Gets the borders of this RtfCell
        * 
        * @return The borders of this RtfCell
        */
        protected internal RtfBorderGroup GetBorders() {
            return this.borders;
        }
        
        /**
        * Set the borders of this RtfCell
        * 
        * @param borderGroup The RtfBorderGroup to use as borders
        */
        public void SetBorders(RtfBorderGroup borderGroup) {
            this.borders = new RtfBorderGroup(this.document, RtfBorder.CELL_BORDER, borderGroup);
        }
        
        /**
        * Get the background color of this RtfCell
        * 
        * @return The background color of this RtfCell
        */
        protected internal RtfColor GetRtfBackgroundColor() {
            return this.backgroundColor;
        }

        /**
        * Merge this cell into the parent cell.
        * 
        * @param mergeParent The RtfCell to merge with
        */
        protected internal void SetCellMergeChild(RtfCell mergeParent) {
            this.mergeType = MERGE_VERT_CHILD;
            this.cellWidth = mergeParent.GetCellWidth();
            this.cellRight = mergeParent.GetCellRight();
            this.cellPadding = mergeParent.GetCellpadding();
            this.borders = mergeParent.GetBorders();
            this.verticalAlignment = mergeParent.VerticalAlignment;
            this.backgroundColor = mergeParent.GetRtfBackgroundColor();
        }

        /**
        * Sets the RtfDocument this RtfCell belongs to
        * 
        * @param doc The RtfDocument to use
        */
        public void SetRtfDocument(RtfDocument doc) {
            this.document = doc;
        }
        
        /**
        * Unused
        * @param inTable
        */
        public void SetInTable(bool inTable) {
        }
        
        /**
        * Sets whether this RtfCell is in a header
        * 
        * @param inHeader <code>True</code> if this RtfCell is in a header, <code>false</code> otherwise
        */
        public void SetInHeader(bool inHeader) {
            this.inHeader = inHeader;
            for (int i = 0; i < this.content.Count; i++) {
                ((IRtfBasicElement) this.content[i]).SetInHeader(inHeader);
            }
        }

        /**
        * Gets whether this <code>RtfCell</code> is in a header
        * 
        * @return <code>True</code> if this <code>RtfCell</code> is in a header, <code>false</code> otherwise
        */
        public bool IsInHeader() {
            return this.inHeader;
        }

        /**
        * Transforms an integer into its String representation and then returns the bytes
        * of that string.
        *
        * @param i The integer to convert
        * @return A byte array representing the integer
        */
        private byte[] IntToByteArray(int i) {
            return DocWriter.GetISOBytes(i.ToString());
        }

        /**
        * Checks whether this RtfCell is a placeholder for
        * a table cell that has been removed due to col/row spanning.
        * 
        * @return <code>True</code> if this RtfCell is deleted, <code>false</code> otherwise.
        */
        public bool IsDeleted() {
            return this.deleted;
        }
    }
}