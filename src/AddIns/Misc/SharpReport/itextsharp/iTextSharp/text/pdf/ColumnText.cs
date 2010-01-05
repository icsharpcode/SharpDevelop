using System;
using System.Collections;
using System.util.collections;
using iTextSharp.text.pdf.draw;

/*
 * Copyright 2001-2005 by Paulo Soares.
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
 * Formats text in a columnwise form. The text is bound
 * on the left and on the right by a sequence of lines. This allows the column
 * to have any shape, not only rectangular.
 * <P>
 * Several parameters can be set like the first paragraph line indent and
 * extra space between paragraphs.
 * <P>
 * A call to the method <CODE>go</CODE> will return one of the following
 * situations: the column ended or the text ended.
 * <P>
 * I the column ended, a new column definition can be loaded with the method
 * <CODE>setColumns</CODE> and the method <CODE>go</CODE> can be called again.
 * <P>
 * If the text ended, more text can be loaded with <CODE>addText</CODE>
 * and the method <CODE>go</CODE> can be called again.<BR>
 * The only limitation is that one or more complete paragraphs must be loaded
 * each time.
 * <P>
 * Full bidirectional reordering is supported. If the run direction is
 * <CODE>PdfWriter.RUN_DIRECTION_RTL</CODE> the meaning of the horizontal
 * alignments and margins is mirrored.
 * @author Paulo Soares (psoares@consiste.pt)
 */

public class ColumnText {
    /** Eliminate the arabic vowels */    
    public int AR_NOVOWEL = ArabicLigaturizer.ar_novowel;
    /** Compose the tashkeel in the ligatures. */    
    public const int AR_COMPOSEDTASHKEEL = ArabicLigaturizer.ar_composedtashkeel;
    /** Do some extra double ligatures. */    
    public const int AR_LIG = ArabicLigaturizer.ar_lig;
    /**
     * Digit shaping option: Replace European digits (U+0030...U+0039) by Arabic-Indic digits.
     */
    public const int DIGITS_EN2AN = ArabicLigaturizer.DIGITS_EN2AN;
    
    /**
     * Digit shaping option: Replace Arabic-Indic digits by European digits (U+0030...U+0039).
     */
    public const int DIGITS_AN2EN = ArabicLigaturizer.DIGITS_AN2EN;
    
    /**
     * Digit shaping option:
     * Replace European digits (U+0030...U+0039) by Arabic-Indic digits
     * if the most recent strongly directional character
     * is an Arabic letter (its Bidi direction value is RIGHT_TO_LEFT_ARABIC).
     * The initial state at the start of the text is assumed to be not an Arabic,
     * letter, so European digits at the start of the text will not change.
     * Compare to DIGITS_ALEN2AN_INIT_AL.
     */
    public const int DIGITS_EN2AN_INIT_LR = ArabicLigaturizer.DIGITS_EN2AN_INIT_LR;
    
    /**
     * Digit shaping option:
     * Replace European digits (U+0030...U+0039) by Arabic-Indic digits
     * if the most recent strongly directional character
     * is an Arabic letter (its Bidi direction value is RIGHT_TO_LEFT_ARABIC).
     * The initial state at the start of the text is assumed to be an Arabic,
     * letter, so European digits at the start of the text will change.
     * Compare to DIGITS_ALEN2AN_INT_LR.
     */
    public const int DIGITS_EN2AN_INIT_AL = ArabicLigaturizer.DIGITS_EN2AN_INIT_AL;
    
    /**
     * Digit type option: Use Arabic-Indic digits (U+0660...U+0669).
     */
    public const int DIGIT_TYPE_AN = ArabicLigaturizer.DIGIT_TYPE_AN;
    
    /**
     * Digit type option: Use Eastern (Extended) Arabic-Indic digits (U+06f0...U+06f9).
     */
    public const int DIGIT_TYPE_AN_EXTENDED = ArabicLigaturizer.DIGIT_TYPE_AN_EXTENDED;
    
    protected int runDirection = PdfWriter.RUN_DIRECTION_DEFAULT;
    public static float GLOBAL_SPACE_CHAR_RATIO = 0;
    
    /** Signals that there is no more text available. */
    public const int NO_MORE_TEXT = 1;
    
    /** Signals that there is no more column. */
    public const int NO_MORE_COLUMN = 2;
    
    /** The column is valid. */
    protected const int LINE_STATUS_OK = 0;
    
    /** The line is out the column limits. */
    protected const int LINE_STATUS_OFFLIMITS = 1;
    
    /** The line cannot fit this column position. */
    protected const int LINE_STATUS_NOLINE = 2;
    
    /** Upper bound of the column. */
    protected float maxY;
    
    /** Lower bound of the column. */
    protected float minY;
    
    protected float leftX;
    
    protected float rightX;
    
    /** The column Element. Default is left Element. */
    protected int alignment = Element.ALIGN_LEFT;
    
    /** The left column bound. */
    protected ArrayList leftWall;
 
    /** The right column bound. */
    protected ArrayList rightWall;
    
    /** The chunks that form the text. */
//    protected ArrayList chunks = new ArrayList();
    protected BidiLine bidiLine;
    
    /** The current y line location. Text will be written at this line minus the leading. */
    protected float yLine;
    
    /** The leading for the current line. */
    protected float currentLeading = 16;
    
    /** The fixed text leading. */
    protected float fixedLeading = 16;
    
    /** The text leading that is multiplied by the biggest font size in the line. */
    protected float multipliedLeading = 0;
    
    /** The <CODE>PdfContent</CODE> where the text will be written to. */
    protected PdfContentByte canvas;
    
    protected PdfContentByte[] canvases;
    
    /** The line status when trying to fit a line to a column. */
    protected int lineStatus;
    
    /** The first paragraph line indent. */
    protected float indent = 0;
    
    /** The following paragraph lines indent. */
    protected float followingIndent = 0;
    
    /** The right paragraph lines indent. */
    protected float rightIndent = 0;
    
    /** The extra space between paragraphs. */
    protected float extraParagraphSpace = 0;
        
    /** The width of the line when the column is defined as a simple rectangle. */
    protected float rectangularWidth = -1;
    
    protected bool rectangularMode = false;

