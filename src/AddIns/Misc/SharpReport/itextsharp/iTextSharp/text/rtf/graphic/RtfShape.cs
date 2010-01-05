using System;
using System.IO;
using System.Collections;
using iTextSharp.text;
using iTextSharp.text.rtf;
/**
 * $Id: RtfShape.cs,v 1.7 2008/05/23 17:24:27 psoares33 Exp $
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
    * The RtfShape provides the interface for adding shapes to
    * the RTF document. This will only work for Word 97+, older
    * Word versions are not supported by this class.<br /><br />
    * 
    * Only very simple shapes are directly supported by the RtfShape.
    * For more complex shapes you will have to read the RTF
    * specification (iText follows the 1.6 specification) and add
    * the desired properties via the RtfShapeProperty.<br /><br />
    * 
    * One thing to keep in mind is that distances are not expressed
    * in the standard iText point, but in EMU where 1 inch = 914400 EMU
    * or 1 cm = 360000 EMU. 
    * 
    * @version $Revision$
    * @author Mark Hall (Mark.Hall@mail.room3b.eu)
    */
    public class RtfShape : RtfAddableElement {
        /**
        * Constant for a free form shape. The shape verticies must
        * be specified with an array of Point objects in a
        * RtfShapeProperty with the name PROPERTY_VERTICIES.
        */
        public const int SHAPE_FREEFORM = 0;
        /**
        * Constant for a rectangle.
        */
        public const int SHAPE_RECTANGLE = 1;
        /**
        * Constant for a rounded rectangle. The roundness is
        * set via a RtfShapeProperty with the name PROPERTY_ADJUST_VALUE.
        */
        public const int SHAPE_ROUND_RECTANGLE = 2;
        /**
        * Constant for an ellipse. Use this to create circles.
        */
        public const int SHAPE_ELLIPSE = 3;
        /**
        * Constant for a diamond.
        */
        public const int SHAPE_DIAMOND = 4;
        /**
        * Constant for a isoscelle triangle.
        */
        public const int SHAPE_TRIANGLE_ISOSCELES = 5;
        /**
        * Constant for a right triangle.
        */
        public const int SHAPE_TRIANGLE_RIGHT = 6;
        /**
        * Constant for a parallelogram.
        */
        public const int SHAPE_PARALLELOGRAM = 7;
        /**
        * Constant for a trapezoid.
        */
        public const int SHAPE_TRAPEZOID = 8;
        /**
        * Constant for a hexagon.
        */
        public const int SHAPE_HEXAGON = 9;
        /**
        * Constant for an ocatagon.
        */
        public const int SHAPE_OCTAGON = 10;
        /**
        * Constant for a star.
        */
        public const int SHAPE_STAR = 12;
        /**
        * Constant for an arrow.
        */
        public const int SHAPE_ARROW = 13;
        /**
        * Constant for a thick arrow.
        */
        public const int SHAPE_ARROR_THICK = 14;
        /**
        * Constant for a home plate style shape.
        */
        public const int SHAPE_HOME_PLATE = 15;
        /**
        * Constant for a cube shape.
        */
        public const int SHAPE_CUBE = 16;
        /**
        * Constant for a balloon shape.
        */
        public const int SHAPE_BALLOON = 17;
        /**
        * Constant for a seal shape.
        */
        public const int SHAPE_SEAL = 18;
        /**
        * Constant for an arc shape.
        */
        public const int SHAPE_ARC = 19;
        /**
        * Constant for a line shape.
        */
        public const int SHAPE_LINE = 20;
        /**
        * Constant for a can shape.
        */
        public const int SHAPE_CAN = 22;
        /**
        * Constant for a donut shape.
        */
        public const int SHAPE_DONUT = 23;
        
        /**
        * Constant for a Picture Frame.
        */
        public const int SHAPE_PICTURE_FRAME = 75;
        /**
        * Text is not wrapped around the shape.
        */
        public const int SHAPE_WRAP_NONE = 0;
        /**
        * Text is wrapped to the top and bottom.
        */
        public const int SHAPE_WRAP_TOP_BOTTOM = 1;
        /**
        * Text is wrapped on the left and right side.
        */
        public const int SHAPE_WRAP_BOTH = 2;
        /**
        * Text is wrapped on the left side.
        */
        public const int SHAPE_WRAP_LEFT = 3;
        /**
        * Text is wrapped on the right side.
        */
        public const int SHAPE_WRAP_RIGHT = 4;
        /**
        * Text is wrapped on the largest side.
        */
        public const int SHAPE_WRAP_LARGEST = 5;
        /**
        * Text is tightly wrapped on the left and right side.
        */
        public const int SHAPE_WRAP_TIGHT_BOTH = 6;
        /**
        * Text is tightly wrapped on the left side.
        */
        public const int SHAPE_WRAP_TIGHT_LEFT = 7;
        /**
        * Text is tightly wrapped on the right side.
        */
        public const int SHAPE_WRAP_TIGHT_RIGHT = 8;
        /**
        * Text is tightly wrapped on the largest side.
        */
        public const int SHAPE_WRAP_TIGHT_LARGEST = 9;
        /**
        * Text is wrapped through the shape.
        */
        public const int SHAPE_WRAP_THROUGH = 10;
        
        /**
        * The shape nr is a random unique id.
        */
        private int shapeNr = 0;
        /**
        * The shape type.
        */
        private int type = 0;
        /**
        * The RtfShapePosition that defines position settings for this RtfShape.
        */
        private RtfShapePosition position = null;
        /**
        * A Hashtable with RtfShapePropertys that define further shape properties.
        */
        private Hashtable properties = null;
        /**
        * The wrapping mode. Defaults to SHAPE_WRAP_NONE;
        */
        private int wrapping = SHAPE_WRAP_NONE;
        /**
        * Text that is contained in the shape.
        */
        private String shapeText = "";
        
        /**
        * Constructs a new RtfShape of a given shape at the given RtfShapePosition.
        * 
        * @param type The type of shape to create.
        * @param position The RtfShapePosition to create this RtfShape at.
        */
        public RtfShape(int type, RtfShapePosition position) {
            this.type = type;
            this.position = position;
            this.properties = new Hashtable();
        }

        /**
        * Sets a property.
        * 
        * @param property The property to set for this RtfShape.
        */
        public void SetProperty(RtfShapeProperty property) {
            this.properties[property.GetName()] = property;
        }
        
        /**
        * Sets the text to display in this RtfShape.
        * 
        * @param shapeText The text to display.
        */
        public void SetShapeText(String shapeText) {
            this.shapeText = shapeText;
        }

        /**
        * Set the wrapping mode.
        * 
        * @param wrapping The wrapping mode to use for this RtfShape.
        */
        public void SetWrapping(int wrapping) {
            this.wrapping = wrapping;
        }

        /**
        * Writes the RtfShape. Some settings are automatically translated into
        * or require other properties and these are set first.
        */    
        public override void WriteContent(Stream result) {
            this.shapeNr = this.doc.GetRandomInt();
            
            this.properties["ShapeType"] = new RtfShapeProperty("ShapeType", this.type);
            if (this.position.IsShapeBelowText()) {
                this.properties["fBehindDocument"] = new RtfShapeProperty("fBehindDocument", true);
            }
            if (this.inTable) {
                this.properties["fLayoutInCell"] = new RtfShapeProperty("fLayoutInCell", true);
            }
            if (this.properties.ContainsKey("posh")) {
                this.position.SetIgnoreXRelative(true);
            }
            if (this.properties.ContainsKey("posv")) {
                this.position.SetIgnoreYRelative(true);
            }
            
            byte[] t;
            result.Write(RtfElement.OPEN_GROUP, 0, RtfElement.OPEN_GROUP.Length);
            result.Write(t = DocWriter.GetISOBytes("\\shp"), 0, t.Length);
            result.Write(t = DocWriter.GetISOBytes("\\shplid"), 0, t.Length);
            result.Write(t = IntToByteArray(this.shapeNr), 0, t.Length);
            this.position.WriteContent(result);
            switch (this.wrapping) {
                case SHAPE_WRAP_NONE:
                    result.Write(t = DocWriter.GetISOBytes("\\shpwr3"), 0, t.Length);
                    break;
                case SHAPE_WRAP_TOP_BOTTOM:
                    result.Write(t = DocWriter.GetISOBytes("\\shpwr1"), 0, t.Length);
                    break;
                case SHAPE_WRAP_BOTH:
                    result.Write(t = DocWriter.GetISOBytes("\\shpwr2"), 0, t.Length);
                    result.Write(t = DocWriter.GetISOBytes("\\shpwrk0"), 0, t.Length);
                    break;
                case SHAPE_WRAP_LEFT:
                    result.Write(t = DocWriter.GetISOBytes("\\shpwr2"), 0, t.Length);
                    result.Write(t = DocWriter.GetISOBytes("\\shpwrk1"), 0, t.Length);
                    break;
                case SHAPE_WRAP_RIGHT:
                    result.Write(t = DocWriter.GetISOBytes("\\shpwr2"), 0, t.Length);
                    result.Write(t = DocWriter.GetISOBytes("\\shpwrk2"), 0, t.Length);
                    break;
                case SHAPE_WRAP_LARGEST:
                    result.Write(t = DocWriter.GetISOBytes("\\shpwr2"), 0, t.Length);
                    result.Write(t = DocWriter.GetISOBytes("\\shpwrk3"), 0, t.Length);
                    break;
                case SHAPE_WRAP_TIGHT_BOTH:
                    result.Write(t = DocWriter.GetISOBytes("\\shpwr4"), 0, t.Length);
                    result.Write(t = DocWriter.GetISOBytes("\\shpwrk0"), 0, t.Length);
                    break;
                case SHAPE_WRAP_TIGHT_LEFT:
                    result.Write(t = DocWriter.GetISOBytes("\\shpwr4"), 0, t.Length);
                    result.Write(t = DocWriter.GetISOBytes("\\shpwrk1"), 0, t.Length);
                    break;
                case SHAPE_WRAP_TIGHT_RIGHT:
                    result.Write(t = DocWriter.GetISOBytes("\\shpwr4"), 0, t.Length);
                    result.Write(t = DocWriter.GetISOBytes("\\shpwrk2"), 0, t.Length);
                    break;
                case SHAPE_WRAP_TIGHT_LARGEST:
                    result.Write(t = DocWriter.GetISOBytes("\\shpwr4"), 0, t.Length);
                    result.Write(t = DocWriter.GetISOBytes("\\shpwrk3"), 0, t.Length);
                    break;
                case SHAPE_WRAP_THROUGH:
                    result.Write(t = DocWriter.GetISOBytes("\\shpwr5"), 0, t.Length);
                    break;
                default:
                    result.Write(t = DocWriter.GetISOBytes("\\shpwr3"), 0, t.Length);
                    break;
            }
            if (this.inHeader) {
                result.Write(t = DocWriter.GetISOBytes("\\shpfhdr1"), 0, t.Length);
            } 
            if (this.doc.GetDocumentSettings().IsOutputDebugLineBreaks()) {
                result.WriteByte((byte)'\n');
            }
            result.Write(RtfElement.OPEN_GROUP, 0, RtfElement.OPEN_GROUP.Length);
            result.Write(t = DocWriter.GetISOBytes("\\*\\shpinst"), 0, t.Length);
            foreach (RtfShapeProperty rsp in this.properties.Values) {
                rsp.WriteContent(result);
            }
            if (!this.shapeText.Equals("")) {
                result.Write(RtfElement.OPEN_GROUP, 0, RtfElement.OPEN_GROUP.Length);
                result.Write(t = DocWriter.GetISOBytes("\\shptxt"), 0, t.Length);
                result.Write(RtfElement.DELIMITER, 0, RtfElement.DELIMITER.Length);
                result.Write(t = DocWriter.GetISOBytes(this.shapeText), 0, t.Length);
                result.Write(RtfElement.CLOSE_GROUP, 0, RtfElement.CLOSE_GROUP.Length);
            }
            result.Write(RtfElement.CLOSE_GROUP, 0, RtfElement.CLOSE_GROUP.Length);
            if (this.doc.GetDocumentSettings().IsOutputDebugLineBreaks()) {
                result.WriteByte((byte)'\n');
            }
            result.Write(RtfElement.CLOSE_GROUP, 0, RtfElement.CLOSE_GROUP.Length);
        }
    }
}