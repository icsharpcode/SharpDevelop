using System;
using System.Collections;
using iTextSharp.text.pdf;
using iTextSharp.text;
/*
 * $Id: PdfAnnotationsImp.cs,v 1.1 2007/02/09 15:34:40 psoares33 Exp $
 *
 * Copyright 2006 Bruno Lowagie
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
 * the Initial Developer are Copyright (C) 1999, 2000, 2001, 2002 by Bruno Lowagie.
 * All Rights Reserved.
 * Co-Developer of the code is Paulo Soares. Portions created by the Co-Developer
 * are Copyright (C) 2000, 2001, 2002 by Paulo Soares. All Rights Reserved.
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

namespace iTextSharp.text.pdf.intern {

    public class PdfAnnotationsImp {

        /**
        * This is the AcroForm object for the complete document.
        */
        protected internal PdfAcroForm acroForm;

        /**
        * This is the array containing the references to annotations
        * that were added to the document.
        */
        protected internal ArrayList annotations;
        
        /**
        * This is an array containg references to some delayed annotations
        * (that were added for a page that doesn't exist yet).
        */
        protected internal ArrayList delayedAnnotations = new ArrayList();
        
        
        public PdfAnnotationsImp(PdfWriter writer) {
            acroForm = new PdfAcroForm(writer);
        }
        
        /**
        * Checks if the AcroForm is valid.
        */
        public bool HasValidAcroForm() {
            return acroForm.IsValid();
        }
        
        /**
        * Gets the AcroForm object.
        * @return the PdfAcroform object of the PdfDocument
        */
        public PdfAcroForm AcroForm {
            get {
                return acroForm;
            }
        }
        
        public int SigFlags {
            set {
                acroForm.SigFlags = value;
            }
        }
        
        public void AddCalculationOrder(PdfFormField formField) {
            acroForm.AddCalculationOrder(formField);
        }
        
        public void AddAnnotation(PdfAnnotation annot) {
            if (annot.IsForm()) {
                PdfFormField field = (PdfFormField)annot;
                if (field.Parent == null)
                    AddFormFieldRaw(field);
            }
            else
                annotations.Add(annot);
        }
        
        public void AddPlainAnnotation(PdfAnnotation annot) {
            annotations.Add(annot);
        }
        
        void AddFormFieldRaw(PdfFormField field) {
            annotations.Add(field);
            ArrayList kids = field.Kids;
            if (kids != null) {
                for (int k = 0; k < kids.Count; ++k)
                    AddFormFieldRaw((PdfFormField)kids[k]);
            }
        }
        
        public bool HasUnusedAnnotations() {
            return annotations.Count > 0;
        }

        public void ResetAnnotations() {
            annotations = delayedAnnotations;
            delayedAnnotations = new ArrayList();
        }
        
        public PdfArray RotateAnnotations(PdfWriter writer, Rectangle pageSize) {
            PdfArray array = new PdfArray();
            int rotation = pageSize.Rotation % 360;
            int currentPage = writer.CurrentPageNumber;
            for (int k = 0; k < annotations.Count; ++k) {
                PdfAnnotation dic = (PdfAnnotation)annotations[k];
                int page = dic.PlaceInPage;
                if (page > currentPage) {
                    delayedAnnotations.Add(dic);
                    continue;
                }
                if (dic.IsForm()) {
                    if (!dic.IsUsed()) {
                        Hashtable templates = dic.Templates;
                        if (templates != null)
                            acroForm.AddFieldTemplates(templates);
                    }
                    PdfFormField field = (PdfFormField)dic;
                    if (field.Parent == null)
                        acroForm.AddDocumentField(field.IndirectReference);
                }
                if (dic.IsAnnotation()) {
                    array.Add(dic.IndirectReference);
                    if (!dic.IsUsed()) {
                        PdfRectangle rect = (PdfRectangle)dic.Get(PdfName.RECT);
                        if (rect != null) {
                            switch (rotation) {
                                case 90:
                                    dic.Put(PdfName.RECT, new PdfRectangle(
                                            pageSize.Top - rect.Bottom,
                                            rect.Left,
                                            pageSize.Top - rect.Top,
                                            rect.Right));
                                    break;
                                case 180:
                                    dic.Put(PdfName.RECT, new PdfRectangle(
                                            pageSize.Right - rect.Left,
                                            pageSize.Top - rect.Bottom,
                                            pageSize.Right - rect.Right,
                                            pageSize.Top - rect.Top));
                                    break;
                                case 270:
                                    dic.Put(PdfName.RECT, new PdfRectangle(
                                            rect.Bottom,
                                            pageSize.Right - rect.Left,
                                            rect.Top,
                                            pageSize.Right - rect.Right));
                                    break;
                            }
                        }
                    }
                }
                if (!dic.IsUsed()) {
                    dic.SetUsed();
                    writer.AddToBody(dic, dic.IndirectReference);
                }
            }
            return array;
        }
        
        public static PdfAnnotation ConvertAnnotation(PdfWriter writer, Annotation annot, Rectangle defaultRect) {
            switch (annot.AnnotationType) {
                case Annotation.URL_NET:
                    return new PdfAnnotation(writer, annot.GetLlx(), annot.GetLly(), annot.GetUrx(), annot.GetUry(), new PdfAction((Uri) annot.Attributes[Annotation.URL]));
                case Annotation.URL_AS_STRING:
                    return new PdfAnnotation(writer, annot.GetLlx(), annot.GetLly(), annot.GetUrx(), annot.GetUry(), new PdfAction((String) annot.Attributes[Annotation.FILE]));
                case Annotation.FILE_DEST:
                    return new PdfAnnotation(writer, annot.GetLlx(), annot.GetLly(), annot.GetUrx(), annot.GetUry(), new PdfAction((String) annot.Attributes[Annotation.FILE], (String) annot.Attributes[Annotation.DESTINATION]));
                case Annotation.SCREEN:
                    bool[] sparams = (bool[])annot.Attributes[Annotation.PARAMETERS];
                    String fname = (String) annot.Attributes[Annotation.FILE];
                    String mimetype = (String) annot.Attributes[Annotation.MIMETYPE];
                    PdfFileSpecification fs;
                    if (sparams[0])
                        fs = PdfFileSpecification.FileEmbedded(writer, fname, fname, null);
                    else
                        fs = PdfFileSpecification.FileExtern(writer, fname);
                    PdfAnnotation ann = PdfAnnotation.CreateScreen(writer, new Rectangle(annot.GetLlx(), annot.GetLly(), annot.GetUrx(), annot.GetUry()),
                            fname, fs, mimetype, sparams[1]);
                    return ann;
                case Annotation.FILE_PAGE:
                    return new PdfAnnotation(writer, annot.GetLlx(), annot.GetLly(), annot.GetUrx(), annot.GetUry(), new PdfAction((String) annot.Attributes[Annotation.FILE], (int)annot.Attributes[Annotation.PAGE]));
                case Annotation.NAMED_DEST:
                    return new PdfAnnotation(writer, annot.GetLlx(), annot.GetLly(), annot.GetUrx(), annot.GetUry(), new PdfAction((int) annot.Attributes[Annotation.NAMED]));
                case Annotation.LAUNCH:
                    return new PdfAnnotation(writer, annot.GetLlx(), annot.GetLly(), annot.GetUrx(), annot.GetUry(), new PdfAction((String) annot.Attributes[Annotation.APPLICATION],(String) annot.Attributes[Annotation.PARAMETERS],(String) annot.Attributes[Annotation.OPERATION],(String) annot.Attributes[Annotation.DEFAULTDIR]));
                default:
                    return new PdfAnnotation(writer, defaultRect.Left, defaultRect.Bottom, defaultRect.Right, defaultRect.Top, new PdfString(annot.Title, PdfObject.TEXT_UNICODE), new PdfString(annot.Content, PdfObject.TEXT_UNICODE));
            }
        }
    }
}