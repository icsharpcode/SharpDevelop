using System;
using System.IO;
using System.Collections;
using iTextSharp.text;
using iTextSharp.text.pdf;

/*
 * Copyright 2005 by Bruno Lowagie
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
 * the Initial Developer are Copyright (C) 1999-2005 by Bruno Lowagie.
 * All Rights Reserved.
 * Co-Developer of the code is Paulo Soares. Portions created by the Co-Developer
 * are Copyright (C) 2000-2005 by Paulo Soares. All Rights Reserved.
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
namespace iTextSharp.text.pdf.events {

    /**
    * Class for an index.
    * 
    * @author Michael Niedermair
    */
    public class FieldPositioningEvents : PdfPageEventHelper, IPdfPCellEvent {

        /**
        * Keeps a map with fields that are to be positioned in inGenericTag.
        */
        protected Hashtable genericChunkFields = new Hashtable();

        /**
        * Keeps the form field that is to be positioned in a cellLayout event.
        */
        protected PdfFormField cellField = null;
        
        /**
        * The PdfWriter to use when a field has to added in a cell event. 
        */
        protected PdfWriter fieldWriter = null;
        /**
        * The PdfFormField that is the parent of the field added in a cell event. 
        */
        protected PdfFormField parent = null;
        
        /** Creates a new event. This constructor will be used if you need to position fields with Chunk objects. */
        public FieldPositioningEvents() {}
        
        /** Some extra padding that will be taken into account when defining the widget. */
        public float padding;
        
        /**
        * Add a PdfFormField that has to be tied to a generic Chunk.
        */
        public void AddField(String text, PdfFormField field) {
            genericChunkFields[text] = field;
        }
        
        /** Creates a new event. This constructor will be used if you need to position fields with a Cell Event. */
        public FieldPositioningEvents(PdfWriter writer, PdfFormField field) {
            this.cellField = field;
            this.fieldWriter = writer;
        }  
        
        /** Creates a new event. This constructor will be used if you need to position fields with a Cell Event. */
        public FieldPositioningEvents(PdfFormField parent, PdfFormField field) {
            this.cellField = field;
            this.parent = parent;
        }
        
        /** Creates a new event. This constructor will be used if you need to position fields with a Cell Event. 
        * @throws DocumentException
        * @throws IOException*/
        public FieldPositioningEvents(PdfWriter writer, String text) {
            this.fieldWriter = writer;
            TextField tf = new TextField(writer, new Rectangle(0, 0), text);
            tf.FontSize = 14;
            cellField = tf.GetTextField();
        }   
            
        /** Creates a new event. This constructor will be used if you need to position fields with a Cell Event. 
        * @throws DocumentException
        * @throws IOException*/
        public FieldPositioningEvents(PdfWriter writer, PdfFormField parent, String text) {
            this.parent = parent;
            TextField tf = new TextField(writer, new Rectangle(0, 0), text);
            tf.FontSize = 14;
            cellField = tf.GetTextField();
        }  

        /**
        * @param padding The padding to set.
        */
        public float Padding {
            set {
                padding = value;
            }
            get {
                return padding;
            }
        }
        
        /**
        * @param parent The parent to set.
        */
        public PdfFormField Parent {
            set {
                parent = value;
            }
            get {
                return parent;
            }
        }

        /**
        * @see com.lowagie.text.pdf.PdfPageEvent#onGenericTag(com.lowagie.text.pdf.PdfWriter, com.lowagie.text.Document, com.lowagie.text.Rectangle, java.lang.String)
        */
        public override void OnGenericTag(PdfWriter writer, Document document,
                Rectangle rect, String text) {
            rect.Bottom = rect.Bottom - 3;
            PdfFormField field = (PdfFormField) genericChunkFields[text];
            if (field == null) {
                TextField tf = new TextField(writer, new Rectangle(rect.GetLeft(padding), rect.GetBottom(padding), rect.GetRight(padding), rect.GetTop(padding)), text);
                tf.FontSize = 14;
                field = tf.GetTextField();
            }
            else {
                field.Put(PdfName.RECT,  new PdfRectangle(rect.GetLeft(padding), rect.GetBottom(padding), rect.GetRight(padding), rect.GetTop(padding)));
            }
            if (parent == null)
                writer.AddAnnotation(field);
            else
                parent.AddKid(field);
        }

        /**
        * @see com.lowagie.text.pdf.PdfPCellEvent#cellLayout(com.lowagie.text.pdf.PdfPCell, com.lowagie.text.Rectangle, com.lowagie.text.pdf.PdfContentByte[])
        */
        public void CellLayout(PdfPCell cell, Rectangle rect, PdfContentByte[] canvases) {
            if (cellField == null || (fieldWriter == null && parent == null)) throw new ArgumentException("You have used the wrong constructor for this FieldPositioningEvents class.");
            cellField.Put(PdfName.RECT, new PdfRectangle(rect.GetLeft(padding), rect.GetBottom(padding), rect.GetRight(padding), rect.GetTop(padding)));
            if (parent == null)
                fieldWriter.AddAnnotation(cellField);
            else
                parent.AddKid(cellField);
        }
    }
}