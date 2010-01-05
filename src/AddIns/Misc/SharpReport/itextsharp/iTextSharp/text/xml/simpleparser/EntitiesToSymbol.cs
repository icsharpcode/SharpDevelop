using System;
using System.Collections;
using iTextSharp.text;
/*
 * $Id: EntitiesToSymbol.cs,v 1.3 2008/05/13 11:26:14 psoares33 Exp $
 * 
 *
 * Copyright 1999, 2000, 2001, 2002 Bruno Lowagie.
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

namespace iTextSharp.text.xml.simpleparser {

    /**
    * This class contains entities that can be used in an entity tag.
    */

    public class EntitiesToSymbol {
        
        /**
        * This is a map that contains all possible id values of the entity tag
        * that can be translated to a character in font Symbol.
        */
        public static readonly Hashtable map;
        
        static EntitiesToSymbol() {
            map = new Hashtable();
            map["169"] = (char)227;
            map["172"] = (char)216;
            map["174"] = (char)210;
            map["177"] = (char)177;
            map["215"] = (char)180;
            map["247"] = (char)184;
            map["8230"] = (char)188;
            map["8242"] = (char)162;
            map["8243"] = (char)178;
            map["8260"] = (char)164;
            map["8364"] = (char)240;
            map["8465"] = (char)193;
            map["8472"] = (char)195;
            map["8476"] = (char)194;
            map["8482"] = (char)212;
            map["8501"] = (char)192;
            map["8592"] = (char)172;
            map["8593"] = (char)173;
            map["8594"] = (char)174;
            map["8595"] = (char)175;
            map["8596"] = (char)171;
            map["8629"] = (char)191;
            map["8656"] = (char)220;
            map["8657"] = (char)221;
            map["8658"] = (char)222;
            map["8659"] = (char)223;
            map["8660"] = (char)219;
            map["8704"] = (char)34;
            map["8706"] = (char)182;
            map["8707"] = (char)36;
            map["8709"] = (char)198;
            map["8711"] = (char)209;
            map["8712"] = (char)206;
            map["8713"] = (char)207;
            map["8717"] = (char)39;
            map["8719"] = (char)213;
            map["8721"] = (char)229;
            map["8722"] = (char)45;
            map["8727"] = (char)42;
            map["8729"] = (char)183;
            map["8730"] = (char)214;
            map["8733"] = (char)181;
            map["8734"] = (char)165;
            map["8736"] = (char)208;
            map["8743"] = (char)217;
            map["8744"] = (char)218;
            map["8745"] = (char)199;
            map["8746"] = (char)200;
            map["8747"] = (char)242;
            map["8756"] = (char)92;
            map["8764"] = (char)126;
            map["8773"] = (char)64;
            map["8776"] = (char)187;
            map["8800"] = (char)185;
            map["8801"] = (char)186;
            map["8804"] = (char)163;
            map["8805"] = (char)179;
            map["8834"] = (char)204;
            map["8835"] = (char)201;
            map["8836"] = (char)203;
            map["8838"] = (char)205;
            map["8839"] = (char)202;
            map["8853"] = (char)197;
            map["8855"] = (char)196;
            map["8869"] = (char)94;
            map["8901"] = (char)215;
            map["8992"] = (char)243;
            map["8993"] = (char)245;
            map["9001"] = (char)225;
            map["9002"] = (char)241;
            map["913"] = (char)65;
            map["914"] = (char)66;
            map["915"] = (char)71;
            map["916"] = (char)68;
            map["917"] = (char)69;
            map["918"] = (char)90;
            map["919"] = (char)72;
            map["920"] = (char)81;
            map["921"] = (char)73;
            map["922"] = (char)75;
            map["923"] = (char)76;
            map["924"] = (char)77;
            map["925"] = (char)78;
            map["926"] = (char)88;
            map["927"] = (char)79;
            map["928"] = (char)80;
            map["929"] = (char)82;
            map["931"] = (char)83;
            map["932"] = (char)84;
            map["933"] = (char)85;
            map["934"] = (char)70;
            map["935"] = (char)67;
            map["936"] = (char)89;
            map["937"] = (char)87;
            map["945"] = (char)97;
            map["946"] = (char)98;
            map["947"] = (char)103;
            map["948"] = (char)100;
            map["949"] = (char)101;
            map["950"] = (char)122;
            map["951"] = (char)104;
            map["952"] = (char)113;
            map["953"] = (char)105;
            map["954"] = (char)107;
            map["955"] = (char)108;
            map["956"] = (char)109;
            map["957"] = (char)110;
            map["958"] = (char)120;
            map["959"] = (char)111;
            map["960"] = (char)112;
            map["961"] = (char)114;
            map["962"] = (char)86;
            map["963"] = (char)115;
            map["964"] = (char)116;
            map["965"] = (char)117;
            map["966"] = (char)102;
            map["967"] = (char)99;
            map["9674"] = (char)224;
            map["968"] = (char)121;
            map["969"] = (char)119;
            map["977"] = (char)74;
            map["978"] = (char)161;
            map["981"] = (char)106;
            map["982"] = (char)118;
            map["9824"] = (char)170;
            map["9827"] = (char)167;
            map["9829"] = (char)169;
            map["9830"] = (char)168;
            map["Alpha"] = (char)65;
            map["Beta"] = (char)66;
            map["Chi"] = (char)67;
            map["Delta"] = (char)68;
            map["Epsilon"] = (char)69;
            map["Eta"] = (char)72;
            map["Gamma"] = (char)71;
            map["Iota"] = (char)73;
            map["Kappa"] = (char)75;
            map["Lambda"] = (char)76;
            map["Mu"] = (char)77;
            map["Nu"] = (char)78;
            map["Omega"] = (char)87;
            map["Omicron"] = (char)79;
            map["Phi"] = (char)70;
            map["Pi"] = (char)80;
            map["Prime"] = (char)178;
            map["Psi"] = (char)89;
            map["Rho"] = (char)82;
            map["Sigma"] = (char)83;
            map["Tau"] = (char)84;
            map["Theta"] = (char)81;
            map["Upsilon"] = (char)85;
            map["Xi"] = (char)88;
            map["Zeta"] = (char)90;
            map["alefsym"] = (char)192;
            map["alpha"] = (char)97;
            map["and"] = (char)217;
            map["ang"] = (char)208;
            map["asymp"] = (char)187;
            map["beta"] = (char)98;
            map["cap"] = (char)199;
            map["chi"] = (char)99;
            map["clubs"] = (char)167;
            map["cong"] = (char)64;
            map["copy"] = (char)211;
            map["crarr"] = (char)191;
            map["cup"] = (char)200;
            map["dArr"] = (char)223;
            map["darr"] = (char)175;
            map["delta"] = (char)100;
            map["diams"] = (char)168;
            map["divide"] = (char)184;
            map["empty"] = (char)198;
            map["epsilon"] = (char)101;
            map["equiv"] = (char)186;
            map["eta"] = (char)104;
            map["euro"] = (char)240;
            map["exist"] = (char)36;
            map["forall"] = (char)34;
            map["frasl"] = (char)164;
            map["gamma"] = (char)103;
            map["ge"] = (char)179;
            map["hArr"] = (char)219;
            map["harr"] = (char)171;
            map["hearts"] = (char)169;
            map["hellip"] = (char)188;
            map["horizontal arrow extender"] = (char)190;
            map["image"] = (char)193;
            map["infin"] = (char)165;
            map["int"] = (char)242;
            map["iota"] = (char)105;
            map["isin"] = (char)206;
            map["kappa"] = (char)107;
            map["lArr"] = (char)220;
            map["lambda"] = (char)108;
            map["lang"] = (char)225;
            map["large brace extender"] = (char)239;
            map["large integral extender"] = (char)244;
            map["large left brace (bottom)"] = (char)238;
            map["large left brace (middle)"] = (char)237;
            map["large left brace (top)"] = (char)236;
            map["large left bracket (bottom)"] = (char)235;
            map["large left bracket (extender)"] = (char)234;
            map["large left bracket (top)"] = (char)233;
            map["large left parenthesis (bottom)"] = (char)232;
            map["large left parenthesis (extender)"] = (char)231;
            map["large left parenthesis (top)"] = (char)230;
            map["large right brace (bottom)"] = (char)254;
            map["large right brace (middle)"] = (char)253;
            map["large right brace (top)"] = (char)252;
            map["large right bracket (bottom)"] = (char)251;
            map["large right bracket (extender)"] = (char)250;
            map["large right bracket (top)"] = (char)249;
            map["large right parenthesis (bottom)"] = (char)248;
            map["large right parenthesis (extender)"] = (char)247;
            map["large right parenthesis (top)"] = (char)246;
            map["larr"] = (char)172;
            map["le"] = (char)163;
            map["lowast"] = (char)42;
            map["loz"] = (char)224;
            map["minus"] = (char)45;
            map["mu"] = (char)109;
            map["nabla"] = (char)209;
            map["ne"] = (char)185;
            map["not"] = (char)216;
            map["notin"] = (char)207;
            map["nsub"] = (char)203;
            map["nu"] = (char)110;
            map["omega"] = (char)119;
            map["omicron"] = (char)111;
            map["oplus"] = (char)197;
            map["or"] = (char)218;
            map["otimes"] = (char)196;
            map["part"] = (char)182;
            map["perp"] = (char)94;
            map["phi"] = (char)102;
            map["pi"] = (char)112;
            map["piv"] = (char)118;
            map["plusmn"] = (char)177;
            map["prime"] = (char)162;
            map["prod"] = (char)213;
            map["prop"] = (char)181;
            map["psi"] = (char)121;
            map["rArr"] = (char)222;
            map["radic"] = (char)214;
            map["radical extender"] = (char)96;
            map["rang"] = (char)241;
            map["rarr"] = (char)174;
            map["real"] = (char)194;
            map["reg"] = (char)210;
            map["rho"] = (char)114;
            map["sdot"] = (char)215;
            map["sigma"] = (char)115;
            map["sigmaf"] = (char)86;
            map["sim"] = (char)126;
            map["spades"] = (char)170;
            map["sub"] = (char)204;
            map["sube"] = (char)205;
            map["sum"] = (char)229;
            map["sup"] = (char)201;
            map["supe"] = (char)202;
            map["tau"] = (char)116;
            map["there4"] = (char)92;
            map["theta"] = (char)113;
            map["thetasym"] = (char)74;
            map["times"] = (char)180;
            map["trade"] = (char)212;
            map["uArr"] = (char)221;
            map["uarr"] = (char)173;
            map["upsih"] = (char)161;
            map["upsilon"] = (char)117;
            map["vertical arrow extender"] = (char)189;
            map["weierp"] = (char)195;
            map["xi"] = (char)120;
            map["zeta"] = (char)122;
        }
        
    /**
    * Gets a chunk with a symbol character.
    * @param e a symbol value (see Entities class: alfa is greek alfa,...)
    * @param font the font if the symbol isn't found (otherwise Font.SYMBOL)
    * @return a Chunk
    */
        
        public static Chunk Get(String e, Font font) {
            char s = GetCorrespondingSymbol(e);
            if (s == '\0') {
                try {
                    return new Chunk("" + (char)int.Parse(e), font);
                }
                catch (Exception) {
                    return new Chunk(e, font);
                }
            }
            Font symbol = new Font(Font.SYMBOL, font.Size, font.Style, font.Color);
            return new Chunk(s.ToString(), symbol);
        }
        
    /**
    * Looks for the corresponding symbol in the font Symbol.
    *
    * @param    name    the name of the entity
    * @return   the corresponding character in font Symbol
    */
        
        public static char GetCorrespondingSymbol(String name) {
            if (map.ContainsKey(name))
                return (char)map[name];
            else
                return '\0';
        }
    }
}