    /** Holds value of property spaceCharRatio. */
    private float spaceCharRatio = GLOBAL_SPACE_CHAR_RATIO;

    private bool lastWasNewline = true;

    /** Holds value of property linesWritten. */
    private int linesWritten;
    
    private float firstLineY;

    private bool firstLineYDone = false;
    
    /** Holds value of property arabicOptions. */
    private int arabicOptions = 0;
    
    protected float descender;
    
    protected bool composite = false;
    
    protected ColumnText compositeColumn;
    
    protected internal ArrayList compositeElements;
    
    protected int listIdx = 0;
    
    private bool splittedRow;
    
    protected Phrase waitPhrase;
    
    /** if true, first line height is adjusted so that the max ascender touches the top */
    private bool useAscender = false;

    /**
     * Creates a <CODE>ColumnText</CODE>.
     * @param text the place where the text will be written to. Can
     * be a template.
     */
    public ColumnText(PdfContentByte canvas) {
        this.canvas = canvas;
    }
    
    /** Creates an independent duplicated of the instance <CODE>org</CODE>.
     * @param org the original <CODE>ColumnText</CODE>
     * @return the duplicated
     */    
    public static ColumnText Duplicate(ColumnText org) {
        ColumnText ct = new ColumnText(null);
        ct.SetACopy(org);
        return ct;
    }
    
    /** Makes this instance an independent copy of <CODE>org</CODE>.
     * @param org the original <CODE>ColumnText</CODE>
     * @return itself
     */    
    public ColumnText SetACopy(ColumnText org) {
        SetSimpleVars(org);
        if (org.bidiLine != null)
            bidiLine = new BidiLine(org.bidiLine);
        return this;
    }
    
    protected internal void SetSimpleVars(ColumnText org) {
        maxY = org.maxY;
        minY = org.minY;
        alignment = org.alignment;
        leftWall = null;
        if (org.leftWall != null)
            leftWall = new ArrayList(org.leftWall);
        rightWall = null;
        if (org.rightWall != null)
            rightWall = new ArrayList(org.rightWall);
        yLine = org.yLine;
        currentLeading = org.currentLeading;
        fixedLeading = org.fixedLeading;
        multipliedLeading = org.multipliedLeading;
        canvas = org.canvas;
        canvases = org.canvases;
        lineStatus = org.lineStatus;
        indent = org.indent;
        followingIndent = org.followingIndent;
        rightIndent = org.rightIndent;
        extraParagraphSpace = org.extraParagraphSpace;
        rectangularWidth = org.rectangularWidth;
        rectangularMode = org.rectangularMode;
        spaceCharRatio = org.spaceCharRatio;
        lastWasNewline = org.lastWasNewline;
        linesWritten = org.linesWritten;
        arabicOptions = org.arabicOptions;
        runDirection = org.runDirection;
        descender = org.descender;
        composite = org.composite;
        splittedRow = org.splittedRow;
        if (org.composite) {
            compositeElements = new ArrayList(org.compositeElements);
            if (splittedRow) {
                PdfPTable table = (PdfPTable)compositeElements[0];
                compositeElements[0] = new PdfPTable(table);
            }
            if (org.compositeColumn != null)
                compositeColumn = Duplicate(org.compositeColumn);
        }
        listIdx = org.listIdx;
        firstLineY = org.firstLineY;
        leftX = org.leftX;
        rightX = org.rightX;
        firstLineYDone = org.firstLineYDone;
        waitPhrase = org.waitPhrase;
        useAscender = org.useAscender;
        filledWidth = org.filledWidth;
        adjustFirstLine = org.adjustFirstLine;
    }
    
    private void AddWaitingPhrase() {
        if (bidiLine == null && waitPhrase != null) {
            bidiLine = new BidiLine();
            foreach (Chunk ck in waitPhrase.Chunks) {
                bidiLine.AddChunk(new PdfChunk(ck, null));
            }
            waitPhrase = null;
        }
    }

    /**
     * Adds a <CODE>Phrase</CODE> to the current text array.
     * @param phrase the text
     */
    public void AddText(Phrase phrase) {
        if (phrase == null || composite)
            return;
        AddWaitingPhrase();
        if (bidiLine == null) {
            waitPhrase = phrase;
            return;
        }
        foreach (Chunk c in phrase.Chunks) {
            bidiLine.AddChunk(new PdfChunk(c, null));
        }
    }
    
    /**
     * Replaces the current text array with this <CODE>Phrase</CODE>.
     * Anything added previously with AddElement() is lost.
     * @param phrase the text
     */
    public void SetText(Phrase phrase) {
        bidiLine = null;
        composite = false;
        compositeColumn = null;
        compositeElements = null;
        listIdx = 0;
        splittedRow = false;
        waitPhrase = phrase;
    }
    
    /**
     * Adds a <CODE>Chunk</CODE> to the current text array.
     * Will not have any effect if AddElement() was called before.
     * @param chunk the text
     */
    public void AddText(Chunk chunk) {
        if (chunk == null || composite)
            return;
        AddText(new Phrase(chunk));
    }
    
    /**
     * Adds an element. Elements supported are <CODE>Paragraph</CODE>,
     * <CODE>List</CODE>, <CODE>PdfPTable</CODE>, <CODE>Image</CODE> and
     * <CODE>Graphic</CODE>.
     * <p>
     * It removes all the text placed with <CODE>addText()</CODE>.
     * @param element the <CODE>Element</CODE>
     */    
    public void AddElement(IElement element) {
        if (element == null)
            return;
        if (element is Image) {
            Image img = (Image)element;
            PdfPTable t = new PdfPTable(1);
            float w = img.WidthPercentage;
            if (w == 0) {
                t.TotalWidth = img.ScaledWidth;
                t.LockedWidth = true;
            }
            else
                t.WidthPercentage = w;
            t.SpacingAfter = img.SpacingAfter;
            t.SpacingBefore = img.SpacingBefore;
            switch (img.Alignment) {
                case Image.LEFT_ALIGN:
                    t.HorizontalAlignment = Element.ALIGN_LEFT;
                    break;
                case Image.RIGHT_ALIGN:
                    t.HorizontalAlignment = Element.ALIGN_RIGHT;
                    break;
                default:
                    t.HorizontalAlignment = Element.ALIGN_CENTER;
                    break;
            }
            PdfPCell c = new PdfPCell(img, true);
            c.Padding = 0;
            c.Border = img.Border;
            c.BorderColor = img.BorderColor;
            c.BorderWidth = img.BorderWidth;
            c.BackgroundColor = img.BackgroundColor;
            t.AddCell(c);
            element = t;
        }
        if (element.Type == Element.CHUNK) {
            element = new Paragraph((Chunk)element);
        }
        else if (element.Type == Element.PHRASE) {
            element = new Paragraph((Phrase)element);
        }
        if (element is SimpleTable) {
            try {
                element = ((SimpleTable)element).CreatePdfPTable();
            } catch (DocumentException) {
                throw new ArgumentException("Element not allowed.");
            }
        }
        else if (element.Type != Element.PARAGRAPH && element.Type != Element.LIST && element.Type != Element.PTABLE && element.Type != Element.YMARK)
            throw new ArgumentException("Element not allowed.");
        if (!composite) {
            composite = true;
            compositeElements = new ArrayList();
            bidiLine = null;
            waitPhrase = null;
        }
        compositeElements.Add(element);
    }
    
