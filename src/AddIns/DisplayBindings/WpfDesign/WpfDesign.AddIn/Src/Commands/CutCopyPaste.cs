// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Core;
using ICSharpCode.WpfDesign.Designer;

namespace ICSharpCode.WpfDesign.AddIn.Commands
{
	/// <summary>
	/// Invokes Cut command on the Design surface.
	/// </summary>
	class Cut : AbstractMenuCommand
	{
		public override void Run()
		{
			var surface = Owner as DesignSurface;
            if(surface!=null) 
            	surface.Cut();
		}
	}
	
	/// <summary>
	/// Invokes Copy command on the Design surface.
	/// </summary>
	class Copy : AbstractMenuCommand
    {
        public override void Run()
        {
            var surface = Owner as DesignSurface;
            if (surface != null)
            	surface.Copy();
            
        }
    }
	
	/// <summary>
	/// Invokes Paste operation on the Design surface.
	/// </summary>
	class Paste : AbstractMenuCommand
    {
        public override void Run()
        {
            var surface = Owner as DesignSurface;
            if (surface != null)
                surface.Paste();
        }
    }
	
	/// <summary>
	/// Provides implementation of <see cref="IConditionEvaluator"/> for <see cref="Cut"/> and <see cref="Copy"/>.
	/// </summary>
	class IsCutCopyEnabled : IConditionEvaluator 
    {
        public bool IsValid(object owner, Condition condition)
        {
            var surface = owner as DesignSurface;
            if(surface!=null) {
                return surface.CanCopyOrCut();
            }
            return false;
        }
    }
	
	/// <summary>
	/// Provides implementation of <see cref="IConditionEvaluator"/> for <see cref="Paste"/>.
	/// </summary>
	class IsPasteEnabled : IConditionEvaluator
    {
        public bool IsValid(object owner, Condition condition)
        {
            var surface = owner as DesignSurface;
            if (surface != null)
                return surface.CanPaste();
            return false;
        }
    }
}
