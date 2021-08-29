using ExitGames.Client.Photon;
using Photon.Realtime;

public static class PlayerPropertiesExtensions
{
    private const string Index_x = "X";
    private const string Index_y = "Y";

    private static readonly Hashtable propsToSet = new Hashtable();

    public static void PosUpdate(this Player player, int x, int y) {
        propsToSet[Index_x] = x;
        propsToSet[Index_y] = y;
        player.SetCustomProperties(propsToSet);
        propsToSet.Clear();
    }
}