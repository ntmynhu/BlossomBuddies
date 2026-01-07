using UnityEngine;

public class AnimalSpawn : MonoBehaviour
{
    [SerializeField] private GameObject animalPrefab;
    [SerializeField] private int maxNumberOfAnimals = 5;
    [SerializeField] private float spawnInterval = 10f;
    [SerializeField] private float dissappearInterval = 15f;
    [SerializeField] private TimeOfDay[] timeOfDays;

    private bool isSpawning = false;
    private bool isDissappearing = false;

    private float spawnIntervalTimer;
    private float dissappearIntervalTimer;

    private void OnEnable()
    {
        GameEventManager.Instance.OnTimeOfDayChange += HandleTimeOfDayChange;
    }

    private void OnDisable()
    {
        GameEventManager.Instance.OnTimeOfDayChange -= HandleTimeOfDayChange;
    }

    private void Update()
    {
        if (isSpawning)
        {
            spawnIntervalTimer += Time.deltaTime;

            if (spawnIntervalTimer >= spawnInterval)
            {
                if (transform.childCount < maxNumberOfAnimals)
                {
                    SpawnAnimal();
                }
                else
                {
                    Debug.Log("Max number of animals reached.");
                    StartSpawning(false);
                }

                spawnIntervalTimer = 0f;
            }
        }

        if (isDissappearing)
        {
            dissappearIntervalTimer += Time.deltaTime;

            if (dissappearIntervalTimer >= dissappearInterval)
            {
                if (transform.childCount > 0)
                {
                    Transform animalToDissappear = transform.GetChild(0);
                    Destroy(animalToDissappear.gameObject);
                }
                else
                {
                    Debug.Log("No animals to dissappear.");
                    StartDissappearing(false);
                }

                dissappearIntervalTimer = 0f;
            }
        }
    }

    private void HandleTimeOfDayChange(TimeOfDay currentTimeOfDay)
    {
        if (System.Array.Exists(timeOfDays, timeOfDay => timeOfDay == currentTimeOfDay))
        {
            StartSpawning(true);
            StartDissappearing(false);
        }
        else
        {
            StartSpawning(false);
            StartDissappearing(true);
        }
    }

    private void StartSpawning(bool value)
    {
        isSpawning = value;
    }

    private void StartDissappearing(bool value)
    {
        isDissappearing = value;
    }

    private void SpawnAnimal()
    {
        Instantiate(animalPrefab, transform);
    }
}
