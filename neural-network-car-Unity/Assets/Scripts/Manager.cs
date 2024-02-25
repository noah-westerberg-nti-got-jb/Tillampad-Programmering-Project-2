using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{

    private float timeScale = 1;

    // prefab för bil-objectet
    [SerializeField] GameObject car;

    // track-genererings variabler; används inte
    GenerateTrack trackGenerator;
    [SerializeField] float trackLength, trackIncrementSize, trackWidth, trackCheckpointDensity, turnRange, minTurnDistance, maxTurnDistance;
    [SerializeField] int turns;


    [SerializeField] Transform spawnObject; // bilar skapas som ett "barn" till objectet vid dess position och i riktingen den pekar
    [SerializeField] int populationSize;
    [SerializeField] float eliteCutOf, childToMutationRatio; 
    // eliteCutOf: Hur stor andel av populationen som går vidare utan att förändras
    // childToMutationRatio: hur stor andel av resten av populationen som kommer att antingen bli ett barn eller muteras
    // 1: alla blir barn; 0: alla blir muterade
    [SerializeField] int[] networkSize; // storleken av nätverket; hur många noder det är vid varje lager
    [SerializeField] CarControllerArgs carControllerValues;
    [SerializeField] int viewLines; // Hur många syn intag bilarna har
    [SerializeField] float viewAngle, maxViewDistance, scoreDistanceMultiplyer, crashPenalty;
    // viewAngle: Vinkeln som bilen kan se från mitten av dess vy i grader
    // scoreDistanceMultiplyer: multiplicerar poängen get från distance så att, att köra långt lite långsammare väger mer än att köra en kort distance snabbt
    [SerializeField] int rngSeed;

    Learning learning;

    private void Start()
    {
        trackGenerator = GetComponent<GenerateTrack>();
        learning = GetComponent<Learning>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            // gör att tiden går dubbelt så snabbt
            timeScale = timeScale * 2;
            SetTime(timeScale);
        }
        else if (Input.GetKeyDown(KeyCode.P))
        {
            // gör att tiden går hälften så snabbt
            timeScale = timeScale / 2;
            SetTime(timeScale);
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            learning.Inizialize(spawnObject, populationSize, eliteCutOf, childToMutationRatio, networkSize, car, carControllerValues, viewLines, viewAngle, maxViewDistance, scoreDistanceMultiplyer, crashPenalty, rngSeed);
        }
    }

    void SetTime(float timeScale)
    {
        Time.timeScale = this.timeScale;
        Debug.Log("Time Set to: " + this.timeScale);
    }
}
