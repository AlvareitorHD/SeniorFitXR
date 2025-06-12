[System.Serializable]
public class Logro
{
    public int id;
    public string name;
    public string description;
    public string iconUrl;
    public string dateAchieved;

    // Para el logro semanal:
    public int semanasCumplidas = 0;

    public Logro(int id, string name, string description = "", string iconUrl = "", string dateAchieved = null, int semanasCumplidas = 0)
    {
        this.id = id;
        this.name = name;
        this.description = description;
        this.iconUrl = iconUrl;
        this.dateAchieved = dateAchieved ?? System.DateTime.UtcNow.ToString("o");
        this.semanasCumplidas = semanasCumplidas;
    }

    public override bool Equals(object obj)
    {
        if (obj is Logro other)
            return this.id == other.id;
        return false;
    }

    public override int GetHashCode()
    {
        return id.GetHashCode();
    }
}
