using System;
using System.Collections;
using System.Text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.intern;

/*
 * $Id: PdfContentByte.cs,v 1.23 2008/05/13 11:25:19 psoares33 Exp $
 * 
 *
 * Copyright 1999, 2000, 2001, 2002 Bruno Lowagie
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
     * <CODE>PdfContentByte</CODE> is an object containing the user positioned
     * text and graphic contents of a page. It knows how to apply the proper
     * font encoding.
     */

    public class PdfContentByte {
    
        /**
         * This class keeps the graphic state of the current page
         */
    
        public class GraphicState {
        
            /** This is the font in use */
            internal FontDetails fontDetails;
        
            /** This is the color in use */
            internal ColorDetails colorDetails;
        
            /** This is the font size in use */
            internal float size;
        
            /** The x position of the text line matrix. */
            protected internal float xTLM = 0;
            /** The y position of the text line matrix. */
            protected internal float yTLM = 0;        
            /** The current text leading. */
            protected internal float leading = 0;

            /** The current horizontal scaling */
            protected internal float scale = 100;

            /** The current character spacing */
            protected internal float charSpace = 0;

            /** The current word spacing */
            protected internal float wordSpace = 0;

            internal GraphicState() {
            }

            internal GraphicState(GraphicState cp) {
                fontDetails = cp.fontDetails;
                colorDetails = cp.colorDetails;
                size = cp.size;
                xTLM = cp.xTLM;
                yTLM = cp.yTLM;
                leading = cp.leading;
                scale = cp.scale;
                charSpace = cp.charSpace;
                wordSpace = cp.wordSpace;
            }
        }
    
        /** The alignement is center */
        public const int ALIGN_CENTER = Element.ALIGN_CENTER;
        
        /** The alignement is left */
        public const int ALIGN_LEFT = Element.ALIGN_LEFT;
        
        /** The alignement is right */
        public const int ALIGN_RIGHT = Element.ALIGN_RIGHT;

        /** A possible line cap value */
        public const int LINE_CAP_BUTT = 0;
        /** A possible line cap value */
        public const int LINE_CAP_ROUND = 1;
        /** A possible line cap value */
        public const int LINE_CAP_PROJECTING_SQUARE = 2;
        
        /** A possible line join value */
        public const int LINE_JOIN_MITER = 0;
        /** A possible line join value */
        public const int LINE_JOIN_ROUND = 1;
        /** A possible line join value */
        public const int LINE_JOIN_BEVEL = 2;

        /** A possible text rendering value */
        public const int TEXT_RENDER_MODE_FILL = 0;
        /** A possible text rendering value */
        public const int TEXT_RENDER_MODE_STROKE = 1;
        /** A possible text rendering value */
        public const int TEXT_RENDER_MODE_FILL_STROKE = 2;
        /** A possible text rendering value */
        public const int TEXT_RENDER_MODE_INVISIBLE = 3;
        /** A possible text rendering value */
        public const int TEXT_RENDER_MODE_FILL_CLIP = 4;
        /** A possible text rendering value */
        public const int TEXT_RENDER_MODE_STROKE_CLIP = 5;
        /** A possible text rendering value */
        public const int TEXT_RENDER_MODE_FILL_STROKE_CLIP = 6;
        /** A possible text rendering value */
        public const int TEXT_RENDER_MODE_CLIP = 7;
    
        private static float[] unitRect = {0, 0, 0, 1, 1, 0, 1, 1};
        // membervariables
    
        /** This is the actual content */
        protected ByteBuffer content = new ByteBuffer();
    
        /** This is the writer */
        protected PdfWriter writer;
    
        /** This is the PdfDocument */
        protected PdfDocument pdf;
    
        /** This is the GraphicState in use */
        protected GraphicState state = new GraphicState();
    
        /** The list were we save/restore the layer depth */
        protected ArrayList layerDepth;
        
        /** The list were we save/restore the state */
        protected ArrayList stateList = new ArrayList();
    
        /** The separator between commands.
         */    
        protected int separator = '\n';
    
        private static Hashtable abrev = new Hashtable();
        
        static PdfContentByte() {
            abrev[PdfName.BITSPERCOMPONENT] = "/BPC ";
            abrev[PdfName.COLORSPACE] = "/CS ";
            abrev[PdfName.DECODE] = "/D ";
            abrev[PdfName.DECODEPARMS] = "/DP ";
            abrev[PdfName.FILTER] = "/F ";
            abrev[PdfName.HEIGHT] = "/H ";
            abrev[PdfName.IMAGEMASK] = "/IM ";
            abrev[PdfName.INTENT] = "/Intent ";
            abrev[PdfName.INTERPOLATE] = "/I ";
            abrev[PdfName.WIDTH] = "/W ";
        }
        
        // constructors
    
        /**
         * Constructs a new <CODE>PdfContentByte</CODE>-object.
         *
         * @param wr the writer associated to this content
         */
    
        public PdfContentByte(PdfWriter wr) {
            if (wr != null) {
                writer = wr;
                pdf = writer.PdfDocument;
            }
        }
    
        // methods to get the content of this object
    
        /**
         * Returns the <CODE>string</CODE> representation of this <CODE>PdfContentByte</CODE>-object.
         *
         * @return      a <CODE>string</CODE>
         */
    
        public override string ToString() {
            return content.ToString();
        }
    
        /**
         * Gets the internal buffer.
         * @return the internal buffer
         */
        public ByteBuffer InternalBuffer {
            get {
                return content;
            }
        }
    
        /** Returns the PDF representation of this <CODE>PdfContentByte</CODE>-object.
         *
         * @param writer the <CODE>PdfWriter</CODE>
         * @return a <CODE>byte</CODE> array with the representation
         */
    
        public byte[] ToPdf(PdfWriter writer) {
            return content.ToByteArray();
        }
    
        // methods to add graphical content
    
        /**
         * Adds the content of another <CODE>PdfContent</CODE>-object to this object.
         *
         * @param       other       another <CODE>PdfByteContent</CODE>-object
         */
    
        public void Add(PdfContentByte other) {
            if (other.writer != null && writer != other.writer)
                throw new Exception("Inconsistent writers. Are you mixing two documents?");
            content.Append(other.content);
        }
    
        /**
         * Gets the x position of the text line matrix.
         *
         * @return the x position of the text line matrix
         */
        public float XTLM {
            get {
                return state.xTLM;
            }
        }
    
        /**
         * Gets the y position of the text line matrix.
         *
         * @return the y position of the text line matrix
         */
        public float YTLM {
            get {
                return state.yTLM;
            }
        }
    
        /**
        * Gets the current character spacing.
        *
        * @return the current character spacing
        */
        public float CharacterSpacing {
            get {
                return state.charSpace;
            }
        }

        /**
        * Gets the current word spacing.
        *
        * @return the current word spacing
        */
        public float WordSpacing {
            get {
                return state.wordSpace;
            }
        }

        /**
        * Gets the current character spacing.
        *
        * @return the current character spacing
        */
        public float HorizontalScaling {
            get {
                return state.scale;
            }
        }

        /**
         * Gets the current text leading.
         *
         * @return the current text leading
         */
        public float Leading {
            get {
                return state.leading;
            }
        }

        public void SetLeading(float v) {
            state.leading = v;
            content.Append(v).Append(" TL").Append_i(separator);
        }
    
        /**
         * Changes the <VAR>Flatness</VAR>.
         * <P>
         * <VAR>Flatness</VAR> sets the maximum permitted distance in device pixels between the
         * mathematically correct path and an approximation constructed from straight line segments.<BR>
         *
         * @param       flatness        a value
         */
    
        public void SetFlatness(float value) {
            if (value >= 0 && value <= 100) {
                content.Append(value).Append(" i").Append_i(separator);
            }
        }
    
        /**
         * Changes the <VAR>Line cap style</VAR>.
         * <P>
         * The <VAR>line cap style</VAR> specifies the shape to be used at the end of open subpaths
         * when they are stroked.<BR>
         * Allowed values are 0 (Butt end caps), 1 (Round end caps) and 2 (Projecting square end caps).<BR>
         *
         * @param       style       a value
         */
    
        public void SetLineCap(int value) {
            if (value >= 0 && value <= 2) {
                content.Append(value).Append(" J").Append_i(separator);
            }
        }
    
        /**
         * Changes the value of the <VAR>line dash pattern</VAR>.
         * <P>
         * The line dash pattern controls the pattern of dashes and gaps used to stroke paths.
         * It is specified by an <I>array</I> and a <I>phase</I>. The array specifies the length
         * of the alternating dashes and gaps. The phase specifies the distance into the dash
         * pattern to start the dash.<BR>
         *
         * @param       phase       the value of the phase
         */
    
        public void SetLineDash(float value) {
            content.Append("[] ").Append(value).Append(" d").Append_i(separator);
        }
    
        /**
         * Changes the value of the <VAR>line dash pattern</VAR>.
         * <P>
         * The line dash pattern controls the pattern of dashes and gaps used to stroke paths.
         * It is specified by an <I>array</I> and a <I>phase</I>. The array specifies the length
         * of the alternating dashes and gaps. The phase specifies the distance into the dash
         * pattern to start the dash.<BR>
         *
         * @param       phase       the value of the phase
         * @param       unitsOn     the number of units that must be 'on' (equals the number of units that must be 'off').
         */
    
        public void SetLineDash(float unitsOn, float phase) {
            content.Append('[').Append(unitsOn).Append("] ").Append(phase).Append(" d").Append_i(separator);
        }
    
        /**
         * Changes the value of the <VAR>line dash pattern</VAR>.
         * <P>
         * The line dash pattern controls the pattern of dashes and gaps used to stroke paths.
         * It is specified by an <I>array</I> and a <I>phase</I>. The array specifies the length
         * of the alternating dashes and gaps. The phase specifies the distance into the dash
         * pattern to start the dash.<BR>
         *
         * @param       phase       the value of the phase
         * @param       unitsOn     the number of units that must be 'on'
         * @param       unitsOff    the number of units that must be 'off'
         */
    
        public void SetLineDash(float unitsOn, float unitsOff, float phase) {
            content.Append('[').Append(unitsOn).Append(' ').Append(unitsOff).Append("] ").Append(phase).Append(" d").Append_i(separator);
        }
    
        /**
        * Changes the value of the <VAR>line dash pattern</VAR>.
        * <P>
        * The line dash pattern controls the pattern of dashes and gaps used to stroke paths.
        * It is specified by an <I>array</I> and a <I>phase</I>. The array specifies the length
        * of the alternating dashes and gaps. The phase specifies the distance into the dash
        * pattern to start the dash.<BR>
        *
        * @param        array        length of the alternating dashes and gaps
        * @param        phase        the value of the phase
        */
        
        public void SetLineDash(float[] array, float phase) {
            content.Append('[');
            for (int i = 0; i < array.Length; i++) {
                content.Append(array[i]);
                if (i < array.Length - 1) content.Append(' ');
            }
            content.Append("] ").Append(phase).Append(" d").Append_i(separator);
        }

        /**
         * Changes the <VAR>Line join style</VAR>.
         * <P>
         * The <VAR>line join style</VAR> specifies the shape to be used at the corners of paths
         * that are stroked.<BR>
         * Allowed values are 0 (Miter joins), 1 (Round joins) and 2 (Bevel joins).<BR>
         *
         * @param       style       a value
         */
    
        public void SetLineJoin(int value) {
            if (value >= 0 && value <= 2) {
                content.Append(value).Append(" j").Append_i(separator);
            }
        }
    
        /**
         * Changes the <VAR>line width</VAR>.
         * <P>
         * The line width specifies the thickness of the line used to stroke a path and is measured
         * in used space units.<BR>
         *
         * @param       w           a width
         */
    
        public void SetLineWidth(float value) {
            content.Append(value).Append(" w").Append_i(separator);
        }
    
        /**
         * Changes the <VAR>Miter limit</VAR>.
         * <P>
         * When two line segments meet at a sharp angle and mitered joins have been specified as the
         * line join style, it is possible for the miter to extend far beyond the thickness of the line
         * stroking path. The miter limit imposes a maximum on the ratio of the miter length to the line
         * witdh. When the limit is exceeded, the join is converted from a miter to a bevel.<BR>
         *
         * @param       miterLimit      a miter limit
         */
    
        public void SetMiterLimit(float value) {
            if (value > 1) {
                content.Append(value).Append(" M").Append_i(separator);
            }
        }
    
        /**
         * Modify the current clipping path by intersecting it with the current path, using the
         * nonzero winding number rule to determine which regions lie inside the clipping
         * path.
         */
    
        public void Clip() {
            content.Append('W').Append_i(separator);
        }
    
        /**
         * Modify the current clipping path by intersecting it with the current path, using the
         * even-odd rule to determine which regions lie inside the clipping path.
         */
    
        public void EoClip() {
            content.Append("W*").Append_i(separator);
        }
    
        /**
         * Changes the currentgray tint for filling paths (device dependent colors!).
         * <P>
         * Sets the color space to <B>DeviceGray</B> (or the <B>DefaultGray</B> color space),
         * and sets the gray tint to use for filling paths.</P>
         *
         * @param   gray    a value between 0 (black) and 1 (white)
         */
    
        public virtual void SetGrayFill(float value) {
            content.Append(value).Append(" g").Append_i(separator);
        }
    
        /**
         * Changes the current gray tint for filling paths to black.
         */
    
        public virtual void ResetGrayFill() {
            content.Append("0 g").Append_i(separator);
        }
    
        /**
         * Changes the currentgray tint for stroking paths (device dependent colors!).
         * <P>
         * Sets the color space to <B>DeviceGray</B> (or the <B>DefaultGray</B> color space),
         * and sets the gray tint to use for stroking paths.</P>
         *
         * @param   gray    a value between 0 (black) and 1 (white)
         */
    
        public virtual void SetGrayStroke(float value) {
            content.Append(value).Append(" G").Append_i(separator);
        }
    
        /**
         * Changes the current gray tint for stroking paths to black.
         */
    
        public virtual void ResetGrayStroke() {
            content.Append("0 G").Append_i(separator);
        }
    
        /**
         * Helper to validate and write the RGB color components
         * @param   red     the intensity of red. A value between 0 and 1
         * @param   green   the intensity of green. A value between 0 and 1
         * @param   blue    the intensity of blue. A value between 0 and 1
         */
        private void HelperRGB(float red, float green, float blue) {
            PdfXConformanceImp.CheckPDFXConformance(writer, PdfXConformanceImp.PDFXKEY_RGB, null);
            if (red < 0)
                red = 0.0f;
            else if (red > 1.0f)
                red = 1.0f;
            if (green < 0)
                green = 0.0f;
            else if (green > 1.0f)
                green = 1.0f;
            if (blue < 0)
                blue = 0.0f;
            else if (blue > 1.0f)
                blue = 1.0f;
            content.Append(red).Append(' ').Append(green).Append(' ').Append(blue);
        }
    
        /**
         * Changes the current color for filling paths (device dependent colors!).
         * <P>
         * Sets the color space to <B>DeviceRGB</B> (or the <B>DefaultRGB</B> color space),
         * and sets the color to use for filling paths.</P>
         * <P>
         * Following the PDF manual, each operand must be a number between 0 (minimum intensity) and
         * 1 (maximum intensity).</P>
         *
         * @param   red     the intensity of red. A value between 0 and 1
         * @param   green   the intensity of green. A value between 0 and 1
         * @param   blue    the intensity of blue. A value between 0 and 1
         */
    
        public virtual void SetRGBColorFillF(float red, float green, float blue) {
            HelperRGB(red, green, blue);
            content.Append(" rg").Append_i(separator);
        }
    
        /**
         * Changes the current color for filling paths to black.
         */
    
        public virtual void ResetRGBColorFill() {
            content.Append("0 g").Append_i(separator);
        }
    
        /**
         * Changes the current color for stroking paths (device dependent colors!).
         * <P>
         * Sets the color space to <B>DeviceRGB</B> (or the <B>DefaultRGB</B> color space),
         * and sets the color to use for stroking paths.</P>
         * <P>
         * Following the PDF manual, each operand must be a number between 0 (miniumum intensity) and
         * 1 (maximum intensity).
         *
         * @param   red     the intensity of red. A value between 0 and 1
         * @param   green   the intensity of green. A value between 0 and 1
         * @param   blue    the intensity of blue. A value between 0 and 1
         */
    
        public virtual void SetRGBColorStrokeF(float red, float green, float blue) {
            HelperRGB(red, green, blue);
            content.Append(" RG").Append_i(separator);
        }
    
        /**
         * Changes the current color for stroking paths to black.
         *
         */
    
        public virtual void ResetRGBColorStroke() {
            content.Append("0 G").Append_i(separator);
        }
    
        /**
         * Helper to validate and write the CMYK color components.
         *
         * @param   cyan    the intensity of cyan. A value between 0 and 1
         * @param   magenta the intensity of magenta. A value between 0 and 1
         * @param   yellow  the intensity of yellow. A value between 0 and 1
         * @param   black   the intensity of black. A value between 0 and 1
         */
        private void HelperCMYK(float cyan, float magenta, float yellow, float black) {
            if (cyan < 0)
                cyan = 0.0f;
            else if (cyan > 1.0f)
                cyan = 1.0f;
            if (magenta < 0)
                magenta = 0.0f;
            else if (magenta > 1.0f)
                magenta = 1.0f;
            if (yellow < 0)
                yellow = 0.0f;
            else if (yellow > 1.0f)
                yellow = 1.0f;
            if (black < 0)
                black = 0.0f;
            else if (black > 1.0f)
                black = 1.0f;
            content.Append(cyan).Append(' ').Append(magenta).Append(' ').Append(yellow).Append(' ').Append(black);
        }
    
        /**
         * Changes the current color for filling paths (device dependent colors!).
         * <P>
         * Sets the color space to <B>DeviceCMYK</B> (or the <B>DefaultCMYK</B> color space),
         * and sets the color to use for filling paths.</P>
         * <P>
         * Following the PDF manual, each operand must be a number between 0 (no ink) and
         * 1 (maximum ink).</P>
         *
         * @param   cyan    the intensity of cyan. A value between 0 and 1
         * @param   magenta the intensity of magenta. A value between 0 and 1
         * @param   yellow  the intensity of yellow. A value between 0 and 1
         * @param   black   the intensity of black. A value between 0 and 1
         */
    
        public virtual void SetCMYKColorFillF(float cyan, float magenta, float yellow, float black) {
            HelperCMYK(cyan, magenta, yellow, black);
            content.Append(" k").Append_i(separator);
        }
    
        /**
         * Changes the current color for filling paths to black.
         *
         */
    
        public virtual void ResetCMYKColorFill() {
            content.Append("0 0 0 1 k").Append_i(separator);
        }
    
        /**
         * Changes the current color for stroking paths (device dependent colors!).
         * <P>
         * Sets the color space to <B>DeviceCMYK</B> (or the <B>DefaultCMYK</B> color space),
         * and sets the color to use for stroking paths.</P>
         * <P>
         * Following the PDF manual, each operand must be a number between 0 (miniumum intensity) and
         * 1 (maximum intensity).
         *
         * @param   cyan    the intensity of cyan. A value between 0 and 1
         * @param   magenta the intensity of magenta. A value between 0 and 1
         * @param   yellow  the intensity of yellow. A value between 0 and 1
         * @param   black   the intensity of black. A value between 0 and 1
         */
    
        public virtual void SetCMYKColorStrokeF(float cyan, float magenta, float yellow, float black) {
            HelperCMYK(cyan, magenta, yellow, black);
            content.Append(" K").Append_i(separator);
        }
    
        /**
         * Changes the current color for stroking paths to black.
         *
         */
    
        public virtual void ResetCMYKColorStroke() {
            content.Append("0 0 0 1 K").Append_i(separator);
        }
    
        /**
         * Move the current point <I>(x, y)</I>, omitting any connecting line segment.
         *
         * @param       x               new x-coordinate
         * @param       y               new y-coordinate
         */
    
        public void MoveTo(float x, float y) {
            content.Append(x).Append(' ').Append(y).Append(" m").Append_i(separator);
        }
    
        /**
         * Appends a straight line segment from the current point <I>(x, y)</I>. The new current
         * point is <I>(x, y)</I>.
         *
         * @param       x               new x-coordinate
         * @param       y               new y-coordinate
         */
    
        public void LineTo(float x, float y) {
            content.Append(x).Append(' ').Append(y).Append(" l").Append_i(separator);
        }
    
        /**
         * Appends a Bêzier curve to the path, starting from the current point.
         *
         * @param       x1      x-coordinate of the first control point
         * @param       y1      y-coordinate of the first control point
         * @param       x2      x-coordinate of the second control point
         * @param       y2      y-coordinate of the second control point
         * @param       x3      x-coordinaat of the ending point (= new current point)
         * @param       y3      y-coordinaat of the ending point (= new current point)
         */
    
        public void CurveTo(float x1, float y1, float x2, float y2, float x3, float y3) {
            content.Append(x1).Append(' ').Append(y1).Append(' ').Append(x2).Append(' ').Append(y2).Append(' ').Append(x3).Append(' ').Append(y3).Append(" c").Append_i(separator);
        }
    
        /**
         * Appends a Bêzier curve to the path, starting from the current point.
         *
         * @param       x2      x-coordinate of the second control point
         * @param       y2      y-coordinate of the second control point
         * @param       x3      x-coordinaat of the ending point (= new current point)
         * @param       y3      y-coordinaat of the ending point (= new current point)
         */
    
        public void CurveTo(float x2, float y2, float x3, float y3) {
            content.Append(x2).Append(' ').Append(y2).Append(' ').Append(x3).Append(' ').Append(y3).Append(" v").Append_i(separator);
        }
    
        /**
         * Appends a Bêzier curve to the path, starting from the current point.
         *
         * @param       x1      x-coordinate of the first control point
         * @param       y1      y-coordinate of the first control point
         * @param       x3      x-coordinaat of the ending point (= new current point)
         * @param       y3      y-coordinaat of the ending point (= new current point)
         */
    
        public void CurveFromTo(float x1, float y1, float x3, float y3) {
            content.Append(x1).Append(' ').Append(y1).Append(' ').Append(x3).Append(' ').Append(y3).Append(" y").Append_i(separator);
        }
    
        /** Draws a circle. The endpoint will (x+r, y).
         *
         * @param x x center of circle
         * @param y y center of circle
         * @param r radius of circle
         */
        public void Circle(float x, float y, float r) {
            float b = 0.5523f;
            MoveTo(x + r, y);
            CurveTo(x + r, y + r * b, x + r * b, y + r, x, y + r);
            CurveTo(x - r * b, y + r, x - r, y + r * b, x - r, y);
            CurveTo(x - r, y - r * b, x - r * b, y - r, x, y - r);
            CurveTo(x + r * b, y - r, x + r, y - r * b, x + r, y);
        }
    
    
    
        /**
         * Adds a rectangle to the current path.
         *
         * @param       x       x-coordinate of the starting point
         * @param       y       y-coordinate of the starting point
         * @param       w       width
         * @param       h       height
         */
    
        public void Rectangle(float x, float y, float w, float h) {
            content.Append(x).Append(' ').Append(y).Append(' ').Append(w).Append(' ').Append(h).Append(" re").Append_i(separator);
        }
    
        private bool CompareColors(Color c1, Color c2) {
            if (c1 == null && c2 == null)
                return true;
            if (c1 == null || c2 == null)
                return false;
            if (c1 is ExtendedColor)
                return c1.Equals(c2);
            return c2.Equals(c1);
        }
        
        /**
        * Adds a variable width border to the current path.
        * Only use if {@link com.lowagie.text.Rectangle#isUseVariableBorders() Rectangle.isUseVariableBorders}
        * = true.
        * @param rect a <CODE>Rectangle</CODE>
        */
        public void VariableRectangle(Rectangle rect) {
            float t = rect.Top;
            float b = rect.Bottom;
            float r = rect.Right;
            float l = rect.Left;
            float wt = rect.BorderWidthTop;
            float wb = rect.BorderWidthBottom;
            float wr = rect.BorderWidthRight;
            float wl = rect.BorderWidthLeft;
            Color ct = rect.BorderColorTop;
            Color cb = rect.BorderColorBottom;
            Color cr = rect.BorderColorRight;
            Color cl = rect.BorderColorLeft;
            SaveState();
            SetLineCap(PdfContentByte.LINE_CAP_BUTT);
            SetLineJoin(PdfContentByte.LINE_JOIN_MITER);
            float clw = 0;
            bool cdef = false;
            Color ccol = null;
            bool cdefi = false;
            Color cfil = null;
            // draw top
            if (wt > 0) {
                SetLineWidth(clw = wt);
                cdef = true;
                if (ct == null)
                    ResetRGBColorStroke();
                else
                    SetColorStroke(ct);
                ccol = ct;
                MoveTo(l, t - wt / 2f);
                LineTo(r, t - wt / 2f);
                Stroke();
            }

            // Draw bottom
            if (wb > 0) {
                if (wb != clw)
                    SetLineWidth(clw = wb);
                if (!cdef || !CompareColors(ccol, cb)) {
                    cdef = true;
                    if (cb == null)
                        ResetRGBColorStroke();
                    else
                        SetColorStroke(cb);
                    ccol = cb;
                }
                MoveTo(r, b + wb / 2f);
                LineTo(l, b + wb / 2f);
                Stroke();
            }

            // Draw right
            if (wr > 0) {
                if (wr != clw)
                    SetLineWidth(clw = wr);
                if (!cdef || !CompareColors(ccol, cr)) {
                    cdef = true;
                    if (cr == null)
                        ResetRGBColorStroke();
                    else
                        SetColorStroke(cr);
                    ccol = cr;
                }
                bool bt = CompareColors(ct, cr);
                bool bb = CompareColors(cb, cr);
                MoveTo(r - wr / 2f, bt ? t : t - wt);
                LineTo(r - wr / 2f, bb ? b : b + wb);
                Stroke();
                if (!bt || !bb) {
                    cdefi = true;
                    if (cr == null)
                        ResetRGBColorFill();
                    else
                        SetColorFill(cr);
                    cfil = cr;
                    if (!bt) {
                        MoveTo(r, t);
                        LineTo(r, t - wt);
                        LineTo(r - wr, t - wt);
                        Fill();
                    }
                    if (!bb) {
                        MoveTo(r, b);
                        LineTo(r, b + wb);
                        LineTo(r - wr, b + wb);
                        Fill();
                    }
                }
            }
            
            // Draw Left
            if (wl > 0) {
                if (wl != clw)
                    SetLineWidth(wl);
                if (!cdef || !CompareColors(ccol, cl)) {
                    if (cl == null)
                        ResetRGBColorStroke();
                    else
                        SetColorStroke(cl);
                }
                bool bt = CompareColors(ct, cl);
                bool bb = CompareColors(cb, cl);
                MoveTo(l + wl / 2f, bt ? t : t - wt);
                LineTo(l + wl / 2f, bb ? b : b + wb);
                Stroke();
                if (!bt || !bb) {
                    if (!cdefi || !CompareColors(cfil, cl)) {
                        if (cl == null)
                            ResetRGBColorFill();
                        else
                            SetColorFill(cl);
                    }
                    if (!bt) {
                        MoveTo(l, t);
                        LineTo(l, t - wt);
                        LineTo(l + wl, t - wt);
                        Fill();
                    }
                    if (!bb) {
                        MoveTo(l, b);
                        LineTo(l, b + wb);
                        LineTo(l + wl, b + wb);
                        Fill();
                    }
                }
            }
            RestoreState();
        }

        /**
        * Adds a border (complete or partially) to the current path..
        *
        * @param        rectangle        a <CODE>Rectangle</CODE>
        */
        
        public void Rectangle(Rectangle rectangle) {
            // the coordinates of the border are retrieved
            float x1 = rectangle.Left;
            float y1 = rectangle.Bottom;
            float x2 = rectangle.Right;
            float y2 = rectangle.Top;

            // the backgroundcolor is set
            Color background = rectangle.BackgroundColor;
            if (background != null) {
                SetColorFill(background);
                Rectangle(x1, y1, x2 - x1, y2 - y1);
                Fill();
                ResetRGBColorFill();
            }

            // if the element hasn't got any borders, nothing is added
            if (! rectangle.HasBorders()) {
                return;
            }
        
            // if any of the individual border colors are set
            // we draw the borders all around using the
            // different colors
            if (rectangle.UseVariableBorders) {
                VariableRectangle(rectangle);
            }
            else {
                // the width is set to the width of the element
                if (rectangle.BorderWidth != iTextSharp.text.Rectangle.UNDEFINED) {
                    SetLineWidth(rectangle.BorderWidth);
                }
            
                // the color is set to the color of the element
                Color color = rectangle.BorderColor;
                if (color != null) {
                    SetColorStroke(color);
                }
            
                // if the box is a rectangle, it is added as a rectangle
                if (rectangle.HasBorder(iTextSharp.text.Rectangle.BOX)) {
                    this.Rectangle(x1, y1, x2 - x1, y2 - y1);
                }
                    // if the border isn't a rectangle, the different sides are added apart
                else {
                    if (rectangle.HasBorder(iTextSharp.text.Rectangle.RIGHT_BORDER)) {
                        MoveTo(x2, y1);
                        LineTo(x2, y2);
                    }
                    if (rectangle.HasBorder(iTextSharp.text.Rectangle.LEFT_BORDER)) {
                        MoveTo(x1, y1);
                        LineTo(x1, y2);
                    }
                    if (rectangle.HasBorder(iTextSharp.text.Rectangle.BOTTOM_BORDER)) {
                        MoveTo(x1, y1);
                        LineTo(x2, y1);
                    }
                    if (rectangle.HasBorder(iTextSharp.text.Rectangle.TOP_BORDER)) {
                        MoveTo(x1, y2);
                        LineTo(x2, y2);
                    }
                }
            
                Stroke();
            
                if (color != null) {
                    ResetRGBColorStroke();
                }
            }
        }
    
        /**
         * Closes the current subpath by appending a straight line segment from the current point
         * to the starting point of the subpath.
         */
    
        public void ClosePath() {
            content.Append('h').Append_i(separator);
        }
    
        /**
         * Ends the path without filling or stroking it.
         */
    
        public void NewPath() {
            content.Append('n').Append_i(separator);
        }
    
        /**
         * Strokes the path.
         */
    
        public void Stroke() {
            content.Append('S').Append_i(separator);
        }
    
        /**
         * Closes the path and strokes it.
         */
    
        public void ClosePathStroke() {
            content.Append('s').Append_i(separator);
        }
    
        /**
         * Fills the path, using the non-zero winding number rule to determine the region to fill.
         */
    
        public void Fill() {
            content.Append('f').Append_i(separator);
        }
    
        /**
         * Fills the path, using the even-odd rule to determine the region to fill.
         */
    
        public void EoFill() {
            content.Append("f*").Append_i(separator);
        }
    
        /**
         * Fills the path using the non-zero winding number rule to determine the region to fill and strokes it.
         */
    
        public void FillStroke() {
            content.Append('B').Append_i(separator);
        }
    
        /**
         * Closes the path, fills it using the non-zero winding number rule to determine the region to fill and strokes it.
         */
    
        public void ClosePathFillStroke() {
            content.Append('b').Append_i(separator);
        }
    
        /**
         * Fills the path, using the even-odd rule to determine the region to fill and strokes it.
         */
    
        public void EoFillStroke() {
            content.Append("B*").Append_i(separator);
        }
    
        /**
         * Closes the path, fills it using the even-odd rule to determine the region to fill and strokes it.
         */
    
        public void ClosePathEoFillStroke() {
            content.Append("b*").Append_i(separator);
        }
    
        /**
         * Adds an <CODE>Image</CODE> to the page. The <CODE>Image</CODE> must have
         * absolute positioning.
         * @param image the <CODE>Image</CODE> object
         * @throws DocumentException if the <CODE>Image</CODE> does not have absolute positioning
         */
        public virtual void AddImage(Image image) {
            AddImage(image, false);
        }
        
        /**
        * Adds an <CODE>Image</CODE> to the page. The <CODE>Image</CODE> must have
        * absolute positioning. The image can be placed inline.
        * @param image the <CODE>Image</CODE> object
        * @param inlineImage <CODE>true</CODE> to place this image inline, <CODE>false</CODE> otherwise
        * @throws DocumentException if the <CODE>Image</CODE> does not have absolute positioning
        */
        public virtual void AddImage(Image image, bool inlineImage) {
            if (!image.HasAbsolutePosition())
                throw new DocumentException("The image must have absolute positioning.");
            float[] matrix = image.Matrix;
            matrix[Image.CX] = image.AbsoluteX - matrix[Image.CX];
            matrix[Image.CY] = image.AbsoluteY - matrix[Image.CY];
            AddImage(image, matrix[0], matrix[1], matrix[2], matrix[3], matrix[4], matrix[5], inlineImage);
        }
    
        /**
         * Adds an <CODE>Image</CODE> to the page. The positioning of the <CODE>Image</CODE>
         * is done with the transformation matrix. To position an <CODE>image</CODE> at (x,y)
         * use AddImage(image, image_width, 0, 0, image_height, x, y).
         * @param image the <CODE>Image</CODE> object
         * @param a an element of the transformation matrix
         * @param b an element of the transformation matrix
         * @param c an element of the transformation matrix
         * @param d an element of the transformation matrix
         * @param e an element of the transformation matrix
         * @param f an element of the transformation matrix
         * @throws DocumentException on error
         */
        public virtual void AddImage(Image image, float a, float b, float c, float d, float e, float f) {
            AddImage(image, a, b, c, d, e, f, false);
        }
        
        /**
        * Adds an <CODE>Image</CODE> to the page. The positioning of the <CODE>Image</CODE>
        * is done with the transformation matrix. To position an <CODE>image</CODE> at (x,y)
        * use AddImage(image, image_width, 0, 0, image_height, x, y). The image can be placed inline.
        * @param image the <CODE>Image</CODE> object
        * @param a an element of the transformation matrix
        * @param b an element of the transformation matrix
        * @param c an element of the transformation matrix
        * @param d an element of the transformation matrix
        * @param e an element of the transformation matrix
        * @param f an element of the transformation matrix
        * @param inlineImage <CODE>true</CODE> to place this image inline, <CODE>false</CODE> otherwise
        * @throws DocumentException on error
        */
        public virtual void AddImage(Image image, float a, float b, float c, float d, float e, float f, bool inlineImage) {
            if (image.Layer != null)
                BeginLayer(image.Layer);
            if (image.IsImgTemplate()) {
                writer.AddDirectImageSimple(image);
                PdfTemplate template = image.TemplateData;
                float w = template.Width;
                float h = template.Height;
                AddTemplate(template, a / w, b / w, c / h, d / h, e, f);
            }
            else {
                content.Append("q ");
                content.Append(a).Append(' ');
                content.Append(b).Append(' ');
                content.Append(c).Append(' ');
                content.Append(d).Append(' ');
                content.Append(e).Append(' ');
                content.Append(f).Append(" cm");
                if (inlineImage) {
                    content.Append("\nBI\n");
                    PdfImage pimage = new PdfImage(image, "", null);
                    foreach (PdfName key in pimage.Keys) {
                        PdfObject value = pimage.Get(key);
                        String s = (String)abrev[key];
                        if (s == null)
                            continue;
                        content.Append(s);
                        bool check = true;
                        if (key.Equals(PdfName.COLORSPACE) && value.IsArray()) {
                            ArrayList ar = ((PdfArray)value).ArrayList;
                            if (ar.Count == 4 
                                && PdfName.INDEXED.Equals(ar[0]) 
                                && ((PdfObject)ar[1]).IsName()
                                && ((PdfObject)ar[2]).IsNumber()
                                && ((PdfObject)ar[3]).IsString()
                            ) {
                                check = false;
                            }
                            
                        }
                        if (check && key.Equals(PdfName.COLORSPACE) && !value.IsName()) {
                            PdfName cs = writer.GetColorspaceName();
                            PageResources prs = PageResources;
                            prs.AddColor(cs, writer.AddToBody(value).IndirectReference);
                            value = cs;
                        }
                        value.ToPdf(null, content);
                        content.Append('\n');
                    }
                    content.Append("ID\n");
                    pimage.WriteContent(content);
                    content.Append("\nEI\nQ").Append_i(separator);
                }
                else {
                    PdfName name;
                    PageResources prs = PageResources;
                    Image maskImage = image.ImageMask;
                    if (maskImage != null) {
                        name = writer.AddDirectImageSimple(maskImage);
                        prs.AddXObject(name, writer.GetImageReference(name));
                    }
                    name = writer.AddDirectImageSimple(image);
                    name = prs.AddXObject(name, writer.GetImageReference(name));
                    content.Append(' ').Append(name.GetBytes()).Append(" Do Q").Append_i(separator);
                }
            }
            if (image.HasBorders()) {
                SaveState();
                float w = image.Width;
                float h = image.Height;
                ConcatCTM(a / w, b / w, c / h, d / h, e, f);
                Rectangle(image);
                RestoreState();
            }
            if (image.Layer != null)
                EndLayer();
            Annotation annot = image.Annotation;
            if (annot == null)
                return;
            float[] r = new float[unitRect.Length];
            for (int k = 0; k < unitRect.Length; k += 2) {
                r[k] = a * unitRect[k] + c * unitRect[k + 1] + e;
                r[k + 1] = b * unitRect[k] + d * unitRect[k + 1] + f;
            }
            float llx = r[0];
            float lly = r[1];
            float urx = llx;
            float ury = lly;
            for (int k = 2; k < r.Length; k += 2) {
                llx = Math.Min(llx, r[k]);
                lly = Math.Min(lly, r[k + 1]);
                urx = Math.Max(urx, r[k]);
                ury = Math.Max(ury, r[k + 1]);
            }
            annot = new Annotation(annot);
            annot.SetDimensions(llx, lly, urx, ury);
            PdfAnnotation an = PdfAnnotationsImp.ConvertAnnotation(writer, annot, new Rectangle(llx, lly, urx, ury));
            if (an == null)
                return;
            AddAnnotation(an);
        }
    
        /**
         * Makes this <CODE>PdfContentByte</CODE> empty.
         */
        public void Reset() {
            content.Reset();
            stateList.Clear();
            state = new GraphicState();
        }
    
        /**
         * Starts the writing of text.
         */
        public void BeginText() {
            state.xTLM = 0;
            state.yTLM = 0;
            content.Append("BT").Append_i(separator);
        }
    
        /**
         * Ends the writing of text and makes the current font invalid.
         */
        public void EndText() {
            content.Append("ET").Append_i(separator);
        }
    
        /**
         * Saves the graphic state. <CODE>saveState</CODE> and
         * <CODE>restoreState</CODE> must be balanced.
         */
        public void SaveState() {
            content.Append('q').Append_i(separator);
            stateList.Add(new GraphicState(state));
        }
    
        /**
         * Restores the graphic state. <CODE>saveState</CODE> and
         * <CODE>restoreState</CODE> must be balanced.
         */
        public void RestoreState() {
            content.Append('Q').Append_i(separator);
            int idx = stateList.Count - 1;
            if (idx < 0)
                throw new Exception("Unbalanced save/restore state operators.");
            state = (GraphicState)stateList[idx];
            stateList.RemoveAt(idx);
        }
    
        /**
         * Sets the character spacing parameter.
         *
         * @param       charSpace           a parameter
         */
        public void SetCharacterSpacing(float value) {
            state.charSpace = value;
            content.Append(value).Append(" Tc").Append_i(separator);
        }
    
        /**
         * Sets the word spacing parameter.
         *
         * @param       wordSpace           a parameter
         */
        public void SetWordSpacing(float value) {
            state.wordSpace = value;
            content.Append(value).Append(" Tw").Append_i(separator);
        }
    
        /**
         * Sets the horizontal scaling parameter.
         *
         * @param       scale               a parameter
         */
        public void SetHorizontalScaling(float value) {
            state.scale = value;
            content.Append(value).Append(" Tz").Append_i(separator);
        }
    
        /**
         * Set the font and the size for the subsequent text writing.
         *
         * @param bf the font
         * @param size the font size in points
         */
        public virtual void SetFontAndSize(BaseFont bf, float size) {
            CheckWriter();
            if (size < 0.0001f && size > -0.0001f)
                throw new ArgumentException("Font size too small: " + size);
            state.size = size;
            state.fontDetails = writer.AddSimple(bf);
            PageResources prs = PageResources;
            PdfName name = state.fontDetails.FontName;
            name = prs.AddFont(name, state.fontDetails.IndirectReference);
            content.Append(name.GetBytes()).Append(' ').Append(size).Append(" Tf").Append_i(separator);
        }
    
        /**
         * Sets the text rendering parameter.
         *
         * @param       rendering               a parameter
         */
        public void SetTextRenderingMode(int value) {
                content.Append(value).Append(" Tr").Append_i(separator);
        }
    
        /**
         * Sets the text rise parameter.
         * <P>
         * This allows to write text in subscript or basescript mode.</P>
         *
         * @param       rise                a parameter
         */
        public void SetTextRise(float value) {
            content.Append(value).Append(" Ts").Append_i(separator);
        }
    
        /**
         * A helper to insert into the content stream the <CODE>text</CODE>
         * converted to bytes according to the font's encoding.
         *
         * @param text the text to write
         */
        private void ShowText2(string text) {
            if (state.fontDetails == null)
                throw new Exception("Font and size must be set before writing any text");
            byte[] b = state.fontDetails.ConvertToBytes(text);
            EscapeString(b, content);
        }
    
        /**
         * Shows the <CODE>text</CODE>.
         *
         * @param text the text to write
         */
        public void ShowText(string text) {
            ShowText2(text);
            content.Append("Tj").Append_i(separator);
        }
        
        /**
        * Constructs a kern array for a text in a certain font
        * @param text the text
        * @param font the font
        * @return a PdfTextArray
        */
        public static PdfTextArray GetKernArray(String text, BaseFont font) {
            PdfTextArray pa = new PdfTextArray();
            StringBuilder acc = new StringBuilder();
            int len = text.Length - 1;
            char[] c = text.ToCharArray();
            if (len >= 0)
                acc.Append(c, 0, 1);
            for (int k = 0; k < len; ++k) {
                char c2 = c[k + 1];
                int kern = font.GetKerning(c[k], c2);
                if (kern == 0) {
                    acc.Append(c2);
                }
                else {
                    pa.Add(acc.ToString());
                    acc.Length = 0;
                    acc.Append(c, k + 1, 1);
                    pa.Add(-kern);
                }
            }
            pa.Add(acc.ToString());
            return pa;
        }
        
        /**
        * Shows the <CODE>text</CODE> kerned.
        *
        * @param text the text to write
        */
        public void ShowTextKerned(String text) {
            if (state.fontDetails == null)
                throw new ArgumentNullException("Font and size must be set before writing any text");
            BaseFont bf = state.fontDetails.BaseFont;
            if (bf.HasKernPairs())
                ShowText(GetKernArray(text, bf));
            else
                ShowText(text);
        }

        /**
         * Moves to the next line and shows <CODE>text</CODE>.
         *
         * @param text the text to write
         */
        public void NewlineShowText(string text) {
            state.yTLM -= state.leading;
            ShowText2(text);
            content.Append('\'').Append_i(separator);
        }
        
        /**
         * Moves to the next line and shows text string, using the given values of the character and word spacing parameters.
         *
         * @param       wordSpacing     a parameter
         * @param       charSpacing     a parameter
         * @param text the text to write
         */
        public void NewlineShowText(float wordSpacing, float charSpacing, string text) {
            state.yTLM -= state.leading;
            content.Append(wordSpacing).Append(' ').Append(charSpacing);
            ShowText2(text);
            content.Append("\"").Append_i(separator);
        
            // The " operator sets charSpace and wordSpace into graphics state
            // (cfr PDF reference v1.6, table 5.6)
            state.charSpace = charSpacing;
            state.wordSpace = wordSpacing;
        }
    
        /**
         * Changes the text matrix.
         * <P>
         * Remark: this operation also initializes the current point position.</P>
         *
         * @param       a           operand 1,1 in the matrix
         * @param       b           operand 1,2 in the matrix
         * @param       c           operand 2,1 in the matrix
         * @param       d           operand 2,2 in the matrix
         * @param       x           operand 3,1 in the matrix
         * @param       y           operand 3,2 in the matrix
         */
        public void SetTextMatrix(float a, float b, float c, float d, float x, float y) {
            state.xTLM = x;
            state.yTLM = y;
            content.Append(a).Append(' ').Append(b).Append_i(' ')
                .Append(c).Append_i(' ').Append(d).Append_i(' ')
                .Append(x).Append_i(' ').Append(y).Append(" Tm").Append_i(separator);
        }
    
        /**
         * Changes the text matrix. The first four parameters are {1,0,0,1}.
         * <P>
         * Remark: this operation also initializes the current point position.</P>
         *
         * @param       x           operand 3,1 in the matrix
         * @param       y           operand 3,2 in the matrix
         */
        public void SetTextMatrix(float x, float y) {
            SetTextMatrix(1, 0, 0, 1, x, y);
        }
    
        /**
         * Moves to the start of the next line, offset from the start of the current line.
         *
         * @param       x           x-coordinate of the new current point
         * @param       y           y-coordinate of the new current point
         */
        public void MoveText(float x, float y) {
            state.xTLM += x;
            state.yTLM += y;
            content.Append(x).Append(' ').Append(y).Append(" Td").Append_i(separator);
        }
    
        /**
         * Moves to the start of the next line, offset from the start of the current line.
         * <P>
         * As a side effect, this sets the leading parameter in the text state.</P>
         *
         * @param       x           offset of the new current point
         * @param       y           y-coordinate of the new current point
         */
        public void MoveTextWithLeading(float x, float y) {
            state.xTLM += x;
            state.yTLM += y;
            state.leading = -y;
            content.Append(x).Append(' ').Append(y).Append(" TD").Append_i(separator);
        }
    
        /**
         * Moves to the start of the next line.
         */
        public void NewlineText() {
            state.yTLM -= state.leading;
            content.Append("T*").Append_i(separator);
        }
    
        /**
         * Gets the size of this content.
         *
         * @return the size of the content
         */
        internal int Size {
            get {
                return content.Size;
            }
        }
    
        /**
         * Escapes a <CODE>byte</CODE> array according to the PDF conventions.
         *
         * @param b the <CODE>byte</CODE> array to escape
         * @return an escaped <CODE>byte</CODE> array
         */
        internal static byte[] EscapeString(byte[] b) {
            ByteBuffer content = new ByteBuffer();
            EscapeString(b, content);
            return content.ToByteArray();
        }
    
        /**
         * Escapes a <CODE>byte</CODE> array according to the PDF conventions.
         *
         * @param b the <CODE>byte</CODE> array to escape
         */
        internal static void EscapeString(byte[] b, ByteBuffer content) {
            content.Append_i('(');
            for (int k = 0; k < b.Length; ++k) {
                byte c = b[k];
                switch ((int)c) {
                    case '\r':
                        content.Append("\\r");
                        break;
                    case '\n':
                        content.Append("\\n");
                        break;
                    case '\t':
                        content.Append("\\t");
                        break;
                    case '\b':
                        content.Append("\\b");
                        break;
                    case '\f':
                        content.Append("\\f");
                        break;
                    case '(':
                    case ')':
                    case '\\':
                        content.Append_i('\\').Append_i(c);
                        break;
                    default:
                        content.Append_i(c);
                        break;
                }
            }
            content.Append(')');
        }
    
        /**
         * Adds a named outline to the document.
         *
         * @param outline the outline
         * @param name the name for the local destination
         */
        public void AddOutline(PdfOutline outline, string name) {
            CheckWriter();
            pdf.AddOutline(outline, name);
        }
        /**
         * Gets the root outline.
         *
         * @return the root outline
         */
        public PdfOutline RootOutline {
            get {
                CheckWriter();
                return pdf.RootOutline;
            }
        }
    
        /**
        * Computes the width of the given string taking in account
        * the current values of "Character spacing", "Word Spacing"
        * and "Horizontal Scaling".
        * The additional spacing is not computed for the last character
        * of the string.
        * @param text the string to get width of
        * @param kerned the kerning option
        * @return the width
        */

        public float GetEffectiveStringWidth(String text, bool kerned) {
            BaseFont bf = state.fontDetails.BaseFont;
            
            float w;
            if (kerned)
                w = bf.GetWidthPointKerned(text, state.size);
            else
                w = bf.GetWidthPoint(text, state.size);
            
            if (state.charSpace != 0.0f && text.Length > 1) {
                w += state.charSpace * (text.Length -1);
            }
            
            int ft = bf.FontType;
            if (state.wordSpace != 0.0f && (ft == BaseFont.FONT_TYPE_T1 || ft == BaseFont.FONT_TYPE_TT || ft == BaseFont.FONT_TYPE_T3)) {
                for (int i = 0; i < (text.Length -1); i++) {
                    if (text[i] == ' ')
                        w += state.wordSpace;
                }
            }
            if (state.scale != 100.0)
                w = (w * state.scale) / 100.0f;
            
            //System.out.Println("String width = " + Float.ToString(w));
            return w;
        }
        
        /**
        * Shows text right, left or center aligned with rotation.
        * @param alignment the alignment can be ALIGN_CENTER, ALIGN_RIGHT or ALIGN_LEFT
        * @param text the text to show
        * @param x the x pivot position
        * @param y the y pivot position
        * @param rotation the rotation to be applied in degrees counterclockwise
        */
        public void ShowTextAligned(int alignment, String text, float x, float y, float rotation) {
            ShowTextAligned(alignment, text, x, y, rotation, false);
        }
        
        private void ShowTextAligned(int alignment, String text, float x, float y, float rotation, bool kerned) {
            if (state.fontDetails == null)
                throw new Exception("Font and size must be set before writing any text");
            if (rotation == 0) {
                switch (alignment) {
                    case ALIGN_CENTER:
                        x -= GetEffectiveStringWidth(text, kerned) / 2;
                        break;
                    case ALIGN_RIGHT:
                        x -= GetEffectiveStringWidth(text, kerned);
                        break;
                }
                SetTextMatrix(x, y);
                if (kerned)
                    ShowTextKerned(text);
                else
                    ShowText(text);
            }
            else {
                double alpha = rotation * Math.PI / 180.0;
                float cos = (float)Math.Cos(alpha);
                float sin = (float)Math.Sin(alpha);
                float len;
                switch (alignment) {
                    case ALIGN_CENTER:
                        len = GetEffectiveStringWidth(text, kerned) / 2;
                        x -=  len * cos;
                        y -=  len * sin;
                        break;
                    case ALIGN_RIGHT:
                        len = GetEffectiveStringWidth(text, kerned);
                        x -=  len * cos;
                        y -=  len * sin;
                        break;
                }
                SetTextMatrix(cos, sin, -sin, cos, x, y);
                if (kerned)
                    ShowTextKerned(text);
                else
                    ShowText(text);
                SetTextMatrix(0f, 0f);
            }
        }
        
        /**
        * Shows text kerned right, left or center aligned with rotation.
        * @param alignment the alignment can be ALIGN_CENTER, ALIGN_RIGHT or ALIGN_LEFT
        * @param text the text to show
        * @param x the x pivot position
        * @param y the y pivot position
        * @param rotation the rotation to be applied in degrees counterclockwise
        */
        public void ShowTextAlignedKerned(int alignment, String text, float x, float y, float rotation) {
            ShowTextAligned(alignment, text, x, y, rotation, true);
        }

        /**
         * Concatenate a matrix to the current transformation matrix.
         * @param a an element of the transformation matrix
         * @param b an element of the transformation matrix
         * @param c an element of the transformation matrix
         * @param d an element of the transformation matrix
         * @param e an element of the transformation matrix
         * @param f an element of the transformation matrix
         **/
        public void ConcatCTM(float a, float b, float c, float d, float e, float f) {
            content.Append(a).Append(' ').Append(b).Append(' ').Append(c).Append(' ');
            content.Append(d).Append(' ').Append(e).Append(' ').Append(f).Append(" cm").Append_i(separator);
        }
    
        /**
         * Generates an array of bezier curves to draw an arc.
         * <P>
         * (x1, y1) and (x2, y2) are the corners of the enclosing rectangle.
         * Angles, measured in degrees, start with 0 to the right (the positive X
         * axis) and increase counter-clockwise.  The arc extends from startAng
         * to startAng+extent.  I.e. startAng=0 and extent=180 yields an openside-down
         * semi-circle.
         * <P>
         * The resulting coordinates are of the form float[]{x1,y1,x2,y2,x3,y3, x4,y4}
         * such that the curve goes from (x1, y1) to (x4, y4) with (x2, y2) and
         * (x3, y3) as their respective Bezier control points.
         * <P>
         * Note: this code was taken from ReportLab (www.reportlab.com), an excelent
         * PDF generator for Python.
         *
         * @param x1 a corner of the enclosing rectangle
         * @param y1 a corner of the enclosing rectangle
         * @param x2 a corner of the enclosing rectangle
         * @param y2 a corner of the enclosing rectangle
         * @param startAng starting angle in degrees
         * @param extent angle extent in degrees
         * @return a list of float[] with the bezier curves
         */
        public static ArrayList BezierArc(float x1, float y1, float x2, float y2, float startAng, float extent) {
            float tmp;
            if (x1 > x2) {
                tmp = x1;
                x1 = x2;
                x2 = tmp;
            }
            if (y2 > y1) {
                tmp = y1;
                y1 = y2;
                y2 = tmp;
            }
        
            float fragAngle;
            int Nfrag;
            if (Math.Abs(extent) <= 90f) {
                fragAngle = extent;
                Nfrag = 1;
            }
            else {
                Nfrag = (int)(Math.Ceiling(Math.Abs(extent)/90f));
                fragAngle = extent / Nfrag;
            }
            float x_cen = (x1+x2)/2f;
            float y_cen = (y1+y2)/2f;
            float rx = (x2-x1)/2f;
            float ry = (y2-y1)/2f;
            float halfAng = (float)(fragAngle * Math.PI / 360.0);
            float kappa = (float)(Math.Abs(4.0 / 3.0 * (1.0 - Math.Cos(halfAng)) / Math.Sin(halfAng)));
            ArrayList pointList = new ArrayList();
            for (int i = 0; i < Nfrag; ++i) {
                float theta0 = (float)((startAng + i*fragAngle) * Math.PI / 180.0);
                float theta1 = (float)((startAng + (i+1)*fragAngle) * Math.PI / 180.0);
                float cos0 = (float)Math.Cos(theta0);
                float cos1 = (float)Math.Cos(theta1);
                float sin0 = (float)Math.Sin(theta0);
                float sin1 = (float)Math.Sin(theta1);
                if (fragAngle > 0f) {
                    pointList.Add(new float[]{x_cen + rx * cos0,
                                                 y_cen - ry * sin0,
                                                 x_cen + rx * (cos0 - kappa * sin0),
                                                 y_cen - ry * (sin0 + kappa * cos0),
                                                 x_cen + rx * (cos1 + kappa * sin1),
                                                 y_cen - ry * (sin1 - kappa * cos1),
                                                 x_cen + rx * cos1,
                                                 y_cen - ry * sin1});
                }
                else {
                    pointList.Add(new float[]{x_cen + rx * cos0,
                                                 y_cen - ry * sin0,
                                                 x_cen + rx * (cos0 + kappa * sin0),
                                                 y_cen - ry * (sin0 - kappa * cos0),
                                                 x_cen + rx * (cos1 - kappa * sin1),
                                                 y_cen - ry * (sin1 + kappa * cos1),
                                                 x_cen + rx * cos1,
                                                 y_cen - ry * sin1});
                }
            }
            return pointList;
        }
    
        /**
         * Draws a partial ellipse inscribed within the rectangle x1,y1,x2,y2,
         * starting at startAng degrees and covering extent degrees. Angles
         * start with 0 to the right (+x) and increase counter-clockwise.
         *
         * @param x1 a corner of the enclosing rectangle
         * @param y1 a corner of the enclosing rectangle
         * @param x2 a corner of the enclosing rectangle
         * @param y2 a corner of the enclosing rectangle
         * @param startAng starting angle in degrees
         * @param extent angle extent in degrees
         */
        public void Arc(float x1, float y1, float x2, float y2, float startAng, float extent) {
            ArrayList ar = BezierArc(x1, y1, x2, y2, startAng, extent);
            if (ar.Count == 0)
                return;
            float[] pt = (float [])ar[0];
            MoveTo(pt[0], pt[1]);
            for (int k = 0; k < ar.Count; ++k) {
                pt = (float [])ar[k];
                CurveTo(pt[2], pt[3], pt[4], pt[5], pt[6], pt[7]);
            }
        }
    
        /**
         * Draws an ellipse inscribed within the rectangle x1,y1,x2,y2.
         *
         * @param x1 a corner of the enclosing rectangle
         * @param y1 a corner of the enclosing rectangle
         * @param x2 a corner of the enclosing rectangle
         * @param y2 a corner of the enclosing rectangle
         */
        public void Ellipse(float x1, float y1, float x2, float y2) {
            Arc(x1, y1, x2, y2, 0f, 360f);
        }
    
        /**
         * Create a new colored tiling pattern.
         *
         * @param width the width of the pattern
         * @param height the height of the pattern
         * @param xstep the desired horizontal spacing between pattern cells.
         * May be either positive or negative, but not zero.
         * @param ystep the desired vertical spacing between pattern cells.
         * May be either positive or negative, but not zero.
         * @return the <CODE>PdfPatternPainter</CODE> where the pattern will be created
         */
        public PdfPatternPainter CreatePattern(float width, float height, float xstep, float ystep) {
            CheckWriter();
            if ( xstep == 0.0f || ystep == 0.0f )
                throw new Exception("XStep or YStep can not be ZERO.");
            PdfPatternPainter painter = new PdfPatternPainter(writer);
            painter.Width = width;
            painter.Height = height;
            painter.XStep = xstep;
            painter.YStep = ystep;
            writer.AddSimplePattern(painter);
            return painter;
        }
    
        /**
         * Create a new colored tiling pattern. Variables xstep and ystep are set to the same values
         * of width and height.
         * @param width the width of the pattern
         * @param height the height of the pattern
         * @return the <CODE>PdfPatternPainter</CODE> where the pattern will be created
         */
        public PdfPatternPainter CreatePattern(float width, float height) {
            return CreatePattern(width, height, width, height);
        }
    
        /**
         * Create a new uncolored tiling pattern.
         *
         * @param width the width of the pattern
         * @param height the height of the pattern
         * @param xstep the desired horizontal spacing between pattern cells.
         * May be either positive or negative, but not zero.
         * @param ystep the desired vertical spacing between pattern cells.
         * May be either positive or negative, but not zero.
         * @param color the default color. Can be <CODE>null</CODE>
         * @return the <CODE>PdfPatternPainter</CODE> where the pattern will be created
         */
        public PdfPatternPainter CreatePattern(float width, float height, float xstep, float ystep, Color color) {
            CheckWriter();
            if ( xstep == 0.0f || ystep == 0.0f )
                throw new Exception("XStep or YStep can not be ZERO.");
            PdfPatternPainter painter = new PdfPatternPainter(writer, color);
            painter.Width = width;
            painter.Height = height;
            painter.XStep = xstep;
            painter.YStep = ystep;
            writer.AddSimplePattern(painter);
            return painter;
        }
    
        /**
         * Create a new uncolored tiling pattern.
         * Variables xstep and ystep are set to the same values
         * of width and height.
         * @param width the width of the pattern
         * @param height the height of the pattern
         * @param color the default color. Can be <CODE>null</CODE>
         * @return the <CODE>PdfPatternPainter</CODE> where the pattern will be created
         */
        public PdfPatternPainter CreatePattern(float width, float height, Color color) {
            return CreatePattern(width, height, width, height, color);
        }
    
        /**
         * Creates a new template.
         * <P>
         * Creates a new template that is nothing more than a form XObject. This template can be included
         * in this <CODE>PdfContentByte</CODE> or in another template. Templates are only written
         * to the output when the document is closed permitting things like showing text in the first page
         * that is only defined in the last page.
         *
         * @param width the bounding box width
         * @param height the bounding box height
         * @return the templated created
         */
        public PdfTemplate CreateTemplate(float width, float height) {
            return CreateTemplate(width, height, null);
        }
        
        internal PdfTemplate CreateTemplate(float width, float height, PdfName forcedName) {
            CheckWriter();
            PdfTemplate template = new PdfTemplate(writer);
            template.Width = width;
            template.Height = height;
            writer.AddDirectTemplateSimple(template, forcedName);
            return template;
        }
    
        /**
         * Creates a new appearance to be used with form fields.
         *
         * @param width the bounding box width
         * @param height the bounding box height
         * @return the appearance created
         */
        public PdfAppearance CreateAppearance(float width, float height) {
            return CreateAppearance(width, height, null);
        }
        
        internal PdfAppearance CreateAppearance(float width, float height, PdfName forcedName) {
            CheckWriter();
            PdfAppearance template = new PdfAppearance(writer);
            template.Width = width;
            template.Height = height;
            writer.AddDirectTemplateSimple(template, forcedName);
            return template;
        }
    
        /**
        * Adds a PostScript XObject to this content.
        *
        * @param psobject the object
        */
        public void AddPSXObject(PdfPSXObject psobject) {
            CheckWriter();
            PdfName name = writer.AddDirectTemplateSimple(psobject, null);
            PageResources prs = PageResources;
            name = prs.AddXObject(name, psobject.IndirectReference);
            content.Append(name.GetBytes()).Append(" Do").Append_i(separator);
        }

        /**
         * Adds a template to this content.
         *
         * @param template the template
         * @param a an element of the transformation matrix
         * @param b an element of the transformation matrix
         * @param c an element of the transformation matrix
         * @param d an element of the transformation matrix
         * @param e an element of the transformation matrix
         * @param f an element of the transformation matrix
         */
        public virtual void AddTemplate(PdfTemplate template, float a, float b, float c, float d, float e, float f) {
            CheckWriter();
            CheckNoPattern(template);
            PdfName name = writer.AddDirectTemplateSimple(template, null);
            PageResources prs = PageResources;
            name = prs.AddXObject(name, template.IndirectReference);
            content.Append("q ");
            content.Append(a).Append(' ');
            content.Append(b).Append(' ');
            content.Append(c).Append(' ');
            content.Append(d).Append(' ');
            content.Append(e).Append(' ');
            content.Append(f).Append(" cm ");
            content.Append(name.GetBytes()).Append(" Do Q").Append_i(separator);
        }
    
        internal void AddTemplateReference(PdfIndirectReference template, PdfName name, float a, float b, float c, float d, float e, float f) {
            CheckWriter();
            PageResources prs = PageResources;
            name = prs.AddXObject(name, template);
            content.Append("q ");
            content.Append(a).Append(' ');
            content.Append(b).Append(' ');
            content.Append(c).Append(' ');
            content.Append(d).Append(' ');
            content.Append(e).Append(' ');
            content.Append(f).Append(" cm ");
            content.Append(name.GetBytes()).Append(" Do Q").Append_i(separator);
        }
        
        /**
         * Adds a template to this content.
         *
         * @param template the template
         * @param x the x location of this template
         * @param y the y location of this template
         */
        public void AddTemplate(PdfTemplate template, float x, float y) {
            AddTemplate(template, 1, 0, 0, 1, x, y);
        }
    
        /**
         * Changes the current color for filling paths (device dependent colors!).
         * <P>
         * Sets the color space to <B>DeviceCMYK</B> (or the <B>DefaultCMYK</B> color space),
         * and sets the color to use for filling paths.</P>
         * <P>
         * This method is described in the 'Portable Document Format Reference Manual version 1.3'
         * section 8.5.2.1 (page 331).</P>
         * <P>
         * Following the PDF manual, each operand must be a number between 0 (no ink) and
         * 1 (maximum ink). This method however accepts only ints between 0x00 and 0xFF.</P>
         *
         * @param cyan the intensity of cyan
         * @param magenta the intensity of magenta
         * @param yellow the intensity of yellow
         * @param black the intensity of black
         */
    
        public virtual void SetCMYKColorFill(int cyan, int magenta, int yellow, int black) {
            content.Append((float)(cyan & 0xFF) / 0xFF);
            content.Append(' ');
            content.Append((float)(magenta & 0xFF) / 0xFF);
            content.Append(' ');
            content.Append((float)(yellow & 0xFF) / 0xFF);
            content.Append(' ');
            content.Append((float)(black & 0xFF) / 0xFF);
            content.Append(" k").Append_i(separator);
        }
        /**
         * Changes the current color for stroking paths (device dependent colors!).
         * <P>
         * Sets the color space to <B>DeviceCMYK</B> (or the <B>DefaultCMYK</B> color space),
         * and sets the color to use for stroking paths.</P>
         * <P>
         * This method is described in the 'Portable Document Format Reference Manual version 1.3'
         * section 8.5.2.1 (page 331).</P>
         * Following the PDF manual, each operand must be a number between 0 (miniumum intensity) and
         * 1 (maximum intensity). This method however accepts only ints between 0x00 and 0xFF.
         *
         * @param cyan the intensity of red
         * @param magenta the intensity of green
         * @param yellow the intensity of blue
         * @param black the intensity of black
         */
    
        public virtual void SetCMYKColorStroke(int cyan, int magenta, int yellow, int black) {
            content.Append((float)(cyan & 0xFF) / 0xFF);
            content.Append(' ');
            content.Append((float)(magenta & 0xFF) / 0xFF);
            content.Append(' ');
            content.Append((float)(yellow & 0xFF) / 0xFF);
            content.Append(' ');
            content.Append((float)(black & 0xFF) / 0xFF);
            content.Append(" K").Append_i(separator);
        }
    
        /**
         * Changes the current color for filling paths (device dependent colors!).
         * <P>
         * Sets the color space to <B>DeviceRGB</B> (or the <B>DefaultRGB</B> color space),
         * and sets the color to use for filling paths.</P>
         * <P>
         * This method is described in the 'Portable Document Format Reference Manual version 1.3'
         * section 8.5.2.1 (page 331).</P>
         * <P>
         * Following the PDF manual, each operand must be a number between 0 (miniumum intensity) and
         * 1 (maximum intensity). This method however accepts only ints between 0x00 and 0xFF.</P>
         *
         * @param red the intensity of red
         * @param green the intensity of green
         * @param blue the intensity of blue
         */
    
        public virtual void SetRGBColorFill(int red, int green, int blue) {
            HelperRGB((float)(red & 0xFF) / 0xFF, (float)(green & 0xFF) / 0xFF, (float)(blue & 0xFF) / 0xFF);
            content.Append(" rg").Append_i(separator);
        }
    
        /**
         * Changes the current color for stroking paths (device dependent colors!).
         * <P>
         * Sets the color space to <B>DeviceRGB</B> (or the <B>DefaultRGB</B> color space),
         * and sets the color to use for stroking paths.</P>
         * <P>
         * This method is described in the 'Portable Document Format Reference Manual version 1.3'
         * section 8.5.2.1 (page 331).</P>
         * Following the PDF manual, each operand must be a number between 0 (miniumum intensity) and
         * 1 (maximum intensity). This method however accepts only ints between 0x00 and 0xFF.
         *
         * @param red the intensity of red
         * @param green the intensity of green
         * @param blue the intensity of blue
         */
    
        public virtual void SetRGBColorStroke(int red, int green, int blue) {
            HelperRGB((float)(red & 0xFF) / 0xFF, (float)(green & 0xFF) / 0xFF, (float)(blue & 0xFF) / 0xFF);
            content.Append(" RG").Append_i(separator);
        }
    
        /** Sets the stroke color. <CODE>color</CODE> can be an
         * <CODE>ExtendedColor</CODE>.
         * @param color the color
         */    
        public virtual void SetColorStroke(Color value) {
            PdfXConformanceImp.CheckPDFXConformance(writer, PdfXConformanceImp.PDFXKEY_COLOR, value);
            int type = ExtendedColor.GetType(value);
            switch (type) {
                case ExtendedColor.TYPE_GRAY: {
                    SetGrayStroke(((GrayColor)value).Gray);
                    break;
                }
                case ExtendedColor.TYPE_CMYK: {
                    CMYKColor cmyk = (CMYKColor)value;
                    SetCMYKColorStrokeF(cmyk.Cyan, cmyk.Magenta, cmyk.Yellow, cmyk.Black);
                    break;
                }
                case ExtendedColor.TYPE_SEPARATION: {
                    SpotColor spot = (SpotColor)value;
                    SetColorStroke(spot.PdfSpotColor, spot.Tint);
                    break;
                }
                case ExtendedColor.TYPE_PATTERN: {
                    PatternColor pat = (PatternColor)value;
                    SetPatternStroke(pat.Painter);
                    break;
                }
                case ExtendedColor.TYPE_SHADING: {
                    ShadingColor shading = (ShadingColor)value;
                    SetShadingStroke(shading.PdfShadingPattern);
                    break;
                }
                default:
                    SetRGBColorStroke(value.R, value.G, value.B);
                    break;
            }
        }
    
        /** Sets the fill color. <CODE>color</CODE> can be an
         * <CODE>ExtendedColor</CODE>.
         * @param color the color
         */    
        public virtual void SetColorFill(Color value) {
            PdfXConformanceImp.CheckPDFXConformance(writer, PdfXConformanceImp.PDFXKEY_COLOR, value);
            int type = ExtendedColor.GetType(value);
            switch (type) {
                case ExtendedColor.TYPE_GRAY: {
                    SetGrayFill(((GrayColor)value).Gray);
                    break;
                }
                case ExtendedColor.TYPE_CMYK: {
                    CMYKColor cmyk = (CMYKColor)value;
                    SetCMYKColorFillF(cmyk.Cyan, cmyk.Magenta, cmyk.Yellow, cmyk.Black);
                    break;
                }
                case ExtendedColor.TYPE_SEPARATION: {
                    SpotColor spot = (SpotColor)value;
                    SetColorFill(spot.PdfSpotColor, spot.Tint);
                    break;
                }
                case ExtendedColor.TYPE_PATTERN: {
                    PatternColor pat = (PatternColor)value;
                    SetPatternFill(pat.Painter);
                    break;
                }
                case ExtendedColor.TYPE_SHADING: {
                    ShadingColor shading = (ShadingColor)value;
                    SetShadingFill(shading.PdfShadingPattern);
                    break;
                }
                default:
                    SetRGBColorFill(value.R, value.G, value.B);
                    break;
            }
        }
    
        /** Sets the fill color to a spot color.
         * @param sp the spot color
         * @param tint the tint for the spot color. 0 is no color and 1
         * is 100% color
         */    
        public virtual void SetColorFill(PdfSpotColor sp, float tint) {
            CheckWriter();
            state.colorDetails = writer.AddSimple(sp);
            PageResources prs = PageResources;
            PdfName name = state.colorDetails.ColorName;
            name = prs.AddColor(name, state.colorDetails.IndirectReference);
            content.Append(name.GetBytes()).Append(" cs ").Append(tint).Append(" scn").Append_i(separator);
        }
    
        /** Sets the stroke color to a spot color.
         * @param sp the spot color
         * @param tint the tint for the spot color. 0 is no color and 1
         * is 100% color
         */    
        public virtual void SetColorStroke(PdfSpotColor sp, float tint) {
            CheckWriter();
            state.colorDetails = writer.AddSimple(sp);
            PageResources prs = PageResources;
            PdfName name = state.colorDetails.ColorName;
            name = prs.AddColor(name, state.colorDetails.IndirectReference);
            content.Append(name.GetBytes()).Append(" CS ").Append(tint).Append(" SCN").Append_i(separator);
        }
    
        /** Sets the fill color to a pattern. The pattern can be
         * colored or uncolored.
         * @param p the pattern
         */    
        public virtual void SetPatternFill(PdfPatternPainter p) {
            if (p.IsStencil()) {
                SetPatternFill(p, p.DefaultColor);
                return;
            }
            CheckWriter();
            PageResources prs = PageResources;
            PdfName name = writer.AddSimplePattern(p);
            name = prs.AddPattern(name, p.IndirectReference);
            content.Append(PdfName.PATTERN.GetBytes()).Append(" cs ").Append(name.GetBytes()).Append(" scn").Append_i(separator);
        }
    
        /** Outputs the color values to the content.
         * @param color The color
         * @param tint the tint if it is a spot color, ignored otherwise
         */    
        internal void OutputColorNumbers(Color color, float tint) {
            PdfXConformanceImp.CheckPDFXConformance(writer, PdfXConformanceImp.PDFXKEY_COLOR, color);
            int type = ExtendedColor.GetType(color);
            switch (type) {
                case ExtendedColor.TYPE_RGB:
                    content.Append((float)(color.R) / 0xFF);
                    content.Append(' ');
                    content.Append((float)(color.G) / 0xFF);
                    content.Append(' ');
                    content.Append((float)(color.B) / 0xFF);
                    break;
                case ExtendedColor.TYPE_GRAY:
                    content.Append(((GrayColor)color).Gray);
                    break;
                case ExtendedColor.TYPE_CMYK: {
                    CMYKColor cmyk = (CMYKColor)color;
                    content.Append(cmyk.Cyan).Append(' ').Append(cmyk.Magenta);
                    content.Append(' ').Append(cmyk.Yellow).Append(' ').Append(cmyk.Black);
                    break;
                }
                case ExtendedColor.TYPE_SEPARATION:
                    content.Append(tint);
                    break;
                default:
                    throw new Exception("Invalid color type.");                
            }
        }
    
        /** Sets the fill color to an uncolored pattern.
         * @param p the pattern
         * @param color the color of the pattern
         */    
        public virtual void SetPatternFill(PdfPatternPainter p, Color color) {
            if (ExtendedColor.GetType(color) == ExtendedColor.TYPE_SEPARATION)
                SetPatternFill(p, color, ((SpotColor)color).Tint);
            else
                SetPatternFill(p, color, 0);
        }
    
        /** Sets the fill color to an uncolored pattern.
         * @param p the pattern
         * @param color the color of the pattern
         * @param tint the tint if the color is a spot color, ignored otherwise
         */    
        public virtual void SetPatternFill(PdfPatternPainter p, Color color, float tint) {
            CheckWriter();
            if (!p.IsStencil())
                throw new Exception("An uncolored pattern was expected.");
            PageResources prs = PageResources;
            PdfName name = writer.AddSimplePattern(p);
            name = prs.AddPattern(name, p.IndirectReference);
            ColorDetails csDetail = writer.AddSimplePatternColorspace(color);
            PdfName cName = prs.AddColor(csDetail.ColorName, csDetail.IndirectReference);
            content.Append(cName.GetBytes()).Append(" cs").Append_i(separator);
            OutputColorNumbers(color, tint);
            content.Append(' ').Append(name.GetBytes()).Append(" scn").Append_i(separator);
        }
    
        /** Sets the stroke color to an uncolored pattern.
         * @param p the pattern
         * @param color the color of the pattern
         */    
        public virtual void SetPatternStroke(PdfPatternPainter p, Color color) {
            if (ExtendedColor.GetType(color) == ExtendedColor.TYPE_SEPARATION)
                SetPatternStroke(p, color, ((SpotColor)color).Tint);
            else
                SetPatternStroke(p, color, 0);
        }
    
        /** Sets the stroke color to an uncolored pattern.
         * @param p the pattern
         * @param color the color of the pattern
         * @param tint the tint if the color is a spot color, ignored otherwise
         */    
        public virtual void SetPatternStroke(PdfPatternPainter p, Color color, float tint) {
            CheckWriter();
            if (!p.IsStencil())
                throw new Exception("An uncolored pattern was expected.");
            PageResources prs = PageResources;
            PdfName name = writer.AddSimplePattern(p);
            name = prs.AddPattern(name, p.IndirectReference);
            ColorDetails csDetail = writer.AddSimplePatternColorspace(color);
            PdfName cName = prs.AddColor(csDetail.ColorName, csDetail.IndirectReference);
            content.Append(cName.GetBytes()).Append(" CS").Append_i(separator);
            OutputColorNumbers(color, tint);
            content.Append(' ').Append(name.GetBytes()).Append(" SCN").Append_i(separator);
        }
    
        /** Sets the stroke color to a pattern. The pattern can be
         * colored or uncolored.
         * @param p the pattern
         */    
        public virtual void SetPatternStroke(PdfPatternPainter p) {
            if (p.IsStencil()) {
                SetPatternStroke(p, p.DefaultColor);
                return;
            }
            CheckWriter();
            PageResources prs = PageResources;
            PdfName name = writer.AddSimplePattern(p);
            name = prs.AddPattern(name, p.IndirectReference);
            content.Append(PdfName.PATTERN.GetBytes()).Append(" CS ").Append(name.GetBytes()).Append(" SCN").Append_i(separator);
        }
    
        /**
        * Paints using a shading object. 
        * @param shading the shading object
        */
        public virtual void PaintShading(PdfShading shading) {
            writer.AddSimpleShading(shading);
            PageResources prs = PageResources;
            PdfName name = prs.AddShading(shading.ShadingName, shading.ShadingReference);
            content.Append(name.GetBytes()).Append(" sh").Append_i(separator);
            ColorDetails details = shading.ColorDetails;
            if (details != null)
                prs.AddColor(details.ColorName, details.IndirectReference);
        }
        
        /**
        * Paints using a shading pattern. 
        * @param shading the shading pattern
        */
        public virtual void PaintShading(PdfShadingPattern shading) {
            PaintShading(shading.Shading);
        }
    
        /**
        * Sets the shading fill pattern.
        * @param shading the shading pattern
        */
        public virtual void SetShadingFill(PdfShadingPattern shading) {
            writer.AddSimpleShadingPattern(shading);
            PageResources prs = PageResources;
            PdfName name = prs.AddPattern(shading.PatternName, shading.PatternReference);
            content.Append(PdfName.PATTERN.GetBytes()).Append(" cs ").Append(name.GetBytes()).Append(" scn").Append_i(separator);
            ColorDetails details = shading.ColorDetails;
            if (details != null)
                prs.AddColor(details.ColorName, details.IndirectReference);
        }
        
        /**
        * Sets the shading stroke pattern
        * @param shading the shading pattern
        */
        public virtual void SetShadingStroke(PdfShadingPattern shading) {
            writer.AddSimpleShadingPattern(shading);
            PageResources prs = PageResources;
            PdfName name = prs.AddPattern(shading.PatternName, shading.PatternReference);
            content.Append(PdfName.PATTERN.GetBytes()).Append(" CS ").Append(name.GetBytes()).Append(" SCN").Append_i(separator);
            ColorDetails details = shading.ColorDetails;
            if (details != null)
                prs.AddColor(details.ColorName, details.IndirectReference);
        }

        /** Check if we have a valid PdfWriter.
         *
         */
        protected virtual void CheckWriter() {
            if (writer == null)
                throw new ArgumentNullException("The writer in PdfContentByte is null.");
        }
    
        /**
         * Show an array of text.
         * @param text array of text
         */
        public void ShowText(PdfTextArray text) {
            if (state.fontDetails == null)
                throw new ArgumentNullException("Font and size must be set before writing any text");
            content.Append('[');
            ArrayList arrayList = text.ArrayList;
            bool lastWasNumber = false;
            for (int k = 0; k < arrayList.Count; ++k) {
                Object obj = arrayList[k];
                if (obj is string) {
                    ShowText2((string)obj);
                    lastWasNumber = false;
                }
                else {
                    if (lastWasNumber)
                        content.Append(' ');
                    else
                        lastWasNumber = true;
                    content.Append(((float)obj));
                }
            }
            content.Append("]TJ").Append_i(separator);
        }
    
        /**
         * Gets the <CODE>PdfWriter</CODE> in use by this object.
         * @return the <CODE>PdfWriter</CODE> in use by this object
         */
        public PdfWriter PdfWriter {
            get {
                return writer;
            }
        }
    
        /**
         * Gets the <CODE>PdfDocument</CODE> in use by this object.
         * @return the <CODE>PdfDocument</CODE> in use by this object
         */
        public PdfDocument PdfDocument {
            get {
                return pdf;
            }
        }
    
        /**
         * Implements a link to other part of the document. The jump will
         * be made to a local destination with the same name, that must exist.
         * @param name the name for this link
         * @param llx the lower left x corner of the activation area
         * @param lly the lower left y corner of the activation area
         * @param urx the upper right x corner of the activation area
         * @param ury the upper right y corner of the activation area
         */
        public void LocalGoto(string name, float llx, float lly, float urx, float ury) {
            pdf.LocalGoto(name, llx, lly, urx, ury);
        }
    
        /**
         * The local destination to where a local goto with the same
         * name will jump.
         * @param name the name of this local destination
         * @param destination the <CODE>PdfDestination</CODE> with the jump coordinates
         * @return <CODE>true</CODE> if the local destination was added,
         * <CODE>false</CODE> if a local destination with the same name
         * already exists
         */
        public bool LocalDestination(string name, PdfDestination destination) {
            return pdf.LocalDestination(name, destination);
        }
    
        /**
         * Gets a duplicate of this <CODE>PdfContentByte</CODE>. All
         * the members are copied by reference but the buffer stays different.
         *
         * @return a copy of this <CODE>PdfContentByte</CODE>
         */
        public virtual PdfContentByte Duplicate {
            get {
                return new PdfContentByte(writer);
            }
        }
    
        /**
         * Implements a link to another document.
         * @param filename the filename for the remote document
         * @param name the name to jump to
         * @param llx the lower left x corner of the activation area
         * @param lly the lower left y corner of the activation area
         * @param urx the upper right x corner of the activation area
         * @param ury the upper right y corner of the activation area
         */
        public void RemoteGoto(string filename, string name, float llx, float lly, float urx, float ury) {
            RemoteGoto(filename, name, llx, lly, urx, ury);
        }
    
        /**
         * Implements a link to another document.
         * @param filename the filename for the remote document
         * @param page the page to jump to
         * @param llx the lower left x corner of the activation area
         * @param lly the lower left y corner of the activation area
         * @param urx the upper right x corner of the activation area
         * @param ury the upper right y corner of the activation area
         */
        public void RemoteGoto(string filename, int page, float llx, float lly, float urx, float ury) {
            pdf.RemoteGoto(filename, page, llx, lly, urx, ury);
        }
        /**
         * Adds a round rectangle to the current path.
         *
         * @param x x-coordinate of the starting point
         * @param y y-coordinate of the starting point
         * @param w width
         * @param h height
         * @param r radius of the arc corner
         */
        public void RoundRectangle(float x, float y, float w, float h, float r) {
            if (w < 0) {
                x += w;
                w = -w;
            }
            if (h < 0) {
                y += h;
                h = -h;
            }
            if (r < 0)
                r = -r;
            float b = 0.4477f;
            MoveTo(x + r, y);
            LineTo(x + w - r, y);
            CurveTo(x + w - r * b, y, x + w, y + r * b, x + w, y + r);
            LineTo(x + w, y + h - r);
            CurveTo(x + w, y + h - r * b, x + w - r * b, y + h, x + w - r, y + h);
            LineTo(x + r, y + h);
            CurveTo(x + r * b, y + h, x, y + h - r * b, x, y + h - r);
            LineTo(x, y + r);
            CurveTo(x, y + r * b, x + r * b, y, x + r, y);
        }
    
        /** Implements an action in an area.
         * @param action the <CODE>PdfAction</CODE>
         * @param llx the lower left x corner of the activation area
         * @param lly the lower left y corner of the activation area
         * @param urx the upper right x corner of the activation area
         * @param ury the upper right y corner of the activation area
         */
        public virtual void SetAction(PdfAction action, float llx, float lly, float urx, float ury) {
            pdf.SetAction(action, llx, lly, urx, ury);
        }
    
        /** Outputs a <CODE>string</CODE> directly to the content.
         * @param s the <CODE>string</CODE>
         */    
        public void SetLiteral(string s) {
            content.Append(s);
        }
    
        /** Outputs a <CODE>char</CODE> directly to the content.
         * @param c the <CODE>char</CODE>
         */    
        public void SetLiteral(char c) {
            content.Append(c);
        }
    
        /** Outputs a <CODE>float</CODE> directly to the content.
         * @param n the <CODE>float</CODE>
         */    
        public void SetLiteral(float n) {
            content.Append(n);
        }
    
        /** Throws an error if it is a pattern.
         * @param t the object to check
         */    
        internal void CheckNoPattern(PdfTemplate t) {
            if (t.Type == PdfTemplate.TYPE_PATTERN)
                throw new ArgumentException("Invalid use of a pattern. A template was expected.");
        }
    
        /**
         * Draws a TextField.
         */
    
        public void DrawRadioField(float llx, float lly, float urx, float ury, bool on) {
            if (llx > urx) { float x = llx; llx = urx; urx = x; }
            if (lly > ury) { float y = lly; lly = ury; ury = y; }
            // silver circle
            SetLineWidth(1);
            SetLineCap(1);
            SetColorStroke(new Color(0xC0, 0xC0, 0xC0));
            Arc(llx + 1f, lly + 1f, urx - 1f, ury - 1f, 0f, 360f);
            Stroke();
            // gray circle-segment
            SetLineWidth(1);
            SetLineCap(1);
            SetColorStroke(new Color(0xA0, 0xA0, 0xA0));
            Arc(llx + 0.5f, lly + 0.5f, urx - 0.5f, ury - 0.5f, 45, 180);
            Stroke();
            // black circle-segment
            SetLineWidth(1);
            SetLineCap(1);
            SetColorStroke(new Color(0x00, 0x00, 0x00));
            Arc(llx + 1.5f, lly + 1.5f, urx - 1.5f, ury - 1.5f, 45, 180);
            Stroke();
            if (on) {
                // gray circle
                SetLineWidth(1);
                SetLineCap(1);
                SetColorFill(new Color(0x00, 0x00, 0x00));
                Arc(llx + 4f, lly + 4f, urx - 4f, ury - 4f, 0, 360);
                Fill();
            }
        }
    
        /**
         * Draws a TextField.
         */
    
        public void DrawTextField(float llx, float lly, float urx, float ury) {
            if (llx > urx) { float x = llx; llx = urx; urx = x; }
            if (lly > ury) { float y = lly; lly = ury; ury = y; }
            // silver rectangle not filled
            SetColorStroke(new Color(0xC0, 0xC0, 0xC0));
            SetLineWidth(1);
            SetLineCap(0);
            Rectangle(llx, lly, urx - llx, ury - lly);
            Stroke();
            // white rectangle filled
            SetLineWidth(1);
            SetLineCap(0);
            SetColorFill(new Color(0xFF, 0xFF, 0xFF));
            Rectangle(llx + 0.5f, lly + 0.5f, urx - llx - 1f, ury -lly - 1f);
            Fill();
            // silver lines
            SetColorStroke(new Color(0xC0, 0xC0, 0xC0));
            SetLineWidth(1);
            SetLineCap(0);
            MoveTo(llx + 1f, lly + 1.5f);
            LineTo(urx - 1.5f, lly + 1.5f);
            LineTo(urx - 1.5f, ury - 1f);
            Stroke();
            // gray lines
            SetColorStroke(new Color(0xA0, 0xA0, 0xA0));
            SetLineWidth(1);
            SetLineCap(0);
            MoveTo(llx + 1f, lly + 1);
            LineTo(llx + 1f, ury - 1f);
            LineTo(urx - 1f, ury - 1f);
            Stroke();
            // black lines
            SetColorStroke(new Color(0x00, 0x00, 0x00));
            SetLineWidth(1);
            SetLineCap(0);
            MoveTo(llx + 2f, lly + 2f);
            LineTo(llx + 2f, ury - 2f);
            LineTo(urx - 2f, ury - 2f);
            Stroke();
        }
    
        /**
         * Draws a button.
         */
    
        public void DrawButton(float llx, float lly, float urx, float ury, string text, BaseFont bf, float size) {
            if (llx > urx) { float x = llx; llx = urx; urx = x; }
            if (lly > ury) { float y = lly; lly = ury; ury = y; }
            // black rectangle not filled
            SetColorStroke(new Color(0x00, 0x00, 0x00));
            SetLineWidth(1);
            SetLineCap(0);
            Rectangle(llx, lly, urx - llx, ury - lly);
            Stroke();
            // silver rectangle filled
            SetLineWidth(1);
            SetLineCap(0);
            SetColorFill(new Color(0xC0, 0xC0, 0xC0));
            Rectangle(llx + 0.5f, lly + 0.5f, urx - llx - 1f, ury -lly - 1f);
            Fill();
            // white lines
            SetColorStroke(new Color(0xFF, 0xFF, 0xFF));
            SetLineWidth(1);
            SetLineCap(0);
            MoveTo(llx + 1f, lly + 1f);
            LineTo(llx + 1f, ury - 1f);
            LineTo(urx - 1f, ury - 1f);
            Stroke();
            // dark grey lines
            SetColorStroke(new Color(0xA0, 0xA0, 0xA0));
            SetLineWidth(1);
            SetLineCap(0);
            MoveTo(llx + 1f, lly + 1f);
            LineTo(urx - 1f, lly + 1f);
            LineTo(urx - 1f, ury - 1f);
            Stroke();
            // text
            ResetRGBColorFill();
            BeginText();
            SetFontAndSize(bf, size);
            ShowTextAligned(PdfContentByte.ALIGN_CENTER, text, llx + (urx - llx) / 2, lly + (ury - lly - size) / 2, 0);
            EndText();
        }
    
        internal virtual PageResources PageResources {
            get {
                return pdf.PageResources;
            }
        }
        
        /** Sets the graphic state
        * @param gstate the graphic state
        */    
        public void SetGState(PdfGState gstate) {
            PdfObject[] obj = writer.AddSimpleExtGState(gstate);
            PageResources prs = PageResources;
            PdfName name = prs.AddExtGState((PdfName)obj[0], (PdfIndirectReference)obj[1]);
            content.Append(name.GetBytes()).Append(" gs").Append_i(separator);
        }
        
        /**
        * Begins a graphic block whose visibility is controled by the <CODE>layer</CODE>.
        * Blocks can be nested. Each block must be terminated by an {@link #endLayer()}.<p>
        * Note that nested layers with {@link PdfLayer#addChild(PdfLayer)} only require a single
        * call to this method and a single call to {@link #endLayer()}; all the nesting control
        * is built in.
        * @param layer the layer
        */    
        public void BeginLayer(IPdfOCG layer) {
            if ((layer is PdfLayer) && ((PdfLayer)layer).Title != null)
                throw new ArgumentException("A title is not a layer");
            if (layerDepth == null)
                layerDepth = new ArrayList();
            if (layer is PdfLayerMembership) {
                layerDepth.Add(1);
                BeginLayer2(layer);
                return;
            }
            int n = 0;
            PdfLayer la = (PdfLayer)layer;
            while (la != null) {
                if (la.Title == null) {
                    BeginLayer2(la);
                    ++n;
                }
                la = la.Parent;
            }
            layerDepth.Add(n);
        }
        
        private void BeginLayer2(IPdfOCG layer) {
            PdfName name = (PdfName)writer.AddSimpleProperty(layer, layer.Ref)[0];
            PageResources prs = PageResources;
            name = prs.AddProperty(name, layer.Ref);
            content.Append("/OC ").Append(name.GetBytes()).Append(" BDC").Append_i(separator);
        }
        
        /**
        * Ends a layer controled graphic block. It will end the most recent open block.
        */    
        public void EndLayer() {
            int n = 1;
            if (layerDepth != null && layerDepth.Count > 0) {
                n = (int)layerDepth[layerDepth.Count - 1];
                layerDepth.RemoveAt(layerDepth.Count - 1);
            }
            while (n-- > 0)
                content.Append("EMC").Append_i(separator);
        }
        
        internal virtual void AddAnnotation(PdfAnnotation annot) {
            writer.AddAnnotation(annot);
        }
        
        /**
        * Sets the default colorspace.
        * @param name the name of the colorspace. It can be <CODE>PdfName.DEFAULTGRAY</CODE>, <CODE>PdfName.DEFAULTRGB</CODE>
        * or <CODE>PdfName.DEFAULTCMYK</CODE>
        * @param obj the colorspace. A <CODE>null</CODE> or <CODE>PdfNull</CODE> removes any colorspace with the same name
        */    
        public virtual void SetDefaultColorspace(PdfName name, PdfObject obj) {
            PageResources prs = PageResources;
            prs.AddDefaultColor(name, obj);
        }

        public void Transform(System.Drawing.Drawing2D.Matrix tx) {
            float[] c = tx.Elements;
            ConcatCTM(c[0], c[1], c[2], c[3], c[4], c[5]);
        }

        /**
        * Begins a marked content sequence. This sequence will be tagged with the structure <CODE>struc</CODE>.
        * The same structure can be used several times to connect text that belongs to the same logical segment
        * but is in a different location, like the same paragraph crossing to another page, for example.
        * @param struc the tagging structure
        */    
        public void BeginMarkedContentSequence(PdfStructureElement struc) {
            PdfObject obj = struc.Get(PdfName.K);
            int mark = pdf.GetMarkPoint();
            if (obj != null) {
                PdfArray ar = null;
                if (obj.IsNumber()) {
                    ar = new PdfArray();
                    ar.Add(obj);
                    struc.Put(PdfName.K, ar);
                }
                else if (obj.IsArray()) {
                    ar = (PdfArray)obj;
                    if (!((PdfObject)ar.ArrayList[0]).IsNumber())
                        throw new ArgumentException("The structure has kids.");
                }
                else
                    throw new ArgumentException("Unknown object at /K " + obj.GetType().ToString());
                PdfDictionary dic = new PdfDictionary(PdfName.MCR);
                dic.Put(PdfName.PG, writer.CurrentPage);
                dic.Put(PdfName.MCID, new PdfNumber(mark));
                ar.Add(dic);
                struc.SetPageMark(writer.PageNumber - 1, -1);
            }
            else {
                struc.SetPageMark(writer.PageNumber - 1, mark);
                struc.Put(PdfName.PG, writer.CurrentPage);
            }
            pdf.IncMarkPoint();
            content.Append(struc.Get(PdfName.S).GetBytes()).Append(" <</MCID ").Append(mark).Append(">> BDC").Append_i(separator);
        }
        
        /**
        * Ends a marked content sequence
        */    
        public void EndMarkedContentSequence() {
            content.Append("EMC").Append_i(separator);
        }
        
        /**
        * Begins a marked content sequence. If property is <CODE>null</CODE> the mark will be of the type
        * <CODE>BMC</CODE> otherwise it will be <CODE>BDC</CODE>.
        * @param tag the tag
        * @param property the property
        * @param inline <CODE>true</CODE> to include the property in the content or <CODE>false</CODE>
        * to include the property in the resource dictionary with the possibility of reusing
        */    
        public void BeginMarkedContentSequence(PdfName tag, PdfDictionary property, bool inline) {
            if (property == null) {
                content.Append(tag.GetBytes()).Append(" BMC").Append_i(separator);
                return;
            }
            content.Append(tag.GetBytes()).Append(' ');
            if (inline)
                property.ToPdf(writer, content);
            else {
                PdfObject[] objs;
                if (writer.PropertyExists(property))
                    objs = writer.AddSimpleProperty(property, null);
                else
                    objs = writer.AddSimpleProperty(property, writer.PdfIndirectReference);
                PdfName name = (PdfName)objs[0];
                PageResources prs = PageResources;
                name = prs.AddProperty(name, (PdfIndirectReference)objs[1]);
                content.Append(name.GetBytes());
            }
            content.Append(" BDC").Append_i(separator);
        }
        
        /**
        * This is just a shorthand to <CODE>beginMarkedContentSequence(tag, null, false)</CODE>.
        * @param tag the tag
        */    
        public void BeginMarkedContentSequence(PdfName tag) {
            BeginMarkedContentSequence(tag, null, false);
        }
    }
}