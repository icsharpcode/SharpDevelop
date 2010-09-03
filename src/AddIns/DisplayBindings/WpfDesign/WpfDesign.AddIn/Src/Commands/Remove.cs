// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Core;

using ICSharpCode.WpfDesign.Designer;

namespace ICSharpCode.WpfDesign.AddIn.Commands
{
	/// <summary>
	/// Removes selected element from the designer.
	/// </summary>
	class Remove : AbstractMenuCommand
    {
        public override void Run()
        {
            var surface = Owner as DesignSurface;
            if (surface != null)
                surface.Delete();
        }
    }
	
	/// <summary>
	/// Provides implementation of <see cref="IConditionEvaluator"/> for <see cref="Remove"/>.
	/// </summary>
    class IsRemoveEnabled : IConditionEvaluator
    {
        public bool IsValid(object owner, Condition condition)
        {
            var surface = owner as DesignSurface;
            if (surface != null)
                return surface.CanDelete();
            return false;
        }
    }
}
