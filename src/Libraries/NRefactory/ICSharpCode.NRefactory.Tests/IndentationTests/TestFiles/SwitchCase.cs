/// <summary>
///     Switch case with CSharpFormattingOptions.IndentBreakStatements = false;
/// </summary>
class SwitchCase
{
	void Main(int param)
	{
		switch (param)
		{
			case 1:
				while (true)
				{
					break;
					continue;
					return;
					goto case 2;
					goto default;
				}
			break;
			case 2:
				// ...
			continue;
			case 3:
				// ...
			return;
			case 4:
				// ...
			goto default;
			default:
				// ...
			goto case 1;
		}
	}
}
