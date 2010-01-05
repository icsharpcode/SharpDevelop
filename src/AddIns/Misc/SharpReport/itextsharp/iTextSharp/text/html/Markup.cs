using System;
using System.util;
using System.IO;
using System.Text;
using System.Collections;
using System.Globalization;
using iTextSharp.text;

/*
 * $Id: Markup.cs,v 1.2 2008/05/13 11:25:16 psoares33 Exp $
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
    /// <summary>
    /// A class that contains all the possible tagnames and their attributes.
    /// </summary>
    public class Markup {
        // iText specific
        
        /** the key for any tag */
        public const string ITEXT_TAG = "tag";

        // HTML tags

        /** the markup for the body part of a file */
        public const string HTML_TAG_BODY = "body";
        
        /** The DIV tag. */
        public const string HTML_TAG_DIV = "div";

        /** This is a possible HTML-tag. */
        public const string HTML_TAG_LINK = "link";

        /** The SPAN tag. */
        public const string HTML_TAG_SPAN = "span";

        // HTML attributes

        /** the height attribute. */
        public const string HTML_ATTR_HEIGHT = "height";

        /** the hyperlink reference attribute. */
        public const string HTML_ATTR_HREF = "href";

        /** This is a possible HTML attribute for the LINK tag. */
        public const string HTML_ATTR_REL = "rel";

        /** This is used for inline css style information */
        public const string HTML_ATTR_STYLE = "style";

        /** This is a possible HTML attribute for the LINK tag. */
        public const string HTML_ATTR_TYPE = "type";

        /** This is a possible HTML attribute. */
        public const string HTML_ATTR_STYLESHEET = "stylesheet";

        /** the width attribute. */
        public const string HTML_ATTR_WIDTH = "width";

        /** attribute for specifying externally defined CSS class */
        public const string HTML_ATTR_CSS_CLASS = "class";

        /** The ID attribute. */
        public const string HTML_ATTR_CSS_ID = "id";

        // HTML values
        
        /** This is a possible value for the language attribute (SCRIPT tag). */
        public const string HTML_VALUE_JAVASCRIPT = "text/javascript";
        
        /** This is a possible HTML attribute for the LINK tag. */
        public const string HTML_VALUE_CSS = "text/css";

        // CSS keys

        /** the CSS tag for background color */
        public const string CSS_KEY_BGCOLOR = "background-color";

        /** the CSS tag for text color */
        public const string CSS_KEY_COLOR = "color";

        /** CSS key that indicate the way something has to be displayed */
        public const string CSS_KEY_DISPLAY = "display";

        /** the CSS tag for the font family */
        public const string CSS_KEY_FONTFAMILY = "font-family";

        /** the CSS tag for the font size */
        public const string CSS_KEY_FONTSIZE = "font-size";

        /** the CSS tag for the font style */
        public const string CSS_KEY_FONTSTYLE = "font-style";

        /** the CSS tag for the font weight */
        public const string CSS_KEY_FONTWEIGHT = "font-weight";

        /** the CSS tag for text decorations */
        public const string CSS_KEY_LINEHEIGHT = "line-height";

        /** the CSS tag for the margin of an object */
        public const string CSS_KEY_MARGIN = "margin";

        /** the CSS tag for the margin of an object */
        public const string CSS_KEY_MARGINLEFT = "margin-left";

        /** the CSS tag for the margin of an object */
        public const string CSS_KEY_MARGINRIGHT = "margin-right";

        /** the CSS tag for the margin of an object */
        public const string CSS_KEY_MARGINTOP = "margin-top";

        /** the CSS tag for the margin of an object */
        public const string CSS_KEY_MARGINBOTTOM = "margin-bottom";

        /** the CSS tag for the margin of an object */
        public const String CSS_KEY_PADDING = "padding";

        /** the CSS tag for the margin of an object */
        public const String CSS_KEY_PADDINGLEFT = "padding-left";

        /** the CSS tag for the margin of an object */
        public const String CSS_KEY_PADDINGRIGHT = "padding-right";

        /** the CSS tag for the margin of an object */
        public const String CSS_KEY_PADDINGTOP = "padding-top";

        /** the CSS tag for the margin of an object */
        public const String CSS_KEY_PADDINGBOTTOM = "padding-bottom";

        /** the CSS tag for the margin of an object */
        public const String CSS_KEY_BORDERCOLOR = "border-color";

        /** the CSS tag for the margin of an object */
        public const String CSS_KEY_BORDERWIDTH = "border-width";

        /** the CSS tag for the margin of an object */
        public const String CSS_KEY_BORDERWIDTHLEFT = "border-left-width";

        /** the CSS tag for the margin of an object */
        public const String CSS_KEY_BORDERWIDTHRIGHT = "border-right-width";

        /** the CSS tag for the margin of an object */
        public const String CSS_KEY_BORDERWIDTHTOP = "border-top-width";

        /** the CSS tag for the margin of an object */
        public const String CSS_KEY_BORDERWIDTHBOTTOM = "border-bottom-width";

        /** the CSS tag for adding a page break when the document is printed */
        public const string CSS_KEY_PAGE_BREAK_AFTER = "page-break-after";

        /** the CSS tag for adding a page break when the document is printed */
        public const string CSS_KEY_PAGE_BREAK_BEFORE = "page-break-before";

        /** the CSS tag for the horizontal alignment of an object */
        public const string CSS_KEY_TEXTALIGN = "text-align";

        /** the CSS tag for text decorations */
        public const string CSS_KEY_TEXTDECORATION = "text-decoration";

        /** the CSS tag for text decorations */
        public const string CSS_KEY_VERTICALALIGN = "vertical-align";

        /** the CSS tag for the visibility of objects */
        public const string CSS_KEY_VISIBILITY = "visibility";

        // CSS values

        /** value for the CSS tag for adding a page break when the document is printed */
        public const string CSS_VALUE_ALWAYS = "always";

        /** A possible value for the DISPLAY key */
        public const string CSS_VALUE_BLOCK = "block";

        /** a CSS value for text font weight */
        public const string CSS_VALUE_BOLD = "bold";

        /** the value if you want to hide objects. */
        public const string CSS_VALUE_HIDDEN = "hidden";

        /** A possible value for the DISPLAY key */
        public const string CSS_VALUE_INLINE = "inline";
        
        /** a CSS value for text font style */
        public const string CSS_VALUE_ITALIC = "italic";

        /** a CSS value for text decoration */
        public const string CSS_VALUE_LINETHROUGH = "line-through";

        /** A possible value for the DISPLAY key */
        public const string CSS_VALUE_LISTITEM = "list-item";
        
        /** a CSS value */
        public const string CSS_VALUE_NONE = "none";

        /** a CSS value */
        public const string CSS_VALUE_NORMAL = "normal";

        /** a CSS value for text font style */
        public const string CSS_VALUE_OBLIQUE = "oblique";

        /** A possible value for the DISPLAY key */
        public const string CSS_VALUE_TABLE = "table";

        /** A possible value for the DISPLAY key */
        public const string CSS_VALUE_TABLEROW = "table-row";

        /** A possible value for the DISPLAY key */
        public const string CSS_VALUE_TABLECELL = "table-cell";

        /** the CSS value for a horizontal alignment of an object */
        public const string CSS_VALUE_TEXTALIGNLEFT = "left";

        /** the CSS value for a horizontal alignment of an object */
        public const string CSS_VALUE_TEXTALIGNRIGHT = "right";

        /** the CSS value for a horizontal alignment of an object */
        public const string CSS_VALUE_TEXTALIGNCENTER = "center";

        /** the CSS value for a horizontal alignment of an object */
        public const string CSS_VALUE_TEXTALIGNJUSTIFY = "justify";

        /** a CSS value for text decoration */
        public const string CSS_VALUE_UNDERLINE = "underline";

        /// <summary>
        /// Parses a length.
        /// </summary>
        /// <param name="str">a length in the form of an optional + or -, followed by a number and a unit.</param>
        /// <returns>a float</returns>
        public static float ParseLength(string str) {
            int pos = 0;
            int length = str.Length;
            bool ok = true;
            while (ok && pos < length) {
                switch (str[pos]) {
                    case '+':
                    case '-':
                    case '0':
                    case '1':
                    case '2':
                    case '3':
                    case '4':
                    case '5':
                    case '6':
                    case '7':
                    case '8':
                    case '9':
                    case '.':
                        pos++;
                        break;
                    default:
                        ok = false;
                        break;
                }
            }
            if (pos == 0) return 0f;
            if (pos == length) return float.Parse(str, System.Globalization.NumberFormatInfo.InvariantInfo);
            float f = float.Parse(str.Substring(0, pos), System.Globalization.NumberFormatInfo.InvariantInfo);
            str = str.Substring(pos);
            // inches
            if (str.StartsWith("in")) {
                return f * 72f;
            }
            // centimeters
            if (str.StartsWith("cm")) {
                return (f / 2.54f) * 72f;
            }
            // millimeters
            if (str.StartsWith("mm")) {
                return (f / 25.4f) * 72f;
            }
            // picas
            if (str.StartsWith("pc")) {
                return f * 12f;
            }
            // default: we assume the length was measured in points
            return f;
        }
    
        /// <summary>
        /// Converts a <CODE>Color</CODE> into a HTML representation of this <CODE>Color</CODE>.
        /// </summary>
        /// <param name="color">the <CODE>Color</CODE> that has to be converted.</param>
        /// <returns>the HTML representation of this <CODE>Color</CODE></returns>
        public static Color DecodeColor(String s) {
            if (s == null)
                return null;
            s = s.ToLower(CultureInfo.InvariantCulture).Trim();
            Color c = (Color)WebColors.GetRGBColor(s);
            if (c != null)
                return c;
            try {
                if (s.StartsWith("#")) {
                    if (s.Length == 4)
                        s = "#" + s.Substring(1, 1) + s.Substring(1, 1)
                            + s.Substring(2, 1) + s.Substring(2, 1) 
                            + s.Substring(3, 1) + s.Substring(3, 1);
                    if (s.Length == 7)
                        return new Color(int.Parse(s.Substring(1), NumberStyles.HexNumber));
                }
                else if (s.StartsWith("rgb")) {
                    StringTokenizer tk = new StringTokenizer(s.Substring(3), " \t\r\n\f(),");
                    int[] cc = new int [3];
                    for (int k = 0; k < 3; ++k) {
                        if (!tk.HasMoreTokens())
                            return null;
                        String t = tk.NextToken();
                        float n;
                        if (t.EndsWith("%")) {
                            n = float.Parse(t.Substring(0, t.Length - 1), System.Globalization.NumberFormatInfo.InvariantInfo);
                            n = n * 255f / 100f;
                        }
                        else
                            n = float.Parse(t, System.Globalization.NumberFormatInfo.InvariantInfo);
                        int ni = (int)n;
                        if (ni > 255)
                            ni = 255;
                        else if (ni < 0)
                            ni = 0;
                        cc[k] = ni;
                    }
                    return new Color(cc[0], cc[1], cc[2]);
                }
            }
            catch {
            }
            return null;
        }

        /// <summary>
        /// This method parses a string with attributes and returns a Properties object.
        /// </summary>
        /// <param name="str">a string of this form: 'key1="value1"; key2="value2";... keyN="valueN" '</param>
        /// <returns>a Properties object</returns>
        public static Properties ParseAttributes(string str) {
            Properties result = new Properties();
            if (str == null) return result;
            StringTokenizer keyValuePairs = new StringTokenizer(str, ";");
            StringTokenizer keyValuePair;
            string key;
            string value;
            while (keyValuePairs.HasMoreTokens()) {
                keyValuePair = new StringTokenizer(keyValuePairs.NextToken(), ":");
                if (keyValuePair.HasMoreTokens()) key = keyValuePair.NextToken().Trim().Trim();
                else continue;
                if (keyValuePair.HasMoreTokens()) value = keyValuePair.NextToken().Trim();
                else continue;
                if (value.StartsWith("\"")) value = value.Substring(1);
                if (value.EndsWith("\"")) value = value.Substring(0, value.Length - 1);
                result.Add(key.ToLower(CultureInfo.InvariantCulture), value);
            }
            return result;
        }

        /**
        * Removes the comments sections of a String.
        * 
        * @param string
        *            the original String
        * @param startComment
        *            the String that marks the start of a Comment section
        * @param endComment
        *            the String that marks the end of a Comment section.
        * @return the String stripped of its comment section
        */
        public static string RemoveComment(String str, String startComment,
                String endComment) {
            StringBuilder result = new StringBuilder();
            int pos = 0;
            int end = endComment.Length;
            int start = str.IndexOf(startComment, pos);
            while (start > -1) {
                result.Append(str.Substring(pos, start - pos));
                pos = str.IndexOf(endComment, start) + end;
                start = str.IndexOf(startComment, pos);
            }
            result.Append(str.Substring(pos));
            return result.ToString();
        }
    }
}
