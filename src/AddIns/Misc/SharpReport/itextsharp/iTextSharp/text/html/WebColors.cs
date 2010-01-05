using System;
using System.Collections;
using System.Globalization;
using iTextSharp.text;
using System.util;
/*
 * $Id: WebColors.cs,v 1.3 2008/05/13 11:25:16 psoares33 Exp $
 * 
 *
 * Copyright 2001, 2002 by Bruno Lowagie.
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

namespace iTextSharp.text.html {

    /**
    * This class is a HashMap that contains the names of colors as a key and the
    * corresponding Color as value. (Source: Wikipedia
    * http://en.wikipedia.org/wiki/Web_colors )
    * 
    * @author blowagie
    */
    public class WebColors : Hashtable {

        public static WebColors NAMES = new WebColors();
        static WebColors() {
            NAMES["aliceblue"] = new int[] { 0xf0, 0xf8, 0xff, 0x00 };
            NAMES["antiquewhite"] = new int[] { 0xfa, 0xeb, 0xd7, 0x00 };
            NAMES["aqua"] = new int[] { 0x00, 0xff, 0xff, 0x00 };
            NAMES["aquamarine"] = new int[] { 0x7f, 0xff, 0xd4, 0x00 };
            NAMES["azure"] = new int[] { 0xf0, 0xff, 0xff, 0x00 };
            NAMES["beige"] = new int[] { 0xf5, 0xf5, 0xdc, 0x00 };
            NAMES["bisque"] = new int[] { 0xff, 0xe4, 0xc4, 0x00 };
            NAMES["black"] = new int[] { 0x00, 0x00, 0x00, 0x00 };
            NAMES["blanchedalmond"] = new int[] { 0xff, 0xeb, 0xcd, 0x00 };
            NAMES["blue"] = new int[] { 0x00, 0x00, 0xff, 0x00 };
            NAMES["blueviolet"] = new int[] { 0x8a, 0x2b, 0xe2, 0x00 };
            NAMES["brown"] = new int[] { 0xa5, 0x2a, 0x2a, 0x00 };
            NAMES["burlywood"] = new int[] { 0xde, 0xb8, 0x87, 0x00 };
            NAMES["cadetblue"] = new int[] { 0x5f, 0x9e, 0xa0, 0x00 };
            NAMES["chartreuse"] = new int[] { 0x7f, 0xff, 0x00, 0x00 };
            NAMES["chocolate"] = new int[] { 0xd2, 0x69, 0x1e, 0x00 };
            NAMES["coral"] = new int[] { 0xff, 0x7f, 0x50, 0x00 };
            NAMES["cornflowerblue"] = new int[] { 0x64, 0x95, 0xed, 0x00 };
            NAMES["cornsilk"] = new int[] { 0xff, 0xf8, 0xdc, 0x00 };
            NAMES["crimson"] = new int[] { 0xdc, 0x14, 0x3c, 0x00 };
            NAMES["cyan"] = new int[] { 0x00, 0xff, 0xff, 0x00 };
            NAMES["darkblue"] = new int[] { 0x00, 0x00, 0x8b, 0x00 };
            NAMES["darkcyan"] = new int[] { 0x00, 0x8b, 0x8b, 0x00 };
            NAMES["darkgoldenrod"] = new int[] { 0xb8, 0x86, 0x0b, 0x00 };
            NAMES["darkgray"] = new int[] { 0xa9, 0xa9, 0xa9, 0x00 };
            NAMES["darkgreen"] = new int[] { 0x00, 0x64, 0x00, 0x00 };
            NAMES["darkkhaki"] = new int[] { 0xbd, 0xb7, 0x6b, 0x00 };
            NAMES["darkmagenta"] = new int[] { 0x8b, 0x00, 0x8b, 0x00 };
            NAMES["darkolivegreen"] = new int[] { 0x55, 0x6b, 0x2f, 0x00 };
            NAMES["darkorange"] = new int[] { 0xff, 0x8c, 0x00, 0x00 };
            NAMES["darkorchid"] = new int[] { 0x99, 0x32, 0xcc, 0x00 };
            NAMES["darkred"] = new int[] { 0x8b, 0x00, 0x00, 0x00 };
            NAMES["darksalmon"] = new int[] { 0xe9, 0x96, 0x7a, 0x00 };
            NAMES["darkseagreen"] = new int[] { 0x8f, 0xbc, 0x8f, 0x00 };
            NAMES["darkslateblue"] = new int[] { 0x48, 0x3d, 0x8b, 0x00 };
            NAMES["darkslategray"] = new int[] { 0x2f, 0x4f, 0x4f, 0x00 };
            NAMES["darkturquoise"] = new int[] { 0x00, 0xce, 0xd1, 0x00 };
            NAMES["darkviolet"] = new int[] { 0x94, 0x00, 0xd3, 0x00 };
            NAMES["deeppink"] = new int[] { 0xff, 0x14, 0x93, 0x00 };
            NAMES["deepskyblue"] = new int[] { 0x00, 0xbf, 0xff, 0x00 };
            NAMES["dimgray"] = new int[] { 0x69, 0x69, 0x69, 0x00 };
            NAMES["dodgerblue"] = new int[] { 0x1e, 0x90, 0xff, 0x00 };
            NAMES["firebrick"] = new int[] { 0xb2, 0x22, 0x22, 0x00 };
            NAMES["floralwhite"] = new int[] { 0xff, 0xfa, 0xf0, 0x00 };
            NAMES["forestgreen"] = new int[] { 0x22, 0x8b, 0x22, 0x00 };
            NAMES["fuchsia"] = new int[] { 0xff, 0x00, 0xff, 0x00 };
            NAMES["gainsboro"] = new int[] { 0xdc, 0xdc, 0xdc, 0x00 };
            NAMES["ghostwhite"] = new int[] { 0xf8, 0xf8, 0xff, 0x00 };
            NAMES["gold"] = new int[] { 0xff, 0xd7, 0x00, 0x00 };
            NAMES["goldenrod"] = new int[] { 0xda, 0xa5, 0x20, 0x00 };
            NAMES["gray"] = new int[] { 0x80, 0x80, 0x80, 0x00 };
            NAMES["green"] = new int[] { 0x00, 0x80, 0x00, 0x00 };
            NAMES["greenyellow"] = new int[] { 0xad, 0xff, 0x2f, 0x00 };
            NAMES["honeydew"] = new int[] { 0xf0, 0xff, 0xf0, 0x00 };
            NAMES["hotpink"] = new int[] { 0xff, 0x69, 0xb4, 0x00 };
            NAMES["indianred"] = new int[] { 0xcd, 0x5c, 0x5c, 0x00 };
            NAMES["indigo"] = new int[] { 0x4b, 0x00, 0x82, 0x00 };
            NAMES["ivory"] = new int[] { 0xff, 0xff, 0xf0, 0x00 };
            NAMES["khaki"] = new int[] { 0xf0, 0xe6, 0x8c, 0x00 };
            NAMES["lavender"] = new int[] { 0xe6, 0xe6, 0xfa, 0x00 };
            NAMES["lavenderblush"] = new int[] { 0xff, 0xf0, 0xf5, 0x00 };
            NAMES["lawngreen"] = new int[] { 0x7c, 0xfc, 0x00, 0x00 };
            NAMES["lemonchiffon"] = new int[] { 0xff, 0xfa, 0xcd, 0x00 };
            NAMES["lightblue"] = new int[] { 0xad, 0xd8, 0xe6, 0x00 };
            NAMES["lightcoral"] = new int[] { 0xf0, 0x80, 0x80, 0x00 };
            NAMES["lightcyan"] = new int[] { 0xe0, 0xff, 0xff, 0x00 };
            NAMES["lightgoldenrodyellow"] = new int[] { 0xfa, 0xfa, 0xd2, 0x00 };
            NAMES["lightgreen"] = new int[] { 0x90, 0xee, 0x90, 0x00 };
            NAMES["lightgrey"] = new int[] { 0xd3, 0xd3, 0xd3, 0x00 };
            NAMES["lightpink"] = new int[] { 0xff, 0xb6, 0xc1, 0x00 };
            NAMES["lightsalmon"] = new int[] { 0xff, 0xa0, 0x7a, 0x00 };
            NAMES["lightseagreen"] = new int[] { 0x20, 0xb2, 0xaa, 0x00 };
            NAMES["lightskyblue"] = new int[] { 0x87, 0xce, 0xfa, 0x00 };
            NAMES["lightslategray"] = new int[] { 0x77, 0x88, 0x99, 0x00 };
            NAMES["lightsteelblue"] = new int[] { 0xb0, 0xc4, 0xde, 0x00 };
            NAMES["lightyellow"] = new int[] { 0xff, 0xff, 0xe0, 0x00 };
            NAMES["lime"] = new int[] { 0x00, 0xff, 0x00, 0x00 };
            NAMES["limegreen"] = new int[] { 0x32, 0xcd, 0x32, 0x00 };
            NAMES["linen"] = new int[] { 0xfa, 0xf0, 0xe6, 0x00 };
            NAMES["magenta"] = new int[] { 0xff, 0x00, 0xff, 0x00 };
            NAMES["maroon"] = new int[] { 0x80, 0x00, 0x00, 0x00 };
            NAMES["mediumaquamarine"] = new int[] { 0x66, 0xcd, 0xaa, 0x00 };
            NAMES["mediumblue"] = new int[] { 0x00, 0x00, 0xcd, 0x00 };
            NAMES["mediumorchid"] = new int[] { 0xba, 0x55, 0xd3, 0x00 };
            NAMES["mediumpurple"] = new int[] { 0x93, 0x70, 0xdb, 0x00 };
            NAMES["mediumseagreen"] = new int[] { 0x3c, 0xb3, 0x71, 0x00 };
            NAMES["mediumslateblue"] = new int[] { 0x7b, 0x68, 0xee, 0x00 };
            NAMES["mediumspringgreen"] = new int[] { 0x00, 0xfa, 0x9a, 0x00 };
            NAMES["mediumturquoise"] = new int[] { 0x48, 0xd1, 0xcc, 0x00 };
            NAMES["mediumvioletred"] = new int[] { 0xc7, 0x15, 0x85, 0x00 };
            NAMES["midnightblue"] = new int[] { 0x19, 0x19, 0x70, 0x00 };
            NAMES["mintcream"] = new int[] { 0xf5, 0xff, 0xfa, 0x00 };
            NAMES["mistyrose"] = new int[] { 0xff, 0xe4, 0xe1, 0x00 };
            NAMES["moccasin"] = new int[] { 0xff, 0xe4, 0xb5, 0x00 };
            NAMES["navajowhite"] = new int[] { 0xff, 0xde, 0xad, 0x00 };
            NAMES["navy"] = new int[] { 0x00, 0x00, 0x80, 0x00 };
            NAMES["oldlace"] = new int[] { 0xfd, 0xf5, 0xe6, 0x00 };
            NAMES["olive"] = new int[] { 0x80, 0x80, 0x00, 0x00 };
            NAMES["olivedrab"] = new int[] { 0x6b, 0x8e, 0x23, 0x00 };
            NAMES["orange"] = new int[] { 0xff, 0xa5, 0x00, 0x00 };
            NAMES["orangered"] = new int[] { 0xff, 0x45, 0x00, 0x00 };
            NAMES["orchid"] = new int[] { 0xda, 0x70, 0xd6, 0x00 };
            NAMES["palegoldenrod"] = new int[] { 0xee, 0xe8, 0xaa, 0x00 };
            NAMES["palegreen"] = new int[] { 0x98, 0xfb, 0x98, 0x00 };
            NAMES["paleturquoise"] = new int[] { 0xaf, 0xee, 0xee, 0x00 };
            NAMES["palevioletred"] = new int[] { 0xdb, 0x70, 0x93, 0x00 };
            NAMES["papayawhip"] = new int[] { 0xff, 0xef, 0xd5, 0x00 };
            NAMES["peachpuff"] = new int[] { 0xff, 0xda, 0xb9, 0x00 };
            NAMES["peru"] = new int[] { 0xcd, 0x85, 0x3f, 0x00 };
            NAMES["pink"] = new int[] { 0xff, 0xc0, 0xcb, 0x00 };
            NAMES["plum"] = new int[] { 0xdd, 0xa0, 0xdd, 0x00 };
            NAMES["powderblue"] = new int[] { 0xb0, 0xe0, 0xe6, 0x00 };
            NAMES["purple"] = new int[] { 0x80, 0x00, 0x80, 0x00 };
            NAMES["red"] = new int[] { 0xff, 0x00, 0x00, 0x00 };
            NAMES["rosybrown"] = new int[] { 0xbc, 0x8f, 0x8f, 0x00 };
            NAMES["royalblue"] = new int[] { 0x41, 0x69, 0xe1, 0x00 };
            NAMES["saddlebrown"] = new int[] { 0x8b, 0x45, 0x13, 0x00 };
            NAMES["salmon"] = new int[] { 0xfa, 0x80, 0x72, 0x00 };
            NAMES["sandybrown"] = new int[] { 0xf4, 0xa4, 0x60, 0x00 };
            NAMES["seagreen"] = new int[] { 0x2e, 0x8b, 0x57, 0x00 };
            NAMES["seashell"] = new int[] { 0xff, 0xf5, 0xee, 0x00 };
            NAMES["sienna"] = new int[] { 0xa0, 0x52, 0x2d, 0x00 };
            NAMES["silver"] = new int[] { 0xc0, 0xc0, 0xc0, 0x00 };
            NAMES["skyblue"] = new int[] { 0x87, 0xce, 0xeb, 0x00 };
            NAMES["slateblue"] = new int[] { 0x6a, 0x5a, 0xcd, 0x00 };
            NAMES["slategray"] = new int[] { 0x70, 0x80, 0x90, 0x00 };
            NAMES["snow"] = new int[] { 0xff, 0xfa, 0xfa, 0x00 };
            NAMES["springgreen"] = new int[] { 0x00, 0xff, 0x7f, 0x00 };
            NAMES["steelblue"] = new int[] { 0x46, 0x82, 0xb4, 0x00 };
            NAMES["tan"] = new int[] { 0xd2, 0xb4, 0x8c, 0x00 };
            NAMES["transparent"] = new int[] { 0x00, 0x00, 0x00, 0xff };
            NAMES["teal"] = new int[] { 0x00, 0x80, 0x80, 0x00 };
            NAMES["thistle"] = new int[] { 0xd8, 0xbf, 0xd8, 0x00 };
            NAMES["tomato"] = new int[] { 0xff, 0x63, 0x47, 0x00 };
            NAMES["turquoise"] = new int[] { 0x40, 0xe0, 0xd0, 0x00 };
            NAMES["violet"] = new int[] { 0xee, 0x82, 0xee, 0x00 };
            NAMES["wheat"] = new int[] { 0xf5, 0xde, 0xb3, 0x00 };
            NAMES["white"] = new int[] { 0xff, 0xff, 0xff, 0x00 };
            NAMES["whitesmoke"] = new int[] { 0xf5, 0xf5, 0xf5, 0x00 };
            NAMES["yellow"] = new int[] { 0xff, 0xff, 0x00, 0x00 };
            NAMES["yellowgreen"] = new int[] { 0x9, 0xacd, 0x32, 0x00 };
        }
        
        /**
        * Gives you a Color based on a name.
        * 
        * @param name
        *            a name such as black, violet, cornflowerblue or #RGB or #RRGGBB
        *            or rgb(R,G,B)
        * @return the corresponding Color object
        * @throws IllegalArgumentException
        *             if the String isn't a know representation of a color.
        */
        public static Color GetRGBColor(String name) {
            int[] c = { 0, 0, 0, 0 };
            if (name.StartsWith("#")) {
                if (name.Length == 4) {
                    c[0] = int.Parse(name.Substring(1, 1), NumberStyles.HexNumber) * 16;
                    c[1] = int.Parse(name.Substring(2, 1), NumberStyles.HexNumber) * 16;
                    c[2] = int.Parse(name.Substring(3), NumberStyles.HexNumber) * 16;
                    return new Color(c[0], c[1], c[2], c[3]);
                }
                if (name.Length == 7) {
                    c[0] = int.Parse(name.Substring(1, 2), NumberStyles.HexNumber);
                    c[1] = int.Parse(name.Substring(3, 2), NumberStyles.HexNumber);
                    c[2] = int.Parse(name.Substring(5), NumberStyles.HexNumber);
                    return new Color(c[0], c[1], c[2], c[3]);
                }
                throw new ArgumentException(
                        "Unknown color format. Must be #RGB or #RRGGBB");
            }
            else if (name.StartsWith("rgb(")) {
                StringTokenizer tok = new StringTokenizer(name, "rgb(), \t\r\n\f");
                for (int k = 0; k < 3; ++k) {
                    String v = tok.NextToken();
                    if (v.EndsWith("%"))
                        c[k] = int.Parse(v.Substring(0, v.Length - 1)) * 255 / 100;
                    else
                        c[k] = int.Parse(v);
                    if (c[k] < 0)
                        c[k] = 0;
                    else if (c[k] > 255)
                        c[k] = 255;
                }
                return new Color(c[0], c[1], c[2], c[3]);
            }
            name = name.ToLower(CultureInfo.InvariantCulture);
            if (!NAMES.ContainsKey(name))
                throw new ArgumentException("Color '" + name
                        + "' not found.");
            c = (int[]) NAMES[name];
            return new Color(c[0], c[1], c[2], c[3]);
        }
    }
}