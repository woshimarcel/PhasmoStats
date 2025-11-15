namespace Logic;

public class SortIndicator
{
	public static string GetHeaderName(string header, string sortingColumn, bool ascending)
	{
		const char ASCENDING_CHARACTER = '⬆';
		const char DESCENDING_CHARACTER = '⬇';
		const char NOTHING = '▪';

		if (header != sortingColumn)
			return $"{header} {NOTHING}";

		if (ascending)
			return $"{header} {ASCENDING_CHARACTER}";
		else
			return $"{header} {DESCENDING_CHARACTER}";
	}
}
