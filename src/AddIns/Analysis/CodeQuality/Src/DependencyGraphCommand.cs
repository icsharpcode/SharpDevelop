using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;

namespace ICSharpCode.CodeQualityAnalysis
{
    public class DependencyGraphCommand : AbstractMenuCommand
    {
    	public override void Run()
    	{
            var window = new MainWindow();
            window.Show();
    	}
    }
}
