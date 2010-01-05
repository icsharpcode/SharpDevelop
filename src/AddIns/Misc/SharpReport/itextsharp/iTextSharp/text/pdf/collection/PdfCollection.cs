using System;
using iTextSharp.text.pdf;

namespace iTextSharp.text.pdf.collection {

    public class PdfCollection : PdfDictionary {

        /** A type of PDF Collection */
        public const int DETAILS = 0;
        /** A type of PDF Collection */
        public const int TILE = 1;
        /** A type of PDF Collection */
        public const int HIDDEN = 2;
        
        /**
        * Constructs a PDF Collection.
        * @param   type    the type of PDF collection.
        */
        public PdfCollection(int type) : base(PdfName.COLLECTION) {
            switch(type) {
            case TILE:
                Put(PdfName.VIEW, PdfName.T);
                break;
            case HIDDEN:
                Put(PdfName.VIEW, PdfName.H);
                break;
            default:
                Put(PdfName.VIEW, PdfName.D);
                break;
            }
        }
        
        /**
        * Identifies the document that will be initially presented
        * in the user interface.
        * @param description   the description that was used when attaching the file to the document
        */
        public String InitialDocument {
            set {
                Put(PdfName.D, new PdfString(value, null));
            }
        }
        
        /**
        * Sets the Collection schema dictionary.
        * @param schema    an overview of the collection fields
        */
        public PdfCollectionSchema Schema {
            set {
                Put(PdfName.SCHEMA, value);
            }
            get {
                return (PdfCollectionSchema)Get(PdfName.SCHEMA);
            }
        }
        
        /**
        * Sets the Collection sort dictionary.
        * @param sort  a collection sort dictionary
        */
        public PdfCollectionSort Sort {
            set {
                Put(PdfName.SORT, value);
            }
        }
    }
}