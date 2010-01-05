using System;
using System.Collections;
using System.Text;
/*
 * Copyright 2003-2005 by Paulo Soares.
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
    /** Supports text, combo and list fields generating the correct appearances.
    * All the option in the Acrobat GUI are supported in an easy to use API.
    * @author Paulo Soares (psoares@consiste.pt)
    */
    public class TextField : BaseField {
        
        /** Holds value of property defaultText. */
        private String defaultText;
        
        /** Holds value of property choices. */
        private String[] choices;
        
        /** Holds value of property choiceExports. */
        private String[] choiceExports;
        
        /** Holds value of property choiceSelection. */
        private int choiceSelection;
        
        private int topFirst;

        private float extraMarginLeft;
        private float extraMarginTop;

        /** Creates a new <CODE>TextField</CODE>.
        * @param writer the document <CODE>PdfWriter</CODE>
        * @param box the field location and dimensions
        * @param fieldName the field name. If <CODE>null</CODE> only the widget keys
        * will be included in the field allowing it to be used as a kid field.
        */
        public TextField(PdfWriter writer, Rectangle box, String fieldName) : base(writer, box, fieldName) {
        }
        
        private static bool CheckRTL(String text) {
            if (text == null || text.Length == 0)
                return false;
            char[] cc = text.ToCharArray();
            for (int k = 0; k < cc.Length; ++k) {
                int c = (int)cc[k];
                if (c >= 0x590 && c < 0x0780)
                    return true;
            }
            return false;
        }
        
        private static void ChangeFontSize(Phrase p, float size) {
            foreach (Chunk ck in p) {
                ck.Font.Size = size;
            }
        }
        
        private Phrase ComposePhrase(String text, BaseFont ufont, Color color, float fontSize) {
            Phrase phrase = null;
            if (extensionFont == null && (substitutionFonts == null || substitutionFonts.Count == 0))
                phrase = new Phrase(new Chunk(text, new Font(ufont, fontSize, 0, color)));
            else {
                FontSelector fs = new FontSelector();
                fs.AddFont(new Font(ufont, fontSize, 0, color));
                if (extensionFont != null)
                    fs.AddFont(new Font(extensionFont, fontSize, 0, color));
                if (substitutionFonts != null) {
                    foreach (BaseFont bf in substitutionFonts) {
                        fs.AddFont(new Font(bf, fontSize, 0, color));
                    }
                }
                phrase = fs.Process(text);
            }
            return phrase;
        }
        
        private static String RemoveCRLF(String text) {
            if (text.IndexOf('\n') >= 0 || text.IndexOf('\r') >= 0) {
                char[] p = text.ToCharArray();
                StringBuilder sb = new StringBuilder(p.Length);
                for (int k = 0; k < p.Length; ++k) {
                    char c = p[k];
                    if (c == '\n')
                        sb.Append(' ');
                    else if (c == '\r') {
                        sb.Append(' ');
                        if (k < p.Length - 1 && p[k + 1] == '\n')
                            ++k;
                    }
                    else
                        sb.Append(c);
                }
                return sb.ToString();
            }
            return text;
        }
        
        /**
        * Gets the appearance for this TextField.
        * @return the appearance object for this TextField
        * @throws IOException
        * @throws DocumentException
        */
        public PdfAppearance GetAppearance() {
            PdfAppearance app = GetBorderAppearance();
            app.BeginVariableText();
            if (text == null || text.Length == 0) {
                app.EndVariableText();
                return app;
            }
            BaseFont ufont = RealFont;
            bool borderExtra = borderStyle == PdfBorderDictionary.STYLE_BEVELED || borderStyle == PdfBorderDictionary.STYLE_INSET;
            float h = box.Height - borderWidth * 2;
            float bw2 = borderWidth;
            if (borderExtra) {
                h -= borderWidth * 2;
                bw2 *= 2;
            }
            h -= extraMarginTop;
            float offsetX = (borderExtra ? 2 * borderWidth : borderWidth);
            offsetX = Math.Max(offsetX, 1);
            float offX = Math.Min(bw2, offsetX);
            app.SaveState();
            app.Rectangle(offX, offX, box.Width - 2 * offX, box.Height - 2 * offX);
            app.Clip();
            app.NewPath();
            Color fcolor = (textColor == null) ? GrayColor.GRAYBLACK : textColor;
            String ptext = text; //fixed by Kazuya Ujihara (ujihara.jp)
            if ((options & PASSWORD) != 0) {
                ptext = new String('*', ptext.Length);
            }
            int rtl = CheckRTL(ptext) ? PdfWriter.RUN_DIRECTION_LTR : PdfWriter.RUN_DIRECTION_NO_BIDI;
            if ((options & MULTILINE) == 0) {
                ptext = RemoveCRLF(text);
            }
            Phrase phrase = ComposePhrase(ptext, ufont, fcolor, fontSize);
            if ((options & MULTILINE) != 0) {
                float usize = fontSize;
                float width = box.Width - 4 * offsetX - extraMarginLeft;
                float factor = ufont.GetFontDescriptor(BaseFont.BBOXURY, 1) - ufont.GetFontDescriptor(BaseFont.BBOXLLY, 1);
                ColumnText ct = new ColumnText(null);
                if (usize == 0) {
                    usize = h / factor;
                    if (usize > 4) {
                        if (usize > 12)
                            usize = 12;
                        float step = Math.Max((usize - 4) / 10, 0.2f);
                        ct.SetSimpleColumn(0, -h, width, 0);
                        ct.Alignment = alignment;
                        ct.RunDirection = rtl;
                        for (; usize > 4; usize -= step) {
                            ct.YLine = 0;
                            ChangeFontSize(phrase, usize);
                            ct.SetText(phrase);
                            ct.Leading = factor * usize;
                            int status = ct.Go(true);
                            if ((status & ColumnText.NO_MORE_COLUMN) == 0)
                                break;
                        }
                    }
                    if (usize < 4) {
                        usize = 4;
                    }
                }
                ChangeFontSize(phrase, usize);
                ct.Canvas = app;
                float leading = usize * factor;
                float offsetY = offsetX + h - ufont.GetFontDescriptor(BaseFont.BBOXURY, usize);
                ct.SetSimpleColumn(extraMarginLeft + 2 * offsetX, -20000, box.Width - 2 * offsetX, offsetY + leading);
                ct.Leading = leading;
                ct.Alignment = alignment;
                ct.RunDirection = rtl;
                ct.SetText(phrase);
                ct.Go();
            }
            else {
                float usize = fontSize;
                if (usize == 0) {
                    float maxCalculatedSize = h / (ufont.GetFontDescriptor(BaseFont.BBOXURX, 1) - ufont.GetFontDescriptor(BaseFont.BBOXLLY, 1));
                    ChangeFontSize(phrase, 1);
                    float wd = ColumnText.GetWidth(phrase, rtl, 0);
                    if (wd == 0)
                        usize = maxCalculatedSize;
                    else
                        usize = (box.Width - extraMarginLeft - 4 * offsetX) / wd;
                    if (usize > maxCalculatedSize)
                        usize = maxCalculatedSize;
                    if (usize < 4)
                        usize = 4;
                }
                ChangeFontSize(phrase, usize);
                float offsetY = offX + ((box.Height - 2*offX) - ufont.GetFontDescriptor(BaseFont.ASCENT, usize)) / 2;
                if (offsetY < offX)
                    offsetY = offX;
                if (offsetY - offX < -ufont.GetFontDescriptor(BaseFont.DESCENT, usize)) {
                    float ny = -ufont.GetFontDescriptor(BaseFont.DESCENT, usize) + offX;
                    float dy = box.Height - offX - ufont.GetFontDescriptor(BaseFont.ASCENT, usize);
                    offsetY = Math.Min(ny, Math.Max(offsetY, dy));
                }
                if ((options & COMB) != 0 && maxCharacterLength > 0) {
                    int textLen = Math.Min(maxCharacterLength, ptext.Length);
                    int position = 0;
                    if (alignment == Element.ALIGN_RIGHT) {
                        position = maxCharacterLength - textLen;
                    }
                    else if (alignment == Element.ALIGN_CENTER) {
                        position = (maxCharacterLength - textLen) / 2;
                    }
                    float step = (box.Width - extraMarginLeft) / maxCharacterLength;
                    float start = step / 2 + position * step;
                    if (textColor == null)
                        app.SetGrayFill(0);
                    else
                        app.SetColorFill(textColor);
                    app.BeginText();
                    foreach (Chunk ck in phrase) {
                        BaseFont bf = ck.Font.BaseFont;
                        app.SetFontAndSize(bf, usize);
                        StringBuilder sb = ck.Append("");
                        for (int j = 0; j < sb.Length; ++j) {
                            String c = sb.ToString(j, 1);
                            float wd = bf.GetWidthPoint(c, usize);
                            app.SetTextMatrix(extraMarginLeft + start - wd / 2, offsetY - extraMarginTop);
                            app.ShowText(c);
                            start += step;
                        }
                    }
                    app.EndText();
                }
                else {
                    if (alignment == Element.ALIGN_RIGHT) {
                        ColumnText.ShowTextAligned(app, Element.ALIGN_RIGHT, phrase, extraMarginLeft + box.Width - 2 * offsetX, offsetY - extraMarginTop, 0, rtl, 0);
                    }
                    else if (alignment == Element.ALIGN_CENTER) {
                        ColumnText.ShowTextAligned(app, Element.ALIGN_CENTER, phrase, extraMarginLeft + box.Width / 2, offsetY - extraMarginTop, 0, rtl, 0);
                    }
                    else
                        ColumnText.ShowTextAligned(app, Element.ALIGN_LEFT, phrase, extraMarginLeft + 2 * offsetX, offsetY - extraMarginTop, 0, rtl, 0);
                }
            }
            app.RestoreState();
            app.EndVariableText();
            return app;
        }

        internal PdfAppearance GetListAppearance() {
            PdfAppearance app = GetBorderAppearance();
            app.BeginVariableText();
            if (choices == null || choices.Length == 0) {
                app.EndVariableText();
                return app;
            }
            int topChoice = choiceSelection;
            if (topChoice >= choices.Length) {
                topChoice = choices.Length - 1;
            }
            if (topChoice < 0)
                topChoice = 0;
            BaseFont ufont = RealFont;
            float usize = fontSize;
            if (usize == 0)
                usize = 12;
            bool borderExtra = borderStyle == PdfBorderDictionary.STYLE_BEVELED || borderStyle == PdfBorderDictionary.STYLE_INSET;
            float h = box.Height - borderWidth * 2;
            if (borderExtra)
                h -= borderWidth * 2;
            float offsetX = (borderExtra ? 2 * borderWidth : borderWidth);
            float leading = ufont.GetFontDescriptor(BaseFont.BBOXURY, usize) - ufont.GetFontDescriptor(BaseFont.BBOXLLY, usize);
            int maxFit = (int)(h / leading) + 1;
            int first = 0;
            int last = 0;
            last = topChoice + maxFit / 2 + 1;
            first = last - maxFit;
            if (first < 0) {
                last += first;
                first = 0;
            }
    //        first = topChoice;
            last = first + maxFit;
            if (last > choices.Length)
                last = choices.Length;
            topFirst = first;
            app.SaveState();
            app.Rectangle(offsetX, offsetX, box.Width - 2 * offsetX, box.Height - 2 * offsetX);
            app.Clip();
            app.NewPath();
            Color fcolor = (textColor == null) ? GrayColor.GRAYBLACK : textColor;
            app.SetColorFill(new Color(10, 36, 106));
            app.Rectangle(offsetX, offsetX + h - (topChoice - first + 1) * leading, box.Width - 2 * offsetX, leading);
            app.Fill();
            float xp = offsetX * 2;
            float yp = offsetX + h - ufont.GetFontDescriptor(BaseFont.BBOXURY, usize);
            for (int idx = first; idx < last; ++idx, yp -= leading) {
                String ptext = choices[idx];
                int rtl = CheckRTL(ptext) ? PdfWriter.RUN_DIRECTION_LTR : PdfWriter.RUN_DIRECTION_NO_BIDI;
                ptext = RemoveCRLF(ptext);
                Phrase phrase = ComposePhrase(ptext, ufont, (idx == topChoice) ? GrayColor.GRAYWHITE : fcolor, usize);
                ColumnText.ShowTextAligned(app, Element.ALIGN_LEFT, phrase, xp, yp, 0, rtl, 0);
            }
            app.RestoreState();
            app.EndVariableText();
            return app;
        }

        /** Gets a new text field.
        * @throws IOException on error
        * @throws DocumentException on error
        * @return a new text field
        */    
        public PdfFormField GetTextField() {
            if (maxCharacterLength <= 0)
                options &= ~COMB;
            if ((options & COMB) != 0)
                options &= ~MULTILINE;
            PdfFormField field = PdfFormField.CreateTextField(writer, false, false, maxCharacterLength);
            field.SetWidget(box, PdfAnnotation.HIGHLIGHT_INVERT);
            switch (alignment) {
                case Element.ALIGN_CENTER:
                    field.Quadding = PdfFormField.Q_CENTER;
                    break;
                case Element.ALIGN_RIGHT:
                    field.Quadding = PdfFormField.Q_RIGHT;
                    break;
            }
            if (rotation != 0)
                field.MKRotation = rotation;
            if (fieldName != null) {
                field.FieldName = fieldName;
                if ((options & REQUIRED) == 0 && !"".Equals(text))
                    field.ValueAsString = text;
                if (defaultText != null)
                    field.DefaultValueAsString = defaultText;
                if ((options & READ_ONLY) != 0)
                    field.SetFieldFlags(PdfFormField.FF_READ_ONLY);
                if ((options & REQUIRED) != 0)
                    field.SetFieldFlags(PdfFormField.FF_REQUIRED);
                if ((options & MULTILINE) != 0)
                    field.SetFieldFlags(PdfFormField.FF_MULTILINE);
                if ((options & DO_NOT_SCROLL) != 0)
                    field.SetFieldFlags(PdfFormField.FF_DONOTSCROLL);
                if ((options & PASSWORD) != 0)
                    field.SetFieldFlags(PdfFormField.FF_PASSWORD);
                if ((options & FILE_SELECTION) != 0)
                    field.SetFieldFlags(PdfFormField.FF_FILESELECT);
                if ((options & DO_NOT_SPELL_CHECK) != 0)
                    field.SetFieldFlags(PdfFormField.FF_DONOTSPELLCHECK);
                if ((options & COMB) != 0)
                    field.SetFieldFlags(PdfFormField.FF_COMB);
            }
            field.BorderStyle = new PdfBorderDictionary(borderWidth, borderStyle, new PdfDashPattern(3));
            PdfAppearance tp = GetAppearance();
            field.SetAppearance(PdfAnnotation.APPEARANCE_NORMAL, tp);
            PdfAppearance da = (PdfAppearance)tp.Duplicate;
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
        
        /** Gets a new combo field.
        * @throws IOException on error
        * @throws DocumentException on error
        * @return a new combo field
        */    
        public PdfFormField GetComboField() {
            return GetChoiceField(false);
        }
        
        /** Gets a new list field.
        * @throws IOException on error
        * @throws DocumentException on error
        * @return a new list field
        */    
        public PdfFormField GetListField() {
            return GetChoiceField(true);
        }

        protected PdfFormField GetChoiceField(bool isList) {
            options &= (~MULTILINE) & (~COMB);
            String[] uchoices = choices;
            if (uchoices == null)
                uchoices = new String[0];
            int topChoice = choiceSelection;
            if (topChoice >= uchoices.Length)
                topChoice = uchoices.Length - 1;
            if (text == null) text = ""; //fixed by Kazuya Ujihara (ujihara.jp)
            if (topChoice >= 0)
                text = uchoices[topChoice];
            if (topChoice < 0)
                topChoice = 0;
            PdfFormField field = null;
            String[,] mix = null;
            if (choiceExports == null) {
                if (isList)
                    field = PdfFormField.CreateList(writer, uchoices, topChoice);
                else
                    field = PdfFormField.CreateCombo(writer, (options & EDIT) != 0, uchoices, topChoice);
            }
            else {
                mix = new String[uchoices.Length, 2];
                for (int k = 0; k < mix.GetLength(0); ++k)
                    mix[k, 0] = mix[k, 1] = uchoices[k];
                int top = Math.Min(uchoices.Length, choiceExports.Length);
                for (int k = 0; k < top; ++k) {
                    if (choiceExports[k] != null)
                        mix[k, 0] = choiceExports[k];
                }
                if (isList)
                    field = PdfFormField.CreateList(writer, mix, topChoice);
                else
                    field = PdfFormField.CreateCombo(writer, (options & EDIT) != 0, mix, topChoice);
            }
            field.SetWidget(box, PdfAnnotation.HIGHLIGHT_INVERT);
            if (rotation != 0)
                field.MKRotation = rotation;
            if (fieldName != null) {
                field.FieldName = fieldName;
                if (uchoices.Length > 0) {
                    if (mix != null) {
                        field.ValueAsString = mix[topChoice, 0];
                        field.DefaultValueAsString = mix[topChoice, 0];
                    }
                    else {
                        field.ValueAsString = text;
                        field.DefaultValueAsString = text;
                    }
                }
                if ((options & READ_ONLY) != 0)
                    field.SetFieldFlags(PdfFormField.FF_READ_ONLY);
                if ((options & REQUIRED) != 0)
                    field.SetFieldFlags(PdfFormField.FF_REQUIRED);
                if ((options & DO_NOT_SPELL_CHECK) != 0)
                    field.SetFieldFlags(PdfFormField.FF_DONOTSPELLCHECK);
            }
            field.BorderStyle = new PdfBorderDictionary(borderWidth, borderStyle, new PdfDashPattern(3));
            PdfAppearance tp;
            if (isList) {
                tp = GetListAppearance();
                if (topFirst > 0)
                    field.Put(PdfName.TI, new PdfNumber(topFirst));
            }
            else
                tp = GetAppearance();
            field.SetAppearance(PdfAnnotation.APPEARANCE_NORMAL, tp);
            PdfAppearance da = (PdfAppearance)tp.Duplicate;
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
        
        /** Sets the default text. It is only meaningful for text fields.
        * @param defaultText the default text
        */
        public string DefaultText {
            get {
                return defaultText;
            }
            set {
                defaultText = value;
            }
        }

        /** Sets the choices to be presented to the user in list/combo
        * fields.
        * @param choices the choices to be presented to the user
        */
        public string[] Choices {
            get {
                return choices;
            }
            set {
                choices = value;
            }
        }

        /** Sets the export values in list/combo fields. If this array
        * is <CODE>null</CODE> then the choice values will also be used
        * as the export values.
        * @param choiceExports the export values in list/combo fields
        */
        public string[] ChoiceExports {
            get {
                return choiceExports;
            }
            set {
                choiceExports = value;
            }
        }
        
        /** Sets the zero based index of the selected item.
        * @param choiceSelection the zero based index of the selected item
        */
        public int ChoiceSelection {
            get {
                return choiceSelection;
            }
            set {
                choiceSelection = value;
            }
        }
        
        internal int TopFirst {
            get {
                return topFirst;
            }
        }

        /**
        * Sets extra margins in text fields to better mimic the Acrobat layout.
        * @param extraMarginLeft the extra marging left
        * @param extraMarginTop the extra margin top
        */    
        public void SetExtraMargin(float extraMarginLeft, float extraMarginTop) {
            this.extraMarginLeft = extraMarginLeft;
            this.extraMarginTop = extraMarginTop;
        }

        /**
        * Holds value of property substitutionFonts.
        */
        private ArrayList substitutionFonts;

        /**
        * Sets a list of substitution fonts. The list is composed of <CODE>BaseFont</CODE> and can also be <CODE>null</CODE>. The fonts in this list will be used if the original
        * font doesn't contain the needed glyphs.
        * @param substitutionFonts the list
        */
        public ArrayList SubstitutionFonts {
            set {
                substitutionFonts = value;
            }
            get {
                return substitutionFonts;
            }
        }

        /**
        * Holds value of property extensionFont.
        */
        private BaseFont extensionFont;

        /**
        * Sets the extensionFont. This font will be searched before the
        * substitution fonts. It may be <code>null</code>.
        * @param extensionFont New value of property extensionFont.
        */
        public BaseFont ExtensionFont {
            set {
                extensionFont = value;
            }
            get {
                return extensionFont;
            }
        }
    }
}