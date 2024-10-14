using UnityEngine;

public class vykresleniObjektu : MonoBehaviour
{
    public GameObject prefab;
    public float posX;
    public float posY;
    public float posZ;
       
    public void vykreslitElipsu(float xOff, float yOff, float z, float width, float height, int pocetObjektu)
    {
        for (int i = 0; i < pocetObjektu; i++)
        {
            float angle = i * Mathf.PI * 2 / pocetObjektu;
            float x = width * Mathf.Cos(angle) +xOff;
            float y = height * Mathf.Sin(angle)+yOff;
            Vector3 position = new Vector3(x, y, z);
            Instantiate(prefab, position, Quaternion.identity);
        }
    }
    void Start(){
        vykreslitElipsu(posX, posY, posZ, 15, 8, 20);
    }
}
