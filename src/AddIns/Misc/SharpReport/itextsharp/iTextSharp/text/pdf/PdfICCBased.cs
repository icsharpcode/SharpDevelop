using System;

namespace iTextSharp.text.pdf {

    /**
     * A <CODE>PdfICCBased</CODE> defines a ColorSpace
     *
     * @see        PdfStream
     */

    public class PdfICCBased : PdfStream {
    
        public PdfICCBased(ICC_Profile profile) {
            int numberOfComponents = profile.NumComponents;
            switch (numberOfComponents) {
                case 1:
                    Put(PdfName.ALTERNATE, PdfName.DEVICEGRAY);
                    break;
                case 3:
                    Put(PdfName.ALTERNATE, PdfName.DEVICERGB);
                    break;
                case 4:
                    Put(PdfName.ALTERNATE, PdfName.DEVICECMYK);
                    break;
                default:
                    throw new PdfException(numberOfComponents + " Component(s) is not supported in iText");
            }
            Put(PdfName.N, new PdfNumber(numberOfComponents));
            bytes = profile.Data;
            FlateCompress();
        }
    }
}