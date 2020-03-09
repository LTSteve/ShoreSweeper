using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WakeSpawner : MonoBehaviour {

    public Transform Left;
    public Transform Right;

    public Wake[] WakePrefabs;

    private PlayerController player;

    private float timeSinceLastSpawn = 0f;

    private Ship playerShip;

    private bool floop = false;

	void Update ()
    {
        if(player == null)
        {
            if (!Director.D.Player)
            {
                return;
            }

            player = Director.D.Player.GetComponent<PlayerController>();
        }

        var pShip = player.ShipHolder.GetComponentInChildren<Ship>();
        if(pShip != playerShip)
        {
            playerShip = pShip;

            Left.localPosition = playerShip.WakeOffset;
            Right.localPosition = playerShip.WakeOffset - new Vector3(2 * playerShip.WakeOffset.x, 0, 0);
        }

        var velocity = player.GetVelocity();

        var wakeType = (velocity / 6f < 1f) ? ((velocity % 6f) / 2f > 1f ? 1 : 0) : 2;

        var gap = 0f;

        if(velocity < 0.5f)
        {
            gap = 4 - velocity * 7f;
        }
        else if( velocity < 3f)
        {
            gap = 0.8f - (velocity - 0.5f) * 0.1f; //0.8 to 0.55
        }
        else
        {
            gap = 0.55f - (velocity - 3f) * 0.03f;
        }

        if(timeSinceLastSpawn > gap)
        {
            timeSinceLastSpawn = gap;
        }

        if(timeSinceLastSpawn <= 0)
        {
            timeSinceLastSpawn = gap;

            var toSpawn = wakeType > 1 ? 1 : wakeType;

            var spawned1 = Instantiate(WakePrefabs[toSpawn], Left);
            var spawned2 = Instantiate(WakePrefabs[toSpawn], Right);
            spawned1.ExtraForce = spawned2.ExtraForce = velocity / 8f;

            if (wakeType == 2)
            {
                if (floop)
                {
                    Instantiate(WakePrefabs[wakeType], Left);
                    Instantiate(WakePrefabs[wakeType], Right);
                }

                floop = !floop;
            }
        }

        timeSinceLastSpawn -= Time.deltaTime;
	}
}
