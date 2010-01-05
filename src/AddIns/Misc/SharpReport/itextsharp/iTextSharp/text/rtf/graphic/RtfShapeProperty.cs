using System;
using System.IO;
using System.Drawing;
using iTextSharp.text;
using iTextSharp.text.rtf;
/**
 * $Id: RtfShapeProperty.cs,v 1.8 2008/05/23 17:24:27 psoares33 Exp $
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
    * The RtfShapeProperty stores all shape properties that are
    * not handled by the RtfShape and RtfShapePosition.<br /><br />
    * 
    * There is a huge selection of properties that can be set. For
    * the most important properites there are constants for the
    * property name, for all others you must find the correct
    * property name in the RTF specification (version 1.6).<br /><br />
    * 
    * The following types of property values are supported:
    * <ul>
    *   <li>long</li>
    *   <li>double</li>
    *   <li>bool</li>
    *   <li>Color</li>
    *   <li>int[]</li>
    *   <li>Point[]</li>
    * </ul>
    * 
    * @version $Revision$
    * @author Mark Hall (Mark.Hall@mail.room3b.eu)
    */
    public class RtfShapeProperty : RtfAddableElement {
        /**
        * Property for defining an image.
        */
        public const String PROPERTY_IMAGE = "pib";
        /**
        * Property for defining vertices in freeform shapes. Requires a
        * Point array as the value.
        */
        public const String PROPERTY_VERTICIES = "pVerticies";
        /**
        * Property for defining the minimum vertical coordinate that is
        * visible. Requires a long value.
        */
        public const String PROPERTY_GEO_TOP = "geoTop";
        /**
        * Property for defining the minimum horizontal coordinate that is
        * visible. Requires a long value.
        */
        public const String PROPERTY_GEO_LEFT = "geoLeft";
        /**
        * Property for defining the maximum horizontal coordinate that is
        * visible. Requires a long value.
        */
        public const String PROPERTY_GEO_RIGHT = "geoRight";
        /**
        * Property for defining the maximum vertical coordinate that is
        * visible. Requires a long value.
        */
        public const String PROPERTY_GEO_BOTTOM = "geoBottom";
        /**
        * Property for defining that the shape is in a table cell. Requires
        * a bool value.
        */
        public const String PROPERTY_LAYOUT_IN_CELL = "fLayoutInCell";
        /**
        * Property for signalling a vertical flip of the shape. Requires a
        * bool value.
        */
        public const String PROPERTY_FLIP_V = "fFlipV";
        /**
        * Property for signalling a horizontal flip of the shape. Requires a
        * bool value.
        */
        public const String PROPERTY_FLIP_H = "fFlipH";
        /**
        * Property for defining the fill color of the shape. Requires a
        * Color value.
        */
        public const String PROPERTY_FILL_COLOR = "fillColor";
        /**
        * Property for defining the line color of the shape. Requires a
        * Color value.
        */
        public const String PROPERTY_LINE_COLOR = "lineColor";
        /**
        * Property for defining the first adjust handle for shapes. Used
        * with the rounded rectangle. Requires a long value.
        */
        public const String PROPERTY_ADJUST_VALUE = "adjustValue";

        /**
        * The stored value is a long.
        */
        private const int PROPERTY_TYPE_LONG = 1;
        /**
        * The stored value is bool.
        */
        private const int PROPERTY_TYPE_BOOLEAN = 2;
        /**
        * The stored value is a double.
        */
        private const int PROPERTY_TYPE_DOUBLE = 3;
        /**
        * The stored value is a Color.
        */
        private const int PROPERTY_TYPE_COLOR = 4;
        /**
        * The stored value is either an int or a Point array.
        */
        private const int PROPERTY_TYPE_ARRAY = 5;
        /**
        * The stored value is an Image.
        */
        private const int PROPERTY_TYPE_IMAGE = 6;
        
        /**
        * The value type.
        */
        private int type = 0;
        /**
        * The RtfShapeProperty name.
        */
        private String name = "";
        /**
        * The RtfShapeProperty value.
        */
        private Object value = null;
        
        /**
        * Internaly used to create the RtfShape.
        * 
        * @param name The property name to use.
        * @param value The property value to use.
        */
        private RtfShapeProperty(String name, Object value) {
            this.name = name;
            this.value = value;
        }
        
        /**
        * Constructs a RtfShapeProperty with a long value.
        * 
        * @param name The property name to use.
        * @param value The long value to use.
        */
        public RtfShapeProperty(String name, long value) {
            this.name = name;
            this.value = value;
            this.type = PROPERTY_TYPE_LONG;
        }
        
        /**
        * Constructs a RtfShapeProperty with a double value.
        * 
        * @param name The property name to use.
        * @param value The double value to use.
        */
        public RtfShapeProperty(String name, double value) {
            this.name = name;
            this.value = value;
            this.type = PROPERTY_TYPE_DOUBLE;
        }
        
        /**
        * Constructs a RtfShapeProperty with a bool value.
        * 
        * @param name The property name to use.
        * @param value The bool value to use.
        */
        public RtfShapeProperty(String name, bool value) {
            this.name = name;
            this.value = value;
            this.type = PROPERTY_TYPE_BOOLEAN;
        }
        
        /**
        * Constructs a RtfShapeProperty with a Color value.
        * 
        * @param name The property name to use.
        * @param value The Color value to use.
        */
        public RtfShapeProperty(String name, Color value) {
            this.name = name;
            this.value = value;
            this.type = PROPERTY_TYPE_COLOR;
        }
        
        /**
        * Constructs a RtfShapeProperty with an int array value.
        * 
        * @param name The property name to use.
        * @param value The int array to use.
        */
        public RtfShapeProperty(String name, int[] value) {
            this.name = name;
            this.value = value;
            this.type = PROPERTY_TYPE_ARRAY;
        }
        
        /**
        * Constructs a RtfShapeProperty with a Point array value.
        * 
        * @param name The property name to use.
        * @param value The Point array to use.
        */
        public RtfShapeProperty(String name, Point[] value) {
            this.name = name;
            this.value = value;
            this.type = PROPERTY_TYPE_ARRAY;
        }
        
        /**
        * Constructs a RtfShapeProperty with an Image value.
        * 
        * @param name The property name to use.
        * @param value The Image to use.
        */
        public RtfShapeProperty(String name, Image value) {
            this.name = name;
            this.value = value;
            this.type = PROPERTY_TYPE_IMAGE;
        }
        
        /**
        * Gets the name of this RtfShapeProperty.
        * 
        * @return The name of this RtfShapeProperty.
        */
        public String GetName() {
            return this.name;
        }
        
        /**
        * Write this RtfShapePosition.
        */
        public override void WriteContent(Stream result) {       
            byte[] t;
            result.Write(RtfElement.OPEN_GROUP, 0, RtfElement.OPEN_GROUP.Length);
            result.Write(t = DocWriter.GetISOBytes("\\sp"), 0, t.Length);
            result.Write(RtfElement.OPEN_GROUP, 0, RtfElement.OPEN_GROUP.Length);
            result.Write(t = DocWriter.GetISOBytes("\\sn"), 0, t.Length);
            result.Write(RtfElement.DELIMITER, 0, RtfElement.DELIMITER.Length);
            result.Write(t = DocWriter.GetISOBytes(this.name), 0, t.Length);
            result.Write(RtfElement.CLOSE_GROUP, 0, RtfElement.CLOSE_GROUP.Length);
            result.Write(RtfElement.OPEN_GROUP, 0, RtfElement.OPEN_GROUP.Length);
            result.Write(t = DocWriter.GetISOBytes("\\sv"), 0, t.Length);
            result.Write(RtfElement.DELIMITER, 0, RtfElement.DELIMITER.Length);
            switch (this.type) {
            case PROPERTY_TYPE_LONG: 
            case PROPERTY_TYPE_DOUBLE:
                result.Write(t = DocWriter.GetISOBytes(this.value.ToString()), 0, t.Length);
                break;
            case PROPERTY_TYPE_BOOLEAN:
                if ((bool)this.value) {
                    result.Write(t = DocWriter.GetISOBytes("1"), 0, t.Length);
                } else {
                    result.Write(t = DocWriter.GetISOBytes("0"), 0, t.Length);
                }
                break;
            case PROPERTY_TYPE_COLOR:
                Color color = (Color) this.value;
                result.Write(t = IntToByteArray(color.R | (color.G << 8) | (color.B << 16)), 0, t.Length);
                break;
            case PROPERTY_TYPE_ARRAY:
                if (this.value is int[]) {
                    int[] values = (int[]) this.value;
                    result.Write(t = DocWriter.GetISOBytes("4;"), 0, t.Length);
                    result.Write(t = IntToByteArray(values.Length), 0, t.Length);
                    result.Write(RtfElement.COMMA_DELIMITER, 0, RtfElement.COMMA_DELIMITER.Length);
                    for (int i = 0; i < values.Length; i++) {
                        result.Write(t = IntToByteArray(values[i]), 0, t.Length);
                        if (i < values.Length - 1) {
                            result.Write(RtfElement.COMMA_DELIMITER, 0, RtfElement.COMMA_DELIMITER.Length);
                        }
                    }
                } else if (this.value is Point[]) {
                    Point[] values = (Point[]) this.value;
                    result.Write(t = DocWriter.GetISOBytes("8;"), 0, t.Length);
                    result.Write(t = IntToByteArray(values.Length), 0, t.Length);
                    result.Write(RtfElement.COMMA_DELIMITER, 0, RtfElement.COMMA_DELIMITER.Length);
                    for (int i = 0; i < values.Length; i++) {
                        result.Write(t = DocWriter.GetISOBytes("("), 0, t.Length);
                        result.Write(t = IntToByteArray(values[i].X), 0, t.Length);
                        result.Write(t = DocWriter.GetISOBytes(","), 0, t.Length);
                        result.Write(t = IntToByteArray(values[i].Y), 0, t.Length);
                        result.Write(t = DocWriter.GetISOBytes(")"), 0, t.Length);
                        if (i < values.Length - 1) {
                            result.Write(RtfElement.COMMA_DELIMITER, 0, RtfElement.COMMA_DELIMITER.Length);
                        }
                    }
                }
                break;
            case PROPERTY_TYPE_IMAGE:
                Image image = (Image)this.value;
                RtfImage img = new RtfImage(this.doc, image);
                img.SetTopLevelElement(true);
                result.Write(RtfElement.OPEN_GROUP, 0, RtfElement.OPEN_GROUP.Length);
                img.WriteContent(result);
                result.Write(RtfElement.CLOSE_GROUP, 0, RtfElement.CLOSE_GROUP.Length);
                break;
            }
            result.Write(RtfElement.CLOSE_GROUP, 0, RtfElement.CLOSE_GROUP.Length);
            result.Write(RtfElement.CLOSE_GROUP, 0, RtfElement.CLOSE_GROUP.Length);
        }
    }
}