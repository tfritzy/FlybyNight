using System.Collections.Generic;
using UnityEngine;

public class Store : MonoBehaviour
{
    public Transform PreviewPos;
    public List<SkinType> Helicoptors = new List<SkinType>
    {
        SkinType.Default, SkinType.MechaCoptor, SkinType.NightCoptor, SkinType.SteamPunk,
    };

    private GameObject previewHeli;
    public void SelectSkin(string type)
    {
        SkinType parsedType = (SkinType)System.Enum.Parse(typeof(SkinType), type);
        Destroy(previewHeli);
        GameObject heli = Managers.GetHelicoptorBody(parsedType);
        previewHeli = GameObject.Instantiate(heli, Vector3.zero, heli.transform.rotation, PreviewPos);
        Destroy(previewHeli.GetComponent<Helicopter>());
        Destroy(previewHeli.GetComponent<InterpolatedTransform>());
        Destroy(previewHeli.GetComponent<InterpolatedTransformUpdater>());
        var previewScript = previewHeli.AddComponent<PreviewHelicopter>();
        previewScript.Init();
        previewHeli.transform.localScale = Vector3.one * 500f;
    }
}