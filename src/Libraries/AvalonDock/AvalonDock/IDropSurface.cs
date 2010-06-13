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
using System.Text;
using System.Windows;

namespace AvalonDock
{
    /// <summary>
    /// Defines an interface that must be implemented by objects that can host dragged panes
    /// </summary>
    internal interface IDropSurface
    {
        /// <summary>
        /// Gets a value indicating if this area is avilable for drop a dockable pane
        /// </summary>
        bool IsSurfaceVisible { get; }

        /// <summary>
        /// Gets the sensible area for drop a pane
        /// </summary>
        Rect SurfaceRectangle { get; }

        /// <summary>
        /// Called by <see cref="DragPaneService"/> when user dragged pane enter this surface
        /// </summary>
        /// <param name="point">Location of the mouse</param>
        void OnDragEnter(Point point);

        /// <summary>
        /// Called by <see cref="DragPaneService"/> when user dragged pane is over this surface
        /// </summary>
        /// <param name="point">Location of the mouse</param>
        void OnDragOver(Point point);

        /// <summary>
        /// Called by <see cref="DragPaneService"/> when user dragged pane leave this surface
        /// </summary>
        /// <param name="point">Location of the mouse</param>
        void OnDragLeave(Point point);

        /// <summary>
        /// Called by <see cref="DragPaneService"/> when user drops a pane to this surface
        /// </summary>
        /// <param name="point">Location of the mouse</param>
        bool OnDrop(Point point);
    }
}
