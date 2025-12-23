using UnityEngine;

public class Res : MonoBehaviour,IRes
{
    public enum Resourses {Wood, Stone, OilCristall, None }
    public Resourses ress;
    public Renderer rd;
    void Start()
    {
        rd = GetComponent<Renderer>();
        int i = Random.Range(0, 2);
        if(i == 0)
        {
            ress = Resourses.Wood;
            rd.material.color = new Color(0.65f, 0.46f, 0.16f);
        }
        else if(i == 1)
        {
            ress = Resourses.Stone;
            rd.material.color = new Color(0.5f, 0.5f, 0.5f);
        }
        /*
        else if(i == 2)
        {
            ress = Resourses.OilCristall;
            rd.material.color = new Color(0.1f, 0.1f, 0.2f);
        }
        */
    }
}
