using System;
using System.Collections;
using iTextSharp.text.pdf;

namespace iTextSharp.text
{
        /// <summary>
        /// Summary description for SimpleTable.
        /// </summary>
    public class SimpleTable : Rectangle, IPdfPTableEvent, ITextElementArray {

        /** the content of a Table. */
        private ArrayList content = new ArrayList();
        /** the width of the Table. */
        private float width = 0f;
        /** the widthpercentage of the Table. */
        private float widthpercentage = 0f;
        /** the spacing of the Cells. */
        private float cellspacing;
        /** the padding of the Cells. */
        private float cellpadding;
        /** the alignment of the table. */
        private int alignment;
        
        /**
        * A RectangleCell is always constructed without any dimensions.
        * Dimensions are defined after creation.
        */
        public SimpleTable() : base(0f, 0f, 0f, 0f) {
            Border = BOX;
            BorderWidth = 2f;
        }
        
        /**
        * Adds content to this object.
        * @param element
        * @throws BadElementException
        */
        public void AddElement(SimpleCell element) {
            if (!element.Cellgroup) {
                throw new BadElementException("You can't add cells to a table directly, add them to a row first.");
            }
            content.Add(element);
        }
        
        /**
        * Creates a Table object based on this TableAttributes object.
        * @return a com.lowagie.text.Table object
        * @throws BadElementException
        */
        public Table CreateTable() {
            if (content.Count == 0) throw new BadElementException("Trying to create a table without rows.");
            SimpleCell rowx = (SimpleCell)content[0];
            int columns = 0;
            foreach (SimpleCell cell in rowx.Content) {
                columns += cell.Colspan;
            }
            float[] widths = new float[columns];
            float[] widthpercentages = new float[columns];
            Table table = new Table(columns);
            table.Alignment = alignment;
            table.Spacing = cellspacing;
            table.Padding = cellpadding;
            table.CloneNonPositionParameters(this);
            int pos;
            foreach (SimpleCell row in content) {
                pos = 0;
                foreach (SimpleCell cell in row.Content) {
                    table.AddCell(cell.CreateCell(row));
                    if (cell.Colspan == 1) {
                        if (cell.Width > 0) widths[pos] = cell.Width;
                        if (cell.Widthpercentage > 0) widthpercentages[pos] = cell.Widthpercentage;
                    }
                    pos += cell.Colspan;
                }
            }
            float sumWidths = 0f;
            for (int i = 0; i < columns; i++) {
                if (widths[i] == 0) {
                    sumWidths = 0;
                    break;
                }
                sumWidths += widths[i];
            }
            if (sumWidths > 0) {
                table.Width = sumWidths;
                table.Locked = true;
                table.Widths = widths;
            }
            else {
                for (int i = 0; i < columns; i++) {
                    if (widthpercentages[i] == 0) {
                        sumWidths = 0;
                        break;
                    }
                    sumWidths += widthpercentages[i];
                }
                if (sumWidths > 0) {
                    table.Widths = widthpercentages;
                }
            }
            if (width > 0) {
                table.Width = width;
                table.Locked = true;
            }
            else if (widthpercentage > 0) {
                table.Width = widthpercentage;
            }
            return table;
        }
        
