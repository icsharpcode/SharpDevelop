using System;
using System.Drawing;
using System.Collections;

using iTextSharp.text;

/*
 * Copyright 2002 by Paulo Soares.
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

namespace iTextSharp.text.pdf {

    /** Implements form fields.
     *
     * @author Paulo Soares (psoares@consiste.pt)
     */
    public class PdfFormField : PdfAnnotation {

        public const int FF_READ_ONLY = 1;
        public const int FF_REQUIRED = 2;
        public const int FF_NO_EXPORT = 4;
        public const int FF_NO_TOGGLE_TO_OFF = 16384;
        public const int FF_RADIO = 32768;
        public const int FF_PUSHBUTTON = 65536;
        public const int FF_MULTILINE = 4096;
        public const int FF_PASSWORD = 8192;
        public const int FF_COMBO = 131072;
        public const int FF_EDIT = 262144;
        public const int FF_FILESELECT = 1048576;
        public const int FF_MULTISELECT = 2097152;
        public const int FF_DONOTSPELLCHECK = 4194304;
        public const int FF_DONOTSCROLL = 8388608;
        public const int FF_COMB = 16777216;
        public const int FF_RADIOSINUNISON = 1 << 25;
        public const int Q_LEFT = 0;
        public const int Q_CENTER = 1;
        public const int Q_RIGHT = 2;
        public const int MK_NO_ICON = 0;
        public const int MK_NO_CAPTION = 1;
        public const int MK_CAPTION_BELOW = 2;
        public const int MK_CAPTION_ABOVE = 3;
        public const int MK_CAPTION_RIGHT = 4;
        public const int MK_CAPTION_LEFT = 5;
        public const int MK_CAPTION_OVERLAID = 6;
        public static readonly PdfName IF_SCALE_ALWAYS = PdfName.A;
        public static readonly PdfName IF_SCALE_BIGGER = PdfName.B;
        public static readonly PdfName IF_SCALE_SMALLER = PdfName.S;
        public static readonly PdfName IF_SCALE_NEVER = PdfName.N;
        public static readonly PdfName IF_SCALE_ANAMORPHIC = PdfName.A;
        public static readonly PdfName IF_SCALE_PROPORTIONAL = PdfName.P;
        public const bool MULTILINE = true;
        public const bool SINGLELINE = false;
        public const bool PLAINTEXT = false;
        public const bool PASSWORD = true;
        public static PdfName[] mergeTarget = {PdfName.FONT, PdfName.XOBJECT, PdfName.COLORSPACE, PdfName.PATTERN};
    
        /** Holds value of property parent. */
        internal PdfFormField parent;
    
        internal ArrayList kids;
    
        /**
         * Constructs a new <CODE>PdfAnnotation</CODE> of subtype link (Action).
         */
    
        public PdfFormField(PdfWriter writer, float llx, float lly, float urx, float ury, PdfAction action) : base(writer, llx, lly, urx, ury, action) {
            Put(PdfName.TYPE, PdfName.ANNOT);
            Put(PdfName.SUBTYPE, PdfName.WIDGET);
            annotation = true;
        }

        /** Creates new PdfFormField */
        internal PdfFormField(PdfWriter writer) : base(writer, null) {
            form = true;
            annotation = false;
        }
    
        public void SetWidget(Rectangle rect, PdfName highlight) {
            Put(PdfName.TYPE, PdfName.ANNOT);
            Put(PdfName.SUBTYPE, PdfName.WIDGET);
            Put(PdfName.RECT, new PdfRectangle(rect));
            annotation = true;
            if (highlight != null && !highlight.Equals(HIGHLIGHT_INVERT))
                Put(PdfName.H, highlight);
        }
    
        public static PdfFormField CreateEmpty(PdfWriter writer) {
            PdfFormField field = new PdfFormField(writer);
            return field;
        }
    
        public int Button {
            set {
                Put(PdfName.FT, PdfName.BTN);
                if (value != 0)
                    Put(PdfName.FF, new PdfNumber(value));
            }
        }
        
        protected static PdfFormField CreateButton(PdfWriter writer, int flags) {
            PdfFormField field = new PdfFormField(writer);
            field.Button = flags;
            return field;
        }
        
        public static PdfFormField CreatePushButton(PdfWriter writer) {
            return CreateButton(writer, FF_PUSHBUTTON);
        }

        public static PdfFormField CreateCheckBox(PdfWriter writer) {
            return CreateButton(writer, 0);
        }

        public static PdfFormField CreateRadioButton(PdfWriter writer, bool noToggleToOff) {
            return CreateButton(writer, FF_RADIO + (noToggleToOff ? FF_NO_TOGGLE_TO_OFF : 0));
        }
        
        public static PdfFormField CreateTextField(PdfWriter writer, bool multiline, bool password, int maxLen) {
            PdfFormField field = new PdfFormField(writer);
            field.Put(PdfName.FT, PdfName.TX);
            int flags = (multiline ? FF_MULTILINE : 0);
            flags += (password ? FF_PASSWORD : 0);
            field.Put(PdfName.FF, new PdfNumber(flags));
            if (maxLen > 0)
                field.Put(PdfName.MAXLEN, new PdfNumber(maxLen));
            return field;
        }
        
        protected static PdfFormField CreateChoice(PdfWriter writer, int flags, PdfArray options, int topIndex) {
            PdfFormField field = new PdfFormField(writer);
            field.Put(PdfName.FT, PdfName.CH);
            field.Put(PdfName.FF, new PdfNumber(flags));
            field.Put(PdfName.OPT, options);
            if (topIndex > 0)
                field.Put(PdfName.TI, new PdfNumber(topIndex));
            return field;
        }
        
        public static PdfFormField CreateList(PdfWriter writer, String[] options, int topIndex) {
            return CreateChoice(writer, 0, ProcessOptions(options), topIndex);
        }

        public static PdfFormField CreateList(PdfWriter writer, String[,] options, int topIndex) {
            return CreateChoice(writer, 0, ProcessOptions(options), topIndex);
        }

        public static PdfFormField CreateCombo(PdfWriter writer, bool edit, String[] options, int topIndex) {
            return CreateChoice(writer, FF_COMBO + (edit ? FF_EDIT : 0), ProcessOptions(options), topIndex);
        }
        
        public static PdfFormField CreateCombo(PdfWriter writer, bool edit, String[,] options, int topIndex) {
            return CreateChoice(writer, FF_COMBO + (edit ? FF_EDIT : 0), ProcessOptions(options), topIndex);
        }
        
        protected static PdfArray ProcessOptions(String[] options) {
            PdfArray array = new PdfArray();
            for (int k = 0; k < options.Length; ++k) {
                array.Add(new PdfString(options[k], PdfObject.TEXT_UNICODE));
            }
            return array;
        }
        
        protected static PdfArray ProcessOptions(String[,] options) {
            PdfArray array = new PdfArray();
            for (int k = 0; k < options.GetLength(0); ++k) {
                PdfArray ar2 = new PdfArray(new PdfString(options[k, 0], PdfObject.TEXT_UNICODE));
                ar2.Add(new PdfString(options[k, 1], PdfObject.TEXT_UNICODE));
                array.Add(ar2);
            }
            return array;
        }
        
        public static PdfFormField CreateSignature(PdfWriter writer) {
            PdfFormField field = new PdfFormField(writer);
            field.Put(PdfName.FT, PdfName.SIG);
            return field;
        }
        
        /** Getter for property parent.
        * @return Value of property parent.
        */
        public PdfFormField Parent {
            get {
                return parent;
            }
        }
        
        public void AddKid(PdfFormField field) {
            field.parent = this;
            if (kids == null)
                kids = new ArrayList();
            kids.Add(field);
        }
        
        public ArrayList Kids {
            get {
                return kids;
            }
        }
        
        public int SetFieldFlags(int flags) {
            PdfNumber obj = (PdfNumber)Get(PdfName.FF);
            int old;
            if (obj == null)
                old = 0;
            else
                old = obj.IntValue;
            int v = old | flags;
            Put(PdfName.FF, new PdfNumber(v));
            return old;
        }
        
        public string ValueAsString {
            set {
                Put(PdfName.V, new PdfString(value, PdfObject.TEXT_UNICODE));
            }
        }

        public string ValueAsName {
            set {
                Put(PdfName.V, new PdfName(value));
            }
        }

        public PdfSignature ValueAsSig {
            set {
                Put(PdfName.V, value);
            }
        }

        public string DefaultValueAsString {
            set {
                Put(PdfName.DV, new PdfString(value, PdfObject.TEXT_UNICODE));
            }
        }

        public string DefaultValueAsName {
            set {
                Put(PdfName.DV, new PdfName(value));
            }
        }
        
        public string FieldName {
            set {
                if (value != null)
                    Put(PdfName.T, new PdfString(value, PdfObject.TEXT_UNICODE));
            }
        }
        
        public string UserName {
            set {
                Put(PdfName.TU, new PdfString(value, PdfObject.TEXT_UNICODE));
            }
        }
        
        public string MappingName {
            set {
                Put(PdfName.TM, new PdfString(value, PdfObject.TEXT_UNICODE));
            }
        }
        
        public int Quadding {
            set {
                Put(PdfName.Q, new PdfNumber(value));
            }
        }
        
        internal static void MergeResources(PdfDictionary result, PdfDictionary source, PdfStamperImp writer) {
            PdfDictionary dic = null;
            PdfDictionary res = null;
            PdfName target = null;
            for (int k = 0; k < mergeTarget.Length; ++k) {
                target = mergeTarget[k];
                PdfDictionary pdfDict = (PdfDictionary)PdfReader.GetPdfObject(source.Get(target));
                if ((dic = pdfDict) != null) {
                    if ((res = (PdfDictionary)PdfReader.GetPdfObject(result.Get(target), result)) == null) {
                        res = new PdfDictionary();
                    }
                    res.MergeDifferent(dic);
                    result.Put(target, res);
                    if (writer != null)
                        writer.MarkUsed(res);
                }
            }
        }

        internal static void MergeResources(PdfDictionary result, PdfDictionary source) {
            MergeResources(result, source, null);
        }

        public override void SetUsed() {
            used = true;
            if (parent != null)
                Put(PdfName.PARENT, parent.IndirectReference);
            if (kids != null) {
                PdfArray array = new PdfArray();
                for (int k = 0; k < kids.Count; ++k)
                    array.Add(((PdfFormField)kids[k]).IndirectReference);
                Put(PdfName.KIDS, array);
            }
            if (templates == null)
                return;
            PdfDictionary dic = new PdfDictionary();
            foreach (PdfTemplate template in templates.Keys) {
                MergeResources(dic, (PdfDictionary)template.Resources);
            }
            Put(PdfName.DR, dic);
        }
    }
}