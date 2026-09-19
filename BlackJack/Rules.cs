namespace Blackjack;

public static class Rules
{

    /// <summary>
    /// Best value of a hand: one Ace counts as 11 if that doesn't bust, otherwise all Aces are 1.
    /// </summary>
    /// <param name="hand"></param>
    /// <returns></returns>
    public static int HandValue(IReadOnlyList<Card> hand)
    {
        int total = hand.Sum(card => card.BaseValue); // every Ace counted as 1
        bool hasAce = hand.Any(card => card.Rank == Rank.Ace);

        // Only one Ace can ever be 11 (two would already be 22).
        if (hasAce && total + 10 <= 21)
            total += 10;

        return total;
    }

    /// <summary>
    /// Checks if the given hand is a bust.
    /// </summary>
    /// <param name="hand"></param>
    /// <returns></returns>
    public static bool IsBust(IReadOnlyList<Card> hand) => HandValue(hand) > 21;

    /// <summary>
    /// Checks if the given hand is black jack.
    /// </summary>
    /// <param name="hand"></param>
    /// <returns></returns>
    public static bool IsBlackjack(IReadOnlyList<Card> hand) =>
        hand.Count == 2 && HandValue(hand) == 21;
}