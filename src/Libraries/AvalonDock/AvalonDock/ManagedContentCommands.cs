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
    /// Defines commands shared beteween all contents (Dockable or Documents)
    /// </summary>
    public sealed class ManagedContentCommands
    {
        private static object syncRoot = new object();


        private static RoutedUICommand closeCommand = null;

        /// <summary>
        /// This command closes the content
        /// </summary>
        public static RoutedUICommand Close
        {
            get
            {
                lock (syncRoot)
                {
                    if (null == closeCommand)
                    {
                        closeCommand = new RoutedUICommand(AvalonDock.Properties.Resources.ManagedContentCommands_Close, "Close", typeof(ManagedContentCommands));
                    }
                }
                return closeCommand;
            }
        }

        private static RoutedUICommand hideCommand = null;

        /// <summary>
        /// This command hides the content
        /// </summary>
        public static RoutedUICommand Hide
        {
            get
            {
                lock (syncRoot)
                {
                    if (null == hideCommand)
                    {
                        hideCommand = new RoutedUICommand(AvalonDock.Properties.Resources.ManagedContentCommands_Hide, "Hide", typeof(ManagedContentCommands));
                    }
                }
                return hideCommand;
            }
        }

        private static RoutedUICommand showCommand = null;

        /// <summary>
        /// This command shows the content
        /// </summary>
        /// <remarks>How content is shown by default depends from the type of content. A <see cref="DockableContent"/> is shown as docked pane, instead
        /// a <see cref="DocumentContent"/> is shown as tabbed document</remarks>
        public static RoutedUICommand Show
        {
            get
            {
                lock (syncRoot)
                {
                    if (null == showCommand)
                    {
                        showCommand = new RoutedUICommand(AvalonDock.Properties.Resources.ManagedContentCommands_Show, "Show", typeof(ManagedContentCommands));
                    }
                }
                return showCommand;
            }
        }


        private static RoutedUICommand activateCommand = null;

        /// <summary>
        /// This command activate the commands (i.e. select it inside the conatiner pane)
        /// </summary>
        /// <remarks>Activating a content means essentially putting it in evidence. For a content that is auto-hidden this command opens a flyout window containing the content.</remarks>
        public static RoutedUICommand Activate
        {
            get
            {
                lock (syncRoot)
                {
                    if (null == activateCommand)
                    {
                        activateCommand = new RoutedUICommand(AvalonDock.Properties.Resources.ManagedContentCommands_Activate, "Activate", typeof(ManagedContentCommands));
                    }
                }
                return activateCommand;
            }
        }
    }
}
