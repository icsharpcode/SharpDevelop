using System;
using System.IO;
using System.Collections;
using iTextSharp.text;
using iTextSharp.text.rtf;
using iTextSharp.text.rtf.document;
using iTextSharp.text.rtf.text;
using iTextSharp.text.rtf.style;
/*
 * $Id: RtfTable.cs,v 1.9 2008/05/23 17:24:29 psoares33 Exp $
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
    * The RtfTable wraps a Table.
    * INTERNAL USE ONLY
    * 
    * @version $Version:$
    * @author Mark Hall (Mark.Hall@mail.room3b.eu)
    * @author Steffen Stundzig
    * @author Benoit WIART <b.wiart@proxiad.com>
    */
    public class RtfTable : RtfElement {

        /**
        * The rows of this RtfTable
        */
        private ArrayList rows = null;
        /**
        * The percentage of the page width that this RtfTable covers
        */
        private float tableWidthPercent = 80;
        /**
        * An array with the proportional widths of the cells in each row
        */
        private float[] proportionalWidths = null;
        /**
        * The cell padding
        */
        private float cellPadding = 0;
        /**
        * The cell spacing
        */
        private float cellSpacing = 0;
        /**
        * The border style of this RtfTable 
        */
        private RtfBorderGroup borders = null;
        /**
        * The alignment of this RtfTable
        */
        private int alignment = Element.ALIGN_CENTER;
        /**
        * Whether the cells in this RtfTable must fit in a page
        */
        private bool cellsFitToPage = false;
        /**
        * Whether the whole RtfTable must fit in a page
        */
        private bool tableFitToPage = false;
        /**
        * The number of header rows in this RtfTable
        */
        private int headerRows = 0;
        /**
        * The offset from the previous text
        */
        private int offset = -1;
        
        /**
        * Constructs a RtfTable based on a Table for a RtfDocument.
        * 
        * @param doc The RtfDocument this RtfTable belongs to
        * @param table The Table that this RtfTable wraps
        */
        public RtfTable(RtfDocument doc, Table table) : base(doc) {
            table.Complete();
            ImportTable(table);
        }
        
        /**
        * Imports the rows and settings from the Table into this
        * RtfTable.
        * 
        * @param table The source Table
        */
        private void ImportTable(Table table) {
            this.rows = new ArrayList();
            this.tableWidthPercent = table.Width;
            this.proportionalWidths = table.ProportionalWidths;
            this.cellPadding = (float) (table.Cellpadding * TWIPS_FACTOR);
            this.cellSpacing = (float) (table.Cellspacing * TWIPS_FACTOR);
            this.borders = new RtfBorderGroup(this.document, RtfBorder.ROW_BORDER, table.Border, table.BorderWidth, table.BorderColor);
            this.alignment = table.Alignment;
            
            int i = 0;
            foreach (Row row in table) {
                this.rows.Add(new RtfRow(this.document, this, row, i));
                i++;
            }
            for (i = 0; i < this.rows.Count; i++) {
                ((RtfRow) this.rows[i]).HandleCellSpanning();
                ((RtfRow) this.rows[i]).CleanRow();
            }
            this.headerRows = table.LastHeaderRow;
            this.cellsFitToPage = table.CellsFitPage;
            this.tableFitToPage = table.TableFitsPage;
            if (!float.IsNaN(table.Offset)) {
                this.offset = (int) (table.Offset * 2);
            }
        }
        
        /**
        * Writes the content of this RtfTable
        */    
        public override void WriteContent(Stream result) {
            if (!inHeader) {
                if(this.offset != -1) {
                    result.Write(RtfFont.FONT_SIZE, 0, RtfFont.FONT_SIZE.Length);
                    byte[] t;
                    result.Write(t = IntToByteArray(this.offset), 0, t.Length);
                }
                result.Write(RtfParagraph.PARAGRAPH, 0, RtfParagraph.PARAGRAPH.Length);
            }
            
            for (int i = 0; i < this.rows.Count; i++) {
        	    RtfElement re = (RtfElement)this.rows[i];
        	    re.WriteContent(result);
            }
            
            result.Write(RtfParagraph.PARAGRAPH_DEFAULTS, 0, RtfParagraph.PARAGRAPH_DEFAULTS.Length);
        }
        
        /**
        * Gets the alignment of this RtfTable
        * 
        * @return The alignment of this RtfTable.
        */
        protected internal int GetAlignment() {
            return alignment;
        }
        
        /**
        * Gets the borders of this RtfTable
        * 
        * @return The borders of this RtfTable.
        */
        protected internal RtfBorderGroup GetBorders() {
            return this.borders;
        }
        
        /**
        * Gets the cell padding of this RtfTable
        * 
        * @return The cell padding of this RtfTable.
        */
        protected internal float GetCellPadding() {
            return cellPadding;
        }
        
        /**
        * Gets the cell spacing of this RtfTable
        * 
        * @return The cell spacing of this RtfTable.
        */
        protected internal float GetCellSpacing() {
            return cellSpacing;
        }
        
        /**
        * Gets the proportional cell widths of this RtfTable
        * 
        * @return The proportional widths of this RtfTable.
        */
        protected internal float[] GetProportionalWidths() {
            return (float[]) proportionalWidths.Clone();
        }
        
        /**
        * Gets the percentage of the page width this RtfTable covers 
        * 
        * @return The percentage of the page width.
        */
        protected internal float GetTableWidthPercent() {
            return tableWidthPercent;
        }
        
        /**
        * Gets the rows of this RtfTable
        * 
        * @return The rows of this RtfTable
        */
        protected internal ArrayList GetRows() {
            return this.rows;
        }
        
        /**
        * Gets the cellsFitToPage setting of this RtfTable.
        * 
        * @return The cellsFitToPage setting of this RtfTable.
        */
        protected internal bool GetCellsFitToPage() {
            return this.cellsFitToPage;
        }
        
        /**
        * Gets the tableFitToPage setting of this RtfTable.
        * 
        * @return The tableFitToPage setting of this RtfTable.
        */
        protected internal bool GetTableFitToPage() {
            return this.tableFitToPage;
        }
        
        /**
        * Gets the number of header rows of this RtfTable
        * 
        * @return The number of header rows
        */
        protected internal int GetHeaderRows() {
            return this.headerRows;
        }
    }
}