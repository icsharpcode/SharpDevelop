using System;

using iTextSharp.text;

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

    /**
    * A row in a PdfPTable.
    * 
    * @author Paulo Soares (psoares@consiste.pt)
    */

    public class PdfPRow {

        /** the bottom limit (bottom right y) */
        public const float BOTTOM_LIMIT = -(1 << 30);

        protected PdfPCell[] cells;

        protected float[] widths;

        protected float maxHeight = 0;

        protected bool calculated = false;

        private int[] canvasesPos;

        /**
        * Constructs a new PdfPRow with the cells in the array that was passed as a parameter.
        * @param cells
        */
        public PdfPRow(PdfPCell[] cells) {
            this.cells = cells;
            widths = new float[cells.Length];
        }

        /**
        * Makes a copy of an existing row.
        * @param row
        */
        public PdfPRow(PdfPRow row) {
            maxHeight = row.maxHeight;
            calculated = row.calculated;
            cells = new PdfPCell[row.cells.Length];
            for (int k = 0; k < cells.Length; ++k) {
                if (row.cells[k] != null)
                    cells[k] = new PdfPCell(row.cells[k]);
            }
            widths = new float[cells.Length];
            Array.Copy(row.widths, 0, widths, 0, cells.Length);
        }

        /**
        * Sets the widths of the columns in the row.
        * @param widths
        * @return true if everything went right
        */
        public bool SetWidths(float[] widths) {
            if (widths.Length != cells.Length)
                return false;
            Array.Copy(widths, 0, this.widths, 0, cells.Length);
            float total = 0;
            calculated = false;
            for (int k = 0; k < widths.Length; ++k) {
                PdfPCell cell = cells[k];
                cell.Left = total;
                int last = k + cell.Colspan;
                for (; k < last; ++k)
                    total += widths[k];
                --k;
                cell.Right = total;
                cell.Top = 0;
            }
            return true;
        }

        /**
        * Calculates the heights of each cell in the row.
        * @return the maximum height of the row.
        */
        public float CalculateHeights() {
            maxHeight = 0;
            for (int k = 0; k < cells.Length; ++k) {
                PdfPCell cell = cells[k];
                if (cell == null)
                    continue;
                Image img = cell.Image;
                if (img != null) {
                    img.ScalePercent(100);
                    float refWidth = img.ScaledWidth;
                    if (cell.Rotation == 90 || cell.Rotation == 270) {
                        refWidth = img.ScaledHeight;
                    }
                    float scale = (cell.Right - cell.EffectivePaddingRight
                            - cell.EffectivePaddingLeft - cell.Left)
                            / refWidth;
                    img.ScalePercent(scale * 100);
                    float refHeight = img.ScaledHeight;
                    if (cell.Rotation == 90 || cell.Rotation == 270) {
                        refHeight = img.ScaledWidth;
                    }
                    cell.Bottom = cell.Top - cell.EffectivePaddingTop
                                    - cell.EffectivePaddingBottom
                                    - refHeight;
                } else {
                    if (cell.Rotation == 0 || cell.Rotation == 180) {
                        float rightLimit = cell.NoWrap ? 20000 : cell.Right
                                - cell.EffectivePaddingRight;
                        float bry = (cell.FixedHeight > 0) ? cell.Top
                                - cell.EffectivePaddingTop
                                + cell.EffectivePaddingBottom
                                - cell.FixedHeight : BOTTOM_LIMIT;
                        ColumnText ct = ColumnText.Duplicate(cell.Column);
                        SetColumn(ct,
                                cell.Left + cell.EffectivePaddingLeft, bry,
                                rightLimit, cell.Top - cell.EffectivePaddingTop);
                        ct.Go(true);
                        float yLine = ct.YLine;
                        if (cell.UseDescender)
                            yLine += ct.Descender;
                        cell.Bottom = yLine - cell.EffectivePaddingBottom;
                    }
                    else {
                        if (cell.FixedHeight > 0) {
                            cell.Bottom = cell.Top - cell.FixedHeight;
                        }
                        else {
                            ColumnText ct = ColumnText.Duplicate(cell.Column);
                            SetColumn(ct, 0, cell.Left + cell.EffectivePaddingLeft,
                                    20000, cell.Right - cell.EffectivePaddingRight);
                            ct.Go(true);
                            cell.Bottom = cell.Top - cell.EffectivePaddingTop
                                - cell.EffectivePaddingBottom - ct.FilledWidth;
                        }
                    }
                }
                float height = cell.FixedHeight;
                if (height <= 0)
                    height = cell.Height;
                if (height < cell.FixedHeight)
                    height = cell.FixedHeight;
                else if (height < cell.MinimumHeight)
                    height = cell.MinimumHeight;
                if (height > maxHeight)
                    maxHeight = height;
            }
            calculated = true;
            return maxHeight;
        }

        /**
        * Writes the border and background of one cell in the row.
        * @param xPos
        * @param yPos
        * @param cell
        * @param canvases
        */
        public void WriteBorderAndBackground(float xPos, float yPos, PdfPCell cell,
                PdfContentByte[] canvases) {
            PdfContentByte lines = canvases[PdfPTable.LINECANVAS];
            PdfContentByte backgr = canvases[PdfPTable.BACKGROUNDCANVAS];
            // the coordinates of the border are retrieved
            float x1 = cell.Left + xPos;
            float y2 = cell.Top + yPos;
            float x2 = cell.Right + xPos;
            float y1 = y2 - maxHeight;

            // the backgroundcolor is set
            Color background = cell.BackgroundColor;
            if (background != null) {
                backgr.SetColorFill(background);
                backgr.Rectangle(x1, y1, x2 - x1, y2 - y1);
                backgr.Fill();
            }
            // if the element hasn't got any borders, nothing is added
            if (cell.HasBorders()) {
                if (cell.UseVariableBorders) {
                    Rectangle borderRect = new Rectangle(cell.Left + xPos, cell.Top
                            - maxHeight + yPos, cell.Right + xPos, cell.Top
                            + yPos);
                    borderRect.CloneNonPositionParameters(cell);
                    borderRect.BackgroundColor = null;
                    lines.Rectangle(borderRect);
                } else {
                    // the width is set to the width of the element
                    if (cell.BorderWidth != Rectangle.UNDEFINED) {
                        lines.SetLineWidth(cell.BorderWidth);
                    }
                    // the color is set to the color of the element
                    Color color = cell.BorderColor;
                    if (color != null) {
                        lines.SetColorStroke(color);
                    }

                    // if the box is a rectangle, it is added as a rectangle
                    if (cell.HasBorder(Rectangle.BOX)) {
                        lines.Rectangle(x1, y1, x2 - x1, y2 - y1);
                    }
                    // if the border isn't a rectangle, the different sides are
                    // added apart
                    else {
                        if (cell.HasBorder(Rectangle.RIGHT_BORDER)) {
                            lines.MoveTo(x2, y1);
                            lines.LineTo(x2, y2);
                        }
                        if (cell.HasBorder(Rectangle.LEFT_BORDER)) {
                            lines.MoveTo(x1, y1);
                            lines.LineTo(x1, y2);
                        }
                        if (cell.HasBorder(Rectangle.BOTTOM_BORDER)) {
                            lines.MoveTo(x1, y1);
                            lines.LineTo(x2, y1);
                        }
                        if (cell.HasBorder(Rectangle.TOP_BORDER)) {
                            lines.MoveTo(x1, y2);
                            lines.LineTo(x2, y2);
                        }
                    }
                    lines.Stroke();
                    if (color != null) {
                        lines.ResetRGBColorStroke();
                    }
                }
            }
        }

        private void SaveAndRotateCanvases(PdfContentByte[] canvases, float a, float b, float c, float d, float e, float f) {
            int last = PdfPTable.TEXTCANVAS + 1;
            if (canvasesPos == null) {
                canvasesPos = new int[last * 2];
            }
            for (int k = 0; k < last; ++k) {
                ByteBuffer bb = canvases[k].InternalBuffer;
                canvasesPos[k * 2] = bb.Size;
                canvases[k].SaveState();
                canvases[k].ConcatCTM(a, b, c, d, e, f);
                canvasesPos[k * 2 + 1] = bb.Size;
            }
        }
        
        private void RestoreCanvases(PdfContentByte[] canvases) {
            int last = PdfPTable.TEXTCANVAS + 1;
            for (int k = 0; k < last; ++k) {
                ByteBuffer bb = canvases[k].InternalBuffer;
                int p1 = bb.Size;
                canvases[k].RestoreState();
                if (p1 == canvasesPos[k * 2 + 1])
                    bb.Size = canvasesPos[k * 2];
            }
        }
        
        private float SetColumn(ColumnText ct, float llx, float lly, float urx, float ury) {
            if (llx > urx)
                urx = llx;
            if (lly > ury)
                ury = lly;
            ct.SetSimpleColumn(llx, lly, urx, ury);
            return ury;
        }

        /**
        * Writes a number of cells (not necessarily all cells).
        * @param colStart
        * @param colEnd
        * @param xPos
        * @param yPos
        * @param canvases
        */
        public void WriteCells(int colStart, int colEnd, float xPos, float yPos,
                PdfContentByte[] canvases) {
            if (!calculated)
                CalculateHeights();
            if (colEnd < 0)
                colEnd = cells.Length;
            colEnd = Math.Min(colEnd, cells.Length);
            if (colStart < 0)
                colStart = 0;
            if (colStart >= colEnd)
                return;
            int newStart;
            for (newStart = colStart; newStart >= 0; --newStart) {
                if (cells[newStart] != null)
                    break;
                xPos -= widths[newStart - 1];
            }
            xPos -= cells[newStart].Left;
            for (int k = newStart; k < colEnd; ++k) {
                PdfPCell cell = cells[k];
                if (cell == null)
                    continue;
                WriteBorderAndBackground(xPos, yPos, cell, canvases);
                Image img = cell.Image;
                float tly = 0;
                switch (cell.VerticalAlignment) {
                case Element.ALIGN_BOTTOM:
                    tly = cell.Top + yPos - maxHeight + cell.Height
                            - cell.EffectivePaddingTop;
                    break;
                case Element.ALIGN_MIDDLE:
                    tly = cell.Top + yPos + (cell.Height - maxHeight) / 2
                            - cell.EffectivePaddingTop;
                    break;
                default:
                    tly = cell.Top + yPos - cell.EffectivePaddingTop;
                    break;
                }
                if (img != null) {
                    if (cell.Rotation != 0) {
                        img = Image.GetInstance(img);
                        img.Rotation = img.GetImageRotation() + (float)(cell.Rotation * Math.PI / 180.0);
                    }
                    bool vf = false;
                    if (cell.Height > maxHeight) {
                        img.ScalePercent(100);
                        float scale = (maxHeight - cell.EffectivePaddingTop - cell
                                .EffectivePaddingBottom)
                                / img.ScaledHeight;
                        img.ScalePercent(scale * 100);
                        vf = true;
                    }
                    float left = cell.Left + xPos
                            + cell.EffectivePaddingLeft;
                    if (vf) {
                        switch (cell.HorizontalAlignment) {
                        case Element.ALIGN_CENTER:
                            left = xPos
                                    + (cell.Left + cell.EffectivePaddingLeft
                                            + cell.Right
                                            - cell.EffectivePaddingRight - img
                                            .ScaledWidth) / 2;
                            break;
                        case Element.ALIGN_RIGHT:
                            left = xPos + cell.Right
                                    - cell.EffectivePaddingRight
                                    - img.ScaledWidth;
                            break;
                        default:
                            break;
                        }
                        tly = cell.Top + yPos - cell.EffectivePaddingTop;
                    }
                    img.SetAbsolutePosition(left, tly - img.ScaledHeight);
                    canvases[PdfPTable.TEXTCANVAS].AddImage(img);
                } else {
                    if (cell.Rotation == 90 || cell.Rotation == 270) {
                        float netWidth = maxHeight - cell.EffectivePaddingTop - cell.EffectivePaddingBottom;
                        float netHeight = cell.Width - cell.EffectivePaddingLeft - cell.EffectivePaddingRight;
                        ColumnText ct = ColumnText.Duplicate(cell.Column);
                        ct.Canvases = canvases;
                        ct.SetSimpleColumn(0, 0, netWidth + 0.001f, -netHeight);
                        ct.Go(true);
                        float calcHeight = -ct.YLine;
                        if (netWidth <= 0 || netHeight <= 0)
                            calcHeight = 0;
                        if (calcHeight > 0) {
                            if (cell.UseDescender)
                                calcHeight -= ct.Descender;
                            ct = ColumnText.Duplicate(cell.Column);
                            ct.Canvases = canvases;
                            ct.SetSimpleColumn(0, -0.001f, netWidth + 0.001f, calcHeight);
                            float pivotX;
                            float pivotY;
                            if (cell.Rotation == 90) {
                                pivotY = cell.Top + yPos - maxHeight + cell.EffectivePaddingBottom;
                                switch (cell.VerticalAlignment) {
                                case Element.ALIGN_BOTTOM:
                                    pivotX = cell.Left + xPos + cell.Width - cell.EffectivePaddingRight;
                                    break;
                                case Element.ALIGN_MIDDLE:
                                    pivotX = cell.Left + xPos + (cell.Width + cell.EffectivePaddingLeft - cell.EffectivePaddingRight + calcHeight) / 2;
                                    break;
                                default: //top
                                    pivotX = cell.Left + xPos + cell.EffectivePaddingLeft + calcHeight;
                                    break;
                                }
                                SaveAndRotateCanvases(canvases, 0,1,-1,0,pivotX,pivotY);
                            }
                            else {
                                pivotY = cell.Top + yPos - cell.EffectivePaddingTop;
                                switch (cell.VerticalAlignment) {
                                case Element.ALIGN_BOTTOM:
                                    pivotX = cell.Left + xPos + cell.EffectivePaddingLeft;
                                    break;
                                case Element.ALIGN_MIDDLE:
                                    pivotX = cell.Left + xPos + (cell.Width + cell.EffectivePaddingLeft - cell.EffectivePaddingRight - calcHeight) / 2;
                                    break;
                                default: //top
                                    pivotX = cell.Left + xPos + cell.Width - cell.EffectivePaddingRight - calcHeight;
                                    break;
                                }
                                SaveAndRotateCanvases(canvases, 0,-1,1,0,pivotX,pivotY);
                            }
                            try {
                                ct.Go();
                            } finally {
                                RestoreCanvases(canvases);
                            }
                        }
                    } 
                    else {
                        float fixedHeight = cell.FixedHeight;
                        float rightLimit = cell.Right + xPos
                                - cell.EffectivePaddingRight;
                        float leftLimit = cell.Left + xPos
                                + cell.EffectivePaddingLeft;
                        if (cell.NoWrap) {
                            switch (cell.HorizontalAlignment) {
                            case Element.ALIGN_CENTER:
                                rightLimit += 10000;
                                leftLimit -= 10000;
                                break;
                            case Element.ALIGN_RIGHT:
                                leftLimit -= 20000;
                                break;
                            default:
                                rightLimit += 20000;
                                break;
                            }
                        }
                        ColumnText ct = ColumnText.Duplicate(cell.Column);
                        ct.Canvases = canvases;
                        float bry = tly
                                - (maxHeight /* cell.Height */
                                        - cell.EffectivePaddingTop - cell
                                        .EffectivePaddingBottom);
                        if (fixedHeight > 0) {
                            if (cell.Height > maxHeight) {
                                tly = cell.Top + yPos - cell.EffectivePaddingTop;
                                bry = cell.Top + yPos - maxHeight
                                        + cell.EffectivePaddingBottom;
                            }
                        }
                        if ((tly > bry || ct.ZeroHeightElement()) && leftLimit < rightLimit) {
                            ct.SetSimpleColumn(leftLimit, bry - 0.001f, rightLimit, tly);
                            if (cell.Rotation == 180) {
                                float shx = leftLimit + rightLimit;
                                float shy = yPos + yPos - maxHeight + cell.EffectivePaddingBottom - cell.EffectivePaddingTop;
                                SaveAndRotateCanvases(canvases, -1,0,0,-1,shx,shy);
                            }
                            try {
                                ct.Go();
                            } finally {
                                if (cell.Rotation == 180) {
                                    RestoreCanvases(canvases);
                                }
                            }
                        }
                    }
                }
                IPdfPCellEvent evt = cell.CellEvent;
                if (evt != null) {
                    Rectangle rect = new Rectangle(cell.Left + xPos, cell.Top
                            + yPos - maxHeight, cell.Right + xPos, cell.Top
                            + yPos);
                    evt.CellLayout(cell, rect, canvases);
                }
            }
        }

        /**
        * Checks if the dimensions of the columns were calculated.
        * @return true if the dimensions of the columns were calculated
        */
        public bool IsCalculated() {
            return calculated;
        }

        /**
        * Gets the maximum height of the row (i.e. of the 'highest' cell).
        * @return the maximum height of the row
        */
        public float MaxHeights {
            get {
                if (calculated)
                    return maxHeight;
                else
                    return CalculateHeights();
            }
            set {
                this.maxHeight = value;
            }
        }


        internal float[] GetEventWidth(float xPos) {
            int n = 0;
            for (int k = 0; k < cells.Length; ++k) {
                if (cells[k] != null)
                    ++n;
            }
            float[] width = new float[n + 1];
            n = 0;
            width[n++] = xPos;
            for (int k = 0; k < cells.Length; ++k) {
                if (cells[k] != null) {
                    width[n] = width[n - 1] + cells[k].Width;
                    ++n;
                }
            }
            return width;
        }

        /**
        * Splits a row to newHeight. The returned row is the remainder. It will
        * return null if the newHeight was so small that only an empty row would
        * result.
        * 
        * @param newHeight
        *            the new height
        * @return the remainder row or null if the newHeight was so small that only
        *         an empty row would result
        */
        public PdfPRow SplitRow(float newHeight) {
            PdfPCell[] newCells = new PdfPCell[cells.Length];
            float[] fh = new float[cells.Length * 2];
            bool allEmpty = true;
            for (int k = 0; k < cells.Length; ++k) {
                PdfPCell cell = cells[k];
                if (cell == null)
                    continue;
                fh[k * 2] = cell.FixedHeight;
                fh[k * 2 + 1] = cell.MinimumHeight;
                Image img = cell.Image;
                PdfPCell c2 = new PdfPCell(cell);
                if (img != null) {
                    if (newHeight > cell.EffectivePaddingBottom
                            + cell.EffectivePaddingTop + 2) {
                        c2.Phrase = null;
                        allEmpty = false;
                    }
                } else {
                    int status;
                    float y;
                    ColumnText ct = ColumnText.Duplicate(cell.Column);
                    if (cell.Rotation == 90 || cell.Rotation == 270) {
                        y = SetColumn(ct,
                                cell.Top - newHeight + cell.EffectivePaddingBottom,
                                cell.Left + cell.EffectivePaddingLeft,
                                cell.Top - cell.EffectivePaddingTop,
                                cell.Right - cell.EffectivePaddingRight);
                    }
                    else {
                        float rightLimit = cell.NoWrap ? 20000 : cell.Right
                                - cell.EffectivePaddingRight;
                        float y1 = cell.Top - newHeight
                                + cell.EffectivePaddingBottom;
                        float y2 = cell.Top - cell.EffectivePaddingTop;
                        y = Math.Max(y1, y2);
                        y = SetColumn(ct,
                                cell.Left + cell.EffectivePaddingLeft, y1,
                                rightLimit, y2);
                    }
                    status = ct.Go(true);
                    bool thisEmpty = (ct.YLine == y);
                    if (thisEmpty)
                        ct = ColumnText.Duplicate(cell.Column);
                    allEmpty = (allEmpty && thisEmpty);
                    if ((status & ColumnText.NO_MORE_TEXT) == 0 || thisEmpty) {
                        c2.Column = ct;
                        ct.FilledWidth = 0;
                    } else {
                        c2.Phrase = null;
                    }
                }
                newCells[k] = c2;
                cell.FixedHeight = newHeight;
            }
            if (allEmpty) {
                for (int k = 0; k < cells.Length; ++k) {
                    PdfPCell cell = cells[k];
                    if (cell == null)
                        continue;
                    float f = fh[k * 2];
                    float m = fh[k * 2 + 1];
                    if (f <= 0)
                        cell.MinimumHeight = m;
                    else
                        cell.FixedHeight = f;
                }
                return null;
            }
            CalculateHeights();
            PdfPRow split = new PdfPRow(newCells);
            split.widths = (float[]) widths.Clone();
            split.CalculateHeights();
            return split;
        }

        /**
        * Returns the array of cells in the row.
        * Please be extremely careful with this method.
        * Use the cells as read only objects.
        * @return  an array of cells
        * @since   2.1.1
        */
        public PdfPCell[] Cells {
            get {
                return cells;
            }
        }
    }
}