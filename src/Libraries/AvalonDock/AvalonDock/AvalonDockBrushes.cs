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

namespace AvalonDock
{
    /// <summary>
    /// Defines a list of brushes used by AvalonDock templates
    /// </summary>
    public enum AvalonDockBrushes
    {
        /// <summary>
        /// Default brush for DockingManager background
        /// </summary>
        DefaultBackgroundBrush,

        /// <summary>
        /// Brush used for the title background of a <see cref="DockablePane"/>.
        /// </summary>
        DockablePaneTitleBackground,

        /// <summary>
        /// Brush used for the title background of a <see cref="DockablePane"/> when is focused.
        /// </summary>
        DockablePaneTitleBackgroundSelected,

        /// <summary>
        /// Brush used for the title foreground of a <see cref="DockablePane"/>.
        /// </summary>
        DockablePaneTitleForeground,

        /// <summary>
        /// Brush used for the title foreground of a <see cref="DockablePane"/> when is focused.
        /// </summary>
        DockablePaneTitleForegroundSelected,

        /// <summary>
        /// Brush used for the background of the pane command pins.
        /// </summary>
        PaneHeaderCommandBackground,

        /// <summary>
        /// Brush used for the border of the pane command pins.
        /// </summary>
        PaneHeaderCommandBorderBrush,

        /// <summary>
        /// Brush used for the background of a document header.
        /// </summary>
        DocumentHeaderBackground,

        /// <summary>
        /// Brush used for the foreground of a document header.
        /// </summary>
        DocumentHeaderForeground,

        /// <summary>
        /// Brush used for fonts while a document header is selected but not activated
        /// </summary>
        DocumentHeaderForegroundSelected,

        /// <summary>
        /// Brush used for fonts while a document header is selected and activated
        /// </summary>
        DocumentHeaderForegroundSelectedActivated,

        /// <summary>
        /// Brush used for the background of a document header when selected (<see cref="ManagedContent.IsActiveContent"/>).
        /// </summary>
        DocumentHeaderBackgroundSelected,

        /// <summary>
        /// Brush used for the background of a document header when active and selected (<see cref="ManagedContent.IsActiveDocument"/>).
        /// </summary>
        DocumentHeaderBackgroundSelectedActivated,

        /// <summary>
        /// Brush used for the background of a document header when mouse is over it.
        /// </summary>
        DocumentHeaderBackgroundMouseOver,

        /// <summary>
        /// Brush used for the border brush of a document header when mouse is over it (but is not selected).
        /// </summary>
        DocumentHeaderBorderBrushMouseOver,

        /// <summary>
        /// Brush for the document header border
        /// </summary>
        DocumentHeaderBorder,

        /// <summary>
        /// Brush for the document header border when contains a document selected
        /// </summary>
        DocumentHeaderBorderSelected,

        /// <summary>
        /// Brush for the document header border when contains a document selected and activated
        /// </summary>
        DocumentHeaderBorderSelectedActivated,



        NavigatorWindowTopBackground,
        
        NavigatorWindowTitleForeground,

        NavigatorWindowDocumentTypeForeground,

        NavigatorWindowInfoTipForeground,

        NavigatorWindowForeground,

        NavigatorWindowBackground,

        NavigatorWindowSelectionBackground,

        NavigatorWindowSelectionBorderbrush,
        
        NavigatorWindowBottomBackground,
    }
}
