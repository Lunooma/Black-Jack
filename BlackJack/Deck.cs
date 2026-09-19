namespace Blackjack;

public class Deck
{
    private readonly List<Card> _cards = [];

    public int Count => _cards.Count;

    public Deck() => Reset();

    /// <summary>
    /// Rebuilds a full deck and shuffles it.
    /// </summary>
    public void Reset()
    {
        _cards.Clear();

        foreach (Suit suit in Enum.GetValues<Suit>())
            foreach (Rank rank in Enum.GetValues<Rank>())
                _cards.Add(new Card(rank, suit));

        Shuffle();
    }

    /// <summary>
    /// Shuffles the deck using Fisher-Yates
    /// </summary>
    private void Shuffle()
    {
        // Fisher-Yates shuffle: every ordering is equally likely.
        for (int i = _cards.Count - 1; i > 0; i--)
        {
            int j = Random.Shared.Next(i + 1);
            (_cards[i], _cards[j]) = (_cards[j], _cards[i]);
        }
    }

    /// <summary>
    /// Takes the top card of the deck.
    /// </summary>
    /// <returns>The top card of the deck.</returns>
    /// <exception cref="InvalidOperationException"></exception>
    public Card Draw()
    {
        if (_cards.Count == 0)
            throw new InvalidOperationException("The deck is empty.");

        Card top = _cards[^1]; // ^1 = last item
        _cards.RemoveAt(_cards.Count - 1);
        return top;
    }
}