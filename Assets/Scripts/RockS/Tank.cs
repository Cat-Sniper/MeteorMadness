using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tank : Rock
{
     protected int hp;

     public override void OnEnable() {
          base.OnEnable();
          hp = 3;
     }

     public override void DisableRock() {

          hp--;
          SpawnDestructionEffect();

          if(hp <= 0){
               ReturnToFactory();
          }
     }
}
