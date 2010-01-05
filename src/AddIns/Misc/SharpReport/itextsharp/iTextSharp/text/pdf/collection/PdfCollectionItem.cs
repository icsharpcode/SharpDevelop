using System;
using iTextSharp.text.pdf;

namespace iTextSharp.text.pdf.collection {

    public class PdfCollectionItem : PdfDictionary {
        
        /** The PdfCollectionSchema with the names and types of the items. */
        internal PdfCollectionSchema schema;
        
        /**
        * Constructs a Collection Item that can be added to a PdfFileSpecification.
        */
        public PdfCollectionItem(PdfCollectionSchema schema) : base(PdfName.COLLECTIONITEM) {
            this.schema = schema;
        }
        
        /**
        * Sets the value of the collection item.
        * @param value
        */
        public void AddItem(String key, String value) {
            PdfName fieldname = new PdfName(key);
            PdfCollectionField field = (PdfCollectionField)schema.Get(fieldname);
            Put(fieldname, field.GetValue(value));
        }
        
        /**
        * Sets the value of the collection item.
        * @param value
        */
        public void AddItem(String key, PdfString value) {
            PdfName fieldname = new PdfName(key);
            PdfCollectionField field = (PdfCollectionField)schema.Get(fieldname);
            if (field.fieldType == PdfCollectionField.TEXT) {
                Put(fieldname, value);
            }
        }
        
        /**
        * Sets the value of the collection item.
        * @param value
        */
        public void AddItem(String key, PdfDate d) {
            PdfName fieldname = new PdfName(key);
            PdfCollectionField field = (PdfCollectionField)schema.Get(fieldname);
            if (field.fieldType == PdfCollectionField.DATE) {
                Put(fieldname, d);
            }
        }
        
        /**
        * Sets the value of the collection item.
        * @param value
        */
        public void AddItem(String key, PdfNumber n) {
            PdfName fieldname = new PdfName(key);
            PdfCollectionField field = (PdfCollectionField)schema.Get(fieldname);
            if (field.fieldType == PdfCollectionField.NUMBER) {
                Put(fieldname, n);
            }
        }
        
        /**
        * Sets the value of the collection item.
        * @param value
        */
        public void AddItem(String key, DateTime c) {
            AddItem(key, new PdfDate(c));
        }
        
        /**
        * Sets the value of the collection item.
        * @param value
        */
        public void AddItem(String key, int i) {
            AddItem(key, new PdfNumber(i));
        }
        
        /**
        * Sets the value of the collection item.
        * @param value
        */
        public void AddItem(String key, float f) {
            AddItem(key, new PdfNumber(f));
        }
        
        /**
        * Sets the value of the collection item.
        * @param value
        */
        public void AddItem(String key, double d) {
            AddItem(key, new PdfNumber(d));
        }
        
        /**
        * Adds a prefix for the Collection item.
        * You can only use this method after you have set the value of the item.
        * @param prefix    a prefix
        */
        public void SetPrefix(String key, String prefix) {
            PdfName fieldname = new PdfName(key);
            PdfObject o = Get(fieldname);
            if (o == null)
                throw new InvalidOperationException("You must set a value before adding a prefix.");
            PdfDictionary dict = new PdfDictionary(PdfName.COLLECTIONSUBITEM);
            dict.Put(PdfName.D, o);
            dict.Put(PdfName.P, new PdfString(prefix, PdfObject.TEXT_UNICODE));
            Put(fieldname, dict);
        }
    }
}