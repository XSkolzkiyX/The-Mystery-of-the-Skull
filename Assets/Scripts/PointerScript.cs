using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointerScript : MonoBehaviour
{
    [SerializeField] Transform player;
    [SerializeField] Camera cam;
    [SerializeField] Transform pointer;
    [SerializeField] bool needToRotate;
    [SerializeField] float[] offsets;

    void Update()
    {
        Vector3 fromPlayerToShop = transform.position - player.position;
        Ray ray = new Ray(player.position, fromPlayerToShop);
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(cam);
        float minDistance = Mathf.Infinity;
        int planeIndex = 0;

        for(int i = 0; i < 4; i++)
        {
            if(planes[i].Raycast(ray, out float distance))
            {
                if (distance < minDistance)
                {
                    minDistance = distance;
                    planeIndex = i;
                }
            }
        }
        minDistance = Mathf.Clamp(minDistance, 0, fromPlayerToShop.magnitude);
        Vector3 worldPosition = ray.GetPoint(minDistance);
        float multiplierX = (cam.WorldToScreenPoint(worldPosition).x - Screen.width / 2) / Screen.width / 2,
            multiplierY = (cam.WorldToScreenPoint(worldPosition).y - Screen.height / 2) / Screen.height / 2;
        pointer.position = new Vector2(cam.WorldToScreenPoint(worldPosition).x - offsets[0] * multiplierX,
            cam.WorldToScreenPoint(worldPosition).y - offsets[1] * multiplierY);
        //if ((pointer.position.x > Screen.width - Screen.width / 10 || pointer.position.x < Screen.width / 10)
        //    && (pointer.position.y > Screen.height - Screen.height / 10 || pointer.position.y < Screen.height / 10))
        //    pointer.gameObject.SetActive(false);
        //else pointer.gameObject.SetActive(true);
        if (needToRotate) pointer.rotation = Quaternion.Euler(pointer.transform.rotation.eulerAngles.x,
            pointer.transform.rotation.eulerAngles.y, Mathf.Atan2(player.position.z - transform.position.z,
            player.position.x - transform.position.x) * Mathf.Rad2Deg + 90);
    }
}
