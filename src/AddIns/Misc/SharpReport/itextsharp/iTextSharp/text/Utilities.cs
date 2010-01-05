using System;
using System.util;
using System.Collections;
using System.Text;
using System.IO;
using iTextSharp.text.pdf;
/*
 * $Id: Utilities.cs,v 1.9 2008/05/13 11:25:13 psoares33 Exp $
 * 
 *
 * Copyright 1999, 2000, 2001, 2002 by Bruno Lowagie.
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
namespace iTextSharp.text {

    /**
    * A collection of convenience methods that were present in many different iText
    * classes.
    */

    public class Utilities {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        public static ICollection GetKeySet(Properties table) {
            return (table == null) ? new Properties().Keys : table.Keys;
        }

        /**
        * Utility method to extend an array.
        * @param original the original array or <CODE>null</CODE>
        * @param item the item to be added to the array
        * @return a new array with the item appended
        */    
        public static Object[][] AddToArray(Object[][] original, Object[] item) {
            if (original == null) {
                original = new Object[1][];
                original[0] = item;
                return original;
            }
            else {
                Object[][] original2 = new Object[original.Length + 1][];
                Array.Copy(original, 0, original2, 0, original.Length);
                original2[original.Length] = item;
                return original2;
            }
        }

	    /**
	    * Checks for a true/false value of a key in a Properties object.
	    * @param attributes
	    * @param key
	    * @return
	    */
	    public static bool CheckTrueOrFalse(Properties attributes, String key) {
		    return Util.EqualsIgnoreCase("true", attributes[key]);
	    }

        /// <summary>
        /// This method makes a valid URL from a given filename.
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <param name="filename">a given filename</param>
        /// <returns>a valid URL</returns>
        public static Uri ToURL(string filename) {
            try {
                return new Uri(filename);
            }
            catch {
                return new Uri("file:///" + filename);
            }
        }
    
        /**
        * Unescapes an URL. All the "%xx" are replaced by the 'xx' hex char value.
        * @param src the url to unescape
        * @return the eunescaped value
        */    
        public static String UnEscapeURL(String src) {
            StringBuilder bf = new StringBuilder();
            char[] s = src.ToCharArray();
            for (int k = 0; k < s.Length; ++k) {
                char c = s[k];
                if (c == '%') {
                    if (k + 2 >= s.Length) {
                        bf.Append(c);
                        continue;
                    }
                    int a0 = PRTokeniser.GetHex((int)s[k + 1]);
                    int a1 = PRTokeniser.GetHex((int)s[k + 2]);
                    if (a0 < 0 || a1 < 0) {
                        bf.Append(c);
                        continue;
                    }
                    bf.Append((char)(a0 * 16 + a1));
                    k += 2;
                }
                else
                    bf.Append(c);
            }
            return bf.ToString();
        }
        
        private static byte[] skipBuffer = new byte[4096];

        /// <summary>
        /// This method is an alternative for the Stream.Skip()-method
        /// that doesn't seem to work properly for big values of size.
        /// </summary>
        /// <param name="istr">the stream</param>
        /// <param name="size">the number of bytes to skip</param>
        public static void Skip(Stream istr, int size) {
            while (size > 0) {
                int r = istr.Read(skipBuffer, 0, Math.Min(skipBuffer.Length, size));
                if (r <= 0)
                    return;
                size -= r;
            }
        }

        /**
        * Measurement conversion from millimeters to points.
        * @param    value   a value in millimeters
        * @return   a value in points
        * @since    2.1.2
        */
        public static float MillimetersToPoints(float value) {
            return InchesToPoints(MillimetersToInches(value));
        }

        /**
        * Measurement conversion from millimeters to inches.
        * @param    value   a value in millimeters
        * @return   a value in inches
        * @since    2.1.2
        */
        public static float MillimetersToInches(float value) {
            return value / 25.4f;
        }

        /**
        * Measurement conversion from points to millimeters.
        * @param    value   a value in points
        * @return   a value in millimeters
        * @since    2.1.2
        */
        public static float PointsToMillimeters(float value) {
            return InchesToMillimeters(PointsToInches(value));
        }

        /**
        * Measurement conversion from points to inches.
        * @param    value   a value in points
        * @return   a value in inches
        * @since    2.1.2
        */
        public static float PointsToInches(float value) {
            return value / 72f;
        }

        /**
        * Measurement conversion from inches to millimeters.
        * @param    value   a value in inches
        * @return   a value in millimeters
        * @since    2.1.2
        */
        public static float InchesToMillimeters(float value) {
            return value * 25.4f;
        }

        /**
        * Measurement conversion from inches to points.
        * @param    value   a value in inches
        * @return   a value in points
        * @since    2.1.2
        */
        public static float InchesToPoints(float value) {
            return value * 72f;
        }

        public static bool IsSurrogateHigh(char c) {
            return c >= '\ud800' && c <= '\udbff';
        }

        public static bool IsSurrogateLow(char c) {
            return c >= '\udc00' && c <= '\udfff';
        }

        public static bool IsSurrogatePair(string text, int idx) {
            if (idx < 0 || idx > text.Length - 2)
                return false;
            return IsSurrogateHigh(text[idx]) && IsSurrogateLow(text[idx + 1]);
        }

        public static bool IsSurrogatePair(char[] text, int idx) {
            if (idx < 0 || idx > text.Length - 2)
                return false;
            return IsSurrogateHigh(text[idx]) && IsSurrogateLow(text[idx + 1]);
        }

        public static int ConvertToUtf32(char highSurrogate, char lowSurrogate) {
             return (((highSurrogate - 0xd800) * 0x400) + (lowSurrogate - 0xdc00)) + 0x10000;
        }

        public static int ConvertToUtf32(char[] text, int idx) {
             return (((text[idx] - 0xd800) * 0x400) + (text[idx + 1] - 0xdc00)) + 0x10000;
        }

        public static int ConvertToUtf32(string text, int idx) {
             return (((text[idx] - 0xd800) * 0x400) + (text[idx + 1] - 0xdc00)) + 0x10000;
        }

        public static string ConvertFromUtf32(int codePoint) {
            if (codePoint < 0x10000)
                return Char.ToString((char)codePoint);
            codePoint -= 0x10000;
            return new string(new char[]{(char)((codePoint / 0x400) + 0xd800), (char)((codePoint % 0x400) + 0xdc00)});
        }
    }
}