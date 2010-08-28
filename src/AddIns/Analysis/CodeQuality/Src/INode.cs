using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;

namespace ICSharpCode.CodeQualityAnalysis
{
    public interface INode
    {
        string Name { set; get; }
        IDependency Dependency { set; get; }
        string GetInfo();
        Relationship GetRelationship(INode node);
        BitmapSource Icon { get; }
    }
}
