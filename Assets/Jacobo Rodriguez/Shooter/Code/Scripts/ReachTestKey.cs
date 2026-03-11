using UnityEngine;

public class ReachTestKey : MonoBehaviour
{
  Animator animator;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            animator.SetTrigger("Reach");
        }

    }
}
