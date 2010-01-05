using System;
using System.Collections;
using iTextSharp.text.rtf.parser;
/* 
 * $Id: RtfCtrlWordMgr.cs,v 1.2 2008/05/13 11:25:59 psoares33 Exp $
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

namespace iTextSharp.text.rtf.parser.ctrlwords {

    /**
    * <code>RtfCtrlWordMgr</code> handles the dispatching of control words from
    * the table of known control words.
    * 
    * @author Howard Shank (hgshank@yahoo.com)
    * @since 2.0.8
    */
    public sealed class RtfCtrlWordMgr {
        public static bool debug = false;
        public static bool debugFound = false;
        public static bool debugNotFound = true;
        private PushbackStream reader = null;
        private RtfParser rtfParser = null;
        private RtfCtrlWordMap ctrlWordMap = null;
        
        /** The <code>RtfCtrlWordListener</code>. */
        private ArrayList listeners = new ArrayList();

    //  // TIMING DEBUG INFO
    //  private long endTime = 0;
    //  private Date endDate = null;        
    //  private long endFree = 0;
    //  private DecimalFormat df = new DecimalFormat("#,##0");
    //  private Date startDate = new Date();
    //  private long startTime = System.CurrentTimeMillis();
    //  private long startFree = Runtime.GetRuntime().FreeMemory();
        
        /**
        * Constructor
        * @param rtfParser The parser object this manager works with.
        * @param reader the PushbackReader from the tokeniser.
        */
        public RtfCtrlWordMgr(RtfParser rtfParser, PushbackStream reader) {
            this.rtfParser = rtfParser; // set the parser
            this.reader = reader;   // set the reader value
            ctrlWordMap = new RtfCtrlWordMap(rtfParser);
            
    //      // TIMING DEBUG INFO
    //      endFree = Runtime.GetRuntime().FreeMemory();
    //      endTime = System.CurrentTimeMillis();
    //      endDate = new Date();
    //      Console.Out.WriteLine("RtfCtrlWordMgr start date: " + startDate.ToLocaleString());
    //      Console.Out.WriteLine("RtfCtrlWordMgr end date  : " + endDate.ToLocaleString());
    //      Console.Out.WriteLine("  Elapsed time    : " + Long.ToString(endTime - startTime) + " milliseconds.");
    //      Console.Out.WriteLine("Begin Constructor RtfCtrlWordMgr , free mem is " + df.Format(startFree / 1024) + "k");
    //      Console.Out.WriteLine("End Constructor RtfCtrlWordMgr , free mem is " + df.Format(endFree / 1024) + "k");
    //        Console.Out.WriteLine("RtfCtrlWordMgr used approximately " + df.Format((startFree - endFree) / 1024) + "k");
        }
        
        /**
        * Internal to control word manager class.
        * 
        * @param ctrlWordData The <code>RtfCtrlWordData</code> object with control word and param
        * @param groupLevel The current document group parsing level
        * @return errOK if ok, otherwise an error code.
        */
        public int HandleKeyword(RtfCtrlWordData ctrlWordData, int groupLevel) {
            //TODO: May be used for event handling.
            int result = RtfParser.errOK;
            
            // Call before handler event here
            BeforeCtrlWord(ctrlWordData);
            
            result = DispatchKeyword(ctrlWordData, groupLevel);
            
            // call after handler event here
            AfterCtrlWord(ctrlWordData);
            
            return result;
        }
        
        /**
        * Dispatch the token to the correct control word handling object.
        *  
        * @param ctrlWordData The <code>RtfCtrlWordData</code> object with control word and param
        * @param groupLevel The current document group parsing level
        * @return errOK if ok, otherwise an error code.
        */
        private int DispatchKeyword(RtfCtrlWordData ctrlWordData, int groupLevel) {
            int result = RtfParser.errOK;
            if (ctrlWordData != null) {
                RtfCtrlWordHandler ctrlWord = ctrlWordMap.GetCtrlWordHandler(ctrlWordData.ctrlWord);
                if (ctrlWord != null) {
                    ctrlWord.HandleControlword(ctrlWordData);
                    if (debug && debugFound) {
                        Console.Out.WriteLine("Keyword found:" +
                            " New:" + ctrlWordData.ctrlWord + 
                            " Param:" + ctrlWordData.param + 
                            " bParam=" + ctrlWordData.hasParam.ToString());
                    }
                } else {
                    result = RtfParser.errCtrlWordNotFound;
                    //result = RtfParser2.errAssertion;
                    if (debug && debugNotFound) {
                        Console.Out.WriteLine("Keyword unknown:" + 
                            " New:" + ctrlWordData.ctrlWord + 
                            " Param:" + ctrlWordData.param + 
                            " bParam=" + ctrlWordData.hasParam.ToString());
                    }
                }   
            }
            return result;
        }
        

        // listener methods

        /**
        * Adds a <CODE>RtfCtrlWordListener</CODE> to the <CODE>RtfCtrlWordMgr</CODE>.
        *
        * @param listener
        *            the new RtfCtrlWordListener.
        */
        public void AddRtfCtrlWordListener(IRtfCtrlWordListener listener) {
            listeners.Add(listener);
        }

        /**
        * Removes a <CODE>RtfCtrlWordListener</CODE> from the <CODE>RtfCtrlWordMgr</CODE>.
        *
        * @param listener
        *            the RtfCtrlWordListener that has to be removed.
        */
        public void RemoveRtfCtrlWordListener(IRtfCtrlWordListener listener) {
            listeners.Remove(listener);
        }
        
        private bool BeforeCtrlWord(RtfCtrlWordData ctrlWordData) {
            foreach (IRtfCtrlWordListener listener in listeners) {
                listener.BeforeCtrlWord(ctrlWordData);
            }
            return true;
        }
        
        private bool OnCtrlWord(RtfCtrlWordData ctrlWordData) {
            foreach (IRtfCtrlWordListener listener in listeners) {
                listener.OnCtrlWord(ctrlWordData);
            }
            return true;
        }
        
        private bool AfterCtrlWord(RtfCtrlWordData ctrlWordData) {
            foreach (IRtfCtrlWordListener listener in listeners) {
                listener.AfterCtrlWord(ctrlWordData);
            }
            return true;
        }
    }
}