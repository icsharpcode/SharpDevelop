using System;
using System.Collections;
using System.Globalization;
using System.util;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.html;
/*
 * Copyright 2004 Paulo Soares
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

namespace iTextSharp.text.html.simpleparser {

    /**
    *
    * @author  psoares
    */
    public class FactoryProperties {
        
        private FontFactoryImp fontImp = FontFactory.FontImp;

        /** Creates a new instance of FactoryProperties */
        public FactoryProperties() {
        }
        
        public Chunk CreateChunk(String text, ChainedProperties props) {
            Font font = GetFont(props);
            float size = font.Size;
            size /= 2;
            Chunk ck = new Chunk(text, font);
            if (props.HasProperty("sub"))
                ck.SetTextRise(-size);
            else if (props.HasProperty("sup"))
                ck.SetTextRise(size);
            ck.SetHyphenation(GetHyphenation(props));
            return ck;
        }
        
        private static void SetParagraphLeading(Paragraph p, String leading) {
            if (leading == null) {
                p.SetLeading(0, 1.5f);
                return;
            }
            try {
                StringTokenizer tk = new StringTokenizer(leading, " ,");
                String v = tk.NextToken();
                float v1 = float.Parse(v, System.Globalization.NumberFormatInfo.InvariantInfo);
                if (!tk.HasMoreTokens()) {
                    p.SetLeading(v1, 0);
                    return;
                }
                v = tk.NextToken();
                float v2 = float.Parse(v, System.Globalization.NumberFormatInfo.InvariantInfo);
                p.SetLeading(v1, v2);
            }
            catch {
                p.SetLeading(0, 1.5f);
            }

        }

        public static Paragraph CreateParagraph(Hashtable props) {
            Paragraph p = new Paragraph();
            String value = (String)props["align"];
            if (value != null) {
                if (Util.EqualsIgnoreCase(value, "center"))
                    p.Alignment = Element.ALIGN_CENTER;
                else if (Util.EqualsIgnoreCase(value, "right"))
                    p.Alignment = Element.ALIGN_RIGHT;
                else if (Util.EqualsIgnoreCase(value, "justify"))
                    p.Alignment = Element.ALIGN_JUSTIFIED;
            }
            SetParagraphLeading(p, (String)props["leading"]);
            p.Hyphenation = GetHyphenation(props);
            return p;
        }

        public static void CreateParagraph(Paragraph p, ChainedProperties props) {
            String value = props["align"];
            if (value != null) {
                if (Util.EqualsIgnoreCase(value, "center"))
                    p.Alignment = Element.ALIGN_CENTER;
                else if (Util.EqualsIgnoreCase(value, "right"))
                    p.Alignment = Element.ALIGN_RIGHT;
                else if (Util.EqualsIgnoreCase(value, "justify"))
                    p.Alignment = Element.ALIGN_JUSTIFIED;
            }
            p.Hyphenation = GetHyphenation(props);
            SetParagraphLeading(p, props["leading"]);
            value = props["before"];
            if (value != null) {
                try {
                    p.SpacingBefore = float.Parse(value, System.Globalization.NumberFormatInfo.InvariantInfo);
                }
                catch {}
            }
            value = props["after"];
            if (value != null) {
                try {
                    p.SpacingAfter = float.Parse(value, System.Globalization.NumberFormatInfo.InvariantInfo);
                }
                catch {}
            }
            value = props["extraparaspace"];
            if (value != null) {
                try {
                    p.ExtraParagraphSpace = float.Parse(value, System.Globalization.NumberFormatInfo.InvariantInfo);
                }
                catch {}
            }
        }

        public static Paragraph CreateParagraph(ChainedProperties props) {
            Paragraph p = new Paragraph();
            CreateParagraph(p, props);
            return p;
        }

        public static ListItem CreateListItem(ChainedProperties props) {
            ListItem p = new ListItem();
            CreateParagraph(p, props);
            return p;
        }

        public Font GetFont(ChainedProperties props) {
            String face = props["face"];
            if (face != null) {
                StringTokenizer tok = new StringTokenizer(face, ",");
                while (tok.HasMoreTokens()) {
                    face = tok.NextToken().Trim();
                    if (face.StartsWith("\""))
                        face = face.Substring(1);
                    if (face.EndsWith("\""))
                        face = face.Substring(0, face.Length - 1);
                    if (fontImp.IsRegistered(face))
                        break;
                }
            }
            int style = 0;
            if (props.HasProperty("i"))
                style |= Font.ITALIC;
            if (props.HasProperty("b"))
                style |= Font.BOLD;
            if (props.HasProperty("u"))
                style |= Font.UNDERLINE;
            if (props.HasProperty("s"))
                style |= Font.STRIKETHRU ;

            String value = props["size"];
            float size = 12;
            if (value != null)
                size = float.Parse(value, System.Globalization.NumberFormatInfo.InvariantInfo);
            Color color = Markup.DecodeColor(props["color"]);
            String encoding = props["encoding"];
            if (encoding == null)
                encoding = BaseFont.WINANSI;
            return fontImp.GetFont(face, encoding, true, size, style, color);
        }
        
        /**
        * Gets a HyphenationEvent based on the hyphenation entry in ChainedProperties.
        * @param    props   ChainedProperties
        * @return   a HyphenationEvent
        * @since    2.1.2
        */
        public static IHyphenationEvent GetHyphenation(ChainedProperties props) {
            return GetHyphenation(props["hyphenation"]);
        }

        /**
        * Gets a HyphenationEvent based on the hyphenation entry in a HashMap.
        * @param    props   a HashMap with properties
        * @return   a HyphenationEvent
        * @since    2.1.2
        */
        public static IHyphenationEvent GetHyphenation(Hashtable props) {
            return GetHyphenation((String)props["hyphenation"]);
        }
        
        /**
        * Gets a HyphenationEvent based on a String.
        * For instance "en_UK,3,2" returns new HyphenationAuto("en", "UK", 3, 2);
        * @param    a String, for instance "en_UK,2,2"
        * @return   a HyphenationEvent
        * @since    2.1.2
        */
        public static IHyphenationEvent GetHyphenation(String s) {
            if (s == null || s.Length == 0) {
                return null;
            }
            String lang = s;
            String country = null;
            int leftMin = 2;
            int rightMin = 2;
            
            int pos = s.IndexOf('_');
            if (pos == -1) {
                return new HyphenationAuto(lang, country, leftMin, rightMin);
            }
            lang = s.Substring(0, pos);
            country = s.Substring(pos + 1);
            pos = country.IndexOf(',');
            if (pos == -1) {
                return new HyphenationAuto(lang, country, leftMin, rightMin);
            }
            s = country.Substring(pos + 1);
            country = country.Substring(0, pos);
            pos = s.IndexOf(',');
            if (pos == -1) {
                leftMin = int.Parse(s);
            }
            else {
                leftMin = int.Parse(s.Substring(0, pos));
                rightMin = int.Parse(s.Substring(pos + 1));
            }   
            return new HyphenationAuto(lang, country, leftMin, rightMin);
        }
        
        public static void InsertStyle(Hashtable h) {
            String style = (String)h["style"];
            if (style == null)
                return;
            Properties prop = Markup.ParseAttributes(style);
            foreach (String key in prop.Keys) {
                if (key.Equals(Markup.CSS_KEY_FONTFAMILY)) {
                    h["face"] = prop[key];
                }
                else if (key.Equals(Markup.CSS_KEY_FONTSIZE)) {
                    h["size"] = Markup.ParseLength(prop[key]).ToString(NumberFormatInfo.InvariantInfo) + "px";
                }
                else if (key.Equals(Markup.CSS_KEY_FONTSTYLE)) {
                    String ss = prop[key].Trim().ToLower(CultureInfo.InvariantCulture);
                    if (ss.Equals("italic") || ss.Equals("oblique"))
                        h["i"] = null;
                }
                else if (key.Equals(Markup.CSS_KEY_FONTWEIGHT)) {
                    String ss = prop[key].Trim().ToLower(CultureInfo.InvariantCulture);
                    if (ss.Equals("bold") || ss.Equals("700") || ss.Equals("800") || ss.Equals("900"))
                        h["b"] = null;
                }
                else if (key.Equals(Markup.CSS_KEY_FONTWEIGHT)) {
                    String ss = prop[key].Trim().ToLower(CultureInfo.InvariantCulture);
                    if (ss.Equals("underline"))
                        h["u"] = null;
                }
                else if (key.Equals(Markup.CSS_KEY_COLOR)) {
                    Color c = Markup.DecodeColor(prop[key]);
                    if (c != null) {
                        int hh = c.ToArgb() & 0xffffff;
                        String hs = "#" + hh.ToString("X06", NumberFormatInfo.InvariantInfo);
                        h["color"] = hs;
                    }
                }
                else if (key.Equals(Markup.CSS_KEY_LINEHEIGHT)) {
                    String ss = prop[key].Trim();
                    float v = Markup.ParseLength(prop[key]);
                    if (ss.EndsWith("%")) {
                        v /= 100;
                        h["leading"] = "0," + v.ToString(NumberFormatInfo.InvariantInfo);
                    }
                    else {
                        h["leading"] = v.ToString(NumberFormatInfo.InvariantInfo) + ",0";
                    }
                }
                else if (key.Equals(Markup.CSS_KEY_TEXTALIGN)) {
                    String ss = prop[key].Trim().ToLower(System.Globalization.CultureInfo.InvariantCulture);
                    h["align"] = ss;
                }
            }
        }
        
        public FontFactoryImp FontImp {
            get {
                return fontImp;
            }
            set {
                fontImp = value;
            }
        }

        public static Hashtable followTags = new Hashtable();

        static FactoryProperties() {
            followTags["i"] = "i";
            followTags["b"] = "b";
            followTags["u"] = "u";
            followTags["sub"] = "sub";
            followTags["sup"] = "sup";
            followTags["em"] = "i";
            followTags["strong"] = "b";
            followTags["s"] = "s";
            followTags["strike"] = "s";
        }
    }
}