using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ICSharpCode.CodeQualityAnalysis
{
    public interface INode
    {
        string Name { set; get; }
        IDependency Dependency { set; get; }
        string GetInfo();
    }
}