    /**
     * Converts a sequence of lines representing one of the column bounds into
     * an internal format.
     * <p>
     * Each array element will contain a <CODE>float[4]</CODE> representing
     * the line x = ax + b.
     * @param cLine the column array
     * @return the converted array
     */
    protected ArrayList ConvertColumn(float[] cLine) {
        if (cLine.Length < 4)
            throw new Exception("No valid column line found.");
        ArrayList cc = new ArrayList();
        for (int k = 0; k < cLine.Length - 2; k += 2) {
            float x1 = cLine[k];
            float y1 = cLine[k + 1];
            float x2 = cLine[k + 2];
            float y2 = cLine[k + 3];
            if (y1 == y2)
                continue;
            // x = ay + b
            float a = (x1 - x2) / (y1 - y2);
            float b = x1 - a * y1;
            float[] r = new float[4];
            r[0] = Math.Min(y1, y2);
            r[1] = Math.Max(y1, y2);
            r[2] = a;
            r[3] = b;
            cc.Add(r);
            maxY = Math.Max(maxY, r[1]);
            minY = Math.Min(minY, r[0]);
        }
        if (cc.Count == 0)
            throw new Exception("No valid column line found.");
        return cc;
    }
    
    /**
     * Finds the intersection between the <CODE>yLine</CODE> and the column. It will
     * set the <CODE>lineStatus</CODE> apropriatly.
     * @param wall the column to intersect
     * @return the x coordinate of the intersection
     */
    protected float FindLimitsPoint(ArrayList wall) {
        lineStatus = LINE_STATUS_OK;
        if (yLine < minY || yLine > maxY) {
            lineStatus = LINE_STATUS_OFFLIMITS;
            return 0;
        }
        for (int k = 0; k < wall.Count; ++k) {
            float[] r = (float[])wall[k];
            if (yLine < r[0] || yLine > r[1])
                continue;
            return r[2] * yLine + r[3];
        }
        lineStatus = LINE_STATUS_NOLINE;
        return 0;
    }
    
    /**
     * Finds the intersection between the <CODE>yLine</CODE> and the two
     * column bounds. It will set the <CODE>lineStatus</CODE> apropriatly.
     * @return a <CODE>float[2]</CODE>with the x coordinates of the intersection
     */
    protected float[] FindLimitsOneLine() {
        float x1 = FindLimitsPoint(leftWall);
        if (lineStatus == LINE_STATUS_OFFLIMITS || lineStatus == LINE_STATUS_NOLINE)
            return null;
        float x2 = FindLimitsPoint(rightWall);
        if (lineStatus == LINE_STATUS_NOLINE)
            return null;
        return new float[]{x1, x2};
    }
    
    /**
     * Finds the intersection between the <CODE>yLine</CODE>,
     * the <CODE>yLine-leading</CODE>and the two
     * column bounds. It will set the <CODE>lineStatus</CODE> apropriatly.
     * @return a <CODE>float[4]</CODE>with the x coordinates of the intersection
     */
    protected float[] FindLimitsTwoLines() {
        bool repeat = false;
        for (;;) {
            if (repeat && currentLeading == 0)
                return null;
            repeat = true;
            float[] x1 = FindLimitsOneLine();
            if (lineStatus == LINE_STATUS_OFFLIMITS)
                return null;
            yLine -= currentLeading;
            if (lineStatus == LINE_STATUS_NOLINE) {
                continue;
            }
            float[] x2 = FindLimitsOneLine();
            if (lineStatus == LINE_STATUS_OFFLIMITS)
                return null;
            if (lineStatus == LINE_STATUS_NOLINE) {
                yLine -= currentLeading;
                continue;
            }
            if (x1[0] >= x2[1] || x2[0] >= x1[1])
                continue;
            return new float[]{x1[0], x1[1], x2[0], x2[1]};
        }
    }
    
    /**
     * Sets the columns bounds. Each column bound is described by a
     * <CODE>float[]</CODE> with the line points [x1,y1,x2,y2,...].
     * The array must have at least 4 elements.
     * @param leftLine the left column bound
     * @param rightLine the right column bound
     */
    public void SetColumns(float[] leftLine, float[] rightLine) {
        maxY = -10e20f;
        minY = 10e20f;
        rightWall = ConvertColumn(rightLine);
        leftWall = ConvertColumn(leftLine);
        rectangularWidth = -1;
        rectangularMode = false;
    }
    
    /**
     * Simplified method for rectangular columns.
     * @param phrase a <CODE>Phrase</CODE>
     * @param llx the lower left x corner
     * @param lly the lower left y corner
     * @param urx the upper right x corner
     * @param ury the upper right y corner
     * @param leading the leading
     * @param alignment the column alignment
     */
    public void SetSimpleColumn(Phrase phrase, float llx, float lly, float urx, float ury, float leading, int alignment) {
        AddText(phrase);
        SetSimpleColumn(llx, lly, urx, ury, leading, alignment);
    }
    
