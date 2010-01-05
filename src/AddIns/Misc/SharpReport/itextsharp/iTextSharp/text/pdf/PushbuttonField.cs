using System;
using iTextSharp.text;
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
    * Creates a pushbutton field. It supports all the text and icon alignments.
    * The icon may be an image or a template.
    * <p>
    * Example usage:
    * <p>
    * <PRE>
    * Document document = new Document(PageSize.A4, 50, 50, 50, 50);
    * PdfWriter writer = PdfWriter.GetInstance(document, new FileOutputStream("output.pdf"));
    * document.Open();
    * PdfContentByte cb = writer.GetDirectContent();
    * Image img = Image.GetInstance("image.png");
    * PushbuttonField bt = new PushbuttonField(writer, new Rectangle(100, 100, 200, 200), "Button1");
    * bt.SetText("My Caption");
    * bt.SetFontSize(0);
    * bt.SetImage(img);
    * bt.SetLayout(PushbuttonField.LAYOUT_ICON_TOP_LABEL_BOTTOM);
    * bt.SetBackgroundColor(Color.cyan);
    * bt.SetBorderStyle(PdfBorderDictionary.STYLE_SOLID);
    * bt.SetBorderColor(Color.red);
    * bt.SetBorderWidth(3);
    * PdfFormField ff = bt.GetField();
    * PdfAction ac = PdfAction.CreateSubmitForm("http://www.submit-site.com", null, 0);
    * ff.SetAction(ac);
    * writer.AddAnnotation(ff);
    * document.Close();
    * </PRE>
    * @author Paulo Soares (psoares@consiste.pt)
    */
    public class PushbuttonField : BaseField {
       
        /** A layout option */    
        public const int LAYOUT_LABEL_ONLY = 1;
        /** A layout option */    
        public const int LAYOUT_ICON_ONLY = 2;
        /** A layout option */    
        public const int LAYOUT_ICON_TOP_LABEL_BOTTOM = 3;
        /** A layout option */    
        public const int LAYOUT_LABEL_TOP_ICON_BOTTOM = 4;
        /** A layout option */    
        public const int LAYOUT_ICON_LEFT_LABEL_RIGHT = 5;
        /** A layout option */    
        public const int LAYOUT_LABEL_LEFT_ICON_RIGHT = 6;
        /** A layout option */    
        public const int LAYOUT_LABEL_OVER_ICON = 7;
        /** An icon scaling option */    
        public const int SCALE_ICON_ALWAYS  = 1;
        /** An icon scaling option */    
        public const int SCALE_ICON_NEVER = 2;
        /** An icon scaling option */    
        public const int SCALE_ICON_IS_TOO_BIG = 3;
        /** An icon scaling option */    
        public const int SCALE_ICON_IS_TOO_SMALL = 4;

        /**
        * Holds value of property layout.
        */
        private int layout = LAYOUT_LABEL_ONLY;
        
        /**
        * Holds value of property image.
        */
        private Image image;    
        
        /**
        * Holds value of property template.
        */
        private PdfTemplate template;
        
        /**
        * Holds value of property scaleIcon.
        */
        private int scaleIcon = SCALE_ICON_ALWAYS;
        
        /**
        * Holds value of property proportionalIcon.
        */
        private bool proportionalIcon = true;
        
        /**
        * Holds value of property iconVerticalAdjustment.
        */
        private float iconVerticalAdjustment = 0.5f;
        
        /**
        * Holds value of property iconHorizontalAdjustment.
        */
        private float iconHorizontalAdjustment = 0.5f;
        
        /**
        * Holds value of property iconFitToBounds.
        */
        private bool iconFitToBounds;
        
        private PdfTemplate tp;
        
        /**
        * Creates a new instance of PushbuttonField
        * @param writer the document <CODE>PdfWriter</CODE>
        * @param box the field location and dimensions
        * @param fieldName the field name. If <CODE>null</CODE> only the widget keys
        * will be included in the field allowing it to be used as a kid field.
        */
        public PushbuttonField(PdfWriter writer, Rectangle box, String fieldName) : base(writer, box, fieldName) {
        }
        
        /**
        * Sets the icon and label layout. Possible values are <CODE>LAYOUT_LABEL_ONLY</CODE>,
        * <CODE>LAYOUT_ICON_ONLY</CODE>, <CODE>LAYOUT_ICON_TOP_LABEL_BOTTOM</CODE>,
        * <CODE>LAYOUT_LABEL_TOP_ICON_BOTTOM</CODE>, <CODE>LAYOUT_ICON_LEFT_LABEL_RIGHT</CODE>,
        * <CODE>LAYOUT_LABEL_LEFT_ICON_RIGHT</CODE> and <CODE>LAYOUT_LABEL_OVER_ICON</CODE>.
        * The default is <CODE>LAYOUT_LABEL_ONLY</CODE>.
        * @param layout New value of property layout.
        */
        public int Layout {
            set {
                if (value < LAYOUT_LABEL_ONLY || value > LAYOUT_LABEL_OVER_ICON)
                    throw new ArgumentException("Layout out of bounds.");
                this.layout = value;
            }
            get {
                return layout;
            }
        }
        
        /**
        * Sets the icon as an image.
        * @param image the image
        */
        public Image Image {
            get {
                return this.image;
            }
            set {
                image = value;
                template = null;
            }
        }
        
        /**
        * Sets the icon as a template.
        * @param template the template
        */
        public PdfTemplate Template {
            set {
                this.template = value;
                image = null;
            }
            get {
                return template;
            }
        }
        
        /**
        * Sets the way the icon will be scaled. Possible values are
        * <CODE>SCALE_ICON_ALWAYS</CODE>, <CODE>SCALE_ICON_NEVER</CODE>,
        * <CODE>SCALE_ICON_IS_TOO_BIG</CODE> and <CODE>SCALE_ICON_IS_TOO_SMALL</CODE>.
        * The default is <CODE>SCALE_ICON_ALWAYS</CODE>.
        * @param scaleIcon the way the icon will be scaled
        */
        public int ScaleIcon {
            set {
                if (value < SCALE_ICON_ALWAYS || value > SCALE_ICON_IS_TOO_SMALL)
                    scaleIcon = SCALE_ICON_ALWAYS;
                else
                    scaleIcon = value;
            }
            get {
                return scaleIcon;
            }
        }
        
        /**
        * Sets the way the icon is scaled. If <CODE>true</CODE> the icon is scaled proportionally,
        * if <CODE>false</CODE> the scaling is done anamorphicaly.
        * @param proportionalIcon the way the icon is scaled
        */
        public bool ProportionalIcon {
            get {
                return proportionalIcon;
            }
            set {
                proportionalIcon = value;
            }
        }

        /**
        * A number between 0 and 1 indicating the fraction of leftover space to allocate at the bottom of the icon.
        * A value of 0 positions the icon at the bottom of the annotation rectangle.
        * A value of 0.5 centers it within the rectangle. The default is 0.5.
        * @param iconVerticalAdjustment a number between 0 and 1 indicating the fraction of leftover space to allocate at the bottom of the icon
        */
        public float IconVerticalAdjustment {
            get {
                return iconVerticalAdjustment;
            }
            set {
                iconVerticalAdjustment = value;
                if (iconVerticalAdjustment < 0)
                    iconVerticalAdjustment = 0;
                else if (iconVerticalAdjustment > 1)
                    iconVerticalAdjustment = 1;
            }
        }
        
        /**
        * A number between 0 and 1 indicating the fraction of leftover space to allocate at the left of the icon.
        * A value of 0 positions the icon at the left of the annotation rectangle.
        * A value of 0.5 centers it within the rectangle. The default is 0.5.
        * @param iconHorizontalAdjustment a number between 0 and 1 indicating the fraction of leftover space to allocate at the left of the icon
        */
        public float IconHorizontalAdjustment {
            get {
                return iconHorizontalAdjustment;
            }
            set {
                iconHorizontalAdjustment = value;
                if (iconHorizontalAdjustment < 0)
                    iconHorizontalAdjustment = 0;
                else if (iconHorizontalAdjustment > 1)
                    iconHorizontalAdjustment = 1;
            }
        }
        private float CalculateFontSize(float w, float h) {
            BaseFont ufont = RealFont;
            float fsize = fontSize;
            if (fsize == 0) {
                float bw = ufont.GetWidthPoint(text, 1);
                if (bw == 0)
                    fsize = 12;
                else
                    fsize = w / bw;
                float nfsize = h / (1 - ufont.GetFontDescriptor(BaseFont.DESCENT, 1));
                fsize = Math.Min(fsize, nfsize);
                if (fsize < 4)
                    fsize = 4;
            }
            return fsize;
        }
        
        /**
        * Gets the button appearance.
        * @throws IOException on error
        * @throws DocumentException on error
        * @return the button appearance
        */    
        public PdfAppearance GetAppearance() {
            PdfAppearance app = GetBorderAppearance();
            Rectangle box = new Rectangle(app.BoundingBox);
            if ((text == null || text.Length == 0) && (layout == LAYOUT_LABEL_ONLY || (image == null && template == null && iconReference == null))) {
                return app;
            }
            if (layout == LAYOUT_ICON_ONLY && image == null && template == null && iconReference == null)
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
            tp = null;
            float textX = float.NaN;
            float textY = 0;
            float fsize = fontSize;
            float wt = box.Width - 2 * offX - 2;
            float ht = box.Height - 2 * offX;
            float adj = (iconFitToBounds ? 0 : offX + 1);
            int nlayout = layout;
            if (image == null && template == null && iconReference == null)
                nlayout = LAYOUT_LABEL_ONLY;
            Rectangle iconBox = null;
            while (true) {
                switch (nlayout) {
                    case LAYOUT_LABEL_ONLY:
                    case LAYOUT_LABEL_OVER_ICON:
                        if (text != null && text.Length > 0 && wt > 0 && ht > 0) {
                            fsize = CalculateFontSize(wt, ht);
                            textX = (box.Width - ufont.GetWidthPoint(text, fsize)) / 2;
                            textY = (box.Height - ufont.GetFontDescriptor(BaseFont.ASCENT, fsize)) / 2;
                        }
                        goto case LAYOUT_ICON_ONLY;
                    case LAYOUT_ICON_ONLY:
                        if (nlayout == LAYOUT_LABEL_OVER_ICON || nlayout == LAYOUT_ICON_ONLY)
                            iconBox = new Rectangle(box.Left + adj, box.Bottom + adj, box.Right - adj, box.Top - adj);
                        break;
                    case LAYOUT_ICON_TOP_LABEL_BOTTOM:
                        if (text == null || text.Length == 0 || wt <= 0 || ht <= 0) {
                            nlayout = LAYOUT_ICON_ONLY;
                            continue;
                        }
                        float nht = box.Height * 0.35f - offX;
                        if (nht > 0)
                            fsize = CalculateFontSize(wt, nht);
                        else
                            fsize = 4;
                        textX = (box.Width - ufont.GetWidthPoint(text, fsize)) / 2;
                        textY = offX - ufont.GetFontDescriptor(BaseFont.DESCENT, fsize);
                        iconBox = new Rectangle(box.Left + adj, textY + fsize, box.Right - adj, box.Top - adj);
                        break;
                    case LAYOUT_LABEL_TOP_ICON_BOTTOM:
                        if (text == null || text.Length == 0 || wt <= 0 || ht <= 0) {
                            nlayout = LAYOUT_ICON_ONLY;
                            continue;
                        }
                        nht = box.Height * 0.35f - offX;
                        if (nht > 0)
                            fsize = CalculateFontSize(wt, nht);
                        else
                            fsize = 4;
                        textX = (box.Width - ufont.GetWidthPoint(text, fsize)) / 2;
                        textY = box.Height - offX - fsize;
                        if (textY < offX)
                            textY = offX;
                        iconBox = new Rectangle(box.Left + adj, box.Bottom + adj, box.Right - adj, textY + ufont.GetFontDescriptor(BaseFont.DESCENT, fsize));
                        break;
                    case LAYOUT_LABEL_LEFT_ICON_RIGHT:
                        if (text == null || text.Length == 0 || wt <= 0 || ht <= 0) {
                            nlayout = LAYOUT_ICON_ONLY;
                            continue;
                        }
                        float nw = box.Width * 0.35f - offX;
                        if (nw > 0)
                            fsize = CalculateFontSize(wt, nw);
                        else
                            fsize = 4;
                        if (ufont.GetWidthPoint(text, fsize) >= wt) {
                            nlayout = LAYOUT_LABEL_ONLY;
                            fsize = fontSize;
                            continue;
                        }
                        textX = offX + 1;
                        textY = (box.Height - ufont.GetFontDescriptor(BaseFont.ASCENT, fsize)) / 2;
                        iconBox = new Rectangle(textX + ufont.GetWidthPoint(text, fsize), box.Bottom + adj, box.Right - adj, box.Top - adj);
                        break;
                    case LAYOUT_ICON_LEFT_LABEL_RIGHT:
                        if (text == null || text.Length == 0 || wt <= 0 || ht <= 0) {
                            nlayout = LAYOUT_ICON_ONLY;
                            continue;
                        }
                        nw = box.Width * 0.35f - offX;
                        if (nw > 0)
                            fsize = CalculateFontSize(wt, nw);
                        else
                            fsize = 4;
                        if (ufont.GetWidthPoint(text, fsize) >= wt) {
                            nlayout = LAYOUT_LABEL_ONLY;
                            fsize = fontSize;
                            continue;
                        }
                        textX = box.Width - ufont.GetWidthPoint(text, fsize) - offX - 1;
                        textY = (box.Height - ufont.GetFontDescriptor(BaseFont.ASCENT, fsize)) / 2;
                        iconBox = new Rectangle(box.Left + adj, box.Bottom + adj, textX - 1, box.Top - adj);
                        break;
                }
                break;
            }
            if (textY < box.Bottom + offX)
                textY = box.Bottom + offX;
            if (iconBox != null && (iconBox.Width <= 0 || iconBox.Height <= 0))
                iconBox = null;
            bool haveIcon = false;
            float boundingBoxWidth = 0;
            float boundingBoxHeight = 0;
            PdfArray matrix = null;
            if (iconBox != null) {
                if (image != null) {
                    tp = new PdfTemplate(writer);
                    tp.BoundingBox = new Rectangle(image);
                    writer.AddDirectTemplateSimple(tp, PdfName.FRM);
                    tp.AddImage(image, image.Width, 0, 0, image.Height, 0, 0);
                    haveIcon = true;
                    boundingBoxWidth = tp.BoundingBox.Width;
                    boundingBoxHeight = tp.BoundingBox.Height;
                }
                else if (template != null) {
                    tp = new PdfTemplate(writer);
                    tp.BoundingBox = new Rectangle(template.Width, template.Height);
                    writer.AddDirectTemplateSimple(tp, PdfName.FRM);
                    tp.AddTemplate(template, template.BoundingBox.Left, template.BoundingBox.Bottom);
                    haveIcon = true;
                    boundingBoxWidth = tp.BoundingBox.Width;
                    boundingBoxHeight = tp.BoundingBox.Height;
                }
                else if (iconReference != null) {
                    PdfDictionary dic = (PdfDictionary)PdfReader.GetPdfObject(iconReference);
                    if (dic != null) {
                        Rectangle r2 = PdfReader.GetNormalizedRectangle((PdfArray)PdfReader.GetPdfObject(dic.Get(PdfName.BBOX)));
                        matrix = (PdfArray)PdfReader.GetPdfObject(dic.Get(PdfName.MATRIX));
                        haveIcon = true;
                        boundingBoxWidth = r2.Width;
                        boundingBoxHeight = r2.Height;
                    }
                }
            }
            if (haveIcon) {
                float icx = iconBox.Width / boundingBoxWidth;
                float icy = iconBox.Height / boundingBoxHeight;
                if (proportionalIcon) {
                    switch (scaleIcon) {
                        case SCALE_ICON_IS_TOO_BIG:
                            icx = Math.Min(icx, icy);
                            icx = Math.Min(icx, 1);
                            break;
                        case SCALE_ICON_IS_TOO_SMALL:
                            icx = Math.Min(icx, icy);
                            icx = Math.Max(icx, 1);
                            break;
                        case SCALE_ICON_NEVER:
                            icx = 1;
                            break;
                        default:
                            icx = Math.Min(icx, icy);
                            break;
                    }
                    icy = icx;
                }
                else {
                    switch (scaleIcon) {
                        case SCALE_ICON_IS_TOO_BIG:
                            icx = Math.Min(icx, 1);
                            icy = Math.Min(icy, 1);
                            break;
                        case SCALE_ICON_IS_TOO_SMALL:
                            icx = Math.Max(icx, 1);
                            icy = Math.Max(icy, 1);
                            break;
                        case SCALE_ICON_NEVER:
                            icx = icy = 1;
                            break;
                        default:
                            break;
                    }
                }
                float xpos = iconBox.Left + (iconBox.Width - (boundingBoxWidth * icx)) * iconHorizontalAdjustment;
                float ypos = iconBox.Bottom + (iconBox.Height - (boundingBoxHeight * icy)) * iconVerticalAdjustment;
                app.SaveState();
                app.Rectangle(iconBox.Left, iconBox.Bottom, iconBox.Width, iconBox.Height);
                app.Clip();
                app.NewPath();
                if (tp != null)
                    app.AddTemplate(tp, icx, 0, 0, icy, xpos, ypos);
                else {
                    float cox = 0;
                    float coy = 0;
                    if (matrix != null && matrix.Size == 6) {
                        PdfNumber nm = (PdfNumber)PdfReader.GetPdfObject((PdfObject)matrix.ArrayList[4]);
                        if (nm != null)
                            cox = nm.FloatValue;
                        nm = (PdfNumber)PdfReader.GetPdfObject((PdfObject)matrix.ArrayList[5]);
                        if (nm != null)
                            coy = nm.FloatValue;
                    }
                    app.AddTemplateReference(iconReference, PdfName.FRM, icx, 0, 0, icy, xpos - cox * icx, ypos - coy * icy);
                }
                app.RestoreState();
            }
            if (!float.IsNaN(textX)) {
                app.SaveState();
                app.Rectangle(offX, offX, box.Width - 2 * offX, box.Height - 2 * offX);
                app.Clip();
                app.NewPath();
                if (textColor == null)
                    app.ResetGrayFill();
                else
                    app.SetColorFill(textColor);
                app.BeginText();
                app.SetFontAndSize(ufont, fsize);
                app.SetTextMatrix(textX, textY);
                app.ShowText(text);
                app.EndText();
                app.RestoreState();
            }
            return app;
        }

        /**
        * Gets the pushbutton field.
        * @throws IOException on error
        * @throws DocumentException on error
        * @return the pushbutton field
        */    
        public PdfFormField Field {
            get {
                PdfFormField field = PdfFormField.CreatePushButton(writer);
                field.SetWidget(box, PdfAnnotation.HIGHLIGHT_INVERT);
                if (fieldName != null) {
                    field.FieldName = fieldName;
                    if ((options & READ_ONLY) != 0)
                        field.SetFieldFlags(PdfFormField.FF_READ_ONLY);
                    if ((options & REQUIRED) != 0)
                        field.SetFieldFlags(PdfFormField.FF_REQUIRED);
                }
                if (text != null)
                    field.MKNormalCaption = text;
                if (rotation != 0)
                    field.MKRotation = rotation;
                field.BorderStyle = new PdfBorderDictionary(borderWidth, borderStyle, new PdfDashPattern(3));
                PdfAppearance tpa = GetAppearance();
                field.SetAppearance(PdfAnnotation.APPEARANCE_NORMAL, tpa);
                PdfAppearance da = (PdfAppearance)tpa.Duplicate;
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
                if (tp != null)
                    field.MKNormalIcon = tp;
                field.MKTextPosition = layout - 1;
                PdfName scale = PdfName.A;
                if (scaleIcon == SCALE_ICON_IS_TOO_BIG)
                    scale = PdfName.B;
                else if (scaleIcon == SCALE_ICON_IS_TOO_SMALL)
                    scale = PdfName.S;
                else if (scaleIcon == SCALE_ICON_NEVER)
                    scale = PdfName.N;
                field.SetMKIconFit(scale, proportionalIcon ? PdfName.P : PdfName.A, iconHorizontalAdjustment,
                    iconVerticalAdjustment, iconFitToBounds);
                return field;
            }
        }
        
        /**
        * If <CODE>true</CODE> the icon will be scaled to fit fully within the bounds of the annotation,
        * if <CODE>false</CODE> the border width will be taken into account. The default
        * is <CODE>false</CODE>.
        * @param iconFitToBounds if <CODE>true</CODE> the icon will be scaled to fit fully within the bounds of the annotation,
        * if <CODE>false</CODE> the border width will be taken into account
        */
        public bool IconFitToBounds {
            get {
                return iconFitToBounds;
            }
            set {
                iconFitToBounds = value;
            }
        }
        /**
        * Holds value of property iconReference.
        */
        private PRIndirectReference iconReference;

        /**
        * Sets the reference to an existing icon.
        * @param iconReference the reference to an existing icon
        */
        public PRIndirectReference IconReference {
            get {
                return iconReference;
            }
            set {
                iconReference = value;
            }
        }
    }
}