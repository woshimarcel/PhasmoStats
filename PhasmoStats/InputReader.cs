namespace PhasmoStats;

internal class InputReader
{
	private static string _input = string.Empty;

	internal static async Task ReadInput()
	{
		while (true)
		{
			if (Console.KeyAvailable)
				HandleInputKey(Console.ReadKey(intercept: true));

			await Task.Delay(millisecondsDelay: 10);
		}
	}

	internal static string GetInput() => _input;

	private static void HandleInputKey(ConsoleKeyInfo keyInfo)
	{
		if (keyInfo.Key == ConsoleKey.Backspace && _input.Length > 0)
			Backspace();
		else if (keyInfo.Key == ConsoleKey.Enter)
			Enter();
		else if (char.IsLetter(keyInfo.KeyChar) || keyInfo.KeyChar == ' ')
			Letter(keyInfo.KeyChar);
	}

	private static void Backspace()
	{
		Console.Write("\b \b");
		_input = _input[..^1];
	}

	private static void Letter(char keyChar)
	{
		Console.Write(keyChar);
		_input += keyChar;
	}

	private static void Enter()
	{
		string input = _input;
		_input = string.Empty;
		InputProcessor.ProcessInput(input);
	}
}
