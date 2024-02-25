using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
 * Klass för att håll koll på hur långt en bil har kört och hur lång tid den har kört
 */
[Serializable]
public class Score : MonoBehaviour
{
    [SerializeField, HideInInspector] GameObject[] checkpoints;
    int currentCheckpointIndex = 0; // indexen av checkpointet som bilen ska köra till
    Checkpoint currentCheckpoint; // class-obectet av checkpointet som bilen ska köra till. gör koden mer "clean" genom att ha det som en separat variable/atribut; annars hade koden på rad 83 behövts skrivas många fler gånger

    float distancePassed, distanceFromPassedCheckpoints, distancePassedToNextCheckpoint;
    // Jag mätte inte bara ut stärckan som bilen körde för att undvika att den skulle få höga poäng av att bara köra runt i cirklar
    // distancePassed: den totala passerade sträckan
    // distanceFromPassedCheckpoints: den sammanlagda sträckan från distance atributen i checkpointsen som bilen har passerad
    // distancePassedToNextCheckpoint: raka sträckan till nästa checkpoint
    float timeSinceStart;

    bool crashed = true;

    bool finished = false;

    [SerializeField, HideInInspector] float distanceScoreMultiplyer, crashPenalty;
    // scoreDistanceMultiplyer: multiplicerar poängen get från distance så att, att köra långt lite långsammare väger mer än att köra en kort distance snabbt

    public void Initialize(float scoreFromDistanceMultiplyer, float crashScorePenalty)
    {
        distanceScoreMultiplyer = scoreFromDistanceMultiplyer;
        crashPenalty = crashScorePenalty;

        StartScore();
    }

    /*
     * Startar igång poängräkningen
     * 
     * Separat från initsialiserings funktionen för att den inte anger några externa värden
     * 
     * Funktionen hade använts om samma bil flytades till en annan bana
     */
    public void StartScore()
    {
        checkpoints = GameObject.FindGameObjectsWithTag("Checkpoint");
        checkpoints = SortCheckpoints(checkpoints);

        currentCheckpoint = checkpoints[0].GetComponent<Checkpoint>();

        timeSinceStart = 0;

        crashed = false;

        distancePassed = 0;
        distanceFromPassedCheckpoints = 0;
    }

    private void Update()
    {
        if (crashed) return;

        timeSinceStart += Time.deltaTime;

        distancePassedToNextCheckpoint = currentCheckpoint.GetDistance() - (transform.position - currentCheckpoint.transform.position).magnitude;
        distancePassed = distanceFromPassedCheckpoints + distancePassedToNextCheckpoint;
    }

    public void PassedCheckpoint(int checkpointIndex)
    {
        if (checkpointIndex != currentCheckpointIndex || crashed) return;

        if (checkpointIndex == checkpoints.Length - 1)
        {
            finished = true;
            crashed = true;
            return;
        }

        distanceFromPassedCheckpoints += currentCheckpoint.GetDistance();

        currentCheckpointIndex++;
        currentCheckpoint = checkpoints[currentCheckpointIndex].GetComponent<Checkpoint>();
    }

    public float GetScore()
    {
        float finishBonus = 0;
        if (finished)
            finishBonus = 10000000000; // lägger till ett stort tal så att de som klarar hela banan får höga poäng
        else if (crashed)
            return (distancePassed * distanceScoreMultiplyer / timeSinceStart) - (crashPenalty / timeSinceStart); // delar poäng straffet på tiden så den blir mindre viktigare senare (gör nog ingen stor sklidnad)
        return (distancePassed * distanceScoreMultiplyer / timeSinceStart) + finishBonus;
    }

    /*
     * sorterar checkpoint objecten efter deras index
     * tidskomplexitet är inte så viktigt för att det kommer inte vara så många object att sortera
     */
    GameObject[] SortCheckpoints(GameObject[] checkpoints)
    {
        GameObject[] sorted = new GameObject[checkpoints.Length];

        foreach (GameObject checkpoint in checkpoints)
        {
            int checkpointIndex = checkpoint.GetComponent<Checkpoint>().index;
            sorted[checkpointIndex] = checkpoint;
        }

        return sorted;
    }

    public void Crash()
    {
        crashed = true;
    }
}
