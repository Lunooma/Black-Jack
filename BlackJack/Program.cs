namespace Blackjack;

class Program
{
    static void Main()
    {
        var deck = new Deck();

        Console.Clear();
        Console.ResetColor();

        while (true)
        {
            if (deck.Count <= 15)
                deck.Reset();

            PlayRound(deck);

            if (!AskYesNo("\nPlay again? (y/N)"))
                break;
        }
    }

    static void PlayRound(Deck deck)
    {
        List<Card> player = [];
        List<Card> house = [];

        // Deal like a real table: player, house (face up), player, house (face down).
        player.Add(deck.Draw());
        house.Add(deck.Draw());
        player.Add(deck.Draw());
        house.Add(deck.Draw());

        // Blackjack check right after the deal.
        if (Rules.IsBlackjack(player) || Rules.IsBlackjack(house))
        {
            ShowTable(house, player, hideHoleCard: false);

            if (Rules.IsBlackjack(player) && Rules.IsBlackjack(house))
                ColoredMessage("You both have blackjack, it's a push.", ConsoleColor.Yellow);
            else if (Rules.IsBlackjack(player))
                ColoredMessage("You won by BLACK JACK! :D", ConsoleColor.Green);
            else
                ColoredMessage("House has blackjack. You lose. :(", ConsoleColor.Red);

            return;
        }

        // Player's turn
        while (true)
        {
            ShowTable(house, player, hideHoleCard: true);

            if (Rules.IsBust(player))
            {
                ColoredMessage("You busted! :(", ConsoleColor.Red);
                return;
            }

            if (!AskYesNo("Hit? (y/N)"))
                break; // stand

            player.Add(deck.Draw());
        }

        DealerTurn(deck, house, player);
    }

    static void DealerTurn(Deck deck, List<Card> house, List<Card> player)
    {
        ShowTable(house, player, hideHoleCard: false);

        while (Rules.HandValue(house) < 17)
        {
            Thread.Sleep(800);
            house.Add(deck.Draw());
            ShowTable(house, player, false);
        }

        int houseValue = Rules.HandValue(house);
        int playerValue = Rules.HandValue(player);

        if (Rules.IsBust(house))
            ColoredMessage("You won! House Busted!", ConsoleColor.Green);
        else if (houseValue < playerValue)
            ColoredMessage("You won!", ConsoleColor.Green);
        else if (houseValue > playerValue)
            ColoredMessage("You lost!", ConsoleColor.Red);
        else
            ColoredMessage("You tied, it's a push!", ConsoleColor.Yellow);

    }

    static void ShowTable(List<Card> house, List<Card> player, bool hideHoleCard)
    {
        Console.Clear();

        if (hideHoleCard)
        {
            // Only the first house card is visible until the dealer's turn.
            ColoredMessage($"House's hand:\n{house[0]}; [hidden]\n{Rules.HandValue([house[0]])}\n", ConsoleColor.Magenta);
        }
        else
        {
            ColoredMessage($"House's hand:\n{string.Join("; ", house)}\n{Rules.HandValue(house)}\n", ConsoleColor.Magenta);
        }

        ColoredMessage($"Your hand:\n{string.Join("; ", player)}\n{Rules.HandValue(player)}\n", ConsoleColor.Cyan);
    }

    // Only an explicit "y" counts as yes. Empty or anything else counts as no.
    static bool AskYesNo(string prompt)
    {
        ColoredMessage(prompt, ConsoleColor.White);
        string? answer = Console.ReadLine();
        return answer?.Trim().ToLower() == "y";
    }

    /// <summary>
    /// Prints a message with a certain color
    /// </summary>
    /// <param name="message">The message to print</param>
    /// <param name="color">The color of the message</param>
    static void ColoredMessage(string message, ConsoleColor color)
    {
        Console.ForegroundColor = color;
        Console.WriteLine(message);
        Console.ResetColor();
    }
}