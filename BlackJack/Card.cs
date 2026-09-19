namespace Blackjack;

public enum Suit { Clubs, Hearts, Spades, Diamonds }

public enum Rank
{
    Ace = 1, Two, Three, Four, Five, Six, Seven, Eight, Nine, Ten, Jack, Queen, King
}


/// <summary>
/// A single playing card. A record gives us equality and a constructor for free.
/// </summary>
/// <param name="Rank"></param>
/// <param name="Suit"></param>
public record Card(Rank Rank, Suit Suit)
{

    /// <summary>
    /// What the card is worth with an Ace counted as 1.
    /// (Whether an Ace can count as 11 depends on the whole hand, see Rules.HandValue.)
    /// </summary>
    public int BaseValue => Rank switch
    {
        Rank.Ace => 1,
        Rank.Jack or Rank.Queen or Rank.King => 10,
        _ => (int)Rank // Two = 2 ... Ten = 10
    };

    /// <summary>
    /// Example: "Queen of Hearts", "2 of Spacdes", etc.
    /// </summary>
    /// <returns></returns>
    public override string ToString() => $"{Rank} of {Suit}";
}