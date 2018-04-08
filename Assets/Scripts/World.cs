using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour {

    [SerializeField]
    Material skybox_clear, skybox_rainy; //materials for different skybox weathers

    [SerializeField]
    GameObject rainmaker; //prefab from unity assets that allow to make realistic rain

    [SerializeField]
    GameObject Light; // gameobject which controls light on scene

    [SerializeField]
    GameObject soil, stone; //prefabs for blocks out of which the world consists

    GameObject[,,] world = new GameObject[1000, 100, 1000]; //3-dimensional array of blocks that make up map

    [SerializeField]
    GameObject deer; // prefab for deer

    [SerializeField]
    GameObject charact; // character himself

    [SerializeField]
    GameObject tree; //prefab for tree generation

    GameObject[] animals = new GameObject[100]; // array of all animals that exist

    int animalcount; // number of currently existing animals

    [SerializeField]
    float seed; //seed is a thing used to generate world, it is generated once and forever
                // the lower the value of seed is, the calmer generated landcape will be

    [SerializeField]
    float raindelay = 120F;//delay until next rain

    [SerializeField]
    float rainLenght = 100f; //determines how long the rain will go

    [SerializeField]
    float rainMaxIntensity = 0.5f; //determines max intensity that rain can possibly hit 

    [SerializeField]
    bool can_weather_change=true; //determines whether weather cycle is active or not

    [SerializeField]
    bool generate_world = true; //determines whether the wrold is generated at start or not

    bool isRainGoing = false; // wther the rain is going right now or not

    float rainIntesifierTime = 3f, rainCurrentIntensity = 0f;
    //first determines the delay inbetween rain intensifying cycle, second shows current rain intensity

    float rainDecreasingTime = 1f;
    //determines the delay inbetween rain decreasing cycle


    void Start () {
        if (can_weather_change) StartCoroutine(ChangeWeather()); //start weather cycle if allowed
        seed = Random.Range(0.01F, 0.04F); // generate a seed for map generation
        if (generate_world) GenerateChunk(0, 0, 100, 100, 50); //generate the map itself
        charact.transform.position = new Vector3(0, HeightFormula(0,0)+4, 0);//put character on map
        GenerateDeer(10); //generate animals
    }
	
	
	void FixedUpdate () {
        
        Animal_moves();
    }


    int HeightFormula(float z, float x)
    {
        //a general formula to calculate height
        return Mathf.RoundToInt(Mathf.PerlinNoise(z, x) * 45);
    }

    void GenerateChunk(int startx, int startz, int wz, int wx, int woodiness)
    //a procedure for generating a chunk of blocks starting from block startz, startx
    //supposedly starting block doesnt exist
    //z -- front + back, x -- right + left, y -- up + down 
    //wz and wx define size of a chunk starting from block with coords startz, startx, starty
    //woodiness -- parameter which defines number of trees generated in chunk
    {
        int x = startx, y = HeightFormula(startz, startx), z = startz;
        //coords of the fisrt block in chunk

        for (int i = 0; i <= wz; i++)
            for (int j = 0; j <= wx; j++)
            {
                GameObject newblock = soil;

                int block_x = x + j, block_z = z + i, block_y = HeightFormula(z + i * seed, x + j * seed);

                newblock.transform.position = new Vector3(block_z, block_y, block_x);
                Instantiate(newblock);

                world[block_z, block_y, block_x] = newblock;
                
            }
        //This was a part that generated ~90% of blocks in the world
        //Now we need to fix every hole that there is in generation

        for (int i = 1; i < wz; i++)
            for (int j = 1; j < wx; j++)
            {
                int block_x = x + j, block_z = z + i, block_y = HeightFormula(z + i * seed, x + j * seed);


                if (world[block_z - 1, block_y + 1, block_x] == null ||
                    world[block_z - 1, block_y, block_x] == null ||
                    world[block_z - 1, block_y - 1, block_x] == null)
                {
                    
                    if (world[block_z - 1, block_y + 2, block_x] != null)
                    {
                        GameObject newblock = soil;

                        newblock.transform.position = new Vector3(block_z - 1, block_y + 1, block_x);
                        Instantiate(newblock);

                        world[block_z - 1, block_y + 1, block_x] = newblock;
                        
                    }
                    else
                    {
                        if (world[block_z - 1, block_y - 2, block_x] != null)
                        {
                            GameObject newblock = soil;

                            newblock.transform.position = new Vector3(block_z - 1, block_y - 1, block_x);
                            Instantiate(newblock);

                            world[block_z - 1, block_y - 1, block_x] = newblock;
                            
                        }
                    }
                }

                if (world[block_z + 1, block_y + 1, block_x] == null ||
                    world[block_z + 1, block_y, block_x] == null ||
                    world[block_z + 1, block_y - 1, block_x] == null)
                {
                    if (world[block_z + 1, block_y + 2, block_x] != null)
                    {
                        GameObject newblock = soil;

                        newblock.transform.position = new Vector3(block_z + 1, block_y + 1, block_x);
                        Instantiate(newblock);

                        world[block_z + 1, block_y + 1, block_x] = newblock;
                        
                    }
                    else
                        if (world[block_z + 1, block_y - 2, block_x] != null)
                        {
                            GameObject newblock = soil;

                            newblock.transform.position = new Vector3(block_z + 1, block_y - 1, block_x);
                            Instantiate(newblock);

                            world[block_z + 1, block_y - 1, block_x] = newblock;
                            
                        }
                }

                if (world[block_z, block_y + 1, block_x + 1] == null ||
                   world[block_z, block_y, block_x + 1] == null ||
                   world[block_z, block_y - 1, block_x + 1] == null)
                {
                    if (world[block_z, block_y + 2, block_x + 1] != null)
                    {
                        GameObject newblock = soil;

                        newblock.transform.position = new Vector3(block_z + 1, block_y + 1, block_x + 1);
                        Instantiate(newblock);

                        world[block_z, block_y + 1, block_x + 1] = newblock;
                        
                    }
                    else
                        if (world[block_z, block_y - 2, block_x + 1] != null)
                        {
                            GameObject newblock = soil;

                            newblock.transform.position = new Vector3(block_z, block_y - 1, block_x + 1);
                            Instantiate(newblock);

                            world[block_z, block_y - 1, block_x + 1] = newblock;
                           
                        }
                }

                if (world[block_z, block_y + 1, block_x - 1] == null ||
                   world[block_z, block_y, block_x - 1] == null ||
                   world[block_z, block_y - 1, block_x - 1] == null)
                {
                    if (world[block_z, block_y + 2, block_x - 1] != null)
                    {
                        GameObject newblock = soil;

                        newblock.transform.position = new Vector3(block_z + 1, block_y + 1, block_x - 1);
                        Instantiate(newblock);

                        world[block_z, block_y + 1, block_x - 1] = newblock;
                        
                    }
                    else
                        if (world[block_z, block_y - 2, block_x - 1] != null)
                        {
                            GameObject newblock = soil;

                            newblock.transform.position = new Vector3(block_z, block_y - 1, block_x - 1);
                            Instantiate(newblock);

                            world[block_z, block_y - 1, block_x - 1] = newblock;
                            
                        }
                }
            }

        //OK, holes should be fixed, now we need to place a bit of trees here and there

        

        int square = wz * wx; // calculate total square of chunk

        int avg_distribution = square / woodiness; //how much of blocks are there for one tree for 
                                                       //average cover of all the land

        int lastplaced=0;
        //number of the block where last tree was placed
        int numberofblocks=0;
        //number of blocks counted

        for (int i = 1; i < wz; i++)
            for (int j = 1; j < wx; j++)
            {
                int randomfactor = Random.Range(avg_distribution / 10, avg_distribution/2);
                //randomfactor should bring in some noise in destribution of trees

                int block_x = x + j, block_z = z + i, block_y = HeightFormula(z + i * seed, x + j * seed);
                numberofblocks++;

                if ((numberofblocks - lastplaced) >= (avg_distribution - randomfactor) && (numberofblocks - lastplaced) >= 5)
                {
                    GameObject newtree = tree;
                    newtree.transform.position= new Vector3(block_z, block_y, block_x);
                    Instantiate(newtree);
                    lastplaced = numberofblocks;
                }
            }

    }

    //simply spawn given num of Deer
    void GenerateDeer(int num)
    {
        for (int i = 0; i < num; i++)
        {
            GameObject newdeer = deer;
            // newdeer=newdeer.Reproduction(deer,deer);
            newdeer.transform.position = new Vector3(Random.Range(-50, 50), 0, Random.Range(-50, 50));
            animals[animalcount++] = newdeer;
            Instantiate(newdeer);
        }
    }

    IEnumerator ChangeWeather() //If it is rainy changes to clear, if clear changes to rainy
    { //ToDo Light games
        while (true)
        {
            if (!isRainGoing)
            {
                if (rainCurrentIntensity <= 0)//if weather is now clear
                {
                    rainCurrentIntensity = 0;
                    (rainmaker.GetComponent("RainScript") as DigitalRuby.RainMaker.RainScript).RainIntensity = rainCurrentIntensity;
                    RenderSettings.skybox.SetFloat("_Exposure", 1f);
                    isRainGoing = true; //kickstarts the new cycle
                    yield return new WaitForSeconds(raindelay);
                }
                else
                {  
                    rainCurrentIntensity -= 0.05f;
                    if (rainCurrentIntensity <= 0f)
                    {
                        (rainmaker.GetComponent("RainScript") as DigitalRuby.RainMaker.RainScript).RainIntensity = 0;
                    }
                    else
                    {
                        (rainmaker.GetComponent("RainScript") as DigitalRuby.RainMaker.RainScript).RainIntensity = rainCurrentIntensity;
                        if (RenderSettings.skybox.GetFloat("_Exposure") < 1f)
                            RenderSettings.skybox.SetFloat("_Exposure", RenderSettings.skybox.GetFloat("_Exposure") + 0.05f);
                    }
                    if (rainCurrentIntensity < 0.2f) RenderSettings.skybox = skybox_clear;
                    yield return new WaitForSeconds(rainDecreasingTime);
                }
            }
            else
            {
                if (rainCurrentIntensity >= rainMaxIntensity)//if t is already raining to max
                {
                    rainCurrentIntensity = rainMaxIntensity;
                    (rainmaker.GetComponent("RainScript") as DigitalRuby.RainMaker.RainScript).RainIntensity = rainCurrentIntensity;
                    isRainGoing = false; //kickstarts decreasing the rain part
                    yield return new WaitForSeconds(rainLenght);
                }
                else
                {
                    rainCurrentIntensity += 0.05f;
                    (rainmaker.GetComponent("RainScript") as DigitalRuby.RainMaker.RainScript).RainIntensity = rainCurrentIntensity;
                     if (RenderSettings.skybox.GetFloat("_Exposure") > 0.5f)
                          RenderSettings.skybox.SetFloat("_Exposure", RenderSettings.skybox.GetFloat("_Exposure") - 0.17f);
                    if (rainCurrentIntensity>0.2f) RenderSettings.skybox = skybox_rainy;
                    yield return new WaitForSeconds(rainIntesifierTime);
                }
            }
        }
    }

    void Animal_moves()
    {
        for (int i = 0; i < animalcount; i++)
        {

        }
    }
}
