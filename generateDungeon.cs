using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class generateDungeon : MonoBehaviour
{
    public struct tile
    {
        public GameObject prefab;
        public int Left;
        public int right;
        public int up;
        public int down;
    };

    public GameObject start;

    public List<GameObject> ends = new List<GameObject>();
    public List<GameObject> Ts = new List<GameObject>();
    public List<GameObject> rooms = new List<GameObject>();


    public int seed;
    int randomNumber;

    public System.Random rand;

    // Start is called before the first frame update
    void Start()
    {
        // Random rand = new Random(Guid.NewGuid().GetHashCode());
        rand = new System.Random(seed);

        Instantiate(start, new Vector3(0, 0, 0), Quaternion.identity);

        

        int dist = -4;
        int numRooms = 0;
        // Generating rooms to the left
        for (int i = 0; numRooms < 10; i++)
        {
            Debug.Log(randomNumber);
            randomNumber = rand.Next(0, 3);

            if (randomNumber == 0)
            {
                dist -= 4;
                Instantiate(Ts[0], new Vector3(dist, 0, 0), Quaternion.identity);
                Instantiate(ends[2], new Vector3(dist, 3, 0), Quaternion.identity);
                dist -= 4;
            }
            else if (randomNumber == 1)
            {
                dist -= 4;
                Instantiate(Ts[1], new Vector3(dist, 0, 0), Quaternion.identity);
                Instantiate(ends[0], new Vector3(dist, -3, 0), Quaternion.identity);
                dist -= 4;
            }
            else if (randomNumber == 2)
            {
                numRooms++;
                dist -= 7;
                Instantiate(rooms[0], new Vector3(dist, 0, 0), Quaternion.identity);
                Instantiate(ends[0], new Vector3(dist, -7, 0), Quaternion.identity);
                Instantiate(ends[2], new Vector3(dist, 7, 0), Quaternion.identity);
                dist -= 7;
            }
        }

        dist -= 1;
        Instantiate(ends[1], new Vector3(dist, 0, 0), Quaternion.identity);

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {

    }
}
