using System;
using System.Collections;
using iTextSharp.text.pdf;

namespace iTextSharp.text
{
    /// <summary>
    /// Summary description for SimpleCell.
    /// </summary>
    public class SimpleCell : Rectangle, IPdfPCellEvent, ITextElementArray {

        /** the CellAttributes object represents a row. */
        public new const bool ROW = true;
        /** the CellAttributes object represents a cell. */
        public new const bool CELL = false;
        /** the content of the Cell. */
        private ArrayList content = new ArrayList();
        /** the width of the Cell. */
        private float width = 0f;
        /** the widthpercentage of the Cell. */
        private float widthpercentage = 0f;
        /** an extra spacing variable */
        private float spacing_left = float.NaN;
        /** an extra spacing variable */
        private float spacing_right = float.NaN;
        /** an extra spacing variable */
        private float spacing_top = float.NaN;
        /** an extra spacing variable */
        private float spacing_bottom = float.NaN;
        /** an extra padding variable */
        private float padding_left = float.NaN;
        /** an extra padding variable */
        private float padding_right = float.NaN;
        /** an extra padding variable */
        private float padding_top = float.NaN;
        /** an extra padding variable */
        private float padding_bottom = float.NaN;
        /** the colspan of a Cell */
        private int colspan = 1;
        /** horizontal alignment inside the Cell. */
        private int horizontalAlignment = Element.ALIGN_UNDEFINED;
        /** vertical alignment inside the Cell. */
        private int verticalAlignment = Element.ALIGN_UNDEFINED;
        /** indicates if these are the attributes of a single Cell (false) or a group of Cells (true). */
        private bool cellgroup = false;
        /** Indicates that the largest ascender height should be used to determine the
        * height of the first line.  Note that this only has an effect when rendered
        * to PDF.  Setting this to true can help with vertical alignment problems. */
        protected bool useAscender = false;
        /** Indicates that the largest descender height should be added to the height of
        * the last line (so characters like y don't dip into the border).   Note that
        * this only has an effect when rendered to PDF. */
        protected bool useDescender = false;
        /**
        * Adjusts the cell contents to compensate for border widths.  Note that
        * this only has an effect when rendered to PDF.
        */
        protected bool useBorderPadding;
        
        /**
        * A CellAttributes object is always constructed without any dimensions.
        * Dimensions are defined after creation.
        * @param row only true if the CellAttributes object represents a row.
        */
        public SimpleCell(bool row) : base (0f, 0f, 0f, 0f) {
            cellgroup = row;
            Border = BOX;
        }
        
        /**
        * Adds content to this object.
        * @param element
        * @throws BadElementException
        */
        public void AddElement(IElement element) {
            if (cellgroup) {
                if (element is SimpleCell) {
                    if (((SimpleCell)element).Cellgroup) {
                        throw new BadElementException("You can't add one row to another row.");
                    }
                    content.Add(element);
                    return;
                }
                else {
                    throw new BadElementException("You can only add cells to rows, no objects of type " + element.GetType().ToString());
                }
            }
            if (element.Type == Element.PARAGRAPH
                    || element.Type == Element.PHRASE
                    || element.Type == Element.ANCHOR
                    || element.Type == Element.CHUNK
                    || element.Type == Element.LIST
                    || element.Type == Element.MARKED
                    || element.Type == Element.JPEG
    				|| element.Type == Element.JPEG2000
                    || element.Type == Element.IMGRAW
                    || element.Type == Element.IMGTEMPLATE) {
                content.Add(element);
            }
            else {
                throw new BadElementException("You can't add an element of type " + element.GetType().ToString() + " to a SimpleCell.");
            }
        }
        
