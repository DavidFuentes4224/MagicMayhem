using UnityEngine;

public static class LayerMaskUtility
{
    /// <summary>
    /// Checks to see if an game object's layer is captured in a LayerMask. LayerMasks are bitmasks under the hood.
    /// </summary>
    /// <param name="gameObject">Object to be tested.</param>
    /// <param name="mask">Mask to be tested against.</param>
    /// <returns></returns>
    public static bool GameObjectIsInLayer(GameObject gameObject, LayerMask mask)
    {
        int goBit = 1 << gameObject.layer;
        int maskBits = mask.value;
        return (goBit & maskBits) != (int)0;
    }
}