        /**
        * Creates a PdfPTable object based on this TableAttributes object.
        * @return a com.lowagie.text.pdf.PdfPTable object
        * @throws DocumentException
        */
        public PdfPTable CreatePdfPTable() {
            if (content.Count == 0) throw new BadElementException("Trying to create a table without rows.");
            SimpleCell rowx = (SimpleCell)content[0];
            int columns = 0;
            foreach (SimpleCell cell in rowx.Content) {
                columns += cell.Colspan;
            }
            float[] widths = new float[columns];
            float[] widthpercentages = new float[columns];
            PdfPTable table = new PdfPTable(columns);
            table.TableEvent = this;
            table.HorizontalAlignment = alignment;
            int pos;
            foreach (SimpleCell row in content) {
                pos = 0;
                foreach (SimpleCell cell in row.Content) {
                    if (float.IsNaN(cell.Spacing_left))    {
                        cell.Spacing_left = cellspacing / 2f;
                    }
                    if (float.IsNaN(cell.Spacing_right))   {
                        cell.Spacing_right = cellspacing / 2f;
                    }
                    if (float.IsNaN(cell.Spacing_top)) {
                        cell.Spacing_top = cellspacing / 2f;
                    }
                    if (float.IsNaN(cell.Spacing_bottom))  {
                        cell.Spacing_bottom = cellspacing / 2f;
                    }
                    cell.Padding = cellpadding;
                    table.AddCell(cell.CreatePdfPCell(row));
                    if (cell.Colspan == 1) {
                        if (cell.Width > 0) widths[pos] = cell.Width;
                        if (cell.Widthpercentage > 0) widthpercentages[pos] = cell.Widthpercentage;
                    }
                    pos += cell.Colspan;
                }
            }
            float sumWidths = 0f;
            for (int i = 0; i < columns; i++) {
                if (widths[i] == 0) {
                    sumWidths = 0;
                    break;
                }
                sumWidths += widths[i];
            }
            if (sumWidths > 0) {
                table.TotalWidth = sumWidths;
                table.SetWidths(widths);
            }
            else {
                for (int i = 0; i < columns; i++) {
                    if (widthpercentages[i] == 0) {
                        sumWidths = 0;
                        break;
                    }
                    sumWidths += widthpercentages[i];
                }
                if (sumWidths > 0) {
                    table.SetWidths(widthpercentages);
                }
            }
            if (width > 0) {
                table.TotalWidth = width;
            }
            if (widthpercentage > 0) {
                table.WidthPercentage = widthpercentage;
            }
            return table;
        }
        
        /**
        * @param rectangle
        * @param spacing
        * @return a rectangle
        */
        public static SimpleTable GetDimensionlessInstance(Rectangle rectangle, float spacing) {
            SimpleTable ev = new SimpleTable();
            ev.CloneNonPositionParameters(rectangle);
            ev.Cellspacing = spacing;
            return ev;
        }
        
        /**
        * @see com.lowagie.text.pdf.PdfPTableEvent#tableLayout(com.lowagie.text.pdf.PdfPTable, float[][], float[], int, int, com.lowagie.text.pdf.PdfContentByte[])
         */
        public void TableLayout(PdfPTable table, float[][] widths, float[] heights, int headerRows, int rowStart, PdfContentByte[] canvases) {
            float[] width = widths[0];
            Rectangle rect = new Rectangle(width[0], heights[heights.Length - 1], width[width.Length - 1], heights[0]);
            rect.CloneNonPositionParameters(this);
            int bd = rect.Border;
            rect.Border = Rectangle.NO_BORDER;
            canvases[PdfPTable.BACKGROUNDCANVAS].Rectangle(rect);
            rect.Border = bd;
            rect.BackgroundColor = null;
            canvases[PdfPTable.LINECANVAS].Rectangle(rect);
        }
        
        /**
        * @return Returns the cellpadding.
        */
        public float Cellpadding {
            get {
                return cellpadding;
            }
            set {
                cellpadding = value;
            }
        }

        /**
        * @return Returns the cellspacing.
        */
        public float Cellspacing {
            get {
                return cellspacing;
            }
            set {
                cellspacing = value;
            }
        }
        
        /**
        * @return Returns the alignment.
        */
        public int Alignment {
            get {
                return alignment;
            }
            set {
                alignment = value;
            }
        }

        /**
        * @return Returns the width.
        */
        public override float Width {
            get {
                return width;
            }
            set {
                width = value;
            }
        }
        /**
        * @return Returns the widthpercentage.
        */
        public float Widthpercentage {
            get {
                return widthpercentage;
            }
            set {
                widthpercentage = value;
            }
        }
        /**
        * @see com.lowagie.text.Element#type()
        */
        public override int Type {
            get {
                return Element.TABLE;
            }
        }

        /**
        * @see com.lowagie.text.Element#isNestable()
        * @since   iText 2.0.8
        */
        public override bool IsNestable() {
            return true;
        }

        /**
        * @see com.lowagie.text.TextElementArray#add(java.lang.Object)
        */
        public bool Add(Object o) {
            try {
                AddElement((SimpleCell)o);
                return true;
            }
            catch (InvalidCastException) {
                return false;
            }
        }
    }
}
