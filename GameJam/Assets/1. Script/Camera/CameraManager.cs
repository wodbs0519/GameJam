using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public Transform player;
    public Transform target;

    public CinemachineImpulseSource SoftImpulse;
    public CinemachineImpulseSource HardImpulse;

    private Vector2 prevPosition;
    
    private Vector3 targetPos, shakePos;

    public ParallaxData[] parallaxData;

    private Camera mainCam;

    private static CameraManager instance;
    public static CameraManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType(typeof(CameraManager)) as CameraManager;

                if (instance == null)
                    Debug.LogError("There's no active ManagerClass object");
            }
            return instance;
        }
    }

    private void Awake()
    {
        mainCam = Camera.main;
        target = player;

        prevPosition = transform.position;

        if (parallaxData != null)
        {
            foreach (ParallaxData pd in parallaxData)
                pd.Setup();
        }
    }


    private void OnPreRender()
    {
        if (parallaxData != null)
        {
            Vector2 difference = (Vector2)target.position - prevPosition;
            foreach (ParallaxData pd in parallaxData)
            {
                if (pd.parallaxObject)
                {
                    //pd.beltscrolls[0].Translate(difference.x * pd.parallaxSpeed.x, difference.y * pd.parallaxSpeed.y, 0);
                    pd.beltscrolls[0].position = pd.beltscrolls[1].position - Vector3.right * pd.size.x;
                    pd.beltscrolls[1].Translate(difference.x * pd.parallaxSpeed.x, difference.y * pd.parallaxSpeed.y, 0);
                    //pd.beltscrolls[2].Translate(difference.x * pd.parallaxSpeed.x, difference.y * pd.parallaxSpeed.y, 0);
                    pd.beltscrolls[2].position = pd.beltscrolls[1].position + Vector3.right * pd.size.x;

                    Vector3 objectPos = pd.beltscrolls[1].position;

                    Vector3 point = objectPos + (Vector3)pd.leftbottom * 2;
                    point = mainCam.WorldToViewportPoint(point);

                    if (point.x < 0)
                    {
                        pd.beltscrolls[0].position += Vector3.right * (pd.size.x - pd.graceSize.x);
                        pd.beltscrolls[1].position += Vector3.right * (pd.size.x - pd.graceSize.x);
                        pd.beltscrolls[2].position += Vector3.right * (pd.size.x - pd.graceSize.x);
                    }

                    point = objectPos + (Vector3)pd.rightbottom * 2;
                    point = mainCam.WorldToViewportPoint(point);

                    if (point.x > 1)
                    {
                        pd.beltscrolls[0].position -= Vector3.right * (pd.size.x - pd.graceSize.x);
                        pd.beltscrolls[1].position -= Vector3.right * (pd.size.x - pd.graceSize.x);
                        pd.beltscrolls[2].position -= Vector3.right * (pd.size.x - pd.graceSize.x);
                    }
                }
            }
            prevPosition = target.position;
        }
    }


    [System.Serializable]
    public class ParallaxData
    {
        public Transform parallaxObject;
        public Vector2 parallaxSpeed;

        public Vector2 graceSize;

        [HideInInspector]
        public Vector2 leftbottom, rightbottom;
        [HideInInspector]
        public Vector2 size;

        [HideInInspector]
        public Transform[] beltscrolls;

        public void Setup()
        {
            beltscrolls = new Transform[3];
            beltscrolls[1] = parallaxObject;
            SpriteRenderer spriteRenderer = parallaxObject.GetComponent<SpriteRenderer>();
            leftbottom = spriteRenderer.bounds.min - parallaxObject.position;
            rightbottom = new Vector2(-leftbottom.x, leftbottom.y);
            size = spriteRenderer.bounds.max - spriteRenderer.bounds.min;

            beltscrolls[0] = Instantiate(parallaxObject, parallaxObject.position, parallaxObject.rotation, parallaxObject.parent);
            beltscrolls[2] = Instantiate(parallaxObject, parallaxObject.position, parallaxObject.rotation, parallaxObject.parent);

            beltscrolls[0].position = beltscrolls[1].position - Vector3.right * size.x;
            beltscrolls[2].position = beltscrolls[1].position + Vector3.right * size.x;
        }
    }
}