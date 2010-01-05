using System;
using System.Collections;
using System.util;
using iTextSharp.text;
using iTextSharp.text.html;
using iTextSharp.text.factories;
/*
 * $Id: ElementFactory.cs,v 1.9 2008/05/13 11:25:14 psoares33 Exp $
 * 
 *
 * Copyright 2007 by Bruno Lowagie.
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
namespace iTextSharp.text.factories {

    /**
    * This class is able to create Element objects based on a list of properties.
    */

    public class ElementFactory {

        public static Chunk GetChunk(Properties attributes) {
            Chunk chunk = new Chunk();
            
            chunk.Font = FontFactory.GetFont(attributes);
            String value;
            
            value = attributes[ElementTags.ITEXT];
            if (value != null) {
                chunk.Append(value);
            }
            value = attributes[ElementTags.LOCALGOTO];
            if (value != null) {
                chunk.SetLocalGoto(value);
            }
            value = attributes[ElementTags.REMOTEGOTO];
            if (value != null) {
                String page = attributes[ElementTags.PAGE];
                if (page != null) {
                    chunk.SetRemoteGoto(value, int.Parse(page));
                }
                else {
                    String destination = attributes[ElementTags.DESTINATION];
                    if (destination != null) {
                        chunk.SetRemoteGoto(value, destination);
                    }
                }
            }
            value = attributes[ElementTags.LOCALDESTINATION];
            if (value != null) {
                chunk.SetLocalDestination(value);
            }
            value = attributes[ElementTags.SUBSUPSCRIPT];
            if (value != null) {
                chunk.SetTextRise(float.Parse(value, System.Globalization.NumberFormatInfo.InvariantInfo));
            }
            value = attributes[Markup.CSS_KEY_VERTICALALIGN];
            if (value != null && value.EndsWith("%")) {
                float p = float.Parse(value.Substring(0, value.Length - 1), System.Globalization.NumberFormatInfo.InvariantInfo) / 100f;
                chunk.SetTextRise(p * chunk.Font.Size);
            }
            value = attributes[ElementTags.GENERICTAG];
            if (value != null) {
                chunk.SetGenericTag(value);
            }
            value = attributes[ElementTags.BACKGROUNDCOLOR];
            if (value != null) {
                chunk.SetBackground(Markup.DecodeColor(value));
            }
            return chunk;
        }
        
        public static Phrase GetPhrase(Properties attributes) {
            Phrase phrase = new Phrase();
            phrase.Font = FontFactory.GetFont(attributes);
            String value;
            value = attributes[ElementTags.LEADING];
            if (value != null) {
                phrase.Leading = float.Parse(value, System.Globalization.NumberFormatInfo.InvariantInfo);
            }
            value = attributes[Markup.CSS_KEY_LINEHEIGHT];
            if (value != null) {
                phrase.Leading = Markup.ParseLength(value);
            }
            value = attributes[ElementTags.ITEXT];
            if (value != null) {
                Chunk chunk = new Chunk(value);
                if ((value = attributes[ElementTags.GENERICTAG]) != null) {
                    chunk.SetGenericTag(value);
                }
                phrase.Add(chunk);
            }
            return phrase;
        }
        
        public static Anchor GetAnchor(Properties attributes) {
            Anchor anchor = new Anchor(GetPhrase(attributes));
            String value;
            value = attributes[ElementTags.NAME];
            if (value != null) {
                anchor.Name = value;
            }
            value = (String)attributes.Remove(ElementTags.REFERENCE);
            if (value != null) {
                anchor.Reference = value;
            }
            return anchor;
        }
        
        public static Paragraph GetParagraph(Properties attributes) {
            Paragraph paragraph = new Paragraph(GetPhrase(attributes));
            String value;
            value = attributes[ElementTags.ALIGN];
            if (value != null) {
                paragraph.SetAlignment(value);
            }
            value = attributes[ElementTags.INDENTATIONLEFT];
            if (value != null) {
                paragraph.IndentationLeft = float.Parse(value, System.Globalization.NumberFormatInfo.InvariantInfo);
            }
            value = attributes[ElementTags.INDENTATIONRIGHT];
            if (value != null) {
                paragraph.IndentationRight = float.Parse(value, System.Globalization.NumberFormatInfo.InvariantInfo);
            }
            return paragraph;
        }
        
        public static ListItem GetListItem(Properties attributes) {
            ListItem item = new ListItem(GetParagraph(attributes));
            return item;
        }
        
        public static List GetList(Properties attributes) {
            List list = new List();

            list.Numbered = Utilities.CheckTrueOrFalse(attributes, ElementTags.NUMBERED);
            list.Lettered = Utilities.CheckTrueOrFalse(attributes, ElementTags.LETTERED);
            list.Lowercase = Utilities.CheckTrueOrFalse(attributes, ElementTags.LOWERCASE);
            list.Autoindent = Utilities.CheckTrueOrFalse(attributes, ElementTags.AUTO_INDENT_ITEMS);
            list.Alignindent = Utilities.CheckTrueOrFalse(attributes, ElementTags.ALIGN_INDENTATION_ITEMS);
            
            String value;
            
            value = attributes[ElementTags.FIRST];
            if (value != null) {
                char character = value[0];
                if (char.IsLetter(character) ) {
                    list.First = (int)character;
                }
                else {
                    list.First = int.Parse(value);
                }
            }
            
            value = attributes[ElementTags.LISTSYMBOL];
            if (value != null) {
                list.ListSymbol = new Chunk(value, FontFactory.GetFont(attributes));
            }
            
            value = attributes[ElementTags.INDENTATIONLEFT];
            if (value != null) {
                list.IndentationLeft = float.Parse(value, System.Globalization.NumberFormatInfo.InvariantInfo);
            }
            
            value = attributes[ElementTags.INDENTATIONRIGHT];
            if (value != null) {
                list.IndentationRight = float.Parse(value, System.Globalization.NumberFormatInfo.InvariantInfo);
            }
            
            value = attributes[ElementTags.SYMBOLINDENT];
            if (value != null) {
                list.SymbolIndent = float.Parse(value, System.Globalization.NumberFormatInfo.InvariantInfo);
            }
            
            return list;
        }

        public static Cell GetCell(Properties attributes) {
            Cell cell = new Cell();
            String value;

            cell.SetHorizontalAlignment(attributes[ElementTags.HORIZONTALALIGN]);
            cell.SetVerticalAlignment(attributes[ElementTags.VERTICALALIGN]);
            value = attributes[ElementTags.WIDTH];
            if (value != null) {
                cell.SetWidth(value);
            }
            value = attributes[ElementTags.COLSPAN];
            if (value != null) {
                cell.Colspan = int.Parse(value);
            }
            value = attributes[ElementTags.ROWSPAN];
            if (value != null) {
                cell.Rowspan = int.Parse(value);
            }
            value = attributes[ElementTags.LEADING];
            if (value != null) {
                cell.Leading = float.Parse(value, System.Globalization.NumberFormatInfo.InvariantInfo);
            }
            cell.Header = Utilities.CheckTrueOrFalse(attributes, ElementTags.HEADER);
            if (Utilities.CheckTrueOrFalse(attributes, ElementTags.NOWRAP)) {
                cell.MaxLines = 1;
            }
            SetRectangleProperties(cell, attributes);
            return cell;
        }

        /**
        * Creates an Table object based on a list of properties.
        * @param attributes
        * @return a Table
        */
        public static Table GetTable(Properties attributes) {
            String value;
            Table table;

            value = attributes[ElementTags.WIDTHS];
            if (value != null) {
                StringTokenizer widthTokens = new StringTokenizer(value, ";");
                ArrayList values = new ArrayList();
                while (widthTokens.HasMoreTokens()) {
                    values.Add(widthTokens.NextToken());
                }
                table = new Table(values.Count);
                float[] widths = new float[table.Columns];
                for (int i = 0; i < values.Count; i++) {
                    value = (String)values[i];
                    widths[i] = float.Parse(value, System.Globalization.NumberFormatInfo.InvariantInfo);
                }
                table.Widths = widths;
            }
            else {
                value = attributes[ElementTags.COLUMNS];
                try {
                    table = new Table(int.Parse(value));
                }
                catch {
                    table = new Table(1);
                }
            }
            
            table.Border = Table.BOX;
            table.BorderWidth = 1;
            table.DefaultCell.Border = Table.BOX;
            
            value = attributes[ElementTags.LASTHEADERROW];
            if (value != null) {
                table.LastHeaderRow = int.Parse(value);
            }
            value = attributes[ElementTags.ALIGN];
            if (value != null) {
                table.SetAlignment(value);
            }
            value = attributes[ElementTags.CELLSPACING];
            if (value != null) {
                table.Spacing = float.Parse(value, System.Globalization.NumberFormatInfo.InvariantInfo);
            }
            value = attributes[ElementTags.CELLPADDING];
            if (value != null) {
                table.Padding = float.Parse(value, System.Globalization.NumberFormatInfo.InvariantInfo);
            }
            value = attributes[ElementTags.OFFSET];
            if (value != null) {
                table.Offset = float.Parse(value, System.Globalization.NumberFormatInfo.InvariantInfo);
            }
            value = attributes[ElementTags.WIDTH];
            if (value != null) {
                if (value.EndsWith("%"))
                    table.Width = float.Parse(value.Substring(0, value.Length - 1), System.Globalization.NumberFormatInfo.InvariantInfo);
                else {
                    table.Width = float.Parse(value, System.Globalization.NumberFormatInfo.InvariantInfo);
                    table.Locked = true;
                }
            }
            table.TableFitsPage = Utilities.CheckTrueOrFalse(attributes, ElementTags.TABLEFITSPAGE);
            table.CellsFitPage = Utilities.CheckTrueOrFalse(attributes, ElementTags.CELLSFITPAGE);
            table.Convert2pdfptable = Utilities.CheckTrueOrFalse(attributes, ElementTags.CONVERT2PDFP);
            
            SetRectangleProperties(table, attributes);
            return table;
        }

        /**
        * Sets some Rectangle properties (for a Cell, Table,...).
        */
        private static void SetRectangleProperties(Rectangle rect, Properties attributes) {
            String value;
            value = attributes[ElementTags.BORDERWIDTH];
            if (value != null) {
                rect.BorderWidth = float.Parse(value, System.Globalization.NumberFormatInfo.InvariantInfo);
            }
            int border = 0;
            if (Utilities.CheckTrueOrFalse(attributes, ElementTags.LEFT)) {
                border |= Rectangle.LEFT_BORDER;
            }
            if (Utilities.CheckTrueOrFalse(attributes, ElementTags.RIGHT)) {
                border |= Rectangle.RIGHT_BORDER;
            }
            if (Utilities.CheckTrueOrFalse(attributes, ElementTags.TOP)) {
                border |= Rectangle.TOP_BORDER;
            }
            if (Utilities.CheckTrueOrFalse(attributes, ElementTags.BOTTOM)) {
                border |= Rectangle.BOTTOM_BORDER;
            }
            rect.Border = border;
            
            String r = attributes[ElementTags.RED];
            String g = attributes[ElementTags.GREEN];
            String b = attributes[ElementTags.BLUE];
            if (r != null || g != null || b != null) {
                int red = 0;
                int green = 0;
                int blue = 0;
                if (r != null) red = int.Parse(r);
                if (g != null) green = int.Parse(g);
                if (b != null) blue = int.Parse(b);
                rect.BorderColor = new Color(red, green, blue);
            }
            else {
                rect.BorderColor = Markup.DecodeColor(attributes[ElementTags.BORDERCOLOR]);
            }
            r = (String)attributes.Remove(ElementTags.BGRED);
            g = (String)attributes.Remove(ElementTags.BGGREEN);
            b = (String)attributes.Remove(ElementTags.BGBLUE);
            value = attributes[ElementTags.BACKGROUNDCOLOR];
            if (r != null || g != null || b != null) {
                int red = 0;
                int green = 0;
                int blue = 0;
                if (r != null) red = int.Parse(r);
                if (g != null) green = int.Parse(g);
                if (b != null) blue = int.Parse(b);
                rect.BackgroundColor = new Color(red, green, blue);
            }
            else if (value != null) {
                rect.BackgroundColor = Markup.DecodeColor(value);
            }
            else {
                value = attributes[ElementTags.GRAYFILL];
                if (value != null) {
                    rect.GrayFill = float.Parse(value, System.Globalization.NumberFormatInfo.InvariantInfo);
                }
            }
        }
        
        public static ChapterAutoNumber GetChapter(Properties attributes) {
            ChapterAutoNumber chapter = new ChapterAutoNumber("");
            SetSectionParameters(chapter, attributes);
            return chapter;
        }
        
        public static Section GetSection(Section parent, Properties attributes) {
            Section section = parent.AddSection("");
            SetSectionParameters(section, attributes);
            return section;
        }
        
        private static void SetSectionParameters(Section section, Properties attributes) {
            String value;
            value = attributes[ElementTags.NUMBERDEPTH];
            if (value != null) {
                section.NumberDepth = int.Parse(value);
            }
            value = attributes[ElementTags.INDENT];
            if (value != null) {
                section.Indentation = float.Parse(value, System.Globalization.NumberFormatInfo.InvariantInfo);
            }
            value = attributes[ElementTags.INDENTATIONLEFT];
            if (value != null) {
                section.IndentationLeft = float.Parse(value, System.Globalization.NumberFormatInfo.InvariantInfo);
            }
            value = attributes[ElementTags.INDENTATIONRIGHT];
            if (value != null) {
                section.IndentationRight = float.Parse(value, System.Globalization.NumberFormatInfo.InvariantInfo);
            }
        }

        /// <summary>
        /// Returns an Image that has been constructed taking in account
        /// the value of some attributes.
        /// </summary>
        /// <param name="attributes">Some attributes</param>
        /// <returns>an Image</returns>
        public static Image GetImage(Properties attributes) {
            String value;
            
            value = attributes[ElementTags.URL];
            if (value == null)
                throw new ArgumentException("The URL of the image is missing.");
            Image image = Image.GetInstance(value);
            
            value = attributes[ElementTags.ALIGN];
            int align = 0;
            if (value != null) {
                if (Util.EqualsIgnoreCase(ElementTags.ALIGN_LEFT, value))
                    align |= Image.ALIGN_LEFT;
                else if (Util.EqualsIgnoreCase(ElementTags.ALIGN_RIGHT, value))
                    align |= Image.ALIGN_RIGHT;
                else if (Util.EqualsIgnoreCase(ElementTags.ALIGN_MIDDLE, value))
                    align |= Image.ALIGN_MIDDLE;
            }
            if (Util.EqualsIgnoreCase("true", attributes[ElementTags.UNDERLYING]))
                align |= Image.UNDERLYING;
            if (Util.EqualsIgnoreCase("true", attributes[ElementTags.TEXTWRAP]))
                align |= Image.TEXTWRAP;
            image.Alignment = align;
            
            value = attributes[ElementTags.ALT];
            if (value != null) {
                image.Alt = value;
            }
            
            String x = attributes[ElementTags.ABSOLUTEX];
            String y = attributes[ElementTags.ABSOLUTEY];
            if ((x != null) && (y != null)) {
                image.SetAbsolutePosition(float.Parse(x, System.Globalization.NumberFormatInfo.InvariantInfo),
                        float.Parse(y, System.Globalization.NumberFormatInfo.InvariantInfo));
            }
            value = attributes[ElementTags.PLAINWIDTH];
            if (value != null) {
                image.ScaleAbsoluteWidth(float.Parse(value, System.Globalization.NumberFormatInfo.InvariantInfo));
            }
            value = attributes[ElementTags.PLAINHEIGHT];
            if (value != null) {
                image.ScaleAbsoluteHeight(float.Parse(value, System.Globalization.NumberFormatInfo.InvariantInfo));
            }
            value = attributes[ElementTags.ROTATION];
            if (value != null) {
                image.Rotation = float.Parse(value, System.Globalization.NumberFormatInfo.InvariantInfo);
            }
            return image;
        }

        /**
        * Creates an Annotation object based on a list of properties.
        * @param attributes
        * @return an Annotation
        */
        public static Annotation GetAnnotation(Properties attributes) {
            float llx = 0, lly = 0, urx = 0, ury = 0;
            String value;
            
            value = attributes[ElementTags.LLX];
            if (value != null) {
                llx = float.Parse(value, System.Globalization.NumberFormatInfo.InvariantInfo);
            }
            value = attributes[ElementTags.LLY];
            if (value != null) {
                lly = float.Parse(value, System.Globalization.NumberFormatInfo.InvariantInfo);
            }
            value = attributes[ElementTags.URX];
            if (value != null) {
                urx = float.Parse(value, System.Globalization.NumberFormatInfo.InvariantInfo);
            }
            value = attributes[ElementTags.URY];
            if (value != null) {
                ury = float.Parse(value, System.Globalization.NumberFormatInfo.InvariantInfo);
            }
            
            String title = attributes[ElementTags.TITLE];
            String text = attributes[ElementTags.CONTENT];
            if (title != null || text != null) {
                return new Annotation(title, text, llx, lly, urx, ury);
            }
            value = attributes[ElementTags.URL];
            if (value != null) {
                return new Annotation(llx, lly, urx, ury, value);
            }
            value = attributes[ElementTags.NAMED];
            if (value != null) {
                return new Annotation(llx, lly, urx, ury, int.Parse(value));
            }
            String file = attributes[ElementTags.FILE];
            String destination = attributes[ElementTags.DESTINATION];
            String page = (String) attributes.Remove(ElementTags.PAGE);
            if (file != null) {
                if (destination != null) {
                    return new Annotation(llx, lly, urx, ury, file, destination);
                }
                if (page != null) {
                    return new Annotation(llx, lly, urx, ury, file, int.Parse(page));
                }
            }
            if (title == null)
                title = "";
            if (text == null)
                text = "";
            return new Annotation(title, text, llx, lly, urx, ury);
        }
    }
}