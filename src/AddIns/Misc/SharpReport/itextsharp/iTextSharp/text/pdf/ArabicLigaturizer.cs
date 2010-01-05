using System;
using System.Text;
/*
 * Copyright 2003 by Paulo Soares.
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
    * Shape arabic characters. This code was inspired by an LGPL'ed C library:
    * Pango ( see http://www.pango.com/ ). Note that the code of this is the
    * original work of Paulo Soares. Hence it is perfectly justifiable to distribute
    * it under the MPL.
    *
    * @author Paulo Soares (psoares@consiste.pt)
    */
    public class ArabicLigaturizer {
        
        static bool IsVowel(char s) {
            return ((s >= '\u064B') && (s <= '\u0655')) || (s == '\u0670');
        }

        static char Charshape(char s, int which)
        /* which 0=isolated 1=final 2=initial 3=medial */
        {
            int l, r, m;
            if ((s >= '\u0621') && (s <= '\u06D3')) {
                l = 0;
                r = chartable.Length - 1;
                while (l <= r) {
                    m = (l + r) / 2;
                    if (s == chartable[m][0]) {
                        return chartable[m][which + 1];
                    }
                    else if (s < chartable[m][0]) {
                        r = m - 1;
                    }
                    else {
                        l = m + 1;
                    }
                }
            }
            else if (s >= '\ufef5' && s <= '\ufefb')
                return (char)(s + which);
            return s;
        }

        static int Shapecount(char s) {
            int l, r, m;
            if ((s >= '\u0621') && (s <= '\u06D3') && !IsVowel(s)) {
                l = 0;
                r = chartable.Length - 1;
                while (l <= r) {
                    m = (l + r) / 2;
                    if (s == chartable[m][0]) {
                        return chartable[m].Length - 1;
                    }
                    else if (s < chartable[m][0]) {
                        r = m - 1;
                    }
                    else {
                        l = m + 1;
                    }
                }
            }
            else if (s == ZWJ) {
                return 4;
            }
            return 1;
        }
        
        static int Ligature(char newchar, Charstruct oldchar) {
        /* 0 == no ligature possible; 1 == vowel; 2 == two chars; 3 == Lam+Alef */
            int retval = 0;
            
            if (oldchar.basechar == 0)
                return 0;
            if (IsVowel(newchar)) {
                retval = 1;
                if ((oldchar.vowel != 0) && (newchar != SHADDA)) {
                    retval = 2;           /* we eliminate the old vowel .. */
                }
                switch (newchar) {
                    case SHADDA:
                        if (oldchar.mark1 == 0) {
                            oldchar.mark1 = SHADDA;
                        }
                        else {
                            return 0;         /* no ligature possible */
                        }
                        break;
                    case HAMZABELOW:
                        switch (oldchar.basechar) {
                            case ALEF:
                                oldchar.basechar = ALEFHAMZABELOW;
                                retval = 2;
                                break;
                            case LAM_ALEF:
                                oldchar.basechar = LAM_ALEFHAMZABELOW;
                                retval = 2;
                                break;
                            default:
                                oldchar.mark1 = HAMZABELOW;
                                break;
                        }
                        break;
                    case HAMZAABOVE:
                        switch (oldchar.basechar) {
                            case ALEF:
                                oldchar.basechar = ALEFHAMZA;
                                retval = 2;
                                break;
                            case LAM_ALEF:
                                oldchar.basechar = LAM_ALEFHAMZA;
                                retval = 2;
                                break;
                            case WAW:
                                oldchar.basechar = WAWHAMZA;
                                retval = 2;
                                break;
                            case YEH:
                            case ALEFMAKSURA:
                            case FARSIYEH:
                                oldchar.basechar = YEHHAMZA;
                                retval = 2;
                                break;
                            default:           /* whatever sense this may make .. */
                                oldchar.mark1 = HAMZAABOVE;
                                break;
                        }
                        break;
                    case MADDA:
                        switch (oldchar.basechar) {
                            case ALEF:
                                oldchar.basechar = ALEFMADDA;
                                retval = 2;
                                break;
                        }
                        break;
                    default:
                        oldchar.vowel = newchar;
                        break;
                }
                if (retval == 1) {
                    oldchar.lignum++;
                }
                return retval;
            }
            if (oldchar.vowel != 0) {  /* if we already joined a vowel, we can't join a Hamza */
                return 0;
            }
            
            switch (oldchar.basechar) {
                case LAM:
                    switch (newchar) {
                        case ALEF:
                            oldchar.basechar = LAM_ALEF;
                            oldchar.numshapes = 2;
                            retval = 3;
                            break;
                        case ALEFHAMZA:
                            oldchar.basechar = LAM_ALEFHAMZA;
                            oldchar.numshapes = 2;
                            retval = 3;
                            break;
                        case ALEFHAMZABELOW:
                            oldchar.basechar = LAM_ALEFHAMZABELOW;
                            oldchar.numshapes = 2;
                            retval = 3;
                            break;
                        case ALEFMADDA:
                            oldchar.basechar = LAM_ALEFMADDA;
                            oldchar.numshapes = 2;
                            retval = 3;
                            break;
                    }
                    break;
                case (char)0:
                    oldchar.basechar = newchar;
                    oldchar.numshapes = Shapecount(newchar);
                    retval = 1;
                    break;
            }
            return retval;
        }
        
        static void Copycstostring(StringBuilder str, Charstruct s, int level) {
        /* s is a shaped charstruct; i is the index into the string */
            if (s.basechar == 0)
                return;
            
            str.Append(s.basechar);
            s.lignum--;
            if (s.mark1 != 0) {
                if ((level & ar_novowel) == 0) {
                    str.Append(s.mark1);
                    s.lignum--;
                }
                else {
                    s.lignum--;
                }
            }
            if (s.vowel != 0) {
                if ((level & ar_novowel) == 0) {
                    str.Append(s.vowel);
                    s.lignum--;
                }
                else {                       /* vowel elimination */
                    s.lignum--;
                }
            }
        }

        // return len
        internal static void Doublelig(StringBuilder str, int level)
        /* Ok. We have presentation ligatures in our font. */
        {
            int len;
            int olen = len = str.Length;
            int j = 0, si = 1;
            char lapresult;
            
            while (si < olen) {
                lapresult = (char)0;
                if ((level & ar_composedtashkeel) != 0) {
                    switch (str[j]) {
                        case SHADDA:
                            switch (str[si]) {
                                case KASRA:
                                    lapresult = '\uFC62';
                                    break;
                                case FATHA:
                                    lapresult = '\uFC60';
                                    break;
                                case DAMMA:
                                    lapresult = '\uFC61';
                                    break;
                                case '\u064C':
                                    lapresult = '\uFC5E';
                                    break;
                                case '\u064D':
                                    lapresult = '\uFC5F';
                                    break;
                            }
                            break;
                        case KASRA:
                            if (str[si] == SHADDA)
                                lapresult = '\uFC62';
                            break;
                        case FATHA:
                            if (str[si] == SHADDA)
                                lapresult = '\uFC60';
                            break;
                        case DAMMA:
                            if (str[si] == SHADDA)
                                lapresult = '\uFC61';
                            break;
                    }
                }
                
                if ((level & ar_lig) != 0) {
                    switch (str[j]) {
                        case '\uFEDF':       /* LAM initial */
                            switch (str[si]) {
                                case '\uFE9E':
                                    lapresult = '\uFC3F';
                                    break;        /* JEEM final */
                                case '\uFEA0':
                                    lapresult = '\uFCC9';
                                    break;        /* JEEM medial */
                                case '\uFEA2':
                                    lapresult = '\uFC40';
                                    break;        /* HAH final */
                                case '\uFEA4':
                                    lapresult = '\uFCCA';
                                    break;        /* HAH medial */
                                case '\uFEA6':
                                    lapresult = '\uFC41';
                                    break;        /* KHAH final */
                                case '\uFEA8':
                                    lapresult = '\uFCCB';
                                    break;        /* KHAH medial */
                                case '\uFEE2':
                                    lapresult = '\uFC42';
                                    break;        /* MEEM final */
                                case '\uFEE4':
                                    lapresult = '\uFCCC';
                                    break;        /* MEEM medial */
                            }
                            break;
                        case '\uFE97':       /* TEH inital */
                            switch (str[si]) {
                                case '\uFEA0':
                                    lapresult = '\uFCA1';
                                    break;        /* JEEM medial */
                                case '\uFEA4':
                                    lapresult = '\uFCA2';
                                    break;        /* HAH medial */
                                case '\uFEA8':
                                    lapresult = '\uFCA3';
                                    break;        /* KHAH medial */
                            }
                            break;
                        case '\uFE91':       /* BEH inital */
                            switch (str[si]) {
                                case '\uFEA0':
                                    lapresult = '\uFC9C';
                                    break;        /* JEEM medial */
                                case '\uFEA4':
                                    lapresult = '\uFC9D';
                                    break;        /* HAH medial */
                                case '\uFEA8':
                                    lapresult = '\uFC9E';
                                    break;        /* KHAH medial */
                            }
                            break;
                        case '\uFEE7':       /* NOON inital */
                            switch (str[si]) {
                                case '\uFEA0':
                                    lapresult = '\uFCD2';
                                    break;        /* JEEM initial */
                                case '\uFEA4':
                                    lapresult = '\uFCD3';
                                    break;        /* HAH medial */
                                case '\uFEA8':
                                    lapresult = '\uFCD4';
                                    break;        /* KHAH medial */
                            }
                            break;
                            
                        case '\uFEE8':       /* NOON medial */
                            switch (str[si]) {
                                case '\uFEAE':
                                    lapresult = '\uFC8A';
                                    break;        /* REH final  */
                                case '\uFEB0':
                                    lapresult = '\uFC8B';
                                    break;        /* ZAIN final */
                            }
                            break;
                        case '\uFEE3':       /* MEEM initial */
                            switch (str[si]) {
                                case '\uFEA0':
                                    lapresult = '\uFCCE';
                                    break;        /* JEEM medial */
                                case '\uFEA4':
                                    lapresult = '\uFCCF';
                                    break;        /* HAH medial */
                                case '\uFEA8':
                                    lapresult = '\uFCD0';
                                    break;        /* KHAH medial */
                                case '\uFEE4':
                                    lapresult = '\uFCD1';
                                    break;        /* MEEM medial */
                            }
                            break;
                            
                        case '\uFED3':       /* FEH initial */
                            switch (str[si]) {
                                case '\uFEF2':
                                    lapresult = '\uFC32';
                                    break;        /* YEH final */
                            }
                            break;
                            
                        default:
                            break;
                    }                   /* end switch string[si] */
                }
                if (lapresult != 0) {
                    str[j] = lapresult;
                    len--;
                    si++;                 /* jump over one character */
                    /* we'll have to change this, too. */
                }
                else {
                    j++;
                    str[j] = str[si];
                    si++;
                }
            }
            str.Length = len;
        }

        static bool Connects_to_left(Charstruct a) {
            return a.numshapes > 2;
        }
        
        internal static void Shape(char[] text, StringBuilder str, int level) {
    /* string is assumed to be empty and big enough.
    * text is the original text.
    * This routine does the basic arabic reshaping.
    * *len the number of non-null characters.
    *
    * Note: We have to unshape each character first!
    */
            int join;
            int which;
            char nextletter;
            
            int p = 0;                     /* initialize for output */
            Charstruct oldchar = new Charstruct();
            Charstruct curchar = new Charstruct();
            while (p < text.Length) {
                nextletter = text[p++];
                //nextletter = unshape (nextletter);
                
                join = Ligature(nextletter, curchar);
                if (join == 0) {                       /* shape curchar */
                    int nc = Shapecount(nextletter);
                    //(*len)++;
                    if (nc == 1) {
                        which = 0;        /* final or isolated */
                    }
                    else {
                        which = 2;        /* medial or initial */
                    }
                    if (Connects_to_left(oldchar)) {
                        which++;
                    }
                    
                    which = which % (curchar.numshapes);
                    curchar.basechar = Charshape(curchar.basechar, which);
                    
                    /* get rid of oldchar */
                    Copycstostring(str, oldchar, level);
                    oldchar = curchar;    /* new values in oldchar */
                    
                    /* init new curchar */
                    curchar = new Charstruct();
                    curchar.basechar = nextletter;
                    curchar.numshapes = nc;
                    curchar.lignum++;
                    //          (*len) += unligature (&curchar, level);
                }
                else if (join == 1) {
                }
                //      else
                //        {
                //          (*len) += unligature (&curchar, level);
                //        }
                //      p = g_utf8_next_char (p);
            }
            
            /* Handle last char */
            if (Connects_to_left(oldchar))
                which = 1;
            else
                which = 0;
            which = which % (curchar.numshapes);
            curchar.basechar = Charshape(curchar.basechar, which);
            
            /* get rid of oldchar */
            Copycstostring(str, oldchar, level);
            Copycstostring(str, curchar, level);
        }

        internal static int Arabic_shape(char[] src, int srcoffset, int srclength, char[] dest, int destoffset, int destlength, int level) {
            char[] str = new char[srclength];
            for (int k = srclength + srcoffset - 1; k >= srcoffset; --k)
                str[k - srcoffset] = src[k];
            StringBuilder str2 = new StringBuilder(srclength);
            Shape(str, str2, level);
            if ((level & (ar_composedtashkeel | ar_lig)) != 0)
                Doublelig(str2, level);
    //        string.Reverse();
            System.Array.Copy(str2.ToString().ToCharArray(), 0, dest, destoffset, str2.Length);
            return str2.Length;
        }

        internal static void ProcessNumbers(char[] text, int offset, int length, int options) {
            int limit = offset + length;
            if ((options & DIGITS_MASK) != 0) {
                char digitBase = '\u0030'; // European digits
                switch (options & DIGIT_TYPE_MASK) {
                    case DIGIT_TYPE_AN:
                        digitBase = '\u0660';  // Arabic-Indic digits
                        break;
                        
                    case DIGIT_TYPE_AN_EXTENDED:
                        digitBase = '\u06f0';  // Eastern Arabic-Indic digits (Persian and Urdu)
                        break;
                        
                    default:
                        break;
                }
                
                switch (options & DIGITS_MASK) {
                    case DIGITS_EN2AN: {
                        int digitDelta = digitBase - '\u0030';
                        for (int i = offset; i < limit; ++i) {
                            char ch = text[i];
                            if (ch <= '\u0039' && ch >= '\u0030') {
                                text[i] += (char)digitDelta;
                            }
                        }
                    }
                    break;
                    
                    case DIGITS_AN2EN: {
                        char digitTop = (char)(digitBase + 9);
                        int digitDelta = '\u0030' - digitBase;
                        for (int i = offset; i < limit; ++i) {
                            char ch = text[i];
                            if (ch <= digitTop && ch >= digitBase) {
                                text[i] += (char)digitDelta;
                            }
                        }
                    }
                    break;
                    
                    case DIGITS_EN2AN_INIT_LR:
                        ShapeToArabicDigitsWithContext(text, 0, length, digitBase, false);
                        break;
                        
                    case DIGITS_EN2AN_INIT_AL:
                        ShapeToArabicDigitsWithContext(text, 0, length, digitBase, true);
                        break;
                        
                    default:
                        break;
                }
            }
        }
        
        internal static void ShapeToArabicDigitsWithContext(char[] dest, int start, int length, char digitBase,  bool lastStrongWasAL) {
            digitBase -= '0'; // move common adjustment out of loop
     
            int limit = start + length;
            for (int i = start; i < limit; ++i) {
                char ch = dest[i];
                switch (BidiOrder.GetDirection(ch)) {
                case BidiOrder.L:
                case BidiOrder.R:
                    lastStrongWasAL = false;
                    break;
                case BidiOrder.AL:
                    lastStrongWasAL = true;
                    break;
                case BidiOrder.EN:
                    if (lastStrongWasAL && ch <= '\u0039') {
                        dest[i] = (char)(ch + digitBase);
                    }
                    break;
                default:
                    break;
                }
            }
        }

        private const char ALEF = '\u0627';
        private const char ALEFHAMZA = '\u0623';
        private const char ALEFHAMZABELOW = '\u0625';
        private const char ALEFMADDA = '\u0622';
        private const char LAM = '\u0644';
        private const char HAMZA = '\u0621';
        private const char TATWEEL = '\u0640';
        private const char ZWJ = '\u200D';

        private const char HAMZAABOVE = '\u0654';
        private const char HAMZABELOW = '\u0655';

        private const char WAWHAMZA = '\u0624';
        private const char YEHHAMZA = '\u0626';
        private const char WAW = '\u0648';
        private const char ALEFMAKSURA = '\u0649';
        private const char YEH = '\u064A';
        private const char FARSIYEH = '\u06CC';

        private const char SHADDA = '\u0651';
        private const char KASRA = '\u0650';
        private const char FATHA = '\u064E';
        private const char DAMMA = '\u064F';
        private const char MADDA = '\u0653';

        private const char LAM_ALEF = '\uFEFB';
        private const char LAM_ALEFHAMZA = '\uFEF7';
        private const char LAM_ALEFHAMZABELOW = '\uFEF9';
        private const char LAM_ALEFMADDA = '\uFEF5';

        private static char[][] chartable = {
            new char[]{'\u0621', '\uFE80'}, /* HAMZA */
            new char[]{'\u0622', '\uFE81', '\uFE82'}, /* ALEF WITH MADDA ABOVE */
            new char[]{'\u0623', '\uFE83', '\uFE84'}, /* ALEF WITH HAMZA ABOVE */
            new char[]{'\u0624', '\uFE85', '\uFE86'}, /* WAW WITH HAMZA ABOVE */
            new char[]{'\u0625', '\uFE87', '\uFE88'}, /* ALEF WITH HAMZA BELOW */
            new char[]{'\u0626', '\uFE89', '\uFE8A', '\uFE8B', '\uFE8C'}, /* YEH WITH HAMZA ABOVE */
            new char[]{'\u0627', '\uFE8D', '\uFE8E'}, /* ALEF */
            new char[]{'\u0628', '\uFE8F', '\uFE90', '\uFE91', '\uFE92'}, /* BEH */
            new char[]{'\u0629', '\uFE93', '\uFE94'}, /* TEH MARBUTA */
            new char[]{'\u062A', '\uFE95', '\uFE96', '\uFE97', '\uFE98'}, /* TEH */
            new char[]{'\u062B', '\uFE99', '\uFE9A', '\uFE9B', '\uFE9C'}, /* THEH */
            new char[]{'\u062C', '\uFE9D', '\uFE9E', '\uFE9F', '\uFEA0'}, /* JEEM */
            new char[]{'\u062D', '\uFEA1', '\uFEA2', '\uFEA3', '\uFEA4'}, /* HAH */
            new char[]{'\u062E', '\uFEA5', '\uFEA6', '\uFEA7', '\uFEA8'}, /* KHAH */
            new char[]{'\u062F', '\uFEA9', '\uFEAA'}, /* DAL */
            new char[]{'\u0630', '\uFEAB', '\uFEAC'}, /* THAL */
            new char[]{'\u0631', '\uFEAD', '\uFEAE'}, /* REH */
            new char[]{'\u0632', '\uFEAF', '\uFEB0'}, /* ZAIN */
            new char[]{'\u0633', '\uFEB1', '\uFEB2', '\uFEB3', '\uFEB4'}, /* SEEN */
            new char[]{'\u0634', '\uFEB5', '\uFEB6', '\uFEB7', '\uFEB8'}, /* SHEEN */
            new char[]{'\u0635', '\uFEB9', '\uFEBA', '\uFEBB', '\uFEBC'}, /* SAD */
            new char[]{'\u0636', '\uFEBD', '\uFEBE', '\uFEBF', '\uFEC0'}, /* DAD */
            new char[]{'\u0637', '\uFEC1', '\uFEC2', '\uFEC3', '\uFEC4'}, /* TAH */
            new char[]{'\u0638', '\uFEC5', '\uFEC6', '\uFEC7', '\uFEC8'}, /* ZAH */
            new char[]{'\u0639', '\uFEC9', '\uFECA', '\uFECB', '\uFECC'}, /* AIN */
            new char[]{'\u063A', '\uFECD', '\uFECE', '\uFECF', '\uFED0'}, /* GHAIN */
            new char[]{'\u0640', '\u0640', '\u0640', '\u0640', '\u0640'}, /* TATWEEL */
            new char[]{'\u0641', '\uFED1', '\uFED2', '\uFED3', '\uFED4'}, /* FEH */
            new char[]{'\u0642', '\uFED5', '\uFED6', '\uFED7', '\uFED8'}, /* QAF */
            new char[]{'\u0643', '\uFED9', '\uFEDA', '\uFEDB', '\uFEDC'}, /* KAF */
            new char[]{'\u0644', '\uFEDD', '\uFEDE', '\uFEDF', '\uFEE0'}, /* LAM */
            new char[]{'\u0645', '\uFEE1', '\uFEE2', '\uFEE3', '\uFEE4'}, /* MEEM */
            new char[]{'\u0646', '\uFEE5', '\uFEE6', '\uFEE7', '\uFEE8'}, /* NOON */
            new char[]{'\u0647', '\uFEE9', '\uFEEA', '\uFEEB', '\uFEEC'}, /* HEH */
            new char[]{'\u0648', '\uFEED', '\uFEEE'}, /* WAW */
            new char[]{'\u0649', '\uFEEF', '\uFEF0', '\uFBE8', '\uFBE9'}, /* ALEF MAKSURA */
            new char[]{'\u064A', '\uFEF1', '\uFEF2', '\uFEF3', '\uFEF4'}, /* YEH */
            new char[]{'\u0671', '\uFB50', '\uFB51'}, /* ALEF WASLA */
            new char[]{'\u0679', '\uFB66', '\uFB67', '\uFB68', '\uFB69'}, /* TTEH */
            new char[]{'\u067A', '\uFB5E', '\uFB5F', '\uFB60', '\uFB61'}, /* TTEHEH */
            new char[]{'\u067B', '\uFB52', '\uFB53', '\uFB54', '\uFB55'}, /* BEEH */
            new char[]{'\u067E', '\uFB56', '\uFB57', '\uFB58', '\uFB59'}, /* PEH */
            new char[]{'\u067F', '\uFB62', '\uFB63', '\uFB64', '\uFB65'}, /* TEHEH */
            new char[]{'\u0680', '\uFB5A', '\uFB5B', '\uFB5C', '\uFB5D'}, /* BEHEH */
            new char[]{'\u0683', '\uFB76', '\uFB77', '\uFB78', '\uFB79'}, /* NYEH */
            new char[]{'\u0684', '\uFB72', '\uFB73', '\uFB74', '\uFB75'}, /* DYEH */
            new char[]{'\u0686', '\uFB7A', '\uFB7B', '\uFB7C', '\uFB7D'}, /* TCHEH */
            new char[]{'\u0687', '\uFB7E', '\uFB7F', '\uFB80', '\uFB81'}, /* TCHEHEH */
            new char[]{'\u0688', '\uFB88', '\uFB89'}, /* DDAL */
            new char[]{'\u068C', '\uFB84', '\uFB85'}, /* DAHAL */
            new char[]{'\u068D', '\uFB82', '\uFB83'}, /* DDAHAL */
            new char[]{'\u068E', '\uFB86', '\uFB87'}, /* DUL */
            new char[]{'\u0691', '\uFB8C', '\uFB8D'}, /* RREH */
            new char[]{'\u0698', '\uFB8A', '\uFB8B'}, /* JEH */
            new char[]{'\u06A4', '\uFB6A', '\uFB6B', '\uFB6C', '\uFB6D'}, /* VEH */
            new char[]{'\u06A6', '\uFB6E', '\uFB6F', '\uFB70', '\uFB71'}, /* PEHEH */
            new char[]{'\u06A9', '\uFB8E', '\uFB8F', '\uFB90', '\uFB91'}, /* KEHEH */
            new char[]{'\u06AD', '\uFBD3', '\uFBD4', '\uFBD5', '\uFBD6'}, /* NG */
            new char[]{'\u06AF', '\uFB92', '\uFB93', '\uFB94', '\uFB95'}, /* GAF */
            new char[]{'\u06B1', '\uFB9A', '\uFB9B', '\uFB9C', '\uFB9D'}, /* NGOEH */
            new char[]{'\u06B3', '\uFB96', '\uFB97', '\uFB98', '\uFB99'}, /* GUEH */
            new char[]{'\u06BA', '\uFB9E', '\uFB9F'}, /* NOON GHUNNA */
            new char[]{'\u06BB', '\uFBA0', '\uFBA1', '\uFBA2', '\uFBA3'}, /* RNOON */
            new char[]{'\u06BE', '\uFBAA', '\uFBAB', '\uFBAC', '\uFBAD'}, /* HEH DOACHASHMEE */
            new char[]{'\u06C0', '\uFBA4', '\uFBA5'}, /* HEH WITH YEH ABOVE */
            new char[]{'\u06C1', '\uFBA6', '\uFBA7', '\uFBA8', '\uFBA9'}, /* HEH GOAL */
            new char[]{'\u06C5', '\uFBE0', '\uFBE1'}, /* KIRGHIZ OE */
            new char[]{'\u06C6', '\uFBD9', '\uFBDA'}, /* OE */
            new char[]{'\u06C7', '\uFBD7', '\uFBD8'}, /* U */
            new char[]{'\u06C8', '\uFBDB', '\uFBDC'}, /* YU */
            new char[]{'\u06C9', '\uFBE2', '\uFBE3'}, /* KIRGHIZ YU */
            new char[]{'\u06CB', '\uFBDE', '\uFBDF'}, /* VE */
            new char[]{'\u06CC', '\uFBFC', '\uFBFD', '\uFBFE', '\uFBFF'}, /* FARSI YEH */
            new char[]{'\u06D0', '\uFBE4', '\uFBE5', '\uFBE6', '\uFBE7'}, /* E */
            new char[]{'\u06D2', '\uFBAE', '\uFBAF'}, /* YEH BARREE */
            new char[]{'\u06D3', '\uFBB0', '\uFBB1'} /* YEH BARREE WITH HAMZA ABOVE */
            };

            public const int ar_nothing  = 0x0;
            public const int ar_novowel = 0x1;
            public const int ar_composedtashkeel = 0x4;
            public const int ar_lig = 0x8;
            /**
            * Digit shaping option: Replace European digits (U+0030...U+0039) by Arabic-Indic digits.
            */
            public const int DIGITS_EN2AN = 0x20;
            
            /**
            * Digit shaping option: Replace Arabic-Indic digits by European digits (U+0030...U+0039).
            */
            public const int DIGITS_AN2EN = 0x40;
            
            /**
            * Digit shaping option:
            * Replace European digits (U+0030...U+0039) by Arabic-Indic digits
            * if the most recent strongly directional character
            * is an Arabic letter (its Bidi direction value is RIGHT_TO_LEFT_ARABIC).
            * The initial state at the start of the text is assumed to be not an Arabic,
            * letter, so European digits at the start of the text will not change.
            * Compare to DIGITS_ALEN2AN_INIT_AL.
            */
            public const int DIGITS_EN2AN_INIT_LR = 0x60;
            
            /**
            * Digit shaping option:
            * Replace European digits (U+0030...U+0039) by Arabic-Indic digits
            * if the most recent strongly directional character
            * is an Arabic letter (its Bidi direction value is RIGHT_TO_LEFT_ARABIC).
            * The initial state at the start of the text is assumed to be an Arabic,
            * letter, so European digits at the start of the text will change.
            * Compare to DIGITS_ALEN2AN_INT_LR.
            */
            public const int DIGITS_EN2AN_INIT_AL = 0x80;
            
            /** Not a valid option value. */
            private const int DIGITS_RESERVED = 0xa0;
            
            /**
            * Bit mask for digit shaping options.
            */
            public const int DIGITS_MASK = 0xe0;
            
            /**
            * Digit type option: Use Arabic-Indic digits (U+0660...U+0669).
            */
            public const int DIGIT_TYPE_AN = 0;
            
            /**
            * Digit type option: Use Eastern (Extended) Arabic-Indic digits (U+06f0...U+06f9).
            */
            public const int DIGIT_TYPE_AN_EXTENDED = 0x100;

            /**
            * Bit mask for digit type options.
            */
            public const int DIGIT_TYPE_MASK = '\u0100'; // '\u3f00'?

            private class Charstruct {
                internal char basechar;
                internal char mark1;               /* has to be initialized to zero */
                internal char vowel;
                internal int lignum;           /* is a ligature with lignum aditional characters */
                internal int numshapes = 1;
            };


    }
}
