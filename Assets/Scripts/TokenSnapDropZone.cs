using UnityEngine;
using VRTK;

public class TokenSnapDropZone : VRTK_SnapDropZone
{
    public Vector3Int index = Vector3Int.zero;

    private bool snapForced = false;

    public void PlaceToken(GameObject tokenToPlace)
    {
        snapForced = true;
        ForceSnap(tokenToPlace);
    }

    public override void OnObjectSnappedToDropZone(SnapDropZoneEventArgs e)
    {
        currentSnappedObject.isGrabbable = false;
        TokenZoneController.instance.OnTokenPlaced(index, snapForced);
        base.OnObjectSnappedToDropZone(e);
    }
}
