using Actions;
using Game;
using UnityEngine;

public class PlayerBehaviour : BaseBehaviour
{
   public override void OnUpdate()
   {
        base.OnUpdate();

        Canvas canvas = Battlefield.sm_instance.GetComponentInChildren<Canvas>();
        PlayerActionMenu pam = PlayerActionMenu.Create(canvas.transform, GetComponent<Mech>());
        ActionStack.Main.PushAction(pam);

    }
}
