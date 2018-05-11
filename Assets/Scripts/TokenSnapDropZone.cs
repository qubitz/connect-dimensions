using UnityEngine;
using VRTK;

public class TokenSnapDropZone : VRTK_SnapDropZone
{
    public Vector3Int index = Vector3Int.zero;

    public override void OnObjectSnappedToDropZone(SnapDropZoneEventArgs e)
    {
        TokenZoneController.instance.OnTokenPlaced(index);
        base.OnObjectSnappedToDropZone(e);
    }
}
