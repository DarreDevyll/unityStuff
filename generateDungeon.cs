using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class generateDungeon : MonoBehaviour
{
    // tile class used to hold game objects as well as the attach point direction data
    public class tile
    {
        public GameObject prefab;

        // attach point direction data of game object
        public int? left;
        public int? right;
        public int? up;
        public int? down;

        // constructor when passed a game object
        public tile(GameObject piece)
        {
            prefab = piece;
            left = 0;
            right = 0;
            up = 0;
            down = 0;
        }

        // function used to print direction data to console
        public void printDirections()
        {
            Debug.Log(down);
            Debug.Log(left);
            Debug.Log(up);
            Debug.Log(right);
        }

        // implicit initialization of tile from game object
        public static implicit operator tile(GameObject x) => new tile(x);
    };

    // public list of game objects set in the unity editor
    public List<GameObject> singleWall = new List<GameObject>();
    public List<GameObject> doubleWalls = new List<GameObject>();
    public List<GameObject> Ends = new List<GameObject>();
    public List<GameObject> Rooms = new List<GameObject>();

    // Private Lists of tiles that are initialized on start
    List<tile> sWall = new List<tile>();
    List<tile> dWall = new List<tile>();
    List<tile> ends = new List<tile>();
    List<tile> rooms = new List<tile>();

    // list all possible attach points in current level
    List<Vector3> attachPoints = new List<Vector3>();

    //random number generator variables
    public int seed; // seed for generator
    int randomNumber; // int to hold randomly generated number
    public System.Random rand; // rand variable to hold random function


    // Start is called before the first frame update
    void Start()
    {
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

        // Sets up tile lists with direction data
        setTileData();

        // seeding the random number generator
        rand = new System.Random(seed);
        
        // getting random room and setting it as the first room in the level
        randomNumber = rand.Next(0, 3);
        Instantiate(rooms[randomNumber].prefab, new Vector3(0, 0, 0), Quaternion.identity);

        // debug room number and direction data to make sure it all works right
        Debug.Log(randomNumber);
        rooms[randomNumber].printDirections();

        // setting attach points based on first room placed
        if (rooms[randomNumber].down != null)
            attachPoints.Add(new Vector3(0,(float) rooms[randomNumber].down, 0));
        if (rooms[randomNumber].left != null)
            attachPoints.Add(new Vector3((float)rooms[randomNumber].left, 0, 1));
        if (rooms[randomNumber].up != null)
            attachPoints.Add(new Vector3(0,(float) rooms[randomNumber].up, 2));
        if (rooms[randomNumber].right != null)
            attachPoints.Add(new Vector3((float)rooms[randomNumber].right, 0, 3));

        // getting a random number of hall pieces
        randomNumber = rand.Next(1, 5);
        int rAttachIndex; // element index of right most attach point
        float roomCenter; // holds x value of room center

        // setting random number of hall pieces
        for (int i = 0; i < randomNumber; i++)
        {
            // getting right attach point for layer
            if (getLayerRightAttach() == null)
            {
                Debug.Log("No Right Attach Point in Layer 1");
                break;
            }
            else
                rAttachIndex = (int) getLayerRightAttach();
            // setting hall piece
            Instantiate(dWall[0].prefab, new Vector3((float) (attachPoints[rAttachIndex].x - dWall[0].left), 0, 0), Quaternion.identity);
            // adding halls right attach point to attachPoints list
            attachPoints.Add(new Vector3((float) (attachPoints[rAttachIndex].x - dWall[0].left + dWall[0].right), 0, 3));
            // removing old attach point
            attachPoints.RemoveAt(rAttachIndex);
        }

        // getting a new random room and printing its data to console
        randomNumber = rand.Next(0, 3);
        Debug.Log(randomNumber);
        rooms[randomNumber].printDirections();

        // setting it at the right attach point in layer
        rAttachIndex = (int) getLayerRightAttach();
        Instantiate(rooms[randomNumber].prefab, new Vector3((float) (attachPoints[rAttachIndex].x - rooms[randomNumber].left), 0, 0), Quaternion.identity);

        // getting room center and removing old right attach point
        roomCenter = (float) (attachPoints[rAttachIndex].x - rooms[randomNumber].left);
        attachPoints.RemoveAt(rAttachIndex);

        // setting 3 new attach points from room
        if (rooms[randomNumber].down != null)
            attachPoints.Add(new Vector3(roomCenter,(float) rooms[randomNumber].down, 0));
        if (rooms[randomNumber].up != null)
            attachPoints.Add(new Vector3(roomCenter,(float) rooms[randomNumber].up, 2));
        if (rooms[randomNumber].right != null)
            attachPoints.Add(new Vector3((float)(roomCenter + rooms[randomNumber].right), 0, 3));

        // placing ends at all remaining attach points
        placeEnds();


    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {

    }

    // Check if attach direction exists. If so return it's value casted as int
    int? tryAttach(tile piece, int dir)
    {
        if (dir == 0 && piece.prefab.transform.Find("attachDown") != null)
            return (int) piece.prefab.transform.Find("attachDown").localPosition.y;
        else if (dir == 1 && piece.prefab.transform.Find("attachLeft") != null)
            return (int) piece.prefab.transform.Find("attachLeft").localPosition.x;
        else if (dir == 2 && piece.prefab.transform.Find("attachUp") != null)
            return (int) piece.prefab.transform.Find("attachUp").localPosition.y;
        else if (dir == 3 && piece.prefab.transform.Find("attachRight") != null)
            return (int) piece.prefab.transform.Find("attachRight").localPosition.x;
        return null;
    }

    void placeEnds()
    {
        for (int i = 0; i < attachPoints.Count; i++)
        {
            if (attachPoints[i].z == 0)
                Instantiate(ends[0].prefab, new Vector3(attachPoints[i].x, attachPoints[i].y, 0), Quaternion.identity);
            else if (attachPoints[i].z == 1)
                Instantiate(ends[1].prefab, new Vector3(attachPoints[i].x, attachPoints[i].y, 0), Quaternion.identity);
            else if (attachPoints[i].z == 2)
                Instantiate(ends[2].prefab, new Vector3(attachPoints[i].x, attachPoints[i].y, 0), Quaternion.identity);
            else if (attachPoints[i].z == 3)
                Instantiate(ends[3].prefab, new Vector3(attachPoints[i].x, attachPoints[i].y, 0), Quaternion.identity);
        }
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
                if (right == null || attachPoints[i].z > right)
                {
                    right = (int)attachPoints[i].z;
                    location = i;
                }
            }
        }
        return location;
    }

    // Setting prefab and direction data of each room piece to an element in tile list
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

            // setting rooms
            if (i < 3)
            {
                rooms.Insert(i, Rooms[i]);

                rooms[i].down = tryAttach(rooms[i], 0);
                rooms[i].left = tryAttach(rooms[i], 1);
                rooms[i].up = tryAttach(rooms[i], 2);
                rooms[i].right = tryAttach(rooms[i], 3);
            }
        }

        // setting halls
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
