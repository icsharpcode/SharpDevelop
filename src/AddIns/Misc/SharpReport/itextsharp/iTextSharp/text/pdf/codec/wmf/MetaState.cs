using System;
using System.Collections;
using iTextSharp.text;

/*
 * $Id: MetaState.cs,v 1.6 2008/05/13 11:25:37 psoares33 Exp $
 * 
 *
 * Copyright 2001, 2002 Paulo Soares
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

namespace iTextSharp.text.pdf.codec.wmf {
    public class MetaState {
    
        public static int TA_NOUPDATECP = 0;
        public static int TA_UPDATECP = 1;
        public static int TA_LEFT = 0;
        public static int TA_RIGHT = 2;
        public static int TA_CENTER = 6;
        public static int TA_TOP = 0;
        public static int TA_BOTTOM = 8;
        public static int TA_BASELINE = 24;
    
        public static int TRANSPARENT = 1;
        public static int OPAQUE = 2;

        public static int ALTERNATE = 1;
        public static int WINDING = 2;

        public Stack savedStates;
        public ArrayList MetaObjects;
        public System.Drawing.Point currentPoint;
        public MetaPen currentPen;
        public MetaBrush currentBrush;
        public MetaFont currentFont;
        public Color currentBackgroundColor = Color.WHITE;
        public Color currentTextColor = Color.BLACK;
        public int backgroundMode = OPAQUE;
        public int polyFillMode = ALTERNATE;
        public int lineJoin = 1;
        public int textAlign;
        public int offsetWx;
        public int offsetWy;
        public int extentWx;
        public int extentWy;
        public float scalingX;
        public float scalingY;
    

        /** Creates new MetaState */
        public MetaState() {
            savedStates = new Stack();
            MetaObjects = new ArrayList();
            currentPoint = new System.Drawing.Point(0, 0);
            currentPen = new MetaPen();
            currentBrush = new MetaBrush();
            currentFont = new MetaFont();
        }

        public MetaState(MetaState state) {
            metaState = state;
        }
    
        public MetaState metaState {
            set {
                savedStates = value.savedStates;
                MetaObjects = value.MetaObjects;
                currentPoint = value.currentPoint;
                currentPen = value.currentPen;
                currentBrush = value.currentBrush;
                currentFont = value.currentFont;
                currentBackgroundColor = value.currentBackgroundColor;
                currentTextColor = value.currentTextColor;
                backgroundMode = value.backgroundMode;
                polyFillMode = value.polyFillMode;
                textAlign = value.textAlign;
                lineJoin = value.lineJoin;
                offsetWx = value.offsetWx;
                offsetWy = value.offsetWy;
                extentWx = value.extentWx;
                extentWy = value.extentWy;
                scalingX = value.scalingX;
                scalingY = value.scalingY;
            }
        }

        public void AddMetaObject(MetaObject obj) {
            for (int k = 0; k < MetaObjects.Count; ++k) {
                if (MetaObjects[k] == null) {
                    MetaObjects[k] = obj;
                    return;
                }
            }
            MetaObjects.Add(obj);
        }
    
        public void SelectMetaObject(int index, PdfContentByte cb) {
            MetaObject obj = (MetaObject)MetaObjects[index];
            if (obj == null)
                return;
            int style;
            switch (obj.Type) {
                case MetaObject.META_BRUSH:
                    currentBrush = (MetaBrush)obj;
                    style = currentBrush.Style;
                    if (style == MetaBrush.BS_SOLID) {
                        Color color = currentBrush.Color;
                        cb.SetColorFill(color);
                    }
                    else if (style == MetaBrush.BS_HATCHED) {
                        Color color = currentBackgroundColor;
                        cb.SetColorFill(color);
                    }
                    break;
                case MetaObject.META_PEN: {
                    currentPen = (MetaPen)obj;
                    style = currentPen.Style;
                    if (style != MetaPen.PS_NULL) {
                        Color color = currentPen.Color;
                        cb.SetColorStroke(color);
                        cb.SetLineWidth(Math.Abs((float)currentPen.PenWidth * scalingX / extentWx));
                        switch (style) {
                            case MetaPen.PS_DASH:
                                cb.SetLineDash(18, 6, 0);
                                break;
                            case MetaPen.PS_DASHDOT:
                                cb.SetLiteral("[9 6 3 6]0 d\n");
                                break;
                            case MetaPen.PS_DASHDOTDOT:
                                cb.SetLiteral("[9 3 3 3 3 3]0 d\n");
                                break;
                            case MetaPen.PS_DOT:
                                cb.SetLineDash(3, 0);
                                break;
                            default:
                                cb.SetLineDash(0);
                                break;                            
                        }
                    }
                    break;
                }
                case MetaObject.META_FONT: {
                    currentFont = (MetaFont)obj;
                    break;
                }
            }
        }
    
        public void DeleteMetaObject(int index) {
            MetaObjects[index] =  null;
        }
    
        public void SaveState(PdfContentByte cb) {
            cb.SaveState();
            MetaState state = new MetaState(this);
            savedStates.Push(state);
        }

        public void RestoreState(int index, PdfContentByte cb) {
            int pops;
            if (index < 0)
                pops = Math.Min(-index, savedStates.Count);
            else
                pops = Math.Max(savedStates.Count - index, 0);
            if (pops == 0)
                return;
            MetaState state = null;
            while (pops-- != 0) {
                cb.RestoreState();
                state = (MetaState)savedStates.Pop();
            }
            metaState = state;
        }
    
        public void Cleanup(PdfContentByte cb) {
            int k = savedStates.Count;
            while (k-- > 0)
                cb.RestoreState();
        }

        public float TransformX(int x) {
            return ((float)x - offsetWx) * scalingX / extentWx;
        }

        public float TransformY(int y) {
            return (1f - ((float)y - offsetWy) / extentWy) * scalingY;
        }
    
        public float ScalingX {
            set {
                this.scalingX = value;
            }
        }
    
        public float ScalingY {
            set {
                this.scalingY = value;
            }
        }
    
        public int OffsetWx {
            set {
                this.offsetWx = value;
            }
        }
    
        public int OffsetWy {
            set {
                this.offsetWy = value;
            }
        }
    
        public int ExtentWx {
            set {
                this.extentWx = value;
            }
        }
    
        public int ExtentWy {
            set {
                this.extentWy = value;
            }
        }
    
        public float TransformAngle(float angle) {
            float ta = scalingY < 0 ? -angle : angle;
            return (float)(scalingX < 0 ? Math.PI - ta : ta);
        }
        
        public System.Drawing.Point CurrentPoint {
            get {
                return currentPoint;
            }

            set {
                currentPoint = value;
            }
        }
    
        public MetaBrush CurrentBrush {
            get {
                return currentBrush;
            }
        }

        public MetaPen CurrentPen {
            get {
                return currentPen;
            }
        }

        public MetaFont CurrentFont {
            get {
                return currentFont;
            }
        }
    
        /** Getter for property currentBackgroundColor.
         * @return Value of property currentBackgroundColor.
         */
        public Color CurrentBackgroundColor {
            get {
                return currentBackgroundColor;
            }

            set {
                this.currentBackgroundColor = value;
            }
        }
    
        /** Getter for property currentTextColor.
         * @return Value of property currentTextColor.
         */
        public Color CurrentTextColor {
            get {
                return currentTextColor;
            }

            set {
                this.currentTextColor = value;
            }
        }
    
        /** Getter for property backgroundMode.
         * @return Value of property backgroundMode.
         */
        public int BackgroundMode {
            get {
                return backgroundMode;
            }

            set {
                this.backgroundMode = value;
            }
        }
    
        /** Getter for property textAlign.
         * @return Value of property textAlign.
         */
        public int TextAlign {
            get {
                return textAlign;
            }

            set {
                this.textAlign = value;
            }
        }
    
        /** Getter for property polyFillMode.
         * @return Value of property polyFillMode.
         */
        public int PolyFillMode {
            get {
                return polyFillMode;
            }

            set {
                this.polyFillMode = value;
            }
        }
    
        public PdfContentByte LineJoinRectangle {
            set {
                if (lineJoin != 0) {
                    lineJoin = 0;
                    value.SetLineJoin(0);
                }
            }
        }
    
        public PdfContentByte LineJoinPolygon {
            set {
                if (lineJoin == 0) {
                    lineJoin = 1;
                    value.SetLineJoin(1);
                }
            }
        }
    
        public bool LineNeutral {
            get {
                return (lineJoin == 0);
            }
        }
    }
}
