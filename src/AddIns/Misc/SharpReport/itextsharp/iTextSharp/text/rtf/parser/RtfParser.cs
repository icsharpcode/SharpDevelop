using System;
using System.IO;
using System.Collections;
using System.Globalization;
using System.Text;
using iTextSharp.text;
using iTextSharp.text.rtf.direct;
using iTextSharp.text.rtf.document;
using iTextSharp.text.rtf.parser.ctrlwords;
using iTextSharp.text.rtf.parser.destinations;
/*
 * $Id: RtfParser.cs,v 1.4 2008/05/16 19:31:08 psoares33 Exp $
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
namespace iTextSharp.text.rtf.parser {

    /**
    * The RtfParser allows the importing of RTF documents or
    * RTF document fragments. The RTF document or fragment is tokenised,
    * font and color definitions corrected and then added to
    * the document being written.
    * 
    * @author Mark Hall (Mark.Hall@mail.room3b.eu)
    * @author Howard Shank (hgshank@yahoo.com)
    * @since 2.0.8
    */

    public class RtfParser {
        /**
        * Debugging flag.
        */
        private static bool debugParser = false;   // DEBUG Files are unlikely to be read by any reader!
        private String logFile = null;
        private bool logging = false;
        private bool logAppend = false;
        
        /**
        * The iText document to add the RTF document to.
        */
        private Document document = null;
        /**
        * The RtfDocument to add the RTF document or fragment to.
        */
        private RtfDocument rtfDoc = null;
        /**
        * The RtfKeywords that creates and handles keywords that are implemented.
        */
        private RtfCtrlWordMgr rtfKeywordMgr = null;
        /**
        * The RtfImportHeader to store imported font and color mappings in.
        */
        private RtfImportMgr importMgr = null;
        /**
        * The RtfDestinationMgr object to manage destinations.
        */
        private RtfDestinationMgr destinationMgr = null;
        /**
        * Stack for saving states for groups
        */
        private Stack stackState = null;
        /**
        * The current parser state.
        */  
        private RtfParserState currentState = null;
        /**
        * The pushback reader to read the input stream.
        */
        private PushbackStream pbReader = null;
        /**
        * Conversion type. Identifies if we are doing in import or a convert.
        */
        private int conversionType = TYPE_IMPORT_FULL;
        

        /*
        * Bitmapping:
        * 
        * 0111 1111 1111 1111 = Unkown state
        * 0xxx xxxx xxxx xxxx = In Header
        * 1xxx xxxx xxxx xxxx = In Document
        * 2xxx xxxx xxxx xxxx = Reserved
        * 4xxx xxxx xxxx xxxx = Other
        * 8xxx xxxx xxxx xxxx = Errors
        */
        
        /*
        * Header state values
        */

        /**
        * Currently the RTF document header is being parsed.
        */
        public const int PARSER_IN_HEADER = (0x0 << 28) | 0x000000;
        /**
        * Currently the RTF charset is being parsed.
        */
        public const int PARSER_IN_CHARSET = PARSER_IN_HEADER | 0x000001;
        /**
        * Currently the RTF deffont is being parsed.
        */
        public const int PARSER_IN_DEFFONT = PARSER_IN_HEADER | 0x000002;
        /**
        * Currently the RTF font table is being parsed.
        */
        public const int PARSER_IN_FONT_TABLE = PARSER_IN_HEADER | 0x000003;
        /**
        * Currently a RTF font table info element is being parsed.
        */
        public const int PARSER_IN_FONT_TABLE_INFO = PARSER_IN_HEADER | 0x000004;
        /**
        * Currently the RTF filetbl is being parsed.
        */
        public const int PARSER_IN_FILE_TABLE = PARSER_IN_HEADER | 0x000005;
        /**
        * Currently the RTF color table is being parsed.
        */
        public const int PARSER_IN_COLOR_TABLE = PARSER_IN_HEADER | 0x000006;
        /**
        * Currently the RTF  stylesheet is being parsed.
        */
        public const int PARSER_IN_STYLESHEET = PARSER_IN_HEADER | 0x000007;
        /**
        * Currently the RTF listtables is being parsed.
        */
        public const int PARSER_IN_LIST_TABLE = PARSER_IN_HEADER | 0x000008;
        /**
        * Currently the RTF listtable override is being parsed.
        */
        public const int PARSER_IN_LISTOVERRIDE_TABLE = PARSER_IN_HEADER | 0x000009;
        /**
        * Currently the RTF revtbl is being parsed.
        */
        public const int PARSER_IN_REV_TABLE = PARSER_IN_HEADER | 0x00000A;
        /**
        * Currently the RTF rsidtable is being parsed.
        */
        public const int PARSER_IN_RSID_TABLE = PARSER_IN_HEADER | 0x0000B;
        /**
        * Currently the RTF generator is being parsed.
        */
        public const int PARSER_IN_GENERATOR = PARSER_IN_HEADER | 0x00000C;
        /**
        * Currently the RTF Paragraph group properties Table (word 2002)
        */
        public const int PARSER_IN_PARAGRAPH_TABLE = PARSER_IN_HEADER | 0x00000E;
        /**
        * Currently the RTF Old Properties.
        */
        public const int PARSER_IN_OLDCPROPS = PARSER_IN_HEADER | 0x00000F;
        /**
        * Currently the RTF Old Properties.
        */
        public const int PARSER_IN_OLDPPROPS = PARSER_IN_HEADER | 0x000010;
        /**
        * Currently the RTF Old Properties.
        */
        public const int PARSER_IN_OLDTPROPS = PARSER_IN_HEADER | 0x000012;
        /**
        * Currently the RTF Old Properties.
        */
        public const int PARSER_IN_OLDSPROPS = PARSER_IN_HEADER | 0x000013;
        /**
        * Currently the RTF User Protection Information.
        */
        public const int PARSER_IN_PROT_USER_TABLE = PARSER_IN_HEADER | 0x000014;
        /**
        * Currently the Latent Style and Formatting usage restrictions
        */
        public const int PARSER_IN_LATENTSTYLES = PARSER_IN_HEADER | 0x000015;
        
        public const int PARSER_IN_PARAGRAPH_GROUP_PROPERTIES =PARSER_IN_HEADER | 0x000016;
        
        /*
        * Document state values
        */
        
        /**
        * Currently the RTF document content is being parsed.
        */
        public const int PARSER_IN_DOCUMENT = (0x2 << 28 ) | 0x000000;

        /**
        * Currently the RTF info group is being parsed.
        */
        public const int PARSER_IN_INFO_GROUP = PARSER_IN_DOCUMENT | 0x000001;

        
        public const int PARSER_IN_UPR = PARSER_IN_DOCUMENT | 0x000002;
        /**
        * Currently a shppict control word is being parsed.
        */
        public const int PARSER_IN_SHPPICT = PARSER_IN_DOCUMENT | 0x000010; //16
        /**
        * Currently a pict control word is being parsed.
        */
        public const int PARSER_IN_PICT = PARSER_IN_DOCUMENT | 0x000011; //17
        /**
        * Currently a picprop control word is being parsed.
        */
        public const int PARSER_IN_PICPROP = PARSER_IN_DOCUMENT | 0x000012; //18
        /**
        * Currently a blipuid control word is being parsed.
        */
        public const int PARSER_IN_BLIPUID = PARSER_IN_DOCUMENT | 0x000013; //18

        /* other states */
        /**
        * The parser is at the beginning or the end of the file.
        */
        public const int PARSER_STARTSTOP = (0x4 << 28)| 0x0001;
        /* ERRORS */
        /**
        * Currently the parser is in an error state.
        */
        public const int PARSER_ERROR = (0x8 << 28) | 0x0000;
        /**
        * The parser reached the end of the file.
        */
        public const int PARSER_ERROR_EOF = PARSER_ERROR | 0x0001;
        /**
        * Currently the parser is in an unknown state.
        */
        public const int PARSER_IN_UNKNOWN = PARSER_ERROR | 0x0FFFFFFF;
        
        
        /**
        * Conversion type is unknown
        */
        public const int TYPE_UNIDENTIFIED = -1;
        /**
        * Conversion type is an import. Uses direct content to add everything.
        * This is what the original import does.
        */
        public const int TYPE_IMPORT_FULL = 0;
        /**
        * Conversion type is an import of a partial file/fragment. Uses direct content to add everything.
        */
        public const int TYPE_IMPORT_FRAGMENT = 1;
        /**
        * Conversion type is a conversion. This uses the document (not rtfDoc) to add
        * all the elements making it a different supported documents depending on the writer used.
        */
        public const int TYPE_CONVERT = 2;

        
        /**
        * Destination is normal. Text is processed.
        */
        public const int DESTINATION_NORMAL = 0;
        /**
        * Destination is skipping. Text is ignored.
        */
        public const int DESTINATION_SKIP = 1;
        
        //////////////////////////////////// TOKENISE VARIABLES ///////////////////
        /*
        * State flags use 4/28 bitmask.
        * First 4 bits (nibble) indicates major state. Used for unknown and error
        * Last 28 bits indicates the value;
        */
        
        /**
        * The RtfTokeniser is in its ground state. Any token may follow.
        */
        public const int TOKENISER_NORMAL = 0x00000000;
        /**
        * The last token parsed was a slash.
        */
        public const int TOKENISER_SKIP_BYTES = 0x00000001;
        /**
        * The RtfTokeniser is currently tokenising a control word.
        */
        public const int TOKENISER_SKIP_GROUP = 0x00000002;
        /**
        * The RtfTokeniser is currently reading binary stream.
        */
        public const int TOKENISER_BINARY= 0x00000003;
        /**
        * The RtfTokeniser is currently reading hex data.
        */
        public const int TOKENISER_HEX= 0x00000004;
	    /**
	    * The RtfTokeniser ignore result
	    */
	    public const int TOKENISER_IGNORE_RESULT= 0x00000005;
        /**
        * The RtfTokeniser is currently in error state
        */
        public const int TOKENISER_STATE_IN_ERROR =  unchecked((int)0x80000000); // 1000 0000 0000 0000 0000 0000 0000 0000
        /**
        * The RtfTokeniser is currently in an unkown state
        */
        public const int TOKENISER_STATE_IN_UNKOWN = unchecked((int)0xFF000000); // 1111 0000 0000 0000 0000 0000 0000 0000
        
        /**
        * The current group nesting level.
        */
        private int groupLevel = 0;
        /**
        * The current document group nesting level. Used for fragments.
        */
        private int docGroupLevel = 0;
        /**
        * When the tokeniser is Binary.
        */
        private long binByteCount = 0;
        /**
        * When the tokeniser is set to skip bytes, binSkipByteCount is the number of bytes to skip.
        */
        private long binSkipByteCount = 0;
        /**
        * When the tokeniser is set to skip to next group, this is the group indentifier to return to.
        */
        private int skipGroupLevel = 0;

        //RTF parser error codes
        public const int  errOK =0;                        // Everything's fine!
        public const int  errStackUnderflow   =  -1;       // Unmatched '}'
        public const int  errStackOverflow    =  -2;       // Too many '{' -- memory exhausted
        public const int  errUnmatchedBrace   =  -3;       // RTF ended during an open group.
        public const int  errInvalidHex       =  -4;       // invalid hex character found in data
        public const int  errBadTable         =  -5;       // RTF table (sym or prop) invalid
        public const int  errAssertion        =  -6;       // Assertion failure
        public const int  errEndOfFile        =  -7;       // End of file reached while reading RTF
        public const int  errCtrlWordNotFound =  -8;       // control word was not found
        //////////////////////////////////// TOKENISE VARIABLES ///////////////////
        
        
        //////////////////////////////////// STATS VARIABLES ///////////////////
        /**
        * Total bytes read.
        */
        private long byteCount = 0;
        /**
        * Total control words processed.
        * 
        * Contains both known and unknown.
        * 
        * <code>ctrlWordCount</code> should equal 
        * <code>ctrlWrodHandlecCount</code> + <code>ctrlWordNotHandledCount</code + <code>ctrlWordSkippedCount</code>
        */
        private long ctrlWordCount = 0;
        /**
        * Total { encountered as an open group token.
        */
        private long openGroupCount = 0;
        /**
        * Total } encountered as a close group token.
        */
        private long closeGroupCount = 0;
        /**
        * Total clear text characters processed.
        */
        private long characterCount = 0;
        /**
        * Total control words recognized.
        */
        private long ctrlWordHandledCount = 0;
        /**
        * Total control words not handled.
        */
        private long ctrlWordNotHandledCount = 0;
        /**
        * Total control words skipped.
        */
        private long ctrlWordSkippedCount = 0;
        /**
        * Total groups skipped. Includes { and } as a group.
        */
        private long groupSkippedCount = 0;
        /**
        * Start time as a long.
        */
        private long startTime = 0;
        /**
        * Stop time as a long.
        */
        private long endTime = 0;
        /**
        * Start date as a date.
        */
        private DateTime startDate;
        /**
        * End date as a date.
        */
        private DateTime endDate;
        //////////////////////////////////// STATS VARIABLES ///////////////////
        /**
        * Last control word and parameter processed.
        */
        private RtfCtrlWordData lastCtrlWordParam = null;
        
        /** The <code>RtfCtrlWordListener</code>. */
        private ArrayList listeners = new ArrayList();

        /* *********
        *  READER *
        ***********/
        /**
        * Imports a complete RTF document.
        * 
        * @param readerIn 
        *       The Reader to read the RTF document from.
        * @param rtfDoc 
        *       The RtfDocument to add the imported document to.
        * @throws IOException On I/O errors.
        */
        public void ImportRtfDocument(Stream readerIn, RtfDocument rtfDoc) {
            if (readerIn == null || rtfDoc == null) return;
            this.Init(TYPE_IMPORT_FULL, rtfDoc, readerIn, null);
            this.SetCurrentDestination(RtfDestinationMgr.DESTINATION_NULL);
            startDate = DateTime.Now;
            startTime = startDate.Ticks / 10000L;
            this.groupLevel = 0;
            try {
                this.Tokenise();
            }
            catch {
            }
            endDate = DateTime.Now;
            endTime = endDate.Ticks / 10000L;
        }
        
        /**
        * Converts an RTF document to an iText document.
        * 
        * Usage: Create a parser object and call this method with the input stream and the iText Document object
        * 
        * @param readerIn 
        *       The Reader to read the RTF file from.
        * @param doc 
        *       The iText document that the RTF file is to be added to.
        * @throws IOException 
        *       On I/O errors.
        */
        public void ConvertRtfDocument(Stream readerIn, Document doc) {
            if (readerIn == null || doc == null) return;
            this.Init(TYPE_CONVERT, null, readerIn, doc);
            this.SetCurrentDestination(RtfDestinationMgr.DESTINATION_DOCUMENT);
            startDate = DateTime.Now;
            startTime = startDate.Ticks / 10000L;
            this.groupLevel = 0;
            this.Tokenise();
            endDate = DateTime.Now;
            endTime = endDate.Ticks / 10000L;
        }

        /**
        * Imports an RTF fragment.
        * 
        * @param readerIn 
        *       The Reader to read the RTF fragment from.
        * @param rtfDoc 
        *       The RTF document to add the RTF fragment to.
        * @param importMappings 
        *       The RtfImportMappings defining font and color mappings for the fragment.
        * @throws IOException 
        *       On I/O errors.
        */
        public void ImportRtfFragment(Stream readerIn, RtfDocument rtfDoc, RtfImportMappings importMappings) {
        //public void ImportRtfFragment2(Reader readerIn, RtfDocument rtfDoc, RtfImportMappings importMappings) throws IOException {
            if (readerIn == null || rtfDoc == null || importMappings==null) return;
            this.Init(TYPE_IMPORT_FRAGMENT, rtfDoc, readerIn, null);
            this.HandleImportMappings(importMappings);
            this.SetCurrentDestination(RtfDestinationMgr.DESTINATION_DOCUMENT);
            this.groupLevel = 1;
            SetParserState(RtfParser.PARSER_IN_DOCUMENT);
            startDate = DateTime.Now;
            startTime = startDate.Ticks / 10000L;
            this.Tokenise();
            endDate = DateTime.Now;
            endTime = endDate.Ticks / 10000L;
        }
        
        // listener methods

        /**
        * Adds a <CODE>EventListener</CODE> to the <CODE>RtfCtrlWordMgr</CODE>.
        *
        * @param listener
        *            the new EventListener.
        */
        public void AddListener(IEventListener listener) {
            listeners.Add(listener);
        }

        /**
        * Removes a <CODE>EventListener</CODE> from the <CODE>RtfCtrlWordMgr</CODE>.
        *
        * @param listener
        *            the EventListener that has to be removed.
        */
        public void RemoveListener(IEventListener listener) {
            listeners.Remove(listener);
        }

        /**
        * Initialize the parser object values. 
        * 
        * @param type Type of conversion or import
        * @param rtfDoc The <code>RtfDocument</code>
        * @param readerIn The input stream
        * @param doc The iText <code>Document</code>
        */
        private void Init(int type, RtfDocument rtfDoc, Stream readerIn, Document doc) {

            Init_stats();
            // initialize reader to a PushbackReader
            this.pbReader = Init_Reader(readerIn);
            
            this.conversionType = type;
            this.rtfDoc = rtfDoc;
            this.document = doc;
            this.currentState = new RtfParserState();
            this.stackState = new Stack();
            this.SetParserState(PARSER_STARTSTOP);
            this.importMgr = new RtfImportMgr(this.rtfDoc, this.document);

            // get destination Mgr
            this.destinationMgr = RtfDestinationMgr.GetInstance(this);
            // set the parser
            RtfDestinationMgr.SetParser(this);


            // DEBUG INFO for timing and memory usage of RtfCtrlWordMgr object
            // create multiple new RtfCtrlWordMgr objects to check timing and memory usage
    //      System.Gc();
    //      long endTime = 0;
    //      Date endDate = null;        
    //      long endFree = 0;
    //      DecimalFormat df = new DecimalFormat("#,##0");
    //      Date startDate = new Date();
    //      long startTime = System.CurrentTimeMillis();
    //      long startFree = Runtime.GetRuntime().FreeMemory();
    //      System.out.Println("1:");
            
            this.rtfKeywordMgr = new RtfCtrlWordMgr(this, this.pbReader);/////////DO NOT COMMENT OUT THIS LINE ///////////
            
            foreach (object listener in listeners) {
                if (listener is IRtfCtrlWordListener) {
                    this.rtfKeywordMgr.AddRtfCtrlWordListener((IRtfCtrlWordListener)listener);    
                }
            }
    //      endFree = Runtime.GetRuntime().FreeMemory();
    //      endTime = System.CurrentTimeMillis();
    //      endDate = new Date();
    //      System.out.Println("RtfCtrlWordMgr start date: " + startDate.ToLocaleString());
    //      System.out.Println("RtfCtrlWordMgr end date  : " + endDate.ToLocaleString());
    //      System.out.Println("  Elapsed time    : " + Long.ToString(endTime - startTime) + " milliseconds.");
    //      System.out.Println("Begin Constructor RtfCtrlWordMgr , free mem is " + df.Format(startFree / 1024) + "k");
    //      System.out.Println("End Constructor RtfCtrlWordMgr , free mem is " + df.Format(endFree / 1024) + "k");
    //      System.out.Println("RtfCtrlWordMgr used approximately " + df.Format((startFree - endFree) / 1024) + "k");
    //      
    //      System.Gc();
    //      System.out.Println("2:");
    //      startDate = new Date();
    //      startTime = System.CurrentTimeMillis();
    //      startFree = Runtime.GetRuntime().FreeMemory();
    //      RtfCtrlWordMgr rtfKeywordMgr2 = new RtfCtrlWordMgr(this, this.pbReader);        
    //      endFree = Runtime.GetRuntime().FreeMemory();
    //      endTime = System.CurrentTimeMillis();
    //      endDate = new Date();
    //      System.out.Println("RtfCtrlWordMgr start date: " + startDate.ToLocaleString());
    //      System.out.Println("RtfCtrlWordMgr end date  : " + endDate.ToLocaleString());
    //      System.out.Println("  Elapsed time    : " + Long.ToString(endTime - startTime) + " milliseconds.");
    //      System.out.Println("Begin Constructor RtfCtrlWordMgr , free mem is " + df.Format(startFree / 1024) + "k");
    //      System.out.Println("End Constructor RtfCtrlWordMgr , free mem is " + df.Format(endFree / 1024) + "k");
    //      System.out.Println("RtfCtrlWordMgr used approximately " + df.Format((startFree - endFree) / 1024) + "k");
    //      
    //      System.Gc();
    //      System.out.Println("3:");
    //      startDate = new Date();
    //      startTime = System.CurrentTimeMillis();
    //      startFree = Runtime.GetRuntime().FreeMemory();
    //      RtfCtrlWordMgr rtfKeywordMgr3 = new RtfCtrlWordMgr(this, this.pbReader);    
    //      endFree = Runtime.GetRuntime().FreeMemory();
    //      endTime = System.CurrentTimeMillis();
    //      endDate = new Date();
    //      System.out.Println("RtfCtrlWordMgr start date: " + startDate.ToLocaleString());
    //      System.out.Println("RtfCtrlWordMgr end date  : " + endDate.ToLocaleString());
    //      System.out.Println("  Elapsed time    : " + Long.ToString(endTime - startTime) + " milliseconds.");
    //      System.out.Println("Begin Constructor RtfCtrlWordMgr , free mem is " + df.Format(startFree / 1024) + "k");
    //      System.out.Println("End Constructor RtfCtrlWordMgr , free mem is " + df.Format(endFree / 1024) + "k");
    //      System.out.Println("RtfCtrlWordMgr used approximately " + df.Format((startFree - endFree) / 1024) + "k");
    //
    //      System.Gc();
    //      System.out.Println("4:");
    //      startDate = new Date();
    //      startTime = System.CurrentTimeMillis();
    //      startFree = Runtime.GetRuntime().FreeMemory();
    //      RtfCtrlWordMgr rtfKeywordMgr4 = new RtfCtrlWordMgr(this, this.pbReader);    
    //      endFree = Runtime.GetRuntime().FreeMemory();
    //      endTime = System.CurrentTimeMillis();
    //      endDate = new Date();
    //      System.out.Println("RtfCtrlWordMgr start date: " + startDate.ToLocaleString());
    //      System.out.Println("RtfCtrlWordMgr end date  : " + endDate.ToLocaleString());
    //      System.out.Println("  Elapsed time    : " + Long.ToString(endTime - startTime) + " milliseconds.");
    //      System.out.Println("Begin Constructor RtfCtrlWordMgr , free mem is " + df.Format(startFree / 1024) + "k");
    //      System.out.Println("End Constructor RtfCtrlWordMgr , free mem is " + df.Format(endFree / 1024) + "k");
    //      System.out.Println("RtfCtrlWordMgr used approximately " + df.Format((startFree - endFree) / 1024) + "k");
    //
    //      System.Gc();
    //      System.out.Println("5:");
    //      startDate = new Date();
    //      startTime = System.CurrentTimeMillis();
    //      startFree = Runtime.GetRuntime().FreeMemory();
    //      RtfCtrlWordMgr rtfKeywordMgr5 = new RtfCtrlWordMgr(this, this.pbReader);    
    //      endFree = Runtime.GetRuntime().FreeMemory();
    //      endTime = System.CurrentTimeMillis();
    //      endDate = new Date();
    //      System.out.Println("RtfCtrlWordMgr start date: " + startDate.ToLocaleString());
    //      System.out.Println("RtfCtrlWordMgr end date  : " + endDate.ToLocaleString());
    //      System.out.Println("  Elapsed time    : " + Long.ToString(endTime - startTime) + " milliseconds.");
    //      System.out.Println("Begin Constructor RtfCtrlWordMgr , free mem is " + df.Format(startFree / 1024) + "k");
    //      System.out.Println("End Constructor RtfCtrlWordMgr , free mem is " + df.Format(endFree / 1024) + "k");
    //      System.out.Println("RtfCtrlWordMgr used approximately " + df.Format((startFree - endFree) / 1024) + "k");
    //      System.Gc();
    //      System.out.Println("At ed:");
    //      startDate = new Date();
    //      startTime = System.CurrentTimeMillis();
    //      startFree = Runtime.GetRuntime().FreeMemory();
    //      //RtfCtrlWordMgr rtfKeywordMgr6 = new RtfCtrlWordMgr(this, this.pbReader);  
    //      endFree = Runtime.GetRuntime().FreeMemory();
    //      endTime = System.CurrentTimeMillis();
    //      endDate = new Date();
    //      System.out.Println("RtfCtrlWordMgr start date: " + startDate.ToLocaleString());
    //      System.out.Println("RtfCtrlWordMgr end date  : " + endDate.ToLocaleString());
    //      System.out.Println("  Elapsed time    : " + Long.ToString(endTime - startTime) + " milliseconds.");
    //      System.out.Println("Begin Constructor RtfCtrlWordMgr , free mem is " + df.Format(startFree / 1024) + "k");
    //      System.out.Println("End Constructor RtfCtrlWordMgr , free mem is " + df.Format(endFree / 1024) + "k");
    //      System.out.Println("RtfCtrlWordMgr used approximately " + df.Format((startFree - endFree) / 1024) + "k");
        }
        /**
        * Initialize the statistics values.
        */
        protected void Init_stats() {
            byteCount = 0;
            ctrlWordCount = 0;
            openGroupCount = 0;
            closeGroupCount = 0;
            characterCount = 0;
            ctrlWordHandledCount = 0;
            ctrlWordNotHandledCount = 0;
            ctrlWordSkippedCount = 0;
            groupSkippedCount = 0;
            startTime = 0;
            endTime = 0;
            //startDate = null;
            //endDate = null;
        }
        
        /**
        * Casts the input reader to a PushbackReader or 
        * creates a new PushbackReader from the Reader passed in.
        * The reader is also transformed into a BufferedReader if necessary.
        * 
        * @param readerIn
        *       The Reader object for the input file.
        * @return
        *       PushbackReader object
        */
        private PushbackStream Init_Reader(Stream readerIn) {
            if (readerIn is PushbackStream) {
                return (PushbackStream)readerIn;
            }
            
            // return the proper reader object to the parser setup
            return new PushbackStream(readerIn);
        }
        
        /**
        * Imports the mappings defined in the RtfImportMappings into the
        * RtfImportHeader of this RtfParser2.
        * 
        * @param importMappings 
        *       The RtfImportMappings to import.
        */
        private void HandleImportMappings(RtfImportMappings importMappings) {
            foreach (String fontNr in importMappings.GetFontMappings().Keys) {
                this.importMgr.ImportFont(fontNr, (String) importMappings.GetFontMappings()[fontNr]);
            }
            foreach (String colorNr in importMappings.GetColorMappings().Keys) {
                this.importMgr.ImportColor(colorNr, (Color) importMappings.GetColorMappings()[colorNr]);
            }
            foreach (String listNr in importMappings.GetListMappings().Keys) {
                this.importMgr.ImportList(listNr, (List) importMappings.GetListMappings()[listNr]);
            }
            foreach (String stylesheetListNr in importMappings.GetStylesheetListMappings().Keys) {
                this.importMgr.ImportStylesheetList(stylesheetListNr, (List) importMappings.GetStylesheetListMappings()[stylesheetListNr]);
            }
            
        }
        
        
        /* *****************************************
        *   DOCUMENT CONTROL METHODS
        *   
        *   Handles -
        *   handleOpenGroup:    Open groups     - '{'
        *   handleCloseGroup:   Close groups    - '}'
        *   handleCtrlWord:     Ctrl Words      - '\...'
        *   handleCharacter:    Characters      - Plain Text, etc.
        * 
        */
        
        /**
        * Handles open group tokens. ({)
        * 
        * @return errOK if ok, other if an error occurred.
        */
        public int HandleOpenGroup() {
            int result = errOK;
            this.openGroupCount++;  // stats
            this.groupLevel++;      // current group level in tokeniser
            this.docGroupLevel++;   // current group level in document
            if (this.GetTokeniserState() == TOKENISER_SKIP_GROUP) { 
                this.groupSkippedCount++;
            }
        
            RtfDestination dest = (RtfDestination)this.GetCurrentDestination();
            bool handled = false;
            
            if (dest != null) {
                if (debugParser) {
                    RtfParser.OutputDebug(this.rtfDoc, groupLevel, "DEBUG: before dest.HandleOpeningSubGroup()");
                    RtfParser.OutputDebug(this.rtfDoc, groupLevel, "DEBUG: destination=" + dest.ToString());
                }
                handled = dest.HandleOpeningSubGroup();
                if (debugParser) {
                    RtfParser.OutputDebug(this.rtfDoc, groupLevel, "DEBUG: after dest.HandleOpeningSubGroup()");
                }
            }

            this.stackState.Push(this.currentState);
            this.currentState = new RtfParserState(this.currentState);
            // do not set this true until after the state is pushed
            // otherwise it inserts a { where one does not belong.
            this.currentState.newGroup = true;
            dest = (RtfDestination)this.GetCurrentDestination();
            
            if (debugParser) {
                RtfParser.OutputDebug(this.rtfDoc, groupLevel, "DEBUG: HandleOpenGroup()");
                if (this.lastCtrlWordParam != null)
                    RtfParser.OutputDebug(this.rtfDoc, groupLevel, "DEBUG: LastCtrlWord=" + this.lastCtrlWordParam.ctrlWord);
                RtfParser.OutputDebug(this.rtfDoc, groupLevel, "DEBUG: grouplevel=" + groupLevel.ToString());
                RtfParser.OutputDebug(this.rtfDoc, groupLevel, "DEBUG: destination=" + dest.ToString());
            }

            if (dest != null) {
                handled = dest.HandleOpenGroup();
            }
            
            if (debugParser) {
                RtfParser.OutputDebug(this.rtfDoc, groupLevel, "DEBUG: after dest.HandleOpenGroup(); handled=" + handled.ToString());
            }
            
            return result;
        }
        public static void OutputDebug(object doc, int groupLevel, String str) {
            Console.Out.WriteLine(str);
            if(doc == null) return;
            if (groupLevel<0) groupLevel = 0;
            String spaces = new String(' ', groupLevel*2);
            if(doc is RtfDocument) {
                ((RtfDocument)doc).Add(new RtfDirectContent("\n" + spaces + str));
            }
            else if(doc is Document) {
                try {
                    ((Document)doc).Add(new RtfDirectContent("\n" + spaces + str));
                } catch (DocumentException) {
                }
            }
        }
        /**
        * Handles close group tokens. (})
        * 
        * @return errOK if ok, other if an error occurred. 
        */
        public int HandleCloseGroup() {
            int result = errOK;
            this.closeGroupCount++; // stats

            if (this.GetTokeniserState() != TOKENISER_SKIP_GROUP) {
                if (debugParser) {
                    RtfParser.OutputDebug(this.rtfDoc, groupLevel, "DEBUG: HandleCloseGroup()");
                    if (this.lastCtrlWordParam != null)
                        RtfParser.OutputDebug(this.rtfDoc, groupLevel, "DEBUG: LastCtrlWord=" + this.lastCtrlWordParam.ctrlWord);
                    RtfParser.OutputDebug(this.rtfDoc, groupLevel, "DEBUG: grouplevel=" + groupLevel.ToString());
                    RtfParser.OutputDebug(this.rtfDoc, groupLevel, "DEBUG: destination=" + this.GetCurrentDestination().ToString());
                    RtfParser.OutputDebug(this.rtfDoc, groupLevel, "");
                }
                RtfDestination dest = (RtfDestination)this.GetCurrentDestination();
                bool handled = false;
                
                if (dest != null) {
                    handled = dest.HandleCloseGroup();
                }
                if (debugParser) {
                    RtfParser.OutputDebug(this.rtfDoc, groupLevel, "DEBUG: After dest.HandleCloseGroup(); handled = " + handled.ToString());
                    RtfParser.OutputDebug(this.rtfDoc, groupLevel, "");
                }
            }
            
            if (this.stackState.Count >0 ) {
                this.currentState = (RtfParserState)this.stackState.Pop();
            } else {
                result = errStackUnderflow;
            }
            
            this.docGroupLevel--;
            this.groupLevel--;
            
            if (this.GetTokeniserState() == TOKENISER_SKIP_GROUP && this.groupLevel < this.skipGroupLevel) {
                this.SetTokeniserState(TOKENISER_NORMAL);
            }

            return result;  
        }
        

        /**
        * Handles control word tokens. Depending on the current
        * state a control word can lead to a state change. When
        * parsing the actual document contents, certain tabled
        * values are remapped. i.e. colors, fonts, styles, etc.
        * 
        * @param ctrlWordData The control word to handle.
        * @return errOK if ok, other if an error occurred.
        */
        public int HandleCtrlWord(RtfCtrlWordData ctrlWordData) {
            int result = errOK;
            this.ctrlWordCount++; // stats

            if (debugParser) {
                RtfParser.OutputDebug(this.rtfDoc, groupLevel, "DEBUG: handleCtrlWord=" + ctrlWordData.ctrlWord);
            }

            if (this.GetTokeniserState() == TOKENISER_SKIP_GROUP) { 
                this.ctrlWordSkippedCount++;
                if (debugParser) {
                    RtfParser.OutputDebug(this.rtfDoc, groupLevel, "DEBUG: SKIPPED");
                }
                return result;
            }

            //      RtfDestination dest = (RtfDestination)this.GetCurrentDestination();
    //      bool handled = false;
    //      if (dest != null) {
    //          handled = dest.HandleControlWord(ctrlWordData);
    //      }
            
            result = this.rtfKeywordMgr.HandleKeyword(ctrlWordData, this.groupLevel);

            if ( result == errOK){
                this.ctrlWordHandledCount++;
            } else {
                this.ctrlWordNotHandledCount++;
                result = errOK; // hack for now.
            }
            
            return result;
        }

        /**
        * Handles text tokens. These are either handed on to the
        * appropriate destination handler.
        * 
        * @param nextChar
        *       The text token to handle.
        * @return errOK if ok, other if an error occurred. 
        */
        public int HandleCharacter(int nextChar) {       
            this.characterCount++;  // stats

            if (this.GetTokeniserState() == TOKENISER_SKIP_GROUP) { 
                return errOK;
            }

            bool handled = false;

            RtfDestination dest = (RtfDestination)this.GetCurrentDestination();
            if (dest != null) {
                handled = dest.HandleCharacter(nextChar);
            }

            return errOK;
        }

        /**
        * Get the state of the parser.
        *
        * @return
        *       The current RtfParserState state object.
        */
        public RtfParserState GetState(){
            return this.currentState;
        }   

        /**
        * Get the current state of the parser.
        * 
        * @return 
        *       The current state of the parser.
        */
        public int GetParserState(){
            return this.currentState.parserState;
        }
        
        /**
        * Set the state value of the parser.
        *
        * @param newState
        *       The new state for the parser
        * @return
        *       The state of the parser.
        */
        public int SetParserState(int newState){
            this.currentState.parserState = newState;
            return this.currentState.parserState;
        }

        /**
        * Get the conversion type.
        * 
        * @return
        *       The type of the conversion. Import or Convert.
        */
        public int GetConversionType() {
            return this.conversionType;
        }
        
        /**
        * Get the RTF Document object.
        * @return
        *       Returns the object rtfDoc.
        */
        public RtfDocument GetRtfDocument() {
            return this.rtfDoc;
        }
        
        /**
        * Get the Document object.
        * @return
        *       Returns the object rtfDoc.
        */
        public Document GetDocument() {
            return this.document;
        }

        /**
        * Get the RtfImportHeader object.
        * @return
        *       Returns the object importHeader.
        */
        public RtfImportMgr GetImportManager() {
            return importMgr;
        }
        
        
        /////////////////////////////////////////////////////////////
        // accessors for destinations
        /**
        * Set the current destination object for the current state.
        * @param dest The destination value to set.
        */
        public bool SetCurrentDestination(String destination) {
                RtfDestination dest = RtfDestinationMgr.GetDestination(destination);
                if (dest != null) {
                    this.currentState.destination = dest;
                    return false;
                } else {
                    this.SetTokeniserStateSkipGroup();
                    return false;
                }
        }
        /**
        * Get the current destination object.
        * 
        * @return The current state destination
        */
        public RtfDestination GetCurrentDestination() {
            return this.currentState.destination;
        }
        /**
        * Get a destination from the map
        * 
        * @para destination The string destination.
        * @return The destination object from the map
        */
        public RtfDestination GetDestination(String destination) {
            return RtfDestinationMgr.GetDestination(destination);
        }
        
        /**
        * Helper method to determine if this is a new group.
        * 
        * @return true if this is a new group, otherwise it returns false.
        */
        public bool IsNewGroup() {
            return this.currentState.newGroup;
        }
        /**
        * Helper method to set the new group flag
        * @param value The bool value to set the flag
        * @return The value of newGroup
        */
        public bool SetNewGroup(bool value) {
            this.currentState.newGroup = value;
            return this.currentState.newGroup;
        }
        
        /* ************
        *  TOKENISER *
        **************/
        
        /**
        * Read through the input file and parse the data stream into tokens.
        * 
        * @throws IOException on IO error.
        */  
        public void Tokenise() {
            int errorCode = errOK;  // error code
            int nextChar = 0;
            this.SetTokeniserState(TOKENISER_NORMAL);   // set initial tokeniser state
            
            
            while((nextChar = this.pbReader.ReadByte()) != -1) {
                this.byteCount++;
                
                if (this.GetTokeniserState() == TOKENISER_BINARY)                      // if we're parsing binary data, handle it directly
                {
                    if ((errorCode = ParseChar(nextChar)) != errOK)
                        return; 
                }  else {
                    switch (nextChar) {
                        case '{':   // scope delimiter - Open
                            this.HandleOpenGroup();
                            break;
                        case '}':  // scope delimiter - Close
                            this.HandleCloseGroup();
                            break;
                        case '\n':  // noise character
                        case '\r':  // noise character
    //                      if (this.IsImport()) {
    //                          this.rtfDoc.Add(new RtfDirectContent(new String(nextChar)));
    //                      }
                            break;
                        case '\\':  // Control word start delimiter
                                if (ParseCtrlWord(pbReader) != errOK) {
                                // TODO: Indicate some type of error
                                return;
                            }
                            break;
                        default:
                            if (groupLevel == 0) { // BOMs
                                break;
                            }
                            if (this.GetTokeniserState() == TOKENISER_HEX) {
                                StringBuilder hexChars = new StringBuilder();
                                hexChars.Append(nextChar);
                                if((nextChar = pbReader.ReadByte()) == -1) {
                                    return;
                                }
                                this.byteCount++;
                                hexChars.Append(nextChar);
                                try {
                                    nextChar=int.Parse(hexChars.ToString(), NumberStyles.HexNumber);
                                } catch {
                                    return;
                                }
                                this.SetTokeniserState(TOKENISER_NORMAL);
                            }
                            if ((errorCode = ParseChar(nextChar)) != errOK) {
                                return; // some error occurred. we should send a
                                        // real error
                            }
                            break;
                    }   // switch (nextChar[0])
                }   // end if (this.GetTokeniserState() == TOKENISER_BINARY)
                
    //          if (groupLevel < 1 && this.IsImportFragment()) return; //return errOK;
    //          if (groupLevel < 0 && this.IsImportFull()) return; //return errStackUnderflow;
    //          if (groupLevel < 0 && this.IsConvert()) return; //return errStackUnderflow;
                
            }// end while (reader.Read(nextChar) != -1)
            RtfDestination dest = (RtfDestination)this.GetCurrentDestination();
            if (dest != null) {
                dest.CloseDestination();
            }
        }
        
        /**
        * Process the character and send it to the current destination.
        * @param nextChar
        *       The character to process
        * @return
        *       Returns an error code or errOK if no error.
        */
        private int ParseChar(int nextChar) {
            // figure out where to put the character
            // needs to handle group levels for parsing
            // examples
            /*
            * {\f3\froman\fcharset2\fprq2{\*\panose 05050102010706020507}Symbol;}
            * {\f7\fswiss\fcharset0\fprq2{\*\panose 020b0604020202030204}Helv{\*\falt Arial};} <- special case!!!!
            * {\f5\froman\fcharset0 Tahoma;}
            * {\f6\froman\fcharset0 Arial Black;}
            * {\info(\author name}{\company company name}}
            * ... document text ...
            */
            if (this.GetTokeniserState() == TOKENISER_BINARY && --binByteCount <= 0)
                this.SetTokeniserStateNormal();
            if (this.GetTokeniserState() == TOKENISER_SKIP_BYTES && --binSkipByteCount <= 0)
                this.SetTokeniserStateNormal();
            return this.HandleCharacter(nextChar);
        }
        
        /**
        * Parses a keyword and it's parameter if one exists
        * @param reader
        *       This is a pushback reader for file input.
        * @return
        *       Returns an error code or errOK if no error.
        * @throws IOException
        *       Catch any file read problem.
        */
        private int ParseCtrlWord(PushbackStream reader) {
            int nextChar = 0;
            int result = errOK;
            
            if((nextChar = reader.ReadByte()) == -1) {
                    return errEndOfFile;
            }
            this.byteCount++;

            StringBuilder parsedCtrlWord = new StringBuilder();
            StringBuilder parsedParam= new StringBuilder();
            RtfCtrlWordData ctrlWordParam = new RtfCtrlWordData();
            
            if (!Char.IsLetterOrDigit((char)nextChar)) {
                parsedCtrlWord.Append((char)nextChar);
                ctrlWordParam.ctrlWord = parsedCtrlWord.ToString();
                result =  this.HandleCtrlWord(ctrlWordParam);
                lastCtrlWordParam = ctrlWordParam;
                return result;
            }
            
    //      for ( ; Character.IsLetter(nextChar[0]); reader.Read(nextChar) ) {
    //          parsedCtrlWord.Append(nextChar[0]);
    //      }
            do {
                parsedCtrlWord.Append((char)nextChar);
                //TODO: catch EOF
                nextChar = reader.ReadByte();
                this.byteCount++;
            } while  (Char.IsLetter((char)nextChar));
            
            ctrlWordParam.ctrlWord = parsedCtrlWord.ToString();

            if ((char)nextChar == '-') {
                ctrlWordParam.isNeg = true;
                if((nextChar = reader.ReadByte()) == -1) {
                        return errEndOfFile;
                }
                this.byteCount++;
            }
            
            if (Char.IsDigit((char)nextChar)) {
                ctrlWordParam.hasParam = true;
    //          for ( ; Character.IsDigit(nextChar[0]); reader.Read(nextChar) ) {
    //              parsedParam.Append(nextChar[0]);
    //          }
                do {
                    parsedParam.Append((char)nextChar);
                    //TODO: catch EOF
                    nextChar = reader.ReadByte();
                    this.byteCount++;
                } while  (Char.IsDigit((char)nextChar));
                
                ctrlWordParam.param = parsedParam.ToString();
            }
            
            // push this character back into the stream
            if ((char)nextChar != ' ') { // || this.IsImport() ) {
                reader.Unread(nextChar);
            }
            
            if (debugParser) {
        //      // debug: insrsid6254399
        //      if (ctrlWordParam.ctrlWord.Equals("proptype") && ctrlWordParam.param.Equals("30")) {
        //          System.out.Print("Debug value found\n");
        //      }
        //      if (ctrlWordParam.ctrlWord.Equals("panose") ) {
        //          System.out.Print("Debug value found\n");
        //      }
            }
            
            result = this.HandleCtrlWord(ctrlWordParam);
            lastCtrlWordParam = ctrlWordParam;
            return result;

        }
        
        /**
        * Set the current state of the tokeniser.
        * @param value The new state of the tokeniser.
        * @return The state of the tokeniser.
        */
        public int SetTokeniserState(int value) {
            this.currentState.tokeniserState = value;
            return this.currentState.tokeniserState;
        }
        
        /**
        * Get the current state of the tokeniser.
        * @return The current state of the tokeniser.
        */
        public int GetTokeniserState() {
            return this.currentState.tokeniserState;
        }

        /**
        * Gets the current group level
        * 
        * @return
        *       The current group level value.
        */
        public int GetLevel() {
            return this.groupLevel;
        }
        

        /**
        * Set the tokeniser state to skip to the end of the group.
        * Sets the state to TOKENISER_SKIP_GROUP and skipGroupLevel to the current group level.
        */
        public void SetTokeniserStateNormal() {
            this.SetTokeniserState(TOKENISER_NORMAL);
        }

        /**
        * Set the tokeniser state to skip to the end of the group.
        * Sets the state to TOKENISER_SKIP_GROUP and skipGroupLevel to the current group level.
        */
        public void SetTokeniserStateSkipGroup() {
            this.SetTokeniserState(TOKENISER_SKIP_GROUP);
            this.skipGroupLevel = this.groupLevel;
        }
        
        /**
        * Sets the number of bytes to skip and the state of the tokeniser.
        * 
        * @param numberOfBytesToSkip
        *           The numbere of bytes to skip in the file.
        */
        public void SetTokeniserSkipBytes(long numberOfBytesToSkip) {
            this.SetTokeniserState(TOKENISER_SKIP_BYTES);
            this.binSkipByteCount = numberOfBytesToSkip;
        }
        
        /**
        * Sets the number of binary bytes.
        * 
        * @param binaryCount
        *           The number of binary bytes.
        */
        public void SetTokeniserStateBinary(int binaryCount) {
            this.SetTokeniserState(TOKENISER_BINARY);
            this.binByteCount = binaryCount;
        }
        /**
        * Sets the number of binary bytes.
        * 
        * @param binaryCount
        *           The number of binary bytes.
        */
        public void SetTokeniserStateBinary(long binaryCount) {
            this.SetTokeniserState(TOKENISER_BINARY);
            this.binByteCount = binaryCount;
        }
        /**
        * Helper method to determin if conversion is TYPE_CONVERT
        * @return true if TYPE_CONVERT, otherwise false
        * @see com.lowagie.text.rtf.direct.RtfParser#TYPE_CONVERT
        */
        public bool IsConvert() {
            return (this.GetConversionType() == RtfParser.TYPE_CONVERT);
        }
        
        /**
        * Helper method to determin if conversion is TYPE_IMPORT_FULL or TYPE_IMPORT_FRAGMENT
        * @return true if TYPE_CONVERT, otherwise false
        * @see com.lowagie.text.rtf.direct.RtfParser#TYPE_IMPORT_FULL
        * @see com.lowagie.text.rtf.direct.RtfParser#TYPE_IMPORT_FRAGMENT
        */
        public bool IsImport() {
            return (IsImportFull() || this.IsImportFragment());
        }
        /**
        * Helper method to determin if conversion is TYPE_IMPORT_FULL
        * @return true if TYPE_CONVERT, otherwise false
        * @see com.lowagie.text.rtf.direct.RtfParser#TYPE_IMPORT_FULL
        */
        public bool IsImportFull() {
            return (this.GetConversionType() == RtfParser.TYPE_IMPORT_FULL);
        }
        /**
        * Helper method to determin if conversion is TYPE_IMPORT_FRAGMENT
        * @return true if TYPE_CONVERT, otherwise false
        * @see com.lowagie.text.rtf.direct.RtfParser#TYPE_IMPORT_FRAGMENT
        */
        public bool IsImportFragment() {
            return (this.GetConversionType() == RtfParser.TYPE_IMPORT_FRAGMENT);
        }
        /**
        * Helper method to indicate if this control word was a \* control word.
        * @return true if it was a \* control word, otherwise false
        */
        public bool GetExtendedDestination() {
            return this.currentState.isExtendedDestination;
        }
        /**
        * Helper method to set the extended control word flag.
        * @param value Boolean to set the value to.
        * @return isExtendedDestination.
        */
        public bool SetExtendedDestination(bool value) {
            this.currentState.isExtendedDestination = value;
            return this.currentState.isExtendedDestination;
        }

        /**
        * Get the logfile name.
        * 
        * @return the logFile
        */
        public String GetLogFile() {
            return logFile;
        }

        /**
        * Set the logFile name
        * 
        * @param logFile the logFile to set
        */
        public void SetLogFile(String logFile) {
            this.logFile = logFile;
        }
        /**
        * Set the logFile name
        * 
        * @param logFile the logFile to set
        */
        public void SetLogFile(String logFile, bool logAppend) {
            this.logFile = logFile;
            this.SetLogAppend(logAppend);
        }

        /**
        * Get flag indicating if logging is on or off.
        * 
        * @return the logging
        */
        public bool IsLogging() {
            return logging;
        }

        /**
        * Set flag indicating if logging is on or off
        * @param logging <code>true</code> to turn on logging, <code>false</code> to turn off logging.
        */
        public void SetLogging(bool logging) {
            this.logging = logging;
        }

        /**
        * @return the logAppend
        */
        public bool IsLogAppend() {
            return logAppend;
        }

        /**
        * @param logAppend the logAppend to set
        */
        public void SetLogAppend(bool logAppend) {
            this.logAppend = logAppend;
        }

    /*  
    *   Statistics
    *
        public void PrintStats(PrintStream out) {
            if (out == null) return;
            
            out.Println("");
            out.Println("Parser statistics:");
            out.Println("Process start date: " + startDate.ToLocaleString());
            out.Println("Process end date  : " + endDate.ToLocaleString());
            out.Println("  Elapsed time    : " + Long.ToString(endTime - startTime) + " milliseconds.");
            out.Println("Total bytes read  : " + Long.ToString(byteCount));
            out.Println("Open group count  : " + Long.ToString(openGroupCount));
            out.Print("Close group count : " + Long.ToString(closeGroupCount));
            out.Println(" (Groups Skipped): " + Long.ToString(groupSkippedCount));
            out.Print("Control word count: " + Long.ToString(ctrlWordCount));
            out.Print(" - Handled: " + Long.ToString(ctrlWordHandledCount));
            out.Print(" Not Handled: " + Long.ToString(ctrlWordNotHandledCount));
            out.Println(" Skipped: " + Long.ToString(ctrlWordSkippedCount));
            out.Println("Plain text char count: " + Long.ToString(characterCount));     
        }*/
    }
}