        /**
        * Creates a Cell with these attributes.
        * @param rowAttributes
        * @return a cell based on these attributes.
        * @throws BadElementException
        */
        public Cell CreateCell(SimpleCell rowAttributes) {
            Cell cell = new Cell();
            cell.CloneNonPositionParameters(rowAttributes);
            cell.SoftCloneNonPositionParameters(this);
            cell.Colspan = colspan;
            cell.HorizontalAlignment = horizontalAlignment;
            cell.VerticalAlignment = verticalAlignment;
            cell.UseAscender = useAscender;
            cell.UseBorderPadding = useBorderPadding;
            cell.UseDescender = useDescender;
            foreach (IElement element in content) {
                cell.AddElement(element);
            }
            return cell;
        }
        
        /**
        * Creates a PdfPCell with these attributes.
        * @param rowAttributes
        * @return a PdfPCell based on these attributes.
        */
        public PdfPCell CreatePdfPCell(SimpleCell rowAttributes) {
            PdfPCell cell = new PdfPCell();
            cell.Border = NO_BORDER;
            SimpleCell tmp = new SimpleCell(CELL);
            tmp.Spacing_left = spacing_left;
            tmp.Spacing_right = spacing_right;
            tmp.Spacing_top = spacing_top;
            tmp.Spacing_bottom = spacing_bottom;
            tmp.CloneNonPositionParameters(rowAttributes);
            tmp.SoftCloneNonPositionParameters(this);
            cell.CellEvent = tmp;
            cell.HorizontalAlignment = rowAttributes.horizontalAlignment;
            cell.VerticalAlignment = rowAttributes.verticalAlignment;
            cell.UseAscender = rowAttributes.useAscender;
            cell.UseBorderPadding = rowAttributes.useBorderPadding;
            cell.UseDescender = rowAttributes.useDescender;
            cell.Colspan = colspan;
            if (horizontalAlignment != Element.ALIGN_UNDEFINED)
                cell.HorizontalAlignment = horizontalAlignment;
            if (verticalAlignment != Element.ALIGN_UNDEFINED)
                cell.VerticalAlignment = verticalAlignment;
            if (useAscender)
                cell.UseAscender = useAscender;
            if (useBorderPadding)
                cell.UseBorderPadding = useBorderPadding;
            if (useDescender)
                cell.UseDescender = useDescender;
            float p;
            float sp_left = spacing_left;
            if (float.IsNaN(sp_left)) sp_left = 0f;
            float sp_right = spacing_right;
            if (float.IsNaN(sp_right)) sp_right = 0f;
            float sp_top = spacing_top;
            if (float.IsNaN(sp_top)) sp_top = 0f;
            float sp_bottom = spacing_bottom;
            if (float.IsNaN(sp_bottom)) sp_bottom = 0f;
            p = padding_left;
            if (float.IsNaN(p)) p = 0f; 
            cell.PaddingLeft = p + sp_left;
            p = padding_right;
            if (float.IsNaN(p)) p = 0f; 
            cell.PaddingRight = p + sp_right;
            p = padding_top;
            if (float.IsNaN(p)) p = 0f; 
            cell.PaddingTop = p + sp_top;
            p = padding_bottom;
            if (float.IsNaN(p)) p = 0f; 
            cell.PaddingBottom = p + sp_bottom;
            foreach (IElement element in content) {
                cell.AddElement(element);
            }
            return cell;
        }
        
        /**
        * @param rectangle
        * @param spacing
        * @return a rectangle
        */
        public static SimpleCell GetDimensionlessInstance(Rectangle rectangle, float spacing) {
            SimpleCell cell = new SimpleCell(CELL);
            cell.CloneNonPositionParameters(rectangle);
            cell.Spacing = spacing * 2;
            return cell;
        }

