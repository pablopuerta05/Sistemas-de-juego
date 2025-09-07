using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSelector : MonoBehaviour
{
    public static CharacterSelector instance;
    public CharacterData characterData;

    private void Awake()
    {
        if (characterData == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Debug.LogWarning("There is more than one instance of this object");
            Destroy(gameObject);
        }
    }

    public static CharacterData GetData()
    {
        if (instance && instance.characterData)
        {
            return instance.characterData;
        }
        else
        {
            // if no character data is assigned, we randomly pick one
            CharacterData[] characters = Resources.FindObjectsOfTypeAll<CharacterData>();
            if (characters.Length > 0)
            {
                return characters[Random.Range(0, characters.Length)];
            }
        }

        return null;
    }

    public void SelectCharacter(CharacterData character)
    {
        characterData = character;
    }

    public void DestroySingleton()
    {
        instance = null;
        Destroy(gameObject);
    }
}
