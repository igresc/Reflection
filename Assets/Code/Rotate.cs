using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    [SerializeField] GameObject window;
    float cooldown;
    bool canInteract;
    // Start is called before the first frame update
    void Start()
    {
        cooldown = 1;
        canInteract = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!canInteract)
        {
            if (cooldown <= 0) 
            {
                canInteract = true;
                cooldown = 1;
            }
            else
            {
                cooldown -= Time.deltaTime;
            }
        }
        
        Debug.Log(cooldown);
        Debug.Log(canInteract);
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (Input.GetKey(KeyCode.E) && collision.CompareTag("Player") && canInteract)
        {
            Debug.Log("IIIIIIIIIIN");
            Quaternion newRot = window.transform.rotation * new Quaternion(0, 0, 15, 0);
            window.transform.rotation = newRot;
            canInteract = false;
        }
    }
}