        /**
        * @see com.lowagie.text.pdf.PdfPCellEvent#cellLayout(com.lowagie.text.pdf.PdfPCell, com.lowagie.text.Rectangle, com.lowagie.text.pdf.PdfContentByte[])
        */
        public void CellLayout(PdfPCell cell, Rectangle position, PdfContentByte[] canvases) {
            float sp_left = spacing_left;
            if (float.IsNaN(sp_left)) sp_left = 0f;
            float sp_right = spacing_right;
            if (float.IsNaN(sp_right)) sp_right = 0f;
            float sp_top = spacing_top;
            if (float.IsNaN(sp_top)) sp_top = 0f;
            float sp_bottom = spacing_bottom;
            if (float.IsNaN(sp_bottom)) sp_bottom = 0f;
            Rectangle rect = new Rectangle(position.GetLeft(sp_left), position.GetBottom(sp_bottom), position.GetRight(sp_right), position.GetTop(sp_top));
            rect.CloneNonPositionParameters(this);
            canvases[PdfPTable.BACKGROUNDCANVAS].Rectangle(rect);
            rect.BackgroundColor = null;
            canvases[PdfPTable.LINECANVAS].Rectangle(rect);
        }
        
        /** Sets the padding parameters if they are undefined. 
        * @param padding*/
        public float Padding {
            set {
                if (float.IsNaN(padding_right)) {
                    Padding_right = value;
                }
                if (float.IsNaN(padding_left)) {
                    Padding_left = value;
                }
                if (float.IsNaN(padding_bottom)) {
                    Padding_bottom = value;
                }
                if (float.IsNaN(padding_top)) {
                    Padding_top = value;
                }
            }
        }
        
        /**
        * @return Returns the colspan.
        */
        public int Colspan {
            get {
                return colspan;
            }
            set {
                if (value > 0) this.colspan = value;
            }
        }

        /**
        * @param padding_bottom The padding_bottom to set.
        */
        public float Padding_bottom {
            get {
                return padding_bottom;
            }
            set {
                padding_bottom = value;
            }
        }

        public float Padding_left {
            get {
                return padding_left;
            }
            set {
                padding_left = value;
            }
        }

        public float Padding_right {
            get {
                return padding_right;
            }
            set {
                padding_right = value;
            }
        }

        public float Padding_top {
            get {
                return padding_top;
            }
            set {
                padding_top = value;
            }
        }

        /**
        * @return Returns the spacing.
        */
        public float Spacing {
            set {
                this.spacing_left = value;
                this.spacing_right = value;
                this.spacing_top = value;
                this.spacing_bottom = value;
            }
            
        }
        
        public float Spacing_top {
            get {
                return spacing_top;
            }
            set {
                spacing_top = value;
            }
        }

        public float Spacing_bottom {
            get {
                return spacing_bottom;
            }
            set {
                spacing_bottom = value;
            }
        }

        public float Spacing_left {
            get {
                return spacing_left;
            }
            set {
                spacing_left = value;
            }
        }

        public float Spacing_right {
            get {
                return spacing_right;
            }
            set {
                spacing_right = value;
            }
        }

        /**
        * @return Returns the cellgroup.
        */
        public bool Cellgroup {
            get {
                return cellgroup;
            }
            set {
                cellgroup = value;
            }
        }

        /**
        * @return Returns the horizontal alignment.
        */
        public int HorizontalAlignment {
            get {
                return horizontalAlignment;
            }
            set {
                horizontalAlignment = value;
            }
        }

        public int VerticalAlignment {
            get {
                return verticalAlignment;
            }
            set {
                verticalAlignment = value;
            }
        }
        /**
        * @return Returns the width.
        */
        public new float Width {
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
        * @return Returns the useAscender.
        */
        public bool UseAscender {
            get {
                return useAscender;
            }
            set {
                useAscender = value;
            }
        }

        public bool UseDescender {
            get {
                return useDescender;
            }
            set {
                useDescender = value;
            }
        }
        /**
        * @return Returns the useBorderPadding.
        */
        public bool UseBorderPadding {
            get {
                return useBorderPadding;
            }
            set {
                useBorderPadding = value;
            }
        }
        
        /**
        * @return Returns the content.
        */
        internal ArrayList Content {
            get {
                return content;
            }
        }

        /**
        * @see com.lowagie.text.TextElementArray#add(java.lang.Object)
        */
        public bool Add(Object o) {
            try {
                AddElement((IElement)o);
                return true;
            }
            catch (InvalidCastException) {
                return false;
            }
        }

        public override int Type {
            get {
                return Element.CELL;
            }
        }

    }
}
