using System;
using iTextSharp.text.rtf;
using iTextSharp.text.rtf.document;
using iTextSharp.text.rtf.parser;
using iTextSharp.text.rtf.parser.destinations;
using iTextSharp.text.rtf.parser.properties;
using iTextSharp.text.rtf.direct;

/* $Id: RtfCtrlWordHandler.cs,v 1.3 2008/05/13 11:25:58 psoares33 Exp $
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
namespace iTextSharp.text.rtf.parser.ctrlwords {

    /**
    * <code>RtfCtrlWordBase</code> is the base class for all
    * control word handlers to extend from.
    * 
    * @author Howard Shank (hgshank@yahoo.com)
    * @since 2.0.8
    */
    public class RtfCtrlWordHandler {
        /**
        * Debug flag - internal use
        * @since 2.0.8
        */
        private static bool debug = false;

        /**
        * Local variable referencing the parser object. 
        * @since 2.0.8
        */
        protected RtfParser rtfParser = null;
        /**
        * The control word for this class.
        * @since 2.0.8
        */
        protected String ctrlWord = "";
        /**
        * The default value for this control word.
        * Not all control words use a default parameter value.
        * @since 2.0.8
        */
        protected int defaultParameterValue = 0;
        /**
        * Does this control word use the default value?
        * @since 2.0.8
        */
        protected bool passDefaultParameterValue = false;
        /**
        * Control Word type. Destination, toggle, value, etc.
        * @since 2.0.8
        */
        protected int ctrlWordType = RtfCtrlWordType.UNIDENTIFIED;
        /**
        * Class, property, etc.
        * @since 2.0.8
        */
        protected String specialHandler = "";
        /**
        * What version of the RTF spec the control word was introduced.
        * @since 2.0.8
        */
        protected float rtfVersionSupported = -1.0f;    // -1.0 unknown. Each class should override this as implemented.
        /**
        * The control word as parsed by the parser.
        * @since 2.0.8
        */
        protected RtfCtrlWordData ctrlWordData = null;
        /**
        * String containing the value of "{" or "" (blank) depending on if this is the
        * first control word in a group.
        * @since 2.0.8
        */
        protected String groupPrefix = "";
        /**
        * The prefix for all control words.
        * @since 2.0.8
        */
        protected String ctrlWordPrefix = "\\";
        /**
        * The prefix for all control words.
        * @since 2.0.8
        */
        protected String ctrlWordSuffix = " ";
        
        /**
        * Constructor:
        *
        * @param rtfParser
        *      The parser for this control word.
        * @param ctrlWord
        *      The string value of this control word.
        * @param defaultParameterValue
        *      The default value of this control word. Not all control words have values.
        * @param passDefaultParameterValue
        *      Flag indicating if this control word should use the default value.
        * @param ctrlWordType
        *      Indicator of the type of control word this is. DESTINATION|DESTINATION_EX|VALUE|FLAG|TOGGLE|SYMBOL
        * @param prefix
        *      String to prefix the ctrl word with. "\" or "\*\" are the 2 used values.
        * @param suffix
        *      String to add as suffix to the ctrl word. " " and "" are the 2 used values.
        * @param specialHandler
        *      If TOGGLE then the property name as String (propertyGroup.propertyName format ex. "character.bold")
        *      If FLAG then the property name as String (propertyGroup.propertyName format ex. "character.bold")
        *      If VALUE then the property name as String (propertyGroup.propertyName format ex. "character.bold")
        *      If SYMBOL then the character to use for substitution as String
        *      If DESTINATION|DESTINATION_EX then the RtfDestination class name as String
        * 
        * @since 2.0.8
        */
        public RtfCtrlWordHandler(RtfParser rtfParser, String ctrlWord, int defaultParameterValue, bool passDefaultParameterValue, 
                int ctrlWordType, String prefix, String suffix, String specialHandler) {
            this.rtfParser = rtfParser;
            this.ctrlWord = ctrlWord;
            this.defaultParameterValue = defaultParameterValue;
            this.passDefaultParameterValue = passDefaultParameterValue;
            this.ctrlWordType = ctrlWordType;
            this.ctrlWordPrefix = prefix;
            this.ctrlWordSuffix = suffix;
            this.specialHandler = specialHandler;

            if (this.ctrlWordType == RtfCtrlWordType.DESTINATION || this.ctrlWordType == RtfCtrlWordType.DESTINATION_EX){
                if (this.specialHandler == null) {
                    this.specialHandler = "RtfDestinationNull";
                }
                String arg1 = ""; // stylesheet value - S, CS, TS
                RtfDestinationMgr.AddDestination(this.ctrlWord, new Object[] { this.specialHandler, arg1 });            
            } else {
                if (this.ctrlWordType == RtfCtrlWordType.SYMBOL){
                    
                } else {
                    if (this.specialHandler == null) {
                        this.specialHandler = this.ctrlWord;    // if null, make the property the name of the ctrl word
                    } else {
                        if (this.specialHandler.Length > 1 && this.specialHandler.EndsWith(".")) {
                            this.specialHandler += this.ctrlWord;   // if string length>1 and ends with a period, it's a group. Add ctrlWord
                        }
                    }
                }
            }
        }
        
        /**
        * The primary control word handler method.
        * Called by the parser once it has a control word and parameter if applicable.
        * 
        * @param ctrlWordDataIn
        *      The control word and associated parameter if applicable.
        * @return
        *      <code>true</code> or <code>false</code> if the control word was handled.
        * @since 2.0.8
        */
        public bool HandleControlword(RtfCtrlWordData ctrlWordDataIn){
            bool result = false;
            this.ctrlWordData = ctrlWordDataIn;
            RtfDestination dest = null;
            bool handled = false;
            
            this.ctrlWordData.prefix  = this.ctrlWordPrefix;
            this.ctrlWordData.suffix  = this.ctrlWordSuffix;
            this.ctrlWordData.newGroup = this.rtfParser.GetState().newGroup;
            this.ctrlWordData.ctrlWordType = this.ctrlWordType;
            this.ctrlWordData.specialHandler = this.specialHandler;
            
            if (!this.ctrlWordData.hasParam && this.passDefaultParameterValue) {
                this.ctrlWordData.hasParam = true;
                this.ctrlWordData.param = this.defaultParameterValue.ToString();
            }

            if (debug) {
                PrintDebug("handleKeyword: [" + this.ctrlWordData.ctrlWord + "] param=" + ctrlWordDataIn.param);
                RtfParser.OutputDebug(this.rtfParser.GetRtfDocument(), this.rtfParser.GetLevel()+1, "RtfCtrlWordHandler debug Start: " + this.ctrlWordData.ctrlWord + " ");
            }
            if (this.ctrlWordData.ctrlWord.Equals("*")) {
                return true;
            }
            
            if (!BeforeControlWord()) {
                return true;
            }
            
            switch (this.ctrlWordType) {
            case RtfCtrlWordType.FLAG:
            case RtfCtrlWordType.TOGGLE:
            case RtfCtrlWordType.VALUE:
                dest = (RtfDestination)this.rtfParser.GetCurrentDestination();
                if (dest != null) {
                    handled = dest.HandleControlWord(this.ctrlWordData);
                }
                break;
            
            case RtfCtrlWordType.SYMBOL:
                dest = (RtfDestination)this.rtfParser.GetCurrentDestination();
                if (dest != null) {
                    String data = null;
                    // if doing an import, then put the control word in the output stream through the character handler
                    if (this.rtfParser.IsImport()) {
                        data = this.ctrlWordPrefix + this.ctrlWordData.ctrlWord + this.ctrlWordSuffix;
                    }
                    if (this.rtfParser.IsConvert()) {
                        data = this.specialHandler;
                    }
                    
                    // If there is a substitute character, process the character.
                    // If no substitute character, then provide special handling in the destination for the ctrl word. 
                    if (data != null) {
                        foreach (char cc in data.ToCharArray()) {
                            handled = dest.HandleCharacter((int)cc);
                        }
                    } else {
                        handled = dest.HandleControlWord(this.ctrlWordData);
                    }
                }
                break;

            case RtfCtrlWordType.DESTINATION_EX:
            case RtfCtrlWordType.DESTINATION:
                // set the destination
                int x=0;
                if(this.ctrlWord == "shppict" || this.ctrlWord == "nonshppict") {
                    x++;
                }
                handled = this.rtfParser.SetCurrentDestination(this.ctrlWord);
                // let destination handle the ctrl word now.
                dest = (RtfDestination)this.rtfParser.GetCurrentDestination();
                if(dest != null) {
                    if(dest.GetNewTokeniserState() == RtfParser.TOKENISER_IGNORE_RESULT) {
                        handled = dest.HandleControlWord(this.ctrlWordData);
                    }
                    else {
                        this.rtfParser.SetTokeniserState(dest.GetNewTokeniserState());
                    }
                }
                break;
            }

            AfterControlWord();
            
            if (debug) {
                RtfParser.OutputDebug(this.rtfParser.GetRtfDocument(), this.rtfParser.GetLevel()+1, "RtfCtrlWordHandler debug End: " + this.ctrlWordData.ctrlWord + " ");
            }

            return result;
        }
        
        /**
        * Pre-processing before the control word.
        * 
        * If return value is true, no further processing will be performed on
        * this control word.
        * 
        * @return <code>false</code> = stop processing, <code>true</code> = continue processing 
        * @since 2.0.8
        */
        //Primary purpose is for \* control word and event handling.
        protected bool BeforeControlWord() {
            if (debug) PrintDebug("beforeControlWord");
            // TODO: This is where events would be triggered
            return true;
        }
        /**
        * Handle the control word.
        * 
        * @return <code>true</code> if control word was handled, <code>false</code> if it was not handled.
        * @since 2.0.8
        */
        protected bool OnControlWord() {
            if (debug) PrintDebug("onCtrlWord");
            // TODO: This is where events would be triggered
            return false;
        }
        /**
        * Post-processing after the control word.
        * 
        * @return <code>false</code> = stop processing, <code>true</code> = continue processing
        * @since 2.0.8
        */
        protected bool AfterControlWord() {
            if (debug) PrintDebug("afterControlWord");
            // TODO: This is where events would be triggered
            return true;
        }
        
    //  public String CtrlWordString() {
    //      //String out = ctrlWordPrefix + this.ctrlWord;
    //      String out = "";
    //      if (this.bExtendedDestination) {
    //          out += "\\*";
    //      }
    //      out = ctrlWordPrefix + this.ctrlWordData.ctrlWord;
    //      if (this.ctrlWordData.hasParam) {
    //          if (this.ctrlWordData.isNeg) out += "-";
    //          out += this.ctrlWordData.param;
    //      } else {
    //          if (this.passDefaultParameterValue == true) {
    //              out += Integer.ToString(this.defaultParameterValue);
    //          } 
    //      }
    //      out += this.ctrlWordSuffix;
    //      return out;
    //  }
        
        /**
        * Debug function to print class/method
        * @param txt The <code>String</code> to output.
        * @since 2.0.8
        */
        private void PrintDebug(String txt) {
            Console.Out.WriteLine(this.GetType().Name + " : " + txt);
        }
    }
}