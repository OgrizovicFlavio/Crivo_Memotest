public class CardData
{
    // Identificador único de la carta (0, 1, 2...)
    public int Id { get; }

    // Nombre simbólico o descriptivo de la carta
    public string Name { get; }

    // Constructor: crea una carta con ID y nombre
    public CardData(int id, string name)
    {
        Id = id;
        Name = name;
    }

    // Compara esta carta con otra
    // Dos cartas son "iguales" si tienen el mismo ID
    public override bool Equals(object obj)
    {
        if (obj is not CardData other) return false;
        return Id == other.Id;
    }

    // Devuelve un número entero que representa esta carta
    // Dos cartas con el mismo ID devolverán el mismo hash
    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }
}
