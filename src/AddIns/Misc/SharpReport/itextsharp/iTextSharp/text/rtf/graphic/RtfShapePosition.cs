using System;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.rtf;
/**
 * $Id: RtfShapePosition.cs,v 1.6 2008/05/23 17:24:27 psoares33 Exp $
 * 
 *
 * Copyright 2006 by Mark Hall
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
 * the Initial Developer are Copyright (C) 1999-2006 by Bruno Lowagie.
 * All Rights Reserved.
 * Co-Developer of the code is Paulo Soares. Portions created by the Co-Developer
 * are Copyright (C) 2000-2006 by Paulo Soares. All Rights Reserved.
 *
 * Contributor(s): all the names of the contributors are added in the source code
 * where applicable.
 *
 * Alternatively, the contents of this file may be used under the terms of the
 * LGPL license (the ?GNU LIBRARY GENERAL PUBLIC LICENSE?), in which case the
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

namespace iTextSharp.text.rtf.graphic {

    /**
    * The RtfShapePosition stores position and ordering
    * information for one RtfShape.
    * 
    * @version $Revision$
    * @author Mark Hall (Mark.Hall@mail.room3b.eu)
    */
    public class RtfShapePosition : RtfAddableElement {
        /**
        * Constant for horizontal positioning relative to the page.
        */
        public const int POSITION_X_RELATIVE_PAGE = 0;
        /**
        * Constant for horizontal positioning relative to the margin.
        */
        public const int POSITION_X_RELATIVE_MARGIN = 1;
        /**
        * Constant for horizontal positioning relative to the column.
        */
        public const int POSITION_X_RELATIVE_COLUMN = 2;
        /**
        * Constant for vertical positioning relative to the page.
        */
        public const int POSITION_Y_RELATIVE_PAGE = 0;
        /**
        * Constant for vertical positioning relative to the margin.
        */
        public const int POSITION_Y_RELATIVE_MARGIN = 1;
        /**
        * Constant for vertical positioning relative to the paragraph.
        */
        public const int POSITION_Y_RELATIVE_PARAGRAPH = 2;
        
        /**
        * The top coordinate of this RtfShapePosition.
        */
        private int top = 0;
        /**
        * The left coordinate of this RtfShapePosition.
        */
        private int left = 0;
        /**
        * The right coordinate of this RtfShapePosition.
        */
        private int right = 0;
        /**
        * The bottom coordinate of this RtfShapePosition.
        */
        private int bottom = 0;
        /**
        * The z order of this RtfShapePosition.
        */
        private int zOrder = 0;
        /**
        * The horizontal relative position.
        */
        private int xRelativePos = POSITION_X_RELATIVE_PAGE;
        /**
        * The vertical relative position.
        */
        private int yRelativePos = POSITION_Y_RELATIVE_PAGE;
        /**
        * Whether to ignore the horizontal relative position.
        */
        private bool ignoreXRelative = false;
        /**
        * Whether to ignore the vertical relative position.
        */
        private bool ignoreYRelative = false;
        /**
        * Whether the shape is below the text.
        */
        private bool shapeBelowText = false;

        /**
        * Constructs a new RtfShapePosition with the four bounding coordinates.
        * 
        * @param top The top coordinate.
        * @param left The left coordinate.
        * @param right The right coordinate.
        * @param bottom The bottom coordinate.
        */
        public RtfShapePosition(int top, int left, int right, int bottom) {
            this.top = top;
            this.left = left;
            this.right = right;
            this.bottom = bottom;
        }
        
        /**
        * Gets whether the shape is below the text.
        * 
        * @return <code>True</code> if the shape is below, <code>false</code> if the text is below.
        */
        public bool IsShapeBelowText() {
            return shapeBelowText;
        }

        /**
        * Sets whether the shape is below the text.
        * 
        * @param shapeBelowText <code>True</code> if the shape is below, <code>false</code> if the text is below.
        */
        public void SetShapeBelowText(bool shapeBelowText) {
            this.shapeBelowText = shapeBelowText;
        }

        /**
        * Sets the relative horizontal position. Use one of the constants
        * provided in this class.
        * 
        * @param relativePos The relative horizontal position to use.
        */
        public void SetXRelativePos(int relativePos) {
            xRelativePos = relativePos;
        }

        /**
        * Sets the relative vertical position. Use one of the constants
        * provides in this class.
        * 
        * @param relativePos The relative vertical position to use.
        */
        public void SetYRelativePos(int relativePos) {
            yRelativePos = relativePos;
        }

        /**
        * Sets the z order to use.
        * 
        * @param order The z order to use.
        */
        public void SetZOrder(int order) {
            zOrder = order;
        }

        /**
        * Set whether to ignore the horizontal relative position.
        * 
        * @param ignoreXRelative <code>True</code> to ignore the horizontal relative position, <code>false</code> otherwise.
        */
        protected internal void SetIgnoreXRelative(bool ignoreXRelative) {
            this.ignoreXRelative = ignoreXRelative;
        }

        /**
        * Set whether to ignore the vertical relative position.
        * 
        * @param ignoreYRelative <code>True</code> to ignore the vertical relative position, <code>false</code> otherwise.
        */
        protected internal void SetIgnoreYRelative(bool ignoreYRelative) {
            this.ignoreYRelative = ignoreYRelative;
        }

        /**
        * Write this RtfShapePosition.
        */
        public override void WriteContent(Stream result) {       
            byte[] t;
            result.Write(t = DocWriter.GetISOBytes("\\shpleft"), 0, t.Length);
            result.Write(t = IntToByteArray(this.left), 0, t.Length);
            result.Write(t = DocWriter.GetISOBytes("\\shptop"), 0, t.Length);
            result.Write(t = IntToByteArray(this.top), 0, t.Length);
            result.Write(t = DocWriter.GetISOBytes("\\shpright"), 0, t.Length);
            result.Write(t = IntToByteArray(this.right), 0, t.Length);
            result.Write(t = DocWriter.GetISOBytes("\\shpbottom"), 0, t.Length);
            result.Write(t = IntToByteArray(this.bottom), 0, t.Length);
            result.Write(t = DocWriter.GetISOBytes("\\shpz"), 0, t.Length);
            result.Write(t = IntToByteArray(this.zOrder), 0, t.Length);
            switch(this.xRelativePos) {
            case POSITION_X_RELATIVE_PAGE: result.Write(t = DocWriter.GetISOBytes("\\shpbxpage"), 0, t.Length); break;
            case POSITION_X_RELATIVE_MARGIN: result.Write(t = DocWriter.GetISOBytes("\\shpbxmargin"), 0, t.Length); break;
            case POSITION_X_RELATIVE_COLUMN: result.Write(t = DocWriter.GetISOBytes("\\shpbxcolumn"), 0, t.Length); break;
            }
            if(this.ignoreXRelative) {
                result.Write(t = DocWriter.GetISOBytes("\\shpbxignore"), 0, t.Length);
            }
            switch(this.yRelativePos) {
            case POSITION_Y_RELATIVE_PAGE: result.Write(t = DocWriter.GetISOBytes("\\shpbypage"), 0, t.Length); break;
            case POSITION_Y_RELATIVE_MARGIN: result.Write(t = DocWriter.GetISOBytes("\\shpbymargin"), 0, t.Length); break;
            case POSITION_Y_RELATIVE_PARAGRAPH: result.Write(t = DocWriter.GetISOBytes("\\shpbypara"), 0, t.Length); break;
            }
            if(this.ignoreYRelative) {
                result.Write(t = DocWriter.GetISOBytes("\\shpbyignore"), 0, t.Length);
            }
            if(this.shapeBelowText) {
                result.Write(t = DocWriter.GetISOBytes("\\shpfblwtxt1"), 0, t.Length);
            } else {
                result.Write(t = DocWriter.GetISOBytes("\\shpfblwtxt0"), 0, t.Length);
            }
        }
    }
}