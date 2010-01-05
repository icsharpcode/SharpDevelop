using System;
using System.Collections;
using System.Text;
using iTextSharp.text;
using iTextSharp.text.rtf.direct;
using iTextSharp.text.rtf.parser;
using iTextSharp.text.rtf.parser.ctrlwords;
/*
 * $Id: RtfDestinationFontTable.cs,v 1.4 2008/05/13 11:26:00 psoares33 Exp $
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
 
namespace iTextSharp.text.rtf.parser.destinations {

    /**
    * <code>RtfDestinationFontTable</code> handles data destined for the font table destination
    * 
    * @author Howard Shank (hgshank@yahoo.com)
    *
    * @since 2.0.8
    */
    public sealed class RtfDestinationFontTable : RtfDestination {
        /**
        * The RtfImportHeader to add font mappings to.
        */
        private RtfImportMgr importHeader = null;
        /**
        * The theme (Office 2007)
        */
        private String themeFont = "";
        /**
        * The number of the font being parsed.
        */
        private String fontNr = "";
        /**
        * The family of the font being parsed.
        */
        private String fontFamily = "";
        /**
        * The \charset value
        */
        private String charset = "";
        private const String CHARSET_DEFAULT = "0";
        /**
        * The \fprq
        */
        private int fprq = 0;
        /**
        * The \*\panose font matching value if primary font is not available.
        */
        private String panose = "";
        /**
        * The \*\fname
        */
        //private String nontaggedname = "";
        /**
        * The name of the font being parsed.
        */
        private String fontName = "";
        /**
        * The \falt alternate font if primary font is not available.
        */
        private String falt = "";
        /**
        * The \falt alternate font if primary font is not available.
        */
        //private String fontemb = "";
        /**
        * The \falt alternate font if primary font is not available.
        */
        //private String fontType = "";
        /**
        * The \falt alternate font if primary font is not available.
        */
        //private String fontFile = "";
        /**
        * The \falt alternate font if primary font is not available.
        */
        //private String fontFileCpg = "";
        /**
        * The \fbias value
        */
        private int fbias = 0;
        /**
        * The \cpg value
        */
        private String cpg = "";
        /**
        * The \fnil, \fttruetype value
        */
        private String trueType = "";

        /**
        * state flag to handle different parsing of a font element
        */
        private int state = 0;
        /* state values */
        /** Normal   */
        private const int SETTING_NORMAL = 0;
        /** \falt    */
        private const int SETTING_ALTERNATE = 1;
        /** \fname   */
        private const int SETTING_FONTNAME = 2;
        /** \panose      */
        private const int SETTING_PANOSE = 3;
        /** \fontemb    */
        private const int SETTING_FONT_EMBED = 4;
        /** \ffile  */
        private const int SETTING_FONT_FILE = 5;
        
        /**
        * Convert font mapping to <code>FontFactory</code> font objects.
        */
        private Hashtable fontMap = null;
        
        /**
        * Constructor
        */
        public RtfDestinationFontTable() : base(null) {
        }
        /**
        * Constructs a new RtfFontTableParser.
        * 
        * @param importHeader The RtfImportHeader to add font mappings to.
        * 
        * @since 2.0.8
        */
        public RtfDestinationFontTable(RtfParser parser) : base(parser) {
            this.Init(true);
        }
        
        /* (non-Javadoc)
        * @see com.lowagie.text.rtf.parser.destinations.RtfDestination#setParser(com.lowagie.text.rtf.parser.RtfParser)
        * 
        * @since 2.0.8
        */
        public override void SetParser(RtfParser parser) {
            if (this.rtfParser != null && this.rtfParser.Equals(parser)) return;
            this.rtfParser = parser;
            this.Init(true);
        }
        /**
        * Initialize the object.
        * 
        * @param importFonts true to import the fonts into the FontFactory, false do not load fonts
        * 
        * @since 2.0.8
        */
        private void Init(bool importFonts) {
            fontMap = new Hashtable();
            if (this.rtfParser != null) {
                this.importHeader = this.rtfParser.GetImportManager();
            }
            this.SetToDefaults();
            if (importFonts) {
                ImportSystemFonts();
            }
        }
        /* (non-Javadoc)
        * @see com.lowagie.text.rtf.parser.destinations.RtfDestination#handleOpenNewGroup()
        * 
        * @since 2.0.8
        */
        public override bool HandleOpeningSubGroup() {
            return true;
        }
        /* (non-Javadoc)
        * @see com.lowagie.text.rtf.direct.RtfDestination#closeDestination()
        * 
        * @since 2.0.8
        */
        public override bool CloseDestination() {
            return true;
        }

        /* (non-Javadoc)
        * @see com.lowagie.text.rtf.direct.RtfDestination#handleGroupEnd()
        * 
        * @since 2.0.8
        */
        public override bool HandleCloseGroup() {
            if (this.state == SETTING_NORMAL) {
                ProcessFont();
            }
            this.state = SETTING_NORMAL;
            return true;
        }

        /* (non-Javadoc)
        * @see com.lowagie.text.rtf.direct.RtfDestination#handleGroupStart()
        * 
        * @since 2.0.8
        */
        public override bool HandleOpenGroup() {

            return true;
        }
        
        /* (non-Javadoc)
        * @see com.lowagie.text.rtf.direct.RtfDestination#handleCharacter(char[])
        * 
        * @since 2.0.8
        */
        public override bool HandleCharacter(int ch) {
            switch (this.state) {
            case SETTING_NORMAL:
                this.fontName += (char)ch;
                break;
            case SETTING_ALTERNATE:
                this.falt += (char)ch;
                break;
            case SETTING_PANOSE:
                this.panose += (char)ch;
                break;
            case SETTING_FONT_EMBED:
                break;
            case SETTING_FONT_FILE:
                break;
            case SETTING_FONTNAME:
                break;
                
            }
            return true;
        }
        /* (non-Javadoc)
        * @see com.lowagie.text.rtf.parser.destinations.RtfDestination#handleControlWord(com.lowagie.text.rtf.parser.ctrlwords.RtfCtrlWordData)
        * 
        * @since 2.0.8
        */
        public override bool HandleControlWord(RtfCtrlWordData ctrlWordData) {
            bool result = true;
            // just let fonttbl fall through and set last ctrl word object.
            
            if (ctrlWordData.ctrlWord.Equals("f")) { this.SetFontNumber(ctrlWordData.param); result=true;}
            if (ctrlWordData.ctrlWord.Equals("fcharset")) { this.SetCharset(ctrlWordData.param); result=true; }

            // font families
            if (ctrlWordData.ctrlWord.Equals("fnil")) { this.SetFontFamily("roman"); result=true; }
            if (ctrlWordData.ctrlWord.Equals("froman")) { this.SetFontFamily("roman"); result=true; }
            if (ctrlWordData.ctrlWord.Equals("fswiss")) { this.SetFontFamily("swiss"); result=true; }
            if (ctrlWordData.ctrlWord.Equals("fmodern")) { this.SetFontFamily("modern"); result=true; }
            if (ctrlWordData.ctrlWord.Equals("fscript")) { this.SetFontFamily("script"); result=true; }
            if (ctrlWordData.ctrlWord.Equals("fdecor")) { this.SetFontFamily("decor"); result=true; }
            if (ctrlWordData.ctrlWord.Equals("ftech")) { this.SetFontFamily("tech"); result=true; }
            if (ctrlWordData.ctrlWord.Equals("fbidi")) { this.SetFontFamily("bidi"); result=true; }
            // pitch
            if (ctrlWordData.ctrlWord.Equals("fprq")) { this.SetPitch(ctrlWordData.param); result=true; }
            // bias
            if (ctrlWordData.ctrlWord.Equals("fbias")) { this.SetBias(ctrlWordData.param); result=true; }
            // theme font information
            if (ctrlWordData.ctrlWord.Equals("flomajor")) { this.SetThemeFont("flomajor"); result= true; }
            if (ctrlWordData.ctrlWord.Equals("fhimajor")) { this.SetThemeFont("fhimajor"); result= true; }
            if (ctrlWordData.ctrlWord.Equals("fdbmajor")) { this.SetThemeFont("fdbmajor"); result= true; }
            if (ctrlWordData.ctrlWord.Equals("fbimajor")) { this.SetThemeFont("fbimajor"); result= true; }
            if (ctrlWordData.ctrlWord.Equals("flominor")) { this.SetThemeFont("flominor"); result= true; }
            if (ctrlWordData.ctrlWord.Equals("fhiminor")) { this.SetThemeFont("fhiminor"); result= true; }
            if (ctrlWordData.ctrlWord.Equals("fdbminor")) { this.SetThemeFont("fdbminor"); result= true; }
            if (ctrlWordData.ctrlWord.Equals("fbiminor")) { this.SetThemeFont("fbiminor"); result= true; }

            // panose
            if (ctrlWordData.ctrlWord.Equals("panose")) {state = SETTING_PANOSE; result = true; }
            
            // \*\fname
            // <font name> #PCDATA
            if (ctrlWordData.ctrlWord.Equals("fname")) {state = SETTING_FONTNAME; result = true; }

            // \*\falt
            if (ctrlWordData.ctrlWord.Equals("falt")) { state = SETTING_ALTERNATE; result = true; }
            
            // \*\fontemb
            if (ctrlWordData.ctrlWord.Equals("fontemb")) { state = SETTING_FONT_EMBED; result = true; }

            // font type
            if (ctrlWordData.ctrlWord.Equals("ftnil")) { this.SetTrueType("ftnil"); result= true; }
            if (ctrlWordData.ctrlWord.Equals("fttruetype")) { this.SetTrueType("fttruetype"); result= true; }
            
            // \*\fontfile
            if (ctrlWordData.ctrlWord.Equals("fontemb")) { state = SETTING_FONT_FILE; result = true; }

            // codepage
            if (ctrlWordData.ctrlWord.Equals("cpg")) { this.SetCodePage(ctrlWordData.param); result= true; }
            
            this.lastCtrlWord = ctrlWordData;
            return result;
        }
        /**
        * Set the code page
        * @param value The code page value
        * 
        * @since 2.0.8
        */
        public void SetCodePage(String value) {
            this.cpg = value;
        }
        /**
        * Set the TrueTtype type
        * @param value The type
        * 
        * @since 2.0.8
        */
        public void SetTrueType(String value) {
            this.trueType = value;
        }
        /**
        * Set the font pitch
        * @param value Pitch value
        * 
        * @since 2.0.8
        */
        public void SetPitch(String value) {
            this.fprq = int.Parse(value);
        }
        /**
        * Set the font bias
        * @param value Bias value
        * 
        * @since 2.0.8
        */
        public void SetBias(String value) {
            this.fbias = int.Parse(value);
        }
        /**
        * Set the font theme
        * 
        * @param themeFont Theme value
        * 
        * @since 2.0.8
        */
        public void SetThemeFont(String themeFont) {
            this.themeFont = themeFont;
        }
        /**
        * Set the font name to the parsed value.
        * 
        * @param fontName The font name.
        * 
        * @since 2.0.8
        */
        public void SetFontName(String fontName) {
            this.fontName = fontName;
        }
        /**
        * Set the font family to the parsed value.
        * 
        * @param fontFamily The font family.
        * 
        * @since 2.0.8
        */
        public void SetFontFamily(String fontFamily) {
            this.fontFamily = fontFamily;
        }
        /**
        * Set the font number to the parsed value.
        * This is used for mapping fonts to the new font numbers
        * 
        * @param fontNr The font number.
        * 
        * @since 2.0.8
        */
        public void SetFontNumber(String fontNr) {
            this.fontNr = fontNr;
        }
        /**
        * Set the alternate font name.
        * 
        * @param fontAlternate The falt font value
        * 
        * @since 2.0.8
        */
        public void SetFontAlternate(String fontAlternate) {
            this.falt = fontAlternate;
        }
        /**
        * Set the character-set to the parsed value.
        * 
        * @param charset The charset value
        * 
        * @since 2.0.8
        */
        public void SetCharset(String charset) {
            if (charset.Length == 0) {
                charset = "0";
            }
            this.charset = charset;
        }

        /* (non-Javadoc)
        * @see com.lowagie.text.rtf.direct.RtfDestination#setDefaults()
        * 
        * @since 2.0.8
        */
        public override void SetToDefaults() {
            this.themeFont = "";
            this.fontNr = "";
            this.fontName = "";
            this.fontFamily = "";
            
            this.charset = "";
            this.fprq = 0;
            this.panose = "";
            //this.nontaggedname = "";
            this.falt = "";
            //this.fontemb = "";
            //this.fontType = "";
            //this.fontFile = "";
            //this.fontFileCpg = "";
            this.fbias = 0;
            this.cpg = "";
            this.trueType = "";
            this.state = SETTING_NORMAL;
        }
        /**
        * Process the font information that was parsed from the input.
        * 
        * @since 2.0.8
        */
        private void ProcessFont() {
            this.fontName = this.fontName.Trim();
            if (fontName.Length == 0) return;
            if (fontNr.Length == 0) return;
            
            if (fontName.Length>0 && fontName.IndexOf(';') >= 0) {
                fontName = fontName.Substring(0,fontName.IndexOf(';'));
            }

            if (this.rtfParser.IsImport()) {
                //TODO: If primary font fails, use the alternate
                    //TODO: Problem: RtfFont defaults family to \froman and doesn't allow any other family.
                    // if you set the family, it changes the font name and not the family in the Font.java class.
                    
        //          if (this.fontFamily.Length() > 0) {
        //              if (this.importHeader.ImportFont(this.fontNr, this.fontName, this.fontFamily, Integer.ParseInt(this.charset)) == false) {
        //                  if (this.falt.Length() > 0) {
        //                      this.importHeader.ImportFont(this.fontNr, this.falt, this.fontFamily, Integer.ParseInt(this.charset));
        //                  }
        //              }
        //          } else {
                        if (!this.importHeader.ImportFont(this.fontNr, this.fontName, int.Parse(this.charset==""?CHARSET_DEFAULT:this.charset))) {
                            if (this.falt.Length > 0) {
                                this.importHeader.ImportFont(this.fontNr, this.falt, int.Parse(this.charset==""?CHARSET_DEFAULT:this.charset));
                            }
                        }
        //          }
                }
            if (this.rtfParser.IsConvert()) {
                // This could probably be written as a better font matching function
                
                String fName = this.fontName;   // work variable for trimming name if needed.
                Font f1 = Createfont(fName);
                if (f1.BaseFont == null && this.falt.Length>0)
                    f1 = Createfont(this.falt);
                
                if (f1.BaseFont == null) {
                    // Did not find a font, let's try a substring of the first name.
                    if (FontFactory.COURIER.IndexOf(fName) > -1 ) {
                        f1 = FontFactory.GetFont(FontFactory.COURIER);
                    } else if (FontFactory.HELVETICA.IndexOf(fName) > -1 ) {
                        f1 = FontFactory.GetFont(FontFactory.HELVETICA);
                    } else if (FontFactory.TIMES.IndexOf(fName) > -1 ) {
                        f1 = FontFactory.GetFont(FontFactory.TIMES);
                    } else if (FontFactory.SYMBOL.IndexOf(fName) > -1 ) {
                        f1 = FontFactory.GetFont(FontFactory.SYMBOL);
                    } else if (FontFactory.ZAPFDINGBATS.IndexOf(fName) > -1 ) {
                        f1 = FontFactory.GetFont(FontFactory.ZAPFDINGBATS);
                    } else {
                        // we did not find a matching font in any form.
                        // default to HELVETICA for now.
                        f1 = FontFactory.GetFont(FontFactory.HELVETICA);
                    }
                }
                fontMap[this.fontNr] = f1;
                //System.out.Println(f1.GetFamilyname());
            }
            this.SetToDefaults();
        }
        /**
        * Create a font via the <code>FontFactory</code>
        * 
        * @param fontName The font name to create
        * @return The created <code>Font</code> object
        * 
        * @since 2.0.8
        */
        private Font Createfont(String fontName) {
            Font f1 = null;
            int pos=-1;
            do {
                f1 = FontFactory.GetFont(fontName);
                
                if (f1.BaseFont != null) break; // found a font, exit the do/while
                
                pos = fontName.LastIndexOf(' ');    // find the last space
                if (pos>0) {
                    fontName = fontName.Substring(0, pos ); // truncate it to the last space
                }
            } while (pos>0);
            return f1;
        }
        /**
        * Get a <code>Font</code> object from the font map object
        * 
        * @param key The font number to get
        * @return The mapped <code>Font</code> object.
        * 
        * @since 2.0.8
        */
        public Font GetFont(String key) {
            return (Font) fontMap[key];
        }
        /**
        * Load system fonts into the static <code>FontFactory</code> object
        * 
        * @since 2.0.8
        */
        private void ImportSystemFonts() {
            FontFactory.RegisterDirectories();
        }
        
        /**
        * Utility method to load the environment variables.
        * 
        * @return Properties object with environment variable information
        * @throws Throwable
        * 
        * @since 2.0.8
        */
//        private Properties GetEnvironmentVariables() {
//            Properties environmentVariables = new Properties();
//            String operatingSystem = System.GetProperty("os.name").ToLowerCase();
//            Runtime runtime = Runtime.GetRuntime();
//            Process process = null;
//            if (operatingSystem.IndexOf("windows 95") > -1
//                    || operatingSystem.IndexOf("windows 98") > -1
//                    || operatingSystem.IndexOf("me") > -1) {
//                process = runtime.Exec("command.com /c set");
//            } else if ((operatingSystem.IndexOf("nt") > -1)
//                    || (operatingSystem.IndexOf("windows 2000") > -1)
//                    || (operatingSystem.IndexOf("windows xp") > -1)
//                    || (operatingSystem.IndexOf("windows 2003") > -1)) {
//                process = runtime.Exec("cmd.exe /c set");
//            } else {
//                process = runtime.Exec("env");
//            }
//            BufferedReader environmentStream = new BufferedReader(new InputStreamReader(process.GetInputStream()));
//            String inputLine = "";
//            int idx = -1;
//            while ((inputLine = environmentStream.ReadLine()) != null) {
//                idx = inputLine.IndexOf('=');
//                environmentVariables.SetProperty(inputLine.Substring(0, idx),
//                        inputLine.Substring(idx + 1));
//            }
//            return environmentVariables;
//        }
    }
}