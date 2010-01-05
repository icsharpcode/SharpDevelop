using System;
using System.IO;
using System.Net;
using iTextSharp.text;
using System.Collections;

/*
 * $Id: MetaDo.cs,v 1.4 2008/05/13 11:25:36 psoares33 Exp $
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

namespace iTextSharp.text.pdf.codec.wmf
{
    /// <summary>
    /// Summary description for MetaDo.
    /// </summary>
    public class MetaDo
    {
    
    public const int META_SETBKCOLOR            = 0x0201;
    public const int META_SETBKMODE             = 0x0102;
    public const int META_SETMAPMODE            = 0x0103;
    public const int META_SETROP2               = 0x0104;
    public const int META_SETRELABS             = 0x0105;
    public const int META_SETPOLYFILLMODE       = 0x0106;
    public const int META_SETSTRETCHBLTMODE     = 0x0107;
    public const int META_SETTEXTCHAREXTRA      = 0x0108;
    public const int META_SETTEXTCOLOR          = 0x0209;
    public const int META_SETTEXTJUSTIFICATION  = 0x020A;
    public const int META_SETWINDOWORG          = 0x020B;
    public const int META_SETWINDOWEXT          = 0x020C;
    public const int META_SETVIEWPORTORG        = 0x020D;
    public const int META_SETVIEWPORTEXT        = 0x020E;
    public const int META_OFFSETWINDOWORG       = 0x020F;
    public const int META_SCALEWINDOWEXT        = 0x0410;
    public const int META_OFFSETVIEWPORTORG     = 0x0211;
    public const int META_SCALEVIEWPORTEXT      = 0x0412;
    public const int META_LINETO                = 0x0213;
    public const int META_MOVETO                = 0x0214;
    public const int META_EXCLUDECLIPRECT       = 0x0415;
    public const int META_INTERSECTCLIPRECT     = 0x0416;
    public const int META_ARC                   = 0x0817;
    public const int META_ELLIPSE               = 0x0418;
    public const int META_FLOODFILL             = 0x0419;
    public const int META_PIE                   = 0x081A;
    public const int META_RECTANGLE             = 0x041B;
    public const int META_ROUNDRECT             = 0x061C;
    public const int META_PATBLT                = 0x061D;
    public const int META_SAVEDC                = 0x001E;
    public const int META_SETPIXEL              = 0x041F;
    public const int META_OFFSETCLIPRGN         = 0x0220;
    public const int META_TEXTOUT               = 0x0521;
    public const int META_BITBLT                = 0x0922;
    public const int META_STRETCHBLT            = 0x0B23;
    public const int META_POLYGON               = 0x0324;
    public const int META_POLYLINE              = 0x0325;
    public const int META_ESCAPE                = 0x0626;
    public const int META_RESTOREDC             = 0x0127;
    public const int META_FILLREGION            = 0x0228;
    public const int META_FRAMEREGION           = 0x0429;
    public const int META_INVERTREGION          = 0x012A;
    public const int META_PAINTREGION           = 0x012B;
    public const int META_SELECTCLIPREGION      = 0x012C;
    public const int META_SELECTOBJECT          = 0x012D;
    public const int META_SETTEXTALIGN          = 0x012E;
    public const int META_CHORD                 = 0x0830;
    public const int META_SETMAPPERFLAGS        = 0x0231;
    public const int META_EXTTEXTOUT            = 0x0a32;
    public const int META_SETDIBTODEV           = 0x0d33;
    public const int META_SELECTPALETTE         = 0x0234;
    public const int META_REALIZEPALETTE        = 0x0035;
    public const int META_ANIMATEPALETTE        = 0x0436;
    public const int META_SETPALENTRIES         = 0x0037;
    public const int META_POLYPOLYGON           = 0x0538;
    public const int META_RESIZEPALETTE         = 0x0139;
    public const int META_DIBBITBLT             = 0x0940;
    public const int META_DIBSTRETCHBLT         = 0x0b41;
    public const int META_DIBCREATEPATTERNBRUSH = 0x0142;
    public const int META_STRETCHDIB            = 0x0f43;
    public const int META_EXTFLOODFILL          = 0x0548;
    public const int META_DELETEOBJECT          = 0x01f0;
    public const int META_CREATEPALETTE         = 0x00f7;
    public const int META_CREATEPATTERNBRUSH    = 0x01F9;
    public const int META_CREATEPENINDIRECT     = 0x02FA;
    public const int META_CREATEFONTINDIRECT    = 0x02FB;
    public const int META_CREATEBRUSHINDIRECT   = 0x02FC;
    public const int META_CREATEREGION          = 0x06FF;

    public PdfContentByte cb;
    public InputMeta meta;
    int left;
    int top;
    int right;
    int bottom;
    int inch;
    MetaState state = new MetaState();

    public MetaDo(Stream meta, PdfContentByte cb) {
        this.cb = cb;
        this.meta = new InputMeta(meta);
    }
    
    public void ReadAll() {
        if (meta.ReadInt() != unchecked((int)0x9AC6CDD7)) {
            throw new DocumentException("Not a placeable windows metafile");
        }
        meta.ReadWord();
        left = meta.ReadShort();
        top = meta.ReadShort();
        right = meta.ReadShort();
        bottom = meta.ReadShort();
        inch = meta.ReadWord();
        state.ScalingX = (float)(right - left) / (float)inch * 72f;
        state.ScalingY = (float)(bottom - top) / (float)inch * 72f;
        state.OffsetWx = left;
        state.OffsetWy = top;
        state.ExtentWx = right - left;
        state.ExtentWy = bottom - top;
        meta.ReadInt();
        meta.ReadWord();
        meta.Skip(18);
        
        int tsize;
        int function;
        cb.SetLineCap(1);
        cb.SetLineJoin(1);
        for (;;) {
            int lenMarker = meta.Length;
            tsize = meta.ReadInt();
            if (tsize < 3)
                break;
            function = meta.ReadWord();
            switch (function) {
                case 0:
                    break;
                case META_CREATEPALETTE:
                case META_CREATEREGION:
                case META_DIBCREATEPATTERNBRUSH:
                    state.AddMetaObject(new MetaObject());
                    break;
                case META_CREATEPENINDIRECT:
                {
                    MetaPen pen = new MetaPen();
                    pen.Init(meta);
                    state.AddMetaObject(pen);
                    break;
                }
                case META_CREATEBRUSHINDIRECT:
                {
                    MetaBrush brush = new MetaBrush();
                    brush.Init(meta);
                    state.AddMetaObject(brush);
                    break;
                }
                case META_CREATEFONTINDIRECT:
                {
                    MetaFont font = new MetaFont();
                    font.Init(meta);
                    state.AddMetaObject(font);
                    break;
                }
                case META_SELECTOBJECT:
                {
                    int idx = meta.ReadWord();
                    state.SelectMetaObject(idx, cb);
                    break;
                }
                case META_DELETEOBJECT:
                {
                    int idx = meta.ReadWord();
                    state.DeleteMetaObject(idx);
                    break;
                }
                case META_SAVEDC:
                    state.SaveState(cb);
                    break;
                case META_RESTOREDC:
                {
                    int idx = meta.ReadShort();
                    state.RestoreState(idx, cb);
                    break;
                }
                case META_SETWINDOWORG:
                    state.OffsetWy = meta.ReadShort();
                    state.OffsetWx = meta.ReadShort();
                    break;
                case META_SETWINDOWEXT:
                    state.ExtentWy = meta.ReadShort();
                    state.ExtentWx = meta.ReadShort();
                    break;
                case META_MOVETO:
                {
                    int y = meta.ReadShort();
                    System.Drawing.Point p = new System.Drawing.Point(meta.ReadShort(), y);
                    state.CurrentPoint = p;
                    break;
                }
                case META_LINETO:
                {
                    int y = meta.ReadShort();
                    int x = meta.ReadShort();
                    System.Drawing.Point p = state.CurrentPoint;
                    cb.MoveTo(state.TransformX(p.X), state.TransformY(p.Y));
                    cb.LineTo(state.TransformX(x), state.TransformY(y));
                    cb.Stroke();
                    state.CurrentPoint = new System.Drawing.Point(x, y);
                    break;
                }
                case META_POLYLINE:
                {
                    state.LineJoinPolygon = cb;
                    int len = meta.ReadWord();
                    int x = meta.ReadShort();
                    int y = meta.ReadShort();
                    cb.MoveTo(state.TransformX(x), state.TransformY(y));
                    for (int k = 1; k < len; ++k) {
                        x = meta.ReadShort();
                        y = meta.ReadShort();
                        cb.LineTo(state.TransformX(x), state.TransformY(y));
                    }
                    cb.Stroke();
                    break;
                }
                case META_POLYGON:
                {
                    if (IsNullStrokeFill(false))
                        break;
                    int len = meta.ReadWord();
                    int sx = meta.ReadShort();
                    int sy = meta.ReadShort();
                    cb.MoveTo(state.TransformX(sx), state.TransformY(sy));
                    for (int k = 1; k < len; ++k) {
                        int x = meta.ReadShort();
                        int y = meta.ReadShort();
                        cb.LineTo(state.TransformX(x), state.TransformY(y));
                    }
                    cb.LineTo(state.TransformX(sx), state.TransformY(sy));
                    StrokeAndFill();
                    break;
                }
                case META_POLYPOLYGON:
                {
                    if (IsNullStrokeFill(false))
                        break;
                    int numPoly = meta.ReadWord();
                    int[] lens = new int[numPoly];
                    for (int k = 0; k < lens.Length; ++k)
                        lens[k] = meta.ReadWord();
                    for (int j = 0; j < lens.Length; ++j) {
                        int len = lens[j];
                        int sx = meta.ReadShort();
                        int sy = meta.ReadShort();
                        cb.MoveTo(state.TransformX(sx), state.TransformY(sy));
                        for (int k = 1; k < len; ++k) {
                            int x = meta.ReadShort();
                            int y = meta.ReadShort();
                            cb.LineTo(state.TransformX(x), state.TransformY(y));
                        }
                        cb.LineTo(state.TransformX(sx), state.TransformY(sy));
                    }
                    StrokeAndFill();
                    break;
                }
                case META_ELLIPSE:
                {
                    if (IsNullStrokeFill(state.LineNeutral))
                        break;
                    int b = meta.ReadShort();
                    int r = meta.ReadShort();
                    int t = meta.ReadShort();
                    int l = meta.ReadShort();
                    cb.Arc(state.TransformX(l), state.TransformY(b), state.TransformX(r), state.TransformY(t), 0, 360);
                    StrokeAndFill();
                    break;
                }
                case META_ARC:
                {
                    if (IsNullStrokeFill(state.LineNeutral))
                        break;
                    float yend = state.TransformY(meta.ReadShort());
                    float xend = state.TransformX(meta.ReadShort());
                    float ystart = state.TransformY(meta.ReadShort());
                    float xstart = state.TransformX(meta.ReadShort());
                    float b = state.TransformY(meta.ReadShort());
                    float r = state.TransformX(meta.ReadShort());
                    float t = state.TransformY(meta.ReadShort());
                    float l = state.TransformX(meta.ReadShort());
                    float cx = (r + l) / 2;
                    float cy = (t + b) / 2;
                    float arc1 = GetArc(cx, cy, xstart, ystart);
                    float arc2 = GetArc(cx, cy, xend, yend);
                    arc2 -= arc1;
                    if (arc2 <= 0)
                        arc2 += 360;
                    cb.Arc(l, b, r, t, arc1, arc2);
                    cb.Stroke();
                    break;
                }
                case META_PIE:
                {
                    if (IsNullStrokeFill(state.LineNeutral))
                        break;
                    float yend = state.TransformY(meta.ReadShort());
                    float xend = state.TransformX(meta.ReadShort());
                    float ystart = state.TransformY(meta.ReadShort());
                    float xstart = state.TransformX(meta.ReadShort());
                    float b = state.TransformY(meta.ReadShort());
                    float r = state.TransformX(meta.ReadShort());
                    float t = state.TransformY(meta.ReadShort());
                    float l = state.TransformX(meta.ReadShort());
                    float cx = (r + l) / 2;
                    float cy = (t + b) / 2;
                    float arc1 = GetArc(cx, cy, xstart, ystart);
                    float arc2 = GetArc(cx, cy, xend, yend);
                    arc2 -= arc1;
                    if (arc2 <= 0)
                        arc2 += 360;
                    ArrayList ar = PdfContentByte.BezierArc(l, b, r, t, arc1, arc2);
                    if (ar.Count == 0)
                        break;
                    float[] pt = (float [])ar[0];
                    cb.MoveTo(cx, cy);
                    cb.LineTo(pt[0], pt[1]);
                    for (int k = 0; k < ar.Count; ++k) {
                        pt = (float [])ar[k];
                        cb.CurveTo(pt[2], pt[3], pt[4], pt[5], pt[6], pt[7]);
                    }
                    cb.LineTo(cx, cy);
                    StrokeAndFill();
                    break;
                }
                case META_CHORD:
                {
                    if (IsNullStrokeFill(state.LineNeutral))
                        break;
                    float yend = state.TransformY(meta.ReadShort());
                    float xend = state.TransformX(meta.ReadShort());
                    float ystart = state.TransformY(meta.ReadShort());
                    float xstart = state.TransformX(meta.ReadShort());
                    float b = state.TransformY(meta.ReadShort());
                    float r = state.TransformX(meta.ReadShort());
                    float t = state.TransformY(meta.ReadShort());
                    float l = state.TransformX(meta.ReadShort());
                    float cx = (r + l) / 2;
                    float cy = (t + b) / 2;
                    float arc1 = GetArc(cx, cy, xstart, ystart);
                    float arc2 = GetArc(cx, cy, xend, yend);
                    arc2 -= arc1;
                    if (arc2 <= 0)
                        arc2 += 360;
                    ArrayList ar = PdfContentByte.BezierArc(l, b, r, t, arc1, arc2);
                    if (ar.Count == 0)
                        break;
                    float[] pt = (float [])ar[0];
                    cx = pt[0];
                    cy = pt[1];
                    cb.MoveTo(cx, cy);
                    for (int k = 0; k < ar.Count; ++k) {
                        pt = (float [])ar[k];
                        cb.CurveTo(pt[2], pt[3], pt[4], pt[5], pt[6], pt[7]);
                    }
                    cb.LineTo(cx, cy);
                    StrokeAndFill();
                    break;
                }
                case META_RECTANGLE:
                {
                    if (IsNullStrokeFill(true))
                        break;
                    float b = state.TransformY(meta.ReadShort());
                    float r = state.TransformX(meta.ReadShort());
                    float t = state.TransformY(meta.ReadShort());
                    float l = state.TransformX(meta.ReadShort());
                    cb.Rectangle(l, b, r - l, t - b);
                    StrokeAndFill();
                    break;
                }
                case META_ROUNDRECT:
                {
                    if (IsNullStrokeFill(true))
                        break;
                    float h = state.TransformY(0) - state.TransformY(meta.ReadShort());
                    float w = state.TransformX(meta.ReadShort()) - state.TransformX(0);
                    float b = state.TransformY(meta.ReadShort());
                    float r = state.TransformX(meta.ReadShort());
                    float t = state.TransformY(meta.ReadShort());
                    float l = state.TransformX(meta.ReadShort());
                    cb.RoundRectangle(l, b, r - l, t - b, (h + w) / 4);
                    StrokeAndFill();
                    break;
                }
                case META_INTERSECTCLIPRECT:
                {
                    float b = state.TransformY(meta.ReadShort());
                    float r = state.TransformX(meta.ReadShort());
                    float t = state.TransformY(meta.ReadShort());
                    float l = state.TransformX(meta.ReadShort());
                    cb.Rectangle(l, b, r - l, t - b);
                    cb.EoClip();
                    cb.NewPath();
                    break;
                }
                case META_EXTTEXTOUT:
                {
                    int y = meta.ReadShort();
                    int x = meta.ReadShort();
                    int count = meta.ReadWord();
                    int flag = meta.ReadWord();
                    int x1 = 0;
                    int y1 = 0;
                    int x2 = 0;
                    int y2 = 0;
                    if ((flag & (MetaFont.ETO_CLIPPED | MetaFont.ETO_OPAQUE)) != 0) {
                        x1 = meta.ReadShort();
                        y1 = meta.ReadShort();
                        x2 = meta.ReadShort();
                        y2 = meta.ReadShort();
                    }
                    byte[] text = new byte[count];
                    int k;
                    for (k = 0; k < count; ++k) {
                        byte c = (byte)meta.ReadByte();
                        if (c == 0)
                            break;
                        text[k] = c;
                    }
                    string s;
                    try {
                        s = System.Text.Encoding.GetEncoding(1252).GetString(text, 0, k);
                    }
                    catch  {
                        s = System.Text.ASCIIEncoding.ASCII.GetString(text, 0, k);
                    }
                    OutputText(x, y, flag, x1, y1, x2, y2, s);
                    break;
                }
                case META_TEXTOUT:
                {
                    int count = meta.ReadWord();
                    byte[] text = new byte[count];
                    int k;
                    for (k = 0; k < count; ++k) {
                        byte c = (byte)meta.ReadByte();
                        if (c == 0)
                            break;
                        text[k] = c;
                    }
                    string s;
                    try {
                        s = System.Text.Encoding.GetEncoding(1252).GetString(text, 0, k);
                    }
                    catch {
                        s = System.Text.ASCIIEncoding.ASCII.GetString(text, 0, k);
                    }
                    count = (count + 1) & 0xfffe;
                    meta.Skip(count - k);
                    int y = meta.ReadShort();
                    int x = meta.ReadShort();
                    OutputText(x, y, 0, 0, 0, 0, 0, s);
                    break;
                }
                case META_SETBKCOLOR:
                    state.CurrentBackgroundColor = meta.ReadColor();
                    break;
                case META_SETTEXTCOLOR:
                    state.CurrentTextColor = meta.ReadColor();
                    break;
                case META_SETTEXTALIGN:
                    state.TextAlign = meta.ReadWord();
                    break;
                case META_SETBKMODE:
                    state.BackgroundMode = meta.ReadWord();
                    break;
                case META_SETPOLYFILLMODE:
                    state.PolyFillMode = meta.ReadWord();
                    break;
                case META_SETPIXEL:
                {
                    Color color = meta.ReadColor();
                    int y = meta.ReadShort();
                    int x = meta.ReadShort();
                    cb.SaveState();
                    cb.SetColorFill(color);
                    cb.Rectangle(state.TransformX(x), state.TransformY(y), .2f, .2f);
                    cb.Fill();
                    cb.RestoreState();
                    break;
                }
                case META_DIBSTRETCHBLT:
                case META_STRETCHDIB: {
                    int rop = meta.ReadInt();
                    if (function == META_STRETCHDIB) {
                        /*int usage = */ meta.ReadWord();
                    }
                    int srcHeight = meta.ReadShort();
                    int srcWidth = meta.ReadShort();
                    int ySrc = meta.ReadShort();
                    int xSrc = meta.ReadShort();
                    float destHeight = state.TransformY(meta.ReadShort()) - state.TransformY(0);
                    float destWidth = state.TransformX(meta.ReadShort()) - state.TransformX(0);
                    float yDest = state.TransformY(meta.ReadShort());
                    float xDest = state.TransformX(meta.ReadShort());
                    byte[] b = new byte[(tsize * 2) - (meta.Length - lenMarker)];
                    for (int k = 0; k < b.Length; ++k)
                        b[k] = (byte)meta.ReadByte();
                    try {
                        MemoryStream inb = new MemoryStream(b);
                        Image bmp = BmpImage.GetImage(inb, true, b.Length);
                        cb.SaveState();
                        cb.Rectangle(xDest, yDest, destWidth, destHeight);
                        cb.Clip();
                        cb.NewPath();
                        bmp.ScaleAbsolute(destWidth * bmp.Width / srcWidth, -destHeight * bmp.Height / srcHeight);
                        bmp.SetAbsolutePosition(xDest - destWidth * xSrc / srcWidth, yDest + destHeight * ySrc / srcHeight - bmp.ScaledHeight);
                        cb.AddImage(bmp);
                        cb.RestoreState();
                    }
                    catch {
                        // empty on purpose
                    }
                    break;
                }
            }
            meta.Skip((tsize * 2) - (meta.Length - lenMarker));
        }
        state.Cleanup(cb);
    }
    
    public void OutputText(int x, int y, int flag, int x1, int y1, int x2, int y2, string text) {
        MetaFont font = state.CurrentFont;
        float refX = state.TransformX(x);
        float refY = state.TransformY(y);
        float angle = state.TransformAngle(font.Angle);
        float sin = (float)Math.Sin(angle);
        float cos = (float)Math.Cos(angle);
        float fontSize = font.GetFontSize(state);
        BaseFont bf = font.Font;
        int align = state.TextAlign;
        float textWidth = bf.GetWidthPoint(text, fontSize);
        float tx = 0;
        float ty = 0;
        float descender = bf.GetFontDescriptor(BaseFont.DESCENT, fontSize);
        float ury = bf.GetFontDescriptor(BaseFont.BBOXURY, fontSize);
        cb.SaveState();
        cb.ConcatCTM(cos, sin, -sin, cos, refX, refY);
        if ((align & MetaState.TA_CENTER) == MetaState.TA_CENTER)
            tx = -textWidth / 2;
        else if ((align & MetaState.TA_RIGHT) == MetaState.TA_RIGHT)
            tx = -textWidth;
        if ((align & MetaState.TA_BASELINE) == MetaState.TA_BASELINE)
            ty = 0;
        else if ((align & MetaState.TA_BOTTOM) == MetaState.TA_BOTTOM)
            ty = -descender;
        else
            ty = -ury;
        Color textColor;
        if (state.BackgroundMode == MetaState.OPAQUE) {
            textColor = state.CurrentBackgroundColor;
            cb.SetColorFill(textColor);
            cb.Rectangle(tx, ty + descender, textWidth, ury - descender);
            cb.Fill();
        }
        textColor = state.CurrentTextColor;
        cb.SetColorFill(textColor);
        cb.BeginText();
        cb.SetFontAndSize(bf, fontSize);
        cb.SetTextMatrix(tx, ty);
        cb.ShowText(text);
        cb.EndText();
        if (font.IsUnderline()) {
            cb.Rectangle(tx, ty - fontSize / 4, textWidth, fontSize / 15);
            cb.Fill();
        }
        if (font.IsStrikeout()) {
            cb.Rectangle(tx, ty + fontSize / 3, textWidth, fontSize / 15);
            cb.Fill();
        }
        cb.RestoreState();
    }
    
    public bool IsNullStrokeFill(bool isRectangle) {
        MetaPen pen = state.CurrentPen;
        MetaBrush brush = state.CurrentBrush;
        bool noPen = (pen.Style == MetaPen.PS_NULL);
        int style = brush.Style;
        bool isBrush = (style == MetaBrush.BS_SOLID || (style == MetaBrush.BS_HATCHED && state.BackgroundMode == MetaState.OPAQUE));
        bool result = noPen && !isBrush;
        if (!noPen) {
            if (isRectangle)
                state.LineJoinRectangle = cb;
            else
                state.LineJoinPolygon = cb;
        }
        return result;
    }

    public void StrokeAndFill(){
        MetaPen pen = state.CurrentPen;
        MetaBrush brush = state.CurrentBrush;
        int penStyle = pen.Style;
        int brushStyle = brush.Style;
        if (penStyle == MetaPen.PS_NULL) {
            cb.ClosePath();
            if (state.PolyFillMode == MetaState.ALTERNATE) {
                cb.EoFill();
            }
            else {
                cb.Fill();
            }
        }
        else {
            bool isBrush = (brushStyle == MetaBrush.BS_SOLID || (brushStyle == MetaBrush.BS_HATCHED && state.BackgroundMode == MetaState.OPAQUE));
            if (isBrush) {
                if (state.PolyFillMode == MetaState.ALTERNATE)
                    cb.ClosePathEoFillStroke();
                else
                    cb.ClosePathFillStroke();
            }
            else {
                cb.ClosePathStroke();
            }
        }
    }
    
    internal static float GetArc(float xCenter, float yCenter, float xDot, float yDot) {
        double s = Math.Atan2(yDot - yCenter, xDot - xCenter);
        if (s < 0)
            s += Math.PI * 2;
        return (float)(s / Math.PI * 180);
    }

    public static byte[] WrapBMP(Image image)  {
        if (image.OriginalType != Image.ORIGINAL_BMP)
            throw new IOException("Only BMP can be wrapped in WMF.");
        Stream imgIn;
        byte[] data = null;
        if (image.OriginalData == null) {
            imgIn = WebRequest.Create(image.Url).GetResponse().GetResponseStream();
            MemoryStream outp = new MemoryStream();
            int b = 0;
            while ((b = imgIn.ReadByte()) != -1)
                outp.WriteByte((byte)b);
            imgIn.Close();
            data = outp.ToArray();
        }
        else
            data = image.OriginalData;
        int sizeBmpWords = (data.Length - 14 + 1) >> 1;
        MemoryStream os = new MemoryStream();
        // write metafile header
        WriteWord(os, 1);
        WriteWord(os, 9);
        WriteWord(os, 0x0300);
        WriteDWord(os, 9 + 4 + 5 + 5 + (13 + sizeBmpWords) + 3); // total metafile size
        WriteWord(os, 1);
        WriteDWord(os, 14 + sizeBmpWords); // max record size
        WriteWord(os, 0);
        // write records
        WriteDWord(os, 4);
        WriteWord(os, META_SETMAPMODE);
        WriteWord(os, 8);

        WriteDWord(os, 5);
        WriteWord(os, META_SETWINDOWORG);
        WriteWord(os, 0);
        WriteWord(os, 0);

        WriteDWord(os, 5);
        WriteWord(os, META_SETWINDOWEXT);
        WriteWord(os, (int)image.Height);
        WriteWord(os, (int)image.Width);

        WriteDWord(os, 13 + sizeBmpWords);
        WriteWord(os, META_DIBSTRETCHBLT);
        WriteDWord(os, 0x00cc0020);
        WriteWord(os, (int)image.Height);
        WriteWord(os, (int)image.Width);
        WriteWord(os, 0);
        WriteWord(os, 0);
        WriteWord(os, (int)image.Height);
        WriteWord(os, (int)image.Width);
        WriteWord(os, 0);
        WriteWord(os, 0);
        os.Write(data, 14, data.Length - 14);
        if ((data.Length & 1) == 1)
            os.WriteByte(0);
//        WriteDWord(os, 14 + sizeBmpWords);
//        WriteWord(os, META_STRETCHDIB);
//        WriteDWord(os, 0x00cc0020);
//        WriteWord(os, 0);
//        WriteWord(os, (int)image.Height);
//        WriteWord(os, (int)image.Width);
//        WriteWord(os, 0);
//        WriteWord(os, 0);
//        WriteWord(os, (int)image.Height);
//        WriteWord(os, (int)image.Width);
//        WriteWord(os, 0);
//        WriteWord(os, 0);
//        os.Write(data, 14, data.length - 14);
//        if ((data.length & 1) == 1)
//            os.Write(0);

        WriteDWord(os, 3);
        WriteWord(os, 0);
        os.Close();
        return os.ToArray();
    }

    public static void WriteWord(Stream os, int v) {
        os.WriteByte((byte)(v & 0xff));
        os.WriteByte((byte)((v >> 8) & 0xff));
    }
    
    public static void WriteDWord(Stream os, int v) {
        WriteWord(os, v & 0xffff);
        WriteWord(os, (v >> 16) & 0xffff);
    }
    }
}
