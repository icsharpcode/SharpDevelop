using System;

namespace iTextSharp.text.pdf
{
    /// <summary>
    /// Summary description for IPdfPCellEvent.
    /// </summary>
    public interface IPdfPCellEvent    {
        /** This method is called at the end of the cell rendering. The text or graphics are added to
        * one of the 4 <CODE>PdfContentByte</CODE> contained in
        * <CODE>canvases</CODE>.<br>
        * The indexes to <CODE>canvases</CODE> are:<p>
        * <ul>
        * <li><CODE>PdfPTable.BASECANVAS</CODE> - the original <CODE>PdfContentByte</CODE>. Anything placed here
        * will be under the cell.
        * <li><CODE>PdfPTable.BACKGROUNDCANVAS</CODE> - the layer where the background goes to.
        * <li><CODE>PdfPTable.LINECANVAS</CODE> - the layer where the lines go to.
        * <li><CODE>PdfPTable.TEXTCANVAS</CODE> - the layer where the text go to. Anything placed here
        * will be over the cell.
        * </ul>
        * The layers are placed in sequence on top of each other.
        * <p>
        * @param cell the cell
        * @param position the coordinates of the cell
        * @param canvases an array of <CODE>PdfContentByte</CODE>
        */    
        void CellLayout(PdfPCell cell, Rectangle position, PdfContentByte[] canvases);
    }
}
