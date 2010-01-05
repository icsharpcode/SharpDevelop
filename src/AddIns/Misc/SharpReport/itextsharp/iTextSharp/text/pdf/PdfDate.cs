using System;
using System.Text;
using System.Globalization;

/*
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
     * <CODE>PdfDate</CODE> is the PDF date object.
     * <P>
     * PDF defines a standard date format. The PDF date format closely follows the format
     * defined by the international standard ASN.1 (Abstract Syntax Notation One, defined
     * in CCITT X.208 or ISO/IEC 8824). A date is a <CODE>PdfString</CODE> of the form:
     * <P><BLOCKQUOTE>
     * (D:YYYYMMDDHHmmSSOHH'mm')
     * </BLOCKQUOTE><P>
     * This object is described in the 'Portable Document Format Reference Manual version 1.3'
     * section 7.2 (page 183-184)
     *
     * @see     PdfString
     * @see     java.util.GregorianCalendar
     */

    public class PdfDate : PdfString {
    
        // constructors
    
        /**
         * Constructs a <CODE>PdfDate</CODE>-object.
         *
         * @param       d           the date that has to be turned into a <CODE>PdfDate</CODE>-object
         */
    
        public PdfDate(DateTime d) : base() {
            //d = d.ToUniversalTime();
            
            value = d.ToString("\\D\\:yyyyMMddHHmmss", DateTimeFormatInfo.InvariantInfo);
            string timezone = d.ToString("zzz", DateTimeFormatInfo.InvariantInfo);
            timezone = timezone.Replace(":", "'");
            value += timezone + "'";
        }
    
        /**
         * Constructs a <CODE>PdfDate</CODE>-object, representing the current day and time.
         */
    
        public PdfDate() : this(DateTime.Now) {}
    
        /**
         * Adds a number of leading zeros to a given <CODE>string</CODE> in order to get a <CODE>string</CODE>
         * of a certain length.
         *
         * @param       i           a given number
         * @param       length      the length of the resulting <CODE>string</CODE>
         * @return      the resulting <CODE>string</CODE>
         */
    
        private static String SetLength(int i, int length) {
            return i.ToString().PadLeft(length, '0');
        }

        /**
        * Gives the W3C format of the PdfDate.
        * @return a formatted date
        */
        public String GetW3CDate() {
            return GetW3CDate(value);
        }
        
        /**
        * Gives the W3C format of the PdfDate.
        * @param d the date in the format D:YYYYMMDDHHmmSSOHH'mm'
        * @return a formatted date
        */
        public static String GetW3CDate(String d) {
            if (d.StartsWith("D:"))
                d = d.Substring(2);
            StringBuilder sb = new StringBuilder();
            if (d.Length < 4)
                return "0000";
            sb.Append(d.Substring(0, 4)); //year
            d = d.Substring(4);
            if (d.Length < 2)
                return sb.ToString();
            sb.Append('-').Append(d.Substring(0, 2)); //month
            d = d.Substring(2);
            if (d.Length < 2)
                return sb.ToString();
            sb.Append('-').Append(d.Substring(0, 2)); //day
            d = d.Substring(2);
            if (d.Length < 2)
                return sb.ToString();
            sb.Append('T').Append(d.Substring(0, 2)); //hour
            d = d.Substring(2);
            if (d.Length < 2) {
                sb.Append(":00Z");
                return sb.ToString();
            }
            sb.Append(':').Append(d.Substring(0, 2)); //minute
            d = d.Substring(2);
            if (d.Length < 2) {
                sb.Append('Z');
                return sb.ToString();
            }
            sb.Append(':').Append(d.Substring(0, 2)); //second
            d = d.Substring(2);
            if (d.StartsWith("-") || d.StartsWith("+")) {
                String sign = d.Substring(0, 1);
                d = d.Substring(1);
                String h = "00";
                String m = "00";
                if (d.Length >= 2) {
                    h = d.Substring(0, 2);
                    if (d.Length > 2) {
                        d = d.Substring(3);
                        if (d.Length >= 2)
                            m = d.Substring(0, 2);
                    }
                    sb.Append(sign).Append(h).Append(':').Append(m);
                    return sb.ToString();
                }
            }
            sb.Append('Z');
            return sb.ToString();
        }

        public static DateTime Decode(string date) {
            if (date.StartsWith("D:"))
                date = date.Substring(2);
            int year, month = 1, day = 1, hour = 0, minute = 0, second = 0;
            int offsetHour = 0, offsetMinute = 0;
            char variation = '\0';
            year = int.Parse(date.Substring(0, 4));
            if (date.Length >= 6) {
                month = int.Parse(date.Substring(4, 2));
                if (date.Length >= 8) {
                    day = int.Parse(date.Substring(6, 2));
                    if (date.Length >= 10) {
                        hour = int.Parse(date.Substring(8, 2));
                        if (date.Length >= 12) {
                            minute = int.Parse(date.Substring(10, 2));
                            if (date.Length >= 14) {
                                second = int.Parse(date.Substring(12, 2));
                            }
                        }
                    }
                }
            }
            DateTime d = new DateTime(year, month, day, hour, minute, second);
            if (date.Length <= 14)
                return d;
            variation = date[14];
            if (variation == 'Z')
                return d.ToLocalTime();
            if (date.Length >= 17) {
                offsetHour = int.Parse(date.Substring(15, 2));
                if (date.Length >= 20) {
                    offsetMinute = int.Parse(date.Substring(18, 2));
                }
            }
            TimeSpan span = new TimeSpan(offsetHour, offsetMinute, 0);
            if (variation == '-')
                d += span;
            else
                d -= span;
            return d.ToLocalTime();
        }
    }
}