using System;
using System.Collections;
using iTextSharp.text;
using iTextSharp.text.rtf.parser.ctrlwords;

/* $Id: RtfProperty.cs,v 1.3 2008/05/13 11:26:03 psoares33 Exp $
 * 
 *
 * Copyright 2007 by Howard Shank (hgshank@yahoo.com)
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
 * the Initial Developer are Copyright (C) 1999-2006 by Bruno Lowagie.
 * All Rights Reserved.
 * Co-Developer of the code is Paulo Soares. Portions created by the Co-Developer
 * are Copyright (C) 2000-2006 by Paulo Soares. All Rights Reserved.
 *
 * Contributor(s): all the names of the contributors are added in the source code
 * where applicable.
 *
 * Alternatively, the contents of this file may be used under the terms of the
 * LGPL license (the ?GNU LIBRARY GENERAL PUBLIC LICENSE?), in which case the
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
namespace iTextSharp.text.rtf.parser.properties {

    /**
    * <code>RtfProperty</code> handles document, paragraph, etc. property values
    * 
    * @author Howard Shank (hgshank@yahoo.com)
    * @since 2.0.8
    */
    public class RtfProperty {
        public const int OFF = 0;
        public const int ON = 1;
        
        /* property groups */
        public const String COLOR = "color.";
        public const String CHARACTER = "character.";
        public const String PARAGRAPH = "paragraph.";
        public const String SECTION = "section.";
        public const String DOCUMENT = "document.";
        
        /* color properties */
        public const String COLOR_FG = COLOR + "fg"; //Color Object
        public const String COLOR_BG = COLOR + "bg"; //Color Object
        
        /* character properties */
        public const String CHARACTER_BOLD = CHARACTER + "bold"; 
        public const String CHARACTER_UNDERLINE = CHARACTER + "underline";
        public const String CHARACTER_ITALIC = CHARACTER + "italic";
        public const String CHARACTER_SIZE = CHARACTER + "size"; 
        public const String CHARACTER_FONT = CHARACTER + "font";
        public const String CHARACTER_STYLE = CHARACTER + "style";

        /* paragraph properties */
        /** Justify left */
        public const int JUSTIFY_LEFT = 0;
        /** Justify right */
        public const int JUSTIFY_RIGHT = 1;
        /** Justify center */
        public const int JUSTIFY_CENTER = 2;
        /** Justify full */
        public const int JUSTIFY_FULL = 3;
        
        public const String PARAGRAPH_INDENT_LEFT = PARAGRAPH + "indentLeft";    //  twips
        public const String PARAGRAPH_INDENT_RIGHT = PARAGRAPH + "indentRight";  // twips
        public const String PARAGRAPH_INDENT_FIRST_LINE = PARAGRAPH + "indentFirstLine"; // twips
        public const String PARAGRAPH_JUSTIFICATION = PARAGRAPH + "justification";
        
        public const String PARAGRAPH_BORDER = PARAGRAPH + "border";
        public const String PARAGRAPH_BORDER_CELL = PARAGRAPH + "borderCell";
        
        /** possible border settting */
        public const int PARAGRAPH_BORDER_NIL = 0;
        /** possible border settting */
        public const int PARAGRAPH_BORDER_BOTTOM = 1;
        /** possible border settting */
        public const int PARAGRAPH_BORDER_TOP = 2;
        /** possible border settting */
        public const int PARAGRAPH_BORDER_LEFT = 4;
        /** possible border settting */
        public const int PARAGRAPH_BORDER_RIGHT = 8;
        /** possible border settting */
        public const int PARAGRAPH_BORDER_DIAGONAL_UL_LR = 16;
        /** possible border settting */
        public const int PARAGRAPH_BORDER_DIAGONAL_UR_LL = 32;
        /** possible border settting */
        public const int PARAGRAPH_BORDER_TABLE_HORIZONTAL = 64;
        /** possible border settting */
        public const int PARAGRAPH_BORDER_TABLE_VERTICAL = 128;
        
        /* section properties */
        /** Decimal number format */
        public const int PGN_DECIMAL = 0; 
        /** Uppercase Roman Numeral */
        public const int PGN_ROMAN_NUMERAL_UPPERCASE = 1;
        /** Lowercase Roman Numeral */
        public const int PGN_ROMAN_NUMERAL_LOWERCASE = 2;
        /** Uppercase Letter */
        public const int PGN_LETTER_UPPERCASE = 3;
        /** Lowercase Letter */
        public const int PGN_LETTER_LOWERCASE = 4;
        /** Section Break None */
        public const int SBK_NONE = 0;
        /** Section Break Column break */
        public const int SBK_COLUMN = 1;
        /** Section Break Even page break */
        public const int SBK_EVEN = 2;
        /** Section Break Odd page break */
        public const int SBK_ODD = 3;
        /** Section Break Page break */
        public const int SBK_PAGE = 4;
        
        public const String SECTION_NUMBER_OF_COLUMNS =  SECTION + "numberOfColumns";
        public const String SECTION_BREAK_TYPE = SECTION + "SectionBreakType";
        public const String SECTION_PAGE_NUMBER_POSITION_X = SECTION + "pageNumberPositionX";
        public const String SECTION_PAGE_NUMBER_POSITION_Y = SECTION + "pageNumberPositionY";
        public const String SECTION_PAGE_NUMBER_FORMAT = SECTION + "pageNumberFormat";
        
        /* document properties */
        /** Portrait orientation */
        public const String PAGE_PORTRAIT = "0";
        /** Landscape orientation */
        public const String PAGE_LANDSCAPE = "1";
        
        public const String DOCUMENT_PAGE_WIDTH_TWIPS = DOCUMENT + "pageWidthTwips";
        public const String DOCUMENT_PAGE_HEIGHT_TWIPS = DOCUMENT + "pageHeightTwips";
        public const String DOCUMENT_MARGIN_LEFT_TWIPS = DOCUMENT + "marginLeftTwips";
        public const String DOCUMENT_MARGIN_TOP_TWIPS = DOCUMENT + "marginTopTwips";
        public const String DOCUMENT_MARGIN_RIGHT_TWIPS = DOCUMENT + "marginRightTwips";
        public const String DOCUMENT_MARGIN_BOTTOM_TWIPS = DOCUMENT + "marginBottomTwips";
        public const String DOCUMENT_PAGE_NUMBER_START = DOCUMENT + "pageNumberStart";
        public const String DOCUMENT_ENABLE_FACING_PAGES = DOCUMENT + "enableFacingPages";
        public const String DOCUMENT_PAGE_ORIENTATION = DOCUMENT + "pageOrientation";
        public const String DOCUMENT_DEFAULT_FONT_NUMER = DOCUMENT + "defaultFontNumber";
        
        /** Properties for this RtfProperty object */
        protected Hashtable properties = new Hashtable();
        
        private bool modifiedCharacter = false; 
        private bool modifiedParagraph = false; 
        private bool modifiedSection = false; 
        private bool modifiedDocument = false; 

        
        /** The <code>RtfPropertyListener</code>. */
        private ArrayList listeners = new ArrayList();
        /**
        * Set all property objects to default values.
        * @since 2.0.8
        */
        public void SetToDefault() {
            SetToDefault(COLOR);
            SetToDefault(CHARACTER);
            SetToDefault(PARAGRAPH);
            SetToDefault(SECTION);
            SetToDefault(DOCUMENT);
        }
        /**
        * Set individual property group to default values.
        * @param propertyGroup <code>String</code> name of the property group to set to default.
        * @since 2.0.8
        */
        public void SetToDefault(String propertyGroup) {
            if (COLOR.Equals(propertyGroup)) {
                SetProperty(COLOR_FG, new Color(0,0,0));
                SetProperty(COLOR_BG, new Color(255,255,255));
                return;
            }
            if (CHARACTER.Equals(propertyGroup)) {
                SetProperty(CHARACTER_BOLD, 0);
                SetProperty(CHARACTER_UNDERLINE, 0);
                SetProperty(CHARACTER_ITALIC, 0);
                SetProperty(CHARACTER_SIZE, 24);// 1/2 pt sizes
                SetProperty(CHARACTER_FONT, 0);
                return;
            }
            if (PARAGRAPH.Equals(propertyGroup)) {
                SetProperty(PARAGRAPH_INDENT_LEFT, 0);
                SetProperty(PARAGRAPH_INDENT_RIGHT, 0);
                SetProperty(PARAGRAPH_INDENT_FIRST_LINE, 0);
                SetProperty(PARAGRAPH_JUSTIFICATION, JUSTIFY_LEFT);
                SetProperty(PARAGRAPH_BORDER, PARAGRAPH_BORDER_NIL);
                SetProperty(PARAGRAPH_BORDER_CELL, PARAGRAPH_BORDER_NIL);
                return;
            }
            if (SECTION.Equals(propertyGroup)) {
                SetProperty(SECTION_NUMBER_OF_COLUMNS, 0);
                SetProperty(SECTION_BREAK_TYPE, SBK_NONE);
                SetProperty(SECTION_PAGE_NUMBER_POSITION_X, 0);
                SetProperty(SECTION_PAGE_NUMBER_POSITION_Y, 0);
                SetProperty(SECTION_PAGE_NUMBER_FORMAT, PGN_DECIMAL);
                return;
            }
            if (DOCUMENT.Equals(propertyGroup)) {
                SetProperty(DOCUMENT_PAGE_WIDTH_TWIPS, 12240);
                SetProperty(DOCUMENT_PAGE_HEIGHT_TWIPS, 15480);
                SetProperty(DOCUMENT_MARGIN_LEFT_TWIPS, 1800);
                SetProperty(DOCUMENT_MARGIN_TOP_TWIPS, 1440);
                SetProperty(DOCUMENT_MARGIN_RIGHT_TWIPS, 1800);
                SetProperty(DOCUMENT_MARGIN_BOTTOM_TWIPS, 1440);
                SetProperty(DOCUMENT_PAGE_NUMBER_START, 1);
                SetProperty(DOCUMENT_ENABLE_FACING_PAGES, 1);
                SetProperty(DOCUMENT_PAGE_ORIENTATION, PAGE_PORTRAIT);
                SetProperty(DOCUMENT_DEFAULT_FONT_NUMER, 0);    
                return;
            }
        }


        /**
        * Toggle the value of the property identified by the <code>RtfCtrlWordData.specialHandler</code> parameter.
        * Toggle values are assumed to be integer values per the RTF spec with a value of 0=off or 1=on.
        * 
        * @param ctrlWordData The property name to set
        * @return <code>true</code> for handled or <code>false</code> if <code>propertyName</code> is <code>null</code> or <i>blank</i>
        */
        public bool ToggleProperty(RtfCtrlWordData ctrlWordData) { //String propertyName) {
            
            String propertyName = ctrlWordData.specialHandler;
            
            if (propertyName == null || propertyName.Length == 0) return false;
            
            Object propertyValue = GetProperty(propertyName);
            if (propertyValue == null) {
                propertyValue = RtfProperty.ON;
            } else {
                if (propertyValue is int) {
                    int value = (int)propertyValue;
                    if (value != 0) {
                        RemoveProperty(propertyName);
                    }
                    return true;
                } else {
                    if (propertyValue is long) {
                        long value = (long)propertyValue;
                        if (value != 0) {
                            RemoveProperty(propertyName);
                        }
                        return true;
                    }
                }
            }
            SetProperty(propertyName, propertyValue);
            return true;
        }
        /**
        * Set the value of the property identified by the parameter.
        * 
        * @param ctrlWordData The controlword with the name to set
        * @return <code>true</code> for handled or <code>false</code> if <code>propertyName</code> or <code>propertyValue</code> is <code>null</code>
        */
        public bool SetProperty(RtfCtrlWordData ctrlWordData) { //String propertyName, Object propertyValueNew) {
            String propertyName = ctrlWordData.specialHandler;
            Object propertyValueNew = ctrlWordData.param;
            // depending on the control word, set mulitiple or reset settings, etc.
            //if pard then reset settings
            //
            SetProperty(propertyName, propertyValueNew);
            return true;
        }
        /**
        * Set the value of the property identified by the parameter.
        * 
        * @param propertyName The property name to set
        * @param propertyValueNew The object to set the property value to
        * @return <code>true</code> for handled or <code>false</code> if <code>propertyName</code> or <code>propertyValue</code> is <code>null</code>
        */
        private bool SetProperty(String propertyName, Object propertyValueNew) {
            if (propertyName == null || propertyValueNew == null) return false;
            
            Object propertyValueOld = GetProperty(propertyName);
            if (propertyValueOld is int && propertyValueNew is int) {
                int valueOld = (int)propertyValueOld;
                int valueNew = (int)propertyValueNew;
                if (valueOld==valueNew) return true;
            } else {
                if (propertyValueOld is long && propertyValueNew is long) {
                    long valueOld = (long)propertyValueOld;
                    long valueNew = (long)propertyValueNew;
                    if (valueOld==valueNew) return true;
                }
            }
            BeforeChange(propertyName);
            properties[propertyName] = propertyValueNew;
            AfterChange(propertyName);
            SetModified(propertyName, true);
            return true;
        }
        /**
        * Set the value of the property identified by the parameter.
        * 
        * @param propertyName The property name to set
        * @param propertyValue The object to set the property value to
        * @return <code>true</code> for handled or <code>false</code> if <code>propertyName</code> is <code>null</code>
        */
        private bool SetProperty(String propertyName, int propertyValueNew) {
            if (propertyName == null) return false;
            Object propertyValueOld = GetProperty(propertyName);
            if (propertyValueOld is int) {
                int valueOld = (int)propertyValueOld;
                if (valueOld==propertyValueNew) return true;
            } 
            BeforeChange(propertyName);
            properties[propertyName] = propertyValueNew;
            AfterChange(propertyName);
            SetModified(propertyName, true);
            return true;
        }
        /**
        * Add the value of the property identified by the parameter.
        * 
        * @param propertyName The property name to set
        * @param propertyValue The object to set the property value to
        * @return <code>true</code> for handled or <code>false</code> if <code>propertyName</code> is <code>null</code>
        */
        private bool AddToProperty(String propertyName, int propertyValue) {
            if (propertyName == null) return false;
            int value = (int)properties[propertyName];
            if ((value | propertyValue) == value) return true;
            value |= propertyValue;
            BeforeChange(propertyName);
            properties[propertyName] = value;
            AfterChange(propertyName);
            SetModified(propertyName, true);
            return true;
        }
        /**
        * Set the value of the property identified by the parameter.
        * 
        * @param propertyName The property name to set
        * @param propertyValue The object to set the property value to
        * @return <code>true</code> for handled or <code>false</code> if <code>propertyName</code> is <code>null</code>
        */
        private bool SetProperty(String propertyName, long propertyValueNew) {
            if (propertyName == null) return false;
            Object propertyValueOld = GetProperty(propertyName);
            if (propertyValueOld is long) {
                long valueOld = (long)propertyValueOld;
                if (valueOld==propertyValueNew) return true;
            } 
            BeforeChange(propertyName);
            properties[propertyName] = propertyValueNew;
            AfterChange(propertyName);
            SetModified(propertyName, true);
            return true;
        }
        /**
        * Add the value of the property identified by the parameter.
        * 
        * @param propertyName The property name to set
        * @param propertyValue The object to set the property value to
        * @return <code>true</code> for handled or <code>false</code> if <code>propertyName</code> is <code>null</code>
        */
        private bool AddToProperty(String propertyName, long propertyValue) {
            if (propertyName == null) return false;
            long value = (long)properties[propertyName];
            if ((value | propertyValue) == value) return true;
            value |= propertyValue;
            BeforeChange(propertyName);
            properties[propertyName] = value;
            AfterChange(propertyName);
            SetModified(propertyName, true);
            return true;
        }
        private bool RemoveProperty(String propertyName) {
            if (propertyName == null) return false;
            if (properties.ContainsKey(propertyName)) {
                BeforeChange(propertyName);
                properties.Remove(propertyName);
                AfterChange(propertyName);
                SetModified(propertyName, true);
            }
            return true;
        }
        /**
        * Get the value of the property identified by the parameter.
        * 
        * @param propertyName String containing the property name to get
        * @return Property Object requested or null if not found in map.
        */
        public Object GetProperty(String propertyName) {
            return properties[propertyName];
        }
        /**
        * Get a group of properties.
        * 
        * @param propertyGroup The group name to obtain.
        * @return Properties object with requested values.
        */
        public Hashtable GetProperties(String propertyGroup) {
            Hashtable props = new Hashtable();
            if (properties.Count != 0) {
                //properties.get
                foreach (String key in properties.Keys) {
                    if (key.StartsWith(propertyGroup)) {
                        props[key] = properties[key];
                    }
                }
            }
            return props;
        }
        
        /**
        * @return the modified
        */
        public bool IsModified() {
            return modifiedCharacter || modifiedParagraph || modifiedSection || modifiedDocument;
        }
        /**
        * @param propertyName the propertyName that is modified
        * @param modified the modified to set
        */
        public void SetModified(String propertyName, bool modified) {
            if (propertyName.StartsWith(CHARACTER)) {
                this.SetModifiedCharacter(modified);
            } else {
                if (propertyName.StartsWith(PARAGRAPH)) {
                    this.SetModifiedParagraph(modified);
                } else {
                    if (propertyName.StartsWith(SECTION)) {
                        this.SetModifiedSection(modified);
                    } else {
                        if (propertyName.StartsWith(DOCUMENT)) {
                            this.SetModifiedDocument(modified);
                        }
                    }
                }
            }
        }
        /**
        * @return the modifiedCharacter
        */
        public bool IsModifiedCharacter() {
            return modifiedCharacter;
        }
        /**
        * @param modifiedCharacter the modifiedCharacter to set
        */
        public void SetModifiedCharacter(bool modifiedCharacter) {
            this.modifiedCharacter = modifiedCharacter;
        }
        /**
        * @return the modifiedParagraph
        */
        public bool IsModifiedParagraph() {
            return modifiedParagraph;
        }
        /**
        * @param modifiedParagraph the modifiedParagraph to set
        */
        public void SetModifiedParagraph(bool modifiedParagraph) {
            this.modifiedParagraph = modifiedParagraph;
        }
        /**
        * @return the modifiedSection
        */
        public bool IsModifiedSection() {
            return modifiedSection;
        }
        /**
        * @param modifiedSection the modifiedSection to set
        */
        public void SetModifiedSection(bool modifiedSection) {
            this.modifiedSection = modifiedSection;
        }
        /**
        * @return the modifiedDocument
        */
        public bool IsModifiedDocument() {
            return modifiedDocument;
        }
        /**
        * @param modifiedDocument the modifiedDocument to set
        */
        public void SetModifiedDocument(bool modifiedDocument) {
            this.modifiedDocument = modifiedDocument;
        }
        
        /**
        * Adds a <CODE>RtfPropertyListener</CODE> to the <CODE>RtfProperty</CODE>.
        *
        * @param listener
        *            the new RtfPropertyListener.
        */
        public void AddRtfPropertyListener(IRtfPropertyListener listener) {
            listeners.Add(listener);
        }
        /**
        * Removes a <CODE>RtfPropertyListener</CODE> from the <CODE>RtfProperty</CODE>.
        *
        * @param listener
        *            the new RtfPropertyListener.
        */
        public void RemoveRtfPropertyListener(IRtfPropertyListener listener) {
            listeners.Remove(listener);
        }
        
        public void BeforeChange(String propertyName) {
            // call listener for all
            foreach (IRtfPropertyListener listener in listeners) {
                listener.BeforePropertyChange(propertyName);
            }
            
            if (propertyName.StartsWith(CHARACTER)) {
                // call listener for character chane
            } else {
                if (propertyName.StartsWith(PARAGRAPH)) {
                    // call listener for paragraph change
                } else {
                    if (propertyName.StartsWith(SECTION)) {
                        // call listener for section change
                    } else {
                        if (propertyName.StartsWith(DOCUMENT)) {
                            // call listener for document change
                        }
                    }
                }
            }
        }
        
        public void AfterChange(String propertyName) {
            // call listener for all
            foreach (IRtfPropertyListener listener in listeners) {
                listener.AfterPropertyChange(propertyName);
            }

            if (propertyName.StartsWith(CHARACTER)) {
                // call listener for character chane
            } else {
                if (propertyName.StartsWith(PARAGRAPH)) {
                    // call listener for paragraph change
                } else {
                    if (propertyName.StartsWith(SECTION)) {
                        // call listener for section change
                    } else {
                        if (propertyName.StartsWith(DOCUMENT)) {
                            // call listener for document change
                        }
                    }
                }
            }
        }
    }
}