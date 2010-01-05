using System;
using iTextSharp.text.pdf;

namespace iTextSharp.text.pdf.collection {

    /**
    * @author blowagie
    *
    */
    public class PdfCollectionField : PdfDictionary {
        /** A possible type of collection field. */
        public const int TEXT = 0;
        /** A possible type of collection field. */
        public const int DATE = 1;
        /** A possible type of collection field. */
        public new const int NUMBER = 2;
        /** A possible type of collection field. */
        public const int FILENAME = 3;
        /** A possible type of collection field. */
        public const int DESC = 4;
        /** A possible type of collection field. */
        public const int MODDATE = 5;
        /** A possible type of collection field. */
        public const int CREATIONDATE = 6;
        /** A possible type of collection field. */
        public const int SIZE = 7;
        
        /** The type of the PDF collection field. */
        protected internal int fieldType;

        /**
        * Creates a PdfCollectionField.
        * @param name      the field name
        * @param type      the field type
        */
        public PdfCollectionField(String name, int type) : base(PdfName.COLLECTIONFIELD) {
            Put(PdfName.N, new PdfString(name, PdfObject.TEXT_UNICODE));
            this.fieldType = type;
            switch (type) {
            default:
                Put(PdfName.SUBTYPE, PdfName.S);
                break;
            case DATE:
                Put(PdfName.SUBTYPE, PdfName.D);
                break;
            case NUMBER:
                Put(PdfName.SUBTYPE, PdfName.N);
                break;
            case FILENAME:
                Put(PdfName.SUBTYPE, PdfName.F);
                break;
            case DESC:
                Put(PdfName.SUBTYPE, PdfName.DESC);
                break;
            case MODDATE:
                Put(PdfName.SUBTYPE, PdfName.MODDATE);
                break;
            case CREATIONDATE:
                Put(PdfName.SUBTYPE, PdfName.CREATIONDATE);
                break;
            case SIZE:
                Put(PdfName.SUBTYPE, PdfName.SIZE);
                break;
            }
        }
        
        /**
        * The relative order of the field name. Fields are sorted in ascending order.
        * @param i a number indicating the order of the field
        */
        public int Order {
            set {
                Put(PdfName.O, new PdfNumber(value));
            }
        }
        
        /**
        * Sets the initial visibility of the field.
        * @param visible   the default is true (visible)
        */
        public bool Visible {
            set {
                Put(PdfName.V, new PdfBoolean(value));
            }
        }
        
        /**
        * Indication if the field value should be editable in the viewer.
        * @param editable  the default is false (not editable)
        */
        public bool Editable {
            set {
                Put(PdfName.E, new PdfBoolean(value));
            }
        }

        /**
        * Checks if the type of the field is suitable for a Collection Item.
        */
        public bool IsCollectionItem() {
            switch (fieldType) {
            case TEXT:
            case DATE:
            case NUMBER:
                return true;
            default:
                return false;
            }
        }
        
        /**
        * Returns a PdfObject that can be used as the value of a Collection Item.
        * @param String    value   the value that has to be changed into a PdfObject (PdfString, PdfDate or PdfNumber) 
        */
        public PdfObject GetValue(String v) {
            switch (fieldType) {
            case TEXT:
                return new PdfString(v, PdfObject.TEXT_UNICODE);
            case DATE:
                return new PdfDate(PdfDate.Decode(v));
            case NUMBER:
                return new PdfNumber(v);
            }
            throw new InvalidOperationException(v + " is not an acceptable value for the field " + Get(PdfName.N).ToString());
        }
    }
}