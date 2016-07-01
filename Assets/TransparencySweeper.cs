using UnityEngine;
using System.Collections;

public class TransparencySweeper : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        var gotem = Physics.SphereCastAll(gameObject.transform.position, gameObject.transform.lossyScale.z / 2, Vector3.forward, gameObject.transform.lossyScale.z / 2);
        foreach(RaycastHit hit in gotem) {
            var dist = hit.distance;
            var size = gameObject.transform.lossyScale.z / 2;
            var ratio = dist / size;
            Debug.Log(hit.collider.gameObject + " " + dist + "/" + size + " = " + ratio);
        }
        //, float maxDistance = Mathf.Infinity, int layerMask = DefaultRaycastLayers, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal);
    }
}
