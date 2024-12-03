using System;

public class NotificationCenter : Singleton<NotificationCenter>
{
    public EventHandler<GenericEventArgs<DamageIndicator>> damageTextFinishedMoving;

    public void RaiseMovementFinished(DamageIndicator dmg)
    {
        if (dmg)
			damageTextFinishedMoving?.Invoke(this, new GenericEventArgs<DamageIndicator>(dmg));
    }
}
