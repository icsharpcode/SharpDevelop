//Copyright (c) 2007-2010, Adolfo Marinucci
//All rights reserved.

//Redistribution and use in source and binary forms, with or without modification, 
//are permitted provided that the following conditions are met:
//
//* Redistributions of source code must retain the above copyright notice, 
//  this list of conditions and the following disclaimer.
//* Redistributions in binary form must reproduce the above copyright notice, 
//  this list of conditions and the following disclaimer in the documentation 
//  and/or other materials provided with the distribution.
//* Neither the name of Adolfo Marinucci nor the names of its contributors may 
//  be used to endorse or promote products derived from this software without 
//  specific prior written permission.
//
//THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
//AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED 
//WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. 
//IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, 
//INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, 
//PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
//HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, 
//OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, 
//EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE. 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace AvalonDock
{
    /// <summary>
    /// Contains a list of commands that can be applied to a <see cref="DocumentContent"/>
    /// </summary>
    public sealed class DocumentContentCommands
    {
        static object syncRoot = new object();

        private static RoutedUICommand _floatingDocumentCommand = null;

        /// <summary>
        /// Shows the <see cref="DocumentContent"/> as a floating window document 
        /// </summary>
        public static RoutedUICommand FloatingDocument
        {
            get
            {
                lock (syncRoot)
                {
                    if (null == _floatingDocumentCommand)
                    {
                        _floatingDocumentCommand = new RoutedUICommand(AvalonDock.Properties.Resources.DocumentContentCommands_FloatingDocument, "FloatingDocument", typeof(DocumentContentCommands));
                    }
                }
                return _floatingDocumentCommand;
            }
        }


        private static RoutedUICommand _tabbedDocumentCommand = null;

        /// <summary>
        /// Shows the <see cref="DocumentContent"/> as a tabbed document 
        /// </summary>
        public static RoutedUICommand TabbedDocument
        {
            get
            {
                lock (syncRoot)
                {
                    if (null == _tabbedDocumentCommand)
                    {
                        _tabbedDocumentCommand = new RoutedUICommand(AvalonDock.Properties.Resources.DocumentContentCommands_TabbedDocument, "TabbedDocument", typeof(DocumentContentCommands));
                    }
                }
                return _tabbedDocumentCommand;
            }
        }        

    }
}
