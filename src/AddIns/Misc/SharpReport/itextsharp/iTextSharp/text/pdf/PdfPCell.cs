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

    /** A cell in a PdfPTable.
    */

    public class PdfPCell : Rectangle{
        
        private ColumnText column = new ColumnText(null);
        
        /** Holds value of property verticalAlignment. */
        private int verticalAlignment = Element.ALIGN_TOP;
        
        /** Holds value of property paddingLeft. */
        private float paddingLeft = 2;
        
        /** Holds value of property paddingLeft. */
        private float paddingRight = 2;
        
        /** Holds value of property paddingTop. */
        private float paddingTop = 2;
        
        /** Holds value of property paddingBottom. */
        private float paddingBottom = 2;
        
        /** Holds value of property fixedHeight. */
        private float fixedHeight = 0;
        
        /** Holds value of property noWrap. */
        private bool noWrap = false;
        
        /** Holds value of property table. */
        private PdfPTable table;
        
        /** Holds value of property minimumHeight. */
        private float minimumHeight;
        
        /** Holds value of property colspan. */
        private int colspan = 1;
        
        /** Holds value of property image. */
        private Image image;
        
        /** Holds value of property cellEvent. */
        private IPdfPCellEvent cellEvent;

        /** Holds value of property useDescender. */
        private bool useDescender;

        /** Increases padding to include border if true */
        private bool useBorderPadding = false;


        /** The text in the cell. */
        protected Phrase phrase;

        /** Constructs an empty <CODE>PdfPCell</CODE>.
        * The default padding is 2.
        */
        public PdfPCell() : base(0, 0, 0, 0) {
            borderWidth = 0.5f;
            border = BOX;
            column.SetLeading(0, 1);
        }

        /** Constructs a <CODE>PdfPCell</CODE> with a <CODE>Phrase</CODE>.
        * The default padding is 2.
        * @param phrase the text
        */
        public PdfPCell(Phrase phrase) : base(0, 0, 0, 0) {
            borderWidth = 0.5f;
            border = BOX;
            column.AddText(this.phrase = phrase);
            column.SetLeading(0, 1);
        }
        
        /** Constructs a <CODE>PdfPCell</CODE> with an <CODE>Image</CODE>.
        * The default padding is 0.
        * @param image the <CODE>Image</CODE>
        */
        public PdfPCell(Image image) : this(image, false) {
        }
        
        /** Constructs a <CODE>PdfPCell</CODE> with an <CODE>Image</CODE>.
        * The default padding is 0.25 for a border width of 0.5.
        * @param image the <CODE>Image</CODE>
        * @param fit <CODE>true</CODE> to fit the image to the cell
        */
        public PdfPCell(Image image, bool fit) : base(0, 0, 0, 0) {
            if (fit) {
                borderWidth = 0.5f;
                border = BOX;
                this.image = image;
                column.SetLeading(0, 1);
                Padding = borderWidth / 2;
            }
            else {
                borderWidth = 0.5f;
                border = BOX;
                column.AddText(this.phrase = new Phrase(new Chunk(image, 0, 0)));
                column.SetLeading(0, 1);
                Padding = 0;
            }
        }
        
        /** Constructs a <CODE>PdfPCell</CODE> with a <CODE>PdfPtable</CODE>.
        * This constructor allows nested tables.
        * The default padding is 0.
        * @param table The <CODE>PdfPTable</CODE>
        */
        public PdfPCell(PdfPTable table) : this(table, null) {
        }
        
        /** Constructs a <CODE>PdfPCell</CODE> with a <CODE>PdfPtable</CODE>.
        * This constructor allows nested tables.
        * 
        * @param table The <CODE>PdfPTable</CODE>
        * @param style  The style to apply to the cell (you could use getDefaultCell())
        * @since 2.1.0
        */
        public PdfPCell(PdfPTable table, PdfPCell style) : base(0, 0, 0, 0) {
            borderWidth = 0.5f;
            border = BOX;
            column.SetLeading(0, 1);
            this.table = table;
            table.WidthPercentage = 100;
            table.ExtendLastRow = true;
            column.AddElement(table);
            if (style != null) {
                CloneNonPositionParameters(style);
                verticalAlignment = style.verticalAlignment;
                paddingLeft = style.paddingLeft;
                paddingRight = style.paddingRight;
                paddingTop = style.paddingTop;
                paddingBottom = style.paddingBottom;
                colspan = style.colspan;
                cellEvent = style.cellEvent;
                useDescender = style.useDescender;
                useBorderPadding = style.useBorderPadding;
                rotation = style.rotation;
            }
            else {
                Padding = 0;
            }
        }
        
        /** Constructs a deep copy of a <CODE>PdfPCell</CODE>.
        * @param cell the <CODE>PdfPCell</CODE> to duplicate
        */
        public PdfPCell(PdfPCell cell) : base(cell.llx, cell.lly, cell.urx, cell.ury) {
            CloneNonPositionParameters(cell);
            verticalAlignment = cell.verticalAlignment;
            paddingLeft = cell.paddingLeft;
            paddingRight = cell.paddingRight;
            paddingTop = cell.paddingTop;
            paddingBottom = cell.paddingBottom;
            phrase = cell.phrase;
            fixedHeight = cell.fixedHeight;
            minimumHeight = cell.minimumHeight;
            noWrap = cell.noWrap;
            colspan = cell.colspan;
            if (cell.table != null)
                table = new PdfPTable(cell.table);
            image = Image.GetInstance(cell.image);
            cellEvent = cell.cellEvent;
            useDescender = cell.useDescender;
            column = ColumnText.Duplicate(cell.column);
            useBorderPadding = cell.useBorderPadding;
            rotation = cell.rotation;
        }
        
        /**
        * Adds an iText element to the cell.
        * @param element
        */
        public void AddElement(IElement element) {
            if (table != null) {
                table = null;
                column.SetText(null);
            }
            column.AddElement(element);
        }
        
        /** Gets the <CODE>Phrase</CODE> from this cell.
        * @return the <CODE>Phrase</CODE>
        */
        public Phrase Phrase {
            get {
                return phrase;
            }
            set {
                table = null;
                image = null;
                column.SetText(this.phrase = value);
            }
        }
        
        /** Gets the horizontal alignment for the cell.
        * @return the horizontal alignment for the cell
        */
        public int HorizontalAlignment {
            get {
                return column.Alignment;
            }
            set {
                column.Alignment = value;
            }
        }
        
        /** Gets the vertical alignment for the cell.
        * @return the vertical alignment for the cell
        */
        public int VerticalAlignment {
            get {
                return verticalAlignment;
            }
            set {
                verticalAlignment = value;
                if (table != null)
                    table.ExtendLastRow = (verticalAlignment == Element.ALIGN_TOP);
            }
        }
        
        /** Gets the effective left padding.  This will include
        *  the left border width if {@link #UseBorderPadding} is true.
        * @return effective value of property paddingLeft.
        */
        public float EffectivePaddingLeft {
            get {
                return paddingLeft + (UseBorderPadding ? (BorderWidthLeft/(UseVariableBorders?1f:2f)) : 0);
            }
        }
        
        /**
        * @return Value of property paddingLeft.
        */
        public float PaddingLeft {
            get {
                return paddingLeft;
            }
            set {
                paddingLeft = value;
            }
        }

        /** Gets the effective right padding.  This will include
        *  the right border width if {@link #UseBorderPadding} is true.
        * @return effective value of property paddingRight.
        */
        public float EffectivePaddingRight {
            get {
                return paddingRight + (UseBorderPadding ? (BorderWidthRight/(UseVariableBorders?1f:2f)) : 0);
            }
        }
        
        /**
        * Getter for property paddingRight.
        * @return Value of property paddingRight.
        */
        public float PaddingRight {
            get {
                return paddingRight;
            }
            set {
                paddingRight = value;
            }
        }

        /** Gets the effective top padding.  This will include
        *  the top border width if {@link #isUseBorderPadding()} is true.
        * @return effective value of property paddingTop.
        */
        public float EffectivePaddingTop {
            get {
                return paddingTop + (UseBorderPadding ? (BorderWidthTop/(UseVariableBorders?1f:2f)) : 0);
            }
        }
        
        /**
        * Getter for property paddingTop.
        * @return Value of property paddingTop.
        */
        public float PaddingTop {
            get {
                return paddingTop;
            }
            set {
                paddingTop = value;
            }
        }

        /**
        /** Gets the effective bottom padding.  This will include
        *  the bottom border width if {@link #UseBorderPadding} is true.
        * @return effective value of property paddingBottom.
        */
        public float EffectivePaddingBottom {
            get {
                return paddingBottom + (UseBorderPadding ? (BorderWidthBottom/(UseVariableBorders?1f:2f)) : 0);
            }
        }
        
        /**
        * Getter for property paddingBottom.
        * @return Value of property paddingBottom.
        */
        public float PaddingBottom {
            get {
                return paddingBottom;
            }
            set {
                paddingBottom = value;
            }
        }

        /**
        * Sets the padding of the contents in the cell (space between content and border).
        * @param padding
        */
        public float Padding {
            set {
                paddingBottom = value;
                paddingTop = value;
                paddingLeft = value;
                paddingRight = value;
            }
        }

        /**
        * Adjusts effective padding to include border widths.
        * @param use adjust effective padding if true
        */
        public bool UseBorderPadding {
            set {
                useBorderPadding = value;
            }
            get {
                return useBorderPadding;
            }
        }

        /**
        * Sets the leading fixed and variable. The resultant leading will be
        * fixedLeading+multipliedLeading*maxFontSize where maxFontSize is the
        * size of the bigest font in the line.
        * @param fixedLeading the fixed leading
        * @param multipliedLeading the variable leading
        */
        public void SetLeading(float fixedLeading, float multipliedLeading) {
            column.SetLeading(fixedLeading, multipliedLeading);
        }
        
        /**
        * Gets the fixed leading
        * @return the leading
        */
        public float Leading {
            get {
                return column.Leading;
            }
        }
        
        /**
        * Gets the variable leading
        * @return the leading
        */
        public float MultipliedLeading {
            get {
                return column.MultipliedLeading;
            }
        }
        
        /**
        * Gets the first paragraph line indent.
        * @return the indent
        */
        public float Indent {
            get {
                return column.Indent;
            }
            set {
                column.Indent = value;
            }
        }
        
        /**
        * Gets the extra space between paragraphs.
        * @return the extra space between paragraphs
        */
        public float ExtraParagraphSpace {
            get {
                return column.ExtraParagraphSpace;
            }
            set {
                column.ExtraParagraphSpace = value;
            }
        }
        
        /**
        * Getter for property fixedHeight.
        * @return Value of property fixedHeight.
        */
        public float FixedHeight {
            get {
                return fixedHeight;
            }
            set {
                fixedHeight = value;
                minimumHeight = 0;
            }
        }
        
        /**
        * Setter for property noWrap.
        * @param noWrap New value of property noWrap.
        */
        public bool NoWrap {
            set {
                noWrap = value;
            }
            get {
                return noWrap;
            }
        }
        
        /**
        * Getter for property table.
        * @return Value of property table.
        */
        internal PdfPTable Table {
            get {
                return table;
            }
            set {
                table = value;
                column.SetText(null);
                image = null;
                if (table != null) {
                    table.ExtendLastRow = (verticalAlignment == Element.ALIGN_TOP);
                    column.AddElement(table);
                    table.WidthPercentage = 100;
                }
            }
        }
        
        /** Getter for property minimumHeight.
        * @return Value of property minimumHeight.
        */
        public float MinimumHeight {
            get {
                return minimumHeight;
            }
            set {
                this.minimumHeight = value;
                fixedHeight = 0;
            }
        }
        
        /** Getter for property colspan.
        * @return Value of property colspan.
        */
        public int Colspan {
            get {
                return colspan;
            }
            set {
                colspan = value;
            }
        }
                
        /**
        * Gets the following paragraph lines indent.
        * @return the indent
        */
        public float FollowingIndent {
            get {
                return column.FollowingIndent;
            }
            set {
                column.FollowingIndent = value;
            }
        }
        
        /**
        * Gets the right paragraph lines indent.
        * @return the indent
        */
        public float RightIndent {
            get {
                return column.RightIndent;
            }
            set {
                column.RightIndent = value;
            }
        }
        
        /** Gets the space/character extra spacing ratio for
        * fully justified text.
        * @return the space/character extra spacing ratio
        */
        public float SpaceCharRatio {
            get {
                return column.SpaceCharRatio;
            }
            set {
                column.SpaceCharRatio = value;
            }
        }
        
        /**
        * Gets the run direction of the text content in the cell
        * @return One of the following values: PdfWriter.RUN_DIRECTION_DEFAULT, PdfWriter.RUN_DIRECTION_NO_BIDI, PdfWriter.RUN_DIRECTION_LTR or PdfWriter.RUN_DIRECTION_RTL.
        */
        public int RunDirection {
            get {
                return column.RunDirection;
            }
            set {
                column.RunDirection = value;
            }
        }
        
        /** Getter for property image.
        * @return Value of property image.
        *
        */
        public Image Image {
            get {
                return this.image;
            }
            set {
                column.SetText(null);
                table = null;
                this.image = value;
            }
        }
        
        /** Gets the cell event for this cell.
        * @return the cell event
        *
        */
        public IPdfPCellEvent CellEvent {
            get {
                return this.cellEvent;
            }
            set {
                if (value == null) this.cellEvent = null;
                else if (this.cellEvent == null) this.cellEvent = value;
                else if (this.cellEvent is PdfPCellEventForwarder) ((PdfPCellEventForwarder)this.cellEvent).AddCellEvent(value);
                else {
                    PdfPCellEventForwarder forward = new PdfPCellEventForwarder();
                    forward.AddCellEvent(this.cellEvent);
                    forward.AddCellEvent(value);
                    this.cellEvent = forward;
                }
            }
        }
        
        /** Gets the arabic shaping options.
        * @return the arabic shaping options
        */
        public int ArabicOptions {
            get {
                return column.ArabicOptions;
            }
            set {
                column.ArabicOptions = value;
            }
        }
        
        /** Gets state of first line height based on max ascender
        * @return true if an ascender is to be used.
        */
        public bool UseAscender {
            get {
                return column.UseAscender;
            }
            set {
                column.UseAscender = value;
            }
        }

        /** Getter for property useDescender.
        * @return Value of property useDescender.
        *
        */
        public bool UseDescender {
            get {
                return this.useDescender;
            }
            set {
                useDescender = value;
            }
        }

        /**
        * Gets the ColumnText with the content of the cell.
        * @return a columntext object
        */
        public ColumnText Column {
            get {
                return column;
            }
            set {
                column = value;
            }
        }

        /**
        * Returns the list of composite elements of the column.
        * @return   a List object.
        * @since    2.1.1
        */
        public ArrayList CompositeElements {
            get {
                return column.compositeElements;
            }
        }

        /**
        * The rotation of the cell. Possible values are
        * 0, 90, 180 and 270.
        */
        private new int rotation;

        /**
        * Sets the rotation of the cell. Possible values are
        * 0, 90, 180 and 270.
        * @param rotation the rotation of the cell
        */
        public new int Rotation {
            set {
                int rot = value % 360;
                if (rot < 0)
                    rot += 360;
                if ((rot % 90) != 0)
                    throw new ArgumentException("Rotation must be a multiple of 90.");
                rotation = rot;
            }
            get {
                return rotation;
            }
        }
    }
}