    /**
     * Simplified method for rectangular columns.
     * @param llx the lower left x corner
     * @param lly the lower left y corner
     * @param urx the upper right x corner
     * @param ury the upper right y corner
     * @param leading the leading
     * @param alignment the column alignment
     */
    public void SetSimpleColumn(float llx, float lly, float urx, float ury, float leading, int alignment) {
        Leading = leading;
        this.alignment = alignment;
        SetSimpleColumn(llx, lly, urx, ury);
    }
    
    /**
     * Simplified method for rectangular columns.
     * @param llx
     * @param lly
     * @param urx
     * @param ury
     */
    public void SetSimpleColumn(float llx, float lly, float urx, float ury) {
        leftX = Math.Min(llx, urx);
        maxY = Math.Max(lly, ury);
        minY = Math.Min(lly, ury);
        rightX = Math.Max(llx, urx);
        yLine = maxY;
        rectangularWidth = rightX - leftX;
        if (rectangularWidth < 0)
            rectangularWidth = 0;
        rectangularMode = true;
    }

    /**
     * Sets the leading fixed and variable. The resultant leading will be
     * fixedLeading+multipliedLeading*maxFontSize where maxFontSize is the
     * size of the bigest font in the line.
     * @param fixedLeading the fixed leading
     * @param multipliedLeading the variable leading
     */
    public void SetLeading(float fixedLeading, float multipliedLeading) {
        this.fixedLeading = fixedLeading;
        this.multipliedLeading = multipliedLeading;
    }
    
    /**
     * Gets the fixed leading
     * @return the leading
     */
    public float Leading {
        get {
            return fixedLeading;
        }

        set {
            this.fixedLeading = value;
            this.multipliedLeading = 0;
        }
    }
    
    /**
     * Gets the variable leading
     * @return the leading
     */
    public float MultipliedLeading {
        get {
            return multipliedLeading;
        }
    }
    
    /**
     * Gets the yLine.
     * @return the yLine
     */
    public float YLine {
        get {
            return yLine;
        }

        set {
            this.yLine = value;
        }
    }
    
    /**
     * Gets the Element.
     * @return the alignment
     */
    public int Alignment{
        get {
            return alignment;
        }

        set {
            this.alignment = value;
        }
    }
    
    /**
     * Gets the first paragraph line indent.
     * @return the indent
     */
    public float Indent {
        get {
            return indent;
        }

        set {
            this.indent = value;
            lastWasNewline = true;
        }
    }
    
    /**
     * Gets the following paragraph lines indent.
     * @return the indent
     */
    public float FollowingIndent {
        get {
            return followingIndent;
        }

        set {
            this.followingIndent = value;
            lastWasNewline = true;
        }
    }
    
    /**
     * Gets the right paragraph lines indent.
     * @return the indent
     */
    public float RightIndent {
        get {
            return rightIndent;
        }

        set {
            this.rightIndent = value;
            lastWasNewline = true;
        }
    }
    
    /**
     * Outputs the lines to the document. It is equivalent to <CODE>go(false)</CODE>.
     * @return returns the result of the operation. It can be <CODE>NO_MORE_TEXT</CODE>
     * and/or <CODE>NO_MORE_COLUMN</CODE>
     * @throws DocumentException on error
     */
    public int Go() {
        return Go(false);
    }
    
    /**
     * Outputs the lines to the document. The output can be simulated.
     * @param simulate <CODE>true</CODE> to simulate the writting to the document
     * @return returns the result of the operation. It can be <CODE>NO_MORE_TEXT</CODE>
     * and/or <CODE>NO_MORE_COLUMN</CODE>
     * @throws DocumentException on error
     */
    public int Go(bool simulate) {
        if (composite)
            return GoComposite(simulate);
        AddWaitingPhrase();
        if (bidiLine == null)
            return NO_MORE_TEXT;
        descender = 0;
        linesWritten = 0;
        bool dirty = false;
        float ratio = spaceCharRatio;
        Object[] currentValues = new Object[2];
        PdfFont currentFont = null;
        float lastBaseFactor = 0F;
        currentValues[1] = lastBaseFactor;
        PdfDocument pdf = null;
        PdfContentByte graphics = null;
        PdfContentByte text = null;
        firstLineY = float.NaN;
        int localRunDirection = PdfWriter.RUN_DIRECTION_NO_BIDI;
        if (runDirection != PdfWriter.RUN_DIRECTION_DEFAULT)
            localRunDirection = runDirection;
        if (canvas != null) {
            graphics = canvas;
            pdf = canvas.PdfDocument;
            text = canvas.Duplicate;
        }
        else if (!simulate)
            throw new Exception("ColumnText.go with simulate==false and text==null.");
        if (!simulate) {
            if (ratio == GLOBAL_SPACE_CHAR_RATIO)
                ratio = text.PdfWriter.SpaceCharRatio;
            else if (ratio < 0.001f)
                ratio = 0.001f;
        }
        float firstIndent = 0;
        
        int status = 0;
        if (rectangularMode) {
            for (;;) {
                firstIndent = (lastWasNewline ? indent : followingIndent);
                if (rectangularWidth <= firstIndent + rightIndent) {
                    status = NO_MORE_COLUMN;
                    if (bidiLine.IsEmpty())
                        status |= NO_MORE_TEXT;
                    break;
                }
                if (bidiLine.IsEmpty()) {
                    status = NO_MORE_TEXT;
                    break;
                }
                PdfLine line = bidiLine.ProcessLine(leftX, rectangularWidth - firstIndent - rightIndent, alignment, localRunDirection, arabicOptions);
                if (line == null) {
                    status = NO_MORE_TEXT;
                    break;
                }
                float maxSize = line.MaxSizeSimple;
                if (UseAscender && float.IsNaN(firstLineY)) {
                    currentLeading = line.Ascender;
                }
                else {
                    currentLeading = fixedLeading + maxSize * multipliedLeading;
                }
                if (yLine > maxY || yLine - currentLeading < minY ) {
                    status = NO_MORE_COLUMN;
                    bidiLine.Restore();
                    break;
                }
                yLine -= currentLeading;
                if (!simulate && !dirty) {
                    text.BeginText();
                    dirty = true;
                }
                if (float.IsNaN(firstLineY)) {
                    firstLineY = yLine;
                }
                UpdateFilledWidth(rectangularWidth - line.WidthLeft);
                if (!simulate) {
                    currentValues[0] = currentFont;
                    text.SetTextMatrix(leftX + (line.RTL ? rightIndent : firstIndent) + line.IndentLeft, yLine);
                    pdf.WriteLineToContent(line, text, graphics, currentValues, ratio);
                    currentFont = (PdfFont)currentValues[0];
                }
                lastWasNewline = line.NewlineSplit;
                yLine -= line.NewlineSplit ? extraParagraphSpace : 0;
                ++linesWritten;
                descender = line.Descender;
            }
        }
        else {
            currentLeading = fixedLeading;
            for (;;) {
                firstIndent = (lastWasNewline ? indent : followingIndent);
                float yTemp = yLine;
                float[] xx = FindLimitsTwoLines();
                if (xx == null) {
                    status = NO_MORE_COLUMN;
                    if (bidiLine.IsEmpty())
                        status |= NO_MORE_TEXT;
                    yLine = yTemp;
                    break;
                }
                if (bidiLine.IsEmpty()) {
                    status = NO_MORE_TEXT;
                    yLine = yTemp;
                    break;
                }
                float x1 = Math.Max(xx[0], xx[2]);
                float x2 = Math.Min(xx[1], xx[3]);
                if (x2 - x1 <= firstIndent + rightIndent)
                    continue;
                if (!simulate && !dirty) {
                    text.BeginText();
                    dirty = true;
                }
                PdfLine line = bidiLine.ProcessLine(x1, x2 - x1 - firstIndent - rightIndent, alignment, localRunDirection, arabicOptions);
                if (line == null) {
                    status = NO_MORE_TEXT;
                    yLine = yTemp;
                    break;
                }
                if (!simulate) {
                    currentValues[0] = currentFont;
                    text.SetTextMatrix(x1 + (line.RTL ? rightIndent : firstIndent) + line.IndentLeft, yLine);
                    pdf.WriteLineToContent(line, text, graphics, currentValues, ratio);
                    currentFont = (PdfFont)currentValues[0];
                }
                lastWasNewline = line.NewlineSplit;
                yLine -= line.NewlineSplit ? extraParagraphSpace : 0;
                ++linesWritten;
                descender = line.Descender;
            }
        }
        if (dirty) {
            text.EndText();
            canvas.Add(text);
        }
        return status;
    }
    
