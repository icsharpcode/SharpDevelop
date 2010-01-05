using System;
using iTextSharp.text.pdf;

namespace iTextSharp.text.pdf.collection {

    public class PdfCollectionSort : PdfDictionary {
        
        /**
        * Constructs a PDF Collection Sort Dictionary.
        * @param key   the key of the field that will be used to sort entries
        */
        public PdfCollectionSort(String key) : base(PdfName.COLLECTIONSORT) {
            Put(PdfName.S, new PdfName(key));
        }
        
        /**
        * Constructs a PDF Collection Sort Dictionary.
        * @param keys  the keys of the fields that will be used to sort entries
        */
        public PdfCollectionSort(String[] keys) : base(PdfName.COLLECTIONSORT) {
            PdfArray array = new PdfArray();
            for (int i = 0; i < keys.Length; i++) {
                array.Add(new PdfName(keys[i]));
            }
            Put(PdfName.S, array);
        }
        
        /**
        * Defines the sort order of the field (ascending or descending).
        * @param ascending true is the default, use false for descending order
        */
        public void SetSortOrder(bool ascending) {
            PdfObject o = (PdfObject)Get(PdfName.S);
            if (o is PdfName) {
                Put(PdfName.A, new PdfBoolean(ascending));
            }
            else {
                throw new InvalidOperationException("You have to define a bool array for this collection sort dictionary.");
            }
        }
        
        /**
        * Defines the sort order of the field (ascending or descending).
        * @param ascending an array with every element corresponding with a name of a field.
        */
        public void SetSortOrder(bool[] ascending) {
            PdfObject o = (PdfObject)Get(PdfName.S);
            if (o is PdfArray) {
                if (((PdfArray)o).Size != ascending.Length) {
                    throw new InvalidOperationException("The number of booleans in this array doesn't correspond with the number of fields.");
                }
                PdfArray array = new PdfArray();
                for (int i = 0; i < ascending.Length; i++) {
                    array.Add(new PdfBoolean(ascending[i]));
                }
                Put(PdfName.A, array);
            }
            else {
                throw new InvalidOperationException("You need a single bool for this collection sort dictionary.");
            }
        }
    }
}