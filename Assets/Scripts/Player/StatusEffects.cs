using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusEffects : MonoBehaviour
{
    public List<StatusEffect> currentStatusEffects = new List<StatusEffect>();

    public void RemoveStatusEffectsByName(string name)
    {
        currentStatusEffects.RemoveAll(x => x.name == name);
    }

    public void AddStatusEffect(StatusEffect effect)
    {
        // remove if existing to ensure only one of each status effect exists
        RemoveStatusEffectsByName(effect.name);
        currentStatusEffects.Add(effect);
        var hasDuration = effect.duration > 0;
        if (hasDuration)
        {
            StartCoroutine(RemoveStatusEffectAfterDuration(effect.duration, effect.name));
        }
    }

    private IEnumerator RemoveStatusEffectAfterDuration(float duration, string effectName)
    {
        yield return new WaitForSeconds(duration);
        RemoveStatusEffectsByName(effectName);
    }
}
