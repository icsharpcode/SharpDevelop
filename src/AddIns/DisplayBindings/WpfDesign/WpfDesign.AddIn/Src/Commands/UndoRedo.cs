// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using ICSharpCode.Core;
using ICSharpCode.WpfDesign.Designer;
namespace ICSharpCode.WpfDesign.AddIn.Commands
{
	/// <summary>
	/// Invokes the Undo command if available on the Design Surface.
	/// </summary>
	class Undo : AbstractMenuCommand
    {
        public override void Run()
        {
            var surface = Owner as DesignSurface;
            if (surface != null)
                surface.Undo();
        }
    }
	
	/// <summary>
	/// Invokes the Redo command if available on the Design surface.
	/// </summary>
	class Redo : AbstractMenuCommand
    {
        public override void Run()
        {
            var surface = Owner as DesignSurface;
            if (surface != null)
                surface.Redo();
        }
    }
	
	/// <summary>
	/// Provides implementation of <see cref="IConditionEvaluator"/> for <see cref="Undo"/>.
	/// </summary>
	class IsUndoEnabled : IConditionEvaluator
    {
        public bool IsValid(object owner, Condition condition)
        {
            var surface = owner as DesignSurface;
            if (surface != null)
                return surface.CanUndo();
            return false;
        }
    }
	
	/// <summary>
	/// Provides implementation of <see cref="IConditionEvaluator"/> for <see cref="Redo"/>.
	/// </summary>
	class IsRedoEnabled : IConditionEvaluator 
    {
        public bool IsValid(object owner, Condition condition)
        {
            var surface = owner as DesignSurface;
            if (surface != null)
                return surface.CanRedo();
            return false;
        }
    }
}
