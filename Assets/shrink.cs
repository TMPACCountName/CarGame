using UnityEngine;
using System.Collections;

public class shrink : MonoBehaviour
{

    public GameObject objectToScale;
    public float growRate = -3f;


    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {


        

            transform.localScale += new Vector3(0.1F, .1f, .1f) * growRate * Time.deltaTime;


        


           
    }
}
