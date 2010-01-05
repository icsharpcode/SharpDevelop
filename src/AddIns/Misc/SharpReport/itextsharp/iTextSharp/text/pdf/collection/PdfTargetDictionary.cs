using System;
using iTextSharp.text.pdf;

namespace iTextSharp.text.pdf.collection {

    public class PdfTargetDictionary : PdfDictionary {
        
        /**
        * Creates dictionary referring to a target document that is the parent of the current document.
        * @param nested    null if this is the actual target, another target if this is only an intermediate target.
        */
        public PdfTargetDictionary(PdfTargetDictionary nested) : base() {
            Put(PdfName.R, PdfName.P);
            if (nested != null)
                AdditionalPath = nested;
        }
        
        /**
        * Creates a dictionary referring to a target document.
        * @param child if false, this refers to the parent document; if true, this refers to a child document, and you'll have to specify where to find the child using the other methods of this class
        */
        public PdfTargetDictionary(bool child) : base() {
            if (child) {
                Put(PdfName.R, PdfName.C);
            }
            else {
                Put(PdfName.R, PdfName.P);
            }
        }
        
        /**
        * If this dictionary refers to a child that is a document level attachment,
        * you need to specify the name that was used to attach the document.
        * @param   name    the name in the EmbeddedFiles name tree
        */
        public String EmbeddedFileName {
            set {
                Put(PdfName.N, new PdfString(value, null));
            }
        }
        
        /**
        * If this dictionary refers to a child that is a file attachment added to a page,
        * you need to specify the name of the page (or use setFileAttachmentPage to specify the page number).
        * Once you have specified the page, you still need to specify the attachment using another method.
        * @param name  the named destination referring to the page with the file attachment.
        */
        public String FileAttachmentPagename {
            set {
                Put(PdfName.P, new PdfString(value, null));
            }
        }
        
        /**
        * If this dictionary refers to a child that is a file attachment added to a page,
        * you need to specify the page number (or use setFileAttachmentPagename to specify a named destination).
        * Once you have specified the page, you still need to specify the attachment using another method.
        * @param page  the page number of the page with the file attachment.
        */
        public int FileAttachmentPage {
            set {
                Put(PdfName.P, new PdfNumber(value));
            }
        }
        
        /**
        * If this dictionary refers to a child that is a file attachment added to a page,
        * you need to specify the page with setFileAttachmentPage or setFileAttachmentPageName,
        * and then specify the name of the attachment added to this page (or use setFileAttachmentIndex).
        * @param name      the name of the attachment
        */
        public String FileAttachmentName {
            set {
                Put(PdfName.A, new PdfString(value, PdfObject.TEXT_UNICODE));
            }
        }
        
        /**
        * If this dictionary refers to a child that is a file attachment added to a page,
        * you need to specify the page with setFileAttachmentPage or setFileAttachmentPageName,
        * and then specify the index of the attachment added to this page (or use setFileAttachmentName).
        * @param name      the name of the attachment
        */
        public int FileAttachmentIndex {
            set {
                Put(PdfName.A, new PdfNumber(value));
            }
        }
        
        /**
        * If this dictionary refers to an intermediate target, you can
        * add the next target in the sequence.
        * @param nested    the next target in the sequence
        */
        public PdfTargetDictionary AdditionalPath {
            set {
                Put(PdfName.T, value);
            }
        }
    }
}