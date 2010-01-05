using System;
using System.Collections;
using System.Text;
/*
 * $Id: IanaEncodings.cs,v 1.4 2008/05/13 11:26:14 psoares33 Exp $
 * 
 *
 * Copyright 2003-2007 Paulo Soares and Bruno Lowagie.
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
 *
 * The values used in this class are based on class org.apache.xercis.util.EncodingMap
 * http://svn.apache.org/viewvc/xerces/java/trunk/src/org/apache/xerces/util/EncodingMap.java?view=markup
 * This class was originally published under the following license:
 *
 * Licensed to the Apache Software Foundation (ASF) under one or more
 * contributor license agreements.  See the NOTICE file distributed with
 * this work for additional information regarding copyright ownership.
 * The ASF licenses this file to You under the Apache License, Version 2.0
 * (the "License"); you may not use this file except in compliance with
 * the License.  You may obtain a copy of the License at
 * 
 *      http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

namespace iTextSharp.text.xml.simpleparser {

    /**
    * Translates a IANA encoding name to a Java encoding.
    */

    public class IanaEncodings {

	    /** The object that maps IANA to Java encodings. */
        private static readonly Hashtable map = new Hashtable();

        static IanaEncodings() {        
            // add IANA to .NET encoding mappings.
            map["CP037"] = 37;
            map["CSIBM037"] = 37;
            map["EBCDIC-CP-CA"] = 37;
            map["EBCDIC-CP-NL"] = 37;
            map["EBCDIC-CP-US"] = 37;
            map["EBCDIC-CP-WT"] = 37;
            map["IBM037"] = 37;
            map["CP437"] = 437;
            map["CSPC8CODEPAGE437"] = 437;
            map["IBM437"] = 437;
            map["CP500"] = 500;
            map["CSIBM500"] = 500;
            map["EBCDIC-CP-BE"] = 500;
            map["EBCDIC-CP-CH"] = 500;
            map["IBM500"] = 500;
            map["ASMO-708"] = 708;
            map["DOS-720"] = 720;
            map["IBM737"] = 737;
            map["IBM775"] = 775;
            map["CP850"] = 850;
            map["IBM850"] = 850;
            map["CP852"] = 852;
            map["IBM852"] = 852;
            map["CP855"] = 855;
            map["IBM855"] = 855;
            map["CP857"] = 857;
            map["IBM857"] = 857;
            map["CCSID00858"] = 858;
            map["CP00858"] = 858;
            map["CP858"] = 858;
            map["IBM00858"] = 858;
            map["PC-MULTILINGUAL-850+EURO"] = 858;
            map["CP860"] = 860;
            map["IBM860"] = 860;
            map["CP861"] = 861;
            map["IBM861"] = 861;
            map["CP862"] = 862;
            map["DOS-862"] = 862;
            map["IBM862"] = 862;
            map["CP863"] = 863;
            map["IBM863"] = 863;
            map["CP864"] = 864;
            map["IBM864"] = 864;
            map["CP865"] = 865;
            map["IBM865"] = 865;
            map["CP866"] = 866;
            map["IBM866"] = 866;
            map["CP869"] = 869;
            map["IBM869"] = 869;
            map["CP870"] = 870;
            map["CSIBM870"] = 870;
            map["EBCDIC-CP-ROECE"] = 870;
            map["EBCDIC-CP-YU"] = 870;
            map["IBM870"] = 870;
            map["DOS-874"] = 874;
            map["ISO-8859-11"] = 874;
            map["MS874"] = 874;
            map["TIS620"] = 874;
            map["TIS-620"] = 874;
            map["WINDOWS-874"] = 874;
            map["CP875"] = 875;
            map["CSSHIFTJIS"] = 932;
            map["CSWINDOWS31J"] = 932;
            map["MS932"] = 932;
            map["MS_KANJI"] = 932;
            map["SHIFT-JIS"] = 932;
            map["SHIFT_JIS"] = 932;
            map["SJIS"] = 932;
            map["X-MS-CP932"] = 932;
            map["X-SJIS"] = 932;
            map["CHINESE"] = 936;
            map["CN-GB"] = 936;
            map["CSGB2312"] = 936;
            map["CSGB231280"] = 936;
            map["CSISO58GB231280"] = 936;
            map["GB2312"] = 936;
            map["GB2312-80"] = 936;
            map["GB231280"] = 936;
            map["GB_2312-80"] = 936;
            map["GBK"] = 936;
            map["ISO-IR-58"] = 936;
            map["MS936"] = 936;
            map["CSKSC56011987"] = 949;
            map["ISO-IR-149"] = 949;
            map["KOREAN"] = 949;
            map["KS-C-5601"] = 949;
            map["KS-C5601"] = 949;
            map["KS_C_5601"] = 949;
            map["KS_C_5601-1987"] = 949;
            map["KS_C_5601-1989"] = 949;
            map["KS_C_5601_1987"] = 949;
            map["KSC5601"] = 949;
            map["KSC_5601"] = 949;
            map["MS949"] = 949;
            map["BIG5"] = 950;
            map["BIG5-HKSCS"] = 950;
            map["CN-BIG5"] = 950;
            map["CSBIG5"] = 950;
            map["MS950"] = 950;
            map["X-X-BIG5"] = 950;
            map["CP1026"] = 1026;
            map["CSIBM1026"] = 1026;
            map["IBM1026"] = 1026;
            map["IBM01047"] = 1047;
            map["CCSID01140"] = 1140;
            map["CP01140"] = 1140;
            map["EBCDIC-US-37+EURO"] = 1140;
            map["IBM01140"] = 1140;
            map["CCSID01141"] = 1141;
            map["CP01141"] = 1141;
            map["EBCDIC-DE-273+EURO"] = 1141;
            map["IBM01141"] = 1141;
            map["CCSID01142"] = 1142;
            map["CP01142"] = 1142;
            map["EBCDIC-DK-277+EURO"] = 1142;
            map["EBCDIC-NO-277+EURO"] = 1142;
            map["IBM01142"] = 1142;
            map["CCSID01143"] = 1143;
            map["CP01143"] = 1143;
            map["EBCDIC-FI-278+EURO"] = 1143;
            map["EBCDIC-SE-278+EURO"] = 1143;
            map["IBM01143"] = 1143;
            map["CCSID01144"] = 1144;
            map["CP01144"] = 1144;
            map["EBCDIC-IT-280+EURO"] = 1144;
            map["IBM01144"] = 1144;
            map["CCSID01145"] = 1145;
            map["CP01145"] = 1145;
            map["EBCDIC-ES-284+EURO"] = 1145;
            map["IBM01145"] = 1145;
            map["CCSID01146"] = 1146;
            map["CP01146"] = 1146;
            map["EBCDIC-GB-285+EURO"] = 1146;
            map["IBM01146"] = 1146;
            map["CCSID01147"] = 1147;
            map["CP01147"] = 1147;
            map["EBCDIC-FR-297+EURO"] = 1147;
            map["IBM01147"] = 1147;
            map["CCSID01148"] = 1148;
            map["CP01148"] = 1148;
            map["EBCDIC-INTERNATIONAL-500+EURO"] = 1148;
            map["IBM01148"] = 1148;
            map["CCSID01149"] = 1149;
            map["CP01149"] = 1149;
            map["EBCDIC-IS-871+EURO"] = 1149;
            map["IBM01149"] = 1149;
            map["ISO-10646-UCS-2"] = 1200;
            map["UCS-2"] = 1200;
            map["UNICODE"] = 1200;
            map["UTF-16"] = 1200;
            map["UTF-16LE"] = 1200;
            map["UNICODELITTLEUNMARKED"] = 1200;
            map["UNICODELITTLE"] = 1200;
            map["UNICODEFFFE"] = 1201;
            map["UTF-16BE"] = 1201;
            map["UNICODEBIGUNMARKED"] = 1201;
            map["UNICODEBIG"] = 1201;
            map["CP1250"] = 1250;
            map["WINDOWS-1250"] = 1250;
            map["X-CP1250"] = 1250;
            map["CP1251"] = 1251;
            map["WINDOWS-1251"] = 1251;
            map["X-CP1251"] = 1251;
            map["CP1252"] = 1252;
            map["WINDOWS-1252"] = 1252;
            map["X-ANSI"] = 1252;
            map["CP1253"] = 1253;
            map["WINDOWS-1253"] = 1253;
            map["CP1254"] = 1254;
            map["WINDOWS-1254"] = 1254;
            map["CP1255"] = 1255;
            map["WINDOWS-1255"] = 1255;
            map["CP1256"] = 1256;
            map["WINDOWS-1256"] = 1256;
            map["CP1257"] = 1257;
            map["WINDOWS-1257"] = 1257;
            map["CP1258"] = 1258;
            map["WINDOWS-1258"] = 1258;
            map["JOHAB"] = 1361;
            map["MACINTOSH"] = 10000;
            map["MACROMAN"] = 10000;
            map["X-MAC-JAPANESE"] = 10001;
            map["X-MAC-CHINESETRAD"] = 10002;
            map["X-MAC-KOREAN"] = 10003;
            map["MACARABIC"] = 10004;
            map["X-MAC-ARABIC"] = 10004;
            map["MACHEBREW"] = 10005;
            map["X-MAC-HEBREW"] = 10005;
            map["MACGREEK"] = 10006;
            map["X-MAC-GREEK"] = 10006;
            map["MACCYRILLIC"] = 10007;
            map["X-MAC-CYRILLIC"] = 10007;
            map["X-MAC-CHINESESIMP"] = 10008;
            map["MACROMANIA"] = 10010;
            map["MACROMANIAN"] = 10010;
            map["X-MAC-ROMANIAN"] = 10010;
            map["MACUKRAINE"] = 10017;
            map["MACUKRAINIAN"] = 10017;
            map["X-MAC-UKRAINIAN"] = 10017;
            map["MACTHAI"] = 10021;
            map["X-MAC-THAI"] = 10021;
            map["MACCENTRALEUROPE"] = 10029;
            map["X-MAC-CE"] = 10029;
            map["MACICELANDIC"] = 10079;
            map["MACICELAND"] = 10079;
            map["X-MAC-ICELANDIC"] = 10079;
            map["MACTURKISH"] = 10081;
            map["X-MAC-TURKISH"] = 10081;
            map["MACCROATIAN"] = 10082;
            map["X-MAC-CROATIAN"] = 10082;
            map["X-CHINESE-CNS"] = 20000;
            map["X-CP20001"] = 20001;
            map["X-CHINESE-ETEN"] = 20002;
            map["X-CP20003"] = 20003;
            map["X-CP20004"] = 20004;
            map["X-CP20005"] = 20005;
            map["IRV"] = 20105;
            map["X-IA5"] = 20105;
            map["DIN_66003"] = 20106;
            map["GERMAN"] = 20106;
            map["X-IA5-GERMAN"] = 20106;
            map["SEN_850200_B"] = 20107;
            map["SWEDISH"] = 20107;
            map["X-IA5-SWEDISH"] = 20107;
            map["NORWEGIAN"] = 20108;
            map["NS_4551-1"] = 20108;
            map["X-IA5-NORWEGIAN"] = 20108;
            map["ANSI_X3.4-1968"] = 20127;
            map["ANSI_X3.4-1986"] = 20127;
            map["ASCII"] = 20127;
            map["CP367"] = 20127;
            map["CSASCII"] = 20127;
            map["IBM367"] = 20127;
            map["ISO-IR-6"] = 20127;
            map["ISO646-US"] = 20127;
            map["ISO_646.IRV:1991"] = 20127;
            map["US"] = 20127;
            map["US-ASCII"] = 20127;
            map["X-CP20261"] = 20261;
            map["X-CP20269"] = 20269;
            map["CP273"] = 20273;
            map["CSIBM273"] = 20273;
            map["IBM273"] = 20273;
            map["CSIBM277"] = 20277;
            map["EBCDIC-CP-DK"] = 20277;
            map["EBCDIC-CP-NO"] = 20277;
            map["IBM277"] = 20277;
            map["CP278"] = 20278;
            map["CSIBM278"] = 20278;
            map["EBCDIC-CP-FI"] = 20278;
            map["EBCDIC-CP-SE"] = 20278;
            map["IBM278"] = 20278;
            map["CP280"] = 20280;
            map["CSIBM280"] = 20280;
            map["EBCDIC-CP-IT"] = 20280;
            map["IBM280"] = 20280;
            map["CP284"] = 20284;
            map["CSIBM284"] = 20284;
            map["EBCDIC-CP-ES"] = 20284;
            map["IBM284"] = 20284;
            map["CP285"] = 20285;
            map["CSIBM285"] = 20285;
            map["EBCDIC-CP-GB"] = 20285;
            map["IBM285"] = 20285;
            map["CP290"] = 20290;
            map["CSIBM290"] = 20290;
            map["EBCDIC-JP-KANA"] = 20290;
            map["IBM290"] = 20290;
            map["CP297"] = 20297;
            map["CSIBM297"] = 20297;
            map["EBCDIC-CP-FR"] = 20297;
            map["IBM297"] = 20297;
            map["CP420"] = 20420;
            map["CSIBM420"] = 20420;
            map["EBCDIC-CP-AR1"] = 20420;
            map["IBM420"] = 20420;
            map["CP423"] = 20423;
            map["CSIBM423"] = 20423;
            map["EBCDIC-CP-GR"] = 20423;
            map["IBM423"] = 20423;
            map["CP424"] = 20424;
            map["CSIBM424"] = 20424;
            map["EBCDIC-CP-HE"] = 20424;
            map["IBM424"] = 20424;
            map["X-EBCDIC-KOREANEXTENDED"] = 20833;
            map["CSIBMTHAI"] = 20838;
            map["IBM-THAI"] = 20838;
            map["CSKOI8R"] = 20866;
            map["KOI"] = 20866;
            map["KOI8"] = 20866;
            map["KOI8-R"] = 20866;
            map["KOI8R"] = 20866;
            map["CP871"] = 20871;
            map["CSIBM871"] = 20871;
            map["EBCDIC-CP-IS"] = 20871;
            map["IBM871"] = 20871;
            map["CP880"] = 20880;
            map["CSIBM880"] = 20880;
            map["EBCDIC-CYRILLIC"] = 20880;
            map["IBM880"] = 20880;
            map["CP905"] = 20905;
            map["CSIBM905"] = 20905;
            map["EBCDIC-CP-TR"] = 20905;
            map["IBM905"] = 20905;
            map["CCSID00924"] = 20924;
            map["CP00924"] = 20924;
            map["EBCDIC-LATIN9--EURO"] = 20924;
            map["IBM00924"] = 20924;
            map["X-CP20936"] = 20936;
            map["X-CP20949"] = 20949;
            map["CP1025"] = 21025;
            map["X-CP21027"] = 21027;
            map["KOI8-RU"] = 21866;
            map["KOI8-U"] = 21866;
            map["CP819"] = 28591;
            map["CSISOLATIN1"] = 28591;
            map["IBM819"] = 28591;
            map["ISO-8859-1"] = 28591;
            map["ISO-IR-100"] = 28591;
            map["ISO8859-1"] = 28591;
            map["ISO_8859-1"] = 28591;
            map["ISO_8859-1:1987"] = 28591;
            map["L1"] = 28591;
            map["LATIN1"] = 28591;
            map["CSISOLATIN2"] = 28592;
            map["ISO-8859-2"] = 28592;
            map["ISO-IR-101"] = 28592;
            map["ISO8859-2"] = 28592;
            map["ISO_8859-2"] = 28592;
            map["ISO_8859-2:1987"] = 28592;
            map["L2"] = 28592;
            map["LATIN2"] = 28592;
            map["CSISOLATIN3"] = 28593;
            map["ISO-8859-3"] = 28593;
            map["ISO-IR-109"] = 28593;
            map["ISO_8859-3"] = 28593;
            map["ISO_8859-3:1988"] = 28593;
            map["L3"] = 28593;
            map["LATIN3"] = 28593;
            map["CSISOLATIN4"] = 28594;
            map["ISO-8859-4"] = 28594;
            map["ISO-IR-110"] = 28594;
            map["ISO_8859-4"] = 28594;
            map["ISO_8859-4:1988"] = 28594;
            map["L4"] = 28594;
            map["LATIN4"] = 28594;
            map["CSISOLATINCYRILLIC"] = 28595;
            map["CYRILLIC"] = 28595;
            map["ISO-8859-5"] = 28595;
            map["ISO-IR-144"] = 28595;
            map["ISO_8859-5"] = 28595;
            map["ISO_8859-5:1988"] = 28595;
            map["ARABIC"] = 28596;
            map["CSISOLATINARABIC"] = 28596;
            map["ECMA-114"] = 28596;
            map["ISO-8859-6"] = 28596;
            map["ISO-IR-127"] = 28596;
            map["ISO_8859-6"] = 28596;
            map["ISO_8859-6:1987"] = 28596;
            map["CSISOLATINGREEK"] = 28597;
            map["ECMA-118"] = 28597;
            map["ELOT_928"] = 28597;
            map["GREEK"] = 28597;
            map["GREEK8"] = 28597;
            map["ISO-8859-7"] = 28597;
            map["ISO-IR-126"] = 28597;
            map["ISO_8859-7"] = 28597;
            map["ISO_8859-7:1987"] = 28597;
            map["CSISOLATINHEBREW"] = 28598;
            map["HEBREW"] = 28598;
            map["ISO-8859-8"] = 28598;
            map["ISO-IR-138"] = 28598;
            map["ISO_8859-8"] = 28598;
            map["ISO_8859-8:1988"] = 28598;
            map["LOGICAL"] = 28598;
            map["VISUAL"] = 28598;
            map["CSISOLATIN5"] = 28599;
            map["ISO-8859-9"] = 28599;
            map["ISO-IR-148"] = 28599;
            map["ISO_8859-9"] = 28599;
            map["ISO_8859-9:1989"] = 28599;
            map["L5"] = 28599;
            map["LATIN5"] = 28599;
            map["ISO-8859-13"] = 28603;
            map["CSISOLATIN9"] = 28605;
            map["ISO-8859-15"] = 28605;
            map["ISO_8859-15"] = 28605;
            map["L9"] = 28605;
            map["LATIN9"] = 28605;
            map["X-EUROPA"] = 29001;
            map["ISO-8859-8-I"] = 38598;
            map["ISO-2022-JP"] = 50220;
            map["CSISO2022JP"] = 50221;
            map["CSISO2022KR"] = 50225;
            map["ISO-2022-KR"] = 50225;
            map["ISO-2022-KR-7"] = 50225;
            map["ISO-2022-KR-7BIT"] = 50225;
            map["CP50227"] = 50227;
            map["X-CP50227"] = 50227;
            map["CP930"] = 50930;
            map["X-EBCDIC-JAPANESEANDUSCANADA"] = 50931;
            map["CP933"] = 50933;
            map["CP935"] = 50935;
            map["CP937"] = 50937;
            map["CP939"] = 50939;
            map["CSEUCPKDFMTJAPANESE"] = 51932;
            map["EUC-JP"] = 51932;
            map["EXTENDED_UNIX_CODE_PACKED_FORMAT_FOR_JAPANESE"] = 51932;
            map["ISO-2022-JPEUC"] = 51932;
            map["X-EUC"] = 51932;
            map["X-EUC-JP"] = 51932;
            map["EUC-CN"] = 51936;
            map["X-EUC-CN"] = 51936;
            map["CSEUCKR"] = 51949;
            map["EUC-KR"] = 51949;
            map["ISO-2022-KR-8"] = 51949;
            map["ISO-2022-KR-8BIT"] = 51949;
            map["HZ-GB-2312"] = 52936;
            map["GB18030"] = 54936;
            map["X-ISCII-DE"] = 57002;
            map["X-ISCII-BE"] = 57003;
            map["X-ISCII-TA"] = 57004;
            map["X-ISCII-TE"] = 57005;
            map["X-ISCII-AS"] = 57006;
            map["X-ISCII-OR"] = 57007;
            map["X-ISCII-KA"] = 57008;
            map["X-ISCII-MA"] = 57009;
            map["X-ISCII-GU"] = 57010;
            map["X-ISCII-PA"] = 57011;
            map["CSUNICODE11UTF7"] = 65000;
            map["UNICODE-1-1-UTF-7"] = 65000;
            map["UNICODE-2-0-UTF-7"] = 65000;
            map["UTF-7"] = 65000;
            map["X-UNICODE-1-1-UTF-7"] = 65000;
            map["X-UNICODE-2-0-UTF-7"] = 65000;
            map["UNICODE-1-1-UTF-8"] = 65001;
            map["UNICODE-2-0-UTF-8"] = 65001;
            map["UTF-8"] = 65001;
            map["X-UNICODE-1-1-UTF-8"] = 65001;
            map["X-UNICODE-2-0-UTF-8"] = 65001;
        }
        
        public static int GetEncodingNumber(string name) {
            object n = map[name.ToUpper(System.Globalization.CultureInfo.InvariantCulture)];
            if (n == null)
                return 0;
            return (int)n;
        }

        public static Encoding GetEncodingEncoding(string name) {
            String nameU = name.ToUpper(System.Globalization.CultureInfo.InvariantCulture);
            if (nameU.Equals("UNICODEBIGUNMARKED"))
                return new UnicodeEncoding(true, false);
            if (nameU.Equals("UNICODEBIG"))
                return new UnicodeEncoding(true, true);
            if (nameU.Equals("UNICODELITTLEUNMARKED"))
                return new UnicodeEncoding(false, false);
            if (nameU.Equals("UNICODELITTLE"))
                return new UnicodeEncoding(false, true);
            if (map.ContainsKey(nameU))
                return Encoding.GetEncoding((int)map[nameU]);
            else
                return Encoding.GetEncoding(name);
        }
    }
}