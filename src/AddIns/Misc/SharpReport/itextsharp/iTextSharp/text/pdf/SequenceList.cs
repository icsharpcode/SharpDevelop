using System;
using System.Collections;
using System.Text;
using System.util;
/*
 * Copyright 2004 by Paulo Soares.
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
    * This class expands a string into a list of numbers. The main use is to select a
    * range of pages.
    * <p>
    * The general systax is:<br>
    * [!][o][odd][e][even]start-end
    * <p>
    * You can have multiple ranges separated by commas ','. The '!' modifier removes the
    * range from what is already selected. The range changes are incremental, that is,
    * numbers are added or deleted as the range appears. The start or the end, but not both, can be ommited.
    */
    public class SequenceList {
        protected const int COMMA = 1;
        protected const int MINUS = 2;
        protected const int NOT = 3;
        protected const int TEXT = 4;
        protected const int NUMBER = 5;
        protected const int END = 6;
        protected const char EOT = '\uffff';

        private const int FIRST = 0;
        private const int DIGIT = 1;
        private const int OTHER = 2;
        private const int DIGIT2 = 3;
        private const String NOT_OTHER = "-,!0123456789";

        protected char[] text;
        protected int ptr;
        protected int number;
        protected String other;

        protected int low;
        protected int high;
        protected bool odd;
        protected bool even;
        protected bool inverse;

        protected SequenceList(String range) {
            ptr = 0;
            text = range.ToCharArray();
        }
        
        protected char NextChar() {
            while (true) {
                if (ptr >= text.Length)
                    return EOT;
                char c = text[ptr++];
                if (c > ' ')
                    return c;
            }
        }
        
        protected void PutBack() {
            --ptr;
            if (ptr < 0)
                ptr = 0;
        }
        
        protected int Type {
            get {
                StringBuilder buf = new StringBuilder();
                int state = FIRST;
                while (true) {
                    char c = NextChar();
                    if (c == EOT) {
                        if (state == DIGIT) {
                            number = int.Parse(other = buf.ToString());
                            return NUMBER;
                        }
                        else if (state == OTHER) {
                            other = buf.ToString().ToLower(System.Globalization.CultureInfo.InvariantCulture);
                            return TEXT;
                        }
                        return END;
                    }
                    switch (state) {
                        case FIRST:
                            switch (c) {
                                case '!':
                                    return NOT;
                                case '-':
                                    return MINUS;
                                case ',':
                                    return COMMA;
                            }
                            buf.Append(c);
                            if (c >= '0' && c <= '9')
                                state = DIGIT;
                            else
                                state = OTHER;
                            break;
                        case DIGIT:
                            if (c >= '0' && c <= '9')
                                buf.Append(c);
                            else {
                                PutBack();
                                number = int.Parse(other = buf.ToString());
                                return NUMBER;
                            }
                            break;
                        case OTHER:
                            if (NOT_OTHER.IndexOf(c) < 0)
                                buf.Append(c);
                            else {
                                PutBack();
                                other = buf.ToString().ToLower(System.Globalization.CultureInfo.InvariantCulture);
                                return TEXT;
                            }
                            break;
                    }
                }
            }
        }
        
        private void OtherProc() {
            if (other.Equals("odd") || other.Equals("o")) {
                odd = true;
                even = false;
            }
            else if (other.Equals("even") || other.Equals("e")) {
                odd = false;
                even = true;
            }
        }
        
        protected bool GetAttributes() {
            low = -1;
            high = -1;
            odd = even = inverse = false;
            int state = OTHER;
            while (true) {
                int type = Type;
                if (type == END || type == COMMA) {
                    if (state == DIGIT)
                        high = low;
                    return (type == END);
                }
                switch (state) {
                    case OTHER:
                        switch (type) {
                            case NOT:
                                inverse = true;
                                break;
                            case MINUS:
                                state = DIGIT2;
                                break;
                            default:
                                if (type == NUMBER) {
                                    low = number;
                                    state = DIGIT;
                                }
                                else
                                    OtherProc();
                                break;
                        }
                        break;
                    case DIGIT:
                        switch (type) {
                            case NOT:
                                inverse = true;
                                state = OTHER;
                                high = low;
                                break;
                            case MINUS:
                                state = DIGIT2;
                                break;
                            default:
                                high = low;
                                state = OTHER;
                                OtherProc();
                                break;
                        }
                        break;
                    case DIGIT2:
                        switch (type) {
                            case NOT:
                                inverse = true;
                                state = OTHER;
                                break;
                            case MINUS:
                                break;
                            case NUMBER:
                                high = number;
                                state = OTHER;
                                break;
                            default:
                                state = OTHER;
                                OtherProc();
                                break;
                        }
                        break;
                }
            }
        }
        
        /**
        * Generates a list of numbers from a string.
        * @param ranges the comma separated ranges
        * @param maxNumber the maximum number in the range
        * @return a list with the numbers as <CODE>Integer</CODE>
        */    
        public static ArrayList Expand(String ranges, int maxNumber) {
            SequenceList parse = new SequenceList(ranges);
            ArrayList list = new ArrayList();
            bool sair = false;
            while (!sair) {
                sair = parse.GetAttributes();
                if (parse.low == -1 && parse.high == -1 && !parse.even && !parse.odd)
                    continue;
                if (parse.low < 1)
                    parse.low = 1;
                if (parse.high < 1 || parse.high > maxNumber)
                    parse.high = maxNumber;
                if (parse.low > maxNumber)
                    parse.low = maxNumber;
                
                //System.out.Println("low="+parse.low+",high="+parse.high+",odd="+parse.odd+",even="+parse.even+",inverse="+parse.inverse);
                int inc = 1;
                if (parse.inverse) {
                    if (parse.low > parse.high) {
                        int t = parse.low;
                        parse.low = parse.high;
                        parse.high = t;
                    }
                    for (ListIterator it = new ListIterator(list); it.HasNext();) {
                        int n = (int)it.Next();
                        if (parse.even && (n & 1) == 1)
                            continue;
                        if (parse.odd && (n & 1) == 0)
                            continue;
                        if (n >= parse.low && n <= parse.high)
                            it.Remove();
                    }
                }
                else {
                    if (parse.low > parse.high) {
                        inc = -1;
                        if (parse.odd || parse.even) {
                            --inc;
                            if (parse.even)
                                parse.low &= ~1;
                            else
                                parse.low -= ((parse.low & 1) == 1 ? 0 : 1);
                        }
                        for (int k = parse.low; k >= parse.high; k += inc) {
                            list.Add(k);
                        }
                    }
                    else {
                        if (parse.odd || parse.even) {
                            ++inc;
                            if (parse.odd)
                                parse.low |= 1;
                            else
                                parse.low += ((parse.low & 1) == 1 ? 1 : 0);
                        }
                        for (int k = parse.low; k <= parse.high; k += inc)
                            list.Add(k);
                    }
                }
            }
            return list;
        }
    }
}