    /**
     * Sets the extra space between paragraphs.
     * @return the extra space between paragraphs
     */
    public float ExtraParagraphSpace {
        get {
            return extraParagraphSpace;
        }

        set {
            this.extraParagraphSpace = value;
        }
    }
    
    /**
     * Clears the chunk array. A call to <CODE>go()</CODE> will always return
     * NO_MORE_TEXT.
     */
    public void ClearChunks() {
        if (bidiLine != null)
            bidiLine.ClearChunks();
    }
    
    /** Gets the space/character extra spacing ratio for
     * fully justified text.
     * @return the space/character extra spacing ratio
     */    
    public float SpaceCharRatio {
        get {
            return spaceCharRatio;
        }

        set {
            this.spaceCharRatio = value;
        }
    }
    
    /** Gets the run direction.
     * @return the run direction
     */    
    public int RunDirection {
        get {
            return runDirection;
        }

        set {
            if (value < PdfWriter.RUN_DIRECTION_DEFAULT || value > PdfWriter.RUN_DIRECTION_RTL)
                throw new Exception("Invalid run direction: " + value);
            this.runDirection = value;
        }
    }

    /** Gets the number of lines written.
     * @return the number of lines written
     */
    public int LinesWritten {
        get {
            return this.linesWritten;
        }
    }
    
    /** Sets the arabic shaping options. The option can be AR_NOVOWEL,
     * AR_COMPOSEDTASHKEEL and AR_LIG.
     * @param arabicOptions the arabic shaping options
     */
    public int ArabicOptions {
        set {
            this.arabicOptions = value;
        }
        get {
            return arabicOptions;
        }
    }
    
    /** Gets the biggest descender value of the last line written.
     * @return the biggest descender value of the last line written
     */    
    public float Descender {
        get {
            return descender;
        }
    }
    
    /** Gets the width that the line will occupy after writing.
     * Only the width of the first line is returned.
     * @param phrase the <CODE>Phrase</CODE> containing the line
     * @param runDirection the run direction
     * @param arabicOptions the options for the arabic shaping
     * @return the width of the line
     */    
    public static float GetWidth(Phrase phrase, int runDirection, int arabicOptions) {
        ColumnText ct = new ColumnText(null);
        ct.AddText(phrase);
        ct.AddWaitingPhrase();
        PdfLine line = ct.bidiLine.ProcessLine(0, 20000, Element.ALIGN_LEFT, runDirection, arabicOptions);
        if (line == null)
            return 0;
        else
            return 20000 - line.WidthLeft;
    }
    
    /** Gets the width that the line will occupy after writing.
     * Only the width of the first line is returned.
     * @param phrase the <CODE>Phrase</CODE> containing the line
     * @return the width of the line
     */    
    public static float GetWidth(Phrase phrase) {
        return GetWidth(phrase, PdfWriter.RUN_DIRECTION_NO_BIDI, 0);
    }
    
