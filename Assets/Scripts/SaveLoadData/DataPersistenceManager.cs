using UnityEngine;

public class DataPersistenceManager : MonoBehaviour
{
    #region Singleton
    private static DataPersistenceManager instance;
    public static DataPersistenceManager Instance => instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #endregion

}
