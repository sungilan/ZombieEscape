
public class Database : Singleton<Database>
{
    private ItemDB _item;
    public static ItemDB Item
    {
        get
        {
            if(Instance._item == null)
                Instance._item = new ItemDB();
            return Instance._item;
        }
    }
}