    /** Shows a line of text. Only the first line is written.
     * @param canvas where the text is to be written to
     * @param alignment the alignment. It is not influenced by the run direction
     * @param phrase the <CODE>Phrase</CODE> with the text
     * @param x the x reference position
     * @param y the y reference position
     * @param rotation the rotation to be applied in degrees counterclockwise
     * @param runDirection the run direction
     * @param arabicOptions the options for the arabic shaping
     */    
    public static void ShowTextAligned(PdfContentByte canvas, int alignment, Phrase phrase, float x, float y, float rotation, int runDirection, int arabicOptions) {
        if (alignment != Element.ALIGN_LEFT && alignment != Element.ALIGN_CENTER
            && alignment != Element.ALIGN_RIGHT)
            alignment = Element.ALIGN_LEFT;
        canvas.SaveState();
        ColumnText ct = new ColumnText(canvas);
        if (rotation == 0) {
            if (alignment == Element.ALIGN_LEFT)
                ct.SetSimpleColumn(phrase, x, y - 1, 20000 + x, y + 2, 2, alignment);
            else if (alignment == Element.ALIGN_RIGHT)
                ct.SetSimpleColumn(phrase, x-20000, y-1, x, y+2, 2, alignment);
            else
                ct.SetSimpleColumn(phrase, x-20000, y-1, x+20000, y+2, 2, alignment);
        }
        else {
            double alpha = rotation * Math.PI / 180.0;
            float cos = (float)Math.Cos(alpha);
            float sin = (float)Math.Sin(alpha);
            canvas.ConcatCTM(cos, sin, -sin, cos, x, y);
            if (alignment == Element.ALIGN_LEFT)
                ct.SetSimpleColumn(phrase, 0, -1, 20000, 2, 2, alignment);
            else if (alignment == Element.ALIGN_RIGHT)
                ct.SetSimpleColumn(phrase, -20000, -1, 0, 2, 2, alignment);
            else
                ct.SetSimpleColumn(phrase, -20000, -1, 20000, 2, 2, alignment);
        }
        if (runDirection == PdfWriter.RUN_DIRECTION_RTL) {
            if (alignment == Element.ALIGN_LEFT)
                alignment = Element.ALIGN_RIGHT;
            else if (alignment == Element.ALIGN_RIGHT)
                alignment = Element.ALIGN_LEFT;
        }
        ct.Alignment = alignment;
        ct.ArabicOptions = arabicOptions;
        ct.RunDirection = runDirection;
        ct.Go();
        canvas.RestoreState();
    }

    /** Shows a line of text. Only the first line is written.
     * @param canvas where the text is to be written to
     * @param alignment the alignment
     * @param phrase the <CODE>Phrase</CODE> with the text
     * @param x the x reference position
     * @param y the y reference position
     * @param rotation the rotation to be applied in degrees counterclockwise
     */    
    public static void ShowTextAligned(PdfContentByte canvas, int alignment, Phrase phrase, float x, float y, float rotation) {
        ShowTextAligned(canvas, alignment, phrase, x, y, rotation, PdfWriter.RUN_DIRECTION_NO_BIDI, 0);
    }

