using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class generateDungeon : MonoBehaviour
{
    public class tile
    {
        public GameObject prefab;
        public int? left;
        public int? right;
        public int? up;
        public int? down;

        public tile(GameObject piece)
        {
            prefab = piece;
            left = 0;
            right = 0;
            up = 0;
            down = 0;
        }

        public void printDirections()
        {
            Debug.Log(down);
            Debug.Log(left);
            Debug.Log(up);
            Debug.Log(right);
        }

        public static implicit operator tile(GameObject x) => new tile(x);
    };

    public List<GameObject> singleWall = new List<GameObject>();
    public List<GameObject> doubleWalls = new List<GameObject>();
    public List<GameObject> Ends = new List<GameObject>();
    public List<GameObject> Rooms = new List<GameObject>();

    List<tile> sWall = new List<tile>();
    List<tile> dWall = new List<tile>();
    List<tile> ends = new List<tile>();
    List<tile> rooms = new List<tile>();

    List<Vector3> attachPoints = new List<Vector3>(); // all possible attach points

    public int seed;
    int randomNumber;

    public System.Random rand;
    // Start is called before the first frame update
    void Start()
    {
        /*
        for (int i = 0; i < 4; i++)
        {
            ends.Insert(i, stuff[i]);
        }
        ends[0].left = 5;
        Debug.Log(ends[0].left);
        */
        /*
        tile piece = start;
        piece.left = (int) start.transform.Find("attachLeft").localPosition.x;
        Debug.Log(piece.left);
        */


        // Random rand = new Random(Guid.NewGuid().GetHashCode());

        // Old Level Gen to the left
        /*
        int dist = -3;
        int numRooms = 0;
        // Generating rooms to the left
        for (int i = 0; numRooms < 10; i++)
        {
            Debug.Log(randomNumber);
            randomNumber = rand.Next(0, 3);

            if (randomNumber == 0)
            {
                dist -= 3;
                Instantiate(Ts[0], new Vector3(dist, 0, 0), Quaternion.identity);
                Instantiate(ends[2].prefab, new Vector3(dist, 3, 0), Quaternion.identity);
                dist -= 3;
            }
            else if (randomNumber == 1)
            {
                dist -= 3;
                Instantiate(Ts[1], new Vector3(dist, 0, 0), Quaternion.identity);
                Instantiate(ends[0].prefab, new Vector3(dist, -3, 0), Quaternion.identity);
                dist -= 3;
            }
            else if (randomNumber == 2)
            {
                numRooms++;
                dist -= 5;
                Instantiate(rooms[0], new Vector3(dist, 0, 0), Quaternion.identity);
                Instantiate(ends[0].prefab, new Vector3(dist, -5, 0), Quaternion.identity);
                Instantiate(ends[2].prefab, new Vector3(dist, 5, 0), Quaternion.identity);
                dist -= 5;
            }
        }

        Instantiate(ends[1].prefab, new Vector3(dist, 0, 0), Quaternion.identity);
        */

        setTileData(); // Sets up tile lists with direction data
        
        /*
        sWall.Insert(0, singleWall[0]);

        int? direction;

        if(sWall[0].prefab.transform.Find("attachDown") == null)
        {
            Debug.Log("Couldn't find attachDown");
            direction = null;
        }
        else
        {
            direction = (int) sWall[0].prefab.transform.Find("attachDown").localPosition.y;
        }
        */

        rand = new System.Random(seed); // random number generator
        //Instantiate(start, new Vector3(0, 0, 0), Quaternion.identity);
        //randomNumber = rand.Next(0, 3);

        
        randomNumber = rand.Next(0, 3);
        Instantiate(rooms[randomNumber].prefab, new Vector3(0, 0, 0), Quaternion.identity);

        Debug.Log(randomNumber);
        rooms[randomNumber].printDirections();

        if (rooms[randomNumber].down != null)
            attachPoints.Add(new Vector3(0,(float) rooms[randomNumber].down, 0));
        if (rooms[randomNumber].left != null)
            attachPoints.Add(new Vector3((float)rooms[randomNumber].left, 0, 1));
        if (rooms[randomNumber].up != null)
            attachPoints.Add(new Vector3(0,(float) rooms[randomNumber].up, 2));
        if (rooms[randomNumber].right != null)
            attachPoints.Add(new Vector3((float)rooms[randomNumber].right, 0, 3));

        randomNumber = rand.Next(1, 5);
        int? rightAttach;
        int rAttachIndex;

        for (int i = 0; i < randomNumber; i++)
        {
            rightAttach = getLayerRightAttach();
            if (rightAttach == null)
            {
                Debug.Log("No Right Attach Point in Layer 1");
                break;
            }
            else
                rAttachIndex = (int) rightAttach;
            Instantiate(dWall[0].prefab, new Vector3((float) (attachPoints[rAttachIndex].x - dWall[0].left), 0, 0), Quaternion.identity);
            attachPoints.Add(new Vector3((float) (attachPoints[rAttachIndex].x - dWall[0].left + dWall[0].right), 0, 3));
            attachPoints.RemoveAt(rAttachIndex);
        }
        

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {

    }

    int? tryAttach(tile piece, int dir)
    {
        int? direction;

        if (dir == 0 && piece.prefab.transform.Find("attachDown") != null)
        {
            direction = (int) piece.prefab.transform.Find("attachDown").localPosition.y;
            return direction;
        }
        else if (dir == 1 && piece.prefab.transform.Find("attachLeft") != null)
        {
            direction = (int) piece.prefab.transform.Find("attachLeft").localPosition.x;
            return direction;
        }
        else if (dir == 2 && piece.prefab.transform.Find("attachUp") != null)
        {
            direction = (int) piece.prefab.transform.Find("attachUp").localPosition.y;
            return direction;
        }
        else if (dir == 3 && piece.prefab.transform.Find("attachRight") != null)
        {
            direction = (int) piece.prefab.transform.Find("attachRight").localPosition.x;
            return direction;
        }

        return null;
    }

    // get index of right most attach of layer
    int? getLayerRightAttach()
    {
        int? right = null;
        int? location = null;

        Debug.Log(attachPoints.Count);

        for (int i = 0; i < attachPoints.Count; i++)
        {
            if (attachPoints[i].z == 3)
            {
                if (right == null)
                {
                    right = (int)attachPoints[i].z;
                    location = i;
                }
                else if (attachPoints[i].z > right)
                {
                    right = (int)attachPoints[i].z;
                    location = i;
                }
            }
        }
        return location;
    }

    void setTileData()
    {
        for (int i = 0; i < 4; i++)
        {
            // add prefabs to tile data
            sWall.Insert(i, singleWall[i]);
            ends.Insert(i, Ends[i]);

            // getting attach points for sWalls
            sWall[i].down = tryAttach(sWall[i], 0);
            sWall[i].left = tryAttach(sWall[i], 1);
            sWall[i].up = tryAttach(sWall[i], 2);
            sWall[i].right = tryAttach(sWall[i], 3);

            // getting attach points for ends
            ends[i].down = tryAttach(ends[i], 0);
            ends[i].left = tryAttach(ends[i], 1);
            ends[i].up = tryAttach(ends[i], 2);
            ends[i].right = tryAttach(ends[i], 3);

            if (i < 3)
            {
                rooms.Insert(i, Rooms[i]);

                rooms[i].down = tryAttach(rooms[i], 0);
                rooms[i].left = tryAttach(rooms[i], 1);
                rooms[i].up = tryAttach(rooms[i], 2);
                rooms[i].right = tryAttach(rooms[i], 3);
            }
        }

        for (int i = 0; i < 2; i++)
        {
            dWall.Insert(i, doubleWalls[i]);

            dWall[i].down = tryAttach(dWall[i], 0);
            dWall[i].left = tryAttach(dWall[i], 1);
            dWall[i].up = tryAttach(dWall[i], 2);
            dWall[i].right = tryAttach(dWall[i], 3);
        }

        
    }
}
