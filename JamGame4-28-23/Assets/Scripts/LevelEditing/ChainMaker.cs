using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class ChainMaker : MonoBehaviour {
    public GameObject chainSegmentPrefab;
    public int segments = 1;
    public Vector2 anchor = Vector2.right / 2;
    public Rigidbody2D target;
    public Vector2 connectedAnchor;
    public bool autoConfigureConnectedAnchor = true;
    public bool rebuild;

#if UNITY_EDITOR
    private void Update() {
        if (rebuild) {
            Transform segment = transform.Find("Chain0");
            if(segment != null) {
                DestroyImmediate(segment.gameObject);
            }
            segment = transform;
            HingeJoint2D firstHinge = segment.gameObject.GetComponent<HingeJoint2D>();
            if(firstHinge != null) {
                DestroyImmediate(firstHinge);
            }
            for (int i = 0;i < segments; i++) {
                Rigidbody2D segmentRigid = segment.GetComponent<Rigidbody2D>();
                Vector3 pos;
                if (i == 0) {
                    pos = segment.position;
                } else {
                    pos = segment.TransformPoint(anchor);
                }
                GameObject next = Instantiate(chainSegmentPrefab, pos, segment.rotation, segment);
                next.name = "Chain" + i.ToString();
                Rigidbody2D nextRigid = next.GetComponent<Rigidbody2D>();
                HingeJoint2D hinge = segment.gameObject.AddComponent<HingeJoint2D>();
                if (i == 0) {
                    hinge.anchor = Vector2.zero;
                } else {
                    hinge.anchor = anchor;
                }
                hinge.connectedBody = nextRigid;
                if (i == segments - 1 && target != null) {
                    HingeJoint2D finalHinge = next.gameObject.AddComponent<HingeJoint2D>();
                    finalHinge.anchor = anchor;
                    finalHinge.autoConfigureConnectedAnchor = autoConfigureConnectedAnchor;
                    finalHinge.connectedAnchor = connectedAnchor;
                    finalHinge.connectedBody = target;
                }
                segment = next.transform;
            }
            rebuild = false;
        }
    }
#endif
}
