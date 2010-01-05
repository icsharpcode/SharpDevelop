using System;
using System.IO;
using System.Collections;
using iTextSharp.text;
using iTextSharp.text.rtf;
using iTextSharp.text.rtf.document;
/*
 * $Id: RtfRow.cs,v 1.10 2008/05/16 19:31:19 psoares33 Exp $
 * 
 *
 * Copyright 2001, 2002, 2003, 2004, 2005 by Mark Hall
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
    * The RtfRow wraps one Row for a RtfTable.
    * INTERNAL USE ONLY
    * 
    * @version $Version:$
    * @author Mark Hall (Mark.Hall@mail.room3b.eu)
    * @author Steffen Stundzig
    * @author Lorenz Maierhofer <larry@sbox.tugraz.at>
    */
    public class RtfRow : RtfElement {

        /**
        * Constant for the RtfRow beginning
        */
        private static byte[] ROW_BEGIN = DocWriter.GetISOBytes("\\trowd");
        /**
        * Constant for the RtfRow width style
        */
        private static byte[] ROW_WIDTH_STYLE = DocWriter.GetISOBytes("\\trftsWidth3");
        /**
        * Constant for the RtfRow width
        */
        private static byte[] ROW_WIDTH = DocWriter.GetISOBytes("\\trwWidth");
        /**
        * Constant to specify that this RtfRow are not to be broken across pages
        */
        private static byte[] ROW_KEEP_TOGETHER = DocWriter.GetISOBytes("\\trkeep");
        /**
        * Constant to specify that this is a header RtfRow
        */
        private static byte[] ROW_HEADER_ROW = DocWriter.GetISOBytes("\\trhdr");
        /**
        * Constant for left alignment of this RtfRow
        */
        private static byte[] ROW_ALIGN_LEFT = DocWriter.GetISOBytes("\\trql");
        /**
        * Constant for right alignment of this RtfRow
        */
        private static byte[] ROW_ALIGN_RIGHT = DocWriter.GetISOBytes("\\trqr");
        /**
        * Constant for center alignment of this RtfRow
        */
        private static byte[] ROW_ALIGN_CENTER = DocWriter.GetISOBytes("\\trqc");
        /**
        * Constant for justified alignment of this RtfRow
        */
        private static byte[] ROW_ALIGN_JUSTIFIED = DocWriter.GetISOBytes("\\trqj");
        /**
        * Constant for the graph style of this RtfRow
        */
        private static byte[] ROW_GRAPH = DocWriter.GetISOBytes("\\trgaph10");
        /**
        * Constant for the cell left spacing
        */
        private static byte[] ROW_CELL_SPACING_LEFT = DocWriter.GetISOBytes("\\trspdl");
        /**
        * Constant for the cell top spacing
        */
        private static byte[] ROW_CELL_SPACING_TOP = DocWriter.GetISOBytes("\\trspdt");
        /**
        * Constant for the cell right spacing
        */
        private static byte[] ROW_CELL_SPACING_RIGHT = DocWriter.GetISOBytes("\\trspdr");
        /**
        * Constant for the cell bottom spacing
        */
        private static byte[] ROW_CELL_SPACING_BOTTOM = DocWriter.GetISOBytes("\\trspdb");
        /**
        * Constant for the cell left spacing style
        */
        private static byte[] ROW_CELL_SPACING_LEFT_STYLE = DocWriter.GetISOBytes("\\trspdfl3");
        /**
        * Constant for the cell top spacing style
        */
        private static byte[] ROW_CELL_SPACING_TOP_STYLE = DocWriter.GetISOBytes("\\trspdft3");
        /**
        * Constant for the cell right spacing style
        */
        private static byte[] ROW_CELL_SPACING_RIGHT_STYLE = DocWriter.GetISOBytes("\\trspdfr3");
        /**
        * Constant for the cell bottom spacing style
        */
        private static byte[] ROW_CELL_SPACING_BOTTOM_STYLE = DocWriter.GetISOBytes("\\trspdfb3");
        /**
        * Constant for the cell left padding
        */
        private static byte[] ROW_CELL_PADDING_LEFT = DocWriter.GetISOBytes("\\trpaddl");
        /**
        * Constant for the cell right padding
        */
        private static byte[] ROW_CELL_PADDING_RIGHT = DocWriter.GetISOBytes("\\trpaddr");
        /**
        * Constant for the cell left padding style
        */
        private static byte[] ROW_CELL_PADDING_LEFT_STYLE = DocWriter.GetISOBytes("\\trpaddfl3");
        /**
        * Constant for the cell right padding style
        */
        private static byte[] ROW_CELL_PADDING_RIGHT_STYLE = DocWriter.GetISOBytes("\\trpaddfr3");
        /**
        * Constant for the end of a row
        */
        private static byte[] ROW_END = DocWriter.GetISOBytes("\\row");

        /**
        * The RtfTable this RtfRow belongs to
        */
        private RtfTable parentTable = null;
        /**
        * The cells of this RtfRow
        */
        private ArrayList cells = null;
        /**
        * The width of this row
        */
        private int width = 0;
        /**
        * The row number
        */
        private int rowNumber = 0;
        
        /**
        * Constructs a RtfRow for a Row.
        * 
        * @param doc The RtfDocument this RtfRow belongs to
        * @param rtfTable The RtfTable this RtfRow belongs to
        * @param row The Row this RtfRow is based on
        * @param rowNumber The number of this row
        */
        protected internal RtfRow(RtfDocument doc, RtfTable rtfTable, Row row, int rowNumber) : base(doc) {
            this.parentTable = rtfTable;
            this.rowNumber = rowNumber;
            ImportRow(row);
        }
        
        /**
        * Imports a Row and copies all settings
        * 
        * @param row The Row to import
        */
        private void ImportRow(Row row) {
            this.cells = new ArrayList();
            this.width = this.document.GetDocumentHeader().GetPageSetting().GetPageWidth() - this.document.GetDocumentHeader().GetPageSetting().GetMarginLeft() - this.document.GetDocumentHeader().GetPageSetting().GetMarginRight();
            this.width = (int) (this.width * this.parentTable.GetTableWidthPercent() / 100);
            
            int cellRight = 0;
            int cellWidth = 0;
            for (int i = 0; i < row.Columns; i++) {
                cellWidth = (int) (this.width * this.parentTable.GetProportionalWidths()[i] / 100);
                cellRight = cellRight + cellWidth;
                
                Cell cell = (Cell) row.GetCell(i);
                RtfCell rtfCell = new RtfCell(this.document, this, cell);
                rtfCell.SetCellRight(cellRight);
                rtfCell.SetCellWidth(cellWidth);
                this.cells.Add(rtfCell);
            }
        }
        
        /**
        * Performs a second pass over all cells to handle cell row/column spanning.
        */
        protected internal void HandleCellSpanning() {
            RtfCell deletedCell = new RtfCell(true);
            for (int i = 0; i < this.cells.Count; i++) {
                RtfCell rtfCell = (RtfCell) this.cells[i];
                if (rtfCell.Colspan > 1) {
                    int cSpan = rtfCell.Colspan;
                    for (int j = i + 1; j < i + cSpan; j++) {
                        if (j < this.cells.Count) {
                            RtfCell rtfCellMerge = (RtfCell) this.cells[j];
                            rtfCell.SetCellRight(rtfCell.GetCellRight() + rtfCellMerge.GetCellWidth());
                            rtfCell.SetCellWidth(rtfCell.GetCellWidth() + rtfCellMerge.GetCellWidth());
                            this.cells[j] = deletedCell;
                        }
                    }
                }
                if (rtfCell.Rowspan > 1) {
                    ArrayList rows = this.parentTable.GetRows();
                    for (int j = 1; j < rtfCell.Rowspan; j++) {
                        RtfRow mergeRow = (RtfRow) rows[this.rowNumber + j];
                        if (this.rowNumber + j < rows.Count) {
                            RtfCell rtfCellMerge = (RtfCell) mergeRow.GetCells()[i];
                            rtfCellMerge.SetCellMergeChild(rtfCell);
                        }
                        if (rtfCell.Colspan > 1) {
                            int cSpan = rtfCell.Colspan;
                            for (int k = i + 1; k < i + cSpan; k++) {
                                if (k < mergeRow.GetCells().Count) {
                                    mergeRow.GetCells()[k] = deletedCell;
                                }
                            }
                        }
                    }
                }
            }
        }

        /**
        * Cleans the deleted RtfCells from the total RtfCells.
        */
        protected internal void CleanRow() {
            int i = 0;
            while (i < this.cells.Count) {
                if (((RtfCell) this.cells[i]).IsDeleted()) {
                    this.cells.RemoveAt(i);
                } else {
                    i++;
                }
            }
        }
        
        /**
        * Writes the row definition/settings.
        *
        * @param result The <code>OutputStream</code> to write the definitions to.
        */
        private void WriteRowDefinition(Stream result) {
            byte[] t;
            result.Write(ROW_BEGIN, 0, ROW_BEGIN.Length);
            result.WriteByte((byte)'\n');
            result.Write(ROW_WIDTH_STYLE, 0, ROW_WIDTH_STYLE.Length);
            result.Write(ROW_WIDTH, 0, ROW_WIDTH.Length);
            result.Write(t = IntToByteArray(this.width), 0, t.Length);
            if (this.parentTable.GetCellsFitToPage()) {
                result.Write(ROW_KEEP_TOGETHER, 0, ROW_KEEP_TOGETHER.Length);
            }
            if (this.rowNumber <= this.parentTable.GetHeaderRows()) {
                result.Write(ROW_HEADER_ROW, 0, ROW_HEADER_ROW.Length);
            }
            switch (this.parentTable.GetAlignment()) {
                case Element.ALIGN_LEFT:
                    result.Write(ROW_ALIGN_LEFT, 0, ROW_ALIGN_LEFT.Length);
                    break;
                case Element.ALIGN_RIGHT:
                    result.Write(ROW_ALIGN_RIGHT, 0, ROW_ALIGN_RIGHT.Length);
                    break;
                case Element.ALIGN_CENTER:
                    result.Write(ROW_ALIGN_CENTER, 0, ROW_ALIGN_CENTER.Length);
                    break;
                case Element.ALIGN_JUSTIFIED:
                case Element.ALIGN_JUSTIFIED_ALL:
                    result.Write(ROW_ALIGN_JUSTIFIED, 0, ROW_ALIGN_JUSTIFIED.Length);
                    break;
            }
            result.Write(ROW_GRAPH, 0, ROW_GRAPH.Length);
            
            this.parentTable.GetBorders().WriteContent(result);
            
            if (this.parentTable.GetCellSpacing() > 0) {
                result.Write(ROW_CELL_SPACING_LEFT, 0, ROW_CELL_SPACING_LEFT.Length);
                result.Write(t = IntToByteArray((int) (this.parentTable.GetCellSpacing() / 2)), 0, t.Length);
                result.Write(ROW_CELL_SPACING_LEFT_STYLE, 0, ROW_CELL_SPACING_LEFT_STYLE.Length);
                result.Write(ROW_CELL_SPACING_TOP, 0, ROW_CELL_SPACING_TOP.Length);
                result.Write(t = IntToByteArray((int) (this.parentTable.GetCellSpacing() / 2)), 0, t.Length);
                result.Write(ROW_CELL_SPACING_TOP_STYLE, 0, ROW_CELL_SPACING_TOP_STYLE.Length);
                result.Write(ROW_CELL_SPACING_RIGHT, 0, ROW_CELL_SPACING_RIGHT.Length);
                result.Write(t = IntToByteArray((int) (this.parentTable.GetCellSpacing() / 2)), 0, t.Length);
                result.Write(ROW_CELL_SPACING_RIGHT_STYLE, 0, ROW_CELL_SPACING_RIGHT_STYLE.Length);
                result.Write(ROW_CELL_SPACING_BOTTOM, 0, ROW_CELL_SPACING_BOTTOM.Length);
                result.Write(t = IntToByteArray((int) (this.parentTable.GetCellSpacing() / 2)), 0, t.Length);
                result.Write(ROW_CELL_SPACING_BOTTOM_STYLE, 0, ROW_CELL_SPACING_BOTTOM_STYLE.Length);
            }
            
            result.Write(ROW_CELL_PADDING_LEFT, 0, ROW_CELL_PADDING_LEFT.Length);
            result.Write(t = IntToByteArray((int) (this.parentTable.GetCellPadding() / 2)), 0, t.Length);
            result.Write(ROW_CELL_PADDING_RIGHT, 0, ROW_CELL_PADDING_RIGHT.Length);
            result.Write(t = IntToByteArray((int) (this.parentTable.GetCellPadding() / 2)), 0, t.Length);
            result.Write(ROW_CELL_PADDING_LEFT_STYLE, 0, ROW_CELL_PADDING_LEFT_STYLE.Length);
            result.Write(ROW_CELL_PADDING_RIGHT_STYLE, 0, ROW_CELL_PADDING_RIGHT_STYLE.Length);
            
            result.WriteByte((byte)'\n');
            
            for (int i = 0; i < this.cells.Count; i++) {
                RtfCell rtfCell = (RtfCell) this.cells[i];
                rtfCell.WriteDefinition(result);
            }
        }
        
        /**
        * Writes the content of this RtfRow
        */    
        public override void WriteContent(Stream result) {
            WriteRowDefinition(result);
            
            for (int i = 0; i < this.cells.Count; i++) {
                RtfCell rtfCell = (RtfCell) this.cells[i];
                rtfCell.WriteContent(result);
            }

            result.Write(DELIMITER, 0, DELIMITER.Length);

            if (this.document.GetDocumentSettings().IsOutputTableRowDefinitionAfter()) {
                WriteRowDefinition(result);
            }

            result.Write(ROW_END, 0, ROW_END.Length);
            result.WriteByte((byte)'\n');
        }
        
        /**
        * Gets the parent RtfTable of this RtfRow
        * 
        * @return The parent RtfTable of this RtfRow
        */
        protected internal RtfTable GetParentTable() {
            return this.parentTable;
        }
        
        /**
        * Gets the cells of this RtfRow
        * 
        * @return The cells of this RtfRow
        */
        protected internal ArrayList GetCells() {
            return this.cells;
        }
    }
}