    protected int GoComposite(bool simulate) {
        if (!rectangularMode)
            throw new DocumentException("Irregular columns are not supported in composite mode.");
        linesWritten = 0;
        descender = 0;
        bool firstPass = adjustFirstLine;
        main_loop:
        while (true) {
            if (compositeElements.Count == 0)
                return NO_MORE_TEXT;
            IElement element = (IElement)compositeElements[0];
            if (element.Type == Element.PARAGRAPH) {
                Paragraph para = (Paragraph)element;
                int status = 0;
                for (int keep = 0; keep < 2; ++keep) {
                    float lastY = yLine;
                    bool createHere = false;
                    if (compositeColumn == null) {
                        compositeColumn = new ColumnText(canvas);
                        compositeColumn.UseAscender = (firstPass ? useAscender : false);
                        compositeColumn.Alignment = para.Alignment;
                        compositeColumn.Indent = para.IndentationLeft + para.FirstLineIndent;
                        compositeColumn.ExtraParagraphSpace = para.ExtraParagraphSpace;
                        compositeColumn.FollowingIndent = para.IndentationLeft;
                        compositeColumn.RightIndent = para.IndentationRight;
                        compositeColumn.SetLeading(para.Leading, para.MultipliedLeading);
                        compositeColumn.RunDirection = runDirection;
                        compositeColumn.ArabicOptions = arabicOptions;
                        compositeColumn.SpaceCharRatio = spaceCharRatio;
                        compositeColumn.AddText(para);
                        if (!firstPass) {
                            yLine -= para.SpacingBefore;
                        }
                        createHere = true;
                    }
                    compositeColumn.leftX = leftX;
                    compositeColumn.rightX = rightX;
                    compositeColumn.yLine = yLine;
                    compositeColumn.rectangularWidth = rectangularWidth;
                    compositeColumn.rectangularMode = rectangularMode;
                    compositeColumn.minY = minY;
                    compositeColumn.maxY = maxY;
                    bool keepCandidate = (para.KeepTogether && createHere && !firstPass);
                    status = compositeColumn.Go(simulate || (keepCandidate && keep == 0));
                    UpdateFilledWidth(compositeColumn.filledWidth);
                    if ((status & NO_MORE_TEXT) == 0 && keepCandidate) {
                        compositeColumn = null;
                        yLine = lastY;
                        return NO_MORE_COLUMN;
                    }
                    if (simulate || !keepCandidate)
                        break;
                    if (keep == 0) {
                        compositeColumn = null;
                        yLine = lastY;
                    }
                }
                firstPass = false;
                yLine = compositeColumn.yLine;
                linesWritten += compositeColumn.linesWritten;
                descender = compositeColumn.descender;
                if ((status & NO_MORE_TEXT) != 0) {
                    compositeColumn = null;
                    compositeElements.RemoveAt(0);
                    yLine -= para.SpacingAfter;
                }
                if ((status & NO_MORE_COLUMN) != 0) {
                    return NO_MORE_COLUMN;
                }
            }
            else if (element.Type == Element.LIST) {
                List list = (List)element;
                ArrayList items = list.Items;
                ListItem item = null;
                float listIndentation = list.IndentationLeft;
                int count = 0;
                Stack stack = new Stack();
                for (int k = 0; k < items.Count; ++k) {
                    Object obj = items[k];
                    if (obj is ListItem) {
                        if (count == listIdx) {
                            item = (ListItem)obj;
                            break;
                        }
                        else ++count;
                    }
                    else if (obj is List) {
                        stack.Push(new Object[]{list, k, listIndentation});
                        list = (List)obj;
                        items = list.Items;
                        listIndentation += list.IndentationLeft;
                        k = -1;
                        continue;
                    }
                    if (k == items.Count - 1) {
                        if (stack.Count > 0) {
                            Object[] objs = (Object[])stack.Pop();
                            list = (List)objs[0];
                            items = list.Items;
                            k = (int)objs[1];
                            listIndentation = (float)objs[2];
                        }
                    }
                }
                int status = 0;
                for (int keep = 0; keep < 2; ++keep) {
                    float lastY = yLine;
                    bool createHere = false;
                    if (compositeColumn == null) {
                        if (item == null) {
                            listIdx = 0;
                            compositeElements.RemoveAt(0);
                            goto main_loop;
                        }
                        compositeColumn = new ColumnText(canvas);

                        compositeColumn.UseAscender = (firstPass ? useAscender : false);
                        compositeColumn.Alignment = item.Alignment;
                        compositeColumn.Indent = item.IndentationLeft + listIndentation + item.FirstLineIndent;
                        compositeColumn.ExtraParagraphSpace = item.ExtraParagraphSpace;
                        compositeColumn.FollowingIndent = compositeColumn.Indent;
                        compositeColumn.RightIndent = item.IndentationRight + list.IndentationRight;
                        compositeColumn.SetLeading(item.Leading, item.MultipliedLeading);
                        compositeColumn.RunDirection = runDirection;
                        compositeColumn.ArabicOptions = arabicOptions;
                        compositeColumn.SpaceCharRatio = spaceCharRatio;
                        compositeColumn.AddText(item);
                        if (!firstPass) {
                            yLine -= item.SpacingBefore;
                        }
                        createHere = true;
                    }
                    compositeColumn.leftX = leftX;
                    compositeColumn.rightX = rightX;
                    compositeColumn.yLine = yLine;
                    compositeColumn.rectangularWidth = rectangularWidth;
                    compositeColumn.rectangularMode = rectangularMode;
                    compositeColumn.minY = minY;
                    compositeColumn.maxY = maxY;
                    bool keepCandidate = (item.KeepTogether && createHere && !firstPass);
                    status = compositeColumn.Go(simulate || (keepCandidate && keep == 0));
                    UpdateFilledWidth(compositeColumn.filledWidth);
                    if ((status & NO_MORE_TEXT) == 0 && keepCandidate) {
                        compositeColumn = null;
                        yLine = lastY;
                        return NO_MORE_COLUMN;
                    }
                    if (simulate || !keepCandidate)
                        break;
                    if (keep == 0) {
                        compositeColumn = null;
                        yLine = lastY;
                    }
                }
                firstPass = false;
                yLine = compositeColumn.yLine;
                linesWritten += compositeColumn.linesWritten;
                descender = compositeColumn.descender;
                if (!float.IsNaN(compositeColumn.firstLineY) && !compositeColumn.firstLineYDone) {
                    if (!simulate)
                        ShowTextAligned(canvas, Element.ALIGN_LEFT, new Phrase(item.ListSymbol), compositeColumn.leftX + listIndentation, compositeColumn.firstLineY, 0);
                    compositeColumn.firstLineYDone = true;
                }
                if ((status & NO_MORE_TEXT) != 0) {
                    compositeColumn = null;
                    ++listIdx;
                    yLine -= item.SpacingAfter;
                }
                if ((status & NO_MORE_COLUMN) != 0) {
                    return NO_MORE_COLUMN;
                }
            }
            else if (element.Type == Element.PTABLE) {
                // don't write anything in the current column if there's no more space available
                if (yLine < minY || yLine > maxY)
                    return NO_MORE_COLUMN;
                
                // get the PdfPTable element
                PdfPTable table = (PdfPTable)element;
                
                // we ignore tables without a body
                if (table.Size <= table.HeaderRows) {
                    compositeElements.RemoveAt(0);
                    continue;
                }
                
                // offsets
                float yTemp = yLine;
                if (!firstPass && listIdx == 0) {
                    yTemp -= table.SpacingBefore;
                }
                float yLineWrite = yTemp;
                
                // don't write anything in the current column if there's no more space available
                if (yTemp < minY || yTemp > maxY) {
                    return NO_MORE_COLUMN;
                }
                
                // coordinates
                currentLeading = 0;
                float x1 = leftX;
                float tableWidth;
                if (table.LockedWidth) {
                    tableWidth = table.TotalWidth;
                    UpdateFilledWidth(tableWidth);
                }
                else {
                    tableWidth = rectangularWidth * table.WidthPercentage / 100f;
                    table.TotalWidth = tableWidth;
                }
                
                // how many header rows are real header rows; how many are footer rows?
                int headerRows = table.HeaderRows;
                int footerRows = table.FooterRows;
                if (footerRows > headerRows)
                    footerRows = headerRows;
                int realHeaderRows = headerRows - footerRows;
                float headerHeight = table.HeaderHeight;
                float footerHeight = table.FooterHeight;

                // make sure the header and footer fit on the page
                bool skipHeader = (!firstPass && table.SkipFirstHeader && listIdx <= headerRows);
                if (!skipHeader) {
                    yTemp -= headerHeight;
                    if (yTemp < minY || yTemp > maxY) {
                        if (firstPass) {
                            compositeElements.RemoveAt(0);
                            continue;
                        }
                        return NO_MORE_COLUMN;
                    }
                }
                
                // how many real rows (not header or footer rows) fit on a page?
                int k;
                if (listIdx < headerRows) {
                    listIdx = headerRows;
                }
                if (!table.ElementComplete) {
                    yTemp -= footerHeight;
                }
                for (k = listIdx; k < table.Size; ++k) {
                    float rowHeight = table.GetRowHeight(k);
                    if (yTemp - rowHeight < minY)
                        break;
                    yTemp -= rowHeight;
                }
                if (!table.ElementComplete) {
                    yTemp += footerHeight;
                }
                // either k is the first row that doesn't fit on the page (break);
                if (k < table.Size) {
                    if (table.SplitRows && (!table.SplitLate || (k == listIdx && firstPass))) {
                        if (!splittedRow) {
                            splittedRow = true;
                            table = new PdfPTable(table);
                            compositeElements[0] = table;
                            ArrayList rows = table.Rows;
                            for (int i = headerRows; i < listIdx; ++i)
                                rows[i] = null;
                        }
                        float h = yTemp - minY;
                        PdfPRow newRow = table.GetRow(k).SplitRow(h);
                        if (newRow == null) {
                            if (k == listIdx) {
                                return NO_MORE_COLUMN;
                            }
                        }
                        else {
                            yTemp = minY;
                            table.Rows.Insert(++k, newRow);
                        }
                    }
                    else if (!table.SplitRows && k == listIdx && firstPass) {
                        compositeElements.RemoveAt(0);
                        splittedRow = false;
                        continue;
                    }
                    else if (k == listIdx && !firstPass && (!table.SplitRows || table.SplitLate) && (table.FooterRows == 0 || table.ElementComplete)) {
                        return NO_MORE_COLUMN;
                    }
                }
                // or k is the number of rows in the table (for loop was done).
                firstPass = false;
                // we draw the table (for real now)
                if (!simulate) {
                    // set the alignment
                    switch (table.HorizontalAlignment) {
                        case Element.ALIGN_LEFT:
                            break;
                        case Element.ALIGN_RIGHT:
                            x1 += rectangularWidth - tableWidth;
                            break;
                        default:
                            x1 += (rectangularWidth - tableWidth) / 2f;
                            break;
                    }
                    // copy the rows that fit on the page in a new table nt
                    PdfPTable nt = PdfPTable.ShallowCopy(table);
                    ArrayList rows = table.Rows;
                    ArrayList sub = nt.Rows;
                    
                    // first we add the real header rows (if necessary)
                    if (!skipHeader) {
                        for (int j = 0; j < realHeaderRows; ++j) {
                            PdfPRow headerRow = (PdfPRow)rows[j];
                            sub.Add(headerRow);
                        }
                    }
                    else {
                        nt.HeaderRows = footerRows;
                    }
                    // then we add the real content
                    for (int j = listIdx; j < k; ++j) {
                        sub.Add(rows[j]);
                    }
                    // if k < table.size(), we must indicate that the new table is complete;
                    // otherwise no footers will be added (because iText thinks the table continues on the same page)
                    if (k < table.Size) {
                        nt.ElementComplete = true;
                    }
                    // we add the footer rows if necessary (not for incomplete tables)
                    for (int j = 0; j < footerRows && nt.ElementComplete; ++j) {
                        sub.Add(rows[j + realHeaderRows]);
                    }

                    // we need a correction if the last row needs to be extended
                    float rowHeight = 0;
                    if (table.ExtendLastRow) {
                        PdfPRow last = (PdfPRow)sub[sub.Count - 1 - footerRows];
                        rowHeight = last.MaxHeights;
                        last.MaxHeights = yTemp - minY + rowHeight;
                        yTemp = minY;
                    }
                    
                    // now we render the rows of the new table
                    if (canvases != null)
                        nt.WriteSelectedRows(0, -1, x1, yLineWrite, canvases);
                    else
                        nt.WriteSelectedRows(0, -1, x1, yLineWrite, canvas);
                    if (table.ExtendLastRow) {
                        PdfPRow last = (PdfPRow)sub[sub.Count - 1 - footerRows];
                        last.MaxHeights = rowHeight;
                    }
                }
                else if (table.ExtendLastRow && minY > PdfPRow.BOTTOM_LIMIT) {
                    yTemp = minY;
                }
                yLine = yTemp;
                if (!(skipHeader || table.ElementComplete)) {
                    yLine += footerHeight;
                }
                if (k >= table.Size) {
                    yLine -= table.SpacingAfter;
                    compositeElements.RemoveAt(0);
                    splittedRow = false;
                    listIdx = 0;
                }
                else {
                    if (splittedRow) {
                        ArrayList rows = table.Rows;
                        for (int i = listIdx; i < k; ++i)
                            rows[i] = null;
                    }
                    listIdx = k;
                    return NO_MORE_COLUMN;
                }
            }
            else if (element.Type == Element.YMARK) {
                if (!simulate) {
                    IDrawInterface zh = (IDrawInterface)element;
                    zh.Draw(canvas, leftX, minY, rightX, maxY, yLine);
                }
                compositeElements.RemoveAt(0);
            }
            else
                compositeElements.RemoveAt(0);
        }
    }
    
