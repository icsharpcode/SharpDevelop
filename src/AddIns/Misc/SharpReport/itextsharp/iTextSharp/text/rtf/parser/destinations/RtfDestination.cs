using System;
using System.Collections;
using iTextSharp.text.rtf.parser;
using iTextSharp.text.rtf.parser.ctrlwords;
/*
 * $Id: RtfDestination.cs,v 1.2 2008/05/13 11:26:00 psoares33 Exp $
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
    * <code>RtfDestination</code> is the base class for destinations according
    * to the RTF Specification. All destinations must extend from this class.
    * 
    * @author Howard Shank (hgshank@yahoo.com
    * 
    * @since 2.0.8
    */
    public abstract class RtfDestination {
        /** Parser object */
        protected RtfParser rtfParser = null;
        
        /** Is data in destination modified? */
        protected bool modified = false;

        /** The last control word handled by this destination */
        protected RtfCtrlWordData lastCtrlWord = null;
        
        /** The <code>RtfDestinationListener</code>. */
        private static ArrayList listeners = new ArrayList();
        
        /**
        * Constructor.
        */
        public RtfDestination() {
            rtfParser = null;
        }
        /**
        * Constructor
        * @param parser <code>RtfParser</code> object.
        */
        public RtfDestination(RtfParser parser) {
            this.rtfParser = parser;
        }
        /**
        * Set the parser to use with the RtfDestination object.
        * 
        * @param parser The RtfParser object.
        */
        public virtual void SetParser(RtfParser parser) {
            if (this.rtfParser != null && this.rtfParser.Equals(parser)) return;
            this.rtfParser = parser;
        }
        /**
        * Clean up when destination is closed.
        * @return true if handled, false if not handled
        */
        public abstract bool CloseDestination();
        /**
        * Handle a new subgroup contained within this group
        * @return true if handled, false if not handled
        */
        public abstract bool HandleOpeningSubGroup();
        /**
        * Clean up when group is closed.
        * @return true if handled, false if not handled
        */
        public abstract bool HandleCloseGroup();

        /**
        * Setup when group is opened.
        * @return true if handled, false if not handled
        */
        public abstract bool HandleOpenGroup();
        /**
        * Handle text for this destination
        * @return true if handled, false if not handled
        */
        public abstract bool HandleCharacter(int ch);
        /**
        * Handle control word for this destination
        * @param ctrlWordData The control word and parameter information object
        * @return true if handled, false if not handled
        */
        public abstract bool HandleControlWord(RtfCtrlWordData ctrlWordData);
        /**
        * Method to set this object to the default values. Must be implemented in child class.
        */
        public abstract void SetToDefaults();

        /**
        * Method to indicate if data in this destination has changed.
        * @return true if modified, false if not modified.
        */
        public bool IsModified() {
            return modified;
        }

        // listener methods

        /**
        * Adds a <CODE>RtfDestinationListener</CODE> to the <CODE>RtfDestinationMgr</CODE>.
        *
        * @param listener
        *            the new RtfDestinationListener.
        */
        public bool AddListener(IRtfDestinationListener listener) {
            listeners.Add(listener);
            return true;
        }

        /**
        * Removes a <CODE>RtfDestinationListener</CODE> from the <CODE>RtfDestinationMgr</CODE>.
        *
        * @param listener
        *            the RtfCtrlWordListener that has to be removed.
        */
        public bool RemoveListener(IRtfDestinationListener listener) {
            int i = listeners.IndexOf(listener);
            if (i >= 0) {
                listeners.RemoveAt(i);
                return true;
            }
            return false;
        }
        
        protected RtfCtrlWordData BeforeCtrlWord(RtfCtrlWordData ctrlWordData) {
            foreach (IRtfDestinationListener listener in listeners) {
                listener.BeforeCtrlWord(ctrlWordData);
            }
            return null;
        }
        /**
        * 
        */
        protected  RtfCtrlWordData OnCtrlWord(RtfCtrlWordData ctrlWordData){
            foreach (IRtfDestinationListener listener in listeners) {
                listener.OnCtrlWord(ctrlWordData);
            }
            return null;
        }

        /**
        * 
        */
        protected  RtfCtrlWordData AfterCtrlWord(RtfCtrlWordData ctrlWordData){
            foreach (IRtfDestinationListener listener in listeners) {
                listener.AfterCtrlWord(ctrlWordData);
            }
            return null;
        }

        /**
        * 
        */
        protected int BeforeCharacter(int ch){
            foreach (IRtfDestinationListener listener in listeners) {
                listener.BeforeCharacter(ch);
            }
            return 0;
        }

        /**
        * 
        */
        protected int OnCharacter(int ch){
            foreach (IRtfDestinationListener listener in listeners) {
                listener.OnCharacter(ch);
            }
            return 0;
        }

        /**
        * 
        */
        protected int AfterCharacter(int ch){
            foreach (IRtfDestinationListener listener in listeners) {
                listener.AfterCharacter(ch);
            }
            return 0;
        }

        /**
        * 
        * @return
        */
        protected  bool OnOpenGroup(){
            foreach (IRtfDestinationListener listener in listeners) {
                listener.OnOpenGroup();
            }
            return true;
        }

        /**
        * 
        * @return
        */
        protected  bool OnCloseGroup(){
            foreach (IRtfDestinationListener listener in listeners) {
                listener.OnCloseGroup();
            }
            return true;
        }

        public virtual int GetNewTokeniserState() {
            return RtfParser.TOKENISER_IGNORE_RESULT;
        }
   }
}