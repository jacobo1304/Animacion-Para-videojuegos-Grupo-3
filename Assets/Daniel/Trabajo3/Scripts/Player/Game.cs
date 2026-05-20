using UnityEditor;
using UnityEngine;

public class Game : MonoBehaviour
{

    private static Game _instance;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void CreateGame()
    {
        GameObject gameOb = new GameObject(name: "[Game]");
        _instance = gameOb.AddComponent<Game>();
        DontDestroyOnLoad(gameOb);
    }

    public static Game Instance
    {
        get 
        { 
            if (_instance == null)
            {
                CreateGame();
            }
            return _instance; 
        }
    }


    private CharacterState playerOne;
    public CharacterState PlayerOne => playerOne;

    private void Awake()
    {
        CreatePlayer();
    }

    private void CreatePlayer()
    {
        GameObject PlayerGo = new GameObject(name: "[PlayerOne]");
        playerOne = PlayerGo.AddComponent<CharacterState>();
        DontDestroyOnLoad(PlayerGo);
    }

}