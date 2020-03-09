using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    public static PlayerController Instance;

    public Transform ShipHolder;
    public Transform Waker;
    public Sprite HorizSprite;
    public Sprite DiagSprite;

    public float speed = 1f;
    public float rotation = 30f;

    public static Sprite FlagSprite;
    public Sprite flagspritehack;

    public Ship[] ShipPrefabs;
    public Sail[] SailPrefabs;

    private float animationTimeScale = 0.25f;

    public Ship MyShip;
    private Rigidbody2D myBody;

    public int hull = 0;
    public int[] sails = { 0 };

    private static Vector3[] HorizEulers =  {
        Vector3.zero, //right
        new Vector3(0, 0, 90), //up
        new Vector3(0, 0, 180), //left
        new Vector3(0, 0, 270) //down
    };

    private static Vector3[] DiagEulers =  {
        Vector3.zero, // upright
        new Vector3(0, 0, 90), //upleft
        new Vector3(0, 0, 180), //downleft
        new Vector3(0, 0, 270), //downright
    };

    private bool isAnimating = false;
    private Vector3 myTarget;

    public void Start()
    {
        Instance = this;

        FlagSprite = flagspritehack;

        myBody = GetComponent<Rigidbody2D>();
    }

    public void Update()
    {
        if(BuyView.Instance == null || BuyView.Instance.open)
        {
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            _raycastActivate();
        }
        if(Input.GetMouseButtonDown(1))
        {
            _raycastActivate(true);
        }

        if (isAnimating)
        {
            return;
        }

        var horiz = Input.GetAxisRaw("Horizontal");
        var vert = Input.GetAxisRaw("Vertical");

        if (vert > 0)
        {
            myBody.AddForce(ShipHolder.rotation * Vector2.up * speed);
        }
        else if (vert < 0)
        {
            myBody.AddForce(ShipHolder.rotation * Vector2.down * speed * 0.25f);
        }

        var rot = ShipHolder.rotation.eulerAngles + new Vector3(0f, 0f, rotation * Time.deltaTime * -horiz);

        if(myBody.velocity.magnitude > 2f)
        {
            var extrarot = (Mathf.Clamp(myBody.velocity.magnitude, 2f, 3f) - 2f) / 2f;
            rot.z += rotation * Time.deltaTime * extrarot * -horiz;
        }

        //add some extra force to counteract drifting
        var turnDir = ShipHolder.rotation * Vector3.up;
        var velDir = myBody.velocity.normalized;
        var rightDir = new Vector2(turnDir.y, -turnDir.x);
        var leftDir = new Vector2(-turnDir.y, turnDir.x);

        if(Vector2.Dot(velDir, rightDir) > Vector2.Dot(velDir, leftDir))
        {
            myBody.AddForce(leftDir * myBody.velocity.magnitude * .9f);
        }
        else if (Vector2.Dot(velDir, rightDir) < Vector2.Dot(velDir, leftDir))
        {
            myBody.AddForce(rightDir * myBody.velocity.magnitude * .9f);
        }

        if (rot.x != 0)
        {
            rot = new Vector3(0f, 0f, -180f);
        }

        ShipHolder.rotation = Quaternion.Euler(rot);

        if(Vector2.Dot(velDir,turnDir) < 0)
        {
            rot += new Vector3(0, 0, 180);
        }

        Waker.rotation = Quaternion.Euler(rot);
    }

    private void _raycastActivate(bool right = false)
    {
        var tile = RaycastUtil.RaycastTile();
        if (tile != null)
        {
            if (!right)
            {
                tile.Activate();
            }
            else
            {
                tile.Flagify();
            }
        }
    }

    public void SetHull(int hull, bool refresh = true)
    {
        this.hull = hull;

        if (refresh)
        {
            _reloadShip();
        }
    }

    public void SetSails(int[] sails, bool refresh = true)
    {
        this.sails = sails;

        if (refresh)
        {
            _reloadShip();
        }
    }

    public int GetHullType()
    {
        return hull;
    }

    public int[] GetSails()
    {
        return sails;
    }

    public float GetVelocity()
    {
        return myBody.velocity.magnitude;
    }

    private void _reloadShip()
    {
        if (MyShip)
        {
            Destroy(MyShip.gameObject);
        }

        MyShip = Instantiate(ShipPrefabs[hull], ShipHolder);

        for(var i = 0; i < sails.Length; i++)
        {
            if(MyShip.Masts.Length <= i)
            {
                break;
            }

            Instantiate(SailPrefabs[sails[i]], MyShip.Masts[i]);
        }

        speed = MyShip.GetTotalSpeed();

        StartCoroutine(_animateCameraShift(MyShip.ViewDistance));
    }

    private IEnumerator _animateCameraShift(float viewDistance)
    {
        if(viewDistance != Camera.main.orthographicSize)
        {

            var startingDistance = Camera.main.orthographicSize;
            var transitionTime = 2f;

            while(viewDistance != Camera.main.orthographicSize)
            {
                yield return null;

                Camera.main.orthographicSize = Mathf.Lerp(Camera.main.orthographicSize, viewDistance, 1f-transitionTime/2f);

                transitionTime -= Time.deltaTime;
            }

            Camera.main.orthographicSize = viewDistance;
        }
    }
}
