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
    /// Defines commands that can be executed against a DockableContent
    /// </summary>
    public sealed class DockableContentCommands
    {
        static object syncRoot = new object();
        
        private static RoutedUICommand documentCommand = null;

        /// <summary>
        /// Shows the DockableContent as a tabbed document
        /// </summary>
        public static RoutedUICommand ShowAsDocument
        {
            get
            {
                lock (syncRoot)
                {
                    if (null == documentCommand)
                    {
                        documentCommand = new RoutedUICommand(AvalonDock.Properties.Resources.DockableContentCommands_ShowAsDocument, "Document", typeof(DockableContentCommands));
                    }
                }
                return documentCommand;
            }
        }


        private static RoutedUICommand floatingWindowCommand = null;
        
        /// <summary>
        /// Shows the <see cref="DockableContent"/> as a <see cref="FloatingWindow"/> which overlays the <see cref="DockingManager"/>
        /// </summary>
        /// <remarks>A floating window can't be redocked to the docking manager.</remarks>
        public static RoutedUICommand FloatingWindow
        {
            get
            {
                lock (syncRoot)
                {
                    if (null == floatingWindowCommand)
                    {
                        floatingWindowCommand = new RoutedUICommand(AvalonDock.Properties.Resources.DockableContentCommands_FloatingWindow, "FloatingWindow", typeof(DockableContentCommands));
                    }
                }
                return floatingWindowCommand;
            }
        }

        private static RoutedUICommand dockableWindowCommand = null;

        /// <summary>
        /// Shows the <see cref="DockableContent"/> as a <see cref="DockableFloatingWindow"/> which overlays the <see cref="DockingManager"/>
        /// </summary>
        /// <remarks>A floating window can't be redocked to the docking manager.</remarks>
        public static RoutedUICommand DockableFloatingWindow
        {
            get
            {
                lock (syncRoot)
                {
                    if (null == dockableWindowCommand)
                    {
                        dockableWindowCommand = new RoutedUICommand(AvalonDock.Properties.Resources.DockableContentCommands_DockableFloatingWindow, "DockableFloatingWindow", typeof(DockableContentCommands));
                    }
                }
                return dockableWindowCommand;
            }
        }

        private static RoutedUICommand autoHideCommand = null;

        /// <summary>
        /// Switch the state of a <see cref="DockableContent"/> from <see cref="DockableContentState.AutoHidden"/> to <see cref="DockableContentState.Docked"/> and viceversa
        /// </summary>
        /// <remarks>This command has the same effect as applying command <see cref="DockablePaneCommands.ToggleAutoHide"/> to container pane
        public static RoutedUICommand ToggleAutoHide
        {
            get
            {
                lock (syncRoot)
                {
                    if (null == autoHideCommand)
                    {
                        autoHideCommand = new RoutedUICommand(AvalonDock.Properties.Resources.DockableContentCommands_ToggleAutoHide, "AutoHide", typeof(DockableContentCommands));
                    }
                }
                return autoHideCommand;
            }
        }



    }
}
