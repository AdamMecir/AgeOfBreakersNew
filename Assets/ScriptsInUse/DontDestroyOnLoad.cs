using UnityEngine;

public class DontDestroyOnLoad : MonoBehaviour
{
    private void Awake()
    {
        // Ensure this GameObject and its children are not destroyed when loading a new scene
        DontDestroyOnLoad(gameObject);

        // Prevent duplicates by destroying any existing instance of this GameObject
        DontDestroyDuplicates();
    }

    private void DontDestroyDuplicates()
    {
        // Check if there are other instances of this GameObject in the scene
        DontDestroyOnLoad[] objects = FindObjectsOfType<DontDestroyOnLoad>();

        foreach (DontDestroyOnLoad obj in objects)
        {
            if (obj != this && obj.gameObject.name == gameObject.name)
            {
                Destroy(obj.gameObject);
            }
        }
    }
}
