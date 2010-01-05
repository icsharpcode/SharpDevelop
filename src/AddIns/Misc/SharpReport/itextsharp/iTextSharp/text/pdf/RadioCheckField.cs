using System;
/*
 * Copyright 2005 by Paulo Soares.
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
    /**
    * Creates a radio or a check field.
    * <p>
    * Example usage:
    * <p>
    * <PRE>
    * Document document = new Document(PageSize.A4, 50, 50, 50, 50);
    * PdfWriter writer = PdfWriter.GetInstance(document, new FileOutputStream("output.pdf"));
    * document.Open();
    * PdfContentByte cb = writer.GetDirectContent();
    * RadioCheckField bt = new RadioCheckField(writer, new Rectangle(100, 100, 200, 200), "radio", "v1");
    * bt.SetCheckType(RadioCheckField.TYPE_CIRCLE);
    * bt.SetBackgroundColor(Color.CYAN);
    * bt.SetBorderStyle(PdfBorderDictionary.STYLE_SOLID);
    * bt.SetBorderColor(Color.red);
    * bt.SetTextColor(Color.yellow);
    * bt.SetBorderWidth(BaseField.BORDER_WIDTH_THICK);
    * bt.SetChecked(false);
    * PdfFormField f1 = bt.GetRadioField();
    * bt.SetOnValue("v2");
    * bt.SetChecked(true);
    * bt.SetBox(new Rectangle(100, 300, 200, 400));
    * PdfFormField f2 = bt.GetRadioField();
    * bt.SetChecked(false);
    * PdfFormField top = bt.GetRadioGroup(true, false);
    * bt.SetOnValue("v3");
    * bt.SetBox(new Rectangle(100, 500, 200, 600));
    * PdfFormField f3 = bt.GetRadioField();
    * top.AddKid(f1);
    * top.AddKid(f2);
    * top.AddKid(f3);
    * writer.AddAnnotation(top);
    * bt = new RadioCheckField(writer, new Rectangle(300, 300, 400, 400), "check1", "Yes");
    * bt.SetCheckType(RadioCheckField.TYPE_CHECK);
    * bt.SetBorderWidth(BaseField.BORDER_WIDTH_THIN);
    * bt.SetBorderColor(Color.black);
    * bt.SetBackgroundColor(Color.white);
    * PdfFormField ck = bt.GetCheckField();
    * writer.AddAnnotation(ck);
    * document.Close();
    * </PRE>
    * @author Paulo Soares (psoares@consiste.pt)
    */
    public class RadioCheckField : BaseField {

        /** A field with the symbol check */
        public const int TYPE_CHECK = 1;
        /** A field with the symbol circle */
        public const int TYPE_CIRCLE = 2;
        /** A field with the symbol cross */
        public const int TYPE_CROSS = 3;
        /** A field with the symbol diamond */
        public const int TYPE_DIAMOND = 4;
        /** A field with the symbol square */
        public const int TYPE_SQUARE = 5;
        /** A field with the symbol star */
        public const int TYPE_STAR = 6;
        
        private static String[] typeChars = {"4", "l", "8", "u", "n", "H"};
        
        /**
        * Holds value of property checkType.
        */
        private int checkType;
        
        /**
        * Holds value of property onValue.
        */
        private String onValue;
        
        /**
        * Holds value of property checked.
        */
        private bool vchecked;
        
        /**
        * Creates a new instance of RadioCheckField
        * @param writer the document <CODE>PdfWriter</CODE>
        * @param box the field location and dimensions
        * @param fieldName the field name. It must not be <CODE>null</CODE>
        * @param onValue the value when the field is checked
        */
        public RadioCheckField(PdfWriter writer, Rectangle box, String fieldName, String onValue) : base(writer, box, fieldName) {
            OnValue = onValue;
            CheckType = TYPE_CIRCLE;
        }
        
        /**
        * Sets the checked symbol. It can be
        * <CODE>TYPE_CHECK</CODE>,
        * <CODE>TYPE_CIRCLE</CODE>,
        * <CODE>TYPE_CROSS</CODE>,
        * <CODE>TYPE_DIAMOND</CODE>,
        * <CODE>TYPE_SQUARE</CODE> and
        * <CODE>TYPE_STAR</CODE>.
        * @param checkType the checked symbol
        */
        public int CheckType {
            get {
                return checkType;
            }
            set {
                checkType = value;
                if (checkType < TYPE_CHECK || checkType > TYPE_STAR)
                    checkType = TYPE_CIRCLE;
                Text = typeChars[checkType - 1];
                Font = BaseFont.CreateFont(BaseFont.ZAPFDINGBATS, BaseFont.WINANSI, false);
            }
        }
        
        /**
        * Sets the value when the field is checked.
        * @param onValue the value when the field is checked
        */
        public string OnValue {
            get {
                return onValue;
            }
            set {
                onValue = value;
            }
        }
        
        /**
        * Sets the state of the field to checked or unchecked.
        * @param checked the state of the field, <CODE>true</CODE> for checked
        * and <CODE>false</CODE> for unchecked
        */
        public bool Checked {
            get {
                return vchecked;
            }
            set {
                vchecked = value;
            }
        }
        /**
        * Gets the field appearance.
        * @param isRadio <CODE>true</CODE> for a radio field and <CODE>false</CODE>
        * for a check field
        * @param on <CODE>true</CODE> for the checked state, <CODE>false</CODE>
        * otherwise
        * @throws IOException on error
        * @throws DocumentException on error
        * @return the appearance
        */    
        public PdfAppearance GetAppearance(bool isRadio, bool on) {
            if (isRadio && checkType == TYPE_CIRCLE)
                return GetAppearanceRadioCircle(on);
            PdfAppearance app = GetBorderAppearance();
            if (!on)
                return app;
            BaseFont ufont = RealFont;
            bool borderExtra = borderStyle == PdfBorderDictionary.STYLE_BEVELED || borderStyle == PdfBorderDictionary.STYLE_INSET;
            float h = box.Height - borderWidth * 2;
            float bw2 = borderWidth;
            if (borderExtra) {
                h -= borderWidth * 2;
                bw2 *= 2;
            }
            float offsetX = (borderExtra ? 2 * borderWidth : borderWidth);
            offsetX = Math.Max(offsetX, 1);
            float offX = Math.Min(bw2, offsetX);
            float wt = box.Width - 2 * offX;
            float ht = box.Height - 2 * offX;
            float fsize = fontSize;
            if (fsize == 0) {
                float bw = ufont.GetWidthPoint(text, 1);
                if (bw == 0)
                    fsize = 12;
                else
                    fsize = wt / bw;
                float nfsize = h / (ufont.GetFontDescriptor(BaseFont.ASCENT, 1));
                fsize = Math.Min(fsize, nfsize);
            }
            app.SaveState();
            app.Rectangle(offX, offX, wt, ht);
            app.Clip();
            app.NewPath();
            if (textColor == null)
                app.ResetGrayFill();
            else
                app.SetColorFill(textColor);
            app.BeginText();
            app.SetFontAndSize(ufont, fsize);
            app.SetTextMatrix((box.Width - ufont.GetWidthPoint(text, fsize)) / 2, 
                (box.Height - ufont.GetAscentPoint(text, fsize)) / 2);
            app.ShowText(text);
            app.EndText();
            app.RestoreState();
            return app;
        }

        /**
        * Gets the special field appearance for the radio circle.
        * @param on <CODE>true</CODE> for the checked state, <CODE>false</CODE>
        * otherwise
        * @return the appearance
        */    
        public PdfAppearance GetAppearanceRadioCircle(bool on) {
            PdfAppearance app = PdfAppearance.CreateAppearance(writer, box.Width, box.Height);
            switch (rotation) {
                case 90:
                    app.SetMatrix(0, 1, -1, 0, box.Height, 0);
                    break;
                case 180:
                    app.SetMatrix(-1, 0, 0, -1, box.Width, box.Height);
                    break;
                case 270:
                    app.SetMatrix(0, -1, 1, 0, 0, box.Width);
                    break;
            }
            Rectangle boxc = new Rectangle(app.BoundingBox);
            float cx = boxc.Width / 2;
            float cy = boxc.Height / 2;
            float r = (Math.Min(boxc.Width, boxc.Height) - borderWidth) / 2;
            if (r <= 0)
                return app;
            if (backgroundColor != null) {
                app.SetColorFill(backgroundColor);
                app.Circle(cx, cy, r + borderWidth / 2);
                app.Fill();
            }
            if (borderWidth > 0 && borderColor != null) {
                app.SetLineWidth(borderWidth);
                app.SetColorStroke(borderColor);
                app.Circle(cx, cy, r);
                app.Stroke();
            }
            if (on) {
                if (textColor == null)
                    app.ResetGrayFill();
                else
                    app.SetColorFill(textColor);
                app.Circle(cx, cy, r / 2);
                app.Fill();
            }
            return app;
        }
        
        /**
        * Gets a radio group. It's composed of the field specific keys, without the widget
        * ones. This field is to be used as a field aggregator with {@link PdfFormField#addKid(PdfFormField) AddKid()}.
        * @param noToggleToOff if <CODE>true</CODE>, exactly one radio button must be selected at all
        * times; clicking the currently selected button has no effect.
        * If <CODE>false</CODE>, clicking
        * the selected button deselects it, leaving no button selected.
        * @param radiosInUnison if <CODE>true</CODE>, a group of radio buttons within a radio button field that
        * use the same value for the on state will turn on and off in unison; that is if
        * one is checked, they are all checked. If <CODE>false</CODE>, the buttons are mutually exclusive
        * (the same behavior as HTML radio buttons)
        * @return the radio group
        */    
        public PdfFormField GetRadioGroup(bool noToggleToOff, bool radiosInUnison) {
            PdfFormField field = PdfFormField.CreateRadioButton(writer, noToggleToOff);
            if (radiosInUnison)
                field.SetFieldFlags(PdfFormField.FF_RADIOSINUNISON);
            field.FieldName = fieldName;
            if ((options & READ_ONLY) != 0)
                field.SetFieldFlags(PdfFormField.FF_READ_ONLY);
            if ((options & REQUIRED) != 0)
                field.SetFieldFlags(PdfFormField.FF_REQUIRED);
            field.ValueAsName = vchecked ? onValue : "Off";
            return field;
        }
        
        /**
        * Gets the radio field. It's only composed of the widget keys and must be used
        * with {@link #getRadioGroup(bool,bool)}.
        * @return the radio field
        * @throws IOException on error
        * @throws DocumentException on error
        */    
        public PdfFormField RadioField {
            get {
                return GetField(true);
            }
        }
        
        /**
        * Gets the check field.
        * @return the check field
        * @throws IOException on error
        * @throws DocumentException on error
        */    
        public PdfFormField CheckField {
            get {
                return GetField(false);
            }
        }
        
        /**
        * Gets a radio or check field.
        * @param isRadio <CODE>true</CODE> to get a radio field, <CODE>false</CODE> to get
        * a check field
        * @throws IOException on error
        * @throws DocumentException on error
        * @return the field
        */    
        protected PdfFormField GetField(bool isRadio) {
            PdfFormField field = null;
            if (isRadio)
                field = PdfFormField.CreateEmpty(writer);
            else
                field = PdfFormField.CreateCheckBox(writer);
            field.SetWidget(box, PdfAnnotation.HIGHLIGHT_INVERT);
            if (!isRadio) {
                field.FieldName = fieldName;
                if ((options & READ_ONLY) != 0)
                    field.SetFieldFlags(PdfFormField.FF_READ_ONLY);
                if ((options & REQUIRED) != 0)
                    field.SetFieldFlags(PdfFormField.FF_REQUIRED);
                field.ValueAsName = vchecked ? onValue : "Off";
            }
            if (text != null)
                field.MKNormalCaption = text;
            if (rotation != 0)
                field.MKRotation = rotation;
            field.BorderStyle = new PdfBorderDictionary(borderWidth, borderStyle, new PdfDashPattern(3));
            PdfAppearance tpon = GetAppearance(isRadio, true);
            PdfAppearance tpoff = GetAppearance(isRadio, false);
            field.SetAppearance(PdfAnnotation.APPEARANCE_NORMAL, onValue, tpon);
            field.SetAppearance(PdfAnnotation.APPEARANCE_NORMAL, "Off", tpoff);
            field.AppearanceState = vchecked ? onValue : "Off";
            PdfAppearance da = (PdfAppearance)tpon.Duplicate;
            da.SetFontAndSize(RealFont, fontSize);
            if (textColor == null)
                da.SetGrayFill(0);
            else
                da.SetColorFill(textColor);
            field.DefaultAppearanceString = da;
            if (borderColor != null)
                field.MKBorderColor = borderColor;
            if (backgroundColor != null)
                field.MKBackgroundColor = backgroundColor;
            switch (visibility) {
                case HIDDEN:
                    field.Flags = PdfAnnotation.FLAGS_PRINT | PdfAnnotation.FLAGS_HIDDEN;
                    break;
                case VISIBLE_BUT_DOES_NOT_PRINT:
                    break;
                case HIDDEN_BUT_PRINTABLE:
                    field.Flags = PdfAnnotation.FLAGS_PRINT | PdfAnnotation.FLAGS_NOVIEW;
                    break;
                default:
                    field.Flags = PdfAnnotation.FLAGS_PRINT;
                    break;
            }
            return field;
        }
    }
}