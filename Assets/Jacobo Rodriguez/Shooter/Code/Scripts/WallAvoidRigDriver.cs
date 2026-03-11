using System;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.Serialization;

public class WallAvoidRigDriver : MonoBehaviour
{
    public Camera _cam;
    public Transform gunProbe;
    public bool useGunForward = false;
    public Rig weaponPoseRig;
    public MultiPositionConstraint posC;
    public LayerMask geometryMask = ~0;
    
    
    private Ray camRayDbg;
    private Ray gunRayDbg;
    private bool camHit, gunHit;
    [SerializeField] private float radius;
    [SerializeField] private float maxCheck;
    
    
    private bool avoiding;
    [SerializeField] private float enterStart = 0.6f;
    [SerializeField]private  float exitStart = 0.7f;
    [SerializeField] private float enterFull = 0.35f;
    [SerializeField] private float exitFull = 0.45f;
    private float aCam;
    private float aGun;
    private float alpha,vel;
    [SerializeField] private float smooth = 0.08f;

    private void Awake()
    {
        if(!_cam) _cam = Camera.main;
    }

    private void Update()
    {
        if(!_cam) return;

        camRayDbg = new Ray(_cam.transform.position, _cam.transform.forward);
        
        Vector3 gOrigin = gunProbe?gunProbe.position : _cam.transform.position;
        Vector3 gDirection = (useGunForward && gunProbe) ? gunProbe.forward : _cam.transform.forward;
        gunRayDbg = new Ray(gOrigin, gDirection);


        RaycastHit hCam, hGun;
        
        camHit = Physics.SphereCast(camRayDbg, radius, out hCam, maxCheck, geometryMask,
            QueryTriggerInteraction.Ignore);
        gunHit = Physics.SphereCast(gunRayDbg, radius, out hGun, maxCheck, geometryMask,
            QueryTriggerInteraction.Ignore);


        float distCam = camHit ? hCam.distance : float.PositiveInfinity;
        float distGun = gunHit? hGun.distance : float.PositiveInfinity;
        float minDistance = Mathf.Min(distCam, distGun);
        
        if(!avoiding && (minDistance <= enterFull)) avoiding = true; //Entrar
        if(avoiding && (!camHit && !gunHit||minDistance >= exitStart))avoiding = false;//Salir
        
        float aStart = avoiding? exitStart: enterStart;
        float aFull = avoiding? exitFull: enterFull;

        aCam = camHit ? Mathf.Clamp01(Mathf.InverseLerp(aStart, aFull, distCam)) : 0f;
        aGun = gunHit ? Mathf.Clamp01(Mathf.InverseLerp(aStart, aFull, distGun)) : 0f;

        float target = Mathf.Max(aCam, aGun);

        alpha = Mathf.SmoothDamp(alpha, target, ref vel, smooth);

        
        
        float k = 1f - alpha;
        if (weaponPoseRig) weaponPoseRig.weight = k;
        if(posC) posC.weight = k;
    }
}