    /**
     * Sets the canvas.
     * @param canvas
     */
    public PdfContentByte Canvas {
        set {
            canvas = value;
            canvases = null;
            if (compositeColumn != null)
                compositeColumn.Canvas = value;
        }
        get {
            return canvas;
        }
    }
    
    /**
     * Sets the canvases.
     * @param canvas
     */
    public PdfContentByte[] Canvases {
        set {
            canvases = value;
            canvas = canvases[PdfPTable.TEXTCANVAS];
            if (compositeColumn != null)
                compositeColumn.Canvases = canvases;
        }
        get {
            return canvases;
        }
    }
    
    /**
     * Checks if the element has a height of 0.
     * @return true or false
     * @since 2.1.2
     */
    public bool ZeroHeightElement() {
        return composite && compositeElements.Count != 0 && ((IElement)compositeElements[0]).Type == Element.YMARK;
    }

    /**
     * Enables/Disables adjustment of first line height based on max ascender.
     * @param use enable adjustment if true
     */
    public bool UseAscender {
        set {
            useAscender = value;
        }
        get {
            return useAscender;
        }
    }

    /**
     * Checks the status variable and looks if there's still some text.
     */
    public static bool HasMoreText(int status) {
        return (status & ColumnText.NO_MORE_TEXT) == 0;
    }
    /**
     * Holds value of property filledWidth.
     */
    private float filledWidth;

    /**
     * Sets the real width used by the largest line. Only used to set it
     * to zero to start another measurement.
     * @param filledWidth the real width used by the largest line
     */
    public float FilledWidth {
        set {
            filledWidth = value;
        }
        get {
            return filledWidth;
        }
    }
    
    /**
     * Replaces the <CODE>filledWidth</CODE> if greater than the existing one.
     * @param w the new <CODE>filledWidth</CODE> if greater than the existing one
     */
    public void UpdateFilledWidth(float w) {
        if (w > filledWidth)
            filledWidth = w;
    }

    private bool adjustFirstLine = true;

    /**
     * Sets the first line adjustment. Some objects have properties, like spacing before, that
     * behave differently if the object is the first to be written after go() or not. The first line adjustment is 
     * <CODE>true</CODE> by default but can be changed if several objects are to be placed one
     * after the other in the same column calling go() several times.
     * @param adjustFirstLine <CODE>true</CODE> to adjust the first line, <CODE>false</CODE> otherwise
     */
    public bool AdjustFirstLine {
        set {
            adjustFirstLine = value;
        }
        get {
            return adjustFirstLine;
        }
    }
}
}