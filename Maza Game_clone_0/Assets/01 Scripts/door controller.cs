using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class doorcontroller : MonoBehaviour
{
    [SerializeField]
    private Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(FPSController.isDoor)
        {
            animator.SetBool("isOpen", true);
            Debug.Log("door was open");
        }
        else
        {
            animator.SetBool("isOpen", false);
        }
    }
}
