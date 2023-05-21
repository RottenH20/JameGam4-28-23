/*  
    Hellish Battle - 2.5D Retro FPS
    Added in Version: 1.0.0a
    Updated in Version: 1.0.0a
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteAfterAnimation : MonoBehaviour {

    public float delay = 0f;

    void Start()
    {
        Destroy(gameObject, this.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length + delay);
    }

}
