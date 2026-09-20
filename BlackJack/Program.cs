namespace Blackjack;

class Program
{

    const int betMin = 5;
    const int betMax = 50000;

    public static float currentBet = 0;
    public static float money = 100;

    static void Main()
    {

        QuickAnimation("Welcome to Black Jack!\n    GAME IS LOADING", "...", 300, 5, ConsoleColor.Magenta);

        var deck = new Deck();

        Console.Clear();
        Console.ResetColor();

        while (true)
        {
            if (deck.Count <= 15)
                deck.Reset();

            PlayRound(deck);

            if (AskYesNo("\nPlay again? (Y/n)"))
                break;
        }
    }

    static void PlayRound(Deck deck)
    {
        List<Card> player = [];
        List<Card> house = [];

        AskBetAmount();

        // Deal like a real table: player, house (face up), player, house (face down).
        player.Add(deck.Draw());
        house.Add(deck.Draw());
        player.Add(deck.Draw());
        house.Add(deck.Draw());

        // Blackjack check right after the deal.
        if (Rules.IsBlackjack(player) || Rules.IsBlackjack(house))
        {
            ShowTable(house, player, hideHoldCard: false);

            if (Rules.IsBlackjack(player) && Rules.IsBlackjack(house))
            {
                ColoredMessage("You both have blackjack, it's a push.", ConsoleColor.Yellow);
                money += currentBet;
            }
            else if (Rules.IsBlackjack(player))
            {
                ColoredMessage("You won by BLACK JACK! :D", ConsoleColor.Green);
                money += currentBet * 2.25f;
            }
            else
                ColoredMessage("House has blackjack. You lose. :(", ConsoleColor.Red);

            return;
        }

        // Player's turn
        while (true)
        {
            ShowTable(house, player, hideHoldCard: true);

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

    /// <summary>
    /// Start dealer's turn
    /// </summary>
    /// <param name="deck">The current deck</param>
    /// <param name="house">House's cards</param>
    /// <param name="player">Player's cards</param>
    static void DealerTurn(Deck deck, List<Card> house, List<Card> player)
    {
        ShowTable(house, player, false);

        while (Rules.HandValue(house) < 17)
        {
            Thread.Sleep(800);
            house.Add(deck.Draw());
            ShowTable(house, player, false);
        }

        int houseValue = Rules.HandValue(house);
        int playerValue = Rules.HandValue(player);

        if (Rules.IsBust(house))
        {
            ColoredMessage("You won! House Busted!", ConsoleColor.Green);
            money += currentBet * 2;
        }
        else if (houseValue < playerValue)
        {
            ColoredMessage("You won!", ConsoleColor.Green);
            money += currentBet * 2;
        }
        else if (houseValue > playerValue)
            ColoredMessage("You lost!", ConsoleColor.Red);
        else
        {
            ColoredMessage("You tied, it's a push!", ConsoleColor.Yellow);
            money += currentBet;
        }

    }

    /// <summary>
    /// Asks betting amount and sets values accordingly.
    /// </summary>
    static void AskBetAmount()
    {
        Console.Clear();

        if (money <= 0)
        {
            ColoredMessage("YOU DON'T HAVE ANY MONEY!", ConsoleColor.Red);
            Environment.Exit(1);
        }

        ColoredMessage($"MONEY: ${money}\n  How much do you want to bet?\n   Min: ${betMin}\n   Max: ${betMax}", ConsoleColor.Cyan);

#pragma warning disable CS8604 // Possible null reference argument.
        int answer = int.Parse(Console.ReadLine());
#pragma warning restore CS8604 // Possible null reference argument.

        if (answer > money)
        {
            ColoredMessage($"Bet amount ${answer} exceeds your avaliable balance of ${money}\nPress Enter to continue", ConsoleColor.Red);
            Console.ReadLine();
            AskBetAmount();
        }
        else if (answer > betMax || answer < betMin)
        {
            ColoredMessage($"Bet amount ${answer} is not within ${betMin} - ${betMax}\nPress Enter to continue", ConsoleColor.Red);
            Console.ReadLine();
            AskBetAmount();
        }
        else if (answer <= money)
        {
            money -= answer;
            currentBet = answer;
        }

    }

    /// <summary>
    /// Makes a quick animation yayy
    /// </summary>
    /// <param name="previousMessage">The message that's before the animated text</param>
    /// <param name="message">The animated text</param>
    /// <param name="delay">The delay between each new character string</param>
    /// <param name="loopCount">The amount of times it loops</param>
    /// <param name="color">The color of the message</param>
    static void QuickAnimation(string previousMessage, string message, int delay, int? loopCount, ConsoleColor color)
    {
    
        string savedMessage = previousMessage;

        if (loopCount == null || loopCount == 0)
            loopCount = 1;

        for (int i = 0; i < loopCount; i++)
        {
            for (int index = 0; index < message.Length; index++)
            {
                Thread.Sleep(delay);
                Console.Clear();
                ColoredMessage($"{previousMessage}{message[index]}", color);
                previousMessage = $"{previousMessage}{message[index]}";
            }
            previousMessage = savedMessage;
        }

    }

    /// <summary>
    /// Displays the current table: house's hand and player's hand.
    /// </summary>
    /// <param name="house">House's cards</param>
    /// <param name="player">Player's cards</param>
    /// <param name="hideHoldCard">Whether to hide house's second card or not</param>
    static void ShowTable(List<Card> house, List<Card> player, bool hideHoldCard)
    {
        Console.Clear();

        ColoredMessage($"MONEY:${money} BET:${currentBet}", ConsoleColor.Cyan);

        if (hideHoldCard)
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

    /// <summary>
    /// Only an explicit "y" counts as yes. Empty or anything else counts as no.
    /// </summary>
    /// <param name="prompt">The question asked</param>
    /// <returns>True or False</returns>
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