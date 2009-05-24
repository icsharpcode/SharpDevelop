/* Copyright (c) 2008, Dr. WPF
 * All rights reserved.
 *
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions are met:
 *
 *   * Redistributions of source code must retain the above copyright
 *     notice, this list of conditions and the following disclaimer.
 * 
 *   * Redistributions in binary form must reproduce the above copyright
 *     notice, this list of conditions and the following disclaimer in the
 *     documentation and/or other materials provided with the distribution.
 * 
 *   * The name Dr. WPF may not be used to endorse or promote products
 *     derived from this software without specific prior written permission.
 *
 * THIS SOFTWARE IS PROVIDED BY Dr. WPF ``AS IS'' AND ANY
 * EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
 * WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
 * DISCLAIMED. IN NO EVENT SHALL Dr. WPF BE LIABLE FOR ANY
 * DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
 * (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
 * LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
 * ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
 * (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
 * SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Collections.Specialized;

namespace DrWPF.Windows.Controls
{
    /// <summary>
    /// This panel extends ConceptualPanel by ensuring that its conceptual children are also "logical" children.  
    /// Because certain things like property inheritance and resource resolution work through the logical
    /// tree, this allows the disconnected visuals to be connected to the panel's ancestor tree
    /// in a logical manner without being part of it's visual tree. 
    /// </summary>
    public abstract class LogicalPanel : ConceptualPanel
    {
        protected sealed override void OnChildAdded(UIElement child)
        {
            // if the child does not have a logical parent, assume the role
            if (LogicalTreeHelper.GetParent(child) == null)
            {
                AddLogicalChild(child);
            }
            OnLogicalChildrenChanged(child, null);
        }

        protected sealed override void OnChildRemoved(UIElement child)
        {
            // if this panel is the logical parent, remove that relationship
            if (LogicalTreeHelper.GetParent(child) == this)
            {
                RemoveLogicalChild(child);
            }
            OnLogicalChildrenChanged(null, child);
        }

        /// <summary>
        /// This class uses the OnLogicalChildrenChanged method to provide notification to descendants 
        /// when its logical children change.  Note that this is intentionally
        /// similar to the OnVisualChildrenChanged approach supported by all visuals.
        /// </summary>
        /// <param name="childAdded"></param>
        /// <param name="childRemoved"></param>
        protected virtual void OnLogicalChildrenChanged(UIElement childAdded, UIElement childRemoved)
        {
        }
    }
}
